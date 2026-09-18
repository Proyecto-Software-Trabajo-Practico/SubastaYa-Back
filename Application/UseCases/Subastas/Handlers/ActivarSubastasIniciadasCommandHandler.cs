using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.UseCases.Subastas.Commands;
using Domain.Entities;

namespace Application.UseCases.Subastas.Handlers;


public class ActivarSubastasIniciadasCommandHandler : IRequestHandler<ActivarSubastasIniciadasCommand, int>
{
    private readonly ISubastaRepository _subastaRepository;
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivarSubastasIniciadasCommandHandler(
        ISubastaRepository subastaRepository,
        IAuditoriaRepository auditoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _subastaRepository = subastaRepository;
        _auditoriaRepository = auditoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> HandleAsync(ActivarSubastasIniciadasCommand request, CancellationToken cancellationToken = default)
    {
        
        var subastasParaIniciar = await _subastaRepository.GetSubastasProgramadasParaInicioAsync();

        if (!subastasParaIniciar.Any())
        {
            return 0;
        }

        int totalActivadas = 0;

        foreach (var subasta in subastasParaIniciar)
        {
            
            subasta.Activar();
            _subastaRepository.Update(subasta);
            totalActivadas++;

            
            var detalleAuditoria = JsonSerializer.Serialize(new
            {
                SubastaId = subasta.Id,
                SubastaTitulo = subasta.Titulo,
                FechaInicio = subasta.FechaInicio,
                FechaActivacion = DateTime.UtcNow
            });

            var logAuditoria = new Auditoria(
                entidad: "SUBASTA",
                entidadId: subasta.Id,
                accion: "ACTIVACION_SUBASTA",
                usuarioId: subasta.VendedorId,
                detalleJson: detalleAuditoria
            );

            await _auditoriaRepository.AddAsync(logAuditoria);
        }

        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return totalActivadas;
    }
}
