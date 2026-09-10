# Architecture note — TicketOps Live (Module 7 capstone deliverable)

One page. This is the note the lab asks a student to defend: which state is session-specific, which is global,
where background work starts, how UI updates reach the browser, how cancellation works, how subscriptions are
removed, what happens without WebSocket, and what the deployment must support.

## The end-to-end path

```
 GLOBAL (one per server process)            SESSION (one per browser tab)              BROWSER
 ┌──────────────────────────────┐          ┌────────────────────────────────┐       ┌──────────────┐
 │ TicketHub                    │          │ MainPage                       │       │ widgets      │
 │  · List<Ticket> under a lock │          │  · _allTickets / _tickets      │       │              │
 │  · TicketChanged             │          │    (BindingList, session-owned)│       │              │
 │  · GetSnapshot() → clones    │          │  · _tenant, counters, filters  │       │              │
 │ SessionRegistry              │          │  · _context = Application.     │       │              │
 │  · session ids + start times │          │      Current (captured on Load)│       │              │
 │  (values only — never a page)│          │  · loops: heartbeat, import,   │       │              │
 └───────────────┬──────────────┘          │      simulator, ticket feed    │       │              │
                 │ raises on a             └───────────────┬────────────────┘       │              │
                 │ THREAD-POOL thread                      │                        │              │
                 │ (Task.Run — never the                   │ Application.Update(    │              │
                 │  publisher's request thread)            │   _context, () => …)   │              │
                 └────────────────────────────────────────►│ ──────────────────────►│ re-render    │
                                                           │   WebSocket frame      │              │
                                        no WebSocket ──────┤ StartPolling(1000)     │ next poll    │
                                                           └────────────────────────┴──────────────┘
```

## 1. Which state is session-specific

Everything a user can see or change. All of it lives in **instance fields of `MainPage`**, so a second browser tab
gets its own copy: the bound ticket list (`_allTickets` / `_tickets`), the tenant, the Escalated filter, the grid
selection, the notification and filtered-out counters, the cadence choice and its counters, the import job and its
`CancellationTokenSource`, the heartbeat flag and beat number, the push-time window behind *updates/min*, and
`Application.Session.Counter`. The **Session** tab prints them next to `_sharedCounter`, a `static int` kept on
purpose and labelled as the trap: two tabs move it together, which is exactly why per-user values must never live there.

## 2. Which state is global

Two objects, both `Lazy<T>` singletons that store **values only**:

- **`TicketHub`** — the ticket list under a lock, `TicketChanged`, `GetSnapshot()` handing out clones, and the
  counters shown on the Health tab. It never calls `Application.Update`, because it owns no session.
- **`SessionRegistry`** — the live session ids and their start times, keyed on `Application.SessionId`
  (`ClientId` identifies the *browser*: two tabs of one Chrome share it).

Neither ever holds a `MainPage`, a control or an `IWisejComponent`. A global service that stored those would keep
dead sessions alive and would try to render into browsers that are gone.

## 3. Where background work starts

Four loops, each started by the click that owns it, each with a stop condition:

| Loop | Started by | Stops when |
|---|---|---|
| Heartbeat | `startButton_Click` → `Application.StartTask(HeartbeatLoop)` | `_heartbeatRunning` false, `IsDisposed`, or Stop |
| Import | `startImportButton_Click` → `Application.StartTask(() => RunImport(token, …))` | token cancelled, 200 records done, or a fault |
| Dashboard simulator | `liveModeCheckBox` → `DashboardSimulator.Start()` | live mode off, `IsDisposed`, or a fault |
| Ticket feed | `newTicketsButton_Click` | 10 tickets published, `IsDisposed`, or `_feedRunning` false |

`Application.StartTask` keeps this session's context on the new thread; that is what lets the loop touch this
page's controls at all.

## 4. How UI updates reach the browser

Three mechanisms, all visible in the trace:

- **In-request** — a click or a **timer tick** changes controls and the change rides back on that request's own
  response. `refreshTimer_Tick` uses this: a timer tick *is* a browser request, so the cadence refresh needs no push.
- **Out-of-bound push** — a task changes controls and calls `Application.Update(this)` (or
  `Application.Update(_context, …)` when the caller is not on a `StartTask` thread, as in `Hub_TicketChanged` and
  `Registry_SessionsChanged`). Each push is stamped by `NotePush()`, which is what the Health tab measures.
- **Polling fallback** — when a task starts and `Application.IsWebSocket` is false, `BeginPush()` calls
  `StartPolling(1000)`; `EndPush()` calls `EndPolling()` when the last task ends.

## 5. How cancellation works

Cooperatively, never by aborting a thread. The import checks `token.ThrowIfCancellationRequested()` **between**
records so the state is consistent at the stop; the heartbeat, simulator and feed check a `volatile bool` once per
iteration. `Cleanup()` cancels all of them, and it runs from `Disposed` as well as from `ApplicationExit`, so
closing the tab stops the work. Every loop restores its buttons in `finally`, whatever the outcome.

## 6. How subscriptions are removed

There is exactly **one** cleanup method, and it is idempotent (`_cleanedUp`). It unsubscribes `TicketChanged`,
`SessionsChanged`, `ApplicationExit` and `SessionTimeout`, unregisters the session from the registry, and stops
every loop. It is called from `Application_ApplicationExit` and from the page's `Disposed` handler; whichever runs
first does the work. The Health tab prints `active subscriptions` (hub subscribers + registered sessions) so a leak
would be visible as a number that grows and never falls.

## 7. What happens when WebSocket is unavailable

`Application.IsWebSocket` reads **false during `MainPage_Load`** — the socket is opened by the client right after
the first response — so this app never calls `StartPolling` in Load (that would poll forever). It requests polling
where out-of-bound work starts and only if there is no socket at that moment, and it ends polling when the work is
done. To rehearse the fallback, set `"enableWebSocket": false` in `Default.json` and restart: the strip stays
`○ HTTP only`, the Health tab shows *Fallback mode* and *Polling enabled*, updates arrive on the polls about a
second late, and nothing else changes.

## 8. What the deployment must support

- **WebSocket end to end** — browser, corporate proxy, load balancer, web server. Verify from the user's network,
  not from the server console.
- **Sticky sessions** — server-side session state lives in one process; a request routed elsewhere finds no session.
- **Health checks** — `HealthCheck.json` (`maxMemory`, `maxSessions`, `cpuThreshold`) lets the balancer drain a hot
  instance instead of tipping it over.
- **`debug: false`** in production, a real `sessionTimeout`, and a `pollingInterval` chosen for the number of
  sessions, not for the demo.
- **Observability** — every import carries a `JobId`, every hub event an `EventId`, and both appear in the UI trace
  and in the server log with the session id, so support can reconstruct what a user saw.

## Evidence

Verified in the browser at <http://localhost:5307>: heartbeat and import pushing concurrently and interleaved in
one trace; import failing at record 87 with the detail in the server log and a safe message on screen; publish and
escalate fanning out through the hub with a Northwind event counted as *filtered out*; live mode coalescing 266
model events into 14 UI updates; the Health tab reporting *Push expected*, *Polling off*, *active subscriptions 2*
and *63 updates/min* with the running loops named.
