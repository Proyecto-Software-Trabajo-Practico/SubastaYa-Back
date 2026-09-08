using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Usuarios.Commands;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(UsuarioDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var usuarioCreado = await _mediator.SendAsync<RegistrarUsuarioCommand, UsuarioDTO>(command, cancellationToken);
            return CreatedAtAction(nameof(Registrar), new { id = usuarioCreado.Id }, usuarioCreado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginRespuestaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IniciarSesion([FromBody] IniciarSesionCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var respuesta = await _mediator.SendAsync<IniciarSesionCommand, LoginRespuestaDTO>(command, cancellationToken);
            return Ok(respuesta);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
