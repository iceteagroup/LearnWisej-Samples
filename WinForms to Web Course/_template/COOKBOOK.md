# From WinForms to the Web — sample cookbook (Wisej-4 4.1.0, .NET 10)

Conventions for the seven module samples of the course **From WinForms to the Web**. Everything
marked **(verified)** was built and run in the browser on this machine; everything else comes from the
Wisej.Framework.xml API reference of the installed package (`~/.nuget/packages/wisej-4/4.1.0`) — use it,
make `dotnet build` pass, and say in your report which runtime paths you could not exercise.

The course's worked example is **LegacyOrderDesk**, a single-user WinForms order desk, migrated step by
step into **OrderDesk.Web**, a Wisej.NET web app. Every module folder is a snapshot of that migration
at the end of the module.

## Layout (copy `_template`)

```
Module N/
  OrderDesk.slnx                  (from _template; lists OrderDesk.Web and, in Module 1, LegacyOrderDesk)
  .gitignore                      (from _template)
  README.md                       (what it shows, how to run, what to click, deliverables, self-check answers)
  LegacyOrderDesk/                (Module 1 only: the WinForms "before" app — copy from _template as-is)
  OrderDesk.Web/
    OrderDesk.Web.csproj          (from _template; TargetFrameworks net10.0-windows;net10.0; AssemblyName OrderDesk)
    Program.cs  Startup.cs  Default.html  Default.json  Web.config
    Properties/launchSettings.json  (port 5100 + N: Module 1 → 5101 … Module 7 → 5107)
    Domain/                       (shared, reused unchanged: Order, Customer, OrderStore, OrderService, OrderValidator, OrderQuery, User)
    Shared/                       (TracePanel, Palette, Notify — course lab props, same in every module)
    MainPage.cs + MainPage.Designer.cs  (the module's screen; designer-style InitializeComponent)
    Pages/ Dialogs/ Services/     (module code)
    docs/                         (the lab deliverables as Markdown; migration-log.md in every module)
```

- Namespace is always `OrderDesk` (`OrderDesk.Domain`, `OrderDesk.Shared`, `OrderDesk.Pages`, …). The
  WinForms project is `LegacyOrderDesk`.
- Run: `dotnet run -f net10.0` from `OrderDesk.Web` (launchSettings sets the port), or
  `dotnet run -f net10.0 --urls http://localhost:510N`. Visual Studio: open `OrderDesk.slnx`, F5.
- Build with `dotnet build -nologo -v q` and fix every error. Warning CS7022 (Program.Main ignored) is
  suppressed in the csproj. `LegacyOrderDesk` builds too (`net10.0-windows`, `UseWindowsForms`); it
  compiles the `Domain/*.cs` files of `OrderDesk.Web` by link so the business logic is literally shared.
- Do not start servers and do not run the app yourself; the reviewer runs it in the browser.
- Never add NuGet packages. Wisej-4 4.1.0 is the only dependency. No Office Interop, no third-party
  spreadsheet/PDF libraries: write the tiny managed writers yourself (see Module 6 notes).

## Startup — what replaces `Application.Run` **(verified)**

- `Startup.cs` (top-level statements): `WebApplication.CreateBuilder(new WebApplicationOptions { Args = args, WebRootPath = "./" })`,
  `app.UseWisej()`, `app.UseWhen(ctx => !ctx.Request.Path.Value.EndsWith(".json"), a => a.UseFileServer())`, `app.Run()`.
- `Default.json`: `"url": "Default.html"`, `"startup": "OrderDesk.Program.Main, OrderDesk"`, `"theme": "Bootstrap-4"`.
  (`"mainWindow": "OrderDesk.MainPage, OrderDesk"` is the documented alternative that opens a type
  directly; the samples use `startup` because it is the verified path and mirrors the lesson's
  `Application.MainPage = new MainPage();`.)
- `Program.Main(NameValueCollection args)` runs once per **session** and sets `Application.MainPage = new MainPage();`.
  There is no `EnableVisualStyles`, no `Application.Run`: Kestrel owns the process.
- Static files: the file server serves the project folder, so `wwwroot/x.js` is `/wwwroot/x.js`.
- `Web.config` `<appSettings>` and `<connectionStrings>` replace `App.config`. Read them with
  `System.Configuration.ConfigurationManager`? **No** — not referenced. Use
  `Wisej.Web.Application.Configuration` (dynamic; `Application.Configuration.Settings["key"]` is undocumented) or,
  simplest and verified-by-build: parse `Web.config` with `System.Xml.Linq` in a tiny `AppConfig` helper
  (`XDocument.Load(Path.Combine(Application.StartupPath, "Web.config"))`). Say which one you used.

## Verified Wisej.Web facts (from the Application Integration course samples on this machine)

