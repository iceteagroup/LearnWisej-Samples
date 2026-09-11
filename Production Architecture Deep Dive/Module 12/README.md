# TicketOps · Production Architecture Deep Dive · Module 12

Local lab build for **Module 12 · Deployment, Diagnostics, Load Balancing & Capstone Delivery** — the capstone's
release package. It follows the walkthrough video *Prepare the capstone for release*: a `HealthCheck.json`
manifest the load balancer can probe, a **role-protected diagnostics page** (server, port, runtime mode, product
version, sessions, WebSocket state, uptime, live health checks — and no secrets), the sticky-sessions story for
scaling out, and the written package: deployment checklist, release notes with a rollback plan, and the demo
script that ties the twelve modules together.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 12\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5112
```

Then open <http://localhost:5112> (or open `TicketOps.slnx` in Visual Studio and press F5). The probe's view of the
node is <http://localhost:5112/HealthCheck.json>.

## The screen

**TicketOps — Diagnostics** (`Diagnostics/DiagnosticsPage` inside `Views/ReleaseConsole`): the role header, **↻ Refresh**,
the runtime card, the health-check card with the overall status and HTTP code, and the health JSON. The first
refresh runs one client round-trip after load, because `Application.IsWebSocket` is still false during `Load`.
The session signs in as `m.weber` (Supervisor); any other role gets *Access denied* and no data. Open a second tab
and refresh: **Sessions** reads `2 live on this node`.

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | `HealthCheck.json` | [`TicketOps/HealthCheck.json`](TicketOps/HealthCheck.json) — read by `Infrastructure/FileHealthCheckSource`, served to probes at `GET /HealthCheck.json` (the one `.json` exception in `Startup.cs`) |
| 2 | Deployment checklist | [`docs/DeploymentChecklist.md`](TicketOps/docs/DeploymentChecklist.md) |
| 3 | Diagnostics page | `Diagnostics/DiagnosticsPage`, `Services/DiagnosticsService.cs` (role gate and snapshot), `Infrastructure/WisejRuntimeInfo.cs` |
| 4 | Release notes and rollback notes | [`docs/ReleaseNotes.md`](TicketOps/docs/ReleaseNotes.md) |
| 5 | Capstone demo script | [`docs/DemoScript.md`](TicketOps/docs/DemoScript.md) |
| — | Load-balancing note and diagram | [`docs/LoadBalancingNotes.md`](TicketOps/docs/LoadBalancingNotes.md), [`docs/sticky-sessions.svg`](TicketOps/docs/sticky-sessions.svg) |
| — | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |

The video's diagnostics page lists Environment, Version, Uptime, Sessions, Logging and Last error; this page shows
the runtime facts Wisej.NET exposes (server, port, runtime mode, version, framework, sessions, WebSocket, uptime)
and takes the environment from the manifest line under **Status**.

## Where things live

```
TicketOps/
├─ HealthCheck.json       the manifest: app, version, build, environment, checks (database, storage, websocket)
├─ Startup.cs             Kestrel host; the file server refuses .json except /HealthCheck.json
├─ Views/ReleaseConsole   the window hosting the diagnostics page
├─ Diagnostics/           DiagnosticsPage (display only)
├─ Services/              HealthCheckService, DiagnosticsService, IRuntimeInfo, IUserContext, IHealthCheckSource
├─ Domain/                HealthReport (200/503 rule), DiagnosticsSnapshot, OperatorRole, Ticket, OperationResult
├─ Data/                  ITicketRepository + in-memory implementation (the database probe's target)
├─ Infrastructure/        WisejRuntimeInfo, FileHealthCheckSource, HealthCheckJson, SessionUserContext, ILog/ActivityLog, AppComposition
└─ docs/                  DeploymentChecklist, ReleaseNotes, LoadBalancingNotes (+ SVG), DemoScript, ProductionReadinessNote
```
