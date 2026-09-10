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

One context, one statement, disposed before the method returns. `CountTicketsSlowlyAsync(TimeSpan)` is the
same method with a `Task.Delay` inside the unit of work — a lab prop that makes the guard visible.

## The handler (`SupportDesk.Web/TicketBrowserPage.cs`, `CountAsync`)

```csharp
if (_loading) { AddTrace("guard", "… a count is already running — this click is ignored"); return; }

using var trace = QueryTrace.Begin(OnTrace);          // lab instrument
try
{
    _loading = true;
    SetBusy(true);                                    // countButton and the slow/rapid buttons off
    HideBanner();
    statusLabel.Text = "Counting tickets…";

    var count = await TicketQueries.CountTicketsAsync();

    statusLabel.Text = $"{count} tickets in the Support Desk database";
}
catch (DatabaseUnavailableException ex) { Fail("The Support Desk database is not reachable right now. Nothing was changed — please try again in a moment.", ex); }
catch (Exception ex)                    { Fail("The ticket count is not available right now. Please try again in a moment.", ex); }
finally
{
    _loading = false;
    SetBusy(false);
    Application.Update(this);                         // push the final state to the browser
}
```

The shape is the one every later module reuses: **guard → busy UI → one awaited call → result**, the
message in `catch`, the UI restored in `finally`.

### Why the guard is a field on the page

`_loading` is per page, and the page is per session — so it is per user. A click that arrives while an
`await` is pending (Wisej.NET does deliver it; the page is not blocked) returns early and leaves a trace
line, instead of starting a second operation that would compete for the UI state. It does not need a lock:
Wisej.NET serialises the handlers of one session, and the flag is only read and written inside them.

### Why the buttons are disabled too

The guard protects the server; disabling the buttons tells the user. Both are needed: a disabled button
is only a hint the browser applies after the response arrives, and a fast double-click can beat it.

### What the user sees on failure

A friendly sentence in the red banner, `statusLabel` = *Count unavailable*, status `● fault`. The
exception type and message go to the trace (and would go to `ILogger` in production); the user never sees
a SQL error, a connection string or a stack trace. The button is back in `finally`, and the next click
creates a fresh context — nothing from the failed operation survives.

## Evidence (from the running app)

```
09:47:05.149 • countButton_Click TicketQueryService.CountTicketsAsync()
09:47:05.151 ◦ context #2 created (SupportDeskContext from the factory)
09:47:05.189 → SQL SELECT COUNT(*) FROM "Tickets" AS "t" (0.1 ms)
09:47:05.190 ◦ context #2 disposed (0 tracked entities released)
09:47:05.190 ← result 12 tickets · 1 statement(s) · 0.1 ms in the database · 1 context created, 1 disposed

09:45:01.214 • rapid click 1 TicketQueryService.CountTicketsSlowlyAsync(2.5 s)
09:45:01.214 ◦ context #6 created (SupportDeskContext from the factory)
09:45:01.216 • guard rapid click 2: a count is already running — this click is ignored
09:45:01.216 • guard rapid click 3: a count is already running — this click is ignored
09:45:03.728 → SQL SELECT COUNT(*) FROM "Tickets" AS "t" (0.1 ms)
09:45:03.729 ◦ context #6 disposed (0 tracked entities released)

09:44:57.148 • outage DevelopmentOutageSwitch.IsDown = true — every connection open now fails
09:44:57.149 ◦ context #4 created (SupportDeskContext from the factory)
09:44:57.177 ◦ context #4 disposed (0 tracked entities released)
09:44:57.178 • caught DatabaseUnavailableException: Simulated outage: the Support Desk database is unreachable … → friendly message shown, full exception logged server-side
09:44:59.194 • outage DevelopmentOutageSwitch.IsDown = false — the next operation gets a fresh context and a working connection
09:44:59.195 ← result 0 tickets · 1 statement(s) · 0.1 ms in the database · 1 context created, 1 disposed
```

(The outage run was captured before the development seed existed, hence `0 tickets` on recovery.)
