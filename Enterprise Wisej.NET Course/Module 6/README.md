# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 6

Local lab build for **Advanced Module 6 · Real-Time Systems, Background Pipelines, Notifications & Imports**.
It follows the walkthrough video: the **Import Center** (`ImportCenterPage`) starts imports it does not run,
a **job queue** executes them on a thread with no session, a **job status store** records every milestone, a
**progress observer** pushes to the browser with `Application.StartTask` + `Application.Update` at a bounded
rate, a **notification panel** delivers the result to whoever is there when it lands, and the
**`JobDetailPanel`** shows the history and the per-row report.

The video's failure path is the one that matters: **close the session and the job keeps running**. Here the
**Reopen** button disposes the page and builds a new one — the closest a button can get to closing the tab —
and the new page finds the same job in the store and re-attaches to it, mid-batch.

Nothing here is deployed anywhere and nothing touches a network: a plain Wisej.NET 4 project on this machine,
with in-memory fakes for the files, the work-order table, the queue and the notification store.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Enterprise Wisej.NET Course\Module 6\EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5206
```

Then open <http://localhost:5206>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with 0 warnings and 0 errors on both target frameworks.

The session is seeded as **`ana.ops` (Manager) of tenant `contoso`** (`Program.Main`).

## What to click

Bottom bar, left to right:

| Button | Path | What you should see |
|---|---|---|
| **file combo + `+ New import…`** | success + progress | `contoso_q2.csv` → the job appears as `Queued`, the worker picks it up, `prgJob` steps through 10 batches (~9 s) and it ends `Completed w/ errors · ✓ 998 imported (938 created, 60 updated) · ↻ 12 retried in 18 attempts · ✕ 2 terminal`. The trace gains one `Job:` line per batch, not one per row |
| | | `contoso_pilot.csv` → 300 clean rows, 3 batches, ends `Completed` (the pure success path) |
| **Cancel job** | progress → cancellation | click during batch 4: the status reads *cancelling — finishing the current batch*, the bar advances to the end of that batch, then `Canceled after batch 4 of 10 · 400 rows imported, none half-written.` |
| **Re-import (idempotent)** | success (idempotency) | the same file again: `0 created, 998 updated`. Retrying and re-importing cannot duplicate a work order — the writer upserts by `ExternalRef` |
| **Failure: malformed file** | failure (terminal, whole job) | `contoso_q2_corrupt.csv` fails at 0 % with a red banner; the job detail shows the file-level terminal error and **no** retry attempts — there is nothing to retry |
| **Anti-pattern: push every row** | the lesson's warning, measured | 200 rows published one event per row with the observer unthrottled. When it finishes the trace prints both `Observer:` lines: ~12 events → ~11 pushes (1.4/s) for a milestone job, ~208 events → ~202 pushes (65/s) for this one. Throttling restores itself |
| **Reopen** | **recovery** | start `contoso_q2.csv`, click **Reopen** at ~40 %: the page is disposed and rebuilt, the trace (session state) survives, and the new page prints `UI → re-attached to IMP-… (Running · 45 %) — started by ana.ops N s ago, in a page that no longer exists.` The job never paused |
| **Clear trace** | — | empties the trace card (and the session's trace) |

Header and cards:

| Control | What it is |
|---|---|
| `btnBell` `🔔 Notifications (n)` | unread count; clicking it toasts the newest one |
| `pnlNotifications` / `btnMarkRead` | the notification records with `●` unread markers; **Mark all read** clears them |
| `dgvJobs` | this tenant's jobs — job number, description, status, progress, rows, who started it, when. The seeded `fabrikam` import is in the store and never appears here |
| `prgJob` · `lblPercent` · `lblStatus` · `lblBanner` | the progress observer's output (green ok · amber warning · red error) |
| `pnlJobDetail` (`JobDetailPanel`) | status history (every transition, with its layer and timestamp) and the rows that failed, by line and `ExternalRef` |
| `lstTrace` | **Server · live activity trace** — `UI →`, `Service:`, `Security:`, `Queue:`, `Job:`, `Notify:`, `Data:`, `Observer:` |

## Deliverables

| # | Lab deliverable | Where |
|---|---|---|
| 1 | Job model and status store | [`docs/JobModelAndStatusStore.md`](EnterpriseOps/docs/JobModelAndStatusStore.md) · `Services/Jobs/JobModels.cs`, `JobStatusStore.cs`, `JobStatus` in `BackgroundPipelinePatterns.cs` |
| 2 | Background queue abstraction | [`docs/BackgroundQueueAbstraction.md`](EnterpriseOps/docs/BackgroundQueueAbstraction.md) + [`docs/JobPipeline.svg`](EnterpriseOps/docs/JobPipeline.svg) · `Services/Jobs/JobQueue.cs`, `IBackgroundJob`/`IJobProgressSink`, `JobInfrastructure.cs` |
| 3 | Progress observer UI | [`docs/ProgressObserverUI.md`](EnterpriseOps/docs/ProgressObserverUI.md) · `Services/Jobs/JobProgressObserver.cs`, `UI/ImportCenterPage.RefreshFromObserver` |
| 4 | Notification panel | [`docs/NotificationPanel.md`](EnterpriseOps/docs/NotificationPanel.md) · `Services/Jobs/NotificationService.cs`, `NotifyingSink` in `JobQueue.cs`, `pnlNotifications` |
| 5 | Retry and cancellation policy | [`docs/RetryAndCancellationPolicy.md`](EnterpriseOps/docs/RetryAndCancellationPolicy.md) · `Services/Jobs/RetryPolicy.cs`, `ImportWorkOrdersJob.ProcessRowAsync`, `Data/FakeWorkOrderRepository.Upsert` |
| — | Production-readiness note (last lab step) | [`docs/ProductionReadinessNote.md`](EnterpriseOps/docs/ProductionReadinessNote.md) |

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Open the project and run it once | `EnterpriseOps.slnx` · `dotnet run -f net10.0 --urls http://localhost:5206` |
| Identify the architecture risk before touching the UI | `docs/BackgroundQueueAbstraction.md` (the import in a click handler) and `docs/ProgressObserverUI.md` (the update flood) |
| Create the service / model boundary first | `Services/Jobs/*` — the queue, the store, the sink and the policies exist before any control does; `UI/` has no logic to move |
| Build the screen with the project standards | `UI/ImportCenterPage.cs` + `.Designer.cs`, `UI/JobDetailPanel.cs` + `.Designer.cs` (card layout, `lstTrace`, `lblStatus`, bottom action bar) |
| Deliverable: job model and status store | `Services/Jobs/JobModels.cs`, `JobStatusStore.cs` |
| Deliverable: background queue abstraction | `Services/Jobs/JobQueue.cs` (`IJobQueue`, `InMemoryJobQueue`, `NotifyingSink`), `JobInfrastructure.cs` |
| Deliverable: progress observer UI | `Services/Jobs/JobProgressObserver.cs`, `ImportCenterPage.RefreshAll` |
| Deliverable: notification panel | `Services/Jobs/NotificationService.cs`, `ImportCenterPage.RefreshNotifications` |
| Deliverable: retry and cancellation policy | `Services/Jobs/RetryPolicy.cs` (+ `CancellationPolicy`), `ImportWorkOrdersJob` |
| Add a failure-path demonstration, not only the happy path | `btnMalformedFile_Click` (terminal job), the 2 terminal rows and 12 retried rows in `contoso_q2.csv`, `btnCancelJob_Click`, `btnAntiPattern_Click` |
| Show every path without leaking internals | typed `CommandResult` / `CommandResult<T>` to the UI; the banner shows a sentence, the trace shows the layer, the exception type never reaches a message box |
| Review & run: production-readiness note | `docs/ProductionReadinessNote.md` |

