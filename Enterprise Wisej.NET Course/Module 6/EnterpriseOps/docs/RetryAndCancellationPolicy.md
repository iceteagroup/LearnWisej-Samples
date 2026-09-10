# Deliverable 5 — Retry and cancellation policy

> **Which errors are retryable and which are terminal?** A failure that a later attempt could survive is
> retryable. A failure that will fail identically forever is terminal. Everything unknown is terminal —
> retrying an error you do not understand is how one bad row becomes an outage downstream.

## The classification

`Services/Jobs/RetryPolicy.cs`

| Exception | Class | What happens |
|---|---|---|
| `TransientRowException` (write timeout, lock contention, downstream 503) | **retryable** | up to 3 attempts, 100 ms → 200 ms → 400 ms backoff; counted in `Retried` / `RetryAttempts` |
| `TerminalRowException` (unknown asset code, missing title) | **terminal, one row** | recorded in `RowErrors` with line number and `ExternalRef`; the job carries on |
| `MalformedFileException` (unreadable export) | **terminal, whole job** | the job fails immediately at 0 % — there is nothing to retry |
| anything else | **terminal, whole job** | the queue catches it, records `Failed: <Type>: <message>`; never swallowed |

```csharp
public sealed class RetryPolicy
{
    public int MaxAttempts { get; init; } = 3;
    public int BaseDelayMs { get; init; } = 100;
    public bool IsTransient(Exception ex) => ex is TransientRowException;
    public int DelayFor(int attempt) => BaseDelayMs * (1 << (attempt - 1));   // 100 · 200 · 400 ms
}
```

The bound and the backoff exist for the same reason: a flaky downstream system must not be hammered, and a
job must not retry forever while a user watches a bar that never moves.

## Why retrying is safe: idempotency

`Data/FakeWorkOrderRepository.Upsert` is keyed by **tenant + `ImportRow.ExternalRef`**. Importing the same
row twice produces one work order (the second time increments `Version`). And the simulated failure is
raised **before** the write, so a timed-out row leaves nothing half-written — the retry is a plain repeat,
not a compensation.

Without idempotency, "retry" means "maybe duplicate". The **Re-import (idempotent)** button demonstrates
the property directly: the same file again reports `0 created, 998 updated` — the same 998 work orders, not
1,996 of them (the other two rows are still terminal, and still named).

## The cancellation policy

`CancellationPolicy` in the same file, written down so the job and the reviewer agree:

```
checked between batches · current batch finishes · result records rows imported · status Canceled
```

1. **Cancel sets a flag.** `ImportService.CancelJob` → `IJobQueue.TryCancel` → `CancellationTokenSource.Cancel()`
   on the token source the queue keeps **per job**. Nothing stops yet, and the UI says so:
   *"cancelling — finishing the current batch"*.
2. **The job decides when it is safe to stop.** `ImportWorkOrdersJob.RunAsync` calls
   `cancellationToken.ThrowIfCancellationRequested()` at the top of each batch. Rows inside a batch are
   processed with `CancellationToken.None` **on purpose**: stopping mid-row leaves half-written data, which
   is worse than finishing 40 more rows.
3. **The result is recorded.** The `OperationCanceledException` handler publishes
   `Canceled` with the percentage reached and `"…rows imported, none half-written."`, carrying the
   `JobResultSummary` so the job detail still shows what was done.
4. **Cancelled before it started** is a case of its own: the worker sees the flag on a `Queued` job and
   records `"Canceled before it started."` — no thread was ever spent.
5. **Permission.** `ImportService.CancelJob` allows Managers and Admins to cancel anything, and anyone to
   cancel their own job. A refusal writes a `Security:` line.

## Partial failure is reported per row

`contoso_q2.csv` is built so the outcome is exact and repeatable:

- 12 rows (lines 73, 137, 199, 244, 318, 401, 466, 512, 605, 688, 741, 902) time out once or twice, then
  succeed → `↻ 12 retried in 18 attempts`
- 2 rows (lines 412 and 806) carry the asset code `ZZ-0000`, which is not in the asset register →
  `✕ 2 terminal`, listed by line and `ExternalRef` in the job detail
- everything else imports → `Completed w/ errors · ✓ 998 imported (938 created, 60 updated)`

An import of 1,000 rows with 2 bad ones does not fail. The user fixes two lines and re-imports **those**,
and idempotency means nothing else moves.

## Evidence in the running app

- **+ New import…** with `contoso_q2.csv` → finishes `Completed w/ errors`; the job detail's right-hand list
  names both bad rows and the retried ones are counted, not listed (they succeeded).
- **Cancel job** during batch 4 → the bar advances to the end of batch 4 and *then* the status becomes
  `Canceled`, with `Canceled after batch 4 of 10 · 400 rows imported, none half-written.`
- **Failure: malformed file** → `Failed` at 0 %, banner in red, job detail shows the file-level terminal
  error and no retry attempts at all.
- **Re-import (idempotent)** → `0 created, 998 updated`; the 12 flaky rows time out and retry again (the
  file is generated fresh each time), and the same two rows are terminal again.
