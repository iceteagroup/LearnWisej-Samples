# Search, Next, Previous — the total count and the loading guard

Deliverable 5 of the Module 3 lab: three buttons that each issue **one** new query, a status label that
reports the returned count, the total and the page size, and a guard that keeps the page usable while a
search runs and after one fails.

## The three handlers

```csharp
private async void searchButton_Click(object sender, EventArgs e)
{
    _pageIndex = 0;                      // a new search always starts at page 1
    await LoadTicketsAsync();
}

private async void nextPageButton_Click(object sender, EventArgs e)
{
    _pageIndex++;
    await LoadTicketsAsync();
}

private async void prevPageButton_Click(object sender, EventArgs e)
{
    _pageIndex = Math.Max(0, _pageIndex - 1);
    await LoadTicketsAsync();
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

`LoadTicketsAsync` in `TicketBrowserPage.cs` is the shape every database call on the page goes through:

```csharp
if (_loading)
    return;                              // a click that arrives while a search is pending is dropped

try
{
    _loading = true;
    SetBusy(true);                       // Search, Previous and Next off
    statusLabel.Text = "Loading tickets...";

    var result = await TicketQueries.SearchTicketsAsync(ReadCriteria());   // exactly one awaited service call

    _totalCount = result.TotalCount;
    ticketBindingSource.DataSource = result.Items.ToList();
    ticketBindingSource.ResetBindings(false);
    statusLabel.Text = …;                // "Showing 50 of 312 tickets · page 1 of 7 · page size 50"
}
catch (Exception ex)
{
    Console.Error.WriteLine("[SupportDesk] Ticket search failed: " + ex);   // the server log keeps the details
    AlertBox.Show("Tickets could not be loaded. Your filters are unchanged. Please try again in a moment.",
        MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
    statusLabel.Text = "Tickets could not be loaded";
}
finally
{
    _loading = false;
    SetBusy(false);                      // buttons back on — on EVERY path
    Application.Update(this);            // push the final state to the browser
}
```

Four things it guarantees:

- **One operation at a time.** Wisej.NET does deliver a second click while an awaited handler is pending;
  `_loading` is what drops it. EF Core does not support two concurrent operations on one context, and even
  with a context per call, two searches racing to assign the BindingSource would leave the grid showing
  whichever finished last.
- **Visible progress.** The buttons go grey and the status label says what is happening.
- **A usable page after a failure.** `finally` runs on the exception path too, so Search comes back
  enabled. A page that stays disabled after one failure is a page that gets refreshed — and a refreshed
  Wisej.NET page loses its session state, including the filters the operator typed.
- **Business language only.** The `catch` shows one friendly sentence in an `AlertBox`
  (`MessageBoxIcon.Error`, top-right, `autoCloseDelay: 4000`) and writes the full exception to the server
  console. The connection string and the SQL never reach the browser.

`Application.Update(this)` in `finally` matters because after the `await` the continuation may be running
off the original request — the response the click arrived on can be gone, and without the explicit update
the final state would sit on the server until the next round trip.

## The status label and the paging buttons

```csharp
statusLabel.Text = $"Showing {result.Items.Count} of {result.TotalCount} tickets · page {_pageIndex + 1} of {result.PageCount(PageSize)} · page size {PageSize}";
```

on 312 seeded tickets and a `const int PageSize = 50`:

```
Showing 50 of 312 tickets · page 1 of 7 · page size 50
```

and on the last page, `Showing 12 of 312 tickets · page 7 of 7 · page size 50`. An empty result resets
`_pageIndex` to 0 and reports `No tickets match these filters`, rather than "page 1 of 0".

```csharp
private void UpdatePagingButtons()
{
    var pageCount = _totalCount <= 0 ? 1 : (_totalCount + PageSize - 1) / PageSize;
    prevPageButton.Enabled = _pageIndex > 0;
    nextPageButton.Enabled = _pageIndex + 1 < pageCount;
}
```

`SetBusy(false)` calls it, so re-enabling the buttons after an operation never re-enables Previous on page 1
or Next on the last page.

## The failure path

There is no simulated outage: the `catch` handles a real database failure. Whatever the cause (the SQLite
file cannot be opened, the connection fails), the user gets the `AlertBox` and *Tickets could not be
loaded*, the full exception goes to the server console as one `[SupportDesk] Ticket search failed: …`
line, and `finally` restores the page. The next click gets a **fresh** context from the factory, so
nothing that failed is reused.

## Evidence

**In the browser:** opening the page shows *Showing 50 of 312 tickets · page 1 of 7 · page size 50*;
Next page moves to *page 2 of 7* with the next fifty rows; a filter that matches nothing shows *No tickets
match these filters*.

**On the server console** (Modules 3 to 6, where `appsettings.Development.json` logs
`Microsoft.EntityFrameworkCore.Database.Command` at `Information`): every Search, Next or Previous prints
exactly two statements, a `SELECT COUNT(*)` and a paged `SELECT … LIMIT @p OFFSET @p`.

**Tests** (`SupportDesk.Tests/TicketSearchTests.cs`) — the service side of all of this:

- `A_search_sends_exactly_two_statements_a_COUNT_and_a_paged_SELECT` — one context created and disposed per
  search.
- `Page_two_skips_the_first_fifty_rows_and_keeps_the_same_total`, `The_last_page_returns_the_remainder_only`
  and `A_page_past_the_end_returns_no_rows_but_still_reports_the_total` — the numbers the status label and
  the paging buttons are computed from.
- `Filters_that_match_nothing_return_an_empty_page_and_a_zero_total` — `PageCount` is 1, not 0.

**Not verified here:** the guard's behaviour in the browser — that Wisej.NET really delivers the second
click, that the buttons visibly grey out, and that `Application.Update(this)` pushes the final state. That
was verified for the count path in Module 1 and is the reviewer's job for the search path. See the README's
*Verified / unverified*.
