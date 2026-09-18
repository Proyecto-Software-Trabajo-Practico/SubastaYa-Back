using Application.DTOs;
using Application.Interfaces;

namespace Application.UseCases.Subastas.Queries;


public record ObtenerSubastasOfertadasPorUsuarioQuery(
    int CompradorId,
    int Pagina = 1,
    int TamanoPagina = 10
);
