/// Prueba de WebSocket/SignalR (websocket test).
/// Abre 100-500 conexiones SignalR simultaneas via WebSocket.
/// Un escenario separado hace mutaciones HTTP mientras las conexiones WS escuchan.
/// Mide: latencia de notificacion, conexiones activas y tasa de errores.

import { sleep } from "k6";
import { stressThresholds } from "../helpers/config.js";
import {
  createDataSource,
  createDashboard,
  addWidget,
  updateDashboardTitle,
} from "../helpers/api.js";
import {
  connectAndJoin,
  wsConnected,
  wsMessageReceived,
  wsNotificationLatency,
  wsErrors,
} from "../helpers/signalr.js";

export const options = {
  scenarios: {
    /// Escenario de conexiones WebSocket
    websocket_listeners: {
      executor: "ramping-vus",
      stages: [
        { duration: "30s", target: 100 },
        { duration: "2m", target: 300 },
        { duration: "2m", target: 500 },
        { duration: "1m", target: 0 },
      ],
      exec: "wsListener",
      tags: { scenario: "websocket_listeners" },
    },
    /// Escenario de mutaciones HTTP que generan notificaciones
    http_mutators: {
      executor: "constant-vus",
      vus: 5,
      duration: "5m30s",
      exec: "httpMutator",
      tags: { scenario: "http_mutators" },
    },
  },
  thresholds: {
    ...stressThresholds,
    ws_connected: ["rate>0.90"],
    ws_errors: ["count<100"],
  },
};

/// Datos compartidos entre escenarios
let sharedDashboardId = null;
let sharedDataSourceId = null;

export function setup() {
  /// Crear datasource y dashboard compartido
  const dsId = createDataSource(
    "WebSocket Test DataSource",
    0,
    "https://api.example.com/ws"
  );

  const dashId = createDashboard(
    "WebSocket Test Dashboard",
    "Dashboard para prueba de WebSocket/SignalR"
  );

  if (dashId && dsId) {
    addWidget(dashId, dsId, {
      title: "WS Test Widget",
      type: 0,
    });
  }

  return { dashboardId: dashId, dataSourceId: dsId };
}

/// VU que abre conexion WebSocket y escucha notificaciones
export function wsListener(data) {
  if (!data.dashboardId) { return; }

  const startTime = Date.now();

  connectAndJoin(
    data.dashboardId,
    30,
    function (msg) {
      /// Medir latencia de notificacion si el mensaje incluye timestamp
      if (msg.arguments && msg.arguments.length > 0) {
        const receivedAt = Date.now();
        wsNotificationLatency.add(receivedAt - startTime);
      }
    }
  );

  sleep(Math.random() * 2 + 1);
}

/// VU que hace mutaciones HTTP para generar eventos en los listeners WS
export function httpMutator(data) {
  if (!data.dashboardId || !data.dataSourceId) { return; }

  const rand = Math.random();

  if (rand < 0.4) {
    /// Agregar widget (genera notificacion al grupo)
    addWidget(data.dashboardId, data.dataSourceId, {
      title: `WS Mutate Widget ${Date.now()}`,
      type: Math.floor(Math.random() * 9),
      column: Math.floor(Math.random() * 12),
      row: Math.floor(Math.random() * 12),
    });
  } else if (rand < 0.7) {
    /// Actualizar titulo (genera notificacion al grupo)
    updateDashboardTitle(
      data.dashboardId,
      `WS Updated ${Date.now()}`
    );
  } else {
    /// Crear nuevo dashboard y agregar widget
    const newDashId = createDashboard(
      `WS Extra ${Date.now()}`,
      "Dashboard extra de WebSocket test"
    );
    if (newDashId) {
      addWidget(newDashId, data.dataSourceId, {
        title: `WS Extra Widget`,
        type: Math.floor(Math.random() * 9),
      });
    }
  }

  sleep(Math.random() * 3 + 1);
}
