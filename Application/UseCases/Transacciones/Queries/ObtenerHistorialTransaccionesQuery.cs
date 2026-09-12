namespace Application.UseCases.Transacciones.Queries;

/*
 * Query CQRS para consultar el historial de movimientos contables de una billetera
 * de forma paginada y ordenada cronológicamente (más recientes primero).
 */
public record ObtenerHistorialTransaccionesQuery(
    int UsuarioId,
    int Pagina = 1,
    int TamanoPagina = 10
);
