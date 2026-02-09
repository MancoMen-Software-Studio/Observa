<div align="center">

# Observa Dashboard

**Plataforma de visualizacion de datos en tiempo real de nivel empresarial**

[![Build Status](https://github.com/MancoMen-Software-Studio/Observa/actions/workflows/ci.yml/badge.svg)](https://github.com/MancoMen-Software-Studio/Observa/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)

[Documentacion](docs/) |
[Arquitectura](docs/architecture/README.md) |
[Contribuir](CONTRIBUTING.md)

</div>

---

## Descripcion

Observa es una implementacion de referencia que demuestra arquitectura frontend moderna,
sincronizacion de datos en tiempo real y diseno accesible. El backend proporciona APIs
de alto rendimiento y comunicacion bidireccional via SignalR para dashboards de datos
empresariales.

### Funcionalidades Principales

- **Clean Architecture** - Diseno centrado en el dominio con inversion de dependencias
- **Datos en Tiempo Real** - SignalR con protocolo MessagePack para latencia < 100ms
- **CQRS** - Modelos separados de lectura/escritura para rendimiento optimizado
- **Observabilidad** - OpenTelemetry para tracing distribuido, logging estructurado con Serilog
- **Seguridad** - Headers de seguridad, rate limiting, validacion de entrada
- **Testing** - xUnit, NSubstitute, FluentAssertions, NetArchTest para enforcement de arquitectura

## Arquitectura

```
                    +------------------+
                    |    Observa.Api   |
                    | Endpoints + Hubs |
                    +--------+---------+
                             |
                    +--------v-----------+
                    | Observa.Application|
                    | Commands + Queries |
                    +--------+-----------+
                             |
                    +--------v---------+
                    |  Observa.Domain  |
                    | Entidades + Reglas|
                    +--------+---------+
                             ^
                    +--------+--------------+
                    | Observa.Infrastructure|
                    | EF Core + Redis + Hub |
                    +-----------------------+
```

## Inicio Rapido

```bash
git clone https://github.com/MancoMen-Software-Studio/Observa.git
cd Observa

docker-compose up -d

dotnet run --project src/Observa.Api
```

## Prerequisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started) (para PostgreSQL y Redis)
- [PostgreSQL 17](https://www.postgresql.org/) (o usar Docker)
- [Redis 7.4](https://redis.io/) (o usar Docker)

## Instalacion

### Usando Docker (Recomendado)

```bash
docker-compose up --build
```

### Configuracion Manual

1. **Clonar y restaurar**
   ```bash
   git clone https://github.com/MancoMen-Software-Studio/Observa.git
   cd Observa
   dotnet restore
   ```

2. **Configurar entorno**
   ```bash
   cp src/Observa.Api/appsettings.Development.example.json \
      src/Observa.Api/appsettings.Development.json
   ```

3. **Ejecutar migraciones**
   ```bash
   dotnet ef database update \
     --project src/Observa.Infrastructure \
     --startup-project src/Observa.Api
   ```

4. **Iniciar la API**
   ```bash
   dotnet run --project src/Observa.Api
   ```

## Estructura del Proyecto

```
Observa/
|-- src/
|   |-- Observa.Domain/           # Logica de negocio pura
|   |-- Observa.Application/      # Casos de uso (MediatR)
|   |-- Observa.Infrastructure/   # EF Core, Redis, SignalR
|   +-- Observa.Api/              # Endpoints HTTP + Hubs
|-- tests/
|   |-- Observa.Domain.Tests/
|   |-- Observa.Application.Tests/
|   |-- Observa.Api.Tests/
|   +-- Observa.Architecture.Tests/
|-- docs/
|   |-- architecture/
|   +-- guides/
|-- .github/
|   |-- workflows/
|   +-- ISSUE_TEMPLATE/
+-- CLAUDE.md
```

## Stack Tecnologico

| Categoria | Tecnologia |
|-----------|------------|
| Runtime | .NET 9.0 |
| Lenguaje | C# 13 |
| Framework | ASP.NET Core 9.0 Minimal APIs |
| Mediator | MediatR 12.x |
| Validacion | FluentValidation 11.x |
| ORM | Entity Framework Core 9.0 |
| Base de Datos | PostgreSQL 17 |
| Cache | Redis 7.4 |
| Tiempo Real | SignalR + MessagePack |
| Observabilidad | OpenTelemetry, Serilog, Seq |
| Testing | xUnit, NSubstitute, FluentAssertions |

## Testing

```bash
dotnet test

dotnet test --collect:"XPlat Code Coverage"

dotnet test tests/Observa.Domain.Tests
```

### Requisitos de Cobertura

| Capa | Cobertura Minima |
|------|------------------|
| Domain | 90% |
| Application | 85% |
| Infrastructure | 70% |
| Api | 60% |

## Configuracion

| Variable | Descripcion | Default |
|----------|-------------|---------|
| `ConnectionStrings__Database` | Connection string de PostgreSQL | -- |
| `ConnectionStrings__Redis` | Connection string de Redis | `localhost:6379` |
| `SignalR__MaxReconnectAttempts` | Intentos de reconexion | `5` |
| `Serilog__MinimumLevel` | Nivel minimo de logging | `Information` |

## Contribuir

Aceptamos contribuciones. Consulta [CONTRIBUTING.md](CONTRIBUTING.md) para las guias.

1. Fork del repositorio
2. Crear rama feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit con Conventional Commits (`git commit -m 'feat: agregar funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crear Pull Request

## Licencia

Este proyecto esta bajo la Licencia MIT - ver [LICENSE](LICENSE) para detalles.

---

<div align="center">

**Desarrollado por [MancoMen Software Studio](https://mancomen.com)**

Bogota, Colombia

</div>
