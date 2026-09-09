# Data binding notes — the model, the service and the BindingSource (lab deliverable)

The Module 4 lab turns a static screen into a data-driven one: a list of tickets in a `DataGridView`, the
selected ticket in detail controls, a Save button that goes through a service and refreshes the grid on
purpose. This is how the pieces are split in `TicketsWindow`.

## 1. The layers (lesson s16 §1)

| Layer | What belongs there | In this sample |
|---|---|---|
| Model | The shape of the data object. | `Models/Ticket.cs` — Id, Title, Customer, Status, Priority, AssignedTo, CreatedDate, Description. Properties only. |
| Service / repository | Get, save, add, update, delete. The rules. | `Services/TicketService.cs` — `GetTickets()`, `GetTicket(id)`, `AddTicket`, `UpdateTicket`, `DeleteTicket`, `SaveTicket` (add or update). Validation lives here. |
| UI page | Controls, binding, selection, messages. | `TicketsWindow.cs` / `.Designer.cs` — `dgvTickets`, `grpTicketDetails`, `btnSaveTicket`, `btnRefresh`, `lblStatus`, the event log. |
| Persisted data | Where data eventually lives. | The service's private `List<Ticket>` — one instance per user session. A database would replace it without touching the form. |

Why keep them apart: the form never decides whether a ticket is valid, and the service never touches a
control. Swap the list for a database and `TicketsWindow` does not change; redesign the screen and
`TicketService` does not change.

## 2. The model

```csharp
public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Customer { get; set; }
    public string Status { get; set; }       // Open | In Progress | Closed
    public string Priority { get; set; }     // Low | Medium | High
    public string AssignedTo { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Description { get; set; }
}
```

Plain properties, no `INotifyPropertyChanged`: the lesson's model. That is why a refresh has to be
asked for — see §5.

## 3. The service

`GetTickets()` returns a **fresh list of copies**, and `SaveTicket()` copies the values back into the
store after validating. That small choice is what makes the three kinds of state (§7) visibly
different in the running app: an edit changes the bound copy (business state) and nothing else until
Save writes it to the service (persisted data).

```csharp
public void SaveTicket(Ticket ticket)          // add or update
{
    if (ticket == null) throw new ArgumentException("No ticket is selected.");
    if (ticket.Id == 0) AddTicket(ticket); else UpdateTicket(ticket);
}

private static void Validate(Ticket ticket)
{
    if (string.IsNullOrWhiteSpace(ticket.Title))
        throw new ArgumentException("Title is required.");
    if (!Statuses.Contains(ticket.Status))
        throw new ArgumentException("Status must be Open, In Progress or Closed.");
    …
}
```

The service throws `ArgumentException` with a message written for a person; the form catches it and
decides *how* to show it (red status label + toast). Six tickets are seeded in the constructor
(Northwind, Contoso, Fabrikam, Adventure Works, Tailspin, Wide World Importers).

## 4. The BindingSource pattern (lesson s16 §4)

```csharp
private TicketService ticketService = new TicketService();
private BindingSource ticketsBindingSource = new BindingSource();

private void LoadTickets()
{
    ticketsBindingSource.DataSource = ticketService.GetTickets();
    dgvTickets.DataSource = ticketsBindingSource;
}

private Ticket SelectedTicket
{
    get { return ticketsBindingSource.Current as Ticket; }
}
```

The grid has `AutoGenerateColumns = false` and six explicit `DataGridViewTextBoxColumn`s whose
`DataPropertyName` is the model property (`Id`, `Title`, `Customer`, `Status`, `Priority`,
`AssignedTo`). `SelectionMode = FullRowSelect`, `MultiSelect = false`, `ReadOnly = true`: clicking a
row selects a whole ticket, and edits happen in the detail panel, never in a grid cell.

Both fields are **instance** fields of the form: one service and one BindingSource per user session.

## 5. Binding the detail controls (lesson s16 §5)

```csharp
txtTitle.DataBindings.Add("Text", ticketsBindingSource, "Title", true, DataSourceUpdateMode.OnPropertyChanged);
txtCustomer.DataBindings.Add("Text", ticketsBindingSource, "Customer", true, DataSourceUpdateMode.OnPropertyChanged);
cmbStatus.DataBindings.Add("Text", ticketsBindingSource, "Status", true, DataSourceUpdateMode.OnPropertyChanged);
cmbPriority.DataBindings.Add("Text", ticketsBindingSource, "Priority", true, DataSourceUpdateMode.OnPropertyChanged);
txtAssignedTo.DataBindings.Add("Text", ticketsBindingSource, "AssignedTo", true, DataSourceUpdateMode.OnPropertyChanged);
dtpCreated.DataBindings.Add("Value", ticketsBindingSource, "CreatedDate", true, DataSourceUpdateMode.OnPropertyChanged);
txtDescription.DataBindings.Add("Text", ticketsBindingSource, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

ticketsBindingSource.CurrentChanged += ticketsBindingSource_CurrentChanged;
```

