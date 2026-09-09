# Wisej.NET Foundations cookbook (Wisej-4 4.1.0, .NET 10)

The conventions every Foundations module sample follows. Facts marked **(verified)** were executed in the
browser while building these samples (or the Application Integration Course samples, which use the same
framework build). Facts marked **(unverified)** come from the Wisej.NET XML docs / reflection over
`Wisej.Framework.dll`; implement them, make sure `dotnet build` passes, and say so in your report.

## Project layout (copy `_template`)

```
Module N/
  WisejTrainingApp.slnx                (from _template)
  .gitignore                           (from _template)
  README.md                            (what it shows, how to run, what to click, lab-step → code map)
  WisejTrainingApp/
    WisejTrainingApp.csproj            (from _template; TargetFrameworks = net10.0-windows;net10.0 — keep it)
    Program.cs  Startup.cs  Default.html  Default.json  Web.config
    Properties/launchSettings.json     (port: Module N → http://localhost:508N; Module 10 → 5090)
    <MainWindow>.cs + .Designer.cs     the screen the course names (Window1, DashboardWindow, MainPage …)
    Models/  Services/  Views/  Dialogs/  Widgets/   as the module needs them
    docs/                              the lab deliverables as Markdown
```

- Namespace is always `WisejTrainingApp` (the practice project name the course suggests in "Getting started").
- `Program.Main(NameValueCollection args)` is the session entry point (`Default.json` → `"startup"`). It does
  `new Window1().Show();` for a Form, or `Application.MainPage = new MainPage();` for a Page.
- Run: `dotnet run -f net10.0 --urls http://localhost:508N` from the project folder. The static file server
  serves the **project folder**, so a Widget package `Source = "Widgets/statusGauge.css"` is fetched as
  `/Widgets/statusGauge.css`. `Default.json` / `Web.config` are never served.
- Build with `dotnet build -nologo -v q` and fix every error. Warning CS7022 (Program.Main ignored) is expected and
  already silenced in the csproj.
- Do not run the app yourself and do not start servers; the reviewer runs it in the browser.

## Verified runtime facts (from the Application Integration Course samples, same framework build)

- `Wisej.Web.Timer(components)` with `Interval`, `Tick`, `Start()/Stop()`, `Enabled` — a Component with no visual surface.
- `Application.StartTask(() => { …change controls…; Application.Update(this); })` runs work on a background thread;
  `Application.Update(component)` pushes pending changes over WebSocket from a non-request thread. Bound the frequency;
  check `IsDisposed`.
- `AlertBox.Show(text, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000)`
  — always TopRight so toasts do not cover the buttons.
- Fonts: `new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold)`, monospace `new System.Drawing.Font("monospace", 9F)`.
  `Label.TextAlign` uses `System.Drawing.ContentAlignment`. `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`.
- `Wisej.Web.Widget`: `Packages` (List<Widget.Package>{Name, Source}), `InitScript` (string), `Options` (assign an
  anonymous object — property names are **camel-cased** on the client), `WidgetEvent += (s, e) => …` with `e.Type` and
  `dynamic e.Data`. `widget.Update()` after changing `Options` runs `this.update(options, old)` in the browser;
  `widget.Call("pulse")` runs `this.pulse()` on the client wrapper. `fireWidgetEvent` from a **user-triggered** DOM
  callback (click) reaches the server fine; a `fireWidgetEvent` raised synchronously *during* `update()` is dropped.
  Never define `getWidth`, `getHeight`, `destroy`, `resize`, `show`, `hide`, `getValue` on the wrapper (`this`).
- `Application.LoadTheme("Material-3")` restyles the running app live. Themes embedded in Wisej-4 4.1.0:
  Blue-1, Blue-2, Blue-3, Bootstrap-4, BootstrapDark-4, Classic-2, Clear-1/2/3, FluentDark-5, FluentLight-5,
  Graphite-3, Material-3, Material-4, MaterialDark-4, Vista-2. The course's `Application.Theme.Name = …` line is the
  shorthand; `Application.Theme` is a settable `ClientTheme`, `LoadTheme(name)` is the call that loads a built-in theme.

## Signatures checked by reflection (Wisej.Framework.dll 4.1.0) — **(unverified at runtime unless noted)**

