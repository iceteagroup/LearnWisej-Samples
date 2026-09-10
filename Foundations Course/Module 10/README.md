# WisejTrainingApp · Wisej.NET Foundations · Module 10 (Capstone)

Local lab build for **Module 10 · Capstone: the mini helpdesk**. `Program.cs` starts `Window1`, a shell that
owns only the left navigation, the theme selector, the status bar and the activity log; every screen is a
`HelpdeskView` (a `UserControl`) swapped into the content panel by one `NavigateTo`. Tickets are created,
edited and deleted through one `TicketDialog`, validated by `TicketValidator`, stored by `TicketService`
over a fake `TicketRepository`; the dashboard cards recount after every change. Jobs, Architecture, Code
Review, Deployment and Next Steps round out what the course taught.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 10/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5090
```

Then open <http://localhost:5090>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Click every nav button | navigation | the content panel swaps, the active button is highlighted, the status bar and the Dashboard's activity list log `Opened … screen` |
| **Tickets → Create Ticket**, fill Title + Customer, Save | success | `TicketDialog` returns `DialogResult.OK`, `TicketService.AddTicket` runs, the grid refreshes, status "Ticket created successfully.", Dashboard counts change |
| **Create Ticket**, Save with a blank Title | failure | `TicketValidator` lists what to fix in red, the dialog stays open, nothing is saved |
| Fix the Title and Save | recovery | the ticket appears in `dgvTickets` |
| **Edit Ticket** / **Delete Ticket** with no row selected | failure (guard) | a status message asks for a selection; no dialog opens |
| Select a row → **Edit Ticket**, change Status, Save | success | the same dialog pre-filled as "Edit Ticket #n"; `UpdateTicket` + refresh |
| Select a row → **Delete Ticket** | confirmation | `MessageBox.ShowAsync` asks *Delete ticket #n?*; **Yes** deletes and refreshes, **No** logs the cancel |
| **Jobs → Start job** | progress | the progress bar and job log advance step by step while the handler awaits; **Cancel job** stops it cleanly |
| **Jobs → Simulate error**, Start job | failure | the log gets the exception detail, the user sees a short safe message, buttons reset in `finally` |
| **Deployment**: tick items | not ready → ready | the package status turns green only when every required check is ticked; **Reset checklist** goes back |
| Header theme selector | theme | `Application.LoadTheme` restyles the whole app; every workflow still works afterwards |

## Where things live

```
WisejTrainingApp/
├─ Program.cs                     new Window1().Show()
├─ Window1.cs / .Designer.cs      the shell: nav buttons → NavigateTo, SetActiveButton, cboTheme, AddActivity, SetStatus
├─ Models/Ticket.cs, Customer.cs  plain data classes
├─ Services/
│  ├─ TicketRepository.cs         the fake repository — the only class that owns the ticket list
│  ├─ TicketService.cs            GetTickets / AddTicket / UpdateTicket / DeleteTicket / SaveTicket / GetSummary
│  ├─ TicketValidator.cs          reusable rules shared by create and edit (ValidationResult)
│  └─ CustomerService.cs
├─ Dialogs/TicketDialog(.Designer).cs   one dialog for New and Edit: ValidateForm → TicketResult → DialogResult.OK
├─ Views/
│  ├─ HelpdeskView.cs, IHelpdeskShell.cs   base view + the small interface the shell exposes to views
│  ├─ DashboardView   summary cards (total / open / in progress / closed) + recent activity
│  ├─ TicketsView     dgvTickets + Create / Edit / Delete / Refresh
│  ├─ CustomersView   customer list + Add customer
│  ├─ JobsView        background job with progress, cancel, simulated error, SetJobRunning / LogError
│  ├─ ArchitectureView, CodeReviewView, DeploymentView, NextStepsView   the notes screens
└─ docs/
   ├─ ArchitectureNotes.md        data flow mapped to files, layer table, feature checklist with evidence
   ├─ CodeReviewChecklist.md      the peer-review focus and where a reviewer looks in this project
   ├─ DeploymentChecklist.md      the release checklist applied to this project
   ├─ NextSteps.md                what a production version still needs
   └─ ReadinessNote.md            the short deployment/readiness note for submission
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Start the shell | `Program.cs` → `new Window1().Show()`; nav buttons in `Window1.Designer.cs`, `NavigateTo` in `Window1.cs` |
| 2 · Add the screens | `Views/*View.cs`, registered in `Window1.BuildScreens()` |
| 3 · Dashboard cards | `Views/DashboardView.cs` ← `TicketService.GetSummary()` |
| 4 · Ticket model & service | `Models/Ticket.cs`, `Services/TicketService.cs` over `Services/TicketRepository.cs` |
| 5 · Tickets grid | `Views/TicketsView.cs` — `dgvTickets`, `RefreshTicketGrid()` after every change |
| 6 · Create/Edit dialogs | `Dialogs/TicketDialog.cs` — `TicketDialog(Ticket ticket = null)`, `btnSave_Click`, `btnCancel_Click` |
| 7 · Reusable validation | `Services/TicketValidator.cs`, used by the dialog and guarded again in `TicketService` |
| 8 · Polish the UI | `cboTheme_SelectedIndexChanged` → `Application.LoadTheme`; `SetActiveButton`; the spacing constants commented in the shell |
| 9 · Notes screens | `ArchitectureView`, `CodeReviewView`, `DeploymentView`, `NextStepsView` + the `docs/` files |
| 10 · Clean code habits | small handlers (`btnCreateTicket_Click`, `btnStartJob_Click`) that call services/helpers; safe messages via `SetStatus` |
| 11 · Run & test the whole flow | the "What to try" table above |

