using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Billeteras.Commands;
using Application.UseCases.Billeteras.Queries;
using Application.UseCases.Transacciones.Queries;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BilleterasController : ControllerBase
{
    private readonly IMediator _mediator;

   
    public BilleterasController(IMediator mediator)
    {
        _mediator = mediator;
    }

   
    /// Ruta: GET /api/billeteras/{usuarioId}/saldos
    
    [HttpGet("{usuarioId:int}/saldos")]
    public async Task<IActionResult> ObtenerSaldos(int usuarioId, CancellationToken cancellationToken)
    {
        var resultado = await _mediator.SendAsync<ObtenerSaldosQuery, BilleteraSaldosDto?>(
            new ObtenerSaldosQuery(usuarioId),
            cancellationToken
        );

        if (resultado is null)
        {
            return NotFound(new { mensaje = $"No se encontró una billetera asociada al usuario con ID {usuarioId}." });
        }

        return Ok(resultado);
    }
    [HttpPost("{usuarioId:int}/depositos")]
    public async Task<IActionResult> Depositar(
        int usuarioId,
        [FromBody] CargarSaldoDto request,
        CancellationToken cancellationToken)
    {
        var command = new DepositarFondosCommand(usuarioId, request.Monto);

        var saldosActualizados = await _mediator.SendAsync<DepositarFondosCommand, BilleteraSaldosDto>(
            command,
            cancellationToken
        );
        return Ok(saldosActualizados);
    }
    
    [HttpGet("{usuarioId:int}/transacciones")]
    public async Task<IActionResult> ObtenerHistorialTransacciones(
        int usuarioId,
        CancellationToken cancellationToken)
    {
        var transacciones = await _mediator.SendAsync<ObtenerHistorialTransaccionesQuery, List<TransaccionLedgerDTO>?>(
            new ObtenerHistorialTransaccionesQuery(usuarioId),
            cancellationToken
        );

        if (transacciones is null)
        {
            return NotFound(new { mensaje = $"No se encontró una billetera asociada al usuario con ID {usuarioId}." });
        }

        return Ok(transacciones);
    }
}