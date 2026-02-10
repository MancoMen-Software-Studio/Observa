# Observa - Suite de Pruebas de Carga k6

Suite de pruebas de carga para el API de Observa usando [k6](https://grafana.com/docs/k6/) (Grafana).

## Requisitos

- **k6** instalado: `brew install k6`
- Backend de Observa corriendo en `http://localhost:5268`
- PostgreSQL disponible y con migraciones aplicadas

## Estructura

```
tests/load/
  helpers/
    config.js       # BASE_URL, thresholds (SLOs), metricas custom
    api.js          # Funciones helper para cada endpoint HTTP
    signalr.js      # Helper de negociacion y conexion SignalR/WebSocket
  scenarios/
    smoke.js        # 2 VUs, 30s - sanity check
    load.js         # 50 VUs, 5min - carga normal sostenida
    stress.js       # 10-500 VUs, ~9min - encontrar breaking point
    spike.js        # 10-1000 VUs, ~3min - pico subito
    soak.js         # 50 VUs, 30min - estabilidad prolongada
    websocket.js    # 100-500 conexiones SignalR + mutaciones HTTP
  run-all.sh        # Ejecutar toda la suite con reportes
  results/          # Reportes generados (gitignored)
```

## Uso

### Ejecutar un escenario individual

```bash
k6 run tests/load/scenarios/smoke.js
```

### Ejecutar con URL personalizada

```bash
K6_BASE_URL=http://mi-servidor:5268 k6 run tests/load/scenarios/smoke.js
```

### Ejecutar toda la suite

```bash
bash tests/load/run-all.sh
```

### Ejecutar escenarios especificos

```bash
bash tests/load/run-all.sh smoke load stress
```

## Escenarios

### Smoke (Prueba de humo)
- **VUs**: 2
- **Duracion**: 30 segundos
- **Objetivo**: Verificar que todos los endpoints responden correctamente
- **Flujo**: Health check -> Crear datasource -> Crear dashboard -> Agregar widget -> Publicar -> Listar -> Archivar

### Load (Carga normal)
- **VUs**: 0 -> 50 -> 0 (rampa)
- **Duracion**: 5 minutos
- **Objetivo**: Simular uso normal de la plataforma
- **Mix**: 60% lecturas, 30% escrituras, 10% acciones

### Stress (Estres)
- **VUs**: 10 -> 100 -> 200 -> 500 -> 0
- **Duracion**: ~9 minutos
- **Objetivo**: Encontrar el punto de quiebre del sistema
- **Thresholds**: Relajados (p99 < 2000ms, error rate < 5%)

### Spike (Pico)
- **VUs**: 10 -> 1000 -> 10 -> 0
- **Duracion**: ~3 minutos
- **Objetivo**: Evaluar reaccion ante picos subitos
- **Thresholds**: Muy relajados (p99 < 3000ms, error rate < 10%)

### Soak (Resistencia)
- **VUs**: 50 constantes
- **Duracion**: 30 minutos
- **Objetivo**: Detectar memory leaks, connection pool exhaustion, degradacion gradual
- **Indicador clave**: Si el p95 crece con el tiempo, hay un problema

### WebSocket (SignalR)
- **Conexiones WS**: 100 -> 300 -> 500 -> 0
- **Mutadores HTTP**: 5 VUs constantes
- **Duracion**: ~5.5 minutos
- **Objetivo**: Medir latencia de notificaciones en tiempo real
- **Flujo**: Negotiate -> Handshake -> JoinGroup -> Escuchar eventos

## SLOs (Thresholds)

### Default (smoke, load, soak)
| Metrica | Threshold |
|---------|-----------|
| `http_req_duration` p95 | < 500ms |
| `http_req_duration` p99 | < 1000ms |
| `http_req_failed` rate | < 1% |
| `dashboard_created` rate | > 95% |
| `widget_added` rate | > 95% |

### Stress
| Metrica | Threshold |
|---------|-----------|
| `http_req_duration` p95 | < 1500ms |
| `http_req_duration` p99 | < 2000ms |
| `http_req_failed` rate | < 5% |

### Spike
| Metrica | Threshold |
|---------|-----------|
| `http_req_duration` p95 | < 2000ms |
| `http_req_duration` p99 | < 3000ms |
| `http_req_failed` rate | < 10% |

## Metricas Custom

| Metrica | Tipo | Descripcion |
|---------|------|-------------|
| `dashboard_created` | Rate | Tasa de exito al crear dashboards |
| `widget_added` | Rate | Tasa de exito al agregar widgets |
| `datasource_created` | Rate | Tasa de exito al crear datasources |
| `dashboard_published` | Rate | Tasa de exito al publicar |
| `dashboard_archived` | Rate | Tasa de exito al archivar |
| `create_dashboard_duration` | Trend | Latencia de creacion de dashboards |
| `get_dashboards_duration` | Trend | Latencia de listado de dashboards |
| `add_widget_duration` | Trend | Latencia de agregar widgets |
| `ws_connected` | Rate | Tasa de conexiones WebSocket exitosas |
| `ws_message_received` | Counter | Total de mensajes WS recibidos |
| `ws_notification_latency` | Trend | Latencia de notificaciones en tiempo real |
| `ws_errors` | Counter | Total de errores WebSocket |
| `error_count` | Counter | Total de errores HTTP por endpoint |

## Reportes

Los reportes se generan en `tests/load/results/{timestamp}/` con:

- `{scenario}.json` - Datos crudos de metricas (formato k6 JSON)
- `{scenario}_summary.json` - Resumen exportado con percentiles
- `{scenario}.log` - Log completo de la ejecucion

## Endpoints bajo prueba

| Metodo | Endpoint | Descripcion |
|--------|----------|-------------|
| GET | `/health` | Health check |
| GET | `/api/dashboards` | Listar dashboards |
| GET | `/api/dashboards/{id}` | Obtener dashboard |
| POST | `/api/dashboards` | Crear dashboard |
| PUT | `/api/dashboards/{id}/title` | Actualizar titulo |
| POST | `/api/dashboards/{id}/publish` | Publicar |
| POST | `/api/dashboards/{id}/archive` | Archivar |
| POST | `/api/dashboards/{id}/widgets` | Agregar widget |
| DELETE | `/api/dashboards/{id}/widgets/{widgetId}` | Eliminar widget |
| POST | `/api/datasources` | Crear datasource |
| WS | `/hubs/dashboard` | Hub SignalR |
