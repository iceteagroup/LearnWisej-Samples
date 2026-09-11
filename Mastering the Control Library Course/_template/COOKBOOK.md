# Operations Console cookbook (Mastering the Control Library · Wisej-4 4.1.0 · .NET 10)

The conventions every module sample of this course follows. Facts marked **(verified)** were executed in
the browser while building these samples or the Application Integration / Foundations samples (same
framework build). Facts marked **(from XML docs)** come from `Wisej.Framework.xml` 4.1.0 and have not been
executed yet: implement them, make sure `dotnet build` passes, and say so in your final report so the
reviewer can check them at runtime.

## The one solution that grows across the course

The course is cumulative: the lab of every module opens the `OperationsConsole` solution of the previous
module. Each `Module N` folder therefore contains the **complete solution as it stands after module N**
(everything earlier modules built, plus this module's work), so a learner can open any module folder and
run it. Never delete earlier modules' work; only replace the placeholder your module is about.

```
Mastering the Control Library Course/
  README.md                          course-level index (one row per module, how to run)
  _template/                         this scaffold + COOKBOOK.md
  Module N/
    OperationsConsole.slnx           (from _template)
    .gitignore                       (from _template)
    README.md                        what it shows, how to run, what to click, lab steps → code map, self-check answers
    OperationsConsole/
      OperationsConsole.csproj       (from _template; TargetFrameworks = net10.0-windows;net10.0 — keep it)
      Program.cs  Startup.cs  Default.html  Default.json  Web.config  ClientProfiles.json
      Properties/launchSettings.json (port: Module N → http://localhost:570N, i.e. 5701 … 5707)
      MainPage.cs + MainPage.Designer.cs      the Operations Console shell (a Wisej.Web.Page)
      Shell/                          IConsoleShell + ConsoleLog + ISection (Module 1), RecordHeader / StatusStrip UserControls (Module 3+)
      Sections/                       one UserControl per section: EditorsPage, LayoutsPage, ListsTreesPage,
                                      DataGridViewPage, DashboardPage, WidgetsPage (placeholders in Module 1)
      Editors/  ListsTrees/  Orders/  Dashboard/  Widgets/   the module-specific controls (CustomerEditor, DocumentDetailControl …)
      Models/  Services/              view models and in-memory services (CustomerService, DocumentService, OrderService …)
      wwwroot/                        static files (rating.js / rating.css, sample PDF …) served at /wwwroot/...
      docs/                           the lab deliverables as Markdown (ControlSelection.md, LayoutNotes.md, …)
```

- Namespace is always `OperationsConsole` (the lab tells the learner to name the solution that way).
- `Program.Main(NameValueCollection args)` is the session entry point (`Default.json` → `"startup"`) and does
  `Application.MainPage = new MainPage();`.
- Run: `dotnet run -f net10.0 --urls http://localhost:570N` from the project folder (5701 … 5707; 5091-5097 and 5601-5607 belong to other courses). The static file server serves the
  **project folder**, so a Widget package `Source = "wwwroot/rating.js"` is fetched as `/wwwroot/rating.js`.
  `Default.json` / `Web.config` / `ClientProfiles.json` are never served (`Startup.cs` skips `.json`).
- Build with `dotnet build -nologo -v q` and fix every error. Warning CS7022 is already silenced in the csproj.
- Do not run the app yourself and do not start servers; the reviewer runs it in the browser.
- When a module starts from the previous module's folder, copy the whole `Module N-1` folder (without `bin/`, `obj/`,
  `.vs/`) and then change the port in `Properties/launchSettings.json` and the module number in README / csproj comment.

## Verified runtime facts

- **Dock order (verified in Module 1):** docking is applied from the LAST `Controls.Add` to the FIRST. The designer serialises the reverse z-order, so the
  `Fill` control must be the **first** `Controls.Add(...)` and the edge bars (Top / Bottom) the **last** ones: `Controls.Add(contentPanel); Controls.Add(pnlEventLog); Controls.Add(navigationPanel); Controls.Add(statusPanel); Controls.Add(commandPanel);`
  gives Top / Bottom bars spanning the full width, Left / Right cards between them, content in the middle. Inside a panel the same rule holds
  (`Controls.Add(statusLabel /*Fill*/); Controls.Add(diagnosticPanel /*Right*/);`). Adding the bars first makes the Left/Right panels claim the full height and a Right-docked child overlaps a Fill sibling.