Lab code check (`labs.js` m10): `btnCreateTicket_Click` … `_Click` handlers, `TicketService.AddTicket/UpdateTicket/DeleteTicket`,
`TicketDialog` + `ShowDialogAsync` + `DialogResult`, `RefreshTicketGrid` / `dgvTickets`, `lblStatus` + `MessageBox` — all in `Views/TicketsView.cs`.

## Course skills → where they are used

| Module | Skill | In the capstone |
|---|---|---|
| 1 | design → name → handle event → run | every control is named (`btnCreateTicket`, `dgvTickets`, `lblStatus`), handlers in code-behind |
| 2 | properties, events, `AddLog` helper | `AddActivity` / `SetStatus` in the shell; one helper, short handlers |
| 3 | shell + `NavigateTo` + UserControl views + permissions as a concept | `Window1` + `Views/`; `lblUser` shows the role idea |
| 4 | model + service + BindingSource + DataGridView | `Ticket`, `TicketService`, `TicketsView.ConfigureGrid/RefreshTicketGrid` |
| 5 | one dialog for New/Edit, `ValidateForm`, `DialogResult.OK`, refresh after success | `TicketDialog`, `btnCreateTicket_Click`, `btnEditTicket_Click`, `btnDeleteTicket_Click` |
| 6 | async job, progress, try/catch/finally, safe messages, per-user state | `JobsView` |
| 7 | theme selector, active nav state, cards, consistent spacing | `cboTheme`, `SetActiveButton`, `DashboardView` |
| 8 | keep rules in C# | no JavaScript in the capstone; every rule lives in `Services/` |
| 9 | release checklist, package status, safe logging | `DeploymentView`, `docs/DeploymentChecklist.md` |

## Self-check

- **Navigation shell** — Dashboard, Tickets, Customers, Jobs, Architecture, Code Review, Deployment and Next Steps all open from one left nav through `NavigateTo`.
- **Ticket CRUD** — create, edit and delete go through `TicketService`; the grid and the dashboard counts refresh after each change.
- **Dialogs & validation** — `TicketDialog` validates through `TicketValidator` and reports clearly; Save returns `DialogResult.OK` only after a valid ticket.
- **Service layer** — `TicketService` + `TicketRepository` own the data; views and the shell never touch the list.
- **UI polish** — theme applied and switchable, spacing constants shared, buttons grouped where the content is.
- **Deployment readiness** — `DeploymentView` and `docs/DeploymentChecklist.md` cover configuration, secrets, logging, themes/static resources and the target.
- **Peer-review focus** — naming, separation of concerns, navigation, usability and deployment: see `docs/CodeReviewChecklist.md` for where a reviewer looks.

## Verified / unverified

Built on this machine (`dotnet build`, 0 errors). Run in the browser by the reviewer: navigation, create/edit/delete
with the dialog and the delete confirmation (`ShowDialogAsync`, `MessageBox.ShowAsync`), the job's progress pushed
with `Application.Update`, the deployment checklist and the theme switch (`Application.LoadTheme`) are the
calls to watch — the Module 5, 6 and 7 samples verified the same calls in isolation.
