# Error handling notes — safe messages for users, useful detail for developers

Lesson s27 in one sentence: the user needs a clear message about what happened and what to do next; the
developer needs enough detail in the log to troubleshoot; the two must never be the same string. This note
shows how `JobsWindow.cs` applies that.

## 1. "Show to user?" — applied to this project's failure

When **Simulate error at step 4** is on, `RunJobAsync` throws
`InvalidOperationException("Row 1,204: column 'Email' is not a valid address (TicketImport.csv)")`.
That message is *developer* detail: a row number, a column name, a file name. Here is where each piece goes.

| Error detail | Show to user? | Why | Where it goes here |
|---|---|---|---|
| "Import failed. Please try again or contact support." | **Yes** | Useful and safe: says what happened and what to do next | `lblStatus.Text` in the `catch (Exception ex)` block |
| `InvalidOperationException: Row 1,204: column 'Email' is not a valid address (TicketImport.csv)` | **No** | Implementation detail — the row, the column, the file name | `LogError(ex)` → `ERROR InvalidOperationException: Row 1,204 …` in `lstLog` |
| `NullReferenceException at TicketService.cs line 52` (a stack frame) | **No** | Internal detail; a real app logs `ex.ToString()` to a file / ILogger | not shown; `LogError` logs type + message + job context |
| Database password or connection string | **Never** | Sensitive | never logged either — nothing in this project puts secrets in a message |
| Job name, step, session id, timestamp | **Sometimes** | Helps support find the log line without exposing secrets | `LogError` logs `job = Import, step = 4/6, session = ab12cd34`; the status says only "Import failed …" |

## 2. The logging checklist — as applied

| Checklist item | Where in the code |
|---|---|
| Log when the job **starts** | `RunJobAsync`: `Import: started — 6 steps, TicketImport.csv, simulateError = …` |
| Log **each major step** | `RunJobAsync` loop: `Import: step 3/6 — Validating rows…` |
| Log **completion** | `Import: completed in 4.2 s (6/6 steps)` |
| Log **cancellation** | `catch (OperationCanceledException)`: `Import: cancelled by the user at step 3/6` (plus the `btnCancel_Click → _currentJob.Cancel() requested …` line) |
| Log **failure** | `LogError(ex)`: `ERROR InvalidOperationException: …` + the context line |
| **Timestamps and job names** on every line | `AddLog` prefixes `HH:mm:ss`; every job message starts with the job name |
| User-facing messages **short**; technical detail **in the log** | `lblStatus` = one sentence; `lstLog` = everything else |
| **No passwords / keys / private data** in the log | nothing sensitive exists in this sample; the rule is written next to `LogError` for the day it does |
| The UI state after the failure is logged too | `SetJobRunning(false) → buttons reset (outcome: Failed) — the UI is back in a safe state` |

## 3. The handler, annotated

```csharp
private async void btnStartImport_Click(object sender, EventArgs e)
{
    try
    {
        SetJobRunning(true);                        // guard: Start off, Cancel on, error switch frozen
        await RunImportJobAsync();                  // the long work; progress is pushed from inside
        lblStatus.Text = "Import completed successfully.";
    }
    catch (OperationCanceledException)              // Cancel Job → token → Task.Delay throws this
    {
        _outcome = JobOutcome.Cancelled;
        lblStatus.Text = "Import cancelled.";       // not a failure: amber, not red
        AddLog($"Import: cancelled by the user at step {_currentStep}/{_totalSteps}");
    }
    catch (Exception ex)                            // anything else: the simulated row error, a real bug …
    {
        LogError(ex);                               // developer detail → log (type, message, job, step, session)
        lblStatus.Text = "Import failed. Please try again or contact support.";   // the user's message
    }
    finally
    {
        SetJobRunning(false);                       // ALWAYS runs: after success, cancel and failure
    }
}
```

- The **specific** catch comes first. `OperationCanceledException` is expected behaviour, not an error, so it
  gets its own short message and never reaches `LogError`.
- `LogError` is the only place that knows how to describe an exception. The handler does not format
  anything; that keeps it readable and keeps the safe message next to the log call.
- `finally` is why the page never gets stuck in "running" mode: whichever branch ran, the buttons come back.
  Try it — Simulate error, watch the failure, then click Start Import again without touching anything else.

## 4. What to check when troubleshooting (the lesson's five things, in this UI)

| Check | Where to look |
|---|---|
| **Button state** | Start Import / Start Export greyed = a job is running (or `finally` never ran — which this handler makes impossible); the `lblStatusHint` line under the status says `state: RUNNING / COMPLETED / CANCELLED / FAILED` |
| **Current job state** | `lblJobInfo`: job name, `Token: live / cancel requested / none` |
| **Progress value** | the bar and `Step: 4/6` in `lblJobInfo`; the `progress = 50%` fragment in the ERROR context line |
| **Exception message** | the `ERROR <type>: <message>` line in the job log |
| **Most recent log entry** | the last (auto-selected) line in `lstLog` — after a failure it is the `SetJobRunning(false) … (outcome: Failed)` line, which proves the reset ran |

## Evidence

| Action | Job log | Screen |
|---|---|---|
| Tick **Simulate error at step 4**, click **Start Import** | steps 1–4 logged, then `ERROR InvalidOperationException: Row 1,204: column 'Email' is not a valid address (TicketImport.csv)`, `job = Import, step = 4/6, session = …, simulateError = True, progress = 50%`, `user sees only: "Import failed. Please try again or contact support."`, `SetJobRunning(false) → buttons reset (outcome: Failed)` | bar stops at 50 %; `lblStatus` red **"Import failed. Please try again or contact support."** — no row number, no file name; Start buttons enabled again |
| Same with **Start Export** | the same shape with `Export:` and `(TicketExport.xlsx)` in the developer line | red "Export failed. Please try again or contact support." |
| **Cancel Job** mid-run | `… cancelled by the user at step n/6` — no `ERROR` line, because cancelling is not an error | amber "Import cancelled." |
| Untick the switch, **Start Import** again (recovery) | a fresh `Import: started …` through `Import: completed …` | green "Import completed successfully." — the failure left nothing behind |
