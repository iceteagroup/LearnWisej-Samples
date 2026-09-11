# Shell composition · the Adaptive Operations Console as nested containers

The container tree of the running console and the Dock / Anchor / AutoSize / AutoScroll / Padding /
MinimumSize decision behind every node. The before/after comparison with the resize-code shell is in
[`LayoutComparison.md`](LayoutComparison.md).

## 1. The five regions and the edge each one claims

Written down first, before any code was touched:

| Region | Edge it claims | Size | Child index in `MainPage.Controls` | Docked |
|---|---|---|---|---|
| `toolbarPanel` | Top | 56 px high | 4 (added last) | first |
| `statusPanel` | Bottom | 28 px high | 3 | second |
| `navigationPanel` | Left | 220 px wide | 2 | third |
| `detailsPanel` | Right | 340 px wide (Min 260, Max 480) | 1 | fourth |
| `workspacePanel` | Fill: what is left | MinimumSize 320 × 240 | 0 (added first) | last |

Docking priority follows the child order: the control added **last** is docked **first**.
`InitializeComponent()` in `MainPage.Designer.cs` therefore ends with

```csharp
this.Controls.Add(this.workspacePanel);   // index 0 → docked last (Fill takes what is left)
this.Controls.Add(this.detailsPanel);     // index 1 → Right
this.Controls.Add(this.navigationPanel);  // index 2 → Left
this.Controls.Add(this.statusPanel);      // index 3 → Bottom
this.Controls.Add(this.toolbarPanel);     // index 4 → docked first (Top)
```

In the Visual Studio designer the same order is set with **BringToFront** / **SendToBack**. No method in
the project sets `Bounds`, `Location` or `Size` on a region.

## 2. The container tree

```
MainPage : Page                         AutoScroll = true (the page scrolls when the minimum sizes exceed the viewport)
├─ toolbarPanel      Panel  Dock=Top      56    Padding (8,8,8,4)
│  └─ toolbarCard    Panel  Dock=Fill           the application title
├─ statusPanel       Panel  Dock=Bottom   28    Padding (8,0,8,4)
│  └─ statusBar      UserControl Dock=Fill      Shell/StatusBar: lblStatus (Left) · widthLabel (Fill)
├─ navigationPanel   Panel  Dock=Left     220   Padding (8,4,0,4)
│  └─ navigationRail UserControl Dock=Fill      Shell/NavigationRail: AutoScroll, ScrollBars.Hidden, AutoScrollMargin (0,12)
│     ├─ lblNavTitle                          AutoSize = true
│     └─ 7 × Button                           Anchor Top|Left|Right
├─ detailsPanel      Panel  Dock=Right    340   Padding (0,4,8,4) · MinimumSize 260 · MaximumSize 480
│  └─ detailsEditor  UserControl Dock=Fill      Shell/DetailsEditor:
│     ├─ headerPanel   Panel Dock=Top   62       lblDetailsTitle AutoSize · lblDetailsSubtitle fixed + AutoEllipsis, Anchor T|L|R
│     ├─ fieldsPanel   Panel Dock=Fill           AutoScroll, ScrollBars.Hidden, AutoScrollMargin (0,24)
│     │   ├─ captions                          AutoSize labels, Anchor Top|Left (second column Top|Right)
│     │   ├─ txtTitle, txtId, txtNotes         Anchor Top|Left|Right
│     │   ├─ cboPriority, txtOwner             Anchor Top|Left
│     │   └─ cboStatus, dtpDue                 Anchor Top|Right
│     └─ commandBar    Panel Dock=Bottom 48     btnCancel, btnSave Anchor Bottom|Right
└─ workspacePanel    Panel  Dock=Fill           Padding (8,4,8,4) · MinimumSize 320×240
   └─ workspace      UserControl Dock=Fill      Shell/Workspace: BorderStyle None, no AutoSize
      ├─ metricsPanel  Panel Dock=Top    84     four slots Dock=Left, slot Padding (0,0,8,0) is the gap
      └─ gridCard      Panel Dock=Fill          lblWorkspaceTitle Dock=Top · gridTickets Dock=Fill, MinimumSize 300×160
```

