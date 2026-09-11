# AdaptiveOps · Theming & Responsive Layouts Course · Module 5

Lab build for **Module 5 · FlowLayoutPanel, TableLayoutPanel, FlexLayoutPanel, and Extended Layout
Properties**. The console keeps its docked shell regions; the content inside them is built three ways:

- **`Layout/FilterBar : FlowLayoutPanel`** (the toolbar): title, `txtSearch` (`FillWeight 1`, `MinimumSize` 180),
  `cmbStatus`, `btnApply` (`FlowBreak`), then the four metric cards, which therefore always start a new row
  and wrap as the browser narrows.
- **`Shell/TicketEditor`** around a two-column `TableLayoutPanel` (`ColumnStyle(Absolute, 140)` for captions,
  `ColumnStyle(Percent, 100)` for editors, editors docked Fill in their cells, Notes spanning both columns with
  a `MinimumSize` height of 120).
- **`Layout/DashboardWorkspace : FlexLayoutPanel`** (the workspace): `pnlList` and `detailsPanel` weighted 2 : 1,
  `MinimumSize` on both, `MaximumSize` and `AlignY Top` on the details, written entirely in code with the
  `Wisej.Web.Markup` fluent extensions.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 5/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5505
```

Then open <http://localhost:5505> (or open `AdaptiveOps.slnx` in Visual Studio and press F5).

## What to try

- **Resize the browser** from 1400 to 1024 to 700 px: the metric cards wrap from four to fewer per row, the
  search box stretches and stops at 180 px, the editor captions stay at 140 px while the editors narrow, and the
  details region stops at its `MinimumSize` (280 px) instead of vanishing. The status bar shows `Width: N px`.
  At narrow widths the filter bar scrolls to reach the wrapped cards.
- **Type `printer` and press Apply** (or Enter): the grid shows the matching tickets and the status label reads
  `Filters applied: 3 of 12 tickets`; combine with a status in the drop-down.
- **Type `/[/` and press Apply**: the regular expression does not parse, the status label reports the error and
  the grid keeps its last good filter.
- **Select a ticket, edit, Save**; clear the Owner and Save to see the server reject it.
- **Click a navigation button**: the list heading changes.

## Where things live

```
AdaptiveOps/
├─ MainPage.Designer.cs            the docked regions, the TopDown FlowLayoutPanel rail, the status bar
├─ MainPage.cs                     ApplyFilters(), tickets, save; no layout code, no Resize handler
├─ Layout/FilterBar.cs (+ .Designer.cs)   FlowLayoutPanel: SetFillWeight(txtSearch, 1), SetFlowBreak(btnApply, true), the cards
├─ Layout/DashboardWorkspace.cs    FlexLayoutPanel list / details split, built with Wisej.Web.Markup (no Designer file)
├─ Shell/TicketEditor.cs (+ .Designer.cs) the TableLayoutPanel form
├─ Shell/MetricCard.cs (+ .Designer.cs)   one card: Title, Value, Accent; 192×76, Margin 4
└─ docs/
   ├─ LayoutComparison.md          which container fits each region, the extended property that did the work, per-width geometry
   ├─ LayoutNotes.md               the Margin / Padding audit
   └─ FluentMarkupRegion.md        the DashboardWorkspace chain and the one-notation rule
```

## Lab step → code map

| Lab step | Where |
|---|---|
| `FilterBar : FlowLayoutPanel`, LeftToRight, `WrapContents`, `AutoScroll`, `Padding` 8; `txtSearch`, `cmbStatus`, `btnApply`; `SetFillWeight(txtSearch, 1)` + `MinimumSize` 180; `SetFlowBreak(btnApply, true)` so the cards start a new row | `Layout/FilterBar.Designer.cs` |
| `TicketEditor` around a `TableLayoutPanel`: Absolute 140 + Percent 100 columns, rows with `Controls.Add(control, column, row)`, Notes spanning both columns with `MinimumSize` height 120, editors docked Fill | `Shell/TicketEditor.Designer.cs` (the pack's `TableLayoutColumnStyle` is `ColumnStyle` in Wisej.NET) |
| `DashboardWorkspace : FlexLayoutPanel`, Horizontal; `pnlList` and `detailsPanel` with `MinimumSize`, `MaximumSize` on details; FillWeight 2 / 1; AlignY Top | `Layout/DashboardWorkspace.cs` (`Spacing 8` between the regions; the workspace region supplies the 8-px padding) |
| One region entirely in code with `Wisej.Web.Markup` | `Layout/DashboardWorkspace.cs`; `docs/FluentMarkupRegion.md` |
| Audit spacing: Margin honoured by flow / table / flex, docked regions get gaps from Padding | `docs/LayoutNotes.md` |
| `btnApply_Click` → `ApplyFilters()` refreshes the list and the status label; no Resize handler or `Bounds` arithmetic | `FilterBar.btnApply_Click` raises `Apply`; `MainPage.ApplyFilters()` |
| Show every path at 1400 / 1024 / 700, including a failed `ApplyFilters()` in the status label | *What to try* above; `docs/LayoutComparison.md` |

## Self-check answers (lab guide)

- **You gave the details region FillWeight 1 but no MinimumSize, and at 700 px it vanished. What did the flex
  engine do, and why is the fix a size constraint rather than a different weight?** The flex engine fills its
  client area and hands the remaining space to the weighted children in proportion. At 700 px the list, which
  has a `MinimumSize` of 300, cannot shrink further, so the engine takes the shortfall from the only child that
  can: the details region goes to 0 px. A weight is a ratio, not a floor, so any weight still describes a share
  of a shrinking total. The fix is `MinimumSize` on the details region (280 in `DashboardWorkspace.cs`): the
  engine distributes by weight until a child reaches its minimum, then holds it there.
- **The Apply button ends the first row even when there is room for more. Which extended property did that, and
  how would the FilterBar behave without it and with `WrapContents = false`?** `SetFlowBreak(btnApply, true)`:
  the flow engine ends the row after Apply at every width, so the metric cards always start a new row. Without
  it, the cards would join the first row wherever they fit; with `WrapContents = false` as well, every child sits
  on one row and whatever does not fit is clipped, or reachable only by scrolling because the bar has
  `AutoScroll = true`.
- **A Margin of 8 on the cards produced gaps, but the same Margin on the docked navigation rail did nothing. Which
  engine honours margins in each case, and which property creates the gap for the rail?** The cards are children
  of a `FlowLayoutPanel`, whose engine places each child at the previous child's edge plus both margins. The
  `FlexLayoutPanel` and `TableLayoutPanel` honour margins too. The rail is docked by the default layout engine,
  which ignores `Margin`; its gap comes from `Padding` on the container (`navigationPanel.Padding`).
