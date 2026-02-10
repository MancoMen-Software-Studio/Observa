/// Configuracion global para la suite de pruebas de carga k6.
/// Define BASE_URL, thresholds (SLOs) y metricas custom.

import { Counter, Rate, Trend } from "k6/metrics";

/// URL base del API, configurable via variable de entorno
export const BASE_URL =
  __ENV.K6_BASE_URL || "http://localhost:5268";

/// Thresholds globales que representan los SLOs del sistema
export const defaultThresholds = {
  http_req_duration: ["p(95)<500", "p(99)<1000"],
  http_req_failed: ["rate<0.01"],
  dashboard_created: ["rate>0.95"],
  widget_added: ["rate>0.95"],
};

/// Thresholds relajados para pruebas de estres
export const stressThresholds = {
  http_req_duration: ["p(95)<1500", "p(99)<2000"],
  http_req_failed: ["rate<0.05"],
  dashboard_created: ["rate>0.90"],
  widget_added: ["rate>0.90"],
};

/// Thresholds relajados para pruebas de spike
export const spikeThresholds = {
  http_req_duration: ["p(95)<2000", "p(99)<3000"],
  http_req_failed: ["rate<0.10"],
  dashboard_created: ["rate>0.80"],
  widget_added: ["rate>0.80"],
};

/// Metricas custom para rastrear operaciones de negocio
export const dashboardCreated = new Rate("dashboard_created");
export const widgetAdded = new Rate("widget_added");
export const datasourceCreated = new Rate("datasource_created");
export const dashboardPublished = new Rate("dashboard_published");
export const dashboardArchived = new Rate("dashboard_archived");

/// Metricas de latencia por operacion
export const createDashboardDuration = new Trend(
  "create_dashboard_duration",
  true
);
export const getDashboardsDuration = new Trend(
  "get_dashboards_duration",
  true
);
export const getDashboardByIdDuration = new Trend(
  "get_dashboard_by_id_duration",
  true
);
export const addWidgetDuration = new Trend("add_widget_duration", true);
export const createDatasourceDuration = new Trend(
  "create_datasource_duration",
  true
);

/// Contador de errores por tipo
export const errorCount = new Counter("error_count");

/// Tags comunes para agrupar metricas
export const tags = {
  health: { name: "GET /health" },
  listDashboards: { name: "GET /api/dashboards" },
  getDashboard: { name: "GET /api/dashboards/{id}" },
  createDashboard: { name: "POST /api/dashboards" },
  updateTitle: { name: "PUT /api/dashboards/{id}/title" },
  publishDashboard: { name: "POST /api/dashboards/{id}/publish" },
  archiveDashboard: { name: "POST /api/dashboards/{id}/archive" },
  addWidget: { name: "POST /api/dashboards/{id}/widgets" },
  removeWidget: { name: "DELETE /api/dashboards/{id}/widgets/{widgetId}" },
  createDatasource: { name: "POST /api/datasources" },
};

/// Headers JSON por defecto
export const jsonHeaders = {
  "Content-Type": "application/json",
  Accept: "application/json",
};
