# Deliverable 2 · The edit dialog with disposal — return values, lifetime, reuse

`Dialogs/EditOrderDialog` is `Legacy/WinForms/EditOrderDialog` ported to `Wisej.Web`: the same four fields, the
same `AcceptButton`/`CancelButton`, the same contract (`DialogResult.OK` on Save, `Cancel` otherwise). The port
changes nothing inside the dialog except the validation message's namespace. Everything that changed is on the
**caller's** side, and it is all about lifetime.

## 1 · `ShowDialog` does not block

```csharp
// ✕ desktop (OrdersForm.ordersGrid_CellDoubleClick)
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
| callback | `form.ShowDialog((f, result) => { …; f.Dispose(); });` | `ReportsScreen.PrintSelected` (InvoicePreviewForm), the leak and reuse buttons in `MainPage` |
| awaitable | `DialogResult result = await form.ShowDialogAsync();` in an `async void` handler | `OrdersScreen.EditOrder` (the product path), `MainPage.buttonDelete_Click` with `MessageBox.ShowAsync` |

Both leave the session free while the dialog is open: the trace shows `→ .NET→JS ShowDialogAsync modal in the
browser; the handler yields …` and the close arrives later as an ordinary event (`← JS→.NET EditOrderDialog closed
DialogResult = OK`).

## 2 · Closed is not disposed — the leak

`Close()` hides a Wisej.NET form; it does not dispose it, and the framework does not dispose it for you. On the
desktop nobody noticed: a forgotten dialog died with the process at the end of the day. On the server the session
lives for hours and the process for weeks, and every forgotten dialog is a server-side object graph (the form, its
controls, its event handlers, the order it holds) that stays reachable through the session.

`Dialogs/DialogTracker.cs` makes that visible: `EditOrderDialog`'s constructor calls `DialogTracker.Opened(sessionId)`
and its `Dispose(bool)` calls `Released(sessionId)` once. The console shows
`live EditOrderDialog instances   this session n   ·   process-wide m` and refreshes it on every trace line.

| Button | Code | Counter |
|---|---|---|
| `buttonEditGood` **Edit (disposed)** / row double-click | `using (var dialog = new EditOrderDialog(…)) { var result = await dialog.ShowDialogAsync(); … }` | +1 while open, back to the previous value on close: `• server EditOrderDialog.Dispose disposed by the caller · live dialogs: session 0 · process 0` |
| `buttonEditLeak` **Edit (leak ×1)** | `var dlg = new EditOrderDialog(…); dlg.ShowDialog((f, r) => { /* no Dispose */ });` | +1 per click, **stays**: `⚠ boundary dialog leak 1 live dialog(s) held by session …`; the label turns red, the banner explains |
| `buttonReuse` **Reuse one dialog** | one instance in a field, `Bind(order)` before each show, disposed in the page's `Disposed` handler | +1 once, by design; the label says `reuse instance 1 (kept on purpose, disposed with the page)` |

The `using` block is the lesson's rule spelled for an awaited call: the `await` completes when the user closed the
dialog, the block ends, `Dispose` runs — deterministically, every time, on OK, on Cancel and on an exception in
`Save`. The callback form does the same with an explicit `f.Dispose()` as the last line of the callback.

## 3 · Reuse on purpose: make it visible and reset the state

Reusing a dialog is legitimate (a heavy editor, a dialog that keeps a user's filter) but it must be a decision, not
an accident:

- the instance lives in a **named field** (`MainPage._reusableDialog`) so there is an owner;
- `EditOrderDialog.Bind(order)` is public and **resets** the fields, the title and `DialogResult` before every show —
  stale state (the previous order's PO number, a leftover `DialogResult.OK`) is the reuse trap the storyboard warns
  about (`_editDialog?.ResetState()`);
- one place disposes it: `MainPage_Disposed` (subscribed to the page's `Disposed` event so the generated
  `Dispose(bool)` in `MainPage.Designer.cs` stays designer-owned).

Second click on **Reuse one dialog**: `• server EditOrderDialog.Bind order 1042 — fields and DialogResult reset
before re-showing (stale state is the reuse trap)` and the counter does not move.

## 4 · Return values that are decisions stay modal

`buttonDelete` **Delete order…**:

```csharp
var result = await MessageBox.ShowAsync($"Delete order {order.Id}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
trace.Add(TraceKind.FromClient, "MessageBox closed", $"DialogResult = {result}");
if (result == DialogResult.Yes) { _orderService.Delete(order.Id); …; Ui.Toast($"Order {order.Id} deleted."); }
```

A wrong answer destroys data, so the question blocks — but it blocks the **browser**, not the server thread; the
confirmation afterwards is a Toast. `NotificationsReview.md` applies the same rule to every MessageBox.

## 5 · Long work inside the workflow: blocking vs `Application.StartTask`

The dialog workflow is only as responsive as the handlers around it. The console pairs two buttons with a
`Wisej.Web.Timer` (200 ms) driving a ProgressBar as the probe:

| Button | Code | What the probe shows |
|---|---|---|
| `buttonBlocking` **Blocking op (3 s)** | `Thread.Sleep(3000)` inside the Click handler ✕ | the begin trace, the timer start and the end trace reach the browser in **one** response 3 s late; `• server Timer.Tick 0 tick(s) were answered while the operation ran (0 = the session was frozen)`; the bar jumps 0 → 100 |
| `buttonStartTask` **StartTask (3 s)** | `Application.StartTask(() => { Thread.Sleep(3000); Application.Update(this, () => { … }); })` ✓ | the handler returns at once, the bar animates, `~14 tick(s)` were answered, `• server StartTask.end … Application.Update(this, …) pushed this line from the worker thread`, a Toast confirms |

On the desktop a blocking handler froze one user's window. On the server it holds the session's request: every
other event of that session (timer ticks, clicks, the dialog's own Save) queues behind it, and a long one times
out. `StartTask` keeps the session context on the worker thread; `Application.Update(page, callback)` runs the
callback back in that context and pushes the changes (check `IsDisposed` first — the user may have left).

## Evidence (in the running app)

1. Double-click **1042** in the shell → the modal opens over the page; the counter reads `this session 1`. Press
   **Save** → Toast `Order 1042 saved.`, trace `OrderService.Save order 1042 · … CalculateOrderTotal = 4,820.00` then
   `EditOrderDialog.Dispose … session 0 · process 0`.
2. **Edit (leak ×1)** → close the dialog with Cancel → counter `this session 1` in red, banner
   `✕ Leak: 1 live EditOrderDialog(s) in this session …`. Press it again → `2`. Nothing brings it down.
3. **Reuse one dialog** twice → `new EditOrderDialog (reused) created once for this page …`, then `EditOrderDialog.Bind …`;
   the counter shows the one kept instance and does not grow.
4. **Delete order…** → **No**: `MessageBox closed DialogResult = No`, nothing changes. → **Yes**: `OrderService.Delete
   order 1042 removed from the shared repository (every session sees it)`, the grid reloads with 4 orders, Toast.
   (The repository is process-wide: restart the app to get 1042 back.)
5. **Blocking op (3 s)** then **StartTask (3 s)**: compare the two `Timer.Tick` lines (0 ticks vs ~14) and the two
   banners.
