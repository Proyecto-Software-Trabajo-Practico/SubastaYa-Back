using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.UseCases.Subastas.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SubastaYa.Workers;


public class SubastasIniciadasWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SubastasIniciadasWorker> _logger;
    private readonly TimeSpan _intervalo = TimeSpan.FromSeconds(5);

    public SubastasIniciadasWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<SubastasIniciadasWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SubastasIniciadasWorker iniciado. Intervalo: {Intervalo}s", _intervalo.TotalSeconds);

        using var timer = new PeriodicTimer(_intervalo);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var command = new ActivarSubastasIniciadasCommand();
                var totalActivadas = await mediator.SendAsync<ActivarSubastasIniciadasCommand, int>(
                    command,
                    stoppingToken
                );

                if (totalActivadas > 0)
                {
                    _logger.LogInformation(
                        "[Worker] Apertura automática ejecutada con éxito: {Cantidad} subastas pasaron a ACTIVA.",
                        totalActivadas
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Worker] Error inesperado al activar subastas programadas en segundo plano.");
            }
        }

        _logger.LogInformation("SubastasIniciadasWorker detenido.");
    }
}