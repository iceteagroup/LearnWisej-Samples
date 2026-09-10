# Deliverable 2 · Search projection model

`Services/WorkQueues/HighVolumeGridPatterns.cs` — the record `WorkQueueRow`, filled by
`WorkQueueQueryService.Project`.

```csharp
public sealed record WorkQueueRow(
    int Id, string Number, string Title,
    string StatusText, string PriorityText, string AssignedTo,
    DateTimeOffset DueAt,
    bool CanApprove, bool CanReassign,
    string DueText, int AgeDays, bool IsOverdue, int Version);
```

## The two models and why they are two

| | `Domain/WorkOrder.cs` | `WorkQueueRow` |
|---|---|---|
| Shape | foreign keys, timestamps, enums, business fields | the columns the grid displays |
| `Status` | `WorkOrderStatus.InProgress` | `"In progress"` |
| Assignee | `AssignedTo` may be `""` | `"—"` when nobody is assigned |
| Due | `DateTime? DueUtc` | `DueAt` + `DueText` (`"Jun 14"`) + `IsOverdue` |
| Age | derived from `CreatedUtc` | `AgeDays`, computed once on the server |
| Permissions | none — the entity does not know who is asking | `CanApprove`, `CanReassign` |
| Hidden fields | `RequiredCertification`, `OpenApprovalId`, `Customer`, `Site` | not carried — the grid does not show them |
| Changes when | a business rule changes | a grid column changes |

Binding the entity to the grid would force the screen to compute a status label, an age and an "am I allowed to"
flag row by row, and would push presentation concerns into the domain class. Keeping them apart means a new grid
column changes the projection only, and a new business rule changes the entity only.

## What the projection deliberately does **not** carry

`RequiredCertification` and `OpenApprovalId` are business state the batch validates against. They are not on the
row because the grid never shows them — and because a value that reached the browser is a value a user can forge.
The batch re-reads both from the store before it writes.

`Customer` and `Site` are searched but not displayed, so they are filtered on the server and left out of the
payload. That is the point of a projection: **a page of 50 rows is ≈ 13.7 KB, not a graph of entities**.

## Permission flags are a hint, not an authority

`CanReassign` is computed once per row by `Security/PermissionService.cs` so the screen can say *"3 selected · 3 the
server would allow"* and grey out what is pointless. It is a **UI hint**. `BatchReassignWorkflow.ProcessRow` asks
`PermissionService.CanReassign` again, against the entity it just read, before every write. A flag that arrived
from the browser never authorizes anything.

Press **Run as ben.tech (Technician)** to see both halves: the next page comes back with `CanReassign = false`
(the selection line drops to *0 the server would allow*) **and** the workflow denies every row with
`permission-denied` if you run the batch anyway.

## `Version` is part of the projection on purpose

The grid shows the optimistic-concurrency token in the **v** column, and the selection snapshot keeps it. The batch
sends it back as `BatchItem.ExpectedVersion`, and `WorkOrderStore.TryReassign` re-checks it under the lock. That is
how a row somebody else changed fails with `stale-version` instead of silently overwriting their edit.

Press **Fail: concurrent edit** to bump a selected row's version behind the screen's back; the page is deliberately
not reloaded, so the stale value stays in the selection and the next batch refuses that row.

## Evidence in the running app

| Do this | What you should see |
|---|---|
| Look at the grid | `Status` reads *In progress*, not `InProgress`; unassigned rows read `—`; `Age (d)` and `Due` are display values |
| Look at an overdue row | painted red — from `IsOverdue`, computed on the server, not by the grid |
| Watch the footer | `… · 13.7 KB page` — that is the projected page serialized, the number that would cross the wire |
| Click **Anti-pattern: load everything** | ≈ 806 KB of projected rows for the same tenant — the same projection, 59× the payload, because the page size is gone |
