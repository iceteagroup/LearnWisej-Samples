# OperationsConsole · Mastering the Control Library · Module 3

Local lab build for **Module 3 · Containers, Layouts, Navigation, and Reuse**. It follows the lesson guide, the
lab / exam guide and the walkthrough video: the Module 1 placeholders are gone and the **real Operations Console
shell** is here — a `ToolBar` docked Top, a `StatusBar` docked Bottom and a `SplitContainer splitMain` docked
Fill, with the navigation list in `Panel1`, a `TabControl tabDetail` in `Panel2` and one `TabPage` per peer
section. The repeated record header and status strip are now the `RecordHeader` and `StatusStrip` UserControls,
whose entire public surface is `Title`, `RecordCount`, `LastRefresh` and `RefreshRequested`. A narrow client
profile collapses the navigation and shows `btnMobileMenu`, and the StatusBar shows real status — active profile,
record count, selected record, last refresh.

The course is cumulative: this folder is the whole solution as it stands after Module 3. The `Shell/` contract
(`IConsoleShell`, `ConsoleLog`, `ISection`) and the right-hand **Event log** card come from Module 1 and are
unchanged; the five other sections are still their Module 1 placeholders and are filled in by their own modules.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 3/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5703
```

Then open <http://localhost:5703>. (Visual Studio: open `OperationsConsole.slnx` in this folder, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Nothing yet | ready state | The **Layouts** tab is open. StatusBar: green `Showing Layouts`, `Desktop profile · Desktop`, `6 records`, `Record: LAY-001`, `Control: —`, `Refreshed: —`. Log: `Operations Console shell ready · 6 sections…`, `child order: splitMain (Fill) added first, …`, `startup → SectionCatalog.CreatePage(LayoutsPage)`, `✓ LayoutsPage hosted in tabLayouts (Dock = Fill)`, then the LayoutsPage lines |
| Click a **chip** in the FlowLayoutPanel | success | The TableLayoutPanel form fills with Region / Container / Why; StatusBar `Record: LAY-004`, `Control: chipLAY004` |
| Click a section in the **navigation list** | success | The matching tab is selected and its page is created on first use; status `Showing Dashboard — still a placeholder, Module 6 builds this section.`; log `navList → SectionCatalog.CreatePage(DashboardPage)` then `✓ …` |
| Click a **tab** directly | success | The navigation selection follows it (log `tabDetail → …`) — the two surfaces are always in step |
| **New** on the ToolBar (Layouts tab) | success | `LayoutCardService.AddNext() → LAY-007 …`, a new chip appears and the FlowLayoutPanel rewraps, StatusBar `Added LAY-007 to Layouts.`, `7 records`, green Toast |
| **New** on any other tab | validation | Amber `New is not available on Editors — that section holds no records yet.` + amber Toast (the placeholder does not implement `ISectionRecords`) |
| **Refresh** on the ToolBar | success | `btnRefresh → LayoutsPage.RefreshSection()`, the chips are rebuilt, `Refreshed: HH:mm:ss` is stamped in the StatusBar **and** in both UserControls, green status |
| **Save** right after a Refresh | validation | Amber `Nothing has changed on Layouts since the last save.` + amber Toast — the service is not even called |
| **New**, then **Save** | success | Green `Saved 7 layout cards.` + green Toast |
| Tick **Simulate service failure** (Layouts command row), **New**, then **Save** | failure (service) | Red `Layouts could not be saved — the service refused the write. Try again in a moment.` + red Toast; the exception type and message go to the Event log only. Untick it and press Save again → green (recovery) |
| **Save** on any other tab | validation | Amber `Nothing to save on Editors — that section holds no records yet.` |
| Toggle **Simulate page failure** on the ToolBar, then open a tab that has never been opened | failure (page) | `✗ WidgetsPage could not be created — InvalidOperationException`, the tab shows the friendly *"The Widgets section is not available right now"* card with a **Try again** button, the StatusBar turns red and an `AlertBox` appears top-right in plain words |
| Untick it and press **Refresh** (or **Try again**) on that tab | recovery | `btnRefresh → retrying WidgetsPage (recovery)`, the page appears, status green |
| **Force narrow** on the ToolBar | narrow profile | `splitMain.Panel1Collapsed = true`, **☰ Sections** appears at the left of the ToolBar, StatusBar reads `Narrow profile · Desktop (forced)`, the diagnostic panels shrink to their text |
| **☰ Sections** | narrow navigation | A ContextMenu with the six sections plus **Show the navigation panel** — picking a section navigates, picking the last item expands `Panel1` again with the selection intact |
| **Force narrow** again | back to desktop | Everything comes back; nothing was rebuilt, so the navigation selection, the tabs and the card list are untouched |
| Resize the browser under 1025 px / 600 px | real profile | `profile changed → Tablet (…)` / `Phone`, `ApplyNarrowProfile(true)`, and the Layouts section stacks its three demos into one column on its own |
| Tick **Narrow layout** on the Layouts command row | section narrow | `LayoutsPage.ApplyNarrowLayout(true) → tblDemo.ColumnCount = 1` — the section reflows its content, the shell is untouched |
| Drag the **splitter** | anchoring | `lblAnchorStretch` changes width, `Top | Right` keeps its width and its gap, `Bottom | Right` stays in the corner, the unanchored label does not move; the card chips rewrap |
| **Refresh** in the record header card, or **↻** in the strip below it | reuse boundary | Both raise `RefreshRequested`; the log names which control asked (`recordHeader.RefreshRequested → LayoutsPage.RefreshSection()`) and the same method runs |
| Right-click the **navigation list** | command model | Refresh / New / Save / Simulate page failure — the same command methods as the ToolBar, from a surface tied to the selection |
| **Clear** in the Event log card | – | empties the log |

## Where things live

```
OperationsConsole/
├─ MainPage.cs / .Designer.cs        the shell: toolBar (Top), statusBar (Bottom), splitMain (Fill) + Event log card;
│                                    navList | tabDetail; ApplyNarrowProfile; IConsoleShell implementation
├─ Shell/
│  ├─ IConsoleShell.cs               AddLog / SetStatus / SetSelectedControl / SetSelectedRecord (Module 1, unchanged)
│  ├─ ConsoleLog.cs                  static gateway sections and services call (Module 1, unchanged)
│  ├─ ISection.cs                    Title + RefreshSection() (Module 1, unchanged)
│  ├─ ISectionRecords.cs             NEW · optional: RecordCount / NewRecord() / Save() → SectionSaveResult
│  ├─ RecordHeader.cs / .Designer.cs NEW · reusable card header — Title, RecordCount, LastRefresh, RefreshRequested
│  └─ StatusStrip.cs / .Designer.cs  NEW · the same surface, a completely different layout
├─ Sections/
│  ├─ LayoutsPage.cs / .Designer.cs  REBUILT · TableLayoutPanel + FlowLayoutPanel + anchored panel, the two
│  │                                 UserControls, a narrow layout of its own and every path of the section
│  └─ EditorsPage, ListsTreesPage, DataGridViewPage, DashboardPage, WidgetsPage   Module 1 placeholders
├─ Models/  SectionKey.cs, SectionInfo.cs, LayoutCard.cs (NEW)
├─ Services/ SectionCatalog.cs (page factory, SimulateFailure), LayoutCardService.cs (NEW, SimulateFailure)
├─ docs/
│  ├─ LayoutNotes.md                 deliverable · the docking plan, the child order, anchoring, the narrow profile
│  ├─ ContainerDecisions.md          deliverable · why these containers and these UserControls
│  └─ ControlSelection.md            Module 1 deliverable
├─ ClientProfiles.json               Phone ≤ 600 px, Tablet ≤ 1024 px, Desktop
├─ Program.cs                        Application.MainPage = new MainPage()
└─ Startup.cs                        Kestrel host (app.UseWisej(), static files from the project folder)
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Write the docking plan into `LayoutNotes.md` **first** (ToolBar Top, StatusBar Bottom, SplitContainer Fill, in that child order) | `docs/LayoutNotes.md` §1–§2 |
| Build `MainPage` from a `ToolBar`, a `StatusBar` and a `SplitContainer` named `splitMain` docked in that order | `MainPage.Designer.cs` — `toolBar`, `statusBar`, `splitMain`; the `this.Controls.Add(...)` block at the end (Fill first, bars last — docking is applied last-to-first) |
| Navigation in `splitMain.Panel1`, `TabControl tabDetail` docked Fill in `splitMain.Panel2` | `MainPage.Designer.cs` — `splitMain.Panel1.Controls.Add(navList /*Fill*/ … lblSections /*Top*/)`, `splitMain.Panel2.Controls.Add(tabDetail)` |
| Turn the placeholder pages into `TabPage`s for the peer sections | `tabEditors`, `tabLayouts`, `tabListsTrees`, `tabDataGridView`, `tabDashboard`, `tabWidgets` (each `Tag` is its `SectionKey`); `MainPage.EnsurePage()` hosts `SectionCatalog.CreatePage(key)` docked Fill |
| `btnNew`, `btnRefresh`, `btnSave` on the ToolBar, one-line handlers calling named command methods | `MainPage.toolBar_ButtonClick` → `NewRecord()`, `RefreshCurrentSection()`, `SaveCurrentSection()` |
| A `ContextMenu` on the navigation list | `navContextMenu` (`mnuRefresh` / `mnuNew` / `mnuSave` / `mnuSimulateFailure`) assigned to `navList.ContextMenu`; handlers are one line each |
| Extract the record header and the status strip into UserControls exposing only `Title`, `RecordCount`, `LastRefresh` and `RefreshRequested`, children private | `Shell/RecordHeader.cs` + `.Designer.cs`, `Shell/StatusStrip.cs` + `.Designer.cs`; used in `Sections/LayoutsPage` (`UpdateReusableControls()`) |
| Define a narrow client profile and implement `ApplyNarrowProfile(bool narrow)` collapsing `splitMain.Panel1` and showing `btnMobileMenu` | `ClientProfiles.json`; `MainPage.ApplyNarrowProfile()`, `UpdateProfile()`, `Application_ResponsiveProfileChanged`, `ToggleForceNarrow()`, `ShowSectionsMenu()` |
| StatusBar shows real status (active profile, record count, last refresh), updated from the same command methods | `MainPage` — `pnlProfile`, `pnlRecords`, `pnlRecord`, `pnlControl`, `pnlRefresh`; `UpdateProfilePanel()`, `UpdateRecordCount()`, `StampRefresh()`, `SetStatus()` |
| Show every path: desktop → narrow → desktop, every tab, Refresh and Save with success / validation / error | ToolBar **Force narrow**, **Simulate page failure**; the Layouts command row (**Add card**, **Narrow layout**, **Simulate service failure**); `LayoutsPage.Save()` returns `SectionSaveResult.Ok / Rejected / Failed` |
| A short note explaining the container and UserControl decisions | `docs/ContainerDecisions.md` |
| Stretch (Module 1): diagnostic readout | moved into the StatusBar — `Control:`, `Record:`, profile and last refresh panels |

