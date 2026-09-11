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
            // Ejecuta el controlador y los UseCases
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            // Recurso no encontrado -> 404 Not Found
            await ManejarExcepcionAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            // Operación inválida o regla de negocio fallida -> 400 Bad Request
            await ManejarExcepcionAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Permiso denegado -> 403 Forbidden
            await ManejarExcepcionAsync(context, StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (DomainException ex)
        {
            // Excepción de Dominio -> 400 Bad Request
            await ManejarExcepcionAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Conflicto de Optimistic Locking -> 409 Conflict
            await ManejarExcepcionAsync(context, StatusCodes.Status409Conflict, "Otra operación modificó el recurso al mismo tiempo. Por favor actualizá y reintentá.");
        }
        catch (Exception ex)
        {
            // Errores no controlados -> 500 Internal Server Error
            _logger.LogError(ex, "Error no controlado atrapado en ExceptionMiddleware");
            await ManejarExcepcionAsync(context, StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado en el servidor.");
        }
    }

    private static async Task ManejarExcepcionAsync(HttpContext context, int statusCode, string mensaje)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new { error = mensaje });
    }
}