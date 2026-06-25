using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SGE.Aplicacion.Comun;

namespace SGE.WebApi.Middlewares;

public class ExcepcionGlobalMiddleware(RequestDelegate next, ILogger<ExcepcionGlobalMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExcepcionGlobalMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió un error no controlado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        
        var statusCode = HttpStatusCode.InternalServerError;
        var title = "Error interno del servidor";
        var detail = exception.Message;

        switch (exception)
        {
            case ValidacionException:
                statusCode = HttpStatusCode.BadRequest; // 400
                title = "Solicitud incorrecta o datos inválidos";
                break;

            case AutorizacionException:
                statusCode = HttpStatusCode.Forbidden; // 403
                title = "Acceso denegado / No autorizado";
                break;

            case EntidadNoEncontradaException:
                statusCode = HttpStatusCode.NotFound; // 404
                title = "Recurso no encontrado";
                break;

            case EntidadDuplicadaException:
                statusCode = HttpStatusCode.Conflict; // 409
                title = "Conflicto con un recurso existente";
                break;

            case RepositoryException:
                statusCode = HttpStatusCode.UnprocessableEntity; // 422
                title = "Error al procesar la persistencia";
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        var json = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(json);
    }
}