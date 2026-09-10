# Session cleanup — logout, timeout and ApplicationExit (lab step 6)

Lab step covered: *document cleanup requirements for timeout and logout*. On the desktop the process ended when the
user closed the window and the OS released everything. On the server a browser tab can be abandoned, a laptop can
sleep, a network can drop — the session ends **without** the user ever reaching "workflow complete", and whatever the
session held on the server stays held unless something releases it. The storyboard's common pitfall: *letting session
state survive logout or user switching*.

## The three exits and what fires

| Exit | Trigger | Event / call | Cleanup runs… | UI still there? |
|---|---|---|---|---|
| **Logout** | the user clicks *Sign out* (`buttonSignOut`) | `MainPage.buttonSignOut_Click` → `RunCleanup("logout")` | immediately, in the request | yes — the page shows the result and stays usable (anonymous) |
| **Timeout** | no request from the browser for `sessionTimeout` seconds (`Default.json`) | static `Application.SessionTimeout` (`HandledEventArgs`) fires **before** the session is destroyed; the built-in *"prolong the session?"* dialog appears while `Handled == false`; if nobody answers the session ends → `Application.ApplicationExit` | on `ApplicationExit` — not on `SessionTimeout`, because the user may still click *continue* | the dialog, then nothing |
| **ApplicationExit** | the session is torn down for any reason: timeout expired, `Application.Exit()`, browser gone, server recycling | static `Application.ApplicationExit` | in the handler, with no UI work (the page may already be disposed) | no |

`Application.SessionTimeout` and `Application.ApplicationExit` are **static** events. `MainPage` subscribes in its
constructor and unsubscribes in `Dispose` (`MainPage.Designer.cs` → `DetachApplicationEvents()`); a page that forgets
the second half is never collected.

```csharp
// MainPage() — subscribe
Application.SessionTimeout += Application_SessionTimeout;
Application.ApplicationExit += Application_ApplicationExit;

// MainPage.Designer.cs — Dispose(bool)
DetachApplicationEvents();   // -= both

private void Application_SessionTimeout(object sender, HandledEventArgs e)
{
    // Handled stays false: the built-in "prolong the session?" dialog must still appear.
    // Nothing is released yet — the user may click "continue"; ApplicationExit is the point of no return.
    if (IsDisposed) return;
    trace.Add(TraceKind.Server, "Application.SessionTimeout",
        $"session {Short(Application.SessionId)} is about to time out — Handled = false, the prolong dialog is shown; cleanup waits for ApplicationExit");
}

private void Application_ApplicationExit(object sender, EventArgs e)
{
    // No UI work here — the page may already be disposed; release what the session holds and log to the host.
    foreach (var step in SessionCleanup.Run("ApplicationExit", Application.SessionId))
        Console.WriteLine($"[OrderDesk] session {Short(Application.SessionId)} · {step}");
}
```

Setting `e.Handled = true` in `SessionTimeout` would suppress the prolong dialog (useful for a kiosk that must not
nag, or when the app draws its own countdown). This sample leaves it `false` on purpose so the built-in behaviour is
visible.

## The one routine — `SessionCleanup.Run(reason, sessionId)`

Logout and every exit call the **same** method (`Services/SessionCleanup.cs`), so nothing can be released on one path
and forgotten on the other. It is idempotent — running it on logout and again on `ApplicationExit` is harmless — and
returns one line per step for the trace:

