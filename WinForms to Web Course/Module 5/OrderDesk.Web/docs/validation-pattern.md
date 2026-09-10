# Validation pattern — server-side rule, field-level feedback (Module 5 · lab step "Add validation to the edit workflow")

## The rule has no UI dependency

`Domain/OrderValidator.cs` is the reusable rule the video shows: a plain class that returns a `ValidationResult`
with one message per offending field. Nothing in it references `Wisej.Web` or `System.Windows.Forms`, so the same
class serves the edit dialog, a batch import (Module 6), a web API, or an AI action.

| Field (`ValidationResult.Errors` key) | Rule | Message |
|---|---|---|
| `Customer` | a customer must be selected | `Customer is required.` |
| `Owner` | an owner must be assigned | `Assign an owner before saving.` |
| `Lines` | at least one line, every quantity ≥ 1 | `Add at least one order line.` / `Every line needs a quantity of 1 or more.` |
| `Total` | not negative; within the customer's credit limit | `Total $… exceeds the credit limit $….` |

Where it runs:

1. **`OrderService.Save(order)`** — always. If the result has errors it throws `ValidationException` and the store is
   never touched. This is the boundary: it does not matter what the browser sent.
2. **`Dialogs/EditOrderDialog.Save_Click`** — as a courtesy to the user, before the dialog returns `DialogResult.OK`.
   The dialog never saves; the caller (`MainPage.EditOrder`) calls `OrderService.Save`, which validates again.

## Field-level messages instead of a MessageBox

The WinForms dialog joined all messages into one blocking `MessageBox.Show(..., "Cannot save")`. The ported dialog
maps every `Errors` key onto the control that owns it and calls `ErrorProvider.SetError(control, message)`:

| Key | Control | Why |
|---|---|---|
| `Customer` | `customerCombo` | the choice that is wrong |
| `Owner` | `ownerCombo` | pick `(none)` to reproduce |
| `Lines` | `quantityBox` | quantity 0 breaks the line rule |
| `Total` | `quantityBox` | the quantity is what pushed the total over the credit limit |

A one-line summary under the fields (`2 fields need attention: Owner, Total`) keeps the dialog open; the Total label
turns red as soon as the live total exceeds the customer's credit limit, before Save is clicked. The validation
MessageBox of Module 3 is gone from this workflow; the only blocking dialog left is the edit form itself.

Pattern (the lesson's snippet, applied):

```csharp
errorProvider.Clear();
var result = new OrderValidator().Validate(Order);   // reusable rule, no UI
if (result.HasErrors)
{
    foreach (var error in result.Errors)
        errorProvider.SetError(ControlFor(error.Key), error.Value);   // inline, not a MessageBox
    return;
}
DialogResult = DialogResult.OK; Close();               // the caller saves through OrderService.Save
```

## Evidence

- **Edit selected → validate** (or a row double-click, or the grid's edit tool): trace `• server EditOrderDialog  new EditOrderDialog(order 1042 · Northwind Traders · $4,820.00) from button — inside using, ShowDialog() blocks this handler`.
  In the dialog set Owner to `(none)` and click Save: the Owner combo gets the ErrorProvider icon with `Assign an owner before saving.`, the
  summary says `1 field needs attention: Owner`, the trace shows `✖ fail ErrorProvider → Owner  Assign an owner before saving.` and
  `• server OrderValidator.Validate  1 error(s) → ErrorProvider.SetError on Owner (dialog stays open)`. Raise the quantity to 200 for order 1042
  (Northwind, credit limit $50,000): the Total label turns red, Save adds `✖ fail ErrorProvider → Total  Total $91,580.00 exceeds the credit limit $50,000.00.`
  (Gold discount included). Fix both, Save: `✓ ok OrderValidator.Validate  valid — same rule OrderService.Save runs again`,
  `• server ShowDialog()  returned DialogResult.OK — the handler resumed here`, `✓ ok OrderService.Save  order 1042 saved · …`, `→ .NET→JS Toast  "Order 1042 saved."`,
  the grid reloads, and finally `✓ ok dialog disposed  dlg.IsDisposed = True after the using block`.
- **Save invalid ✕** (programmatic, no dialog): `• server order 1039 clone  Owner = null · Lines[0].Quantity = 99,999 → Total $147,235,275.00 vs credit limit $80,000.00 (Adventure Works)`,
  `✖ fail OrderService.Save  ValidationException — 2 field(s)`, `✖ fail   Owner  Assign an owner before saving.`, `✖ fail   Total  Total $147,235,275.00 exceeds the credit limit $80,000.00.`,
  banner `✖ ValidationException from OrderService.Save — Owner: … · Total: …`, then `✓ ok store unchanged  order 1039 still $12,400.00 · owner Sam · Invoiced`
  and `★ log validation boundary  the rule lives in OrderValidator and the service enforces it …`.
