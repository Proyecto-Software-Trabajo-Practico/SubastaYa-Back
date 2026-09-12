using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Commands;
using Domain.Entities;

namespace Application.UseCases.Subastas.Handlers;

/*
 * Handler responsable del cierre masivo y liquidación contable de subastas vencidas.
 * Identifica ofertas ganadoras o desiertas, ejecuta el traspaso de fondos
 * mediante Escrow, genera asientos contables en Ledger, registra auditoría
 * y consolida todo atómicamente con IUnitOfWork.
 */
public class ProcesarSubastasFinalizadasCommandHandler : IRequestHandler<ProcesarSubastasFinalizadasCommand, SubastasProcesadasDTO>
{
    private readonly ISubastaRepository _subastaRepository;
    private readonly IBilleteraRepository _billeteraRepository;
    private readonly ITransaccionLedgerRepository _transaccionLedgerRepository;
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcesarSubastasFinalizadasCommandHandler(
        ISubastaRepository subastaRepository,
        IBilleteraRepository billeteraRepository,
        ITransaccionLedgerRepository transaccionLedgerRepository,
        IAuditoriaRepository auditoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _subastaRepository = subastaRepository;
        _billeteraRepository = billeteraRepository;
        _transaccionLedgerRepository = transaccionLedgerRepository;
        _auditoriaRepository = auditoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SubastasProcesadasDTO> HandleAsync(ProcesarSubastasFinalizadasCommand request, CancellationToken cancellationToken = default)
    {
        // 1. Obtener todas las subastas activas cuya fecha de fin ya expiró
        var subastasVencidas = await _subastaRepository.GetSubastasVencidasParaCierreAsync();

        if (!subastasVencidas.Any())
        {
            return new SubastasProcesadasDTO(
                TotalProcesadas: 0,
                FinalizadasConGanador: 0,
                DeclaradasDesiertas: 0,
                Mensaje: "No se encontraron subastas vencidas pendientes de procesamiento."
            );
        }

        int finalizadasConGanador = 0;
        int declaradasDesiertas = 0;

        foreach (var subasta in subastasVencidas)
        {
            // Bifurcación de Negocio 1: Subasta sin postores
            if (!subasta.Pujas.Any())
            {
                subasta.MarcarDesierta();
                declaradasDesiertas++;
                continue;
            }

            // Bifurcación de Negocio 2: Subasta con postor ganador
            var pujaGanadora = subasta.Pujas.OrderByDescending(p => p.Monto).First();
            subasta.Finalizar();
            finalizadasConGanador++;

            // Liquidación contable del comprador ganador: debita definitivamente los fondos en custodia
            var billeteraComprador = await _billeteraRepository.GetByUsuarioIdAsync(pujaGanadora.CompradorId);
            if (billeteraComprador is not null)
            {
                billeteraComprador.DebitarSaldoRetenido(pujaGanadora.Monto);
                _billeteraRepository.Update(billeteraComprador);

                var txPago = new TransaccionLedger(billeteraComprador.Id, "PAGO", pujaGanadora.Monto, subasta.Id);
                await _transaccionLedgerRepository.AddAsync(txPago);
            }

            // Liquidación contable del vendedor: acredita el monto obtenido por la venta
            var billeteraVendedor = await _billeteraRepository.GetByUsuarioIdAsync(subasta.VendedorId);
            if (billeteraVendedor is not null)
            {
                billeteraVendedor.Depositar(pujaGanadora.Monto);
                _billeteraRepository.Update(billeteraVendedor);

                var txCobro = new TransaccionLedger(billeteraVendedor.Id, "COBRO", pujaGanadora.Monto, subasta.Id);
                await _transaccionLedgerRepository.AddAsync(txCobro);
            }

            // Registro inmutable de Auditoría para trazabilidad de la liquidación
            var detalleAuditoria = JsonSerializer.Serialize(new
            {
                SubastaId = subasta.Id,
                SubastaTitulo = subasta.Titulo,
                GanadorId = pujaGanadora.CompradorId,
                VendedorId = subasta.VendedorId,
                MontoFinal = pujaGanadora.Monto,
                FechaLiquidacion = DateTime.UtcNow
            });

            var logAuditoria = new Auditoria(
                entidad: "SUBASTA",
                entidadId: subasta.Id,
                accion: "LIQUIDACION_SUBASTA",
                usuarioId: subasta.VendedorId,
                detalleJson: detalleAuditoria
            );

            await _auditoriaRepository.AddAsync(logAuditoria);
        }

        // Confirmar todos los cambios en una única transacción atómica (ACID)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubastasProcesadasDTO(
            TotalProcesadas: subastasVencidas.Count,
            FinalizadasConGanador: finalizadasConGanador,
            DeclaradasDesiertas: declaradasDesiertas,
            Mensaje: $"Procesamiento completado exitosamente: {finalizadasConGanador} finalizada(s) con ganador y {declaradasDesiertas} declarada(s) desierta(s)."
        );
    }
}
