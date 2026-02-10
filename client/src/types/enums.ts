import type { DashboardStatus, WidgetType, DataSourceType, AlertSeverity, RefreshInterval } from './api';

export const DASHBOARD_STATUS_LABELS: Record<DashboardStatus, string> = {
  Draft: 'Borrador',
  Published: 'Publicado',
  Archived: 'Archivado',
};

export const WIDGET_TYPE_LABELS: Record<WidgetType, string> = {
  LineChart: 'Grafico de lineas',
  BarChart: 'Grafico de barras',
  PieChart: 'Grafico circular',
  HeatMap: 'Mapa de calor',
  ScatterPlot: 'Diagrama de dispersion',
  Gauge: 'Indicador',
  Table: 'Tabla',
  KpiCard: 'Tarjeta KPI',
  Map: 'Mapa',
};

export const WIDGET_TYPE_VALUES: Record<WidgetType, number> = {
  LineChart: 0,
  BarChart: 1,
  PieChart: 2,
  HeatMap: 3,
  ScatterPlot: 4,
  Gauge: 5,
  Table: 6,
  KpiCard: 7,
  Map: 8,
};

export const DATA_SOURCE_TYPE_LABELS: Record<DataSourceType, string> = {
  RestApi: 'API REST',
  Database: 'Base de datos',
  WebSocket: 'WebSocket',
  StaticFile: 'Archivo estatico',
  SignalR: 'SignalR',
};

export const DATA_SOURCE_TYPE_VALUES: Record<DataSourceType, number> = {
  RestApi: 0,
  Database: 1,
  WebSocket: 2,
  StaticFile: 3,
  SignalR: 4,
};

export const ALERT_SEVERITY_LABELS: Record<AlertSeverity, string> = {
  Info: 'Informacion',
  Warning: 'Advertencia',
  Critical: 'Critico',
};

export const REFRESH_INTERVAL_LABELS: Record<RefreshInterval, string> = {
  RealTime: 'Tiempo real',
  FiveSeconds: '5 segundos',
  ThirtySeconds: '30 segundos',
  OneMinute: '1 minuto',
  FiveMinutes: '5 minutos',
  FifteenMinutes: '15 minutos',
};

export const REFRESH_INTERVAL_VALUES: Record<RefreshInterval, number> = {
  RealTime: 0,
  FiveSeconds: 5,
  ThirtySeconds: 30,
  OneMinute: 60,
  FiveMinutes: 300,
  FifteenMinutes: 900,
};
