/// Prueba de pico (spike test).
/// 10 VUs constantes -> spike instantaneo a 1000 VUs por 30s -> vuelta a 10 VUs.
/// Evalua como reacciona el sistema ante picos subitos y mide tiempo de recuperacion.

import { sleep } from "k6";
import { spikeThresholds } from "../helpers/config.js";
import {
  createDataSource,
  createDashboard,
  addWidget,
  getDashboards,
  getDashboard,
  updateDashboardTitle,
  publishDashboard,
} from "../helpers/api.js";

export const options = {
  stages: [
    { duration: "30s", target: 10 },
    { duration: "10s", target: 1000 },
    { duration: "30s", target: 1000 },
    { duration: "10s", target: 10 },
    { duration: "1m", target: 10 },
    { duration: "10s", target: 0 },
  ],
  thresholds: spikeThresholds,
  tags: { scenario: "spike" },
};

export function setup() {
  const dsId = createDataSource(
    "Spike Test DataSource",
    0,
    "https://api.example.com/spike"
  );

  const dashboards = [];
  for (let i = 0; i < 5; i++) {
    const dashId = createDashboard(
      `Spike Setup ${i}`,
      "Dashboard para test de pico"
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

  if (rand < 0.6) {
    /// Lecturas durante el spike
    getDashboards();
    if (data.dashboardIds && data.dashboardIds.length > 0) {
      const idx = Math.floor(Math.random() * data.dashboardIds.length);
      getDashboard(data.dashboardIds[idx]);
    }
  } else if (rand < 0.85) {
    /// Escrituras
    const dashId = createDashboard(
      `Spike ${__VU}-${Date.now()}`,
      "Dashboard de spike"
    );
    if (dashId && data.dataSourceId) {
      addWidget(dashId, data.dataSourceId, {
        title: `Spike Widget`,
        type: Math.floor(Math.random() * 9),
      });
    }
  } else {
    /// Flujo de publicacion
    const dsId = data.dataSourceId;
    if (!dsId) { return; }

    const dashId = createDashboard(
      `SpikePub ${__VU}-${Date.now()}`,
      "Publicacion durante spike"
    );
    if (!dashId) { return; }

    addWidget(dashId, dsId, {
      title: `Spike Pub Widget`,
      type: Math.floor(Math.random() * 9),
    });
    publishDashboard(dashId);
    updateDashboardTitle(dashId, `Spiked ${Date.now()}`);
  }

  sleep(Math.random() * 0.5 + 0.2);
}
