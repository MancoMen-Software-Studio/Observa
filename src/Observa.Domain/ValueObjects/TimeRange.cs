using System;
using System.Collections.Generic;
using Observa.Domain.Abstractions;

namespace Observa.Domain.ValueObjects;

/// <summary>
/// Rango de tiempo para consultas y visualizacion de datos.
/// </summary>
public sealed class TimeRange : ValueObject
{
    private TimeRange(DateTime from, DateTime to)
    {
        From = from;
        To = to;
    }

    public DateTime From { get; }

    public DateTime To { get; }

    public TimeSpan Duration => To - From;

    public static Result<TimeRange> Create(DateTime from, DateTime to)
    {
        if (from >= to)
        {
            return Result<TimeRange>.Failure(TimeRangeErrors.InvalidRange);
        }

        return Result<TimeRange>.Success(new TimeRange(from, to));
    }

    public bool Contains(DateTime dateTime)
    {
        return dateTime >= From && dateTime <= To;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return From;
        yield return To;
    }
}

/// <summary>
/// Errores relacionados con el value object TimeRange.
/// </summary>
public static class TimeRangeErrors
{
    public static readonly Error InvalidRange = new("TimeRange.InvalidRange", "La fecha de inicio debe ser anterior a la fecha de fin.");
}
