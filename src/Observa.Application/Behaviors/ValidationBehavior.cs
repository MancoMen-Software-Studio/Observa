using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Observa.Domain.Abstractions;

namespace Observa.Application.Behaviors;

/// <summary>
/// Behavior del pipeline de MediatR que ejecuta validaciones FluentValidation antes del handler.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failure = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .FirstOrDefault();

        if (failure is null)
        {
            return await next();
        }

        return (TResponse)(object)Result.Failure(new Error(failure.PropertyName, failure.ErrorMessage));
    }
}