- **`Button.AccessibleRole` does not exist** in Wisej.NET 4.1 (`AccessibleName` and `AccessibleDescription` do) — use `AccessibleName` only.
- **Testing a CheckBox from the browser pane:** `widget.setValue(true)` changes the client only (no `CheckedChanged` on the server); use `widget.execute()` (toggles and posts) or a real click.
  Buttons: `qx.core.ObjectRegistry` → `wisej.web.Button` by `getName()` → `.execute()`. A hidden/scaled pane may need `qx.ui.core.queue.Manager.flush()` before the first screenshot.
- **Page lifecycle:** the browser keeps the Wisej session across a reload (F5 shows the same Event log); restart the server to start clean.
- `Navigate()` disposes the page it removes (`previous?.Dispose()` after `contentPanel.Controls.Clear()`): removed pages are server objects and would otherwise stay alive for the session.

- **Responsive profiles (verified in this template):** `ClientProfiles.json` in the project root (csproj:
  `<Content Update="ClientProfiles.json"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></Content>`) defines
  `Phone` (≤ 600 px), `Tablet` (601–1024 px), `Desktop` (≥ 1025 px). `Application.ActiveProfile.Name` returns the
  matching name, `Application.ResponsiveProfileChanged += (s, e) => …` fires when the browser is resized across a
  boundary (`ResponsiveProfileChangedEventArgs`), `Application.Browser.Size` is the current browser size and
  `Application.BrowserSizeChanged` fires on every resize. Profiles must be defined in the JSON file, not in `Default.json`.
- **Toast (verified):** `new Toast("Saved.", "icon-info") { AutoCloseDelay = 3000, Alignment = ContentAlignment.TopRight }.Show();`
  Icons (verified: `icon-ok` does NOT exist → 404): `icon-info`, `icon-warning`, `icon-error`, `icon-check`, `icon-question`, `icon-alert`, `icon-save`, `icon-refresh`, `icon-new`, `icon-upload`, `icon-search`, `icon-settings`, `icon-help`, `icon-file`, `icon-folder`, `icon-*-outlined` — or an image URL.
- **AlertBox (verified):** `AlertBox.Show(text, MessageBoxIcon.Warning, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000)`.
  Always TopRight so it never covers buttons.
- **MessageBox (verified in Foundations):** `await MessageBox.ShowAsync("Discard changes?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)`
  in an `async void` handler; fire-and-forget `MessageBox.Show(text, caption, buttons, icon)`. Reserve it for real confirmations.
- `Wisej.Web.Timer(components)` with `Interval`, `Tick`, `Start()/Stop()`, `Enabled` — a Component with no visual surface.
- `Application.StartTask(() => { …change controls…; Application.Update(this); })` runs work on a background thread;
  `Application.Update(component)` pushes pending changes over WebSocket from a non-request thread. Bound the frequency.
- `async void` event handlers with `await Task.Delay(…)` work; after an `await` push progress with `Application.Update(this)`.
- Fonts: `new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold)`, monospace `new System.Drawing.Font("monospace", 9F)`.
  `Label.TextAlign` uses `System.Drawing.ContentAlignment`. `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`.
- `Application.LoadTheme("Material-3")` restyles the running app live (built-in: Blue-1/2/3, Bootstrap-4, BootstrapDark-4,
  Classic-2, Clear-1/2/3, FluentDark-5, FluentLight-5, Graphite-3, Material-3/4, MaterialDark-4, Vista-2).