## Where things live

```
Module 6/
├─ EnterpriseOps.slnx
├─ README.md
└─ EnterpriseOps/
   ├─ Domain/
   │  ├─ WorkOrder.cs                  shared course vocabulary + ExternalRef (the idempotency key)
   │  └─ ImportRow.cs                  one parsed line of an import file
   ├─ Data/
   │  ├─ FakeImportFileSource.cs       4 generated files: the 1,000-row job, a clean pilot, the flood sample, the corrupt one
   │  └─ FakeWorkOrderRepository.cs    the "table": idempotent Upsert, transient timeouts, terminal validation
   ├─ Services/
   │  ├─ ActivityTrace.cs              the session's trace (survives the page being recreated)
   │  ├─ SessionContext.cs             tenant · user · role · correlation ids · CommandResult
   │  └─ Jobs/
   │     ├─ BackgroundPipelinePatterns.cs  JobStatus · JobProgress · IJobProgressSink · IBackgroundJob · ImportWorkOrdersJob
   │     ├─ JobModels.cs                   JobRecord · JobHistoryEntry · JobResultSummary · RowError · commands
   │     ├─ JobStatusStore.cs              IJobStatusStore + in-memory store (clones, tenant reads, Changed event)
   │     ├─ JobQueue.cs                    IJobQueue + one-worker queue, per-job CancellationTokenSource, NotifyingSink
   │     ├─ NotificationService.cs         notification records, read/unread, per user or role, tenant-aware
   │     ├─ RetryPolicy.cs                 transient vs terminal, backoff, CancellationPolicy
   │     ├─ JobProgressObserver.cs         Application.StartTask loop · Application.Update every 400 ms
   │     ├─ ImportService.cs               the service the page calls: permissions, tenant boundary, commands
   │     └─ JobInfrastructure.cs           the process-wide bundle (queue · store · notifications · data) + seeds
   ├─ UI/
   │  ├─ ImportCenterPage.cs / .Designer.cs   the Import Center (observer only)
   │  └─ JobDetailPanel.cs / .Designer.cs     the job detail screen (a UserControl, so it can move to its own page)
   ├─ docs/                            the five deliverables, the pipeline SVG and the readiness note
   ├─ Program.cs                       session entry point: SessionContext + ActivityTrace in Application.Session
   └─ Startup.cs                       Kestrel host (app.UseWisej())
```

