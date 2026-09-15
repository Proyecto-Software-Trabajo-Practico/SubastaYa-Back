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

/*
 * Handler responsable de la activación masiva de subastas programadas.
 * Recupera las subastas cuya fecha de inicio se ha cumplido, invoca el método
 * de dominio Activar(), genera registros de auditoría y persiste los cambios
 * de manera atómica con IUnitOfWork.
 */
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
        // 1. Obtener todas las subastas programadas cuya fecha de inicio ya fue alcanzada
        var subastasParaIniciar = await _subastaRepository.GetSubastasProgramadasParaInicioAsync();

        if (!subastasParaIniciar.Any())
        {
            return 0;
        }

        int totalActivadas = 0;

        foreach (var subasta in subastasParaIniciar)
        {
            // 2. Invocar el método de dominio (protege la invariante de negocio)
            subasta.Activar();
            _subastaRepository.Update(subasta);
            totalActivadas++;

            // 3. Registrar auditoría obligatoria de la transición de estado
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

        // 4. Confirmar todos los cambios en una única transacción atómica (ACID)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return totalActivadas;
    }
}
