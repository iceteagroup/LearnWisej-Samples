# WisejTrainingApp · Wisej.NET Foundations · Module 5

The Create / Edit ticket workflow from **Module 5 · Dialogs, Validation & Workflows**, as the walkthrough
video builds it: one `TicketDialog` for both New and Edit, `ValidateForm()` that keeps the dialog open until
the required fields are filled, a Save that returns `DialogResult.OK`, and a Tickets window that saves
through `TicketService` and refreshes the grid only after a successful save.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 5/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5085
```

Then open <http://localhost:5085>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

## What to try

| Action | What you should see |
|---|---|
| **New Ticket**, leave Title empty, **Save** | *Please enter a title.* — the dialog stays open |
| Fill Title and Status, **Save** | the dialog closes, the new ticket appears in the grid, *Ticket created.* |
| Select a row, **Edit Ticket**, change a field, **Save** | the grid row updates, *Ticket saved.* |
| **Cancel** in the dialog | nothing is saved, the grid is unchanged |

## Where things live

| File | What it's for |
|---|---|
| `Models/Ticket.cs` | The shape of one ticket |
| `Services/TicketService.cs` | `GetTickets`, `AddTicket`, `UpdateTicket`, `DeleteTicket` |
| `Dialogs/TicketDialog.cs` | The dialog: optional `Ticket` constructor, `ValidateForm()`, `BuildTicketFromFields()`, Save / Cancel |
| `TicketsWindow.cs` | `btnNew_Click` / `btnEdit_Click` — `await ShowDialogAsync()`, then the service and a grid refresh |
