# TicketOpsLive · Real-Time Apps with Server Push · Module 2

Local lab build for **Module 2 · Who Owns the UI: Sessions, Context, Exit, and Timeout**. It is the
**Session Inspector + Lifecycle Logger** the walkthrough names: a diagnostics group box (`diagnosticsGroupBox`) that
shows what the current session actually is — `Application.ClientId`, `Application.SessionId`, `Application.Browser`,
the server thread serving *this* event, `Application.IsWebSocket`, the per-session counter in `Application.Session`
and, right underneath, the same counter kept in a `static int` so the trap can be watched leaking across sessions —
plus a lifecycle logger (`lifecycleListBox`) that records load, every button, the arrival of a background update,
`ApplicationExit`, `SessionTimeout` and disposal, and a live-session list fed by a **global** `SessionRegistry`.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 2/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5302
```

Then open <http://localhost:5302> — and, for the interesting half of this module, open it **twice** (or click
`Open second session ↗`). (Visual Studio: open `TicketOpsLive.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Button | Path | What you should see |
|---|---|---|
| (page load) | — | lifecycle log `Page loaded for this session.`; trace `← request MainPage_Load … IsWebSocket=false (the socket opens after this response)` and `• server SessionRegistry.Register xxxxxxxx registered · SessionsChanged subscribed`; the inspector fills in |
| Session counter +1 | success · per-session state | `counterLabel` grows; `SERVER STATE` shows `session.Counter=n`; a second tab keeps its own value |
| Static counter +1 (trap) | the anti-pattern | `sharedCounterLabel` grows **and so does the other tab's**; amber banner "The static counter is shared by every session on this server"; trace `• server static int _sharedCounter …` |
| Background update | progress | the handler returns at once; ~1.5 s later `→ push Application.Update(context, …) "Background update at …"` arrives **with no request in between**, and `threadLabel` shows a *task thread* id |
| Background fault | failure | the task throws after 500 ms, catches it **inside**, writes the detail to the server console (`[TicketOpsLive] … background job failed for client …`), pushes a safe red banner, and re-enables both buttons in `finally` |
| Background update (after the fault) | recovery | runs normally again from the same page; the banner clears |
| Open second session ↗ | sessions | `Application.Navigate("/", "_blank")`: a second tab with the **same** `ClientId` (it identifies the browser) but its **own** `SessionId` and its own counter — and both tabs log `Session yyyyyyyy joined · 2 live session(s)` through the registry |
| End this session | lifecycle | `Application.Exit()` → `TicketOps session exited: <ClientId>` in the lifecycle log *and* on the server console, `cleanup (ApplicationExit) …` on the console, and the **other** tab logs `Session xxxxxxxx left · 1 live session(s)` without anyone clicking there |
| Clear trace | — | empties the right-hand list (the lifecycle log is kept) |
| (wait for the timeout) | lifecycle | `Application.SessionTimeout` fires before the session dies: amber banner, `SessionTimeout: this session is about to expire.` in the log, and Wisej.NET's own "prolong the session?" dialog (the handler leaves `e.Handled = false`) |

**Watch the thread id.** `threadLabel` is refreshed on every event and prints `#id · which kind of thread · event n`.
Click the same button three times and the id moves: the server thread is a pool thread, reused for whichever session
sends the next event. It is never "your" thread — which is why out-of-bound code needs the *context*, not the thread.

**Polling fallback.** Set `"enableWebSocket": false` in `Default.json` and restart: `websocketLabel` stays `false`,
`Background update` logs `• server Application.StartPolling(1000) …`, the browser console shows one
`Wisej: Poll request.` per second, the pushed line still arrives (about a second late) and `EndPolling()` stops the
polls when the task ends. Remove the setting to get the WebSocket back.

## After F5, and after "End this session"

Refresh the browser (F5): the old session is torn down and a **new** one starts.

