# Capstone demo script — "the capstone ships" (≈ 5 minutes)

*Module 12 deliverable · what to open and click, in order. Every step touches a module of this course.*

Each module of the Production Architecture Deep Dive is its own runnable app (see the course `README.md`), so the
demo runs the modules side by side — each module's `README.md` describes its screen. Start every app from its
`TicketOps` folder with `dotnet run -f net10.0 --urls http://localhost:51NN`.

| # | Step (what the video's script says) | Open | Show | Module(s) it proves |
|---|---|---|---|---|
| 1 | **Sign in — identity & role** | <http://localhost:5111> | the login gate, a Supervisor vs a Technician, the permission service refusing a force-enabled Delete at the *service* level, the safe HTML policy | 11 · Security |
| 2 | **Create and approve a work order** | <http://localhost:5104>, then <http://localhost:5106> | the bound Work Order grid (search, dirty tracking, formatted columns), then `ApprovalDialog` returning a typed `ApprovalDialogResult` through `ApprovalService` — and `SaveCommand`'s validation summary if you type a bad value (5105) | 4 · Binding, 5 · Validation, 6 · Dialogs |
| 3 | **Import the CSV with progress & cancel** | <http://localhost:5107> | `Application.StartTask` + `Application.Update` pushing progress over the WebSocket, per-row errors, cancellation mid-run | 7 · Background tasks |
| 4 | **Show HealthCheck.json & diagnostics** | <http://localhost:5112> (this module) | see the detailed script below | 12 · Deployment, 2 · Session state, 8 · Composition |
| 5 | **Show the audit log** | back to <http://localhost:5111> | the audit entries the earlier steps produced; who did what, when | 11 · Security |

Closing line: *"Hosting chosen and written down, health probed, diagnostics role-protected and secret-free, sticky
sessions explained, release notes and a rollback plan in the package — the capstone is a deliverable someone can
operate, not just run."*

## Step 4 in detail — Module 12 (this app), ≈ 2 minutes

Open <http://localhost:5112>. Give the page a moment: the first refresh is deferred one round-trip so the WebSocket
check is honest. Read the page aloud:

1. **The role header** — *Role required: Supervisor · ✓ m.weber (Supervisor)*. Say: "The role check runs in
   `DiagnosticsService`, not in a button — any other role gets Access denied and no data."
2. **RUNTIME card** — server, port **5112**, runtime mode (`debug` here; `release` in production), product
   version, framework, **Sessions** (`1 live on this node`), **WebSocket** (`connected`), **Uptime**.
   Say: "Everything here comes through an `IRuntimeInfo` interface; nothing is a secret."
3. **HEALTH CHECKS card** — `database OK · 6 rows reachable`, `storage OK`, `websocket OK`; `Status: Healthy → HTTP 200 (in rotation)`;
   the manifest line `TicketOps Console v1.0.0 · build 2026.09.10.1 · Development`. Say: "Version proof after a deploy."
4. **The JSON box** — the report as a probe would read it. In a second tab open <http://localhost:5112/HealthCheck.json>:
   the static manifest the load balancer polls (the only `.json` the file server answers; `/Default.json` stays blocked).
5. **↻ Refresh** in the first tab after opening the second one: **Sessions** shows `2 live on this node` — the
   number a restart would evict.
6. Open `docs/DeploymentChecklist.md`, `docs/ReleaseNotes.md` (with the **Rollback** table) and `docs/LoadBalancingNotes.md`
   (the sticky-sessions diagram). Say: "the package, not just the app."

## Evidence to capture for the submission

- Screenshot A: the diagnostics page **Healthy** (1–2 sessions, WebSocket connected).
- Screenshot B: `GET /HealthCheck.json` in a plain browser tab (the probe's view).
- The three docs above plus `HealthCheck.json` in the repo.
