using System;
using System.Collections.Generic;

namespace Observa.Domain.Abstractions;

/// <summary>
/// Resultado paginado que contiene una coleccion de elementos y metadatos de paginacion.
/// </summary>
public sealed class PagedResult<T>
{
    public PagedResult(
        IReadOnlyCollection<T> items,
        int totalCount,
        int page,
        int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    /// <summary>
    /// Elementos de la pagina actual.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; }

    /// <summary>
    /// Numero total de elementos sin paginar.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Pagina actual (basada en 1).
    /// </summary>
    public int Page { get; }

    /// <summary>
    /// Cantidad de elementos por pagina.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Numero total de paginas.
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Indica si existe una pagina siguiente.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Indica si existe una pagina anterior.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}
