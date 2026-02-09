using System;
using Observa.Domain.Abstractions;

namespace Observa.Domain.Events;

/// <summary>
/// Evento emitido cuando se agrega un widget a un dashboard.
/// </summary>
public sealed record WidgetAddedEvent(Guid DashboardId, Guid WidgetId, string WidgetTitle) : DomainEvent;
