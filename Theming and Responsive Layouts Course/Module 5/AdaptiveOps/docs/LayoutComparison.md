# Deliverable 5 · LayoutComparison.md — the same region in Flow, Table and Flex

The comparison note the lab asks for: which container fits each region of the Adaptive Operations
Console, which extended property did the work, and what each engine does when the browser is dragged
from 1400 to 1024 to 700 pixels. Everything below is read off the running Module 5 build
(`MainPage.cs`, `Layout/FilterBar.cs`, `Shell/TicketEditor.cs`, `Layout/DashboardWorkspace.cs`); the
numbers are the geometry the Designer files declare, and the **Evidence** section names the trace lines
that confirm them at runtime.

## 1 · The decision rule, applied

Look at the behaviour, not the visual. The rule from the lesson — flow for content that wraps, table for
structured forms, flex for regions that share space proportionally, docking for the stable shell — gives
one container per region:

| Region | Behaviour the region needs | Container | Extended properties that do the work | Where |
|---|---|---|---|---|
| Toolbar / filter bar | "as many controls per row as fit, the search box takes what is left, the lab buttons always start a new row" | `FlowLayoutPanel` (`FilterBar : FlowLayoutPanel`) | `FlowDirection = LeftToRight`, `WrapContents = true`, `SetFillWeight(txtSearch, 1)` + `MinimumSize 180`, `SetFlowBreak(btnApply, true)`, `SetFillWeight(lblProgress, 1)`, each child's `Margin (0,0,8,6)` | `Layout/FilterBar.Designer.cs` |
| Ticket details editor | "every editor's left edge on the same line, however long the caption; the editors grow with the panel; Notes takes the remaining height" | `TableLayoutPanel` inside `TicketEditor : UserControl` | `ColumnStyles` = `ColumnStyle(Absolute, 140)` + `ColumnStyle(Percent, 100)`; `RowStyles` = nine `AutoSize` + one `Percent 100` (Notes); `Controls.Add(control, column, row)`; `SetColumnSpan(txtNotes, 2)`; every editor `Dock = Fill` **inside its cell**; `txtNotes.MinimumSize = (0, 120)`; `GrowStyle = FixedSize` | `Shell/TicketEditor.Designer.cs` |
| List / details split of the workspace | "the list takes two thirds, the details one third, and neither collapses" | `FlexLayoutPanel` (`DashboardWorkspace : FlexLayoutPanel`) | `LayoutStyle = Horizontal`, `Spacing = 8`, `FillWeight(list, 2)`, `FillWeight(details, 1)`, `MinimumSize` 300×200 / 280×200, `MaximumSize` 520×0 on details, `AlignY(details, Top)` — written as one `Wisej.Web.Markup` chain | `Layout/DashboardWorkspace.cs` |
| Metric cards ("Same region, three ways") | "as many cards per row as fit, then wrap" | `FlowLayoutPanel` wins; the Table and Flex hosts are kept so the difference can be watched | Flow: nothing but `Margin 4` (FillWeight 0 = own size). Table: `4 × ColumnStyle(Percent, 25)`, `RowStyle(Absolute, 84)`, `GrowStyle = AddRows`, cards `Dock = Fill` in their cells. Flex: `SetFillWeight(card, 1)`, `SetAlignY(card, Top)`, `Spacing 8`, cards `MinimumSize 140×76` | `MainPage.Designer.cs` (hosts), `MainPage.SwitchEngine()` (per-host card properties) |
| Navigation rail | "one column of buttons, top to bottom" | `FlowLayoutPanel` with `FlowDirection = TopDown`, `WrapContents = false` | each button's `Margin (0,0,0,8)`; no `Anchor` (the flow engine ignores it) | `MainPage.Designer.cs` |
| The five shell regions (toolbar Top 100, status Bottom 28, trace Bottom 150, navigation Left 220, workspace Fill min 320×240) | stable frame | **Dock**, as in Module 4 | child order = dock order; `Padding` on each region for the gaps — Dock ignores `Margin` | `MainPage.Designer.cs` |

