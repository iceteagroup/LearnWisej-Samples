# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 12

Local lab build for **Advanced Module 12 — Cloud, Containers, Load Balancing & Release Engineering**.
It is the walkthrough's `ReleaseDashboardPage`: the eight-chip release runbook strip, node health read from
`HealthCheck.json`, **Deploy…** / **Rollback…**, the environment configuration table read from the real
`appsettings*.json`, and the live activity trace that shows which layer decided what.

The module's failure paths are all reachable from the bottom button bar: a node that fails its health check and
is routed away, a balancer without session affinity, an environment that would refuse to boot because a secret is
missing, and a release refused because a Technician pressed Deploy.

Everything runs in this one process. **No container is built and no server other than this app is started** —
`deployment/Dockerfile`, `deployment/docker-compose.yml` and `deployment/nginx.conf` are deliverables to read.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Enterprise Wisej.NET Course/Module 12/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5212
```

Then open <http://localhost:5212>. (Visual Studio: open `EnterpriseOps.slnx`, press F5.)

The host reads `ASPNETCORE_ENVIRONMENT`; the launch profile sets it to `Development`, and `Startup.cs` falls back to
`Development` when the variable is absent (so `dotnet run --no-launch-profile` still boots). Set it to `Production`
without the `EnterpriseOps__*` secrets and the process exits at startup with the two configuration errors — verified
2026-09-10: `[startup] configuration error: missing EnterpriseOps:ConnectionString …` — which is the fail-fast rule
the module teaches. `/healthz`, `/healthz/live` (200, JSON) and the 404 for `/HealthCheck.json` and
`/appsettings.Production.json` were also verified with curl.

Two extra endpoints the browser can hit directly:

| URL | What it is |
|---|---|
| <http://localhost:5212/healthz> | readiness — 200 with the probe body, 503 once a critical check fails |
| <http://localhost:5212/healthz/live> | liveness — 200 while the process can run code |
| <http://localhost:5212/HealthCheck.json> | **not served** — `Startup.cs` keeps every `.json` out of the file server |

The project multi-targets `net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## What to click

| Action | Path | What you should see |
|---|---|---|
| The page loads | – | Header shows `env Development · node app-node-A`; the runbook chips are grey; both nodes are `Healthy · 2.4.2`; the configuration table lists nine settings across three environments; the trace opens with `Host:`, `Health:` and `Security:` lines. |
| **Deploy…** | success + progress | Chips 1→7 turn green one at a time (the page pushes over WebSocket while the service works); chip 8 stays grey with *rollback not needed*; green banner *Release 2.4.2 live on both nodes*; toast top-right. |
| **Run smoke tests** | success | Seven `Health: smoke n. …: PASS` trace lines and a green banner with the elapsed time — the checklist of `docs/RollbackAndSmokeTestChecklist.md`, run in-process against this node. |
| **Fail: node B health check** | **failure 1** | Arms the 2.4.2 migration fault and runs the same deploy: chips 1–3 green, **chip 4 red** (smoke checks 3 and 4 — the health probe and the known query — both fail), chip 8 amber and armed; `app-node-B` turns red with `ROUTED AWAY — no new sessions`; the dark line shows `{ "status": "Unhealthy", … database: FAIL · storage: OK · websocket: OK }`. The release never reaches step 5, which is the point of running smoke tests before the health gate. |
| **Rollback…** | **recovery 1** | Chip 8 turns `↩`; node B returns on `2.4.1`, the smoke tests re-run green, it rejoins the rotation, and the banner ends *"Users never saw a broken node."* |
| **Fail: balancer without affinity** | **failure 2** | Affinity off; this session's next request is routed round-robin, lands on the other node and the trace reads `app-node-B has no session … in memory → a NEW session starts`. |
| (the same button, now **Recover: sticky sessions on**) | **recovery 2** | Affinity back on; the request returns to the pinned node and the banner explains why that is a requirement, not an optimisation. |
| Pick **Production** in the environment combo | **failure 3** | Red banner: `missing EnterpriseOps:ConnectionString — expected from environment variable … (vault / platform secret store)`; the trace shows one `StartupValidation REJECTED:` line per broken rule. This is exactly what `Startup.cs` throws on a real production host. |
| **Recover: inject platform secrets** | **recovery 3** | The same preview with the two environment variables supplied — *startup validation passed*, the node would boot and join the balancer. |
| **Sign in as ben.tech**, then **Deploy…** | **failure 4** | The button stays enabled on purpose; the **service** refuses: `Security: ben.tech (Technician) may NOT deploy — rule: Manager or Admin`. A hidden button is not a permission. |
| **Clear trace** | – | Empties the activity trace. |

