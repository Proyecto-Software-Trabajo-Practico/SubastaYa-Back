using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Interfaces;
using Application.UseCases.Categorias.Handlers;
using Application.UseCases.Categorias.Queries;

namespace Application.Mediators;

public class Mediator : IMediator
{
    // 1. El mediador es el único que conoce a TODOS los handlers del sistema
    private readonly ObtenerCategoriasQueryHandler _obtenerCategoriasHandler;
    // private readonly CrearSubastaCommandHandler _crearSubastaHandler;

    public Mediator (ObtenerCategoriasQueryHandler obtenerCategoriasHandler)
    {
        _obtenerCategoriasHandler = obtenerCategoriasHandler;
    }

    // 2. Implementamos los métodos SendAsync. 
    // Cuando alguien llame a SendAsync pasándole un ObtenerCategoriasQuery, 
    // el Mediador se lo pasa a su respectivo Handler.
    public Task<List<CategoriaDto>> SendAsync(ObtenerCategoriasQuery query)
    {
        return _obtenerCategoriasHandler.Handle(query);
    }
}