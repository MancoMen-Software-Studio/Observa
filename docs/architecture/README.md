# Arquitectura - Observa Dashboard Backend

## Descripcion General

Observa sigue Clean Architecture con estricta separacion de capas e inversion de dependencias.
El dominio se encuentra en el centro sin dependencias externas, y la infraestructura implementa
las abstracciones definidas en las capas internas.

## Capas

### Domain (Observa.Domain)

Capa central con logica de negocio pura en C#.

**Contenido:**
- Aggregates: raices de agregado
- Entities: entidades de dominio
- ValueObjects: objetos de valor inmutables
- Events: eventos de dominio
- Abstractions: interfaces de repositorios y servicios

**Reglas:**
- Cero dependencias en NuGet externos
- Nunca referencia Application, Infrastructure o Api
- Todas las invariantes de negocio se validan aqui
- Constructores privados con factory methods cuando aplique

### Application (Observa.Application)

Orquestacion de casos de uso.

**Contenido:**
- Commands: operaciones de escritura (CQRS)
- Queries: operaciones de lectura (CQRS)
- Behaviors: pipeline de MediatR (validacion, logging)
- DTOs: objetos de transferencia de datos
- Validators: validadores FluentValidation
- Abstractions: interfaces de servicios de aplicacion

**Reglas:**
- Solo referencia Domain
- Sin acceso directo a infraestructura
- Toda validacion de entrada via FluentValidation

### Infrastructure (Observa.Infrastructure)

Implementaciones de concerns externos.

**Contenido:**
- Persistence: DbContext de EF Core, implementaciones de repositorios
- Caching: implementacion de Redis
- RealTime: servicios de SignalR
- ExternalServices: adaptadores de servicios de terceros

**Reglas:**
- Implementa abstracciones definidas en Domain y Application
- Referencia Domain y Application
- Contiene toda configuracion de persistencia

### Api (Observa.Api)

Punto de entrada HTTP.

**Contenido:**
- Endpoints: Minimal API endpoints agrupados por funcionalidad
- Hubs: SignalR hubs para datos en tiempo real
- Middleware: middleware personalizado
- Configuration: configuracion de startup

**Reglas:**
- No contiene logica de negocio
- Delega todo a Application via MediatR
- Maneja autenticacion, rate limiting, versionado

## Patrones Aplicados

| Patron | Uso |
|--------|-----|
| Clean Architecture | Estructura por capas con inversion de dependencias |
| Mediator (MediatR) | Manejo desacoplado de requests |
| Repository | Abstraccion sobre acceso a datos |
| Result Pattern | Retornos explicitos de exito/fallo |
| Options Pattern | Configuracion tipada con validacion |
| CQRS | Modelos separados de lectura/escritura |

## Flujo de una Request

```
HTTP Request
    |
    v
[Api Layer] Endpoint recibe request
    |
    v
[Application Layer] MediatR despacha Command/Query
    |
    v
[Application Layer] ValidationBehavior valida input
    |
    v
[Application Layer] Handler ejecuta logica de caso de uso
    |
    v
[Domain Layer] Entidades/Aggregates aplican reglas de negocio
    |
    v
[Infrastructure Layer] Repository persiste/consulta datos
    |
    v
HTTP Response
```

## Diagrama de Dependencias

```
Observa.Api ──> Observa.Application ──> Observa.Domain
     |                                        ^
     v                                        |
Observa.Infrastructure ───────────────────────┘
```
