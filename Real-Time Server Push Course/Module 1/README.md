# TicketOpsLive · Real-Time Apps with Server Push · Module 1

Local lab build for **Module 1 · WebSocket Push vs. Polling: Build the Mental Model**: the first screen of
TicketOps Live. A live status strip (`statusPanel`, `clockLabel`, `connectionLabel`, `activityLabel`,
`serverLoadBar`, `startButton`, `stopButton`) driven by a simulated server heartbeat, plus the walkthrough's
in-request **Refresh**, the 5-step **server event** pushed with `Application.Update(this)`, and an update trace.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 1/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5301
```

Then open <http://localhost:5301>. (Visual Studio: open `TicketOpsLive.slnx`, press F5.)

## What to try

| Control | What you should see |
|---|---|
| Refresh | `statusLabel` shows "Refreshed at …" — the change comes back with the click's response (no `Application.Update`) |
| Start server event | `pushStatusLabel` goes `step 1/5` … `completed`, 600 ms apart, with no further clicks |
| ▶ Start heartbeat | once per second: clock, load bar and `Heartbeat #n` are pushed; the button stays disabled while running |
| ■ Stop | the loop ends within one second and Start is enabled again |
| Update trace | one line per request and per pushed step |

**Polling fallback.** Add `"enableWebSocket": false` to `Default.json` and restart: `connectionLabel` shows
"No WebSocket — polling every second" while a task runs, the browser console shows one `Wisej: Poll request.` per
second, and the heartbeat still arrives (about a second late). Polling is requested when a task starts without a
WebSocket, not in `MainPage_Load` (`IsWebSocket` is still false during Load), and ended when the task finishes.

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| `statusPanel`; `clockLabel`, `connectionLabel`, `activityLabel`; `serverLoadBar`; `startButton`, `stopButton` | [`MainPage.Designer.cs`](TicketOpsLive/MainPage.Designer.cs) |
| Simulated heartbeat on `startButton` | `startButton_Click` → `Application.StartTask(HeartbeatLoop)` |
| Clock, load and activity once per second | `HeartbeatLoop()`: `Application.Update(this, () => …)` then `Thread.Sleep(1000)` |
| Stop on `stopButton` or when the page is disposed | `_running = false`; loop condition `_running && !this.IsDisposed`; `Disposed += …` |
| Starting twice does not create two loops | `_running` guard + `startButton.Enabled = false` |
| Walkthrough: a visible connection / update status area | `connectionLabel` (`RenderConnection()`) |
| Walkthrough: one-paragraph architecture note | [`docs/ArchitectureNote.md`](TicketOpsLive/docs/ArchitectureNote.md) |

## Self-check answers

- **Why call `Application.Update(this)` inside the loop?** The loop runs on a task, outside any browser request.
  Changing `clockLabel.Text` only changes the server-side control; `Application.Update(this)` pushes the pending
  changes over the WebSocket right away.
- **What changes without WebSocket?** `Application.Update` has no live channel. The sample requests
  `Application.StartPolling(1000)` while a task runs, so each poll carries whatever is pending, and calls
  `EndPolling()` when the task ends.
- **Why is `_running` not static?** A static is shared by every session: one user's Start would block everyone
  else's and one user's Stop would stop another's heartbeat. Each tab has its own `MainPage` instance.
- **Risk of updating every 10 ms?** Every push costs serialization, a WebSocket frame and browser work; at 10 ms that
  is 100 pushes per second per session for states no one can read. Once per second is plenty for a status strip.
