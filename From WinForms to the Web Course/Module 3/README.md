# OrderDesk.Web · From WinForms to the Web · Module 3

Local lab build for **Module 3 · Forms, Navigation, Layouts, and Modal Workflow**. It follows the lesson and the
walkthrough video: the main shell of **LegacyOrderDesk** (MenuStrip, action buttons, StatusStrip, one grid) is
ported to a `MenuBar` / `ToolBar` shell that swaps three screens, the modal `EditOrderDialog` is ported as a
`Wisej.Web.Form` with the **same fields and DialogResult** and is **disposed by the caller** every time, the
"Saved." MessageBox becomes a **Toast**, and the fixed 716×372 layout becomes Dock/Anchor. A live counter
(`DialogTracker`) shows what the desktop habit — create a dialog, never dispose it — does on a server, and two
buttons compare a blocking handler with `Application.StartTask`.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 3/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5603
```

Then open <http://localhost:5603>. (Visual Studio: open `OrderDesk.slnx`, F5. The WinForms originals of the three
ported forms are under `OrderDesk.Web/Legacy/WinForms/` for side-by-side reading; they are not compiled.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Page load | success | Trace: `• server startup …`, `• server shell MenuStrip → MenuBar · buttons → ToolBar · StatusStrip → StatusBar · Form → screen host (Dock = Fill)`, `← JS→.NET navigate Orders → created OrdersScreen in screenHost (Dock = Fill)`, `• server OrderService.GetOrders 5 orders …`; the shell shows 1042 Northwind Traders 4,820.00 Open first; the StatusBar reads `Orders · 5 orders · filter all · session …`; the counter reads `this session 0 · process-wide 0` |
| ToolBar **Customers**, **Reports**, **Orders** (`toolCustomers`, `toolReports`, `toolOrders`) or the View menu | success (deliverable 1) | `navigate Customers → created CustomersScreen …` + `CustomerService.GetCustomers 8 customers`; `navigate Reports → created ReportsScreen …`; back to Orders: `navigate Orders → reused OrdersScreen …` — no second `GetOrders`, the screen instance is kept |
| **View › Open orders only** / **All orders** (`menuViewOpenOnly`, `menuViewAllOrders`) | success | `← JS→.NET View › filter status = Open (kept on this screen, not in a static)`, `OrderService.Search 2 orders · status = Open`; heading `Orders · Open only`; All restores 5 |
| **Double-click order 1042** (or `buttonEditGood` **Edit (disposed)**, or **Edit…** on the screen) → change Status → **Save** | success (deliverables 2 + 3) | The modal opens; counter `this session 1`; `EditOrderDialog closed DialogResult = OK`, `OrderService.Save order 1042 · status InProgress · CalculateOrderTotal = 4,820.00`, Toast **Order 1042 saved.** (`→ .NET→JS Ui.Toast … (was MessageBox.Show("Saved.") …)`), then `• server EditOrderDialog.Dispose disposed by the caller · live dialogs: session 0 · process 0` |
| `buttonEditLeak` **Edit (leak ×1)** → **Cancel** — twice | failure (the leak) | `• server new EditOrderDialog … ✕ no using, no Dispose planned`, on close `… closed, NOT disposed · live dialogs: session 1 · process 1` and `⚠ boundary dialog leak 1 live dialog(s) held by session …`; red banner `✕ Leak: 1 live EditOrderDialog(s) …`; status `● 1 leaked dialog(s) in this session`; second click → 2 — nothing brings it down |
| `buttonReuse` **Reuse one dialog** — twice | recovery (alternative) | First: `new EditOrderDialog (reused) created once for this page …`, counter `reuse instance 1 (kept on purpose, disposed with the page)`; second: `EditOrderDialog.Bind order 1042 — fields and DialogResult reset before re-showing …`; the count does not grow; green banner |
| `buttonDelete` **Delete order…** → **No**, then → **Yes** | success (a decision stays modal) | `→ .NET→JS MessageBox.ShowAsync "Delete order 1042?" YesNo — awaited …`, `← JS→.NET MessageBox closed DialogResult = No` / `= Yes`; on Yes `OrderService.Delete order 1042 removed …`, the grid reloads with 4 orders, Toast **Order 1042 deleted.** (restart the app to get 1042 back — the repository is process-wide) |
| `buttonBlocking` **Blocking op (3 s)** | failure (frozen session) | Nothing happens for 3 s, then `blocking.begin` and `blocking.end` arrive together, followed by `• server Timer.Tick 0 tick(s) were answered while the operation ran (0 = the session was frozen)`; the bar jumps 0 → 100; orange banner; status `● session frozen for 3 s by a blocking handler` |
| `buttonStartTask` **StartTask (3 s)** | progress / recovery | `StartTask.begin … the handler returns now`, the bar animates and the label counts `x.x s · n ticks`, after 3 s `StartTask.end … Application.Update(this, …) pushed this line from the worker thread`, Toast, `Timer.Tick ~14 tick(s) were answered …`; status `● background work finished · UI stayed responsive` |
| ToolBar **Print Invoice** (`toolPrintInvoice`) / **Export** (`toolExport`) | recovery (Module 1 boundaries, callback disposal) | `⚠ boundary Print Invoice PrintDocument → local printer ⇒ server PDF (n bytes) → PdfViewer`, the PdfViewer window; **Close** → `• server InvoicePreviewForm.Dispose closed with OK · disposed in the ShowDialog callback`. Export: `Application.Download orders.csv …` + Toast |
| ToolBar **New Order** (`toolNewOrder`) → **Save** | success | `New Order new Order (Northwind Traders · 1 × Developer seat) → EditOrder`; `OrderService.Save order 1043 · status Open · CalculateOrderTotal = 190.00`; Toast **Order 1043 saved.**; 1043 at the top of the grid |
| **File › Settings…**, **File › Exit**, **Help › About**, **Attach file…** (`menuFileSettings`, `menuFileExit`, `menuHelpAbout`, `buttonScreenAttach`) | failure (logged, not faked) / review | A `⚠ boundary` line and a warning Toast for Settings (HKCU), Exit (no process to end) and Attach (the user's disk); About is `MessageBox.Show("About") ⇒ Ui.Toast (informational, no decision)` |
| **Clear** (`buttonClear`) | – | Empties the trace |

The right-hand card is the **migration log · live trace**: every user action (`← JS→.NET`), every business-logic call
(`• server`), everything pushed to the browser (`→ .NET→JS`) and every desktop boundary hit and replaced (`⚠ boundary`).
The counter under the shell is `DialogTracker.LiveFor(Application.SessionId)` / `LiveTotal`, refreshed on every trace line.

## Where things live

```
Module 3/
└─ OrderDesk.Web/                        the Wisej.NET 4 app (net10.0-windows;net10.0)
   ├─ Program.cs / Startup.cs            session entry point (Application.MainPage) / Kestrel host (app.UseWisej())
   ├─ Default.html / Default.json / Web.config
   ├─ Domain/                            ✓ reused unchanged — Order, Customer, OrderService, CustomerService, InvoiceDocument, SampleData
   ├─ Legacy/WinForms/                   ✕ the WinForms originals of OrdersForm, EditOrderDialog, SettingsForm — excluded from compilation
   ├─ Shell/AppShell.cs (+ .Designer.cs) the ported shell: MenuBar · ToolBar · screen host (NavigateTo) · StatusBar; raises Trace
   ├─ Screens/ScreenBase.cs              what every screen shares: Trace event, StatusText, OnShown
   ├─ Screens/OrdersScreen.cs (+ .Designer.cs)     the list screen: Dock/Anchor layout, CellDoubleClick → EditOrder (using + await), Toast
   ├─ Screens/CustomersScreen.cs (+ .Designer.cs)  the customer lookup (direct-port)
   ├─ Screens/ReportsScreen.cs (+ .Designer.cs)    Print Invoice → InvoicePreviewForm (disposed in the callback), Export → download
   ├─ Dialogs/EditOrderDialog.cs (+ .Designer.cs)  the ported modal dialog: same fields, DialogResult, Bind(order) for reuse, Dispose → DialogTracker
   ├─ Dialogs/DialogTracker.cs           live EditOrderDialog instances per session and process-wide
   ├─ Reporting/InvoicePdfWriter.cs, CsvExport.cs  the Module 1 replacements for PrintDocument / Excel Interop
   ├─ Views/TracePanel.cs, TraceEventArgs.cs, Ui.cs, InvoicePreviewForm.cs
   ├─ MainPage.cs / .Designer.cs         the lab console
   └─ docs/                              the lab deliverables
