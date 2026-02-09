using System;
using System.Collections.Generic;
using Observa.Domain.Abstractions;
using Observa.Domain.Enums;

namespace Observa.Domain.ValueObjects;

/// <summary>
/// Regla de umbral para activar alertas cuando una metrica supera o baja de un valor.
/// </summary>
public sealed class ThresholdRule : ValueObject
{
    private ThresholdRule(string metricName, double value, ThresholdOperator @operator, AlertSeverity severity)
    {
        MetricName = metricName;
        Value = value;
        Operator = @operator;
        Severity = severity;
    }

    public string MetricName { get; }

    public double Value { get; }

    public ThresholdOperator Operator { get; }

    public AlertSeverity Severity { get; }

    public static Result<ThresholdRule> Create(
        string metricName,
        double value,
        ThresholdOperator thresholdOperator,
        AlertSeverity severity)
    {
        if (string.IsNullOrWhiteSpace(metricName))
        {
            return Result<ThresholdRule>.Failure(ThresholdRuleErrors.EmptyMetricName);
        }

        return Result<ThresholdRule>.Success(new ThresholdRule(metricName, value, thresholdOperator, severity));
    }

    public bool IsTriggered(double currentValue)
    {
        return Operator switch
        {
            ThresholdOperator.GreaterThan => currentValue > Value,
            ThresholdOperator.LessThan => currentValue < Value,
            ThresholdOperator.Equal => Math.Abs(currentValue - Value) < 0.0001,
            _ => false
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return MetricName;
        yield return Value;
        yield return Operator;
        yield return Severity;
    }
}

/// <summary>
/// Operadores de comparacion para reglas de umbral.
/// </summary>
public enum ThresholdOperator
{
    GreaterThan = 0,
    LessThan = 1,
    Equal = 2
}

/// <summary>
/// Errores relacionados con el value object ThresholdRule.
/// </summary>
public static class ThresholdRuleErrors
{
    public static readonly Error EmptyMetricName = new("ThresholdRule.EmptyMetricName", "El nombre de la metrica no puede estar vacio.");
}
