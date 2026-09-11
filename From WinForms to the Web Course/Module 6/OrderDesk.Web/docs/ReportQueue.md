# Report queue — queued report jobs with progress, status, cancel and download (lab deliverable 4)

Lab step covered: *add a simple job queue for long reports*. Deliverable 3 of the storyboard: "queued report —
progress, status, cancel, download link". The lesson's two reasons for a queue: a reporting engine that cannot handle
heavy parallel use is protected by serialising the work, and a long report no longer freezes the UI — the page shows
job status instead. The desktop loop it replaces (`for each order → PrintDocument`, UI frozen until the last page) is
row 8 of `FileBoundaryClassification.md`.

Code: `Reporting/ReportQueue.cs` (the queue and the worker), `Reporting/ReportBuilders.cs` (the three jobs),
`MainPage.cs` (the UI that polls it).

## Design

```
 session A (kelly)            ┌──────────── ReportQueue (static, one per process) ────────────┐
 buttonQueueBatch ──Enqueue──▶│ Jobs: List<ReportJob>   lock (Gate)   _nextId   _worker: Task │
 timerQueue (1 s) ◀─Snapshot──│                                                                │
 buttonCancelJob ──Cancel────▶│  WorkerLoop (Task.Run, thread pool, NO session):               │
                              │    take first Queued → Running → job.Work(progress, token)     │
 session B (sam)              │    → File.WriteAllBytes(Path.Combine(outputFolder, "0002-…"))  │
 buttonQueueSummary ─Enqueue─▶│    → Done | Cancelled | Failed                                 │
 timerQueue (1 s) ◀─Snapshot──└────────────────────────────────────────────────────────────────┘
                                                        │
                                                        ▼
                              App_Data/reports/0001-Q2-summary.pdf   ← View ▸ (PdfViewer) / Download ⬇
```

* **One list for the whole process.** `ReportQueue` is a `static` class with one `List<ReportJob>` behind one lock —
  deliberately process-wide, like a database table, so **every session sees the same queue**. This is the one static
  in the module that passes Module 4's rule ("would it differ for two users?" — no, the queue is shared by design, and
  every access is locked). What is per user is the job's **`Owner`**, taken from `Application.Session.User` at
  `Enqueue` time — never from a static "current user".
* **One worker, sequential.** `Enqueue` starts `Task.Run(WorkerLoop)` if no worker is running; the loop takes the first
  `Queued` job, marks it `Running`, runs it, and repeats until nothing is queued, then sets `_worker = null` so the next
  `Enqueue` starts a fresh one. Reports therefore never run in parallel — the protection the lesson asks for.
* **Outside any session.** The worker runs on the thread pool. It has **no `Application` context**: it must never touch
  a control, never call `Application.StartupPath`, never `Application.Update`. It only computes bytes, writes them under
  a folder the **caller** resolved inside the session (`StorageRoot.Reports`, passed to `Enqueue` as `outputFolder`),
  and updates the job record under the lock. `Application.StartTask` would be the tool for work owned by *one* session;
  this queue is owned by the process.
* **Copies out, tokens in.** `Snapshot()` / `Find()` return `Clone()`s — pages bind copies to the grid and can never
  mutate a live job. Cancellation goes the other way through a `CancellationTokenSource` per job.
* **States** `Queued → Running → Done | Failed | Cancelled`, with `Progress` 0–100 written by the worker
  (`SetProgress` ignores writes once the job left `Running`). `ProgressText` renders `waiting` / `42%` / `100%` /
  `42% · stopped` / `42% · failed`; `ResultText` renders `Q2-summary.pdf · view ▸` / the error / `—` / `12s elapsed`.
* **Results under the storage root.** `Path.Combine(outputFolder, $"{job.Id:D4}-{job.ResultFileName}")` →
  `App_Data/reports/0001-Q2-summary.pdf`. The id prefix keeps two jobs with the same name apart; the user-facing name
  (`ResultFileName`) is what `Application.Download(job.ResultPath, job.ResultFileName)` hands to the browser.
* **Cancel.** `Cancel(id)`: a `Queued` job turns `Cancelled` immediately; a `Running` job gets its token cancelled and
  turns `Cancelled` when the builder observes it (`token.ThrowIfCancellationRequested()` between units of work). A job
  that is already Done / Failed / Cancelled returns `false` — nothing to cancel.
