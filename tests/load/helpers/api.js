/// Funciones helper para interactuar con cada endpoint del API de Observa.
/// Cada funcion realiza check() del status code y retorna el body parseado.

import http from "k6/http";
import { check, group } from "k6";
import {
  BASE_URL,
  jsonHeaders,
  tags,
  dashboardCreated,
  widgetAdded,
  datasourceCreated,
  dashboardPublished,
  dashboardArchived,
  createDashboardDuration,
  getDashboardsDuration,
  getDashboardByIdDuration,
  addWidgetDuration,
  createDatasourceDuration,
  errorCount,
} from "./config.js";

/// Verifica el estado de salud del API
export function healthCheck() {
  const res = http.get(`${BASE_URL}/health`, {
    tags: tags.health,
  });
  check(res, {
    "health: status 200": (r) => r.status === 200,
  });
  return res;
}

/// Obtiene la lista paginada de dashboards
export function getDashboards(page, pageSize) {
  const p = page || 1;
  const ps = pageSize || 20;
  const res = http.get(
    `${BASE_URL}/api/dashboards?page=${p}&pageSize=${ps}`,
    {
      headers: jsonHeaders,
      tags: tags.listDashboards,
    }
  );
  getDashboardsDuration.add(res.timings.duration);
  const ok = check(res, {
    "getDashboards: status 200": (r) => r.status === 200,
    "getDashboards: has items": (r) => r.json("items") !== undefined,
  });
  if (!ok) {
    errorCount.add(1, tags.listDashboards);
  }
  return res.json();
}

/// Obtiene un dashboard por su ID
export function getDashboard(id) {
  const res = http.get(`${BASE_URL}/api/dashboards/${id}`, {
    headers: jsonHeaders,
    tags: tags.getDashboard,
  });
  getDashboardByIdDuration.add(res.timings.duration);
  const ok = check(res, {
    "getDashboard: status 200": (r) => r.status === 200,
    "getDashboard: has id": (r) => r.json("id") !== undefined,
  });
  if (!ok) {
    errorCount.add(1, tags.getDashboard);
  }
  return res.json();
}

/// Crea un nuevo dashboard con titulo y descripcion opcionales
export function createDashboard(title, description) {
  const payload = JSON.stringify({
    title: title || `Dashboard ${Date.now()}`,
    description: description || "Creado por prueba de carga k6",
  });
  const res = http.post(`${BASE_URL}/api/dashboards`, payload, {
    headers: jsonHeaders,
    tags: tags.createDashboard,
  });
  createDashboardDuration.add(res.timings.duration);
  const ok = check(res, {
    "createDashboard: status 201": (r) => r.status === 201,
    "createDashboard: has id": (r) => r.json("id") !== undefined,
  });
  dashboardCreated.add(ok);
  if (!ok) {
    errorCount.add(1, tags.createDashboard);
    return null;
  }
  return res.json("id");
}

/// Actualiza el titulo de un dashboard existente
export function updateDashboardTitle(id, newTitle) {
  const payload = JSON.stringify({
    newTitle: newTitle || `Titulo actualizado ${Date.now()}`,
  });
  const res = http.put(`${BASE_URL}/api/dashboards/${id}/title`, payload, {
    headers: jsonHeaders,
    tags: tags.updateTitle,
  });
  const ok = check(res, {
    "updateTitle: status 204": (r) => r.status === 204,
  });
  if (!ok) {
    errorCount.add(1, tags.updateTitle);
  }
  return ok;
}

/// Publica un dashboard (requiere al menos 1 widget)
export function publishDashboard(id) {
  const res = http.post(
    `${BASE_URL}/api/dashboards/${id}/publish`,
    null,
    {
      headers: jsonHeaders,
      tags: tags.publishDashboard,
    }
  );
  const ok = check(res, {
    "publishDashboard: status 204": (r) => r.status === 204,
  });
  dashboardPublished.add(ok);
  if (!ok) {
    errorCount.add(1, tags.publishDashboard);
  }
  return ok;
}

/// Archiva un dashboard
export function archiveDashboard(id) {
  const res = http.post(
    `${BASE_URL}/api/dashboards/${id}/archive`,
    null,
    {
      headers: jsonHeaders,
      tags: tags.archiveDashboard,
    }
  );
  const ok = check(res, {
    "archiveDashboard: status 204": (r) => r.status === 204,
  });
  dashboardArchived.add(ok);
  if (!ok) {
    errorCount.add(1, tags.archiveDashboard);
  }
  return ok;
}

