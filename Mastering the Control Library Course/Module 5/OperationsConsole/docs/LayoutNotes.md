# LayoutNotes.md — the docking and anchoring plan of the Operations Console shell

**Module 3 · Containers, Layouts, Navigation, and Reuse.** This file was written *before* the designer was
opened, exactly as the lab asks: sketch the shell, write the child order down, then build it. Everything below
describes `MainPage.Designer.cs` and `Sections/LayoutsPage.Designer.cs` as they are in this folder.

---

## 1. The plan (written first)

The shell is three regions, and the order in which they claim space is the whole design:

| # | Region | Control | Dock | Why |
|---|---|---|---|---|
| 1 | Command surface | `toolBar` (`Wisej.Web.ToolBar`) | `Top` | Frequently used commands (New, Refresh, Save) belong on a ToolBar, and a ToolBar claims the full width of the page. |
| 2 | Status surface | `statusBar` (`Wisej.Web.StatusBar`) | `Bottom` | Real status — active profile, record count, last refresh — lives on the bottom edge and must span the same full width. |
| 3 | Work surface | `splitMain` (`Wisej.Web.SplitContainer`) | `Fill` | Whatever is left belongs to the user: navigation on the left, detail on the right, with a splitter the user drags. |

Plus one instrument that is not part of the production shell:

| # | Region | Control | Dock | Why |
|---|---|---|---|---|
| 4 | Event log card | `pnlEventLog` (`Panel`) | `Right` | The lab instrument defined in Module 1 (`ConsoleLog` writes here). It sits between the two bars, next to `splitMain`. |

Inside `splitMain`:

- `splitMain.Panel1` — the navigation: `navList` (`ListBox`) `Dock = Fill`, `lblSections` (`Label`) `Dock = Top`.
- `splitMain.Panel2` — the detail surface: `tabDetail` (`TabControl`) `Dock = Fill` with one `TabPage` per **peer** section
  (`tabEditors`, `tabLayouts`, `tabListsTrees`, `tabDataGridView`, `tabDashboard`, `tabWidgets`).
- `splitMain.Orientation = Vertical`, `SplitterDistance = 240`, `Panel1MinSize = 180`, `Panel2MinSize = 360`,
  `FixedPanel = Panel1` — the navigation keeps its width when the browser is resized; the detail surface takes the change.

Each tab hosts the section's `UserControl` (`Sections/*Page`) `Dock = Fill`, created by `SectionCatalog.CreatePage`.
A tab page is a container, not a step: the six sections are peers, none of them is a required stage of a workflow,
and no required field is hidden behind a tab.

## 2. The child order (and why the `Controls.Add` list looks reversed)

Docking is applied in child order, so **the order of `Controls.Add` decides the shell**. The conceptual order is
"edges first, fill last": the ToolBar takes the top strip, the StatusBar the bottom strip, the Event log card the
right strip, and the SplitContainer fills the rectangle that is left.

The Wisej.NET designer serialises the **reverse z-order**, so in `InitializeComponent()` the same plan reads
bottom-up — the `Fill` control is the **first** `Controls.Add` and the edge bars are the **last**:

```csharp
// MainPage.Designer.cs — docking is applied from the LAST added control to the FIRST
this.Controls.Add(this.splitMain);     // Fill      — added first, laid out last: takes what is left
this.Controls.Add(this.pnlEventLog);   // Right 320 — the lab's Event log card
this.Controls.Add(this.statusBar);     // Bottom    — full width
this.Controls.Add(this.toolBar);       // Top       — full width, added last, laid out first
```

Verified in Module 1 of this course: adding the bars first instead makes the Left/Right panels claim the full
height and a `Right`-docked child overlap its `Fill` sibling — the exact "wrong order" picture the module video
shows (ToolBar overlapping the split panel, StatusBar hiding the last row).

The same rule holds inside every container:

```csharp
this.splitMain.Panel1.Controls.Add(this.navList);      // Fill first
this.splitMain.Panel1.Controls.Add(this.lblSections);  // Top last  → the caption strip wins the top edge
```

## 3. What is docked, what is anchored

**Docked** — the application regions, because they must survive any browser size:

| Control | Dock | Container |
|---|---|---|
| `toolBar` | Top | `MainPage` |
| `statusBar` | Bottom | `MainPage` |
| `pnlEventLog` | Right (320) | `MainPage` |
| `splitMain` | Fill | `MainPage` |
| `lblSections` | Top | `splitMain.Panel1` |
| `navList` | Fill | `splitMain.Panel1` |
| `tabDetail` | Fill | `splitMain.Panel2` |
| `Sections/*Page` | Fill | its `TabPage` |
| `recordHeader`, `statusStripLayouts`, `pnlCommands` | Top | `LayoutsPage` |
| `tblDemo` | Fill | `LayoutsPage` (added first) |

**Anchored** — the controls *inside* those regions, where the position is relative and the region already has a size:

| Control | Anchor | Effect |
|---|---|---|
| `lstEventLog` | Top, Bottom, Left, Right | grows in both directions with the Event log card |
| `btnClearLog` | Bottom, Left | stays on the bottom edge of the card |
| `lblEventLogTitle` | Top, Left, Right | stretches with the card width |
| `lblAnchorStretch` (Layouts demo) | Top, Left, Right | changes width with the panel — the classic "stretches" case |
| `btnAnchorRight` (Layouts demo) | Top, Right | keeps its distance from the right edge, keeps its width |
| `btnAnchorBottom` (Layouts demo) | Bottom, Right | stays in the bottom-right corner |
| `lblAnchorFloat` (Layouts demo) | *(none)* | floats: keeps its relative placement, changes nothing |

