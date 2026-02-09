# Observa Dashboard - Backend API

## Proyecto

Observa es la plataforma de visualizacion de datos en tiempo real de MancoMen Software Studio.
Backend construido con ASP.NET Core 9.0, Clean Architecture, SignalR y PostgreSQL.

## Stack Tecnologico

- Runtime: .NET 9.0
- Lenguaje: C# 13 (nullable habilitado, implicit usings deshabilitado)
- Framework: ASP.NET Core 9.0 Minimal APIs
- Mediator: MediatR 12.x
- Validacion: FluentValidation 11.x
- ORM: Entity Framework Core 9.0 (code-first)
- Base de datos: PostgreSQL 17
- Cache: Redis 7.4
- Tiempo real: SignalR con protocolo MessagePack
- Observabilidad: OpenTelemetry, Serilog, Seq

## Arquitectura

Clean Architecture con 4 capas:

- **Domain**: Logica de negocio pura. Cero dependencias externas.
- **Application**: Orquestacion de casos de uso via MediatR. Solo referencia Domain.
- **Infrastructure**: Implementaciones de EF Core, Redis, servicios externos. Referencia Domain y Application.
- **Api**: Capa HTTP. Minimal API endpoints + SignalR hubs. Referencia Application e Infrastructure.

## Comandos

```bash
dotnet build                                    # Compilar solucion
dotnet test                                     # Ejecutar todos los tests
dotnet run --project src/Observa.Api            # Iniciar API
dotnet ef database update --project src/Observa.Infrastructure --startup-project src/Observa.Api  # Migraciones
```

## Reglas de Codigo

### Comentarios
- SOLO usar comentarios de documentacion XML: `///`
- NO usar comentarios de linea: `//`
- Todos los comentarios deben estar en espanol
- NO usar emoticones en ningun archivo del proyecto

### Convenciones de Nomenclatura (Microsoft C#)
- Clases, interfaces, metodos, propiedades: PascalCase
- Interfaces: prefijo `I` (IRepository, IService)
- Campos privados: `_camelCase` con prefijo guion bajo
- Variables locales y parametros: camelCase
- Constantes: PascalCase
- Namespaces: PascalCase siguiendo estructura de carpetas
- Archivos: nombre identico a la clase/interfaz que contienen

### Estilo de Codigo
- Namespaces file-scoped obligatorio
- Llaves obligatorias en todos los bloques
- Una clase/interfaz por archivo
- Complejidad ciclomatica < 10 por metodo
- Complejidad cognitiva < 15

### Patrones Obligatorios
- Result Pattern para fallos esperados (nunca excepciones para flujo de control)
- Options Pattern para configuracion tipada
- Repository Pattern para acceso a datos
- Mediator Pattern (MediatR) para manejo de requests
- Dependency Injection explicita (nunca Service Locator)

### Principios Arquitectonicos
1. Dependency Inversion: capas superiores no dependen de inferiores, ambas dependen de abstracciones
2. Explicito sobre implicito: sin magic strings, sin estado implicito
3. Fail Fast, Fail Loud: estados invalidos imposibles, validacion en fronteras
4. Observabilidad como ciudadano de primera clase: logging estructurado, tracing distribuido, metricas

### Testing
- Minimo 80% cobertura en tests unitarios
- xUnit como framework de testing
- NSubstitute para mocks
- FluentAssertions para aserciones
- NetArchTest para enforcement de arquitectura

### Git
- Rama principal: main
- Ramas: feature/*, bugfix/*, hotfix/*, release/*
- Commits: Conventional Commits (feat:, fix:, docs:, refactor:, test:, chore:)
- Siempre Pull Request para merge a main

### Seguridad
- Nunca commitear secretos, claves API o connection strings
- Usar appsettings.Development.json (gitignored) para configuracion local
- Variables de entorno para produccion
