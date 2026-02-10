# Observa - Guia de Uso Completa

Guia paso a paso para levantar Observa desde cero, crear datos, visualizar dashboards interactivos y ejecutar pruebas de carga.

## Arquitectura del Sistema

```
  ┌─────────────────┐     ┌──────────────────┐
  │  Frontend React  │     │  k6 Load Tests   │
  │  :5173 / :5174   │     │  (simulaciones)  │
  └────────┬─────────┘     └────────┬─────────┘
           │ HTTP + WebSocket       │ HTTP + WebSocket
           ▼                        ▼
  ┌──────────────────────────────────────────┐
  │       ASP.NET Core API  (:5268)          │
  │                                          │
  │  Minimal API Endpoints                   │
  │       ▼                                  │
  │  MediatR (Commands / Queries)            │
  │       ▼                                  │
  │  Domain (Logica de negocio)              │
  │       ▼                                  │
  │  EF Core (Persistencia)                  │
  │       ▼                                  │
  │  SignalR Hub (/hubs/dashboard)           │
  │  (Notificaciones en tiempo real)         │
  └────────┬─────────────────────────────────┘
           │
           ▼
  ┌──────────────────┐
  │  PostgreSQL 17   │
  │  :5432           │
  │  Base: observa_dev│
  └──────────────────┘
```

## Flujo de Datos

```
1. Cliente crea/modifica datos (HTTP POST/PUT/DELETE)
2. API recibe request → MediatR Command → Validacion → Dominio → EF Core
3. Datos se persisten en PostgreSQL
4. SignalR envia notificacion al grupo del dashboard
5. Frontend recibe evento → Invalida cache React Query → Refetch → Re-render
6. Graficas se actualizan automaticamente sin recargar pagina
```

## Flujo de Simulacion en Tiempo Real

```
1. DataSimulationService (BackgroundService) arranca con el API
2. Cada 30s refresca la lista de dashboards publicados desde PostgreSQL
3. Cada 500ms genera datos simulados para cada widget de cada dashboard publicado
4. Envia evento "WidgetDataUpdated" via SignalR al grupo del dashboard
5. Frontend recibe los datos y actualiza las graficas en vivo
6. LineChart acumula puntos en sliding window (ultimos 30)
7. Los demas charts reemplazan sus datos completos en cada tick
```

## Prerequisitos

- **Docker Desktop** (para PostgreSQL)
- **.NET SDK 9.0**
- **Node.js 20+** (para el frontend)
- **k6** (para pruebas de carga): `brew install k6`

## Paso 1: Iniciar PostgreSQL

```bash
docker compose up -d
```

Verifica que este corriendo:

```bash
docker compose ps
```

Debe mostrar el contenedor `observa-postgres` en estado `running`.

Credenciales:
- Host: `localhost`
- Puerto: `5432`
- Usuario: `observa`
- Password: `observa`
- Base de datos: `observa_dev`

## Paso 2: Iniciar el Backend

```bash
dotnet run --project src/Observa.Api
```

Al arrancar en modo Development, el API automaticamente:
- Aplica las migraciones de EF Core (crea las tablas en PostgreSQL)
- Escucha en `http://localhost:5268`
- Monta Swagger UI en `http://localhost:5268/swagger`
- Configura CORS para los puertos `5173` y `5174`
- Monta el hub SignalR en `/hubs/dashboard`

Verificar que funcione:

```bash
curl http://localhost:5268/health
# Respuesta: {"status":"Healthy"}
```

## Paso 3: Iniciar el Frontend

```bash
cd client
npm install
npm run dev
```

Vite inicia en `http://localhost:5173` (o `5174` si el 5173 esta ocupado).

El proxy de Vite redirige automaticamente:
- `/api/*` → `http://localhost:5268` (API REST)
- `/hubs/*` → `http://localhost:5268` (SignalR WebSocket)

Abre el navegador en **http://localhost:5173**.