- **Widget (verified in the Application Integration samples):** `Packages` (`List<Widget.Package>{Name, Source}`, loaded in list order,
  stylesheets are packages too), `InitScript` (string; embed the .js as a resource and read it with
  `GetResourceString("OperationsConsole.wwwroot.rating.js")` or set it from a file at startup), `Options` (assign an anonymous
  object — property names are **camel-cased** on the client), `WiredEvents = new[] { "ratingChanged" }`,
  `WidgetEvent += (s, e) => …` with `e.Type` and `dynamic e.Data`. `widget.Update()` after changing `Options` runs
  `this.update(options, old)` in the browser; `widget.Call("setSaved", value)` runs `this.setSaved(value)` on the client
  wrapper (one-way); `var r = await widget.CallAsync("getState")` returns the function's return value as a dynamic;
  `await widget.EvalAsync("this.widget.value")` evaluates an **expression** (never `return …;`).
  `this` in the InitScript is the wrapper, `this.container` is the DOM element the framework owns: create a child element
  inside it and keep the vendor object in `this.widget`. `fireWidgetEvent` from a **user-triggered** DOM callback (click)
  reaches the server fine (`var me = this` captured in `init`); a `fireWidgetEvent` raised synchronously *during*
  `update()` is dropped — defer with `setTimeout(…, 0)`. Never define `getWidth`, `getHeight`, `destroy`, `resize`,
  `show`, `hide`, `getValue`, `setValue` on the wrapper (`this`) — they replace qooxdoo methods; prefix your own
  (`setSaved`, `ratingGetState`). Wrap `dispose`, never replace it.

## Signatures from the XML docs (Wisej.Framework.xml 4.1.0) — **(from XML docs, verify and report)**

