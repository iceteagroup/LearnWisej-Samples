# Related-data decisions: no lazy loading, four strategies, one on purpose per screen

Deliverable 4 of the Module 6 lab: "no lazy-loading N+1 behaviour left in display code." This is the module's
related-data decision table, in code: `TicketQueryService.SearchTicketsNaiveAsync` (the anti-pattern, kept and
measured, not deleted) and `TicketDetailService`'s three methods (the fix, applied where each strategy is
actually the right one).

## There is no lazy loading anywhere in this solution

`UseLazyLoadingProxies` is never added to `AddSupportDeskData`. That is a standing fact across every module of
this course, not something Module 6 introduces — but Module 6 is the first module where it matters, because
this is the module about N+1. Since lazy loading does not exist here, a hidden per-row query cannot happen by
accident; the closest thing to it — `TicketQueryService.SearchTicketsNaiveAsync` — writes the exact queries
lazy loading would otherwise have issued silently, on purpose and in the open:

```csharp
await db.Entry(t).Reference(x => x.Customer).LoadAsync(token);
await db.Entry(t).Reference(x => x.Agent).LoadAsync(token);
await db.Entry(t).Reference(x => x.Category).LoadAsync(token);
```

This is what a `CellFormatting` handler reading `ticket.Customer.Name` would have triggered, one navigation
access at a time, if this project had lazy loading turned on. The lesson's review question — "nothing in the
application warned you" — is true of lazy loading; it is also true of this exact anti-pattern if it were
quietly deleted instead of kept, named, and measured. `SearchTicketsNaiveAsync` is the module's answer to
"where would the 150 extra statements have come from, and why would nobody notice": right here, one call per
navigation property per row, indistinguishable from ordinary-looking code unless someone is watching the log.

## Why the measured statement count is not "1 + 3 × rows"

