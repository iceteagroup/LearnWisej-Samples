# Release notes — TicketOps Console 1.0.0 (build 2026.09.10.1)

*Module 12 deliverable · structured: Added / Changed / Fixed / Ops / Migration / Rollback*

```
// TicketOps Console — Release 1.0.0 — build 2026.09.10.1 — 2026-09-10
Added:     HealthCheck.json manifest (app, version, build, environment, dependencies) served at GET /HealthCheck.json
Added:     Diagnostics page (Supervisor role only): server, port, runtime mode, product version, framework,
           live sessions, WebSocket state, uptime, live health checks, the health JSON a probe would read
Added:     IHealthCheckService — probes database (ITicketRepository), storage (temp write), websocket (Application.IsWebSocket);
           Healthy / Degraded → 200, Unhealthy → 503
Added:     Simulated load test (40 work units through ITicketService) for the capstone demo
Changed:   Startup.cs — the static-file server answers /HealthCheck.json (the only .json exception; Default.json stays blocked)
Fixed:     first health check no longer reports websocket Degraded on page load (Application.IsWebSocket is false during Load;
           the first refresh is deferred one client round-trip)
Ops:       single Kestrel instance behind a reverse proxy. Before adding a 2nd instance: sticky sessions ON, WebSocket
           upgrade forwarded (docs/LoadBalancingNotes.md). Recycle/redeploy only in a drain window — every live
           session on the node is evicted.
Migration: none (in-memory store). Placeholder kept so the line is never forgotten.
Rollback:  redeploy the previous published folder / image tag (0.9.x); no data change to undo. See below.
```

## What changed, in user terms

- Supervisors get a **Diagnostics** page that answers "what is this node doing right now?" without a remote desktop.
- Technicians who open it see **Access denied** — the page maps the system, so it is for operators only.
- Support can read `/HealthCheck.json` to confirm which build is serving traffic after a deploy.

## Operational steps for this release

1. Publish with `dotnet publish -c Release -f net10.0` — `HealthCheck.json` ships in the output.
2. Update `HealthCheck.json`'s `version`/`build`/`environment` for the target (`Production`, `Staging`).
3. Set `"debug": false` in the production `Default.json`; the diagnostics **Runtime mode** row shows `release` when it took.
4. Point the load balancer / uptime monitor at `GET /HealthCheck.json`; alert on non-200 or on a `version` that does not match the release.
5. Run `docs/DemoScript.md` as the smoke test.

## Rollback plan

| | |
|---|---|
| **Trigger** | `/HealthCheck.json` non-200 or wrong `version` for 5 minutes after deploy; **or** the smoke test (DemoScript steps 4–5) fails; **or** the diagnostics page shows `Unhealthy` with `database → ticket store unreachable` and the store itself is fine |
| **Owner** | the release engineer on duty makes the call; the Supervisor on the support rota confirms the user impact from the **Sessions** row |
| **Steps** | 1. announce the rollback window (a restart evicts every session on the node) · 2. redeploy the retained previous artifact (folder or image tag `0.9.x`) · 3. restart the process / recycle the pool · 4. `curl -i /HealthCheck.json` → `200` with the **previous** `version` · 5. run the smoke test · 6. write the incident note |
| **Data** | nothing to undo: this release has no migration. When a future release has one, it must be **additive/backward-safe** so the previous build runs against the new schema |
| **Rehearsed?** | yes — the reviewer script in the README exercises outage → `Unhealthy · HTTP 503` → recovery → `Healthy · HTTP 200`, which is the same signal a rollback watches |

## Evidence (what the running app shows)

- After a deploy the diagnostics page's **Status** line reads `TicketOps Console v1.0.0 · build 2026.09.10.1 · Development`
  straight from the manifest: version proof without reading a file on the server.
- **Simulate repository outage** → status **● Unhealthy · HTTP 503 · out of rotation**, red banner
  *"A required dependency is unavailable…"*; the trace has `[DATA] ✖ outage: SELECT * FROM Tickets failed — timeout connecting to sql01:1433 …`
  and `[SVC] ⚠ database probe caught DataOutageException → reported as Unhealthy, not thrown`. `sql01` never appears in the UI.
- **Recover the repository** → **● Healthy · HTTP 200 · in rotation** — the recovery signal the rollback plan watches for.
