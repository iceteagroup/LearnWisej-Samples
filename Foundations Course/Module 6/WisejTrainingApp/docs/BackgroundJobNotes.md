# Background job notes — session state, responsive jobs, progress on purpose

The Module 6 lab is a job runner: two long jobs (Import, Export) that run with `async`/`await`, show
progress in a bar, a status label and a log, can be cancelled, and never leave the page stuck. This
note maps lesson s26 onto what `JobsWindow.cs` actually does.

## 1. The four kinds of state — in this project

| State type | What it means | In this project |
|---|---|---|
| **UI state** | Used only to draw the current screen | `progressBar.Value`, `lblStatus.Text`, `lblJobInfo.Text`, which buttons are enabled (`SetJobRunning`) |
| **Session state** | Belongs to one connected user session | `_currentJob` (the `CancellationTokenSource`), `_jobsRunInThisSession`, `_currentStep`, `Application.Session.LastJob` — all per user because `JobsWindow` is created once per session by `Program.Main` |
| **Business state** | Real data, saved or shared on purpose | The tickets a real import would write through `TicketService` (this module simulates the steps; Modules 4/5 own the data) |
| **Static / shared state** | Shared across users unless carefully designed | `_jobsRunOnThisServer` — a `static int`, kept deliberately so the trap can be watched |

**Why the window's instance fields are per user.** Wisej.NET keeps one server-side session per connected
browser tab; `Default.json` names `Program.Main` as the startup and it does `new JobsWindow().Show()`.
Every user therefore gets their own `JobsWindow` object, so an instance field is automatically "this
user's". A `static` field is one value for the whole process — every session reads and writes the same
number.

**The trap, made visible.** The "Session vs shared state" card shows both counters side by side. Open a
second browser tab: its *Jobs run in this session* is 0 and its *Last job (session bag)* is "(none yet)",
but *Jobs run on this server (static)* already carries the first tab's count and keeps climbing for both.
Now imagine that static field held the current job's token instead of a number — tab 2's Cancel would
kill tab 1's import. That is why `_currentJob` is an instance field and the comment above it says so.

## 2. The workflow (lesson diagram → code)

```
User opens page        → Program.Main → new JobsWindow()   (a new session sees its own fields = 0)
Button click           → btnStartImport_Click               (SetJobRunning(true): Start off, Cancel on)
Background job         → await RunImportJobAsync()          (RunJobAsync loops the steps, awaits each)
Progress update        → ShowProgress + AddLog + Application.Update(this)   (bar, label, log — pushed)
Complete / error       → lblStatus = safe message; finally SetJobRunning(false)   (buttons back)
```

`RunImportJobAsync()` and `RunExportJobAsync()` are one-liners that hand a `JobDefinition` from
`Services/JobCatalog` to the single `RunJobAsync(string jobName, string[] steps, string inputFile)`.
Adding a job means adding a catalog entry, not another loop.

## 3. Bad pattern vs better pattern — where each one lives

| Bad pattern | Better pattern | In this project |
|---|---|---|
| All the work inside the click event; the page freezes | Start the work asynchronously; update progress on purpose | `await RunImportJobAsync()`; each step is `await Task.Delay(700, token)` (the stand-in for real work) so the server thread is released between steps |
| Users click Start many times → duplicate jobs | Disable Start while a job runs; enable Cancel | `SetJobRunning(true)` at the top of the `try`; `RunJobAsync` also throws if `_currentJob != null` (belt and braces) |
| Only a generic error, or a crash | Log details for developers, show a short friendly message | `LogError(ex)` writes `ERROR InvalidOperationException: Row 1,204 …` + job/step/session to the log; `lblStatus` says "Import failed. Please try again or contact support." |
| UI left in "running" mode after a failure | Reset in `finally` | `finally { SetJobRunning(false); }` — runs after success, cancel and failure |
| Per-user job in a `static` field | Instance field / `Application.Session` | `_currentJob`, `_jobsRunInThisSession`, `Application.Session.LastJob`; the static `_jobsRunOnThisServer` is the counter-example |

## 4. How progress reaches the browser while the handler is still awaiting

A Wisej.NET event handler normally sends its changed properties to the browser **when it returns**. An
`async void` handler returns at its first `await`, and the rest of the method runs later as a continuation
outside the original request. Nothing in that continuation reaches the browser on its own — so after every
change that the user should see, the job calls:

```csharp
ShowProgress(jobName, stepNumber, steps.Length, steps[i], elapsed.Elapsed);   // lblStatus, lblJobInfo
AddLog($"{jobName}: step {stepNumber}/{steps.Length} — {steps[i]}…");         // lstLog
Application.Update(this);                                                     // push now
await Task.Delay(700, token);                                                 // the "work"
progressBar.Value = stepNumber * 100 / steps.Length;
Application.Update(this);                                                     // push again
```

`Application.Update(this)` sends the pending changes of the Form (and everything on it) over the
WebSocket. Two pushes per step (start and finish) is a deliberate, bounded frequency — pushing in a tight
loop would flood the client. `SetJobRunning` also ends with `Application.Update(this)` because when it is
called from `finally` it, too, is running in a continuation.

Cancellation rides on the same `await`: `btnCancel_Click` calls `_currentJob.Cancel()`, the pending
`Task.Delay(700, token)` throws `OperationCanceledException`, and the Start handler's first `catch`
turns that into "Import cancelled." plus a log line with the step it stopped at.

## Evidence

| Action | Job log (right card) | Screen |
|---|---|---|
| **Start Import** | `Import: started — 6 steps, TicketImport.csv, simulateError = False`, then `Import: step 1/6 — Opening TicketImport.csv…` … `Import: step 6/6 — Refreshing the ticket index…`, `Import: completed in 4.2 s (6/6 steps)`, `SetJobRunning(false) → buttons reset (outcome: Completed)` | bar climbs 17 → 33 → 50 → 67 → 83 → 100 %; `lblJobInfo` shows the step and elapsed time; Start Import / Start Export are greyed while it runs; Cancel Job is enabled; ends green "Import completed successfully." |
| **Start Export** | same shape with the `Export:` prefix and 5 steps (20 % per step) | ends green "Export completed successfully." |
| **Cancel Job** during a run | `Import: btnCancel_Click → _currentJob.Cancel() requested at step 3/6`, `Import: cancelled by the user at step 3/6`, `SetJobRunning(false) → buttons reset (outcome: Cancelled)` | amber "Import cancelled."; bar stops where it was; Start is back |
| Click **Start** twice | impossible — the button is disabled from `SetJobRunning(true)` until `finally` | the tooltip on the disabled button still shows the handler |
| **Refresh counters** in a second tab | `btnRefreshState_Click → session ab12cd34: instance counter = 0, static counter = 3` | the green per-session lines read 0 / "(none yet)"; the red static line carries the other tab's count |
