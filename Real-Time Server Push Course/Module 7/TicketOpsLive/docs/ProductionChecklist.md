# Production checklist — TicketOps Live

The ten performance questions of the lesson, plus the security and observability rules, each with **where this app
satisfies it** and the evidence a reviewer can see. The same ten items are in the app itself, on the
**Health · config** tab: ticking one writes it to the trace.

## Performance

| # | Question | Where in this app | Evidence |
|---|---|---|---|
| 1 | Does every background loop have a stop condition? | `_heartbeatRunning`, `_feedRunning`, the import's `CancellationToken`, `DashboardSimulator`'s flag — every loop also tests `this.IsDisposed` | Stop ends the heartbeat within one second; closing the tab runs `Cleanup` |
| 2 | Does every global subscription unsubscribe? | one idempotent `Cleanup()`, called from `Application_ApplicationExit` **and** `Disposed` | the server log prints `cleanup (…): loops stopped, hub and registry unsubscribed`; `active subscriptions` falls |
| 3 | Are UI updates batched or throttled? | import pushes every 10th record; the heartbeat pushes once per second; every `finally` pushes the final state once | 200 records → 21 pushes in the trace |
| 4 | Are high-frequency model events coalesced? | the simulator changes the model every 50 ms and only sets `_dashboardDirty`; `refreshTimer_Tick` applies one snapshot | 266 model events → 14 UI updates at 1 s cadence |
| 5 | Are bound collections updated in the session context? | `Hub_TicketChanged` → `SafeUpdate` → `Application.Update(_context, …)`; the list is only ever touched inside that callback | every board change is a `→ push Application.Update(_context)` line |
| 6 | Are static fields used only for global state? | `TicketHub.Instance` and `SessionRegistry.Instance`; `_sharedCounter` is the deliberate, labelled trap | two tabs move the static counter together and the session counter separately |
| 7 | Are counters, filters and selection per session? | instance fields: tenant, Escalated filter, notification and filtered-out counts, cadence counters, selection | two tabs on different tenants show different boards from the same hub |
| 8 | Are exceptions caught and logged inside background tasks? | `try/catch/finally` in `HeartbeatLoop`, `RunImport`, the feed task, `SimulatorFailed` | record 87 → server log has the stack trace, the UI has "Import failed. Review the server log." |
| 9 | Does the app behave acceptably when WebSocket is unavailable? | `BeginPush`/`EndPush` request `StartPolling(1000)` only while work runs, and `EndPolling()` after | with `"enableWebSocket": false` the Health tab reads *Fallback mode* / *Polling enabled* and updates still arrive |
| 10 | Does the load balancer keep a session on one instance? | deployment requirement, not code: sticky sessions — see `ConfigurationReview.md` | the config panel states it; `SessionId` is per tab and lives in this process only |

## Security

- **Encode what users typed.** Ticket titles, customers and owners are rendered by Wisej.NET controls, which
  synchronize properties rather than concatenating HTML. Anything that would render user text *as HTML* needs a
  sanitization step first — this app has none, and adds none.
- **Trust forwarded headers only behind a trusted proxy.** Client IP and scheme must not be taken from headers an
  attacker can set; enable forwarded-headers handling only when the proxy strips and rewrites them.
- **Bound the resources one session can take.** One heartbeat, one import, one simulator and one feed per session,
  each refused while already running; the feed publishes 10 tickets and stops. Nothing here lets a user start
  unlimited loops or subscriptions.
- **Do not show raw exceptions.** Every catch sends the detail to the server log with the session id and shows a
  short, safe message.

## Observability

Every long-running thing carries an id that appears in **both** the UI and the log:

| Correlation | Where it is created | Where it shows |
|---|---|---|
| `JobId` (`J-3F9A2C`) | `ImportJob` | import log, trace, banner, server log on failure |
| `EventId` | `TicketChangedEventArgs.NewEventId()` | publisher's trace and every subscriber's trace |
| `SessionId` | Wisej.NET | SERVER STATE, registry list, every server-log line |
| `updates/min` | `NotePush()` + the 60-second window | Health tab, SERVER STATE |

What a support engineer can reconstruct from that: which session saw what, in which order, how fast the server was
pushing to it, and whether the failure was in the job or in the delivery.

## Before you deploy

1. Set `debug: false` and re-test — the client is bundled and minified differently.
2. Choose `sessionTimeout` from real user behaviour, not from the lab's 180 seconds.
3. Choose `pollingInterval` for the worst case: one request per second per fallback session.
4. Test the WebSocket upgrade from the user's network path, through the real proxy.
5. Turn on sticky sessions and confirm with two instances behind the balancer.
6. Point `HealthCheck.json` thresholds at numbers you measured under load.
7. Watch `active subscriptions` over a day: it must come back down.
