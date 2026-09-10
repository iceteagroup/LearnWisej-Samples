# From WinForms to the Web — sample cookbook (Wisej-4 4.1.0, .NET 10)

Conventions for the seven module samples of the course **From WinForms to the Web**. Everything marked
**(verified)** was built and run in the browser on this machine (Module 1 and the other course sample sets in this
repo); everything marked **(xml)** comes from the API reference shipped in the installed package
(`%USERPROFILE%\.nuget\packages\wisej-4\4.1.0\lib\net48\Wisej.Framework.xml`) and is compile-checked, not yet run.

The course's worked example is **LegacyOrderDesk**, a single-user WinForms order desk (login → orders grid → edit
dialog → Print Invoice / Export to Excel / Attach file, settings in HKCU), migrated step by step into
**OrderDesk.Web**, a Wisej.NET web app. Every module folder is a snapshot of that migration at the end of the module
**plus a lab console** that exercises the module's success path, a progress path, at least one failure path (the
desktop assumption breaking on the server, caught and explained) and the recovery (the web-safe replacement).

## Layout (copy `_template`)

```
Module N/
  OrderDesk.slnx                  from _template (Modules 1–2 list LegacyOrderDesk too; later modules only OrderDesk.Web)
  .gitignore                      from _template
  README.md                       what it shows, how to run, what to click (table), where things live, deliverables, self-check answers
  LegacyOrderDesk/                Modules 1 and 2 only: the WinForms "before" app — copy from _template unchanged
  OrderDesk.Web/
    OrderDesk.Web.csproj          from _template: <TargetFrameworks>net10.0-windows;net10.0</TargetFrameworks>, AssemblyName + RootNamespace OrderDesk, Wisej-4 4.1.0
    Program.cs  Startup.cs  Default.html  Default.json  Web.config
    Properties/launchSettings.json   port 5600 + N  (Module 1 → 5601 … Module 7 → 5607)
    Domain/                       shared business logic, reused unchanged from LegacyOrderDesk (Order, Customer, OrderService, CustomerService, InvoiceDocument, SampleData)
    Views/                        Ui.cs (colours, fonts, Toast, SetStatus, banners), TracePanel.cs (the migration log), InvoicePreviewForm.cs
    Legacy/                       copies of the desktop plumbing the module demonstrates, marked ✕ in comments (AppState, RegistrySettings, ExcelExport …)
    Migration/ Reporting/ Services/ Dialogs/ …   module code
    MainPage.cs + MainPage.Designer.cs           the lab console (designer-style InitializeComponent)
    docs/                         the lab deliverables as Markdown + migration-log.md (carried forward and extended every module)
```

- Namespace is always `OrderDesk` (`OrderDesk.Domain`, `OrderDesk.Views`, `OrderDesk.Legacy`, `OrderDesk.Migration`, …). The WinForms project is `LegacyOrderDesk`; **never reference it from the web project** — copy the file you need into `Legacy/` instead.
- Run: `dotnet run -f net10.0` from `OrderDesk.Web` (launchSettings sets the port) or `dotnet run -f net10.0 --urls http://localhost:560N`. Visual Studio: open `OrderDesk.slnx`, F5.
- Build with `dotnet build -nologo -v q -f net10.0` **and** `-f net10.0-windows` (both must pass). CS7022 (Program.Main ignored), CA1416 and WFO1000 are suppressed in the csproj.
- An XML comment in a csproj must not contain `--` (MSB4025) — write `-\-urls` in csproj comments. **(verified)**
- Never add NuGet packages. Wisej-4 4.1.0 is the only dependency. No Office Interop, no third-party PDF/spreadsheet libraries: the tiny managed writers are hand-written (`Reporting/InvoicePdfWriter.cs` is a dependency-free PDF writer; a minimal .xlsx can be written with `System.IO.Compression.ZipArchive` + OpenXML part strings).
- `Application.StartupPath` is the project folder under `dotnet run` (content root), so `Path.Combine(Application.StartupPath, "App_Data", …)` works; create the folder on demand. **(verified)**

## Startup — what replaces `Application.Run` **(verified, Module 1)**

