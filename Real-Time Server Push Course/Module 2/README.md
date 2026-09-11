# TicketOpsLive · Real-Time Apps with Server Push · Module 2

Local lab build for **Module 2 · Who Owns the UI: Sessions, Context, Exit, and Timeout**: the Session Inspector +
Lifecycle Logger of TicketOps Live. A diagnostics group box (`diagnosticsGroupBox`) shows the current time, the
client id, the session id, the browser, the server thread serving the current event and a per-session counter kept
in `Application.Session`; a lifecycle log (`lifecycleListBox`) records load, the counter, the background update,
other sessions joining and leaving (through a global `SessionRegistry`), the session timeout and the exit.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 2/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5302
```

Then open <http://localhost:5302>, and open it a second time (or click **Open second window**).

## What to try

| Control | What you should see |
|---|---|
| Counter +1 | `counterLabel` grows; a second tab keeps its own value |
| Background update | ~1.5 s later "Background update at …" appears in the log with no click in between; the server thread id changes |
| Open second window | a second tab: same client id (it identifies the browser), its own session id and counter; both logs show `Session xxxxxxxx joined` |
| Clear log | empties the lifecycle log |
| Close or refresh the other tab | once its session ends, this tab logs `Session xxxxxxxx left` without anyone clicking here |

**Server thread.** `threadLabel` is refreshed on every event: the id moves because the server thread is a pool
thread reused for whichever session sends the next event. Out-of-bound code needs the *context*, not the thread.

## After a refresh (F5), and after the session ends

| What you look at | After F5 | Why |
|---|---|---|
| `counterLabel` (`Application.Session.Counter`) | back to 0 | the bag belonged to the session that ended |
| lifecycle log | empty | control state of a page that no longer exists |
| `clientIdLabel` / `sessionIdLabel` | same client id, new session id | a new session in the same browser |
| `liveSessionsLabel` | the old id leaves, the new one joins | the global registry outlives sessions |

When a session ends (`Application.Exit()` or the timeout), `ApplicationExit` logs `TicketOps session exited: <ClientId>`
to the server console and `Cleanup()` unsubscribes `SessionsChanged`, unregisters the session and detaches
`ApplicationExit`/`SessionTimeout` — once, even though `Disposed` calls it too. About two minutes before the idle timeout,
`Application.SessionTimeout` logs "Session is about to time out" and Wisej.NET shows its "prolong the session?" dialog.

**Polling fallback.** Add `"enableWebSocket": false` to `Default.json` and restart: the background update still
arrives (about a second late) because polling is requested while the task runs.

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| Diagnostics group box on `MainPage` | [`MainPage.Designer.cs`](TicketOpsLive/MainPage.Designer.cs) — `diagnosticsGroupBox` |
| Current time, client id, browser device type, session counter | `timeLabel`, `clientIdLabel`, `sessionIdLabel`, `browserLabel` (`DescribeBrowser()`), `counterLabel` |
| Counter in `Application.Session` | [`MainPage.cs`](TicketOpsLive/MainPage.cs) `MainPage_Load`, `incrementButton_Click` |
| Buttons to increment the counter and simulate a background update | `incrementButton_Click`, `backgroundButton_Click` (`Application.Current` → `StartTask` → `Application.Update(context, …)`) |
| Cleanup logging when the application exits | `Application_ApplicationExit` → `Cleanup()` |
| Refresh the browser and observe what persists | the table above |
| Walkthrough: one cleanup rule for every subscription or task | [`docs/CleanupRules.md`](TicketOpsLive/docs/CleanupRules.md) |

## Self-check answers

- **Why capture `Application.Current` before the out-of-bound action?** It can only be read in context. On a task
  or a thread-pool callback there is no context, and `Application.Update` would not know which session owns
  `lifecycleListBox`. `Application.Update(context, …)` restores the captured one, runs the callback and pushes once.
- **What if the counter were static?** Every session would increment the same field: the number would be the sum
  of everybody's clicks and would never reset until the server process stops.
- **Which cleanup action for a global service subscription?** A matching `-=` on a path that always runs, plus an
  `IsDisposed` guard in the handler. Here `SessionsChanged +=` / `Register` in Load, `-=` / `Unregister` in
  `Cleanup()`, called from both `ApplicationExit` and `Disposed`.
