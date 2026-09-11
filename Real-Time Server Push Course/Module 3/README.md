# TicketOpsLive · Real-Time Apps with Server Push · Module 3

Local lab build for **Module 3 · Progress Without Refresh: Background Import Monitor**: the Import Monitor panel of
TicketOps Live. An import of 200 simulated records runs on a task started from the click (`Application.StartTask`),
pushes progress to the browser every 10 records with `Application.Update(this)`, supports cooperative cancellation
(`CancellationTokenSource`), can fail on purpose at record 87, and always restores the UI in `finally`. Every log
line carries a JobId (the lab's extension challenge).

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 3/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5303
```

Then open <http://localhost:5303>. (Visual Studio: open `TicketOpsLive.slnx`, press F5.)

## What to try

| Button | What you should see |
|---|---|
| Start Import | the bar, `recordsImportedLabel` (`n/200`) and the elapsed time move without a refresh; the log gets "Import started", "Imported 50/100/150/200 records", "Import completed" and a summary line (`Job J-xxxxxx: 200 records in 5.3 s — completed`) |
| Cancel Import (while running) | the loop stops between two records: "Import cancelled by user.", "Cancelled at … after record n"; Start is enabled again |
| Fail at 87 | the import runs to record 86, then "Import failed. Review the server log." and "Failed at … on record 87"; the full exception is on the server console under the JobId; Start is enabled again |

**Polling fallback.** Add `"enableWebSocket": false` to `Default.json` and restart: progress still arrives on the
polls (about a second late); polling is requested when the import starts and ended when it finishes.

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| `startImportButton`, `cancelImportButton`, `importProgressBar`, `importStatusLabel`, `importLogListBox`, `recordsImportedLabel` | [`MainPage.Designer.cs`](TicketOpsLive/MainPage.Designer.cs) |
| Import loop with 200 simulated records | [`MainPage.cs`](TicketOpsLive/MainPage.cs) `RunImport(job, token, failAt87)` |
| Push progress every 10 records | `if (i % PushEvery == 0) Application.Update(this)` |
| Log start, every 50 records, cancellation, failure, completion | `Log(job, …)` in `StartImport` and in the `try` / `catch` / `finally` blocks of `RunImport` |
| Cancellation with `CancellationTokenSource` | `_cts` instance field, `cancelImportButton_Click` → `RequestCancel()`; `Disposed` cancels too |
| Deliberate failure at record 87 | `failAt87Button_Click` → `StartImport(failAt87: true)` → `throw` at `i == 87` |
| UI re-enabled in all outcomes | the `finally` block, pushed with `Application.Update(this, () => …)` |
| Elapsed time + final summary line; extension: JobId | `elapsedLabel`; [`Services/ImportJob.cs`](TicketOpsLive/Services/ImportJob.cs) (`JobId`, `Summary`) |

## Self-check

- **Progress appears while running** — 200 records → 20 progress pushes + the final one; the handler returns immediately after `StartTask`.
- **Cancellation leaves the UI usable** — the token is checked between records; `finally` re-enables Start.
- **No raw stack traces** — the exception goes to the server console with the JobId; the UI shows a safe message.
- **The final update always reaches the browser** — `finally` pushes with `Application.Update(this, …)` after an `IsDisposed` check.
- **No more pushes than necessary** — controls change on every record, but the browser is updated every 10th.
