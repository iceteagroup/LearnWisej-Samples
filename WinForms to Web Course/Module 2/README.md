# OrderDesk · From WinForms to the Web · Module 2

Local lab build for **Module 2 · Project Conversion: the Wisej.NET shell**. It follows the walkthrough
video (`wisej-wf-project-conversion`: two conversion paths, the shell anatomy, the namespace swap,
startup moving to the host, the moved `OrdersPage` in the Designer, the error funnel to zero) and
the lab (*migrate the first form into a Wisej.NET app shell, run it, and document each compiler
error category*).

The app is the migration at the end of Module 2: the `OrderDesk.Web` shell (Wisej-4 4.1.0,
`net10.0-windows;net10.0`) hosting the first ported form — `LegacyOrderDesk.OrdersForm` moved to
`Pages/OrdersPage` — with a **Shell anatomy** card reading the startup files live, a
**Compiler-error log** card, and the `App.config` → `Web.config` move as failure + recovery.
The `LegacyOrderDesk` WinForms project itself lives in Module 1 (the "before"); this module only
carries `Legacy/AppState.cs` across so the ported form compiles unchanged.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/WinForms to Web Course/Module 2/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5102
```

Then open <http://localhost:5102>. (Visual Studio: open `OrderDesk.slnx`, press F5 — `launchSettings.json`
uses port 5102.) The page is laid out for a 1400×760 viewport: cards on the left, the migration trace on the right.

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

On load the trace shows the startup: `• Program.Main  Application.MainPage = new MainPage() · session ……`,
`• Startup.cs  Kestrel owns the process · app.UseWisej() · no EnableVisualStyles, no Application.Run`,
`• Default.json  "startup": "OrderDesk.Program.Main, OrderDesk" …`, one `• <file>` line per shell part, and
`★ shell  OrderDesk.Web = Wisej-4 4.1.0 · net10.0-windows;net10.0 · Path B …`. The status label is `● idle`.

| Action | Path | What you should see |
|---|---|---|
| **Open OrdersPage ✓** | success | `★ namespace swap  System.Windows.Forms → Wisej.Web · OrdersForm : Form → OrdersPage : UserControl …`, `• new OrdersPage()  InitializeComponent() — the migrated .Designer.cs …`, `• OrdersPage.OrderService.GetAll()  5000 orders · first 1042 Northwind Traders $4,820.00 Open`, `• OrdersPage.detail panel  1042 Northwind Traders $4,820.00 Open · AppState.CurrentOrder set (✕ static)`, `→ ordersGrid.DataSource  List<Order> ×5000 → DataGridView …`, `✓ OrdersPage  renders in the browser · MenuBar File/Edit/View/Reports/Help · StatusBar 'Ready · 5000 orders · single-user desktop'`, `★ parity, not done  compiles + renders ≠ migrated …`. The card shows the ported form: menu bar, grid with **1042 Northwind Traders $4,820.00 Open · 1041 Contoso Ltd $1,290.50 Shipped · 1040 Fabrikam Inc $760.00 Open · 1039 Adventure Works $12,400.00 Invoiced · 1038 Globex Corp $3,090.00 Open**, detail panel "Order 1042 / Northwind Traders / NW-88231 / 10 items", status bar. Banner: `✓ OrdersPage open — the first form runs in the browser: same OrderService, same handlers, MessageBox.Show still works (try Help → About).` Click again → `• OrdersPage.ReloadGrid()  already open — reloading (idempotent)`. |
| Inside the ported form: click a row · double-click · **New Order** · **Print Invoice** · **Export to Excel** · **File → Exit** · **Help → About** | parity | one `• OrdersPage.<action>` line each (`detail panel …`, `CellDoubleClick … EditOrderDialog is ported in Module 3 · MessageBox.Show works`, `Print Invoice ✕ … PrintDocument targets the SERVER's printer — Module 6 makes a PDF`, `Export to Excel ✕ … C:\Orders on the SERVER — Module 6`, `File → Exit  Close() removed — the session ends with the tab …`, `Help → About  MessageBox.Show works in Wisej.Web`) and the desktop's MessageBox text shown as a dialog in the page. Nothing prints, nothing is written. |
| **Replay conversion** | progress (Timer, 650 ms) | `• replay  Lab 2 · 13 steps …`, then step by step: `• git  switch -c migration/m2-shell …`, `• dotnet new  Wisej-4 4.1.0 web app → OrderDesk.Web …`, `• copy  OrdersForm.cs + OrdersForm.Designer.cs → Pages/OrdersPage.cs …`, `★ namespace swap …`, `✖ dotnet build  14 errors …`, `★ classify · renamed types  5 — …`, `★ classify · designer-only properties  3 — …`, `★ classify · missing members  4 — …`, `★ classify · startup  2 — …`, `• fix direct differences …` + `✖ dotnet build  6 errors left …`, `• fix the rest …` + `✓ dotnet build  0 errors · 0 warnings · net10.0-windows + net10.0`, `✓ dotnet run  http://localhost:5102 …`, `★ business rule preserved  … 0 edits in Domain/*.cs`. The **Compiler-error log** grid fills one row per category and its counter goes `14 errors` → `6 errors` → `0 errors ✓`; the status label is `● working` until the replay opens the OrdersPage and ends with the banner `✓ Replay done: … 14 → 6 → 0 errors → runs. Compiling is not "done" …`. Clicking again restarts it. |
| **App.config lookup ✕** | failure | `• Legacy.AppConfig  ConfigurationManager-style: read <entry assembly>.config next to the exe — on the desktop C:\Program Files\LegacyOrderDesk\LegacyOrderDesk.exe.config`, `• Legacy.AppConfig  on this server the entry assembly is OrderDesk.dll → looking for …\bin\Debug\net10.0\OrderDesk.dll.config`, `✖ FileNotFoundException  Configuration file 'OrderDesk.dll.config' was not found next to the server assembly. App.config is a desktop artifact … configuration moved to Web.config.`, `✖ App.config  ExportFolder = C:\Orders and connectionStrings[OrderDesk] → LAN-SQL01 were never deployed …`, `★ config  App.config → Web.config …`. Red banner `✖ configuration moved to Web.config — OrderDesk.dll.config is not next to the server assembly (there is no exe on the web).`; status `● alarm`. |
| **Web.config lookup ✓** | recovery | `• Services.AppConfig.Load  XDocument.Load(…\OrderDesk.Web\Web.config) · Application.StartupPath = …`, `✓ appSettings  OrderDesk.StorageRoot = App_Data · Wisej.DefaultTheme = Bootstrap-4 · Wisej.LicenseKey = (empty)`, `✓ connectionStrings  OrderDesk → Server=(local);Database=OrderDesk;Trusted_Connection=True; (System.Data.SqlClient)`, `✓ storage root  …\OrderDesk.Web\App_Data (relative to the web root; created on demand in Module 6)`, `★ config  ExportFolder=C:\Orders → OrderDesk.StorageRoot=App_Data · LAN-SQL01 → (local) …`. Green banner `✓ Web.config: OrderDesk.StorageRoot = App_Data · connectionStrings[OrderDesk] → (local) · theme Bootstrap-4 (read with System.Xml.Linq).`; the Web.config row of the Shell anatomy card refreshes; status `● idle`. |
| Hover a **Shell anatomy** value | — | the full live value as a tooltip (Default.html script tag and size · Default.json `startup`/`theme`/`url` · Program/Startup with session id and .NET version · Web.config counts · Wisej.Framework version + TFM · browser type/version/OS/viewport/session count). |
| **Clear** (trace title) | — | empties the trace list. |

