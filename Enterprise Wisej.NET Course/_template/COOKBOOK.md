# EnterpriseOps cookbook — Enterprise Wisej.NET: Architecture to Cloud (Wisej-4 4.1.0, .NET 10)

The conventions every module sample of the advanced course follows. Facts marked **(verified)** were executed in
the browser while building the Application Integration and Foundations course samples (same framework build).
Facts marked **(unverified)** come from the Wisej.NET XML docs / reflection over `Wisej.Framework.dll`: implement
them, make sure `dotnet build` passes, and say so in your report so the reviewer checks them at runtime.

## Project layout (copy `_template`)

```
Module N/
  EnterpriseOps.slnx                   (from _template)
  .gitignore                           (from _template)
  README.md                            (what it shows, how to run, what to click, lab-step → code map, self-check)
  EnterpriseOps/
    EnterpriseOps.csproj               (from _template; TargetFrameworks = net10.0-windows;net10.0 — keep it exactly)
    Program.cs  Startup.cs  Default.html  Default.json  Web.config
    Properties/launchSettings.json     (port: Module N → http://localhost:520N, i.e. 5200 + N; Module 14 → 5214)
    UI/            the screens the course names (CommandCenterDashboard, WorkQueuePage, ImportCenterPage,
                   EscalationWizard, WorkOrderHistoryPage, DiagnosticsPage, AuditLogPage, ReleaseDashboardPage,
                   FieldTechnicianPage …) — a Page (or Form) + its .Designer.cs, plus dialogs (ConflictDialog …)
    Domain/        entities, value objects, enums (WorkOrder, Tenant, WorkOrderStatus …)
    Services/      application services, commands, results, workflows, jobs (WorkOrderService, SessionContext …)
    Data/          repositories / DbContext / fake stores
    Security/      identity, permissions, audit
    Integrations/  Diagnostics/  Interop/  Controls/  Widgets/  Hybrid/   only when the module needs them
    Architecture/  ADRs as Markdown when the module writes one (Module 1)
    docs/          the lab deliverables as Markdown (+ SVG diagrams when the lab asks for one)
```

- One web project per module, **folder-per-layer**; namespaces follow the folders: `EnterpriseOps.UI`,
  `EnterpriseOps.Domain`, `EnterpriseOps.Services`, `EnterpriseOps.Data`, `EnterpriseOps.Security`,
  `EnterpriseOps.Integrations`, `EnterpriseOps.Diagnostics`, `EnterpriseOps.Interop`, `EnterpriseOps.Controls`,
  `EnterpriseOps.Hybrid`. That is the "projects or folders" baseline Module 1 asks for; ADR-001 records why the sample
  keeps one project (runnable with one `dotnet run`) while the folders map 1:1 onto the multi-project split.
- `Program.Main(NameValueCollection args)` is the session entry point (`Default.json` → `"startup"`). Use
  `Application.MainPage = new UI.<Screen>();` for a Page, `new UI.<Screen>().Show();` for a Form.
- Run: `dotnet run -f net10.0 --urls http://localhost:520N` from the project folder. The static file server serves
  the **project folder**, so a Widget package `Source = "Widgets/statusChart.js"` is fetched as `/Widgets/statusChart.js`.
  `Default.json` / `Web.config` are never served.
- Build with `dotnet build -nologo -v q` and fix every error. Warning CS7022 (Program.Main ignored) is expected and
  already silenced in the csproj. **Do not run the app yourself and do not start servers**; the reviewer runs it.
- NuGet: `Wisej-4` 4.1.0 is in the local cache. Module 4 may add `Microsoft.EntityFrameworkCore.Sqlite` **10.0.12**
  (an in-memory SQLite connection kept open for the session) — if `dotnet restore` cannot fetch it, fall back to a
  hand-written repository behind the same interface and say so in the report. Every other module uses in-memory
  fakes in `Data/` (no database, no network, no cloud account, nothing deployed).

## The EnterpriseOps domain (shared by all 14 modules — reuse these names)

Work orders for a multi-tenant field-service company. Seed ~40–200 in-memory rows so grids, filters and batches
look real. Keep the same vocabulary across modules:

```csharp
namespace EnterpriseOps.Domain
{
    public enum WorkOrderStatus { New, Assigned, InProgress, OnHold, Escalated, Completed, Cancelled }
    public enum Priority { Low, Normal, High, Critical }
    public class Tenant   { public string Id; public string Name; }            // "contoso", "fabrikam", "northwind"
    public class WorkOrder
    {
        public int Id; public string TenantId; public string Title; public string Customer; public string Site;
        public WorkOrderStatus Status; public Priority Priority; public string AssignedTo;
        public DateTime CreatedUtc; public DateTime? DueUtc; public int Version;   // Version = optimistic concurrency token
    }
}
```

