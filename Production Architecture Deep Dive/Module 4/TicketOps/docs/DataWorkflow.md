# Data workflow — load → filter → edit → save / discard

*Module 4 deliverable · TicketOps Console · Work Orders screen*

![Work Orders data workflow](DataWorkflow.svg)

The diagram (`DataWorkflow.svg`) shows the five layers and the direction every value travels. Read it
top-down for user actions and bottom-up for notifications.

## 1. Load

| Step | Layer | What happens | Trace |
|---|---|---|---|
| Screen shown | View | `WorkOrdersPage_Load` → `LoadAsync("WorkOrdersPage.Load")` | `[UI] WorkOrdersPage.Load — screen shown → IWorkOrderService.LoadAsync()` |
| Query the store | Service → Data | `WorkOrderService.LoadAsync` → `IWorkOrderRepository.GetAllAsync()` (copies of the 6 seeded rows) | `[SVC] … → IWorkOrderRepository.GetAllAsync()` · `[DATA] InMemoryWorkOrderRepository.GetAllAsync — 6 rows` |
| Clean rows | Service → Domain | `AcceptChanges()` on each object: a freshly loaded row is not a change | `[SVC] … 6 work orders, all clean (IsDirty = false)` |
| Fill the master list | View | `_allOrders` receives the objects; then `ApplyFilter` runs with the current query (empty at start) | `[SVC] WorkOrderService.Filter — {text:"", status:All} → 6 of 6 match` |
| Fill the grid | Binding | `RebindVisible` rebuilds the `BindingList` and calls `ResetBindings()` once; the grid reads the list through `workOrderSource`; the BindingSource makes row 0 current → `CurrentChanged` → the detail shows #2001 | `[UI] WorkOrdersPage.RebindVisible — BindingList rebuilt with 6 rows … current: #2001` · `[UI] workOrderSource.CurrentChanged — current → #2001 …` |

Nothing on the screen called `Rows.Add`. The grid's columns were declared in the Designer with
`DataPropertyName`; `Cost` and `DueDate` are formatted by the column style; `CellFormatting` adds the
enum text and colours.

## 2. Filter (search box + status dropdown)

| Step | Layer | What happens |
|---|---|---|
| The user types `pump` or picks **Open** | View | `textSearch_TextChanged` / `comboStatusFilter_SelectedIndexChanged` → `ApplyFilter` → `ReadQueryFromForm()` builds a `WorkOrderQuery` (UI → data) |
| Decide which rows match | Service → Domain | `IWorkOrderService.Filter(_allOrders, query)` applies `WorkOrderQuery.Matches` (title or assignee contains the text; status equals) to the **live objects the screen already holds** — it does not go back to the repository, so unsaved edits survive a filter |
| Show the subset | Binding | `RebindVisible`: events off → `Clear` → `Add` each match → events on → one `ResetBindings()`. `labelCount` shows "1 of 6 work orders"; the BindingSource picks a new current item; the detail follows |
| No match | View | Not a failure: grey banner *No work orders match the search…*, status **● no matches**, empty grid, editor disabled (`panelDetail.Enabled = false`, header *Select a work order to edit it here.*). Trace: `[SVC] … → 0 of 6 match — an empty result, not an error` |
| Clear the search | View | Same path with an empty query: every row comes back from the master list, no reload |

Why not `BindingSource.Filter = "Status = 'Open'"`? That string is forwarded to the underlying list, and
only list types that implement `IBindingListView` (a `DataView`) honour it. A `BindingList<T>` of objects
ignores it silently — which is exactly the kind of "the value doesn't appear" bug this module is about.

## 3. Edit (master-detail through one BindingSource)

