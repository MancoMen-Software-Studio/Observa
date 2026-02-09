using MediatR;
using Observa.Domain.Abstractions;

namespace Observa.Application.Abstractions.Messaging;

/// <summary>
/// Comando que no retorna valor, solo resultado de exito o fallo.
/// </summary>
public interface ICommand : IRequest<Result>;

/// <summary>
/// Comando que retorna un valor envuelto en Result en caso de exito.
/// </summary>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
