using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Transacciones.Queries;

namespace Application.UseCases.Transacciones.Handlers;


public class ObtenerHistorialTransaccionesQueryHandler : IRequestHandler<ObtenerHistorialTransaccionesQuery, List<TransaccionLedgerDTO>?>
{
    private readonly IBilleteraRepository _billeteraRepository;
    private readonly ITransaccionLedgerRepository _transaccionLedgerRepository;

    public ObtenerHistorialTransaccionesQueryHandler(
        IBilleteraRepository billeteraRepository,
        ITransaccionLedgerRepository transaccionLedgerRepository)
    {
        _billeteraRepository = billeteraRepository;
        _transaccionLedgerRepository = transaccionLedgerRepository;
    }

    public async Task<List<TransaccionLedgerDTO>?> HandleAsync(
        ObtenerHistorialTransaccionesQuery request,
        CancellationToken cancellationToken = default)
    {
       
        var billetera = await _billeteraRepository.GetByUsuarioIdAsync(request.UsuarioId);

        if (billetera is null)
            return null;

        var transacciones = await _transaccionLedgerRepository.GetByBilleteraIdAsync(billetera.Id);

        return transacciones.Select(t => new TransaccionLedgerDTO(
            t.Id,
            t.BilleteraId,
            t.Tipo,
            t.Monto,
            t.Fecha,
            t.SubastaId
        )).ToList();
    }
}