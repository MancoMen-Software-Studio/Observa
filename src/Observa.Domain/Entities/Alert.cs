using System;
using Observa.Domain.Abstractions;
using Observa.Domain.Enums;
using Observa.Domain.ValueObjects;

namespace Observa.Domain.Entities;

/// <summary>
/// Alerta generada cuando una metrica supera un umbral configurado.
/// </summary>
public sealed class Alert : Entity
{
    private Alert(Guid id, Guid dashboardId, ThresholdRule rule, string message)
        : base(id)
    {
        DashboardId = dashboardId;
        Rule = rule;
        Message = message;
        IsAcknowledged = false;
        TriggeredAt = DateTime.UtcNow;
    }

    private Alert()
    {
    }

    public Guid DashboardId { get; private set; }

    public ThresholdRule Rule { get; private set; } = null!;

    public string Message { get; private set; } = string.Empty;

    public bool IsAcknowledged { get; private set; }

    public DateTime TriggeredAt { get; private set; }

    public DateTime? AcknowledgedAt { get; private set; }

    public static Alert Create(Guid dashboardId, ThresholdRule rule, string message)
    {
        return new Alert(Guid.NewGuid(), dashboardId, rule, message);
    }

    public void Acknowledge()
    {
        IsAcknowledged = true;
        AcknowledgedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Errores relacionados con la entidad Alert.
/// </summary>
public static class AlertErrors
{
    public static readonly Error NotFound = new("Alert.NotFound", "La alerta no fue encontrada.");
    public static readonly Error AlreadyAcknowledged = new("Alert.AlreadyAcknowledged", "La alerta ya fue reconocida.");
}
