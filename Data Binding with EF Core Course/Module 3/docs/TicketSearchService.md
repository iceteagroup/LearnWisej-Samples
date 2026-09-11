# `TicketListItem`, `TicketSearchCriteria` and `SearchTicketsAsync`

Deliverable 1 of the Module 3 lab: the records the browser moves around, and the one service method that
turns a set of filters into a page of rows.

All three types live in `SupportDesk.Services/TicketBrowsing.cs`; the method is in
`SupportDesk.Services/TicketQueryService.cs`. Nothing here knows that a grid exists.

## The three records

```csharp
public sealed record TicketListItem(
    int Id, string Number, string Title, string CustomerName, string? AgentName,
    string CategoryName, string Status, string Priority, DateTime? DueDate, DateTime UpdatedAt);

public sealed record TicketSearchCriteria
{
    public string? Text { get; init; }
    public string? Status { get; init; }
    public int? CustomerId { get; init; }
    public DateTime? DueFrom { get; init; }
    public DateTime? DueTo { get; init; }
    public int PageIndex { get; init; }
    public int PageSize { get; init; } = 50;
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);
```

`TicketListItem` is a **projection**, not an entity, and the difference is the whole point of the module:

| | `Ticket` (the entity) | `TicketListItem` (the projection) |
|---|---|---|
| columns | 14, including `Description` (4000 chars) and `RowVersion` | the 10 the browser's list needs |
| related data | `Customer`, `Agent`, `Category` objects and a `Comments` collection | three joined **names** and nothing else |
| change tracker | one snapshot per row, per session | nothing — the query is `AsNoTracking` |
| after the context is disposed | a detached graph nobody should save | still a perfectly valid list |

The grid needs a customer's *name*; it does not need a `Customer`. Loading the entity to reach one string
is entity flooding: wire bytes, session memory and a tracker snapshot per row, multiplied by the number of
open browsers — and a grid full of tracked entities looks editable, so a stray `SaveChangesAsync` becomes a
data-loss bug rather than a compile error.

`TicketSearchCriteria` uses `init` properties rather than a positional constructor because most of it is
optional: `new TicketSearchCriteria { PageSize = 50 }` is a valid "everything, first page". `Describe()`
renders only the filters that are set; the service writes it into the `QueryTrace` note the tests
record.

`PagedResult<T>` carries `TotalCount` alongside the page because the operator needs *"50 of 312"*, and 312
cannot be derived from 50 rows. `PageCount(pageSize)` never returns 0, so an empty result still reads
"page 1 of 1".

## The method

```csharp
public async Task<PagedResult<TicketListItem>> SearchTicketsAsync(TicketSearchCriteria criteria, CancellationToken token = default)
{
    await using var db = await _dbFactory.CreateDbContextAsync(token);
    return await RunSearchAsync(db, criteria, token);
}
```

One context from the factory, created when the method starts and disposed before it returns — the same
rule as Module 1's count. The page never sees a `DbContext`, an `IQueryable` or an entity; it receives a
`PagedResult<TicketListItem>` and hands the list to a `BindingSource`.

`RunSearchAsync` is the composed query, in order:

1. `db.Tickets.AsNoTracking()` — a recipe. Nothing has been sent.
2. one `Where` per filter **that is set**;
3. `await query.CountAsync(token)` — statement one, the total *before* the page is cut;
4. `OrderByDescending(t => t.UpdatedAt).Skip(...).Take(...).Select(...)` then
   `await …ToListAsync(token)` — statement two, the page.

The filters are covered in [PagingInTheDatabase.md](PagingInTheDatabase.md); the binding side in
[TicketBrowserBinding.md](TicketBrowserBinding.md).

## Why the projection is written inside the `Select`

```csharp
.Select(t => new TicketListItem(
    t.Id, t.Number, t.Title,
    t.Customer.Name,                                // INNER JOIN "Customers"
    t.Agent == null ? null : t.Agent.DisplayName,   // LEFT JOIN "Agents" — a ticket may be unassigned
    t.Category.Name,                                // INNER JOIN "Categories"
    t.Status, t.Priority, t.DueDate, t.UpdatedAt))
```

Navigation properties are still usable inside a projection: EF Core turns `t.Customer.Name` into a join and
selects one column, instead of materialising a `Customer`. `t.Agent == null ? null : t.Agent.DisplayName`
is what makes the agent join a `LEFT JOIN`; without the null check an unassigned ticket would disappear
from the grid, which is exactly the kind of bug a "the list is shorter than the count" report starts with.

## Evidence

**The SQL the projection produces** (`ToQueryString()` on the page query, page 2 of 50, captured in a
console check against SQLite in memory):

```sql
SELECT "t0"."Id", "t0"."Number", "t0"."Title", "c"."Name", "a"."DisplayName", "c0"."Name",
       "t0"."Status", "t0"."Priority", "t0"."DueDate", "t0"."UpdatedAt"
FROM (
    SELECT "t"."Id", "t"."AgentId", "t"."CategoryId", "t"."CustomerId", "t"."DueDate",
           "t"."Number", "t"."Priority", "t"."Status", "t"."Title", "t"."UpdatedAt"
    FROM "Tickets" AS "t"
    ORDER BY "t"."UpdatedAt" DESC
    LIMIT @p OFFSET @p
) AS "t0"
INNER JOIN "Customers" AS "c" ON "t0"."CustomerId" = "c"."Id"
LEFT JOIN "Agents" AS "a" ON "t0"."AgentId" = "a"."Id"
INNER JOIN "Categories" AS "c0" ON "t0"."CategoryId" = "c0"."Id"
ORDER BY "t0"."UpdatedAt" DESC
```

Ten columns, three joins, `LEFT JOIN` on the agent, and no `Description`, `RowVersion`, `CreatedAt`,
`IsUrgent` or comment anywhere. Note that SQLite pages the **ticket** rows in the subquery and joins the
names afterwards — the joins never widen the page.

**One search from the page:** `statusLabel` reads *Showing 50 of 312 tickets · page 1 of 7 · page size
50*. Where `appsettings.Development.json` logs `Microsoft.EntityFrameworkCore.Database.Command` at
`Information` (Modules 3 to 6), the server console shows the two statements of every search: the
`SELECT COUNT(*)` and the paged `SELECT … LIMIT … OFFSET …`. Nothing is tracked, which is `AsNoTracking`
doing its job (`The_search_tracks_nothing_and_never_returns_an_entity` below).

**Tests** (`SupportDesk.Tests/TicketSearchTests.cs`, all against the real 312-ticket seed):

- `Empty_criteria_return_one_page_and_the_total_of_every_ticket` — 312 total, 50 rows, 7 pages.
- `The_projection_carries_the_names_the_database_joined` — the customer, category and agent names on the
  first row match what a separate query reads from the entities.
- `An_unassigned_ticket_comes_back_with_a_null_agent_name` — all seven pages together contain 312 distinct
  tickets, some with a null `AgentName` and some with a null `DueDate`: the `LEFT JOIN` drops nothing.
- `The_search_tracks_nothing_and_never_returns_an_entity` — a context opened alongside the search has the
  same number of tracked entries before and after it.
- `A_search_sends_exactly_two_statements_a_COUNT_and_a_paged_SELECT` — two commands, one context created
  and disposed.
