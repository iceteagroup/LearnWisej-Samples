# Deliverable 4 — Transaction example

**The rule.** The transaction boundary lives in the command handler, because that is the only place
where the whole operation is visible. Not in the event handler (it cannot see the audit write), not in
the repository (it cannot see the validation), not in the `DbContext` (it cannot see the intent).

## What is inside the transaction

For **Approve**, exactly this, in this order:

```
BEGIN
  1. SELECT the aggregate  … WHERE TenantId = @tenant AND Id = @id      (tracked)
  2. concurrency check     original Version := the version the user saw
  3. domain validation     WorkOrderTransitions.CanApprove(status)
  4. persist               UPDATE WorkOrders SET Status='Completed', ApprovedBy, ApprovedUtc,
                                  ApprovalComment, Version = @expected + 1
                           WHERE  Id = @id AND Version = @expected
  5. audit                 INSERT AuditEntries (Approve, Committed, correlation id, v8 → v9)
COMMIT
```

**Outside** the transaction, before it opens: the permission check
(`Permissions.IsAllowed(context.Role, Operation.Approve, …)`). A caller who may not perform the
operation never gets a transaction at all.

**Outside** it, after a rollback: the *rejection* audit row. An audit row that rolls back together with
the failure it records is no audit at all, so it is written by a second short-lived context in its own
transaction (`WriteRejectionAuditAsync`).

Nothing else is inside. No e-mail, no notification queue, no file write, no HTTP call — anything that
cannot be rolled back is queued only after the commit.

## The code

`Data/WorkOrderCommandService.cs` — the template every command runs through:

```csharp
// A bounded wait: a command that cannot finish becomes DB_TIMEOUT, not a hung session.
var timeout = _faults.TakeTimeout() ?? _commandTimeout();
using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
timeoutSource.CancelAfter(timeout);
var ct = timeoutSource.Token;

// One session, one SQLite connection: commands and queries take turns instead of overlapping.
await _database.Gate.WaitAsync(cancellationToken);
try
{
    // Security first: no transaction is opened for a caller who may not perform the operation.
    if (!Permissions.IsAllowed(context.Role, op, out string denied))
    {
        await WriteRejectionAuditAsync(context, operation, workOrderId, ErrorMap.PermissionDenied, denied);
        return ErrorMap.Rejected(ErrorMap.PermissionDenied, denied, context.CorrelationId, _trace);
    }

    CommandResult result = null;
    string auditDetail = null;

    var dbContext = _database.CreateContext($"{operation} command");     // the unit of work
    try
    {
        var repository  = new WorkOrderRepository(dbContext, _trace);
        var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            result = await body(repository, dbContext, ct);               // load → check → validate → persist → audit

            if (result.Success)
                await transaction.CommitAsync(ct);
            else
                await transaction.RollbackAsync(CancellationToken.None);  // a domain rejection: nothing persisted
        }
        catch (Exception exception)
        {
            var mapped = ErrorMap.Map(exception, operation, context.CorrelationId);
            TryRollback(transaction);
            auditDetail = mapped.AuditDetail;
            result = CommandResult.Fail(mapped.UserMessage, mapped.Code, context.CorrelationId);
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }
    finally
    {
        _database.Release(dbContext, …);        // short-lived means short-lived
    }

    if (!result.Success)
        await WriteRejectionAuditAsync(context, operation, workOrderId ?? result.WorkOrderId,
                                       result.ErrorCode, auditDetail ?? result.UserMessage);
    return result;
}
finally
{
    _database.Gate.Release();
}
```

And the body of **Approve**, which is the part a reviewer reads:

