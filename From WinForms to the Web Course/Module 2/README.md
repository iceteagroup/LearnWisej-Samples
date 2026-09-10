# OrderDesk.Web · From WinForms to the Web · Module 2

Local lab build for **Module 2 · Project Conversion and Wisej.NET Startup**. It follows the lesson and the walkthrough
video: a clean **Wisej.NET 4 shell** is created on the migration branch (Path B — the WinForms original
**LegacyOrderDesk** stays in the solution untouched), the first form (`OrdersForm` + its `EditOrderDialog`) is copied
in, `System.Windows.Forms` becomes `Wisej.Web` with one search-and-replace, and the compiler errors are
**categorized, fixed and rebuilt** (15 → 24 → 10 → 0) until the form runs in the browser. App.config's connection
string moves to `Web.config`. Everything the port could *not* fix yet is logged, not faked.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 2/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5602
```

Then open <http://localhost:5602>. (Visual Studio: open `OrderDesk.slnx`, set `OrderDesk.Web` as the startup project, F5.
The solution also contains `LegacyOrderDesk`, the WinForms original — it runs on Windows only and is the "before"
the compiler error log was measured against.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Page load | success | Trace: `• server startup Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()`, `Application.Configuration StartUp = OrderDesk.Program.Main, OrderDesk · MainWindow = (none) · ThemeName = Bootstrap-4 · Url = Default.html`, `Kestrel localhost:5602 · … · StartupPath = …`, `Startup.cs app.UseWisej() — AddWisej() does not exist in Wisej-4 4.1.0 …`, `namespace swap …`, `OrdersForm is still a Form …`, `CompilerErrorLog 39 error lines over 4 passes: 15 → 24 → 10 → 0 · 11 open items`. Status `● shell running · OrdersForm ported · 0 compiler errors` |
| Select a row in **Wisej.NET shell** | success (deliverable 1) | The first lines of the *real* file (from `Application.StartupPath`) appear under the grid; trace `→ .NET→JS ShellAnatomy.Preview Startup.cs · 23 lines · replaces (nothing — the .exe owned the process)` |
| **All / Namespace / Controls / Styling / Desktop op / Deferred** | success (deliverable 2) | The error log filters by category; the count label updates (`10 of 30 rows · 26 error lines · Control substitution`); trace `← JS→.NET errors.filter category = control substitution → 10 rows · 26 error lines`; selecting a row shows the compiler message and the fix underneath |
| **▶ Replay the build funnel** | progress (Timer) | Every ~900 ms the big label steps `15 errors` → `24 errors` → `10 errors` → `0 errors · build succeeded` (green); one `• server build pass n/4 …` trace line per pass; status `● rebuilding — pass 2 of 4` … then green `● build succeeded — runs in the browser, but compiling isn't done`; green banner `✓ Build succeeded — but compiling isn't done: 11 items …` |
| **Legacy: ConfigurationManager** | failure | Red banner `✕ FileNotFoundException: App.config was deployed as 'LegacyOrderDesk.dll.config' next to the desktop .exe; the web host has no such file. …`; the values label shows the path it looked for; trace `⚠ boundary ConfigurationManager looked for …\bin\…\LegacyOrderDesk.dll.config — App.config was the .exe's file; the web host has Web.config in the content root → XDocument`; status red `● App.config is gone — the desktop read fails on the web host` |
| **Web.config** | recovery (deliverable 3) | Green banner `✓ Web.config: OrderDesk = Server=(local);Database=OrderDesk;User Id=orderdesk;Password=•••••` (password masked); trace `• server WebConfig.ConnectionString XDocument.Load(…\Web.config) → connectionStrings/add[@name='OrderDesk'] → …`; status `● connection string read from Web.config` |
| **Open the ported OrdersForm ↗** | success + boundaries | The ported `Form` floats over the page (MenuBar File/View/Reports/Help, grid with 1042 Northwind Traders $4,820.00 Open first, StatusBar); trace `→ .NET→JS OrdersForm.Show() …` followed immediately by `⚠ boundary RegistrySettings.Load HKCU window size not restored …` (from the form's own `Load`). In the form: **Print Invoice**, **Export to Excel**, **Attach file…**, **File › Settings…** each toast a warning and add a `⚠ boundary` line; double-click a row → `EditOrderDialog` (result in a callback, Save shows `MessageBox.Show("Saved.")`); **File › Exit** → `⚠ boundary RegistrySettings.Save …` then `• server OrdersForm.FormClosed closed and disposed …`. A second click while it is open traces `OrdersForm.BringToFront() already open — one instance per session` |
| **Clear** | – | Empties the trace |

The right-hand card is the **migration log · live trace**: every user action (`← JS→.NET`), every server decision
(`• server`), everything pushed to the browser (`→ .NET→JS`) and every desktop boundary hit and replaced (`⚠ boundary`).

## Where things live

```
Module 2/
├─ OrderDesk.slnx                   both projects
├─ LegacyOrderDesk/                 the WinForms original — untouched (the reliable backup, Path B)
│  ├─ Program.cs                    ✕ NOT copied: EnableVisualStyles + LoginForm.ShowDialog + Application.Run (the shell's Program.cs replaces it)
│  ├─ LoginForm (+ Designer)        ✕ NOT copied: a blocking modal before the main window → session sign-in (Module 4)
│  ├─ SettingsForm (+ Designer)     ✕ NOT copied: edits HKCU → per-user settings store (Module 4)
│  ├─ Settings/RegistrySettings.cs  ✕ NOT copied: HKCU is the service account's registry on a server (Module 4)
│  ├─ Reporting/InvoicePrinter.cs   ✕ NOT copied: PrintDocument → local printer (Module 6)
│  ├─ Reporting/ExcelExport.cs      ✕ NOT copied: Excel Interop + C:\Orders\out.xlsx (Module 6)
│  ├─ OrdersForm, EditOrderDialog   ✓ copied (the first form and its dependency)
│  └─ Domain/                       ✓ copied verbatim
└─ OrderDesk.Web/                   the Wisej.NET 4 shell (net10.0-windows;net10.0) — deliverable 1
   ├─ Default.html / Default.json / Program.cs / Startup.cs / Web.config   the six shell files (Migration/ShellAnatomy.cs lists them)
   ├─ OrderDesk.Web.csproj          Microsoft.NET.Sdk.Web + PackageReference Wisej-4 4.1.0
   ├─ OrdersForm.cs / .Designer.cs  ✓ the ported form: still a Form; MenuBar/StatusBar substitutions; desktop calls commented ✕ with BoundaryHit stand-ins
   ├─ Dialogs/EditOrderDialog.cs    ✓ ported unchanged (the CALLER changed: ShowDialog callback)
   ├─ Domain/                       ✓ the reused business logic
   ├─ Legacy/AppState.cs            ✕ copied on purpose (static current user — Module 4)
   ├─ Legacy/AppConfig.cs           ✕ copied on purpose: the ConfigurationManager pattern that throws on the web host
   ├─ Configuration/WebConfig.cs    ✓ XDocument over Web.config (connectionStrings + appSettings, password masking)
   ├─ Migration/ShellAnatomy.cs     deliverable 1 as data (+ Preview of the real files)
   ├─ Migration/CompilerErrorLog.cs deliverable 2 as data (30 rows, 4 passes)
   ├─ Views/TracePanel.cs, Ui.cs
   ├─ MainPage.cs / .Designer.cs    the lab console
   └─ docs/                         the lab deliverables
```

## Deliverables

1. **The Wisej.NET shell** (project file, package ref, startup files) — [`OrderDesk.Web/docs/ShellAnatomy.md`](OrderDesk.Web/docs/ShellAnatomy.md); the files themselves are the project root, listed by [`Migration/ShellAnatomy.cs`](OrderDesk.Web/Migration/ShellAnatomy.cs)
2. **One form moved and building, errors categorized** — [`OrderDesk.Web/docs/CompilerErrorLog.md`](OrderDesk.Web/docs/CompilerErrorLog.md) (the same rows as [`Migration/CompilerErrorLog.cs`](OrderDesk.Web/Migration/CompilerErrorLog.cs)); the form is [`OrdersForm.cs`](OrderDesk.Web/OrdersForm.cs) + [`OrdersForm.Designer.cs`](OrderDesk.Web/OrdersForm.Designer.cs); why it stays a `Form`: [`docs/FormVsPage.md`](OrderDesk.Web/docs/FormVsPage.md)
3. **Migration log updated** — [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md) (Module 1's log carried forward + the Module 2 section: shell, namespace swap, substitutions, App.config → Web.config, ShowDialog minimal fix, deferred items)

## Self-check answers (lab guide + video)

- **Which business logic was reused as-is?** `Domain/` again, verbatim — and now the *form's* logic too: every handler
  in `OrdersForm.cs` (`ReloadGrid` → `OrderService.Search`, `newOrderButton_Click` → `OrderService.Save`, the
  `EditOrderDialog` validation) is the same C# it was on the desktop. The namespace swap touched `using` lines and
  designer types, not behaviour.
- **Which desktop boundary was replaced with a web-safe pattern?** The configuration read: `ConfigurationManager.ConnectionStrings`
  over `App.config` → `XDocument` over `Web.config` in the content root (`Configuration/WebConfig.cs`). And startup
  itself: `Application.Run` → `Default.json` + `Program.Main` + `Startup.cs`. The other boundaries (`RegistrySettings`,
  `InvoicePrinter`, `ExcelExport`, `OpenFileDialog`, `SettingsForm`) are **commented and logged** with stand-ins, not
  replaced — that is Modules 4 and 6.
- **How is per-user state kept out of static fields?** It is not, yet — on purpose. `AppState.CurrentUser / CurrentFilter /
  LastSearch` compile unchanged and are on the open-items list (unclear/deferred → Module 4). The WinForms `LoginForm`
  that filled the static was deliberately not copied, so the port does not pretend the leak is fixed.
- **What was tested before calling the migrated feature complete?** Four builds on both target frameworks (the log
  records each pass); the form opened in the browser with the five orders, the menu, the status bar and the detail
  panel; double-click → edit → Save round-trips through the callback; every commented desktop call surfaces as a
  `⚠ boundary` line instead of silently doing nothing; the connection string reads from `Web.config` with the password
  masked. Compiling was explicitly *not* the finish line.
- **A form compiles but won't launch — where do you look first?** (video, pause & predict) `Default.json`: is `"startup"`
  (or `"mainWindow"`) the right `Type.Method, Assembly` / `Type, Assembly`? Then `Program.Main`: does it set
  `Application.MainPage` (or `Show()` a form)? Then the browser console with `"debug": true`. The console traces
  `Application.Configuration.StartUp` on load for exactly this reason.
- **Where do connection strings move when App.config is gone?** (video, pause & predict) To the web host's configuration:
  `Web.config` `<connectionStrings>` (or `appsettings.json`), read from the content root — not `<exe>.config` next to
  the binary, which is why the legacy button throws `FileNotFoundException`.
- **Common pitfall from the video:** leaving `Application.EnableVisualStyles()` / `Application.Run()` in the migrated
  startup (CS0117 — the log has both rows) and forgetting to move the connection string (the red banner).

## Runtime facts

- `Application.Configuration` (`Wisej.Core.Configuration`) exposes what `Default.json` loaded: `StartUp`, `MainWindow`, `ThemeName`, `Url`.
- There is no `AddWisej()` in Wisej-4 4.1.0; `app.UseWisej()` is the only middleware registration (`Startup.cs`).
- `Application.StartupPath` is the project folder under `dotnet run` / F5 — `Web.config` and the shell files are read from there (`CopyToOutputDirectory = Never`).
- A `Form` shown with `Show()` floats over the page and is disposed by Wisej.NET when it closes; a `Form` shown with `ShowDialog(callback)` is disposed by the caller. `ShowDialog` never blocks.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
