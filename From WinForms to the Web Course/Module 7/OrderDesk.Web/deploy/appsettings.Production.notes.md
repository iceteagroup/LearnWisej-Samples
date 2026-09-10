# appsettings.Production — where every setting moves for a real deployment

OrderDesk.Web has no `appsettings.json`: the WinForms `App.config` became `Web.config` `<appSettings>` in Module 2
(read with `System.Xml.Linq` by `Services/AppConfig.cs`), and Wisej.NET reads `Default.json` for its own options.
This note says which value lives where once the app leaves `dotnet run`, and which ones must **not** stay in a
file that is committed.

| Setting | Today (dev, `dotnet run`) | Production | Why |
|---|---|---|---|
| `Wisej.LicenseKey` | `Web.config`, empty | environment variable / secret store at deploy time (`WISEJ_LICENSEKEY` in `docker-compose.yml`, IIS environment variable or the app-pool identity's user secrets); copied into `web.config` by the deployment script, never committed | a secret in source control is a leak the first time the repo is shared |
| `Wisej.DefaultTheme` (`Default.json` `"theme"`) | `Bootstrap-4` | unchanged, or the house theme; the `orderdesk` mixin in `/Themes` is merged at startup | theming is a build asset, not a per-server setting |
| `OrderDesk.StorageRoot` | `App_Data` (relative to the content root) | IIS: an absolute path outside the site folder (`D:\OrderDeskData`), Modify rights for the app-pool identity; container: `/app/App_Data` mounted as a volume; cloud: a mounted file share (Azure Files, EFS) — not the container's own disk | the container is ephemeral; the site folder is replaced on every publish; uploads, exports, reports and **logs** live under this root |
| `OrderDesk.ThemeMixin` | `orderdesk` | unchanged | names a file that ships with the build |
| `OrderDesk.Version` | `7.0.0-capstone` | stamped by the build pipeline (the health check reports it) | so `/health` says which build is running |
| Session timeout | Wisej default (`Default.json` `"sessionTimeout"`, seconds; not set → framework default) | set explicitly, e.g. `"sessionTimeout": 1200` (20 min), and handle `Application.SessionTimeout` (Module 4) | idle sessions hold memory per browser; the business decides how long |
| Ports / URLs | `launchSettings.json` → `http://localhost:5607` | IIS: the site binding; container: `ASPNETCORE_URLS=http://+:8080`, mapped by compose/orchestrator | `launchSettings.json` is a Visual Studio file — it is not deployed |
| HTTPS | none (localhost) | terminate TLS at IIS or at the reverse proxy (nginx / Traefik / cloud LB) in front of Kestrel; forward `X-Forwarded-Proto`; HSTS at the proxy | Kestrel behind a proxy speaks plain HTTP inside the network; the browser only ever sees HTTPS |
| Session affinity | one process, n/a | **required** as soon as there is more than one instance: ARR client affinity (IIS web farm), `sticky` cookie on the proxy / ingress | Wisej.NET session state is in-process; a request routed to another instance is a new, empty session |
| Diagnostics / logging | `App_Data/logs/orderdesk-yyyyMMdd.log` + `System.Diagnostics.Trace` | keep the file log on the storage root **and** ship `Trace`/stdout to the host: IIS `stdoutLogFile=.\logs\stdout` (first-start problems only), container stdout → the orchestrator's log driver | a log next to the .exe was one user's; on the server one file serves every session and must survive a redeploy |
| Health check | `/health` (`Startup.cs`) | the load balancer / orchestrator probe: `HEALTHCHECK` in the Dockerfile, `healthcheck:` in compose, an IIS/ARR health test on `/health`; alert on `"ok":false` | takes a broken instance out of rotation before users find it |
| Temp folders | none used (PDF and CSV are built in memory) | if a report library needs a temp folder, point it under the storage root and grant write rights | the service account's `%TEMP%` may be read-only or wiped |
| Report fonts | `InvoicePdfWriter` uses the built-in Courier | a real PDF library needs its fonts installed on the Linux image (`fonts-liberation` etc.) | "works on my Windows PC" is the Module 6 lesson again |
| Connection string | none (in-memory repository) | `ConnectionStrings` via environment variable; never in `Web.config` | secret, and different per environment |

## Things that stay in the build

`Default.html`, `Default.json` (minus secrets), `Web.config` (minus the license key — the IIS file in
`deploy/iis/web.config` is the merged production version), `ClientProfiles.json`, `Themes/*.mixin.theme`,
`OrderDesk.dll` and its dependencies. `docs/` and `deploy/` are documentation; the csproj lists them as
`None` items so Visual Studio shows them, and nothing copies them to the output.

## Verification after the first deployment

1. `GET /health` returns `"ok":true` **from the target**, not from localhost (storage root writable, mixin found).
2. Two browsers, two users (kelly and sam): the audit grid of each shows the other's sign-in — proves one process,
   separate sessions, shared audit log.
3. Restart the instance: uploads, exports and today's log are still there (storage root outside the build output).
4. Roll back: redeploy the previous build folder / image tag; the storage root is untouched by a rollback.
