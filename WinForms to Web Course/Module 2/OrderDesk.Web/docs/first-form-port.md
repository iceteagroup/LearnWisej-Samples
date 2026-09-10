# First form port — `OrdersForm` → `Pages/OrdersPage`

Lab 2 deliverable: *one form moved and building — namespace swapped, errors categorized*. This
documents what moved, what changed and what deliberately did **not** change when the main WinForms
screen of LegacyOrderDesk became the first Wisej.NET page.

## What moved

| Desktop (`LegacyOrderDesk/`) | Web (`OrderDesk.Web/`) | Change |
|---|---|---|
| `OrdersForm.cs` | `Pages/OrdersPage.cs` | `using System.Windows.Forms;` → `using Wisej.Web;` · `: Form` → `: UserControl` · handlers unchanged except `exitMenuItem_Click` (see below) |
| `OrdersForm.Designer.cs` | `Pages/OrdersPage.Designer.cs` | `System.Windows.Forms.` → `Wisej.Web.` · 14 fixes, all commented `// was: …` in the file |
| `Legacy/AppState.cs` | `Legacy/AppState.cs` | copied as-is (namespace `OrderDesk.Legacy`) so the `AppState.CurrentOrder = o;` lines compile — marked ✕, refactored in Module 4 |
| `App.config` | `Web.config` + `Services/AppConfig.cs` | see [`shell-anatomy.md`](shell-anatomy.md); `Legacy/AppConfig.cs` shows the old lookup failing |
| `OrderDesk.Web/Domain/*.cs` (compiled by link) | `Domain/*.cs` | **0 edits** — `OrderService`, `OrderCalculator`, `OrderValidator`, `OrderStore` are the same files |
| `Program.cs` | `Program.cs` + `Startup.cs` + `Default.json` | rewritten, not ported — see [`shell-anatomy.md`](shell-anatomy.md) |
| `LoginForm.cs`, `EditOrderDialog.cs`, `Legacy/{UserPreferences,LocalExport,ExcelExport,InvoicePrinter}.cs` | not yet | Modules 3, 4 and 6 |

## Form → Page … or UserControl?

The video shows `public partial class OrdersPage : Page`. A `Wisej.Web.Page` is a top-level view
that fills the browser and is shown with `Application.MainPage = …` (or `Default.json "mainWindow"`).
In this lab the ported form is **hosted inside the lab's `MainPage`** next to the migration trace,
so it derives from `Wisej.Web.UserControl` — a designable container that can be placed in a
`Panel`. Everything else (fields, `InitializeComponent()`, handlers) is identical; when the ported
form is the whole app, change the base class to `Page` and it becomes the video's variant.

## Control by control

| WinForms | Wisej.Web | Notes |
|---|---|---|
| `MenuStrip menuStrip` + `ToolStripMenuItem` ×7 | `MenuBar menuBar` + `MenuItem` ×7 | `Items.AddRange(new ToolStripItem[]…)` → `MenuItems.AddRange(new MenuItem[]…)`; `DropDownItems` → `MenuItems`; `Click` unchanged; `MainMenuStrip` gone |
| `DataGridView ordersGrid` + 4 `DataGridViewTextBoxColumn` | same names | columns kept: Order (70) · Customer (Fill) · Total (90, `C2`, right) · Status (80); `FullRowSelect`, no row headers, read-only |
| `Panel detailPanel` (`BorderStyle.FixedSingle`) | `Panel` (`BorderStyle.Solid`) | rows packed from 292 to 250 px because the hosted view is 296 px tall instead of 341 |
| `Label` ×4, `TextBox` ×3 (`ReadOnly`) | same | `Font("Segoe UI", 9F, Bold)` → `Font("default", 10F, Bold)` so the theme decides |
| `Button` ×3 — New Order · Print Invoice · Export to Excel | same | handlers keep the desktop's `MessageBox` texts; the real work is Module 3 (dialog) and Module 6 (PDF, xlsx) |
| `StatusStrip statusStrip` + `ToolStripStatusLabel statusLabel` | `StatusBar statusBar` + `StatusBarPanel statusLabel` (`AutoSize = Spring`, `ShowPanels = true`) | `Items.AddRange` → `Panels.AddRange`; `statusLabel.Text = "Ready · N orders · single-user desktop"` unchanged |
| `ClientSize (720, 341)`, `MinimumSize`, `StartPosition`, `Text` | `Size (720, 341)`, `MinimumSize`, —, `Text` | no window frame, no screen |
| `BindingList<Order> _rows` | `List<Order> _rows` | the grid binds any list; `Rows[i].DataBoundItem` still returns the `Order` |
| `MessageBox.Show(...)` | `MessageBox.Show(...)` | same call, same overloads — shown as a dialog in the page |
| `Close()` (File → Exit) | removed — shows a message | a hosted view has nothing to close; the session ends with the tab (Module 4) |

## Kept on purpose (parity first) — and marked ✕

```csharp
AppState.CurrentOrder = o;                 // ✕ static per-user state (Module 4)
AppState.CurrentCustomer = o?.Customer;    // ✕ static per-user state (Module 4)
_rows = _service.GetAll();                 // ✕ loads EVERY row (Module 5)
statusLabel.Text = "Ready · " + _rows.Count + " orders · single-user desktop";   // ✕ (Module 4)
```

The module's outcome is *a migrated form runs in the browser*, not *the form is web-ready*. The
video's closing caution applies: it compiles and renders, but connection strings, statics and the
two-session test are still ahead.

## The lab hook

`OrdersPage` exposes one thing the desktop form did not have:
`public event Action<string, string> Activity;` — the host subscribes and writes every call the
ported form makes (`OrderService.GetAll()`, detail-panel updates, each button/menu click) into the
trace. It is a course prop, not a product feature, and is the only non-port line in the file.

## Evidence (what the running app shows)

- **Open OrdersPage ✓** — trace: `← click Open OrdersPage ✓`, `★ namespace swap System.Windows.Forms → Wisej.Web · OrdersForm : Form → OrdersPage : UserControl …`,
  `• new OrdersPage() InitializeComponent() — the migrated .Designer.cs …`, `• OrdersPage.OrderService.GetAll() 5000 orders · first 1042 Northwind Traders $4,820.00 Open`,
  `• OrdersPage.detail panel 1042 Northwind Traders $4,820.00 Open · AppState.CurrentOrder set (✕ static)`,
  `→ ordersGrid.DataSource List<Order> ×5000 → DataGridView …`, `✓ OrdersPage renders in the browser · MenuBar … · StatusBar 'Ready · 5000 orders · single-user desktop'`,
  `★ parity, not done …`. The card shows the grid with **1042 Northwind Traders $4,820.00 Open · 1041 Contoso Ltd $1,290.50 Shipped ·
  1040 Fabrikam Inc $760.00 Open · 1039 Adventure Works $12,400.00 Invoiced · 1038 Globex Corp $3,090.00 Open** on top, the detail panel
  on "Order 1042 / Northwind Traders / NW-88231 / 10 items", the menu bar and the status bar.
- Clicking a row → `• OrdersPage.detail panel <order> …`. Double-click, **New Order**, **Print Invoice**, **Export to Excel**,
  **File → Exit**, **Help → About** → one `• OrdersPage.<action>` line each plus a `MessageBox` in the page (the desktop texts,
  pointing at the module that finishes the job).
- Clicking **Open OrdersPage ✓** again → `• OrdersPage.ReloadGrid() already open — reloading (idempotent)`.
