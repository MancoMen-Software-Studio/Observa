using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Observa.Domain.Abstractions;

namespace Observa.Application.Behaviors;

/// <summary>
/// Behavior del pipeline de MediatR que registra la ejecucion y resultado de cada request.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private static readonly Action<ILogger, string, Exception?> LogProcessing =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, "Processing"), "Procesando request {RequestName}");

    private static readonly Action<ILogger, string, string, string, Exception?> LogFailed =
        LoggerMessage.Define<string, string, string>(LogLevel.Warning, new EventId(2, "Failed"), "Request {RequestName} fallo con error {ErrorCode}: {ErrorDescription}");

    private static readonly Action<ILogger, string, Exception?> LogCompleted =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(3, "Completed"), "Request {RequestName} completado exitosamente");

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        LogProcessing(_logger, requestName, null);

        var result = await next();

        if (result.IsFailure)
        {
            LogFailed(_logger, requestName, result.Error.Code, result.Error.Description, null);
        }
        else
        {
            LogCompleted(_logger, requestName, null);
        }

        return result;
    }
}
