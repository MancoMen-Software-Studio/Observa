namespace Observa.Domain.Abstractions;

/// <summary>
/// Representa un error de dominio con codigo y descripcion.
/// </summary>
#pragma warning disable CA1716
public sealed record Error(string Code, string Description)
#pragma warning restore CA1716
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Error NullValue = new("Error.NullValue", "El valor proporcionado es nulo.");
}
