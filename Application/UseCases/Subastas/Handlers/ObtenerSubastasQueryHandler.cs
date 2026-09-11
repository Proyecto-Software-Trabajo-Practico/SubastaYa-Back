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
 Handler responsable de atender la consulta del catálogo de subastas.
 Invoca al repositorio con los filtros correspondientes y proyecta 
 las entidades resultantes a SubastaCardDTO.
*/
public class ObtenerSubastasQueryHandler : IRequestHandler<ObtenerSubastasQuery, List<SubastaCardDTO>>
{
    private readonly ISubastaRepository _subastaRepository;

    public ObtenerSubastasQueryHandler(ISubastaRepository subastaRepository)
    {
        _subastaRepository = subastaRepository;
    }

    public async Task<List<SubastaCardDTO>> HandleAsync(ObtenerSubastasQuery request, CancellationToken cancellationToken = default)
    {
        // 1. Obtener subastas filtradas y ordenadas desde el repositorio (Infrastructure)
        var subastas = await _subastaRepository.GetFiltradasAsync(
            request.Estado,
            request.CategoriaId,
            request.Orden,
            cancellationToken
        );

        // 2. Proyectar entidades de dominio a SubastaCardDTO para la vista
        return subastas.Select(s => new SubastaCardDTO(
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
    }
}