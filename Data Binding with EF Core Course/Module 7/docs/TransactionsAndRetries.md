# Transactions and retries

## Why one `SaveChangesAsync` is already a transaction

`TicketCommandService.SaveAsync` and `DeleteAsync` each call `SaveChangesAsync` exactly once, after making
every change the method intends (mapping every field, or one `Remove`). EF Core wraps a single
`SaveChangesAsync` call in a database transaction automatically: every INSERT/UPDATE/DELETE it generates for
that call either all commit or all roll back together. Nothing in this module needed an explicit
`BeginTransactionAsync` for those methods, and adding one would have been redundant ceremony around
something EF Core already guarantees.

## Why `CloseTicketWithCommentAsync` needed an explicit one

`CloseTicketWithCommentAsync` is different on purpose: it performs **two** separate writes — the ticket's
status, then a closing `TicketComment` — as **two separate `SaveChangesAsync` calls**, so the lab's failure
switch (`TransactionFailureSwitch.FailAfterFirstWrite`) can fail the unit *between* them and prove the first
write alone rolls back. That needs an explicit transaction spanning both calls:

```csharp
await using var tx = await db.Database.BeginTransactionAsync(token);
try
{
    ticket.Status = TicketStatuses.Closed;
    await db.SaveChangesAsync(token);                      // write 1

    if (_transactionFailure.FailAfterFirstWrite)
    {
        _transactionFailure.FailAfterFirstWrite = false;   // fires once
        throw new SimulatedTransactionFailureException();
    }

    db.TicketComments.Add(ticketComment);
    await db.SaveChangesAsync(token);                      // write 2

    await tx.CommitAsync(token);
}
catch (Exception)
{
    await tx.RollbackAsync(token);
    throw;
}
```

A real, non-simulated version of "the second half threw" is exactly the shape a multi-step business
operation takes in production: update a status, then a downstream call fails (a queue is unreachable, a
second table's constraint is violated, a network hiccup) — the transaction is what stops the first write
from being left half-applied. The switch resets itself the moment it fires, so the next click — with or
without re-arming it — runs the ordinary, successful path.

## `EnableRetryOnFailure` and execution strategies — why SQLite has none

On SQL Server, `options.EnableRetryOnFailure()` wraps every database operation in an **execution strategy**
that retries transient failures (a dropped connection, a failover) automatically. An execution strategy
that retries is incompatible with a transaction begun outside it — a retried operation might replay a
transaction that already partially committed — so EF Core refuses to let you call `BeginTransactionAsync`
directly once retries are enabled; the whole unit, `BeginTransactionAsync` through `CommitAsync`, has to be
handed to `db.Database.CreateExecutionStrategy().ExecuteAsync(...)` so a retry replays the complete thing:

```csharp
// SQL Server shape — NOT used in this module, shown for the docs only.
var strategy = db.Database.CreateExecutionStrategy();
await strategy.ExecuteAsync(async () =>
{
    await using var tx = await db.Database.BeginTransactionAsync();
    // ... both writes ...
    await tx.CommitAsync();
});
```

**SQLite has no retrying execution strategy.** `Microsoft.EntityFrameworkCore.Sqlite` does not ship one, and
this module does not add a custom one, or fake the behaviour by wrapping calls in a manual retry loop — a
manual loop around a transaction that is not idempotent-safe would be worse than no retry at all
(`CloseTicketWithCommentAsync`'s two `SaveChangesAsync` calls are not automatically safe to replay: a retried
first write, for instance, would try to set `Status = Closed` again, which is harmless, but a retried second
write could insert the closing comment twice if the failure happened after the INSERT reached the server but
before the confirmation came back). `SupportDeskDataServiceCollectionExtensions.AddSupportDeskData` does not
call `EnableRetryOnFailure`, and none of this module's connection handling pretends otherwise — the local
SQLite file either opens or it does not (see `DevelopmentOutageSwitch`'s "Break the database" demo), and
there is no transient-failure class to retry against the way there is against a networked server.

## Evidence

- `SupportDesk.Tests/ConcurrencyAndTransactionsTests.cs`:
  - `CloseTicketWithCommentAsync_commits_both_writes_together` — the ordinary path: status Closed, the
    comment present, both survive.
  - `CloseTicketWithCommentAsync_rolls_back_the_status_write_when_the_switch_fails_after_it` — the status is
    unchanged, the comment was never written, and the switch reset itself.
  - `Prints_the_trace_for_the_transaction_rollback` — the verbatim trace:

    ```
    Note: CloseTicketWithCommentAsync(#1): BeginTransactionAsync — status update and comment insert share one transaction
    Command: UPDATE "Tickets" SET "RowVersion" = @p0, "Status" = @p1, "UpdatedAt" = @p2 WHERE "Id" = @p3 AND "RowVersion" = @p4 RETURNING 1;
    Note: CloseTicketWithCommentAsync(#1 / SD-1001): write 1 of 2 done — Status = Closed, SaveChangesAsync
    Note: CloseTicketWithCommentAsync: TransactionFailureSwitch armed — throwing before the comment is written
    Note: CloseTicketWithCommentAsync(#1): transaction rolled back — the comment was not written and the status is unchanged
    ```

    Note there is only **one** `UPDATE` in the trace and **no** `INSERT INTO "TicketComments"` at all — the
    comment write was never attempted, exactly as the rollback promises (the ticket's row-level `UPDATE`
    itself did commit to the connection at the SQL level before the rollback, but `ROLLBACK` at the
    transaction level undoes it along with everything else in the unit — the test's own re-read of the row
    after the exception confirms the status is back to its original value).
