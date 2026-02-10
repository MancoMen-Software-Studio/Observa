/// Helper para negociacion y conexion SignalR via WebSocket.
/// Implementa el protocolo de handshake JSON de SignalR.

import http from "k6/http";
import { check } from "k6";
import ws from "k6/ws";
import { Counter, Rate, Trend } from "k6/metrics";
import { BASE_URL, jsonHeaders } from "./config.js";

/// Separador de registro del protocolo SignalR (ASCII 0x1E)
const RECORD_SEPARATOR = String.fromCharCode(0x1e);

/// Metricas custom para WebSocket/SignalR
export const wsConnected = new Rate("ws_connected");
export const wsMessageReceived = new Counter("ws_message_received");
export const wsNotificationLatency = new Trend(
  "ws_notification_latency",
  true
);
export const wsErrors = new Counter("ws_errors");
export const wsConnectionDuration = new Trend(
  "ws_connection_duration",
  true
);

/// Negocia la conexion SignalR y obtiene el connectionToken.
/// Retorna el connectionToken necesario para abrir el WebSocket.
export function negotiate() {
  const res = http.post(
    `${BASE_URL}/hubs/dashboard/negotiate?negotiateVersion=1`,
    null,
    {
      headers: jsonHeaders,
      tags: { name: "POST /hubs/dashboard/negotiate" },
    }
  );

  const ok = check(res, {
    "negotiate: status 200": (r) => r.status === 200,
    "negotiate: has connectionToken": (r) =>
      r.json("connectionToken") !== undefined,
  });

  if (!ok) {
    wsErrors.add(1);
    return null;
  }

  return {
    connectionToken: res.json("connectionToken"),
    connectionId: res.json("connectionId"),
    availableTransports: res.json("availableTransports"),
  };
}

/// Construye la URL de WebSocket a partir del resultado de negociacion
export function buildWsUrl(negotiation) {
  const baseWs = BASE_URL.replace("http://", "ws://").replace(
    "https://",
    "wss://"
  );
  return `${baseWs}/hubs/dashboard?id=${negotiation.connectionToken}&transport=WebSockets`;
}

/// Crea un mensaje con formato de protocolo SignalR JSON
export function signalrMessage(target, args) {
  const msg = JSON.stringify({
    type: 1,
    target: target,
    arguments: args || [],
  });
  return msg + RECORD_SEPARATOR;
}

/// Mensaje de handshake del protocolo SignalR
export function handshakeMessage() {
  return JSON.stringify({ protocol: "json", version: 1 }) + RECORD_SEPARATOR;
}

/// Parsea un mensaje recibido del protocolo SignalR.
/// Retorna un array de mensajes parseados.
export function parseSignalrMessages(data) {
  const messages = [];
  const parts = data.split(RECORD_SEPARATOR);
  for (let i = 0; i < parts.length; i++) {
    const part = parts[i].trim();
    if (part.length === 0) {
      continue;
    }
    try {
      messages.push(JSON.parse(part));
    } catch (_e) {
      continue;
    }
  }
  return messages;
}

/// Abre una conexion SignalR completa:
/// negotiate -> WebSocket -> handshake -> join group.
/// Ejecuta el callback durante la conexion y cierra al terminar.
export function connectAndJoin(dashboardId, durationSeconds, onMessage) {
  const negotiation = negotiate();
  if (!negotiation) {
    wsConnected.add(false);
    return;
  }

  const wsUrl = buildWsUrl(negotiation);
  const duration = durationSeconds || 30;

  const res = ws.connect(wsUrl, {}, function (socket) {
    let handshakeCompleted = false;
    let joinedGroup = false;
    const connectionStart = Date.now();

    socket.on("open", function () {
      socket.send(handshakeMessage());
    });

    socket.on("message", function (data) {
      const messages = parseSignalrMessages(data);

      for (let i = 0; i < messages.length; i++) {
        const msg = messages[i];

        /// Respuesta de handshake (objeto vacio = exito)
        if (!handshakeCompleted && msg.type === undefined) {
          handshakeCompleted = true;
          wsConnected.add(true);

          if (dashboardId) {
            socket.send(
              signalrMessage("JoinDashboardGroup", [dashboardId])
            );
            joinedGroup = true;
          }
          continue;
        }

        /// Ping del servidor (type 6)
        if (msg.type === 6) {
          socket.send(JSON.stringify({ type: 6 }) + RECORD_SEPARATOR);
          continue;
        }

        /// Invocacion del servidor (type 1) - notificacion
        if (msg.type === 1) {
          wsMessageReceived.add(1);
          if (onMessage) {
            onMessage(msg, socket);
          }
        }

        /// Completion (type 3)
        if (msg.type === 3) {
          continue;
        }

        /// Close (type 7)
        if (msg.type === 7) {
          socket.close();
          return;
        }
      }
    });

    socket.on("error", function (e) {
      wsErrors.add(1);
      wsConnected.add(false);
    });

    socket.on("close", function () {
      const elapsed = Date.now() - connectionStart;
      wsConnectionDuration.add(elapsed);
    });

    /// Mantener conexion abierta durante el tiempo especificado
    socket.setTimeout(function () {
      if (joinedGroup && dashboardId) {
        socket.send(
          signalrMessage("LeaveDashboardGroup", [dashboardId])
        );
      }
      socket.close();
    }, duration * 1000);
  });

  check(res, {
    "ws: status 101": (r) => r && r.status === 101,
  });
}

/// Abre multiples conexiones SignalR en paralelo dentro de un VU.
/// Util para simular multiples clientes escuchando el mismo dashboard.
export function connectMultiple(dashboardIds, durationSeconds) {
  for (let i = 0; i < dashboardIds.length; i++) {
    connectAndJoin(dashboardIds[i], durationSeconds);
  }
}
