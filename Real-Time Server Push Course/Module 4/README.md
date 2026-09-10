# TicketOpsLive · Real-Time Apps with Server Push · Module 4

Local lab build for **Module 4 · When Push Is Not Enough: Cadence, Timers, and Fallbacks**. It adds the
**Update cadence** panel to TicketOps Live: a simulated dashboard whose model changes every 50 ms, a
`refreshTimer` that applies **one coalesced update per tick** at a cadence you choose (`250 ms` / `1 sec` /
`5 sec`), the polling fallback tied to live mode, and the instrumentation the lesson asks for — events received,
updates applied, ticks refused, last applied.

The point of the module in one line: **the model event never pushes.** It counts itself and sets a dirty flag;
the timer decides when the user sees the result.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 4/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5304
```

Then open <http://localhost:5304>. (Visual Studio: open `TicketOpsLive.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Control | Path | What you should see |
|---|---|---|
| (page load) | — | trace `← request MainPage_Load … IsWebSocket=false (the socket opens after this response)`; the dashboard shows the model's starting values; the timer is stopped |
| **Live mode** ✔ | success · progress | `• server refreshTimer.Start() UI cadence: one tick every 1 s …` and `• server DashboardSimulator.Start() model cadence: one change every 50 ms …`; then one `← request refreshTimer_Tick applied model v… · ~20 model change(s) coalesced into 1 update …` **per second**, with the KPI tiles moving. `EVENTS RECEIVED` climbs ~20/s, `UPDATES APPLIED` climbs 1/s |
| **UI cadence → 250 ms** | cadence | `← request cadenceComboBox_SelectedIndexChanged UI cadence 1 s → 250 ms (floor 250 ms) · refreshTimer.Interval reprogrammed on the running timer, no new timer`; four tick lines per second, ratio in SERVER STATE drops to ≈ 5 events per update |
| **UI cadence → 5 sec** | cadence | one tick line every five seconds, ratio ≈ 100 events per update — same information, 1/20th of the traffic, at the price of feeling stale |
| **Burst 50 model events** | coalescing | `← request burstButton_Click 50 model changes at once …` + `• server burst events received n → n+50 (+50), updates applied still m …`; the very next tick applies **one** update carrying ≥ 50 changes. Events **+50**, updates **+1** |
| **Simulate slow tick (1.5 s)** ✔ | overlap guard · warning | every applied line carries `· slow tick 1500 ms`; at 250 ms cadence the updates arrive ~1.5 s apart because Wisej.NET serialises this session's requests. `TICKS REFUSED` counts any tick that still lands while `_refreshInProgress` is true (expected to stay near 0 — see [`docs/TimerOwnership.md`](TicketOpsLive/docs/TimerOwnership.md)) |
| **Start timer again (anti-pattern)** | failure · guard | `• server duplicateTimerButton_Click refused — refreshTimer is already running at 1 s — start timers once` + amber banner; the tick rate does **not** double |
| **Start timer again**, live mode off | guard | `• server duplicateTimerButton_Click nothing to duplicate — refreshTimer is stopped (live mode is off)` — no stray timer is ever created |
| **Live mode** ✘ | cancellation · cleanup | `• server DashboardSimulator.Stop() cooperative stop …`, `• server refreshTimer.Stop() the view is no longer live …`, `• server Application.EndPolling() …` (when polling was on). The dashboard keeps the last snapshot — the final state is applied by the checkbox request itself |
| **Live mode** ✔ again | recovery | everything restarts from the same page; the counters keep counting, the banner clears |
| Clear trace | — | empties the right-hand list |

**Polling fallback.** Set `"enableWebSocket": false` in `Default.json` and restart: the strip stays `○ HTTP only`,
switching Live mode on logs `• server Application.StartPolling(1000) IsWebSocket=false → the browser polls every
second while live mode is on …`, the browser console shows one `Wisej: Poll request.` per second, and Live mode off
logs `Application.EndPolling() … fallback polling OFF`. With a WebSocket the trace says instead
`• server polling fallback not requested — IsWebSocket=true …`, because `StartPolling` is **not** ignored when a
socket is up (verified in Module 1) and would be pure cost.

