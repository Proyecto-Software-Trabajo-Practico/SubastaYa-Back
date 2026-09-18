using System.Collections.Generic;

namespace Application.DTOs;


public record ResultadoPaginadoDTO<T>(
    IReadOnlyList<T> Items,
    int TotalItems,
    int Pagina,
    int TamanoPagina,
    int TotalPaginas
)
{
    public bool TienePaginaPrevia => Pagina > 1;
    public bool TienePaginaSiguiente => Pagina < TotalPaginas;
}
