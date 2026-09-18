using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Subastas.Queries;


public record ObtenerSubastasQuery(
    string? Estado = null,
    int? CategoriaId = null,
    int? VendedorId = null,
    string? Orden = null,
    int Pagina = 1,
    int TamanoPagina = 10
);
