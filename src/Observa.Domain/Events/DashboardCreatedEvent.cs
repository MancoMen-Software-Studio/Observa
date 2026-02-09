using System;
using Observa.Domain.Abstractions;

namespace Observa.Domain.Events;

/// <summary>
/// Evento emitido cuando se crea un nuevo dashboard.
/// </summary>
public sealed record DashboardCreatedEvent(Guid DashboardId, string Title) : DomainEvent;