## Where things live

```
Module 12/
├─ EnterpriseOps.slnx
├─ deployment/                       sample container + proxy notes — READ, never built or run by this lab
│  ├─ DeploymentReleasePatterns.md   the walkthrough's document: config table + runbook, and the index
│  ├─ Dockerfile                     multi-stage image, liveness HEALTHCHECK, no secrets
│  ├─ Dockerfile.notes.md            review checklist for the image + what must never go in it
│  ├─ docker-compose.yml             proxy + two nodes + shared volume; rolling-release commands
│  ├─ nginx.conf                     WebSocket upgrade, forwarded headers, ip_hash, probe ACL
│  ├─ nginx.proxy.notes.md           why each line exists; Apache and IIS equivalents
│  └─ LoadBalancerNotes.md           sticky sessions, draining, scaling, health-aware routing
└─ EnterpriseOps/
   ├─ Startup.cs                     configuration → StartupValidation (fail fast) → HealthCheck.json →
   │                                 UseForwardedHeaders → UseWisej → /healthz + /healthz/live → file server
   ├─ Program.cs                     session entry point; counts the session for the sessionStore check
   ├─ appsettings.json               base contract — no secrets, ever
   ├─ appsettings.Development.json   everything local
   ├─ appsettings.Staging.json       test values; the IdP secret is deliberately absent
   ├─ appsettings.Production.json    no connection string, no secret — both come from the environment
   ├─ HealthCheck.json               the probe contract (endpoints, thresholds, checks, safeToExpose)
   ├─ UI/ReleaseDashboardPage.cs         thin handlers, painting only
   ├─ UI/ReleaseDashboardPage.Designer.cs   the screen the walkthrough designs (dgvNodes, the chip strip…)
   ├─ Domain/ReleaseNode.cs          HealthReport, HealthCheckResult, ReleaseNode, NodeRow
   ├─ Domain/Runbook.cs              the eight RunbookSteps and the ReleasePackage
   ├─ Domain/WorkOrder.cs            the shared EnterpriseOps vocabulary
   ├─ Data/WorkOrderRepository.cs    60 in-memory rows — the "known query returns rows" check
   ├─ Security/ReleaseAuthorization.cs  who may deploy and roll back
   ├─ Services/HostConfiguration.cs  what THIS process booted with
   ├─ Services/StartupValidation.cs  the configuration contract, enforced before the host is built
   ├─ Services/ConfigurationService.cs  the environment table + the "preview a boot" failure path
   ├─ Services/HealthProbeService.cs  runs the HealthCheck.json checks; answers /healthz
   ├─ Services/LoadBalancerSimulator.cs  two nodes, affinity, health-aware routing
   ├─ Services/SmokeTestService.cs   the seven-check smoke test, in-process
   ├─ Services/ReleaseService.cs     the eight runbook steps, executed
   ├─ Services/ActivityTrace.cs      one line per layer decision
   ├─ Services/SessionContext.cs     per-session identity, CommandContext, ServiceRegistry
   └─ docs/
      ├─ DeploymentArchitecture.md / .svg     deliverable 1
      ├─ EnvironmentConfiguration.md          deliverable 2
      ├─ HealthCheck.md                       deliverable 3 (annotates ../HealthCheck.json)
      ├─ ReleaseRunbook.md                    deliverable 4
      └─ RollbackAndSmokeTestChecklist.md     deliverable 5
```

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Open the project and run it locally | `EnterpriseOps.slnx`, `dotnet run -f net10.0 --urls http://localhost:5212` |
| Lab goal — a deployment package with environment-specific configuration | `appsettings*.json` + `Services/ConfigurationService.cs`, surfaced on `ReleaseDashboardPage` |
| **Deliverable · Deployment architecture diagram** | `EnterpriseOps/docs/DeploymentArchitecture.md` + `DeploymentArchitecture.svg` |
| **Deliverable · Environment configuration table** | `EnterpriseOps/docs/EnvironmentConfiguration.md`; live in `dgvConfig` via `ConfigurationService.BuildTable()` |
| **Deliverable · HealthCheck.json** | `EnterpriseOps/HealthCheck.json` (read by `HealthProbeService`), annotated in `docs/HealthCheck.md` |
| **Deliverable · Release runbook** | `EnterpriseOps/docs/ReleaseRunbook.md`; executed by `Services/ReleaseService.DeployAsync` |
| **Deliverable · Rollback and smoke test checklist** | `EnterpriseOps/docs/RollbackAndSmokeTestChecklist.md`; executed by `SmokeTestService.RunAsync` and `ReleaseService.RollbackAsync` |
| Sample container notes | `deployment/Dockerfile`, `deployment/docker-compose.yml`, `deployment/Dockerfile.notes.md` |
| Reverse-proxy notes | `deployment/nginx.conf`, `deployment/nginx.proxy.notes.md`, `Startup.cs` `UseForwardedHeaders` |
| Sticky-session / load-balancer guidance | `deployment/LoadBalancerNotes.md`, `Services/LoadBalancerSimulator.cs`, the **Fail: balancer without affinity** button |
| A real `/healthz` probe endpoint | `Startup.cs` — `app.MapGet("/healthz", …)` and `app.MapGet("/healthz/live", …)` |
| Show every path (success, validation, error) | the eight buttons in the table above; every one of them writes to the trace |
| Review & run · production-readiness note | this README's *Instructor acceptance criteria* section and `deployment/DeploymentReleasePatterns.md` |