| Step | Requirement (lesson) | What the sample does | Trace line |
|---|---|---|---|
| 1 | **Temporary files / staged report output** | real: deletes `App_Data/tmp/<session>/` (created at sign-in by `SessionCleanup.CreateWorkspace`, holding `report-job.txt`) | `cleanup  temp files: deleted <full path>\App_Data\tmp\a4f9c2e1 (1 file(s))` — or `temp files: nothing to release` |
| 2 | **Report jobs** | logged: cancel queued jobs keyed by this session (Module 6's `ReportQueue` is where this becomes real) | `cleanup  report jobs: cancel queued jobs keyed by this session (none running in the sample)` |
| 3 | **Long-running transactions** | logged: roll back any open unit of work (the in-memory repository holds none) | `cleanup  transactions: roll back any open unit of work (the in-memory repository holds none)` |
| 4 | **Locks** | logged: release row/record locks held on behalf of this session | `cleanup  locks: release row/record locks held on behalf of this session (none in the sample)` |
| 5 | **Per-user context** | real: `SessionContext.Reset()` — only this session becomes anonymous | `cleanup  UserContext: SessionContext.Reset() → session a4f9c2e1 is anonymous again (logout)` |

The requirement list for a production system, in the order the lesson gives it:

1. **Long-running transactions** — never span a transaction across requests; if the design insists, roll it back here.
2. **Temporary files** — everything under a per-session workspace, deleted here; a sweeper (not built in this sample) for sessions that died with
   the process.
3. **Report jobs** — queued jobs owned by the session are cancelled (or re-owned by the user, if results must survive
   a reconnect — a design decision to write down).
4. **Locks** — pessimistic row locks, checked-out documents, "being edited by" flags: released here, with a timestamp
   so a sweeper can also release them if the exit handler never ran.
5. **Session context** — `SessionContext.Reset()` last, so the steps above can still read who the user was.

`MainPage.RunCleanup` adds the UI half after the routine: in **Legacy statics** mode it also blanks `AppState.*` and
logs `⚠ boundary AppState cleared  ✕ the statics are one slot — this sign-out signed out EVERY session on the server`
— the reason the typed context exists.

## `buttonSignOut` — *Sign out*

* Trace `← JS→.NET sign out  kelly · UserContext (session)`, then the five `• server cleanup  …` lines with `(logout)`.
* The combos reset to **All** / **All customers**, the grid shows `5 of 5 orders · filter All · all customers`, the
  stores label shows `(not signed in)` for this session's context and `(nothing yet)` for *this page last wrote*.
* Status `● signed out (logout) · cleanup ran`.
* Green banner: `✓ Signed out: SessionContext.Reset() cleared this session's context and the cleanup routine released the session's temp files; report jobs, open transactions and locks are released in the same routine — the one place both logout and timeout call.`
  In Legacy statics mode with a signed-in static user the banner is amber and ends with
  ` In Legacy statics mode this logout also blanked AppState — for EVERY session on the server.`

## `buttonTimeout` — *Simulate timeout* (the progress path)

A real timeout takes `sessionTimeout` seconds of silence and ends the session — not something a lab wants to wait for
or lose the page to. The button runs the **same cleanup** after a visible 5-second countdown (`Wisej.Web.Timer`,
1 s ticks, `progressTimeout` bar) **without** ending the session:

* Trace `← JS→.NET simulate timeout  5 s countdown → the cleanup routine Application.SessionTimeout → ApplicationExit would run; the session itself stays alive`.
* Status counts down: `● session times out in 5 s…`, then `● session times out in 4 s… (a real timeout shows the built-in prolong dialog first)` … `1 s…` (amber); the progress bar fills 20 % per tick.
* At zero: trace `• server SessionTimeout (simulated)  cleanup must run BEFORE the session is destroyed — afterwards there is no session left to clean`, the five `cleanup` lines with `(timeout (simulated))`, status `● signed out (timeout (simulated)) · cleanup ran`.
* Amber banner: `⏱ Simulated timeout: the cleanup routine ran (temp files deleted, context reset; report jobs, transactions and locks logged) but the session was NOT ended. The real Application.SessionTimeout fires after the configured sessionTimeout, shows the built-in prolong dialog (this sample leaves Handled = false), and if nobody answers the session ends and Application.ApplicationExit runs this same routine.`

Pressing it again while the countdown runs does nothing (`timerTimeout.Enabled` guard).

## The real events

To see the real hooks, set a short `"sessionTimeout"` in `Default.json` (seconds), sign in, and leave the tab alone:

* Shortly before the limit the trace gets `• server Application.SessionTimeout  session a4f9c2e1 is about to time out — Handled = false, the prolong dialog is shown; cleanup waits for ApplicationExit` and the built-in prolong dialog appears.
* Answer it: the session continues, nothing was released (correct — the user is still there).
* Ignore it: the session ends; the **host console** (not the browser — there is no session left to push to) prints
  `[OrderDesk] session a4f9c2e1 · temp files: deleted …`, `… · report jobs: …`, `… · transactions: …`, `… · locks: …`,
  `… · UserContext: SessionContext.Reset() → session a4f9c2e1 is anonymous again (ApplicationExit)`; the folder
  `App_Data/tmp/a4f9c2e1/` is gone.

## Evidence

After *Sign in as kelly*, `App_Data/tmp/<session>/report-job.txt` exists on disk (the trace names the folder:
`• server workspace  App_Data\tmp\a4f9c2e1 created — temp files this session must release on logout/timeout`). After
*Sign out* or the simulated timeout it is gone and the trace lists the five steps. The same five lines appear on the
host console when a real session ends — the proof that both exits share one routine.

> **Not yet verified at runtime.** The console only *simulates* the timeout with a Timer; the real `Application.SessionTimeout` → `ApplicationExit` path (the `[OrderDesk] session …` console lines and the removal of `App_Data/tmp/<session>`) has not been observed. To see it, set `sessionTimeout` low in `Default.json` and let a tab idle. A process that is killed never runs the exit handler and leaves the session folder behind.
