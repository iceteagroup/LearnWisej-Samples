# Search, Next, Previous — the total count and the loading guard

Deliverable 5 of the Module 3 lab: three buttons that each issue **one** new query, a status label that
reports the returned count, the total and the page size, and a guard that keeps the page usable while a
search runs and after one fails.

## The three handlers

```csharp
private async void searchButton_Click(object sender, EventArgs e)
{
    _pageIndex = 0;                      // a new search always starts at page 1
    await LoadTicketsAsync("searchButton_Click");
}

private async void nextPageButton_Click(object sender, EventArgs e)
{
    _pageIndex++;
    await LoadTicketsAsync("nextPageButton_Click");
}

private async void prevPageButton_Click(object sender, EventArgs e)
{
    _pageIndex = Math.Max(0, _pageIndex - 1);
    await LoadTicketsAsync("prevPageButton_Click");
}
```

Resetting `_pageIndex` on Search is the bug this prevents: filters that leave four pages, applied while the
operator is on page 6, would otherwise return an empty grid over a non-empty result.

Next and Previous change one integer and run the same search. Nothing is cached: page 4 is
`OFFSET 150 LIMIT 50` against the current data, not a slice of a list the server is holding. That is what
makes the browser survive a growing table — and it is why a ticket edited by someone else shows its new
values the next time you page past it.

`async void` is correct here (a Wisej.NET event handler has no caller to await it) and only here: every
method it calls returns a `Task`.

## The guard

`RunAsync` in `TicketBrowserPage.cs` is the shape every handler in this course goes through:

```csharp
if (_loading)
{
    AddTrace(Glyph.Server, "guard", $"{origin}: an operation is already running — this click is ignored");
    return;
}

try
{
    _loading = true;
    SetBusy(true);                       // every button off
    HideBanner();
    statusLabel.Text = busyStatus;       // "Loading tickets…"

    var result = await operation();      // exactly one awaited service call

    statusLabel.Text = result;           // "Showing 50 of 312 tickets · page 1 of 7 · page size 50"
}
catch (DatabaseUnavailableException ex) { Fail("The Support Desk database is not reachable…", ex); }
catch (DbUpdateException ex)            { Fail(dbUpdateMessage, ex); }
catch (Exception ex)                    { Fail(genericMessage, ex); }
finally
{
    _loading = false;
    SetBusy(false);                      // buttons back on — on EVERY path
    await RefreshModelCardAsync();
    UpdateLifetimes();
    Application.Update(this);            // push the final state to the browser
}
```

Four things it guarantees:

- **One operation at a time.** Wisej.NET does deliver a second click while an awaited handler is pending;
  `_loading` is what drops it, and the drop is written into the trace instead of being silent. EF Core does
  not support two concurrent operations on one context, and even with a context per call, two searches
  racing to assign the BindingSource would leave the grid showing whichever finished last.
- **Visible progress.** The buttons go grey and the status label says what is happening. `Slow search
  (2.5 s)` exists purely so that state lasts long enough to read.
- **A usable page after a failure.** `finally` runs on the exception path too, so Search comes back
  enabled. A page that stays disabled after one failure is a page that gets refreshed — and a refreshed
  Wisej.NET page loses its session state, including the filters the operator typed.
- **Business language only.** `Fail` puts a friendly sentence in the red banner and in an `AlertBox`
  (`MessageBoxIcon.Error`, top-right, `autoCloseDelay: 4000`) and writes the exception type and first
  sentence into the server-side trace. The connection string and the SQL never reach the browser.

`Application.Update(this)` in `finally` matters because after the `await` the continuation may be running
off the original request — the response the click arrived on can be gone, and without the explicit update
the final state would sit on the server until the next round trip.

## The status label and the paging buttons

```csharp
return $"Showing {result.Items.Count} of {result.TotalCount} tickets · page {_pageIndex + 1} of {result.PageCount(PageSize)} · page size {PageSize}";
```

on 312 seeded tickets and a `const int PageSize = 50`:

```
Showing 50 of 312 tickets · page 1 of 7 · page size 50
```

and on the last page, `Showing 12 of 312 tickets · page 7 of 7 · page size 50`. An empty result reports
`No tickets match these filters · page size 50` and turns the chip amber, rather than "page 1 of 0".

```csharp
private void UpdatePagingButtons()
{
    var pageCount = _totalCount <= 0 ? 1 : (_totalCount + PageSize - 1) / PageSize;
    this.prevPageButton.Enabled = _pageIndex > 0;
    this.nextPageButton.Enabled = _pageIndex + 1 < pageCount;
}
```

`SetBusy(false)` calls it, so re-enabling the buttons after an operation never re-enables Previous on page 1
or Next on the last page.

