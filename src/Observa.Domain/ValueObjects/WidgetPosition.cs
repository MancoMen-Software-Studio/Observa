using System.Collections.Generic;
using Observa.Domain.Abstractions;

namespace Observa.Domain.ValueObjects;

/// <summary>
/// Posicion y dimensiones de un widget dentro del grid del dashboard.
/// </summary>
public sealed class WidgetPosition : ValueObject
{
    private WidgetPosition(int column, int row, int width, int height)
    {
        Column = column;
        Row = row;
        Width = width;
        Height = height;
    }

    public int Column { get; }

    public int Row { get; }

    public int Width { get; }

    public int Height { get; }

    public static Result<WidgetPosition> Create(int column, int row, int width, int height)
    {
        if (column < 0 || row < 0)
        {
            return Result<WidgetPosition>.Failure(WidgetPositionErrors.NegativePosition);
        }

        if (width <= 0 || height <= 0)
        {
            return Result<WidgetPosition>.Failure(WidgetPositionErrors.InvalidDimensions);
        }

        return Result<WidgetPosition>.Success(new WidgetPosition(column, row, width, height));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Column;
        yield return Row;
        yield return Width;
        yield return Height;
    }
}

/// <summary>
/// Errores relacionados con el value object WidgetPosition.
/// </summary>
public static class WidgetPositionErrors
{
    public static readonly Error NegativePosition = new("WidgetPosition.NegativePosition", "La posicion no puede ser negativa.");
    public static readonly Error InvalidDimensions = new("WidgetPosition.InvalidDimensions", "El ancho y alto deben ser mayores a cero.");
}
