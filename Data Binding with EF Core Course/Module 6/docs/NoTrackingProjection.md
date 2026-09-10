# The grid query is a no-tracking projection

Deliverable 2 of the Module 6 lab: "rewrite the grid query as a no-tracking projection: `AsNoTracking`, the
composed `Where` filters, `OrderByDescending(t => t.UpdatedAt)` and a `Select` into `TicketListItem` that
lets the database join the customer, agent and category names, ending in `ToListAsync`."

## The rewrite already happened — in Module 3

`TicketQueryService.SearchTicketsAsync` (via the shared `RunSearchAsync`) has been exactly this since
Module 3: `db.Tickets.AsNoTracking()`, the composed filters, `OrderByDescending(t => t.UpdatedAt)`, `Select`
into `TicketListItem`, `ToListAsync`. Module 6 does not change that method's shape — it adds
`SearchTicketsNaiveAsync` as a **named anti-pattern next to it**, so the rewrite this deliverable asks for is
something the lab can measure against, not just something the reader is told happened once, off screen, in
an earlier module.

```csharp
var query = ApplyFilters(db.Tickets.AsNoTracking(), criteria);   // no-tracking, before any filter runs

var total = await query.CountAsync(token);                       // statement 1

var items = await query
    .OrderByDescending(t => t.UpdatedAt)
    .Skip(pageIndex * pageSize)
    .Take(pageSize)
    .Select(t => new TicketListItem(
        t.Id, t.Number, t.Title,
        t.Customer.Name,                                        // joined by the database
        t.Agent == null ? null : t.Agent.DisplayName,           // LEFT JOIN: a ticket may be unassigned
        t.Category.Name,
        t.Status, t.Priority, t.DueDate, t.UpdatedAt))
    .ToListAsync(token);                                         // statement 2
```

`ApplyFilters` (new in Module 6, factored out of `RunSearchAsync`) is the one place the `Where` clauses are
written; `SearchTicketsNaiveAsync` calls the exact same method on a **tracked** `IQueryable<Ticket>`, so the
two branches are guaranteed to test identical filters — only the loading and shaping strategy after that
point differs, which is the whole point of calling one "naive" and the other "optimised" instead of comparing
two things that also disagree about what a search means.

## Why `AsNoTracking` is correct here, and where it stops being correct

`TicketListItem` is a projection, not an entity — nothing the grid shows can be written back even by
accident, because there is no tracked `Ticket` anywhere in the call chain to write back. That is exactly
right for a read-only grid, a dashboard, a lookup list or an export. It stops being right the moment a screen
is about to call `SaveChangesAsync` against the same instances it displayed — which is why the editor's
`TicketCommandService.SaveAsync` deliberately loads a **fresh, tracked** entity by key inside its own
context instead of reusing anything the browser projected, and why Module 6's own `TicketDetailService.LoadForEditorAsync`
(`docs/RelatedDataDecisions.md`) is tracked on purpose — it is loading a controlled aggregate the editor might
eventually save through, not a read-only row for a grid.

## `AsNoTrackingWithIdentityResolution` — one real use, in `TicketDetailService`

`TicketQueryService.SearchTicketsAsync` does not need `AsNoTrackingWithIdentityResolution`: it projects
straight to a flat `TicketListItem`, so there are no entity instances for identity resolution to deduplicate
in the first place. The one place this solution actually uses it is
`TicketDetailService.LoadRecentCommentsAsync`, whose query does return entities with a shared `Include(c =>
c.Ticket)` back-reference — see `docs/RelatedDataDecisions.md` for the measured effect (one shared `Ticket`
instance across up to five comment rows, instead of five separate clones under plain `AsNoTracking`).

## Evidence

Captured against SQLite in memory (312-ticket seed) and, for the timing comparison, a file-based SQLite
database — both through the exact shipped `TicketQueryService.SearchTicketsAsync`, not a re-implementation.

```
REAL optimised (no filter): statements=2, ms=0.22, rows shown=50, total=312
```
```sql
SELECT COUNT(*) FROM "Tickets" AS "t"
```
```sql
SELECT "t0"."Id", "t0"."Number", "t0"."Title", "c"."Name", "a"."DisplayName", "c0"."Name",
       "t0"."Status", "t0"."Priority", "t0"."DueDate", "t0"."UpdatedAt"
FROM (
    SELECT "t"."Id", "t"."AgentId", "t"."CategoryId", "t"."CustomerId", "t"."DueDate",
           "t"."Number", "t"."Priority", "t"."Status", "t"."Title", "t"."UpdatedAt"
    FROM "Tickets" AS "t"
    ORDER BY "t"."UpdatedAt" DESC
    LIMIT @p1 OFFSET @p
) AS "t0"
INNER JOIN "Customers" AS "c" ON "t0"."CustomerId" = "c"."Id"
LEFT JOIN "Agents" AS "a" ON "t0"."AgentId" = "a"."Id"
INNER JOIN "Categories" AS "c0" ON "t0"."CategoryId" = "c0"."Id"
ORDER BY "t0"."UpdatedAt" DESC
```

Exactly two statements, 0 tracked entities (`SupportDesk.Tests/TicketSearchTests.cs`,
`The_search_tracks_nothing_and_never_returns_an_entity`) — unchanged since Module 3, reconfirmed here next to
the naive branch's 15 statements and 326 tracked entities for the same criteria
(`SupportDesk.Tests/TicketPerformanceTests.cs`).
