using System.Collections.Generic;

namespace Application.DTOs;

/*
 * Contenedor genérico para respuestas paginadas de la API (RESTful Nivel 2).
 * Provee metadatos de navegación (página actual, total de páginas y total de elementos)
 * junto con la colección de elementos solicitados.
 */
public record ResultadoPaginadoDTO<T>(
    IReadOnlyList<T> Items,
    int TotalItems,
    int Pagina,
    int TamanoPagina,
    int TotalPaginas
)
{
    // Banderas calculadas para que el Frontend sepa si habilitar o deshabilitar los botones "Anterior" y "Siguiente"
    public bool TienePaginaPrevia => Pagina > 1;
    public bool TienePaginaSiguiente => Pagina < TotalPaginas;
}
