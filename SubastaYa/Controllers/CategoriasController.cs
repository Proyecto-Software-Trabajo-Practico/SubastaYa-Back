using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Categorias.Queries;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodas(CancellationToken cancellationToken)
    {
        // Especificamos el tipo de Query y la respuesta esperada List<CategoriaDto>
        var resultado = await _mediator.SendAsync<ObtenerCategoriasQuery, List<CategoriaDto>>(
            new ObtenerCategoriasQuery(),
            cancellationToken
        );

        return Ok(resultado);
    }
}