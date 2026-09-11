# Deliverable 5 · Parent grid refreshed after DialogResult.OK, and Cancel persists nothing

## Opening the editor

`TicketBrowserPage` opens `TicketEditorForm` from two buttons — `btnAdd_Click` (*Add Ticket*) and
`btnEdit_Click` (*Edit Ticket*, on the selected grid row) — both through one helper:

```csharp
private async Task OpenEditorAsync(int? ticketId)
{
    if (_loading)
        return;                          // guard, same as every other page operation

    DialogResult result;
    using (var editor = new TicketEditorForm(ticketId))
    {
        result = await editor.ShowDialogAsync();
    }

    if (result == DialogResult.OK)
    {
        _pageIndex = 0;
        await LoadTicketsAsync();        // re-run the search only when something was written
    }
}
```

(Module 7 also passes the page's selected role to the editor, which decides whether the conflict dialog
offers Overwrite.)

`await editor.ShowDialogAsync()` — the cookbook's verified pattern for an awaitable modal — returns the
`DialogResult` the dialog was closed with. The `using` block disposes the form afterward: the Wisej.NET XML
docs note that `ShowDialogAsync` does not auto-dispose the dialog ("dialogs are reusable... you must
dispose the instance... or use the typical `using` pattern"), so `TicketBrowserPage` follows that pattern
rather than leaking one `TicketEditorForm` per Add/Edit click.

## Page 1, sorted to the top

`TicketQueryService.SearchTicketsAsync` orders `OrderByDescending(t => t.UpdatedAt)` (Module 3), and
`SupportDeskContext.SaveChanges` stamps a fresh `UpdatedAt` on every insert and update (Module 2). Together
that means the ticket the editor just saved is always the most recently updated row in the table —
`_pageIndex = 0` before the refresh is what actually puts it on screen; without it, a save made while
looking at page 4 would refresh page 4, not the page the new row is actually on.

## Cancel writes nothing

`TicketEditorForm.btnCancel_Click` sets `DialogResult.Cancel` and calls `Close()` — no service call, no
`DbContext`:

```csharp
private void btnCancel_Click(object sender, EventArgs e)
{
    this.DialogResult = DialogResult.Cancel;
    Close();
}
```

Because `LoadEditorAsync`'s reads (`GetLookupsAsync`, `LoadEditModelAsync`) each open and dispose their own
context before `Load` finishes, by the time Cancel can be clicked there is no context alive to leak or roll
back — there is simply nothing left to undo.

## Evidence

- `OpenEditorAsync`, `btnAdd_Click`/`btnEdit_Click` and the `DialogResult` branch compile against the real
  Wisej.NET `Form.ShowDialogAsync`/`DialogResult` API (confirmed in the Wisej-4 4.1.0 XML docs, not
  guessed).
- In the browser: after **Save** the grid re-runs the search and the saved ticket is the first row of page
  1; after **Cancel** the grid is left as it was, with no search.
- `TicketCommandServiceTests` cover every outcome `OpenEditorAsync` has to branch on
  (`SaveAsync` returning a result, `SaveAsync` throwing `TicketNotFoundException`, `DeleteAsync` returning
  each of the three outcomes) — what the *service* does after a save or a delete is proved by tests; what
  the *dialog* does with the result (closing with the right `DialogResult`) follows from reading
  `TicketEditorForm.cs`, which calls `this.DialogResult = DialogResult.OK; Close();` on every path that
  wrote something (or found nothing to write to), and never on Cancel or a refused delete.

**Not verified here — for the browser reviewer.** Whether `ShowDialogAsync()` really suspends the page's
click handler until the dialog closes (rather than returning immediately) — the mechanism the whole
"only refresh on OK" design depends on; whether the saved/edited row visibly lands on page 1 at the top of
the grid; whether Cancel visibly leaves the grid exactly as it was, with no flicker or spurious search.
