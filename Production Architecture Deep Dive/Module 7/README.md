# TicketOps · Production Architecture Deep Dive · Module 7

Local lab build for **Module 7 · Background Tasks, Real-Time Updates & Synchronization**. It follows the
walkthrough video *Run a background CSV import*: a 600-row CSV of tickets (with four deliberately bad
rows) is imported on a background task started with `Application.StartTask`, progress and a log panel
are pushed to the browser with `Application.Update(this, () => …)` at a bounded rate, **⏹ Cancel** stops
the run cooperatively between two rows and lets you **resume**, and every field the two threads share is
guarded by one lock. The screen never freezes — **↻ Refresh list** works while the import runs.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 7\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5107
```

Then open <http://localhost:5107>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

To see the **polling fallback**, set `"enableWebSocket": false` in `Default.json` and restart: the pushes
arrive about a second late, carried by `Application.StartPolling(1000)` / `EndPolling()`.

## What to try in the Ticket Import window

| Action | Path | What you should see |
|---|---|---|
| **▶ Start import** | success + progress | the bar and `n of 600 rows · imported · skipped` advance; the import log gets `Row 60 imported — 10%` lines and four `⚠ Row … skipped — …` lines (58 title, 214 due date, 287 assignee, 412 duplicate id). Ends with `✔ Import complete — 596 imported · 4 skipped.` and `602 tickets in the repository` |
| **↻ Refresh list** (during the run) | UI stays responsive | the count grows, the import log gets `✔ List refreshed — 237 tickets`; the bar never stops |
| **⏹ Cancel** (during the run) | cancellation | amber banner **Import cancelled. The rows already imported were kept; resume to continue.**; Start now reads **▶ Resume import (row 349)**; click it to finish the file |

If the data store fails mid-import, the run stops cleanly before the failing row, the details go to the
log, the user sees **The import stopped because the data store is unavailable. Recover it, then resume.**
and Start offers **▶ Resume import (row n)**. A second start while a run is active is refused under the lock.

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | **ImportService** — parsing, rules, per-row writes, no UI | `Services/IImportService.cs`, `Services/ImportService.cs` (`OpenAsync` validates the file; `ImportAsync` runs rows, checks the token before each, turns rule violations into skipped rows and a store failure into a clean stop), `Domain/TicketImportRules.cs`, `Domain/ImportModels.cs` |
| 2 | **Background task launcher** | `Views/ImportPage.cs` — `buttonStartImport_Click` (lock-guarded `TryBeginRun`, new `CancellationTokenSource`, UI state for this response, `BeginPush`, `Application.StartTask(() => RunImportAsync(…))`) and `RunImportAsync` (the task body with try/catch/finally) |
| 3 | **Progress UI** | `Views/ImportPage.cs` — `OnImportProgress` (worker → `ShouldPush` → `Application.Update(this, () => ApplyProgress…)`), `ApplyProgress`, `PushFinalState`/`ShowRunFinished`; `progressImport`, `labelPercent`, `labelRowsDone`, `listImportLog` |
| 4 | **Cancellation handling** | `buttonCancelImport_Click` + `ImportPage_Disposed` (cancel the token), `ImportService.ImportAsync` (checks the token before every row, returns `ImportOutcome.Cancelled` with `NextRow`), `ShowRunFinished` (amber state, **▶ Resume import (row n)**) |
| 5 | **Thread-safety notes** | [`docs/BackgroundTaskNotes.md`](TicketOps/docs/BackgroundTaskNotes.md) — threading model, update rate, the table of every shared field and its protection, per-session vs shared |
| — | Per-row errors without stopping the import | `ImportService.ImportAsync` (rule violation → `RowSkipped` + `ImportRowError`, duplicate id → `InsertAsync` false), `Data/sample-tickets.csv` rows 58, 214, 287, 412 |
| — | Operator runbook and production-readiness note | [`docs/ImportRunbook.md`](TicketOps/docs/ImportRunbook.md), `docs/BackgroundTaskNotes.md` §6 "Three outcomes, one consistent state" |

## Where things live

```
TicketOps/
├─ Views/ImportPage               the screen: launcher, progress marshaling, cancellation, BeginPush/EndPush
├─ Controls/StatusBanner          reusable "● state" + banner UserControl (display only)
├─ Services/                      IImportService / ImportService (the import logic, no Wisej types), ITicketService / TicketService
├─ Domain/                        Ticket, TicketImportRules (pure functions), ImportModels, OperationResult
├─ Data/
│  ├─ ITicketRepository.cs        persistence contract (InsertAsync is an atomic check-and-add)
│  ├─ InMemoryTicketRepository.cs lock-guarded fake store
│  └─ sample-tickets.csv          600 rows, ids 7001–7600, bad rows 58 / 214 / 287 / 412 (copied to the output folder)
├─ Infrastructure/                ILog / ActivityLog (server console), ImportFileLocator, AppComposition
├─ Resources/Strings.cs           safe user-facing messages (ImportCancelled, ImportInterrupted, …)
├─ docs/                          BackgroundTaskNotes.md · ImportRunbook.md
├─ Program.cs                     Wisej.NET session entry point → AppComposition
└─ Startup.cs                     Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **What UI controls are touched from the background task, and is each touch marshaled onto the session context?**
  `progressImport`, `labelPercent`, `labelRowsDone`, `listImportLog`, `statusBanner`, `labelTicketCount` and
  the two buttons. All of them change only inside `Application.Update(this, () => …)` — in
  `OnImportProgress` and `PushFinalState` — after an `IsDisposed` check. `ImportService` touches none: it
  reports through a callback and the callback marshals.
- **How often — and after what kind of change — do you call `Application.Update`?**
  Every 25 imported rows, every 10 % milestone, every skipped row, the last row, and always once from the
  task's `finally` with the final state — about 40 flushes for 600 rows instead of 600. The rule is in
  `ShouldPush`.
- **Does a `CancellationToken` reach every long loop?**
  Yes: the one loop is `ImportService.ImportAsync`, which checks `token.IsCancellationRequested` before
  every row and returns `Cancelled` with the resume row. The token comes from a fresh
  `CancellationTokenSource` created under the lock in `TryBeginRun`; **⏹ Cancel** and the page's
  `Disposed` event signal it.
- **What happens if the user closes the session during the import?**
  `ImportPage_Disposed` cancels the token, so the task stops at its next row (≤ 10 ms). Every callback
  checks `this.IsDisposed` before touching a control and `PushFinalState` pushes nothing; the
  `ObjectDisposedException` a late push could raise is caught.
