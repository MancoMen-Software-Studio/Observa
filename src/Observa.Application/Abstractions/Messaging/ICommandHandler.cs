using MediatR;
using Observa.Domain.Abstractions;

namespace Observa.Application.Abstractions.Messaging;

/// <summary>
/// Handler para comandos que no retornan valor.
/// </summary>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;

/// <summary>
/// Handler para comandos que retornan un valor tipado.
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;
