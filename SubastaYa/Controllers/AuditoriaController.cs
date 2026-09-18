using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Auditorias.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Controllers;

[Authorize]
[ApiController]
[Route("api/auditorias")]
public class AuditoriaController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditoriaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginadoDTO<AuditoriaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerTodas(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new ObtenerAuditoriasQuery(pagina, tamanoPagina);
        var resultado = await _mediator.SendAsync<ObtenerAuditoriasQuery, ResultadoPaginadoDTO<AuditoriaDTO>>(query, cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{entidad}/{entidadId:int}")]
    [ProducesResponseType(typeof(ResultadoPaginadoDTO<AuditoriaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerPorEntidad(
        [FromRoute] string entidad,
        [FromRoute] int entidadId,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new ObtenerAuditoriaPorEntidadQuery(entidad, entidadId, pagina, tamanoPagina);
        var resultado = await _mediator.SendAsync<ObtenerAuditoriaPorEntidadQuery, ResultadoPaginadoDTO<AuditoriaDTO>>(query, cancellationToken);
        return Ok(resultado);
    }
}