Every button is safe to click repeatedly; no printer, Excel, database, network or file outside the
project folder is touched (nothing is written at all in this module).

## Where things live

```
Module 2/
├─ OrderDesk.slnx                  solution (OrderDesk.Web only — LegacyOrderDesk is in Module 1)
├─ README.md
└─ OrderDesk.Web/
   ├─ OrderDesk.Web.csproj         Microsoft.NET.Sdk.Web · net10.0-windows;net10.0 · Wisej-4 4.1.0
   ├─ Default.html                 browser entry point (<script src="wisej.wx">)
   ├─ Default.json                 "startup": "OrderDesk.Program.Main, OrderDesk" · theme Bootstrap-4
   ├─ Program.cs                   Program.Main(args) → Application.MainPage = new MainPage()   (replaces Application.Run)
   ├─ Startup.cs                   Kestrel host · app.UseWisej() · static files
   ├─ Web.config                   appSettings + connectionStrings (replaces App.config)
   ├─ Properties/launchSettings.json   http://localhost:5102
   ├─ MainPage.cs / .Designer.cs   the lab page: app bar · OrdersPage host · Shell anatomy · Compiler-error log · buttons · trace
   ├─ Pages/
   │  ├─ OrdersPage.cs             LegacyOrderDesk.OrdersForm ported (using Wisej.Web; : UserControl; handlers unchanged)
   │  └─ OrdersPage.Designer.cs    the migrated designer file — every changed line carries a "was:" comment
   ├─ Legacy/
   │  ├─ AppState.cs               ✕ copied statics (CurrentUser/CurrentCustomer/ActiveFilter/CurrentOrder) — Module 4
   │  └─ AppConfig.cs              ✕ ConfigurationManager-style <exe>.config lookup — fails on the server
   ├─ Services/
   │  ├─ AppConfig.cs              ✓ Web.config via System.Xml.Linq (appSettings, connectionStrings, StorageRoot)
   │  ├─ ShellAnatomy.cs           reads Default.html / Default.json / Web.config / package / browser live
   │  └─ CompilerErrorLog.cs       the five error categories with example, fix, count, video bucket
   ├─ Domain/                      shared business logic, unchanged (Order, OrderStore, OrderService, OrderValidator, OrderQuery, User)
   ├─ Shared/                      course props: TracePanel, Palette, Notify
   └─ docs/                        the lab deliverables (below)
```

