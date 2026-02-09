using System.Collections.Generic;
using System.Text.RegularExpressions;
using Observa.Domain.Abstractions;

namespace Observa.Domain.ValueObjects;

/// <summary>
/// Representa un color en formato hexadecimal para configuracion visual de widgets.
/// </summary>
public sealed class Color : ValueObject
{
    private static readonly Regex s_hexPattern = new(@"^#([0-9A-Fa-f]{6})$", RegexOptions.Compiled);

    private Color(string hexValue)
    {
        HexValue = hexValue;
    }

    public string HexValue { get; }

    public static Result<Color> Create(string hexValue)
    {
        if (string.IsNullOrWhiteSpace(hexValue))
        {
            return Result<Color>.Failure(ColorErrors.Empty);
        }

        if (!s_hexPattern.IsMatch(hexValue))
        {
            return Result<Color>.Failure(ColorErrors.InvalidFormat);
        }

        return Result<Color>.Success(new Color(hexValue.ToUpperInvariant()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return HexValue;
    }

    public override string ToString()
    {
        return HexValue;
    }
}

/// <summary>
/// Errores relacionados con el value object Color.
/// </summary>
public static class ColorErrors
{
    public static readonly Error Empty = new("Color.Empty", "El color no puede estar vacio.");
    public static readonly Error InvalidFormat = new("Color.InvalidFormat", "El color debe estar en formato hexadecimal (#RRGGBB).");
}