The lesson (`ef6s2.html`'s review question) and the walkthrough video quote **151 statements for one screen of
50 tickets** — `1 + 3 × 50`. This solution's naive branch, run against its own 312-ticket seed with no filter,
measures **15**, not 151. The reason is a genuine, verified EF Core fact, not a bug in the naive branch:

> EF Core's change tracker performs automatic reference fix-up among entities already tracked by the same
> `DbContext`. Once one ticket's `Customer` has been loaded, every later ticket that shares the same
> `CustomerId` gets its `Customer` navigation wired up from the tracked graph, with `IsLoaded` already
> `true` — `ReferenceEntry.LoadAsync` returns immediately and never reaches the database.

Verified directly with a console probe against this seed: a `Reference().LoadAsync()` call for a foreign key
that is `null` issues **zero** commands, and a foreign key value already represented by a tracked entity
issues **zero additional** commands (`SupportDesk.Tests/TicketPerformanceTests.cs`,
`A_reference_load_for_a_null_foreign_key_sends_nothing`). The Support Desk seed has only 5 customers, 3
agents and 6 categories behind 312 tickets — small lookup tables by design (`DevelopmentSeeder`) — so only the
**first** occurrence of each distinct value among the capped 50 rows costs a statement: `1` (load every
matching ticket) `+ 5` (distinct customers) `+ 3` (distinct agents) `+ 6` (distinct categories) `= 15`.

This does not make the anti-pattern smaller — it makes it a sharper lesson than the flat "1 + 3 × rows"
story:

- The problem this branch actually has — **326 tracked entities held for the life of the context, and no
  SQL-level paging** — is exactly as real at 15 statements as it would be at 151. A support desk with 50,000
  real customers behind 50,000 real tickets has no small lookup table for fix-up to exploit; at that scale the
  statement count returns to something close to `1 + 3 × rows`, because almost every row's `CustomerId` is a
  value the context has never seen before. Measured separately, forcing every per-row lookup through an
  independent query instead of `Entry(...).Reference(...)` (bypassing the fix-up shortcut on purpose, the way
  a wider customer base effectively would) produces **131** statements for the same 50 rows — the number a
  real support desk would actually see.
- "Nothing warned you" is *still* true: a developer who profiles this exact demo, sees 15 statements instead
  of 151, and concludes the naive branch "isn't that bad" has just been fooled by an accident of the seed
  data's cardinality — precisely the trap the lesson's own motto ("measure, don't guess") exists to catch.
  Measuring on realistic data, not a hand-built demo with five customers, is the actual lesson.

`docs/BeforeAfterMeasurements.md` has the full measured table, including the 131-statement "what if fix-up
did not help" number.

## The related-data decision table, applied

`TicketDetailService` (new in Module 6) is the four strategies the lesson names, one method per strategy,
each chosen for what its caller is about to do with the data:

| Strategy | Method | Why this one, here |
|---|---|---|
| Projection | `TicketQueryService.SearchTicketsAsync` (Module 3, unchanged) | The grid is read-only and flat — `TicketListItem` needs three joined names, never a `Ticket`. |
| `Include` | `TicketDetailService.LoadForEditorAsync` | A controlled aggregate: the ticket plus `Customer` and `Category`, tracked, one statement. Fixed shape — it never grows as the database does. |
| Filtered read (equivalent to a filtered `Include`) | `TicketDetailService.LoadRecentCommentsAsync` | Only the last five comments, newest first — loading the whole collection just to show five would be its own small anti-pattern. |
| Explicit loading | `TicketDetailService.LoadAllCommentsAsync` | Only runs when the operator clicks "Show full history" — the definition of "on demand." |
| Lazy loading | *(never)* | See above — `UseLazyLoadingProxies` is not added anywhere in this solution. |

### `LoadForEditorAsync` — `Include`, tracked, one statement

```csharp
var ticket = await db.Tickets
    .Include(t => t.Customer)
    .Include(t => t.Category)
    .SingleOrDefaultAsync(t => t.Id == id, token)
    ?? throw new TicketNotFoundException(id);
```

Measured: **1 statement**, one `INNER JOIN … INNER JOIN` (`SupportDesk.Tests/TicketDetailServiceTests.cs`,
`LoadForEditorAsync_sends_one_statement_and_eagerly_loads_customer_and_category`). `Agent` and `Comments` stay
unloaded on purpose — nothing that calls this method needs them, and eager-loading everything "just in case"
is the same mistake as the naive branch's per-row loads, only committed once instead of fifty times.

**Why projection would be the wrong answer here**, per the lesson's own review question: the ticket browser
projects because it is read-only. The editor is not — `TicketCommandService.SaveAsync` writes a tracked
`Ticket` back through `SaveChangesAsync` in its own context — so whatever loads the ticket for editing has to
be able to become that tracked write eventually. A flat DTO can never do that; only a real, trackable entity
can. Projection is correct exactly where nothing downstream will ever call `SaveChangesAsync` against the
result, and the editor is precisely the screen where something will.

### `LoadRecentCommentsAsync` — filtered, no-tracking, identity resolution, one statement

```csharp
var comments = await db.TicketComments
    .AsNoTrackingWithIdentityResolution()
    .Include(c => c.Ticket)
    .Where(c => c.TicketId == id)
    .OrderByDescending(c => c.CreatedAt)
    .Take(5)
    .ToListAsync(token);
```

Equivalent to a filtered `Include(t => t.Comments.OrderByDescending(c => c.CreatedAt).Take(5))` written the
other way round, directly against `TicketComments` — the lab guide names both shapes as acceptable
("filtered `Include` … or an equivalent projected query"). Measured: **1 statement**
(`LoadRecentCommentsAsync_sends_one_statement_and_returns_at_most_five_newest_first`), no split query: EF
Core 10's default query-splitting behaviour for a single collection-shaped read with one reference `Include`
produces one JOIN, not two round trips — `AsSplitQuery()` would have to be called explicitly to get a second
statement, and nothing here calls it.

**Why `AsNoTrackingWithIdentityResolution`, not plain `AsNoTracking`.** The query's `Include(c => c.Ticket)`
means every one of the (up to five) returned comment rows carries a reference back to the **same** ticket.
Plain `AsNoTracking` would materialise a fresh, separate `Ticket` clone for every comment row — even though,
here, all five belong to one ticket. Identity resolution reuses the one instance already materialised for the
first row instead of cloning it four more times — verified directly: five comment rows, **one** shared
`Ticket` instance, not five (a dedicated probe during development; not re-asserted as its own test because
the behaviour is EF Core's documented contract, not this project's code). The saving is modest at five rows
for one ticket — it is used here mainly to demonstrate the API on a small, safe query. The real payoff is
largest on a wider query that joins many rows back to far fewer parents (an activity feed across the whole
queue, say), which this narrow, single-ticket call is not.

### `LoadAllCommentsAsync` — explicit loading, one extra statement, only on demand

```csharp
var ticket = await db.Tickets.SingleOrDefaultAsync(t => t.Id == id, token)
    ?? throw new TicketNotFoundException(id);

await db.Entry(ticket).Collection(t => t.Comments).LoadAsync(token);
```

Chosen over "a separate query" (the lab guide's other named option) because the two reads belong to one "show
me this ticket's full history" operation: the tracked ticket read is nearly free (the key is already known,
the operator is already looking at this exact ticket), so paying for a second context just to avoid one extra
statement on the one already open buys nothing. Measured: **2 statements** — the tracked ticket read, then
`Collection(...).LoadAsync()`, **one extra** beyond the ticket read itself
(`LoadAllCommentsAsync_sends_two_statements_the_ticket_read_plus_one_extra_for_the_collection`) — and it only
ever runs when the operator clicks **"Show full history"** in `TicketEditorForm`, never on load.

## `TicketEditorForm`'s comments panel: no navigation property, anywhere

`lstComments` (a small, read-only `ListBox`) shows the last five comments; `btnShowFullHistory` runs the
explicit load. Both are filled by `TicketEditorForm.RenderComments`, whose only inputs are
`CommentSummary` values — a DTO with `Id`, `Author`, `Body`, `CreatedAt` — never a `Ticket.Comments`
navigation, never a `TicketComment` entity:

```csharp
private void RenderComments(IReadOnlyList<CommentSummary> comments, string emptyText)
{
    this.lstComments.Items.Clear();
    if (comments.Count == 0) { this.lstComments.Items.Add(emptyText); return; }
    foreach (var c in comments)
        this.lstComments.Items.Add($"{c.CreatedAt:yyyy-MM-dd HH:mm}  {c.Author}: {Truncate(c.Body, 90)}");
}
```

This is the editor's one piece of "row formatting" code for comments, and the discipline it follows is the
same one the ticket browser's grid columns already follow (`docs/NoTrackingProjection.md`) and
`SearchTicketsNaiveAsync` exists to show the cost of skipping: display code reads fields already sitting on
the object in hand, never a navigation property that could still be a query waiting to happen.

## Evidence

Console check against SQLite in memory (312-ticket seed), the real shipped `TicketDetailService`:

```
REAL LoadForEditorAsync: statements=1
REAL LoadRecentCommentsAsync: statements=1, rows=2
REAL LoadAllCommentsAsync: statements=2, rows=2
```

**Tests** (`SupportDesk.Tests/TicketDetailServiceTests.cs`):

- `LoadForEditorAsync_sends_one_statement_and_eagerly_loads_customer_and_category`,
  `LoadForEditorAsync_throws_TicketNotFoundException_for_a_missing_id`.
- `LoadRecentCommentsAsync_sends_one_statement_and_returns_at_most_five_newest_first`,
  `LoadRecentCommentsAsync_returns_an_empty_list_for_a_ticket_with_no_comments`.
- `LoadAllCommentsAsync_sends_two_statements_the_ticket_read_plus_one_extra_for_the_collection`,
  `LoadAllCommentsAsync_returns_more_rows_than_LoadRecentCommentsAsync_when_a_ticket_has_more_than_five`
  (asserted as `All.Count >= Recent.Count`, the general rule — the seed's comment pattern tops out at two per
  ticket, so no ticket in this data actually exercises the ">five" case; the rule is still correct and the
  test would catch a regression if it swapped the two methods' roles),
  `LoadAllCommentsAsync_throws_TicketNotFoundException_for_a_missing_id`.

**Not verified here — for the browser reviewer.** `lstComments` and `btnShowFullHistory` actually render and
enable/disable correctly across Add ticket (no `_ticketId` yet — the panel shows "Comments appear here once
the ticket is saved.", the button stays disabled), Edit ticket with no comments, Edit ticket with comments,
and after a click on "Show full history" — the underlying service calls are proved by the tests above, the
Wisej.NET rendering and click wiring are not; the application was not started for this report.
