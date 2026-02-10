/// Prueba de humo (smoke test).
/// 2 VUs durante 30 segundos - verifica que todo funciona antes de escalar.
/// Ejecuta el flujo completo: datasource -> dashboard -> widget -> publicar -> listar -> archivar.

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
  removeWidget,
} from "../helpers/api.js";

export const options = {
  vus: 2,
  duration: "30s",
  thresholds: defaultThresholds,
  tags: { scenario: "smoke" },
};

export default function () {
  /// Verificar salud del API
  healthCheck();
  sleep(0.5);

  /// Crear datasource
  const dsId = createDataSource(
    `Smoke DS ${__VU}-${__ITER}`,
    0,
    "https://api.example.com/smoke"
  );
  if (!dsId) { return; }
  sleep(0.3);

  /// Crear dashboard
  const dashId = createDashboard(
    `Smoke Dashboard ${__VU}-${__ITER}`,
    "Dashboard de prueba de humo"
  );
  if (!dashId) { return; }
  sleep(0.3);

  /// Agregar widget
  const widgetId = addWidget(dashId, dsId, {
    title: `Smoke Widget ${__VU}-${__ITER}`,
    type: 0,
    column: 0,
    row: 0,
    width: 6,
    height: 4,
  });
  sleep(0.3);

  /// Listar dashboards
  getDashboards();
  sleep(0.3);

  /// Obtener dashboard por ID
  getDashboard(dashId);
  sleep(0.3);

  /// Actualizar titulo
  updateDashboardTitle(dashId, `Smoke Updated ${__VU}-${__ITER}`);
  sleep(0.3);

  /// Publicar dashboard (requiere al menos 1 widget)
  publishDashboard(dashId);
  sleep(0.3);

  /// Archivar dashboard
  archiveDashboard(dashId);
  sleep(0.3);

  /// Eliminar widget
  if (widgetId) {
    removeWidget(dashId, widgetId);
  }
  sleep(0.5);
}
