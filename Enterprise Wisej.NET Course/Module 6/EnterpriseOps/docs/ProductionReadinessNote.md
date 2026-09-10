# Production-readiness note — Module 6 Import Center

The last lab step: what this sample would need before it carried a real import, and what it already gets
right. Written the way a review comment is written — short, and specific about what is missing.

## What is production-shaped already

- **State ownership.** The queue, the job store, the notification service and the work-order table live in
  the process (`JobInfrastructure`); the page owns only controls. Closing the page (**Reopen**) proves it.
- **Tenant boundary in the service.** The store is global; `ImportService.ListJobs` / `GetJob` are the only
  ways in, and both filter by `CommandContext.TenantId`. Refusals are traced as `Security:` lines.
- **Bounded UI updates.** One observer per page, one constant (`MinPushIntervalMs = 400`), measured on
  every run and printed in the trace.
- **Errors are classified, not caught-and-hidden.** Transient / terminal-row / terminal-job, each with a
  different consequence, and every one of them recorded.
- **Idempotent writes.** Upsert by tenant + `ExternalRef`, so retries and duplicate messages are safe.
- **Cancellation with a contract.** Flag, checked between batches, current batch finishes, result recorded.

## What must change before this is real

| Gap | What production needs |
|---|---|
| The queue is in-memory and single-worker | a durable queue (Azure Storage/Service Bus queue, or a table with a lease column); jobs must survive a process restart, and `Queued` jobs must be re-drivable by whichever instance picks them up |
| The job store is a `Dictionary` | a `jobs` table (+ `job_history`), with the same `JobRecord` shape; the sample's `IJobStatusStore` is already the seam |
| One process | with more than one instance, the observer must read the store (shared), never a queue in the same process. Nothing in the page changes; `JobInfrastructure` becomes a repository over the database |
| No leases or duplicate protection | a worker crash mid-job must return the job to `Queued` after a visibility timeout; the job body is already idempotent, which is what makes that safe |
| Files are generated in memory | real uploads (`Wisej.Web.UploadedFile` / blob storage), a virus scan, size limits, and a retention policy for the source file — a failed import must be re-runnable from the same bytes |
| Job audit is the history list | keep the history, but also write who started / cancelled each job to the audit trail Module 3 defines; background work must be as accountable as interactive work |
| Cancellation is per process | with a durable queue, cancel is a flag **in the store** that the worker polls between batches, not a `CancellationTokenSource` reference |
| No back-pressure | one worker is a queue depth limit of one. Real systems need concurrency limits per tenant so one 200k-row import cannot starve every other tenant's jobs |
| Notifications are in-memory and in-app | a notifications table, plus a delivery channel (email/Teams) for jobs long enough that nobody will still be in the app — the `INotificationService` seam is already the right one |
| Progress is a percentage of batches | for real files, publish rows-processed as well, and make the ETA come from measured throughput rather than a batch count |

## The three review questions, answered

- **Who owns the job after the session closes?** The queue (execution) and the job store (state) — the
  service layer, never the screen. The screen is one of possibly zero observers.
- **How often does the UI update?** At most 2.5 times a second per page
  (`JobProgressObserver.MinPushIntervalMs = 400`), regardless of how many events the job raises. The
  anti-pattern button runs the same loop unthrottled (a push per event, back to back — only the cost of the
  refresh itself limits it; the verified run shows `212 change event(s) → 6 push(es) in 1.5 s`) for comparison.
- **Which errors are retryable and which are terminal?** Retryable = a failure a later attempt could
  survive (`TransientRowException`: timeouts, contention), bounded to 3 attempts with exponential backoff.
  Terminal = a failure that will always fail the same way (bad asset code, malformed file) — and anything
  unrecognised, because retrying an error you do not understand is worse than reporting it.
