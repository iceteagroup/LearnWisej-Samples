# OperationsConsole · Mastering the Control Library · Module 3

Lab build for **Module 3 · Containers, Layouts, Navigation, and Reuse**: the real Operations Console shell — a
`ToolBar` docked Top, a `StatusBar` docked Bottom and a `SplitContainer splitMain` docked Fill, with the navigation
list in `Panel1` and a `TabControl tabDetail` in `Panel2`, one `TabPage` per peer section. The repeated record
header and status strip are the `RecordHeader` and `StatusStrip` UserControls (`Title`, `RecordCount`,
`LastRefresh`, `RefreshRequested`). A narrow client profile collapses the navigation and shows `btnMobileMenu`,
and the StatusBar shows the active profile, record count, selected record and last refresh.

Editors comes from Module 2; Lists and Trees, DataGridView, Dashboard and Widgets are still placeholders.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 3/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5703
```

Then open <http://localhost:5703>.

## What to try

| Action | What you should see |
|---|---|
| Nothing yet | the **Layouts** tab is open; StatusBar `Showing Layouts`, `Desktop profile · Desktop`, `6 records`, `Record: LAY-001` |
| Click a card chip | the form fills with Region / Container / Why; StatusBar `Record: LAY-004` |
| Click a section in the navigation list, or a tab | the list and the tabs stay in step; the page is created on first use |
| **New** (Layouts tab) | a new chip appears, `Added LAY-007 to Layouts.`, `7 records`, Toast |
| **New** / **Save** on a placeholder tab | amber "New is not available …" / "Nothing to save …" |
| **Refresh** | the chips are rebuilt, `Refreshed: HH:mm:ss` in the StatusBar and in both UserControls |
| **Save** right after a Refresh | amber "Nothing has changed on Layouts since the last save." |
| **New**, then **Save** | green "Saved 7 layout cards." |
| Tick **Simulate service failure**, **New**, **Save** | red "Layouts could not be saved …" + Toast; untick and Save again to recover |
| Resize the browser under 1025 px | the navigation collapses, **☰ Sections** appears, StatusBar `Narrow profile · Tablet`, the Layouts content stacks into one column |
| **☰ Sections** | the six sections plus **Show the navigation panel** |
| **Refresh** in the record header, or **↻** in the strip | both raise `RefreshRequested`; the same `RefreshSection()` runs |
| Right-click the navigation list | Refresh / New / Save — the same command methods as the ToolBar |

## Where things live

```
OperationsConsole/
├─ MainPage.cs / .Designer.cs       toolBar (Top), statusBar (Bottom), splitMain (Fill); navList | tabDetail; ApplyNarrowProfile
├─ Shell/  ISectionRecords.cs       RecordCount / NewRecord() / Save() → SectionSaveResult
│          RecordHeader, StatusStrip   the two reusable UserControls
├─ Sections/LayoutsPage.cs          cards (FlowLayoutPanel), selected card (TableLayoutPanel), the two UserControls
├─ Models/LayoutCard.cs, Services/LayoutCardService.cs (SimulateFailure)
└─ docs/LayoutNotes.md, docs/ContainerDecisions.md
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Docking plan in `LayoutNotes.md` first; `ToolBar`, `StatusBar`, `SplitContainer splitMain` docked in that order | `docs/LayoutNotes.md`; `MainPage.Designer.cs` `Controls.Add` block (Fill first, bars last) |
| Navigation in `Panel1`, `TabControl tabDetail` in `Panel2`, placeholder pages as `TabPage`s | `splitMain.Panel1` / `Panel2`; `tabEditors` … `tabWidgets`; `MainPage.EnsurePage()` |
| `btnNew`, `btnRefresh`, `btnSave` on the ToolBar and a `ContextMenu` on the navigation list, one-line handlers | `toolBar_ButtonClick` → `NewRecord()` / `RefreshCurrentSection()` / `SaveCurrentSection()`; `navContextMenu` |
| `RecordHeader` and `StatusStrip` exposing only `Title`, `RecordCount`, `LastRefresh`, `RefreshRequested` | `Shell/RecordHeader.*`, `Shell/StatusStrip.*`; used in `LayoutsPage.UpdateReusableControls()` |
| Narrow profile, `ApplyNarrowProfile(bool narrow)` collapsing `Panel1` and showing `btnMobileMenu` | `ClientProfiles.json`; `MainPage.ApplyNarrowProfile()`, `ShowSectionsMenu()` |
| StatusBar shows real status from the same command methods | `pnlProfile`, `pnlRecords`, `pnlRecord`, `pnlControl`, `pnlRefresh` |
| Show every path: success, validation and error for Refresh and Save | `LayoutsPage.Save()` → `SectionSaveResult.Ok / Rejected / Failed`; `chkSimulateServiceFailure` |
| Note on container and UserControl decisions | `docs/ContainerDecisions.md` |

## Self-check answers

- **If the SplitContainer were added before the ToolBar and StatusBar, what would the user see?**
  The `Fill` control would take the whole page and the bars, docked afterwards, would overlap it: the ToolBar over
  the top of the split panel, the StatusBar over its last row. In the designer the serialised `Controls.Add` list
  reads the other way round, so the `Fill` control is written first and the bars last.
- **Which UserControl members could you rename without touching a consuming page?**
  Every private child (`lblTitle`, `lblCount`, `lblRefreshed`, `btnRefresh`, `lblStrip`, `btnStripRefresh`) and the
  private fields and methods. Not `Title`, `RecordCount`, `LastRefresh` or `RefreshRequested`: that is the contract.
- **Which properties changed on the narrow profile, and why no separate narrow screen?**
  `splitMain.Panel1Collapsed`, `btnMobileMenu.Visible`, the `MinWidth` of the diagnostic StatusBar panels, and the
  text of `pnlProfile`. The workflow does not change on a small screen, only the space; two screens would drift.