## Student review questions, answered against this sample

- **Who owns the job after the session closes?**
  The service layer: `IJobQueue` owns the execution (the worker thread and the per-job
  `CancellationTokenSource`) and `IJobStatusStore` owns the state. Both live in `JobInfrastructure`, which
  belongs to the process, not to `Application.Session`. The page owns controls and one observer, and is one
  of possibly **zero** observers — `ImportWorkOrdersJob` has no reference to a control, a page or a session,
  and the queue worker runs on a thread that has no Wisej session at all. Click **Reopen** at 40 % to watch
  the owner stay the same while the owner's audience is destroyed and rebuilt.

- **How often does the UI update?**
  At most **2.5 times per second per page** — `JobProgressObserver.MinPushIntervalMs = 400`. The job store's
  `Changed` event only sets a `bool`; the observer's `Application.StartTask` loop coalesces everything that
  happened since the last tick into one `Application.Update`. A 1,000-row import produces about a dozen
  pushes, and the exact numbers are printed in the trace at the end of every job (`Observer: … events → …
  pushes … pushes/s`). **Anti-pattern: push every row** runs the same machinery unthrottled: the loop pushes back to back
  with no pause, so the only limit is how fast a full refresh + `Application.Update` completes. In the verified run
  a 200-row flood raised 212 change events and the loop pushed as fast as it could (`212 change event(s) → 6
  push(es) in 1.5 s (4.1 pushes/s)`), every one of them a full grid rebuild for information nobody asked for —
  the two `Observer:` lines can be compared in the same trace.

