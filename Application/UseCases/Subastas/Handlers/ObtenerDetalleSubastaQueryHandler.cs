using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;

namespace Application.UseCases.Subastas.Handlers;

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
        var subasta = await _subastaRepository.GetDetalleByIdAsync(request.Id);

        if (subasta is null)
            return null;

        var pujaLider = subasta.Pujas.OrderByDescending(p => p.Monto).FirstOrDefault();

        decimal precioActual;
        if (pujaLider is not null){
            precioActual = pujaLider.Monto;
        }else { precioActual = subasta.PrecioBase;}

        var ultimasPujasDto = subasta.Pujas
        .OrderByDescending(p => p.FechaPuja)
        .Select(p => new PujaDTO(
        Id: p.Id,
        SubastaId: p.SubastaId,
        CompradorId: p.CompradorId,
        Monto: p.Monto,
        FechaPuja: p.FechaPuja
        ))
        .ToList();

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