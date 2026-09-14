namespace Application.UseCases.Auditorias.Queries;

public record ObtenerAuditoriaPorEntidadQuery(
    string Entidad,
    int EntidadId,
    int Pagina = 1,
    int TamanoPagina = 10
);