```csharp
// ---- Module 1: Page + docked areas -------------------------------------------------------------
navigationPanel.Dock = DockStyle.Left; commandPanel.Dock = DockStyle.Top; statusPanel.Dock = DockStyle.Bottom;
contentPanel.Dock = DockStyle.Fill;                 // contentPanel is the FIRST Controls.Add (see Verified runtime facts: dock order)
contentPanel.Controls.Clear(); content.Dock = DockStyle.Fill; contentPanel.Controls.Add(content);
button.TabIndex = 1; button.AccessibleName = "Open the Editors section";   // (no AccessibleRole in Wisej 4.1)

// ---- Module 2: editors and validation ----------------------------------------------------------
numCreditLimit.Minimum = 0; numCreditLimit.Maximum = 250000; numCreditLimit.DecimalPlaces = 0; numCreditLimit.Increment = 1000;
dtpStartDate.MinDate = new DateTime(2000, 1, 1); dtpStartDate.MaxDate = DateTime.Today.AddYears(1); dtpStartDate.Format = DateTimePickerFormat.Short;
txtName.MaxLength = 80; txtEmail.CharacterCasing = CharacterCasing.Lower;
cboStatus.DropDownStyle = ComboBoxStyle.DropDownList; cboStatus.DisplayMember = "Text"; cboStatus.ValueMember = "Key"; cboStatus.DataSource = list; // key ≠ display text
toolTip.SetToolTip(txtEmail, "name@company.com");   // Wisej.Web.ToolTip (extender); HelpTip works the same way (helpTip.SetHelpTip(control, text))
errorProvider.SetError(txtEmail, "Enter an email like name@company.com"); errorProvider.SetError(txtEmail, "");   // "" clears
txtEmail.Validating += (s, e) => { if (!ValidateEmail()) e.Cancel = false; };   // never block focus — set the error and let the user move on
txtName.Focus();
btnSave.Enabled = false; btnSave.ShowLoader = true;  // busy state; ProgressBar { Style = ProgressBarStyle.Marquee } is the alternative
control.CausesValidation = false;                    // on Reset so a half-typed value does not re-validate on the way out

// ---- Module 3: containers, ToolBar, StatusBar, SplitContainer, TabControl ----------------------
toolBar.Dock = DockStyle.Top; statusBar.Dock = DockStyle.Bottom; splitMain.Dock = DockStyle.Fill;   // add in exactly that child order
toolBar.Buttons.Add(new ToolBarButton { Name = "btnRefresh", Text = "Refresh", ImageSource = "icon-refresh" });
toolBar.ButtonClick += (s, e) => { switch (e.Button.Name) { case "btnRefresh": RefreshCurrentSection(); break; } };   // ToolBarButtonClickEventArgs.Button
statusBar.Panels.Add(new StatusBarPanel { Name = "pnlProfile", Text = "Desktop", AutoSize = StatusBarPanelAutoSize.Contents, MinWidth = 80 });
statusBar.Panels["pnlRows"].Text = "42 rows";        // (index by name or position)
splitMain.Orientation = Orientation.Vertical; splitMain.SplitterDistance = 240; splitMain.Panel1MinSize = 160;
splitMain.Panel1Collapsed = narrow;                  // narrow profile: hide navigation, show btnMobileMenu
tabDetail.Dock = DockStyle.Fill; tabDetail.TabPages.Add(new TabPage { Name = "tabEditors", Text = "Editors" });
tabDetail.SelectedIndexChanged += …; tabDetail.SelectedTab.Name
contextMenu.MenuItems.Add(new MenuItem("Refresh", (s, e) => RefreshCurrentSection()));  navList.ContextMenu = contextMenu;
// FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = true }, TableLayoutPanel { ColumnCount, RowCount, ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)) }
// Anchor: control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
// UserControl: public string Title { get => lblTitle.Text; set => lblTitle.Text = value; }  public event EventHandler RefreshRequested;

// ---- Module 4: TreeView lazy loading + ListView virtual mode ----------------------------------
var node = new TreeNode("Contracts") { Name = "cat-7", Tag = 7, ImageKey = "folder" }; node.Nodes.Add(new TreeNode("…") { Name = "placeholder" });
categoryTree.ImageList = imageList; imageList.Images.Add("folder", Image);   // keys: folder, contract, invoice, drawing, warning
categoryTree.AfterExpand += (s, e) => { /* e.Node (TreeViewEventArgs) */ if (IsPlaceholder(e.Node)) { e.Node.Nodes.Clear(); foreach (var c in service.GetChildren((int)e.Node.Tag)) e.Node.Nodes.Add(…); } };
categoryTree.AfterSelect += (s, e) => LoadDocuments((int)e.Node.Tag);
documentList.View = View.Details; documentList.Columns.Add("Name", 260); documentList.Columns.Add("Type", 100); documentList.Columns.Add("Size", 80);
documentList.VirtualMode = true; documentList.VirtualListSize = page.Total;           // set the size after the page is cached
documentList.RetrieveVirtualItem += (s, e) => { /* e.ItemIndex → e.Item = new ListViewItem(new[] { … }) { Tag = id, ImageKey = "invoice" } */ };
documentList.CacheVirtualItems += (s, e) => { /* e.StartIndex … e.EndIndex: prefetch the page */ };
documentList.SmallImageList = imageList; documentList.FullRowSelect = true; documentList.MultiSelect = false;
documentList.SelectedIndexChanged += …;  documentList.SelectedIndices[0]   // in virtual mode resolve the id from the cached page, not from Items
listView.ShowLoader = true;  // loading state on any control

// ---- Module 5: DataGridView ----------------------------------------------------------------
ordersGrid.AutoGenerateColumns = false;
ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTotal", DataPropertyName = "Total", HeaderText = "Total", Width = 100,
    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }, ReadOnly = true });
ordersGrid.Columns["colStatus"].AllowHtml = true;       // DataGridViewColumn.AllowHtml (also on DataGridViewCellStyle)
ordersGrid.CellFormatting += (s, e) => { if (e.ColumnIndex == colStatus.Index && e.Value != null) { e.Value = "<span class='badge'>" + WebUtility.HtmlEncode(e.Value.ToString()) + "</span>"; e.FormattingApplied = true; } };
ordersSource.DataSource = rows; ordersGrid.DataSource = ordersSource;   // Wisej.Web.BindingSource
ordersGrid.Columns["colDueDate"].Editor = new MonthCalendar();          // DataGridViewColumn.Editor = any control used to edit the cells (or use DataGridViewDateTimePickerColumn)
ordersGrid.CellBeginEdit / CellEndEdit / CellValidating (DataGridViewCellValidatingEventArgs: Cancel, FormattedValue)
var cmd = new DataGridViewButtonColumn { Name = "colOpen", HeaderText = "", Text = "Open", UseColumnTextForButtonValue = true, Width = 80 };
ordersGrid.CellContentClick += (s, e) => { if (e.ColumnIndex == cmd.Index && e.RowIndex >= 0) OpenOrder(RowIdAt(e.RowIndex)); };
// virtual mode:
ordersGrid.VirtualMode = true; ordersGrid.RowCount = service.Count();
ordersGrid.CellValueNeeded += (s, e) => { e.Value = cache.GetValue(e.RowIndex, e.ColumnIndex); };   // DataGridViewCellValueEventArgs
ordersGrid.DataRead += (s, e) => { cache.Prefetch(e.FirstIndex, e.LastIndex); };                    // DataGridViewDataReadEventArgs
ordersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; ordersGrid.MultiSelect = false; ordersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
ordersGrid.ShowLoader = true;  filterPanel.BringToFront();  statusStrip.SendToBack();

// ---- Module 6: charts, ProgressBar, PdfViewer, HtmlPanel, Upload ------------------------------
// ChartJS is a separate NuGet package: <PackageReference Include="Wisej-4-ChartJS" Version="4.1.2" />   (nuget.org, reachable)
// namespace Wisej.Web.Ext.ChartJS: new ChartJS { ChartType = ChartType.Line, Labels = new[] { "Jan", … } };
// chart.DataSets.Add(new LineDataSet { Label = "Opened", Data = new object[] { 12, 15, … }, BorderColor = Color.…, Fill = false });
// chart.Options.Scales / chart.Options.Legend …  — read the package's XML docs after restore (C:\Users\matte\.nuget\packages\wisej-4-chartjs\4.1.2\lib\...\Wisej.Web.Ext.ChartJS.xml) and report what you used.
progressCompletion.Minimum = 0; progressCompletion.Maximum = 100; progressCompletion.Value = model.CompletionPercent;
pdfPreview.PdfSource = "wwwroot/sample-report.pdf"; pdfPreview.ViewerType = PdfViewerType.Auto;   // Mozilla | Google | Custom; ship a small real PDF in wwwroot
htmlPreview.Html = "<h3>…</h3>";  htmlPreview.HtmlSource = url;   // constrain what you embed (encode user text)
upload.AllowedFileTypes = ".pdf,.png,.jpg"; upload.MaxFileSize = 2 * 1024 * 1024; upload.AllowMultipleFiles = false;
upload.Uploaded += (s, e) => { foreach (HttpPostedFile f in e.Files) { f.FileName; f.ContentLength; f.InputStream; } };   // UploadedEventArgs.Files is Wisej.Core.HttpFileCollection
upload.Error += (s, e) => { e.ErrorType; e.Message; e.FileNames; e.FileSizes; };   // client-side rejections (type / size)
// GoogleMaps is an extension package that needs an API key: reserve a placeholder Panel with a comment instead.

// ---- Module 7: Widget ---------------------------------------------------------------------------
ratingWidget.Packages.Add(new Widget.Package { Name = "rating-css", Source = "wwwroot/rating.css" });
ratingWidget.Packages.Add(new Widget.Package { Name = "rating-js", Source = "wwwroot/rating.js" });
ratingWidget.InitScript = File.ReadAllText(Application.MapPath("wwwroot/rating-init.js"));   // or embedded resource + GetResourceString
ratingWidget.WiredEvents = new[] { "ratingChanged" };
ratingWidget.WidgetEvent += ratingWidget_WidgetEvent;   // e.Type == "ratingChanged", dynamic e.Data → Convert.ToInt32(e.Data.value)
ratingWidget.Call("setSaved", value);  await ratingWidget.CallAsync("getState");
// Theming: Wisej.Web.StyleSheet extender (styleSheet.SetStyle / CSS text) or packaged rating.css with theme-like variables; never hard-code colours in rating.js.
ratingWidget.AccessibleName = "Customer satisfaction rating";
```

