# Theming & Responsive Layouts cookbook (Wisej-4 4.1.0, .NET 10)

The conventions every module sample of this course follows. Facts marked **(verified)** were executed in
the browser while building these samples or the Application Integration / Foundations samples (same
framework build). Facts marked **(docs)** come from the Wisej.NET XML docs / reflection over
`Wisej.Framework.dll` 4.1.0 and the embedded theme + profile resources; implement them, make sure
`dotnet build` passes, and say so in your report so the reviewer checks them at runtime.

The course project is the **Adaptive Operations Console** (project name `AdaptiveOps`): a branded business
dashboard with a navigation rail, a toolbar, metric cards, a ticket grid, a details editor and validation
feedback that adapts across desktop, tablet and phone client profiles. Module 1 builds the shell; every
later module starts from the previous stage of the same console and adds its own topic.

## Project layout (copy `_template`)

```
Module N/
  AdaptiveOps.slnx                    (from _template)
  .gitignore                          (from _template)
  README.md                           (what it shows, how to run, what to click, lab-step → code map, self-check answers)
  AdaptiveOps/
    AdaptiveOps.csproj                (from _template; TargetFrameworks = net10.0-windows;net10.0 — keep it)
    Program.cs  Startup.cs  Default.html  Default.json  Web.config
    Properties/launchSettings.json    (port: Module N → http://localhost:550N, i.e. 5501 … 5507)
    MainPage.cs + MainPage.Designer.cs   the console shell (Wisej.Web.Page, Application.MainPage)
    Shell/  Views/  Dialogs/  Models/  as the module needs them (UserControls for the regions)
    Themes/AdaptiveOps.theme          custom theme (Module 2 onwards) + *.mixin.theme files
    Styles/AdaptiveOps.css            scoped application stylesheet (Module 3 onwards)
    ClientProfiles.json               custom client profiles (Module 6 onwards)
    docs/                             the lab deliverables as Markdown
```

- Namespace is always `AdaptiveOps`. `Program.Main(NameValueCollection args)` does `Application.MainPage = new MainPage();`.
- Run: `dotnet run -f net10.0 --urls http://localhost:550N` from the project folder. The static file server serves
  the **project folder**, so `Styles/AdaptiveOps.css` is fetched as `/Styles/AdaptiveOps.css`. `*.json` files are never served.
- Build with `dotnet build -nologo -v q` and fix every error. CS7022 is already silenced in the csproj.
- Do not run the app yourself and do not start servers; the reviewer runs it in the browser.
- `Default.html` already carries a mobile `viewport` meta tag so phone profiles report real CSS widths.

## Verified runtime facts (from the other course samples, same framework build)

- `Wisej.Web.Timer(components)` with `Interval`, `Tick`, `Start()/Stop()`, `Enabled`.
- `Application.StartTask(() => { …; Application.Update(this); })` pushes changes from a background thread. Check `IsDisposed`.
- `AlertBox.Show(text, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000)` — always TopRight.
- Fonts: `new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold)`, monospace `new System.Drawing.Font("monospace", 9F)`.
  `Label.TextAlign` uses `System.Drawing.ContentAlignment`. `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`.
- `Application.LoadTheme("Material-3")` restyles the running app live **for every session** (it swaps the global theme).
  Embedded themes in Wisej-4 4.1.0: Blue-1, Blue-2, Blue-3, Bootstrap-4, BootstrapDark-4, Classic-2, Clear-1/2/3,
  FluentDark-5, FluentLight-5, Graphite-3, Material-3, Material-4, MaterialDark-4, Vista-2.
- A `Themes/<name>.mixin.theme` JSON file in the project folder is merged into the active theme (verified in the
  Integration course, Module 5). Mixin shape: `{"name":"x","appearances":{"key":{"states":{"default":{"styles":{…},"properties":{…}}}}}}`.
- `Form.ShowDialog()` is non-blocking; use `await form.ShowDialogAsync()` in an `async void` handler or the callback overload
  `form.ShowDialog((f, result) => { … })`. `MessageBox.ShowAsync(...)` likewise.
- Hidden browser tabs never flush the qooxdoo "appear" queue; give a fresh page a few seconds before driving it.

## Theme JSON — the real structure **(docs + embedded Bootstrap-4.theme, dumped from the assembly)**

A theme file is JSON with these top-level keys: `name`, `settings`, `fonts`, `images`, `colors`, `appearances`, `stylesheet`.