**Watch the two clocks.** `SERVER STATE` is written on every request, so after a burst it shows the new model
version immediately while the big `EVENTS RECEIVED` tile still shows the old count until the next tick. That gap
*is* the module: the server is authoritative, the UI catches up at the UI cadence.

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| `liveModeCheckBox`, `cadenceComboBox` (250 ms / 1 sec / 5 sec), `eventsReceivedLabel`, `updatesAppliedLabel`, `lastAppliedLabel`, `refreshTimer` | [`MainPage.Designer.cs`](TicketOpsLive/MainPage.Designer.cs) — `panelCadence` block; `refreshTimer` is a `Wisej.Web.Timer(this.components)` in the Designer tray |
| Start polling when live mode is enabled | `StartLiveMode()` → `StartPollingFallback()` — `Application.StartPolling(1000)` **only if** `!Application.IsWebSocket`, remembered in `_polling` |
| Stop polling when live mode is disabled | `StopLiveMode()` → `StopPollingFallback()` — `Application.EndPolling()` guarded by `_polling` |
| Use a timer to apply dashboard updates at the selected cadence | `refreshTimer_Tick` applies one `_model.Snapshot()`; `cadenceComboBox_SelectedIndexChanged` reprograms `refreshTimer.Interval` (floor 250 ms) on the running timer |
| Simulate faster model changes than UI updates | [`Services/DashboardSimulator.cs`](TicketOpsLive/Services/DashboardSimulator.cs) — a session-owned `Application.StartTask` loop stepping [`Services/DashboardModel.cs`](TicketOpsLive/Services/DashboardModel.cs) every 50 ms; `burstButton_Click` adds 50 changes at once |
| Count model events and applied UI updates separately | `SimulatedModelChanged()` does `Interlocked.Increment(ref _eventsReceived)` + `_dashboardDirty = true`; `refreshTimer_Tick` does `_updatesApplied++` |
| Prevent overlapping timer ticks | `_refreshInProgress` set in `try` / cleared in `finally`; an overlapping tick is counted in `_skippedTicks` (`TICKS REFUSED`) and dropped |
| Show every path (success / progress / cancellation / failure) without a manual refresh | progress = the tick lines; cancellation = Live mode off (cooperative stop, ≤ 50 ms); failure = the refused second timer and `SimulatorFailed` (caught inside the task → server log + safe banner + live mode off); recovery = tick Live mode again |
| Walkthrough deliverable: the cadence policy table | [`docs/CadencePolicy.md`](TicketOpsLive/docs/CadencePolicy.md) |
| Walkthrough deliverable: timer ownership | [`docs/TimerOwnership.md`](TicketOpsLive/docs/TimerOwnership.md) |

## Where things live

```
TicketOpsLive/
├─ MainPage.cs                 live mode (timer + simulator + polling), the coalescing tick, the burst,
│                              the "start timers once" guard, the instrumentation
├─ MainPage.Designer.cs        dashboard strip, Update cadence card, trace card, action bar, refreshTimer
├─ Services/
│  ├─ DashboardModel.cs        thread-safe random walk (open tickets, queue depth, avg wait) + Snapshot()
│  └─ DashboardSimulator.cs    session-owned 50 ms task; marks the model dirty through a callback, never pushes
├─ Program.cs                  Application.MainPage = new MainPage()
├─ Startup.cs                  Kestrel host (app.UseWisej())
├─ Default.json                Wisej.NET config (add "enableWebSocket": false to see the polling fallback)
└─ docs/
   ├─ CadencePolicy.md         the walkthrough deliverable: mechanism / safe default / max rate / coalescing
   │                           rule for status strip, import progress and ticket board + the four cadences
   └─ TimerOwnership.md        who owns which timer, start once, stop on close, overlap guard, polling lifecycle
```

## Self-check — the acceptance criteria, mapped to the code

- [x] **The timer cadence changes visibly.** `cadenceComboBox_SelectedIndexChanged` → `refreshTimer.Interval =
      Math.Max(250, …)`. At 250 ms four tick lines per second, at 5 sec one every five seconds; `lastAppliedLabel`
      and the trace timestamps show the interval.
