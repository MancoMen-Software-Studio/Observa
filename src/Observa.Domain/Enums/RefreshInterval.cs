namespace Observa.Domain.Enums;

/// <summary>
/// Intervalos de actualizacion para widgets del dashboard.
/// </summary>
public enum RefreshInterval
{
    RealTime = 0,
    FiveSeconds = 5,
    ThirtySeconds = 30,
    OneMinute = 60,
    FiveMinutes = 300,
    FifteenMinutes = 900
}
