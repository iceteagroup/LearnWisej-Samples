# Migration log — LegacyOrderDesk → OrderDesk.Web

The running log the course videos keep opening: one dated line per accepted workaround or decision, carried forward
from module to module.

## Module 1 · Migration discovery & the first slice (2026-09-08)

* **Startup** — `Application.EnableVisualStyles()` + `Application.Run(new OrdersForm())` → `Default.json`
  (`"startup": "OrderDesk.Program.Main, OrderDesk"`) → `Program.Main` sets `Application.MainPage`; Kestrel owns the process.
* **Current user** — `static AppState.CurrentUser` is one slot for the whole server → `Application.Session.User`
  (typed context in Module 4).
* **Print Invoice** — `PrintDocument` → local printer → server-generated PDF (`InvoicePdfWriter`) in a `PdfViewer`.
* **Export** — Excel Interop + `C:\Orders\out.xlsx` → in-memory CSV (`CsvExport`) + `Application.Download`; a real
  spreadsheet writer deferred to Module 6.
* **Attach file** — `OpenFileDialog` + `C:\Orders\Attachments` → left out of the first slice, not faked; Upload
  control + storage root deferred to Module 6.
* **Connection string** — `App.config` → `Web.config` `<appSettings>` read with `System.Xml.Linq`
  (no `System.Configuration` reference).

## Module 2 · Project conversion and Wisej.NET startup (2026-09-09)

* **The shell** — `Default.html` / `Default.json` / `Program.cs` / `Startup.cs` / `Web.config` replace `Program.Main`
  + `App.config`; `app.UseWisej()` is the only registration (no `AddWisej()` in Wisej-4 4.1.0).
* **First form ported** — `System.Windows.Forms` → `Wisej.Web`; the compiler errors classified (namespace-only ·
  API-shape · desktop-only) into the error log; nothing rewritten that a namespace swap fixed.
* **`App.config` → `Web.config`** — settings read by the web host; the desktop project keeps building untouched.

## Module 3 · Forms, navigation, layouts and modal workflow (2026-09-09)

* **Navigation** — `MenuStrip`/`StatusStrip` → `MenuBar` / `ToolBar` / `StatusBar` shell with three screens.
* **Modal dialogs do not block** — `EditOrderDialog` ported with `ShowDialog(callback)` / `ShowDialogAsync`; the caller
  disposes the dialog when it closes. `MessageBox` → `Toast` for confirmations; validation stays modal.
* **Layout** — fixed coordinates → `Dock`/`Anchor`.

## Module 4 · Sessions, statics and multi-user safety (2026-09-10)

* **Static-state audit** — every static classified; per-user state moved into `UserSessionContext` behind
  `Application.Session`; the two-session test proves isolation.
* **Registry** — decision: HKCU settings → a per-user server profile + browser `localStorage` for UI prefs.
* **Logout and timeout** — one cleanup routine for sign-out and `ApplicationExit`; per-session temp cleaned up.

## Module 5 · DataGridView, validation and performance (2026-09-10)

* **200,000 orders** — load-all measured once, then replaced by a server-side filter + `VirtualMode` block cache
  (`OrderQuery.Skip/Take`); summary row; `OrderValidator` → `ValidationResult` shown through `ErrorProvider`.

## Module 6 · Files, reports and browser boundaries (2026-09-10)

* **Classified every path** (`docs/FileBoundaryClassification.md`): 9 touches —
  Server storage 3 · Upload 2 · Download 1 · ClientFileSystem 1 · Redesign 2. Nothing is left as "works on my PC".
* **Storage root** — every `C:\Orders\…` literal → `Web.config` `OrderDesk.StorageRoot` (default `App_Data`) resolved
  with `Path.Combine(Application.StartupPath, …)`; `uploads/ exports/ reports/` created on demand; browser file names
  reduced to a sanitized leaf name (`StorageRoot.SafeFileName`: `Path.GetFileName`, reject `..`, replace invalid chars).
* **Import** — `File.ReadAllLines(@"C:\Orders\in.csv")` → `Wisej.Web.Upload` (`.csv`, ≤ 1 MB) → `uploads/` →
  `OrdersCsvImporter` → `CustomerService.FindByName` + `OrderService.Save` (business logic reused unchanged). Two CSV
  layouts accepted (line layout; the Module 1 export layout). Bad rows are skipped with a reason.
* **Export** — CSV upgraded to a real `.xlsx` written by `Reporting/XlsxWriter` (ZipArchive + five OpenXML parts,
  inline strings + numeric cells) into `exports/`, then `Application.Download(path, "Orders.xlsx")` — the lesson's
  `reportService.CreateOrdersWorkbook(rows)` shape. No NuGet package because the samples take none; in a real project
  EPPlus / Open XML SDK / Aspose / Syncfusion / DevExpress replace the class one-for-one.
* **Office Automation removed from the migrated path** — the desktop `ExcelExport` is not carried over (KB 257757).
* **Print** — `InvoicePrinter` cannot compile for `net10.0` (System.Drawing.Printing) and is not carried over; the web
  path is `ReportService.CreateInvoicePdf` → `InvoicePreviewForm` (PdfViewer, disposed by the caller in the
  `ShowDialog` callback) with a Download button.
