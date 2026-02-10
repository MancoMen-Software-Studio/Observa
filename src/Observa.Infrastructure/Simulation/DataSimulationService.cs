using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;
using Observa.Infrastructure.RealTime;

namespace Observa.Infrastructure.Simulation;

/// <summary>
/// Servicio en segundo plano que genera datos simulados y los empuja via SignalR cada 500ms.
/// </summary>
public sealed class DataSimulationService : BackgroundService
{
    private static readonly Action<ILogger, Exception?> s_logStarted =
        LoggerMessage.Define(LogLevel.Information, new EventId(1, "SimulationStarted"), "Servicio de simulacion de datos iniciado");

    private static readonly Action<ILogger, Exception?> s_logStopped =
        LoggerMessage.Define(LogLevel.Information, new EventId(2, "SimulationStopped"), "Servicio de simulacion de datos detenido");

    private static readonly Action<ILogger, Exception?> s_logCycleError =
        LoggerMessage.Define(LogLevel.Error, new EventId(3, "SimulationCycleError"), "Error en el ciclo de simulacion de datos");

    private static readonly Action<ILogger, int, Exception?> s_logRefreshed =
        LoggerMessage.Define<int>(LogLevel.Debug, new EventId(4, "DashboardsRefreshed"), "Dashboards refrescados: {Count} publicados");

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly ILogger<DataSimulationService> _logger;

    private readonly TimeSpan _dataInterval = TimeSpan.FromMilliseconds(500);
    private readonly TimeSpan _refreshInterval = TimeSpan.FromSeconds(30);

    private List<DashboardSnapshot> _dashboards = [];
    private DateTime _lastRefresh = DateTime.MinValue;

    public DataSimulationService(
        IServiceScopeFactory scopeFactory,
        IHubContext<DashboardHub> hubContext,
        ILogger<DataSimulationService> logger)
    {
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        s_logStarted(_logger, null);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (DateTime.UtcNow - _lastRefresh > _refreshInterval)
                {
                    await RefreshDashboardsAsync(stoppingToken);
                }

                await PushDataAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                s_logCycleError(_logger, ex);
            }

            await Task.Delay(_dataInterval, stoppingToken);
        }

        s_logStopped(_logger, null);
    }

    /// <summary>
    /// Refresca la lista de dashboards publicados desde la base de datos.
    /// </summary>
    private async Task RefreshDashboardsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDashboardRepository>();

        var published = await repository.GetByStatusAsync(DashboardStatus.Published, cancellationToken);

        _dashboards = published
            .Select(d => new DashboardSnapshot(
                d.Id,
                d.Widgets.Select(w => new WidgetSnapshot(w.Id, w.Type)).ToList()))
            .ToList();

        _lastRefresh = DateTime.UtcNow;

        s_logRefreshed(_logger, _dashboards.Count, null);
    }

    /// <summary>
    /// Genera y envia datos simulados a cada grupo de dashboard.
    /// </summary>
    private async Task PushDataAsync(CancellationToken cancellationToken)
    {
        foreach (var dashboard in _dashboards)
        {
            if (dashboard.Widgets.Count == 0)
            {
                continue;
            }

            var widgetData = new Dictionary<string, object>(dashboard.Widgets.Count);

            foreach (var widget in dashboard.Widgets)
            {
                widgetData[widget.Id.ToString()] = WidgetDataGenerator.Generate(widget.Id, widget.Type);
            }

            var payload = new
            {
                dashboardId = dashboard.Id.ToString(),
                widgets = widgetData
            };

            await _hubContext.Clients
                .Group(dashboard.Id.ToString())
                .SendAsync("WidgetDataUpdated", payload, cancellationToken);
        }
    }

    /// <summary>
    /// Snapshot inmutable de un dashboard para desacoplar de EF Core.
    /// </summary>
    private sealed record DashboardSnapshot(Guid Id, List<WidgetSnapshot> Widgets);

    /// <summary>
    /// Snapshot inmutable de un widget para desacoplar de EF Core.
    /// </summary>
    private sealed record WidgetSnapshot(Guid Id, WidgetType Type);
}
