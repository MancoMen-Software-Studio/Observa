/// Prueba de estres (stress test).
/// Rampa agresiva: 10->100 VUs (2min) -> 200 VUs (2min) -> 500 VUs (3min) -> 0 (1min).
/// Busca el breaking point donde empiezan errores 5xx, timeouts o degradacion.

import { sleep } from "k6";
import { stressThresholds } from "../helpers/config.js";
import {
  createDataSource,
  createDashboard,
  addWidget,
  getDashboards,
  getDashboard,
  updateDashboardTitle,
  publishDashboard,
  archiveDashboard,
} from "../helpers/api.js";

export const options = {
  stages: [
    { duration: "30s", target: 10 },
    { duration: "2m", target: 100 },
    { duration: "2m", target: 200 },
    { duration: "3m", target: 500 },
    { duration: "1m", target: 0 },
  ],
  thresholds: stressThresholds,
  tags: { scenario: "stress" },
};

export function setup() {
  const dsId = createDataSource(
    "Stress Test DataSource",
    0,
    "https://api.example.com/stress"
  );

  const dashboards = [];
  for (let i = 0; i < 5; i++) {
    const dashId = createDashboard(
      `Stress Setup ${i}`,
      "Dashboard pre-creado para test de estres"
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

  if (rand < 0.5) {
    /// Lecturas - mayor proporcion bajo estres
    getDashboards();
    if (data.dashboardIds && data.dashboardIds.length > 0) {
      const idx = Math.floor(Math.random() * data.dashboardIds.length);
      getDashboard(data.dashboardIds[idx]);
    }
  } else if (rand < 0.8) {
    /// Escrituras
    const dashId = createDashboard(
      `Stress ${__VU}-${Date.now()}`,
      "Dashboard de estres"
    );
    if (dashId && data.dataSourceId) {
      addWidget(dashId, data.dataSourceId, {
        title: `Stress Widget ${Date.now()}`,
        type: Math.floor(Math.random() * 9),
      });
      updateDashboardTitle(dashId, `Stressed ${Date.now()}`);
    }
  } else {
    /// Flujo completo bajo presion
    const dsId = data.dataSourceId;
    if (!dsId) { return; }

    const dashId = createDashboard(
      `StressFull ${__VU}-${Date.now()}`,
      "Flujo completo bajo estres"
    );
    if (!dashId) { return; }

    addWidget(dashId, dsId, {
      title: `StressFull Widget`,
      type: Math.floor(Math.random() * 9),
    });
    publishDashboard(dashId);
    sleep(0.2);
    archiveDashboard(dashId);
  }

  sleep(Math.random() * 1 + 0.3);
}
