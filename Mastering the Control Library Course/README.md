# Mastering the Control Library · lab samples

One runnable Wisej.NET 4 application per module, built from the course's lesson guide, lab / exam guide and
walkthrough video. The course is cumulative — every lab opens the `OperationsConsole` solution of the previous module
— so each `Module N` folder holds the **complete Operations Console as it stands after module N**. Each screen shows
what that module's lab and video build: the shell with its navigation, content and status areas, and the section the
module is about. Each folder has its own `README.md` (what to click, lab steps → code, self-check answers) and a
`docs/` folder with the lab deliverables.

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package (Module 6+ also restores
`Wisej-4-ChartJS` 4.1.0 from nuget.org).

| Module | Folder | What it adds to the Operations Console | Run |
|---|---|---|---|
| 1 · Control Library Mental Model | `Module 1` | the shell: `MainPage` with navigation / command / status / content areas, six placeholder pages, `Navigate()`, `ControlSelection.md` | `dotnet run -f net10.0 --urls http://localhost:5701` |
| 2 · Editors, Buttons, Validation, and Feedback | `Module 2` | `CustomerEditor` on the Editors page: value-matched editors, `ErrorProvider` validators, Save / Reset / Validate, busy state, Toast / AlertBox | `http://localhost:5702` |
| 3 · Containers, Layouts, Navigation, and Reuse | `Module 3` | the real shell: `ToolBar` + `StatusBar` + `SplitContainer` + `TabControl`, `RecordHeader` / `StatusStrip` UserControls, narrow profile, `LayoutNotes.md` | `http://localhost:5703` |
| 4 · Lists, Trees, Repeaters, and Hierarchical Data | `Module 4` | the document explorer: lazy `TreeView`, virtual-mode `ListView`, shared `ImageList`, `DocumentDetailControl`, `DocumentService` | `http://localhost:5704` |
| 5 · DataGridView Mastery | `Module 5` | the Orders grid: `BindingSource` + explicit columns, HTML status badge, `MonthCalendar` editor and command column, virtual mode with `OrderCache`, filter and status strips | `http://localhost:5705` |
| 6 · Charts, Dashboards, Content, Media, and Documents | `Module 6` | the dashboard tab: ChartJS trend, `ProgressBar`, `PdfViewer` / `HtmlPanel`, `Upload`, one `RefreshDashboard(model)` | `http://localhost:5706` |
| 7 · Custom Widgets, Extensions, Theming, and Capstone | `Module 7` | `ratingWidget` (`Widget` + `rating.js` / `rating.css`), `WidgetEvent` → `RatingService`, `CallAsync("setSaved")`, theming, `CapstoneNotes.md` + demo script | `http://localhost:5707` |

Run any module from its `OperationsConsole` project folder (the projects multi-target `net10.0-windows` and `net10.0`):

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 3/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5703
```

or open `OperationsConsole.slnx` in the module folder with Visual Studio and press F5.

## `_template`

The scaffold every module was built from, plus `COOKBOOK.md`: the Wisej.NET conventions and verified runtime facts
used by these samples. Read it before writing a new sample.