## The failure path, on demand

`Break the database` sets `DevelopmentOutageSwitch.IsDown = true` (a development-only singleton read by
`OutageInterceptor : DbConnectionInterceptor`) and then runs a search. Opening the connection throws
`DatabaseUnavailableException`, the `catch` shows the friendly message, and `finally` restores the page.
`Restore and search` switches it off and searches again — the next click gets a **fresh** context from the
factory, so nothing that failed is reused.

## Evidence

**Trace, Search then Next page** (the shape the page writes; statement counts and SQL verified in a console
check and by the tests):

```
• searchButton_Click TicketQueryService.SearchTicketsAsync(no filters · page 1, 50 rows)
◦ context      #4 created (SupportDeskContext from the factory)
• service      composed IQueryable<Ticket>: no filters · page 1, 50 rows — nothing sent yet; the two awaits below are the only statements
→ SQL          SELECT COUNT(*) FROM "Tickets" AS "t"   (0.2 ms)
→ SQL          SELECT "t0"."Id", … LIMIT @p1 OFFSET @p …   (0.6 ms)
• service      materialised 50 TicketListItem rows of 312 matching · 0 entities tracked (AsNoTracking) — the list outlives this context
◦ context      #4 disposed (0 tracked entities released)
← result       Showing 50 of 312 tickets · page 1 of 7 · page size 50 · 2 statement(s) · 0.8 ms in the database · 1 context created, 1 disposed

• paging       nextPageButton_Click → page 2: one new query with OFFSET 50, not a cached copy of the result set
• nextPageButton_Click TicketQueryService.SearchTicketsAsync(no filters · page 2, 50 rows)
◦ context      #5 created (SupportDeskContext from the factory)
→ SQL          SELECT COUNT(*) FROM "Tickets" AS "t"   (0.1 ms)
→ SQL          SELECT "t0"."Id", … LIMIT @p OFFSET @p …   (0.5 ms)
◦ context      #5 disposed (0 tracked entities released)
← result       Showing 50 of 312 tickets · page 2 of 7 · page size 50 · 2 statement(s) · … · 1 context created, 1 disposed
```

**Trace, Search clicked during `Slow search (2.5 s)`** — the guard line sits between the context and the
SQL, because the slow operation had already opened its context when the second click arrived:

```
• buttonSlowSearch_Click TicketQueryService.SearchTicketsSlowlyAsync(no filters · page 1, 50 rows, 2.5 s)
◦ context      #6 created (SupportDeskContext from the factory)
• service      simulated latency of 2.5 s inside the unit of work — the context is already open and the page is guarded
• guard        searchButton_Click: an operation is already running — this click is ignored
→ SQL          SELECT COUNT(*) FROM "Tickets" AS "t"   (0.2 ms)
…
```

**Trace, Break the database → Restore and search:**

```
• outage       DevelopmentOutageSwitch.IsDown = true — every connection open now fails (lab prop, development only)
• buttonBreak_Click TicketQueryService.SearchTicketsAsync(no filters · page 1, 50 rows)
◦ context      #7 created (SupportDeskContext from the factory)
◦ context      #7 disposed (0 tracked entities released)
• caught       DatabaseUnavailableException: Simulated outage: the Support Desk database is unreachable (DevelopmentOutageSwitch.IsDown = true). → friendly message shown, full exception logged server-side
◦ card         Model & migration card not refreshed: DatabaseUnavailableException — …
```

with the red banner *The Support Desk database is not reachable right now. Nothing was changed — please
try again in a moment.*, the same text as a top-right `AlertBox`, `● fault`, and Search enabled again.
`Restore and search` then produces an ordinary two-statement search.

**Tests** (`SupportDesk.Tests/TicketSearchTests.cs`) — the service side of all of this:

- `A_search_sends_exactly_two_statements_a_COUNT_and_a_paged_SELECT` — one context created and disposed per
  search.
- `Page_two_skips_the_first_fifty_rows_and_keeps_the_same_total`, `The_last_page_returns_the_remainder_only`
  and `A_page_past_the_end_returns_no_rows_but_still_reports_the_total` — the numbers the status label and
  the paging buttons are computed from.
- `Filters_that_match_nothing_return_an_empty_page_and_a_zero_total` — `PageCount` is 1, not 0.
- `The_slow_search_returns_the_same_page_and_still_sends_two_statements` — two searches, four statements,
  two contexts created and two disposed: the latency did not extend a lifetime.

**Not verified here:** the guard's behaviour in the browser — that Wisej.NET really delivers the second
click, that the buttons visibly grey out, and that `Application.Update(this)` pushes the final state. That
was verified for the count path in Module 1 and is the reviewer's job for the search path. See the README's
*Verified / unverified*.
