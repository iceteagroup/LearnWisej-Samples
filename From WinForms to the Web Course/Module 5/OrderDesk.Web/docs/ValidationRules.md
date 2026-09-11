# Validation rules — taken out of the form

**Lab step "Add validation to the edit workflow" — server-side, field-level, reusable.**

## Before

`LegacyOrderDesk.EditOrderDialog.saveButton_Click` had one rule, inside the form:

```csharp
if (customerComboBox.SelectedItem == null) { MessageBox.Show("Select a customer."); return; }
```

One message, blocking, and unreachable from anything that is not that dialog.

## After — `Domain/Services/OrderValidator.cs`

| # | Rule | Field key | Message |
|---|---|---|---|
| 1 | customer required | `Customer` | Customer is required. |
| 2 | PO number required, ≤ 20 characters | `PoNumber` | PO number is required. / PO number is 20 characters at most. |
| 3 | at least one order line | *(general)* | Add at least one order line. |
| 4 | every line: quantity ≥ 1 | `Quantity` | Quantity must be at least 1. |
| 4 | every line: unit price ≥ 0 | `UnitPrice` | Unit price can't be negative. |
| 5 | total ≥ 0 | `Total` | Total can't be negative. |

`OrderValidator.Validate(order)` returns a `ValidationResult` — `Errors` keyed by field name plus `General` messages —
and depends on nothing but the `Order` model. Owner stays optional: an order can wait in the queue before someone picks
it up.

## Who calls it

* **The edit dialog** (`Dialogs/EditOrderDialog.cs`): *Save* collects the fields, calls the validator on the server and
  puts each message next to its control with an `ErrorProvider`; general messages become a non-blocking toast. The
  dialog only closes with `DialogResult.OK` when the order is valid; the page then calls `OrderService.Save` (business
  logic reused unchanged).
* **Any other path** — a batch import, a web API — calls the same `OrderValidator.Validate(order)`; nothing in it
  depends on a form.

## Why the browser is not the boundary

Client-side checks are a nice touch, never the security boundary: the server owns the session and the data, and the
browser can be bypassed. A rule that lives in `Domain/` runs on every path that reaches the data; a rule that lives in a
click handler runs only when that handler runs.

## Evidence

| Action | What happens |
|---|---|
| **New order** → *Save* without filling anything | the ErrorProvider marks Customer and PO number; the toast says "Add at least one order line."; the dialog stays open |
| **Edit…** (or double-click a row) → change a value → *Save* | the dialog closes, a toast "Order n saved.", the grid re-counts and re-fetches its blocks |
| **Edit…** → set Quantity to 0 → *Save* | the ErrorProvider marks Quantity; nothing is saved |
