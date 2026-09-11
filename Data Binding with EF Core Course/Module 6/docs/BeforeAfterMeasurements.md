# Before / after: the measured numbers

Deliverable 5 of the Module 6 lab: "lab notes documenting the measured before-and-after improvement." Every
number below came from a throwaway console project (`SupportDesk.Data` + `SupportDesk.Services`, no
Wisej.NET, no browser) calling the exact shipped `TicketQueryService`/`TicketDetailService` methods against
SQLite — in memory for statement counts and SQL text, a real file (`Data Source=<temp file>`, not
`:memory:`) for timing, since the reviewer's own database is a file
(`SupportDesk.Web/App_Data/supportdesk.db`) and in-memory SQLite is fast enough to make every millisecond
noise. Nothing here was written from memory or estimated.

## The headline table — no filter, page 1, 50 rows

| | naive (`SearchTicketsNaiveAsync`) | optimised (`SearchTicketsAsync`) | × |
|---|---:|---:|---:|
| **Statements** | 15 | 2 | **7.5×** |
| **Tracked entities held** | 326 | 0 | — |
| **Rows shown** | 50 (capped in memory) | 50 (paged in SQL) | — |
| **Rows matching** | 312 (the whole table — no SQL paging) | 312 (from a separate `COUNT`) | — |
| **Database command time** (SQLite in memory, `QueryTrace` scope) | 0.49 ms | 0.22 ms | 2.2× |
| **Wall-clock time** (SQLite file, 5 warmed runs, averaged) | 18.79 ms | 0.92 ms | **20.4×** |

The statement-count and wall-clock improvement factors tell different, both true, stories. Fifteen versus two
is a 7.5× reduction in **round trips**, which is what actually drives the 20.4× wall-clock difference — each
of the thirteen extra naive-branch statements pays a full SQLite round trip (parse, execute, marshal a
result), and that per-statement overhead, not raw query cost, is most of what the naive branch is paying for.
The in-memory command-time column (0.49 ms vs 0.22 ms) barely shows this, because `:memory:` SQLite has
almost no I/O cost per round trip — which is exactly why the file-based timing run exists: it is the number
that resembles what the running app, and the reviewer's browser, will actually see.

## Why 15 statements, not the lesson's "151"

The lesson and the walkthrough video quote 151 statements (`1 + 3 × 50`) for one screen of 50 tickets. This
solution's own seed measures 15. Both numbers are correct; they describe different lookup cardinalities. The
full explanation, with the EF Core mechanism (automatic reference fix-up) verified directly, is in
`docs/RelatedDataDecisions.md`. The short version, repeated here because it belongs in "the measured
before-and-after" as much as in "the related-data decisions":

- This seed has only 5 customers, 3 agents, 6 categories behind 312 tickets — the naive branch's 50-row page
  touches at most 14 distinct related rows in total, so only 14 (not 150) of its per-row loads cost a real
  statement; the rest are served from the tracked graph for free.
- Measured separately, with every per-row lookup forced through an independent query instead of
  `Entry(...).Reference(...).LoadAsync()` (i.e. with the fix-up shortcut deliberately bypassed, the way a
  support desk with thousands of distinct customers effectively would experience it): **131** statements for
  the same 50 rows — close to the lesson's 151, and the number that matters once this code runs against real
  production data instead of a five-customer demo seed.

| | fix-up benefits (as shipped) | fix-up bypassed (what scale would look like) |
|---|---:|---:|
| Statements for 50 rows | 15 | 131 |

## Filtered to one customer — fewer distinct lookups, fewer statements

```
naive, CustomerId = <first seeded customer>: rows matching > 0, statements ≤ 1 + 1 + 3 + 6
```

Filtering the naive branch to one customer removes the customer-lookup variance entirely (every row shares
the same, already-tracked customer after the first) without changing the shape of the anti-pattern — still no
SQL paging, still every matching row loaded tracked. `SupportDesk.Tests/TicketPerformanceTests.cs`,
`The_naive_branch_filtered_to_one_customer_still_returns_only_that_customer`, asserts this bound and that
`ApplyFilters` (shared between both branches) really did reach the naive branch's `Where`.

## The naive branch's SQL, captured

```sql
SELECT "t"."Id", "t"."AgentId", "t"."CategoryId", "t"."CreatedAt", "t"."CustomerId", "t"."Description",
       "t"."DueDate", "t"."IsUrgent", "t"."Number", "t"."Priority", "t"."RowVersion", "t"."Status",
       "t"."Title", "t"."UpdatedAt"
FROM "Tickets" AS "t"
ORDER BY "t"."UpdatedAt" DESC
```
```sql
SELECT "c"."Id", "c"."Email", "c"."Name" FROM "Customers" AS "c" WHERE "c"."Id" = @p LIMIT 1
```
```sql
SELECT "a"."Id", "a"."DisplayName", "a"."Email" FROM "Agents" AS "a" WHERE "a"."Id" = @p LIMIT 1
```
```sql
SELECT "c"."Id", "c"."Name" FROM "Categories" AS "c" WHERE "c"."Id" = @p LIMIT 1
```
… twelve more `Customers`/`Agents`/`Categories` lookups (one per distinct value the capped 50 rows had not
already seen), 15 statements total. Note the first statement: the whole `Tickets` table, every column,
**no `WHERE`, no `LIMIT`** — this is what "no SQL-level paging" actually looks like on the wire.

## The optimised branch's SQL, captured

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

Both statements carry a `LIMIT`/`OFFSET` or aggregate over the **filtered**, not the whole, table — the
database does the paging, the filtering and the three joins in two round trips, no matter how many tickets
exist.

## The related-data methods, measured the same way

| Method | Strategy | Statements |
|---|---|---:|
| `TicketDetailService.LoadForEditorAsync` | `Include(Customer).Include(Category)`, tracked | 1 |
| `TicketDetailService.LoadRecentCommentsAsync` | filtered read (last 5), no-tracking, identity resolution | 1 |
| `TicketDetailService.LoadAllCommentsAsync` | tracked ticket read + explicit `Collection(...).LoadAsync()` | 2 (1 extra) |

Full remarks and the SQL for each in `docs/RelatedDataDecisions.md`.

## How to reproduce these numbers

A minimal repro (not part of the shipped solution — the pattern used to produce every number on this page):

```csharp
var factory = /* IDbContextFactory<SupportDeskContext> over a real SQLite connection */;
await new DevelopmentSeeder(factory).SeedDevelopmentDataAsync();          // the real 312-ticket seed
var queries = new TicketQueryService(factory);

using var scope = QueryTrace.Begin(_ => { });
var naive = await queries.SearchTicketsNaiveAsync(new TicketSearchCriteria());
// scope.Commands == 15, naive.TrackedEntities == 326, naive.TotalMatching == 312
```

## Verified / unverified

Verified by `dotnet build`/`dotnet test` and the console checks quoted above, against SQLite (in memory for
statement counts and SQL text, a real file for timing) with the exact shipped code — not a re-implementation,
not remembered numbers. **Not verified**: the numbers a real browser session, the Development host's own
`[EF]`-prefixed console log, and a real `App_Data/supportdesk.db` file under the actual page's load would
show — the application was not started for this report; see the README's "Verified / unverified" section for
the full list.
