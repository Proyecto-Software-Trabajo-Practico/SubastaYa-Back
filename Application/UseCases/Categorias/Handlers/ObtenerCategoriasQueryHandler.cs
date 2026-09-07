using Application.Interfaces;
using Application.UseCases.Categorias.Queries;
using static Application.UseCases.Categorias.Queries.ObtenerCategoriasQuery;

namespace Application.UseCases.Categorias.Handlers;

public class ObtenerCategoriasQueryHandler
{
    private readonly ICategoriaRepository _categoriaRepository;

    // Inyección de dependencias (Principio DIP de SOLID)
    public ObtenerCategoriasQueryHandler(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<List<CategoriaDto>> Handle(ObtenerCategoriasQuery query)
    {
        // 1. Vamos a buscar los datos
        var categoriasDb = await _categoriaRepository.GetAllAsync(); // Ajustá este nombre si le pusieron distinto en tu interfaz

        // 2. Proyectamos la entidad de dominio hacia nuestro DTO de lectura
        var resultado = categoriasDb.Select(c => new CategoriaDto(
            c.Id,
            c.Nombre,
            c.UrlIcono
        )).ToList();

        return resultado;
    }
}