The two mistakes the lesson warns about are both visible in the build: the Table host under the metric
cards is "a grid of controls that should wrap" (it cannot — at 700 px its 25 % columns are narrower than
the cards' `MinimumSize`), and the Flex host is "a wrap problem solved with weights" (it never wraps; it
squeezes and then overflows). The trace shows both.

## 2 · Same region, three ways — what each engine does at each width

The four cards are 192 × 76 (`MinimumSize` 140 × 76, `MaximumSize` 0 × 76, `Margin` 4). The host is the
workspace width minus the 16 px workspace padding and the TabControl chrome (≈ 12 px), so at a browser
width *W* the host is about *W* − 220 (rail) − 28 px wide. Row heights are 84 (76 + 2 × 4). The
`← client card bounds` trace line prints the real values.

| Browser width | Host ≈ | **Flow** (wins) | **Table** (4 × Percent 25) | **Flex** (FillWeight 1 each) |
|---|---|---|---|---|
| 1400 (desktop) | 1150 px | 4 cards × 200 px in **1 row**; 350 px spare on the right, cards keep 192 px | 4 cells of ≈ 285 px; cards stretch to the cell (Dock Fill) → ≈ 277 × 76 each | 4 equal shares of (1150 − 8 − 3 × 8) ≈ 280 px each, one row |
| 1024 (tablet) | 775 px | 3 cards fit (600 px) → **2 rows** (3 + 1) | 4 cells of ≈ 192 px → cards ≈ 184 × 76 | 4 shares of ≈ 186 px |
| 700 (phone) | 450 px | 2 cards fit (400 px) → **2 rows** (2 + 2) | 4 cells of ≈ 110 px, **below the card MinimumSize of 140** → the cells cannot hold the cards; the host shows a horizontal scrollbar (AutoScroll on) | 4 × 140 (MinimumSize wins over the weight) + spacing = 584 px in 450 → the row **overflows and is clipped** on the right; nothing wraps |
| 1024, after **Add cards** (10 cards) | 775 px | 3 per row → **4 rows** (3 + 3 + 3 + 1); rows 3–4 scroll inside the 160-px host (AutoScroll) | `Controls.Add(card)` with no cell: the engine takes the next free cell and `GrowStyle = AddRows` adds rows 1 and 2 (`GetPositionFromControl` in the trace) | 10 shares of 775 px → 140 each (the minimum) → 1 480 px in 775: clipped |

Reading: the flow engine is the only one whose answer to "less width" is "more rows", which is what a card
strip wants. The table keeps its proportions (good for a form, wrong for cards). The flex engine keeps
its single row and, once `MinimumSize` stops the shrinking, overflows — which is exactly the behaviour
the list/details split wants (never collapse) and the metric strip does not.

## 3 · The filter bar (Flow) at each width

Row 1 is title 250 + search (min 180) + status 160 + Apply 90, plus 8-px margins ≈ 720 px. The bar is
*W* − 32 px wide (8 px region padding each side, 8 px bar padding each side).

| Browser width | Row 1 | `txtSearch` width (FillWeight 1) | Row 2 (after the FlowBreak on Apply) |
|---|---|---|---|
| 1400 | title · search · status · Apply | ≈ 1368 − 540 = **828 px** — the whole spare width | six lab buttons (≈ 720 px) + `lblProgress` stretched by its own FillWeight 1 |
| 1024 | same four controls | ≈ 992 − 540 = **452 px** | same six buttons; `lblProgress` shrinks to its MinimumSize 120 |
| 700 | 668 px available < 720 needed → **status and Apply wrap** onto row 2; the search box sits at its MinimumSize 180 | **180 px** | the FlowBreak still ends the row *after* Apply, so the lab buttons form **row 3**, which does not fit in the 100-px toolbar region: the bar scrolls vertically (AutoScroll on). The rows are counted in the `← client resize` trace (`filter bar 3 row(s)`); Module 6 gives the toolbar a per-profile height |

Two extended properties did all of it: **FillWeight** on the search box (stretch into the spare width,
never below 180) and **FlowBreak** on Apply (the lab buttons start a new row at *every* width — remove it
and at 1400 they would join row 1).

## 4 · The ticket editor (Table) at each width