- **Which errors are retryable and which are terminal?**
  Retryable = a failure a later attempt could survive: `TransientRowException` (write timeout, lock
  contention). Bounded — 3 attempts, 100/200/400 ms backoff — and safe only because `Upsert` is idempotent by
  tenant + `ExternalRef`. Terminal for one row = `TerminalRowException` (unknown asset code, missing title):
  recorded in `RowErrors` with the line number, and the job carries on. Terminal for the whole job =
  `MalformedFileException` (nothing to retry) and **anything unrecognised**, which the queue records as
  `Failed` rather than retrying an error it does not understand.

## Instructor acceptance criteria, answered

| Criterion | How this sample meets it |
|---|---|
| Follows the course architecture baseline | folder-per-layer (`Domain` / `Data` / `Services` / `Services/Jobs` / `UI` / `docs`), namespaces matching folders, typed commands and results (`StartImportCommand`, `CancelJobCommand`, `CommandResult<JobRecord>`), `CommandContext` on every service call, per-session `SessionContext` in `Application.Session` |
| UI event handlers remain thin and explainable | every handler builds a command, calls `ImportService`, shows the typed result, and has its own `try/catch` → `ReportFailure`. The longest one, `StartImport`, is ~25 lines and contains no import logic. The single background loop lives in `JobProgressObserver`, not in a handler |
| Service-level logic reviewable without opening the designer | permissions, tenant filtering, queueing, batching, retries, cancellation, notification rules and the push bound are all in `Services/Jobs/*.cs`; the designer files contain layout only |
| At least one failure path demonstrated | four: a job that fails terminally on a malformed file, 2 rows that fail terminally inside a successful import, 12 rows that fail transiently and are retried, and a job cancelled mid-way — plus the measured anti-pattern, and the "session gone" recovery |
| Student can explain state ownership, security implications and production behaviour | the three answers above, `docs/ProductionReadinessNote.md` (durable queue, leases, back-pressure, audit), and the security lines in the trace: role check on start/cancel, and the tenant filter that hides the seeded `fabrikam` job from `ana.ops@contoso` |

## Verified / unverified

Used from the course cookbook and **verified** on this framework build (the Application Integration and
Foundations samples exercised them at runtime):

- `Application.StartTask(() => { …; Application.Update(component); })` for background work in the session
  context, with a bounded push interval (≥ 250 ms — this sample uses 400 ms), `IsDisposed` checks and
  `catch (ObjectDisposedException)`.
- `Application.Session.<name>` as a dynamic per-session bag; `AlertBox.Show(..., ContentAlignment.TopRight,
  autoCloseDelay:)`; `Panel.BorderStyle = BorderStyle.Solid`; `Font("default", 12F, Bold)` /
  `Font("monospace", 9F)`; designer-generated `InitializeComponent` with code-behind handlers.

Used and **unverified at runtime** (they compile on both target frameworks; the reviewer should confirm them
when running the app):

- `DataGridView` with `AutoGenerateColumns = false`, `DataGridViewTextBoxColumn` bound by `DataPropertyName`,
  `SelectionMode = FullRowSelect`, `MultiSelect = false`, and `Rows[i].Selected = true` immediately after a
  `DataSource` assignment (the sample re-selects the current job after each refresh; if Wisej materialises
  rows asynchronously, the guard `index < dgvJobs.Rows.Count` simply skips the re-selection for one tick).
- `Application.MainPage = new ImportCenterPage();` followed by `this.Dispose()` as the "close and reopen the
  screen" gesture.
- `Wisej.Web.ProgressBar` (`Maximum` / `Value`) and `ListBox.BeginUpdate()` / `EndUpdate()`.
- `AlertBox.Show` called from inside the `Application.StartTask` push loop (rather than from a request
  thread) for the "while you were away" toast — it is wrapped in its own `try/catch` so a toast can never
  break the observer.

Everything else in the sample is plain .NET: `BlockingCollection`, `CancellationTokenSource`,
`Interlocked`, `Task.Run`.
