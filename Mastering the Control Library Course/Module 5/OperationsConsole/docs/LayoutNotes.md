# LayoutNotes.md — the docking and anchoring plan of the Operations Console shell

**Module 3 · Containers, Layouts, Navigation, and Reuse.** Written before the designer was opened, as the lab asks:
sketch the shell, write the child order down, then build it. Everything below describes `MainPage.Designer.cs` and
`Sections/LayoutsPage.Designer.cs` as they are in this folder.

## 1. The plan

| # | Region | Control | Dock | Why |
|---|---|---|---|---|
| 1 | Command surface | `toolBar` (`ToolBar`) | `Top` | Frequently used commands (New, Refresh, Save) belong on a ToolBar spanning the full width. |
| 2 | Status surface | `statusBar` (`StatusBar`) | `Bottom` | Real status — active profile, record count, last refresh — on the bottom edge, full width. |
| 3 | Work surface | `splitMain` (`SplitContainer`) | `Fill` | Whatever is left belongs to the user: navigation left, detail right, with a splitter the user drags. |

Inside `splitMain`:

- `splitMain.Panel1` — the navigation: `navList` (`ListBox`) `Dock = Fill`.
- `splitMain.Panel2` — `tabDetail` (`TabControl`) `Dock = Fill` with one `TabPage` per **peer** section
  (`tabEditors`, `tabLayouts`, `tabListsTrees`, `tabDataGridView`, `tabDashboard`, `tabWidgets`).
- `Orientation = Vertical`, `SplitterDistance = 240`, `Panel1MinSize = 180`, `Panel2MinSize = 360`,
  `FixedPanel = Panel1` — the navigation keeps its width when the browser is resized.

Each tab hosts the section's `UserControl` (`Sections/*Page`) `Dock = Fill`, created by `SectionCatalog.CreatePage`.

## 2. The child order (and why the `Controls.Add` list looks reversed)

Docking is applied in child order, so **the order of `Controls.Add` decides the shell**: edges first, fill last.
The Wisej.NET designer serialises the reverse z-order, so in `InitializeComponent()` the plan reads bottom-up — the
`Fill` control is the first `Controls.Add` and the edge bars are the last:

```csharp
this.Controls.Add(this.splitMain);   // Fill   — added first, laid out last: takes what is left
this.Controls.Add(this.statusBar);   // Bottom — full width
this.Controls.Add(this.toolBar);     // Top    — full width, added last, laid out first
```

Adding the bars first instead makes the `Fill` control take the whole page and the bars overlap it.

## 3. What is docked, what is anchored

**Docked** — the application regions, because they must survive any browser size:

| Control | Dock | Container |
|---|---|---|
| `toolBar` | Top | `MainPage` |
| `statusBar` | Bottom | `MainPage` |
| `splitMain` | Fill | `MainPage` |
| `navList` | Fill | `splitMain.Panel1` |
| `tabDetail` | Fill | `splitMain.Panel2` |
| `Sections/*Page` | Fill | its `TabPage` |
| `recordHeader`, `statusStripLayouts`, `pnlOptions` | Top | `LayoutsPage` |
| `tblDemo` | Fill | `LayoutsPage` (added first) |

**Anchored** — controls inside a region: the editors of `tblForm` (`Left | Right`, they take the column width), and
`lblRefreshed` / `btnRefresh` inside `RecordHeader` (`Top | Right`, they stay on the right edge).

**Layout panels** instead of coordinates:

- `tblDemo` (`TableLayoutPanel`, 2 columns at 50 % / 50 %) is the frame of the Layouts section.
- `tblForm` (`TableLayoutPanel`, fixed label column + `Percent 100` editor column) aligns labels and editors.
- `flowChips` (`FlowLayoutPanel`, `LeftToRight`, `WrapContents = true`) holds the card chips; they wrap instead of
  being clipped when the column narrows.

`AutoSize` is deliberately not set on any container that fills a region: auto-sizing a large container forces a full
layout calculation on every resize and every profile change.

## 4. The narrow profile

`ClientProfiles.json` defines `Phone` (≤ 600 px), `Tablet` (601–1024 px) and `Desktop` (≥ 1025 px). `Phone` and
`Tablet` are **narrow**. `MainPage.ApplyNarrowProfile(bool narrow)` runs on load and from
`Application.ResponsiveProfileChanged`:

```csharp
private void ApplyNarrowProfile(bool narrow)
{
    splitMain.Panel1Collapsed = narrow;   // the navigation collapses (it is not destroyed)
    btnMobileMenu.Visible = narrow;       // the way back: opens the sections menu
    sepMobileMenu.Visible = narrow;
    CompactStatusBar(narrow);             // the diagnostic panels stop reserving width
    UpdateProfilePanel();                 // the StatusBar names the active profile
}
```

- The collapsed `Panel1` still holds `navList` with its items and selection; it is collapsed, never rebuilt.
- `btnMobileMenu` opens `sectionsMenu`: the six sections plus **Show the navigation panel**.
- `LayoutsPage` listens to the same event and reflows *its* content (`tblDemo` 2 columns → 1 column).

There is no separate narrow screen: the workflow does not change, only how much room the navigation gets.

## 5. Evidence — what the running app shows

| What the note claims | What you see at <http://localhost:5705> |
|---|---|
| ToolBar Top / StatusBar Bottom / SplitContainer Fill | The ToolBar spans the top, the StatusBar the bottom, the split area sits between them; nothing overlaps at any width. |
| Navigation in `Panel1`, `TabControl` in `Panel2` | The list on the left and the six tabs on the right; the splitter stops at 180 px. |
| Selecting either surface selects the other | Click **Dashboard** in the list → the Dashboard tab is selected, and the other way round. |
| The narrow profile | Resize under 1025 px: the navigation collapses, **☰ Sections** appears on the ToolBar and the StatusBar reads `Narrow profile · Tablet`. Widen again and everything comes back with the selection intact. |
| Layout panels | Drag the splitter: the chips rewrap and the form editors change width while the captions stay put. |
