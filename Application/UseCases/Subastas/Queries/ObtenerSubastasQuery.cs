using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Subastas.Queries;

/*
 Query CQRS para consultar el catálogo de subastas con filtros opcionales 
 por estado y categoría, y ordenamiento dinámico.
*/
public record ObtenerSubastasQuery(
    string? Estado = null,
    int? CategoriaId = null,
    string? Orden = null
);
