# migration-log.md — LegacyOrderDesk → OrderDesk.Web

One line per accepted workaround or decision. Carried forward module by module; Module 2 adds the
shell and the first-form entries.

## Module 1 — Migration Discovery & the First Slice

- 2026-09-10 · M1 · Assessment workbook created; screens tagged; first slice = startup + login/user context + Orders read-only grid + EditOrderDialog + CSV export via Download; acceptance = parity first, modernization second, deployment third.
- 2026-09-10 · M1 · Business logic (OrderService, OrderCalculator, OrderValidator, OrderStore) reused as-is — shared `Domain/*.cs`.

## Module 2 — Project Conversion: the Wisej.NET shell

- 2026-09-10 · M2 · New Wisej.NET shell (`OrderDesk.Web`, Wisej-4 4.1.0, net10.0-windows;net10.0); `Application.Run` → `Program.Main` + `Default.json startup`; `App.config` → `Web.config` read via `AppConfig` (System.Xml.Linq).
- 2026-09-10 · M2 · Compiler errors classified: namespace swap, renamed types (MenuStrip→MenuBar, StatusStrip→StatusBar, ToolStripMenuItem→MenuItem), designer-only properties removed, startup rewritten.
- 2026-09-10 · M2 · Path B chosen (clean shell on branch `migration/m2-shell`, desktop build tagged `legacy-3.2`); forms move in one at a time — `OrdersForm` first.
- 2026-09-10 · M2 · `OrdersForm : Form` → `Pages/OrdersPage : UserControl` (hosted in the lab page); `: Page` + `Default.json mainWindow` is the equivalent when the form is the whole app. `ClientSize` → `Size`; `StartPosition`, `MainMenuStrip` deleted; `BorderStyle.FixedSingle` → `Solid`.
- 2026-09-10 · M2 · `Items`/`DropDownItems` → `MenuItems`; `StatusStrip.Items` → `StatusBar.Panels` (`StatusBarPanelAutoSize.Spring`); `ToolStripStatusLabel` → `StatusBarPanel`.
- 2026-09-10 · M2 · File → Exit: `Close()` removed — a hosted view has nothing to close; the session ends with the tab (cleanup on `ApplicationExit`/`SessionTimeout` is Module 4).
- 2026-09-10 · M2 · `BindingList<Order>` → `List<Order>` bound to `DataGridView.DataSource`; `Rows[i].DataBoundItem` still yields the `Order`. Full-table `GetAll()` kept for parity — Module 5 makes the grid virtual.
- 2026-09-10 · M2 · `Legacy/AppState.cs` copied unchanged so `AppState.CurrentOrder`/`CurrentCustomer` lines compile; marked ✕ — statics are process-wide on the server, Module 4 replaces them with `UserContext` in `Application.Session`.
- 2026-09-10 · M2 · Error counts recorded as distinct fixes (14 → 6 → 0); the raw funnel counts every failing line (38 → 0). Buckets: control substitution 5 · irrelevant styling 3 · unsupported desktop op 6 · business rule preserved 0 edits.
- 2026-09-10 · M2 · `Legacy/AppConfig` (ConfigurationManager-style `<exe>.config` next to the assembly) throws `FileNotFoundException` on the server — kept as the failure demo; `Services/AppConfig` (Web.config via `XDocument`, path from `Application.StartupPath` with `WEBSITE_PATH`/cwd fallbacks) is the replacement. `ExportFolder=C:\Orders` → `OrderDesk.StorageRoot=App_Data`; `LAN-SQL01` → `(local)`.
- 2026-09-10 · M2 · Total column formats with `FormatProvider = en-US` — the desktop used the PC's regional settings, the server formats for every browser; localization is a Module 7 modernization item.
- 2026-09-10 · M2 · Compiles ≠ migrated: two-session test (M4), 200k-row grid (M5), C:\Orders / Excel Interop / PrintDocument (M6) are still open.
