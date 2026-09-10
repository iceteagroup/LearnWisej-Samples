# Timer ownership — who starts it, who stops it, who is allowed to start it again

*"A timer belongs somewhere. Ownership determines cleanup. A duplicated timer can double update traffic and make
bugs appear random."* — this file is that rule applied to Module 4.

## Who owns what in this sample

| Thing | Owner | Lifetime | Started by | Stopped by |
|---|---|---|---|---|
| `refreshTimer` (`Wisej.Web.Timer` in the page's `components` tray) | **the page** (`MainPage`) | one browser tab | `StartRefreshTimer()`, called only from `liveModeCheckBox_CheckedChanged` | `StopRefreshTimer()` on live mode off, on the simulator fault, and on `Disposed` |
| `DashboardSimulator` (a task started with `Application.StartTask`) | **the page** | one browser tab | `_simulator.Start()` on live mode on | `_simulator.Stop()` on live mode off / dispose; the loop also exits by itself when `IsDisposed` or `!_liveMode` |
| `DashboardModel` | **the page** | one browser tab | constructed with the page | garbage-collected with the page |
| The polling fallback | **the session** | while live mode is on **and** there is no WebSocket | `Application.StartPolling(1000)` in `StartPollingFallback()` | `Application.EndPolling()` in `StopPollingFallback()`, and on dispose |

Nothing here is `static`. A static timer, model or simulator would be shared by every session on the server: one
user's "live mode off" would stop another user's dashboard, and the counters would mix sessions. `SERVER STATE`
prints `ClientId` / `SessionId` so the reviewer can open two tabs and see two independent sets of numbers.

## The five rules, and where each one lives in the code

| Rule (lesson) | Where |
|---|---|
| **Start timers once** | `_timerStarted` in `StartRefreshTimer()`. A second call traces `refused — refreshTimer is already running at 1 s — start timers once` and shows a warning banner. `duplicateTimerButton` exists only to trigger it. |
| **Stop timers when the view is no longer active** | `StopRefreshTimer()` from `liveModeCheckBox_CheckedChanged`; `StopEverythingOnDispose()` wired in the constructor (`this.Disposed += …`) stops the timer, the simulator and the polling. |
| **Do not create a new timer on every refresh click** | There is exactly one `refreshTimer`, created in `InitializeComponent()`. Changing the cadence assigns `refreshTimer.Interval` on the **existing** timer (`cadenceComboBox_SelectedIndexChanged`) — it never news up another one. |
| **Avoid long-running work inside a tick** | `slowTickCheckBox` makes the tick sleep 1 500 ms so the cost is visible. The rule the sample follows otherwise: the tick only *applies* state (`_model.Snapshot()` → labels); producing state is the simulator task's job. |
| **Use a flag to prevent overlapping ticks** | `_refreshInProgress`, set in `try`, cleared in `finally`. A tick that arrives while it is true is counted in `_skippedTicks` (`TICKS REFUSED`) and dropped. |

## The honest version of "no overlapping ticks"

Two mechanisms prevent overlap here, and it matters which one you are actually watching:

1. **The framework.** Wisej.NET serialises the requests of one session. A timer tick is a browser request, so
   while a 1 500 ms tick is being processed the next tick's request waits its turn — it does not run concurrently
   with the first. This is why, with the slow tick armed at 250 ms cadence, you see updates about 1.5 s apart
   rather than a pile of half-applied refreshes.
2. **The flag.** `_refreshInProgress` is the guard the lesson asks for, and it is what you would need if the
   refresh were moved off the request thread (a task, a hub event, a `System.Threading.Timer` in a service) where
   the framework's serialisation does not apply. In this sample it is the second line of defence: `TICKS REFUSED`
   is expected to stay at or near **0** precisely *because* rule 1 already holds.

Do not delete the flag because the counter reads 0. The day the refresh becomes out-of-bound, the flag is the only
thing standing between you and two refreshes writing the same labels.

## Cleanup checklist for a page that owns live machinery

- [x] The timer is a component of the page (`new Wisej.Web.Timer(this.components)`), so `Dispose(disposing)`
      disposes it with the page.
- [x] `this.Disposed += (s, e) => StopEverythingOnDispose();` — flag off, simulator stopped, timer stopped,
      polling ended, everything inside a `try`/`catch` that logs instead of throwing (the session may already be
      tearing down).
- [x] The background loop's condition is `_running && generation == _generation && !stopRequested()`, where
      `stopRequested` is `() => this.IsDisposed || !_liveMode` — two independent reasons to exit, checked every
      50 ms.
- [x] `EndPolling()` is called exactly once, guarded by `_polling`, so an idle session never keeps polling.
- [x] Exceptions in the task are caught **inside** the task (`DashboardSimulator.Loop` → `SimulatorFailed`),
      logged to the server console with the `ClientId`, and turned into a safe banner plus an automatic
      live-mode-off, so the page is always recoverable from the UI.

## Evidence (what the running app shows)

*Not yet executed in a browser: the sample builds clean but the reviewer runs it. These are the expected lines.*

- **Start timer again (anti-pattern), while live:**
  `← request duplicateTimerButton_Click the browser asked for a second refreshTimer.Start()` then
  `• server duplicateTimerButton_Click refused — refreshTimer is already running at 1 s — start timers once`,
  the amber banner, and the status flips to `● second timer refused — the page owns refreshTimer`.
  The tick lines keep arriving at the **same** rate afterwards — no doubling.
- **Start timer again, while live mode is off:**
  `• server duplicateTimerButton_Click nothing to duplicate — refreshTimer is stopped (live mode is off)` —
  the button cannot create a stray timer either.
- **Live mode off:** `• server DashboardSimulator.Stop() cooperative stop — the loop exits at its next check
  (≤ 50 ms)`, `• server refreshTimer.Stop() the view is no longer live …`, and (without a WebSocket)
  `• server Application.EndPolling() …`. The tick lines stop; the dashboard keeps the last snapshot, applied by
  the checkbox request itself.
- **Closing the tab:** no trace is visible any more, but the server console stays quiet — the loop exits on
  `IsDisposed` within 50 ms instead of running forever.
