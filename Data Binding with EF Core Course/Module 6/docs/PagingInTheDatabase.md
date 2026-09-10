# Filtering and paging composed in the query

Deliverable 3 of the Module 3 lab: the filters, the ordering and the page are part of the SQL, so the
database does the work and the server never holds more than fifty rows per session.

## Compose, then execute once

`SupportDesk.Services/TicketQueryService.RunSearchAsync`:

```csharp
var query = db.Tickets.AsNoTracking();                       // a recipe — nothing sent

if (!string.IsNullOrWhiteSpace(criteria.Text))
{
    var text = criteria.Text.Trim();
    query = query.Where(t => t.Number.StartsWith(text) || t.Title.Contains(text) || t.Customer.Name.Contains(text));
}
if (!string.IsNullOrWhiteSpace(criteria.Status)) { var status = criteria.Status; query = query.Where(t => t.Status == status); }
if (criteria.CustomerId is int customerId)        query = query.Where(t => t.CustomerId == customerId);
if (criteria.DueFrom is DateTime dueFrom)         query = query.Where(t => t.DueDate != null && t.DueDate >= dueFrom);
if (criteria.DueTo   is DateTime dueTo)           query = query.Where(t => t.DueDate != null && t.DueDate <= dueTo);

var total = await query.CountAsync(token);                   // statement 1

var items = await query
    .OrderByDescending(t => t.UpdatedAt)
    .Skip(pageIndex * pageSize)
    .Take(pageSize)
    .Select(t => new TicketListItem(…))
    .ToListAsync(token);                                     // statement 2
```

Two rules produce everything below.

**One `Where` per filter that is set.** An unset filter is not a clause that matches everything — it does
not exist. `criteria.CustomerId is int customerId` both tests for null and unwraps the value, and the local
`customerId` is what the expression tree captures, so EF Core parameterises it (`@customerId`) instead of
baking a literal into the SQL and defeating the plan cache.

**`await` last.** `CountAsync` and `ToListAsync` are the only lines that send SQL. Everything above them
extends the description. The mistake this replaces is
`db.Tickets.ToList().Where(...).Skip(...).Take(...)`: 312 rows today, three hundred thousand next year,
all of them over the wire and into server memory before the first filter runs.

## Why the count comes first, and separately

The status label needs *"50 of 312"*, and 312 cannot be derived from 50 rows. `CountAsync` runs on the
**filtered** query but before `Skip`/`Take`, so both statements carry the same `WHERE` and the total always
matches the page. It is a second round trip, and it is the cheapest way to get a number the operator
believes.

## Which half of the text search an index can help

```csharp
query.Where(t => t.Number.StartsWith(text) || t.Title.Contains(text) || t.Customer.Name.Contains(text))
```

| Fragment | SQL (SQLite) | Index |
|---|---|---|
| `t.Number.StartsWith(text)` | `"t"."Number" LIKE @text_startswith ESCAPE '\'` with `'printer%'` | **seekable** — `IX_Tickets_Number` (the unique index from Module 2) can range-scan a prefix |
| `t.Title.Contains(text)` | `instr("t"."Title", @text) > 0` | scan; `LIKE '%…%'` cannot use a B-tree |
| `t.Customer.Name.Contains(text)` | `instr("c"."Name", @text) > 0` after an `INNER JOIN` | scan of a five-row table — free here, not free at fifty thousand |

That is why the number is matched by **prefix** and not by `Contains`: an operator who types `SD-1042` gets
an index seek, and the two `Contains` are the price of a free-text box over 312 rows. On a real ticket
table the answer at scale is full-text search or a dedicated search index, not a wider `LIKE`.

The other filters land on the indexes Module 2 created for exactly this screen:

| Filter | Index |
|---|---|
| `Status` (and `Status` + due range) | `IX_Tickets_Status_DueDate` |
| `CustomerId` | `IX_Tickets_CustomerId` |
| `ORDER BY UpdatedAt DESC` | `IX_Tickets_UpdatedAt` |
| the customer lookup's `ORDER BY Name` | `IX_Customers_Name` |

## `Skip`/`Take` on SQLite: `LIMIT` and `OFFSET`

