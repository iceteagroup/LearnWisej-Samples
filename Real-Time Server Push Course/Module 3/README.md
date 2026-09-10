# TicketOpsLive · Real-Time Apps with Server Push · Module 3

Local lab build for **Module 3 · Progress Without Refresh: Background Import Monitor**. It is the Import Monitor panel
of TicketOps Live from the lab guide: an import of 200 simulated records runs on a task started from the click
(`Application.StartTask`), reports progress to the browser **every 10 records** with `Application.Update(this)`,
supports cooperative cancellation (`CancellationTokenSource`), fails on purpose at record 87 when asked, and always
restores the UI in `finally`. Every log line, trace line and server-console entry carries the **JobId** (the
extension challenge). A second button runs the same import *inside* the click handler to show why that is wrong.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 3/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5303
```

Then open <http://localhost:5303>. (Visual Studio: open `TicketOpsLive.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Button | Path | What you should see |
|---|---|---|
| ▶ Start import | success / progress | `• server Application.StartTask job J-xxxxxx: 200 records × 25 ms, push every 10 records …`, then one `→ push Application.Update(this) job J-xxxxxx · 10/200 · 5% · 0.31 s` line per 10 records; the bar, the record counter and the elapsed time move while the browser never refreshes; after ≈6.2 s `Job J-xxxxxx: 200 records in 6.2 s — completed · 21 pushes (final state, finally block)` |
| ▶ Start import (again, while running) | guard | the button is disabled while a job runs; if a click gets through, the trace says `refused — job J-xxxxxx is still running (_importRunning == true)` |
| ■ Cancel | cancellation | `← request cancelImportButton_Click _cts.Cancel() → job … sees the token at its next record (≤ 25 ms)`, then `• server import job … cancelled after record 71 (OperationCanceledException caught inside the task)`; the log shows "Cancel requested by user" and "Cancelled at … after record 71"; Start is enabled again |
| ☑ Throw at record 87 → ▶ Start import | failure | the import runs normally to record 86, then `• server import job … FAILED on record 87: InvalidOperationException caught inside the task → server log, safe message`; the UI shows *Import failed. Review the server log.* and a red banner naming the JobId; the server console has the full exception under that JobId; Start is enabled again by `finally` |
| ▶ Start import (after a failure) | recovery | a new job with a new JobId runs from the same page |
| Blocking import (anti-pattern) | anti-pattern | 60 records run **inside** the click handler: the trace's two lines are written 1.86 s apart but arrive together, the bar jumps from 0 to 100, and Cancel could not have been clicked — `60 model changes → 0 pushes, ONE response after 1860 ms` |
| Clear log / Clear trace | — | empties the import log / the trace |

**Polling fallback.** Add `"enableWebSocket": false` to `Default.json` and restart: Start import logs
`Application.StartPolling(1000) … fallback polling ON until the task ends`, the progress still arrives (on the polls,
≈1 s late) and the final push logs `Application.EndPolling() … fallback polling OFF`.

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| Controls `startImportButton`, `cancelImportButton`, `importProgressBar`, `importStatusLabel`, `importLogListBox`, `recordsImportedLabel` | [`MainPage.Designer.cs`](TicketOpsLive/MainPage.Designer.cs) |
| Import loop with 200 simulated records | [`MainPage.cs`](TicketOpsLive/MainPage.cs) `RunImport(CancellationToken token, bool failAt87)` — `TotalRecords = 200`, `RecordMilliseconds = 25` |
| Push progress every 10 records, not every record | `RunImport`: controls change on every record, `if (i % PushEvery == 0) Application.Update(this)` |
| Log messages for start, every 50 records, cancellation, failure, completion | `Log(job, …)` calls in `startImportButton_Click` and in the `try` / `catch (OperationCanceledException)` / `catch (Exception)` / `finally` blocks |
| Cancellation with `CancellationTokenSource` | `_cts` instance field, `cancelImportButton_Click` → `RequestCancel("user")`; `this.Disposed` also cancels |
| Deliberate failure at record 87 | `failAt87CheckBox` → `throw new InvalidOperationException("Simulated malformed record #87 …")` |
| UI re-enabled in all outcomes | the `finally` block: `startImportButton.Enabled = true`, `cancelImportButton.Enabled = false`, pushed with `Application.Update(this, () => …)` |
| Extension: a JobId on every import | [`Services/ImportJob.cs`](TicketOpsLive/Services/ImportJob.cs) — `J-xxxxxx`, `Summary`, `Outcome`, elapsed |
| Six-step pattern of the walkthrough | comments `Step 1 … Step 6` in `startImportButton_Click` and `RunImport` |

## Where things live

```
TicketOpsLive/
├─ MainPage.cs               start / cancel / RunImport (steps 1-6) / blocking anti-pattern / BeginPush-EndPush / log + trace helpers
├─ MainPage.Designer.cs      Import Monitor card, trace card, action bar (opens in the Wisej Designer)
├─ Services/ImportJob.cs     JobId, outcome, elapsed, summary line — the correlation key for log and trace
├─ Program.cs                Application.MainPage = new MainPage()
├─ Startup.cs                Kestrel host (app.UseWisej())
└─ docs/
   ├─ BackgroundImportNotes.md   the six-step pattern mapped to the code, the cadence choice, evidence
   └─ CancellationAndFailure.md  cooperative cancellation, error reporting, JobId correlation, evidence
```

## Self-check (acceptance criteria → where it is satisfied)

- **Progress updates appear while the operation is running** — one `→ push` per 10 records; the bar and counter move without a click (verified: 20 pushes for 200 records).
- **Cancellation works and leaves the UI usable** — the token is checked between records, the `catch (OperationCanceledException)` writes the safe status, `finally` re-enables Start (verified: cancelled after record 71, next Start ran).
- **Failure is reported without exposing raw stack traces** — `catch (Exception)` sends the exception to the server console under the JobId; the UI gets *Import failed. Review the server log.* (verified at record 87).
- **The final update always reaches the browser when the session is still alive** — the `finally` block pushes with `Application.Update(this, …)` after an `IsDisposed` check (`ObjectDisposedException` is swallowed if the page went away in between).
- **The code does not push more than necessary** — 200 records → 21 pushes; the blocking anti-pattern shows the other extreme (0 pushes, one frozen response).
- **Lesson checkpoint** — the operation starts immediately (the handler returns after `StartTask`), progress is pushed without blocking the browser, cancellation is cooperative, errors are reported safely, and the UI ends in a consistent state in every outcome.
