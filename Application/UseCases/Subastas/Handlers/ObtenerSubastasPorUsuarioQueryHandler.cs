using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;

namespace Application.UseCases.Subastas.Handlers;

/*
 * Handler responsable de atender la consulta paginada de publicaciones de un vendedor (Módulo 5).
 * Invoca al repositorio con paginación a nivel SQL Server y proyecta las entidades Subasta
 * calculando métricas de recaudación efectiva y estado de adjudicación para SubastaVendedorDTO.
 */
public class ObtenerSubastasPorUsuarioQueryHandler : IRequestHandler<ObtenerSubastasPorUsuarioQuery, ResultadoPaginadoDTO<SubastaVendedorDTO>>
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
        // 1. Obtener del repositorio las subastas del vendedor y el conteo total
        var (subastas, totalItems) = await _subastaRepository.GetByVendedorPaginadoAsync(
            request.VendedorId,
            request.Pagina,
            request.TamanoPagina,
            cancellationToken
        );

        // 2. Proyectar cada subasta calculando las métricas de negocio para el vendedor
        var itemsDto = subastas.Select(s =>
        {
            // Determinar la puja líder/ganadora (mayor monto)
            var pujaLider = s.Pujas.OrderByDescending(p => p.Monto).FirstOrDefault();

            // Precio actual o precio de cierre: la puja más alta o el precio base inicial
            var precioActualOFinal = pujaLider != null ? pujaLider.Monto : s.PrecioBase;

            // Métrica de recaudación: solo suma si la subasta finalizó efectivamente con ofertas
            var totalRecaudado = (s.Estado == "FINALIZADA" && pujaLider != null) ? pujaLider.Monto : 0m;

            // Estado de adjudicación para el panel del vendedor
            string estadoAdjudicacion = s.Estado switch
            {
                "FINALIZADA" => pujaLider != null ? "ADJUDICADA" : "DESIERTA",
                "DESIERTA" => "DESIERTA",
                "ACTIVA" => "EN CURSO",
                "PROGRAMADA" => "PROGRAMADA",
                "CANCELADA" => "CANCELADA",
                _ => s.Estado
            };

            // Nombre del postor ganador solo si la subasta finalizó adjudicada
            string? ganadorNombre = (s.Estado == "FINALIZADA" && pujaLider?.Comprador != null)
                ? pujaLider.Comprador.Nombre
                : null;

            return new SubastaVendedorDTO(
                Id: s.Id,
                Titulo: s.Titulo,
                UrlImagen: s.UrlImagen,
                Estado: s.Estado,
                PrecioBase: s.PrecioBase,
                PrecioActualOFinal: precioActualOFinal,
                TotalRecaudado: totalRecaudado,
                CantidadPujas: s.Pujas.Count,
                FechaInicio: s.FechaInicio,
                FechaFin: s.FechaFin,
                EstadoAdjudicacion: estadoAdjudicacion,
                CategoriaNombre: s.Categoria?.Nombre ?? "Sin categoría",
                GanadorNombre: ganadorNombre
            );
        }).ToList();

        // 3. Calcular total de páginas para navegación del Frontend
        var totalPaginas = (int)Math.Ceiling((double)totalItems / request.TamanoPagina);

        // 4. Retornar el contenedor enriquecido con datos y metadatos de paginación
        return new ResultadoPaginadoDTO<SubastaVendedorDTO>(
            Items: itemsDto,
            TotalItems: totalItems,
            Pagina: request.Pagina,
            TamanoPagina: request.TamanoPagina,
            TotalPaginas: totalPaginas
        );
    }
}
