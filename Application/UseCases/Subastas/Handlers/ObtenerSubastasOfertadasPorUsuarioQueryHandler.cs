using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;

namespace Application.UseCases.Subastas.Handlers;

public class ObtenerSubastasOfertadasPorUsuarioQueryHandler
    : IRequestHandler<ObtenerSubastasOfertadasPorUsuarioQuery, ResultadoPaginadoDTO<SubastaCardDTO>>
{
    private readonly ISubastaRepository _subastaRepository;

    public ObtenerSubastasOfertadasPorUsuarioQueryHandler(ISubastaRepository subastaRepository)
    {
        _subastaRepository = subastaRepository;
    }

    public async Task<ResultadoPaginadoDTO<SubastaCardDTO>> HandleAsync(
        ObtenerSubastasOfertadasPorUsuarioQuery request,
        CancellationToken cancellationToken = default)
    {
        var (subastas, totalItems) = await _subastaRepository.GetOfertadasByCompradorPaginadoAsync(
            request.CompradorId,
            request.Pagina,
            request.TamanoPagina,
            cancellationToken
        );

        var itemsDto = subastas.Select(s =>
        {
            var cantidadPujas = s.Pujas != null ? s.Pujas.Count : 0;

            var mayorPuja = (s.Pujas != null && s.Pujas.Any())
                ? s.Pujas.OrderByDescending(p => p.Monto).FirstOrDefault()
                : null;

            var precioActual = mayorPuja != null ? mayorPuja.Monto : s.PrecioBase;
            int? compradorGanadorId = mayorPuja?.CompradorId;

            return new SubastaCardDTO(
                s.Id,
                s.Titulo,
                s.UrlImagen,
                s.Estado,
                s.PrecioBase,
                precioActual,
                cantidadPujas,
                s.FechaFin,
                s.CategoriaId,
                s.Categoria?.Nombre ?? "General",
                compradorGanadorId 
            );
        }).ToList();

        var totalPaginas = (int)Math.Ceiling((double)totalItems / request.TamanoPagina);

        return new ResultadoPaginadoDTO<SubastaCardDTO>(
            Items: itemsDto,
            TotalItems: totalItems,
            Pagina: request.Pagina,
            TamanoPagina: request.TamanoPagina,
            TotalPaginas: totalPaginas
        );
    }
}