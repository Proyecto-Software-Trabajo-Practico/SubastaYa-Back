using System;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Subastas.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SubastaYa.Workers;


public class SubastasVencidasWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SubastasVencidasWorker> _logger;
    private readonly TimeSpan _intervalo = TimeSpan.FromSeconds(30);

    public SubastasVencidasWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<SubastasVencidasWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SubastasVencidasWorker iniciado. Intervalo: {Intervalo}s", _intervalo.TotalSeconds);

        // PeriodicTimer provee una cadencia precisa y eficiente sin consumir hilos innecesarios
        using var timer = new PeriodicTimer(_intervalo);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                // Creamos un scope delimitado para resolver dependencias Scoped (Mediator, DbContext, Repositorios)
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                // Despachamos el mismo caso de uso que ejecutaba el endpoint manual
                var command = new ProcesarSubastasFinalizadasCommand();
                var resultado = await mediator.SendAsync<ProcesarSubastasFinalizadasCommand, SubastasProcesadasDTO>(
                    command,
                    stoppingToken
                );

                if (resultado.TotalProcesadas > 0)
                {
                    _logger.LogInformation(
                        "[Worker] Barrido automático ejecutado con éxito: {Finalizadas} finalizadas con ganador, {Desiertas} desiertas.",
                        resultado.FinalizadasConGanador,
                        resultado.DeclaradasDesiertas
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Worker] Error inesperado al procesar subastas vencidas en segundo plano.");
            }
        }

        _logger.LogInformation("SubastasVencidasWorker detenido.");
    }
}
