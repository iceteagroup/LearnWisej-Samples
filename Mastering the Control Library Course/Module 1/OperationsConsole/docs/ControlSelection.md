# ControlSelection.md — Operations Console, Module 1

The control family chosen for each shell area and each placeholder page, and the simpler native alternative that
was considered. This is the first page of the team's *control selection guide*; later modules extend it
(LayoutNotes.md, ExplorerNotes.md, GridDecisions.md, DashboardNotes.md, WidgetContract.md).

Rule applied everywhere: **choose the simplest native control that expresses the user's task**; reach for an
extender or a UserControl next; build a JavaScript widget only when nothing native fits.

## Shell areas (`MainPage : Page`)

| Area | Control family chosen | Why | Simpler / other alternative considered |
|---|---|---|---|
| Whole shell | `Page` (fills the browser) | The console is the application, not a dialog; a Page participates in responsive profiles and has no window chrome to manage. | `Form` — right for dialogs, wrong for the entry surface. `Desktop` — overkill for one console. |
| `navigationPanel` (Left) | Container: `Panel` + one `Button` per section | Six fixed sections; a button per section is the simplest command control, gets `TabIndex` and `AccessibleName`, and reads as a business action (`editorsButton_Click`). | `ListBox` / `TreeView` — better when the section list is data-driven or hierarchical (Module 3 moves to a list inside a `SplitContainer`). `MenuBar` — hides the sections behind a click. |
| `commandPanel` (Top) | Container: `Panel` with `Button` + `CheckBox` | Holds the commands for the current section (Refresh) and the switch that makes the failure path reproducible. A plain docked panel is enough for two commands. | `ToolBar` — the right control once commands multiply (Module 3). `RibbonBar` — for many task-grouped commands, not two. |
| `statusPanel` (Bottom) | Container: `Panel` + `Label statusLabel` + diagnostic `Label`s | A status area needs text with colour (green / amber / red); a docked Label does that with zero custom code. | `StatusBar` with panels — adopted in Module 3. `Toast` — transient; the status must persist. |
| `contentPanel` (Fill, added last) | Container: `Panel` hosting one `UserControl` | One host, one child docked `Fill`; `Navigate()` swaps the child. It is added last so it fills what the other three leave. | `TabControl` — used in Module 3 once sections are peers the user switches between. |
| Feedback for a page that cannot open | Notification: `AlertBox` (TopRight, auto-close) + red `statusLabel` | Non-blocking, explains in plain words, never covers the buttons, shows no exception text. | `MessageBox` — blocking; reserved for a real decision. `Toast` — too light for an error the user must notice. |

## Placeholder pages (`Sections/*Page : UserControl, ISection`)

Each placeholder is a `UserControl` with a title `Label`, so the later module can fill in the same type. `UserControl`
was chosen over a `Panel` built in code because the page has business meaning, opens in the designer, and exposes a
small surface (`Title`, `RefreshSection()`) instead of its child controls.

| Page | Built in | Control family it will use | Simpler alternative considered (and why it loses) |
|---|---|---|---|
| `EditorsPage` | Module 2 | Editors: `TextBox`, `ComboBox`, `DateTimePicker`, `NumericUpDown` + `ErrorProvider`, `ToolTip`, `Toast` | A `TextBox` for every value — accepts ambiguous input (dates, thousands separators). |
| `LayoutsPage` | Module 3 | Containers: `SplitContainer`, `TabControl`, `ToolBar`, `StatusBar`, `FlowLayoutPanel` / `TableLayoutPanel` | Absolute `Location` / `Size` — breaks on the first resize. |
| `ListsTreesPage` | Module 4 | Lists and trees: `TreeView` (lazy) + `ListView` (virtual mode) + shared `ImageList` + detail `UserControl` | `ComboBox` for categories — hides the hierarchy. |
| `DataGridViewPage` | Module 5 | Data grid: `DataGridView` with `BindingSource`, explicit columns, `AllowHtml` cells, custom `Editor`, virtual mode | `ListView` Details view — no editing or virtual cell values. |
| `DashboardPage` | Module 6 | Content and extensions: ChartJS (NuGet), `ProgressBar`, `PdfViewer` / `HtmlPanel`, `Upload` | A second chart for a single status value — a `ProgressBar` answers "how close to target" better. |
| `WidgetsPage` | Module 7 | Extension: `Widget` (rating.js / rating.css packages, `WidgetEvent`, `CallAsync`) | `NumericUpDown` for a 1–5 rating — functional but not the star gesture; the widget is the *last* rung. |

## Conventions fixed in this module

- Names by business role: `navigationPanel`, `contentPanel`, `statusLabel`, `editorsButton`, `btnRefresh` — never `panel1`, `button3`.
- Dock order: `commandPanel` Top, `statusPanel` Bottom, `navigationPanel` Left, `contentPanel` Fill (added last).
- `TabIndex` 1–6 on the navigation buttons, 10–11 in the command area; `AccessibleName` on every button.
- Sections update the shell only through `Shell/ShellStatus` (`Show`, `Control`, `Record`) and are addressed by the shell only through `Shell/ISection`.
- Status colours: green `31,157,87` ok, amber `232,161,60` warning, red `224,86,59` error.

## Evidence (what the running app shows)

- **Ready state** — green status "Ready", diagnostics `Control: —  Record: —  Profile: Desktop  Refreshed: —`.
- **Success path** — clicking any of the six buttons swaps the placeholder into `contentPanel`, the status reads
  "Showing Lists and Trees" and the diagnostics show `Control: listsTreesButton`.
- **Refresh** — calls `RefreshSection()` on the visible page and stamps `Refreshed: HH:mm:ss`; with no section open the
  status turns amber: "Nothing to refresh — open a section first."
- **Failure path** — tick *Simulate page failure* and click a section: the page factory throws, the status turns red,
  an `AlertBox` explains in plain words and the previous page stays. Untick it and click again to recover.
- **Profile** — resizing the browser below 1025 px updates `Profile:` (ClientProfiles.json: Phone ≤ 600, Tablet ≤ 1024, Desktop).