The site's lab code check greps the handler for: an event handler, `await`/`Task`, a service call, `try`/`catch`,
and a release-engineering word. `btnDeploy_Click` in `UI/ReleaseDashboardPage.cs` satisfies all five:

```csharp
private async void btnDeploy_Click(object sender, EventArgs e)
{
    BeginBusy("deploying…");
    try
    {
        var result = await _release.DeployAsync(CurrentContext, ShowStep);
        ShowReleaseResult(result);
    }
    catch (Exception ex)
    {
        ReportFailure(ex);
    }
    finally
    {
        EndBusy();
    }
}
```

## Student review questions, answered against this sample

**What requires sticky sessions?**
Rich **in-process** session state. A Wisej.NET session — the open page, the grid, the wizard step, the unsaved
edits — lives in the memory of one node, and its server-push WebSocket stays open to that same node. Press
**Fail: balancer without affinity**: the next request is routed round-robin, lands on `app-node-B`, and the trace
says `app-node-B has no session … in memory → a NEW session starts`. `deployment/LoadBalancerNotes.md` lists the
mechanisms (route cookie preferred, `ip_hash` acceptable, ARR affinity / ALB stickiness on the platforms) and the
drain-before-scale-in rule.

**Where are secrets stored?**
In the platform's secret store (vault, Key Vault, Secrets Manager, a docker secret), injected as
`EnterpriseOps__ConnectionString` and `EnterpriseOps__IdentityProvider__ClientSecret` environment variables at
deployment time. They are **absent on purpose** from `appsettings.Production.json`, so a leaked repository
exposes host names and log levels and nothing else. `StartupValidation` refuses to boot without them — select
**Production** in the environment combo to see the exact error, then **Recover: inject platform secrets** to see
the same preview pass.

**How do operations know a node is unhealthy?**
`GET /healthz` answers **503** as soon as a critical check fails, so the balancer drops the node out of rotation
after `probe.unhealthyThreshold` failures — automatically, before a human is involved. In parallel the release
dashboard paints the node red with the failing check, the trace records
`Balancer: app-node-B failed readiness (…) — routed away`, and monitoring alerts on the 503 rate. Liveness is a
*separate* endpoint (`/healthz/live`) so an orchestrator restarts only genuinely dead processes.
Sessions already pinned to the node keep being served until they drain; only **new** sessions are steered away.

## Instructor acceptance criteria, answered against this sample

