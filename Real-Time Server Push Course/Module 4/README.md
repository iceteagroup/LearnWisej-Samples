# TicketOpsLive · Real-Time Apps with Server Push · Module 4

Local lab build for **Module 4 · When Push Is Not Enough: Cadence, Timers, and Fallbacks**: the Update Cadence panel
of TicketOps Live. A simulated dashboard model changes every 50 ms; a `refreshTimer` applies one coalesced update per
tick at the cadence you choose (`250 ms` / `1 sec` / `5 sec`); polling is tied to live mode; and the panel counts
model events and applied updates separately. The model event never pushes: it counts itself and sets a dirty flag,
and the timer decides when the user sees the result.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 4/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5304
```

Then open <http://localhost:5304>. (Visual Studio: open `TicketOpsLive.slnx`, press F5.) Keep the tab in front
while comparing cadences: browsers throttle timers in background tabs.

## What to try

| Control | What you should see |
|---|---|
| Live mode ✔ | the KPI tiles move once per second; `EVENTS RECEIVED` climbs about 20/s, `UPDATES APPLIED` 1/s |
| Cadence → 250 ms | four updates per second (about 5 events per update) |
| Cadence → 5 sec | one update every five seconds (about 100 events per update) — same information, a fraction of the traffic |
| Live mode ✘ | the simulator, the timer and polling stop; the tiles keep the last snapshot |

**Polling fallback.** Add `"enableWebSocket": false` to `Default.json` and restart: `connectionLabel` shows
`○ Polling 1000 ms` while live mode is on and the browser console logs one `Wisej: Poll request.` per second.
With a WebSocket, polling is not requested (`StartPolling` is not ignored when a socket is up and would be pure cost).

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| `liveModeCheckBox`, `cadenceComboBox`, `eventsReceivedLabel`, `updatesAppliedLabel`, `lastAppliedLabel`, `refreshTimer` | [`MainPage.Designer.cs`](TicketOpsLive/MainPage.Designer.cs); `refreshTimer` is a `Wisej.Web.Timer(this.components)` |
| Start / stop polling with live mode | [`MainPage.cs`](TicketOpsLive/MainPage.cs) `StartPollingFallback()` / `StopPollingFallback()` |
| Apply dashboard updates at the selected cadence | `refreshTimer_Tick`; `cadenceComboBox_SelectedIndexChanged` reprograms `refreshTimer.Interval` (floor 250 ms) |
| Simulate faster model changes than UI updates | [`Services/DashboardSimulator.cs`](TicketOpsLive/Services/DashboardSimulator.cs) steps [`Services/DashboardModel.cs`](TicketOpsLive/Services/DashboardModel.cs) every 50 ms |
| Count model events and applied updates separately | `SimulatedModelChanged()` (`_eventsReceived`, dirty flag) vs `refreshTimer_Tick` (`_updatesApplied`) |
| Prevent overlapping timer ticks | `_refreshInProgress`, set in `try`, cleared in `finally` |
| Walkthrough: the cadence policy table | [`docs/CadencePolicy.md`](TicketOpsLive/docs/CadencePolicy.md) |

## Self-check answers

- **Why does the model event not call `Application.Update`?** That would make the model cadence the push cadence:
  20 frames per second per session, showing states no one reads. The event counts and sets a flag; the timer applies
  one snapshot per tick.
- **Why is there no `Application.Update` in the tick?** A `Wisej.Web.Timer` tick is a browser request; its changes
  travel back with the response. The only out-of-bound push is the simulator's failure handler.
- **Timer vs task vs polling vs service event?** A timer for session-scoped periodic refresh, `Application.StartTask`
  for one operation that starts and completes, a shared service event for state many users watch, and polling only as
  a fallback when there is no WebSocket.
- **Why start the timer once?** A second `Start()` means two schedules on one page: double the requests and bugs
  that look random. The page owns `refreshTimer`, starts it once (`_timerStarted`) and stops it on close.
