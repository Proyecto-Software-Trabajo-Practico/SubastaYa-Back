using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Billeteras.Queries;

namespace Application.UseCases.Billeteras.Handlers;


public class ObtenerSaldosQueryHandler : IRequestHandler<ObtenerSaldosQuery, BilleteraSaldosDto?>
{
    private readonly IBilleteraRepository _billeteraRepository;

    public ObtenerSaldosQueryHandler(IBilleteraRepository billeteraRepository)
    {
        _billeteraRepository = billeteraRepository;
    }

    public async Task<BilleteraSaldosDto?> HandleAsync(ObtenerSaldosQuery request, CancellationToken cancellationToken = default)
    {
        var billetera = await _billeteraRepository.GetByUsuarioIdAsync(request.UsuarioId);

        if (billetera is null)
            return null;

        return new BilleteraSaldosDto(
            billetera.Id,
            billetera.UsuarioId,
            billetera.SaldoTotal,
            billetera.SaldoRetenido,
            billetera.SaldoDisponible
        );
    }
}