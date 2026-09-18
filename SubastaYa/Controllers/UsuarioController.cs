using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Queries;
using Application.UseCases.Usuarios.Commands;
using Application.UseCases.Usuarios.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    private void ValidarAutorizacionUsuario(int id)
    {
        var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(claimId, out var usuarioAutenticadoId) || usuarioAutenticadoId != id)
        {
            throw new UnauthorizedAccessException("No tienes permiso para acceder o modificar este recurso.");
        }
    }

    /*
     * GET /api/Usuarios/{id}/subastas
     * Obtiene el listado paginado de publicaciones (como vendedor) del usuario especificado.
     */
    [Authorize]
    [HttpGet("{vendedorId:int}/subastas")]
    [ProducesResponseType(typeof(ResultadoPaginadoDTO<SubastaVendedorDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObtenerSubastasPorUsuario(
        [FromRoute] int vendedorId,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(claimId, out var usuarioAutenticadoId) || usuarioAutenticadoId != vendedorId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { mensaje = "No tienes permiso para consultar las publicaciones de otro usuario." });
        }

        var query = new ObtenerSubastasPorUsuarioQuery(vendedorId, pagina, tamanoPagina);

        var resultado = await _mediator.SendAsync<ObtenerSubastasPorUsuarioQuery, ResultadoPaginadoDTO<SubastaVendedorDTO>>(
            query,
            cancellationToken
        );

        return Ok(resultado);
    }

    /*
     * GET /api/Usuarios/{id}/ofertas
     * Obtiene el listado paginado de subastas donde el usuario especificado realizó ofertas.
     */
    [Authorize]
    [HttpGet("{compradorId:int}/ofertas")]
    [ProducesResponseType(typeof(ResultadoPaginadoDTO<SubastaCardDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObtenerSubastasOfertadasPorUsuario(
        [FromRoute] int compradorId,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 15,
        CancellationToken cancellationToken = default)
    {
        var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(claimId, out var usuarioAutenticadoId) || usuarioAutenticadoId != compradorId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { mensaje = "No tienes permiso para consultar las ofertas de otro usuario." });
        }

        var query = new ObtenerSubastasOfertadasPorUsuarioQuery(compradorId, pagina, tamanoPagina);

        var resultado = await _mediator.SendAsync<ObtenerSubastasOfertadasPorUsuarioQuery, ResultadoPaginadoDTO<SubastaCardDTO>>(
            query,
            cancellationToken
        );

        return Ok(resultado);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UsuarioDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerPorId(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var query = new ObtenerUsuarioPorIdQuery(id);
        var resultado = await _mediator.SendAsync<ObtenerUsuarioPorIdQuery, UsuarioDTO>(query, cancellationToken);
        return Ok(resultado);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UsuarioDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarUsuarioCommand command,
        CancellationToken cancellationToken)
    {
        var usuarioCreado = await _mediator.SendAsync<RegistrarUsuarioCommand, UsuarioDTO>(command, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = usuarioCreado.Id }, usuarioCreado);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginRespuestaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> IniciarSesion(
        [FromBody] IniciarSesionCommand command,
        CancellationToken cancellationToken)
    {
        var respuesta = await _mediator.SendAsync<IniciarSesionCommand, LoginRespuestaDTO>(command, cancellationToken);
        return Ok(respuesta);
    }

    [Authorize]
    [HttpPatch("{id:int}/email")]
    [ProducesResponseType(typeof(UsuarioDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CambiarEmail(
        [FromRoute] int id,
        [FromBody] CambiarEmailDTO dto,
        CancellationToken cancellationToken)
    {
        ValidarAutorizacionUsuario(id);

        var command = new CambiarEmailCommand(id, dto.NuevoEmail);
        var resultado = await _mediator.SendAsync<CambiarEmailCommand, UsuarioDTO>(command, cancellationToken);
        return Ok(resultado);
    }

    [Authorize]
    [HttpPatch("{id:int}/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CambiarPassword(
        [FromRoute] int id,
        [FromBody] CambiarPasswordDTO dto,
        CancellationToken cancellationToken)
    {
        ValidarAutorizacionUsuario(id);

        var command = new CambiarPasswordCommand(id, dto.PasswordActual, dto.NuevaPassword);
        await _mediator.SendAsync<CambiarPasswordCommand, bool>(command, cancellationToken);
        return NoContent();
    }
}
