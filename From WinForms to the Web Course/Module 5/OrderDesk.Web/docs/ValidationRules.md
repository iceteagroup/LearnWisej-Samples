# Validation rules — taken out of the form

**Deliverable 3 · "Add validation to the edit workflow" — server-side, field-level, reusable.**

## Before

`LegacyOrderDesk.EditOrderDialog.saveButton_Click` had one rule, inside the form:

```csharp
if (customerComboBox.SelectedItem == null) { MessageBox.Show("Select a customer."); return; }
```

One message, blocking, and unreachable from anything that is not that dialog. The console keeps it as
`Legacy/DesktopGridHabits.ValidateInsideTheForm` and runs it on the **Desktop rule → MessageBox** button.

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
  dialog only closes with `DialogResult.OK` when the server said the order is valid; the page then calls
  `OrderService.Save` (business logic reused unchanged).
* **A batch** (**Batch 2,000** button): 2,000 stored orders, 200 per timer tick, the same class, no form on screen.
* **A direct call** (**Validate a bad order (no form)** button): the shape a web API or an import job uses.

## Why the browser is not the boundary

Client-side checks are a nice touch, never the security boundary: the server owns the session and the data, and the
browser can be bypassed. A rule that lives in `Domain/` runs on every path that reaches the data; a rule that lives in a
click handler runs only when that handler runs.

## Evidence

| Button | What happens |
|---|---|
| **New order (dialog)** → *Save* | ErrorProvider marks Customer, PO number and Quantity/SKU; the toast says "Add at least one order line."; the trace logs `OrderValidator.Validate order 0: Customer: … · PoNumber: …` |
| **Edit selected…** → change a value → *Save* | `OrderValidator.Validate … valid`, `OrderService.Save order n · total …`, a toast "Order n saved.", the grid re-fetches its blocks |
| **Validate a bad order (no form)** | the field list appears in the card, the banner explains, no dialog opened |
| **Desktop rule → MessageBox** | one blocking MessageBox with the single desktop message; the banner lists what it missed |
| **Batch 2,000** | status counts up 200 per tick; result "2,000 validated · 0 invalid" (the generated data is valid by construction — the point is that the rule ran with no UI) |
