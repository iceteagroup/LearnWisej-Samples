# Cadence policy — TicketOps Live (Module 4 deliverable)

The walkthrough asks for a **cadence policy table**: for three features of TicketOps Live, say which mechanism
delivers the update, what the safe default is, what the maximum update rate is, and how a burst is coalesced.
This is the document a team agrees on once and then holds every dashboard to.

## The four cadences (the vocabulary)

Cadence is not one number. Four different rhythms sit behind every live screen, and they are allowed to differ —
that is the whole point of the module.

| Cadence | Question it answers | In this sample |
|---|---|---|
| **Model** | how often does the underlying data change? | `DashboardSimulator` steps `DashboardModel` every **50 ms** (~20 changes/s) |
| **UI** | how often does a human need to see a change? | `refreshTimer.Interval` — **250 ms / 1 s / 5 s**, chosen in `cadenceComboBox`, floored at 250 ms |
| **Push** | how often does the server send UI updates out-of-bound? | **none for this panel** — a `Wisej.Web.Timer` tick *is* a browser request, so the applied changes ride back in that request's response. The only `Application.Update` in the module is the simulator's fault banner |
| **Polling** | how often does the browser ask, when push is unavailable? | `Application.StartPolling(1000)` while live mode is on **and** `Application.IsWebSocket` is false; `EndPolling()` when live mode goes off |

The model can run 20× faster than the UI and nobody notices, because the UI shows *state*, not *every transition*.
`eventsReceivedLabel` and `updatesAppliedLabel` in the sample are the two counters that make the gap visible.

## The policy table (three TicketOps Live features)

| Feature | Mechanism | Safe default | Max update rate | Coalescing rule |
|---|---|---|---|---|
| **Status strip** (clock, connection, server load, KPI tiles) | session `Wisej.Web.Timer` on the page; the model is fed by a shared watcher | **1 s** | 4/s (250 ms) — anything faster is unreadable and only costs CPU | Model events set a dirty flag only. One tick renders one **snapshot** of the whole strip; intermediate values are never shown. If nothing is dirty the tick does no work at all. |
| **Import progress** (a job started by this user) | `Application.StartTask` + `Application.Update(this)` — a **background task**, because the work starts, runs and completes | **every 10 records** (≈4 pushes/s at 25 ms/record) | 5 pushes/s while the job runs, then silence | The loop updates the server-side model on **every** record and pushes every 10th, so the numbers are always exact server-side and the browser sees a smooth-but-cheap progression. The **final state is always pushed immediately** from `finally`, whatever the outcome (Module 3). |
| **Ticket board** (multi-user list, other people's edits) | shared service event (`TicketHub`) → per-session `Application.Update(context, …)`, with a session timer as the coalescer | **500 ms** batch window | 2 updates/s per session, regardless of how many tickets changed | A burst of hub events marks the board dirty and remembers *which* rows changed; the next tick applies one batch. A 40-ticket import must never become 40 board pushes per session — it is one update per session per window. |

Rules that apply to all three:

1. **Push the final state immediately.** Throttling is for intermediate frames; the last frame is never delayed.
2. **Never push inside a tight loop.** Compute first, apply once.
3. **Choose the interval against cost.** 1 s polling × 2 000 sessions is 2 000 requests/s of pure overhead;
   10 s on an operations board is a stale decision. The interval is a product decision with a server bill.
4. **Instrument it.** Events received, updates applied, ticks refused, last applied time — the four numbers in
   the Update cadence card. You cannot tune a cadence you do not measure.

## Choosing the mechanism (the module checkpoint)

| The job to be done | Choose | Why |
|---|---|---|
| Periodic refresh of a screen this user is looking at | **`Wisej.Web.Timer` on the page** | Session-scoped, owned by the view, stops with the view; its tick is a request, so no push machinery is needed |
| One operation that starts, runs and completes (import, export, report) | **`Application.StartTask` + `Application.Update`** | It has a beginning and an end, its own cancellation, and its own progress cadence |
| State shared by many users (tickets, queues, alerts) | **A shared service that raises events** | One watcher for the whole server instead of one loop per session; each session applies the event in its own context |
| The browser has no WebSocket | **`StartPolling` / `EndPolling`, for the duration of the work** | It is a *fallback*, not an architecture: it does not replace an event model, and it costs one request per session per interval |

## A finding about `StartPolling`

The lesson says polling "is ignored when WebSocket is active". At runtime it is **not** ignored (verified while
building Module 1 of this course): `Application.StartPolling(1000)` starts real HTTP polls (`Wisej: Poll request.`
once per second in the browser console) that keep running even after the socket connects. So this sample guards
the call: `StartPollingFallback()` requests polling only when `Application.IsWebSocket` is false, remembers it in
`_polling`, and `StopPollingFallback()` calls `EndPolling()` exactly once. `Application.IsWebSocket` is also false
during `MainPage_Load`, which is why the call is **not** in Load.

Second honest note: for *this* panel the fallback is belt and braces. The refresh is driven by a `Wisej.Web.Timer`,
whose ticks are browser requests, so the dashboard would keep updating without a WebSocket and without polling.
Polling is requested with live mode because the **out-of-bound** path — the simulator's fault banner, and any
future hub event — has no other way to reach a socket-less browser.

## Evidence (what the running app shows)

*Not yet executed in a browser: the sample builds clean but the reviewer runs it. These are the expected lines.*

- **Live mode ON, 1 sec:** `• server refreshTimer.Start() UI cadence: one tick every 1 s …`, then one
  `← request refreshTimer_Tick applied model v… · ~20 model change(s) coalesced into 1 update …` per second.
  `EVENTS RECEIVED` climbs ~20 per second, `UPDATES APPLIED` climbs by 1 — the ratio in SERVER STATE settles
  around *20 events per update*.
- **Cadence → 250 ms:** `← request cadenceComboBox_SelectedIndexChanged UI cadence 1 s → 250 ms (floor 250 ms) ·
  refreshTimer.Interval reprogrammed on the running timer, no new timer`; the tick lines arrive four times as
  often and the ratio drops to about *5 events per update*. At **5 sec** it rises to about *100*.
- **Burst 50 model events:** `← request burstButton_Click 50 model changes at once …` followed by
  `• server burst events received n → n+50 (+50), updates applied still m · the next refreshTimer_Tick applies ONE
  update with the final state`. The next tick line shows `≥50 model change(s) coalesced into 1 update`.
- **Slow tick at 250 ms:** each applied line carries `· slow tick 1500 ms`; the ticks arrive ~1.5 s apart instead
  of 250 ms because Wisej.NET serialises the requests of one session — the queue, not the guard, is what you see
  first. `TICKS REFUSED` counts any tick that still reaches the server while `_refreshInProgress` is true and is
  expected to stay low (possibly 0): the flag is the second line of defence, not the first.
- **No WebSocket** (`"enableWebSocket": false` in `Default.json`): the strip shows `○ HTTP only`, live mode logs
  `• server Application.StartPolling(1000) IsWebSocket=false → the browser polls every second …`, the browser
  console prints `Wisej: Poll request.` once per second, and switching live mode off logs
  `• server Application.EndPolling() live mode off → fallback polling OFF`.