Services expose **typed commands and results**, never raw entities to the UI:
`SaveWorkOrderCommand`, `ApproveWorkOrderCommand`, `CommandResult { Succeeded, Errors, CorrelationId }`,
`PagedResult<T> { Rows, Total, Page, PageSize }`, `WorkQueueQuery`, `WorkQueueRow` (projection), `CommandContext`
(tenant + user + correlation id), `SessionContext` (per-session, never static). Users: `ana.ops` (Manager),
`ben.tech` (Technician), `cara.admin` (Admin). Fail visibly on purpose: every module has at least one **failure path**
button (validation rejected, stale version, permission denied, simulated exception, simulated slow query…) and a
**recovery** path.

## Verified runtime facts (same framework build)

- `Wisej.Web.Timer(components)` with `Interval`, `Tick`, `Start()/Stop()`, `Enabled` — a Component with no visual surface.
- `Application.StartTask(() => { …change controls…; Application.Update(this); })` runs work on a background thread;
  `Application.Update(component)` pushes pending changes over WebSocket from a non-request thread. Bound the frequency
  (≥ 250 ms between pushes); check `IsDisposed`; catch `ObjectDisposedException`. Use it for progress observers,
  background jobs (Module 6), live diagnostics (Module 11) and sync simulations (Module 13).
- `Application.Session` is a dynamic per-user bag (`Application.Session.Context = sessionContext`); `Application.SessionId`;
  `Application.UserAgent`, `Application.Browser` (name/version/device) for device-aware layout (Module 13) **(unverified)**.
- `AlertBox.Show(text, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000)`
  — always TopRight so toasts do not cover the buttons.
- Dialogs — no blocking `ShowDialog()`; in an `async void` handler: `DialogResult r = await dialog.ShowDialogAsync();`
  or `MessageBox.ShowAsync(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question)`. Inside the dialog:
  `this.DialogResult = DialogResult.OK; this.Close();`.
- `async void` handlers with `await Task.Delay(…)` are supported; after an `await` push UI changes with
  `Application.Update(this)`; keep a `CancellationTokenSource` in an **instance** field; `try/catch/finally` and reset
  button state in `finally`.
- Fonts: `new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold)`, monospace `new System.Drawing.Font("monospace", 9F)`.
  `Label.TextAlign` uses `System.Drawing.ContentAlignment`. `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`.
- `Application.LoadTheme("Material-3")` restyles the running app live (built-in: Bootstrap-4, Material-3, FluentDark-5, …).
- DataGridView: `AutoGenerateColumns = false`, `Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName, HeaderText, Name, Width })`,
  `DataSource = new BindingSource { DataSource = rows }` (or a `List<T>`), `SelectionMode = FullRowSelect`, `MultiSelect`,
  `ReadOnly`, `SelectedRows`, `CurrentRow`, `dgv.Sort(column, ListSortDirection)` / `SortCompare` **(unverified)**; for
  server-side paging bind **one page at a time** and keep page/sort/filter state in a `GridState` object on the server.
- Wizards: a `Form`/`Page` with a `TabControl` (or `Panel` per step) driven by a workflow service; `StepProgressBar`
  is not needed — a `Label` "Step 2 of 5" is enough.
- `Wisej.Web.Widget` (Module 8 chart widget): `Packages` (List<Widget.Package>{Name, Source}), `InitScript` (embedded
  resource string via `GetResourceString`), `Options` (anonymous object — names are **camel-cased** on the client),
  `WidgetEvent += (s, e) => …` with `e.Type` / `dynamic e.Data`, `widget.Update()` after changing Options runs
  `this.update(options, old)` in the browser, `widget.Call("fn", args)` runs `this.fn(args)` on the wrapper.
  Vendor object in `this.widget`; DOM in a child element of `this.container`; wire events through
  `this._addListener(name, handler)` / `_getEventData(type, e)` (a `fireWidgetEvent` raised synchronously *during*
  `update()` is dropped). Never define `getWidth`, `getHeight`, `destroy`, `resize`, `show`, `hide`, `getValue` on the wrapper.
  Write the "vendor" chart as a small self-contained IIFE drawing SVG — never load from a CDN.
