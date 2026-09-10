# Paging with a page size of 50 and indexed filters

Deliverable 3 of the Module 6 lab: "add paging with a page size of 50: a `CountAsync` on the filtered query
for the total, then `Skip` and `Take` before the projection, returned together as a `PagedResult<TicketListItem>`",
and "make every filter index-friendly."

## Paging lives in SQL for one branch and nowhere for the other

`TicketQueryService.SearchTicketsAsync` has paged in the database since Module 3 —
`Skip(pageIndex * pageSize).Take(pageSize)` composed on the query, before `ToListAsync`, so the database
returns at most one page of rows. `NaivePageCap` (a Module 6 constant, `= 50`) is deliberately **not** the
same kind of thing: `SearchTicketsNaiveAsync` sends **no** `Skip`/`Take` to the database at all — it loads
every matching `Ticket`, tracked, and only *then* takes the first 50 of the in-memory list. Naming the naive
branch's cap `NaivePageCap` and giving it the same value as `PageSize` is intentional: it lets the two
branches show a comparable "one screen" of rows, while the SQL each one sends is completely different — one
page cut in the database, one page cut after the whole table already travelled over the wire.

```csharp
// optimised — the database pages
var items = await query.OrderByDescending(t => t.UpdatedAt).Skip(pageIndex * pageSize).Take(pageSize)...

// naive — nothing is paged in SQL; the cap is an in-memory Take after the unbounded load
var all = await query.OrderByDescending(t => t.UpdatedAt).ToListAsync(token);
var page = all.Take(NaivePageCap).ToList();
```

## Which filters and sorts are index-friendly

Unchanged since Module 2/3, and shared by both branches through `TicketQueryService.ApplyFilters` (factored
out in Module 6 so the naive and optimised branches test identical filters):

| Filter / sort | SQL shape | Index |
|---|---|---|
| `t.Number.StartsWith(text)` | `LIKE 'prefix%'` | **seekable** — `IX_Tickets_Number` |
| `t.Title.Contains(text)` / `t.Customer.Name.Contains(text)` | `instr(...) > 0` | scan (the price of a free-text box) |
| `Status` | `= @status` | `IX_Tickets_Status_DueDate` |
| `CustomerId` | `= @customerId` | `IX_Tickets_CustomerId` |
| `DueDate` range | `>= @dueFrom AND <= @dueTo` | `IX_Tickets_Status_DueDate` (leading `DueDate` use depends on whether `Status` is also set) |
| `ORDER BY UpdatedAt DESC` | sort key | `IX_Tickets_UpdatedAt` |

No index was added in Module 6: the log (`docs/LoggingInDevelopment.md`, `docs/BeforeAfterMeasurements.md`)
never showed a frequent filter scanning a large table without one — the only thing it showed was the naive
branch's statement count, which an index cannot fix (the problem is *what* is loaded and *how many round
trips* it costs, not *how fast the database finds rows*). This is itself one of the lab's review-question
answers: not every measured problem is an indexing problem, and the log is what tells you which one you have.

## Evidence

Same 312-ticket seed as `docs/PagingInTheDatabase.md` (Module 3), reconfirmed against the actual Module 6
code:

- Empty criteria, page 1: optimised branch → `total=312 rows=50 pages=7 statements=2`; naive branch →
  `totalMatching=312 rows shown=50 (capped) statements=15` — the naive branch's `TotalMatching` is the size of
  the whole in-memory list, not a separate `COUNT` statement, which is itself part of the anti-pattern: the
  number the UI shows costs nothing extra to compute because the database has already been made to hand over
  everything.
- `SupportDesk.Tests/TicketPerformanceTests.cs`,
  `The_naive_branch_loads_every_matching_ticket_tracked_and_caps_the_shaped_rows` — `TotalMatching == 312`,
  `Items.Count == NaivePageCap (50)`.
- `SupportDesk.Tests/TicketPerformanceTests.cs`,
  `Naive_and_optimised_return_the_same_rows_in_the_same_order_for_the_same_criteria` — the two branches agree
  row for row on `Number`/`CustomerName`/`AgentName`/`CategoryName` for page 1 of the same criteria; only the
  loading strategy differs, never the result.
- `SupportDesk.Tests/TicketSearchTests.cs` (Module 3, still green): every filter/paging test — total and page,
  ordering, no-ties, last page, past-the-end, per-filter counts cross-checked against an independent
  `CountAsync`.
