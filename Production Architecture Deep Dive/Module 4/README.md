# TicketOps · Production Architecture Deep Dive · Module 4

Local lab build for **Module 4 · Data Binding, DataGridView & Data-Oriented Workflows**. It follows the
walkthrough video *Build a data-bound Work Order grid*: an observable `WorkOrder` model
(`INotifyPropertyChanged`) held in a `BindingList<WorkOrder>` behind a `Wisej.Web.BindingSource`, a
`DataGridView` bound to it with formatted columns (currency, date, status/priority colours), a search
box and a status filter that narrow the grid through `IWorkOrderService.Filter`, a master-detail editor
whose fields are bound to the same `BindingSource` (typing updates the grid row live), dirty tracking
with an **● Unsaved changes** indicator, and a Save / Discard pair that commits or rolls back through the
service — with every path (success, validation, empty result, data outage, recovery) visible to the user
without leaking internals.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 4\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5104
```

Then open <http://localhost:5104>. (Visual Studio: open `TicketOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with no warnings for both targets (`net10.0-windows`, `net10.0`).

## What to click in the Work Orders window

The left card is the Work Orders screen (toolbar, grid, detail editor); the right card is the
**Activity trace · UI → Service → Data · binding events**: every click is logged as it crosses a boundary
(`[UI]` → `[SVC]` → `[DATA]` / `[DOMAIN]` → `[UI]`), and every `PropertyChanged` that the `BindingList` turns
into a grid repaint shows up as a `[UI] BindingList.ListChanged` line — so you can see that no handler
copied a value into the grid.

