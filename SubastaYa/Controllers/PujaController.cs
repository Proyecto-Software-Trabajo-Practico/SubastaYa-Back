using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Pujas.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PujasController : ControllerBase
{
    private readonly IMediator _mediator;

    public PujasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PujaDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CrearPuja(
        [FromBody] CrearPujaRequestDTO dto,
        CancellationToken cancellationToken)
    {
        // Extraer el CompradorId directamente desde el Token JWT
        var claimUsuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(claimUsuarioId) || !int.TryParse(claimUsuarioId, out int compradorId))
        {
            return Unauthorized();
        }

        var command = new CrearPujaCommand(dto.SubastaId, compradorId, dto.Monto);

        var resultado = await _mediator.SendAsync<CrearPujaCommand, PujaDTO>(command, cancellationToken);

        return CreatedAtAction(nameof(CrearPuja), new { id = resultado.Id }, resultado);
    }
}

// DTO para el body del request HTTP
public record CrearPujaRequestDTO(
    int SubastaId,
    decimal Monto
);