```json
{
  "name": "AdaptiveOps",
  "inherit": "Bootstrap-4",
  "settings": { "borderRadius": 6, "focusBlurRadius": 0, "focusBorderSize": 3 },
  "colors": {
    "brandPrimary": "#2454A6", "brandAccent": "#F59E0B", "surface": "#FFFFFF", "surfaceAlt": "#F6F7FB",
    "textMain": "#222222", "textMuted": "#677085", "danger": "#B42318", "warning": "#B54708",
    "success": "#027A48", "focusFrame": "#2454A6"
  },
  "fonts": {
    "default": { "size": 13, "family": ["Segoe UI", "Roboto", "Helvetica Neue", "Arial"], "bold": false },
    "heading": { "size": 18, "family": ["Segoe UI", "Roboto", "Helvetica Neue", "Arial"], "bold": true },
    "mono":    { "size": 12, "family": ["Consolas", "Menlo", "monospace"], "bold": false }
  },
  "appearances": {
    "button": { "states": { "default": { "styles": { "backgroundColor": "buttonFace", "radius": "$borderRadius" },
                                           "properties": { "textColor": "buttonText", "height": 36 } },
                            "hovered": { "styles": { "backgroundColor": "#D1E0E5" } } } },
    "action-button": { "inherit": "button",
      "states": { "default": { "styles": { "backgroundColor": "brandPrimary" }, "properties": { "textColor": "white" } },
                  "hovered": { "styles": { "backgroundColor": "#1F4A92" } },
                  "pressed": { "styles": { "backgroundColor": "#183B75" } } } }
  }
}
```

- **Fonts are objects** `{size, family[], bold}` — the pack's `["Segoe UI", 14]` shorthand is not the framework format.
- **Styles vs properties**: `styles` are the CSS-like decorator values (`backgroundColor`, `radius`, `width`/`style`/`color`
  for borders, `shadow*`, `transition`); `properties` are widget properties (`textColor`, `font`, `height`, `padding`,
  `opacity`, `cursor`, `icon`, `center`). A color value is either a literal (`#1F4A92`, `white`, `rgba(...)`) or the
  **name of a color token** from `colors`. `"$borderRadius"` references a `settings` value. Fonts are referenced by
  name in `properties.font`.
- **States** (Bootstrap-4 `button`): `default`, `hovered`, `focused`, `pressed`, `disabled`, `checked`, `borderNone` …;
  `textbox` has `focused`, `invalid`, `multiline` …; later states override earlier ones and the widget's current
  state set is matched in order — that is the "state order" the course talks about. Custom states added with
  `control.AddState("stale")` are matched the same way.
- **Appearance keys** in Bootstrap-4 (real names): `button`, `panel` (components `captionbar`, `title`, `icon`,
  `pane`), `textbox`, `combobox`, `label-wrapper`, `tabview` (components `bar`, `pane`, `page`), `table`
  (`table-header-cell`, `table-row`, `table-cell`, `table-row-header`), `tooltip` (component `atom`), `toolbar`
  (component `button`), `listview`, `statusbar`, `window`, `page`, `desktop`, `widget`. The course's "grid header" is
  `table-header-cell`, "invalid editor" is the `invalid` state of `textbox`, "tooltip" is `tooltip/atom`.
- **Inheritance**: `"inherit": "button"` copies the base appearance; only the states you list are overridden. A theme
  file may inherit a whole base theme with a top-level `"inherit": "Bootstrap-4"` **(docs; verify at runtime — if
  it does not merge, copy the full base theme JSON from the assembly and edit it: the reviewer has it at
  `C:/Users/matte/AppData/Local/Temp/claude/D--Projects-Netlify-WWW-LearnWisej/0ed61427-0229-4a61-bc1f-cbc4e6e44212/scratchpad/Bootstrap-4.theme`)**.
- **Where theme files live (docs)**: Wisej "loads the ClientTheme from a file in the application `/Themes` directory or
  from the embedded resources of any assembly marked `[assembly: WisejResources]`". So `Themes/AdaptiveOps.theme` in the
  project folder + `"theme": "AdaptiveOps"` in `Default.json` (or `Wisej.DefaultTheme` in Web.config) selects it at
  startup; `Application.LoadTheme("AdaptiveOps")` selects it at runtime (name only, no path, no extension).