| Button / action | Path | What you should see |
|---|---|---|
| *(load)* | success | `[SVC] WorkOrderService.LoadAsync → GetAllAsync`, `[DATA] 6 rows`, `[SVC] Filter … → 6 of 6 match`, `[UI] RebindVisible — BindingList rebuilt with 6 rows … one ResetBindings`, `[UI] workOrderSource.CurrentChanged — current → #2001`; the grid shows `$240.00`-style costs, `Jun 14`-style dates, coloured Status/Priority, #2005's Due cell tinted red (overdue); the editor shows **Work Order 2001** |
| **Type `pump`** in the search box (or pick a status) | filter | `[UI] textSearch.TextChanged → IWorkOrderService.Filter(6 rows, {text:"pump", status:All})`, `[SVC] … → 1 of 6 match (… master list untouched)`, `[UI] RebindVisible — … 1 rows`; count **1 of 6 work orders**; the detail follows the new current row |
| **Click row 2002**, then edit **Title** / **Cost** / **Due date** / **Assigned to** | master-detail, live | `[UI] workOrderSource.CurrentChanged — current → #2002 …`; on each edit `[UI] BindingList.ListChanged — #2002.Title PropertyChanged → ItemChanged(row 1) → grid cell repaints · IsDirty = True`; the grid cell changes as you edit (Cost as `$2,150.00`), the Title turns amber, the header shows **● Unsaved changes**, Save / Discard enable |
| **Status → In Progress** (combo) | manual conversion | the Status cell text and colour change; the same `ListChanged` line — the combo handler wrote `current.Status`, the binding did the rest |
| **Save** | success | `[UI] buttonSave_Click — workOrderSource.EndEdit() → IWorkOrderService.SaveAsync(#2002)`, `[SVC] validate …`, `[SVC] valid → UpsertAsync(#2002)`, `[DATA] #2002 written (6 rows)`, `[DOMAIN] WorkOrder.AcceptChanges — #2002 snapshot taken → IsDirty = false`, `[UI] OK · Work order #2002 saved.`; the indicator clears, the Title goes back to black; status **● Work order #2002 saved.** |
| **Discard** (after another edit) | rollback | `[UI] buttonDiscard_Click → IWorkOrderService.Discard(#2002)`, `[DOMAIN] WorkOrder.RejectChanges — #2002 restored to the saved snapshot — every setter raised PropertyChanged, nothing persisted`, one `ListChanged` line per restored property; the fields and the row revert; **● Changes to work order #2002 discarded.** |
| **↻ Reload** while a row is dirty | guard (rule) | `[UI] ⚠ … 1 row(s) with unsaved changes — reload refused`; orange banner **Save or discard the unsaved changes before reloading.**; nothing reloaded |
| **▶ Import 60 work orders** | progress | a `Timer` awaits `ImportBatchAsync(n, 5)` per tick: `[DATA] #2007 written …`, `[SVC] ImportBatchAsync — 5 generated work orders written (#2007–#2011), returned clean`, `[UI] timerImport_Tick — 5 of 5 added to the BindingList → ItemAdded × 5 → grid now 11 rows`; the grid grows on its own, the progress bar and **● importing 25/60** advance; ends with **● 60 work orders imported**, count **66 of 66** |
| **Save with an empty title** | failure (validation) | `[UI] … #2002.Title = "" written on the object — the bound field and the grid cell empty themselves → SaveAsync`, `[SVC] ⚠ rejected: Title is required. — edits kept, IsDirty stays true` — no `[DATA]` line; orange banner **Title is required.**; status **● not saved**; the empty title is still on screen and the row is still dirty (click **Discard** to restore it) |
| **Search with no matches** | empty result (not a failure) | `[UI] … search "turbine" — expect 0 matches …`, `[SVC] Filter — … → 0 of 6 match — an empty result, not an error`; grey banner **No work orders match the search…**, empty grid, the editor is disabled and reads *Select a work order to edit it here.*; the button becomes **Clear the search** — click it and every row comes back with no reload |
| **Simulate data outage**, edit a row, **Save** | error path | `[UI] outage ON …`; on Save: `[DATA] ✖ outage: UPDATE WorkOrders WHERE Id=2002 failed — timeout connecting to sql01:1433 (TicketOps.dbo.WorkOrders)` stays in the trace; `[UI] ✖ buttonSave_Click — caught DataOutageException — user sees the safe message`, `[UI] ⚠ … edits kept: #2002 still dirty = True`; the user sees only the red banner **✖ Your changes could not be saved and are still on screen. Check the log for details.** and a toast; the edits are still in the fields and the row |
| **Recover the data store** (same button), **Save** | recovery | `[UI] outage OFF (recovery) …`; Save now goes through: `[DATA] #2002 written`, `[DOMAIN] AcceptChanges`, indicator clears |
| **Clear trace** | — | empties the right-hand card |

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Observable `WorkOrder` model (or view model) | `Domain/WorkOrder.cs` — `INotifyPropertyChanged` through one `SetField` helper (equality guard), derived `IsOverdue` announced from its inputs, dirty snapshot (`AcceptChanges` / `RejectChanges` / `IsDirty`); `Domain/WorkOrderQuery.cs` (the filter rule) |
| 2 | `BindingSource` configuration | `Views/WorkOrdersPage.cs` → `InitializeBinding()`: `BindingList<WorkOrder>` → `workOrderSource.DataSource` → `dgvWorkOrders.DataSource` (`AutoGenerateColumns = false`) + four `DataBindings.Add(…, DataSourceUpdateMode.OnPropertyChanged)`; the `BindingSource`, `Timer` and columns (`DataPropertyName`) are in `Views/WorkOrdersPage.Designer.cs` — see [`docs/BindingDecisions.md`](TicketOps/docs/BindingDecisions.md) |
| 3 | Grid formatting handlers | `Views/WorkOrdersPage.Designer.cs` (`columnCost.DefaultCellStyle.Format = "C2"`, `columnDueDate … "MMM d"`) + `WorkOrdersPage.dgvWorkOrders_CellFormatting` (status/priority text and colour, "Unassigned", overdue tint, unsaved amber title) |
| 4 | Master-detail editor | `Views/WorkOrdersPage` — grid and detail fields share `workOrderSource`; `workOrderSource_CurrentChanged` → `RefreshDetail()` for the header, the two enum combos and the dirty state |
| 5 | Commit / cancel behaviour notes | [`docs/CommitCancelNotes.md`](TicketOps/docs/CommitCancelNotes.md); the code: `Services/WorkOrderService.SaveAsync` / `Discard`, `WorkOrdersPage.buttonSave_Click` / `buttonDiscard_Click` |
| — | Search / filter through a service | `Services/IWorkOrderService.Filter` + `WorkOrderService.Filter` over `WorkOrderQuery.Matches`; `WorkOrdersPage.ApplyFilter` / `RebindVisible` |
| — | Dirty tracking + Save/Discard through `IWorkOrderService` | `WorkOrder.IsDirty`, `WorkOrdersPage.UpdateDirtyState`, `Strings.UnsavedChanges`; `WorkOrderService.SaveAsync` (validate → persist → `AcceptChanges`), `Discard` (→ `RejectChanges`) |
| — | Every path visible without leaking internals | `WorkOrdersPage.ShowResult` / `ReportFailure`, `Resources/Strings.cs` (`SaveFailedEditsKept`, `NoMatches`, `ReloadBlockedByUnsaved`), the trace panel |
| — | Data workflow + diagram | [`docs/DataWorkflow.md`](TicketOps/docs/DataWorkflow.md) + [`docs/DataWorkflow.svg`](TicketOps/docs/DataWorkflow.svg) |
| — | Designer & code review checklist, applied | [`docs/ReviewChecklist.md`](TicketOps/docs/ReviewChecklist.md) |

