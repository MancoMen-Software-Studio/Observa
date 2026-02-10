/// Prueba de carga normal (load test).
/// Rampa: 0->50 VUs en 1min, sostener 50 VUs por 3min, bajar en 1min.
/// Mix de operaciones: 60% lecturas, 30% escrituras, 10% acciones.

import { sleep } from "k6";
import { defaultThresholds } from "../helpers/config.js";
import {
  healthCheck,
  createDataSource,
  createDashboard,
  addWidget,
  getDashboards,
  getDashboard,
  updateDashboardTitle,
  publishDashboard,
  archiveDashboard,
} from "../helpers/api.js";
import { SharedArray } from "k6/data";

export const options = {
  stages: [
    { duration: "1m", target: 50 },
    { duration: "3m", target: 50 },
    { duration: "1m", target: 0 },
  ],
  thresholds: defaultThresholds,
  tags: { scenario: "load" },
};

/// Estado compartido entre iteraciones del mismo VU
let vuDashboardIds = [];
let vuDataSourceId = null;

export function setup() {
  /// Crear datasource de base para uso en lecturas
  const dsId = createDataSource(
    "Load Test DataSource",
    0,
    "https://api.example.com/load"
  );

  /// Crear dashboards iniciales para las lecturas
  const dashboards = [];
  for (let i = 0; i < 10; i++) {
    const dashId = createDashboard(
      `Load Setup ${i}`,
      "Dashboard pre-creado para test de carga"
    );
    if (dashId && dsId) {
      addWidget(dashId, dsId, {
        title: `Setup Widget ${i}`,
        type: i % 9,
      });
      dashboards.push(dashId);
    }
  }

  return { dashboardIds: dashboards, dataSourceId: dsId };
}

export default function (data) {
  const rand = Math.random();

  if (rand < 0.6) {
    /// 60% lecturas
    readOperations(data);
  } else if (rand < 0.9) {
    /// 30% escrituras
    writeOperations(data);
  } else {
    /// 10% acciones (publicar/archivar)
    actionOperations(data);
  }

  sleep(Math.random() * 2 + 0.5);
}

/// Operaciones de lectura: listar y obtener dashboards
function readOperations(data) {
  getDashboards();

  if (data.dashboardIds && data.dashboardIds.length > 0) {
    const idx = Math.floor(Math.random() * data.dashboardIds.length);
    getDashboard(data.dashboardIds[idx]);
  }
}

/// Operaciones de escritura: crear dashboards, widgets, actualizar titulos
function writeOperations(data) {
  const op = Math.random();

  if (op < 0.4) {
    /// Crear dashboard
    const dashId = createDashboard(
      `Load ${__VU}-${__ITER}-${Date.now()}`,
      "Dashboard creado durante test de carga"
    );
    if (dashId && data.dataSourceId) {
      addWidget(dashId, data.dataSourceId, {
        title: `Load Widget ${Date.now()}`,
        type: Math.floor(Math.random() * 9),
      });
    }
  } else if (op < 0.7) {
    /// Agregar widget a dashboard existente
    if (data.dashboardIds && data.dashboardIds.length > 0 && data.dataSourceId) {
      const idx = Math.floor(Math.random() * data.dashboardIds.length);
      addWidget(data.dashboardIds[idx], data.dataSourceId, {
        title: `Widget ${Date.now()}`,
        type: Math.floor(Math.random() * 9),
        column: Math.floor(Math.random() * 12),
        row: Math.floor(Math.random() * 12),
      });
    }
  } else {
    /// Actualizar titulo
    if (data.dashboardIds && data.dashboardIds.length > 0) {
      const idx = Math.floor(Math.random() * data.dashboardIds.length);
      updateDashboardTitle(
        data.dashboardIds[idx],
        `Actualizado ${Date.now()}`
      );
    }
  }
}

/// Operaciones de accion: publicar y archivar dashboards
function actionOperations(data) {
  /// Crear un dashboard nuevo, agregarle widget, publicar y archivar
  const dsId = data.dataSourceId;
  if (!dsId) { return; }

  const dashId = createDashboard(
    `Action ${__VU}-${Date.now()}`,
    "Dashboard para accion"
  );
  if (!dashId) { return; }

  addWidget(dashId, dsId, {
    title: `Action Widget ${Date.now()}`,
    type: Math.floor(Math.random() * 9),
  });

  publishDashboard(dashId);
  sleep(0.5);
  archiveDashboard(dashId);
}
