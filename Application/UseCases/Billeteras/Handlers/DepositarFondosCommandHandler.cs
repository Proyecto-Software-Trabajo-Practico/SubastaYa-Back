using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Billeteras.Commands;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Billeteras.Handlers;

/*
 * Handler responsable de acreditar fondos simulados en la billetera de un usuario.
 * Modifica el saldo en Dominio, genera el asiento contable inmutable en el Ledger,
 * registra el evento obligatorio en Auditoría y confirma todo atómicamente con IUnitOfWork.
 */
public class DepositarFondosCommandHandler : IRequestHandler<DepositarFondosCommand, BilleteraSaldosDto>
{
    private readonly IBilleteraRepository _billeteraRepository;
    private readonly ITransaccionLedgerRepository _transaccionLedgerRepository;
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DepositarFondosCommandHandler(
        IBilleteraRepository billeteraRepository,
        ITransaccionLedgerRepository transaccionLedgerRepository,
        IAuditoriaRepository auditoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _billeteraRepository = billeteraRepository;
        _transaccionLedgerRepository = transaccionLedgerRepository;
        _auditoriaRepository = auditoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BilleteraSaldosDto> HandleAsync(DepositarFondosCommand request, CancellationToken cancellationToken = default)
    {
        // 1. Obtener la billetera del usuario destinatario
        var billetera = await _billeteraRepository.GetByUsuarioIdAsync(request.UsuarioId);

        if (billetera is null)
            throw new DomainException($"No se encontró una billetera asociada al usuario con ID {request.UsuarioId}.");

        // 2. Modificar el estado en la entidad de dominio (aplica invariantes de negocio)
        billetera.Depositar(request.Monto);
        _billeteraRepository.Update(billetera);

        // 3. Crear asiento contable en el Ledger
        var transaccion = new TransaccionLedger(billetera.Id, "DEPOSITO", request.Monto);
        await _transaccionLedgerRepository.AddAsync(transaccion);

        // 4. Registrar evento inmutable de Auditoría exigido por el enunciado (Sección 2.4)
        var detalleAuditoria = JsonSerializer.Serialize(new
        {
            MontoAcreditado = request.Monto,
            SaldoTotalPosterior = billetera.SaldoTotal,
            SaldoDisponiblePosterior = billetera.SaldoDisponible,
            FechaOperacion = DateTime.UtcNow
        });

        var logAuditoria = new Auditoria(
            entidad: "BILLETERA",
            entidadId: billetera.Id,
            accion: "DEPOSITO_SALDO",
            usuarioId: request.UsuarioId,
            detalleJson: detalleAuditoria
        );

        await _auditoriaRepository.AddAsync(logAuditoria);

        // 5. Confirmar todas las operaciones en una única transacción atómica (ACID)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Proyectar y retornar los saldos actualizados al DTO
        return new BilleteraSaldosDto(
            billetera.Id,
            billetera.UsuarioId,
            billetera.SaldoTotal,
            billetera.SaldoRetenido,
            billetera.SaldoDisponible
        );
    }
}