## Where things live

```
TicketOps/
├─ Views/
│  ├─ WorkOrdersPage.cs             the screen: InitializeBinding, CellFormatting, CurrentChanged → RefreshDetail, thin handlers
│  └─ WorkOrdersPage.Designer.cs    GENERATED-style layout: BindingSource, Timer, grid columns by DataPropertyName, column formats
├─ Controls/StatusBanner            reusable "● state" + banner UserControl (display only)
├─ Services/
│  ├─ IWorkOrderService.cs          LoadAsync · Filter · SaveAsync · Discard · ImportBatchAsync (no UI types)
│  └─ WorkOrderService.cs           validation, filter, commit (AcceptChanges) / rollback (RejectChanges), persistence orchestration
├─ Domain/
│  ├─ WorkOrder.cs                  observable record: INotifyPropertyChanged, IsOverdue, IsDirty snapshot; compiles without Wisej.NET
│  ├─ WorkOrderQuery.cs             what the toolbar collects + the matching rule
│  └─ OperationResult.cs            success / safe explanation handed back to the screen
├─ Data/
│  ├─ IWorkOrderRepository.cs       persistence contract
│  └─ InMemoryWorkOrderRepository.cs fake store (copies in, copies out), the six video rows; SimulateOutage throws like a real driver
├─ Infrastructure/
│  ├─ ILog.cs / ActivityLog.cs      cross-cutting logging (details stay here)
│  └─ AppComposition.cs             who gets what: one object graph per session, constructor injection, no statics
├─ Resources/Strings.cs             safe user-facing messages
├─ Diagnostics/ActivityTracePanel   the live trace card
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
  the new item. When a *property of the current item changes* (a Discard rolls values back, the import
  service sets an id), `PropertyChanged` on the `WorkOrder` — the `BindingList` relays it as
  `ListChanged(ItemChanged)`, the grid repaints the cell and the bound field refreshes. No notification,
  no refresh: that is why every setter goes through `SetField`.
- **What happens if Save fails after the UI changed?**
  Nothing is lost and nothing is lied about. The edits live on the object (the grid already shows them),
  but the *saved state* is the snapshot taken by `AcceptChanges`, which runs only after the repository
  accepted the record. A validation failure returns `Fail("Title is required.")`; an outage throws before
  `AcceptChanges`; in both cases `IsDirty` stays true, the amber indicator stays, and the banner says the
  changes are still on screen. Press **Discard** to roll back, or fix and **Save** again.
