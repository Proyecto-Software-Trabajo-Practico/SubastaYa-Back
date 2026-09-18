namespace Application.UseCases.Subastas.Queries;


public record ObtenerSubastasPorUsuarioQuery(
    int VendedorId,
    int Pagina = 1,
    int TamanoPagina = 10
);
