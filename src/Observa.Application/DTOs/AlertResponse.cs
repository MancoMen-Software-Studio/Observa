using System;

namespace Observa.Application.DTOs;

/// <summary>
/// Respuesta con los datos de una alerta.
/// </summary>
public sealed record AlertResponse(
    Guid Id,
    Guid DashboardId,
    string MetricName,
    string Severity,
    string Message,
    bool IsAcknowledged,
    DateTime TriggeredAt,
    DateTime? AcknowledgedAt);
