# Session inspector notes — who owns the UI (Module 2 deliverable)

The walkthrough opens with five words people blur together. The Session inspector card exists to keep them apart,
because every real-time bug in this course starts with confusing two of them.

## Five things, five owners

| Thing | What it really is | How you see it in this sample | Lifetime |
|---|---|---|---|
| **User** | the human (or the login) | not shown — deliberately: nothing in this sample knows who you are | as long as the person exists |
| **Browser session** | one Wisej.NET application instance = one browser tab | `clientIdLabel` (`Application.ClientId`) and `sessionIdLabel` (`Application.SessionId`); `liveSessionsLabel` lists every live one | from the first request to `Application.Exit()`, a refresh, or the timeout |
| **Server thread** | a pooled worker that happens to be running your event right now | `threadLabel` — `#id · which kind of thread · event n — reused, not owned` | one event. The next event usually arrives on a different thread |
| **Application context** | the handle that tells out-of-bound code *which session* it is talking to | `_context = Application.Current` captured in `MainPage_Load`, used by `Application.Update(_context, …)` | as long as the session lives |
| **Global service** | one object for the whole server process, shared by every session | `SessionRegistry.Instance` → `liveSessionsLabel`, and the deliberately wrong `static int _sharedCounter` | the process |

**One login can open two tabs — and that is two sessions, not one.** `Open second session ↗` proves it:
`Application.Navigate("/", "_blank")` opens a second tab with its own `SessionId` and its own `MainPage` (the `ClientId` is the same: it identifies the browser, verified — two tabs of one Chrome share it, which is why the registry is keyed on `SessionId`), its own
`Application.Session.Counter`. The same user, the same browser, two independent UIs.

**The thread is reused, not owned.** `threadLabel` is refreshed on *every* event — clicks, task callbacks, and the
thread-pool callback of the registry. Click the same button a few times and the id moves. Nothing about the session
lives on the thread: Wisej.NET restores the session state for whichever pooled thread serves the next request. This
is why "just remember it in a `[ThreadStatic]`" is never the answer, and why out-of-bound code needs a *context*
rather than a thread.

## What persists across a refresh (F5)

A refresh is not a redraw — it is the end of one session and the start of another (Wisej.NET raises
`ApplicationRefresh`, the old session is torn down and a new `MainPage` is built).

| Value | Where it lives | Survives F5? |
|---|---|---|
| `counterLabel` — `Application.Session.Counter` | the per-session bag | **no** — the new session starts at 0 |
| `sharedCounterLabel` — `static int _sharedCounter` | the process | **yes** — it never resets while the server runs; that is the trap |
| `lifecycleListBox`, `listTrace`, the banner | control state of the disposed page | **no** — they belong to the page that went away |
| `liveSessionsLabel` | the global `SessionRegistry` | **yes** — but the list changes: the old session id leaves and a new one joins |
| `Application.SessionId` | the session | **no** — a new id for the new session (`ClientId` stays: it belongs to the browser) |

Expected sequence after F5 (not yet verified in a browser): the old session's `Cleanup()` unregisters it, so any
*other* open tab logs `Session xxxxxxxx left`, then `Session yyyyyyyy joined` for the new one. If the old session is
torn down lazily (the browser is simply gone rather than exiting cleanly), the "left" line arrives when the session
times out instead — which is exactly the difference between an explicit exit and a timeout.

## What happens on "End this session"

`Application.Exit()` terminates the session the way quitting a desktop application terminates the process:

1. `Application.ApplicationExit` fires → the handler writes `TicketOps session exited: <ClientId>` to the lifecycle
   log **and** to the server console (`Console.Error.WriteLine`, the "replace with your logging framework" line of the lab).
2. `Cleanup("ApplicationExit")` unsubscribes `SessionsChanged`, unregisters the session and detaches the
   `ApplicationExit` / `SessionTimeout` handlers.
3. Every other live session receives `SessionChange.Left` on a thread-pool thread and updates its own
   `liveSessionsLabel` through `Application.Update(_context, …)`.
4. The page is disposed; `Cleanup("Disposed")` runs and does nothing, because it is idempotent.

## What happens on a timeout

`Application.SessionTimeout` fires *before* the session dies. The sample leaves `e.Handled = false`, so Wisej.NET
shows its built-in "prolong the session?" dialog; the handler only logs the fact and shows an amber banner. If the
session does time out, it is removed from memory: page, controls and `Application.Session` go with it — which is why
`Registry_SessionsChanged` checks `IsDisposed` before it touches a control and why `FinishTask` restores nothing when
there is nothing left to restore.

## Evidence (what the running app shows — expected; not verified in a browser)

- Load: `← request MainPage_Load … IsWebSocket=false` and `• server SessionRegistry.Register  xxxxxxxx registered · SessionsChanged subscribed`.
- Two tabs: each `liveSessionsLabel` reads `live sessions: 2 · xxxxxxxx (this one)@… yyyyyyyy@…`, and each lifecycle
  log has its own `Page loaded for this session.` line.
- `Session counter +1` in tab A: tab A shows 1, tab B still shows 0. `Static counter +1` in tab A: **both** tabs show
  the same number as soon as anything refreshes them — tab B's next event proves the leak.
- `Background update`: `→ push Application.Update(context, …) "Background update at …" delivered to xxxxxxxx` about
  1.5 s after the click, with no request in between; `threadLabel` shows a *task thread* id, different from the click's.
- `End this session` in tab A: tab B logs `Session xxxxxxxx left · 1 live session(s)` and its trace shows
  `→ push Application.Update(_context, …) SessionRegistry: xxxxxxxx left → 1 live · pushed into yyyyyyyy from a thread-pool thread`.
- Server console: `TicketOps session exited: <ClientId>` followed by
  `[TicketOpsLive] … cleanup (ApplicationExit) …: SessionsChanged unsubscribed, session unregistered`.
