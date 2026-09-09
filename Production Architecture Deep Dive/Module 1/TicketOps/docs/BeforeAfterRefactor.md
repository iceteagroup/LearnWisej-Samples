# Before / after — the fat handler refactor

*Module 1 deliverable · shows every path without leaking internals*

## Before — the junior TicketOps app (what the walkthrough starts from)

```csharp
private void btnSave_Click(object sender, EventArgs e)
{
    // validation, business rules, SQL and UI all tangled together
    if (string.IsNullOrWhiteSpace(txtTitle.Text)) { MessageBox.Show("Title required"); return; }
    using var conn = new SqlConnection(_cs);
    conn.Open();
    var cmd = new SqlCommand("INSERT INTO Tickets ...", conn);
    cmd.ExecuteNonQuery();
    lblStatus.Text = "Saved";
}

private void btnClose_Click(object sender, EventArgs e)
{
    if (_status == "Open" && _hoursLogged > 0)
    {
        using var cmd = new SqlCommand("UPDATE Tickets SET Status='Closed' WHERE Id=@id", _conn);
        cmd.Parameters.AddWithValue("@id", _ticketId);
        cmd.ExecuteNonQuery();
        lblStatus.Text = "Closed";
    }
    else
    {
        lblStatus.Text = "Cannot close: log hours first.";
    }
}
```

Each line was asked one question: **is this display, or is this a decision?**

| Line | Display or decision? | Went to |
|---|---|---|
| `txtTitle.Text` is empty → "Title required" | decision (validation) | `TicketService.Validate` → `OperationResult.Fail("Title is required.")` |
| `new SqlConnection` … `ExecuteNonQuery` | dependency (persistence) | `ITicketRepository.UpsertAsync` (`InMemoryTicketRepository` today) |
| `_status == "Open" && _hoursLogged > 0` | decision (business rule) | `Ticket.CanClose(out reason)` in `Domain/` |
| `lblStatus.Text = …` | display | stays: `ShowResult(result)` |

## After — the screen (`Views/TicketEditor.cs`)

```csharp
private async void buttonSave_Click(object sender, EventArgs e)
{
    try
    {
        var draft = ReadDraftFromForm();                 // UI → data
        var result = await _tickets.SaveAsync(draft);   // decision lives in the service
        ShowResult(result);                              // data → UI
        if (result.Succeeded) await RefreshGridAsync();
    }
    catch (Exception ex)
    {
        ReportFailure("TicketEditor.buttonSave_Click", ex);   // log details, show the safe message
    }
}

private async void buttonClose_Click(object sender, EventArgs e)
{
    try
    {
        int? id = SelectedTicketId();
        if (id == null) { ShowResult(OperationResult<Ticket>.Fail("Select a ticket to close.")); return; }
        var result = await _tickets.CloseAsync(id.Value);
        ShowResult(result);
        if (result.Succeeded) await RefreshGridAsync();
    }
    catch (Exception ex) { ReportFailure("TicketEditor.buttonClose_Click", ex); }
}
```

The handler no longer knows the word "Open", the word "INSERT", or the connection — it knows a
draft, an id and a result.

## After — the service (`Services/TicketService.cs`) and the rule (`Domain/Ticket.cs`)

```csharp
public async Task<OperationResult<Ticket>> CloseAsync(int ticketId)
{
    var ticket = await _repository.FindAsync(ticketId);
    if (ticket == null) return OperationResult<Ticket>.Fail("The ticket no longer exists. Refresh the list.");
    if (!ticket.CanClose(out string reason)) return OperationResult<Ticket>.Fail(reason);   // the rule, in one place
    ticket.Close();
    await _repository.UpsertAsync(ticket);
    return OperationResult<Ticket>.Ok(ticket, $"Ticket #{ticketId} closed.");
}
```

`Ticket.CanClose` can be unit-tested with no repository and no browser, and reused by a bulk-close
action without copy-paste.

## Every path, visible, nothing leaked

| Path | Trace (right card) | What the user sees |
|---|---|---|
| Success | `[UI] → SaveAsync` · `[SVC] valid → UpsertAsync` · `[DATA] #1041 written` · `[UI] OK · Ticket #1041 saved.` | **● Ticket #1041 saved.**, grid refreshed |
| Validation | `[SVC] ⚠ rejected: Title is required.` (no `[DATA]` line) | orange banner **Title is required.** |
| Domain rule | `[DOMAIN] ⚠ Ticket.CanClose — #1042 rejected: Log hours before closing.` | orange banner **Log hours before closing.** |
| Data failure | `[DATA] ✖ outage: SELECT * FROM Tickets failed — timeout connecting to sql01:1433 (TicketOps.dbo.Tickets)` · `[UI] ✖ caught DataOutageException — user sees the safe message` | red banner **The action could not be completed. Check the log for details.** — the host name and table never reach the screen |
| Recovery | `[UI] outage OFF → refresh (recovery)` · `[DATA] 6 rows` | **● ready** |

## Evidence

Run the app, press the four bottom-bar buttons in order and compare the trace with the table above.
The "Load 200 tickets" button proves the same thin handler scales: two hundred saves go through
`ITicketService.SaveAsync`, the handler is still the same nine lines, and the Timer only paces the work.
