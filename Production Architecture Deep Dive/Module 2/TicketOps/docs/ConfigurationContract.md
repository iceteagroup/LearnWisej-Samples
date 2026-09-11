# Configuration contract — TicketOps Console

*Module 2 deliverable · "Configuration is a deployable contract"*

The person who deploys TicketOps is often not the person who wrote it. Everything they may safely change is
listed here with what it controls; everything not listed lives in code and must not be edited on a server.
Values are read **once per process** and exposed as the immutable `AppSettings` object (`ProcessScope.Settings`).
Per-session choices (the theme an operator switched to, the tenant they work in) are **not** configuration —
they live on `SessionContext` and are never written back to a file.

## `appsettings.json` (section `TicketOps`) — read by `Infrastructure/AppSettings.Load`

| Key | Type | Default | Controls | Who changes it |
|---|---|---|---|---|
| `Environment` | string | `Training` | The environment name shown on the diagnostics page and in logs | deployer, per environment |
| `BuildVersion` | string | `2.0.0-lab` | Version label shown on the diagnostics page (set by the build pipeline in production) | build pipeline |
| `DispatchApiBaseUrl` | URL | `https://api.ticketops.local/v1` | Base URL of the dispatch integration (the API client would be a singleton built from it) | deployer, per environment — **the key/secret is an environment variable, never here** |
| `UploadLimitMB` | int | `25` | Maximum attachment size the upload controls accept | deployer |
| `LoggingLevel` | string | `Information` | Minimum level written by the production logger | deployer / operations |
| `DefaultTenant` | string | `Contoso` | Tenant a new session starts in until the operator switches | deployer |
| `AvailableThemes` | string[] | `Bootstrap-4, Material-3, FluentDark-5` | Themes an operator may select for their session (`SessionService.SelectTheme` rejects others) | deployer |

A missing or unreadable file does not stop the app: defaults apply.

## `Default.json` — Wisej.NET's own configuration, read through `Application.Configuration`

| Key | Value here | Controls | Notes |
|---|---|---|---|
| `startup` | `TicketOps.Program.Main, TicketOps` | The per-session entry point | code, not a deployer knob |
| `theme` | `Bootstrap-4` | The default theme for every session (`Application.Configuration.ThemeName`) | a session may override it for itself via `Application.Theme` |
| `sessionTimeout` | `1200` | Idle seconds before the session ends (`Application.Configuration.SessionTimeout`); applied to every session | seconds, not minutes |
| `debug` | `true` | Verbose client/server diagnostics | `false` in production |
| `url` | `Default.html` | The page that hosts the app | — |

Other Wisej keys a deployment may add: `maxSessionsPerClient`, `validateClient`, `culture`, `enableWebSocket`
(the Real-Time course cookbook documents the fallback), `sessionStorage`.

## `Web.config`

`Wisej.LicenseKey` (empty in the lab) and `Wisej.DefaultTheme`. The license key is a secret: keep it out of
source control and inject it at deploy time.

## Where each kind of value is read

| Value | Where it lives | Read how often | Object |
|---|---|---|---|
| Environment, build, dispatch URL, upload limit, logging level, default tenant, themes | `appsettings.json` | once per process | `AppSettings` (immutable) |
| Session timeout, default theme name | `Default.json` | once per process by Wisej.NET | `Application.Configuration` |
| Secrets (API keys, connection strings, license) | environment variables / deploy-time injection | once per process | never in source |
| Current operator, tenant, theme, client profile, selected ticket | this session | per session, on connect; changed by `ISessionService` | `SessionContext` |
| A click's inputs and validation result | the round-trip | per request | locals |

## Evidence

Open the app: the **Application** panel lists *Environment / build*, *Dispatch API base URL*, *Upload limit*,
*Logging level* (from `appsettings.json`) and *Idle session timeout (Default.json)*, *Default theme (Default.json)*
(from `Application.Configuration`). Open a second tab: the same values, read once per process.
Change `UploadLimitMB` in `appsettings.json`, restart the process: the value changes for everyone at once —
which is exactly what "application scope" means.
