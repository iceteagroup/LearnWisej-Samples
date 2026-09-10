# Deliverable 2 · Compiler error log — one form moved and building

What the compiler said after `OrdersForm.cs`, `OrdersForm.Designer.cs`, `EditOrderDialog.cs`, `EditOrderDialog.Designer.cs`
and the WinForms `Program.cs` were copied into the shell and `System.Windows.Forms` was replaced by `Wisej.Web`
(and `namespace LegacyOrderDesk` by `namespace OrderDesk`) with one search-and-replace. Every line was **categorized
before it was edited** — the rule of the module is "categorize, fix, rebuild; don't randomly edit designer files".
The same rows are compiled into the app as `Migration/CompilerErrorLog.cs`, which the console shows, filters and replays.

Legend: **#** = error lines the compiler printed for that symbol over all passes (0 = it compiled, the row records a
*silent* difference) · **Pass** = surfaced → fixed · **open** = still open after Module 2, carried in `migration-log.md`.

## Namespace / using  (2 error lines)

| Symbol | Code | # | Pass | Message | Fix | Module | open |
|---|---|---|---|---|---|---|---|
| `using LegacyOrderDesk.Reporting;` | CS0246 | 1 | 1 → 1 | The type or namespace name 'LegacyOrderDesk' could not be found | Deleted — InvoicePrinter/ExcelExport are desktop-only and were not copied | Module 2 | |
| `using LegacyOrderDesk.Settings;` | CS0246 | 1 | 1 → 1 | The type or namespace name 'LegacyOrderDesk' could not be found | Deleted — RegistrySettings is desktop-only and was not copied | Module 2 | |
| `using System.Windows.Forms;` / `namespace LegacyOrderDesk` | — | 0 | — | Done during the copy: one search-and-replace to Wisej.Web and OrderDesk | `using Wisej.Web;` `namespace OrderDesk` | Module 2 | |

## Control substitution  (26 error lines)

| Symbol | Code | # | Pass | Message | Fix | Module | open |
|---|---|---|---|---|---|---|---|
| `MenuStrip` | CS0234 | 1 | 1 → 1 | 'MenuStrip' does not exist in the namespace 'Wisej.Web' | `Wisej.Web.MenuBar`, `Dock = Top` | Module 2 | |
| `ToolStripMenuItem` (×10) | CS0234 | 10 | 1 → 1 | 'ToolStripMenuItem' does not exist in the namespace 'Wisej.Web' | `Wisej.Web.MenuItem` | Module 2 | |
| `StatusStrip` | CS0234 | 1 | 1 → 1 | 'StatusStrip' does not exist in the namespace 'Wisej.Web' | `Wisej.Web.StatusBar`, `Dock = Bottom`, `ShowPanels = true` | Module 2 | |
| `ToolStripStatusLabel` | CS0234 | 1 | 1 → 1 | 'ToolStripStatusLabel' does not exist in the namespace 'Wisej.Web' | `Wisej.Web.StatusBarPanel`, `AutoSize = Spring` | Module 2 | |
| `ToolStripMenuItem.DropDownItems` (×4) | CS1061 | 4 | 2 → 2 | 'MenuItem' does not contain a definition for 'DropDownItems' | `MenuItem.MenuItems.AddRange(…)` | Module 2 | |
| `ToolStripItem[]` (×6) | CS0234 | 6 | 2 → 2 | 'ToolStripItem' does not exist in the namespace 'Wisej.Web' | `MenuItem[]` for menus, `StatusBarPanel[]` for the status bar | Module 2 | |
| `MenuStrip.Items` | CS1061 | 1 | 2 → 2 | 'MenuBar' does not contain a definition for 'Items' | `MenuBar.MenuItems` | Module 2 | |
| `StatusStrip.Items` | CS1061 | 1 | 2 → 2 | 'StatusBar' does not contain a definition for 'Items' | `StatusBar.Panels` | Module 2 | |
| `Form.MainMenuStrip` | CS1061 | 1 | 2 → 2 | 'OrdersForm' does not contain a definition for 'MainMenuStrip' | Deleted — a docked MenuBar needs no owner property | Module 2 | |
| `DataGridView` + `DataGridViewTextBoxColumn`, `GroupBox`, `Label`, `Button`, `ComboBox`, `TextBox` | — | 0 | — | Compiled unchanged: `DataPropertyName`, `DefaultCellStyle.Format`, `Anchor`, `AutoSizeMode.Fill`, `DropDownStyle` | None needed | — | |

## Irrelevant styling  (3 error lines)

