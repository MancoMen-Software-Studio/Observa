# Guia de Contribucion

Gracias por tu interes en contribuir a Observa Dashboard.

## Proceso de Contribucion

1. Fork del repositorio
2. Crear una rama desde `develop` (`git checkout -b feature/mi-funcionalidad`)
3. Realizar los cambios siguiendo los estandares de codigo
4. Agregar tests para los cambios realizados
5. Ejecutar `dotnet build` y `dotnet test` para verificar que todo funciona
6. Hacer commit siguiendo Conventional Commits (`git commit -m 'feat: agregar funcionalidad'`)
7. Push a la rama (`git push origin feature/mi-funcionalidad`)
8. Crear un Pull Request hacia `develop`

## Estandares de Codigo

- Seguir las convenciones de Microsoft para C#
- Solo comentarios de documentacion XML (`///`) en espanol
- No usar comentarios de linea (`//`)
- No usar emoticones en codigo ni documentacion
- Namespaces file-scoped obligatorio
- Llaves obligatorias en todos los bloques

Ver [docs/architecture/coding-standards.md](docs/architecture/coding-standards.md) para detalles completos.

## Conventional Commits

Todos los commits deben seguir el formato:

```
<tipo>(<alcance>): <descripcion>
```

Tipos permitidos: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `chore`, `ci`, `build`

## Pull Requests

- Todo PR debe estar vinculado a un issue
- Minimo 1 aprobacion requerida
- Todos los checks de CI deben pasar
- Llenar completamente el template de PR

## Reportar Bugs

Usa el template de bug report al crear un nuevo issue.

## Solicitar Funcionalidades

Usa el template de feature request al crear un nuevo issue.
