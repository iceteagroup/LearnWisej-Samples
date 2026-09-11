# WisejTrainingApp · Wisej.NET Foundations · Module 10 (Capstone)

The mini helpdesk from **Module 10 · Capstone**, as the lesson and walkthrough describe it: one left-navigation
shell with Dashboard, Tickets, Architecture, Code Review, Deployment and Next Steps; ticket create / edit /
delete in a `DataGridView` with one reusable `TicketDialog`; validation rules in a `TicketValidator` shared by
create and edit; a `TicketService` over a fake repository; and the architecture, code-review, deployment and
next-steps notes screens.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 10/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5090
```

Then open <http://localhost:5090>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

## What to try

| Action | What you should see |
|---|---|
| **Dashboard** | total, open, in-progress and closed ticket counts |
| **Tickets → Create**, Save with an empty Title | the validator's message; the dialog stays open |
| **Create** a valid ticket, **Edit** one, **Delete** one | the grid refreshes after each change; the status bar says what happened |
| Back to **Dashboard** | the counts follow the changes |
| **Architecture**, **Code Review**, **Deployment**, **Next Steps** | the data flow, the review checklist, the release checklist, the road to production |

## Where things live

```
Program.cs     ->  starts Window1
Window1.cs     ->  shell, navigation, pages
Ticket.cs      ->  ticket data model
TicketDialog   ->  create/edit form + validation
TicketService  ->  CRUD logic and a fake repository
DataGridView   ->  displays tickets
Theme          ->  polished UI
Deployment     ->  release checklist
```

| File | What it's for |
|---|---|
| `Window1.cs` | The shell and `NavigateTo` |
| `Views/*.cs` | One `UserControl` per screen |
| `Dialogs/TicketDialog.cs` | Create / Edit, validated through `TicketValidator` |
| `Services/TicketService.cs`, `TicketRepository.cs`, `TicketValidator.cs` | Operations, the in-memory list, the rules |
| `Models/Ticket.cs` | The shape of one ticket |
