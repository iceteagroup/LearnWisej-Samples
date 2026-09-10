# TicketOps · Production Architecture Deep Dive · Module 12

Local lab build for **Module 12 · Deployment, Diagnostics, Load Balancing & Capstone Delivery** — the capstone's
release package. It follows the walkthrough video *Prepare the capstone for release*: a `HealthCheck.json`
manifest the load balancer can probe, a **role-protected diagnostics page** (server, port, runtime mode, product
version, sessions, WebSocket state, uptime, live health checks — and no secrets), the sticky-sessions story for
scaling out, and the written package: deployment checklist, release notes with a rollback plan, and the demo
script that ties the twelve modules together.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine. The store is in-memory and
per session; the probes prove the pattern.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 12\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5112
```

Then open <http://localhost:5112>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.) The probe's view of the node is <http://localhost:5112/HealthCheck.json>.

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with no warnings for both targets (`net10.0-windows`, `net10.0`).

## What to click in the Diagnostics window

The left card is **TicketOps — Diagnostics** (`Diagnostics/DiagnosticsPage`); the right card is the
**Activity trace · UI → Service → Infra → Data**: every click is logged as it crosses a boundary
(`[UI]` → `[SVC]` → `[INFRA]` / `[DATA]` / `[SESSION]` → `[UI]`). The page loads, waits one client round-trip
(`Application.IsWebSocket` is still false during `Load`), then refreshes itself.

| Button | Path | What you should see |
|---|---|---|
| *(page load)* | — | `[INFRA] AppComposition — composing …`, `[SESSION] signed in: m.weber (Supervisor)`, then the first refresh: runtime card filled (server, port **5112**, mode, version, framework, `1 live on this node`, WebSocket `connected`, uptime), health card `database OK · 6 rows reachable`, `storage OK`, `websocket OK`, **Status: Healthy → HTTP 200 (in rotation)**; status **● Healthy · HTTP 200 · in rotation** |
| **↻ Refresh** (top-right of the page) | success | `[UI] → IDiagnosticsService.GetSnapshotAsync()` → `[SVC] authorize m.weber (Supervisor) — Supervisor required` → `[SVC] → IRuntimeInfo (…)` → `[SESSION] 1 live session(s) on <server>:5112 · this one <id> · WebSocket yes · up 0d 00:01:23` → `[SVC] → IHealthCheckService.CheckAsync()` → the health lines below → `[UI] … Healthy · HTTP 200` |
| **✓ Run health check** | success | `[UI] → IHealthCheckService.CheckAsync()`, `[INFRA] File.ReadAllText(Application.MapPath("HealthCheck.json"))`, `[INFRA] manifest: TicketOps Console v1.0.0 build 2026.09.10.1 (Development) · 3 dependencies declared`, `[DATA] 6 rows`, `[SVC] database → OK (6 rows reachable)`, `storage → OK (temp folder writable)`, `websocket → OK (WebSocket connection active)`, `[SVC] Healthy · database:OK, storage:OK, websocket:OK → HTTP 200`; the JSON box shows the report with `"httpStatusCode": 200` |
| **▶ Simulate load (40 units)** | progress | `[SVC] LoadTestService.Start — simulated load: 40 work units …`; a `Timer` opens 4 units per tick through `ITicketService.SaveAsync` (one `[SVC]`+`[DATA]` pair each); *load test: n/40 work units open…* and the bar climb; `[SVC] 10/40 … 40/40 work units open`; then a refresh: `database → OK (46 rows reachable)`, still **Healthy** |
| **Sign in as Technician** | failure 1 (permission) | `[SESSION] SignInAs — l.romero (Technician)`, `[SVC] ⚠ access denied for l.romero: diagnostics map the system, operators only` — no `[INFRA]`/`[DATA]` line follows; header **✖ l.romero (Technician) — access denied**, every value `—`, orange banner **Access denied. The diagnostics page requires the Supervisor role.**; status **● access denied**. Click **Sign back in as Supervisor** to recover |
| **Degrade 'storage'** | failure 2 (dependency) | `[SVC] ⚠ storage forced to Degraded — disk free 9% (threshold 15%)`, `[SVC] ⚠ Degraded · database:OK, storage:Degraded, websocket:OK → HTTP 200 (still serving)`; health row `storage · Degraded`; **Status: Degraded → HTTP 200 (in rotation)**; then `[UI] app keeps working while Degraded: 6 open tickets loaded through ITicketService`; status **● Degraded · HTTP 200 · still serving (6 open tickets loaded)**. Click **Restore 'storage'** → Healthy |
| **Simulate repository outage** | error path | `[DATA] ✖ outage: SELECT * FROM Tickets failed — timeout connecting to sql01:1433 (TicketOps.dbo.Tickets)`, `[SVC] ⚠ database probe caught DataOutageException → reported as Unhealthy, not thrown`, `[SVC] ⚠ Unhealthy · database:Unhealthy, … → HTTP 503 (out of rotation)`; health row `database · Unhealthy · ticket store unreachable`; red banner **A required dependency is unavailable. The node answers HTTP 503 …**; status **● Unhealthy · HTTP 503 · out of rotation**. `sql01` never reaches the left card |
| **Recover the repository** (same button) | recovery | `[DATA] 6 rows` (or 46 after the load test), `[SVC] Healthy … → HTTP 200`; status **● Healthy · HTTP 200 · in rotation** |
| **Clear trace** | — | empties the right-hand card |

**Two tabs.** Open <http://localhost:5112> in a second tab and press **↻ Refresh** in the first: **Sessions** reads
`2 live on this node` — each tab has its own `AppComposition` graph (its own log, repository, user), and the count
is what a restart or a redeploy would evict. That is the number sticky sessions protect.

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | `HealthCheck.json` (version, build, environment, dependencies with status) | [`TicketOps/HealthCheck.json`](TicketOps/HealthCheck.json) — read by `Infrastructure/FileHealthCheckSource` via `Application.MapPath`, served to probes at `GET /HealthCheck.json` (the one `.json` exception in `Startup.cs`; `Default.json` stays blocked) |
| 2 | Deployment checklist | [`docs/DeploymentChecklist.md`](TicketOps/docs/DeploymentChecklist.md) |
| 3 | Diagnostics page | `Diagnostics/DiagnosticsPage.cs` + `.Designer.cs` (display), `Services/DiagnosticsService.cs` (role gate + snapshot), `Infrastructure/WisejRuntimeInfo.cs` (`Application.ServerName/ServerPort/RuntimeMode/ProductVersion/SessionCount/SessionId/IsWebSocket`) |
| 4 | Release notes + rollback notes | [`docs/ReleaseNotes.md`](TicketOps/docs/ReleaseNotes.md) |
| 5 | Capstone presentation / demo script | [`docs/DemoScript.md`](TicketOps/docs/DemoScript.md) |
| — | Load-balancing note (why sticky sessions) + diagram | [`docs/LoadBalancingNotes.md`](TicketOps/docs/LoadBalancingNotes.md), [`docs/sticky-sessions.svg`](TicketOps/docs/sticky-sessions.svg) |
| — | Production-readiness note (lab step 9) | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |
| — | Health check with the 200/503 rule | `Domain/HealthReport.cs` (`Aggregate`, `HttpStatusCode`), `Services/HealthCheckService.cs` (probes), `Infrastructure/HealthCheckJson.cs` (the body a `/health` endpoint would return) |

## Where things live

```
TicketOps/
├─ HealthCheck.json                 the manifest: app, version, build, environment, checks (database, storage, websocket)
├─ Startup.cs                       Kestrel host; UseFileServer refuses .json except /HealthCheck.json (Default.json protected)
├─ Views/
│  ├─ ReleaseConsole.cs             the screen: thin handlers → IDiagnosticsService / IHealthCheckService / ILoadTestService
│  └─ ReleaseConsole.Designer.cs    layout (opens in the Wisej Designer) — no logic here
├─ Diagnostics/
│  ├─ DiagnosticsPage(.Designer).cs the role-protected diagnostics page: runtime card, health card, health JSON, Refresh (display only)
│  └─ ActivityTracePanel            the live trace card
├─ Services/
│  ├─ IHealthCheckService / HealthCheckService   read the manifest, probe each dependency, aggregate; failures are statuses, not exceptions
│  ├─ IDiagnosticsService / DiagnosticsService   Supervisor-only snapshot over IRuntimeInfo + the health check
│  ├─ ILoadTestService / LoadTestService         fake work units through ITicketService (the progress path)
│  ├─ IRuntimeInfo, IUserContext, IHealthCheckSource   the interfaces that keep Wisej.NET out of the services
│  └─ ITicketService / TicketService             from Module 1 (unchanged)
├─ Domain/
│  ├─ HealthReport.cs               DependencyCheck, HealthStatus, the Aggregate rule (Unhealthy → 503), HttpStatusCode
│  ├─ DiagnosticsSnapshot.cs        what the page shows (plain data, no secrets by construction)
│  ├─ OperatorRole.cs               Technician / Supervisor
│  └─ Ticket, TicketDraft, OperationResult      from Module 1 (unchanged)
├─ Data/                            ITicketRepository, InMemoryTicketRepository (SimulateOutage = the lab's outage switch)
├─ Infrastructure/
│  ├─ WisejRuntimeInfo.cs           the one adapter over Wisej.Web.Application.* (IRuntimeInfo)
│  ├─ FileHealthCheckSource.cs      File.ReadAllText(Application.MapPath("HealthCheck.json"))
│  ├─ HealthCheckJson.cs            System.Text.Json in/out for HealthReport
│  ├─ SessionUserContext.cs         who is signed in to this session (SignInAs for the lab)
│  ├─ ILog.cs / ActivityLog.cs      cross-cutting logging (details stay here)
│  └─ AppComposition.cs             who gets what: one object graph per session, constructor injection, no statics
├─ Controls/StatusBanner            "● state" + banner UserControl (display only)
├─ Resources/Strings.cs             safe user-facing messages (DiagnosticsAccessDenied, HealthDegraded, HealthUnhealthy, …)
└─ docs/                            DeploymentChecklist, ReleaseNotes (+ rollback), LoadBalancingNotes (+ SVG), DemoScript, ProductionReadinessNote
```

## Self-check answers (lesson guide)

- **Does the load balancer need sticky sessions?**
  Yes — mandatory, not optional. A Wisej.NET session (this form, its `ActivityLog`, its `InMemoryTicketRepository`)
  is a set of live .NET objects in one node's memory; a request routed to another node finds nothing and the user is
  dropped mid-action. Enable session affinity (a balancer cookie) **and** forward the WebSocket upgrade — see
  [`docs/LoadBalancingNotes.md`](TicketOps/docs/LoadBalancingNotes.md). The **Sessions** row on the diagnostics page
  is the per-node state affinity protects; the **WebSocket** row tells you whether the proxy forwards the upgrade.
- **Where are environment-specific values stored?**
  In the environment, never in code: `ASPNETCORE_URLS`/`--urls` for the port, a per-environment `Default.json`
  (`"debug": false` in production — the **Runtime mode** row shows `release` when it took), the manifest's
  `environment`/`version`/`build`, and platform settings (App Service settings, Key Vault, container env) for
  anything secret. Nothing secret has a place on the diagnostics page or in `HealthCheck.json` by construction.
- **What is the rollback plan if deployment fails?**
  Written before the deploy in [`docs/ReleaseNotes.md`](TicketOps/docs/ReleaseNotes.md): trigger
  (`/HealthCheck.json` non-200 or wrong `version` for 5 minutes, or the smoke test fails), owner (release engineer
  on duty), steps (announce the window, redeploy the retained previous artifact, restart, confirm the previous
  version at `/HealthCheck.json`, smoke-test), data (no migration; future ones must be additive). The app shows the
  signal the plan watches: **Simulate repository outage** → `Unhealthy · HTTP 503`, **Recover** → `Healthy · HTTP 200`.
- **Can you point to the health check and the diagnostics page?**
  `HealthCheck.json` + `Services/HealthCheckService.cs` (probes, `Domain/HealthReport.Aggregate` for the 200/503
  rule) and `Diagnostics/DiagnosticsPage` behind `Services/DiagnosticsService.cs` (Supervisor only).
