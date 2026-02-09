namespace Observa.Domain.Enums;

/// <summary>
/// Tipos de origen de datos soportados por la plataforma.
/// </summary>
public enum DataSourceType
{
    RestApi = 0,
    Database = 1,
    WebSocket = 2,
    StaticFile = 3,
    SignalR = 4
}