- **Session-only theme (docs)**: `var t = new ClientTheme("AdaptiveOps-Dark", Application.Theme); t.Colors.surface = "#111";
  Application.Theme = t;` changes **this session only**. Mutating `Application.Theme.Colors.x` on a theme loaded with
  `LoadTheme` changes it **for every session** — that is the difference Module 3 must demonstrate.
- Reading tokens: `Application.Theme.GetColor("brandPrimary")` (color list), `GetColor("button", "backgroundColor", "hovered")`,
  `GetFont("heading")`, `Application.Theme.Name`.
- Mixins: `Themes/*.mixin.theme` files are merged automatically; `Application.LoadTheme(name, mixins)` takes an explicit list.

## CSS, CssClass, CssStyle, AppearanceKey, States **(docs; CssClass/AppearanceKey verified in Integration Module 5)**

- `control.CssClass = "metric-card elevated"` adds class names to the widget's DOM element. Ship the scoped stylesheet as
  `Styles/AdaptiveOps.css` and load it with `<link rel="stylesheet" href="Styles/AdaptiveOps.css" />` in `Default.html`
  (served by the static file server). The legacy alternative is the `Wisej.Web.StyleSheet` extender component
  (`StyleSheetSource = "Styles/AdaptiveOps.css"`, `SetCssClass(control, "metric-card")`) — mention it, use the link.
- `control.CssStyle = "width:63%"` is an inline style string; use it for exactly one computed one-off value.
- `control.AppearanceKey = "action-button"` selects the theme appearance; `control.States` / `AddState("stale")` /
  `RemoveState("stale")` / `HasState("stale")` manage custom theme states (`AddState(name, true)` propagates to children).
- `Button.Display = Display.Icon | Label | Both` switches icon-only rendering; `Control.ToolTipText` is the accessible name.

## Layout **(docs; Dock / Anchor / AutoScroll verified in the other courses)**

- Docking priority follows the **child order** (`Controls` collection, i.e. z-order in the Designer): the control added
  last docks first against the remaining space. `Padding` on the parent leaves gaps between docked regions; `Margin`
  is honoured only by Flow/Table/Flex engines, not by Dock.
- `ScrollableControl.AutoScroll`, `ScrollBars = ScrollBars.Hidden`, `AutoScrollMargin`; `MinimumSize` / `MaximumSize` on any control.
- `FlowLayoutPanel`: `FlowDirection`, `WrapContents`, `SetFlowBreak(control, true)`, `SetFillWeight(control, n)`.
- `TableLayoutPanel`: `ColumnCount`, `RowCount`, `ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140))`,
  `RowStyles.Add(new RowStyle(SizeType.Percent, 100))`, `Controls.Add(control, column, row)`, `SetColumnSpan`, `SetRowSpan`,
  `SetCellPosition`, `GrowStyle`. **The pack's `TableLayoutColumnStyle` does not exist — use `ColumnStyle` / `RowStyle`.**
- `FlexLayoutPanel`: `LayoutStyle = FlexLayoutStyle.Horizontal | Vertical | Default`, `Spacing` (default 10),
  `HorizontalAlign`, `VerticalAlign`, `SetFillWeight(control, n)`, `SetAlignX(control, HorizontalAlignment.Left|Center|Right)`,
  `SetAlignY(control, VerticalAlignment.Top|Middle|Bottom)`. Constructors `new FlexLayoutPanel(controls[], FlexLayoutStyle)`.
- Fluent markup (`using Wisej.Web.Markup;`): `panel.Dock(DockStyle.Fill).Padding(8).MinimumSize(320, 240).Controls(a, b)`,
  `flex.LayoutStyle(FlexLayoutStyle.Horizontal).Spacing(12).FillWeight(list, 2).FillWeight(details, 1).AlignY(details, VerticalAlignment.Top)`,
  `flow.FlowDirection(...).WrapContents(true).FlowBreak(apply, true).FillWeight(search, 1)`,
  `table.ColumnCount(2).RowCount(4).RowStyles(new RowStyle(SizeType.AutoSize))`, `button.OnClick(b => …)`, `.Text(...)`, `.Name(...)`.
