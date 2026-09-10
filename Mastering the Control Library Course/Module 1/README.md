# OperationsConsole · Mastering the Control Library · Module 1

Local lab build for **Module 1 · Control Library Mental Model**. It follows the lesson guide, the lab / exam
guide and the walkthrough video: the **Operations Console** shell as a Wisej.NET `Page` with four docked areas
(navigation left, command top, status bottom, content fill — added last), six placeholder `UserControl` pages,
one `Navigate(Control content, string title)` method every navigation button routes through, and the
`ControlSelection.md` note. The stretch goal (a diagnostic panel with selected control, selected record, active
profile and last refresh) is in the status area.

The course is cumulative: this is the solution every later module opens. The right-hand **Event log** card and the
`Shell/` contract (`IConsoleShell`, `ConsoleLog`, `ISection`) are defined here and never change afterwards.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 1/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5701
```

Then open <http://localhost:5701>. (Visual Studio: open `OperationsConsole.slnx` in this folder, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Nothing yet | ready state | green status "Ready — choose a section on the left.", diagnostics `Control: — · Record: — · Profile: Desktop · Refreshed: —`, two log lines (catalog size, dock order) |
| Click **Editors** … **Widgets** | success | the placeholder page swaps into the content area, status "Showing Lists and Trees", log `listsTreesButton → Navigate(ListsTreesPage, "Lists and Trees")` then `✓ showing …`, `Control: listsTreesButton` |
| **Refresh** with a section open | progress / command | the shell calls `RefreshSection()` on the visible page (the placeholder logs it has nothing to reload), `Refreshed: HH:mm:ss` is stamped, status "Editors refreshed at …" |
| **Refresh** with no section open | warning | amber status "Nothing to refresh — open a section first." |
| Tick **Simulate page failure**, click a section | failure | `SectionCatalog.CreatePage` throws; red status "The Dashboard section could not be opened — the previous page is still shown.", an `AlertBox` top-right in plain words, the previous page stays, the exception type + message go to the Event log only, the checkbox resets |
| Click the same section again | recovery | the page opens, status is green again |
| Resize the browser under 1025 px / 601 px | profile | log `profile changed → Tablet (…)` / `Phone`, `Profile:` follows (ClientProfiles.json) |
| **Clear** in the Event log card | – | empties the log |

## Where things live

```
OperationsConsole/
├─ MainPage.cs / .Designer.cs       the shell: four docked areas + Event log card, Navigate(), IConsoleShell implementation
├─ Shell/
│  ├─ IConsoleShell.cs              AddLog / SetStatus / SetSelectedControl / SetSelectedRecord (+ StatusLevel)
│  ├─ ConsoleLog.cs                 static gateway sections and services call (Add, Status, Control, Record)
│  └─ ISection.cs                   Title + RefreshSection() — implemented by every section page
├─ Sections/                        six placeholder UserControls: EditorsPage, LayoutsPage, ListsTreesPage,
│                                   DataGridViewPage, DashboardPage, WidgetsPage (title label + module note)
├─ Models/  SectionKey.cs, SectionInfo.cs       the catalog entries (key, title, button name, page type, module)
├─ Services/ SectionCatalog.cs                  the page factory; SimulateFailure drives the failure path
├─ docs/ControlSelection.md         deliverable: control family per area and per placeholder page, alternative considered, evidence
├─ ClientProfiles.json              Phone ≤ 600 px, Tablet ≤ 1024 px, Desktop
├─ Program.cs                       Application.MainPage = new MainPage()
└─ Startup.cs                       Kestrel host (app.UseWisej(), static files from the project folder)
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Four areas docked in order (navigation Left, command Top, status Bottom, content Fill added last) | `MainPage.Designer.cs` — `commandPanel`, `statusPanel`, `navigationPanel`, `pnlEventLog`, `contentPanel` (the `this.Controls.Add` order at the end) |
| One clearly named navigation `Button` per section with `TabIndex` and an accessible label | `MainPage.Designer.cs` — `editorsButton` … `widgetsButton`, `TabIndex` 1–6, `AccessibleName = "Open the … section"` |
| One placeholder `UserControl` per section, each with a title `Label` | `Sections/*Page.cs` + `.Designer.cs` (`lblTitle`) |
| `Navigate(Control content, string title)` clears `contentPanel`, docks Fill, adds, updates `statusLabel` | `MainPage.cs` → `Navigate()`; every button handler → `OpenSection()` → `Navigate()` |
| `ControlSelection.md` with the family chosen and the alternative considered | `docs/ControlSelection.md` |
| Stretch: diagnostic panel (selected control, record ID, active profile, last refresh) | `MainPage.Designer.cs` `diagnosticPanel` (`lblSelectedControl`, `lblSelectedRecord`, `lblActiveProfile`, `lblLastRefresh`); updated by `SetSelectedControl`, `SetSelectedRecord`, `Application_ResponsiveProfileChanged`, `btnRefresh_Click` |
| Show every path: success, page that cannot be created (catch, honest status, `AlertBox`), Ready state | `MainPage.cs` → `OpenSection()` try / catch, `ShowReadyState()`; `Services/SectionCatalog.cs` → `SimulateFailure` |

## Self-check answers (lab / exam guide)

- **When you set `statusLabel.Text` in the navigation method, what actually travels to the browser, and why is that different from writing HTML?**
  Only the changed property of one server object: Wisej.NET serialises the delta (`{ text: "Showing Editors" }` for the
  widget that renders `statusLabel`) and the client widget updates itself. Nothing is re-rendered and no markup is
  written by you; the Label stays a server object with events, theming and design-time support. Writing HTML would
  replace the whole element, lose the widget and its theme appearance, and put presentation into your C#.
- **Which shell area would you turn into a UserControl first, and what would its public surface be?**
  The status area (`statusPanel` + diagnostics). Its surface is exactly what `IConsoleShell` already exposes —
  `SetStatus(text, level)`, `SetSelectedControl(name)`, `SetSelectedRecord(id)` plus a `LastRefresh` property — and
  nothing about its five Labels. Module 3 does exactly this with the `StatusStrip` UserControl.
- **If a later module needs a collapsible navigation area, which native container replaces the docked panel, and why is that preferable to a custom widget?**
  `SplitContainer` (`Panel1` = navigation, `Panel2` = content) with `Panel1Collapsed` for the narrow profile. It is a
  native control: the user can resize it, it participates in responsive profiles, theming and the designer, and the
  collapse is one property instead of JavaScript you have to load, theme, debug in the browser console and maintain.

## Known simplifications

- The placeholder pages hold no data, so `Record:` stays `—` in this module; Module 4 and 5 set it from the selected
  document / order through `ConsoleLog.Record(...)`.
- `Button.AccessibleRole` does not exist in Wisej.NET 4.1 (`AccessibleName` and `AccessibleDescription` do); the
  buttons carry `AccessibleName` only.
