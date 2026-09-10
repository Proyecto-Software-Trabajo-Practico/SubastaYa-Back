using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Auditorias.Queries;

namespace Application.UseCases.Auditorias.Handlers;

public class ObtenerAuditoriasQueryHandler : IRequestHandler<ObtenerAuditoriasQuery, IEnumerable<AuditoriaDTO>>
{
    private readonly IAuditoriaRepository _auditoriaRepository;

    public ObtenerAuditoriasQueryHandler(IAuditoriaRepository auditoriaRepository)
    {
        _auditoriaRepository = auditoriaRepository;
    }

    public async Task<IEnumerable<AuditoriaDTO>> HandleAsync(ObtenerAuditoriasQuery request, CancellationToken cancellationToken = default)
    {
        var logs = await _auditoriaRepository.GetAllAsync();

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
