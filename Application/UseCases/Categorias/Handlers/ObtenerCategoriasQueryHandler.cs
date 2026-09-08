using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Categorias.Queries;
using static Application.UseCases.Categorias.Queries.ObtenerCategoriasQuery;

namespace Application.UseCases.Categorias.Handlers;

public class ObtenerCategoriasQueryHandler : IRequestHandler<ObtenerCategoriasQuery, List<CategoriaDto>>
{
    private readonly ICategoriaRepository _categoriaRepository;

    // Inyección de dependencias (Principio DIP de SOLID)
    public ObtenerCategoriasQueryHandler(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<List<CategoriaDto>> HandleAsync(ObtenerCategoriasQuery request, CancellationToken cancellationToken = default)
    {
        var categoriasDb = await _categoriaRepository.GetAllAsync();

        var resultado = categoriasDb.Select(c => new CategoriaDto(
            c.Id,
            c.Nombre,
            c.UrlIcono
        )).ToList();

        return resultado;
    }
}
