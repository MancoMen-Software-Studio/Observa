using System;
using System.Threading;
using System.Threading.Tasks;

namespace Observa.Infrastructure.RealTime;

/// <summary>
/// Contrato para el servicio de notificaciones en tiempo real de dashboards.
/// </summary>
public interface IDashboardNotificationService
{
    Task NotifyDashboardUpdatedAsync(Guid dashboardId, CancellationToken cancellationToken = default);

    Task NotifyWidgetAddedAsync(Guid dashboardId, Guid widgetId, CancellationToken cancellationToken = default);

    Task NotifyWidgetRemovedAsync(Guid dashboardId, Guid widgetId, CancellationToken cancellationToken = default);

    Task NotifyAlertTriggeredAsync(Guid dashboardId, Guid alertId, string severity, CancellationToken cancellationToken = default);
}
