# Deliverable 1 — Job model and status store

The contract between the half that **executes** and the half that **observes**. Nothing else is shared:
the job never sees a control, the page never sees a thread.

## The job model

`Services/Jobs/JobModels.cs` — `JobRecord`

| Field | Why it is on the record |
|---|---|
| `JobId` (Guid) | identity that survives the screen, the session and (in production) the process |
| `Number` (`IMP-3041`) | what people say out loud in a support call |
| `Type`, `Description`, `Input` | what work this is, in words a user recognises (`ImportWorkOrders`, `contoso_q2.csv`) |
| `TenantId`, `StartedBy`, `CorrelationId` | who owns it — the tenant boundary and the audit trail, inherited from the `CommandContext` |
| `Status` | the state machine, five terminal-or-not values (below) |
| `Percent` | progress, **separate from status**: Running at 10 % and Running at 90 % are the same status |
| `Message` | the current milestone in one sentence |
| `CreatedUtc`, `StartedUtc`, `FinishedUtc` | queued-to-started latency and duration, without asking anyone |
| `Result` (`JobResultSummary`) | counts + **every bad row**, so partial failure is actionable |
| `History` (`List<JobHistoryEntry>`) | when each transition happened and what it said |

`JobStatus` (`Services/Jobs/BackgroundPipelinePatterns.cs`, verbatim from the lesson resource):

```
Queued → Running → Completed
                 → CompletedWithErrors
                 → Failed
                 → Canceled
```

`IsActive` = `Queued || Running`. Everything else is finished, and finished is final: the store never
moves a job back.

## The result summary

`JobResultSummary` counts `TotalRows`, `BatchesCompleted`, `Imported`, `Created`, `Updated`, `Retried`,
`RetryAttempts`, an optional `TerminalError` (a whole-job failure such as a malformed file) and a list of
`RowError { LineNumber, ExternalRef, Message, Retryable }`.

That is what makes "10,000 rows, 12 bad ones" a five-minute fix instead of a re-import: the user gets the
twelve line numbers, not "the import failed".

## The status store

`Services/Jobs/JobStatusStore.cs` — `IJobStatusStore` / `InMemoryJobStatusStore`.

- `Create` / `Get` / `ListAll` / `List(tenantId)` / `Apply(JobProgress)` / `AppendHistory(jobId, layer, message)`
- **Hands out copies.** `Get` and `List` return `record.Clone()`; the page can never mutate a live job.
- **Tenant-scoped reads.** `List(tenantId)` filters; `ImportService.GetJob` refuses a job of another tenant
  and writes a `Security:` line when it happens.
- **`Changed` event** for observers — raised on whatever thread changed the store, which is normally the
  queue worker and therefore has **no Wisej session**. Handlers must not touch controls.
- **Row-level events are not history.** `Apply` records a history entry only when `progress.IsMilestone`
  is true. A thousand rows do not belong in a status history.
- In production this is a table. Here it is a `Dictionary<Guid, JobRecord>` behind a lock, owned by the
  **process** (`JobInfrastructure`) and not by any session — which is the whole reason a reopened page
  finds its job again.

## Evidence in the running app

- Start `contoso_q2.csv`, then select the job: **Job detail** lists every transition with its timestamp —
  `Queued`, `Worker picked up IMP-…`, `Import started.`, `Validated 1,000 rows · 10 batches of 100.`,
  `Processed batch 1 of 10 …` and the final state. That list is `JobRecord.History`, not a UI log.
- The grid shows only tenant `contoso`. The trace's first `Data:` line names how many jobs of other tenants
  the store refused to return (the seeded `fabrikam` import is one of them).
- Press **Reopen**: the page is disposed and rebuilt, and the new page prints
  `UI → re-attached to IMP-… (Running · 60 %)`. The record was in the store the whole time.
