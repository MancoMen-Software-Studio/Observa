using MediatR;
using Observa.Domain.Abstractions;

namespace Observa.Application.Abstractions.Messaging;

/// <summary>
/// Handler para consultas que retornan un valor tipado.
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
