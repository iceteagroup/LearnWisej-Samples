# Cleanup rules — one rule for every subscription and every task (Module 2 walkthrough deliverable)

> "The goal is not just to avoid exceptions. The goal is to make ownership obvious."

Rule of the module: **nothing is subscribed or started in this sample without a written line saying who releases it
and when.** The table below is that list; if a future module adds a subscription, it adds a row here first.

| # | What is subscribed / started | Where | Released by | When | Idempotent? |
|---|---|---|---|---|---|
| 1 | `this.Disposed += MainPage_Disposed` | `MainPage()` ctor | the framework | the page is disposed (the page is both publisher and subscriber — it cannot outlive itself) | n/a |
| 2 | `SessionRegistry.Instance.SessionsChanged += Registry_SessionsChanged` | `MainPage_Load` | `Cleanup()` → `-=` | `ApplicationExit` **or** `Disposed`, whichever comes first | yes (`_cleanedUp`) |
| 3 | `SessionRegistry.Instance.Register(_clientId)` | `MainPage_Load` | `Cleanup()` → `Unregister(_clientId)` | same | yes (`_registered` + `Unregister` returns false for unknown ids) |
| 4 | `Application.ApplicationExit += Application_ApplicationExit` | `MainPage_Load` | `Cleanup()` → `-=` | same | yes |
| 5 | `Application.SessionTimeout += Application_SessionTimeout` | `MainPage_Load` | `Cleanup()` → `-=` | same | yes |
| 6 | background update task (`Application.StartTask`, 1500 ms) | `backgroundButton_Click` | itself: bounded work, `IsDisposed` checked inside every `Application.Update` callback, `finally` → `FinishTask` | when the sleep ends, always | yes (`FinishTask` no-ops on a disposed page) |
| 7 | background fault task (`Application.StartTask`, 500 ms then throws) | `faultButton_Click` | itself: `catch` inside the task, `finally` → `FinishTask` | when the exception is handled, always | yes |
| 8 | `Application.StartPolling(1000)` (fallback delivery) | `BeginPush()` | `EndPush()` in the task's `finally` | when the **last** task ends (`_pushers` reference count) | yes (`_polling` guard) |
| 9 | `Wisej.Web.Timer` | — | — | this module starts no timers; if it did, `Stop()` + `components.Dispose()` would be rows 10 and 11 | — |

## The pattern, in three lines

```csharp
// subscribe where you have the context…
_context = Application.Current;
SessionRegistry.Instance.SessionsChanged += Registry_SessionsChanged;
Application.ApplicationExit += Application_ApplicationExit;

// …guard where the event arrives…
if (this.IsDisposed) return;
Application.Update(_context, () => { if (this.IsDisposed) return; /* touch controls */ });

// …release exactly once, from both ends of the lifecycle.
private void Cleanup(string reason)
{
    if (_cleanedUp) return;
    _cleanedUp = true;
    SessionRegistry.Instance.SessionsChanged -= Registry_SessionsChanged;
    SessionRegistry.Instance.Unregister(_clientId);
    Application.ApplicationExit -= Application_ApplicationExit;
    Application.SessionTimeout -= Application_SessionTimeout;
}
```

`Cleanup` is called from **both** `Application_ApplicationExit` and `MainPage_Disposed` because either can happen
first: an explicit `Application.Exit()` raises the exit event before the page is disposed, while a closed tab that
times out may dispose the page first. Making it idempotent is cheaper than reasoning about the order.

## Why the registry never holds a page

`SessionRegistry` stores `SessionInfo` — a session id and a start time. Values only. If it stored `MainPage`
references (or the `IWisejComponent` context) in a static dictionary and a session forgot to unregister, the server
would hold that page, its controls and its whole session graph forever: the textbook real-time leak. Because it holds
only ids, the worst a missing `Unregister` can cause is a stale line in `liveSessionsLabel` — visible, harmless, and
fixed by the next `Register`/`Unregister` pair.

The event travels the other way for the same reason: the registry raises `SessionsChanged` with **values**
(`SessionId`, `Change`, `Count`) on a thread-pool thread, and each subscriber restores its *own* context before
touching its *own* controls. The service never learns what a control is.

## Failure containment

`SessionRegistry.Raise` invokes each handler in its own `try/catch` and logs to the server console: one broken
subscriber cannot stop the other sessions from being notified. On the subscriber side, `Registry_SessionsChanged`
catches `ObjectDisposedException` (the page can go away between the `IsDisposed` check and the push) and logs
anything else.

## Evidence (expected; not verified in a browser)

- Click `End this session`: the server console prints `TicketOps session exited: <ClientId>` and
  `[TicketOpsLive] … cleanup (ApplicationExit) for client …: SessionsChanged unsubscribed, session unregistered,
  ApplicationExit/SessionTimeout released.` — exactly **once**, even though `Disposed` also calls `Cleanup`.
- The other tab's `liveSessionsLabel` drops by one and its lifecycle log gains `Session xxxxxxxx left · n live session(s)`.
- Click `Background update` and close the tab before the 1.5 s elapse: the server console stays silent (no exception),
  because the `IsDisposed` guard skips the UI work and `FinishTask` returns without pushing.
- `Background fault`: `[TicketOpsLive] … background job failed for client …: System.InvalidOperationException …` in
  the console, a red banner in the UI, and both buttons enabled again — the `finally` ran.