| Symbol | Code | # | Pass | Message | Fix | Module | open |
|---|---|---|---|---|---|---|---|
| `FormBorderStyle.FixedDialog` | CS0117 | 1 | 2 → 2 | 'FormBorderStyle' does not contain a definition for 'FixedDialog' | `FormBorderStyle.Fixed` (Wisej: None, Fixed, FixedToolWindow, Sizable, SizableToolWindow) | Module 2 | |
| `Application.EnableVisualStyles()` | CS0117 | 1 | 2 → 3 | 'Application' does not contain a definition for 'EnableVisualStyles' | Deleted with Program.cs — the theme comes from Default.json (`"theme": "Bootstrap-4"`) | Module 2 | |
| `Application.SetCompatibleTextRenderingDefault(false)` | CS0117 | 1 | 2 → 3 | 'Application' does not contain a definition for 'SetCompatibleTextRenderingDefault' | Deleted — the browser renders text | Module 2 | |
| `ISupportInitialize.BeginInit/EndInit`, `PerformLayout`, `TabIndex`, `ClientSize`, `ColumnHeadersHeightSizeMode` | — | 0 | — | Compiled — harmless in Wisej.Web, left in place so the designer file stays diff-able | None needed | — | |

## Unsupported desktop op  (8 error lines)

| Symbol | Code | # | Pass | Message | Fix | Module | open |
|---|---|---|---|---|---|---|---|
| `RegistrySettings.Load/Save` (×3) | CS0103 | 3 | 2 → 3 | The name 'RegistrySettings' does not exist in the current context | Original lines commented ✕; window size is the browser's; grid density → per-user profile store | Module 4 | open |
| `InvoicePrinter.Print` (PrintDocument, PrintPreviewDialog) | CS0103 | 1 | 2 → 3 | The name 'InvoicePrinter' does not exist in the current context | Commented ✕ + stand-in; InvoiceDocument stays, delivery becomes a server PDF in PdfViewer | Module 6 | open |
| `ExcelExport.ExportOrders` (Excel Interop, `C:\Orders`) | CS0103 | 1 | 2 → 3 | The name 'ExcelExport' does not exist in the current context | Commented ✕ + stand-in; managed writer + `Application.Download` | Module 6 | open |
| `SettingsForm` (edits HKCU) | CS0246 | 1 | 2 → 3 | The type or namespace name 'SettingsForm' could not be found | Commented ✕ + stand-in; ported with the per-user settings store | Module 4 | open |
| `Application.Run(new OrdersForm())` | CS0117 | 1 | 2 → 3 | 'Application' does not contain a definition for 'Run' | `Program.Main(NameValueCollection)` sets `Application.MainPage`; Default.json names it (`"startup"`) | Module 2 | |
| `LoginForm` (in Program.Main) | CS0246 | 1 | 2 → 3 | The type or namespace name 'LoginForm' could not be found | Not copied — the WinForms Program.cs is replaced by the shell's; login becomes the session sign-in | Module 4 | open |
| `[STAThread]` | — | 0 | — | Compiles anywhere but means nothing on a server thread pool — removed with Program.cs | Deleted | Module 2 | |

## Unclear / deferred  (0 error lines — compiles, behaves differently)

| Symbol | Code | # | Pass | Message | Fix | Module | open |
|---|---|---|---|---|---|---|---|
| `dialog.ShowDialog(this) == DialogResult.OK` (×3) | — | 0 | — | Compiles, but ShowDialog returns immediately on the web: the OK branch never runs | Minimal fix now: `ShowDialog(owner, (form, result) => …)`; the Module 3 rule is `ShowDialogAsync` + dispose in the caller | Module 3 | open |
| `using (var dialog = …) { dialog.ShowDialog(this) … }` | — | 0 | — | Compiles, but disposes the dialog while it is still open in the browser | Dispose in the callback (`form.Dispose()`) — done here; formalised in Module 3 | Module 3 | open |
| `new OpenFileDialog { Title = … }` | — | 0 | — | Compiles — `Wisej.Web.OpenFileDialog` exists, but it browses the **server's** file system | Commented ✕ + stand-in; Upload control + server storage root | Module 6 | open |
| static `AppState.CurrentUser / CurrentFilter / LastSearch` | — | 0 | — | Compiles — one static slot for every browser session (Module 1 showed the leak) | `Application.Session` behind a typed UserContext | Module 4 | open |
| `MessageBox.Show("Saved.", "LegacyOrderDesk")` | — | 0 | — | Works as-is for a notification; a Yes/No decision needs `ShowAsync` or the callback overload | Keep; review every MessageBox whose return value is read | Module 3 | open |
| `ordersGrid.DataSource = list; CurrentRow.DataBoundItem` | — | 0 | — | Compiles — list binding with `DataPropertyName` columns; loads every row like the desktop did | Verify in the browser; server-side paging + virtual mode for real volumes | Module 5 | open |

**Totals:** 39 error lines · 2 namespace · 26 control substitution · 3 irrelevant styling · 8 unsupported desktop op ·
0 unclear/deferred · **11 open items** (5 commented desktop ops + 6 silent differences).

## The four build passes: 15 → 24 → 10 → 0

