# Deliverable 5 — Error mapping table

Every failure this application can produce is translated **once, at the boundary**, into three things:
a stable result code, a message the user can act on, and an audit line support can search. The mapping
lives in `Data/ErrorMap.cs` and is applied inside `WorkOrderCommandService.RunAsync`, so a form, a batch
job and a WebMethod all receive the same codes.

The user never sees the middle column of a stack trace. The audit never stores one.

## The table

| # | What happens | .NET / provider signal | Result code | User message | Audit row |
|---|---|---|---|---|---|
| 1 | The status forbids the transition (approving an `OnHold` work order) | none — a domain rule | `WO_STATE_INVALID` | "This work order is on hold — resolve the hold before approving." | `Approve · Rejected · WO_STATE_INVALID` + the reason |
| 2 | Someone else committed first | `DbUpdateConcurrencyException` (`UPDATE … WHERE Version = @expected` → 0 rows) | `WO_CONCURRENCY` | "This work order was changed by someone else while you were editing it. Reload and try again." | `optimistic-concurrency check failed (0 rows updated)` |
| 3 | The work order number is taken in that tenant | `DbUpdateException` → `SqliteException` 19 (`SQLITE_CONSTRAINT`), message contains `UNIQUE` | `WO_NUMBER_IN_USE` | "That work order number is already in use for this tenant. Choose another number." | `UNIQUE constraint UX_WorkOrders_Tenant_Number` |
| 4 | The tenant no longer exists | `DbUpdateException` → `SqliteException` 19, message contains `FOREIGN KEY` | `WO_REFERENCE_INVALID` | "The work order refers to a tenant that no longer exists." | `FOREIGN KEY constraint (TenantId)` |
| 5 | Any other database rule (`CHECK`, `NOT NULL`, length) | `DbUpdateException` → `SqliteException` 19, other text | `VALIDATION_FAILED` | "The work order could not be saved because a database rule rejected it." | `CHECK/NOT NULL constraint (sqlite extended code n)` |
| 6 | The command's own validation fails (no comment, bad number, empty title) | none — checked before any write | `VALIDATION_FAILED` | "Please correct the highlighted fields." + the field messages in `CommandResult.Errors` | `Create · Rejected · VALIDATION_FAILED` + the messages |
| 7 | The caller may not perform the operation | none — `Permissions.IsAllowed` | `PERMISSION_DENIED` | "Only a manager or an administrator can approve a work order." | `Approve · Rejected · PERMISSION_DENIED` (**no transaction was opened**) |
| 8 | The work order is not in this tenant | none — the load returns null | `WO_NOT_FOUND` | "Work order 2002 was not found in tenant contoso." | `Approve · Rejected · WO_NOT_FOUND` |
| 9 | The operation exceeds its budget inside the transaction | `OperationCanceledException` / `TaskCanceledException` / `TimeoutException` | `DB_TIMEOUT` | "The operation could not be completed in time. Try again, or quote correlation id 7c41aa90 to support." | `command timeout elapsed inside the transaction` |
| 10 | The database is unreachable, locked, corrupt | `SqliteException` (any other code) | `DB_UNAVAILABLE` | "The database is not available right now. Quote correlation id 7c41aa90 to support." | `sqlite error n` |
| 11 | Anything the mapping did not anticipate | any other `Exception` | `UNEXPECTED` | "The action could not be completed. Quote correlation id 7c41aa90 to support." | the exception **type name only** — never the stack trace |

Codes 1, 6, 7 and 8 never reach the database; they are rejections the service decides. Codes 2–5 and
9–11 are exceptions the boundary catches. Both kinds come back as the same `CommandResult`, so a caller
has one thing to check.

## Where each column comes from

```csharp
// Data/ErrorMap.cs
public static Mapped Map(Exception exception, string operation, string correlationId)
{
    var sqlite = exception as SqliteException ?? exception.InnerException as SqliteException;

    if (exception is DbUpdateConcurrencyException)
        return new Mapped { Code = Concurrency, UserMessage = "…changed by someone else…",
                            AuditDetail = $"{operation}: optimistic-concurrency check failed (0 rows updated)" };

    if (exception is DbUpdateException && sqlite != null && sqlite.SqliteErrorCode == 19)   // SQLITE_CONSTRAINT
    {
        if (sqlite.Message.IndexOf("UNIQUE", StringComparison.OrdinalIgnoreCase) >= 0)  → WO_NUMBER_IN_USE
        if (sqlite.Message.IndexOf("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) >= 0) → WO_REFERENCE_INVALID
        …
    }
    …
}
```

A `SqlServerException` build of this table would swap the provider check (error 2601/2627 for a unique
violation, 547 for a foreign key, -2 for a timeout) and change nothing else: the codes, the messages and
the audit lines are the application's vocabulary, not the provider's.

## Rules the table encodes

1. **A user message says what to do next.** "Choose another number", "Reload and try again", "Resolve
   the hold" — never "SqliteException 19" and never "contact your administrator" with nothing else.
2. **Nothing that identifies the schema reaches the user.** Table names, index names, column names and
   SQL text stay in the audit row, which lives inside the database, not in the banner.
3. **Codes are stable and greppable.** `WO_CONCURRENCY` is a search term for support, a switch label for
   a caller and a column in the audit grid.
4. **Every failure is auditable.** Rejections are written after the rollback in their own transaction, so
   the record of the failure is not undone by the failure.
5. **The correlation id is on all three.** Banner, log and audit row carry the same eight hex
   characters, so a screenshot is enough to find the row.
6. **Auditing may never make things worse.** `WriteRejectionAuditAsync` swallows its own exceptions and
   logs `could not write the rejection audit: …`; a mapped failure never becomes an unmapped one.

## Evidence in the running app

| Action | Code you should see | Banner / status bar |
|---|---|---|
| **Approve** with `WO-2002` selected (seeded `OnHold`) | `WO_STATE_INVALID` | "This work order is on hold — resolve the hold before approving." · `WO_STATE_INVALID · transaction rolled back · nothing persisted` |
| **Approve** with the comment box emptied | `VALIDATION_FAILED` | "An approval comment is required." |

The screen signs in as `ana.ops` (Manager) for tenant `fabrikam`, so the other rows are reached from code
or from a test rather than from a button: `WO_CONCURRENCY` when another writer commits first,
`WO_NUMBER_IN_USE` when `CreateAsync` is given a number the tenant already uses, `PERMISSION_DENIED` when a
Technician calls `ApproveAsync` or `GetAuditAsync` (no `BEGIN TRANSACTION`, no query), `DB_TIMEOUT` when
a command outlives `SessionContext.CommandTimeout`. `WO_NOT_FOUND` cannot be reached from this screen
precisely because the tenant filter is applied on both sides — the grid only ever offers ids that exist in
the tenant — but a stale link, a replayed batch or a WebMethod caller can still ask for one, so the
service checks. All of them go through the same `ErrorMap.Map` switch or `ErrorMap.Rejected`, and
`UNEXPECTED` is what any new, unmapped exception falls into by design.