The editor lives in the details region of the flex split, so its width is whatever that region gets
(280–520 px, § 5). The table's `Padding 12` frames it; column 0 is **always 140 px**, column 1 is the rest.

| Details region width | Caption column | Editor column (Percent 100) | Rows |
|---|---|---|---|
| 385 px (1400) | 140 | 385 − 24 − 140 = **221 px** — every editor's left edge at x = 152 | rows 0–7 and 9 AutoSize (28-px editors + 6-px bottom margin); row 8 (Notes, span 2) takes the remaining height, ≥ 120 |
| 280 px (1024 and 700, at MinimumSize) | 140 | **116 px** | identical row heights; only the editor column narrowed. The captions did not move a pixel, the alignment survived |

That is the table's whole argument: the proportions live in **ColumnStyles / RowStyles**, the children
only say which cell they are in. The `• server ticket editor` trace prints the styles
(`[Absolute 140 | Percent 100]`), `notes span 2` and the column widths / row heights the engine computed.

## 5 · The list / details split (Flex) at each width

The dashboard is the workspace width minus 16 px of padding; the two regions share (width − 8 Spacing)
by weight 2 : 1, bounded by their sizes.

| Browser width | Dashboard ≈ | List (weight 2, min 300) | Details (weight 1, min 280, max 520) | What held |
|---|---|---|---|---|
| 1400 | 1164 px | ≈ 771 px | ≈ 385 px | the weights alone — 2 : 1 exactly |
| 1024 | 788 px | 780 − 280 = **500 px** | **280 px — at MinimumSize** (`(details at MinimumSize)` in the trace) | `MinimumSize` overrode the weight: the details region stopped shrinking and the list absorbed the loss |
| 700 | 464 px | 300 (its own minimum) | 280 (its minimum) | both regions are at their minimum, 588 px in 464: the flex engine cannot honour both and the workspace overflows to the right. **Neither region vanished** — the failure mode without MinimumSize would have been a 0-px details strip. Module 6 stacks the two regions on the phone profile instead |
| ≥ 1804 (a 1920-px monitor) | ≥ 1568 px | the rest | **520 px — at MaximumSize** | `MaximumSize` capped the details region so the form does not stretch into whitespace |

`AlignY Top` is set on the details region so that, whenever it does not fill the height, it hugs the
top edge; in this horizontal layout the region fills the height, so the alignment only becomes visible
when a `MaximumSize` height is added (try `.MaximumSize(520, 300)` in `DashboardWorkspace.cs`).

## 6 · Margins honoured, Padding needed

- **Honoured:** `Margin 4` on the metric cards produces the 8-px gaps between them in the Flow host and
  the 4-px inset inside the Table cells; `Margin (0,0,8,6)` on the filter-bar children is the 8-px gap
  between the toolbar controls; `Margin (0,0,0,8)` on the rail buttons is the 8-px gap in the rail. All
  three are flow / table / flex children.
- **Ignored:** the five region panels are docked, and the default layout engine does not read `Margin` when
  docking. Their gaps come from **Padding** on the region panel (`toolbarPanel (8,8,8,4)`,
  `navigationPanel (8,4,0,4)`, `workspacePanel (8,4,8,4)`, `tracePanel (8,4,8,4)`, `statusPanel (8,0,8,4)`).
  `docs/LayoutNotes.md` records the audit in detail.

## 7 · Screenshots (taken by the learner)

Resize the browser to each width, wait for the `← client card bounds` line, and save:

| File | Width | What it must show |
|---|---|---|
| `docs/screenshots/desktop-1400.png` | 1400 | Flow tab: 4 cards in one row; toolbar on two rows with a wide search box; list ≈ 2 × details |
| `docs/screenshots/tablet-1024.png` | 1024 | Flow tab: 3 + 1 cards; details region at 280 (`(details at MinimumSize)` in the trace); editor captions still 140 |
| `docs/screenshots/phone-700.png` | 700 | Flow tab: 2 + 2 cards; toolbar wrapped onto three rows; both flex regions at their minimum, editor still aligned |
| `docs/screenshots/table-700.png`, `docs/screenshots/flex-700.png` | 700 | the Table and Flex tabs at the same width, for the "wrong container" comparison of § 2 |