## Deliverables

1. **Navigation shell from SplitContainer, TabControl, ToolBar and StatusBar** — [`OperationsConsole/MainPage.Designer.cs`](OperationsConsole/MainPage.Designer.cs) + [`MainPage.cs`](OperationsConsole/MainPage.cs)
2. **Repeated UI moved into UserControls with a small public surface** — [`OperationsConsole/Shell/RecordHeader.cs`](OperationsConsole/Shell/RecordHeader.cs), [`OperationsConsole/Shell/StatusStrip.cs`](OperationsConsole/Shell/StatusStrip.cs)
3. **Narrow-profile layout using profile-aware logic** — `MainPage.ApplyNarrowProfile()` + [`OperationsConsole/ClientProfiles.json`](OperationsConsole/ClientProfiles.json), and the section's own `LayoutsPage.ApplyNarrowLayout()`
4. **LayoutNotes.md documenting the docking and anchoring choices** — [`OperationsConsole/docs/LayoutNotes.md`](OperationsConsole/docs/LayoutNotes.md)
5. **The short note on container and UserControl decisions** — [`OperationsConsole/docs/ContainerDecisions.md`](OperationsConsole/docs/ContainerDecisions.md)
6. **Screenshots at desktop and narrow widths** — take them from the running app: desktop as it opens, then with **Force narrow** pressed (or the browser under 1025 px). Both are described in the Evidence sections of the two docs.

