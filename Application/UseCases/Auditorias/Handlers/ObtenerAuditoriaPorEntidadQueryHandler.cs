using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Auditorias.Queries;

namespace Application.UseCases.Auditorias.Handlers;

public class ObtenerAuditoriaPorEntidadQueryHandler : IRequestHandler<ObtenerAuditoriaPorEntidadQuery, ResultadoPaginadoDTO<AuditoriaDTO>>
{
    private readonly IAuditoriaRepository _auditoriaRepository;

    public ObtenerAuditoriaPorEntidadQueryHandler(IAuditoriaRepository auditoriaRepository)
    {
        _auditoriaRepository = auditoriaRepository;
    }

    public async Task<ResultadoPaginadoDTO<AuditoriaDTO>> HandleAsync(
        ObtenerAuditoriaPorEntidadQuery request,
        CancellationToken cancellationToken = default)
    {
        var (entidades, totalItems) = await _auditoriaRepository.GetByEntidadPaginadasAsync(
            request.Entidad,
            request.EntidadId,
            request.Pagina,
            request.TamanoPagina,
            cancellationToken);

        var itemsDto = entidades.Select(a => new AuditoriaDTO(
            a.Id,
            a.Entidad,
            a.EntidadId,
            a.Accion,
            a.UsuarioId,
            a.DetalleJson,
            a.Fecha
        )).ToList();

        int totalPaginas = (int)Math.Ceiling((double)totalItems / request.TamanoPagina);

        return new ResultadoPaginadoDTO<AuditoriaDTO>(
            itemsDto,
            totalItems,
            request.Pagina,
            request.TamanoPagina,
            totalPaginas
        );
    }
}