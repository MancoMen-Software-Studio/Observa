using System;
using System.Collections.Generic;

namespace Observa.Application.DTOs;

/// <summary>
/// Respuesta con los datos de un dashboard.
/// </summary>
public sealed record DashboardResponse(
    Guid Id,
    string Title,
    string Description,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyCollection<WidgetResponse> Widgets);