| Step | What happens |
|---|---|
| Click row 2002 | The grid moves `workOrderSource.Current`. The four bound fields re-read `Title`, `AssignedTo`, `DueDate`, `Cost` from the new current item by themselves. `CurrentChanged` fills the header ("Work Order 2002") and the two enum combos, and refreshes the dirty state. No copying code |
| Type in **Title** | `textTitle` pushes the value into `WorkOrder.Title` (`DataSourceUpdateMode.OnPropertyChanged`) → `SetField` raises `PropertyChanged("Title")` and `PropertyChanged("IsDirty")` → `BindingList` raises `ListChanged(ItemChanged, row 1)` → the grid repaints the Title cell (amber, because `IsDirty`) → `UpdateDirtyState` shows **● Unsaved changes** and enables Save / Discard. Trace: `[UI] BindingList.ListChanged — #2002.Title PropertyChanged → ItemChanged(row 1) → grid cell repaints · IsDirty = True` |
| Change **Cost** to 2150 | Same chain; the Cost cell shows `$2,150.00` because the column formats it |
| Pick **Status → In Progress** | Manual conversion: `comboStatus_SelectedIndexChanged` writes `current.Status = (WorkOrderStatus)SelectedIndex`; from there the chain is identical (the Status cell text and colour change, `IsOverdue` is re-announced) |
| Select another row with edits pending | Nothing is lost: the edits live on the *object*, which stays in the master list. The header shows **● 1 unsaved elsewhere**; come back to the row and it is still dirty. **↻ Reload** is refused while any row is dirty (*Save or discard the unsaved changes before reloading.*) |

## 4. Save (commit) and Discard (rollback)

| Step | Layer | What happens | Trace |
|---|---|---|---|
| **Save** | View | `workOrderSource.EndEdit()` flushes a value a bound control may still be holding, then `await _workOrders.SaveAsync(current)` | `[UI] WorkOrdersPage.buttonSave_Click — workOrderSource.EndEdit() → IWorkOrderService.SaveAsync(#2002)` |
| validate | Service | title required (≤ 80 chars), cost between $0 and $250,000, an in-progress order needs an assignee. A failure is a **result**: `OperationResult.Fail("Title is required.")`, the edits stay on screen, `IsDirty` stays true | `[SVC] ⚠ WorkOrderService.SaveAsync — rejected: Title is required. — edits kept, IsDirty stays true` → orange banner |
| persist | Data | `UpsertAsync(order)` stores a **copy** | `[DATA] InMemoryWorkOrderRepository.UpsertAsync — #2002 written (6 rows)` |
| accept | Domain | `order.AcceptChanges()` — only now does the edited object become the saved state. `PropertyChanged("IsDirty")` clears the indicator and the amber title through the same binding chain | `[DOMAIN] WorkOrder.AcceptChanges — #2002 snapshot taken → IsDirty = false` · `[UI] OK · Work order #2002 saved.` |
| **Discard** | View → Service → Domain | `_workOrders.Discard(current)` → `order.RejectChanges()`: every setter is written back from the snapshot, each raises `PropertyChanged`, the fields and the grid row revert. Nothing is persisted | `[DOMAIN] WorkOrder.RejectChanges — #2002 restored to the saved snapshot — every setter raised PropertyChanged, nothing persisted` · one `ListChanged` line per restored property · `[UI] OK · Changes to work order #2002 discarded.` |
| **Save while the store is down** | Data → View | `UpsertAsync` throws `DataOutageException` *before* `AcceptChanges` ran. The handler's `catch` logs the exception with its internal message and shows `Strings.SaveFailedEditsKept`: *Your changes could not be saved and are still on screen.* The row is still dirty, the edits are still visible | `[DATA] ✖ outage: UPDATE WorkOrders WHERE Id=2002 failed — timeout connecting to sql01:1433 (TicketOps.dbo.WorkOrders)` · `[UI] ✖ WorkOrdersPage.buttonSave_Click — caught DataOutageException — user sees the safe message` · `[UI] ⚠ … edits kept: #2002 still dirty = True` |
| **Recover**, Save again | — | Same click, same edits, `[DATA] #2002 written`, `[DOMAIN] AcceptChanges`, indicator clears | |

## 5. Bulk load through the same binding (progress path)

**▶ Import 60 work orders**: a `Timer` ticks every 150 ms; each tick awaits
`IWorkOrderService.ImportBatchAsync(next, 5)` (the service generates and persists five orders and returns
them clean), appends them to the master list, and adds the ones that match the active filter to the
`BindingList`. Each `Add` raises `ItemAdded`; the grid grows by itself. The progress bar and
**● importing 25/60** advance; the trace shows one `[DATA] #20xx written` per order and one
`[UI] … 5 of 5 added to the BindingList → ItemAdded × 5 → grid now 31 rows` per tick. No `Rows.Add`, no rebind.

## Evidence

Run the app and follow the **Reviewer script** in the README: the trace lines quoted above appear in that
order for each step, and the only `[UI]` lines that touch the grid are `RebindVisible` (filter) and
`InitializeBinding` (once).
