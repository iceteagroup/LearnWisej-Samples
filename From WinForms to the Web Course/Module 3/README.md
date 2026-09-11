# OrderDesk.Web · From WinForms to the Web · Module 3

Local lab build for **Module 3 · Forms, Navigation, Layouts, and Modal Workflow**. The main shell of
**LegacyOrderDesk** (MenuStrip, action buttons, StatusStrip, one grid) is ported to a `MenuBar` / `ToolBar` shell that
swaps three screens, the modal `EditOrderDialog` is ported as a `Wisej.Web.Form` with the **same fields and
DialogResult** and is **disposed by the caller** every time, the "Saved." MessageBox becomes a **Toast**, and the fixed
716×372 layout becomes Dock/Anchor.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 3/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5603
```

Then open <http://localhost:5603>. (Visual Studio: open `OrderDesk.slnx`, F5. The WinForms originals of the three
ported forms are under `OrderDesk.Web/Legacy/WinForms/` for side-by-side reading; they are not compiled.)

## What to try

| Action | What you should see |
|---|---|
| Page load | The shell (View · Reports · Help menu, ToolBar, StatusBar) on the Orders screen: 1042 Northwind Traders 4,820.00 Open first; the StatusBar reads `Orders · 5 orders · filter all` |
| ToolBar **Orders**, **Customers**, **Reports** (or the View menu) | The screens swap inside the same host; going back to Orders reuses the same screen instance |
| **View › Open orders only** / **All orders** | 2 open orders, heading `Orders · Open only`; All restores 5 |
| **Double-click order 1042** (or **Edit…**) → change Status → **Save** | The modal dialog opens; Save updates the row and a Toast **Order 1042 saved.** appears top-right. The dialog is awaited (`ShowDialogAsync`) and disposed by a `using` block |
| Clear the Customer and press **Save** | The validation message `Select a customer.` stays a modal MessageBox |
| ToolBar **New Order** → **Save** | Order 1043 (190.00) at the top, Toast **Order 1043 saved.** |
| ToolBar **Print Invoice** / Reports **Print Invoice (PDF)** | The invoice PDF opens in a modal `PdfViewer`; closing it disposes it in the `ShowDialog` callback |
| ToolBar **Export** | `orders.csv` downloads, and a Toast confirms |
| **Help › About** | A Toast instead of the old modal About box |

## Where things live

```
Module 3/
└─ OrderDesk.Web/                        the Wisej.NET 4 app (net10.0-windows;net10.0)
   ├─ Program.cs / Startup.cs            session entry point (Application.MainPage) / Kestrel host (app.UseWisej())
   ├─ Default.html / Default.json / Web.config
   ├─ Domain/                            reused unchanged: Order, Customer, OrderService, CustomerService, InvoiceDocument, SampleData
   ├─ Legacy/WinForms/                   the WinForms originals of OrdersForm, EditOrderDialog, SettingsForm (not compiled)
   ├─ MainPage.cs / .Designer.cs         the page: hosts the shell, Dock = Fill
   ├─ Shell/AppShell.cs (+ .Designer.cs) MenuBar · ToolBar · screen host (NavigateTo) · StatusBar
   ├─ Screens/ScreenBase.cs              what every screen shares: StatusText, OnShown
   ├─ Screens/OrdersScreen.cs (+ .Designer.cs)     the list screen: Dock/Anchor layout, CellDoubleClick → EditOrder (using + await), Toast
   ├─ Screens/CustomersScreen.cs (+ .Designer.cs)  the customer lookup
   ├─ Screens/ReportsScreen.cs (+ .Designer.cs)    Print Invoice → InvoicePreviewForm (disposed in the callback), Export → download
   ├─ Dialogs/EditOrderDialog.cs (+ .Designer.cs)  the ported modal dialog: same fields and DialogResult
   ├─ Reporting/InvoicePdfWriter.cs, CsvExport.cs  the Module 1 replacements for PrintDocument / Excel Interop
   ├─ Views/Ui.cs, InvoicePreviewForm.cs
   └─ docs/                              the lab deliverables
```

## Deliverables

1. **Ported navigation + list screen**: [`OrderDesk.Web/docs/NavigationPort.md`](OrderDesk.Web/docs/NavigationPort.md); the code is [`Shell/AppShell.cs`](OrderDesk.Web/Shell/AppShell.cs) and [`Screens/OrdersScreen.cs`](OrderDesk.Web/Screens/OrdersScreen.cs)
2. **An edit dialog with disposal**: [`OrderDesk.Web/docs/DialogWorkflow.md`](OrderDesk.Web/docs/DialogWorkflow.md); the code is [`Dialogs/EditOrderDialog.cs`](OrderDesk.Web/Dialogs/EditOrderDialog.cs) and `OrdersScreen.EditOrder`
3. **A Toast confirmation**: [`OrderDesk.Web/docs/NotificationsReview.md`](OrderDesk.Web/docs/NotificationsReview.md) (every MessageBox in LegacyOrderDesk reviewed: keep modal / Toast, with the reason)
4. **Before/after screens**: [`OrderDesk.Web/docs/BeforeAfterScreens.md`](OrderDesk.Web/docs/BeforeAfterScreens.md)
5. **Migration log**: [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md), carried forward from Modules 1–2 and extended

## Self-check answers (lab guide + video)

- **Which business logic was reused as-is?** Everything under `Domain/`, again unchanged: `OrderService.Save` is what
  the dialog's OK path calls (`CalculateOrderTotal = 4,820.00`, the same number as the desktop), `Search` is what the
  View filter calls, `CustomerService.GetCustomers` fills the dialog's combo box and the Customers screen.
- **Which desktop boundary was replaced with a web-safe pattern?** The blocking modal: `dialog.ShowDialog() == OK`
  became `await dialog.ShowDialogAsync()` inside a `using` block (the caller disposes), and `MessageBox.Show("Saved.")`
  became a Toast. Settings (HKCU), Exit and Attach file are not ported yet (Modules 4 and 6).
- **How is per-user state kept out of static fields?** The View filter that lived in `AppState.CurrentFilter` is now
  a field of `OrdersScreen`, and there is exactly one `OrdersScreen` per session (the shell keeps one instance per screen).
- **What was tested before calling the migrated feature complete?** The five orders render in the docked layout;
  double-click → Save produces the desktop total and a Toast; Cancel leaves the order untouched; the validation
  message still blocks Save; the invoice preview closes and is disposed.
- **Why can repeatedly creating modal dialogs leak server-side objects?** Because `Close()` hides a Wisej.NET form
  and nothing disposes it: the form, its controls, its handlers and the order it holds stay reachable through the
  session. On the desktop the process end cleaned up; a server session lives for hours and the process for weeks.
- **Which confirmations deserve a block, and which deserve a Toast?** Block when the answer changes what happens next
  or the user must fix input (`Select a customer.`). Toast when nothing is decided ("Saved.", "Exported…", About).

## Runtime facts

- `Form.ShowDialog()` returns immediately in Wisej.NET; use `ShowDialog((form, result) => …)` or `await form.ShowDialogAsync()` in an `async void` handler.
- A closed form is not disposed. `using` around the awaited call (or `form.Dispose()` last in the callback) releases it deterministically.
- Docking is applied in reverse order of the `Controls` collection: add the `Dock = Fill` control first, the edge bars after.
- `StatusBar` shows its `Text` unless `ShowPanels = true`; a `StatusBarPanel` with `AutoSize = Spring` takes the remaining width.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