/// Agrega un widget a un dashboard
export function addWidget(dashboardId, dataSourceId, options) {
  const opts = options || {};
  const payload = JSON.stringify({
    title: opts.title || `Widget ${Date.now()}`,
    type: opts.type !== undefined ? opts.type : 0,
    column: opts.column || 0,
    row: opts.row || 0,
    width: opts.width || 4,
    height: opts.height || 3,
    dataSourceId: dataSourceId,
    refreshInterval: opts.refreshInterval !== undefined
      ? opts.refreshInterval
      : 30,
  });
  const res = http.post(
    `${BASE_URL}/api/dashboards/${dashboardId}/widgets`,
    payload,
    {
      headers: jsonHeaders,
      tags: tags.addWidget,
    }
  );
  addWidgetDuration.add(res.timings.duration);
  const ok = check(res, {
    "addWidget: status 201": (r) => r.status === 201,
    "addWidget: has id": (r) => r.json("id") !== undefined,
  });
  widgetAdded.add(ok);
  if (!ok) {
    errorCount.add(1, tags.addWidget);
    return null;
  }
  return res.json("id");
}

/// Elimina un widget de un dashboard
export function removeWidget(dashboardId, widgetId) {
  const res = http.del(
    `${BASE_URL}/api/dashboards/${dashboardId}/widgets/${widgetId}`,
    null,
    {
      headers: jsonHeaders,
      tags: tags.removeWidget,
    }
  );
  const ok = check(res, {
    "removeWidget: status 204": (r) => r.status === 204,
  });
  if (!ok) {
    errorCount.add(1, tags.removeWidget);
  }
  return ok;
}

/// Crea un nuevo datasource
export function createDataSource(name, type, connectionString) {
  const payload = JSON.stringify({
    name: name || `DataSource ${Date.now()}`,
    type: type !== undefined ? type : 0,
    connectionString: connectionString || "https://api.example.com/data",
  });
  const res = http.post(`${BASE_URL}/api/datasources`, payload, {
    headers: jsonHeaders,
    tags: tags.createDatasource,
  });
  createDatasourceDuration.add(res.timings.duration);
  const ok = check(res, {
    "createDataSource: status 201": (r) => r.status === 201,
    "createDataSource: has id": (r) => r.json("id") !== undefined,
  });
  datasourceCreated.add(ok);
  if (!ok) {
    errorCount.add(1, tags.createDatasource);
    return null;
  }
  return res.json("id");
}

/// Ejecuta el flujo completo de creacion:
/// datasource -> dashboard -> widget -> publicar
export function fullCreationFlow() {
  let result = { dashboardId: null, widgetId: null, dataSourceId: null };

  const dsId = createDataSource();
  if (!dsId) { return result; }
  result.dataSourceId = dsId;

  const dashId = createDashboard();
  if (!dashId) { return result; }
  result.dashboardId = dashId;

  const widgetId = addWidget(dashId, dsId);
  if (!widgetId) { return result; }
  result.widgetId = widgetId;

  publishDashboard(dashId);
  return result;
}

/// Prepara datos iniciales para los tests.
/// Crea N dashboards con widgets y datasources.
export function setupData(count) {
  const n = count || 5;
  const dashboards = [];
  const dataSources = [];

  for (let i = 0; i < n; i++) {
    const dsId = createDataSource(
      `Setup DataSource ${i}`,
      0,
      `https://api.example.com/data/${i}`
    );
    if (dsId) {
      dataSources.push(dsId);
    }
  }

  for (let i = 0; i < n; i++) {
    const dashId = createDashboard(
      `Setup Dashboard ${i}`,
      `Dashboard de prueba de carga #${i}`
    );
    if (dashId && dataSources.length > 0) {
      const dsId = dataSources[i % dataSources.length];
      const widgetId = addWidget(dashId, dsId, {
        title: `Widget setup ${i}`,
        type: i % 9,
        column: 0,
        row: 0,
        width: 4,
        height: 3,
      });
      dashboards.push({
        id: dashId,
        widgetId: widgetId,
        dataSourceId: dsId,
      });
    }
  }

  return { dashboards, dataSources };
}
