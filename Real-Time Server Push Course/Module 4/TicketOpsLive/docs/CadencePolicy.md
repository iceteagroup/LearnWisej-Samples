# Cadence policy — TicketOps Live

For three features of TicketOps Live: which mechanism delivers the update, the safe default, the maximum update
rate, and how a burst is coalesced.

## The four cadences

| Cadence | Question it answers | In this sample |
|---|---|---|
| **Model** | how often does the underlying data change? | `DashboardSimulator` steps `DashboardModel` every **50 ms** (~20 changes/s) |
| **UI** | how often does a human need to see a change? | `refreshTimer.Interval` — **250 ms / 1 s / 5 s**, chosen in `cadenceComboBox`, floored at 250 ms |
| **Push** | how often does the server send UI updates out-of-bound? | none for this panel — a `Wisej.Web.Timer` tick is a browser request, so the applied changes ride back in its response |
| **Polling** | how often does the browser ask, when push is unavailable? | `Application.StartPolling(1000)` while live mode is on and there is no WebSocket; `EndPolling()` when live mode goes off |

The model can run 20× faster than the UI, because the UI shows state, not every transition. `eventsReceivedLabel`
and `updatesAppliedLabel` make the gap visible.

## The policy table

| Feature | Mechanism | Safe default | Max update rate | Coalescing rule |
|---|---|---|---|---|
| **Status strip** (clock, connection, server load, KPI tiles) | session `Wisej.Web.Timer` on the page | **1 s** | 4/s (250 ms) — anything faster is unreadable and only costs CPU | Model events set a dirty flag only. One tick renders one snapshot of the whole strip; if nothing is dirty the tick does no work. |
| **Import progress** (a job started by this user) | `Application.StartTask` + `Application.Update(this)` | **every 10 records** | 5 pushes/s while the job runs | The loop updates the server-side controls on every record and pushes every 10th. The final state is always pushed immediately from `finally`. |
| **Ticket board** (multi-user list) | shared service event (`TicketHub`) → per-session `Application.Update(context, …)`, with a session timer as the coalescer | **500 ms** batch window | 2 updates/s per session, however many tickets changed | A burst of hub events marks the board dirty and remembers which rows changed; the next tick applies one batch. |

Rules that apply to all three:

1. **Push the final state immediately.** Throttling is for intermediate frames; the last frame is never delayed.
2. **Never push inside a tight loop.** Compute first, apply once.
3. **Choose the interval against cost.** 1 s polling × 2 000 sessions is 2 000 requests/s of overhead; 10 s on an
   operations board is a stale decision.
4. **Instrument it.** Events received, updates applied, last applied time — you cannot tune a cadence you do not measure.

## Choosing the mechanism

| The job to be done | Choose |
|---|---|
| Periodic refresh of a screen this user is looking at | `Wisej.Web.Timer` on the page |
| One operation that starts, runs and completes | `Application.StartTask` + `Application.Update` |
| State shared by many users | a shared service that raises events |
| The browser has no WebSocket | `StartPolling` / `EndPolling`, for the duration of the work |

## A note about `StartPolling`

`Application.StartPolling(1000)` is not ignored when a WebSocket is active: it starts real HTTP polls that keep
running after the socket connects. So the sample requests polling only when `Application.IsWebSocket` is false, and
never in `MainPage_Load` (where `IsWebSocket` is still false). For this panel polling is belt and braces — the timer
ticks are requests of their own — but the out-of-bound path (the simulator's failure handler) has no other way to
reach a browser without a WebSocket.
