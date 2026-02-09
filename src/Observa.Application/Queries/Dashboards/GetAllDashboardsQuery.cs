using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Observa.Application.Abstractions.Messaging;
using Observa.Application.DTOs;
using Observa.Domain.Abstractions;
using Observa.Domain.Repositories;

namespace Observa.Application.Queries.Dashboards;

/// <summary>
/// Consulta para obtener todos los dashboards del sistema.
/// </summary>
public sealed record GetAllDashboardsQuery : IQuery<IReadOnlyCollection<DashboardResponse>>;

/// <summary>
/// Handler que procesa la consulta de todos los dashboards.
/// </summary>
public sealed class GetAllDashboardsQueryHandler
    : IQueryHandler<GetAllDashboardsQuery, IReadOnlyCollection<DashboardResponse>>
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetAllDashboardsQueryHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<Result<IReadOnlyCollection<DashboardResponse>>> Handle(
        GetAllDashboardsQuery request,
        CancellationToken cancellationToken)
    {
        var dashboards = await _dashboardRepository.GetAllAsync(cancellationToken);

        var response = dashboards
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

        return Result<IReadOnlyCollection<DashboardResponse>>.Success(response);
    }
}
