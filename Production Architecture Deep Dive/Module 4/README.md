# TicketOps · Production Architecture Deep Dive · Module 4

Local lab build for **Module 4 · Data Binding, DataGridView & Data-Oriented Workflows**. It follows the
walkthrough video *Build a data-bound Work Order grid*: an observable `WorkOrder` model
(`INotifyPropertyChanged`) held in a `BindingList<WorkOrder>` behind a `Wisej.Web.BindingSource`, a
`DataGridView` bound to it with formatted columns (currency, date, status/priority colours), a search
box and a status filter that narrow the grid through `IWorkOrderService.Filter`, a master-detail editor
whose fields are bound to the same `BindingSource` (typing updates the grid row live), dirty tracking
with an **● Unsaved changes** indicator, and a Save / Discard pair that commits or rolls back through the
service.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 4\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5104
```

Then open <http://localhost:5104>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try in the Work Orders window

| Action | What you should see |
|---|---|
| *(load)* | the grid shows `$240.00`-style costs, `Jun 14`-style dates, coloured Status/Priority, #2005's Due cell tinted red (overdue); the editor shows **Work Order 2001** |
| Type `pump` in the search box, or pick a status | count **1 of 6 work orders**; the detail follows the new current row. A search with no matches (e.g. `turbine`) empties the grid, disables the editor and shows a grey banner — not an error |
| Click row 2002, edit **Title** / **Cost** / **Due date** / **Assigned to** | the grid cell changes as you type (Cost as `$2,150.00`), the Title turns amber, **● Unsaved changes** appears, Save / Discard enable |
| **Status → In Progress** | the Status cell text and colour change |
| **Save** | the indicator clears; status **● Work order #2002 saved.** |
| Clear the Title (or set In Progress with no assignee), **Save** | orange banner **Title is required.**; the edit stays on screen and the row stays dirty |
| **Discard** (after an edit) | the fields and the row revert; **● Changes to work order #2002 discarded.** |
| **↻ Reload** while a row is dirty | orange banner **Save or discard the unsaved changes before reloading.**; nothing reloaded |

If the store throws during Save, the handler logs the details and the user sees
**✖ Your changes could not be saved and are still on screen. Check the log for details.** — the edits stay.

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Observable `WorkOrder` model (or view model) | `Domain/WorkOrder.cs` — `INotifyPropertyChanged` through one `SetField` helper (equality guard), derived `IsOverdue` announced from its inputs, dirty snapshot (`AcceptChanges` / `RejectChanges` / `IsDirty`); `Domain/WorkOrderQuery.cs` (the filter rule) |
| 2 | `BindingSource` configuration | `Views/WorkOrdersPage.cs` → `InitializeBinding()`: `BindingList<WorkOrder>` → `workOrderSource.DataSource` → `dgvWorkOrders.DataSource` (`AutoGenerateColumns = false`) + four `DataBindings.Add(…, DataSourceUpdateMode.OnPropertyChanged)`; the `BindingSource` and columns (`DataPropertyName`) are in `Views/WorkOrdersPage.Designer.cs` — see [`docs/BindingDecisions.md`](TicketOps/docs/BindingDecisions.md) |
| 3 | Grid formatting handlers | `Views/WorkOrdersPage.Designer.cs` (`columnCost.DefaultCellStyle.Format = "C2"`, `columnDueDate … "MMM d"`) + `WorkOrdersPage.dgvWorkOrders_CellFormatting` (status/priority text and colour, "Unassigned", overdue tint, unsaved amber title) |
| 4 | Master-detail editor | `Views/WorkOrdersPage` — grid and detail fields share `workOrderSource`; `workOrderSource_CurrentChanged` → `RefreshDetail()` for the header, the two enum combos and the dirty state |
| 5 | Commit / cancel behaviour notes | [`docs/CommitCancelNotes.md`](TicketOps/docs/CommitCancelNotes.md); the code: `Services/WorkOrderService.SaveAsync` / `Discard`, `WorkOrdersPage.buttonSave_Click` / `buttonDiscard_Click` |
| — | Search / filter through a service | `Services/IWorkOrderService.Filter` + `WorkOrderService.Filter` over `WorkOrderQuery.Matches`; `WorkOrdersPage.ApplyFilter` / `RebindVisible` |
| — | Every path visible without leaking internals | `WorkOrdersPage.ShowResult` / `ReportFailure`, `Resources/Strings.cs` (`SaveFailedEditsKept`, `NoMatches`, `ReloadBlockedByUnsaved`) |
| — | Data workflow + diagram | [`docs/DataWorkflow.md`](TicketOps/docs/DataWorkflow.md) + [`docs/DataWorkflow.svg`](TicketOps/docs/DataWorkflow.svg) |
| — | Designer & code review checklist, applied | [`docs/ReviewChecklist.md`](TicketOps/docs/ReviewChecklist.md) |

## Where things live

```
TicketOps/
├─ Views/
│  ├─ WorkOrdersPage.cs             the screen: InitializeBinding, CellFormatting, CurrentChanged → RefreshDetail, thin handlers
│  └─ WorkOrdersPage.Designer.cs    GENERATED-style layout: BindingSource, grid columns by DataPropertyName, column formats
├─ Controls/StatusBanner            reusable "● state" + banner UserControl (display only)
├─ Services/
│  ├─ IWorkOrderService.cs          LoadAsync · Filter · SaveAsync · Discard (no UI types)
│  └─ WorkOrderService.cs           validation, filter, commit (AcceptChanges) / rollback (RejectChanges), persistence orchestration
├─ Domain/
│  ├─ WorkOrder.cs                  observable record: INotifyPropertyChanged, IsOverdue, IsDirty snapshot; compiles without Wisej.NET
│  ├─ WorkOrderQuery.cs             what the toolbar collects + the matching rule
│  └─ OperationResult.cs            success / safe explanation handed back to the screen
├─ Data/
│  ├─ IWorkOrderRepository.cs       persistence contract
│  └─ InMemoryWorkOrderRepository.cs fake store (copies in, copies out), the six video rows
├─ Infrastructure/
│  ├─ ILog.cs / ActivityLog.cs      logging (written to the server console; details stay there)
│  └─ AppComposition.cs             who gets what: one object graph per session, constructor injection, no statics
├─ Resources/Strings.cs             safe user-facing messages
├─ docs/                            BindingDecisions · DataWorkflow (+ .svg) · CommitCancelNotes · ReviewChecklist
├─ Program.cs                       Wisej.NET session entry point → AppComposition
└─ Startup.cs                       Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **What object owns the current selected row?**
  The `BindingSource` (`workOrderSource.Current`). The grid moves it when a row is clicked; the four bound
  fields read from it; `CurrentChanged` is only used for the extras (header text, the two enum combos, the
  dirty label). Neither the grid nor the screen keeps its own "selected work order" variable.
- **What causes the detail editor to refresh?**
  Two events, for two kinds of change. When the *current item moves*, `CurrentChanged` — the bindings re-read
  the new item. When a *property of the current item changes* (a Discard rolls values back),
  `PropertyChanged` on the `WorkOrder` — the `BindingList` relays it as `ListChanged(ItemChanged)`, the grid
  repaints the cell and the bound field refreshes. No notification, no refresh: that is why every setter
  goes through `SetField`.
- **What happens if Save fails after the UI changed?**
  Nothing is lost and nothing is lied about. The edits live on the object (the grid already shows them),
  but the *saved state* is the snapshot taken by `AcceptChanges`, which runs only after the repository
  accepted the record. A validation failure returns `Fail("Title is required.")`; a store failure throws
  before `AcceptChanges`; in both cases `IsDirty` stays true, the amber indicator stays, and the banner says
  the changes are still on screen. Press **Discard** to roll back, or fix and **Save** again.
