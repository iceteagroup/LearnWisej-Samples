# LayoutComparison · which container fits each region

The comparison note the lab asks for: which container fits each region of the Adaptive Operations Console,
which extended property did the work, and what each region does when the browser is dragged from 1400 to 1024
to 700 pixels.

## 1 · The decision rule, applied

Flow for content that wraps, table for structured forms, flex for regions that share space proportionally,
docking for the stable shell.

| Region | Behaviour it needs | Container | Extended properties that do the work | Where |
|---|---|---|---|---|
| Filter and metric-card area | "the search box takes what is left, the cards always start a new row and wrap" | `FlowLayoutPanel` (`FilterBar`) | `WrapContents = true`, `SetFillWeight(txtSearch, 1)` + `MinimumSize` 180, `SetFlowBreak(btnApply, true)`, each child's `Margin` | `Layout/FilterBar.Designer.cs` |
| Ticket details editor | "every editor's left edge on the same line; editors grow with the panel; Notes takes the remaining height" | `TableLayoutPanel` in `TicketEditor` | `ColumnStyle(Absolute, 140)` + `ColumnStyle(Percent, 100)`; AutoSize rows + one Percent 100 row; `Controls.Add(control, column, row)`; `SetColumnSpan(txtNotes, 2)`; editors `Dock = Fill` in their cells | `Shell/TicketEditor.Designer.cs` |
| List / details split | "the list takes two thirds, the details one third, neither collapses" | `FlexLayoutPanel` (`DashboardWorkspace`) | `LayoutStyle = Horizontal`, `Spacing 8`, `FillWeight(pnlList, 2)`, `FillWeight(detailsPanel, 1)`, `MinimumSize` 300×200 / 280×200, `MaximumSize` 520 on details, `AlignY(detailsPanel, Top)` | `Layout/DashboardWorkspace.cs` |
| Navigation rail | "one column of buttons" | `FlowLayoutPanel`, `TopDown`, `WrapContents = false` | each button's `Margin (0,0,0,8)` | `MainPage.Designer.cs` |
| Shell regions (toolbar Top, status Bottom, navigation Left, workspace Fill) | stable frame | Dock, as in Module 4 | child order = dock order; `Padding` on each region | `MainPage.Designer.cs` |

The same four cards in a `TableLayoutPanel` would squeeze into fixed columns, and in a `FlexLayoutPanel` would
share one row and clip the last card once their `MinimumSize` stops the shrinking. Only the flow engine answers
"less width" with "more rows", which is what a card strip wants.

## 2 · The filter and card area (Flow) at each width

The cards are 192 × 76 with `Margin 4`, so each occupies a 200-px slot.

| Browser width | Row 1 | `txtSearch` | Cards |
|---|---|---|---|
| 1400 | title · search · status · Apply | stretches into the spare width (FillWeight 1) | 4 in one row |
| 1024 | same four controls | narrower | 3 + 1 (the bar scrolls to the second card row) |
| 700 | status and Apply wrap onto a second row | at its `MinimumSize` of 180 | 2 per row |

## 3 · The ticket editor (Table)

Column 0 is always 140 px; column 1 is the rest of the details region (280 to 520 px). At 280 px the editor
column is 116 px and the captions have not moved: the proportions live in `ColumnStyles` / `RowStyles`, the
children only say which cell they are in.

## 4 · The list / details split (Flex)

| Browser width | List (weight 2, min 300) | Details (weight 1, min 280, max 520) | What held |
|---|---|---|---|
| 1400 | ≈ two thirds | ≈ one third | the weights alone |
| 1024 | the rest | 280 px, at `MinimumSize` | `MinimumSize` overrode the weight |
| 700 | 300 (its minimum) | 280 (its minimum) | both at their minimum; the workspace overflows instead of losing a region |
| a 1920-px monitor | the rest | 520 px, at `MaximumSize` | `MaximumSize` capped the form |

## 5 · Screenshots (taken by the learner)

`docs/screenshots/desktop-1400.png`, `tablet-1024.png`, `phone-700.png`: the cards per row, the search box, the
editor columns and the details region at each width.

## Evidence

Dragging the browser from 1400 to 700 px wraps the cards, shrinks the search box to 180 px, keeps the editor
captions at 140 px and stops the details region at 280 px; `Width: N px` in the status bar gives the width of
each screenshot. No Resize handler runs.