Every detail control binds to the **same** `BindingSource`, so when the user selects another row the
controls follow the current ticket without any grid-cell code. `cmbStatus` and `cmbPriority` are
`DropDownList` combos, so only the allowed values can be chosen from the UI (the service still checks —
a rule belongs in one place, and that place is not the combo box).

`BindDetailControls()` runs once, in `Load`, before the first `LoadTickets()`. Refresh calls
`LoadTickets()` again and the bindings stay: they point at the BindingSource, not at the list.

## 6. Bind once, refresh intentionally (lesson s16 §6)

```csharp
private void btnSaveTicket_Click(object sender, EventArgs e)
{
    Ticket ticket = ticketsBindingSource.Current as Ticket;
    try
    {
        ticketService.SaveTicket(ticket);
        ticketsBindingSource.ResetBindings(false);
        lblStatus.Text = "Saved ticket #" + ticket.Id;
    }
    catch (ArgumentException ex)
    {
        SetStatus("Not saved — " + ex.Message, StatusKind.Error);   // red label + toast
    }
}
```

The screen does not magically know about business changes:

- After a **Save**, `ResetBindings(false)` re-reads the bound objects so the grid row shows the edited
  Title/Status.
- **Refresh** (`btnRefresh_Click`) calls `LoadTickets()` again: a new snapshot from the service, unsaved
  edits dropped, and tickets added behind the screen's back become visible.
- **Add a ticket through the service** (`btnAddSample_Click`) calls `ticketService.AddTicket(...)` and
  nothing else — the status label and the State card say the service has 7 while the grid still shows 6,
  until Refresh. That is the demonstration of the rule.

### Safe event handling

`ticketsBindingSource_CurrentChanged` sets a label and writes a log line — nothing else. No service
calls hide inside `TextChanged`, `SelectionChanged` or `CurrentChanged`; the real work happens in
`btnSaveTicket_Click` and in `TicketService`, so behaviour is predictable and the log reads in order.

## 7. UI state vs business state vs persisted data (lesson s16 §7)

The "State" card under the grid (`lblState`, monospace) is recomputed by `UpdateStateBox()` after every
action:

```
UI state       : row 2 of 6 selected · lblStatus = "Selected ticket #2"
Business state : Ticket #2 Status = "In Progress" · Title = "Invoice PDF missing logo" · unsaved edits: none
Persisted data : TicketService holds 6 tickets · #2 Status = "In Progress" · Title = "Invoice PDF missing logo" (saved copy)
```

| State type | Meaning | Where it is read from |
|---|---|---|
| UI state | What the screen shows or selects. | `ticketsBindingSource.Position`, `dgvTickets.Rows.Count`, `lblStatus.Text` |
| Business state | The current object values the app is working with. | `ticketsBindingSource.Current as Ticket` — the bound copy, including unsaved edits |
| Persisted data | The saved copy outside the UI. | `ticketService.Count`, `ticketService.GetTicket(id)` |

Change Status in the combo and watch the middle line change while the bottom line does not; click
Save and they agree again.

## Evidence

| Action | Event log line | Screen |
|---|---|---|
| App starts | `LoadTickets → ticketsBindingSource.DataSource = ticketService.GetTickets() (6 tickets) → dgvTickets.DataSource = ticketsBindingSource` then `CurrentChanged → Position 0 → Selected ticket #1 "Login page times out on VPN"` | six rows in the grid, row 1 selected, the detail controls show ticket #1, status green *Selected ticket #1* |
| Click row 3 | `ticketsBindingSource.CurrentChanged → Position 2 → Selected ticket #3 "Add CSV export to the orders grid"` | detail controls follow; State card: UI row 3 of 6 |
| Change Status to *Closed* (leave the combo) | — (a small UI event; no log, no service call) | State card: Business `Status = "Closed"`, `unsaved edits: Status`; Persisted still `Open` |
| **Save Ticket** | `btnSaveTicket_Click → ticketService.SaveTicket(#3 "…", Closed) ✓ → ResetBindings(false) → lblStatus = "Saved ticket #3"` | grid row 3 shows *Closed*, status green *Saved ticket #3*, toast, State card: business and persisted agree |
| **Save with blank title (validation)** | `txtTitle.Text = "" → the OnPropertyChanged binding pushed a blank Title …` then `ticketService.SaveTicket threw ArgumentException: "Title is required." → nothing persisted` | status red *Not saved — Title is required.*, warning toast, focus in `txtTitle`; State card: Business Title blank, `unsaved edits: Title`, Persisted Title unchanged |
| Type a title, **Save Ticket** | the success line again | status green *Saved ticket #n* — recovery through the same handler |
| **Refresh** (instead of retyping) | `btnRefresh_Click → Grid refreshed from TicketService (unsaved edits discarded, …)` | the saved title is back in `txtTitle` and in the grid, same row reselected |
| **Add a ticket through the service** | `ticketService.AddTicket → #7 persisted (7 in the service); grid still bound to the earlier list (6 rows) → click Refresh` | status amber; State card: `TicketService holds 7 tickets` while UI says `of 6` |
| **Refresh** | `Grid refreshed from TicketService` | seven rows |
