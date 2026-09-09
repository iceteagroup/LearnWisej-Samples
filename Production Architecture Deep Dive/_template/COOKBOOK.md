# TicketOps cookbook (verified on Wisej-4 4.1.0, .NET 10)

Everything marked **verified** was built and exercised in the browser while making Module 1.
Follow it exactly. Anything marked **(unverified)** comes from the Wisej.Framework XML docs
(`%USERPROFILE%\.nuget\packages\wisej-4\4.1.0\lib\net8.0\Wisej.Framework.xml`) and has not been
executed yet: implement it, make sure `dotnet build` passes, and say so in your final report so the
reviewer can check it at runtime.

## Project layout (copy `_template`)

```
Module N/
  TicketOps.slnx                  (from _template)
  .gitignore                      (from _template)
  README.md                       (what it shows, how to run, what to click, deliverables, self-check answers)
  TicketOps/
    TicketOps.csproj              (from _template; add <EmbeddedResource> lines for any embedded JS / resx you add)
    Program.cs  Startup.cs  Default.html  Default.json  Web.config
    Properties/launchSettings.json   (port: Module N uses http://localhost:51NN — Module 2 → 5102, Module 12 → 5112)
    Views/          the screens (Form / Page / UserControl + .Designer.cs). Name them after what the user does there.
    Controls/       reusable UserControls (StatusBanner ships in the template)
    Services/       IXxxService + implementations: decisions and workflows, no UI types
    Domain/         records + their rules, no UI and no Wisej dependency
    Data/           repositories (in-memory fakes here), never referenced by Views directly
    Infrastructure/ ILog, ActivityLog, AppComposition (who gets what), config, integration
    Resources/      Strings (safe user-facing text), themes, images, .resx
    Diagnostics/    ActivityTracePanel (ships in the template), health/diagnostics pages
    docs/           the lab deliverables as Markdown (+ SVG diagrams when the lab asks for one)
```

- Namespace is always `TicketOps` (`TicketOps.Views`, `TicketOps.Services`, …). Course app name: **TicketOps Console**.
- Run: `dotnet run -f net10.0 --urls http://localhost:51NN` from the project folder (the csproj multi-targets
  `net10.0-windows;net10.0`, so `dotnet run` needs `-f`). Build with `dotnet build -nologo -v q` and fix every error/warning.
- Do not run the app yourself and do not start servers; the reviewer runs it in the browser.
- `Program.Main(NameValueCollection)` runs once per browser session: `new AppComposition().CreateMainView().Show()`.

## Shared pieces in the template (verified)

| File | What it is | How to use it |
|---|---|---|
| `Infrastructure/ILog.cs` | `ILog` + `LogLayer` (UI, Service, Domain, Data, Infrastructure, Session, Client) + `LogLevel` | every layer takes `ILog` in its constructor and logs `Info/Warn/Error(layer, source, message)` |
| `Infrastructure/ActivityLog.cs` | plain, thread-safe `ILog` implementation with an `EntryAdded` event | one per session, created in `AppComposition`; never static |
| `Diagnostics/ActivityTracePanel` | UserControl (title + monospace `ListBox` + footer) that renders the log live | drop it in the right-hand card; `tracePanel.Attach(activityLog)` in the view constructor; `ClearTrace()` |
| `Controls/StatusBanner` | UserControl: right-aligned "● state" label + a banner line that appears/disappears | `SetStatus("ready", StatusKind.Success)`, `ShowBanner(text, StatusKind.Warning)`, `HideBanner()` |
| `Resources/Strings.cs` | safe user-facing messages (`ActionFailed`, …) | handlers show these, never `ex.Message` |
| `Infrastructure/AppComposition.cs` | the session factory: creates log, repositories, services, and the main view with constructor injection | extend with the module's services; Module 8 swaps it for `Application.Services` |
| `Views/MainView` | skeleton screen: left card 760×560, trace card 508×560 on the right, bottom button bar, 1348×680 form | rename/replace with the module's screen (Module 1 → `TicketEditor`) |

Trace line format (`ActivityTracePanel.Format`): `HH:mm:ss.fff ⚠ [SVC] TicketService.SaveAsync — validate {…}`.
**Verified gotcha:** the `ListBox` renders HTML, so runs of spaces collapse — never align columns with padding; use
separators (`[LAYER]`, `—`, `→`). Keep `Source` short (`Class.Method`) so the line fits.