| What you look at | After F5 | Why |
|---|---|---|
| `counterLabel` (`Application.Session.Counter`) | back to **0** | the bag belonged to the session that just died |
| `sharedCounterLabel` (`static int`) | **unchanged, keeps counting** | one field for the whole server process — the trap, visible |
| lifecycle log / trace / banner | empty | control state of a page that no longer exists |
| `clientIdLabel` / `sessionIdLabel` | new ids | a new session |
| `liveSessionsLabel` (registry) | the old id leaves, the new one joins | a global service outlives sessions — that is its job |

After `End this session`: `ApplicationExit` fires, the handler logs `TicketOps session exited: <ClientId>` to the log
and to the server console, `Cleanup("ApplicationExit")` unsubscribes `SessionsChanged`, unregisters the session and
detaches `ApplicationExit`/`SessionTimeout` — once, even though `Disposed` calls `Cleanup` too. Any other open tab
sees `Session xxxxxxxx left` arrive out-of-bound. If a tab is simply closed instead, the same "left" line appears when
the session times out rather than immediately: an explicit exit is prompt, a timeout is eventual.

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| Add a diagnostics group box to `MainPage` | [`MainPage.Designer.cs`](TicketOpsLive/MainPage.Designer.cs) — `diagnosticsGroupBox` ("Session inspector") |
| Display current time, client id, browser device type if available, and a session-specific counter | `timeLabel`, `clientIdLabel`, `sessionIdLabel`, `browserLabel` (`DescribeBrowser()` → `Application.Browser.Type/Version/OS/Device`), `counterLabel` |
| Store the counter in `Application.Session` or a per-session object | [`MainPage.cs`](TicketOpsLive/MainPage.cs) `MainPage_Load` (`if (Application.Session.Counter == null) …`) and `incrementButton_Click` |
| Buttons to increment the counter and to simulate a background update | `incrementButton_Click`, `backgroundButton_Click` (`Application.Current` → `StartTask` → `Thread.Sleep(1500)` → `Application.Update(context, …)`) |
| Add cleanup logging when the application exits | `Application_ApplicationExit` (`Console.Error.WriteLine("TicketOps session exited: " + _clientId)`) → `Cleanup()` |
| Refresh the browser and observe what persists | the table above + [`docs/SessionInspectorNotes.md`](TicketOpsLive/docs/SessionInspectorNotes.md) |
| The counter is session-specific, not static (acceptance) | `Application.Session.Counter` vs the deliberate `static int _sharedCounter` shown next to it |
| A background update modifies the correct browser session (acceptance) | the captured `context` in `backgroundButton_Click`; the pushed line names the target `ClientId` |
| The diagnostics panel shows when the background update arrives (acceptance) | `lifecycleListBox` line `Background update at …` + the `→ push` trace line |
| Show every path (success / progress / failure / recovery) | `incrementButton_Click` · `backgroundButton_Click` · `faultButton_Click` (caught inside the task) · `FinishTask` restoring the UI in `finally` |
| Walkthrough deliverable: one cleanup rule for every subscription or task | [`docs/CleanupRules.md`](TicketOpsLive/docs/CleanupRules.md) |
| Walkthrough deliverable: where state lives + the static-field trap | [`docs/WhereStateLives.md`](TicketOpsLive/docs/WhereStateLives.md) |
| Walkthrough deliverable: session vs user vs tab vs thread vs context | [`docs/SessionInspectorNotes.md`](TicketOpsLive/docs/SessionInspectorNotes.md) |

## Where things live

