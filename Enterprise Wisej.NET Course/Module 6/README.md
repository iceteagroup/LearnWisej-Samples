# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 6

Local lab build for **Advanced Module 6 · Real-Time Systems, Background Pipelines, Notifications & Imports**.
It follows the walkthrough video: the **Import Center** (`ImportCenterPage`) starts imports it does not run,
a **job queue** executes them on a thread with no session, a **job status store** records every milestone, a
**progress observer** pushes to the browser with `Application.StartTask` + `Application.Update` at a bounded
rate, a **notification panel** delivers the result to whoever is there when it lands, and the
**`JobDetailPanel`** shows the history and the per-row report.

The video's failure path is the one that matters: **close the session and the job keeps running**. Close the
browser tab mid-import and open the app again: the new session finds the same job in the store, selects it
and carries on showing its progress.

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

| Control | What you should see |
|---|---|
| **file combo + `+ New import…`** (header) | `contoso_q2.csv` → the job appears as `Queued`, the worker picks it up, `prgJob` steps through 10 batches (~9 s) and it ends `Completed w/ errors · ✓ 998 imported (938 created, 60 updated) · ↻ 12 retried in 18 attempts · ✕ 2 terminal`. `contoso_pilot.csv` → 300 clean rows, ends `Completed`. Importing the same file again reports `0 created, 998 updated` (idempotent upsert by `ExternalRef`) |
| **Cancel job** (header) | click during batch 4: the status bar reads *cancelling — finishing the current batch*, the bar advances to the end of that batch, then `Canceled after batch 4 of 10 · 400 rows imported, none half-written.` |
| **close the tab, open the app again** | the recovery path: start `contoso_q2.csv`, close the tab at ~40 %, reopen <http://localhost:5206>. The new session selects the running job, toasts `While you were away: IMP-… reached N%.` and the progress never restarted |
| `btnBell` `🔔 n` (header) | unread count; clicking it toasts the newest notification |
| `pnlNotifications` / `btnMarkRead` | the notification records with `●` unread markers; **Mark all read** clears them |
| `dgvJobs` | this tenant's jobs — job number, description, status, progress, rows, who started it, when. The seeded `fabrikam` import is in the store and never appears here |
| `prgJob` · `lblPercent` · `lblBanner` | the progress observer's output; the banner shows a completed-with-errors, cancelled or failed job |
| `pnlJobDetail` (`JobDetailPanel`) | status history (every transition, with its layer and timestamp) and the rows that failed, by line and `ExternalRef` |
| `lblStatus` (status bar) | the active job's state in one line: `IMP-… running · Processed batch 3 of 10 …` |

## Deliverables

| # | Lab deliverable | Where |
|---|---|---|
| 1 | Job model and status store | [`docs/JobModelAndStatusStore.md`](EnterpriseOps/docs/JobModelAndStatusStore.md) · `Services/Jobs/JobModels.cs`, `JobStatusStore.cs`, `JobStatus` in `BackgroundPipelinePatterns.cs` |
| 2 | Background queue abstraction | [`docs/BackgroundQueueAbstraction.md`](EnterpriseOps/docs/BackgroundQueueAbstraction.md) + [`docs/JobPipeline.svg`](EnterpriseOps/docs/JobPipeline.svg) · `Services/Jobs/JobQueue.cs`, `IBackgroundJob`/`IJobProgressSink`, `JobInfrastructure.cs` |
| 3 | Progress observer UI | [`docs/ProgressObserverUI.md`](EnterpriseOps/docs/ProgressObserverUI.md) · `Services/Jobs/JobProgressObserver.cs`, `UI/ImportCenterPage.RefreshAll` |
| 4 | Notification panel | [`docs/NotificationPanel.md`](EnterpriseOps/docs/NotificationPanel.md) · `Services/Jobs/NotificationService.cs`, `NotifyingSink` in `JobQueue.cs`, `pnlNotifications` |
| 5 | Retry and cancellation policy | [`docs/RetryAndCancellationPolicy.md`](EnterpriseOps/docs/RetryAndCancellationPolicy.md) · `Services/Jobs/RetryPolicy.cs`, `ImportWorkOrdersJob.ProcessRowAsync`, `Data/FakeWorkOrderRepository.Upsert` |
| — | Production-readiness note (last lab step) | [`docs/ProductionReadinessNote.md`](EnterpriseOps/docs/ProductionReadinessNote.md) |

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Open the project and run it once | `EnterpriseOps.slnx` · `dotnet run -f net10.0 --urls http://localhost:5206` |
| Build the Import Center | `UI/ImportCenterPage.cs` + `.Designer.cs`, `UI/JobDetailPanel.cs` + `.Designer.cs` |
| Deliverable: job model and status store | `Services/Jobs/JobModels.cs`, `JobStatusStore.cs` |
| Deliverable: background queue abstraction | `Services/Jobs/JobQueue.cs` (`IJobQueue`, `InMemoryJobQueue`, `NotifyingSink`), `JobInfrastructure.cs` |
| Deliverable: progress observer UI | `Services/Jobs/JobProgressObserver.cs`, `ImportCenterPage.RefreshAll` |
| Deliverable: notification panel | `Services/Jobs/NotificationService.cs`, `ImportCenterPage.RefreshNotifications` |
| Deliverable: retry and cancellation policy | `Services/Jobs/RetryPolicy.cs` (+ `CancellationPolicy`), `ImportWorkOrdersJob` |
| Show every path without leaking internals | typed `CommandResult` / `CommandResult<T>` to the UI; `ReportFailure` shows a sentence plus a reference id, the exception goes only to the server log (`ActivityTrace` → `System.Diagnostics.Trace`) |
| Recovery after closing and reopening the session | `ImportCenterPage_Load` (finds the active job in the store), `JobInfrastructure` (process-owned queue and store) |
| Review & run: production-readiness note | `docs/ProductionReadinessNote.md` |

