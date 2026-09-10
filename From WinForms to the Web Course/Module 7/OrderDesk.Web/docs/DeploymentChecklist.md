# Deployment checklist — "publish success is not deployment readiness"

`dotnet publish` succeeding proves the build. Deployment readiness is the list below: the target, the runtime facts
a Wisej.NET app depends on, and the safety net. The assets under `deploy/` are the concrete answers for IIS and
for a Linux container; the same questions apply to a cloud PaaS.

## Targets

| Target | Asset | Notes |
|---|---|---|
| **IIS on Windows** | `deploy/iis/web.config` | ASP.NET Core Hosting Bundle (.NET 10) installed; app pool *No Managed Code*; `web.config` = ASP.NET Core Module handler (`processPath="dotnet" arguments=".\OrderDesk.dll"`, in-process, stdout log for first-start problems) **plus** the `<appSettings>` Wisej.NET and `AppConfig` read — the file replaces the project's `Web.config` in the publish folder. Enable the *WebSocket Protocol* feature. |
| **Linux / container** | `deploy/docker/Dockerfile`, `deploy/docker/docker-compose.yml` | multi-stage: `sdk:10.0` runs `dotnet publish -c Release -f net10.0` (the Kestrel target, no `-windows`), `aspnet:10.0` runs it; `EXPOSE 8080`, `VOLUME /app/App_Data`, `HEALTHCHECK curl /health`. Linux is **case-sensitive**: `Themes/orderdesk.mixin.theme`, `ClientProfiles.json`, `Default.html` must be referenced with the exact casing (they are). |
| **Cloud + storage** | the container image + a mounted file share | the storage root must be a mounted share (Azure Files / EFS), not the instance disk; the license key from the platform's secret store. |

## Runtime

| Item | Fact | Where it is set |
|---|---|---|
| **Session affinity** | Wisej.NET keeps session state in the process. More than one instance ⇒ the load balancer must pin a browser to an instance (ARR client affinity, ingress sticky cookie). Without it a request lands on a different instance as a brand-new empty session — it looks like random sign-outs. | `deploy/iis/web.config` header comment, `docker-compose.yml` (single replica on purpose) |
| **Configuration values** | `Web.config` `<appSettings>`: `OrderDesk.StorageRoot`, `OrderDesk.ThemeMixin`, `OrderDesk.Version`, `Wisej.DefaultTheme`; `Default.json`: theme, session timeout; ports from the host (`ASPNETCORE_URLS` / IIS binding), **never** from `launchSettings.json` (a VS-only file). Secrets (license key, connection strings) come from environment / secret store at deploy time. | `deploy/appsettings.Production.notes.md` |
| **Diagnostics / logging** | `Diagnostics/AppLog.cs` writes `App_Data/logs/orderdesk-yyyyMMdd.log` (one line per level + message, every session in one file) and mirrors to `System.Diagnostics.Trace`; audit lines go there too (`AUDIT` / `AUDIT-DENIED`). Container: stdout to the log driver. IIS: `stdoutLogFile` only for startup failures. | `AppLog`, `AuditLog.Record` |
| **Health check** | `GET /health` (`Startup.cs` `MapGet`) — no Wisej session is created; `HealthCheck.Json()` reports status, version, uptime, storage root writable (a probe file is written and deleted), log folder, `ClientProfiles.json` and the mixin found, `"ok": true/false`. The console's **Health check** adds the session facts (sessions, theme, profile, web socket) an external probe cannot see. | `Diagnostics/HealthCheck.cs`, `Dockerfile HEALTHCHECK`, compose `healthcheck:` |
| **HTTPS** | terminate at IIS / the reverse proxy; Kestrel behind it speaks HTTP on the internal network; forward `X-Forwarded-Proto`. | proxy configuration, not the app |
| **Storage root & temp paths** | everything the app writes goes under `AppConfig.StorageRoot` (`uploads/ exports/ reports/ logs/`); no `%TEMP%`, no `C:\Orders`. The app-pool identity / container user needs write rights there. | `Services/AppConfig.cs`, `Security/DownloadGuard.cs`, `AppLog` |
| **Report fonts** | `InvoicePdfWriter` uses PDF's built-in Courier — nothing to install. A real PDF library on Linux needs its fonts in the image. | `Reporting/InvoicePdfWriter.cs` |
| **Package versions** | Wisej-4 4.1.0, .NET 10 — the same versions on the target as in the build (the health check prints them). | `HealthCheck` "version" item |

## Safety net

| Item | How |
|---|---|
| **Backup & rollback** | The build output and the storage root are separate. Backup = the storage root (`App_Data`) + the configuration values. Rollback = redeploy the previous publish folder / image tag (`orderdesk-web:<previous>`); the storage root is untouched by a rollback because nothing under it is versioned with the build. Keep the previous publish folder / image until the new one has passed the two-session test. |
| **Two concurrent sessions** | After every deployment: two browsers, **kelly** and **sam**. Each audit grid must show the other's sign-in within a second; sam's Export must be denied; kelly's Download must succeed. This proves one process, separate session contexts (Module 4), one shared audit log (Module 7), and that affinity works if a load balancer is in front (a session that "forgets" the user between clicks is the affinity failure). |
| **Remaining migration debt** | listed in `docs/CapstoneReport.md` — *risks that remain*: in-memory repository (no database yet), demo accounts in code (no identity provider), CSV export instead of the Module 6 .xlsx in this snapshot, the report queue not carried into the capstone console, no rate limiting on sign-in. |

## Evidence (in the running app, Readiness & deploy tab)

- On load `labelDeploy` lists `✓ deploy/iis/web.config · ✓ deploy/docker/Dockerfile · ✓ deploy/docker/docker-compose.yml ·
  ✓ deploy/appsettings.Production.notes.md · ✓ docs/DeploymentChecklist.md …` and the trace says
  `• server deploy assets 8/8 files present under <content root>`.
- **Health check**: the monospace label shows the ✓ lines (status, version, uptime, storage root (writable), log
  folder, client profiles, theme mixin, sessions, theme, profile, web socket); each is traced as `• server health · <name>`.
- **GET /health**: a new tab with the JSON, and the trace `• server HttpClient GET http://localhost:5607/health → n bytes`
  followed by the JSON; if the app runs on another port the banner explains the probe-URL mismatch (that is the
  misconfiguration this checklist exists for).
- **Run readiness checklist** item 8: `5/5 deployment files present · /health endpoint in Startup.cs · logs in …\App_Data\logs`.

Not executed on this machine: the IIS publish, the Docker build (no Docker engine assumed), the two-session test on
a real second host. The files are written to be run, and the checklist says so rather than claiming they were.
