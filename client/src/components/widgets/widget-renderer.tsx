import type { WidgetType } from '@/types/api';
import { LineChartWidget } from './charts/line-chart-widget';
import { BarChartWidget } from './charts/bar-chart-widget';
import { PieChartWidget } from './charts/pie-chart-widget';
import { HeatmapWidget } from './charts/heatmap-widget';
import { ScatterPlotWidget } from './charts/scatter-plot-widget';
import { GaugeWidget } from './charts/gauge-widget';
import { TableWidget } from './charts/table-widget';
import { KpiCardWidget } from './charts/kpi-card-widget';
import { MapWidget } from './charts/map-widget';
import { NoDataState } from './shared/no-data-state';

interface WidgetRendererProps {
  type: WidgetType;
  widgetId: string;
}

const renderers: Record<WidgetType, React.ComponentType<{ widgetId: string }>> = {
  LineChart: LineChartWidget,
  BarChart: BarChartWidget,
  PieChart: PieChartWidget,
  HeatMap: HeatmapWidget,
  ScatterPlot: ScatterPlotWidget,
  Gauge: GaugeWidget,
  Table: TableWidget,
  KpiCard: KpiCardWidget,
  Map: MapWidget,
};

export function WidgetRenderer({ type, widgetId }: WidgetRendererProps) {
  const Component = renderers[type];

  if (!Component) {
    return <NoDataState />;
  }

  return <Component widgetId={widgetId} />;
}
