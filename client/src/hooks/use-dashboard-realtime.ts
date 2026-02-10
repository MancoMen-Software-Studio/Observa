import { useEffect } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { getConnection } from '@/lib/signalr';
import { HubConnectionState } from '@microsoft/signalr';
import { dashboardKeys } from '@/hooks/queries/use-dashboards';
import { useAlertStore } from '@/stores/alert-store';
import { useWidgetDataStore } from '@/stores/widget-data-store';
import type { AlertResponse, WidgetResponse } from '@/types/api';

interface WidgetDataPayload {
  dashboardId: string;
  widgets: Record<string, unknown>;
}

export function useDashboardRealtime(
  dashboardId: string | undefined,
  widgets?: WidgetResponse[],
) {
  const queryClient = useQueryClient();
  const addAlert = useAlertStore((s) => s.addAlert);
  const setBatchWidgetData = useWidgetDataStore((s) => s.setBatchWidgetData);
  const appendLineChartPoint = useWidgetDataStore((s) => s.appendLineChartPoint);

  useEffect(() => {
    if (!dashboardId) {
      return;
    }

    const lineChartWidgetIds = new Set(
      (widgets ?? []).filter((w) => w.type === 'LineChart').map((w) => w.id),
    );

    const connection = getConnection();

    function handleDashboardUpdated() {
      queryClient.invalidateQueries({ queryKey: dashboardKeys.detail(dashboardId!) });
      queryClient.invalidateQueries({ queryKey: dashboardKeys.all });
    }

    function handleWidgetAdded() {
      queryClient.invalidateQueries({ queryKey: dashboardKeys.detail(dashboardId!) });
    }

    function handleWidgetRemoved() {
      queryClient.invalidateQueries({ queryKey: dashboardKeys.detail(dashboardId!) });
    }

    function handleAlertTriggered(alert: AlertResponse) {
      addAlert(alert);
    }

    function handleWidgetDataUpdated(payload: WidgetDataPayload) {
      const batchUpdates: Record<string, unknown> = {};

      for (const [widgetId, data] of Object.entries(payload.widgets)) {
        if (lineChartWidgetIds.has(widgetId)) {
          appendLineChartPoint(widgetId, data as { name: string; valor: number; objetivo: number });
        } else {
          batchUpdates[widgetId] = data;
        }
      }

      if (Object.keys(batchUpdates).length > 0) {
        setBatchWidgetData(batchUpdates);
      }
    }

    connection.on('DashboardUpdated', handleDashboardUpdated);
    connection.on('WidgetAdded', handleWidgetAdded);
    connection.on('WidgetRemoved', handleWidgetRemoved);
    connection.on('AlertTriggered', handleAlertTriggered);
    connection.on('WidgetDataUpdated', handleWidgetDataUpdated);

    if (connection.state === HubConnectionState.Connected) {
      connection.invoke('JoinDashboardGroup', dashboardId);
    } else {
      const onConnected = () => {
        connection.invoke('JoinDashboardGroup', dashboardId);
        connection.off('reconnected', onConnected);
      };
      connection.on('reconnected' as never, onConnected);
    }

    return () => {
      connection.off('DashboardUpdated', handleDashboardUpdated);
      connection.off('WidgetAdded', handleWidgetAdded);
      connection.off('WidgetRemoved', handleWidgetRemoved);
      connection.off('AlertTriggered', handleAlertTriggered);
      connection.off('WidgetDataUpdated', handleWidgetDataUpdated);

      if (connection.state === HubConnectionState.Connected) {
        connection.invoke('LeaveDashboardGroup', dashboardId);
      }
    };
  }, [dashboardId, widgets, queryClient, addAlert, setBatchWidgetData, appendLineChartPoint]);
}
