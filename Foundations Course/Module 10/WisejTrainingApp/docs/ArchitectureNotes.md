# Architecture notes — how data moves through the mini helpdesk

Lesson s46 §3 asks you to explain the capstone end to end: "the user clicks Create/Edit, the dialog opens,
validation runs, TicketService updates the ticket list, the DataGridView refreshes, and the status or
deployment notes record what changed." This note maps that sentence to the real files in this project.
The **Architecture** screen (`Views/ArchitectureView.cs`) shows the same list on the left and lets you select
each layer to read its job.

## 1. The data-flow list (s46 §3) → the real files

| Lesson line | In this project | What it does — and what it must not do |
|---|---|---|
| `Program.cs → starts Window1` | `Program.cs` — `static void Main(NameValueCollection args) { new Window1().Show(); }` | `Default.json` names `WisejTrainingApp.Program.Main` as the startup; `Main` opens the shell and nothing else. |
| `Window1.cs → shell, navigation, pages` | `Window1.cs` + `Window1.Designer.cs` — `pnlHeader` (64 px), `pnlNav` (220 px, eight buttons), `pnlContent` (Fill), `lblStatus` (36 px). `BuildScreens()`, `Register()`, `NavButton_Click`, `NavigateTo(string)`, `SetActiveButton`, `AddActivity`, `SetStatus`, `TicketsChanged` | Draws the frame, swaps the one visible `HelpdeskView` in `pnlContent`, keeps the activity log. It creates the two services (`TicketService`, `CustomerService`) once per session and hands them to the views. It never decides what a valid ticket is. |
| `Ticket.cs → ticket data model` | `Models/Ticket.cs` — `Id, Title, Customer, Status, Priority, AssignedTo, CreatedDate, Description` + `Clone()`; `Models/Customer.cs` — `Id, Name, Company, Email` | Plain properties. The grid binds to them, the dialog reads and writes them, the validator checks them. No behaviour besides `Clone()` (a copy for Edit, so Cancel never touches the row the grid shows). |
| `TicketDialog → create/edit form + validation` | `Dialogs/TicketDialog.cs` — `TicketDialog(Ticket ticket = null)`, `LoadTicket`, `ReadTicket`, `ValidateForm()`, `FocusFirstInvalid`, `btnSave_Click`, `btnCancel_Click`, `TicketResult`, `IsEdit`; `Dialogs/TicketDialog.Designer.cs` — `txtTitle, txtCustomer, cboStatus, cboPriority, txtAssignedTo, txtDescription, lblValidation, btnSave (AcceptButton), btnCancel` | One `Form` for New and Edit. `ValidateForm()` asks `TicketValidator` — the dialog has no rules of its own — and Save closes with `DialogResult.OK` only when the result is valid. The dialog never calls `TicketService`; the screen that opened it decides between Add and Update. |
| `TicketService → CRUD logic and a fake repository` | `Services/TicketService.cs` — `GetTickets, GetTicket, AddTicket, UpdateTicket, DeleteTicket, SaveTicket, GetSummary, GetTicketsForCustomer`, private `Guard`; `Services/TicketRepository.cs` — `GetAll, Get, Add (assigns Id), Update, Delete`; `Services/TicketValidator.cs` — `Validate(Ticket) → ValidationResult` | The service validates again (`Guard`) before storing, then talks to the repository. The repository is the only class that owns the `List<Ticket>` (an instance field — one per session, never static). Swap `TicketRepository` for a database and nothing above it changes. |
| `DataGridView → displays tickets` | `Views/TicketsView.cs` — `ConfigureGrid()` (seven hand-declared columns, `AutoGenerateColumns = false`), `ticketsBindingSource`, `RefreshTicketGrid()`, `SelectedTicket`, `btnCreateTicket_Click`, `btnEditTicket_Click`, `btnDeleteTicket_Click`, `btnRefresh_Click`, `UpdateSelectionHint` | `dgvTickets` is read-only, full-row select. It never edits data in place: every change goes dialog → service → `RefreshTicketGrid()` (re-read from the service, `ResetBindings(false)`, `ClearSelection()`). |
| `Theme → polished UI` | `Window1.cs` — `cboTheme_SelectedIndexChanged` → `Application.LoadTheme(name)`, `lblCurrentTheme`; `Default.json` `"theme": "Bootstrap-4"`; spacing numbers in the `Window1` class comment; shared colours in `Views/HelpdeskView.cs` | The header combo restyles every screen live (Bootstrap-4, BootstrapDark-4, Blue-1, Material-3). A failed `LoadTheme` shows a safe message in the status bar and the detail in the activity log. |
| `Deployment → release checklist` | `Views/DeploymentView.cs` — `RequiredChecks[9]`, `chkRequired`, `UpdatePackageStatus()`, `lblPackageStatus`, `btnCheckAll`, `btnResetChecklist`, `lblDeploymentNotes`; `docs/DeploymentChecklist.md`, `docs/ReadinessNote.md` | The s42 §4 required list as a gate: `lblPackageStatus` reads NOT READY until 9 / 9. The notes card says what each check means for this project. |

The flow in one line, using the real names:

```
btnCreateTicket_Click / btnEditTicket_Click   (Views/TicketsView.cs)
  → new TicketDialog(...) → await dialog.ShowDialogAsync()
      → btnSave_Click → ValidateForm() → TicketValidator.Validate(candidate)   (Dialogs/TicketDialog.cs)
      → DialogResult.OK + TicketResult
  → ticketService.AddTicket / UpdateTicket → Guard() → TicketRepository.Add / Update   (Services/)
  → RefreshTicketGrid() → ticketsBindingSource.DataSource = ticketService.GetTickets()
  → lblStatus (green) + Shell.AddActivity(...) + Shell.TicketsChanged() → DashboardView.RefreshSummary()
```

