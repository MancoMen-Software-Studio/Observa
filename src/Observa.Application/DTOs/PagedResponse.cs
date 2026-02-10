using System.Collections.Generic;

namespace Observa.Application.DTOs;

/// <summary>
/// Respuesta paginada generica con metadatos de paginacion.
/// </summary>
public sealed record PagedResponse<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage);
