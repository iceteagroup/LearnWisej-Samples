# The query trace — `Tickets/Search`

Before = the app in `Module 5`. After = this folder. Same dataset, same filter (Status *Open*), same
warm-up.

## Statements, in order, beside the UI action that produced them

**Before**

| # | UI action | Statement |
|---|---|---|
| 1 | click **Search tickets** | `SELECT COUNT(*) FROM Tickets WHERE Status = 'Open'` |
| 2 | the grid asks for its first block of rows | `SELECT … FROM Tickets WHERE Status = 'Open' ORDER BY UpdatedAt DESC, Id LIMIT 200 OFFSET 0` — **every column**, including `Description` and `Notes` |
| 3 … 202 | building each of the 200 rows | `SELECT … FROM Customers WHERE Id = @id` — **one per row** |

**After**

| # | UI action | Statement |
|---|---|---|
| 1 | click **Search tickets** | `SELECT COUNT(*) FROM Tickets WHERE Status = 'Open'` |
| 2 | the grid asks for its first block of rows | one `SELECT t.Id, t.Number, c.Name, t.Status, t.Priority, t.CreatedAt, t.UpdatedAt FROM Tickets t INNER JOIN Customers c ON … WHERE t.Status = 'Open' ORDER BY t.UpdatedAt DESC, t.Id LIMIT 200 OFFSET 0` |

| | Before | After |
|---|---:|---:|
| Statements per page of 200 rows | **201** | **1** |
| Statements for the click | 3 | 3 |
| `Tickets/Search` click | 125 ms | **66 ms** |
| Columns fetched per row | all 12, including two long text columns | **7** |
| Change tracking | on | `AsNoTracking` |
| Statements to fill the first screen | 201 | **1** |

The whole N+1 came from one thing the entity could not do: the grid needs a customer **name** and the
ticket carries a customer **id**. Closing that gap per row is invisible in the code and unmissable in a
Database trace.

## The index

The search filters on `Status` and orders by `UpdatedAt`. Until this module there was no index on
either, so every page was a scan of 50,000 rows to return 200.

```csharp
b.HasIndex(t => new { t.Status, t.UpdatedAt }).HasDatabaseName("IX_Tickets_Status_UpdatedAt");
```

The order matters: the equality column first, the ordering column second, so the same index satisfies
the `WHERE` and the `ORDER BY` and the paging can walk it. **Verify the filter and join columns are
indexed before concluding that the remaining query time is inherent** — an unindexed 44 ms count is not
evidence that counting is expensive.

One sample-specific wrinkle: this app creates its schema with `EnsureCreated`, which does nothing to a
database file that already exists, so an index added in a later module would never reach an existing
`App_Data/perflab.db`. `PerfLabDatabase.EnsureReadyAsync` therefore issues
`CREATE INDEX IF NOT EXISTS …` explicitly. A real application adds a migration; a sample that silently
did nothing would be worse than a sample that explains itself.

## Collecting the trace yourself

1. Release, warm, one browser tab.
2. **Alt+F2 → Database → Start**, click **Search tickets** once, **Stop**.
3. The report lists every statement with its duration. Match them to the UI actions as in the table
   above — and count them. The number to write down is not the slowest query, it is **how many**.
4. Sort by duration and look at the slowest. If it is the count, check the index. If it is the page
   query, look at the columns it selects before you look at anything else.
5. Re-run on the identical scenario after the change and record the new count and the new duration.

The same trace against SQL Server will look worse for the before and better for the after: every one of
those 201 statements is a network round-trip there, and one statement is one round-trip. The statement
count is a property of your code; the cost per statement is a property of the environment.

## What is left

`Tickets/Search` is 66 ms with a 300 ms budget, and 44 ms of that is the count query. Counting 11,907
matching rows out of 50,000 is honest work; if it ever needs to be cheaper, the next step is to stop
asking for an exact count on every search — not to tune the statement further.
