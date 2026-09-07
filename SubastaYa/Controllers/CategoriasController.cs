using Application.Interfaces;
using Application.UseCases.Categorias.Queries;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly IMediator _mediator;

    // Inyectamos el mediador por constructor
    public CategoriasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        // El controller solo le pide al mediador que resuelva la Query
        var resultado = await _mediator.SendAsync(new ObtenerCategoriasQuery());
        return Ok(resultado);
    }
}