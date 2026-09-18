using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;

namespace Application.UseCases.Subastas.Handlers;

public class ObtenerSubastasPorUsuarioQueryHandler
    : IRequestHandler<ObtenerSubastasPorUsuarioQuery, ResultadoPaginadoDTO<SubastaVendedorDTO>>
{
    private readonly ISubastaRepository _subastaRepository;

    public ObtenerSubastasPorUsuarioQueryHandler(ISubastaRepository subastaRepository)
    {
        _subastaRepository = subastaRepository;
    }

    public async Task<ResultadoPaginadoDTO<SubastaVendedorDTO>> HandleAsync(
        ObtenerSubastasPorUsuarioQuery request,
        CancellationToken cancellationToken = default)
    {
        var (subastas, totalItems) = await _subastaRepository.GetByVendedorPaginadoAsync(
            request.VendedorId,
            request.Pagina,
            request.TamanoPagina,
            cancellationToken
        );

        var itemsDto = subastas.Select(s =>
        {
            var estaActiva = s.Estado == "ACTIVA" && s.FechaFin > DateTime.UtcNow;
            var cantidadPujas = s.Pujas != null ? s.Pujas.Count : 0;

            string estadoAdjudicacion = estaActiva ? "ACTIVA" : (cantidadPujas > 0 ? "ADJUDICADA" : "DESIERTA");

            var precioActual = (s.Pujas != null && s.Pujas.Any())
                ? s.Pujas.Max(p => p.Monto)
                : s.PrecioBase;

            string? ganadorNombre = null;

            return new SubastaVendedorDTO(
                s.Id,
                s.Titulo,
                s.UrlImagen, 
                s.Estado,
                s.PrecioBase,
                precioActual,
                s.IncrementoMinimo,
                cantidadPujas,
                s.FechaInicio,
                s.FechaFin,
                estadoAdjudicacion,
                s.Categoria?.Nombre ?? "General",
                ganadorNombre
            );
        }).ToList();

        var totalPaginas = (int)Math.Ceiling((double)totalItems / request.TamanoPagina);

        return new ResultadoPaginadoDTO<SubastaVendedorDTO>(
            Items: itemsDto,
            TotalItems: totalItems,
            Pagina: request.Pagina,
            TamanoPagina: request.TamanoPagina,
            TotalPaginas: totalPaginas
        );
    }
}