# Dialog workflow — the modal add/edit pattern as built in this project

Module 5 adds one dialog to the Module 4 ticket screen and drives the whole add/edit workflow through it.
This note is the lab deliverable for lesson s21: what a dialog is, when to use one, the five-step modal
workflow as it is implemented here (with the real method names), and where message boxes help.

## 1. What a dialog is

A dialog is a smaller screen that appears on top of the main page so the user can complete **one focused
task** — here, filling in one ticket. It is *modal*: the user finishes it (Save) or abandons it (Cancel)
before returning to the page behind it. That focus is what makes add/edit workflows predictable.

In this project the dialog is `Dialogs/TicketDialog.cs` (+ `TicketDialog.Designer.cs`), a `Wisej.Web.Form`
with `FormBorderStyle = Fixed`, `StartPosition = CenterParent`, `MinimizeBox = MaximizeBox = false`,
`AcceptButton = btnSave` (Enter saves) and `CancelButton = btnCancel` (Escape cancels). It does not try to be
the whole application: it collects the values, validates them and returns a clear result. The parent page
owns the service call and the grid.

## 2. Dialog vs page

| Screen type | Best use | In this project |
|---|---|---|
| Page / main content area | Large screens where users spend time and navigate between features | `TicketsWindow` — the Tickets grid with New / Edit / Delete / Refresh |
| Dialog / modal window | A small focused workflow that must be completed or cancelled before going back | `TicketDialog` (Create Ticket, Edit Ticket #n), the "Confirm delete" `MessageBox` |

Rule of thumb used here: if the user would want to keep the grid visible and come back to it in a moment,
it is a dialog; if the user would want to navigate away and stay, it is a page.

## 3. The five-step workflow, as implemented

The right-hand card of the running app shows these five lines and marks the one that is running with `▶`
(`ShowWorkflowStep(int)` in `TicketsWindow.cs`):

| Step | Lesson | Where in the code |
|---|---|---|
| 1 · Button click | user chooses New or Edit | `btnNew_Click` / `btnEdit_Click` in `TicketsWindow.cs` (`async void`). Edit runs its guard first: `GetSelectedTicket() == null` → message, stop |
| 2 · Open dialog | show the modal form | `var dialog = new TicketDialog();` or `new TicketDialog(selected)`, then `var result = await dialog.ShowDialogAsync();` |
| 3 · Validate | check the required fields | `TicketDialog.ValidateForm()` — called first thing in `btnSave_Click`; `return` when it fails, the dialog stays open |
| 4 · Save | call the service layer | dialog: `TicketResult = BuildTicketFromFields(); DialogResult = DialogResult.OK; Close();` — page: `if (result == DialogResult.OK) ticketService.AddTicket(dialog.TicketResult)` / `UpdateTicket(...)` |
| 5 · Refresh UI | update the grid, show feedback | `RefreshTicketGrid()` (`ticketsBindingSource.DataSource = ticketService.GetTickets(); ResetBindings(false)`) then `ShowSuccess("Ticket created.")` / `"Ticket updated."` |

Two things the lesson insists on, and where they are enforced:

- **The dialog never saves.** `btnSave_Click` only validates, builds the `Ticket` and returns `OK`.
  `ticketService.AddTicket / UpdateTicket` are called by the page, inside the `if (result == DialogResult.OK)`
  block. Cancel leaves `TicketResult` null and the page's `else` branch only logs "cancelled — nothing saved".
- **The grid refreshes only after a successful save.** `RefreshTicketGrid()` is called from the `OK` branches,
  from Delete after `DialogResult.Yes`, and from the explicit Refresh button — never after Cancel or after a
  failed validation (the dialog is still open then, so the page has not even continued past the `await`).

The Wisej.NET shape of "modal": `ShowDialogAsync()` returns a `Task<DialogResult>`. The handler is `async void`,
awaits the task, and the code after the `await` runs when the dialog closes. There is no blocking `ShowDialog()`
in Wisej.NET (the server never blocks a request while a browser window is open), which is why the course's
`btnNew_Click` reads `var result = await dialog.ShowDialogAsync();`.

### One dialog for both New and Edit

`TicketDialog(Ticket ticket = null)`:

- `null` → `Text = "Create Ticket"`, fields blank, `cboStatus = "Open"`, `cboPriority = "Medium"`.
- a ticket → `Text = "Edit Ticket #n"`, every field pre-filled; `BuildTicketFromFields()` keeps the original
  `Id` and `CreatedDate` so `UpdateTicket` replaces the right row. A new ticket gets `Id = 0` (the service
  assigns the next one in `AddTicket`) and `CreatedDate = DateTime.Now`.

## 4. Message boxes and confirmations

| Situation | Lesson recommendation | In this project |
|---|---|---|
| Save completed | short success message + refresh the list | `ShowSuccess(message)`: `AlertBox.Show(..., TopRight, 4 s)` + green `lblStatus` + log line, right after `RefreshTicketGrid()` |
| Required field missing | clear validation message near the field or at the top of the dialog | `lblValidation` (red, hidden until needed) above the buttons lists every problem; focus moves to the first bad field |
| Delete selected ticket | ask for confirmation before deleting | `btnDelete_Click`: `await MessageBox.ShowAsync($"Delete ticket #{id}?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)`; only `DialogResult.Yes` calls `ticketService.DeleteTicket(id)` |
| Edit / Delete with nothing selected | show a message and stop | guard at the top of the handler: amber `lblStatus` "Select a ticket to edit." + `AlertBox` warning; no dialog opens |
| Cancel after changing values | optional: warn that unsaved changes will be lost | deliberately not implemented — Cancel is silent here so the Save/Cancel contract stays simple; the event log records "cancelled — nothing saved" |

## Evidence — what the running app shows

| Path | Steps | Event log (right card) | Screen |
|---|---|---|---|
| Create (success) | New Ticket → fill Title, Customer → Save | `btnNew_Click → step 1`, `step 2 Open dialog → … await ShowDialogAsync()`, `step 3 Validate → ValidateForm() = true → … DialogResult.OK → Close()`, `step 4 Save → DialogResult.OK → ticketService.AddTicket("…") → Id 7`, `step 5 Refresh UI → RefreshTicketGrid() → GetTickets() = 7 rows → ResetBindings(false)`, `ShowSuccess("Ticket created.")` | grid shows row #7 selected; toast "Ticket created." top-right; status green; workflow card shows `✓ ✓ ✓ ✓ ▶` |
| Create (validation failure) | New Ticket → leave Title blank → Save | `step 3 Validate → ValidateForm() = false → "Title is required." → return (dialog stays open, nothing saved)`; status red "validation failed — dialog stays open" | the dialog is still open, `lblValidation` in red: *Please fix the following before saving: • Title is required.*; grid unchanged, no step 4/5 lines |
| Recovery | type a Title → Save | the success lines above | ticket appears, toast, green status |
| Create (cancel) | New Ticket → Cancel (or Escape) | `dialog returned DialogResult.Cancel → Create cancelled — nothing saved, grid not refreshed` | amber status; grid unchanged; no `RefreshTicketGrid` line |
| Edit (success) | select a row → Edit Ticket (or double-click) → change Status → Save | `step 2 Open dialog → new TicketDialog(ticket #n) → fields pre-filled`, `… ticketService.UpdateTicket(#n) → Status "Closed" …`, `step 5 Refresh UI …`, `ShowSuccess("Ticket updated.")` | the same row shows the new value and stays selected |
| Edit guard | Clear selection → Edit Ticket | `guard: no row selected → "Select a ticket to edit." → return (no dialog opened)` | amber status "Select a ticket to edit."; warning toast; no dialog |
| Delete (confirm) | select → Delete Ticket → Yes | `await MessageBox.ShowAsync("Delete ticket #n?", YesNo, Question)`, `DialogResult.Yes → ticketService.DeleteTicket(n)`, refresh line, `ShowSuccess("Ticket #n deleted.")` | row gone |
| Delete (decline) | select → Delete Ticket → No | `DialogResult.No → Delete cancelled — ticket #n kept, grid not refreshed` | row still there, amber status |