## Handler shape (verified — copy it)

```csharp
private async void buttonSave_Click(object sender, EventArgs e)
{
    try
    {
        var draft = ReadDraftFromForm();                                   // UI → data
        _log.Info(LogLayer.UI, "TicketEditor.buttonSave_Click", $"→ ITicketService.SaveAsync {draft}");
        var result = await _tickets.SaveAsync(draft);                     // the decision lives in the service
        ShowResult(result);                                                // data → UI (OperationResult: Succeeded/Message)
    }
    catch (Exception ex)
    {
        ReportFailure("TicketEditor.buttonSave_Click", ex);               // log details, show Strings.ActionFailed
    }
}
```

- Expected outcomes (validation, a domain rule saying no) are **results**, not exceptions: `OperationResult<T>.Fail("Title is required.")`.
  Unexpected failures (repository outage) are exceptions caught by the handler: `_log.Error(...)` with the exception, then the safe message.
- `async void` handlers are not awaited by the framework: each owns its try/catch. Never let an exception escape (Wisej shows its own error popup).
- The Designer keeps a parameterless constructor (`public TicketEditor() : this(null, null, new ActivityLog())`); the real one takes the services.
- Never store per-user state in a `static` field. `AppComposition` is instantiated per session; keep it that way.

## Wisej.NET facts used by the course (API names checked against the XML docs)