```
TicketOpsLive/
├─ MainPage.cs                 load/context capture, the two counters, the background task and the fault,
│                              ApplicationExit / SessionTimeout / Disposed → the idempotent Cleanup(),
│                              the SessionRegistry subscription, BeginPush/EndPush
├─ MainPage.Designer.cs        diagnosticsGroupBox, lifecycle card, trace card, action bar (opens in the Wisej Designer)
├─ Models/
│  ├─ SessionInfo.cs           what the registry keeps: a session id and a start time — values only
│  └─ SessionsChangedEventArgs.cs   Joined / Left + the live count
├─ Services/
│  └─ SessionRegistry.cs       the GLOBAL service: locked dictionary, raises SessionsChanged on a thread-pool thread
├─ Program.cs                  Application.MainPage = new MainPage()
├─ Startup.cs                  Kestrel host (app.UseWisej())
├─ Default.json                Wisej.NET application config (add "enableWebSocket": false to see the fallback)
└─ docs/
   ├─ SessionInspectorNotes.md  session vs user vs tab vs server thread vs context; what survives F5, exit and timeout
   ├─ CleanupRules.md           one cleanup rule per subscription/task — the walkthrough deliverable
   └─ WhereStateLives.md        control state / Application.Session / session service / global service + the static trap
```

## Self-check answers

**Reflection — why is `Application.Current` captured before starting the out-of-bound action?**
Because it can only be read *in* context. Inside a click handler the session context is established (the browser made
a request); on a task thread or a thread-pool callback of a global service there is no context to read, and
`Application.Update` would not know which of the server's sessions owns `lifecycleListBox`. `_context =
Application.Current` in `MainPage_Load` (and the local `var context = Application.Current` in the click handlers)
takes that handle while it is available; `Application.Update(context, …)` later restores it, runs the callback in that
session and pushes once. `Application.StartTask` carries the context for you — the explicit capture is what you need
for everything that does *not* start from your session, which is exactly `Registry_SessionsChanged`.

**Reflection — what would happen if the counter were static?**
Every session would increment the same field, so the number would be the sum of everybody's clicks, and it would
never reset — not on refresh, not on exit, only when the server process stops. The sample ships that exact bug next
to the correct version: `static int _sharedCounter` behind `Static counter +1 (trap)`. Open two tabs, click it in one,
and the other tab's value moves. The same mistake with `CurrentTicketId` means users overwrite each other's selection;
with a `CancellationTokenSource` it means one user cancels another user's job; with a `List<MainPage>` it means the
server keeps dead sessions alive. Per-user values belong in control state, `Application.Session`, or a per-session object.

**Reflection — which cleanup action would you add for a global service subscription?**
A matching `-=` on a path that always runs, plus an `IsDisposed` guard in the handler. Here: `SessionsChanged +=` and
`Register(...)` in `MainPage_Load`, `SessionsChanged -=` and `Unregister(...)` in `Cleanup()`, which is called from
**both** `Application_ApplicationExit` and `MainPage_Disposed` (either can come first) and is made idempotent by
`_cleanedUp`. The full list — every subscription and every task in this sample, with its release point — is
[`docs/CleanupRules.md`](TicketOpsLive/docs/CleanupRules.md).

**Checkpoint — why can a background task started from a page update that page?**
Because `Application.StartTask` starts the thread *with the calling session's context*. The task can therefore read
`Application.ClientId`, touch this session's controls, and call `Application.Update(this)` / `Application.Update(context, …)`
to flush them to the browser. Nothing reaches the browser until that call: the task changes server-side state, the
push delivers it.

**Checkpoint — why should a global service not directly own page controls?**
Because its lifetime is the process and the control's lifetime is a session. A static list of pages keeps sessions in
memory after the users are gone, pushes into dead UIs, and touches controls from threads that hold no session context.
`SessionRegistry` therefore stores values (`SessionInfo` = id + start time), raises events carrying values, and lets
each subscriber restore its own context and update its own controls.

**Checkpoint — why must every subscription have an unsubscribe plan?**
Because the subscriber dies and the publisher does not. A global event holds a delegate that holds `MainPage`, which
holds every control of a session; without the `-=` the session can never be collected, and its handler keeps running
against a disposed page for the life of the server. The plan has to cover both exits — `ApplicationExit` (an explicit
`Application.Exit()`, a timeout) and `Disposed` (the page went away) — which is why `Cleanup()` is idempotent and
wired to both.