Every region is a Panel that hosts a card or a UserControl docked Fill. The region panel is the Dock unit
and carries the Padding; the UserControl inside owns its local layout.

## 3. Decision table

| Decision | Where | Why |
|---|---|---|
| `Dock` on the five regions, nothing else | `MainPage.Designer.cs` | the engine hands out space in child order and re-applies it on every resize on the client |
| Child order workspace → details → rail → status → toolbar | `MainPage.Designer.cs` | edges are claimed first, Fill takes what is left |
| `Padding` on the region panels, no `Margin` | region panels | docked children fill the parent's `DisplayRectangle`, which Padding reduces; the default engine ignores Margin on docked children |
| `MinimumSize 320×240` on `workspacePanel`, `300×160` on `gridTickets` | Designer | the fill region and the grid never collapse; below the sum of minimums the page scrolls instead of overlapping |
| `MinimumSize 260` / `MaximumSize 480` on `detailsPanel` | Designer | 260 keeps the two-column editor usable; 480 stops it from swallowing an ultra-wide monitor |
| `AutoScroll` + `ScrollBars.Hidden` on `fieldsPanel` and on the rail | `DetailsEditor.Designer.cs`, `NavigationRail.Designer.cs` | content that does not fit scrolls; hidden bars keep touch scrolling without stealing width |
| Command buttons in a `Dock=Bottom` bar, not inside the scrolling panel | `DetailsEditor.Designer.cs` | inside an AutoScroll container "Bottom" is the virtual bottom of the content; a fixed-height bar makes `Bottom\|Right` stable |
| Anchors inside the editor: captions T\|L, wide editors T\|L\|R, second column T\|R, buttons B\|R | `DetailsEditor.Designer.cs` | nothing anchored Bottom inside the scrolling panel; nothing mixes Dock and Anchor |
| `AutoSize = true` on captions and card titles only | UserControls | content decides the size of leaf labels; no container has AutoSize while its children dock back to it |

## 4. The UserControls' public surface

| UserControl | Public members the shell uses | Private |
|---|---|---|
| `NavigationRail` | `SelectedSection`, `SectionChanged` | the buttons and the title |
| `DetailsEditor` | `Ticket` (get builds a Ticket from the fields; set fills or clears), `Saved` (`TicketSaveEventArgs.Ticket`) | header, fields panel, editors, command bar, buttons |
| `Workspace` | `SetMetrics`, `ShowTickets`, `SelectedTicketId`, `SelectionChanged`, `Title` | cards, grid, columns |
| `StatusBar` | `ShowStatus`, `WidthText` | the two labels |

To move the details editor under the grid on a tablet (Module 6) the shell needs `Ticket`, `Saved` and the
region's `Dock`; it never needs a text box, the scroll panel or a button.

## 5. Smell-list review

| Smell | Status |
|---|---|
| Resize code setting Bounds | none |
| Dock and Anchor on one control | none |
| Absolute coordinates on a screen that must run on a phone | only inside fixed-height bars and inside the scrolling fields panel, all anchored |
| Grid without MinimumSize | `gridTickets.MinimumSize = 300×160`, `workspacePanel.MinimumSize = 320×240` |
| AutoSize + Fill loops | no container has AutoSize |
| Hiding controls instead of scrolling | the fields and the rail entries scroll (`AutoScroll`, hidden bars) |

## 6. Evidence

Screenshots taken by the learner: `screenshots/module4-after-desktop.png`, `module4-after-1100.png`,
`module4-after-576.png`.

- **Resize 1366 → 576 px and back**: only the workspace width changes; the rail, details and bars keep
  theirs; below rail + details + 320 px the page shows a horizontal scrollbar instead of overlapping.
- **Short window**: the details fields scroll by wheel or touch with no scrollbar drawn; Save and Cancel stay
  in the command bar.
- **Save**: a valid ticket shows `Saved T-1042` in the status bar; an empty title shows
  `Not saved: Title is required.`; any other exception is caught and shown as `Save failed: …`, the layout is
  untouched.
- **Rail**: clicking *Reports* changes the workspace heading through `NavigationRail.SelectedSection`.
