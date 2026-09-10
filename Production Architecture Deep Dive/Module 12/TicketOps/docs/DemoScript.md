# Capstone demo script — "the capstone ships" (≈ 5 minutes)

*Module 12 deliverable · what to open and click, in order. Every step touches a module of this course.*

Each module of the Production Architecture Deep Dive is its own runnable app (see the course `README.md`), so the
demo runs the modules side by side — the exact button names of each are in that module's `README.md` under
**What to click**. Start every app from its `TicketOps` folder with `dotnet run -f net10.0 --urls http://localhost:51NN`.

| # | Step (what the video's script says) | Open | Show | Module(s) it proves |
|---|---|---|---|---|
| 1 | **Sign in — identity & role** | <http://localhost:5111> | the login gate, a Supervisor vs a Technician, the permission service refusing a Technician at the *service* level, the safe HTML policy | 11 · Security |
| 2 | **Create and approve a work order** | <http://localhost:5104>, then <http://localhost:5106> | the bound Work Order grid (search, dirty tracking, formatted columns), then `ApprovalDialog` returning a typed `ApprovalDialogResult` through `ApprovalService` — and `SaveCommand`'s validation summary if you type a bad value (5105) | 4 · Binding, 5 · Validation, 6 · Dialogs |
| 3 | **Import the CSV with progress & cancel** | <http://localhost:5107> | `Application.StartTask` + `Application.Update` pushing progress over the WebSocket, per-row errors, cancellation mid-run | 7 · Background tasks |
| 4 | **Show HealthCheck.json & diagnostics** | <http://localhost:5112> (this module) | see the detailed script below | 12 · Deployment, 2 · Session state, 8 · Composition |
| 5 | **Show the audit log** | back to <http://localhost:5111> | the audit entries the earlier steps produced; who did what, when | 11 · Security |

Closing line: *"Hosting chosen and written down, health probed, diagnostics role-protected and secret-free, sticky
sessions explained, release notes and a rollback plan in the package — the capstone is a deliverable someone can
operate, not just run."*

## Step 4 in detail — Module 12 (this app), ≈ 2 minutes

Open <http://localhost:5112>. Give the page a moment: the first refresh is deferred one round-trip so the WebSocket
check is honest. Read the left card aloud:

1. **The role header** — *Role required: Supervisor · ✓ m.weber (Supervisor)*. Say: "Technicians see Access denied — we'll prove it."
2. **RUNTIME card** — server, port **5112**, runtime mode (`debug / design` here; `release` in production), product
   version, framework, **Sessions** (`1 live on this node`), **WebSocket** (`connected`), **Uptime**.
   Say: "Everything here is `Wisej.Web.Application.*` behind an `IRuntimeInfo` interface; nothing is a secret."
3. **HEALTH CHECKS card** — `database OK · 6 rows reachable`, `storage OK`, `websocket OK`; `Status: Healthy → HTTP 200 (in rotation)`;
   the manifest line `TicketOps Console v1.0.0 · build 2026.09.10.1 · Development`. Say: "Version proof after a deploy."
4. **The JSON box** — the report as a probe would read it. In a second tab open <http://localhost:5112/HealthCheck.json>:
   the static manifest the load balancer polls (the only `.json` the file server answers; `/Default.json` stays blocked).
5. **✓ Run health check** (bottom bar) — success path. Point at the trace:
   `[UI] → IHealthCheckService.CheckAsync()` → `[INFRA] File.ReadAllText(Application.MapPath("HealthCheck.json"))` →
   `[DATA] 6 rows` → `[SVC] database → OK …` → `[SVC] Healthy · … → HTTP 200` → `[UI] Healthy · HTTP 200 shown`.
6. **▶ Simulate load (40 units)** — progress path. The bar and *load test: n/40 work units open…* climb; each unit is a
   real `ITicketService.SaveAsync`. When it finishes, diagnostics refresh: `database → OK (46 rows reachable)`, still **Healthy**.
   Optionally open a third tab and press **↻ Refresh**: **Sessions** shows `2 live on this node` — the number a restart would evict.
7. **Sign in as Technician** — failure path (permission). Status **● access denied**, banner
   *"Access denied. The diagnostics page requires the Supervisor role."*, every value `—`; trace `[SVC] ⚠ access denied for l.romero …`.
   Click **Sign back in as Supervisor** — the page is back.
8. **Degrade 'storage'** — failure path (dependency). `storage Degraded · disk free 9% (threshold 15%)`,
   `Status: Degraded → HTTP 200 (in rotation)`, orange banner, status *still serving (6 open tickets loaded)* —
   say: "reported, but the node keeps serving and the app keeps working." Click **Restore 'storage'** → Healthy.
9. **Simulate repository outage** — error path. `database Unhealthy · ticket store unreachable`, `Status: Unhealthy → HTTP 503 (out of rotation)`,
   red banner. In the trace: `[DATA] ✖ outage: SELECT * FROM Tickets failed — timeout connecting to sql01:1433 …` — say:
   "that host name is in the log and nowhere on the screen." Click **Recover the repository** → **● Healthy · HTTP 200 · in rotation**.
   Say: "that flip from 503 back to 200 is the signal the rollback plan watches."
10. Open `docs/DeploymentChecklist.md`, `docs/ReleaseNotes.md` (with the **Rollback** table) and `docs/LoadBalancingNotes.md`
    (the sticky-sessions diagram). Say: "the package, not just the app."

## Evidence to capture for the submission

- Screenshot A: the diagnostics page **Healthy** after the load test (46 rows, 1–2 sessions, WebSocket connected).
- Screenshot B: **Access denied** as Technician.
- Screenshot C: **Unhealthy · HTTP 503** with the trace showing `sql01:1433` only in the right-hand card.
- Screenshot D: `GET /HealthCheck.json` in a plain browser tab (the probe's view).
- The three docs above plus `HealthCheck.json` in the repo.
