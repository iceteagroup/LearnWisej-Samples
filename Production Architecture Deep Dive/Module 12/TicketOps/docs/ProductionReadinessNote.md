# Production-readiness note — TicketOps Console (capstone)

*Module 12 · lab step 9: a short note before the final run*

**Host.** One Kestrel instance behind a reverse proxy, published with `dotnet publish -c Release -f net10.0`. Reason:
the smallest set of moving parts that still terminates TLS and forwards the WebSocket upgrade; it postpones the
affinity question until a second instance is needed — and `docs/LoadBalancingNotes.md` already answers it (sticky
sessions ON, upgrade headers forwarded, `/HealthCheck.json` probed on every node).

**Health.** `HealthCheck.json` ships with the build and is served at `GET /HealthCheck.json` (the only `.json` the
file server answers; `Default.json` stays protected). Inside the app `IHealthCheckService` reads the same manifest
through `Application.MapPath` and probes the store, the disk and the WebSocket; `HealthReport.Aggregate` maps any
Unhealthy dependency to **503** and anything else to **200**. A production `/health` endpoint would return that
report body with that status; the rule and the JSON already exist.

**Diagnostics.** `Diagnostics/DiagnosticsPage` is Supervisor-only — enforced in `DiagnosticsService`, not in a
button — and renders `IRuntimeInfo` values only: server, port, runtime mode, product version, framework, live
sessions, WebSocket state, uptime, health. No connection string, key or token has a place to appear.

**Sessions are the cost model.** `Application.SessionCount` on the diagnostics page is the number of users a
restart, a recycle or a scale-down interrupts. Deploy in a low-traffic window and drain first.

**Configuration.** Environment-specific values come from the environment (`ASPNETCORE_URLS`, the per-environment
`Default.json` with `"debug": false`, the manifest's `environment`), never from code. The **Runtime mode** row
proves the production flag took.

**Logging.** Every layer logs through `ILog` with a layer tag and a short source (`Class.Method`); exception
details stay in the log, the UI shows `Strings.*`. The session id on the diagnostics page is the correlation
handle for an incident.

**Release & rollback.** `docs/ReleaseNotes.md` is structured (Added / Changed / Fixed / Ops / Migration /
Rollback) and names the rollback trigger (`/HealthCheck.json` non-200 or wrong version for 5 minutes; smoke test
fails), the owner and the steps. The previous artifact is retained; this release has no migration to undo.

**Every path visible, nothing leaked.** Healthy, Degraded and Unhealthy each get a status line and, when not
healthy, a banner sentence from `Resources/Strings.cs`; a role without access gets `Strings.DiagnosticsAccessDenied`
and no data.

**Known limitation, stated.** The store is in-memory and per session; the `database` probe therefore proves the
pattern, not a real database. Swapping `InMemoryTicketRepository` for a SQL repository touches `AppComposition`
and nothing on the diagnostics page.
