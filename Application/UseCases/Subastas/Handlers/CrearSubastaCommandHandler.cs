using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Interfaces;
using Application.UseCases.Subastas.Commands;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCases.Subastas.Handlers;

/*
 Handler responsable de orquestar la creación y persistencia de una subasta.
 Verifica dependencias de negocio, delega invariantes a la entidad Subasta
 y asegura la confirmación transaccional con IUnitOfWork.
*/
public class CrearSubastaCommandHandler : IRequestHandler<CrearSubastaCommand, int>
{
    private readonly ISubastaRepository _subastaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearSubastaCommandHandler(
        ISubastaRepository subastaRepository,
        IUsuarioRepository usuarioRepository,
        ICategoriaRepository categoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _subastaRepository = subastaRepository;
        _usuarioRepository = usuarioRepository;
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> HandleAsync(CrearSubastaCommand request, CancellationToken cancellationToken = default)
    {
        // 1. Validar existencia del vendedor
        var vendedor = await _usuarioRepository.GetByIdAsync(request.VendedorId);
        if (vendedor is null)
            throw new DomainException($"No se encontró un vendedor registrado con el ID {request.VendedorId}.");

        // 2. Validar existencia de la categoría
        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId);
        if (categoria is null)
            throw new DomainException($"No se encontró una categoría registrada con el ID {request.CategoriaId}.");

        // 3. Crear entidad de Dominio (ejecuta validaciones invariantes de fechas, precios y asigna estado)
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

        // 4. Agregar al repositorio y confirmar de forma atómica
        await _subastaRepository.AddAsync(subasta);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Retornar el ID autogenerado por la base de datos
        return subasta.Id;
    }
}