- **verified** `Wisej.Web.Timer(components)` with `Interval`, `Tick`, `Start()/Stop()`, `Enabled` — paces a progress path; the tick handler can be `async void`.
- **verified** `DataGridView`: designer-created `DataGridViewTextBoxColumn`s, `Rows.Clear()`, `Rows.Add(values…)`, `CurrentRow.Index`, `SelectionChanged`, `SelectionMode = DataGridViewSelectionMode.FullRowSelect`, `ReadOnly`, `RowHeadersVisible = false`, `ClearSelection()`, `CurrentCell = null`.
- **verified** `ComboBox.DropDownStyle = ComboBoxStyle.DropDownList`, `Items.AddRange`, `SelectedIndex`; `NumericUpDown.Value/DecimalPlaces/Increment/Maximum`; `TextBox.Watermark`; `ProgressBar.Value/Maximum/Visible`.
- **verified** `AlertBox.Show(text, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000)` — top-right toast so it never covers the buttons.
- **verified** Fonts: `new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold)`, monospace `new System.Drawing.Font("monospace", 9F)`. `Label.TextAlign` takes `System.Drawing.ContentAlignment`. `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`.
- **verified** `Form.Show()` from `Program.Main` opens the main window; `Application.MainPage = new MainPage()` is the Page alternative.
- `Application.Session` (dynamic per-session bag: `Application.Session.Tenant = "…"`), `Application.SessionId`, `Application.SessionCount`, `Application.SessionTimeout` event, `Application.SetSessionTimeout(seconds)`, `Application.ApplicationStart/ApplicationExit/ApplicationRefresh` events, `Application.ThreadException` event **(unverified)**.
- `Application.GetInstance<T>(ref SessionReference<T>, factory)` — session-static instances **(unverified)**.
- **DI (Module 8):** `Application.Services` is a `Wisej.Services.ServiceProvider`: `AddService<TService, TImpl>(ServiceLifetime.Session)`, `AddService<T>(instance, lifetime)`, `AddService<T>(Func<Type, object>, lifetime)`, `AddOrReplaceService…`, `GetService<T>()`, `HasService<T>()`, `RemoveService<T>()`, `Inject(object)`. Lifetimes: `Shared` (application-wide), `Session`, `Thread`, `Transient`. `[Inject]` / `[Inject(Required = true)]` on properties (any visibility) of Forms/Pages/Desktops (top-level containers get injected automatically) and of services created by the container; call `Application.Services.Inject(obj)` for anything else **(unverified)**.
- **Responsive (Module 3):** `Application.ActiveProfile` (`Wisej.Core.ClientProfile`: `Name`, `Device`, `MinWidth/MaxWidth`, `Landscape`…), `Application.ResponsiveProfileChanged` (`ResponsiveProfileChangedEventArgs`), `Control.ResponsiveProfileChanged`, `Application.BrowserSizeChanged`; `ClientProfiles.json` in the app folder defines custom profiles (`Wisej.Core.ClientBrowser.LoadClientProfiles`); `Application.Browser.Size/ScreenSize/Device/IsDarkMode/TimezoneId` **(unverified)**. Layout engines: `Dock`, `Anchor`, `FlowLayoutPanel`, `TableLayoutPanel`, `FlexLayoutPanel`, `SplitContainer`, `TabControl` **(unverified)**.
- **Binding (Module 4):** `BindingSource`, `BindingList<T>`, `INotifyPropertyChanged`, `DataGridView.DataSource`, `CellFormatting` **(unverified)**.
- **Validation (Module 5):** `ErrorProvider` (`SetError(control, text)`), `Control.Validating` **(unverified)**.
- **Dialogs (Module 6):** `Form.ShowDialog(Action<Form, DialogResult>)`, `await Form.ShowDialogAsync()`, `Form.DialogResult`, `Form.AcceptButton/CancelButton`, `MessageBox.ShowAsync(...)`, `Application.ConfirmAsync(text)` **(unverified)**.
- **Background (Module 7):** `Application.StartTask(() => { … Application.Update(this, () => { …touch controls… }); })` pushes UI changes over WebSocket from a worker thread; `Application.RunInContext(component, action)` runs code in the session context; `CancellationTokenSource` for cancellation; check `IsDisposed` and bound the update rate **(unverified)**.
- **JavaScript (Module 9):** `Application.Eval("js")`, `await Application.EvalAsync("expr")`, `Application.Call("fn", args)`, `Control.Call/CallAsync/Eval/EvalAsync`, the `Wisej.Web.JavaScript` extender component (per-control script + `JavaScriptSource`), `Control.Click`-style callbacks arrive as normal server events; `[assembly: WisejResources]` + embedded `/Platform/*.js` bundles a script into the client **(unverified — see the Application Integration Course cookbook for the verified widget/InitScript details)**.
- **Theming / localization (Module 10):** `Application.LoadTheme("Material-3")`, `Application.Theme`, `Application.ThemeChanged`; built-in themes include Bootstrap-4, Material-3, FluentDark-5; `Application.CurrentCulture = new CultureInfo("de-DE")`, `Application.CultureChanged`, `Application.AddTranslation(key, text)`, .resx via `ResourceManager` **(unverified)**.
- **Security (Module 11):** `Application.User` (`IPrincipal`), `Application.IsAuthenticated`, `Application.IsSecure`, `Label.AllowHtml` (default false → text is escaped) **(unverified)**.
- **Deployment (Module 12):** `Application.SessionCount`, `Application.ServerName/ServerPort`, `Application.RuntimeMode`, `Application.ProductVersion`, `Application.IsWebSocket`, `Application.Expired`, `Application.Idle` event; a `HealthCheck.json`/diagnostics page is app code (read it with `File.ReadAllText(Application.MapPath("HealthCheck.json"))`) **(unverified)**.
- Browser note for reviewers: the Browser pane is small; call `resize_window` (1400×760) before screenshots. Give a freshly loaded page ~5 s before driving it.

## UI conventions used by every sample (so the samples feel like one course)

- A `Form` sized 1348×680, light grey background `Color.FromArgb(238,242,247)`, white cards with `BorderStyle.Solid`.
- Left card: the module's screen ("Open Tickets", "Work Orders", "Session diagnostics", …) with `StatusBanner` top-right.
- Right card: `ActivityTracePanel` titled "Activity trace · UI → Service → Data" (retitle for the module if useful).
- Bottom bar: buttons for the **success path**, a **progress path**, at least one **failure path** (validation / rule / permission) and the
  **error path with recovery** (simulated outage toggle), plus **Clear trace** on the right. Tooltips (`ToolTipText`) say which path the button shows.
- Designer-style `*.Designer.cs` with `InitializeComponent()` so every screen opens in the Wisej Designer; code-behind in `*.cs`.

## Docs (`docs/`)

Write each lab deliverable as its own Markdown file, named as the course names it (e.g. `ArchitectureNote.md`, `StaticStateAudit.md`,
`ServiceLifetimeTable.md`, `SecurityChecklist.md`, `DeploymentChecklist.md`, `ReleaseNotes.md`). Include a short **Evidence**
section describing what the running app shows for each path, and put the **self-check answers** from the lab guide in the README.
