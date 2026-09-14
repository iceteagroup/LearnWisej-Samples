# Deployment — two instances behind a load balancer

The capacity model says one instance holds 150 sessions. The support team expects 120 at peak and wants
to survive losing a machine, so: **two instances, session affinity, WebSocket upgrade preserved end to
end.**

## Session affinity is not optional

A Wisej.NET session lives **in the memory of one process**: the page, its controls, their state, the
binding sources, the page cache, the PERF buffer. A request from that browser that lands on the other
instance finds no session at all.

So the balancer must send every request of one browser to the same instance:

- **Cookie-based affinity** is the usual answer — the balancer sets its own cookie (`AWSALB`, `.Nginx`
  upstream cookie, Application Request Routing's `ARRAffinity`) and routes by it. It survives a client
  IP change, which IP-hash affinity does not, and mobile clients change IP.
- **IP-hash affinity** works when clients have stable addresses and is the fallback when a cookie cannot
  be added.
- **No affinity** requires an out-of-process session store, which Wisej.NET session state is not: the
  controls are live objects, not serialisable state. Do not plan for it.

What breaks without affinity: the first click on the "wrong" instance starts a **new** session, so the
user sees a freshly loaded application, and every unsaved thing on the screen is gone.

## The WebSocket upgrade has to pass every proxy in the path

Wisej.NET uses a WebSocket for server-initiated updates — the progress reports of the export in
Module 6, and any push a background task makes. The upgrade has to survive every hop:

| Hop | What it needs |
|---|---|
| Load balancer | WebSocket support enabled on the listener; on an HTTP-only listener the upgrade is stripped and nothing says so |
| Reverse proxy (nginx, IIS ARR) | forward `Upgrade` and `Connection` headers; `proxy_http_version 1.1` on nginx |
| Idle timeout | longer than the app's heartbeat, or the connection is dropped every few minutes and re-established |
| TLS termination | fine anywhere in the path, as long as the upgrade is forwarded after it |

**If it cannot pass:** Wisej.NET falls back to long-polling. The application keeps working and updates
still arrive, with more latency and more requests per session — which changes the capacity model,
because every session now costs a polling request every few seconds instead of an idle socket. If you
deploy behind a proxy you do not control, measure again with the WebSocket disabled before promising a
session count.

The three cases the lab asks to be shown, and what the user sees in each:

| Case | What the user sees |
|---|---|
| Healthy | Everything responds. `healthcheck.wx` answers `200` with the session count. |
| Over the limit | **Existing sessions keep working** — every button on every tab still responds. A **new** browser is sent to another instance by the balancer; if there is none, it waits and retries after 10 seconds. Nothing in the running session changes. |
| No WebSocket | The application still works; updates arrive on the next poll instead of immediately. The visible difference is the export progress bar, which moves in steps rather than smoothly, and a slightly later reaction to background work. |

## Production settings the measurements assume

| Setting | Value | Why |
|---|---|---|
| Build | **Release** | every number in this course was taken in Release; a Debug deployment invalidates all of them |
| `debug` in `Default.json` | `false` | it is `true` in this sample for the lab |
| `sessionTimeout` | the production value, **not** the 1800 s used here | session lifetime multiplies the concurrent session count in the capacity model |
| `HealthCheck.json` | as in `Capacity.md` | and **shipped** — it is read from the application root at start |
| Health URL | `http://<instance>/healthcheck.wx` | configured as the balancer's health check |
| Logging | the `PERF` records to the log pipeline | they are the production evidence; see `Monitoring.md` |
| Database | a real server, connection pooling on | the SQLite file in this sample is a lab convenience |

## What changes with a real database server

Every statement becomes a network round-trip. Module 6 removed an N+1 that cost 201 statements per
page: in this sample that was worth about 60 ms, and against a database server across a socket it is
most of a second. The **statement counts** in these documents are properties of the code and carry over;
the **milliseconds** do not. Re-measure `Tickets/Search` and `Dashboard/Refresh` against the production
database before publishing a capacity number.

## The health endpoint in this sample

Wisej.NET's own `healthcheck.wx` handler belongs to the classic System.Web pipeline. Running under
Kestrel with `dotnet run`, it answered `200` regardless of the configured limits (verified on
Wisej-4 4.1.0), so this sample serves the same URL from its own middleware in `Startup.cs` — using the
same `HealthCheck.json` values and the same `HealthPolicy.IsAvailable` that the framework hook
`HealthCheck.IsServerAvailable` is assigned. Under IIS the framework handler applies and the middleware
answers first with the identical decision. Either way, what the balancer receives is a `503` with a
`Retry-After` header and a one-line reason.