```csharp
if (string.IsNullOrWhiteSpace(command.Comment))
    return ErrorMap.Rejected(ErrorMap.ValidationFailed, "An approval comment is required.", …);

var workOrder = await repository.LoadAsync(command.TenantId, command.WorkOrderId, ct);   // by tenant + id
if (workOrder == null)
    return ErrorMap.Rejected(ErrorMap.NotFound, …);

if (!WorkOrderTransitions.CanApprove(workOrder.Status, out string reason))               // pure domain rule
    return ErrorMap.Rejected(ErrorMap.StateInvalid, reason, …);

repository.ExpectVersion(workOrder, command.ExpectedVersion);                            // the WHERE clause
workOrder.Status          = WorkOrderStatus.Completed;
workOrder.ApprovedBy      = command.UserId;
workOrder.ApprovedUtc     = DateTime.UtcNow;
workOrder.ApprovalComment = command.Comment.Trim();
repository.AddAudit(Audit(context, "Approve", workOrder.Id, "Committed", null, $"v{command.ExpectedVersion} → v{workOrder.Version}: …"));

await SaveAsync(dbContext, "UPDATE WorkOrders … · INSERT AuditEntries", ct);              // one round trip, both rows
return CommandResult.Ok("Work order approved.", context.CorrelationId, workOrder.Id, workOrder.Version);
```

The update and the audit insert are in the same `SaveChanges`, inside the same transaction: either both
rows are there or neither is.

## How the concurrency check is really made

Not by an `if`. `WorkOrderRepository.ExpectVersion` tells EF Core which value the user saw, and EF Core
puts it in the `WHERE` clause:

```csharp
_context.Entry(workOrder).Property(x => x.Version).OriginalValue = expectedVersion;
workOrder.Version = expectedVersion + 1;
```

`Version` is configured as `IsConcurrencyToken()` in `EnterpriseOpsDbContext.OnModelCreating`, so the
statement becomes `UPDATE WorkOrders SET … WHERE Id = @id AND Version = @expected`. If someone else
committed in between, that matches **0 rows**, EF Core raises `DbUpdateConcurrencyException`, and the
`catch` above maps it to `WO_CONCURRENCY`. The database arbitrates, not the application — which is the
only way it works when there are two servers.

## What a batch does

**Batch approve** runs six commands, and therefore six transactions — not one. Each work order commits
or rolls back on its own; a rejected `OnHold` row does not undo the five that were approved. That is a
deliberate choice for this operation, and it is the reason the button's tooltip says *one tx each*: a
single transaction around all six would be the right answer only if the six were meaningless apart.

## Evidence in the running app

Watch the trace card while clicking:

| Click | Trace, in order |
|---|---|
| **Approve** an `InProgress` row | `Security: authorize ana.ops (Manager) → Approve allowed` · `Data: DbContext #n created` · `Data: BEGIN TRANSACTION (timeout 5000 ms)` · `Data: SELECT WorkOrders … (tracked)` · `Service: check concurrency: user saw v3, database has v3 → match` · `Service: validate transition InProgress → Completed ✓` · `Data: SaveChanges → UPDATE … WHERE Id=… AND Version=3 · INSERT AuditEntries` · `Data: SaveChanges affected 2 row(s)` · `Data: COMMIT` · `Audit: Approve Committed … (written inside the transaction)` · `Data: DbContext #n disposed … live contexts: 0` |
| **Approve** `WO-2002` (`OnHold`) | … `Service: validate transition OnHold → Completed ✗` · `Data: ROLLBACK (WO_STATE_INVALID)` · `Data: DbContext #n disposed (1 tracked entities released · rolled back)` · `Audit: Approve Rejected WO_STATE_INVALID … (own transaction, survives the rollback)` |
| **Fail: stale version** | `Service: check concurrency: user saw v2, database has v3 → MISMATCH (the database will reject the UPDATE)` · `Data: SaveChanges → UPDATE … WHERE … Version=2` · `Data: ROLLBACK ← DbUpdateConcurrencyException` · `Service: ErrorMap: DbUpdateConcurrencyException → WO_CONCURRENCY` |
| **Batch approve (one tx each)** | six `BEGIN TRANSACTION` lines, each followed by its own `COMMIT` or `ROLLBACK`; the status bar counts `n committed · m rejected` as it goes |

Then open the **Audit log**: the committed approvals and the rejected ones are both there, each with its
correlation id — proof that the rejection audit survived the rollback.