EF Core renders `Skip(50).Take(50)` as **`LIMIT @p OFFSET @p`** on SQLite, not the
`OFFSET … ROWS FETCH NEXT … ROWS ONLY` that SQL Server produces. Both are parameterised — the page number
never becomes a literal, so one cached plan serves every page.

`OFFSET` paging has a known cost: the database still walks the skipped rows, so page 500 is slower than
page 1. With an index on the sort key and a page size of fifty it is the right trade for a browsable grid.
Keyset paging (`WHERE UpdatedAt < @lastSeen ORDER BY UpdatedAt DESC LIMIT 50`) is the alternative when
"next page" must stay constant-time, and it gives up random access to page 5.

One thing the seed makes safe: `UpdatedAt` is unique across the 312 tickets, so the sort has no ties.
A tie in the ordering key is the classic reason a paged grid shows the same row on two pages and silently
skips another.

## Evidence

Captured in a console check against SQLite in memory, on the real 312-ticket seed.

**Empty criteria, page 1** — two statements:

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
    LIMIT @p OFFSET @p
) AS "t0"
INNER JOIN "Customers" AS "c" ON "t0"."CustomerId" = "c"."Id"
LEFT JOIN "Agents" AS "a" ON "t0"."AgentId" = "a"."Id"
INNER JOIN "Categories" AS "c0" ON "t0"."CategoryId" = "c0"."Id"
ORDER BY "t0"."UpdatedAt" DESC
```

`total=312 rows=50 pages=7 statements=2 contexts=1/1`. Page 2 sends the same two statements with a
different `OFFSET` and returns the next fifty rows (`SD-1051` … `SD-1100`).

**Every filter set** — still two statements; the `COUNT` carries all of them:

```sql
SELECT COUNT(*) FROM "Tickets" AS "t"
INNER JOIN "Customers" AS "c" ON "t"."CustomerId" = "c"."Id"
WHERE ("t"."Number" LIKE @text_startswith ESCAPE '\' OR instr("t"."Title", @text) > 0 OR instr("c"."Name", @text) > 0)
  AND "t"."Status" = @status AND "t"."CustomerId" = @customerId
  AND "t"."DueDate" IS NOT NULL AND "t"."DueDate" >= @dueFrom AND "t"."DueDate" <= @dueTo
```

**No filters** — the `COUNT` has no `WHERE` clause at all: `SELECT COUNT(*) FROM "Tickets" AS "t"`.

**Measured totals on the seed:** empty → 312; `Text = "SD-10"` → 99 (`SD-1001` … `SD-1099`);
`Text = "printer"` → 20; `Text = "there is no such ticket"` → 0.

**Tests** (`SupportDesk.Tests/TicketSearchTests.cs`):

- `A_search_sends_exactly_two_statements_a_COUNT_and_a_paged_SELECT` — `scope.Commands == 2`, the first is
  a `SELECT COUNT(*)` with no `LIMIT`, the second contains `LIMIT @`, `OFFSET @` and
  `ORDER BY "t"."UpdatedAt" DESC`.
- `Every_filter_that_is_set_becomes_SQL_and_the_others_do_not_exist` — the `COUNT` text contains the status,
  customer and both due-date comparisons, a `LIKE @` and an `instr(`.
- `An_unset_filter_produces_no_WHERE_clause_at_all`.
- `Page_two_skips_the_first_fifty_rows_and_keeps_the_same_total` — no id appears on both pages, the total is
  identical, and page 2 starts strictly older than page 1 ends.
- `The_last_page_returns_the_remainder_only` — page 7 of 7 returns 12 rows.
- `A_page_past_the_end_returns_no_rows_but_still_reports_the_total`.
- `Status_filter_returns_only_that_status_and_a_smaller_total`,
  `Customer_filter_uses_the_key_not_the_display_text`,
  `Text_matches_a_ticket_number_by_prefix`, `Text_matches_part_of_a_title`,
  `Text_matches_part_of_a_customer_name`,
  `Due_range_filter_keeps_the_bounds_and_drops_tickets_without_a_due_date` — each one compares the reported
  total against a `CountAsync` written independently of the service.
- `Filters_that_match_nothing_return_an_empty_page_and_a_zero_total`.