```

## Deliverables

1. **Ported navigation + list screen** — [`OrderDesk.Web/docs/NavigationPort.md`](OrderDesk.Web/docs/NavigationPort.md) (MenuStrip/ToolStrip → MenuBar/ToolBar/Page mapping, screen hosting, tab order, Dock/Anchor vs fixed layout, designer best practice); the code is [`Shell/AppShell.cs`](OrderDesk.Web/Shell/AppShell.cs) and [`Screens/OrdersScreen.cs`](OrderDesk.Web/Screens/OrdersScreen.cs)
2. **An edit dialog with disposal** — [`OrderDesk.Web/docs/DialogWorkflow.md`](OrderDesk.Web/docs/DialogWorkflow.md) (ShowDialog does not block, callback vs `ShowDialogAsync`, the leak with `DialogTracker` evidence, reuse with `Bind`); the code is [`Dialogs/EditOrderDialog.cs`](OrderDesk.Web/Dialogs/EditOrderDialog.cs) and `OrdersScreen.EditOrder`
3. **A Toast confirmation** — [`OrderDesk.Web/docs/NotificationsReview.md`](OrderDesk.Web/docs/NotificationsReview.md) (every MessageBox in LegacyOrderDesk reviewed: keep modal / AlertBox / Toast, with the reason)
4. **Before/after screens** — [`OrderDesk.Web/docs/BeforeAfterScreens.md`](OrderDesk.Web/docs/BeforeAfterScreens.md) (the lab step "Record before/after screenshots", as text + layout)
5. **Migration log** — [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md), carried forward from Modules 1–2 and extended

## Self-check answers (lab guide + storyboard)

- **Which business logic was reused as-is?** Everything under `Domain/`, again unchanged: `OrderService.Save` is what
  the dialog's OK path calls (`CalculateOrderTotal = 4,820.00`, the same number as the desktop), `Search` is what the
  View filter calls, `CustomerService.GetCustomers` fills the dialog's combo box and the Customers screen.
- **Which desktop boundary was replaced with a web-safe pattern?** The blocking modal: `dialog.ShowDialog() == OK`
  became `await dialog.ShowDialogAsync()` inside a `using` block (the caller disposes), and `MessageBox.Show("Saved.")`
  became `Ui.Toast("Order 1042 saved.")`. The blocking handler became `Application.StartTask` + `Application.Update`.
  Settings (HKCU) and Exit (`Close()` ending the process) are logged as boundaries for Module 4.
- **How is per-user state kept out of static fields?** The View filter that lived in `AppState.CurrentFilter` is now
  a field of `OrdersScreen`, and there is exactly one `OrdersScreen` per session (the shell keeps one instance per
  screen). The reusable dialog is a field of the page, i.e. of the session, disposed with it.
- **What was tested before calling the migrated feature complete?** The five orders render in the docked layout;
  double-click → Save produces the desktop total and a Toast; the counter returns to 0 after every disposed dialog
  and rises by one per leaked one; Cancel leaves the order untouched; Delete › No changes nothing, Delete › Yes removes
  the row; a 3 s handler freezes the session (0 timer ticks) while `StartTask` keeps it responsive (~14 ticks).
- **Why can repeatedly creating modal dialogs leak server-side objects?** (storyboard) Because `Close()` hides a
  Wisej.NET form and nothing disposes it: the form, its controls, its handlers and the order it holds stay reachable
  through the session. On the desktop the process end cleaned up at the end of the day; a server session lives for
  hours and the process for weeks, and every user leaks in parallel. The `DialogTracker` count is the proof.
- **Which confirmations deserve a block, and which deserve a Toast?** (storyboard) Block when the answer changes what
  happens next — a Yes/No before deleting, a validation failure the user must fix (`Select a customer.`). Toast when
  nothing is decided — "Saved.", "Exported…", About. The table in `NotificationsReview.md` applies this to every
  MessageBox in LegacyOrderDesk.

## Runtime facts

- `Form.ShowDialog()` returns immediately in Wisej.NET; use `ShowDialog((form, result) => …)` or `await form.ShowDialogAsync()` in an `async void` handler. `await MessageBox.ShowAsync(…)` is the awaitable MessageBox.
- A closed form is not disposed. `using` around the awaited call (or `form.Dispose()` last in the callback) releases it deterministically; `DialogTracker` counts what is still alive.
- `Application.StartTask` runs work off the request thread with the session context; `Application.Update(page, callback)` runs the callback in that context and pushes once. Check `IsDisposed` first.
- Docking is applied in reverse order of the `Controls` collection: add the `Dock = Fill` control first, the edge bars after.
- `StatusBar` shows its `Text` unless `ShowPanels = true`; a `StatusBarPanel` with `AutoSize = Spring` takes the remaining width.
- The in-memory repository is process-wide: an order deleted (or added) in one session is gone (or there) for every session until the app restarts.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
