# TicketOps · Production Architecture Deep Dive · Module 7

Local lab build for **Module 7 · Background Tasks, Real-Time Updates & Synchronization**. It follows the
walkthrough video *Run a background CSV import*: a 600-row CSV of tickets (with four deliberately bad
rows) is imported on a background task started with `Application.StartTask`, progress and a log panel
are pushed to the browser with `Application.Update(this, () => …)` at a bounded rate, **⏹ Cancel** stops
the run cooperatively between two rows, a simulated data-store outage stops it cleanly and lets you
**resume**, and every field the two threads share is guarded by one lock. The screen never freezes —
**↻ Refresh list** works while the import runs.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 7\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5107
```

Then open <http://localhost:5107>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with no warnings for both targets (`net10.0-windows`, `net10.0`).

To see the **polling fallback**, set `"enableWebSocket": false` in `Default.json` and restart: the pushes
arrive about a second late, carried by `Application.StartPolling(1000)` / `EndPolling()`.

## What to click in the Ticket Import window

The left card is the import screen (the video calls it *Work Order Import*): file line, **▶ Start import /
⏹ Cancel / ↻ Refresh list**, the progress bar with percent and row counts, the **import log** (per-row
errors are listed here without stopping the run) and a **SERVER STATE** line printing the fields both
threads share. The right card is the **Activity trace · UI → Service → Data**; the `[SVC]`/`[DATA]`
lines written by the worker thread are queued and flushed with each push, so the trace itself is never
touched off-context.

| Button | Path | What you should see |
|---|---|---|
| **▶ Start import** | success + progress | `[UI] buttonStartImport_Click → Application.StartTask(IImportService.ImportAsync sample-tickets.csv from row 1) — the round-trip closes now…`; then, in batches of 25 with each push, `[DATA] InMemoryTicketRepository.InsertAsync — #7001 written (7 rows)` …, `[SVC] ImportService.ImportAsync — row 60/600 — 10% …`, `[UI] ImportPage.OnImportProgress — push #n — row 75/600 (12%) → Application.Update(this, …) from thread N`. The bar and `n of 600 rows · imported · skipped` advance; the import log gets `Row 60 imported — 10%` lines and four `⚠ Row … skipped — …` lines (58 title, 214 due date, 287 assignee, 412 duplicate id). Ends with `✔ Import complete — 596 imported · 4 skipped.`, status **● 596 imported · 4 skipped**, `602 tickets in the repository`. |
| **↻ Refresh list** (click *during* the run) | UI stays responsive + thread-safe repository | `[UI] buttonRefreshList_Click → ITicketService.CountAsync() on request thread N — while the import task is writing (the repository lock keeps both safe)`, `[SVC]` + `[DATA]` read lines, the count grows, import log `✔ List refreshed — 237 tickets (request thread; the import kept running)`; the bar never stops |
| **⏹ Cancel** (during the run) | cancellation | `[UI] buttonCancelImport_Click — _importCancel.Cancel() → the task sees the token before its next row…`, `⚠ [SVC] ImportService.ImportAsync — cancelled before row 349 — 346 imported, 2 skipped, resume at 349`; amber banner **Import cancelled. The rows already imported were kept; resume to continue.**; Start now reads **▶ Resume import (row 349)**; click it to finish the file |
| **Import wrong-header file** | failure 1 (validation) | `⚠ [SVC] ImportService.OpenAsync — rejected: header "Id,Summary,Prio,…" lacks TicketId, Title, …`; orange banner **The file is missing required columns: …**; status **● rejected**; no task starts, nothing written |
| **Start import twice** | failure 2 (synchronization guard) | one `StartTask` line, then `⚠ [UI] ImportPage.TryBeginRun — buttonStartImport_Click: refused — _isRunning is true (checked under the lock)`; banner **An import is already running…** |
| **Simulate data outage** (click *during* the run) | error path | `✖ [DATA] InMemoryTicketRepository — outage: INSERT INTO Tickets (Id=7233) failed — timeout connecting to sql01:1433 …`, `✖ [SVC] ImportService.ImportAsync — stopped at row 233: repository unavailable … [DataOutageException]`; the user sees only the red banner **The import stopped because the data store is unavailable. Recover it, then resume.**, import log `✖ Import stopped at row 233 …`, Start reads **▶ Resume import (row 233)** |
| **Recover the data store** (same button), then **▶ Resume import (row 233)** | recovery | `[UI] buttonOutage_Click — outage OFF → recovered; click ▶ Resume import…`; the run continues from the row that failed and completes; nothing written twice |
| **Reset sample data** | — | re-seeds the six demo tickets, resume row back to 1, import log cleared (refused with the same guard while a run is active) |
| **Clear trace** | — | empties the right-hand card |

