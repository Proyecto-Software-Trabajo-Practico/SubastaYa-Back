using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Billeteras.Commands;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Billeteras.Handlers;


public class DepositarFondosCommandHandler : IRequestHandler<DepositarFondosCommand, BilleteraSaldosDto>
{
    private readonly IBilleteraRepository _billeteraRepository;
    private readonly ITransaccionLedgerRepository _transaccionLedgerRepository;
    private readonly IUnitOfWork _unitOfWork;

    
    public DepositarFondosCommandHandler(
        IBilleteraRepository billeteraRepository,
        ITransaccionLedgerRepository transaccionLedgerRepository,
        IUnitOfWork unitOfWork)
    {
        _billeteraRepository = billeteraRepository;
        _transaccionLedgerRepository = transaccionLedgerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BilleteraSaldosDto> HandleAsync(DepositarFondosCommand request, CancellationToken cancellationToken = default)
    {
        var billetera = await _billeteraRepository.GetByUsuarioIdAsync(request.UsuarioId);

        if (billetera is null)
            throw new DomainException($"No se encontró una billetera asociada al usuario con ID {request.UsuarioId}.");

        billetera.Depositar(request.Monto);
        _billeteraRepository.Update(billetera);

        var transaccion = new TransaccionLedger(billetera.Id, "DEPOSITO", request.Monto);
        await _transaccionLedgerRepository.AddAsync(transaccion);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BilleteraSaldosDto(
            billetera.Id,
            billetera.UsuarioId,
            billetera.SaldoTotal,
            billetera.SaldoRetenido,
            billetera.SaldoDisponible
        );
    }
}