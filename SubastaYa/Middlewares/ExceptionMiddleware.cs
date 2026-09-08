using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SubastaYa.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pasa el control al siguiente eslabón de la cebolla (CORS, Routing, Controllers, Handlers)
            await _next(context);
        }
        catch (DomainException ex)
        {
            // Regla de negocio violada -> 400 Bad Request
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (DbUpdateConcurrencyException)
        {
            // Optimistic Locking -> 409 Conflict
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "Otra operación modificó el recurso al mismo tiempo. Por favor actualizá y reintentá." });
        }
        catch (Exception ex)
        {
            // 500 Internal Server Error con log
            _logger.LogError(ex, "Error no controlado atrapado en ExceptionMiddleware");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "Ocurrió un error inesperado en el servidor." });
        }
    }
}