- `AlertBox.Show(text, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000)` — always TopRight so toasts don't cover buttons.
- `MessageBox.Show("Saved.")` works (signature `Show(text, caption = "", buttons, icon, defaultButton, modal = true, allowHtml, keepOnScreen, rtl, onclose)`; returns `DialogResult`).
- Fonts: `new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold)`, monospace `new System.Drawing.Font("monospace", 9F)`.
  `Label.TextAlign` takes `System.Drawing.ContentAlignment`. `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`.
- `Wisej.Web.Timer(components)` with `Interval`, `Tick`, `Start()/Stop()`.
- Background work: `Application.StartTask(() => { …change controls…; Application.Update(this); })`;
  `Application.Update(component)` pushes changes over the WebSocket from a non-request thread. Check `IsDisposed`.
- `Application.SessionId` (string), `Application.SessionCount` (int), `Application.Browser` (`Wisej.Core.ClientBrowser`:
  `Type, Version, OS, Device, Size, ScreenSize, IsSecure, IPAddress, UserAgent, TabId, Language`).
- `Application.LoadTheme("Material-3")` restyles live. Built-in themes in Wisej-4 4.1.0: `Bootstrap-4`, `Material-3`, `FluentDark-5` (verified), plus others in the package.
- A hidden browser tab never fires the qooxdoo "appear" queue; give a freshly loaded page a few seconds before driving it.

## API facts from Wisej.Framework.xml (compile-checked, not yet run) — use exactly these shapes

- **Modal dialogs**: `Form.ShowDialog()` returns `DialogResult` and blocks the handler until the form
  closes (`ShowDialog(Action<Form, DialogResult> onclose = null)`; `ShowDialog(Form owner, …)`; `ShowDialogAsync(Form owner)` → `Task<DialogResult>`).
  Set `this.DialogResult = DialogResult.OK; Close();` in the dialog. **Closed dialogs are not disposed** — the caller
  must `using (var dlg = new EditOrderDialog(order)) { if (dlg.ShowDialog() == DialogResult.OK) … }`.
- **Toast** (non-blocking): `var t = new Toast("Order 1042 saved.", "icon-ok") { AutoCloseDelay = 3500, Alignment = ContentAlignment.BottomRight }; t.Show();`
  (`Toast(string text, string icon)`; props `AutoCloseDelay`, `Alignment`, `AllowHtml`, `AutoDispose`, `BackColor`, `ForeColor`; events `Click`, `Closed`). `Shared/Notify.cs` wraps it.
- **Session state**: `Application.Session` is a dynamic bag: `dynamic s = Application.Session; s.UserContext = ctx; var ctx = (UserContext)s.UserContext;`
  (reading a missing member returns null). Module 4 wraps it in `UserContext.Current`.
- **Session end**: `Application.ApplicationExit` (event, static) fires when the session ends; `Application.SessionTimeout` fires
  before timeout (`HandledEventArgs`, set `Handled = true` to suppress the built-in prolong dialog). Subscribe in `Program.Main` or the page constructor.
- **Download**: `Application.Download(string filePath, string fileName, Action<string> ondownload = null)`;
  `Application.Download(Stream stream, string fileName, …)`; `Application.DownloadAndOpen(target, …)`. Use the `Stream` overload for generated files (`new MemoryStream(bytes)`).
- **Upload**: `Wisej.Web.Upload` control — `Text` (button caption), `AllowMultipleFiles`, `AllowedFileTypes` (e.g. `".csv"`), `MaxFileSize`,
  event `Uploaded` with `UploadedEventArgs.Files` (`Wisej.Core.HttpFileCollection`; each `HttpPostedFile` has `FileName, ContentType, ContentLength, InputStream`).
  Also `Uploading`, `Progress`, `Error` events.
- **PdfViewer**: `Wisej.Web.PdfViewer` — `PdfSource` (string URL/path) or `PdfStream` (Stream), `ViewerType`, `FileName`.
- **DataGridView**: `VirtualMode = true`, `RowCount = n`, handle `CellValueNeeded(DataGridViewCellValueEventArgs e)` (`e.RowIndex`, `e.ColumnIndex`, set `e.Value`);
  `BlockSize` (rows fetched per block), `CellDoubleClick`, `DataSource`, `ReadOnly`, `RowHeadersVisible`, `SelectionMode`, `AutoSizeColumnsMode`,
  `AddSummaryRows(SummaryType.Sum, SummaryRowPosition.Below, groupCol, sumCol, style = null)`, `BeginUpdate/EndUpdate`, `Sort(column, direction)`.
  Columns: `new DataGridViewTextBoxColumn { HeaderText, DataPropertyName, Width, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, DefaultCellStyle = { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } }`.
  Bind a `List<Order>` via `DataSource`; the row object is `grid.Rows[i].DataBoundItem`. In VirtualMode fill `e.Value` from a page cache (`OrderStore.Search` with `Skip/Take`).
