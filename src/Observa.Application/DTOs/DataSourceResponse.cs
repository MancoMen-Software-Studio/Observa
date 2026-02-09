using System;

namespace Observa.Application.DTOs;

/// <summary>
/// Respuesta con los datos de un origen de datos.
/// </summary>
public sealed record DataSourceResponse(
    Guid Id,
    string Name,
    string Type,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastSyncAt);
