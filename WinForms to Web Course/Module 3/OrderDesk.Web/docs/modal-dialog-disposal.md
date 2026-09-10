# Modal dialog disposal — the EditOrderDialog port (Module 3)

## The desktop habit

```csharp
// LegacyOrderDesk/OrdersForm.cs
private void ordersGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
{
    var dlg = new EditOrderDialog(SelectedOrder, _service);   // ✕ never disposed — the process exit hides it
    if (dlg.ShowDialog(this) == DialogResult.OK)
    {
        _service.Save(dlg.Order);
        MessageBox.Show("Saved.");                           // ✕ blocking confirmation (see notification-policy.md)
        ReloadGrid();
    }
}
```

On a desktop this "works": one user, one process, and the handful of forgotten dialogs die with the
process. On a server every browser session is a long-lived object graph inside one process; a dialog
that is closed but not disposed stays registered in the session until the GC happens to collect it —
or forever, if anything still references it.

## What Wisej.NET does (Wisej.Framework.xml, `Form.ShowDialog`)

> When using ShowDialog, the dialog instance is not automatically disposed when the dialog is closed
> because dialogs are reusable. To make sure that memory is released properly, you must dispose the
> instance in your code or use the typical **using** pattern.

Two related facts the samples rely on:

- `ShowDialog()` without a callback **suspends the server handler** until the dialog closes — the
  desktop modal workflow. With the optional `onclose` callback (`ShowDialog((form, result) => …)`) the
  dialog is modal in the browser only and the handler continues immediately.
- Non-modal forms shown with `Show()` **are** disposed on close ("Forms are disposed immediately when
  they are closed and cannot be shown again"). The reusable-dialog rule is specific to `ShowDialog`.

## The port

```csharp
// Pages/OrdersPage.cs
private bool EditOrder(Order order, string origin)
{
    using (var dlg = new EditOrderDialog(order, _service, Trace))
    {
        var result = dlg.ShowDialog();                    // blocks the handler (Wisej modal workflow)
        if (result == DialogResult.OK)
        {
            _service.Save(dlg.Order);                     // ✓ business logic, reused as-is
            Notify.Saved("Order " + dlg.Order.Id + " saved.");   // ✓ Toast — was MessageBox.Show("Saved.")
            saved = true;
        }
    }                                                     // ← Dispose() here, every time, OK or Cancel
    …
}
```

`Dialogs/EditOrderDialog.cs` is the WinForms dialog moved to `Wisej.Web.Form`: the same three
combos (Customer / Owner / Status), Save / Cancel, `AcceptButton` / `CancelButton`, and the same
`OrderValidator` rule. Save sets `DialogResult = DialogResult.OK` and calls `Close()`; Cancel sets
`DialogResult.Cancel` (wired explicitly on `Click` rather than through `Button.DialogResult`, so the
close is deterministic). The layout lives in `EditOrderDialog.Designer.cs` — designer-owned, as the
video insists — and the migration logic in the partial class.

### Instrumentation (lab only)

- `EditOrderDialog.LiveInstances` — a process-wide counter incremented in the constructor and
  decremented in `Dispose(bool)` (via `OnDisposing()`). Process-wide means two browser tabs share it;
  that is fine for a counter and is exactly the Module 4 conversation.
- `Application.FindComponents(c => c is EditOrderDialog && !IsDisposed)` — the session's own registry
  of live components: what the server really still holds for this browser.
- `Application.OpenForms.Count` — the *open* forms; a closed-but-not-disposed dialog is expected to
  drop out of it, which is precisely why it is not a leak detector.
- The status bar panel **Dialogs alive: n** shows `LiveInstances` after every action.

### Reusing a dialog on purpose

If a dialog is kept as a field and shown repeatedly, it must not carry stale data: the sample exposes
`EditOrderDialog.ResetState()` (reloads the combos from `Order`) for that pattern — the video's
`_editDialog?.ResetState();`. The samples themselves use a fresh instance per edit inside `using`.

## Evidence

- **Edit selected ✓** (or grid double-click on 1042): trace `• EditOrder(1042)  from Edit selected ✓ · LiveInstances=0`,
  `• new EditOrderDialog(1042)  Wisej.Web.Form · LiveInstances=1`, `→ dlg.ShowDialog()  modal · this handler is suspended until the browser closes the dialog`.
  The dialog "Edit Order 1042" opens centred. Change Status to `InProgress`, Save:
  `← btnSave.Click`, `• OrderValidator.Validate  valid`, `• EditOrderDialog.Close()  DialogResult.OK → ShowDialog() returns in the caller`,
  `← dlg.ShowDialog() returned  DialogResult.OK · dlg.IsDisposed=False (inside the using block)`,
  `✓ _service.Save(dlg.Order)  Order 1042 · Dana · InProgress`, `→ Toast  "Order 1042 saved." · non-blocking · AutoCloseDelay 3500 ms`,
  `• EditOrderDialog.Dispose()  Edit Order 1042 released · LiveInstances=0`, `✓ using block exit  dlg.IsDisposed=True · LiveInstances=0`.
  The grid reloads with 1042 now `InProgress`; banner "✓ Saved through the using block — dlg.IsDisposed = true …"; status bar "Dialogs alive: 0".
- **Cancel** instead of Save: `← btnCancel.Click`, `← dlg.ShowDialog() returned  DialogResult.Cancel …`, `• EditOrderDialog.Dispose() …`,
  `✓ using block exit  dlg.IsDisposed=True · LiveInstances=0`, `• EditOrder  cancelled — nothing saved, dialog disposed anyway`.
- **Leak dialogs ✕**: five `• dialog #n  ShowDialog() → Close() · IsDisposed=False · LiveInstances=n` lines, then
  `✖ Application.OpenForms.Count …`, `✖ Application.FindComponents(c => c is EditOrderDialog)  5 closed EditOrderDialog instances still registered in this session`,
  `✖ EditOrderDialog.LiveInstances  5 constructed, never disposed (process-wide counter)`, `★ closed dialogs are not disposed  ShowDialog keeps the instance for reuse — the CALLER must Dispose (using block)`.
  Status ● alarm, red banner "✖ closed dialogs are not disposed — LiveInstances = 5 after 5 open/close cycles …", status bar "Dialogs alive: 5". Clicking again adds five more (10, 15, …).
- **Dispose fix ✓**: `• cleanup  disposing 5 leaked dialog(s) · 5 found with Application.FindComponents, 0 from the lab's own list`,
  `✓ leaked dialogs disposed  LiveInstances=0`, five `✓ using #n  ShowDialog() → Close() → Dispose() · IsDisposed=True · LiveInstances=0`,
  `✓ Application.FindComponents(c => c is EditOrderDialog)  0 alive`, `★ using block frees the dialog every time  an intentionally reused dialog must instead call ResetState() before each ShowDialog`.
  Green banner "✓ using block: 5 dialogs opened, closed and disposed — LiveInstances = 0 …"; status bar back to "Dialogs alive: 0".
