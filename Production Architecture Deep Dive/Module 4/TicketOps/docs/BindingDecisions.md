# Binding decisions — what is bound where, and what stays manual

*Module 4 deliverable · TicketOps Console · Work Orders screen (`Views/WorkOrdersPage`)*

A binding is a contract between one control property and one object property. This note lists every
contract on the Work Orders screen, the direction it runs in, and the four things that are deliberately
**not** bound — with the reason for each.

## The contract, in one picture

```
Services/WorkOrderService ──LoadAsync()──►  List<WorkOrder> _allOrders            (master list: every object)
                                                   │  IWorkOrderService.Filter(all, WorkOrderQuery)
                                                   ▼
                                    BindingList<WorkOrder> _visibleOrders          (announces ItemAdded / ItemChanged / Reset)
                                                   │  .DataSource
                                                   ▼
                                    Wisej.Web.BindingSource  workOrderSource       (owns the CURRENT item)
                                       │                          │
                       .DataSource     │                          │  .DataBindings.Add("Text" / "Value", workOrderSource, "Prop")
                                       ▼                          ▼
                          DataGridView dgvWorkOrders     textTitle · textAssignedTo · dateDue · numericCost
                          (complex binding: whole list)  (simple bindings: the CURRENT item, two-way)
```

Everything binds *through* `workOrderSource`, never to the raw list. That is the whole master-detail
link: the row the grid makes current **is** the object the four fields edit. There is no
"SelectionChanged → copy the row into the fields" code anywhere on this screen.

## What is bound

| Control · property | Object · property | Direction | How | Why this way |
|---|---|---|---|---|
| `dgvWorkOrders.DataSource` | `workOrderSource` → `BindingList<WorkOrder>` | object → control | `AutoGenerateColumns = false`; 7 `DataGridViewTextBoxColumn`s with `DataPropertyName` (`Id`, `Title`, `Status`, `Priority`, `AssignedTo`, `DueDate`, `Cost`) declared in the Designer | Declared columns keep order, headers and widths under our control and stop `IsDirty` / `IsOverdue` from leaking into the grid |
| `textTitle.Text` | `WorkOrder.Title` | two-way | `DataBindings.Add("Text", workOrderSource, "Title", true, OnPropertyChanged)` | Typing reaches the object as soon as the control reports a change; the object raises `PropertyChanged`; the grid cell repaints — the "live row" the lab asks for |
| `textAssignedTo.Text` | `WorkOrder.AssignedTo` | two-way | same | same; the grid shows an empty value as "Unassigned" (formatting, not data) |
| `dateDue.Value` | `WorkOrder.DueDate` (`DateTime`) | two-way | `DataBindings.Add("Value", …, OnPropertyChanged)` | `DueDate` is non-nullable on purpose: a `DateTime?` cannot be pushed into `DateTimePicker.Value` without a `Format`/`Parse` pair |
| `numericCost.Value` | `WorkOrder.Cost` (`decimal`) | two-way | `DataBindings.Add("Value", …, OnPropertyChanged)` | Same type on both sides, so no conversion; the *grid* formats it as `C2` |
| `columnCost.DefaultCellStyle.Format` | — | display | `"C2"`, `Alignment = MiddleRight` | Currency is a display concern: the model stores `1850m`, the column shows `$1,850.00` |
| `columnDueDate.DefaultCellStyle.Format` | — | display | `"MMM d"` | Same idea for dates |
| `dgvWorkOrders.CellFormatting` | `Status`, `Priority`, `AssignedTo`, `DueDate`, `Title` | display | enum → text ("In Progress") + colour; "Unassigned" in grey; overdue tint (needs `Status` too, so it reads the row object via `workOrderSource[e.RowIndex]`); unsaved title in amber | Anything that depends on more than the cell's own value, or needs colour, goes here; `e.FormattingApplied = true` whenever `e.Value` is replaced. No service call, no allocation in a loop |

### Notification, the part that makes it live

- `WorkOrder` implements `INotifyPropertyChanged`; every editable setter goes through one `SetField` helper
  that raises `PropertyChanged` **only when the value changed** (no needless repaint, no false dirty flag)
  and also announces `IsDirty`. `DueDate` and `Status` additionally announce the derived `IsOverdue`.
