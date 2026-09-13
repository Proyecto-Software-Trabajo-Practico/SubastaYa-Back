using Application.DTOs;
using Application.Hubs;
using Application.Interfaces;
using Application.UseCases.Pujas.Commands;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Application.UseCases.Pujas.Handlers;

/// <summary>
/// Handler de Aplicación (CQRS): Orquesta la creación de una puja, la retención de saldos,
/// la extensión de tiempo por anti-sniping, los registros contables y las notificaciones en tiempo real.
/// </summary>
public class CrearPujaCommandHandler : IRequestHandler<CrearPujaCommand, PujaDTO>
{
    // Repositorios e interfaces de infraestructura inyectados mediante Inyección de Dependencias (DI)
    private readonly ISubastaRepository _subastaRepository;
    private readonly IPujaRepository _pujaRepository;
    private readonly IBilleteraRepository _billeteraRepository;
    private readonly ITransaccionLedgerRepository _ledgerRepository;
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<SubastaHub> _hubContext;

    public CrearPujaCommandHandler(
        ISubastaRepository subastaRepository,
        IPujaRepository pujaRepository,
        IBilleteraRepository billeteraRepository,
        ITransaccionLedgerRepository ledgerRepository,
        IAuditoriaRepository auditoriaRepository,
        IUnitOfWork unitOfWork,
        IHubContext<SubastaHub> hubContext)
    {
        _subastaRepository = subastaRepository;
        _pujaRepository = pujaRepository;
        _billeteraRepository = billeteraRepository;
        _ledgerRepository = ledgerRepository;
        _auditoriaRepository = auditoriaRepository;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
    }

