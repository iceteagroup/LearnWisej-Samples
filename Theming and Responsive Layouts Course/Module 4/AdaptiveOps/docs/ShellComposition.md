# Shell composition — the Adaptive Operations Console as nested containers

Lab deliverable for Module 4 (*Layout Fundamentals: Docking, Anchoring, AutoSize, AutoScroll, and
Shell Composition*). It records the container tree of the running console, the Dock / Anchor /
AutoSize / AutoScroll / Padding / MinimumSize decision behind every node and why it was taken, and
what the app shows when each lab path is exercised. The before/after comparison with the
resize-code shell is in [`LayoutComparison.md`](LayoutComparison.md).

## 1. The five regions and the edge each one claims

Written down first, as the lab asks, before any code was touched:

| Region | Edge it claims | Size | Child index in `MainPage.Controls` | Docked … |
|---|---|---|---|---|
| `toolbarPanel` | Top | 56 px high | 4 (added last) | first |
| `statusPanel` | Bottom | 28 px high | 3 | second |
| `navigationPanel` | Left | 220 px wide | 2 | third |
| `detailsPanel` | Right | 340 px wide (Min 260, Max 480) | 1 | fourth |
| `workspacePanel` | Fill — what is left | MinimumSize 320 × 240 | 0 (added first) | last |

Docking priority follows the child order: the control added **last** is docked **first** against
the page edges. That is why `InitializeComponent()` in `MainPage.Designer.cs` ends with

```csharp
this.Controls.Add(this.workspacePanel);   // index 0 → docked last  (Fill takes what is left)
this.Controls.Add(this.detailsPanel);     // index 1 → Right
this.Controls.Add(this.navigationPanel);  // index 2 → Left
this.Controls.Add(this.statusPanel);      // index 3 → Bottom
this.Controls.Add(this.toolbarPanel);     // index 4 → docked first (Top)
```

and why `ComposeShell()` in `MainPage.cs` re-applies the same order with `Controls.SetChildIndex`
before it touches a single Dock value. In the Visual Studio designer the same order is set with
**BringToFront** / **SendToBack**: the workspace goes to the back, the four edges to the front.

No `Resize`, `Layout` or load-time method in `MainPage` sets `Bounds`, `Location` or `Size` on a
region. `MainPage_Resize` and `Application.BrowserSizeChanged` only *report* (section 7).

## 2. The container tree

```
MainPage : Page                         AutoScroll = true (the page scrolls when the minimum sizes exceed the viewport)
│                                       BackColor = page grey until Module 2's theme owns it
├─ toolbarPanel      Panel  Dock=Top      56    Padding (8,8,8,4)   ← the gap around the card
│  └─ toolbarCard    Panel  Dock=Fill           AutoScroll + ScrollBars.Hidden (the button row scrolls at narrow widths)
│     ├─ lblAppTitle … btnClearTrace          absolute Location — fine inside a fixed-height bar
│     └─ lblProgress                          fixed size + AutoEllipsis
├─ statusPanel       Panel  Dock=Bottom   28    Padding (8,0,8,4)
│  └─ statusBar      UserControl Dock=Fill      Shell/StatusBar — three docked labels: Left · Fill · Right
├─ navigationPanel   Panel  Dock=Left     220   Padding (8,4,0,4) · Margin (12) — ignored by Dock on purpose (section 4)
│  └─ navigationRail UserControl Dock=Fill      Shell/NavigationRail — AutoScroll, ScrollBars.Hidden, AutoScrollMargin (0,12)
│     ├─ lblNavTitle                          AutoSize = true
│     ├─ 7 × Button                           Anchor Top|Left|Right (stretch with the rail, stay put vertically)
│     └─ lblSelected                          AutoSize = false + AutoEllipsis, Anchor Top|Left|Right
├─ detailsPanel      Panel  Dock=Right    340   Padding (0,4,8,4) · MinimumSize 260 · MaximumSize 480
│  └─ detailsEditor  UserControl Dock=Fill      Shell/DetailsEditor — three nested containers:
│     ├─ headerPanel   Panel Dock=Top   62       lblDetailsTitle AutoSize · lblDetailsSubtitle fixed + AutoEllipsis, Anchor T|L|R
│     ├─ fieldsPanel   Panel Dock=Fill           AutoScroll, ScrollBars.Hidden, AutoScrollMargin (0,24)   ← the scrolling part
│     │   ├─ captions                          AutoSize labels, Anchor Top|Left (second column Top|Right)
│     │   ├─ txtTitle, txtId, txtNotes         Anchor Top|Left|Right (stretch)
│     │   ├─ cboPriority, txtOwner             Anchor Top|Left (fixed 146)
│     │   └─ cboStatus, dtpDue                 Anchor Top|Right (keep their distance from the right edge)
│     └─ commandBar    Panel Dock=Bottom 48     btnCancel, btnSave Anchor Bottom|Right · lblHint Anchor Bottom|Left|Right
└─ workspacePanel    Panel  Dock=Fill           Padding (8,4,8,4) · MinimumSize 320×240
   └─ workspace      UserControl Dock=Fill      Shell/Workspace — BorderStyle None, no AutoSize (its children dock back to it)
      ├─ metricsPanel  Panel Dock=Top    84     four slots Dock=Left, slot Padding (0,0,8,0) is the gap; card titles AutoSize, values fixed
      ├─ bannerPanel   Panel Dock=Top    38     Visible = false until an error (a hidden docked control takes no space)
      ├─ tabs          TabControl Dock=Fill
      │   ├─ tabTickets   Padding 8             lblWorkspaceTitle Dock=Top · gridTickets Dock=Fill, MinimumSize 300×160
      │   └─ tabTwin      Padding 4             Lab/ResizeCodeTwin Dock=Fill — the "before" (LayoutComparison.md)
      └─ tracePanel    Panel Dock=Bottom 176    Padding (0,8,0,0) · traceCard · listTrace ("Layout & theme · live trace")
```