The walkthrough video ("The same region in Flow, Table and Flex") shows the same three widths on the
same console.

## Evidence (what the running app shows)

- On load, in order: `• server shell built: toolbar Dock=Top … · navigation Dock=Left 220×… · trace Dock=Bottom … · status Dock=Bottom … · workspace Dock=Fill … min 320×240`,
  `• server filter bar: FlowLayoutPanel LeftToRight · WrapContents True · AutoScroll True · Padding 8 · FillWeight(txtSearch)=1 min 180 → … px · FlowBreak(btnApply)=True · FillWeight(lblProgress)=1 → … px · 11 children in 2 row(s) · bar …`,
  `• server ticket editor: TableLayoutPanel 2 cols [Absolute 140 | Percent 100] · 10 rows · notes span 2 · col widths 140,… · row heights …`,
  `• server dashboard: FlexLayoutPanel Horizontal · Spacing 8 · list FillWeight 2 min 300×200 → …×… · details FillWeight 1 min 280×200 max 520 AlignY Top → …×… · workspace …`,
  `• server same region, three ways: Flow host: … | Table host: … | Flex host: …`,
  `• server dock order verified: Top, Bottom, Bottom, Left, Fill …`,
  `• server acceptance check: flow ok (…) · table ok (…) · flex ok (…)`.
- **Switch engine** (success): `• server engine → Table (Switch engine): Table host: TableLayoutPanel 4 cols × 1 row(s), ColumnStyles Percent 25 ×4, RowStyles Absolute 84, GrowStyle AddRows; cards Dock=Fill in their cells → … · cardOpen@(0,0), cardOverdue@(1,0), …`, then, once the client has laid the cards out, `← client card bounds (after switch to Table) · Table: 4 cards in 1 row(s) · host …×… · col widths …,…,…,… · row heights 84 · open 277×76 @(8,8) · …`. Click again for Flex (`… FillWeight 1 each …`, then equal widths in the bounds line) and again for Flow (`… 192×76 …`). The status bar reads `engine: Table · 4 cards · 1 row(s)`.
- **Resize** the browser: `← client resize (Page.Resize): browser 1024×… · … · filter bar 2 row(s) · card host 775 px wide · list 500 / details 280 (details at MinimumSize)`, followed by `← client card bounds (card Resize) · Flow: 4 cards in 2 row(s) …` and `← client dashboard (region Resize) · … details … → 280×… (at MinimumSize) …`.
- **Add cards** (progress): `• server add cards started: one card every 700 ms into the Flow host …`, then per tick `→ render card 5: cardExtra1 "Critical" added to the Flow host — 200 px each in 775 px: wraps when the row is full`, and `← client card bounds (after card 5) · Flow: 5 cards in 2 row(s) …`. In the Table host the tick line reads `… the table placed it at (0,1) (GrowStyle AddRows, RowCount now 2)`; in the Flex host `… 5 equal shares of 775 px, none below 140`.
- **Cell collision** (failure): the Table tab is selected, then `• server cell collision: cell (0,0) holds cardOpen; calling tableHost.Controls.Add(cardCollision, 0, 0) …` and either `… no exception — Add accepted the cell. cardCollision reports (0,0), cardOpen reports (…), GetControlFromPosition(0,0) = …` or `… Controls.Add threw …: …`; a red banner and `● layout fault: cell collision`.
- **No-wrap overflow** (failure): the Flow tab is selected, then `• server no-wrap: flowHost.WrapContents=false, AutoScroll=false → one row of 4 × 200 px = 800 px in a 775-px host: anything past the edge is clipped · filterBar.WrapContents=false …` and `← client card bounds (after WrapContents=false) · Flow: 4 cards in 1 row(s) · host 775×… · 1 card(s) past the right edge (CLIPPED) · toolbar overflows by … px (scrolled) · …`.
- **Restore** (recovery): `• server engine → Flow (Restore): …`, `→ render Restore: 12 of 12 tickets (no filter) …`, `• server restored the default arrangement: Flow engine, 4 cards, WrapContents on, filters cleared, banner hidden`, status `● ready`.
