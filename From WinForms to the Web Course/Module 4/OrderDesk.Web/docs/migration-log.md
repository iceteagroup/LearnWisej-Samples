# migration-log.md — LegacyOrderDesk → OrderDesk.Web

One line per accepted decision or workaround. Later modules append to this file; the console's trace panel is the
live version of it.

## Module 1 · Migration discovery (2026-09-09)

- **Assessment first.** 14 items inventoried and classified (4 direct-port · 5 adapt · 3 redesign · 1 defer · 1 remove) before any namespace was replaced. See `MigrationAssessmentWorkbook.md`.
- **Startup → `Default.json` + `Program.Main`.** `Application.EnableVisualStyles` / `Application.Run` have no equivalent; `Program.Main(NameValueCollection)` runs once per browser session and sets `Application.MainPage`. Kestrel (`Startup.cs`, `app.UseWisej()`) owns the process.
- **Business logic copied verbatim.** `Domain/` (Order, Customer, OrderService, CustomerService, InvoiceDocument, SampleData) has no UI or desktop dependency and moved unchanged. Totals are identical on both sides.
- **Static current user is a defect, not a style issue.** `AppState.CurrentUser` is one slot per server process; a second session overwrites it. Kept in `Legacy/` next to `Application.Session.User` so the leak is reproducible; the typed session context is Module 4's job.
- **Print Invoice → server PDF.** `PrintDocument` targets a printer attached to the server. The shared `InvoiceDocument` lines are written by a dependency-free `InvoicePdfWriter` and shown in a `PdfViewer` (+ Download). No printer, no local path.
- **Export to Excel → bytes + `Application.Download`.** Excel Interop is not supported in a server process and `C:\Orders\out.xlsx` is the user's disk. First slice ships CSV built in memory; Module 6 upgrades to a managed .xlsx writer and a report queue.
- **Attach file deferred to Module 6.** `OpenFileDialog` + `C:\Orders\Attachments` cannot exist on the server; the button logs the boundary instead of faking it. Replacement: `Upload` control + configured storage root.
- **Connection string → `Web.config`.** `App.config` is gone; settings live in the web host's configuration (`<appSettings>`, `<connectionStrings>`), read in Module 2.
- **Window-size restore removed.** The browser window belongs to the user; responsive layout (Module 7) replaces it.
- **Installer / ClickOnce deferred.** The web app is deployed once; checklist in Module 7.
- **Modal dialogs do not block in Wisej.NET.** `ShowDialog(callback)` / `ShowDialogAsync()`; the caller disposes the dialog when it closes (Module 3 rule, already applied to the invoice preview).

## Module 2 · Project conversion and Wisej.NET startup

- **The Wisej.NET shell replaces the .exe.** `Default.html` / `Default.json` (`"startup": "OrderDesk.Program.Main, OrderDesk"`) / `Program.cs` / `Startup.cs` (`app.UseWisej()`; there is no `AddWisej()` in Wisej-4 4.1.0) / `Web.config`; the project multi-targets `net10.0-windows;net10.0`.
- **First form ported `System.Windows.Forms → Wisej.Web`.** Same designer shape (`InitializeComponent`, absolute `Location`/`Size`, `Anchor`, event hookups); no `System.Windows.Forms` reference in the web project — desktop files that are needed are copied into `Legacy/`.
- **Compiler errors classified, not fixed blindly.** Each error logged as *rename* (namespace/enum), *no equivalent* (`PrintDocument`, `OpenFileDialog`, `Application.Run`) or *behaviour change* (`ShowDialog` returns immediately), so the fixes land in the right module.
- **`App.config` → `Web.config` `<appSettings>`.** Read with `System.Xml.Linq` in a small `AppConfig` helper; `System.Configuration.ConfigurationManager` is not referenced.

## Module 3 · Forms, navigation, layouts and modal workflow

- **`MenuStrip`/`ToolStrip` → `MenuBar`/`ToolBar` shell** with three screens switched in one page; `StatusStrip` → `StatusBar`.
- **`EditOrderDialog` ported with deterministic disposal.** `ShowDialog((form, result) => { …; form.Dispose(); })` or `await ShowDialogAsync()` in an `async void` handler; the caller disposes; a leak counter proves closed dialogs are collected.
- **`MessageBox` reviewed.** Informational boxes → `Ui.Toast` (`AlertBox`, top-right, auto-close); decisions → `await MessageBox.ShowAsync(...)`; nothing blocks the request thread.
- **Fixed layout vs `Dock`/`Anchor`.** Absolute layouts kept where the desktop had them; the trace and right-hand cards anchored so the page follows the browser size (sized on the server against the first reported browser size).
- **Blocking work → `Application.StartTask`.** Long handlers moved off the request with `Application.StartTask(() => { …; Application.Update(this); })`; `IsDisposed` checked before touching controls.