- Custom control (Module 8 StatusTimeline): a `UserControl` composed of standard controls is enough for the lab; a
  qooxdoo-class control (`Platform/*.js` + `[assembly: WisejResources]` + `OnWebRender(config)` + `OnWebEvent`) is the
  optional advanced variant — see the Application Integration Course `Module 5` for the verified pattern.
- Client-side JavaScript (Module 9): `Application.Eval("…")` (one-way), `await Application.EvalAsync("expr")` (an
  **expression**, never `return …;`), `this.Call("fn")` on a control, `[WebMethod]` public instance methods on a
  top-level Page/Form are callable from JS as `App.MainPage.Name(args, cb)` / `App.MainPage.NameAsync(args)` (return
  values are NOT camel-cased; never pass `null` args). Enforce permissions **server-side** inside the WebMethod.
  The verified way to ship page-level JavaScript (command palette, browser capability collector) is a
  `Wisej.Web.Widget` whose `InitScript` registers `window.EnterpriseOps = { collect: …, … }` and fires
  `fireWidgetEvent`/`_addListener` events back; `Application.Browser` (name, version, device, screen size) and
  `Application.ClientTimeZone` give the server-side view **(unverified)**.
- Health / deployment (Module 12): add a minimal probe endpoint in `Startup.cs` — `app.MapGet("/healthz", …)` after
  `builder.Build()` and before `app.Run()`; Wisej owns only its own `*.wx` paths, so the endpoint coexists
  **(unverified)**. Read environment config from `Default.json` (`Application.Configuration`) and from
  `appsettings.{Environment}.json` via `builder.Configuration`.

## UI conventions used by every sample (so the samples feel like one course)

- The main screen is a `Page` (course screens are Pages) sized for 1348×680, light grey background
  `Color.FromArgb(238,242,247)`, white cards (`Panel`, `BorderStyle.Solid`), card titles `"default" 12F Bold`,
  monospace 9F for logs/code. A slim header bar with the screen name, the tenant, the user and a **correlation id**.
- Right-hand card **"Server · live activity trace"** (`lstTrace`, a `ListBox` with monospace font): every user action,
  every service decision, every failure and every recovery logged as `HH:mm:ss.fff  message` through one
  `Trace(string)` helper (select the last item after adding). Show the layer in the line: `UI →`, `Service:`,
  `Data:`, `Security:`, `Job:`. This is how the reviewer proves the handler is thin and the service decided.
- A status label (`lblStatus`) that changes text and colour: green `Color.FromArgb(31,157,87)` ok, amber
  `Color.FromArgb(232,161,60)` warning, red `Color.FromArgb(224,86,59)` error; plus a banner label for failures.
- Bottom bar of buttons that exercise the **success path**, a **progress path** where the module has one (Timer /
  async job / background sync), at least one **failure path** and the **recovery**, plus **Clear trace**.
- Control names exactly as the lesson/lab/walkthrough use them (`dgvWorkQueue`, `btnApprove`, `btnBatchReassign`,
  `lstTrace`, `lblStatus`, `cboTenant`, `btnRun`, `txtSearch`, `pnlNotifications` …). Never `button1`.
- Designer-style `<Name>.Designer.cs` with `InitializeComponent()` so the file opens in the Wisej Designer; code-behind
  in `<Name>.cs`. **Thin, readable event handlers that call services** — the handler shape the site's lab code checker
  expects: `private async void btnRun_Click(object sender, EventArgs e) { try { var result = await _service.RunAsync(ctx); ShowResult(result); } catch (Exception ex) { _log.Error(ex); AlertBox.Show(…); } }`.
- Services are plain classes created in the page constructor (or a tiny `ServiceRegistry` static factory that returns
  **per-session** instances — never keep user/tenant state in statics; Module 3's static-state audit says why).
- Toasts: `AlertBox.Show(text, icon, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000)`.

## Docs (`docs/`)

Write each lab deliverable as its own Markdown file named after what the lab asks for (`ADR-001-SolutionStructure.md`,
`CodingStandards.md`, `CodeReviewChecklist.md`, `MigrationInventory.md`, `RiskMatrix.md`, `PermissionMatrix.md`,
`ErrorMappingTable.md`, `PerformanceBudget.md`, `ReleaseRunbook.md`, `HealthCheck.json` …). Include a short **Evidence**
section describing what the running app shows for each path. The README carries a **Lab steps → where in the code**
table mapping every lab-guide step / deliverable to a file/method, a **What to click** table (button → path → what
you should see), the **Student review questions** and **Instructor acceptance criteria** from the lab guide answered
against the sample, and a **Verified / unverified** section listing anything from this cookbook you used that is
marked unverified.
