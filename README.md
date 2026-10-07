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

```powershell
copy .env.example .env   # completar los valores marcados como REQUERIDA
docker build -t salud-web .
docker run -d -p 8080:8080 --env-file .env -v ./certs:/certs:ro salud-web
```

Andes, X-Road, LASCHyBS y Recetar son **requeridas**; `AdminLogs__`* y
`Turnos__*` son opcionales. Para X-Road, dejar el `.pfx` en `./certs` (montado en `/certs`) y
configurar `ApiXroadssAndes__CertPath=/certs/xroad.pfx`.

> **Importante:** no agregar `RuntimeIdentifier` a los pasos de `restore`/`publish` del Dockerfile.
> `AndesServices` es un proyecto referenciado y eso provoca el error `NETSDK1152` por salidas duplicadas.



## Configuración relevante


| Sección                                   | Requerida | Propósito                                                        |
| ----------------------------------------- | --------- | ---------------------------------------------------------------- |
| `ApiAndes`                                | Sí        | URL de Andes y token de restablecer contraseña                   |
| `ApiXroadssAndes`                         | Sí        | Gateway X-Road (RENAPER, RANIA): URL, certificado `.pfx` y clave |
| `ApiLASCHyBS`                             | Sí        | Backend de resultados de laboratorio                             |
| `ApiRecetar`                              | Sí        | API de prescripciones Recetar (URL y token)                      |
| `SaludConfiguracion` (`saludConfig.json`) | Incluida  | Paginación y rangos de fecha de laboratorios                     |
| `Turnos:MinutosVisualizacionTelemedicina` | No (480)  | Minutos que un turno de videoconferencia sigue visible           |
| `Blazor:ServerHubPath`                    | Incluida  | Ruta del hub de Blazor Server (`/mi-blazor`)                     |
| `AdminLogs:ApiKey` / `AdminLogs:BaseUrl`  | No        | Envío opcional de logs/telemetría al Admin                       |


### Configuración local (secretos)

`appsettings.json` no contiene URLs de backends, tokens, contraseñas ni rutas personales. En local
se cargan con **User Secrets** (se leen automáticamente en `Development`). Todas las claves de
abajo son **requeridas**; pedir las URLs, tokens y el certificado X-Road al equipo.

```powershell
cd SaludPortal.Web
dotnet user-secrets set "ApiAndes:baseUrl" "<url-andes>"
dotnet user-secrets set "ApiAndes:TokenRestablecerPassword" "<token>"
dotnet user-secrets set "ApiXroadssAndes:baseUrl" "<url>"
dotnet user-secrets set "ApiXroadssAndes:CertPath" "C:\\ruta\\xroad.pfx"
dotnet user-secrets set "ApiXroadssAndes:CertPassword" "<pass>"
dotnet user-secrets set "ApiLASCHyBS:baseUrl" "<url>"
dotnet user-secrets set "ApiRecetar:baseUrl" "<url>"
dotnet user-secrets set "ApiRecetar:AccessToken" "<token>"
```

Alternativa: editar el `secrets.json` directamente ("Manage User Secrets" en Visual Studio/Rider,
o `%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json`; el `UserSecretsId` está en
`SaludPortal.Web.csproj`):

```json
{
  "ApiAndes": {
    "baseUrl": "<url-andes>",
    "TokenRestablecerPassword": "<token>"
  },
  "ApiXroadssAndes": {
    "baseUrl": "<url>",
    "CertPath": "C:\\ruta\\xroad.pfx",
    "CertPassword": ""
  },
  "ApiLASCHyBS": {
    "baseUrl": "<url>"
  },
  "ApiRecetar": {
    "baseUrl": "<url>",
    "AccessToken": "<token>"
  }
}
```

`CertPassword` debe estar definida, pero puede quedar vacía si el `.pfx` no tiene contraseña. El
`.pfx` nunca se commitea (`*.pfx` está en `.gitignore`).

En Docker/producción usar variables de entorno con `__` como separador; la lista completa está en
`[.env.example](.env.example)`.

## Autenticación

Los pacientes inician sesión contra Andes (`modules/mobileApp/login`) vía `AuthController`
(`POST /api/auth/login`). El JWT se guarda en la cookie `MiSalud` (expiración deslizante de 1 hora).

## Observabilidad (opcional)

El portal puede enviar logs (`Information+`) y telemetría de uso al panel **SaludPortal.Admin**
(otro repositorio) por HTTP:

- `POST /api/logs` y `POST /api/telemetry`
- Header `X-Api-Key` = `AdminLogs:ApiKey`

**Si** `AdminLogs:ApiKey` **está vacío, el pipeline queda desactivado** y el portal funciona sin Admin.

En `Development`, `appsettings.Development.json` ya trae `AdminLogs:ApiKey` de desarrollo y
`AdminLogs:BaseUrl=https://localhost:7180`, así que el envío queda **activo** por defecto. Si se
corre el portal sin el Admin levantado, desactivarlo para evitar reintentos contra un host caído:

```powershell
cd SaludPortal.Web
dotnet user-secrets set "AdminLogs:ApiKey" ""
```

Para activarlo:


| Clave               | Ejemplo Development                       | Notas                                     |
| ------------------- | ----------------------------------------- | ----------------------------------------- |
| `AdminLogs:ApiKey`  | misma que `LogIngestion:ApiKey` del Admin | Debe coincidir exactamente                |
| `AdminLogs:BaseUrl` | `https://localhost:7180`                  | URL pública o de red donde corre el Admin |


En Docker/producción, apuntar `AdminLogs__BaseUrl` a la URL real del Admin (no usar service discovery de Aspire).