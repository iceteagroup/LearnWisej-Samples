# Designer & code review checklist — applied to Module 4

| Check | Result | Evidence |
|---|---|---|
| The screen remains usable in the Visual Studio / Wisej.NET Designer | ✔ | `Views/WorkOrdersPage.Designer.cs` holds the whole layout in `InitializeComponent()` (including the `BindingSource`, `Timer`, grid columns with `DataPropertyName` and the two column `DefaultCellStyle`s); a parameterless constructor exists; the `DataSource` / `DataBindings` wiring is in `InitializeBinding()` in the code-behind |
| Controls are named clearly enough for a teammate to follow the event code | ✔ | `dgvWorkOrders`, `workOrderSource`, `textSearch`, `comboStatusFilter`, `textTitle`, `comboStatus`, `comboPriority`, `textAssignedTo`, `dateDue`, `numericCost`, `buttonSave`, `buttonDiscard`, `buttonReload`; handlers are `<control>_<event>` |
| Domain objects implement `INotifyPropertyChanged`; collections are `BindingList<T>` | ✔ | `Domain/WorkOrder.cs` (one `SetField` helper, equality guard, derived `IsOverdue` and `IsDirty` announced); `WorkOrdersPage._visibleOrders` is a `BindingList<WorkOrder>` |
| Formatting lives in the UI layer, not baked into domain objects | ✔ | `WorkOrder` holds `decimal Cost`, `DateTime DueDate`, enums; `$1,850.00` comes from `columnCost.DefaultCellStyle.Format = "C2"`, "In Progress"/colours/"Unassigned"/overdue tint from `dgvWorkOrders_CellFormatting` |
| Business logic is not trapped in visual event handlers | ✔ | validation in `WorkOrderService.Validate`, the matching rule in `WorkOrderQuery.Matches`, commit/rollback in `WorkOrder.AcceptChanges/RejectChanges` called by the service; the handlers read the screen, call the service, show the result |
| Save edits a working copy and only merges back on success — or an equivalent commit point | ✔ (equivalent) | edits reach the live object (so the grid follows), but the *saved state* is the snapshot: `AcceptChanges` runs only after `UpsertAsync` succeeded; a rejected or failed save leaves `IsDirty = true` and the edits on screen — see [`CommitCancelNotes.md`](CommitCancelNotes.md) |
| No per-user state is held in static fields | ✔ | statics are colour constants, `Strings`, `SeedData.WorkOrders()`, `OperationResult.Ok/Fail`, `StatusBanner.ColorFor`, the import name tables — constants and pure functions; `AppComposition` is created per session in `Program.Main`, and the `BindingList` lives in the per-session screen |
| Failure paths are visible, logged, and explained without leaking internals | ✔ | `ShowResult` (expected: validation, no unsaved changes, reload refused) and `ReportFailure` (unexpected: outage); the `sql01:1433` message is in the trace only, the banner shows `Strings.SaveFailedEditsKept` / `Strings.ActionFailed`; an empty search result is a grey banner, not a failure |
| The deliverable can be reviewed without running the whole course | ✔ | `README.md` + this `docs/` folder; the app runs standalone on port 5104 |

## Common mistakes checked

| Mistake | Present? |
|---|---|
| Domain object without `INotifyPropertyChanged` (grid goes stale) | no — every setter notifies through `SetField` |
| Grid bound to a plain `List<T>` (adds never appear) | no — `BindingList<WorkOrder>`; the import proves it: 60 `ItemAdded`s, no `Rows.Add` |
| Cancel that cannot undo because the live object was mutated | no — `RejectChanges` restores the saved snapshot; verified by **Discard** after an edit |
| Forgetting `EndEdit()` before reading the object on Save | no — `buttonSave_Click` calls `workOrderSource.EndEdit()` first |
| Display strings baked into the model | no — `WorkOrder.ToString()` is for the trace only; nothing on screen reads it |
| Master-detail wired by copying fields in `SelectionChanged` | no — the grid and the four fields share `workOrderSource`; `CurrentChanged` only fills the header and the two enum combos |
| `PropertyChanged` raised without an equality guard | no — `SetField` returns `false` for an unchanged value |
| Heavy work in `CellFormatting` | no — value mapping and colour lookups only; the row object is read from `workOrderSource[e.RowIndex]` |
| Blindly resetting `DataSource` when a value does not show | no — the only rebind is `ResetBindings()` after a filter rebuild, by design |