Every push updates the **SERVER STATE** line: `isRunning`, rows, counts, `resumeAt`, `outage`, `pushes`
(≈ 40 for 600 rows: every 25 rows + 10 % milestones + skipped rows + final), `IsWebSocket`, `polling`,
and the thread id the line was written on (a push thread differs from a request thread).

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | **ImportService** — parsing, rules, per-row writes, no UI | `Services/IImportService.cs`, `Services/ImportService.cs` (`OpenAsync` validates the file; `ImportAsync` runs rows, checks the token before each, turns rule violations into skipped rows and a store failure into a clean stop), `Domain/TicketImportRules.cs`, `Domain/ImportModels.cs` (`ImportFile`, `ImportProgress`, `ImportResult`, `ImportRowError`, `ImportOutcome`) |
| 2 | **Background task launcher** | `Views/ImportPage.cs` — `buttonStartImport_Click` (lock-guarded `TryBeginRun`, new `CancellationTokenSource`, UI state for this response, `BeginPush`, `Application.StartTask(() => RunImportAsync(…))`) and `RunImportAsync` (the task body with try/catch/finally) |
| 3 | **Progress UI** | `Views/ImportPage.cs` — `OnImportProgress` (worker → `ShouldPush` → `Application.Update(this, () => ApplyProgress…)`), `ApplyProgress`, `PushFinalState`/`ShowRunFinished`; `progressImport`, `labelPercent`, `labelRowsDone`, `listImportLog`, `labelState` in `ImportPage.Designer.cs` |
| 4 | **Cancellation handling** | `buttonCancelImport_Click` + `ImportPage_Disposed` (cancel the token), `ImportService.ImportAsync` (checks `token.IsCancellationRequested` before every row, returns `ImportOutcome.Cancelled` with `NextRow`), `ShowRunFinished` (amber state, **▶ Resume import (row n)**) |
| 5 | **Thread-safety notes** | [`docs/BackgroundTaskNotes.md`](TicketOps/docs/BackgroundTaskNotes.md) — threading model, update rate, the table of every shared field and its protection, per-session vs shared |
| — | Per-row errors without stopping the import | `ImportService.ImportAsync` (rule violation → `RowSkipped` + `ImportRowError`, duplicate id → `InsertAsync` false), `Data/sample-tickets.csv` rows 58, 214, 287, 412 |
| — | Error path + recovery (outage mid-import → clean stop → resume) | `InMemoryTicketRepository.SimulateOutage` (volatile), `ImportService.ImportAsync` catch → `ImportOutcome.Faulted`, `_resumeAtRow` in `RunImportAsync` — see [`docs/ImportRunbook.md`](TicketOps/docs/ImportRunbook.md) |
| — | Polling fallback (`IsWebSocket` false during Load) | `ImportPage.BeginPush` / `EndPush` |
| — | Trace panel safe for a worker thread | `Diagnostics/ActivityTracePanel.cs` — deferred mode (`BeginDeferred` / `FlushPending` / `EndDeferred`), cap 800 lines |
| — | Production-readiness note (lab step 9) | `docs/BackgroundTaskNotes.md` §6 "Three outcomes, one consistent state" + the runbook |

## Where things live

