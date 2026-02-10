using System;
using System.Threading;
using System.Threading.Tasks;

namespace Observa.Application.Abstractions.Notifications;

/// <summary>
/// Contrato para el servicio de notificaciones en tiempo real de dashboards.
/// Definido en Application para respetar Dependency Inversion.
/// </summary>
public interface IDashboardNotificationService
{
    /// <summary>
    /// Notifica a todos los clientes que la lista global de dashboards cambio.
    /// </summary>
    Task NotifyDashboardListChangedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Notifica al grupo de un dashboard que fue actualizado.
    /// </summary>
    Task NotifyDashboardUpdatedAsync(Guid dashboardId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Notifica al grupo de un dashboard que se agrego un widget.
    /// </summary>
    Task NotifyWidgetAddedAsync(Guid dashboardId, Guid widgetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Notifica al grupo de un dashboard que se elimino un widget.
    /// </summary>
    Task NotifyWidgetRemovedAsync(Guid dashboardId, Guid widgetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Notifica al grupo de un dashboard que se disparo una alerta.
    /// </summary>
    Task NotifyAlertTriggeredAsync(Guid dashboardId, Guid alertId, string severity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Notifica al grupo de un dashboard con datos actualizados de sus widgets.
    /// </summary>
    Task NotifyWidgetDataUpdatedAsync(Guid dashboardId, object widgetData, CancellationToken cancellationToken = default);
}
