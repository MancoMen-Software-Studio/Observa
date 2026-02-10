import { useMemo } from 'react';
import { useWidgetDataStore } from '@/stores/widget-data-store';
import type { WidgetType } from '@/types/api';
import {
  generateLineData,
  generateBarData,
  generatePieData,
  generateHeatmapData,
  generateScatterData,
  generateGaugeData,
  generateTableData,
  generateKpiData,
  generateMapData,
} from '@/lib/mock-data';

function getMockData(widgetId: string, widgetType: WidgetType): unknown {
  switch (widgetType) {
    case 'LineChart':
      return generateLineData(widgetId);
    case 'BarChart':
      return generateBarData(widgetId);
    case 'PieChart':
      return generatePieData(widgetId);
    case 'HeatMap':
      return generateHeatmapData(widgetId);
    case 'ScatterPlot':
      return generateScatterData(widgetId);
    case 'Gauge':
      return generateGaugeData(widgetId);
    case 'Table':
      return generateTableData(widgetId);
    case 'KpiCard':
      return generateKpiData(widgetId);
    case 'Map':
      return generateMapData(widgetId);
  }
}

export function useWidgetData<T>(widgetId: string, widgetType: WidgetType): T {
  const liveData = useWidgetDataStore((s) => s.data[widgetId]);

  const fallback = useMemo(
    () => getMockData(widgetId, widgetType),
    [widgetId, widgetType],
  );

  return (liveData ?? fallback) as T;
}
