namespace Application.UseCases.Subastas.Queries;

/*
 * Query CQRS inmutable para consultar de forma paginada las publicaciones
 * pertenecientes a un vendedor determinado para el panel de usuario (Módulo 5).
 */
public record ObtenerSubastasPorUsuarioQuery(
    int VendedorId,
    int Pagina = 1,
    int TamanoPagina = 10
);
