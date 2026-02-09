using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Observa.Api.Middleware;

/// <summary>
/// Manejador global de excepciones no controladas.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private static readonly Action<ILogger, string, Exception?> s_logUnhandledException =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(500, "UnhandledException"), "Excepcion no controlada: {Message}");

    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        s_logUnhandledException(_logger, exception.Message, exception);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Error interno del servidor",
            Detail = "Ha ocurrido un error inesperado. Intente nuevamente mas tarde."
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