* **Report queue** — `Reporting/ReportQueue` (static, one lock, one `Task.Run` worker, copies out, cancel token in) with
  states Queued / Running / Done / Failed / Cancelled and progress 0–100; results written to `reports/<id>-<name>.pdf`;
  pages poll with a 1 s `Wisej.Web.Timer` and redraw only when `Signature()` changes. Owner is the session's user, so a
  second session shows the same jobs with different owners. Production additions listed in `docs/ReportQueue.md`.
* **Long PDFs** — `InvoicePdfWriter` gained `WritePages` (explicit page breaks) and a linear-time xref layout; the
  Module 1 `Write(lines)` API is unchanged.
* **Per-session user without a login screen** — the first session of the process acts as *kelly*, later ones as *sam*
  (`Application.Session.User`), only so the queue's Owner column can show two users; Module 4's typed session context
  is the real answer.
* **Server-like test plan** — `docs/ServerLikeTest.md`: no-Office pass, non-admin service account, Linux container;
  both target frameworks build with 0 warnings / 0 errors on this machine; the Linux run was not executed here.

## Module 7 · Modernize, secure, deploy (2026-09-10)

* **Modernize after parity, never before** — the Orders screen keeps `Domain/` byte-for-byte (New Order still =
  1,280.00); only the shell changed: `ToolBar` tool buttons for the four push buttons, `Watermark` instead of a
  "Search:" label, `Ui.Toast` instead of `MessageBox`. `docs/ModernizationNotes.md`.
* **Theme + mixin** — `Default.json` theme `Bootstrap-4`; `Themes/orderdesk.mixin.theme` (button + toolbar-button
  radius 14) merged at startup. `Application.LoadTheme` would restyle **every session in the process** — a deployment
  decision, not a user preference, so there is no per-user theme switch.
* **Responsive** — `ClientProfiles.json` (Phone ≤600 · Tablet 601–1024 · Desktop ≥1025, copied next to the assembly,
  never served); `Services/ResponsiveLayout.cs` maps profile → one of three layouts in code (desktop side-by-side;
  tablet detail below, short toolbar text; phone detail + toolbar hidden, actions in a `ComboBox`);
  `Application.ResponsiveProfileChanged` subscribed in `Load`, unsubscribed in `Dispose`. Only what
  `docs/ResponsiveProfiles.md` lists is claimed — no phone device was used.
* **Security as a web app** — `AuthService.SignIn` verifies salted SHA-256 hashes (constant-time compare) and stores
  `UserSessionContext` in `Application.Session`; `Demand(permission)` on every server action (Export needs
  `orders.export`; sam the Clerk is denied although the button is clickable). Export goes through `DownloadGuard`:
  permission → resolve under the storage root (rooted paths and `..` refused) → exists →
  `Application.Download(resolvedPath)`. `docs/SecurityReview.md`.
* **AllowHtml policy** — `Label.Text` is encoded by default (`AllowHtml = false`); `AllowHtml = true` only for trusted
  markup or output of `HtmlSanitizer.Sanitize` (whitelist `b`, `i`, `br`, no attributes) — the order-note preview.
  The biggest injection risk in a migrated app is a property flip on a control that used to be inert.
* **Audit log** — `AuditLog` (process-wide, append-only, locked, `Version` counter): sign-in/out, authorize denials
  (recorded inside `Authorize`, so no caller can forget), export, download, print; mirrored to
  `App_Data/logs/orderdesk-yyyyMMdd.log` by `AppLog` (+ `System.Diagnostics.Trace`). A 1 s `Wisej.Web.Timer`
  redraws the dashboard only when `Version` or `SessionCount` changed — the second session's actions appear in the
  first tab without any push from it.
* **Operations dashboard** — KPIs from the orders (Open, Revenue today, Invoiced, On-time % = not on hold),
  `Wisej.Web.Canvas` bar chart drawn in the `Redraw` handler (and after data changes), activity feed from the audit log.
* **Health** — `/health` (`Startup.cs` `MapGet`, no Wisej session; `HealthCheck.Json()`: status, version, uptime,
  storage root writable, log folder, `ClientProfiles.json`, mixin, `ok`). The lesson's readiness checklist is answered
  in `docs/ReadinessChecklist.md`.
* **Deployment assets** — `deploy/iis/web.config` (ASP.NET Core Module in-process, `dotnet .\OrderDesk.dll`, stdout
  log, env vars, merged `<appSettings>` — replaces the project `Web.config` in the publish folder),
  `deploy/docker/Dockerfile` (sdk:10.0 publish `-f net10.0` → aspnet:10.0, `EXPOSE 8080`, `VOLUME /app/App_Data`,
  `HEALTHCHECK curl /health`), `deploy/docker/docker-compose.yml` (5607:8080, data volume, healthcheck),
  `deploy/appsettings.Production.notes.md` (storage root, mixin, session timeout, HTTPS at the proxy, **session
  affinity** for more than one instance, secrets out of `Web.config`). Not executed here — written to be run.
* **Capstone report** — `docs/CapstoneReport.md`: reused · adapted · modernized · risks that remain (no database,
  demo accounts, CSV vs .xlsx, process-wide theme, narrow responsive claims, untested affinity, in-memory audit) ·
  next slice (Edit Order dialog over a real repository with validation, delete permission and the .xlsx export).
* **Build** — both target frameworks (`net10.0`, `net10.0-windows`) build with 0 errors. `Diagnostics.HealthCheck`
  clashes with `Wisej.Core.HealthCheck` in files that import both namespaces; `Startup.cs` uses the full name.