## Deliverables

1. **The Wisej.NET shell** — project file, package ref, startup files: [`OrderDesk.Web/docs/shell-anatomy.md`](OrderDesk.Web/docs/shell-anatomy.md) (`startup` vs `mainWindow`, what replaces `Application.Run`)
2. **One form moved & building** — namespace swapped, errors categorized: [`OrderDesk.Web/docs/first-form-port.md`](OrderDesk.Web/docs/first-form-port.md) and [`OrderDesk.Web/docs/compiler-error-categories.md`](OrderDesk.Web/docs/compiler-error-categories.md)
3. **Migration log updated** — every accepted workaround captured: [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md)

## Self-check answers

**Knowledge check**

- **What replaces the WinForms `Application.Run` startup pattern?** A Wisej.NET startup method or main
  view configured through the startup files: `Default.json` names either a `startup` method
  (`OrderDesk.Program.Main, OrderDesk` — runs once per session and sets `Application.MainPage`) or a
  `mainWindow` type. `Startup.cs` hosts the process (Kestrel + `app.UseWisej()`); there is no
  `EnableVisualStyles` and no message loop.
- **What is the usual namespace replacement when porting standard controls?** `System.Windows.Forms` →
  `Wisej.Web` (`using Wisej.Web;`, `Wisej.Web.Button`, …). Not `System.Data`, not `System.Drawing`
  (which stays — fonts, colors, points, sizes are still `System.Drawing` types).
- **Why should designer files be changed carefully?** They hold generated control-initialization and
  resource code that is easy to break and hard to diff; one wrong edit in `InitializeComponent()` can
  make the whole form fail to open in the Designer or at runtime. Change them on a branch, one category
  of error at a time, rebuild after each, and let the Designer regenerate what it owns.
- **What should be done with `app.config` connection strings?** Move/adapt them to the web project's
  configuration model — `Web.config` `<connectionStrings>` (or `appsettings.json`), read through one
  helper (`Services/AppConfig`, or `Application.Configuration`). Never hard-code them in forms; never
  leave them in an `App.config` that no longer ships.
- **Main outcome of Module 2?** Convert or recreate the project shell so a migrated form can run in the
  browser — the shell + `OrdersPage` rendering at `http://localhost:5102`.
- **Which activity matches the lab?** Migrate the first form into a Wisej.NET app shell, run it, and
  document each compiler error category (the Compiler-error log card and `compiler-error-categories.md`).