Every region is a Panel that hosts either a card or a UserControl docked Fill. The region panel is
the Dock unit and carries the Padding (the gap); the UserControl inside owns its local layout. The
nesting is what makes the shell stable: the page does not know how the rail arranges its buttons,
and the rail does not know it sits next to a grid.

## 3. Decision table

| Decision | Where | Why |
|---|---|---|
| `Dock` on the five regions, nothing else | `MainPage.Designer.cs` | The engine hands out space in child order and re-applies it on every resize on the client. One rule per region replaces a handler that ran after every paint. |
| Child order workspace → details → rail → status → toolbar | `MainPage.Designer.cs`, `ComposeShell()` | Edges are claimed first, Fill takes what is left. Swapping the workspace to the end makes it dock first and the other regions cover it (evidence, path 3). |
| `Padding` on the page-level region panels, `Margin` untouched (and one deliberate `Margin = 12`) | region panels | Docked children fill the parent's `DisplayRectangle`, which Padding reduces; the default engine ignores Margin on docked children (section 4). |
| `MinimumSize 320×240` on `workspacePanel`, `300×160` on `gridTickets` | Designer | The fill region and the grid never collapse into a sliver when the browser narrows; below the sum of minimums the page scrolls (`Page.AutoScroll`) instead of overlapping. |
| `MinimumSize 260` / `MaximumSize 480` on `detailsPanel` | Designer | 260 keeps the two-column editor usable; 480 stops the details region from swallowing an ultra-wide monitor. "Animate details" runs into both (evidence, path 2). |
| `AutoScroll` + `ScrollBars.Hidden` on `fieldsPanel` and on the rail | `DetailsEditor.Designer.cs`, `NavigationRail.Designer.cs` | Content that does not fit scrolls instead of being clipped or hidden; hidden bars keep touch scrolling on phones without stealing width from the anchored editors. `AutoScrollMargin` gives breathing room after the last field. |
| Command buttons in a `Dock=Bottom` bar, not inside the scrolling panel | `DetailsEditor.Designer.cs` | Inside an AutoScroll container "Bottom" is the virtual bottom of the content: a Bottom-anchored Save would scroll away. A fixed-height bar makes `Bottom\|Right` stable. |
| Anchors inside the editor: captions T\|L, wide editors T\|L\|R, second column T\|R, buttons B\|R | `DetailsEditor.Designer.cs` | The pack's rule for form-like regions. Nothing is anchored Bottom inside the scrolling panel; nothing mixes Dock and Anchor. |
| `AutoSize = true` on captions and card titles only | all UserControls | Content decides the size of leaf labels (translated captions, section names). Subtitles, values and status labels are fixed + `AutoEllipsis` so the container decides. No container has AutoSize while its children dock or anchor back to it — the circular-layout smell. |
| `Workspace.BorderStyle = None`, page-grey background | `Workspace.Designer.cs` | It is a transparent Dock container, not a card; the cards are its children. `UserControl.BorderStyle` defaults to Solid, so the three card-like regions keep it and the workspace turns it off. |
| `toolbarCard.AutoScroll` with hidden bars | `MainPage.Designer.cs` | The button row is absolute-positioned inside a fixed-height bar (fine for a bar); at 576 px it scrolls sideways instead of clipping. Module 5 replaces it with a `FlowLayoutPanel`. |