```
TicketOps/
├─ Views/
│  ├─ ImportPage.cs               the screen: launcher, progress marshaling, cancellation, bottom-bar paths, BeginPush/EndPush
│  └─ ImportPage.Designer.cs      GENERATED-style layout (opens in the Wisej Designer) — no logic here
├─ Controls/StatusBanner          reusable "● state" + banner UserControl (display only)
├─ Services/
│  ├─ IImportService.cs           OpenAsync (validate the file) · ImportAsync (rows, progress callback, token)
│  ├─ ImportService.cs            the import logic: per-row rules → skipped, store failure → clean stop, no Wisej types
│  ├─ ITicketService.cs           the read side (CountAsync / GetOpenTicketsAsync) used from the request thread
│  └─ TicketService.cs
├─ Domain/
│  ├─ Ticket.cs                   record (+ Assignee, DueDate); compiles without Wisej.NET
│  ├─ TicketImportRules.cs        header check + TryParseRow: the rules one CSV row must satisfy (pure functions)
│  ├─ ImportModels.cs             ImportFile, ImportProgress (immutable), ImportResult, ImportRowError, ImportOutcome
│  └─ OperationResult.cs          success / safe explanation handed back to the screen
├─ Data/
│  ├─ ITicketRepository.cs        persistence contract (InsertAsync is an atomic check-and-add)
│  ├─ InMemoryTicketRepository.cs lock-guarded fake store; volatile SimulateOutage throws like a real driver would
│  ├─ sample-tickets.csv          600 rows, ids 7001–7600, bad rows 58 / 214 / 287 / 412 (copied to the output folder)
│  └─ sample-tickets-wrong-header.csv   the validation-path file
├─ Infrastructure/
│  ├─ ILog.cs / ActivityLog.cs    cross-cutting logging (thread-safe; details stay here)
│  ├─ ImportFileLocator.cs        project-relative path → file (Application.MapPath, then the output folder)
│  └─ AppComposition.cs           who gets what: one object graph per session, what is per-session vs shared
├─ Resources/Strings.cs           safe user-facing messages (ImportCancelled, ImportInterrupted, …)
├─ Diagnostics/ActivityTracePanel the live trace card — deferred mode while a task runs
├─ docs/
│  ├─ BackgroundTaskNotes.md      threading model · update rate · what is thread-safe (deliverable 5)
│  └─ ImportRunbook.md            how to run, cancel, recover, resume; every path with its evidence
├─ Program.cs                     Wisej.NET session entry point → AppComposition
└─ Startup.cs                     Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **What UI controls are touched from the background task, and is each touch marshaled onto the session context?**
  `progressImport`, `labelPercent`, `labelRowsDone`, `listImportLog`, `labelState`, `statusBanner`,
  `labelTicketCount`, the three buttons and the trace panel's list. All of them change only inside
  `Application.Update(this, () => …)` — in `OnImportProgress` and `PushFinalState` — after an `IsDisposed`
  check. `ImportService` touches none: it reports through a callback and the callback marshals.
- **How often — and after what kind of change — do you call `Application.Update`?**
  Every 25 imported rows, every 10 % milestone, every skipped row, the last row, and always once from the
  task's `finally` with the final state — about 40 flushes for 600 rows instead of 600. The rule is in
  `ShouldPush`; `pushes=` on the SERVER STATE line counts them.
- **Does a `CancellationToken` reach every long loop?**
  Yes: the one loop is `ImportService.ImportAsync`, which checks `token.IsCancellationRequested` before
  every row and returns `Cancelled` with the resume row. The token comes from a fresh
  `CancellationTokenSource` created under the lock in `TryBeginRun`; **⏹ Cancel** and the page's
  `Disposed` event signal it.
- **What happens if the user closes the session during the import?**
  `ImportPage_Disposed` cancels the token, so the task stops at its next row (≤ 10 ms). Every callback
  checks `this.IsDisposed` before touching a control and `PushFinalState` pushes nothing; the
  `ObjectDisposedException` a late push could raise is caught. The rows already written are in that
  session's repository, which goes away with the session.
- **Designer & code review checklist** — the screen opens in the Designer (parameterless constructor);
  controls are named after what they show; the import logic is in `ImportService`, not in a handler;
  every background UI update runs on the session context and is followed by a flush; the task accepts a
  token and restores the UI on cancel and failure (`finally`); failure paths are visible, logged, and
  the user never sees `ex.Message`.
