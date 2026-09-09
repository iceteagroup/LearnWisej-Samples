# WisejTrainingApp · Wisej.NET Foundations · Module 6

Local lab build for **Module 6 · Session state, background work and safe error handling**. It is the lab's
job runner: **Start Import** / **Start Export** / **Cancel Job** / **Clear Log**, a **Simulate error at step 4**
switch, a `ProgressBar`, a status label that only ever shows a *safe* message, a job-info panel (job, step
x/y, elapsed) and a timestamped job log that carries the *developer* detail. Long work runs with
`async`/`await`, each step pushes progress with `Application.Update(this)`, and the lesson's
`try / catch / finally` handler resets the buttons whatever happens.

A second card, "Session vs shared state", puts an instance counter, the `Application.Session` bag and a
deliberately `static` counter side by side, so the static-field trap from lesson s26 is something you can
watch in a second browser tab, not just read about.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 6/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5086
```

Then open <http://localhost:5086>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## What to try

| Action | Path | What you should see |
|---|---|---|
| **Start Import** | success (progress) | Start Import / Start Export grey out, Cancel Job lights up; the bar climbs 17 → 33 → … → 100 % over ~4 s while the log fills with `Import: step n/6 — …`; `lblJobInfo` shows job, step and elapsed; ends green **"Import completed successfully."** and the buttons are back |
| **Start Export** | success (progress) | same with 5 steps and the `Export:` prefix; ends green "Export completed successfully." |
| **Cancel Job** while a job runs | cancellation | the log shows `_currentJob.Cancel() requested at step n/6` then `Import: cancelled by the user at step n/6`; status amber **"Import cancelled."**; the bar stops where it was; no `ERROR` line — cancelling is not an error |
| Tick **Simulate error at step 4**, then **Start Import** | failure (safe message vs logged detail) | steps 1–4 run, then the log shows `ERROR InvalidOperationException: Row 1,204: column 'Email' is not a valid address (TicketImport.csv)` + a context line (job, step, session, progress); the user's status says only red **"Import failed. Please try again or contact support."**; the buttons come back through `finally` |
| Click **Start** twice | duplicate job | impossible — Start is disabled from `SetJobRunning(true)` until `finally` runs `SetJobRunning(false)` |
| Untick the switch, **Start Import** again | recovery | a clean run to 100 % — the failure left nothing behind |
| Open <http://localhost:5086> in a **second tab**, click **Refresh counters** | the static-field trap | that tab's "Jobs run in this session" is 0 and its session bag says "(none yet)", but "Jobs run on this server (static)" already carries the first tab's count; run a job in either tab and only the static number moves in both |
| **Clear Log** | – | empties the job log |

The right-hand card is the job log: every start, step, cancellation, failure and reset, with a timestamp and
the job name on each line — the developer's view of what the safe status message hides.

## Where things live

```
WisejTrainingApp/
├─ Program.cs                    Wisej.NET session entry point: new JobsWindow().Show()  (one per user session)
├─ JobsWindow.cs                 code-behind: btnStartImport_Click / btnStartExport_Click (try/catch/finally),
│                                btnCancel_Click, RunJobAsync, SetJobRunning, LogError, RefreshStateCard, AddLog
├─ JobsWindow.Designer.cs        Designer-generated layout (InitializeComponent): the three cards and every control
├─ Models/JobDefinition.cs       Name, InputFile, Steps — plain data
├─ Services/JobCatalog.cs        ImportJob() / ExportJob(): the step lists (the reusable "what", not the "how")
├─ Startup.cs                    Kestrel host (app.UseWisej(), static files from the project folder)
├─ Default.json / Default.html / Web.config / Properties/launchSettings.json (port 5086)
└─ docs/
   ├─ BackgroundJobNotes.md      state types in this project, the workflow, bad vs better, how Update(this) pushes progress, evidence
   └─ ErrorHandlingNotes.md      "show to user?" table, the logging checklist as applied, the handler annotated, troubleshooting, evidence
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the project, run it once | `WisejTrainingApp.csproj`, `dotnet run -f net10.0 --urls http://localhost:5086` |
| 2 · Build the job page (Start Import, Cancel Job, ProgressBar, status label, log area) | `JobsWindow.Designer.cs` — the "Job runner" card and the "Job log" card |
| 3 · Name controls clearly | `btnStartImport`, `btnCancel`, `progressBar`, `lblStatus`, `lstLog` (plus `btnStartExport`, `btnClearLog`, `chkSimulateError`, `lblJobInfo`) |
| 4 · Run work in the background with async/await | `btnStartImport_Click` → `await RunImportJobAsync()` → `RunJobAsync` loops `await Task.Delay(700, token)` per step |
| 5 · Guard the buttons with `SetJobRunning(bool)` | `SetJobRunning` in `JobsWindow.cs`: Start off / Cancel on while running; also guards `chkSimulateError` |
| 6 · Show progress (bar, status, log, timestamps, job names) | `RunJobAsync` → `ShowProgress`, `progressBar.Value`, `AddLog($"{jobName}: step …")`, `Application.Update(this)` after each change |
| 7 · Handle errors safely (`try`/`catch`/`finally`, `LogError(ex)`, friendly message) | `btnStartImport_Click` / `btnStartExport_Click`; `LogError` logs type + message + job/step/session; `lblStatus` gets the one safe sentence |
| 8 · Always reset state in `finally` | `finally { SetJobRunning(false); }` in both Start handlers |
| 9 · Keep per-user state out of `static` fields | `_currentJob`, `_jobsRunInThisSession`, `Application.Session.LastJob` are per session; `_jobsRunOnThisServer` is the deliberate `static` counter-example with a warning label |
| 10 · Run & test: job, progress, log, simulated error, safe state | the "What to try" table above; evidence tables in both docs |