## 4. Padding vs Margin under Dock

`navigationPanel.Margin` is set to 12 on purpose and the rail still sits at the page's left edge;
the visible gap beside it is `navigationPanel.Padding.Left = 8`. `MainPage_Load` logs the proof:

```
• server Margin ignored by Dock: navigationPanel.Margin=12 but Left=0, Top=56 (toolbar height) — the 8 px gap is Padding
```

The two mechanisms that do produce a gap between docked regions are **Padding on the container**
(used everywhere here: the page-level region panels, `metricsPanel`, the metric slots, `tracePanel`)
and **a thin docked spacer Panel** (not needed in this shell). Margin is honoured by the Flow, Table
and Flex engines of Module 5, not by the default engine's docking.

## 5. The UserControls' public surface

| UserControl | Public members the shell uses | Private |
|---|---|---|
| `NavigationRail` | `SelectedSection` (string), `SectionChanged` (event), `DescribeScroll()` | seven buttons, title, footer label |
| `DetailsEditor` | `Ticket` (get builds a Ticket from the fields; set fills or clears), `Saved` (`TicketSaveEventArgs` with `Ticket` and a settable `Error`), `Cancelled`, `DescribeScroll()` | header, fields panel, eight editors, command bar, buttons |
| `Workspace` | `SetMetrics`, `ShowTickets`, `SelectedTicketId`, `SelectionChanged`, `Title`, `ShowBanner` / `HideBanner`, `AddTrace` / `ClearTrace`, `DescribeGrid()` | cards, banner, tabs, grid, columns, trace list, the twin |
| `StatusBar` | `ShowStatus(text, kind)`, `BrowserText`, `ThemeText` | three labels |

To move the details editor under the grid on a tablet (Module 6) the shell needs `Ticket`, `Saved`
and the region's `Dock`; it never needs a text box, a caption, the scroll panel or a button.
`MainPage.cs` compiles without a single reference to a child of any UserControl.

## 6. Smell-list review of the running shell

| Smell | Status |
|---|---|
| Resize code setting Bounds on many controls | none in the shell — the only Bounds lines are in `Lab/ResizeCodeTwin.cs`, kept deliberately as the "before" |
| Dock and Anchor fighting on one control | none — "Dock+Anchor fight" creates one at runtime and logs what happens, Restore removes it |
| Absolute coordinates on a screen that must run on a phone | only inside fixed-height bars (toolbar row, command bar) and inside the scrolling fields panel, all anchored |
| Grid without MinimumSize | `gridTickets.MinimumSize = 300×160`, `workspacePanel.MinimumSize = 320×240` |
| AutoSize + Fill loops | no container has AutoSize; AutoSize is on leaf labels only |
| Hiding controls instead of scrolling | the eight fields and the seven rail entries scroll (`AutoScroll`, hidden bars) |

## 7. Evidence — what the running app shows

Every line below appears in the **Layout & theme · live trace** card (`Workspace.AddTrace`), the
status bar or the shell itself. Screenshots are taken by the learner: `screenshots/module4-before-desktop.png`
(twin tab at 1366 px), `module4-after-desktop.png`, `module4-after-1100.png`, `module4-after-576.png`.

