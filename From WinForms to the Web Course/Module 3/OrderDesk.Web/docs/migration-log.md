# migration-log.md — LegacyOrderDesk → OrderDesk.Web

One line per accepted decision or workaround. Later modules append to this file.

## Module 1 · Migration discovery (2026-09-09)

- **Assessment first.** 14 items inventoried and classified (4 direct-port · 5 adapt · 3 redesign · 1 defer · 1 remove) before any namespace was replaced. See `MigrationAssessmentWorkbook.md`.
- **Startup → `Default.json` + `Program.Main`.** `Application.EnableVisualStyles` / `Application.Run` have no equivalent; `Program.Main(NameValueCollection)` runs once per browser session and sets `Application.MainPage`. Kestrel (`Startup.cs`, `app.UseWisej()`) owns the process.
- **Business logic copied verbatim.** `Domain/` (Order, Customer, OrderService, CustomerService, InvoiceDocument, SampleData) has no UI or desktop dependency and moved unchanged. Totals are identical on both sides.
- **Static current user is a defect, not a style issue.** `AppState.CurrentUser` is one slot per server process; a second session overwrites it. Kept in `Legacy/` next to `Application.Session.User` so the leak is reproducible; the typed session context is Module 4's job.
- **Print Invoice → server PDF.** `PrintDocument` targets a printer attached to the server. The shared `InvoiceDocument` lines are written by a dependency-free `InvoicePdfWriter` and shown in a `PdfViewer` (+ Download). No printer, no local path.
- **Export to Excel → bytes + `Application.Download`.** Excel Interop is not supported in a server process and `C:\Orders\out.xlsx` is the user's disk. First slice ships CSV built in memory; Module 6 upgrades to a managed .xlsx writer and a report queue.
- **Attach file deferred to Module 6.** `OpenFileDialog` + `C:\Orders\Attachments` cannot exist on the server; the button logs the boundary instead of faking it. Replacement: `Upload` control + configured storage root.
- **Connection string → `Web.config`.** `App.config` is gone; settings live in the web host's configuration (`<appSettings>`, `<connectionStrings>`), read in Module 2.
- **Window-size restore removed.** The browser window belongs to the user; responsive layout (Module 7) replaces it.
- **Installer / ClickOnce deferred.** The web app is deployed once; checklist in Module 7.
- **Modal dialogs do not block in Wisej.NET.** `ShowDialog(callback)` / `ShowDialogAsync()`; the caller disposes the dialog when it closes (Module 3 rule, already applied to the invoice preview).

## Module 2 · Project conversion and Wisej.NET startup (2026-09-10)

- **The Wisej.NET shell replaces the .exe.** `Default.html` / `Default.json` (`"startup": "OrderDesk.Program.Main, OrderDesk"`, theme, debug) / `Program.cs` / `Startup.cs` (`app.UseWisej()`, no `AddWisej()` in Wisej-4 4.1.0) / `Web.config`; the project multi-targets `net10.0-windows;net10.0`.
- **`System.Windows.Forms` → `Wisej.Web`, one form at a time.** `OrdersForm` ported first: the class stays a `Form`, the handlers and the business logic are the same lines; `MenuStrip → MenuBar`, `ToolStripMenuItem → MenuItem`, `StatusStrip → StatusBar`, `ToolStripStatusLabel → StatusBarPanel`.
- **The compiler-error log is a deliverable.** Every place the compiler stopped is kept as the original line, commented and classified (namespace/using · control substitution · unsupported desktop op · unclear/deferred) with a one-line web-safe stand-in — the stand-ins are not the final replacements.
- **`App.config` → `Web.config`.** `<appSettings>` read through a small `AppConfig` helper (`System.Xml.Linq`), no `ConfigurationManager`.
- **Desktop-only helpers are not copied.** `InvoicePrinter`, `ExcelExport`, `RegistrySettings` stay in `LegacyOrderDesk`; their call sites become classified errors with stand-ins until Modules 3–6 replace them.

## Module 3 · UI porting, navigation, designer & modal workflow (2026-09-10)

- **The main form becomes a shell + screens.** `OrdersForm` → `Shell/AppShell` (`MenuBar` View · Reports · Help, `ToolBar` Orders · Customers · Reports ‖ New Order · Print Invoice · Export, `StatusBar`) with a content host; `Screens/OrdersScreen`, `CustomersScreen`, `ReportsScreen` are `UserControl`s, **one instance each for the life of the session**, hidden — never disposed — when another takes over (`NavigateTo`). See `NavigationPort.md`.
- **Fixed layout → Dock/Anchor, in the designer.** `ClientSize 716×372` for a 1024×768 desktop is gone: heading `Dock = Top`, detail `Dock = Right` (220), grid `Dock = Fill`; buttons in the detail panel `Anchor Top|Left|Right`. Docking order = reverse `Controls` order, so the `Fill` control is added first. `"C2"` totals became `"N2"` (the server's culture is not the user's).
- **`.Designer.cs` stays designer-owned.** Only `InitializeComponent`, the container, `Dispose` and the fields live there; the control swaps are recorded in `NavigationPort.md`.
- **`ShowDialog` returns at once.** The result is awaited (`await dialog.ShowDialogAsync()` in `async void`) or received in the `ShowDialog(callback)`; the desktop `if (dialog.ShowDialog() == OK)` never runs its body on the server. `EditOrderDialog` itself is unchanged (fields, `DialogResult`, `AcceptButton`).
- **Closed is not disposed — the caller owns the lifetime.** `using (var dialog = new EditOrderDialog(…)) { … await … }` (or `f.Dispose()` last in the callback). See `DialogWorkflow.md`.
- **Reuse only on purpose.** A reused dialog lives in a named field, is reset with `Bind(order)` (fields and `DialogResult`) before every show and is disposed with its owner. Accidental reuse (stale `DialogResult.OK`, the previous order's fields) is the trap.
- **MessageBox reviewed one by one.** Validation stays modal (`MessageBox.Show` for "Select a customer."); informational messages ("Saved.", "Exported to …", About) become `Ui.Toast`. See `NotificationsReview.md`.
- **Filter state is per screen, not static.** `AppState.CurrentFilter` → a field of `OrdersScreen` (one per session). The general rule lands in Module 4.
- **Settings, Exit and Attach file not ported.** `File › Settings…` (HKCU), `File › Exit` (`Close()` ended the process) and Attach file (the user's disk) are left out of the shell; sign-out and the per-user profile store are Module 4, the upload is Module 6.
