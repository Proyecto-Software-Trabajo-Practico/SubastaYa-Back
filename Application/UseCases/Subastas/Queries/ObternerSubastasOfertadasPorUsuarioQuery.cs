using Application.DTOs;
using Application.Interfaces;

namespace Application.UseCases.Subastas.Queries;

/*
 * Query CQRS inmutable para consultar el listado paginado de subastas
 * en las que ha participado activamente un comprador (Módulo 5).
 */
public record ObtenerSubastasOfertadasPorUsuarioQuery(
    int CompradorId,
    int Pagina = 1,
    int TamanoPagina = 10
);