**Page load.** `• server shell built from nested containers: toolbar Dock=Top 1348×56 · rail Dock=Left 220×596 · workspace Dock=Fill 788×596 min 320×240 · details Dock=Right 340×596 min 260 max 480 · status Dock=Bottom 1348×28`, then `• server composition verified …`, the Margin line of section 4, the AutoSize line, and the first `← client resize (page load)` block with the rail and details scroll state (`… fields 476 visible / 454 content → fits`).

**Path 1 — success, Compose shell (default).** `• server composition applied (Compose shell): child order [workspace, details, rail, status, toolbar] → docked Top, Bottom, Left, Right, Fill · …` followed by the verified line. Dragging the browser edge afterwards adds one `← client resize (Application.BrowserSizeChanged)` and one `(Page.Resize)` block per resize, each with the five region sizes; the workspace width is the only number that changes with the browser width, the rail, details and bars keep theirs. Below ≈ 600 px of browser height the details line turns into `… → scrolls by N px (AutoScroll, ScrollBars=Hidden)` and the fields scroll by wheel or touch with no scrollbar drawn; below ≈ 480 px the rail line does the same.

**Path 2 — progress, Animate details.** Sixteen `→ render step k/16: details.Width requested 240 → actual 260 (clamped by MinimumSize) · workspace 868×596 · …` lines every 350 ms. The requests 240 and 220 report 260, the requests 520 and 560 report 480 `(clamped by MaximumSize)`; the workspace width in every line equals page width − 16 − 220 − details width, because it is docked Fill and follows without code. The rail, toolbar and status bar never move. *(docs) — the server-side clamp of `Width` by `MinimumSize` / `MaximumSize` is a documented behaviour; check in the browser that the details region visibly stops at 260 and 480.*

**Path 3 — failure, Swap dock order.** `✕ wrong dock order: workspace moved to child index 4 (docked FIRST) → workspace 1348×680 = the whole page; toolbar covers the metric cards, status bar covers the last grid row, rail and details cover the sides` and `✕ server composition BROKEN: … child order wrong — workspace is not docked last`. On screen the workspace card sits under the toolbar and the status bar; the last grid row disappears behind the status bar, the lesson's picture. The status label reads `● dock order wrong`.

**Path 4 — failure, Dock+Anchor fight.** `✕ Dock+Anchor fight: details region had Dock=Right Anchor=Top, Left; after Anchor = Top|Bottom|Right it reports Dock=… Anchor=…` followed by one of two explanations depending on what the framework did: if Dock became `None` the region floats at its last bounds and the Fill workspace extends under it on the next resize; if Dock survived, the Anchor is stored but never applied. *(docs) — which of the two happens is what the trace exists to show; the reviewer reads the line.*

**Path 5 — failure, Squeeze workspace.** `✕ MinimumSize violated: workspace MinimumSize set to 1012×240 but only 772 px are left → workspace reports 1012×596; it cannot shrink, so it runs under the details region and the page (AutoScroll) shows a horizontal scrollbar` plus `this is what the real minimum (320) does when the browser is narrower than 896 px`. A horizontal scrollbar appears on the page and the details region is partly covered. *(docs) — `Page.AutoScroll` growing scrollbars when a docked child's minimum exceeds the viewport is documented ScrollableControl behaviour; verify the scrollbar appears.*

**Path 6 — recovery, Restore.** `• server repository reset to the seed tickets; banner cleared`, the composition-applied and verified lines, `→ render Restore: 12 tickets …`, status `● ready`. Order, docks, widths and MinimumSize are back; the page scrollbar is gone.

**Saving.** A valid Save: `• server saved T-1042 …`, `✓ saved 10:21:04` next to the buttons, grid and metrics re-rendered. An empty title: `• server rejected T-1042: Title is required.`, the red banner pushes the tabs down (a docked panel became visible — no Bounds), the editor hint shows `✖ Title is required.`. Owner `fault`: `• server threw InvalidOperationException while saving T-1042: Simulated storage fault … — caught around DetailsEditor.Saved`, banner and hint show the message, the layout is untouched.

**Rail.** Clicking *Reports*: `← client navigation: NavigationRail.SelectedSection = Reports (the rail's buttons are private to the UserControl)` and the workspace title changes — the page reached the rail through one property.