- **Learning objectives (four items):** create a new Wisej.NET project or convert the original project
  file using a controlled branch; understand `Default.html`, `Default.json`, `Program.cs`, startup
  methods and the `mainWindow` option; replace `System.Windows.Forms` with `Wisej.Web` and resolve
  compiler errors systematically; adapt `app.config` connection strings, embedded resources,
  designer-generated files and package references.
- **True/false:** Wisej.NET startup uses browser-facing files and server-side startup configuration
  rather than the executable model — **true**. Standard controls move to `Wisej.Web` but properties and
  methods still need compiler-guided review — **true** (the 14 errors above are exactly that). Connection
  strings and configuration must be adapted to the web configuration — **true**. Preserve business logic
  and workflow before modernizing the UI — **true** (parity first). Static fields are a safe place for
  per-user state in a multi-user web app — **false** (shared by every session; `Application.Session` or
  a per-session service — Module 4). Registry access and local file paths must be replaced with web-safe
  patterns — **true**. The first migration milestone should be a complete UI redesign — **false** (functional
  parity in the browser). A vertical slice should prove startup, navigation, data, modal workflow,
  files/reports and session context — **true**. Server-side Office COM automation is the recommended
  way to generate browser reports — **false** (desktop boundary; managed export/PDF + Download).

**Pause & predict (video)**

- **A form compiles but won't launch — where do you look first?** `Default.json`: the `startup` (or
  `mainWindow`) value must be `Namespace.Type.Method, AssemblyName` / `Namespace.Type, AssemblyName` and
  match the compiled assembly (`AssemblyName` is `OrderDesk`, not `OrderDesk.Web`). Then `Program.Main`
  — does it actually set `Application.MainPage` (or show a form)? Then `Startup.cs` (`app.UseWisej()`
  present, `.json` files not served by the file server) and `Default.html` (the `wisej.wx` script tag).
  The browser console and the network tab (`wisej.wx` returning 200) confirm which layer failed.
- **Where do connection strings move when `App.config` is gone?** Into the web project's configuration:
  `Web.config` `<connectionStrings>` (this sample) or `appsettings.json`, read once by a small helper
  (`Services/AppConfig`, or Wisej's `Application.Configuration`) so forms never see the string, and
  overridable per environment at deployment (Module 7 adds environment-variable overrides).

## Notes for the reviewer

Built with `dotnet build -nologo -v q` for both `net10.0-windows` and `net10.0`: 0 errors, 0 warnings.
Not run here — compile-checked only, please exercise:

- `UserControl.Load` firing when the `OrdersPage` is added to `panelOrdersHost.Controls` (that is what
  calls `ReloadGrid()`); if the grid stays empty, the fallback is to call `ReloadGrid()` directly after
  `Controls.Add` — `OpenOrdersPage()` in `MainPage.cs`.
- `StatusBar.Panels.AddRange` + `StatusBarPanelAutoSize.Spring` + `ShowPanels` rendering the status text;
  `MenuBar.MenuItems` rendering File/Edit/View/Reports/Help with the two sub-items.
- `DataGridView.Rows[0].Selected = true` / `CurrentRow` / `SelectedRows` — the detail panel falls back to
  the first order if no current row is reported, so it always shows Order 1042 after a load.
- `DefaultCellStyle.FormatProvider = en-US` on the Total column (compile-checked); if the column shows the
  server culture instead, the trace lines still print `$4,820.00` via an explicit en-US format.
- `gridErrors.Rows.Add(params object[])` filling the Compiler-error log during the replay.
- `MessageBox.Show(text, caption, MessageBoxButtons, MessageBoxIcon)` from the ported handlers.
- Docking order: the `TracePanel` is added before the app bar so the bar spans the full width (same
  pattern as the template `MainPage`); if the trace ends up beside the bar instead, it is cosmetic.
- `Application.StartupPath` locating `Web.config`: `Services/AppConfig.ResolvePath()` falls back to
  `WEBSITE_PATH`, the current directory and the bin folder, and logs the path it used.
- Layout is sized for 1400×760 (content 806 px wide + trace 560; buttons at y 658–692). The detail
  panel inside the ported form has ~8 px of slack at the bottom.
- The quiz's stored answer key for "usual namespace replacement" marks the `System.Data → Wisej.Data`
  option; the lesson, the video and this app say `System.Windows.Forms → Wisej.Web`, which is what
  the self-check above answers.
