# Migration assessment workbook — LegacyOrderDesk

**Lab 1 deliverable 1.** The inventory of the WinForms application the course migrates, one row per
form, feature or subsystem, each with the dependency that matters for a web server, a risk tag, a
verdict (direct-port · adapt · redesign · defer · remove) and an effort (XS · S · M · L). The same
table is the C# list in `Migration/AssessmentWorkbook.cs` and the grid the running app shows.

## Inventory

| # | Form / feature | Dependency | Risk tag | Verdict | Effort | Where (LegacyOrderDesk) |
|---|---|---|---|---|---|---|
| 1 | OrdersForm · orders grid | `DataGridView` · full table load (`GetAll()`) | `grid-volume` | adapt | M | `OrdersForm.cs ReloadGrid()` |
| 2 | OrdersForm · Print Invoice | `PrintDocument` → local printer | `report` | redesign | M | `Legacy/InvoicePrinter.cs` |
| 3 | OrdersForm · Export to Excel | Excel Interop (COM `Excel.Application`) | `office` | redesign | M | `Legacy/ExcelExport.cs` |
| 4 | `AppState.CurrentUser / CurrentCustomer / ActiveFilter / CurrentOrder` | static fields | `static-state` | adapt | S | `Legacy/AppState.cs` |
| 5 | `UserPreferences` (LastUser) | HKCU registry `Software\LegacyOrderDesk` | `user-settings` | adapt | S | `Legacy/UserPreferences.cs` |
| 6 | `LocalExport` → `C:\Orders\out.csv` | local file path | `file-system` | adapt | S | `Legacy/LocalExport.cs` |
| 7 | LoginForm | standard controls (ComboBox, Button) | `direct-port` | direct-port | S | `LoginForm.cs` |
| 8 | EditOrderDialog | modal Form · `ShowDialog` · never disposed | `modal` | adapt | S | `EditOrderDialog.cs` |
| 9 | MenuStrip / StatusStrip | ToolStrip family | `ui-shell` | adapt | S | `OrdersForm.Designer.cs` |
| 10 | App.config | `ConfigurationManager` appSettings + connectionStrings | `config` | adapt | XS | `App.config` |
| 11 | OrderService / OrderCalculator / OrderValidator / OrderStore | pure C# · no UI dependency | `business-logic` | direct-port (reuse as-is) | XS | `Domain/*.cs` (linked into both projects) |
| 12 | File → Exit | `Form.Close()` ends the process | `process-lifetime` | remove | XS | `OrdersForm.cs exitMenuItem_Click` |
| 13 | Help → About | MessageBox with the version string | `direct-port` | defer | XS | `OrdersForm.cs aboutMenuItem_Click` |

Totals: direct-port 2 · adapt 7 · redesign 2 · defer 1 · remove 1.

## What each finding means for the server

| # | Finding | Fixed in |
|---|---|---|
| 1 | Binds **every** row. Fine on a LAN desktop; on the web every row is a payload to the browser. | Module 5 — `VirtualMode` + server-side `OrderQuery` |
| 2 | There is no user printer on the server. `PrintDocument` prints on the machine running the code. | Module 6 — server PDF in `PdfViewer` / `Application.Download` |
| 3 | Office Automation is unsupported server-side: no interactive desktop, `COMException 0x80040154`, or a hang. | Module 6 — managed `.xlsx` writer + `Application.Download` |
| 4 | One process = one user on the desktop; one process = **every session** on the server. The static is shared by all browser tabs. | Module 4 — `Application.Session` / `UserContext` |
| 5 | Reads the **server's** registry under the service account: wrong machine, wrong user. | Module 4 — per-user server profile |
| 6 | The file system is the server's, not the user's. `C:\Orders\out.csv` never reaches the browser. | Module 1 (this slice) — `Application.Download`; Module 6 — `App_Data` storage root |
| 7 | `System.Windows.Forms` → `Wisej.Web`, same handlers. The registry default moves with #5. | Module 2 |
| 8 | `ShowDialog` keeps working; closed dialogs are **not** disposed on the server. `MessageBox.Show("Saved.")` should not block. | Module 3 — `using`/`Dispose`, `Toast` |
| 9 | `MenuStrip→MenuBar`, `StatusStrip→StatusBar`, `ToolStripMenuItem→MenuItem`. | Module 2 — compiler pass |
| 10 | `ExportFolder`, `ReportPrinter`, `LAN-SQL01` move to `Web.config`, read by `AppConfig`. | Module 2 |
| 11 | Reuse as-is. The same `Domain/*.cs` files are compiled by `LegacyOrderDesk` (linked) and `OrderDesk.Web`. | — |
| 12 | The browser tab is the exit; the session ends on `ApplicationExit` / `SessionTimeout`. | Module 4 |
| 13 | Harmless, not needed to prove the migration. | after the first slice |

## Migration Assessment Template (from the lesson), answered for LegacyOrderDesk

| Area | Answer | Decision |
|---|---|---|
| Forms and controls | 3 forms (LoginForm, OrdersForm, EditOrderDialog); standard controls only; no custom or third-party controls | direct port (LoginForm), adapt (OrdersForm shell, EditOrderDialog) |
| Business logic | Already extracted: `OrderService`, `OrderCalculator`, `OrderValidator`, `OrderStore`; event handlers only call them | reuse as-is |
| Data access | In-memory `OrderStore` (5,000 orders); the grid loads the whole table | port; optimize the grid (Module 5) |
| State | `AppState` statics for user, customer, filter, current order; `Countries` static readonly | session (per-user), global cache (immutable) |
| Desktop boundary | `C:\Orders` files, HKCU registry, Excel COM, default printer | replace each with a web-safe pattern (Download, server profile, managed writer, PDF) |
| Deployment | Kestrel + Wisej.NET, `net10.0-windows;net10.0`; the `Legacy/*` pieces are the only Windows-only code | target both TFMs; keep Windows-only code out of the web project |

## Acceptance criteria (non-negotiable, in this order)

1. **Parity first** — the same workflow works in the browser: sign in, see the orders, edit one, export.
2. **Modernization second** — no redesign until parity is proven (the dashboard of Module 7 waits).
3. **Deployment third** — the app runs on the target host with no printer, no Excel, no `C:\Orders`.

## Evidence (what the running app shows)

- On load the **Assessment workbook** grid shows the 13 rows above with the risk tags coloured as in
  the video (`grid-volume` amber, `report` pink, `office`/`static-state` red, `file-system` purple,
  `direct-port` green). Trace: `• server  AssessmentWorkbook  13 items · direct-port 2 · adapt 7 · redesign 2 · defer 1 · remove 1`.
- **Replay assessment** clears the grid and a `Wisej.Web.Timer` (450 ms) adds one row at a time, each
  with a `★ log <risk-tag>  <feature> → <verdict> (<effort>) · <finding>` line in the trace, ending
  with `✓ ok  assessment complete  13 items · …` and a blue banner.
- Hovering a row shows the source file (Form / feature column) and the finding (Dependency column).