## The shell contract (defined in Module 1, kept by every later module)

`MainPage` owns the frame. In Module 1–2 that is four docked panels (`commandPanel` Top, `statusPanel` Bottom with
`statusLabel` and the stretch-goal diagnostic labels, `navigationPanel` Left, `contentPanel` Fill, added first in the
serialised list). From Module 3 the frame is `ToolBar` (Top) → `StatusBar` (Bottom) → `SplitContainer splitMain` (Fill)
with the navigation list in `Panel1` and `TabControl tabDetail` in `Panel2`. There is no event-log card: the samples
show only what the lab and the video build. Sections never talk to the shell directly and never to each other — they
go through small types in `Shell/`:

```csharp
// Shell/IConsoleShell.cs — implemented by MainPage (per session: Application.MainPage is this session's page)
public enum StatusLevel { Ok, Warning, Error }
public interface IConsoleShell
{
    void SetStatus(string text, StatusLevel level);       // statusLabel (Module 1–2) / StatusBar panel (Module 3+), coloured by level
    void SetSelectedControl(string controlName);          // diagnostic panel (stretch goal): the control used last
    void SetSelectedRecord(string recordId);              // diagnostic panel: stable ID of the selected record (null/"" → "—")
}

// Shell/ShellStatus.cs — what sections, editors and services call
public static class ShellStatus
{
    static IConsoleShell Shell => Application.MainPage as IConsoleShell;
    public static void Show(string text, StatusLevel level = StatusLevel.Ok) => Shell?.SetStatus(text, level);
    public static void Control(string controlName) => Shell?.SetSelectedControl(controlName);   // e.g. ShellStatus.Control(btnSave.Name)
    public static void Record(string recordId) => Shell?.SetSelectedRecord(recordId);           // e.g. ShellStatus.Record("DOC-000312")
}

// Shell/ISection.cs — implemented by every Sections/*Page UserControl
public interface ISection
{
    string Title { get; }                                 // "Editors", "Layouts", "Lists and Trees", "DataGridView", "Dashboard", "Widgets"
    void RefreshSection();                                // the shell's Refresh command (button / ToolBar / ContextMenu) calls this on the visible section
}
```

