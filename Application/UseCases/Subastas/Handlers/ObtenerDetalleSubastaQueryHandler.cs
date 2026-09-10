using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;

namespace Application.UseCases.Subastas.Handlers;

/// Manejador CQRS encargado de obtener los detalles de una subasta y proyectarlos al DTO de la sala.
public class ObtenerDetalleSubastaQueryHandler : IRequestHandler<ObtenerDetalleSubastaQuery, SubastaDetalleDTO?>
{
    private readonly ISubastaRepository _subastaRepository;

    public ObtenerDetalleSubastaQueryHandler(ISubastaRepository subastaRepository)
    {
        _subastaRepository = subastaRepository;
    }

    public async Task<SubastaDetalleDTO?> HandleAsync(
        ObtenerDetalleSubastaQuery request,
        CancellationToken cancellationToken = default)
    {
        // 1. Buscamos la subasta con sus relaciones (categoría, vendedor y pujas)
        var subasta = await _subastaRepository.GetDetalleByIdAsync(request.Id);

        if (subasta is null)
            return null;

        // 2. Calculamos el precio actual: la puja más alta o el precio base inicial si no hay pujas
        var pujaLider = subasta.Pujas.OrderByDescending(p => p.Monto).FirstOrDefault();

        decimal precioActual;
        if (pujaLider is not null){
            precioActual = pujaLider.Monto;
        }else { precioActual = subasta.PrecioBase;}

        // 3. Proyectamos las pujas ordenadas de la más reciente a la más antigua
        var ultimasPujasDto = subasta.Pujas
            .OrderByDescending(p => p.FechaPuja)
            .Select(p => new PujaDTO(
                p.Id,
                p.Monto,
                p.FechaPuja,
                p.CompradorId,
                p.Comprador?.Nombre ?? "Anónimo"
            ))
            .ToList();

        // 4. Proyectamos y retornamos el DTO de detalle completo
        return new SubastaDetalleDTO(
            subasta.Id,
            subasta.Titulo,
            subasta.Descripcion,
            subasta.UrlImagen,
            subasta.Estado,
            subasta.PrecioBase,
            subasta.IncrementoMinimo,
            precioActual,
            subasta.Pujas.Count,
            subasta.FechaInicio,
            subasta.FechaFin,
            subasta.CategoriaId,
            subasta.Categoria?.Nombre ?? string.Empty,
            subasta.VendedorId,
            subasta.Vendedor?.Nombre ?? string.Empty,
            ultimasPujasDto
        );
    }
}