namespace Application.UseCases.Auditorias.Queries;

public record ObtenerAuditoriasQuery(
    int Pagina = 1,
    int TamanoPagina = 10
);