| Criterion | How this sample meets it |
|---|---|
| The implementation follows the course architecture baseline | Folder-per-layer (`UI`, `Domain`, `Services`, `Data`, `Security`, `docs`), namespaces follow folders, typed commands and results (`CommandContext`, `ReleaseResult`, `SmokeTestRun`, `NodeRow`), per-session services created in the page constructor. |
| UI event handlers remain thin and explainable | Every handler is 3–12 lines: `BeginBusy` → one `await _service.…` → `Show…(result)` → `catch` → `finally EndBusy`. The failure-path buttons only flip a flag and call the same handler. |
| Service-level logic can be reviewed without opening the designer | Who may release (`ReleaseAuthorization`), what a step means (`ReleaseService`), what healthy means (`HealthProbeService` + `HealthCheck.json`), what the balancer does (`LoadBalancerSimulator`), whether an environment may boot (`StartupValidation`) — none of it is in a designer file. |
| At least one failure path is demonstrated | Four: a failed health check on node B (routed away, rollback armed), a balancer without affinity (session lost), a missing production secret (the host would refuse to boot), and a Technician pressing Deploy (refused server-side). Each has a matching recovery. |
| The student can explain state ownership, security implications and production behaviour | **State ownership:** session state is per-node and in-process (`SessionContext`, never a static); process-wide facts (`HostConfiguration`, `HealthProbeService`) are static on purpose and hold nothing user- or tenant-specific. **Security:** secrets never in files or the image; `AllowedHosts` may not be `*` and logging may not be `Debug` in Production; the probe body honours `safeToExpose` (no exception messages, no connection strings); permissions enforced in the service, not by disabling a button; forwarded headers trusted narrowly in production. **Production behaviour:** rolling release one node at a time, drain before stop, readiness evicts while liveness restarts, rollback is runbook step 8 with a named retained artifact. |

## Self-check

- `dotnet build -nologo -v q` → **0 warnings, 0 errors**, both target frameworks.
- Every button writes at least one `Service:` / `Health:` / `Balancer:` / `Runbook:` trace line, so no decision
  happens invisibly in a handler.
- The configuration table in the app is generated from the same `appsettings*.json` files the host reads, so the
  deliverable cannot drift from the code.
- No container is built, no external server is started, no network call is made, no credential is real.

## Verified / unverified

Used from `_template/COOKBOOK.md`:

| Item | Status |
|---|---|
| `Application.Update(this)` after an `await` in an `async void` handler, bounded to ≥ 250 ms, guarded by `IsDisposed` / `ObjectDisposedException` | **verified** in the cookbook |
| `AlertBox.Show(text, icon, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000)` | **verified** |
| `Application.Session.Context = sessionContext`, `Application.SessionId` | **verified** |
| `DataGridView` with `AutoGenerateColumns = false`, explicit columns, `DataSource = new BindingSource { DataSource = rows }`, `Rows[i].DefaultCellStyle` | **verified** |
| Fonts `"default"` / `"monospace"`, `Label.TextAlign`, `Panel.BorderStyle = BorderStyle.Solid` | **verified** |
| `Wisej.Web.FlowLayoutPanel` with `FlowDirection` + `WrapContents` for the runbook chip strip | **verified** (used in the Production Architecture course samples) |
| `Wisej.Web.ComboBoxStyle.DropDownList` | **verified** (used in several course samples) |
| **`app.MapGet("/healthz", …)` after `builder.Build()` coexisting with `app.UseWisej()`** — Wisej owns only its `*.wx` paths | **unverified** — compiles and is wired exactly as the cookbook describes; the reviewer should hit `/healthz` and `/healthz/live` in the browser and confirm 200 / 503 and that the app itself still loads |
| **Reading environment configuration from `appsettings.{Environment}.json` via `builder.Configuration`** alongside Wisej's own `Default.json` | **unverified** in the cookbook; exercised at startup here (`Startup.cs` prints `[startup] environment=… node=… release=…` to the console) |
| `Label.Margin` / `Label.Padding` inside a `FlowLayoutPanel` for chip spacing | **unverified** — builds; if the chips are cramped at runtime the sizes are all in `ReleaseDashboardPage.Designer.cs` |

Known cosmetic risk: the chip strip assumes four 180 px chips per row inside a 772 px `FlowLayoutPanel`. If a
theme's default label padding pushes a chip to a third row, widen `flowRunbook` or shrink the chips — no code
depends on the layout.
