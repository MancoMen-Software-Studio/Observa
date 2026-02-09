using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Observa.Infrastructure.RealTime;

/// <summary>
/// Servicio que envia notificaciones en tiempo real a traves de SignalR.
/// </summary>
public sealed class DashboardNotificationService : IDashboardNotificationService
{
    private readonly IHubContext<DashboardHub> _hubContext;

    public DashboardNotificationService(IHubContext<DashboardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyDashboardUpdatedAsync(Guid dashboardId, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
            .Group(dashboardId.ToString())
            .SendAsync("DashboardUpdated", dashboardId, cancellationToken);
    }

    public async Task NotifyWidgetAddedAsync(
        Guid dashboardId,
        Guid widgetId,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
            .Group(dashboardId.ToString())
            .SendAsync("WidgetAdded", new { DashboardId = dashboardId, WidgetId = widgetId }, cancellationToken);
    }

    public async Task NotifyWidgetRemovedAsync(
        Guid dashboardId,
        Guid widgetId,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
            .Group(dashboardId.ToString())
            .SendAsync("WidgetRemoved", new { DashboardId = dashboardId, WidgetId = widgetId }, cancellationToken);
    }

    public async Task NotifyAlertTriggeredAsync(
        Guid dashboardId,
        Guid alertId,
        string severity,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
            .Group(dashboardId.ToString())
            .SendAsync("AlertTriggered", new { DashboardId = dashboardId, AlertId = alertId, Severity = severity }, cancellationToken);
    }
}
