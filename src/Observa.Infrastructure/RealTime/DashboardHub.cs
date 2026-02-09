using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Observa.Infrastructure.RealTime;

/// <summary>
/// Hub de SignalR para notificaciones en tiempo real de dashboards.
/// </summary>
public sealed class DashboardHub : Hub
{
    public async Task JoinDashboardGroup(string dashboardId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, dashboardId);
    }

    public async Task LeaveDashboardGroup(string dashboardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, dashboardId);
    }
}
