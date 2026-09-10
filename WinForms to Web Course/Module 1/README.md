# OrderDesk · From WinForms to the Web · Module 1

Local lab build for **Module 1 · Migration discovery & the first slice**. It follows the walkthrough
video (`wisej-wf-migration-discovery`) and the Lab 1 guide: meet **LegacyOrderDesk** (the single-user
WinForms order desk, included as-is), build the **assessment workbook** for it, classify every screen
(direct-port · adapt · redesign · defer · remove), and run a **first vertical slice** in the browser that
crosses a real boundary — the same `OrderService.GetAll()` and the same CSV bytes, but Excel Interop and
`C:\Orders` fail on the server and `Application.Download` takes their place. A second browser tab proves
the static `AppState.CurrentUser` cannot be shared.

The web app is "the assessment workbook, alive": the left card is the workbook grid, under it the
first-slice checklist that lights up as the buttons run and the read-only orders grid; the right card is
the migration trace. Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/WinForms to Web Course/Module 1/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5101
```

Then open <http://localhost:5101>. (Visual Studio: open `OrderDesk.slnx`, press F5 — the solution also
contains `LegacyOrderDesk`, the WinForms "before" app; `dotnet run` from its folder starts it on Windows.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. Both projects build
with `dotnet build -nologo -v q` (web: `net10.0-windows` and `net10.0`; legacy: `net10.0-windows`).

## What to try

The page fits a 1400×760 pane: blue app bar, the **Assessment workbook** card (13 rows, risk tags coloured
as in the video), the **First vertical slice** card (seven `○` items), the **Orders · read-only** card,
the banner, five buttons, and the **Server ⇄ Client · migration trace** on the right. On load the trace
shows `• server  Program.Main  Application.MainPage = new MainPage()  · session <id>`,
`• server  Application.SessionCount  1 active session(s) in this process`, a `★ log  runtime shift` line and
`• server  AssessmentWorkbook  13 items · direct-port 2 · adapt 7 · redesign 2 · defer 1 · remove 1`.

| Action | Path | What you should see |
|---|---|---|
| **Run first slice ✓** | success | `← JS→.NET click  Run first slice ✓` · `• server  Startup.cs  Kestrel owns the process · app.UseWisej() · Default.json startup = OrderDesk.Program.Main` · `• server  Program.Main  runs once per SESSION (not per process) …` · `• server  OrderService.GetAll()  the same call OrdersForm.ReloadGrid() made — Domain/OrderService.cs reused as-is` · `• server  OrderService.GetAll()  5000 orders in the store · the slice shows the first 5 (newest first)` · `→ .NET→JS  gridOrders.Rows  5 rows → browser: 1042 Northwind Traders $4,820.00 Open · 1041 Contoso Ltd $1,290.50 Shipped · 1040 Fabrikam Inc $760.00 Open · 1039 Adventure Works $12,400.00 Invoiced · 1038 Globex Corp $3,090.00 Open` · `★ log  grid-volume  5000 rows would ALL travel to the browser the desktop way …` · `✓ ok  first slice  startup ✓ · read-only screen ✓ · grid ✓ — parity first, modernization second, deployment third`. The Orders card shows the five orders (status coloured); the slice card ticks **Startup**, **One read-only screen**, **One grid**; green banner "First slice running: Program.Main → OrderService.GetAll() → 5 orders in a read-only grid. Parity first." |
| **Second session** | finding | `• server  Application.SessionCount  N active session(s) · this session <id>` · `• server  Legacy/AppState.CurrentUser  null — nobody has signed in yet in this PROCESS` (first click) · `• server  AppState.CurrentUser = kelly  the LegacyOrderDesk Program.Main line, copied as-is (static field)` · `• server  read back  AppState.CurrentUser → "kelly" (Acme) · the same static instance for every session in this process` · `★ log  static-state  open a second browser tab: its Load trace shows kelly already signed in …`. Slice item **User context / login** turns amber; amber banner "AppState.CurrentUser = "kelly" is a static field. Open a second tab …"; status `● alarm`. **Now open a second tab** of <http://localhost:5101>: its load trace shows `✖ fail  Legacy/AppState.CurrentUser  already "kelly" in this NEW session — set by session <id> (ANOTHER tab)` and the amber banner, without anyone signing in there. Clicking the button again in either tab logs `✖ fail … is already there — set by session <id> (this tab / ANOTHER tab)`. |
| **Export (desktop way) ✕** | failure | `• server  ExcelExport.ExportToExcel  new Excel.Application() — a COM server on the machine running the code: the SERVER` · `✖ fail  COMException 0x80040154  Retrieving the COM class factory for component with CLSID {00024500-…} failed (Excel.Application). Office Automation needs Excel installed in an interactive user session.` · `★ log  office  Office Automation is unsupported on a server — verdict redesign …` · `• server  LocalExport.WriteCsv  the desktop fallback: Directory.CreateDirectory(C:\Orders) + File.WriteAllText(out.csv)` · `✖ fail  would write C:\Orders\out.csv on the SERVER  would write C:\Orders\out.csv on the SERVER (machine <name>, account <user>) — the desktop code meant the user's PC …` · `★ log  file-system  C:\Orders is the server's disk …`. Slice item **One deployment boundary** turns red `✖ … Excel Interop + C:\Orders hit the SERVER`; red banner "Desktop export failed on the server: Excel Interop needs an interactive desktop, and C:\Orders\out.csv would land on the server's disk, not the user's PC."; status `● alarm`. **Nothing is written anywhere** — the copied `Legacy/LocalExport.WriteCsv` computes the path and throws. If the slice was not run yet the five orders are loaded first (`• server  OrderService.GetAll()  slice not run yet …`). |
| **Export (Download) ✓** | recovery | `• server  LocalExport.ToCsv  5 rows · 2xx bytes — the pure formatting half of LocalExport, reused unchanged` · `→ .NET→JS  Application.Download  orders.csv (2xx bytes) → browser download · nothing written on the server` · `✓ ok  browser boundary crossed  file/report path replaced: C:\Orders\out.csv → Application.Download(stream, "orders.csv")`. The browser downloads `orders.csv` (header `Order,Customer,Owner,Total,Status,Date` + 5 rows); slice items **One file / report** and **One deployment boundary** turn green; green banner; a top-right info alert "orders.csv sent to the browser."; status `● idle`. |
| **Replay assessment** | progress | `• server  Wisej.Web.Timer  Replay · 450 ms per item · 13 items to tag`; the workbook grid empties and refills one row every 450 ms, each with `★ log  <risk-tag>  <feature> → <verdict> (<effort>) · <finding>` (e.g. `★ log  static-state  AppState.CurrentUser / … → adapt (S) · one process = one user on the desktop; one process = EVERY session on the server …`); status `● working` during, then `✓ ok  assessment complete  13 items · direct-port 2 · adapt 7 · redesign 2 · defer 1 · remove 1` and a blue banner. Clicking again while running restarts it. |
| Clear | – | empties the trace |

Every button is idempotent: run them in any order, as often as you like.

## Where things live

```
Module 1/
├─ OrderDesk.slnx                 both projects (F5 runs OrderDesk.Web)
├─ LegacyOrderDesk/               the WinForms "before" app, unchanged from _template (Windows only)
│  ├─ Program.cs                  Application.Run + the static AppState.CurrentUser = login.User
│  ├─ LoginForm.cs · OrdersForm.cs / .Designer.cs · EditOrderDialog.cs
│  ├─ Legacy/                     AppState · UserPreferences (HKCU) · LocalExport (C:\Orders) · ExcelExport (Interop stub) · InvoicePrinter
│  ├─ App.config                  ExportFolder=C:\Orders · ReportPrinter · LAN-SQL01
│  └─ LegacyOrderDesk.csproj      links ..\OrderDesk.Web\Domain\*.cs — the business logic is literally shared
└─ OrderDesk.Web/                 the migrated app (this module's screen)
   ├─ Program.cs · Startup.cs · Default.html · Default.json · Web.config
   ├─ MainPage.cs / MainPage.Designer.cs   the assessment workbook, alive (designer-style InitializeComponent)
   ├─ Migration/AssessmentWorkbook.cs      the 13-row inventory as C# (+ tag/verdict colours)
   ├─ Legacy/                              copied for the demos, namespace OrderDesk.Legacy, ✕ comments kept:
   │  ├─ AppState.cs                       the statics (+ CurrentUserSetBy lab prop)
   │  ├─ LocalExport.cs                    WriteCsv computes C:\Orders\out.csv and throws; ToCsv reused
   │  └─ ExcelExport.cs                    the Interop-shaped stub → COMException 0x80040154
   ├─ Domain/                              Order · OrderStore · OrderService · OrderValidator · OrderQuery · User (reused as-is)
   ├─ Shared/                              TracePanel · Palette · Notify (course lab props)
   └─ docs/                                the Lab 1 deliverables (below)
```

## Deliverables

1. **Migration assessment workbook** — [`OrderDesk.Web/docs/assessment-workbook.md`](OrderDesk.Web/docs/assessment-workbook.md) (the 13-row inventory, findings, the lesson's assessment template answered, acceptance criteria); live in [`OrderDesk.Web/Migration/AssessmentWorkbook.cs`](OrderDesk.Web/Migration/AssessmentWorkbook.cs)
2. **Classification per screen** — [`OrderDesk.Web/docs/screen-classification.md`](OrderDesk.Web/docs/screen-classification.md)
3. **The chosen first slice** — [`OrderDesk.Web/docs/first-slice.md`](OrderDesk.Web/docs/first-slice.md) (what it touches, acceptance criteria: parity first, modernization second, deployment third)
4. **Migration log** — [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md) (the first entries; every later module appends)

## Self-check answers

**What is the safest first milestone in a WinForms-to-Wisej.NET migration?** A small vertical slice that
proves startup, navigation, data, a modal workflow, files/reports and session context — not modernizing
every form, not rewriting the business logic as REST first, not converting all reports. This sample *is*
that slice: startup → `OrderService.GetAll()` → a grid → a download, plus the static-user finding.

**Which item is most likely to require adaptation rather than direct porting?** Code that writes to
HKCU registry keys — `UserPreferences` here. On the server it reads the server's registry under the
service account: wrong machine, wrong user. (The published quiz key marks "a service method that
calculates order totals" as the answer; in this course's own terms that method — `OrderCalculator` — is
the *direct-port / reuse as-is* example, and the registry code is the adaptation. The workbook tags them
`business-logic → direct-port` and `user-settings → adapt`.)

**Why should a migration backlog classify screens by risk?** To decide what can be direct-ported,
adapted, redesigned, deferred or removed — so the expensive surprises (Excel Interop, a static user
object) surface first instead of after all the easy forms are converted.

**Which project goal should come first?** Functional parity in the browser. Modernization (the Module 7
dashboard) second, deployment third. (The published quiz key says "a completely new UI design"; the lesson
text and the video say parity first — and this sample's acceptance criteria follow the lesson.)

**What is the main outcome of Module 1?** Assess a WinForms application and produce a realistic
migration backlog *before* changing code — the workbook, the classification, the chosen slice.

**Which activity best matches the hands-on lab?** Create a migration assessment workbook for
LegacyOrderDesk and rank its screens by effort and risk (`docs/assessment-workbook.md`).

**Learning objectives (four questions):** inventory forms, controls, third-party components, resources,
data access, reports, Office automation, registry use, file-system use and static state (the 13 rows);
classify each screen as direct-port, port-with-adaptation, redesign or defer (`screen-classification.md`);
choose a vertical slice that proves startup, login, navigation, data, modal workflow, files and reporting
early (`first-slice.md`); document the non-negotiable acceptance criteria — parity first, modernization
second, deployment third.

**True/false:** begin with an inventory and risk-based backlog, not random namespace replacement —
true. The first slice should prove startup, navigation, data, modal workflow, file/report behaviour and
session context — true. Decisions are direct port / adapt / redesign / defer / remove — true. Preserve
business logic and workflow before modernizing the UI — true. Standard controls move from
`System.Windows.Forms` to `Wisej.Web` — true (properties still need a compiler-guided review, Module 2).
Static fields are a safe place for per-user state — **false**: static state is shared across all sessions
(press **Second session**, open a second tab). Registry access and local file paths must be replaced with
web-safe patterns — true (press **Export (desktop way) ✕**, then **Export (Download) ✓**). The first
milestone should be a complete UI redesign — **false**: functional parity. A vertical slice should prove
startup, navigation, data, modal workflow, files/reports and session context — true. Server-side Office COM
is the recommended way to generate browser reports — **false**: it is a desktop boundary
(`COMException 0x80040154` in the trace); use a managed writer + download or a server PDF.

**Pause & predict (video).** *What would break if two users ran this workflow at the same time?* The
static `AppState.CurrentUser` / `CurrentCustomer` / `CurrentOrder`: the second sign-in overwrites the
first for everyone, `SelectionChanged` in one tab changes the "current order" of the other, the CSV goes
to one `C:\Orders\out.csv` on the server that neither user can see, and two Excel Interop calls compete for
a COM server that is not there. *Which code is business logic, and which is desktop plumbing?*
`OrderService`, `OrderCalculator`, `OrderValidator`, `OrderStore` and `LocalExport.ToCsv` are business
logic (no UI, no machine assumption — reused as-is). `AppState` statics, `UserPreferences` (HKCU),
`LocalExport.WriteCsv` (C:\Orders), `ExcelExport` (COM), `InvoicePrinter` (PrintDocument),
`Application.Run`/`Form.Close` and `App.config` are desktop plumbing — each is a row in the workbook.

## Notes for the reviewer

- Built to 0 errors / 0 warnings for `net10.0-windows` and `net10.0` (web) and `net10.0-windows`
  (LegacyOrderDesk); **not run** in a browser by the author. Compile-checked, not yet exercised:
  unbound `DataGridView.Rows.Add(params object[])` with per-cell `Cells[i].Style.ForeColor/Font` and
  `Cells[i].ToolTipText`; `Application.Download(Stream, "orders.csv")`; `gridWorkbook.Rows.Clear()`
  during the Timer replay; the `Visible = false → true` switch of `gridOrders`.
- Dock order mirrors the `_template` MainPage (trace added before the top app bar) so the app bar spans
  the full width and the trace sits under it on the right.
- The `COMException` path relies on `ORDERDESK_HAS_EXCEL` **not** being set (it never is); the
  `LocalExport.WriteCsv` copy never touches the disk. `Legacy/UserPreferences` and `Legacy/InvoicePrinter`
  were not copied into the web project — Module 1 only demonstrates the static, the file path and Interop.
- The two-tab demo works within one process only (same `dotnet run`); after a restart the static is
  empty again and the first **Second session** click logs `null — nobody has signed in yet in this PROCESS`.
- Money in the trace is formatted with `en-US` (`$4,820.00`); the grid's `C2` column uses the session
  culture, so a non-US browser may show a different currency symbol in the grid only.