## Paso 4: Crear Datos

La plataforma comienza vacia. Se necesitan **DataSources** antes de poder agregar widgets a los dashboards.

### Desde el Frontend

1. **Crear DataSource**: Ir a **http://localhost:5173/data-sources** y crear al menos uno
2. **Crear Dashboard**: En la pagina principal, clic en "Crear dashboard"
3. **Agregar Widgets**: Dentro del dashboard, agregar widgets seleccionando tipo de grafica, datasource y posicion
4. **Publicar**: Cuando el dashboard tenga al menos 1 widget, publicarlo

### Desde la Linea de Comandos (datos rapidos)

```bash
# 1. Crear DataSource
DS_ID=$(curl -s -X POST http://localhost:5268/api/datasources \
  -H "Content-Type: application/json" \
  -d '{"name":"API de Ventas","type":0,"connectionString":"https://api.ventas.com/v1"}' \
  | jq -r '.id')
echo "DataSource: $DS_ID"

# 2. Crear Dashboard
DASH_ID=$(curl -s -X POST http://localhost:5268/api/dashboards \
  -H "Content-Type: application/json" \
  -d '{"title":"Panel de Ventas","description":"Metricas de ventas en tiempo real"}' \
  | jq -r '.id')
echo "Dashboard: $DASH_ID"

# 3. Agregar widgets de diferentes tipos
# LineChart (tendencia temporal)
curl -s -X POST "http://localhost:5268/api/dashboards/$DASH_ID/widgets" \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"Ventas por Mes\",\"type\":0,\"column\":0,\"row\":0,\"width\":6,\"height\":4,\"dataSourceId\":\"$DS_ID\",\"refreshInterval\":30}"

# BarChart (comparacion por categorias)
curl -s -X POST "http://localhost:5268/api/dashboards/$DASH_ID/widgets" \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"Ventas por Region\",\"type\":1,\"column\":6,\"row\":0,\"width\":6,\"height\":4,\"dataSourceId\":\"$DS_ID\",\"refreshInterval\":60}"

# PieChart (distribucion)
curl -s -X POST "http://localhost:5268/api/dashboards/$DASH_ID/widgets" \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"Distribucion\",\"type\":2,\"column\":0,\"row\":4,\"width\":4,\"height\":4,\"dataSourceId\":\"$DS_ID\",\"refreshInterval\":300}"

# Gauge (indicador circular)
curl -s -X POST "http://localhost:5268/api/dashboards/$DASH_ID/widgets" \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"SLA Uptime\",\"type\":5,\"column\":4,\"row\":4,\"width\":4,\"height\":4,\"dataSourceId\":\"$DS_ID\",\"refreshInterval\":5}"

# KpiCard (metrica con tendencia)
curl -s -X POST "http://localhost:5268/api/dashboards/$DASH_ID/widgets" \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"Revenue Total\",\"type\":7,\"column\":8,\"row\":4,\"width\":4,\"height\":4,\"dataSourceId\":\"$DS_ID\",\"refreshInterval\":60}"

# 4. Publicar dashboard
curl -s -X POST "http://localhost:5268/api/dashboards/$DASH_ID/publish"
echo "Dashboard publicado: $DASH_ID"
```

### Tipos de Widget

| type | Nombre      | Grafica                                    |
|------|-------------|--------------------------------------------|
| 0    | LineChart   | Lineas de tendencia temporal               |
| 1    | BarChart    | Barras comparativas por categoria          |
| 2    | PieChart    | Torta de distribucion por segmentos        |
| 3    | HeatMap     | Mapa de calor (dias x horas)               |
| 4    | ScatterPlot | Dispersion X-Y con correlacion             |
| 5    | Gauge       | Indicador circular con colores por umbral  |
| 6    | Table       | Tabla de datos (servidores, CPU, memoria)  |
| 7    | KpiCard     | Tarjeta KPI con valor y tendencia          |
| 8    | Map         | Mapa geografico con ciudades               |

