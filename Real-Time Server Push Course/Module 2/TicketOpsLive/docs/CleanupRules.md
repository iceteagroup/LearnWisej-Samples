# Cleanup rules — one rule for every subscription and every task

Nothing is subscribed or started in this sample without a line saying who releases it and when.

| # | What is subscribed / started | Where | Released by | When |
|---|---|---|---|---|
| 1 | `this.Disposed += MainPage_Disposed` | `MainPage()` ctor | the framework | the page is disposed |
| 2 | `SessionRegistry.Instance.SessionsChanged += Registry_SessionsChanged` | `MainPage_Load` | `Cleanup()` → `-=` | `ApplicationExit` or `Disposed`, whichever comes first (idempotent: `_cleanedUp`) |
| 3 | `SessionRegistry.Instance.Register(_sessionId)` | `MainPage_Load` | `Cleanup()` → `Unregister(_sessionId)` | same |
| 4 | `Application.ApplicationExit += Application_ApplicationExit` | `MainPage_Load` | `Cleanup()` → `-=` | same |
| 5 | `Application.SessionTimeout += Application_SessionTimeout` | `MainPage_Load` | `Cleanup()` → `-=` | same |
| 6 | background update task (`Application.StartTask`, 1500 ms) | `backgroundButton_Click` | itself: bounded work, `IsDisposed` checked inside every `Application.Update` callback, `finally` → `FinishTask` | when the sleep ends, always |
| 7 | `Application.StartPolling(1000)` (fallback delivery) | `BeginPush()` | `EndPush()` in the task's `finally` | when the last task ends |

## The pattern

```csharp
// subscribe where you have the context…
_context = Application.Current;
SessionRegistry.Instance.SessionsChanged += Registry_SessionsChanged;
Application.ApplicationExit += Application_ApplicationExit;

// …guard where the event arrives…
if (this.IsDisposed) return;
Application.Update(_context, () => { if (this.IsDisposed) return; /* touch controls */ });

// …release exactly once, from both ends of the lifecycle.
private void Cleanup()
{
    if (_cleanedUp) return;
    _cleanedUp = true;
    SessionRegistry.Instance.SessionsChanged -= Registry_SessionsChanged;
    SessionRegistry.Instance.Unregister(_sessionId);
    Application.ApplicationExit -= Application_ApplicationExit;
    Application.SessionTimeout -= Application_SessionTimeout;
}
```

`Cleanup` is called from both `Application_ApplicationExit` and `MainPage_Disposed` because either can happen first.
Making it idempotent is cheaper than reasoning about the order.

## Why the registry never holds a page

`SessionRegistry` stores `SessionInfo` — a session id and a start time. Values only. If it stored `MainPage`
references (or the `IWisejComponent` context) and a session forgot to unregister, the server would hold that page,
its controls and its whole session graph forever. Because it holds only ids, the worst a missing `Unregister` can
cause is a stale entry in `liveSessionsLabel`.

The event travels the other way for the same reason: the registry raises `SessionsChanged` with values (`SessionId`,
`Change`, `Count`) on a thread-pool thread, and each subscriber restores its own context before touching its own
controls. `SessionRegistry.Raise` invokes each handler in its own `try/catch`, so one broken subscriber cannot stop
the others from being notified.