- [x] **Events received can grow faster than updates applied.** The simulator steps the model every 50 ms
      (~20 events/s) while the timer applies 4/s, 1/s or 0.2/s. `SERVER STATE` prints the ratio
      (`n events per update`). `Burst 50 model events` makes it a single jump: events +50, updates +1.
- [x] **Polling starts and stops with live mode.** `StartPollingFallback()` / `StopPollingFallback()` from the
      checkbox handler, guarded by `_polling` and by `Application.IsWebSocket` — see the finding below.
- [x] **No overlapping timer ticks occur.** `_refreshInProgress` in `refreshTimer_Tick` (`try` / `finally`), with
      the refused ticks counted in `TICKS REFUSED`. Wisej.NET also serialises this session's requests, so the flag
      is the second line of defence — the honest version is in [`docs/TimerOwnership.md`](TicketOpsLive/docs/TimerOwnership.md).
- [x] **The student can explain why coalescing is useful.** The card says it, the counters prove it: 20 model
      changes a second, 1 update a second, one snapshot per update, no intermediate state on screen.

## Self-check answers

- **Why does the model event not call `Application.Update`?**
  Because that would make the *model* cadence the *push* cadence: 20 serialisations, 20 WebSocket frames and 20
  re-renders per second per session, showing states no human reads. `SimulatedModelChanged()` does the two
  cheapest things possible — `Interlocked.Increment` and a `volatile bool` — and the timer decides when the user
  sees the result. That is coalescing: many changes in, one update out.
- **Why is there almost no `→ push` line in this module's trace?**
  A `Wisej.Web.Timer` tick **is** a browser request. Everything `refreshTimer_Tick` changes travels back in the
  response of that request, so no `Application.Update()` is needed. The only push in the module is
  `SimulatorFailed`, which runs out-of-bound on the task thread and therefore uses
  `Application.Update(this, () => …)`.
- **Then why request polling at all?**
  For that out-of-bound path. Without a WebSocket the fault banner (and, from Module 6 on, any hub event) has no
  channel; the polls carry it. The timer refresh itself would keep working without polling — say so out loud, it
  is the difference between a fallback and a cargo-culted call.
- **Checkpoint — timer vs. background task vs. polling vs. service event.**
  *Timer* for session-scoped periodic UI refresh (this module's dashboard). *`Application.StartTask`* for a
  session-scoped operation that starts, runs and completes (Module 3's import). *Shared service event* for state
  many users watch (the ticket hub, Module 6) — one watcher instead of one loop per session. *Polling* is not a
  mechanism, it is a **fallback** for sessions without a WebSocket, requested for the duration of the work and
  ended after. The cost of each cadence, per feature, is in
  [`docs/CadencePolicy.md`](TicketOpsLive/docs/CadencePolicy.md).
- **What is the risk of a second `refreshTimer.Start()`?**
  Two schedules on one page: double the requests, double the applied updates, counters that no longer match the
  cadence, and bugs that look random because they depend on which tick wins. The page owns the timer and starts it
  once (`_timerStarted`); `duplicateTimerButton` exists only to be refused.

## Verified in the browser

- Live mode at 1 s: one `← request refreshTimer_Tick applied model v66 · 66 model change(s) coalesced into 1 update` per second while the simulator produced ~20 model changes per second.
- Burst 50: `events received 66 → 116 (+50), updates applied still 1`; the next tick applied one update.
- Start timer again: `refused — refreshTimer is already running at 1 s — start timers once`.
- Cadence 1 s → 250 ms on the running timer: ticks 250 ms apart right after `cadenceComboBox_SelectedIndexChanged … refreshTimer.Interval reprogrammed on the running timer`.
- Live mode off: `DashboardSimulator.Stop()` and `refreshTimer.Stop()` lines; no further ticks.
- Note: the timer runs in the browser. A background tab is throttled by the browser (ticks 1–3 s apart whatever the cadence), so keep the tab in front while comparing cadences.