Nothing skips a layer: the dialog never touches the service, the shell never touches a ticket, the repository
never touches a control.

## 2. Layers — one job each

| Layer | Files | Knows about | Must not know about |
|---|---|---|---|
| Entry | `Program.cs`, `Startup.cs`, `Default.json`, `Default.html`, `Web.config` | which form opens first, the Kestrel host, the theme name | tickets, controls |
| Shell | `Window1.cs`, `Window1.Designer.cs`, `Views/IHelpdeskShell.cs`, `Views/HelpdeskView.cs` | the eight screens by name, the services (creates them), the status bar, the activity log | validation rules, the ticket list |
| Screens (views) | `Views/DashboardView`, `TicketsView`, `CustomersView`, `JobsView`, `ArchitectureView`, `CodeReviewView`, `DeploymentView`, `NextStepsView` (`.cs` + `.Designer.cs`) | their own controls, the service they were given, the shell through `IHelpdeskShell` | `Window1`'s controls, other screens, the repository |
| Dialogs | `Dialogs/TicketDialog.cs`, `.Designer.cs` | its fields, `TicketValidator`, `TicketResult` | `TicketService`, the grid |
| Services | `Services/TicketService.cs`, `TicketValidator.cs`, `CustomerService.cs` | models, rules, the repository | any `Wisej.Web` control |
| Repository | `Services/TicketRepository.cs` | the `List<Ticket>` and Id assignment | validation, the UI |
| Models | `Models/Ticket.cs`, `Models/Customer.cs` | their own properties | everything else |

How a screen talks upward: `IHelpdeskShell` has four members — `AddActivity(string)`, `SetStatus(text, kind)`,
`NavigateTo(screen)`, `TicketsChanged()`. Keeping that interface small is what makes each view testable and
what stops screens from reaching into `Window1`'s controls.

## 3. Capstone feature checklist (s46 §2) — evidence in this app

| Feature area | Required evidence | Where it is in this project |
|---|---|---|
| Navigation shell | Dashboard, Tickets, Architecture, Code Review, Deployment and Next Steps open from one consistent left nav | `pnlNav` with `btnDashboard … btnNextSteps` (plus Customers and Jobs), all wired to one `NavButton_Click`; `NavigateTo()` is the only place a screen is shown; every call is logged to the Dashboard's `lstActivity` |
| Ticket CRUD | Create, edit and delete ticket records; a DataGridView refreshes after changes | `TicketsView.btnCreateTicket_Click / btnEditTicket_Click / btnDeleteTicket_Click` → `TicketService.AddTicket / UpdateTicket / DeleteTicket` → `RefreshTicketGrid()`; Delete asks first with `MessageBox.ShowAsync` YesNo |
| Dialogs & validation | Create/Edit Ticket dialogs validate required fields and show clear success/error feedback | `TicketDialog.ValidateForm()` → `lblValidation` lists every message (Title required, ≤ 80 chars, Customer required, Status/Priority in range, Closed needs an assignee); the page shows "Ticket created successfully." / "Ticket #n updated successfully." in `lblStatus` |
| Service layer | TicketService handles ticket operations instead of putting all logic in UI events | `Services/TicketService.cs` + `TicketRepository.cs` + `TicketValidator.cs`; the handlers in `TicketsView.cs` contain no rule about tickets |
| UI polish | Theme applied, spacing cleaned up, labels readable, buttons grouped consistently | `cboTheme` → `Application.LoadTheme`; the spacing numbers in the `Window1` header comment (header 64 / nav 220 / status 36 / padding 24 / card gap 16 / control gap 8); command buttons top-right of each card in the order primary, secondary, danger, neutral; every screen has a `lblStatus` coloured through `HelpdeskView.ShowStatus` |
| Deployment readiness | Release checklist covers configuration, security, logging, theme/static resources and target notes | `DeploymentView` (nine required checks + `lblPackageStatus` gate + `lblDeploymentNotes`), `docs/DeploymentChecklist.md`, `docs/ReadinessNote.md` |

Two extra screens go beyond the checklist so the capstone shows the whole course: **Customers** (Module 4/5
grid + inline validated form through `CustomerService`) and **Jobs** (Module 7 async job with progress,
cancellation and a safe simulated error).

## Evidence

What the running app shows for this note:

- **Architecture screen**: the left card prints the eight-line data-flow list; the right card's `lstLayers`
  holds the same eight entries. Selecting one fills `lblLayerTitle`, `lblLayerDetail` (the job) and
  `lblLayerFile` ("Where: …"); `lblStatus` reads `● layer n of 8 — <files>`.
- **Dashboard → Recent activity** after a Create: `NavigateTo("Tickets") → TicketsView shown in pnlContent`,
  then `btnCreateTicket_Click → TicketDialog OK → TicketService.AddTicket(#9 "…") → RefreshTicketGrid()` —
  one line per arrow in the flow above, in order.
- **Dashboard cards** (`lblTotal / lblOpen / lblInProgress / lblClosed`) change after every Create / Edit /
  Delete without the Dashboard counting anything itself: `Window1.TicketsChanged()` →
  `DashboardView.RefreshSummary()` → `TicketService.GetSummary()`. On first load they read 8 / 4 / 2 / 2.
- **Second browser tab** = second session: it gets its own `Window1`, its own `TicketService` and its own
  eight seeded tickets; a ticket created in one tab does not appear in the other (the repository is an instance
  field, not static).
