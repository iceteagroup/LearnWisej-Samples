# Session cleanup — logout, timeout and ApplicationExit (lab step 6)

Lab step covered: *document cleanup requirements for timeout and logout*. On the desktop the process ended when the
user closed the window and the OS released everything. On the server a browser tab can be abandoned, a laptop can
sleep, a network can drop — the session ends **without** the user ever reaching "workflow complete", and whatever the
session held on the server stays held unless something releases it. The common pitfall: *letting session state
survive logout or user switching*.

## The exits and what fires

| Exit | Trigger | Event / call | Cleanup runs… | UI still there? |
|---|---|---|---|---|
| **Logout** | the user clicks *Sign out* (`buttonSignOut`) | `MainPage.buttonSignOut_Click` → `SessionCleanup.Run("logout", …)` | immediately, in the request | yes — the page stays usable (anonymous) |
| **Timeout** | no request from the browser for `sessionTimeout` seconds (`Default.json`) | static `Application.SessionTimeout` (`HandledEventArgs`) fires **before** the session is destroyed; the built-in *"prolong the session?"* dialog appears while `Handled == false`; if nobody answers the session ends → `Application.ApplicationExit` | on `ApplicationExit` — not on `SessionTimeout`, because the user may still click *continue* | the dialog, then nothing |
| **ApplicationExit** | the session is torn down for any reason: timeout expired, `Application.Exit()`, browser gone, server recycling | static `Application.ApplicationExit` | in the handler, with no UI work (the page may already be disposed) | no |

`Application.ApplicationExit` is a **static** event. It is subscribed once per session in `Program.Main`, not by the
page: Wisej.NET may dispose the page before the event fires, and a handler the page detached in `Dispose` would then
never run.

```csharp
// Program.Main
Application.ApplicationExit += (s, e) =>
{
    try
    {
        foreach (var step in SessionCleanup.Run("ApplicationExit", Application.SessionId))
            Console.WriteLine($"[OrderDesk] session {SessionCleanup.ShortId(Application.SessionId)} · {step}");
    }
    catch (Exception ex) { /* never throw into the session teardown */ }
};
```

The sample does not handle `Application.SessionTimeout`: the built-in prolong dialog stays, and nothing is released
until the session really ends. Setting `e.Handled = true` there would suppress the dialog (useful for a kiosk, or when
the app draws its own countdown); a page that subscribes to it must unsubscribe in `Dispose`, or it is never collected.

## The one routine — `SessionCleanup.Run(reason, sessionId)`

Logout and every exit call the **same** method (`Services/SessionCleanup.cs`), so nothing can be released on one path
and forgotten on the other. It is idempotent — running it on logout and again on `ApplicationExit` is harmless — and
returns one line per step:

| Step | Requirement (lesson) | What the sample does |
|---|---|---|
| 1 | **Temporary files / staged report output** | real: deletes `App_Data/tmp/<session>/` (created at sign-in by `SessionCleanup.CreateWorkspace`, holding `report-job.txt`) |
| 2 | **Report jobs** | the requirement: cancel queued jobs keyed by this session (Module 6's `ReportQueue` is where this becomes real) |
| 3 | **Long-running transactions** | the requirement: roll back any open unit of work (the in-memory repository holds none) |
| 4 | **Locks** | the requirement: release row/record locks held on behalf of this session |
| 5 | **Per-user context** | real: `SessionContext.Reset()` — only this session becomes anonymous |

The requirement list for a production system, in the order the lesson gives it:

1. **Long-running transactions** — never span a transaction across requests; if the design insists, roll it back here.
2. **Temporary files** — everything under a per-session workspace, deleted here; a sweeper (not built in this sample)
   for sessions that died with the process.
3. **Report jobs** — queued jobs owned by the session are cancelled (or re-owned by the user, if results must survive
   a reconnect — a design decision to write down).
4. **Locks** — pessimistic row locks, checked-out documents, "being edited by" flags: released here, with a timestamp
   so a sweeper can also release them if the exit handler never ran.
5. **Session context** — `SessionContext.Reset()` last, so the steps above can still read who the user was.

## Evidence

After *Sign in as kelly*, `App_Data/tmp/<session>/report-job.txt` exists on disk. After *Sign out* it is gone, the
Session card says `(not signed in)` and the grid shows all orders again. When a real session ends, the host console
prints the same five steps (`[OrderDesk] session a4f9c2e1 · temp files: deleted …` … `UserContext: SessionContext.Reset()
→ session a4f9c2e1 is anonymous again (ApplicationExit)`) — the proof that both exits share one routine.

> **Not yet verified at runtime.** The real `Application.SessionTimeout` → `ApplicationExit` path has not been
> observed. To see it, set `sessionTimeout` low in `Default.json`, sign in and let the tab idle. A process that is
> killed never runs the exit handler and leaves the session folder behind.
