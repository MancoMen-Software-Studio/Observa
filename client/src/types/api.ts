export interface DashboardResponse {
  id: string;
  title: string;
  description: string;
  status: DashboardStatus;
  createdAt: string;
  updatedAt: string;
  widgets: WidgetResponse[];
}

export interface WidgetResponse {
  id: string;
  title: string;
  type: WidgetType;
  column: number;
  row: number;
  width: number;
  height: number;
  dataSourceId: string;
  refreshInterval: RefreshInterval;
  createdAt: string;
}

export interface DataSourceResponse {
  id: string;
  name: string;
  type: DataSourceType;
  isActive: boolean;
  createdAt: string;
  lastSyncAt: string | null;
}

export interface AlertResponse {
  id: string;
  dashboardId: string;
  metricName: string;
  severity: AlertSeverity;
  message: string;
  isAcknowledged: boolean;
  triggeredAt: string;
  acknowledgedAt: string | null;
}

export interface CreateDashboardRequest {
  title: string;
  description: string;
}

export interface UpdateTitleRequest {
  newTitle: string;
}

export interface AddWidgetRequest {
  dashboardId: string;
  title: string;
  type: number;
  column: number;
  row: number;
  width: number;
  height: number;
  dataSourceId: string;
  refreshInterval: number;
}

export interface CreateDataSourceRequest {
  name: string;
  type: number;
  connectionString: string;
}

export interface ApiErrorResponse {
  code: string;
  description: string;
}

export interface CreatedResponse {
  id: string;
}

export interface HealthResponse {
  status: string;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export type DashboardStatus = 'Draft' | 'Published' | 'Archived';
export type WidgetType =
  | 'LineChart'
  | 'BarChart'
  | 'PieChart'
  | 'HeatMap'
  | 'ScatterPlot'
  | 'Gauge'
  | 'Table'
  | 'KpiCard'
  | 'Map';
export type DataSourceType = 'RestApi' | 'Database' | 'WebSocket' | 'StaticFile' | 'SignalR';
export type AlertSeverity = 'Info' | 'Warning' | 'Critical';
export type RefreshInterval =
  | 'RealTime'
  | 'FiveSeconds'
  | 'ThirtySeconds'
  | 'OneMinute'
  | 'FiveMinutes'
  | 'FifteenMinutes';
