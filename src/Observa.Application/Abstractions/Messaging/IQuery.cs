using MediatR;
using Observa.Domain.Abstractions;

namespace Observa.Application.Abstractions.Messaging;

/// <summary>
/// Consulta que retorna un valor tipado envuelto en Result.
/// </summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
