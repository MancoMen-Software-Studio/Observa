using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Observa.Application.Abstractions.Messaging;
using Observa.Application.DTOs;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Repositories;

namespace Observa.Application.Queries.Dashboards;

/// <summary>
/// Consulta para obtener un dashboard por su identificador.
/// </summary>
public sealed record GetDashboardByIdQuery(Guid DashboardId) : IQuery<DashboardResponse>;

/// <summary>
/// Handler que procesa la consulta de un dashboard por identificador.
/// </summary>
public sealed class GetDashboardByIdQueryHandler : IQueryHandler<GetDashboardByIdQuery, DashboardResponse>
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetDashboardByIdQueryHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<Result<DashboardResponse>> Handle(GetDashboardByIdQuery request, CancellationToken cancellationToken)
    {
        var dashboard = await _dashboardRepository.GetByIdAsync(request.DashboardId, cancellationToken);

        if (dashboard is null)
        {
            return Result<DashboardResponse>.Failure(DashboardErrors.NotFound);
        }

        var response = MapToResponse(dashboard);

        return Result<DashboardResponse>.Success(response);
    }

    private static DashboardResponse MapToResponse(Dashboard dashboard)
    {
        var widgets = dashboard.Widgets
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
            .AsReadOnly();

        return new DashboardResponse(
            dashboard.Id,
            dashboard.Title,
            dashboard.Description,
            dashboard.Status.ToString(),
            dashboard.CreatedAt,
            dashboard.UpdatedAt,
            widgets);
    }
}
