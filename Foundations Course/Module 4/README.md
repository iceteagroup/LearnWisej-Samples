# WisejTrainingApp · Wisej.NET Foundations · Module 4

Local lab build for **Module 4 · Data binding, DataGridView and the service layer / SplitContainer, Dock
and AutoSize**. It is the lab's ticket-management screen: a `Ticket` model, a `TicketService` that owns
get / save / add / update / delete, a `SplitContainer` with a read-only `DataGridView` on the left bound
through one `BindingSource`, detail controls on the right that follow the selected row, and a **Save
Ticket** button that goes through the service and refreshes the grid on purpose — plus the course's usual
status label, event-log card, and a "State" card that shows UI state, business state and persisted data
side by side.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 4/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5084
```

Then open <http://localhost:5084>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## What to try

| Action | Path | What you should see |
|---|---|---|
| App starts | load | six tickets in `dgvTickets` (Id, Title, Customer, Status, Priority, Assigned To), row 1 selected, the detail controls filled from ticket #1, `lblStatus` green *Selected ticket #1*, the log shows `LoadTickets → …GetTickets() (6 tickets)` and `CurrentChanged → Position 0` |
| Click another row | selection | the detail controls follow through the BindingSource; `lblStatus` = *Selected ticket #n*; the State card's first line says `row n of 6 selected` |
| Change Status / Priority / Title, leave the field | business state | the State card's middle line shows the new value and `unsaved edits: Status`; the bottom line (the service's saved copy) has not changed; the grid row has not changed either — nothing asked it to |
| **Save Ticket** | success | `ticketService.SaveTicket(#n …) ✓ → ResetBindings(false)`; the grid row shows the edit, `lblStatus` green *Saved ticket #n*, a toast, and the State card's business and persisted lines agree again |
| **Save with blank title (validation)** | failure | `txtTitle` is blanked, the same `btnSaveTicket_Click` runs, `TicketService` throws `ArgumentException("Title is required.")`; `lblStatus` red *Not saved — Title is required.*, a warning toast, focus back in `txtTitle`, log: `→ nothing persisted`; the State card shows the blank business Title against the unchanged saved copy |
| Type a title, **Save Ticket** | recovery 1 | the success path again, through the same handler |
| **Refresh** | recovery 2 / intentional refresh | `LoadTickets()` runs again: the saved title is back, the same ticket is reselected, log `Grid refreshed from TicketService (unsaved edits discarded, …)` |
| **Add a ticket through the service** (State card) | bind once, refresh intentionally | `ticketService.AddTicket` persists #7; `lblStatus` amber *Ticket #7 added in the service — the grid still shows 6 rows; click Refresh*; the State card says `TicketService holds 7 tickets` while the UI line says `of 6` — then **Refresh** shows seven rows |
| Drag the splitter / resize the browser | layout | the grid and the log grow and shrink, the details group keeps its height, the six grid columns re-share the width (`AutoSizeColumnsMode = Fill`) |
| **Clear log** | – | empties the event log |

The event-log card (under the details) logs every server-side decision with a timestamp: binding set-up,
selection changes, service calls, the exception message, and what was *not* refreshed.

## Where things live

```
WisejTrainingApp/
├─ Program.cs                    Wisej.NET session entry point: new TicketsWindow().Show()
├─ TicketsWindow.cs              code-behind: LoadTickets, SelectedTicket, BindDetailControls, CurrentChanged,
│                                btnSaveTicket_Click, btnRefresh_Click, btnSimulateBadSave_Click, btnAddSample_Click,
│                                UpdateStateBox, AddLog/SetStatus helpers
├─ TicketsWindow.Designer.cs     Designer-generated layout: panelHeader (Dock Top), splitContainer1 (Dock Fill, Vertical,
│                                SplitterDistance 760) → Panel1: dgvTickets (Dock Fill) + State card (Dock Bottom);
│                                Panel2: grpTicketDetails (Dock Top) + event-log card (Dock Fill)
├─ Models/Ticket.cs              the record: Id, Title, Customer, Status, Priority, AssignedTo, CreatedDate, Description
├─ Services/TicketService.cs     GetTickets / GetTicket / AddTicket / UpdateTicket / DeleteTicket / SaveTicket; validation;
│                                six seeded tickets; in-memory list per instance (per session)
├─ Startup.cs                    Kestrel host (app.UseWisej(), static files from the project folder)
├─ Default.json / Default.html / Web.config / Properties/launchSettings.json (port 5084)
└─ docs/
   ├─ DataBindingNotes.md        layers, model, service, the BindingSource pattern, binding the detail controls,
   │                             bind once / refresh intentionally, safe event handling, the three state types, evidence
   └─ LayoutNotes.md             SplitContainer steps, Dock table, AutoSize table, "layout tools are not data", evidence
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the project | `WisejTrainingApp.csproj` (Wisej-4 4.1.0, `net10.0-windows;net10.0`), `WisejTrainingApp.slnx`, port 5084 in `Properties/launchSettings.json` |
| 2 · Model the record | `Models/Ticket.cs` — the eight properties, nothing else |
| 3 · Service layer | `Services/TicketService.cs` — `GetTickets()`, `SaveTicket()` (+ `GetTicket`, `AddTicket`, `UpdateTicket`, `DeleteTicket`), `List<Ticket>` seeded with six tickets in `Seed()` |
| 4 · Layout | `TicketsWindow.Designer.cs` — `splitContainer1.Dock = Fill`, `Orientation = Vertical`, `SplitterDistance = 760`; grid in Panel1, details in Panel2 |
| 5 · Grid | `dgvTickets` — `Dock = Fill`, `AutoGenerateColumns = false`, six `DataGridViewTextBoxColumn`s with `DataPropertyName`, `FullRowSelect`, `MultiSelect = false`, `ReadOnly = true`, `AutoSizeColumnsMode = Fill` |
| 6 · Detail controls | `grpTicketDetails` — `txtTitle`, `txtCustomer`, `cmbStatus`, `cmbPriority` (DropDownList), `txtAssignedTo`, `dtpCreated`, `txtDescription`, `btnSaveTicket` ("Save Ticket"), `btnRefresh`, `btnSimulateBadSave` |
| 7 · Bind it | `LoadTickets()` (`ticketsBindingSource.DataSource = ticketService.GetTickets(); dgvTickets.DataSource = ticketsBindingSource;`) and `BindDetailControls()` (`DataBindings.Add("Text"/"Value", ticketsBindingSource, …, true, OnPropertyChanged)`) in `TicketsWindow.cs` |
| 8 · Save & refresh | `btnSaveTicket_Click` — `ticketsBindingSource.Current as Ticket` → `ticketService.SaveTicket(ticket)` → `ticketsBindingSource.ResetBindings(false)` → `lblStatus.Text = "Saved ticket #" + ticket.Id` (try/catch around the service call) |
| 9 · Run & test | `dotnet run -f net10.0 --urls http://localhost:5084`; select rows, edit, Save, watch the grid, the status label and the State card |

Lab code check (`labs.js` m4): the handler is `btnSaveTicket_Click`; it uses `ticketsBindingSource.Current`
and `DataSource`; it works with `dgvTickets` / `Ticket`; it calls `ticketService.SaveTicket` (and `GetTickets`
in `LoadTickets`); it refreshes with `ResetBindings(false)` and `DataSource =`.

## Self-check

**s16 — "You're ready": model a ticket, keep data logic in a service, bind a list to a grid, show the selected record in detail controls, refresh on purpose.**

- **Why do `Ticket.cs` and `TicketService.cs` live apart from the page?** The model describes data (properties
  only), the service owns data logic and the rules (`SaveTicket` validates and adds or updates), and the page only
  handles display and user actions. Swap the in-memory list for a database and `TicketsWindow` does not change.
- **How does a list get into the grid?** `ticketsBindingSource.DataSource = ticketService.GetTickets();
  dgvTickets.DataSource = ticketsBindingSource;` — the grid is never given the list directly.
- **How do the detail controls follow the selection?** They bind to the same `BindingSource`
  (`txtTitle.DataBindings.Add("Text", ticketsBindingSource, "Title", true, DataSourceUpdateMode.OnPropertyChanged)`),
  so `ticketsBindingSource.Current` is what both the grid row and every detail control show.
- **What is `SelectedTicket`?** `ticketsBindingSource.Current as Ticket` — the record, not a grid cell.
- **What does "refresh on purpose" mean?** The screen does not know when business data changes; after Save the
  handler calls `ResetBindings(false)`, and Refresh calls `LoadTickets()` again. The "Add a ticket through the
  service" button shows the opposite: a change the service made stays invisible until Refresh.
- **Where should expensive work go?** Not in `TextChanged` / `SelectionChanged` / `CurrentChanged`
  (`ticketsBindingSource_CurrentChanged` only sets a label and logs). In Save buttons and service methods.
- **UI state, business state, persisted data?** Selected row and `lblStatus` text; the bound `Ticket` object's
  current values (including unsaved edits); the copy held by `TicketService`. The State card prints all three.

**s17 — beginner rules for the layout tools.**

- **SplitContainer:** `Dock = Fill`, `Orientation = Vertical`, grid in Panel1, editor in Panel2, then adjust
  `SplitterDistance` (760 here).
- **Dock:** don't set every control to `Fill`. Here the container, the grid and the log card are docked; the
  header and the details group are `Top`, the State card is `Bottom`; the eleven input controls keep normal
  positions. Too much `Fill` makes controls cover each other.
- **AutoSize:** `true` for the field captions; `false` for the grid (`Dock = Fill` instead), for the GroupBox and
  cards (stable while editing), for the buttons (consistent widths) and for labels whose text changes at runtime.
  Columns use `AutoSizeColumnsMode = Fill` with `FillWeight`s.
- **Layout tools are not data:** SplitContainer, Dock and AutoSize make the screen usable; the model, the service,
  `BindingSource.Current` and the Save handler are unchanged by any resize.

## Verified / unverified

`dotnet build -nologo -v q` passes for both targets with 0 warnings and 0 errors; the app was **not run** while
building this sample — the reviewer runs it. These calls come from the Wisej.NET docs / reflection over
`Wisej.Framework.dll` 4.1.0 and need the runtime check:

- `Control.DataBindings.Add("Text" | "Value", ticketsBindingSource, propertyName, true, DataSourceUpdateMode.OnPropertyChanged)`
  — in particular that a `TextBox` edit reaches the `Ticket` object before `btnSaveTicket_Click` runs, that the
  `"Text"` binding on a `DropDownList` `ComboBox` selects the matching item and pushes the chosen item back, and that
  `DateTimePicker.Value` binds to `CreatedDate`.
- `BindingSource.CurrentChanged` firing on grid row selection (and its `Position` getter / setter, used by
  `UpdateStateBox` and `SelectTicket`).
- `BindingSource.ResetBindings(false)` refreshing both the grid row and the bound detail controls after Save.
- Whether a **server-side** `txtTitle.Text = ""` is pushed through the binding: `btnSimulateBadSave_Click` checks
  `ticket.Title` afterwards, logs which happened, and sets `ticket.Title = ""` itself when the binding did not, so
  the failure path works either way.
- `SplitterPanel.Padding` on `splitContainer1.Panel1` / `Panel2` insetting the docked children, and the dock
  order (`Fill` added first, `Top` / `Bottom` after) laying out as in WinForms.
- `DataGridViewTextBoxColumn.FillWeight` with `AutoSizeColumnsMode = Fill`, and `RowHeadersVisible = false`.
- `AlertBox.Show(text, icon, alignment: TopRight, autoCloseDelay: …)` and `Application`-free `Form.Load` are
  verified in Module 1 / the Application Integration samples.