## Self-check answers (lab / exam guide)

- **If the SplitContainer were added to the page before the ToolBar and StatusBar, what would the user see, and why does the order matter?**
  Docking is applied in child order, so the `Fill` control would be laid out first and would take the whole page;
  the ToolBar and StatusBar, added afterwards, would then claim their edges *on top of* it. The user sees the
  ToolBar overlapping the split panel and the StatusBar covering the last row of the content — the "same four
  controls, wrong order" picture from the video. The order matters because it *is* the layout: the edges must be
  claimed before the fill takes what is left. In the Wisej.NET designer the serialised `Controls.Add` list reads
  the other way round (docking is applied from the last added to the first), so the `Fill` control is written
  first and the bars last — which is exactly what `MainPage.Designer.cs` does. When a shell looks wrong, check
  the child order before touching a single size.

- **Which of your UserControl's members could you rename without touching any consuming page, and which could you not? What does that tell you about its public surface?**
  Everything private: `lblTitle`, `lblCount`, `lblRefreshed`, `btnRefresh` in `RecordHeader`, and `lblStrip` and
  `btnStripRefresh` in `StatusStrip`, plus the private fields and the `UpdateStrip()` method — nothing outside the
  control names them. What cannot be renamed is `Title`, `RecordCount`, `LastRefresh` and `RefreshRequested`,
  because `Sections/LayoutsPage` (and any future page) is written against exactly those four names. That is the
  public surface, and it is deliberately four members wide: the consumer states *what* it is showing and *when* it
  was refreshed, and is told *that* someone asked for a refresh — never how any of it is rendered. The proof that
  the boundary is real is that `StatusStrip` renders the same four members with completely different internals.