    public async Task<PujaDTO> HandleAsync(CrearPujaCommand request, CancellationToken cancellationToken = default)
    {
        // ==========================================
        // ETAPA 1: VALIDACIONES PREVIAS (FAIL-FAST)
        // ==========================================

        // 1.1 Recuperamos la subasta correspondiente desde la BD
        var subasta = await _subastaRepository.GetByIdAsync(request.SubastaId);
        if (subasta == null)
            throw new KeyNotFoundException($"La subasta con ID {request.SubastaId} no existe."); // Retorna 404 mediante Middleware

        // 1.2 Regla de negocio: La subasta debe estar en estado 'ACTIVA'
        if (subasta.Estado != "ACTIVA")
            throw new DomainException("No se pueden realizar pujas en subastas que no estén ACTIVAS."); // Retorna 400/422

        // 1.3 Regla temporal: Si la fecha actual superó la FechaFin, no se aceptan más ofertas
        if (DateTime.UtcNow >= subasta.FechaFin)
            throw new DomainException("La subasta ya ha finalizado.");

        // 1.4 Regla de Fair Play: El creador/vendedor no puede autopujarse
        if (subasta.VendedorId == request.CompradorId)
            throw new DomainException("El vendedor no puede realizar pujas en su propia subasta.");


        // ==========================================
        // ETAPA 2: REGLAS DE LA OFERTA Y LIDERAZGO
        // ==========================================

        // 2.1 Obtenemos la puja líder actual para calcular montos y validar postor
        var pujaLiderAnterior = await _pujaRepository.GetPujaMasAltaBySubastaIdAsync(request.SubastaId);

        // 2.2 Impedimos que el postor líder vuelva a ofertar sobre su propia puja si ya va ganando
        if (pujaLiderAnterior != null && pujaLiderAnterior.CompradorId == request.CompradorId)
            throw new DomainException("Ya eres el postor líder de esta subasta.");

        // 2.3 Calculamos el valor mínimo a superar:
        // Si hay puja previa -> PujaAnterior + IncrementoMinimo
        // Si es la primera puja -> PrecioBase de la subasta
        decimal montoMinimoRequerido = pujaLiderAnterior != null
            ? pujaLiderAnterior.Monto + subasta.IncrementoMinimo
            : subasta.PrecioBase;

        // 2.4 Revalidamos que el monto ofertado cumpla el piso exigido
        if (request.Monto < montoMinimoRequerido)
            throw new DomainException($"El monto de la puja debe ser de al menos ${montoMinimoRequerido:N2}.");


        // ==========================================
        // ETAPA 3: MANEJO ATÓMICO DE SALDOS (ESCROW)
        // ==========================================

        // 3.1 LIBERACIÓN DE FONDOS (Líder Anterior)
        if (pujaLiderAnterior != null)
        {
            var billeteraLiderAnterior = await _billeteraRepository.GetByUsuarioIdAsync(pujaLiderAnterior.CompradorId);
            if (billeteraLiderAnterior != null)
            {
                // Invoca método de dominio en la entidad Billetera: pasa dinero de SaldoRetenido a SaldoDisponible
                billeteraLiderAnterior.LiberarSaldo(pujaLiderAnterior.Monto);

                // Registra el movimiento contable inmutable en la entidad TransaccionLedger (Requisito TP 2.1)
                var ledgerLiberacion = new TransaccionLedger(
                    billeteraLiderAnterior.Id,
                    "LIBERACION",
                    pujaLiderAnterior.Monto,
                    subasta.Id
                );
                await _ledgerRepository.AddAsync(ledgerLiberacion);

                // Auditoría de infraestructura para trazabilidad de billeteras
                var detalleLiberacion = JsonSerializer.Serialize(new
                {
                    SubastaId = subasta.Id,
                    MontoLiberado = pujaLiderAnterior.Monto
                });

                await _auditoriaRepository.AddAsync(new Auditoria(
                    entidad: "BILLETERA",
                    entidadId: billeteraLiderAnterior.Id,
                    accion: "LIBERACION_FONDOS",
                    usuarioId: pujaLiderAnterior.CompradorId,
                    detalleJson: detalleLiberacion
                ));
            }
        }

        // 3.2 RETENCIÓN DE FONDOS (Nuevo Líder)
        var billeteraNuevoPostor = await _billeteraRepository.GetByUsuarioIdAsync(request.CompradorId);
        if (billeteraNuevoPostor == null)
            throw new KeyNotFoundException("El comprador no posee una billetera registrada.");

        // Método de dominio: Pasa dinero de SaldoDisponible a SaldoRetenido.
        // Si SaldoDisponible < Monto, la entidad Billetera arroja una DomainException automáticamente.
        billeteraNuevoPostor.RetenerSaldo(request.Monto);

        // Registra el movimiento contable de congelamiento en el Ledger (Requisito TP 2.1)
        var ledgerRetencion = new TransaccionLedger(
            billeteraNuevoPostor.Id,
            "RETENCION",
            request.Monto,
            subasta.Id
        );
        await _ledgerRepository.AddAsync(ledgerRetencion);

        // Auditoría de la retención de fondos
        var detalleRetencion = JsonSerializer.Serialize(new
        {
            SubastaId = subasta.Id,
            MontoRetenido = request.Monto
        });

        await _auditoriaRepository.AddAsync(new Auditoria(
            entidad: "BILLETERA",
            entidadId: billeteraNuevoPostor.Id,
            accion: "RETENCION_FONDOS",
            usuarioId: request.CompradorId,
            detalleJson: detalleRetencion
        ));


        // ==========================================
        // ETAPA 4: REGLA ANTI-SNIPING (DOMINIO)
        // ==========================================

        // Delega a la entidad Subasta la evaluación: si resta menos de 60 segundos,
        // suma 2 minutos adicionales a FechaFin y retorna true.
        bool huboAntiSniping = subasta.EvaluarExtensionAntiSniping();
        if (huboAntiSniping)
        {
            var detalleAntiSniping = JsonSerializer.Serialize(new
            {
                SubastaId = subasta.Id,
                NuevaFechaFin = subasta.FechaFin
            });

            // Auditoría obligatoria exigida por el TP para la prórroga de tiempo
            await _auditoriaRepository.AddAsync(new Auditoria(
                entidad: "SUBASTA",
                entidadId: subasta.Id,
                accion: "EXTENSION_ANTI_SNIPING",
                usuarioId: request.CompradorId,
                detalleJson: detalleAntiSniping
            ));
        }


        // ==========================================
        // ETAPA 5 Y 6: INSTANCIACIÓN Y AUDITORÍA
        // ==========================================

        // Instanciamos el registro de la nueva Puja
        var nuevaPuja = new Puja(
            subastaId: request.SubastaId,
            compradorId: request.CompradorId,
            monto: request.Monto
        );
        await _pujaRepository.AddAsync(nuevaPuja);

        // Auditoría obligatoria de la oferta aceptada
        var detallePuja = JsonSerializer.Serialize(new
        {
            SubastaId = subasta.Id,
            Monto = request.Monto,
            FechaPuja = nuevaPuja.FechaPuja
        });

        await _auditoriaRepository.AddAsync(new Auditoria(
            entidad: "PUJA",
            entidadId: subasta.Id,
            accion: "OFERTA_REGISTRADA",
            usuarioId: request.CompradorId,
            detalleJson: detallePuja
        ));


        // ==========================================
        // ETAPA 7: PERSISTENCIA ATÓMICA Y CONCURRENCIA
        // ==========================================

        // Persiste TODOS los cambios de la transacción (Subasta, Billeteras, Ledger, Puja, Auditorías).
        // Si dos peticiones compiten en el mismo milisegundo, EF Core detecta la diferencia en el 'RowVersion'
        // (Optimistic Locking) y dispara DbUpdateConcurrencyException, que el Middleware mapea a HTTP 409 Conflict.
        await _unitOfWork.SaveChangesAsync(cancellationToken);


        // ==========================================
        // ETAPA 8: NOTIFICACIÓN TIEMPO REAL (SIGNALR)
        // ==========================================

        // Anonimizamos el seudónimo del usuario para cumplir con la regla de UX (Módulo 3)
        string postorAnonimizado = $"Usuario_{request.CompradorId}";

        // Envolvemos el envío por WebSockets en un try-catch táctico de resiliencia:
        // Si la red del cliente falla, NO debemos fallar la petición HTTP 200, porque
        // la transacción en la base de datos ya fue completada exitosamente.
        try
        {
            await _hubContext.Clients.Group($"subasta-{subasta.Id}").SendAsync("NuevaPujaRecibida", new
            {
                SubastaId = subasta.Id,
                Monto = nuevaPuja.Monto,
                Postor = postorAnonimizado,
                NuevaFechaFin = subasta.FechaFin,
                SeAplicoAntiSniping = huboAntiSniping
            }, cancellationToken);
        }
        catch (Exception)
        {
            // Log de infraestructura: Se omite la interrupción para asegurar la respuesta del Handler.
        }

        // Devolvemos el DTO con el resultado exitoso
        return new PujaDTO(
            nuevaPuja.Id,
            nuevaPuja.SubastaId,
            nuevaPuja.CompradorId,
            nuevaPuja.Monto,
            nuevaPuja.FechaPuja
        );
    }
}