- `TextBox.Watermark` is the placeholder text (**the pack's `PlaceholderText` does not exist**).
- Browser size: `Application.Browser.Size` (window), `Application.Browser.ScreenSize`, `Application.Browser.Device`
  ("Mobile" | "Tablet" | "Desktop"); `Application.BrowserSizeChanged` fires on browser resize; a control's `Resize` event also works.

## Client profiles and responsive behaviour **(docs + embedded ClientProfiles.json)**

- The framework's default `ClientProfiles.json` (embedded) defines, in order: `Phone` (device Mobile, landscape false),
  `Phone (Landscape)`, `Tablet`, `Tablet (Landscape)`, `Small Desktop` (device Desktop, maxWidth 1024). Anything else is
  `ClientProfile.Default` (name "Default" — treat that as the Desktop profile, or add an explicit `Desktop` entry last).
- Properties per profile: `name`, `minWidth`, `maxWidth`, `minScreenWidth`, `maxScreenWidth`, `device` (string or regex),
  `userAgent` (string or regex), `landscape`. Matching is top to bottom, first match wins → order narrow to broad.
- The embedded file's own comment: "place a ClientProfiles.json file in the /bin directory of the application, or add it to
  the root files and set it to EmbeddedResource / Content / Copy if newer. Profiles with names matching the default names
  override them." So keep `ClientProfiles.json` in the project root and add
  `<Content Include="ClientProfiles.json" CopyToOutputDirectory="PreserveNewest" />` to the csproj (both targets).
- `Application.ActiveProfile` (`ClientProfile`: `Name`, `Device`, `MinWidth`, `MaxWidth`, `Landscape` …) is refreshed on
  every request. `Application.ResponsiveProfileChanged += (s, e) => e.CurrentProfile / e.PreviousProfile` fires on change;
  `Control.ResponsiveProfileChanged` exists on every control too. Unsubscribe in `Dispose(bool)`.
- Designer "responsive properties" (per-profile Visible/Size/Location/Dock/Font values) live in `Control.ResponsiveProfiles`
  and are applied by the framework through `IHasResponsiveProfiles.ChangeProfile`. Since these samples are written by hand,
  express the same per-profile values in a small `ApplyProfile(profile)` method and say in the README that in Visual Studio
  the same values would be set in the Designer's profile dropdown.
- Testing profiles without a phone: resize the browser window (Small Desktop ≤ 1024 px) or use the browser devtools device
  emulation (Mobile / Tablet device type + orientation) — the profile is re-evaluated on the next request/resize.

## UI conventions used by every sample (so the samples feel like one course)

- `MainPage : Wisej.Web.Page` with the console shell: **toolbar** (top, 56 px), **navigation rail** (left, 220 px),
  **details panel** (right, 340 px), **status bar** (bottom, 28 px), **workspace** (fill) — light grey page background
  `Color.FromArgb(238,242,247)` until the theme owns it, white cards with `BorderStyle.Solid`.
- A right-hand or bottom card **"Layout & theme · live trace"** — a `ListBox` in monospace font; every server-side decision
  is logged as `HH:mm:ss.fff  → render …` / `← client …` / `• server …`; select the last item after adding. Keep the
  `AddTrace(string)` helper name.
- A button bar (in the toolbar or a "Lab controls" card) that exercises the **success path**, a **progress path**
  (Timer stream or step-through), at least one **failure path** (server-side validation rejection / a caught error) and
  **recovery** (reset to server state). A status label (● ok / warn / error) and a banner label that appears and disappears.
- Designer-style `MainPage.Designer.cs` with `InitializeComponent()` so the file opens in the Wisej Designer; code-behind
  in `MainPage.cs`; UserControls (`Shell/NavigationRail.cs`, `Shell/DetailsEditor.cs` …) follow the same split.
- Ticket data: a small in-memory `Models/TicketRepository` with ~12 tickets (Id, Title, Priority, Status, Owner, DueDate,
  Notes) shown in a `DataGridView` — the same data in every module.

## Docs (`docs/`)

Write each lab deliverable as its own Markdown file, named as the course names it (`VisualInventory.md`, `ThemeNotes.md`,
`CssDecisions.md`, `ShellComposition.md`, `LayoutComparison.md`, `ProfileNotes.md`, `ResponsiveQAMatrix.md`,
`AccessibilityReview.md`, `ArchitectureNote.md`, `ProductionChecklist.md` …). Include a short **Evidence** section
describing what the running app shows for each path. Screenshots the lab asks for are described as "taken by the learner"
with the file name the doc expects. Put the **self-check answers** from the lab guide in the README.
