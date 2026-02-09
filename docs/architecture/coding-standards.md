# Estandares de Codigo - MancoMen Software Studio

## Principios Fundamentales

### Principio 1: Inversion de Dependencias
Los modulos de alto nivel no dependen de modulos de bajo nivel. Ambos dependen de abstracciones.
La capa de dominio tiene cero dependencias en infraestructura. Base de datos, mensajeria y
servicios externos son implementaciones intercambiables detras de interfaces definidas en el dominio.

### Principio 2: Explicito sobre Implicito
Configuracion, dependencias y comportamiento son siempre explicitos. Sin magic strings, sin
descubrimiento basado en convenciones que oculte comportamiento, sin estado implicito.
Cada decision es rastreable en el codigo.

### Principio 3: Fallar Rapido, Fallar Fuerte
Los estados invalidos son imposibles de representar. La validacion ocurre en los limites del sistema.
Los errores nunca se tragan. Cada fallo produce telemetria accionable.

### Principio 4: Observabilidad como Ciudadano de Primera Clase
Los sistemas se disenan para ser observables desde el dia uno. Logging estructurado, tracing
distribuido y metricas son requisitos arquitectonicos, no adiciones posteriores.

## Convenciones de Nomenclatura

### C# (Microsoft Conventions)

| Elemento | Estilo | Ejemplo |
|----------|--------|---------|
| Namespace | PascalCase | `Observa.Domain.Entities` |
| Clase | PascalCase | `DashboardWidget` |
| Interfaz | IPascalCase | `IWidgetRepository` |
| Metodo | PascalCase | `GetActiveWidgets` |
| Propiedad | PascalCase | `CreatedAt` |
| Campo privado | _camelCase | `_repository` |
| Parametro | camelCase | `widgetId` |
| Variable local | camelCase | `activeCount` |
| Constante | PascalCase | `MaxWidgetCount` |
| Enum | PascalCase | `WidgetType` |
| Evento | PascalCase | `WidgetCreated` |

### Nombres de Archivos

- Un archivo por clase/interfaz/record
- Nombre del archivo identico al tipo que contiene
- Ejemplo: `DashboardWidget.cs` contiene `class DashboardWidget`

## Reglas de Comentarios

### Permitido: Documentacion XML (`///`)
```csharp
/// <summary>
/// Representa un widget del dashboard con su configuracion de visualizacion.
/// </summary>
public sealed class DashboardWidget
{
}
```

### Prohibido: Comentarios de linea (`//`)
No se permiten comentarios `//` en el codigo fuente.
Si el codigo necesita explicacion, refactorizar para que sea autoexplicativo.

### Idioma
Todos los comentarios de documentacion deben estar en espanol.

## Formato de Codigo

### Namespaces
```csharp
namespace Observa.Domain.Entities;
```
Siempre file-scoped (una linea, con punto y coma).

### Llaves
Obligatorias en todos los bloques, incluso de una sola linea:
```csharp
if (condition)
{
    DoSomething();
}
```

### Usings
- Sin implicit usings (deshabilitado en proyecto)
- Ordenados: System primero, luego terceros, luego propios
- Sin separacion por grupos

## Patrones de Codigo

### Result Pattern
Para operaciones que pueden fallar de forma esperada:
```csharp
public Result<Widget> CreateWidget(string name, WidgetType type)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return Result<Widget>.Failure("El nombre del widget es requerido");
    }

    var widget = new Widget(name, type);
    return Result<Widget>.Success(widget);
}
```

### Options Pattern
Para configuracion tipada:
```csharp
public sealed class SignalROptions
{
    public const string SectionName = "SignalR";

    public required string RedisBackplane { get; init; }
    public required int MaxReconnectAttempts { get; init; }
}
```

### Repository Pattern
Interfaces en Domain, implementaciones en Infrastructure:
```csharp
public interface IWidgetRepository
{
    Task<Widget?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Widget widget, CancellationToken cancellationToken);
}
```

## Complejidad

| Metrica | Limite |
|---------|--------|
| Complejidad ciclomatica por metodo | < 10 |
| Complejidad cognitiva | < 15 |
| Lineas por metodo | < 30 (recomendado) |
| Parametros por metodo | < 5 |
| Profundidad de anidamiento | < 3 niveles |

## Manejo de Errores

- Result Pattern para fallos esperados (validacion, reglas de negocio)
- Excepciones para fallos inesperados (errores de sistema, infraestructura)
- Nunca bloques catch vacios
- Nunca usar excepciones para flujo de control
- Toda excepcion no manejada produce log estructurado

## Seguridad

- Nunca loggear datos sensibles (passwords, tokens, PII)
- Validar toda entrada en los limites del sistema
- Usar parametros SQL parametrizados (EF Core lo hace automaticamente)
- Nunca exponer stack traces en produccion
- Headers de seguridad en todas las respuestas HTTP

## Git Workflow

### Ramas
| Rama | Proposito |
|------|-----------|
| `main` | Codigo listo para produccion |
| `develop` | Rama de integracion |
| `feature/*` | Nuevas funcionalidades |
| `bugfix/*` | Correccion de errores |
| `hotfix/*` | Correcciones urgentes de produccion |
| `release/*` | Preparacion de release |

### Commits (Conventional Commits)
```
feat(dashboard): agregar endpoint de creacion de widgets
fix(signalr): corregir reconexion en perdida de conexion
docs(readme): actualizar instrucciones de instalacion
refactor(domain): extraer value object para configuracion de widget
test(application): agregar tests para CreateWidgetHandler
chore(deps): actualizar MediatR a v12.4.1
```
