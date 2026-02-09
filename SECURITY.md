# Politica de Seguridad

## Versiones Soportadas

| Version | Soportada |
|---------|-----------|
| 1.x     | Si        |

## Reportar una Vulnerabilidad

Si descubres una vulnerabilidad de seguridad, por favor reportala de forma responsable.

**No crees un issue publico para vulnerabilidades de seguridad.**

En su lugar, envia un correo a security@mancomen.com con:

1. Descripcion de la vulnerabilidad
2. Pasos para reproducirla
3. Impacto potencial
4. Sugerencia de correccion (si la tienes)

Nos comprometemos a:

- Confirmar la recepcion del reporte en 48 horas
- Proporcionar una evaluacion inicial en 5 dias habiles
- Trabajar en una correccion y coordinar la divulgacion
- Dar credito al reportante (si lo desea) en el CHANGELOG

## Practicas de Seguridad

Este proyecto sigue las mejores practicas de seguridad:

- Analisis de codigo estatico con CodeQL
- Escaneo de dependencias con Dependabot
- Headers de seguridad en todas las respuestas HTTP
- Validacion de entrada en todos los endpoints
- Sin secretos en el codigo fuente
- Logging estructurado sin datos sensibles
