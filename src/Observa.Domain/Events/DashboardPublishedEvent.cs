using System;
using Observa.Domain.Abstractions;

namespace Observa.Domain.Events;

/// <summary>
/// Evento emitido cuando un dashboard cambia a estado publicado.
/// </summary>
public sealed record DashboardPublishedEvent(Guid DashboardId) : DomainEvent;
