# SupportDesk · Data Binding with EF Core · Module 4

Local lab build for **Module 4 · Two-Way Binding and CRUD Editors**: the ticket browser from Module 3 gains
**Add Ticket** and **Edit Ticket**, which open the modal `TicketEditorForm`. The form binds a
`TicketEditModel` through `editBindingSource`; Save calls `EndEdit`, a validation placeholder, then
`TicketCommandService.SaveAsync` (fresh context, load or add, map, `SaveChangesAsync`); Delete confirms and
loads the ticket by key in a fresh context. The grid re-runs its search only on `DialogResult.OK`.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 4/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5404
```

Tests: `dotnet test SupportDesk.Tests`.

## What to try

Browser: the Module 3 controls plus `btnAdd` and `btnEdit`. Editor: `txtTitle`, `cboCustomer`,
`cboCategory`, `dtpDueDate`, `chkIsUrgent`, `btnDelete`, `btnCancel`, `btnSave`, with `editBindingSource`
and `errorProvider`.

- **Add Ticket**: fill Title, Customer, Category, Save: the dialog closes and the new ticket is on top of page 1.
- **Edit Ticket** on a row, change the title and tick *Escalate as urgent*, Save: the grid shows the saved row.
- **Cancel**: nothing is written and the grid is not reloaded.
- **Empty title, Save**: the validation placeholder puts an `errorProvider` icon on `txtTitle`; no context is created.
- **Double-click Save**: the `_saving` guard drops the second click.
- **Delete**, answer Yes: the ticket is removed. On a *Closed* ticket the business rule refuses it with a message.
- **Already deleted**: open the same ticket in two browser tabs, delete it in one, then Save or Delete in the
  other: *This ticket was already deleted by someone else*, and the dialog closes so the grid refreshes.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | `TicketEditModel` and `TicketEditorForm` with a `BindingSource` and an `ErrorProvider` | [`docs/EditorFormAndBindingSource.md`](docs/EditorFormAndBindingSource.md) · `TicketEditModel.cs`, `TicketEditorForm(.Designer).cs` |
| 2 | Controls bound to the edit model | [`docs/ControlDataBindings.md`](docs/ControlDataBindings.md) · `TicketEditorForm.Designer.cs`, `BindLookupControls` |
| 3 | `LoadEditorAsync` and `SaveAsync` with `EndEdit`, mapping and `SaveChangesAsync` | [`docs/LoadAndSaveFlows.md`](docs/LoadAndSaveFlows.md) · `TicketEditorForm`, `TicketCommandService` |
| 4 | Delete with confirmation, a fresh load and already-deleted handling | [`docs/DeleteConfirmationAndReload.md`](docs/DeleteConfirmationAndReload.md) · `TicketEditorForm.DeleteAsync`, `TicketCommandService.DeleteAsync` |
| 5 | Grid refreshed after `DialogResult.OK`; Cancel persists nothing | [`docs/DialogResultAndGridRefresh.md`](docs/DialogResultAndGridRefresh.md) · `TicketBrowserPage.OpenEditorAsync` |

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the Module 3 solution | the browser is unchanged apart from Add and Edit |
| 2 · `TicketEditModel` without `Number`, `CreatedAt`, `UpdatedAt` | `SupportDesk.Services/TicketEditModel.cs` |
| 3 · `TicketEditorForm(int? ticketId)` with the named controls | `TicketEditorForm(.Designer).cs`; data access goes through `[Inject] TicketCommandService` |
| 4 · Lookups first, then `DataBindings.Add` with explicit update modes | `TicketEditorForm.Designer.cs` (Text/Checked), `BindLookupControls` (SelectedValue) |
| 5 · `LoadEditorAsync` for new and existing tickets | `TicketEditorForm.LoadEditorAsync`, `TicketCommandService.LoadEditModelAsync` |
| 6 · `SaveAsync` pipeline with separate concurrency and database catches | `TicketEditorForm.SaveAsync`, `TicketCommandService.SaveAsync` |
| 7 · `DeleteAsync` with confirmation and a fresh load | `TicketEditorForm.DeleteAsync`, `TicketCommandService.DeleteAsync` |
| 8 · `btnAdd` / `btnEdit` re-run the search only on OK | `TicketBrowserPage.OpenEditorAsync` |
| 9 · Show every path | see **What to try** |

`dtpDueDate` is copied by hand (`Checked`/`Value`) instead of bound, because `DueDate` is a `DateTime?`.
The lab has the form inject `IDbContextFactory` directly; this sample keeps the context inside
`TicketCommandService` so the form never sees one (same lifetime, one layer further out).

## Self-check answers

- **What lifetime did you choose for the editor's `DbContext`, and when would a form-scoped context be acceptable?**
  One per operation: lookups, load, save and delete each create and dispose their own context inside
  `TicketCommandService`. A form-scoped context would only be acceptable for a short-lived form editing one
  aggregate with guaranteed one-at-a-time operations and explicit disposal; it would also serve stale
  tracked values and survive a failed connection.
- **What is bound, what lives only in the edit model, what stays only in the database, and why is `Number` not editable?**
  Bound: Title, CustomerId, CategoryId, DueDate (by hand), IsUrgent. Model only: `Id` (and the other edit-model
  fields this screen does not show). Database only: `Number`, `CreatedAt`, `UpdatedAt`, `RowVersion` and the
  navigation graph. `Number` is assigned once on insert and referenced everywhere; leaving it out of the edit
  model means no control can ever change it.
- **Which failure modes are handled, and what does the operator see?**
  Double click: dropped by `_saving`. Already deleted: a friendly message and the dialog closes with OK so
  the grid refreshes. Database rejection: *could not be saved because the database rejected the change* in an
  `AlertBox`, dialog stays open. Concurrent edit: `DbUpdateConcurrencyException` is caught with a message;
  real detection arrives in Module 7 with the `RowVersion` round trip.