Lab code check (`labs.js` m6): the pasted code (the `btnStartImport_Click` handler plus `RunJobAsync`) has a
`_Click` handler, `async`/`await`/`Task`, `SetJobRunning` and `Enabled =`, `catch (` with `finally`, and
`LogError` / `AddLog` / `lblStatus`.

## Self-check

- **Session state vs shared state?** Session state belongs to one connected user (this session's job, its token,
  its step, its progress messages); shared state is one value for every user on the server. In Wisej.NET the
  window's instance fields are session state because `Program.Main` creates a new `JobsWindow` per session; a
  `static` field is shared state.
- **Where is the static-field trap?** `private static int _jobsRunOnThisServer` — every tab increments and reads
  the same number. If it held `_currentJob` instead, one user's Cancel would cancel another user's import.
- **Describe the responsive background-job workflow.** Click → `SetJobRunning(true)` → `await RunImportJobAsync()`
  releases the server thread at each step → each step updates bar/status/log and pushes with
  `Application.Update(this)` → complete / cancel / fail sets one clear message → `finally` resets the buttons.
- **What should the user see when the job fails, and what should the log hold?** The user: "Import failed. Please
  try again or contact support." The log: `ERROR InvalidOperationException: Row 1,204: column 'Email' …` plus job,
  step, session id and timestamp — never a connection string, password or stack trace in the UI.
- **Why does `finally` matter?** It runs after success, cancellation and failure, so `SetJobRunning(false)` always
  brings Start back and the page can never be stuck in "running" mode.
- **What do you check when a job misbehaves?** Button state, current job state (`lblJobInfo`), the progress value,
  the exception message in the log, and the most recent log entry.

## Verified / unverified

Built with `dotnet build -nologo -v q` (both targets, 0 warnings, 0 errors). **Not yet run in the browser** —
the reviewer runs it. The calls the cookbook marks as unverified at runtime, all used here:

- `private async void btnStartImport_Click(…)` with `await Task.Delay(700, token)` inside `RunJobAsync` — an
  `async void` handler whose continuation runs outside the original request.
- `Application.Update(this)` called after each progress change **inside the awaiting job** (and at the end of
  `SetJobRunning`) to push `progressBar.Value`, the labels and the new log items while the handler is still
  awaiting. If the bar only jumps to 100 % at the end, this is the call to look at.
- `Application.Session.LastJob = …` / `Application.Session.LastJob as string` — the dynamic per-session bag
  (compiles, so `Application.Session` is `dynamic`; reading it back is what needs the runtime check).
- `Application.SessionId` (first 8 characters shown in the session card).
- `Wisej.Web.ProgressBar` `Minimum` / `Maximum` / `Value` and `Wisej.Web.CheckBox.CheckedChanged` — plain
  properties/events, listed for completeness.
- Cancellation through `Task.Delay(…, token)` throwing `OperationCanceledException` (`TaskCanceledException`)
  into the handler's first `catch` — standard .NET, but its interaction with the Wisej continuation is what the
  Cancel path exercises.