### Tipos de DataSource

| type | Nombre     |
|------|------------|
| 0    | RestApi    |
| 1    | Database   |
| 2    | WebSocket  |
| 3    | StaticFile |
| 4    | SignalR    |

### Intervalos de Refresco

| refreshInterval | Frecuencia    |
|-----------------|---------------|
| 0               | Tiempo real   |
| 5               | 5 segundos    |
| 30              | 30 segundos   |
| 60              | 1 minuto      |
| 300             | 5 minutos     |
| 900             | 15 minutos    |

## Paso 5: Visualizar en el Frontend

1. Abrir **http://localhost:5173**
2. Se ve la lista de dashboards con paginacion (20 por pagina)
3. Clic en un dashboard para ver sus widgets con graficas interactivas
4. Las graficas soportan:
   - Hover para ver tooltips con valores
   - Clic en leyendas para filtrar series
   - El Gauge cambia de color segun el valor (rojo < 40%, amarillo < 70%, verde >= 70%)

### Filtrar Dashboards

En la pagina principal:
- **Busqueda por texto**: Filtra dashboards por titulo
- **Filtro por estado**: Draft (borrador), Published (publicado), Archived (archivado)
- **Paginacion**: Navegar entre paginas con los botones Anterior/Siguiente

### Tiempo Real (SignalR)

Si tienes un dashboard abierto en el navegador y alguien (o k6) modifica datos via API:

```
POST /api/dashboards/{id}/widgets  →  Widget nuevo aparece automaticamente
PUT /api/dashboards/{id}/title     →  Titulo se actualiza automaticamente
DELETE /api/dashboards/{id}/widgets/{wid}  →  Widget desaparece automaticamente
```

No necesitas recargar la pagina. El flujo es:

```
Mutacion HTTP → Backend persiste → SignalR notifica → Frontend re-renderiza
```

El indicador de conexion en la interfaz muestra el estado de la conexion WebSocket.

### Simulacion de Datos en Tiempo Real

Los dashboards **publicados** reciben datos simulados automaticamente cada 500ms (2 veces por segundo). No requiere configuracion adicional: el `DataSimulationService` arranca con el backend y detecta los dashboards publicados.

Para verlo en accion:

1. Crear un dashboard con al menos un widget (ver Paso 4)
2. Publicar el dashboard
3. Abrir el dashboard en el navegador
4. Las graficas se actualizan en vivo cada 500ms

Comportamiento por tipo de widget:

| Widget | Comportamiento de simulacion |
|--------|------------------------------|
| LineChart | Streaming: acumula puntos en sliding window (ultimos 30). Cada tick agrega un punto con timestamp. |
| BarChart | Las 5 categorias fluctuan ventas y retornos gradualmente (+/- 5-15%) |
| PieChart | Los 5 segmentos varian sus valores gradualmente (+/- 3-8%) |
| Gauge | Oscila sinusoidalmente entre 20-95 simulando carga de CPU |
| KpiCard | Valor con random walk gradual. Muestra cambio porcentual vs valor anterior |
| Table | 6 servidores con CPU/memoria fluctuando. Estado cambia segun umbrales (>85% Critico, >65% Advertencia) |
| Map | 6 ciudades con valores variando +/- 5-10% |
| HeatMap | Grid dias x horas con valores variando levemente |
| ScatterPlot | Nube de 30 puntos desplazandose gradualmente |

Notas:
- Solo los dashboards con estado **Published** generan datos simulados
- Los dashboards en Draft o Archived no reciben datos
- La lista de dashboards activos se refresca cada 30 segundos
- Al salir de un dashboard, el frontend deja de recibir datos (invoca `LeaveDashboardGroup`)
- Si no hay clientes conectados a un grupo, SignalR descarta los mensajes (sin impacto en rendimiento)
- Los logs del backend muestran `"Servicio de simulacion de datos iniciado"` al arrancar

