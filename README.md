# SaludPortal.Web — Portal del Paciente

Portal de pacientes (Blazor Server) de la plataforma de salud pública de Neuquén, integrado con
**Andes**.

## Proyectos


| Proyecto                  | Rol                                                        |
| ------------------------- | ---------------------------------------------------------- |
| `SaludPortal.Web`         | Portal Blazor Server                                       |
| `AndesServices`           | Librería: interfaces, servicios, entidades y DTOs de Andes |
| `SaludPortal.Application` | Casos de uso y modelos limpios consumidos por Web          |
| `RecetarServices`         | Adapter de la API de prescripciones Recetar                |
| `LachybsServices`         | Adapter de la API de laboratorio LASCHyBS                  |
| `XroadssAndesServices`    | Adapter del gateway X-Road (RENAPER y laboratorio RANIA)   |
| `AdminLogsServices`       | Envío de logs y telemetría al panel Admin                  |


Los adapters no tienen referencias a otros proyectos; cada uno se registra en `Program.cs` con su
`Add*Services(builder.Configuration)`.

Solución: `[SaludPortal.Web.slnx](SaludPortal.Web.slnx)`.

## Requisitos

- [.NET SDK 9](https://dotnet.microsoft.com/download) (9.0.200+ para `.slnx`)
- Docker (opcional, para publicar imagen)



## Build y ejecución

```bash
dotnet build SaludPortal.Web.slnx
dotnet run --project SaludPortal.Web
```



### Docker

```bash
docker build -t salud-web .
docker run -d -p 8080:8080 --env-file .env -v ./certs:/certs:ro salud-web
```

`.env` (no se commitea) usa los nombres de variables de entorno de
[Configuración local](#configuración-local-secretos). Solo `ApiAndes__baseUrl` es obligatoria;
el resto habilita cada integración. Para X-Road, dejar el `.pfx` en `./certs` y configurar
`ApiXroadssAndes__CertPath=/certs/xroad.pfx`.

```
ApiAndes__baseUrl=<url-andes>
ApiRecetar__baseUrl=<url>
ApiRecetar__AccessToken=<token>
ApiXroadssAndes__baseUrl=<url>
ApiXroadssAndes__CertPath=/certs/xroad.pfx
AdminLogs__BaseUrl=<url-admin>
AdminLogs__ApiKey=<api-key>
```

> **Importante:** no agregar `RuntimeIdentifier` a los pasos de `restore`/`publish` del Dockerfile.
> `AndesServices` es un proyecto referenciado y eso provoca el error `NETSDK1152` por salidas duplicadas.



## Configuración relevante


| Sección                                   | Propósito                                          |
| ----------------------------------------- | -------------------------------------------------- |
| `ApiAndes`                                | URL de Andes y token de restablecer contraseña     |
| `ApiLASCHyBS`                             | Backend de resultados de laboratorio               |
| `ApiRecetar`                              | API de prescripciones Recetar (URL y token)        |
| `ApiXroadssAndes`                         | Gateway X-Road (RENAPER, RANIA): URL y certificado |
| `SaludConfiguracion` (`saludConfig.json`) | Paginación y rangos de fecha de laboratorios       |
| `AdminLogs:ApiKey` / `AdminLogs:BaseUrl`  | Envío opcional de logs/telemetría al Admin         |


`saludConfig.json` se carga explícitamente en `SaludPortal.Web/Program.cs`
(`AddJsonFile(..., optional: false)`).

### Configuración local (secretos)

`appsettings.json` no contiene URLs de backends, tokens, contraseñas ni rutas personales. En local
se cargan con **User Secrets** (se leen automáticamente en `Development`). Como mínimo hace falta
la URL de Andes; Recetar, LASCHyBS, restablecimiento de contraseña y X-Road se habilitan al
cargar sus valores. Pedir las URLs y credenciales al equipo.

```powershell
cd SaludPortal.Web
# Requerido
dotnet user-secrets set "ApiAndes:baseUrl" "<url-andes>"
# Opcionales
dotnet user-secrets set "ApiAndes:TokenRestablecerPassword" "<token>"
dotnet user-secrets set "ApiLASCHyBS:baseUrl" "<url>"
dotnet user-secrets set "ApiRecetar:baseUrl" "<url>"
dotnet user-secrets set "ApiRecetar:AccessToken" "<token>"
dotnet user-secrets set "ApiXroadssAndes:baseUrl" "<url>"
dotnet user-secrets set "ApiXroadssAndes:CertPath" "C:\\ruta\\xroad.pfx"
dotnet user-secrets set "ApiXroadssAndes:CertPassword" "<pass>"
```

En Docker/producción usar variables de entorno con `__` como separador: `ApiAndes__baseUrl`,
`ApiAndes__TokenRestablecerPassword`, `ApiLASCHyBS__baseUrl`, `ApiRecetar__baseUrl`,
`ApiRecetar__AccessToken`, `ApiXroadssAndes__baseUrl`, `ApiXroadssAndes__CertPath`,
`ApiXroadssAndes__CertPassword`, `AdminLogs__BaseUrl`, `AdminLogs__ApiKey`.

## Autenticación

Los pacientes inician sesión contra Andes (`modules/mobileApp/login`) vía `AuthController`
(`POST /api/auth/login`). El JWT se guarda en la cookie `MiSalud` (expiración deslizante de 1 hora).

## Observabilidad (opcional)

El portal puede enviar logs (`Information+`) y telemetría de uso al panel **SaludPortal.Admin**
(otro repositorio) por HTTP:

- `POST /api/logs` y `POST /api/telemetry`
- Header `X-Api-Key` = `AdminLogs:ApiKey`

**Si** `AdminLogs:ApiKey` **está vacío, el pipeline queda desactivado** y el portal funciona sin Admin.

Para activarlo:


| Clave               | Ejemplo Development                       | Notas                                     |
| ------------------- | ----------------------------------------- | ----------------------------------------- |
| `AdminLogs:ApiKey`  | misma que `LogIngestion:ApiKey` del Admin | Debe coincidir exactamente                |
| `AdminLogs:BaseUrl` | `https://localhost:7180`                  | URL pública o de red donde corre el Admin |


En Docker/producción, apuntar `AdminLogs__BaseUrl` a la URL real del Admin (no usar service discovery de Aspire).