## Module 4 · Sessions, statics & multi-user safety (2026-09-10)

- **2026-09-10 — Static-state audit before any refactor.** 11 rows: 8 statics found with `grep -rn --include=*.cs --exclude=*.Designer.cs "static" LegacyOrderDesk` (method-only static classes excluded) + 3 HKCU settings: 3 keep static · 5 move to session · 1 profile store · 1 browser storage · 1 remove. Rule: *would the value differ if two users opened the app at the same time?* See `StaticStateAudit.md`; rows compiled in `Migration/StaticStateAudit.cs`.
- **2026-09-10 — Only per-user statics move.** `AppState.Countries`, `SampleData.Customers`, `InMemoryOrderRepository.Shared` (the database stand-in, locked) and the stateless static helpers stay static. Moving them would be churn, not safety.
- **2026-09-10 — Typed session context instead of the quick fix.** `Services/UserSessionContext` (plain data: UserName, DisplayName, Company, CurrentCustomerId, CurrentFilter, LastSearch, Culture, SignedInAt) behind `Services/SessionContext.Current`, which is the only code that knows the value lives in `Application.Session` (dynamic bag, key `UserContext`, created lazily). Callers write `SessionContext.Current.CurrentFilter` where they wrote `AppState.CurrentFilter`. The quick fix (`Application.Session.X` everywhere) was rejected: untyped, scattered, nothing to clear on logout. See `SessionContextService.md`.
- **2026-09-10 — The session stores ids, not shared objects.** `CurrentCustomer` (a `Customer` instance) became `CurrentCustomerId`, resolved through `CustomerService.Find`, so no mutable object is shared through the session.
- **2026-09-10 — `Legacy/AppState.cs` kept ✕ on purpose.** The console's *Legacy statics* mode runs the Orders screen on it so the corruption is reproducible (tab A kelly, tab B sam, *Re-read* in A → `✕ Static slot corrupted …`). Nothing on the migrated path reads it.
- **2026-09-10 — Two-session test is a gate, not a demo.** `Application.Navigate(Application.Url, "_blank")` opens the second session; the isolation banner (`✓ Isolated: UserContext.Current → session … still says …`) must appear before the feature is called done. Steps in `TwoSessionTest.md`.
- **2026-09-10 — Registry settings relocated by decision table.** `GridDensity` → browser `localStorage` via `Application.Eval`/`EvalAsync` (`Services/BrowserPreferences`, device-bound UI preference) with a roaming copy in the profile store; `ExportFolder` → `Services/UserProfileStore` (`App_Data/profiles/<user>.json`, `System.Text.Json`) as a folder name under `App_Data`, never a client path; `WindowWidth/Height` removed. A database table is the next step for anything that must be audited. `Legacy/RegistrySettings.cs` kept ✕: on a server `HKCU` is the service account's hive on the server machine and throws `PlatformNotSupportedException` on Linux. See `SettingsRelocation.md`.
- **2026-09-10 — One cleanup routine for every exit.** `Services/SessionCleanup.Run(reason, sessionId)`: delete `App_Data/tmp/<session>/`, cancel report jobs, roll back open work, release locks, then `SessionContext.Reset()`. Called by *Sign out*, by the simulated timeout, and by `Application.ApplicationExit`. `Application.SessionTimeout` leaves `Handled = false` (the built-in prolong dialog stays) and releases nothing — the user may still continue. See `SessionCleanup.md`.
- **2026-09-10 — Static `Application.*` events are unsubscribed in `Dispose`.** `SessionTimeout` and `ApplicationExit` are subscribed in the page constructor and detached in `MainPage.Designer.cs` → `Dispose` (`DetachApplicationEvents`); otherwise the page is never collected.
- **2026-09-10 — No ASP.NET authentication yet.** The sample keeps its own tiny user model in the session (`Application.UserIdentity` exists); server-side auth and an audit log land in Module 7 on top of this context.
