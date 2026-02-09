using System;

namespace Observa.Domain.Abstractions;

/// <summary>
/// Marcador para eventos de dominio.
/// Los eventos representan hechos que ocurrieron en el dominio.
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }

    DateTime OccurredOn { get; }
}
