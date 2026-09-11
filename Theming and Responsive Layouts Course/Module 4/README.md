# AdaptiveOps · Theming & Responsive Layouts Course · Module 4

Lab build for **Module 4 · Layout Fundamentals: Docking, Anchoring, AutoSize, AutoScroll, and Shell
Composition**. The Adaptive Operations Console shell is five docked regions in the right child order
(toolbar Top, status Bottom, navigation Left, details Right, workspace Fill), with `Padding` for the gaps,
`MinimumSize` on the workspace and the grid, `MaximumSize` on the details region, and no code that sets
`Bounds`. Four regions are UserControls with their own local layout (`Shell/NavigationRail`,
`Shell/DetailsEditor`, `Shell/Workspace`, `Shell/StatusBar`); the details editor scrolls through
`AutoScroll` with hidden scrollbars and anchors its fields and its Save / Cancel buttons.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 4/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5504
```

Then open <http://localhost:5504> (or open `AdaptiveOps.slnx` in Visual Studio and press F5).

## What to try

- **Drag the browser** from 1366 px down to 576 px and back: only the workspace width changes; it never
  drops below 320 × 240 (below that the page scrolls, `Page.AutoScroll`). The status bar shows `Width: N px`.
- **Make the window short**: the details fields scroll by wheel or touch with no scrollbar; the Save and
  Cancel buttons stay pinned to the bottom-right of the editor. In a very short window the rail scrolls too.
- **Edit a ticket and Save**: the grid and the metric cards follow; the status bar reads `Saved T-1042`.
- **Clear the Title and Save**: the server rejects it; the status bar reads `Not saved: Title is required.`
  Any other exception during the save is caught the same way (`Save failed: …`).
- **Cancel** restores the ticket as it was loaded into the editor.
- **Click a rail section**: the workspace heading follows (`NavigationRail.SelectedSection`).

## Where things live

```
AdaptiveOps/
├─ MainPage.cs                 loads tickets, hands them to the regions, saves on DetailsEditor.Saved (no Bounds)
├─ MainPage.Designer.cs        the five regions: Dock, child order, Padding, MinimumSize / MaximumSize, Page.AutoScroll
├─ Shell/
│  ├─ NavigationRail.cs        Dock=Left region: buttons Anchor T|L|R in an AutoScroll rail; SelectedSection + SectionChanged
│  ├─ DetailsEditor.cs         Dock=Right region: header · scrolling fields (AutoScroll, hidden bars) · Save/Cancel B|R; Ticket + Saved
│  ├─ Workspace.cs             Dock=Fill region: metric cards above the grid (MinimumSize 300×160)
│  └─ StatusBar.cs             Dock=Bottom region: status text and the browser width
├─ Models/                     the same 12 tickets as every module
└─ docs/
   ├─ ShellComposition.md      the container tree and every Dock / Anchor / AutoScroll / Padding / MinimumSize decision
   └─ LayoutComparison.md      resize code vs containers: before / after code and the side-by-side table
```

## Lab step → code map

| Lab step | Where |
|---|---|
| Find every method that sets `Bounds`, list the five regions and their edges, delete the code | `docs/ShellComposition.md` §1; no such code in the project |
| Dock the five panels and fix the child order (BringToFront / SendToBack) | the five `Controls.Add` lines at the end of `MainPage.InitializeComponent()` |
| Padding for the gaps (no Margin), MinimumSize on the workspace and the grid | `Padding` on every region panel; `workspacePanel.MinimumSize = 320×240`; `gridTickets.MinimumSize = 300×160` (`Shell/Workspace.Designer.cs`) |
| Anchor captions T\|L, editors L\|R, notes T\|L\|R, buttons B\|R; AutoScroll, ScrollBars.Hidden, AutoScrollMargin | `Shell/DetailsEditor.Designer.cs` (`fieldsPanel`, `commandBar`) |
| Extract `NavigationRail` and `DetailsEditor` with `Ticket`, `Saved`, `SelectedSection`; children private | `Shell/NavigationRail.cs`, `Shell/DetailsEditor.cs` |
| `btnSave_Click` validates, raises `Saved`, reports through the status region, try / catch | `DetailsEditor.btnSave_Click` raises `Saved`; `MainPage.detailsEditor_Saved` saves in try / catch and calls `statusBar.ShowStatus` |
| Review against the smell list; note on docking order, Padding and MinimumSize | `docs/ShellComposition.md`, `docs/LayoutComparison.md` |

## Self-check answers (lab guide)

- **If the workspace were docked before the status panel, what would the user see at the bottom of the grid,
  and which designer command fixes it without changing a size?** The workspace (Fill) would take the whole
  page and the status panel would be laid out over its bottom strip, covering the last grid row. The fix is a
  child-order change: **SendToBack** on the workspace (or **BringToFront** on the edge regions).
- **You set a 12-pixel Margin on the rail and no gap appeared. Why, and what does produce a gap?** The
  default layout engine ignores `Margin` on docked children (Flow, Table and Flex honour it). Gaps come from
  **Padding on the container** (every region panel here carries its gap as Padding) or a thin docked spacer.
- **Which members of the details editor must the shell know to move it under the grid on a tablet, and which
  must it never need?** The `Ticket` property and the `Saved` event, plus the layout properties every control
  has (`Dock`, `Parent`, `Visible`, `Height`, `MinimumSize`). Never a child control (`txtTitle`, the scrolling
  `fieldsPanel`, the buttons).
- **Knowledge check: what affects docking priority?** The child order in the parent's `Controls` collection:
  the control added last is docked first.
- **Knowledge check: a warning sign with AutoSize?** A container that auto-sizes from children docked or
  anchored back to it. None of the containers in this shell has AutoSize.
