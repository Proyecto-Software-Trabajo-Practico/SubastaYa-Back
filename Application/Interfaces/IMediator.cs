using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IMediator
{
    // ir agregando una sobrecarga por cada caso de uso que tenga el sistema
    Task<List<Application.UseCases.Categorias.Queries.CategoriaDto>> SendAsync(Application.UseCases.Categorias.Queries.ObtenerCategoriasQuery query);

    
    //Task<int> SendAsync(Application.UseCases.Subastas.Commands.CrearSubastaCommand command);
}
