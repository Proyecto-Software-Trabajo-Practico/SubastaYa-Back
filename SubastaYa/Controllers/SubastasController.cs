using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubastasController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubastasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerDetalle(int id, CancellationToken cancellationToken)
    {
        var subasta = await _mediator.SendAsync<ObtenerDetalleSubastaQuery, SubastaDetalleDTO?>(
            new ObtenerDetalleSubastaQuery(id),
            cancellationToken
        );

        if (subasta is null)
        {
            return NotFound(new { mensaje = $"No se encontró una subasta asociada al ID {id}." });
        }

        return Ok(subasta);
    }
}