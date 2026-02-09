using System;
using Observa.Domain.Abstractions;
using Observa.Domain.Enums;

namespace Observa.Domain.Events;

/// <summary>
/// Evento emitido cuando se activa una alerta por superacion de umbral.
/// </summary>
public sealed record AlertTriggeredEvent(
    Guid DashboardId,
    Guid AlertId,
    string MetricName,
    AlertSeverity Severity) : DomainEvent;