- `Startup.cs` (top-level statements): `WebApplication.CreateBuilder(new WebApplicationOptions { Args = args, WebRootPath = "./" })`, `app.UseWisej()`, `app.UseWhen(ctx => !ctx.Request.Path.Value.EndsWith(".json"), a => a.UseFileServer())`, `app.Run()`. There is **no `AddWisej()`** in Wisej-4 4.1.0 (the walkthrough mentions it; the package only has `UseWisej()`).
- `Default.json`: `"url": "Default.html"`, `"startup": "OrderDesk.Program.Main, OrderDesk"`, `"theme": "Bootstrap-4"`, `"debug": true`. (`"mainWindow": "OrderDesk.MainPage, OrderDesk"` is the documented alternative that opens a type directly; the samples use `startup` because it mirrors the lesson's `Application.MainPage = new MainPage();`.)
- `Program.Main(NameValueCollection args)` runs once per **browser session** and sets `Application.MainPage = new MainPage();`. No `EnableVisualStyles`, no `Application.Run`: Kestrel owns the process.
- Static files are served from the project folder (`wwwroot/x.js` → `/wwwroot/x.js`); `.json` files are never served.
- `Web.config` `<appSettings>` replaces `App.config`. `System.Configuration.ConfigurationManager` is **not** referenced — read `Web.config` with `System.Xml.Linq` (`XDocument.Load(Path.Combine(Application.StartupPath, "Web.config"))`) in a small `AppConfig` helper.

## Verified Wisej.Web facts (from Module 1 and the other course sample sets on this machine)

- **Modal dialogs do not block.** `form.ShowDialog()` returns immediately; the modal calls are `form.ShowDialog((f, result) => { … f.Dispose(); })` (callback) or `var result = await form.ShowDialogAsync();` (`async void` handler). `await MessageBox.ShowAsync(...)` is the awaitable MessageBox. Set `DialogResult = DialogResult.OK; Close();` in the dialog. **Closed dialogs are not disposed** — the caller disposes them (in the callback, or `using` around the awaited call).
- `AlertBox.Show(text, MessageBoxIcon.Information, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000)` (`Ui.Toast`) — always TopRight so it doesn't cover the button bar. `MessageBox.Show("text")` works for blocking decisions.
- Fonts: `new Font("default", 14F, FontStyle.Bold)`, `new Font("monospace", 9F)`. `Label.TextAlign` takes `System.Drawing.ContentAlignment`. `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`. `Button.ToolTipText`.
- `Wisej.Web.Timer(components)` with `Interval`, `Tick`, `Start()/Stop()`.
- Background work: `Application.StartTask(() => { …; Application.Update(this); })` keeps the session context; `Application.Update(this, () => { …change controls… })` runs the callback in context and pushes once. Check `IsDisposed` before touching controls.
- `Application.SessionId` (string), `Application.SessionCount`, `Application.Url`, `Application.Navigate(Application.Url, "_blank")` opens a **second browser session** in the same process (the multi-user test). `Application.Browser` (`Wisej.Core.ClientBrowser`: `Type, Version, OS, Device, Size, ScreenSize, UserAgent, Language`).
- `Application.Session` is a dynamic bag: `dynamic s = Application.Session; s.User = "kelly"; var u = s.User as string;` (missing member → null).
- `Application.Download(Stream, fileName)` streams a generated file to the browser (the browser saves it, no server path involved). `Application.Download(string path, fileName)` for a file on disk.
- `PdfViewer` with `PdfStream = new MemoryStream(bytes)`, `ViewerType = PdfViewerType.Auto` inside a `Form` shown with `ShowDialog(callback)` — see `Views/InvoicePreviewForm.cs`.
- `Application.LoadTheme("Material-3")` restyles live. Built-in themes in Wisej-4 4.1.0: Blue-1/2/3, Bootstrap-4, BootstrapDark-4, Classic-2, Clear-1/2/3, FluentDark-5, FluentLight-5, Graphite-3, Material-3/4, MaterialDark-4, Vista-2. A `Themes\*.mixin.theme` file (Content, copy to output, or embedded with `[assembly: WisejResources]`) is merged over the current theme.
- `DataGridView`: `Rows.Add(values…)` returns the index; `Rows[i].Tag`; `Rows[i].Cells[c].Style.ForeColor`; `MultiSelect`, `ReadOnly`, `RowHeadersVisible`, `SelectionMode = FullRowSelect`, `SelectionChanged`, `CurrentRow`. Columns: `new DataGridViewTextBoxColumn { HeaderText, Name, Width }`, `DefaultCellStyle.Format = "N2"`, `DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight`.
- Anchored controls are sized on the **server** against the browser size the client reported first. A page loaded in a 722 px wide Browser pane shrinks a right-anchored 628 px trace to 2 px, and an emulated viewport change afterwards does not always reach the server — set the viewport (≥1400×760) **before** navigating, or reload after resizing. **(verified, Module 1)**
- A hidden Browser-pane tab neither flushes the qooxdoo "appear" queue nor sends queued events until it is fronted: drive one tab at a time, front it first, wait a second, then read the page text. Buttons at the bottom of the 1348×680 page are off-screen in a small pane. **(verified)**

## API facts from Wisej.Framework.xml — compile-checked shapes **(xml)**

- **Toast** (non-blocking, alternative to AlertBox): `new Toast("Order 1042 saved.", "icon-ok") { AutoCloseDelay = 3500, Alignment = ContentAlignment.BottomRight }.Show();` props `AutoCloseDelay, Alignment, AllowHtml, AutoDispose, Text, IconSource`.
- **Session end**: `Application.ApplicationExit` (static event) fires when the session ends; `Application.SessionTimeout` fires before timeout (`HandledEventArgs`; `Handled = true` suppresses the built-in prolong dialog). Subscribe in `Program.Main` or the page constructor; unsubscribe in `Dispose`.
- **Upload**: `Wisej.Web.Upload` control — `Text` (button caption), `AllowMultipleFiles`, `AllowedFileTypes` (".csv"), `MaxFileSize`; event `Uploaded` with `UploadedEventArgs.Files` (`Wisej.Core.HttpFileCollection`; each `HttpPostedFile` has `FileName, ContentType, ContentLength, InputStream`). Also `Uploading`, `Progress`, `Error`.
- **DataGridView virtual mode**: `VirtualMode = true`, `RowCount = n`, handle `CellValueNeeded(DataGridViewCellValueEventArgs e)` (`e.RowIndex`, `e.ColumnIndex`, set `e.Value`); `BlockSize` (rows per fetch block); `CellDoubleClick`; `Sort(column, ListSortDirection)`; `ColumnHeaderMouseClick`; `DataGridViewColumn.SortMode`.
  Summary rows: `AddSummaryRows(SummaryType.Sum, SummaryRowPosition.Below, groupColumnName, summaryColumnName, style = null, customSummary = null)` (`SummaryType`: None, Sum, Count, Average, Min, Max). If a summary API misbehaves, a footer label with the computed totals is an acceptable fallback — say so in the README.
- **ErrorProvider**: `new ErrorProvider(components)`; `SetError(control, message)`, `Clear()`, `GetError(control)`. `TextBoxBase.Watermark` / `ComboBox.Watermark` (there is no `PlaceholderText`).
- **Label.AllowHtml** (bool) — text is encoded by default; `AllowHtml = true` renders markup (Module 7 shows why it must stay off for user data).
- **Responsive**: `Application.ActiveProfile` (`Wisej.Core.ClientProfile`: `Name, MinWidth, MaxWidth, MinScreenWidth, MaxScreenWidth, Landscape`), static event `Application.ResponsiveProfileChanged` (`ResponsiveProfileChangedEventArgs.CurrentProfile / PreviousProfile`), `Application.BrowserSizeChanged`, `Application.Browser.Size`. Profiles come from `ClientProfiles.json` in the project root (Content, copy to output); the embedded defaults are Phone, Phone (Landscape), Tablet, Tablet (Landscape), Small Desktop (maxWidth 1024) and the unnamed desktop. `Control.ResponsiveProfiles` holds designer-set per-profile values; the samples react in code to `ResponsiveProfileChanged` (readable and verifiable) and also offer "simulate" buttons so the layout change can be seen without resizing.
- **Eval**: `Application.Eval(js)` / `await Application.EvalAsync(expression)` — an expression, never `return …;` ("Illegal return statement"). Useful for browser `localStorage` (client-side UI preference storage in Module 4).
- **Navigation/containers**: `MenuBar` (+ `MenuItem`), `ToolBar` + `ToolBarButton`, `StatusBar` + `StatusBarPanel`, `TabControl` + `TabPage`, `SplitContainer`, `FlexLayoutPanel`, `FlowLayoutPanel`, `TableLayoutPanel`, `LinkLabel`, `ProgressBar`, `ComboBox`, `NumericUpDown`, `DateTimePicker`, `TreeView`, `ListBox`, `Canvas`.
- `Application.UserIdentity` exists (IPrincipal-style); the samples keep their own tiny `User` model in the session instead of ASP.NET authentication — say so in docs.

## UI conventions (so the seven consoles look like one product)

- `MainPage : Page`, `Size = 1348×680`, `BackColor = Ui.PageBack` (238,242,247), `Text = "OrderDesk — <module title>"`. White cards (`Panel`, `BorderStyle.Solid`, `Color.White`) with a 12–14pt bold title. The **`TracePanel`** (`Views/TracePanel.cs`) on the right, ~628 wide, anchored Top|Bottom|Left|Right: `trace.Add(TraceKind.Server, name, payload)` for business-logic calls and decisions, `TraceKind.ToClient` (state pushed to the browser), `TraceKind.FromClient` (user actions arriving), `TraceKind.Boundary` (a desktop assumption hit and replaced). Log every button click, every service call, every crossing of the browser boundary and every "this is why" finding.
- A **status label** (`Ui.SetStatus(labelStatus, "…", Ui.Ok/Warn/Error)` → "● text") and a **banner** label (`Ui.ShowBanner(labelBanner, text, Ui.BannerKind.Ok/Warn/Error)`, `Ui.HideBanner`) that explains failures in one paragraph.
- A **button bar** per card that exercises: the module's **success path**, a **progress path** (Timer or `Application.StartTask`), at least one **failure path** (the desktop assumption breaking on the server, caught and explained in the banner + trace), and **recovery** (the web-safe replacement). A **Clear** button empties the trace.
- `MainPage.Designer.cs` with `InitializeComponent()` (absolute `Location/Size`, `Anchor`, event hookups) so the page opens in the Wisej Designer; code-behind in `MainPage.cs` with `#region` per card. Read Module 1's pair before writing a new one and keep the same style.
- Always show the five walkthrough orders first (`SampleData.Orders()`, newest first): 1042 Northwind Traders 4,820.00 Open · 1041 Contoso Ltd 1,290.50 Shipped · 1040 Fabrikam Inc 760.00 Open · 1039 Adventure Works 12,400.00 Invoiced · 1038 Globex Corp 3,090.00 Hold. Users in the videos: **kelly** (Acme, session A, customer Northwind Traders, filter Open) and **sam** (Globex, session B, customer Fabrikam Inc, filter Invoiced); order owners dana, priya, sam, kelly.

## Shared domain (`Domain/`) — reuse, don't rewrite

`OrderService` (`GetOrders()` newest first, `Find(id)`, `Search(OrderFilter { Text, Status })`, `Save(order)` (computes `Total`, assigns `Id`), `Delete(id)`, `CalculateOrderTotal`, `DiscountFor`, `Tax`, `TaxRate`), `CustomerService` (`GetCustomers`, `Find(id)`, `FindByName`), `InvoiceDocument.Build(order, service)` → text lines, `SampleData` (8 customers, 5 orders), `IOrderRepository` + `InMemoryOrderRepository.Shared` (process-wide, locked — one store for every session, exactly like a database). A module that needs paging/sorting/validation **adds** to the Domain (`OrderQuery { Skip, Take, SortBy, Descending }`, `OrderValidator` → `ValidationResult`) without breaking the existing API; a module that needs a big dataset creates a private `InMemoryOrderRepository(seed)` (e.g. 200,000 generated orders) instead of touching `Shared`.

## Docs (`docs/`)

One Markdown file per lab deliverable, named as the course names it, each with an **Evidence** section (what the running app shows, button by button) and the **self-check answers** from the lab guide repeated in the README. Every module keeps `docs/migration-log.md` — the running log the videos keep opening — with one dated line per accepted workaround/decision, carried forward from the previous module and extended.

## Verified at runtime in Modules 4 and 6 (2026-09-10)

- `Application.Session` as a dynamic bag holds a typed object per browser session (`session.UserContext as UserSessionContext`); a second tab is a second session with its own bag, while a `static` field is one slot for the whole process — the corruption is reproducible with two Browser-pane tabs. **(verified, Module 4)**
- `Application.SessionTimeout` / `Application.ApplicationExit` are static events: subscribe in the page constructor, unsubscribe in `Dispose`. **(verified subscribe/unsubscribe; the real timeout was simulated with a Timer)**
- `Application.Eval("localStorage.setItem(…)")` + `await Application.EvalAsync("localStorage.getItem(…)")` round-trips a browser preference; the awaited value comes back as the string. **(verified, Module 4)**
- `Microsoft.Win32.Registry` writes succeed on Windows under `dotnet run` — into HKCU of the account running Kestrel (`Environment.UserName`), which is the point of the demo; on Linux it throws `PlatformNotSupportedException`. **(verified on Windows, Module 4)**
- `System.Text.Json` per-user profile files under `App_Data/profiles` and per-session temp folders under `App_Data/tmp` work with `Application.StartupPath` as the root. `App_Data/` is in every module's `.gitignore`. **(verified, Module 4)**
- `Wisej.Web.Upload` with `AllowedFileTypes = ".csv"` / `MaxFileSize`: `Uploaded` gives `e.Files[i].FileName / ContentLength / ContentType / InputStream`; `Error` gives `e.ErrorType / e.FileNames / e.Message`. The Browser pane cannot drive the native file chooser, so the upload path is verified by code review + the sample CSV; test it by hand. **(Module 6)**
- A hand-written .xlsx (`ZipArchive` + `[Content_Types].xml`, `_rels/.rels`, `xl/workbook.xml`, `xl/_rels/workbook.xml.rels`, `xl/worksheets/sheet1.xml` with `inlineStr` cells) opens as a valid package; `Application.Download(path, "Orders.xlsx")` serves it from `App_Data/exports`. **(verified, Module 6)**
- `PdfViewer.PdfStream = new MemoryStream(bytes)` inside a modal `Form` streams the PDF through `postback.wx` (content-type application/pdf); the embedded pane renders it black but the bytes are a valid PDF (`pdftotext` reads them). `ShowDialog((form, result) => form.Dispose())` closes and disposes. **(verified, Modules 1 and 6)**
- A process-wide queue (`static` list under a lock, one `Task.Run` worker, `CancellationTokenSource` per job) polled by a `Wisej.Web.Timer` every second shows Queued → Running → Done/Cancelled to every session; the worker never touches controls. **(verified, Module 6)**
- `Type.GetTypeFromProgID("Excel.Application")` is a safe probe for "Office is installed here" without creating the COM object. **(verified, Module 6)**
- Driving a console from the Browser pane: find a widget by `getName()` in `qx.core.ObjectRegistry.getRegistry()` and call `execute()` (buttons) or `setValue(true)` (radio buttons); after opening a modal `PdfViewer` window, clicks on coordinates land in the PDF frame — press `buttonClose` by name instead. **(verified)**

## Verified at runtime in Modules 2 and 5 (2026-09-10)

- A ported `Form` shown non-modally from a `Page` (`form.Show()`) floats over the page as a window; closing it through the title bar raises `FormClosing`/`FormClosed` on the server (dispose there). A client-side `window.close()` from script does NOT reach the server — always close through the real title-bar button when testing. **(verified, Module 2)**
- `Application.Configuration.StartUp / MainWindow / ThemeName / Url` read Default.json; `Application.ServerName / ServerPort / StartupPath` are usable in a trace line. `Application.Platform.ToString()` is just the type name — do not print it. **(verified, Module 2)**
- Ported WinForms designer files often have no `.Name` on buttons; the Wisej.Web control still works, but the registry-by-name trick needs `getLabel()` instead. Give every control a Name in new designers. **(verified, Module 2)**
- `DataGridView.VirtualMode = true` + `RowCount = n` + `CellValueNeeded` behave like WinForms: the client asks for visible rows only, so a 50-row block cache over a server-side filtered/sorted/memoized result set serves 80,318 rows at about 7 KB per block. Set `column.SortMode = DataGridViewColumnSortMode.NotSortable` on a virtual grid and sort in the query instead. Binding 20,000 rows through `Rows.Add` in a loop works but costs seconds and megabytes — never bind all 200,000. **(verified, Module 5)**
- `TextBox.Watermark` / `ComboBox.Watermark` exist (no PlaceholderText). `ErrorProvider.SetError(control, message)` shows the field message next to the control inside a modal Form; `Clear()` between attempts. **(verified, Module 5)**
- A `Wisej.Web.Timer` driving a batch (200 items per tick) keeps the UI responsive and is the simplest progress path. **(verified, Module 5)**