## Where things live

```
Module 6/
├─ EnterpriseOps.slnx
├─ README.md
└─ EnterpriseOps/
   ├─ Domain/          WorkOrder.cs (ExternalRef = idempotency key) · ImportRow.cs
   ├─ Data/            FakeImportFileSource.cs (2 generated files) · FakeWorkOrderRepository.cs (idempotent Upsert)
   ├─ Services/
   │  ├─ ActivityTrace.cs     server-side log (System.Diagnostics.Trace)
   │  ├─ SessionContext.cs    tenant · user · role · correlation ids · CommandResult
   │  └─ Jobs/                BackgroundPipelinePatterns · JobModels · JobStatusStore · JobQueue ·
   │                          NotificationService · RetryPolicy · JobProgressObserver · ImportService · JobInfrastructure
   ├─ UI/              ImportCenterPage (observer only) · JobDetailPanel (UserControl)
   ├─ docs/            the five deliverables, the pipeline SVG and the readiness note
   ├─ Program.cs       session entry point: SessionContext in Application.Session
   └─ Startup.cs       Kestrel host (app.UseWisej())
```

## Student review questions, answered against this sample

- **Who owns the job after the session closes?**
  The service layer: `IJobQueue` owns the execution (the worker thread and the per-job
  `CancellationTokenSource`) and `IJobStatusStore` owns the state. Both live in `JobInfrastructure`, which
  belongs to the process, not to `Application.Session`. The page owns controls and one observer —
  `ImportWorkOrdersJob` has no reference to a control, a page or a session.

- **How often does the UI update?**
  At most **2.5 times per second per page** — `JobProgressObserver.MinPushIntervalMs = 400`. The job store's
  `Changed` event only sets a `bool`; the observer's `Application.StartTask` loop coalesces everything that
  happened since the last tick into one `Application.Update`. A 1,000-row import publishes about a dozen
  milestones, never one event per row.

- **Which errors are retryable and which are terminal?**
  Retryable = `TransientRowException` (write timeout, lock contention), bounded to 3 attempts with
  100/200/400 ms backoff, and safe only because `Upsert` is idempotent by tenant + `ExternalRef`. Terminal for
  one row = `TerminalRowException` (unknown asset code, missing title): recorded in `RowErrors` and the job
  carries on. Terminal for the whole job = `MalformedFileException` and **anything unrecognised**, which the
  queue records as `Failed` rather than retrying.

## Instructor acceptance criteria, answered

| Criterion | How this sample meets it |
|---|---|
| Follows the course architecture baseline | folder-per-layer, namespaces matching folders, typed commands and results (`StartImportCommand`, `CancelJobCommand`, `CommandResult<JobRecord>`), `CommandContext` on every service call, per-session `SessionContext` |
| UI event handlers remain thin and explainable | every handler builds a command, calls `ImportService`, shows the typed result, and has its own `try/catch` → `ReportFailure`. The background loop lives in `JobProgressObserver`, not in a handler |
| Service-level logic reviewable without opening the designer | permissions, tenant filtering, queueing, batching, retries, cancellation, notification rules and the push bound are all in `Services/Jobs/*.cs`; the designer files contain layout only |
| At least one failure path demonstrated | `contoso_q2.csv`: 2 rows fail terminally and 12 fail transiently and are retried, reported per row; a job cancelled mid-way; and the session closed mid-import |
| Student can explain state ownership, security implications and production behaviour | the three answers above, `docs/ProductionReadinessNote.md`, the role check on start/cancel and the tenant filter that hides the seeded `fabrikam` job from `ana.ops@contoso` |

## Verified / unverified

**Verified** on this framework build: `Application.StartTask(() => { …; Application.Update(component); })` with a
bounded push interval, `IsDisposed` checks and `catch (ObjectDisposedException)`; `Application.Session.<name>`;
`AlertBox.Show(..., ContentAlignment.TopRight, autoCloseDelay:)`; designer-generated `InitializeComponent`.

**Unverified at runtime** (they compile on both target frameworks): `Rows[i].Selected = true` right after a
`DataSource` assignment, `ProgressBar` `Maximum`/`Value`, `ListBox.BeginUpdate()`/`EndUpdate()`, a docked
`Label` with `Padding` as the status bar, and `AlertBox.Show` called from inside the `Application.StartTask`
push loop (wrapped in its own `try/catch`).
