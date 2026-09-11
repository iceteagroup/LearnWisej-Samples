# OperationsConsole · Mastering the Control Library · Module 1

Lab build for **Module 1 · Control Library Mental Model**: the **Operations Console** shell as a Wisej.NET `Page`
with four docked areas (navigation left, command top, status bottom, content fill — added last), six placeholder
`UserControl` pages, one `Navigate(Control content, string title)` method every navigation button routes through,
and the `ControlSelection.md` note. The stretch goal (selected control, selected record, active profile, last
refresh) sits in the status area.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 1/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5701
```

Then open <http://localhost:5701>. (Visual Studio: open `OperationsConsole.slnx` in this folder, press F5.)

## What to try

| Action | What you should see |
|---|---|
| Nothing yet | green status "Ready", diagnostics `Control: — · Record: — · Profile: Desktop · Refreshed: —` |
| Click **Editors** … **Widgets** | the placeholder page swaps into the content area, status "Showing Lists and Trees", `Control: listsTreesButton` |
| **Refresh** with a section open | `Refreshed: HH:mm:ss` is stamped, status "Editors refreshed at …" |
| **Refresh** with no section open | amber status "Nothing to refresh — open a section first." |
| Tick **Simulate page failure**, click a section | red status, an `AlertBox` top-right in plain words, the previous page stays |
| Untick it, click the section again | the page opens, status is green again |
| Resize the browser under 1025 px / 601 px | `Profile:` follows (ClientProfiles.json) |

## Where things live

```
OperationsConsole/
├─ MainPage.cs / .Designer.cs   the shell: four docked areas, Navigate(), IConsoleShell implementation
├─ Shell/                       IConsoleShell (+ StatusLevel), ShellStatus, ISection
├─ Sections/                    six placeholder UserControls (title label)
├─ Models/                      SectionKey, SectionInfo
├─ Services/SectionCatalog.cs   the page factory (SimulateFailure drives the failure path)
├─ docs/ControlSelection.md     deliverable: control family per area and per page, alternative considered
└─ ClientProfiles.json          Phone ≤ 600 px, Tablet ≤ 1024 px, Desktop
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Four areas docked in order (content Fill added last) | `MainPage.Designer.cs` — `commandPanel`, `statusPanel`, `navigationPanel`, `contentPanel` and the `Controls.Add` order |
| One named navigation `Button` per section with `TabIndex` and an accessible label | `editorsButton` … `widgetsButton`, `TabIndex` 1–6, `AccessibleName` |
| One placeholder `UserControl` per section with a title `Label` | `Sections/*Page.cs` + `.Designer.cs` (`lblTitle`) |
| `Navigate(Control content, string title)` | `MainPage.Navigate()`; every button → `OpenSection()` → `Navigate()` |
| `ControlSelection.md` | `docs/ControlSelection.md` |
| Stretch: diagnostic panel | `diagnosticPanel` (`lblSelectedControl`, `lblSelectedRecord`, `lblActiveProfile`, `lblLastRefresh`) |
| Show every path: success, a page that cannot be created, Ready | `MainPage.OpenSection()` try / catch + `AlertBox`; `SectionCatalog.SimulateFailure` |

## Self-check answers

- **When you set `statusLabel.Text`, what travels to the browser, and why is that different from writing HTML?**
  Only the changed property of one server object: Wisej.NET sends the delta for the widget that renders
  `statusLabel` and the client widget updates itself. Nothing is re-rendered and no markup is written; the Label
  stays a server object with events, theming and design-time support.
- **Which shell area would you turn into a UserControl first, and what would its public surface be?**
  The status area. Its surface is what `IConsoleShell` already exposes — `SetStatus(text, level)`,
  `SetSelectedControl(name)`, `SetSelectedRecord(id)` plus a `LastRefresh` property — and nothing about its Labels.
  Module 3 does this with the `StatusStrip` UserControl.
- **If a later module needs a collapsible navigation area, which native container replaces the docked panel?**
  `SplitContainer` (`Panel1` = navigation) with `Panel1Collapsed` for the narrow profile: resizable, responsive,
  themed and designable, and the collapse is one property instead of JavaScript to maintain.
