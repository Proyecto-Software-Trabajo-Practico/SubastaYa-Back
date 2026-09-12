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
    
    //Permite a un vendedor autenticado publicar una nueva subasta.
    
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
    
     //Permite consultar el catálogo público de subastas con filtros opcionales (estado, categoría) y ordenamiento dinámico.
    
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginadoDTO<SubastaCardDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerSubastas(
        [FromQuery] string? estado,
        [FromQuery] int? categoriaId,
        [FromQuery] int? vendedorId,
        [FromQuery] string? orden,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new ObtenerSubastasQuery(estado, categoriaId, vendedorId, orden, pagina, tamanoPagina);

        var subastas = await _mediator.SendAsync<ObtenerSubastasQuery, ResultadoPaginadoDTO<SubastaCardDTO>>(
            query,
            cancellationToken
        );

        return Ok(subastas);
    }

    // Dispara manualmente el procesamiento y liquidación contable de subastas vencidas.
    [HttpPost("procesar-finalizadas")]
    [ProducesResponseType(typeof(SubastasProcesadasDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> ProcesarFinalizadas(CancellationToken cancellationToken)
    {
        var command = new ProcesarSubastasFinalizadasCommand();

        var resultado = await _mediator.SendAsync<ProcesarSubastasFinalizadasCommand, SubastasProcesadasDTO>(
            command,
            cancellationToken
        );

        return Ok(resultado);
    }
}