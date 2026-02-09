using System;
using Observa.Domain.Abstractions;

namespace Observa.Domain.Events;

/// <summary>
/// Evento emitido cuando se remueve un widget de un dashboard.
/// </summary>
public sealed record WidgetRemovedEvent(Guid DashboardId, Guid WidgetId) : DomainEvent;