Every section is a `UserControl` in `Sections/` (`EditorsPage`, `LayoutsPage`, `ListsTreesPage`, `DataGridViewPage`,
`DashboardPage`, `WidgetsPage`) implementing `ISection`. A module replaces **only** the body of its own section page
and adds its own folders (e.g. Module 5 rebuilds `Sections/DataGridViewPage` and adds `Orders/`, `Services/OrderService.cs`,
`Models/OrderRow.cs`); it never edits another section, `Shell/` or — except Module 3, which rebuilds the frame — `MainPage`.

## UI conventions used by every sample (so the samples feel like one course)

- Light grey page background `Color.FromArgb(238,242,247)`, white cards (`Panel`, `BorderStyle.Solid`), card titles
  `"default" 12F Bold`.
- **No teaching chrome.** A screen shows only what the module's lab guide and walkthrough video build: no caption
  labels that narrate the lesson, no hint labels naming handlers, no "try it" demo buttons, no event-log card, no
  ToolTipText explaining code. Module 7's message trace is kept because the video shows it.
- A status area that changes text and colour: green `Color.FromArgb(31,157,87)` ok, amber `Color.FromArgb(232,161,60)`
  warning, red `Color.FromArgb(224,86,59)` error. In Module 1–2 it is `statusLabel` in `statusPanel`; from Module 3 it is the `StatusBar`.
