using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Transacciones.Queries;

namespace Application.UseCases.Transacciones.Handlers;

/*
 * Handler responsable de consultar el historial contable paginado de una billetera.
 * Recupera los movimientos desde ITransaccionLedgerRepository delegando paginación a SQL Server
 * y encapsula la respuesta en ResultadoPaginadoDTO.
 */
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

        // 1. Consulta paginada y conteo total en SQL Server
        var (transacciones, totalItems) = await _transaccionLedgerRepository.GetByBilleteraIdAsync(
            billetera.Id,
            request.Pagina,
            request.TamanoPagina,
            cancellationToken
        );

        // 2. Proyección de entidades contables a DTO
        var itemsDto = transacciones.Select(t => new TransaccionLedgerDTO(
            t.Id,
            t.BilleteraId,
            t.Tipo,
            t.Monto,
            t.Fecha,
            t.SubastaId
        )).ToList();

        // 3. Cálculo de páginas totales
        var totalPaginas = (int)Math.Ceiling((double)totalItems / request.TamanoPagina);

        // 4. Retorno enriquecido con metadatos de navegación
        return new ResultadoPaginadoDTO<TransaccionLedgerDTO>(
            Items: itemsDto,
            TotalItems: totalItems,
            Pagina: request.Pagina,
            TamanoPagina: request.TamanoPagina,
            TotalPaginas: totalPaginas
        );
    }
}