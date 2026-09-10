using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;
using Application.UseCases.Subastas.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
    /*
    Ruta: POST /api/subastas
    Permite a un vendedor autenticado publicar una nueva subasta.
    Retorna 201 Created con la cabecera Location apuntando a GET /api/subastas/{id}.
    */
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CrearSubasta(
        [FromBody] CrearSubastaDTO request,
        CancellationToken cancellationToken)
    {
        var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(claimId, out var vendedorId))
        {
            return Unauthorized(new { error = "No se pudo identificar al usuario autenticado a partir del token." });
        }

        var command = new CrearSubastaCommand(vendedorId, request);

        var subastaId = await _mediator.SendAsync<CrearSubastaCommand, int>(command, cancellationToken);

        return CreatedAtAction(nameof(ObtenerDetalle), new { id = subastaId }, new { id = subastaId });
    }
}