- **ErrorProvider**: `new ErrorProvider(components)`; `SetError(control, message)`, `Clear()`, `GetError(control)`.
- **Label.AllowHtml** (bool) — encoded text by default; Module 7 shows why it must stay off for user data.
- **Responsive**: `Application.ActiveProfile` (`Wisej.Core.ClientProfile`: `Name, MinWidth, MaxWidth, Device, …`), event
  `Application.ResponsiveProfileChanged` (`ResponsiveProfileChangedEventArgs.CurrentProfile.Name`), `Application.BrowserSizeChanged`, `Application.Browser.Size`.
  Profiles are defined in `ClientProfiles.json` in the app folder (see Module 7 notes; the built-in ones are Desktop/Tablet/Phone-like).
  `Control.ResponsiveProfiles` holds designer-set per-profile values; the samples react in code to `ResponsiveProfileChanged` instead (verifiable, readable).
- **Menus/nav**: `Wisej.Web.MenuBar` (with `MenuItem`), `ToolBar` + `ToolBarButton`, `StatusBar` + `StatusBarPanel`, `TabControl`, `SplitContainer`,
  `FlexLayoutPanel`, `FlowLayoutPanel`, `TableLayoutPanel`, `Accordion`, `LinkLabel`, `PictureBox`, `CheckBox`, `RadioButton`, `ComboBox`, `DateTimePicker`, `NumericUpDown`, `ProgressBar`, `Canvas`, `HtmlPanel`, `IFramePanel`, `TreeView`, `ListBox`.

## UI conventions (so the seven apps look like one product)

- `MainPage : Page` with `BackColor = Palette.PageBackground` (238,242,247). A blue app bar (`Palette.Accent` #1565d8, white bold text "OrderDesk — <module title>") docked top, height 44.
- Content = white cards (`Panel`, `BorderStyle.Solid`, `Palette.CardBackground`). The **`TracePanel`** (Shared) docked right, width ~560:
  `Trace.Server(name, payload)`, `.Out(…)` (server → browser), `.In(…)` (browser → server), `.Finding(…)` (a migration-log line), `.Fail(…)`, `.Ok(…)`.
  Log every button click, every service call, every crossing of the browser boundary and every "this is why" finding.
- A **status label** (● idle / working / alarm) and an **alarm/finding banner** label that appears/disappears, like the integration samples.
- A bottom **button bar** that exercises: the module's **success path**, a **progress path** (Timer or `Application.StartTask`), at least one
  **failure path** (the desktop assumption breaking on the server, caught and explained), and **recovery** (the web-safe replacement).
- `MainPage.Designer.cs` with `InitializeComponent()` so the page opens in the Wisej Designer; code-behind in `MainPage.cs`.
- Always show the five walkthrough orders first: 1042 Northwind Traders $4,820.00 Open · 1041 Contoso Ltd $1,290.50 Shipped · 1040 Fabrikam Inc $760.00 Open ·
  1039 Adventure Works $12,400.00 Invoiced · 1038 Globex Corp $3,090.00 Open (they are the first rows of `OrderStore.Shared()`; `OrderService.GetAll()` returns newest first).
- Users in the videos: **kelly** (Acme, session A, customer Northwind, filter Open) and **sam** (Globex, session B, customer Fabrikam, filter Invoiced); owners Dana, Priya, Sam, Kelly.

## Docs (`docs/`)

One Markdown file per lab deliverable, named as the course names it, each with an **Evidence** section (what the running app shows,
button by button) and the **self-check answers** from the lab guide repeated in the README. Every module also keeps
`docs/migration-log.md` — the running log the videos keep opening (`migration-log.md` tab) with one line per accepted
workaround/decision, carried forward and extended module by module.

## Shared domain (`Domain/`) — reuse, don't rewrite

`OrderStore.Shared(count)` (process-wide, locked reference data; `Create(count, seed)` for a private store), `OrderService`
(`GetAll`, `Find`, `Search(OrderQuery)`, `Count`, `Save` (validates, throws `ValidationException`), `Customers`, `Owners`, `CountByStatus`),
`OrderCalculator.CalculateOrderTotal`, `OrderValidator.Validate → ValidationResult { HasErrors, Errors[field] }`, `OrderQuery { Status, Search, SortBy, Descending, Skip, Take }`,
`Order { Id, PoNumber, Customer, Owner, Status, Date, Lines, Notes, CustomerName, Total }`, `Customer { Name, Country, Tier, TaxRate, CreditLimit, TaxId (sensitive) }`, `User { UserName, DisplayName, Role, Company }`.
The WinForms app's desktop plumbing lives in `LegacyOrderDesk/Legacy/` (`AppState` statics, `UserPreferences` registry, `LocalExport` C:\Orders, `ExcelExport` Interop stub, `InvoicePrinter`) — copy the pieces a module needs to demonstrate into `OrderDesk.Web/Legacy/` and mark them ✕ in comments; never reference the WinForms project from the web project.
