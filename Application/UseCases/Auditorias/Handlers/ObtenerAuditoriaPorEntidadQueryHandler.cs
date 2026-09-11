using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Auditorias.Queries;

namespace Application.UseCases.Auditorias.Handlers;

public class ObtenerAuditoriaPorEntidadQueryHandler : IRequestHandler<ObtenerAuditoriaPorEntidadQuery, IEnumerable<AuditoriaDTO>>
{
    private readonly IAuditoriaRepository _auditoriaRepository;

    public ObtenerAuditoriaPorEntidadQueryHandler(IAuditoriaRepository auditoriaRepository)
    {
        _auditoriaRepository = auditoriaRepository;
    }

    public async Task<IEnumerable<AuditoriaDTO>> HandleAsync(ObtenerAuditoriaPorEntidadQuery request, CancellationToken cancellationToken = default)
    {
        var logs = await _auditoriaRepository.GetByEntidadAsync(request.Entidad, request.EntidadId);

        return logs.Select(a => new AuditoriaDTO(
            a.Id,
            a.Entidad,
            a.EntidadId,
            a.Accion,
            a.UsuarioId,
            a.DetalleJson,
            a.Fecha
        ));
    }
}