# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 12

Local lab build for **Advanced Module 12 — Cloud, Containers, Load Balancing & Release Engineering**.
It is the walkthrough's `ReleaseDashboardPage`: **Deploy…** / **Rollback…**, the eight-chip release runbook strip
and the node grid read from `HealthCheck.json`, plus the deployment package the lab asks for (environment-specific
`appsettings*.json`, `HealthCheck.json`, `/healthz` probes, container / reverse-proxy / load-balancer notes and the
runbook documents).

Everything runs in this one process. **No container is built and no server other than this app is started** —
`deployment/Dockerfile`, `deployment/docker-compose.yml` and `deployment/nginx.conf` are deliverables to read.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Enterprise Wisej.NET Course/Module 12/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5212
```

Then open <http://localhost:5212>. (Visual Studio: open `EnterpriseOps.slnx`, press F5.) The project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

The host reads `ASPNETCORE_ENVIRONMENT` (the launch profile sets `Development`; `Startup.cs` falls back to it).
Set it to `Production` without the `EnterpriseOps__*` secrets and the process exits at startup with the
configuration errors — the fail-fast rule the module teaches.

| URL | What it is |
|---|---|
| <http://localhost:5212/healthz> | readiness — 200 with the probe body, 503 once a critical check fails |
| <http://localhost:5212/healthz/live> | liveness — 200 while the process can run code |
| <http://localhost:5212/HealthCheck.json> | **not served** — `Startup.cs` keeps every `.json` out of the file server |

## What to try

| Action | What you should see |
|---|---|
| The page loads | The runbook chips are grey; both nodes are `Healthy · 2.4.2`; the dark status bar shows node, release and environment. |
| **Deploy…** (first time) | Chips 1–3 turn green, **chip 4 (smoke tests) red**: the 2.4.2 database migration does not complete on `app-node-B`. The node turns red with `ROUTED AWAY — no new sessions`, the dark line under the grid shows its probe body (`{ "status": "Unhealthy", … database: FAIL · storage: OK · websocket: OK }`), chip 8 is armed and the banner tells you to press Rollback. |
| **Rollback…** | Chip 8 turns `↩`; node B returns on `2.4.1`, the smoke tests re-run green, it rejoins the rotation; green banner *"… Users never saw a broken node."* |
| **Deploy…** again | The fix has shipped: chips 1→7 turn green one at a time (pushed over WebSocket while the service works); chip 8 stays grey with *rollback not needed*; green banner *Release 2.4.2 live on both nodes*. |

Deploy and Rollback are refused server-side (`ReleaseAuthorization`) for anyone who is not a Manager or Admin —
the button being visible is not a permission.

## Where things live

```
Module 12/
├─ EnterpriseOps.slnx
├─ deployment/                       sample container + proxy notes — READ, never built or run by this lab
│  ├─ DeploymentReleasePatterns.md   the walkthrough's document: config table + runbook, and the index
│  ├─ Dockerfile · Dockerfile.notes.md
│  ├─ docker-compose.yml
│  ├─ nginx.conf · nginx.proxy.notes.md
│  └─ LoadBalancerNotes.md           sticky sessions, draining, scaling, health-aware routing
└─ EnterpriseOps/
   ├─ Startup.cs                     configuration → StartupValidation (fail fast) → HealthCheck.json →
   │                                 UseForwardedHeaders → UseWisej → /healthz + /healthz/live → file server
   ├─ Program.cs                     session entry point
   ├─ appsettings.json · appsettings.{Development,Staging,Production}.json   — no secrets, ever
   ├─ HealthCheck.json               the probe contract (endpoints, thresholds, checks, safeToExpose)
   ├─ UI/ReleaseDashboardPage.cs + .Designer.cs
   ├─ Domain/                        ReleaseNode (HealthReport, NodeRow) · Runbook · WorkOrder
   ├─ Data/WorkOrderRepository.cs    the "known query returns rows" check
   ├─ Security/ReleaseAuthorization.cs  who may deploy and roll back
   ├─ Services/                      HostConfiguration · StartupValidation · HealthProbeService ·
   │                                 LoadBalancerSimulator · SmokeTestService · ReleaseService ·
   │                                 ActivityTrace (server log) · SessionContext
   └─ docs/                          the five deliverables (below)
```

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Open the project and run it locally | `EnterpriseOps.slnx`, `dotnet run -f net10.0 --urls http://localhost:5212` |
| Environment-specific configuration | `appsettings*.json`, enforced by `Services/StartupValidation.cs` from `Startup.cs` |
| **Deployment architecture diagram** | `EnterpriseOps/docs/DeploymentArchitecture.md` + `.svg` |
| **Environment configuration table** | `EnterpriseOps/docs/EnvironmentConfiguration.md` |
| **HealthCheck.json** | `EnterpriseOps/HealthCheck.json` (read by `HealthProbeService`), annotated in `docs/HealthCheck.md` |
| **Release runbook** | `EnterpriseOps/docs/ReleaseRunbook.md`; executed by `ReleaseService.DeployAsync` |
| **Rollback and smoke test checklist** | `EnterpriseOps/docs/RollbackAndSmokeTestChecklist.md`; executed by `SmokeTestService.RunAsync` and `ReleaseService.RollbackAsync` |
| Sample container notes | `deployment/Dockerfile`, `deployment/docker-compose.yml`, `deployment/Dockerfile.notes.md` |
| Reverse-proxy notes | `deployment/nginx.conf`, `deployment/nginx.proxy.notes.md`, `Startup.cs` `UseForwardedHeaders` |
| Sticky-session / load-balancer guidance | `deployment/LoadBalancerNotes.md`, `Services/LoadBalancerSimulator.cs` |
| Show every path without leaking internals | the failed first deploy + Rollback; `ReportFailure` (generic message + correlation id); the probe body honours `safeToExpose` |

## Student review questions

**What requires sticky sessions?** Rich **in-process** session state: a Wisej.NET session lives in one node's
memory and its WebSocket stays open to that node. `deployment/LoadBalancerNotes.md` lists the mechanisms and the
drain-before-scale-in rule.

**Where are secrets stored?** In the platform's secret store, injected as `EnterpriseOps__ConnectionString` and
`EnterpriseOps__IdentityProvider__ClientSecret` environment variables at deployment time — absent on purpose from
`appsettings.Production.json`. `StartupValidation` refuses to boot without them.

**How do operations know a node is unhealthy?** `GET /healthz` answers **503** as soon as a critical check fails,
so the balancer drops the node after `probe.unhealthyThreshold` failures; the dashboard paints it red with the
failing check and monitoring alerts on the 503 rate. Liveness (`/healthz/live`) is separate, so an orchestrator
restarts only genuinely dead processes.

## Verified

`dotnet build -nologo -v q` → 0 warnings, 0 errors, both target frameworks. `/healthz`, `/healthz/live`, the 404
for `/HealthCheck.json` and the Production fail-fast boot were verified with curl on 2026-09-10.
