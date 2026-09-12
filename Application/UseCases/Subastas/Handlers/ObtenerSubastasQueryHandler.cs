using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;

namespace Application.UseCases.Subastas.Handlers;

/*
 * Handler responsable de atender la consulta paginada del catálogo de subastas.
 * Invoca al repositorio con filtros y paginación a nivel SQL Server, y proyecta 
 * las entidades resultantes a SubastaCardDTO encapsuladas en ResultadoPaginadoDTO.
 */
public class ObtenerSubastasQueryHandler : IRequestHandler<ObtenerSubastasQuery, ResultadoPaginadoDTO<SubastaCardDTO>>
{
    private readonly ISubastaRepository _subastaRepository;

    public ObtenerSubastasQueryHandler(ISubastaRepository subastaRepository)
    {
        _subastaRepository = subastaRepository;
    }

    public async Task<ResultadoPaginadoDTO<SubastaCardDTO>> HandleAsync(ObtenerSubastasQuery request, CancellationToken cancellationToken = default)
    {
        // 1. Obtener subastas paginadas y conteo total desde el repositorio (Infrastructure)
        var (subastas, totalItems) = await _subastaRepository.GetFiltradasAsync(
            request.Estado,
            request.CategoriaId,
            request.Orden,
            request.Pagina,
            request.TamanoPagina,
            cancellationToken
        );

        // 2. Proyectar entidades de dominio a SubastaCardDTO para la vista
        var itemsDto = subastas.Select(s => new SubastaCardDTO(
            s.Id,
            s.Titulo,
            s.UrlImagen,
            s.Estado,
            s.PrecioBase,
            // Precio actual: la mayor puja registrada, o el precio base si no tiene ofertas
            s.Pujas.Any() ? s.Pujas.Max(p => p.Monto) : s.PrecioBase,
            s.Pujas.Count,
            s.FechaFin,
            s.CategoriaId,
            s.Categoria?.Nombre ?? "Sin categoría"
        )).ToList();

        // 3. Calcular el total de páginas necesarias
        var totalPaginas = (int)Math.Ceiling((double)totalItems / request.TamanoPagina);

        // 4. Retornar el contenedor enriquecido con datos y metadatos de navegación
        return new ResultadoPaginadoDTO<SubastaCardDTO>(
            Items: itemsDto,
            TotalItems: totalItems,
            Pagina: request.Pagina,
            TamanoPagina: request.TamanoPagina,
            TotalPaginas: totalPaginas
        );
    }
}