using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Observa.Application.Abstractions.Messaging;
using Observa.Application.DTOs;
using Observa.Domain.Abstractions;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;

namespace Observa.Application.Queries.Dashboards;

/// <summary>
/// Consulta paginada para obtener dashboards del sistema.
/// </summary>
public sealed record GetAllDashboardsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Status = null,
    string? Search = null) : IQuery<PagedResponse<DashboardResponse>>;

/// <summary>
/// Handler que procesa la consulta paginada de dashboards.
/// </summary>
public sealed class GetAllDashboardsQueryHandler
    : IQueryHandler<GetAllDashboardsQuery, PagedResponse<DashboardResponse>>
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetAllDashboardsQueryHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<Result<PagedResponse<DashboardResponse>>> Handle(
        GetAllDashboardsQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        DashboardStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<DashboardStatus>(request.Status, ignoreCase: true, out var parsed))
        {
            statusFilter = parsed;
        }

        var pagedResult = await _dashboardRepository.GetPagedAsync(
            page,
            pageSize,
            statusFilter,
            request.Search,
            cancellationToken);

        var items = pagedResult.Items
            .Select(d => new DashboardResponse(
                d.Id,
                d.Title,
                d.Description,
                d.Status.ToString(),
                d.CreatedAt,
                d.UpdatedAt,
                d.Widgets
                    .Select(w => new WidgetResponse(
                        w.Id,
                        w.Title,
                        w.Type.ToString(),
                        w.Position.Column,
                        w.Position.Row,
                        w.Position.Width,
                        w.Position.Height,
                        w.DataSourceId,
                        w.RefreshInterval.ToString(),
                        w.CreatedAt))
                    .ToList()
                    .AsReadOnly()))
            .ToList()
            .AsReadOnly();

        var response = new PagedResponse<DashboardResponse>(
            items,
            pagedResult.TotalCount,
            pagedResult.Page,
            pagedResult.PageSize,
            pagedResult.TotalPages,
            pagedResult.HasNextPage,
            pagedResult.HasPreviousPage);

        return Result<PagedResponse<DashboardResponse>>.Success(response);
    }
}
