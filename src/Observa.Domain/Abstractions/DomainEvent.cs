using System;

namespace Observa.Domain.Abstractions;

/// <summary>
/// Implementacion base de un evento de dominio con identificador y marca temporal.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();

    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