**Layout panels** — for collections and forms, instead of coordinates:

- `tblDemo` (`TableLayoutPanel`, 2 columns × 2 rows, `ColumnStyles` 50 % / 50 %) is the frame of the Layouts demo.
- `tblForm` (`TableLayoutPanel`, 2 columns: `AutoSize` label column + `Percent 100` editor column) aligns labels and editors.
- `flowChips` (`FlowLayoutPanel`, `FlowDirection = LeftToRight`, `WrapContents = true`) holds the section chips; they wrap
  instead of being clipped when the column narrows.

`AutoSize` is deliberately **not** set on `tblDemo`, on the grid-like panels or on any container that fills a region:
auto-sizing a large container forces a full layout calculation on every resize and on every profile change.

## 4. The narrow profile

`ClientProfiles.json` (project root, copied to the output folder) defines the profiles:

```json
{ "profiles": [
  { "name": "Phone",   "maxWidth": 600 },
  { "name": "Tablet",  "minWidth": 601, "maxWidth": 1024 },
  { "name": "Desktop", "minWidth": 1025 } ] }
```

`Phone` and `Tablet` are **narrow**. `MainPage.ApplyNarrowProfile(bool narrow)` runs on load and from
`Application.ResponsiveProfileChanged`, and changes four things — the desktop layout is not rebuilt:

```csharp
private void ApplyNarrowProfile(bool narrow)
{
    this.splitMain.Panel1Collapsed = narrow;   // 1 · the navigation collapses (it is not destroyed)
    this.btnMobileMenu.Visible = narrow;       // 2 · the way back: a ToolBar button that opens the sections menu
    CompactStatusBar(narrow);                  // 3 · column display: the diagnostic panels stop reserving width
    UpdateProfilePanel();                      // 4 · the StatusBar names the active profile
}
```

`CompactStatusBar` only moves `MinWidth` to 0 on `pnlControl` and `pnlRecord`; because both are
`StatusBarPanelAutoSize.Contents` they then take exactly what their text needs. No panel is removed, so nothing
has to be re-created when the browser widens again — the same "collapse, do not rebuild" rule as the navigation.

- The collapsed `Panel1` still holds `navList` — a server-side control with its items, selection and events. It is
  collapsed, never rebuilt, so nothing is lost and nothing has to be re-created when the browser widens again.
- `btnMobileMenu` opens `sectionsMenu`, a `ContextMenu` with the same six sections plus **Show the navigation panel**,
  which expands `Panel1` again. A collapsed navigation always needs a way back.
- The StatusBar shows `Narrow profile · Phone` / `Desktop profile · Desktop`, so support knows which layout the user sees.
- `btnForceNarrow` (a `ToggleButton` on the ToolBar) forces the same path at any browser width, so the narrow layout can be
  reviewed without resizing the window.
- `LayoutsPage` listens to the same event on its own and reflows *its* content (`tblDemo` 2 columns → 1 column,
  `flowChips` wraps to a single column). A section is responsible for its own content; the shell is responsible for the shell.

Nothing else changes: same `Page`, same controls, same tabs, same commands. There is no separate narrow screen because the
workflow does not change — only how much room the navigation gets.

## 5. Evidence — what the running app shows

| What the note claims | What you see at <http://localhost:5703> |
|---|---|
| ToolBar Top / StatusBar Bottom / SplitContainer Fill, in that order | The ToolBar spans the full width at the top, the StatusBar the full width at the bottom, the split area sits between them, and the Event log card is on the right between the two bars. Nothing overlaps at any width. |
| The `Fill` control is the first `Controls.Add` | Startup log line: `child order: splitMain (Fill) added first, then pnlEventLog (Right), statusBar (Bottom), toolBar (Top) — docking is applied last-to-first`. |
| Navigation in `Panel1`, `TabControl` in `Panel2` | The list on the left and the six tabs on the right; dragging the splitter moves the boundary, and `Panel1MinSize` stops it at 180 px. |
| Selecting either surface selects the other | Click **Dashboard** in the list → the Dashboard tab is selected; click the **Widgets** tab → the list selection moves to Widgets. Both log `navList → Dashboard` / `tabDetail → Widgets`. |
| Anchoring inside a region | Drag the splitter left/right: `lblAnchorStretch` changes width, `btnAnchorRight` keeps its width and its distance from the right edge, `lblAnchorFloat` does not move relative to the panel. |
| The narrow profile changes four properties | Press **Force narrow** (or resize under 1025 px): the navigation panel collapses, **☰ Sections** appears on the ToolBar, the two diagnostic StatusBar panels shrink to their text, and the StatusBar reads `Narrow profile · …`. Press it again and everything comes back with the selection intact. |
| A collapsed panel is not a destroyed panel | Select **Widgets**, force narrow, then expand again: the list still has Widgets selected. |
| `AutoSize` is not set on containers | Resizing the browser reflows instantly; no layout stall on the profile change. |
