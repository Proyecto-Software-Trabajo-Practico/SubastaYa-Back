using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Transacciones.Queries;

namespace Application.UseCases.Transacciones.Handlers;


public class ObtenerHistorialTransaccionesQueryHandler : IRequestHandler<ObtenerHistorialTransaccionesQuery, ResultadoPaginadoDTO<TransaccionLedgerDTO>?>
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

    public async Task<ResultadoPaginadoDTO<TransaccionLedgerDTO>?> HandleAsync(
        ObtenerHistorialTransaccionesQuery request,
        CancellationToken cancellationToken = default)
    {
        var billetera = await _billeteraRepository.GetByUsuarioIdAsync(request.UsuarioId);

        if (billetera is null)
            return null;

        var (transacciones, totalItems) = await _transaccionLedgerRepository.GetByBilleteraIdAsync(
            billetera.Id,
            request.Pagina,
            request.TamanoPagina,
            cancellationToken
        );

        var itemsDto = transacciones.Select(t => new TransaccionLedgerDTO(
            t.Id,
            t.BilleteraId,
            t.Tipo,
            t.Monto,
            t.Fecha,
            t.SubastaId
        )).ToList();

        var totalPaginas = (int)Math.Ceiling((double)totalItems / request.TamanoPagina);

        return new ResultadoPaginadoDTO<TransaccionLedgerDTO>(
            Items: itemsDto,
            TotalItems: totalItems,
            Pagina: request.Pagina,
            TamanoPagina: request.TamanoPagina,
            TotalPaginas: totalPaginas
        );
    }
}