```csharp
// Dialogs — Wisej.NET has no blocking ShowDialog(); use the awaitable form in an async handler:
DialogResult result = await dialog.ShowDialogAsync();            // Task<DialogResult>
dialog.ShowDialog((form, result) => { … });                       // callback form; returns immediately
this.DialogResult = DialogResult.OK; this.Close();                // inside the dialog's Save handler

// Message boxes
DialogResult r = await MessageBox.ShowAsync("Delete ticket #3?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
MessageBox.Show("Ticket saved.", "Tickets", MessageBoxButtons.OK, MessageBoxIcon.Information);   // fire-and-forget

// Data binding
var ticketsBindingSource = new BindingSource();                   // Wisej.Web.BindingSource; create in code or as a component
ticketsBindingSource.DataSource = ticketService.GetTickets();     // List<Ticket>
dgvTickets.AutoGenerateColumns = false;
dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "Title", Name = "colTitle", Width = 220 });
dgvTickets.DataSource = ticketsBindingSource;
txtTitle.DataBindings.Add("Text", ticketsBindingSource, "Title", true, DataSourceUpdateMode.OnPropertyChanged);
Ticket current = ticketsBindingSource.Current as Ticket;
ticketsBindingSource.ResetBindings(false);                        // refresh after a save
ticketsBindingSource.CurrentChanged += …;                         // selection follows the grid

// Grid settings the course asks for
dgvTickets.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgvTickets.MultiSelect = false; dgvTickets.ReadOnly = true;
dgvTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

// Layout
splitContainer1.Dock = DockStyle.Fill; splitContainer1.Orientation = Orientation.Vertical; splitContainer1.SplitterDistance = 760;
splitContainer1.Panel1.Controls.Add(dgvTickets);

// Checklist
chkRequired.CheckOnClick = true; chkRequired.Items.Add("Web.config reviewed…"); chkRequired.AfterItemCheck += …;
bool done = chkRequired.GetItemChecked(i); chkRequired.CheckedItems.Count

// Session / config
Application.SessionId; Application.Session (dynamic bag per user session: Application.Session.CurrentJob = …);
Application.MapPath("Widgets/statusGauge.js"); Application.StartupPath;
Application.Configuration  (Wisej Default.json settings)
```

## async/await in event handlers **(unverified — verify and report)**

`private async void btnStartImport_Click(…)` with `await Task.Delay(…)` / `await ShowDialogAsync()` is supported.
After an `await` the continuation runs outside the original request, so **push each progress update** with
`Application.Update(this)` (the Form/Page) after changing `progressBar.Value`, labels and the log; the final state is
pushed when the handler completes. Wrap the job in `try / catch / finally`, reset button state in `finally`, keep a
`CancellationTokenSource` in an **instance** field (per user session), never in a `static` field.

## UI conventions used by every sample (so the samples feel like one course)

- The main screen is a `Form` (or `Page`) sized about 1348×680, light grey background `Color.FromArgb(238,242,247)`,
  white cards (`Panel`, `BorderStyle.Solid`), card titles `"default" 12F Bold`, monospace 9F for logs/code.
- Right-hand (or bottom) card **"Event log"**: a `ListBox` with monospace font; every user action and every server
  decision is logged as `HH:mm:ss  message` through one `AddLog(string)` helper. Select the last item after adding.
  (`lstEventLog` / `lstLog` / `lstActivity` — use the name the lab guide uses.)
- A status label (`lblStatus`) that changes text and colour: green `Color.FromArgb(31,157,87)` ok, amber
  `Color.FromArgb(232,161,60)` warning, red `Color.FromArgb(224,86,59)` error.
- Bottom bar (or per-card command rows) of buttons that exercise the **success path**, a **progress path** where the
  module has one (Timer / async job), at least one **failure path** (validation rejected, simulated error, permission
  denied) and the **recovery**.
- Control names exactly as the lab guide asks (`lblStatus`, `btnStart`, `lstEventLog`, `dgvTickets`, `txtTitle`,
  `cboStatus`/`cmbStatus`, `btnSaveTicket`, `NavigateTo`, `SetJobRunning`, `ValidateForm`, `widStatus`, …). Never `button1`.
- Designer-style `<Name>.Designer.cs` with `InitializeComponent()` so the file opens in the Wisej Designer; code-behind
  in `<Name>.cs`. Short, readable event handlers that call helpers/services — that is the point of the course.
- Toasts: `AlertBox.Show(text, icon, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000)`.
- Business rules and data live in `Services/` (`TicketService`, `StatusService`, `ReleaseReviewService`), models in `Models/`.

## Docs (`docs/`)

Write each lab deliverable as its own Markdown file named after what the lab asks for (`ProjectStructure.md`,
`DashboardEvents.md`, `ShellLayout.md`, `DataBindingNotes.md`, `DialogWorkflow.md`, `BackgroundJobNotes.md`,
`ThemingNotes.md`, `WidgetIntegrationNotes.md`, `ReleaseChecklist.md`, `ArchitectureNotes.md`, …). Include a short
**Evidence** section describing what the running app shows for each path. The README carries a **Lab steps → where
in the code** table mapping every lab-guide step to a file/method, plus a **Self-check** section answering the
questions the lesson's "Completion check" / "Checkpoint" boxes ask.
