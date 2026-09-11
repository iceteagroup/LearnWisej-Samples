# Transactions and retries

## Why one `SaveChangesAsync` is already a transaction

`TicketCommandService.SaveAsync` and `DeleteAsync` each call `SaveChangesAsync` exactly once, after making
every change the method intends (mapping every field, or one `Remove`). EF Core wraps a single
`SaveChangesAsync` call in a database transaction automatically: every INSERT/UPDATE/DELETE it generates for
that call either all commit or all roll back together. Nothing in this module needs an explicit
`BeginTransactionAsync` for those methods, and adding one would be redundant ceremony around something EF
Core already guarantees.

## When an explicit transaction is needed

An explicit transaction is for a unit of work that spans **more than one** `SaveChangesAsync` call (or a
`SaveChangesAsync` plus raw SQL, or two contexts sharing one connection). A typical Support Desk example is
"close a ticket with a closing comment" done as two separate writes: the ticket's status, then a new
`TicketComment`. If the second write fails, the first must not stay applied on its own. The shape:

```csharp
await using var db = await _dbFactory.CreateDbContextAsync(token);
await using var tx = await db.Database.BeginTransactionAsync(token);
try
{
    ticket.Status = TicketStatuses.Closed;
    await db.SaveChangesAsync(token);                      // write 1

    db.TicketComments.Add(closingComment);
    await db.SaveChangesAsync(token);                      // write 2

    await tx.CommitAsync(token);
}
catch (Exception)
{
    await tx.RollbackAsync(token);
    throw;
}
```

This is the shape a multi-step business operation takes in production: update a status, then a second step
fails (a second table's constraint is violated, a downstream call is unreachable) and the transaction is what
stops the first write from being left half-applied. This solution ships no such operation: every write it
performs is one `SaveChangesAsync`, which is why the snippet above is illustration, not code from the
solution. When the two changes can be staged on one context and saved together, one `SaveChangesAsync` is
simpler and gives the same guarantee.

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
manual loop around a unit that is not safe to replay would be worse than no retry at all (in the two-write
example above, a retried first write would set `Status = Closed` again, which is harmless, but a retried
second write could insert the closing comment twice if the failure happened after the INSERT reached the
database but before the confirmation came back). `SupportDeskDataServiceCollectionExtensions.AddSupportDeskData`
does not call `EnableRetryOnFailure`, and none of this module's connection handling pretends otherwise — the
local SQLite file either opens or it does not, and a failure is caught by the handler, shown as one friendly
sentence and written to the server console. There is no transient-failure class to retry against the way
there is against a networked server.

## Evidence

- `TicketCommandService.cs` — `SaveAsync` and `DeleteAsync` each end in exactly one `SaveChangesAsync`
  call; neither begins a transaction of its own.
- `SupportDeskDataServiceCollectionExtensions.AddSupportDeskData` — `UseSqlite(connectionString)` with no
  `EnableRetryOnFailure` and no custom execution strategy.
- `ModelRulesTests.Restrict_deleting_a_customer_with_tickets_throws_DbUpdateException_and_changes_nothing` —
  a `SaveChangesAsync` the database refuses leaves nothing behind: the five customers are still there.