- **Which properties changed on the narrow profile, and why did you not build a separate narrow screen?**
  Four: `splitMain.Panel1Collapsed` (the navigation collapses), `btnMobileMenu.Visible` (the way back — a
  collapsed navigation always needs one), the `MinWidth` of the two diagnostic StatusBar panels (column display:
  they stop reserving room and take only their text), and the text of `pnlProfile` so the StatusBar names the
  active profile. Nothing else: same `Page`, same controls, same tabs, same commands. A separate narrow screen was
  not built because the *workflow* does not change on a phone — the user still picks a section and works in it;
  only the space available changes. Two screens would mean two things to maintain, two places for a bug fix and a
  guaranteed drift, and the collapsed panel would have to be rebuilt instead of merely collapsed. The rule from
  the reading is to build a good desktop layout first and then change the handful of properties that must change.

- **Which control in this module is doing the most important work?** (video pause question)
  `SplitContainer splitMain`. It is the only control that hands a layout decision to the user, it is the anchor of
  the whole shell (`Dock = Fill` between the two bars), and its `Panel1Collapsed` is the narrow profile in one
  property. The `TabControl` is a close second, but tabs only organise peers; the splitter is what makes the
  screen negotiable.

- **Which part of the implementation belongs in a reusable UserControl or a service?** (video pause question)
  The record header and the status strip belong in UserControls — they are repeated UI with a tiny meaningful
  surface (`Shell/RecordHeader`, `Shell/StatusStrip`). The card data and the save rules belong in a service
  (`Services/LayoutCardService`, with `SimulateFailure` so the failure path is reproducible). What does *not*
  belong in either is the navigation: `navList` and the ToolBar raise events and call named command methods on the
  page — a navigation control that contains business logic is one of the module's pitfalls.

## Known simplifications

- `ISectionRecords` is implemented by `LayoutsPage` only. The other five sections are still Module 1 placeholders,
  so the StatusBar honestly shows `— records` for them and New / Save answer "that section holds no records yet".
- `LayoutCardService.Save()` has nothing to persist — the cards are the state. It exists so the section has a real
  success / rejected / failed answer to give.
- The Event log card is lab instrumentation, not part of the production shell, so `ApplyNarrowProfile` leaves it
  alone; a real narrow layout would drop it.
- ToolBar buttons carry text and no `ImageSource`: the theme icon names for New / Refresh / Save were not verified
  at runtime, and a missing icon is worse than no icon.