## Paso 6: Ejecutar Pruebas de Carga

Requisito: Backend corriendo en `:5268`.

### Smoke Test (30 segundos)

Verifica que todos los endpoints funcionan:

```bash
k6 run tests/load/scenarios/smoke.js
```

Si tienes el frontend abierto, veras dashboards creandose y desapareciendo en tiempo real.

### Load Test (5 minutos)

Simula 50 usuarios con uso normal (60% lecturas, 30% escrituras, 10% acciones):

```bash
k6 run tests/load/scenarios/load.js
```

### Stress Test (~9 minutos)

Escala de 10 a 500 usuarios para encontrar el punto de quiebre:

```bash
k6 run tests/load/scenarios/stress.js
```

### Spike Test (~3 minutos)

Pico subito de 10 a 1000 usuarios y vuelta a 10:

```bash
k6 run tests/load/scenarios/spike.js
```

### Soak Test (30 minutos)

50 usuarios constantes para detectar memory leaks o degradacion:

```bash
k6 run tests/load/scenarios/soak.js
```

### WebSocket Test (~5.5 minutos)

100-500 conexiones SignalR simultaneas con mutaciones HTTP:

```bash
k6 run tests/load/scenarios/websocket.js
```

### Suite Completa

Ejecuta todos los escenarios en secuencia con reportes:

```bash
bash tests/load/run-all.sh
```

Ejecutar solo escenarios especificos:

```bash
bash tests/load/run-all.sh smoke load stress
```

Usar URL personalizada:

```bash
K6_BASE_URL=http://mi-servidor:5268 bash tests/load/run-all.sh
```

## Paso 7: Interpretar Resultados

Al finalizar, k6 muestra un resumen como:

```
✓ http_req_duration..........: avg=45ms  p(90)=78ms  p(95)=120ms  p(99)=380ms
✓ http_req_failed............: 0.23%   ✓ 12    ✗ 5188
✓ dashboard_created..........: 97.50%  ✓ 390   ✗ 10
✓ widget_added...............: 98.00%  ✓ 392   ✗ 8
  iterations.................: 1200    40.0/s
```

### Metricas clave

| Metrica | Que mide | Umbral aceptable |
|---------|----------|-------------------|
| `http_req_duration` p95 | El 95% de requests se completan en menos de X ms | < 500ms (normal), < 1500ms (estres) |
| `http_req_duration` p99 | El 99% de requests se completan en menos de X ms | < 1000ms (normal), < 2000ms (estres) |
| `http_req_failed` | Porcentaje de requests fallidas | < 1% (normal), < 5% (estres) |
| `dashboard_created` | Tasa de exito al crear dashboards | > 95% |
| `widget_added` | Tasa de exito al agregar widgets | > 95% |
| `ws_connected` | Tasa de conexiones WebSocket exitosas | > 90% |
| `ws_notification_latency` | Latencia de notificaciones en tiempo real | cuanto menor, mejor |
| `iterations` | Operaciones completadas por segundo (throughput) | depende del hardware |

### Reportes

Los reportes se generan en `tests/load/results/{timestamp}/`:
- `{escenario}.json` - Datos crudos de metricas
- `{escenario}_summary.json` - Resumen con percentiles
- `{escenario}.log` - Log completo de ejecucion

## Paso 8: Limpiar Datos de Pruebas

Despues de ejecutar pruebas de carga, la base de datos puede tener miles de registros de prueba. Para limpiar:

```bash
# Conectar a PostgreSQL
docker exec -it observa-postgres psql -U observa -d observa_dev

# Dentro de psql, eliminar datos de prueba
DELETE FROM widgets;
DELETE FROM dashboard_threshold_rules;
DELETE FROM alerts;
DELETE FROM dashboards;
DELETE FROM data_sources;

# Salir
\q
```

## API Reference