| Pass | Errors | What the compiler stopped on | What was done before the next build |
|---|---|---|---|
| 1 | **15** | Declarations only: `MenuStrip`, `ToolStripMenuItem` ×10, `StatusStrip`, `ToolStripStatusLabel` in the designer's field list, plus the two dead `using LegacyOrderDesk.*` lines. | Four control substitutions (MenuBar, MenuItem, StatusBar, StatusBarPanel); two usings deleted. |
| 2 | **24** | The bodies surface: `DropDownItems` ×4, `ToolStripItem[]` ×6, `.Items` ×2, `MainMenuStrip`, `FixedDialog` — **plus** every desktop op the missing usings had hidden (`RegistrySettings` ×3, `InvoicePrinter`, `ExcelExport`, `SettingsForm`, `LoginForm`) and the WinForms `Program.cs` (`Run`, `EnableVisualStyles`, `SetCompatibleTextRenderingDefault`). | Member renames (`MenuItems`, `Panels`, `MenuItem[]`/`StatusBarPanel[]`), `MainMenuStrip` line deleted, `FormBorderStyle.Fixed`. |
| 3 | **10** | Only the desktop ops are left: `RegistrySettings` ×3, `InvoicePrinter`, `ExcelExport`, `SettingsForm`, `LoginForm`, `Application.Run` / `EnableVisualStyles` / `SetCompatibleTextRenderingDefault`. | Each original line commented `✕ compiler: <category>` with a one-line stand-in that raises `BoundaryHit`; the WinForms `Program.cs` not copied — the shell's `Program.cs` takes over. |
| 4 | **0** | Build succeeded. | *Compiling isn't done*: the six **unclear/deferred** rows compile and behave differently. The `ShowDialog` result and the `using`-disposal got their minimal fix (callback + dispose in the callback); the rest is logged. |

### Why the count *rises* after pass 1

The C# compiler suppresses errors that depend on a symbol it could not resolve, to avoid cascades. In pass 1 the
designer's field types (`MenuStrip`, `ToolStripMenuItem`…) are unresolved, so every member access on those fields
(`fileMenu.DropDownItems`, `menuStrip.Items`, `this.MainMenuStrip = …`) is not reported yet — and the array types in
`AddRange(new ToolStripItem[] …)` are never bound. Likewise, `using LegacyOrderDesk.Settings;` fails as a namespace,
so `RegistrySettings.Load()` is reported as part of *that* failure rather than as its own `CS0103`. Fix the
declarations and the compiler gets far enough to bind the bodies: 15 lines become 24. This is exactly why the lesson
says **rebuild often** — the first count is never the real count, and editing "until it's quiet" without categorizing
hides the desktop ops inside the noise of the control substitutions.

## Remaining / open items and where each is fixed

| Open item | Kind | Module |
|---|---|---|
| `RegistrySettings.Load/Save`, `SettingsForm` | HKCU is the service account's registry on the server | Module 4 · per-user profile store + browser storage |
| `LoginForm` in `Program.Main`, static `AppState.*` | one identity per process → one per session | Module 4 · `Application.Session` behind a typed context |
| `ShowDialog(...) == DialogResult.OK`, `using (dialog)`, `MessageBox.Show` return values | modal calls do not block | Module 3 · `ShowDialogAsync` / callback, caller disposes, Toast for notifications |
| `InvoicePrinter.Print` | printer attached to the server | Module 6 · server PDF + `PdfViewer` / download |
| `ExcelExport.ExportOrders`, `OpenFileDialog` + `C:\Orders\Attachments` | Office + the user's disk | Module 6 · managed .xlsx writer + `Application.Download`; `Upload` + storage root |
| `ordersGrid.DataSource = every row` | works in the lab, fails at production row counts | Module 5 · server-side filter + `VirtualMode` |

## Evidence (in the running app)

- **Compiler errors · categorized** card: the 30 rows with `Symbol | Code | # | Category | Fix | Module | open`; the
  six buttons filter by category and trace `← JS→.NET errors.filter category = control substitution → 10 rows · 26 error lines`;
  the count label reads `39 error lines over 4 passes → 0 · 11 open items`.
- **▶ Replay the build funnel**: a `Wisej.Web.Timer` (900 ms) walks the passes — the big label shows `15 errors`,
  `24 errors`, `10 errors`, then `0 errors · build succeeded` in green; the trace gets one `• server build pass n/4`
  line per pass with the summary above; the green banner says *Build succeeded — but compiling isn't done* and the
  status turns green.
- **Open the ported OrdersForm ↗**: the form runs; every commented desktop call reports `⚠ boundary` in the trace
  (`RegistrySettings.Load` on open, `InvoicePrinter.Print`, `ExcelExport.ExportOrders`, `OpenFileDialog`, `SettingsForm`,
  `RegistrySettings.Save` on close) — the open items, live.
