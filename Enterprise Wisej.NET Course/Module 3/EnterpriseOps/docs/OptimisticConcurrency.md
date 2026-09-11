# Deliverable 4 — Optimistic concurrency implementation

*EnterpriseOps · Advanced Module 3 · `Domain/ConcurrencyToken.cs`, `Domain/WorkOrder.Version`,
`Data/WorkOrderStore.TrySave`, `Services/WorkOrderService.SaveAsync`*

## Why not a lock

Two dispatchers can open the same work order in two sessions. Locking the record for the first one blocks the
second for as long as a lunch break, so enterprise editing screens use optimistic concurrency instead: read the
version when the record is loaded, check it when the record is saved, and handle the collision when it happens —
which is rarely.

## The token

```csharp
public sealed record ConcurrencyToken(int Version)
{
    public string ToDisplayText() => "v" + Version;
    public bool Matches(int currentVersion) => currentVersion == Version;
    public static ConcurrencyToken From(WorkOrder workOrder) => new ConcurrencyToken(workOrder.Version);
}
```

The walkthrough wraps a SQL `rowversion` (`byte[]`); this in-memory sample wraps the integer
`WorkOrder.Version`. The contract is identical: **opaque to the UI, compared by the store**. `txtVersion` is
read-only — the editor displays the token and hands it back untouched, it never invents one. In Module 4 the
same token becomes an EF Core `[Timestamp]` column and the check becomes
`UPDATE … WHERE Version = @expected` plus EF's `DbUpdateConcurrencyException`; nothing above the repository
changes.

## The check

The comparison and the write happen inside one lock, which is what a single `UPDATE … WHERE Version = @expected`
statement gives you in a database — no window in which two callers both see `v7`:

```csharp
// Data/WorkOrderStore.cs
public StoreSaveResult TrySave(int id, string title, WorkOrderStatus status, int expectedVersion, string modifiedBy)
{
    lock (_gate)
    {
        if (!_rows.TryGetValue(id, out WorkOrder row))
            return new StoreSaveResult { Saved = false, Current = null, ExpectedVersion = expectedVersion };

        if (row.Version != expectedVersion)
            return new StoreSaveResult { Saved = false, Current = row.Clone(), ExpectedVersion = expectedVersion };

        row.Title = title;
        row.Status = status;
        row.Version = row.Version + 1;
        row.ModifiedBy = modifiedBy;
        row.ModifiedUtc = DateTime.UtcNow;

        return new StoreSaveResult { Saved = true, Current = row.Clone(), ExpectedVersion = expectedVersion };
    }
}
```

The current row comes back **either way** — the conflict dialog needs it to show what changed.

## Where the check lives

In the command handler (`WorkOrderService.SaveAsync`), not in the form. That is what makes the rule apply
identically when the save comes from a batch operation or an import: they call the same service with the same
command, and get the same `ConflictInfo` back. The order inside `SaveAsync`:

1. `_guard.DemandTenant(context, command.TenantId)` — the tenant the client claimed;
2. read the row; `_guard.DemandTenant(context, row.TenantId)` — the tenant the row really has;
3. validation (title required) → `SaveWorkOrderResult.Rejected`;
4. `_store.TrySave(…, command.ExpectedVersion.Version, context.UserId)`;
5. mismatch → build a `ConflictInfo` and return `SaveWorkOrderResult.Stale`; success → `Applied`, with the new
   token the tab must use from now on.

## A stale save is data, not an exception

```csharp
public sealed class SaveWorkOrderResult : CommandResult
{
    public WorkOrderEditModel Saved { get; }
    public ConcurrencyToken NewVersion { get; }
    public ConflictInfo Conflict { get; }        // set only when another session saved first
    public bool IsConflict => Conflict != null;
}
```

Two sessions editing one record is an expected outcome of an editing screen, not a bug, so it travels as a
result the screen can explain. Exceptions are kept for what is genuinely unexpected — a cross-tenant read is one
(`CrossTenantAccessException`), and the handler's `catch` logs it and shows a generic message with the
correlation id.

## Evidence — what the running app shows

Reproduce the walkthrough's failure path with two browser tabs (two sessions, one store):

| Step | Action | What you see |
|---|---|---|
| 1 | in **both** tabs select **2002 · Repair loading dock pump** → **Open selected in editor** | `txtVersion` reads `v7`; footer `Editing — expected version v7` |
| 2 | tab 1: change the status, **Save** | footer `Saved — v7 → v8 · correlation …`; the queue's Version column shows `v8` |
| 3 | tab 2: edit the title to *Repair loading dock pump — urgent*, **Save** | footer `Save rejected — expected v7, found v8 · correlation …`, then the conflict dialog; the store wrote nothing |
| 4 | **Reload latest** in the dialog | the editor rebinds at `v8` with tab 1's values; footer `Reloaded v8 — re-apply your edit on the latest version` |
| 5 | re-apply the edit, **Save** | `v8 → v9`, the queue updates |

Step 3 is the assertion that matters: the store says *nothing written*. The first session's work is intact and
the second session's edit is still on screen.
