/// Prueba de resistencia (soak test).
/// 50 VUs constantes durante 30 minutos.
/// Detecta memory leaks, connection pool exhaustion y degradacion gradual.
/// Si el p95 crece con el tiempo, hay un problema de estabilidad.

import { sleep } from "k6";
import { defaultThresholds } from "../helpers/config.js";
import {
  createDataSource,
  createDashboard,
  addWidget,
  getDashboards,
  getDashboard,
  updateDashboardTitle,
  publishDashboard,
  archiveDashboard,
  removeWidget,
} from "../helpers/api.js";

export const options = {
  stages: [
    { duration: "2m", target: 50 },
    { duration: "26m", target: 50 },
    { duration: "2m", target: 0 },
  ],
  thresholds: defaultThresholds,
  tags: { scenario: "soak" },
};

export function setup() {
  const dsId = createDataSource(
    "Soak Test DataSource",
    0,
    "https://api.example.com/soak"
  );

  const dashboards = [];
  for (let i = 0; i < 10; i++) {
    const dashId = createDashboard(
      `Soak Setup ${i}`,
      "Dashboard para test de resistencia"
    );
    if (dashId && dsId) {
      addWidget(dashId, dsId, { title: `Setup ${i}`, type: i % 9 });
      dashboards.push(dashId);
    }
  }

  return { dashboardIds: dashboards, dataSourceId: dsId };
}

export default function (data) {
  const rand = Math.random();

  if (rand < 0.55) {
    /// 55% lecturas
    getDashboards();
    if (data.dashboardIds && data.dashboardIds.length > 0) {
      const idx = Math.floor(Math.random() * data.dashboardIds.length);
      getDashboard(data.dashboardIds[idx]);
    }
  } else if (rand < 0.80) {
    /// 25% escrituras
    const dashId = createDashboard(
      `Soak ${__VU}-${__ITER}`,
      "Dashboard de soak test"
    );
    if (dashId && data.dataSourceId) {
      addWidget(dashId, data.dataSourceId, {
        title: `Soak Widget ${Date.now()}`,
        type: Math.floor(Math.random() * 9),
        column: Math.floor(Math.random() * 12),
        row: Math.floor(Math.random() * 12),
      });
    }
  } else if (rand < 0.90) {
    /// 10% actualizaciones
    if (data.dashboardIds && data.dashboardIds.length > 0) {
      const idx = Math.floor(Math.random() * data.dashboardIds.length);
      updateDashboardTitle(
        data.dashboardIds[idx],
        `Soak Updated ${Date.now()}`
      );
    }
  } else {
    /// 10% flujo completo (crear, publicar, archivar)
    const dsId = data.dataSourceId;
    if (!dsId) { return; }

    const dashId = createDashboard(
      `SoakFull ${__VU}-${Date.now()}`,
      "Flujo completo soak"
    );
    if (!dashId) { return; }

    const widgetId = addWidget(dashId, dsId, {
      title: `Soak Full Widget`,
      type: Math.floor(Math.random() * 9),
    });

    publishDashboard(dashId);
    sleep(0.5);
    archiveDashboard(dashId);

    if (widgetId) {
      removeWidget(dashId, widgetId);
    }
  }

  sleep(Math.random() * 3 + 1);
}
