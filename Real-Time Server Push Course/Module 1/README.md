# TicketOpsLive · Real-Time Apps with Server Push · Module 1

Local lab build for **Module 1 · WebSocket Push vs. Polling: Build the Mental Model**. It is the first screen of
TicketOps Live from the lab guide — a **live status strip** (`statusPanel`, `clockLabel`, `connectionLabel`,
`activityLabel`, `serverLoadBar`, `startButton`, `stopButton`) driven by a simulated server heartbeat — plus the three
update mechanisms of the lesson side by side: the **in-request** update, the **out-of-bound WebSocket push** and the
**polling fallback**, and the walkthrough's 5-step server event.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 1/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5301
```

Then open <http://localhost:5301>. (Visual Studio: open `TicketOpsLive.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Button | Path | What you should see |
|---|---|---|
| (page load) | — | trace `← request MainPage_Load … IsWebSocket=false (the socket opens after this response)`; the strip says `○ HTTP only` until the first event after the socket connects |
| Refresh (in-request) | mechanism 1 · success | `statusLabel` = "Refreshed at …" and trace `• server in-request update … returned with the response, no Application.Update()`; the strip flips to `● WebSocket connected` |
| Server event ×5 (push) | mechanism 2 · success | `Application.StartTask` runs 5 steps 600 ms apart; five `→ push Application.Update(this) pushStatusLabel = "Server event step n/5"` lines arrive **without any click**; the button is re-enabled in `finally` |
| ▶ Start heartbeat | progress | one push per second: clock, load bar, `Heartbeat #n · load x% · pushed HH:mm:ss.fff`; trace `→ push Application.Update(this, …) heartbeat #n … one flush`; **SERVER STATE** shows `running=true beats=n pushes=n` |
| ▶ Start heartbeat (again, while running) | guard | the button is disabled; if the click gets through anyway (`_running == true`) the trace says `refused — the heartbeat is already running` — no second loop |
| ■ Stop | cooperative stop | `_running = false`; within one second `→ push … heartbeat stopped — stopped by operator after n beats (finally block)`; Start is enabled again |
| Inject fault (while running) | failure | the next beat throws `InvalidOperationException` **inside the task**; it is caught, the detail goes to the server console (`[TicketOpsLive] … heartbeat failed for client …`), the UI shows the safe red banner *"Heartbeat failed. See the server log"*, and Start is re-enabled by `finally` |
| ▶ Start heartbeat (after the fault) | recovery | the loop starts again from the same page, the banner clears |
| Burst 100 @ 10 ms | cadence (anti-pattern) | 100 model changes → **100 pushes** in ≈1.6 s; the load bar flickers |
| Batched (10 pushes) | cadence | the same 100 changes → **10 pushes** in the same wall time; the final state is pushed immediately |
| Clear trace | — | empties the right-hand list |

**Polling fallback.** Set `"enableWebSocket": false` in `Default.json` and restart: the strip stays `○ HTTP only`,
Start heartbeat logs `• server Application.StartPolling(1000) no WebSocket when the task started → fallback polling ON`,
the browser console shows one `Wisej: Poll request.` per second, the beats still arrive (≈1 s late) and Stop logs
`Application.EndPolling() … fallback polling OFF` — the polls stop. Remove the setting to get the WebSocket back.

The right-hand card is the live push trace: `→ push` lines are pushes made by a task (`Application.Update`), `← request`
lines are requests the browser sent, `• server` lines are server decisions. Compare it with the Network panel of the browser.

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| Top panel `statusPanel`; labels `clockLabel`, `connectionLabel`, `activityLabel`; progress bar `serverLoadBar`; buttons `startButton`, `stopButton` | [`MainPage.Designer.cs`](TicketOpsLive/MainPage.Designer.cs) — `statusPanel` block and `panelActions` |
| Simulated heartbeat background task on `startButton` | [`MainPage.cs`](TicketOpsLive/MainPage.cs) `startButton_Click` → `Application.StartTask(HeartbeatLoop)` |
| Update the clock, a fake load value and an activity message once per second | `HeartbeatLoop()`: `Application.Update(this, () => { clockLabel…; serverLoadBar.Value…; activityLabel… })` then `Thread.Sleep(1000)` |
| Stop on `stopButton` or when the page is disposed | `stopButton_Click` sets `_running = false`; the loop condition is `_running && !this.IsDisposed`; the constructor wires `this.Disposed += (s, e) => _running = false` |
| Starting twice must not create two loops | `startButton_Click` guard on `_running`; `startButton.Enabled = false` while running |
| Every path visible (success / progress / cancellation / failure) | `faultButton_Click` + the `catch`/`finally` of `HeartbeatLoop`; `labelBanner`, `labelStatus`, the trace |
| Walkthrough deliverable: a visible connection / update status area | `connectionLabel` (`Application.IsWebSocket`) + `RenderConnection()` |
| Walkthrough deliverable: a one-paragraph architecture note | [`docs/ArchitectureNote.md`](TicketOpsLive/docs/ArchitectureNote.md) |

## Where things live

```
TicketOpsLive/
├─ MainPage.cs               the heartbeat loop, the three mechanisms, the cadence experiment, BeginPush/EndPush
├─ MainPage.Designer.cs      status strip, mechanisms card, trace card, action bar (opens in the Wisej Designer)
├─ Program.cs                Application.MainPage = new MainPage()
├─ Startup.cs                Kestrel host (app.UseWisej())
├─ Default.json              Wisej.NET application config (add "enableWebSocket": false to see the fallback)
└─ docs/
   ├─ ArchitectureNote.md    the end-to-end path of a server-generated event (lesson checkpoint) — one paragraph + the diagram
   └─ UpdateMechanisms.md    in-request vs. WebSocket push vs. polling, with the evidence from the trace
```

## Self-check answers (reflection questions of the lab)

- **Why does this lab call `Application.Update(this)` inside the loop?**
  The loop runs on a task, outside any browser request. Changing `clockLabel.Text` only changes the server-side
  control; without a request in flight nothing would carry the change to the browser. `Application.Update(this)`
  pushes the pending changes of the page over the WebSocket right now. The trace shows one `→ push` per beat.
- **What changes if WebSocket is not available?**
  `Application.Update` has no live channel to write to; the changes stay pending on the server. The sample requests
  `Application.StartPolling(1000)` when a task starts and `IsWebSocket` is false, so the browser asks every second and
  receives whatever is pending (verified with `"enableWebSocket": false`: the beats arrive about a second late), and it
  calls `EndPolling()` when the last task ends so idle sessions do not poll for nothing.
- **Why is `_running` not a static field?**
  A static is shared by every session on the server. With a static flag, the first user to click Start would block Start
  for everyone else, and Stop in one tab would stop another user's heartbeat. `_running` is an instance field of the
  page, and every browser tab has its own `MainPage` instance — see SERVER STATE, which prints the session's own `ClientId`.
- **What would be the risk of updating every 10 milliseconds?**
  Every push costs serialization, a WebSocket frame and browser work; at 10 ms that is 100 pushes per second per session.
  The cadence experiment shows it: 100 changes → 100 pushes vs. 10 pushes for the same result in the same wall time.
  Multiply by sessions and the server spends its time pushing states no human can read.
- **Checkpoint — the end-to-end path of a server-generated ticket event:** service changes data → service raises an event →
  a session receives it (in its own context) → the session updates its controls → Wisej.NET pushes the resulting UI changes
  when WebSocket is available. Drawn in [`docs/ArchitectureNote.md`](TicketOpsLive/docs/ArchitectureNote.md).
