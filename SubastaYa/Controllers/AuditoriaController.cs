using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Auditorias.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuditoriaController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditoriaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuditoriaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerTodas(CancellationToken cancellationToken)
    {
        var query = new ObtenerAuditoriasQuery();
        var resultado = await _mediator.SendAsync<ObtenerAuditoriasQuery, IEnumerable<AuditoriaDTO>>(query, cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{entidad}/{entidadId:int}")]
    [ProducesResponseType(typeof(IEnumerable<AuditoriaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerPorEntidad(
        [FromRoute] string entidad,
        [FromRoute] int entidadId,
        CancellationToken cancellationToken)
    {
        var query = new ObtenerAuditoriaPorEntidadQuery(entidad, entidadId);
        var resultado = await _mediator.SendAsync<ObtenerAuditoriaPorEntidadQuery, IEnumerable<AuditoriaDTO>>(query, cancellationToken);
        return Ok(resultado);
    }
}