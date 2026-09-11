# Commit / cancel behaviour notes

*Module 4 deliverable · TicketOps Console · Work Orders screen*

## The rule

**Edits reach the object immediately; they reach the store only on Save; Cancel puts the object back.**

The detail fields are bound two-way to the current `WorkOrder` with `DataSourceUpdateMode.OnPropertyChanged`,
so the grid row follows the editor live — that is the point of the module. The price of that choice is
that "the object" and "the saved record" are no longer the same thing, and something has to remember
the saved state so Cancel has something to go back to. In this sample that something is the object itself.

## How the sample does it

| Moment | Who | What |
|---|---|---|
| Row loaded | `WorkOrderService.LoadAsync` | `order.AcceptChanges()` — snapshot of the six editable values; `IsDirty = false` |
| User edits a field | binding → `WorkOrder.SetField` | value stored; `PropertyChanged(prop)` and `PropertyChanged(IsDirty)`; `IsDirty` is now `values ≠ snapshot` |
| **Save** | `WorkOrdersPage.buttonSave_Click` → `WorkOrderService.SaveAsync` | `workOrderSource.EndEdit()` (flush a pending bound value) → validate → `IWorkOrderRepository.UpsertAsync(order)` (the store keeps a **copy**) → `order.AcceptChanges()` → `Ok("Work order #2002 saved.")` |
| Save rejected (validation) | `WorkOrderService.SaveAsync` | returns `Fail("Title is required.")`; the object is **not** touched, `IsDirty` stays true, the edits stay on screen, the orange banner explains |
| Save fails (store down) | the repository throws | the handler's `catch` logs the details and shows *Your changes could not be saved and are still on screen*; `AcceptChanges` never ran, so the row is still dirty and nothing was lost |
| **Discard** | `WorkOrdersPage.buttonDiscard_Click` → `WorkOrderService.Discard` | `order.RejectChanges()` writes the snapshot back through the normal setters; each raises `PropertyChanged`, so the grid row and the bound fields revert without any copying code; `Ok("Changes to work order #2002 discarded.")`. Nothing is persisted |
| Discard with nothing dirty | `WorkOrderService.Discard` | `Fail("There are no unsaved changes to discard.")` — the button is disabled anyway; the service still decides |
| Selecting another row | binding | edits stay on the object in the master list; the header shows **● 1 unsaved elsewhere**; **↻ Reload** refuses to replace the objects while any row is dirty |

## Why not `BindingSource.CancelEdit()`?

The video's code sketch cancels with `workOrderSource.CancelEdit()`. Wisej.NET's `BindingSource` has
`EndEdit()` / `CancelEdit()` (checked in the framework XML docs), and they behave like WinForms: they
forward to the current item's `IEditableObject.BeginEdit / EndEdit / CancelEdit`. A plain observable
object that does not implement `IEditableObject` has nothing to cancel — the values are already in it.

Two ways to make Cancel real:

1. **A working copy** (the lesson guide's pattern): bind the editor to `selected.Clone()`, and only on a
   successful Save copy the clone back into the live row. Cancel drops the clone. Safe, but the grid row
   does *not* follow the editor while typing — the live-row behaviour this lab asks for is lost.
2. **A snapshot on the object** (this sample): `AcceptChanges` / `RejectChanges` / `IsDirty` on `WorkOrder`.
   The live row follows the editor, and Cancel is still a genuine rollback because the object remembers
   its saved state. `IEditableObject` could be layered on top (`CancelEdit → RejectChanges`) so that the
   BindingSource's own `CancelEdit()` works too; the sample keeps the decision in the service instead, so
   that Save and Discard are the same kind of call with the same kind of result.

Either way the **decision** (when the edit becomes the saved state, when it is thrown away) is not in a
binding or a handler: it is one method on the service, callable from a test with no browser.

## What the user sees

| Situation | Indicator | Banner / status |
|---|---|---|
| Clean row | no label; Save / Discard disabled; Title in the grid black | **● 6 work orders** |
| Dirty row | **● Unsaved changes** (amber); Save / Discard enabled; Title in the grid amber | — |
| Dirty row elsewhere | **● 1 unsaved elsewhere** | — |
| Saved | label clears, buttons disable | **● Work order #2002 saved.** |
| Validation failed | label stays | orange **Title is required.** · **● not saved** |
| Store down | label stays, edits stay | red **✖ Your changes could not be saved and are still on screen. Check the log for details.** + toast · **● failed** |
| Discarded | label clears; fields and row revert | **● Changes to work order #2002 discarded.** |

## Evidence

In the running app: edit #2002's Title and Cost → Discard → the row reads *Repair loading dock pump /
$1,850.00* again and the indicator clears. Edit again, clear the Title → Save → orange **Title is
required.**, the edit is still visible and the row still amber → type a title → Save → indicator clears.
