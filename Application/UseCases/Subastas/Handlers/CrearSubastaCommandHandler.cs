using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Application.Interfaces;
using Application.UseCases.Subastas.Commands;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Subastas.Handlers;


public class CrearSubastaCommandHandler : IRequestHandler<CrearSubastaCommand, int>
{
    private readonly ISubastaRepository _subastaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearSubastaCommandHandler(
        ISubastaRepository subastaRepository,
        IUsuarioRepository usuarioRepository,
        ICategoriaRepository categoriaRepository,
        IAuditoriaRepository auditoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _subastaRepository = subastaRepository;
        _usuarioRepository = usuarioRepository;
        _categoriaRepository = categoriaRepository;
        _auditoriaRepository = auditoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> HandleAsync(CrearSubastaCommand request, CancellationToken cancellationToken = default)
    {
        var vendedor = await _usuarioRepository.GetByIdAsync(request.VendedorId);
        if (vendedor is null)
            throw new DomainException($"No se encontró un vendedor registrado con el ID {request.VendedorId}.");

        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId);
        if (categoria is null)
            throw new DomainException($"No se encontró una categoría registrada con el ID {request.CategoriaId}.");

        var subasta = new Subasta(
            request.VendedorId,
            request.CategoriaId,
            request.Titulo,
            request.Descripcion,
            request.UrlImagen,
            request.PrecioBase,
            request.IncrementoMinimo,
            request.FechaInicio,
            request.FechaFin
        );

        await _subastaRepository.AddAsync(subasta);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        
        var detalleAuditoria = JsonSerializer.Serialize(new
        {
            subasta.Titulo,
            subasta.PrecioBase,
            subasta.IncrementoMinimo,
            subasta.FechaInicio,
            subasta.FechaFin
        });

        var logAuditoria = new Auditoria(
            entidad: "SUBASTA",
            entidadId: subasta.Id,
            accion: "CREACION_SUBASTA",
            usuarioId: subasta.VendedorId,
            detalleJson: detalleAuditoria
        );

        await _auditoriaRepository.AddAsync(logAuditoria);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        
        return subasta.Id;
    }
}
