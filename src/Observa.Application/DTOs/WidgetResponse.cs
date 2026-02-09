using System;

namespace Observa.Application.DTOs;

/// <summary>
/// Respuesta con los datos de un widget.
/// </summary>
public sealed record WidgetResponse(
    Guid Id,
    string Title,
    string Type,
    int Column,
    int Row,
    int Width,
    int Height,
    Guid DataSourceId,
    string RefreshInterval,
    DateTime CreatedAt);
