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

| Path | How to reach it | What the user sees |
|---|---|---|
| Success | edit #1041, **Save ticket** | **● Ticket #1041 saved.**, grid refreshed |
| Validation | clear the title, **Save ticket** | orange banner **Title is required.**; nothing is persisted |
| Domain rule | select #1042 (no hours logged), **Close ticket** | orange banner **Log hours before closing.** |
| Unexpected failure | any exception thrown below the handler (e.g. the data store is down) | red banner and toast **The action could not be completed. Check the log for details.**; the exception type, message and stack go to `ILog` (the server console) and never reach the screen |
