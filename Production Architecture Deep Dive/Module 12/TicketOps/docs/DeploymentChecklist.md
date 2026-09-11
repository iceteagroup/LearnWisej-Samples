# Deployment checklist — TicketOps Console

*Module 12 deliverable · walk it the same way every release, so releases stay boring*

## 0. Decide the host (once, and write the reason down)

| Host | Choose it when | What it changes for TicketOps |
|---|---|---|
| **IIS on Windows** | existing Windows/AD estate, familiar ops | published folder + `web.config`; the ASP.NET Core Module hosts Kestrel; **app-pool recycling evicts every Wisej.NET session on the node** — disable idle time-out and periodic recycle, or recycle in a drain window |
| **Kestrel + reverse proxy (NGINX/Apache, Linux)** | cross-platform, lean process, containers | you own TLS, process supervision (`systemd`) and **affinity + WebSocket upgrade at the proxy** |
| **Azure App Service / AWS (container)** | elastic scale, repeatable image builds | image or zip is the deploy unit; **enable ARR affinity / target-group stickiness before scaling to 2 instances** |

Decision for this capstone: **single Kestrel instance behind a reverse proxy** (Linux or Windows). It dodges the
affinity question until traffic forces a second node — and when it does, `docs/LoadBalancingNotes.md` is the plan.

## 1. Before deploy

| Step | Check | How to verify in this project |
|---|---|---|
| Version | build tagged; `HealthCheck.json` carries the new `version` and `build` | `TicketOps/HealthCheck.json` → `"version": "1.0.0", "build": "2026.09.10.1"`; the diagnostics page shows the same values under **Status** |
| Config | environment-specific values come from the environment, not from files in the package | `Default.json` has `"debug": true` for development only — production sets it false; the port comes from `--urls` / `ASPNETCORE_URLS`; no connection string exists in the repo (in-memory store) |
| Secrets | none in `HealthCheck.json`, none on the diagnostics page | grep the manifest and `DiagnosticsSnapshot` for `password`, `key`, `connection` → nothing; the diagnostics page renders `IRuntimeInfo` values only |
| Data | migrations reviewed and backward-safe | n/a for the in-memory fake; the release note still carries a **Migration** line so the habit survives |
| Health | the probe target exists and is served | `Startup.cs` serves `GET /HealthCheck.json` (the one `.json` exception; `Default.json` stays blocked) |
| Affinity | sticky sessions on if > 1 instance; WebSocket upgrade forwarded | `docs/LoadBalancingNotes.md` §2 — the proxy snippet; single instance today |
| Diagnostics | role-protected, secret-free | `DiagnosticsService.GetSnapshotAsync` refuses any role but Supervisor (`Strings.DiagnosticsAccessDenied`) |
| Smoke test | the key workflow is scripted | `docs/DemoScript.md` |
| Rollback | previous artifact retained; trigger and owner named | `docs/ReleaseNotes.md` → **Rollback** |
| Window | deploy in a low-traffic window; drain the node first | the diagnostics page's **Sessions** row is the number of users a restart evicts — check it before you recycle |

## 2. Deploy

```bash
cd "Module 12/TicketOps"
dotnet publish -c Release -f net10.0 -o ./publish     # HealthCheck.json ships with the output (Content, CopyToPublishDirectory=Always)
# copy ./publish to the node, then:
ASPNETCORE_URLS=http://0.0.0.0:5112 dotnet TicketOps.dll
```

## 3. After deploy

| Step | Check | Expected |
|---|---|---|
| Liveness | `curl -i http://<node>:5112/HealthCheck.json` | `200` and the JSON with the **new** `version`/`build` — proof the new build is the one answering, not a cached instance |
| Readiness | open the app → diagnostics page → **↻ Refresh** | `Status: Healthy → HTTP 200 (in rotation)`; `database` OK with a row count, `storage` OK, `websocket` OK |
| Sessions | diagnostics **Sessions** row | counts the live sessions on this node; open a second tab and refresh — it climbs |
| WebSocket | diagnostics **WebSocket** row | `connected`; `long-polling fallback` means the proxy dropped the upgrade headers |
| Affinity (2+ nodes) | sign in, work for a minute, refresh | the session stays; a lost session means the sticky cookie is missing |
| Config | **Runtime mode** row | `release` in production; `debug` means `Default.json` still has `"debug": true` |
| Smoke test | `docs/DemoScript.md` | passes end to end |
| Rollback trigger | watch `/HealthCheck.json` and the diagnostics page for 5 minutes | if the status stays `Unhealthy`/`503` or the smoke test fails → **roll back** (ReleaseNotes.md) |
