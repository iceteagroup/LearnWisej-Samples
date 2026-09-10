# LayoutNotes.md — the spacing audit: which engine honours Margin, where Padding had to be used

Lab step 7: "give the cards and editors a Margin and confirm the flow, flex and table engines honour it,
then confirm the docked shell regions still get their gaps from Padding because the default layout
engine does not use margins for docking; record both findings." Both findings, with the exact values
the Module 5 build uses.

## The rule

`Padding` belongs to the **container**: the inner space around all of its children. `Margin` belongs to
the **child**: its distance to adjacent controls. Whether a Margin does anything depends on the engine
that lays the child out:

| Engine (container) | Reads the child's `Margin`? | What creates the gap |
|---|---|---|
| Default layout engine — **Dock** (and Anchor / Location) | **No** | `Padding` on the parent |
| `FlowLayoutPanel` | Yes — every child keeps the distance its Margin declares from its neighbours | child `Margin` (+ container `Padding` for the outer frame) |
| `FlexLayoutPanel` | Yes | child `Margin`, plus the container's `Spacing` between children |
| `TableLayoutPanel` | Yes, inside the cell — the Margin becomes the child's distance to the cell border | child `Margin` (+ container `Padding` for the outer frame) |

When a gap does not appear, ask which engine owns the child before changing any number.

## Finding 1 · Margin honoured (flow, flex, table children)

| Child | Container / engine | Margin | Effect in the running app |
|---|---|---|---|
| `MetricCard` (all cards) | `flowHost : FlowLayoutPanel` | `4` all round (`Shell/MetricCard.Designer.cs`) | 8 px between neighbouring cards, 4 px from the host's padding edge; each card occupies a 200 × 84 slot, which is why 3 cards fit in a 775-px host and 2 in a 450-px host |
| `MetricCard` | `tableHost : TableLayoutPanel` | `4` | the card is docked Fill **in its cell**, and the 4-px margin is its inset from the cell border on every side: the 285-px cell shows a 277-px card |
| `MetricCard` | `flexHost : FlexLayoutPanel` | `4` | 4 px inset on each card plus the container's `Spacing 8` between cards |
| `lblAppTitle`, `txtSearch`, `cboStatusFilter`, `btnApply`, the six lab buttons | `FilterBar : FlowLayoutPanel` | `(0, 0, 8, 6)` — right and bottom only (`Layout/FilterBar.Designer.cs`) | 8 px between controls on a row, 6 px between the two rows; the bar's own `Padding (8, 8, 8, 2)` adds the frame. `lblProgress` has `(0, 0, 0, 6)` so the stretched label ends flush with the right padding |
| `btnNavDashboard … btnNavHelp`, `lblNavTitle` | `navigationRail : FlowLayoutPanel` (TopDown) | `(0, 0, 0, 8)` / `(0, 0, 0, 10)` | the 8-px gaps of the rail. In Module 1 the same rail was five buttons with `Location` and `Anchor` inside a plain Panel; the flow engine made the positions disappear and the Margin do the spacing |
| Captions and editors of the ticket editor | `table : TableLayoutPanel` in `Shell/TicketEditor` | captions `(0, 0, 8, 6)`, editors `(0, 0, 0, 6)`, `txtNotes (0, 0, 0, 8)`, `lblSubtitle (0, 0, 0, 8)` | the 6-px rhythm between the rows (an AutoSize row is the tallest child **plus its Margin**), the 8-px gap between a caption and its editor, the 8-px gap above the Save row; the table's `Padding 12` is the frame |

## Finding 2 · Margin ignored — the docked shell regions use Padding

The five region panels are docked on the page and the white cards inside them are docked Fill. A
`Margin` on any of them does nothing (the default layout engine does not use margins for docking), so
every gap of the shell is a `Padding` on the region panel, exactly as in Module 4:

| Region panel | Dock | Padding | Which gaps it creates |
|---|---|---|---|
| `toolbarPanel` | Top (100) | `(8, 8, 8, 4)` | 8 px to the page edges, 4 px above the workspace / rail (their own top padding of 4 adds up to an 8-px gap) |
| `navigationPanel` | Left (220) | `(8, 4, 0, 4)` | 8 px to the left edge, 0 on the right — the workspace's left padding of 8 provides the gap between the rail and the content |
| `workspacePanel` | Fill | `(8, 4, 8, 4)` | 8 px to the rail and to the right edge, 4 + 4 px to the toolbar and the trace |
| `tracePanel` | Bottom (150) | `(8, 4, 8, 4)` | 8 px to the sides, 4 px above (with the workspace's 4) and below (with the status bar's 0) |
| `statusPanel` | Bottom (28) | `(8, 0, 8, 4)` | 8 px to the sides, 4 px to the bottom edge |

Inside the workspace the same rule applies once more: `metricsTabs`, `bannerPanel` and `dashboard` are
docked (Top, Top, Fill), so the 8-px gap under the metric tabs is the banner's `Padding (0, 8, 0, 0)`
when it is visible — and the dashboard, which is the flex container, gets its gap from the tab
control's bottom edge, not from a Margin.

A quick proof for the review: set `this.navigationPanel.Margin = new Wisej.Web.Padding(8)` in
`MainPage.Designer.cs`, run, and watch nothing move; set `Padding` to `(16, 4, 0, 4)` instead and the rail
card moves 8 px to the right.

## A consistent spacing system

The build uses three values consistently, which is the "dense / normal / spacious" idea of the lesson
applied to one console: **4** (card margins, the inset inside table cells, the vertical breathing room of
the region padding), **8** (gaps between regions and between siblings in flow / flex containers, the
`Spacing` of the dashboard) and **12** (the inner frame of the rail and of the ticket editor's table).
Changing the rhythm means changing those three numbers in the Designer files; no code computes a gap.

## Evidence (what the running app shows)

- `• server filter bar: FlowLayoutPanel LeftToRight · WrapContents True · AutoScroll True · Padding 8 · …` — the bar's padding; the 8-px gaps between the toolbar controls are visible in the browser and are the children's Margin (no Location anywhere in `FilterBar.Designer.cs`).
- `← client card bounds (after switch to Flow) · Flow: 4 cards in 1 row(s) · host …×… · open 192×76 @(8,8) · overdue 192×76 @(208,8) · …` — 208 − 8 = 200 = 192 + 4 + 4: two card margins between neighbours; the first card sits at 8 = host padding 4 + margin 4.
- `← client card bounds (after switch to Table) · Table: … col widths 285,285,285,285 · row heights 84 · open 277×76 @(8,8) …` — a 285-px cell shows a 277-px card: the 4-px margin on each side inside the cell.
- `• server dock order verified: Top, Bottom, Bottom, Left, Fill (child order in MainPage.Designer.cs) — gaps come from Padding, not Margin`.
- `• server ticket editor: TableLayoutPanel 2 cols [Absolute 140 | Percent 100] · 10 rows · … · row heights 26,26,34,34,34,34,34,28,…,32` — an AutoSize row is the child height plus its bottom Margin (28 + 6 = 34).
