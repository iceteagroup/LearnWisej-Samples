# Deliverable 2 — DbContext lifetime decision

**Decision.** One `EnterpriseOpsDbContext` per operation — per command, per query, per audit write.
It is created when the operation starts and disposed when it ends. It is never a field on a page, never
stored in `Application.Session`, never static, and never shared with a background task.

**Status.** Accepted for the EnterpriseOps Command Center. Review when the sample gains a real database
server (the same decision holds; only the factory changes).

## Context

A Wisej.NET session is long-lived: a page instance can exist for hours while a user works. That makes a
form field the most tempting place to keep a `DbContext`, and the worst one.

A `DbContext` is a unit of work with a change tracker. Its correct lifetime is the lifetime of the work,
not the lifetime of the window in front of it.

## Options considered

| Option | Verdict |
|---|---|
| One context per **session**, held in a page field | **Rejected.** Tracked entities accumulate for the whole session; a tracked entity is returned from the identity map with its old values even when the query runs again, so the screen serves stale data; the context is not thread-safe, so no background job or `Application.StartTask` may touch it; a failed `SaveChanges` leaves it in a state that is hard to recover. |
| One context per **screen** | **Rejected.** Same failure modes, smaller blast radius. A screen is still open far longer than an operation lasts. |
| One context per **operation** | **Accepted.** The change tracker only ever holds the one aggregate the command loaded. Disposal is deterministic. Two operations cannot corrupt each other's state. A command can be run by a batch job or a WebMethod with no changes. |
| One context per **request**, from a DI container | Correct in ASP.NET MVC, meaningless here: a Wisej.NET session is not a request, and a user's click is not the unit of work. The per-operation rule is the same idea, scoped to the thing that actually is a unit of work. |

## What is session-long, and why that is not a contradiction

`Data/SessionDatabase.cs` keeps **one open `SqliteConnection`** for the life of the session:

```csharp
_connection = new SqliteConnection("DataSource=:memory:");
_connection.Open();                       // closing it would delete the database
_options = new DbContextOptionsBuilder<EnterpriseOpsDbContext>().UseSqlite(_connection).Options;
```

An in-memory SQLite database exists only while a connection to it is open, so the connection is the
sample's stand-in for "the database server". A pooled connection to SQL Server would live just as long.
The **connection** is infrastructure; the **context** is a unit of work. Only the second one is scoped
to the operation.

Because that connection is a single physical connection, `SessionDatabase.Gate` (a `SemaphoreSlim(1,1)`)
serialises database work inside a session: a `SqliteConnection` is not safe for concurrent use, and an
awaited handler could otherwise overlap another click.

## How the decision is enforced in code

```csharp
var dbContext = _database.CreateContext($"{operation} command");   // WorkOrderCommandService.RunAsync
try
{
    var repository = new WorkOrderRepository(dbContext, _trace);
    var transaction = await dbContext.Database.BeginTransactionAsync(ct);
    …
}
finally
{
    _database.Release(dbContext, …);          // disposes it and logs the tracked-entity count
}
```

* Nothing outside `RunAsync` (or one query method) ever holds the reference.
* `SessionDatabase` counts contexts still alive and logs the count on every disposal.
  **Between operations it must read 0.** That is the decision as a number.
* Queries use `AsNoTracking()`, so even during a query the change tracker stays empty.

## Evidence in the running app

Every operation writes its context's lifetime to the server log (`System.Diagnostics.Trace`, e.g. the
debugger's Output window): `Data: DbContext #n created (short-lived · Approve command)` is always followed,
within the same operation, by `Data: DbContext #n disposed (k tracked entities released) · live contexts: 0`.
Search, Approve and Audit log each produce exactly one such pair (a rejected command adds a second pair for
its rejection audit).
