# Mastering the Control Library · lab samples

One runnable Wisej.NET 4 application per module, built from the course's lesson guide, lab / exam guide
and walkthrough video. The course is cumulative — every lab opens the `OperationsConsole` solution of the
previous module — so each `Module N` folder holds the **complete Operations Console as it stands after
module N**: the shell, every earlier section, and this module's work. Every sample has the same frame:
navigation on the left, the section content in the middle, an **Event log** card on the right that records
every user action and every service decision, a status area that follows every change, and a command row on
each section page that exercises the success path, a progress path, at least one failure path and the recovery.
Each folder has its own `README.md` (what to click, lab steps → code map, self-check answers) and a `docs/`
folder with the lab deliverables.

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package (Module 6+ also restores
`Wisej-4-ChartJS` from nuget.org). Nothing is deployed anywhere.

| Module | Folder | What it adds to the Operations Console | Run |
|---|---|---|---|
| 1 · Control Library Mental Model | `Module 1` | the control catalog shell: `MainPage` with navigation / command / status / content areas, six placeholder section pages, `Navigate()`, Event log, `ControlSelection.md` | `dotnet run -f net10.0 --urls http://localhost:5701` |
| 2 · Editors, Buttons, Validation, and Feedback | `Module 2` | `CustomerEditor` UserControl on the Editors page: value-matched editors, `ErrorProvider` validators, Save / Reset / Validate commands, busy state, Toast / AlertBox feedback | `http://localhost:5702` |
| 3 · Containers, Layouts, Navigation, and Reuse | `Module 3` | the real shell: `ToolBar` + `StatusBar` + `SplitContainer` + `TabControl`, `RecordHeader` / `StatusStrip` UserControls, narrow-profile layout, `LayoutNotes.md` | `http://localhost:5703` |
| 4 · Lists, Trees, Repeaters, and Hierarchical Data | `Module 4` | the document explorer: lazy `TreeView`, virtual-mode `ListView`, shared `ImageList`, `DocumentDetailControl`, `DocumentService` | `http://localhost:5704` |
| 5 · DataGridView Mastery | `Module 5` | the Orders grid: `BindingSource` + explicit columns, HTML status badge, custom editor / command column, virtual mode with `OrderCache`, filter and status strips | `http://localhost:5705` |
| 6 · Charts, Dashboards, Content, Media, and Documents | `Module 6` | the dashboard tab: ChartJS trend, `ProgressBar`, `PdfViewer` / `HtmlPanel` preview, `Upload` workflow, one `RefreshDashboard(model)` | `http://localhost:5706` |
| 7 · Custom Widgets, Extensions, Theming, and Capstone | `Module 7` | `ratingWidget` (`Widget` + `rating.js` / `rating.css`), `WidgetEvent` → `RatingService`, `CallAsync("setSaved")`, theming, `CapstoneNotes.md` + demo script | `http://localhost:5707` |

Run any module from its `OperationsConsole` project folder. The projects multi-target `net10.0-windows` and
`net10.0`, so `dotnet run` needs a framework (`-f net10.0`, or `-f net10.0-windows`), e.g.

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 3/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5703
```

or open the `OperationsConsole.slnx` in the module folder with Visual Studio and press F5.

## `_template`

The scaffold every module was built from (`OperationsConsole` project with `ClientProfiles.json` for the
Phone / Tablet / Desktop responsive profiles), plus `COOKBOOK.md`: the Wisej.NET conventions verified while building
and running these samples (responsive profiles, Toast / AlertBox / MessageBox, the shell contract every section
follows, the control signatures used by each module, and the gotchas found along the way). Read it before writing
a new sample.
