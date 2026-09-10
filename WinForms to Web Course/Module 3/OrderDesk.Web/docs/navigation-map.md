# Navigation map — LegacyOrderDesk → OrderDesk.Web (Module 3)

The desktop app had one main Form that carried its own chrome (MenuStrip, StatusStrip) and opened
other Forms from the menu. The web shell keeps the chrome once, in `MainPage`, and hosts the ported
screens as pages that are created once and shown by toggling `Visible`.

## Desktop (LegacyOrderDesk 3.2)

| Element | WinForms | Behaviour |
|---|---|---|
| Main window | `OrdersForm : Form` (720×341, `CenterScreen`) | `Application.Run(new OrdersForm())` after `LoginForm.ShowDialog()` |
| Menu | `MenuStrip` File · Edit · View · Reports · Help → About | `File › Exit` calls `Close()` → process ends |
| Status | `StatusStrip` + `ToolStripStatusLabel` "Ready · N orders · single-user desktop" | one label |
| Orders | the grid + detail panel inside `OrdersForm` | `DataGridView` bound to `BindingList<Order>` |
| Customers | (menu item, separate Form in the real app) | opened as a new top-level Form |
| Reports | Print Invoice / Export to Excel buttons on the detail panel | `PrintDocument` / Excel Interop |
| Edit | `EditOrderDialog : Form`, `ShowDialog(this)` on grid double-click | never disposed |

## Web (OrderDesk.Web, this module)

```
MainPage : Page                                  (1400×760 fits without scrolling)
├─ appBar            Panel  Dock Top 44          "OrderDesk — Module 3 · …" + ● idle/working/alarm
├─ trace             TracePanel  Dock Right 560  the migration trace (lab prop)
└─ shell             Panel  Dock Fill
   ├─ menuBar        MenuBar  Dock Top           File(Exit) · Edit(Edit selected order…, New order…) · View(Orders, Customers, Reports) · Reports(Open orders by customer, Invoices this month) · Help(About)
   ├─ navPanel       Panel  Dock Left 160        "OrderDesk" + Orders / Customers / Reports buttons (the video's dark rail)
   ├─ pageHost       Panel  Dock Fill, Padding 12
   │  ├─ OrdersPage     : ModulePage (Panel)     ← OrdersForm minus its chrome
   │  ├─ CustomersPage  : ModulePage             customer grid (Name · Country · Tier · Credit limit — never TaxId)
   │  └─ ReportsPage    : ModulePage             placeholder report list (Module 6 generates them)
   ├─ labelBanner    Label  Dock Bottom 34       finding / alarm banner (hidden until a path reports)
   ├─ buttonBar      Panel  Dock Bottom 52       Edit selected ✓ · Leak dialogs ✕ · Dispose fix ✓ · Saved via MessageBox ✕ → Toast ✓ · Tab order / docking
   └─ statusBar      StatusBar  Dock Bottom      "Ready · 5000 orders · multi-user web · session xxxxxxxx" | "Dialogs alive: n" | current page
```

### Mapping table

| WinForms | Wisej.NET | Note |
|---|---|---|
| `OrdersForm : Form` | `Pages/OrdersPage : Panel` hosted in `pageHost` | one instance, `Visible` toggled |
| `MenuStrip` / `ToolStripMenuItem` | `MenuBar` / `MenuItem` (`MenuItems.AddRange`) | moved from the form to the shell |
| `StatusStrip` / `ToolStripStatusLabel` | `StatusBar` / `StatusBarPanel` (`ShowPanels = true`, first panel `AutoSize = Spring`) | three panels instead of one label |
| menu item opens a new `Form` | nav button / View menu → `ShowPage(page)` | no new top-level window per screen |
| `File › Exit` → `Close()` (process exit) | logs a finding + `Notify.Info`; the real replacement is Module 4's Sign out | ending the process is not a web action |
| `Help › About` → `MessageBox.Show` | unchanged — a dialog the user asked for | stays modal on purpose |
| `EditOrderDialog : Form` + `ShowDialog(this)` | `Dialogs/EditOrderDialog : Wisej.Web.Form` + `ShowDialog()` **inside `using`** | see `modal-dialog-disposal.md` |

### Every entry point logs

`ShowPage` writes two trace lines: `← JS→.NET navigate → <Page>  <origin>` (origin = `navCustomers.Click`,
`View › Customers`, `Reports › Invoices this month`, `startup`, …) and
`• server ShowPage(<Page>)  <Type>.Visible = true · other pages hidden (same instances)`.

## Evidence

- **Startup**: the Orders page is visible with 1042 Northwind Traders $4,820.00 Open first; the trace shows
  `★ MenuStrip → MenuBar · StatusStrip → StatusBar`, `★ OrdersForm → OrdersPage : Panel`,
  `• new OrdersPage / CustomersPage / ReportsPage  one instance each · navigation toggles Visible, never recreates`,
  `• OrderService.GetAll()  5000 orders → ordersGrid.DataSource (BindingList<Order> → List<Order>)`, then
  `← navigate → Orders  startup` and `• ShowPage(Orders)`.
- **Nav › Customers** (or **View › Customers**): the Customers grid appears (12 rows); trace
  `← navigate → Customers  navCustomers.Click`, `• OrderService.Customers  12 customers → customersGrid.DataSource`,
  `★ TaxId never leaves the server  explicit columns Name/Country/Tier/CreditLimit · AutoGenerateColumns=false`,
  `• ShowPage(Customers)`; the status bar's right panel reads "Customers".
- **Nav › Reports** / **Reports › Open orders by customer**: the placeholder list appears with the item selected;
  trace `← reportsList.SelectedIndexChanged  "Open orders by customer"` and `• ReportsPage  placeholder — generation is Module 6 (PDF / .xlsx / queue)`.
- **File › Exit**: nothing closes; an AlertBox top-right says Exit ends only this session; trace
  `★ Exit is a session action on the web`.
- **Help › About**: a modal MessageBox "OrderDesk 4.0 (Module 3) — …"; trace `• MessageBox.Show(About)  modal on purpose: …`.
- Navigating back and forth never re-runs `OrderService.GetAll()` or `OrderService.Customers` — the pages are the same instances.