- `BindingList<WorkOrder>` hears each `PropertyChanged` and raises `ListChanged(ItemChanged, row, property)`.
  The grid repaints that cell; the screen's `visibleOrders_ListChanged` handler refreshes the dirty indicator.
- Adding to the `BindingList` raises `ItemAdded`: the grid grows. The screen never calls `Rows.Add`.

## What stays manual — and why

| Not bound | What the screen does instead | Why |
|---|---|---|
| **`comboStatus` / `comboPriority`** (enums) | `RefreshDetail()` sets `SelectedIndex = (int)current.Status` when the current item moves; `comboStatus_SelectedIndexChanged` writes `current.Status = (WorkOrderStatus)SelectedIndex`. A `_fillingDetail` flag stops the handler from writing while the combo is being filled | The conversion enum ↔ list index is not something a `Text`/`Value` binding does. Binding `SelectedItem` to an enum would need the `Items` to hold boxed enum values and relies on the control's `SelectedItemChanged` plumbing — one line of explicit code is clearer and testable. Once the value is on the object, notification takes over exactly as for the bound fields |
| **Search / status filter** | `IWorkOrderService.Filter(_allOrders, WorkOrderQuery)` returns the matches; `RebindVisible` rebuilds the `BindingList` with `RaiseListChangedEvents = false` and one `ResetBindings()` | `BindingSource.Filter` is a string the underlying list must implement (`DataView` does); a `BindingList<T>` of objects silently ignores it. Rebuilding from the untouched master list means clearing the search brings every row back with no reload — and because the *same object instances* are re-added, a row with unsaved edits keeps them while it is filtered out |
| **Dirty indicator** (`labelDirty`, `buttonSave.Enabled`, `buttonDiscard.Enabled`) | `UpdateDirtyState()` reads `WorkOrder.IsDirty` on `ListChanged` and `CurrentChanged` | The *decision* ("is this row dirty?") is data on the model — `IsDirty` compares the current values with the snapshot taken by `AcceptChanges`. The label is pure display and needs the count of *other* dirty rows too, which no single binding expresses |
| **Header** (`labelDetailTitle` = "Work Order 2002" / "Select a work order…") and `panelDetail.Enabled` | set in `RefreshDetail()` from `workOrderSource.CurrentChanged` | The lesson's "extras": `CurrentChanged` is for a header or a button state, never for the core wiring |
| **Save / Discard** | `workOrderSource.EndEdit()` then `await _workOrders.SaveAsync(current)`; `_workOrders.Discard(current)` → `WorkOrder.RejectChanges()` | Commit and rollback are *decisions*, so they live in the service and the domain, not in the binding. `BindingSource.CancelEdit()` only restores values when the item implements `IEditableObject`; a plain observable object does not, so the sample restores from its own saved snapshot — see [`CommitCancelNotes.md`](CommitCancelNotes.md) |

## Where each layer stops

- **Domain** (`WorkOrder`, `WorkOrderQuery`): values, notification, dirty snapshot, matching rule. No display strings, no Wisej.NET type.
- **Service** (`WorkOrderService`): load, filter, validate, persist, accept/reject changes. It works on the same objects the grid shows, so what it changes appears without copying.
- **View** (`WorkOrdersPage`): the binding configuration (`InitializeBinding`), the formatting handler, the three manual conversions above, thin handlers.

## Evidence (what the running app shows)

- Type in **Title** with row 2002 selected: the grid's Title cell follows, turns amber, **● Unsaved changes** appears.
  No handler in `WorkOrdersPage.cs` wrote to the grid.
- Change **Cost** to 2150: the Cost cell shows `$2,150.00` — the format lives on the column, the object holds `2150`.
- Move **Due date** to yesterday: the Due cell gets the overdue tint (CellFormatting read `IsOverdue`, announced from the `DueDate` setter).
- Click **Discard**: the fields and the row revert with no code that copies values.
- Search `pump`: the grid shows 1 row and the count reads **1 of 6 work orders**; clear it and all six come back with no reload.
