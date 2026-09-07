using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Mediators;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
    {
        // Forzamos el tipo de la interfaz para evitar ambigüedades de resolucion
        IRequestHandler<TRequest, TResponse> handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

        return await handler.HandleAsync(request, cancellationToken);
    }
}