| Metodo | Endpoint | Descripcion | Body |
|--------|----------|-------------|------|
| GET | `/health` | Health check | - |
| GET | `/api/dashboards?page=1&pageSize=20&status=Draft&search=texto` | Listar dashboards (paginado) | - |
| GET | `/api/dashboards/{id}` | Obtener dashboard con widgets | - |
| POST | `/api/dashboards` | Crear dashboard | `{title, description}` |
| PUT | `/api/dashboards/{id}/title` | Actualizar titulo | `{newTitle}` |
| POST | `/api/dashboards/{id}/publish` | Publicar (requiere >= 1 widget) | - |
| POST | `/api/dashboards/{id}/archive` | Archivar | - |
| POST | `/api/dashboards/{id}/widgets` | Agregar widget (max 20 por dashboard) | `{title, type, column, row, width, height, dataSourceId, refreshInterval}` |
| DELETE | `/api/dashboards/{id}/widgets/{widgetId}` | Eliminar widget | - |
| GET | `/api/datasources` | Listar todos los datasources | - |
| POST | `/api/datasources` | Crear datasource | `{name, type, connectionString}` |

### Parametros de Paginacion (GET /api/dashboards)

| Parametro | Tipo | Default | Descripcion |
|-----------|------|---------|-------------|
| `page` | int | 1 | Numero de pagina (base 1) |
| `pageSize` | int | 20 | Elementos por pagina (max 100) |
| `status` | string | - | Filtrar por estado: `Draft`, `Published`, `Archived` |
| `search` | string | - | Buscar por titulo (case insensitive) |

### Respuesta Paginada

```json
{
  "items": [...],
  "totalCount": 1500,
  "page": 1,
  "pageSize": 20,
  "totalPages": 75,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### Eventos SignalR (WebSocket)

El hub se conecta en `/hubs/dashboard`. Eventos disponibles:

| Evento | Direccion | Descripcion | Payload |
|--------|-----------|-------------|---------|
| `JoinDashboardGroup` | Cliente → Servidor | Unirse al grupo de un dashboard | `dashboardId: string` |
| `LeaveDashboardGroup` | Cliente → Servidor | Salir del grupo de un dashboard | `dashboardId: string` |
| `DashboardUpdated` | Servidor → Cliente | Dashboard fue modificado | `dashboardId: guid` |
| `WidgetAdded` | Servidor → Cliente | Widget agregado al dashboard | `{dashboardId, widgetId}` |
| `WidgetRemoved` | Servidor → Cliente | Widget eliminado del dashboard | `{dashboardId, widgetId}` |
| `AlertTriggered` | Servidor → Cliente | Alerta disparada | `{dashboardId, alertId, severity}` |
| `WidgetDataUpdated` | Servidor → Cliente | Datos simulados actualizados | `{dashboardId, widgets: {widgetId: data}}` |

El evento `WidgetDataUpdated` se emite cada 500ms para dashboards publicados. El payload `widgets` es un diccionario donde cada clave es un `widgetId` y el valor son los datos generados segun el tipo de widget.

## Swagger UI

Explorar la API interactivamente en: **http://localhost:5268/swagger**

## Resumen de Comandos

```bash
# Infraestructura
docker compose up -d                          # Iniciar PostgreSQL
docker compose down                           # Detener PostgreSQL
docker compose logs -f                        # Ver logs de PostgreSQL

# Backend
dotnet build                                  # Compilar
dotnet test                                   # Ejecutar tests
dotnet run --project src/Observa.Api          # Iniciar API

# Frontend
cd client && npm install && npm run dev       # Iniciar frontend
cd client && npx tsc --noEmit                 # Verificar tipos

# Pruebas de carga
k6 run tests/load/scenarios/smoke.js          # Smoke test
k6 run tests/load/scenarios/load.js           # Load test
bash tests/load/run-all.sh                    # Suite completa
bash tests/load/run-all.sh smoke load         # Escenarios especificos
```