* **Polling with `Wisej.Web.Timer` + `Signature()`.** Every page runs `timerQueue` (`Interval = 1000`). Each tick calls
  `ReportQueue.Signature()` — `id:status:progress;` for every job — and **redraws only when the string changed**
  (`RefreshQueue(force: false)`), so an idle queue costs one string compare per second per session and no round trip
  of grid data. A forced refresh follows every local action (queue, cancel). No push: the worker cannot reach a session, and polling is what makes "a second session sees the same queue" free.

## The jobs (`ReportBuilders`)

Each builder returns `Func<Action<int>, CancellationToken, byte[]>`: it reports progress, honours the token, returns
the finished PDF. The pauses stand in for a real rendering engine so the progress column has something to show.

| Button | Job | Work | Simulated time | Result |
|---|---|---|---|---|
| `buttonQueueBatch` | Invoice batch × 1,204 orders | one `InvoiceDocument.Build` page per order (the five orders repeated), 30 ms each, cancel check per page | ≈ 36 s (`1204 × 30 ms`) | `Invoice-batch-1204.pdf`, 1,204 pages (`InvoicePdfWriter.WritePages`) |
| `buttonQueueStatement` | Monthly statement | per-customer statement, 100 steps × 150 ms | ≈ 15 s | `Monthly-statement.pdf` |
| `buttonQueueSummary` | Q2 summary | summary by customer and status, 30 steps × 100 ms | ≈ 3 s | `Q2-summary.pdf` — the first job to be Done, for View ▸ / Download ⬇ |

Totals inside the statements are computed with `OrderService.CalculateOrderTotal` — the reused business rule, not a
stored column.

## What production adds

This queue is the smallest thing that has all four properties the lesson names (progress, status, cancel, download).
A production queue adds, in roughly this order:

1. **A persistent store** — jobs in a table (or a durable queue) so they survive a process restart and a scale-out;
   today a restart loses the list (the PDFs under `reports/` survive, the rows do not).
2. **A separate worker service** — the same `WorkerLoop` in a Windows service / systemd unit / container reading the
   table, so report CPU never competes with the web tier and several workers can run when the engine allows it.
3. **Per-user limits and fairness** — max queued jobs per user, or round-robin by owner, so one user's 1,204-page batch
   does not starve everyone else (today it is strict FIFO).
4. **Retries and dead-lettering** — a `Failed` job is retried n times with back-off; the error is logged with the
   host logger, not only in `job.Error`.
5. **Result expiry and ownership** — results deleted after n days; only the owner (or their team) may View / Download
   (Module 7's download guard); session cleanup (Module 4) cancels a leaving session's queued jobs or re-owns them by
   user.
6. **Push instead of poll** where it matters — `Application.Update` from a session-owned `StartTask` that watches the
   table, or a SignalR-style notification; polling once a second is fine for a queue page, wasteful for a dashboard.
7. **Idempotent enqueue** — a request id so a double click or a retried HTTP request does not queue the batch twice.

None of these change the shape callers see (`Enqueue`, `Snapshot`, `Cancel`, `Find`), which is why the sample keeps
the API this small.

## Evidence

| Action | What you should see |
|---|---|
| Page load | counts label `0 job(s) · 0 queued · 0 running · 0 done` |
| `buttonQueueSummary` | row `1 · Q2 summary · kelly · Running · n% · 0s elapsed`, then within ~3 s `Done · 100% · Q2-summary.pdf · view ▸`; counts `1 job(s) · 0 queued · 0 running · 1 done` |
| `buttonQueueBatch`, then `buttonQueueStatement` | Jobs 2 and 3; the batch runs (progress climbing, the progress bar follows the selected row), the statement waits (`Queued · waiting · —`); counts `3 job(s) · 1 queued · 1 running · 1 done` |
| `buttonCancelJob` on the running batch | on the next tick the row turns `Cancelled · 37% · stopped · —`; the statement starts by itself |
| `buttonViewResult` on job 1 | the PDF opens in the modal `PdfViewer` |
| `buttonDownloadResult` on job 1 | the browser saves `Q2-summary.pdf` |
| `buttonSecondSession`, then queue a job there | the new tab's user is `sam`; it shows the same jobs; a job queued there shows `owner sam` in **both** tabs within a second |

`buttonCancelJob`, `buttonViewResult` and `buttonDownloadResult` are enabled only when the selected row allows it
(active / Done).