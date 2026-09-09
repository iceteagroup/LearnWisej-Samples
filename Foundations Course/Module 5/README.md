# WisejTrainingApp · Wisej.NET Foundations · Module 5

Local lab build for **Module 5 · Dialogs and the modal workflow / Validation and the Save-Cancel pattern**.
It is the Module 4 ticket screen (`dgvTickets` bound through a `BindingSource`, six seeded tickets in a
`TicketService`) with the module's one addition: a reusable `TicketDialog` used for both **New** and **Edit**,
a `ValidateForm()` that blocks bad input with a clear message, a `btnSave_Click` that returns `DialogResult.OK`
only after a valid save, and a Tickets page that adds/updates through the service and refreshes the grid only
when the dialog returned OK. Delete asks for confirmation with `MessageBox.ShowAsync`. The right-hand card shows
the five-step workflow (Button click → Open dialog → Validate → Save → Refresh UI) with the running step marked
`▶`, above the course's usual event log.

The project is self-contained — it rebuilds the Module 4 screen here rather than referencing the Module 4 folder.
Nothing is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 5/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5085
```

Then open <http://localhost:5085>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## What to try

| Action | Path | What you should see |
|---|---|---|
| **New Ticket** → fill Title and Customer → **Save** (or Enter) | success (create) | a *Create Ticket* dialog centred on the page with Status = Open, Priority = Medium; after Save the grid shows the new row (#7) selected, a toast "Ticket created." top-right, the status green, and the log: `step 2 Open dialog …`, `step 3 Validate → ValidateForm() = true …`, `step 4 Save → … AddTicket("…") → Id 7`, `step 5 Refresh UI → RefreshTicketGrid() → 7 rows` |
| **New Ticket** → leave Title blank → **Save** | failure (validation) | the dialog stays open; a red box above the buttons: *Please fix the following before saving: • Title is required.*; cursor back in Title; the log shows `ValidateForm() = false → "Title is required." → return (dialog stays open, nothing saved)`; no step 4/5 lines, grid unchanged |
| type a Title → **Save** | recovery | the create success path above — the same dialog, nothing was lost |
| **New Ticket** → **Cancel** (or Escape) | cancel | dialog closes, status amber "create cancelled — nothing saved", the log says the grid was **not** refreshed |
| select a row → **Edit Ticket** (or double-click the row) → change Status → **Save** | success (edit) | *Edit Ticket #n* pre-filled with the row's values; after Save the same row shows the new Status and stays selected; toast "Ticket updated."; log `… ticketService.UpdateTicket(#n) → Status "Closed" …` |
| **Clear selection (then try Edit → guard)** → **Edit Ticket** | failure (guard) | no dialog; amber status *Select a ticket to edit.*, a warning toast, and the log line `guard: no row selected → … → return (no dialog opened)` |
| select a row → **Delete Ticket** → **Yes** | success (confirm) | a *Confirm delete* message box "Delete ticket #n?" with Yes/No; Yes removes the row, toast "Ticket #n deleted." |
| select a row → **Delete Ticket** → **No** | decline | row kept; log `DialogResult.No → Delete cancelled — ticket #n kept, grid not refreshed` |
| **Refresh** | – | `RefreshTicketGrid()` re-reads the service; status "grid refreshed — n tickets" |
| **Reset sample data** | recovery | a fresh `TicketService` with the six seeded tickets |
| **Clear log** | – | empties the event log |

The workflow card (top of the right-hand column) redraws on every step: `▶` on the running step, `✓` on the
ones already done, blank when idle. The event log under it is the usual course card: every server-side decision,
with a timestamp.

## Where things live

```
WisejTrainingApp/
├─ Program.cs                     Wisej.NET session entry point: new TicketsWindow().Show()
├─ TicketsWindow.cs               code-behind: btnNew_Click / btnEdit_Click / btnDelete_Click (async void),
│                                 RefreshTicketGrid, GetSelectedTicket, ShowSuccess, ShowWorkflowStep, AddLog
├─ TicketsWindow.Designer.cs      Designer layout: header row (lblPageTitle, lblStatus), dgvTickets + six columns,
│                                 ticketsBindingSource, command row (btnNew, btnEdit, btnDelete, btnRefresh),
│                                 workflow + event-log card, bottom bar
├─ Dialogs/
│  ├─ TicketDialog.cs             TicketDialog(Ticket ticket = null), ValidateForm, BuildTicketFromFields,
│  │                              btnSave_Click, btnCancel_Click, TicketResult
│  └─ TicketDialog.Designer.cs    txtTitle, txtCustomer, cboStatus, cboPriority, txtAssignedTo, txtDescription,
│                                 lblValidation, btnSave (AcceptButton), btnCancel (CancelButton)
├─ Models/Ticket.cs               Id, Title, Customer, Status, Priority, AssignedTo, CreatedDate, Description
├─ Services/TicketService.cs      GetTickets, GetTicket, AddTicket (assigns Id), UpdateTicket, DeleteTicket, SaveTicket
├─ Startup.cs                     Kestrel host (app.UseWisej(), static files from the project folder)
├─ Default.json / Default.html / Web.config / Properties/launchSettings.json (port 5085)
└─ docs/
   ├─ DialogWorkflow.md           dialog vs page, the five steps with the method names, message boxes, evidence
   └─ ValidationRules.md          validation placement (UI / business / persistence), the Save handler,
                                  the Save/Cancel checklist, control naming, evidence
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the project (continue from the Module 4 screen) | the same grid screen rebuilt here: `TicketsWindow.Designer.cs` (`dgvTickets`, `ticketsBindingSource`), `Models/Ticket.cs`, `Services/TicketService.cs` |
| 2 · Create the dialog `TicketDialog` with Title, Customer, Assigned To, Description, Status, Priority, Save / Cancel | `Dialogs/TicketDialog.Designer.cs` — a `Form` with `FormBorderStyle = Fixed`, `StartPosition = CenterParent`, no minimize/maximize |
| 3 · Name controls clearly | `txtTitle`, `txtCustomer`, `txtAssignedTo`, `txtDescription`, `cboStatus`, `cboPriority`, `lblValidation`, `btnSave`, `btnCancel` |
| 4 · Reuse for create & edit (optional `Ticket` in the constructor) | `TicketDialog(Ticket ticket = null)` in `Dialogs/TicketDialog.cs` — null → "Create Ticket", Open / Medium; ticket → "Edit Ticket #n", fields pre-filled |
| 5 · Validate first (`ValidateForm()` blocks save with a clear message when Title or Status is missing) | `TicketDialog.ValidateForm()` — Title required + ≤ 80 chars, Status required, Customer required → `lblValidation` |
| 6 · Save & return OK | `TicketDialog.btnSave_Click`: `if (!ValidateForm()) return; TicketResult = BuildTicketFromFields(); DialogResult = DialogResult.OK; Close();` |
| 7 · Open from the Tickets page (`btnNew` / `btnEdit` → on OK `AddTicket` / `UpdateTicket`, refresh, success message) | `TicketsWindow.btnNew_Click` / `btnEdit_Click` — `await dialog.ShowDialogAsync()` → `if (result == DialogResult.OK) { ticketService.AddTicket(...) / UpdateTicket(...); RefreshTicketGrid(); ShowSuccess(...); }` |
| 8 · Guard edit | top of `btnEdit_Click`: `GetSelectedTicket() == null` → `lblStatus` "Select a ticket to edit." + `AlertBox` → `return` |
| 9 · Keep logic in the service | `Services/TicketService.cs` owns the list, Id assignment, add/update/delete; the handlers only call it |
| 10 · Run & test (validation blocks bad input, grid refreshes only after a successful save) | the "What to try" table; `RefreshTicketGrid()` is called only inside the `DialogResult.OK` / `DialogResult.Yes` branches and from Refresh |

Lab code check (`labs.js` m5): the Save handler is `btnSave_Click`; it calls `ValidateForm()` (which uses
`IsNullOrWhiteSpace`); it sets `DialogResult = DialogResult.OK`; feedback goes through `lblStatus`, `AlertBox.Show`
and `MessageBox.ShowAsync`; storage goes through `ticketService.AddTicket` / `UpdateTicket` / `DeleteTicket`
(and `SaveTicket` exists for the lesson's one-call form).

## Self-check

Lesson s21 — *You're ready for the next reading*:

- **What is a dialog?** A smaller, modal screen on top of the main page for one focused task — here, filling in one
  ticket. It collects the values, validates them and returns a clear result (`DialogResult.OK` with `TicketResult`
  set, or `Cancel`); it never tries to be the whole application.
- **When a dialog instead of a page?** When the user should complete or cancel the task and come straight back to
  where they were (Create Ticket, Edit Ticket, Confirm delete). Screens users stay on and navigate between (the
  Tickets grid itself) are pages.
- **The five-step modal workflow?** Button click (`btnNew_Click` / `btnEdit_Click`) → Open dialog
  (`await dialog.ShowDialogAsync()`) → Validate (`ValidateForm()`) → Save (`DialogResult.OK`, then
  `ticketService.AddTicket` / `UpdateTicket` on the page) → Refresh UI (`RefreshTicketGrid()` + `ShowSuccess`).
- **Where do message boxes help?** Immediate feedback after a save (the toast), a clear validation message
  (`lblValidation`), and confirmation before a risky action (`MessageBox.ShowAsync` YesNo before
  `DeleteTicket`).

Lesson s22 — *You're ready for the lab*:

- **Where does validation belong?** Simple field checks close to the UI in `ValidateForm()`; business rules
  and persistence rules in `TicketService`, where they can be tested without a button click.
- **What makes a Save handler readable?** Four lines and three helpers: `ValidateForm()`, `BuildTicketFromFields()`,
  `DialogResult = OK; Close()`. No storage code, no hidden slow calls.
- **The consistent Save/Cancel pattern?** Save validates first, then the service is called, then the dialog closes
  only after success; Cancel closes without saving; messages say exactly what to fix; the parent refreshes the
  grid only when the dialog returned OK; button names are `btnSave` / `btnCancel` / `btnNew` / `btnEdit`.

## Verified / unverified

Built with `dotnet build -nologo -v q` (0 warnings, 0 errors) on Wisej-4 4.1.0 / .NET 10; not yet run in the
browser by the author — the reviewer runs it. Calls that the cookbook marks *unverified at runtime*:

- Wisej.NET has no blocking `ShowDialog()`. This sample uses `var result = await dialog.ShowDialogAsync();` in
  `async void` handlers (`btnNew_Click`, `btnEdit_Click`) and expects the continuation to run when the dialog
  closes with `DialogResult.OK` / `Cancel`. `Application.Update(this)` is called at the end of each handler so
  anything changed after the `await` is pushed to the browser.
- `await MessageBox.ShowAsync(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question)` in `btnDelete_Click`
  and the `DialogResult.Yes` / `No` it returns.
- `Form.AcceptButton` / `CancelButton` on the dialog (Enter saves, Escape cancels), `FormBorderStyle.Fixed`,
  `StartPosition = CenterParent` while the dialog is shown with `ShowDialogAsync()`.
- `BindingSource` bound to a `List<Ticket>` with `AutoGenerateColumns = false` and six `DataGridViewTextBoxColumn`s
  (`DataPropertyName`), `ResetBindings(false)` after `DataSource` is replaced, `AutoSizeColumnsMode = Fill` with
  `FillWeight`s, `SelectedRows[0].DataBoundItem as Ticket`, `DataGridViewRow.Selected = true` to re-select the saved
  row, `ClearSelection()`.
- `DockStyle.Top` / `Bottom` / `Fill` inside a `Panel` with `Padding` (header row, grid, command row).
- `TicketDialog.ValidationChecked` is a plain C# event raised from `ValidateForm()`; the page's handler updates
  `lstEventLog` / `lblStatus` during the dialog's Save request, which should render as part of that request.
