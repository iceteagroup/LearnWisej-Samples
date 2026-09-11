# The first async query · loading guard and error message

Deliverable 4 of the Module 1 lab: an async count behind a loading flag, with a friendly message when the
database is not there.

## The service (`SupportDesk.Services/TicketQueryService.cs`)

```csharp
public async Task<int> CountTicketsAsync(CancellationToken token = default)
{
    await using var db = await _dbFactory.CreateDbContextAsync(token);
    return await db.Tickets.CountAsync(token);
}
```

One context, one statement, disposed before the method returns.

## The handler (`SupportDesk.Web/TicketBrowserPage.cs`, `countButton_Click`)

```csharp
if (_loading)
    return;

try
{
    _loading = true;
    countButton.Enabled = false;
    statusLabel.Text = "Counting tickets...";

    var count = await TicketQueries.CountTicketsAsync();

    statusLabel.Text = $"{count} tickets in the Support Desk database";
}
catch (Exception ex)
{
    Console.Error.WriteLine("[SupportDesk] Ticket count failed: " + ex);   // the server log keeps the details
    AlertBox.Show("Tickets could not be counted. The Support Desk database could not be reached. …", MessageBoxIcon.Error);
    statusLabel.Text = "Count failed";
}
finally
{
    countButton.Enabled = true;
    _loading = false;
    Application.Update(this);                  // push the final state to the browser
}
```

The shape is the one every later module reuses: **guard → busy UI → one awaited call → result**, the
message in `catch`, the UI restored in `finally`.

### Why the guard is a field on the page

`_loading` is per page, and the page is per session, so it is per user. A click that arrives while an
`await` is pending (Wisej.NET does deliver it; the page is not blocked) returns early instead of starting a
second operation that would compete for the UI state. It does not need a lock: Wisej.NET serialises the
handlers of one session, and the flag is only read and written inside them.

### Why the button is disabled too

The guard protects the server; disabling the button tells the user. Both are needed: a disabled button
is only a hint the browser applies after the response arrives, and a fast double-click can beat it.

### What the user sees on failure

A friendly `AlertBox` and `statusLabel` = *Count failed*. The exception goes to the server log (and would go
to `ILogger` in production); the user never sees a SQL error, a connection string or a stack trace. The
button is back in `finally`, and the next click creates a fresh context, so nothing from the failed
operation survives.

## Evidence

- `TicketQueryServiceTests.Each_operation_creates_and_disposes_its_own_context`: two calls, two contexts
  created, two disposed, two `COUNT(*)` statements.
- In the browser: **Count tickets** shows *12 tickets in the Support Desk database*; the error path is shown
  by starting the app against a database it cannot open (see the README).