- The lab's "Show every path" step lists the paths each module must show. Success and validation paths are reached
  through the normal UI; a service failure the lab asks for is reproduced with one "Simulate service failure"
  CheckBox on the page (the service's `SimulateFailure` flag) — nothing more.
- Control names exactly as the lab guide asks (`navigationPanel`, `contentPanel`, `statusLabel`, `editorsButton`,
  `txtName`, `cboStatus`, `dtpStartDate`, `numCreditLimit`, `btnSave`, `splitMain`, `tabDetail`, `btnRefresh`,
  `categoryTree`, `documentList`, `ordersGrid`, `ordersSource`, `chartTickets`, `progressCompletion`, `pdfPreview`,
  `ratingWidget`, …). Never `button1`.
- Designer-style `<Name>.Designer.cs` with `InitializeComponent()` so the file opens in the Wisej Designer; code-behind
  in `<Name>.cs`. Short, readable event handlers that call named command methods / services — that is the point of the course.
- Business rules and data live in `Services/` (`CustomerService`, `DocumentService`, `OrderService`, `DashboardService`,
  `RatingService`) with a `public bool SimulateFailure { get; set; }` so the failure path is reproducible; models in `Models/`.
- Sample data is generated in memory (a few hundred documents, a few thousand order rows for the virtual path); no database.

## Docs (`docs/`)

Write each lab deliverable as its own Markdown file named after what the lab asks for (`ControlSelection.md`,
`EditorDecisions.md`, `LayoutNotes.md`, `ExplorerNotes.md`, `GridDecisions.md`, `DashboardNotes.md`,
`WidgetContract.md`, `CapstoneNotes.md`, `DemoScript.md`, …). Include a short **Evidence** section describing what
the running app shows for each path. The README carries a **Lab steps → where in the code** table mapping every
lab-guide step to a file/method, plus a **Self-check** section answering the questions the lab / exam guide asks.

## Verified while reviewing the finished modules (2026-09-10)

- **`Application.StartTask` exceptions (verified in Module 6):** an exception that escapes the task lambda is shown by
  Wisej.NET in its red "Application Error" dialog with the stack trace, even when the awaiting handler catches the
  Task's exception. Catch inside the lambda, return a `(Result, Error)` tuple, and re-throw on the request thread.
- **`Toast` icon names:** `icon-ok` does not exist (404). Valid theme icons include `icon-check`, `icon-info`, `icon-warning`,
  `icon-error`, `icon-question`, `icon-alert`, `icon-save`, `icon-refresh`, `icon-new`, `icon-upload`, `icon-search`,
  `icon-settings`, `icon-help`, `icon-file`, `icon-folder`, `icon-*-outlined`.
- **`Widget.GetResourceString` is `protected`** — from a page holding a plain `Widget`, read the embedded resource with
  `Assembly.GetManifestResourceStream` (Module 7 `Widgets/RatingInitScript.cs`).
- **`DataGridView.CellContentClick` does not exist** in Wisej.NET 4.1; use `CellClick` (`e.RowIndex` / `e.ColumnIndex`).
  `DataGridView.UpdateCellValue` and `CurrentVirtualRow` are not public either.
- **`StatusBarPanel` has no `ForeColor`**: colour a panel with `AllowHtml = true` and an HTML-encoded `<span style='color:…'>`.
- **`Wisej.Core.HttpFileCollection`** (`UploadedEventArgs.Files`) is not enumerable: use `e.Files.Count` / `e.Files.Get(i)`.
- **ChartJS**: `Wisej-4-ChartJS` 4.1.2 requires `Wisej-4 ≥ 4.1.2` (NU1605 downgrade error with 4.1.0); 4.1.0 has the same public API.
  `Options`, `Scales`, `Ticks`, `GridLines`, `ScaleLabel`, `Title`, `Legend` are auto-created, but `Scales.xAxes` / `yAxes`
  are null until you assign arrays. The client widget class is `wisej.web.Widget`.
- **Driving the app from the browser pane:** two widgets can share a label (every section page has a "Simulate service
  failure" CheckBox), so look controls up by `getName()` inside the right page or click them by coordinates; after a
  responsive-profile change the layout shifts (navigation expands), so take a fresh screenshot before clicking.
- **Placeholder detection in the cumulative folders:** `MainPage` carries `BuiltThroughModule = N`; sections whose
  catalog module number is ≤ N are real, the rest still show "still a placeholder".
