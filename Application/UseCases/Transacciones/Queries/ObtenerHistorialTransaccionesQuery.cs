namespace Application.UseCases.Transacciones.Queries;


public record ObtenerHistorialTransaccionesQuery(
    int UsuarioId,
    int Pagina = 1,
    int TamanoPagina = 10
);
