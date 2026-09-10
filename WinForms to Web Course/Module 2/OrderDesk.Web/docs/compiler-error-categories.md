# Compiler-error categories — porting `OrdersForm` into the Wisej.NET shell

Lab 2 deliverable: *build, classify compiler errors, fix direct differences, document what is
unsupported or unclear*. This is the log kept while `LegacyOrderDesk/OrdersForm.cs` +
`OrdersForm.Designer.cs` became `OrderDesk.Web/Pages/OrdersPage.cs` + `OrdersPage.Designer.cs`.

The rule from the video: **categorize every compile error — do not randomly edit designer files.**
Rebuild after each category so the count falls in steps you can explain: **14 → 6 → 0**.

## How the errors were counted

The counts below are *distinct fixes*. The raw error list of the first build is longer because every
designer line that touches a renamed type fails on its own (seven `ToolStripMenuItem` fields = seven
CS0234 lines for one fix). The video's build-output funnel counts the raw list (38 → 0) and sorts it
into four buckets; the last column maps each category to that bucket.

## The log

| # | Category | Example (from the real designer file) | Fix | Count | Video bucket |
|---|---|---|---|---|---|
| 0 | **Namespace swap** | `using System.Windows.Forms;` · `partial class OrdersForm : Form` · every `System.Windows.Forms.X` in the designer | `using Wisej.Web;` · `OrdersPage : UserControl` (`: Page` when the form is the main view) · find/replace `System.Windows.Forms.` → `Wisej.Web.` | 2 files, done **before** the first build — skipped, every line errors | — |
| 1 | **Renamed types** | `MenuStrip`, `ToolStripMenuItem`, `ToolStripItem[]`, `StatusStrip`, `ToolStripStatusLabel` | `MenuBar`, `MenuItem`, `MenuItem[]`, `StatusBar`, `StatusBarPanel` | 5 | Control substitution |
| 2 | **Designer-only properties** | `this.StartPosition = FormStartPosition.CenterScreen` · `this.MainMenuStrip = this.menuStrip` · `BorderStyle.FixedSingle` | delete (no screen to center on) · delete (the MenuBar is a control, it docks itself) · `BorderStyle.Solid` | 3 | Irrelevant styling |
| 3 | **Missing members** | `menuStrip.Items.AddRange(...)` · `fileMenu.DropDownItems` · `statusStrip.Items` · `Close()` in `exitMenuItem_Click` | `MenuItems.AddRange(...)` · `MenuItems` · `Panels` · removed — a hosted view has nothing to close; the session ends with the tab (Module 4 handles `ApplicationExit`/`SessionTimeout`) | 4 | Unsupported desktop op |
| 4 | **Startup** | `Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false); Application.Run(new OrdersForm());` (+ `[STAThread]`, the `LoginForm.ShowDialog()` before it) | `Program.Main(NameValueCollection args) { Application.MainPage = new MainPage(); }` named by `Default.json "startup"`; the host is `Startup.cs` (`app.UseWisej()`); login is Module 4 | 2 | Unsupported desktop op |
| — | **Business rule preserved** | `_service.GetAll()`, `_service.Save(...)`, `OrderCalculator`, `OrderValidator` | **0 edits** in `Domain/*.cs` — the same files the WinForms project compiled by link | 0 | Business rule |

Build sequence: first build **14** errors → fix categories 1 + 2 (pure renames/deletes) → **6** →
fix categories 3 + 4 (each needs a decision) → **0** → `dotnet run` → the page renders.

## What compiled unchanged (worth knowing)

- `((System.ComponentModel.ISupportInitialize)(this.ordersGrid)).BeginInit()/EndInit()` — `Wisej.Web.DataGridView` implements `ISupportInitialize`.
- `DataGridViewTextBoxColumn`, `DataPropertyName`, `HeaderText`, `Width`, `AutoSizeMode = Fill`,
  `DefaultCellStyle.Format = "C2"`, `DefaultCellStyle.Alignment = MiddleRight`, `SelectionMode = FullRowSelect`,
  `RowHeadersVisible`, `MultiSelect`, `ReadOnly`, `AllowUserToAddRows/DeleteRows`, `CellDoubleClick`, `SelectionChanged`,
  `CurrentRow.DataBoundItem`, `Rows[0].Selected`.
- `Anchor`, `Location`, `Size`, `MinimumSize`, `SuspendLayout/ResumeLayout`, `Label.AutoSize`, `TextBox.ReadOnly`, `Button.Click`.
- `MessageBox.Show(text)` and `MessageBox.Show(text, caption, MessageBoxButtons, MessageBoxIcon)` — the video's point:
  even the message boxes keep working (they are non-blocking dialogs on the web, but the call shape is the same).
- `this.Load += ...` — `UserControl` (and `Page`/`Form`) have `Load`.

## Documented, not fixed in Module 2 (the "unclear or unsupported" list)

| Line | Why it still compiles | Where it is handled |
|---|---|---|
| `AppState.CurrentOrder = o;` / `AppState.CurrentCustomer` | copied to `Legacy/AppState.cs` — a static works on the server, for *everyone at once* | Module 4 (session context) |
| `_service.GetAll()` → `ordersGrid.DataSource` | binds all 5,000 rows; Wisej streams rows in blocks so it renders, but the habit does not scale | Module 5 (VirtualMode + OrderQuery) |
| `BindingList<Order>` | replaced by `List<Order>` — the grid binds any `IList`; change notifications are not needed for a read-only grid | Module 5 |
| "Print Invoice", "Export to Excel" handlers | reduced to the same `MessageBox` text the desktop showed in its `catch` branches (no printer, no Excel on a server) | Module 6 (PDF, XlsxWriter, Download) |
| `statusLabel.Text = "... single-user desktop"` | still true in Module 2 — there is no login yet | Module 4 |
| `Font("Segoe UI", 9F)` on `detailTitle` | compiles; changed to `"default"` so the theme decides — desktop font names are an irrelevant-styling item | — |
| Culture of `"C2"` | the desktop used the PC's regional settings; the server formats for every browser. Pinned `FormatProvider = en-US` on the Total column so the figures match the videos ($4,820.00) | Module 7 (localization is a modernization item) |

## Evidence (what the running app shows)

- **Replay conversion** — the Timer writes one trace line per step: `• git switch -c migration/m2-shell`,
  `• dotnet new Wisej-4 …`, `• copy OrdersForm.cs …`, `★ namespace swap …`, `✖ dotnet build 14 errors …`,
  `★ classify · renamed types 5 — …`, `★ classify · designer-only properties 3 — …`, `★ classify · missing members 4 — …`,
  `★ classify · startup 2 — …`, `• fix direct differences …` + `✖ dotnet build 6 errors left …`, `• fix the rest …` +
  `✓ dotnet build 0 errors · 0 warnings`, `✓ dotnet run http://localhost:5102 …`, `★ business rule preserved …`.
  The **Compiler-error log** card fills one row per category (Category · Example · Fix · Count) and its
  counter goes `14 errors` (red) → `6 errors` (amber) → `0 errors ✓` (green). The replay ends by opening the OrdersPage.
- **Open OrdersPage ✓** — `★ namespace swap System.Windows.Forms → Wisej.Web · OrdersForm : Form → OrdersPage : UserControl …`
  followed by the ported page rendering with the five walkthrough orders on top.
