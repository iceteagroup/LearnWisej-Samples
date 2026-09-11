# Deliverable 2 · The edit dialog with disposal — return values and lifetime

`Dialogs/EditOrderDialog` is `Legacy/WinForms/EditOrderDialog` ported to `Wisej.Web`: the same four fields, the
same `AcceptButton`/`CancelButton`, the same contract (`DialogResult.OK` on Save, `Cancel` otherwise). The port
changes nothing inside the dialog except the validation message's namespace. Everything that changed is on the
**caller's** side, and it is all about lifetime.

## 1 · `ShowDialog` does not block

```csharp
// desktop (OrdersForm.ordersGrid_CellDoubleClick)
var dialog = new EditOrderDialog(order, customers);
if (dialog.ShowDialog(this) == DialogResult.OK)      // the message pump waits here
{
    _orderService.Save(dialog.Order);
    MessageBox.Show("Saved.");
}
```

On the server there is no message pump to wait in: the request that handled the double-click has to return so the
browser can render the modal. `Form.ShowDialog()` therefore **returns immediately** (with `DialogResult.None`), and
the desktop `if` never runs its body. Two shapes give the result back:

| Shape | Code | Used by |
|---|---|---|
| callback | `form.ShowDialog((f, result) => { …; f.Dispose(); });` | `ReportsScreen.PrintSelected` (InvoicePreviewForm) |
| awaitable | `DialogResult result = await form.ShowDialogAsync();` in an `async void` handler | `OrdersScreen.EditOrder` |

Both leave the session free while the dialog is open; the close arrives later as an ordinary event.

## 2 · Closed is not disposed

`Close()` hides a Wisej.NET form; it does not dispose it, and the framework does not dispose it for you. On the
desktop nobody noticed: a forgotten dialog died with the process at the end of the day. On the server the session
lives for hours and the process for weeks, and every forgotten dialog is a server-side object graph (the form, its
controls, its event handlers, the order it holds) that stays reachable through the session.

```csharp
// web (OrdersScreen.EditOrder)
using (var dialog = new EditOrderDialog(order, _customerService.GetCustomers()))
{
    if (await dialog.ShowDialogAsync() == DialogResult.OK)
    {
        var saved = _orderService.Save(dialog.Order);
        Reload(saved.Id);
        Ui.Toast($"Order {saved.Id} saved.");
    }
}   // Dispose runs here, after the await
```

The `using` block is the lesson's rule spelled for an awaited call: the `await` completes when the user closed the
dialog, the block ends, `Dispose` runs — deterministically, every time, on OK, on Cancel and on an exception in
`Save`. The callback form does the same with an explicit `f.Dispose()` as the last line of the callback.

## 3 · Reuse on purpose: make it visible and reset the state

Reusing a dialog is legitimate (a heavy editor, a dialog that keeps a user's filter) but it must be a decision, not
an accident: keep the instance in a **named field** so there is an owner, call `EditOrderDialog.Bind(order)` before
every show (it resets the fields, the title and `DialogResult` — stale state is the reuse trap the video warns about),
and dispose it together with its owner. This sample does not reuse the dialog; every edit gets a new, disposed one.

## 4 · Return values that are decisions stay modal

The validation message in `saveButton_Click` (`Select a customer.`) stays a modal `MessageBox`: Save must not
continue until the input is fixed. A modal box in Wisej.NET blocks the **browser**, not the server thread.
`NotificationsReview.md` applies the same rule to every MessageBox.

## Evidence (in the running app)

1. Double-click **1042** on the Orders screen → the modal opens over the page. Change the status and press **Save** →
   the dialog closes, the grid shows the new status, and a Toast `Order 1042 saved.` appears in the top-right corner.
2. **Cancel** closes the dialog and leaves the order untouched.
3. **New Order** (ToolBar or the Orders screen) → the same dialog for a new order; Save adds order 1043 at the top.
