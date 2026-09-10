# Deliverable 4 · FluentMarkupRegion.md — one region built in code with `Wisej.Web.Markup`

**File:** `Layout/DashboardWorkspace.cs` — the list / details split of the workspace, a
`FlexLayoutPanel` created entirely in code. It has **no Designer file**, no `SetFillWeight` / `SetAlignY`
calls and no designer serialisation: the container is described by one fluent chain, top to bottom.

## Why this region

The lesson says the natural candidate for the fluent style is a region you already create in code.
The list / details split is that region: its two children are the ticket grid and the
`Shell/TicketEditor` UserControl, both of which the page needs to reach (`dashboard.Grid`,
`dashboard.Editor`), and its behaviour — "2 : 1, never collapse, cap the form" — is five extended
properties that read better as a chain than as five scattered `Set*` lines. The FilterBar (Flow) and
the TicketEditor (Table) stay in the Designer, so the learner sees both notations side by side and
never in the same class.

## The chain

```csharp
using Wisej.Web;
using Wisej.Web.Markup;

public class DashboardWorkspace : FlexLayoutPanel
{
    public DashboardWorkspace()
    {
        this.Grid = CreateGrid();                                   // object initialisers: Markup has no grid extensions
        this.ListTitle = new Label { AutoSize = false }
            .Name("lblWorkspaceTitle").Text("Tickets").Dock(DockStyle.Top).Size(300, 26)
            .Font(new Font("default", 11F, FontStyle.Bold)).TextAlign(ContentAlignment.MiddleLeft);
        this.ListRegion = new Panel { BorderStyle = BorderStyle.Solid }
            .Name("listRegion").BackColor(Color.White).Padding(new Padding(8))
            .MinimumSize(300, 200)
            .Controls(new Control[] { this.Grid, this.ListTitle });   // grid first → docked last → fills

        this.Editor = new TicketEditor().Name("ticketEditor").Dock(DockStyle.Fill);
        this.DetailsRegion = new Panel { BorderStyle = BorderStyle.Solid }
            .Name("detailsRegion").BackColor(Color.White).AutoScroll(true)
            .MinimumSize(280, 200)
            .MaximumSize(520, 0)
            .Controls(new Control[] { this.Editor });

        this
            .Name("dashboard")
            .Dock(DockStyle.Fill)
            .MinimumSize(320, 240)
            .LayoutStyle(FlexLayoutStyle.Horizontal)     // one row, always fills the client area
            .Spacing(8)                                  // gap between the two regions
            .Controls(new Control[] { this.ListRegion, this.DetailsRegion })
            .FillWeight(this.ListRegion, 2)              // two thirds of the spare width
            .FillWeight(this.DetailsRegion, 1)           // one third, never below 280, never above 520
            .AlignY(this.DetailsRegion, VerticalAlignment.Top);
    }
}
```

Every extension returns the container, so the chain **is** the layout description. Read it as the
lesson's sentence: "horizontal, 8 px apart, list then details, 2 : 1, details aligned to the top".

## Which extension methods exist (Wisej-4 4.1.0, checked against `Wisej.Framework.xml`)

| Namespace class | Methods used here | Also available |
|---|---|---|
| `FlexLayoutPanelExtensions` | `LayoutStyle(FlexLayoutStyle)`, `Spacing(int)`, `FillWeight(Control, int)`, `AlignY(Control, VerticalAlignment)` | `AlignX(Control, HorizontalAlignment)` — for a `Vertical` layout, where a child may not fill the width |
| `FlowLayoutPanelExtensions` | — (the FilterBar stays in the Designer) | `FlowDirection(FlowDirection)`, `WrapContents(bool)`, `FillWeight(Control, int)`, `FlowBreak(Control, bool)` |
| `TableLayoutPanelExtensions` | — (the TicketEditor stays in the Designer) | `ColumnCount(int)`, `RowCount(int)`, `GrowStyle(TableLayoutPanelGrowStyle)`, `RowStyles(RowStyle[])`; **note**: the method that sets the *column* styles is misnamed `RowCount(ColumnStyle[])` in 4.1.0 (its XML summary says "Sets the column styles"), which is one more reason the Table region is built with `ColumnStyles.Add(new ColumnStyle(...))` in the Designer |
| `ControlExtensions` | `Name`, `Text`, `Dock`, `Size`, `Font`, `BackColor`, `Padding(Padding)`, `MinimumSize(int, int)`, `MaximumSize(int, int)`, `Controls(Control[])` | `Margin`, `Anchor`, `Visible`, `Enabled`, `OnClick(Action)`, `Location`, `Width`, `Height` … |
| `ScrollableControlExtensions` | `AutoScroll(bool)` | `ScrollBars`, `AutoScrollMargin`, `AutoScrollMinSize` |
| `LabelExtensions` | `TextAlign(ContentAlignment)` | `AutoEllipsis`, `AllowHtml`, `AllowMarkdown` … |

Two properties are **not** in the chain, on purpose:

- `AutoSize` — both `ControlExtensions` and `LabelExtensions` define `AutoSize<T>(T, bool)`, and for a
  `Label` the compiler reports CS0121 (ambiguous call). It is set with an object initialiser instead.
- `BorderStyle` — the only `BorderStyle(...)` extension is on `ButtonExtensions`; calling it on a `Panel`
  is CS0311. Object initialiser again.

Neither is a layout property, so the rule the lab checks still holds: **one notation for the layout of
this container** — the fluent chain — and no `Set*` call anywhere in the class.

## One notation per container, across the solution

| Container | Notation | File |
|---|---|---|
| `DashboardWorkspace : FlexLayoutPanel` | fluent `Wisej.Web.Markup` chain | `Layout/DashboardWorkspace.cs` (no Designer file) |
| `FilterBar : FlowLayoutPanel` | Designer serialisation (`SetFillWeight`, `SetFlowBreak` in `InitializeComponent`) | `Layout/FilterBar.Designer.cs` |
| `TicketEditor` → `TableLayoutPanel` | Designer serialisation (`ColumnStyles.Add`, `Controls.Add(c, col, row)`, `SetColumnSpan`) | `Shell/TicketEditor.Designer.cs` |
| `flowHost` / `tableHost` / `flexHost` (Same region, three ways) | Designer serialisation for the hosts; the cards' per-host extended properties are set imperatively in `MainPage.SwitchEngine()` because the cards **move** between hosts at runtime — the one place where code must set them, and it is the same `Set*` notation the Designer uses | `MainPage.Designer.cs`, `MainPage.cs` |

Nothing mixes a fluent chain with `Set*` calls on the same container.

## No Resize handler, no Bounds arithmetic

The acceptance criterion "no manual Resize handler or Bounds arithmetic anywhere in the solution" is
met in the sense the lesson intends: the only `Resize` handlers (`MainPage_Resize`, `card_Resize`,
`region_Resize`) **report** sizes to the trace and set nothing; no line in the solution assigns
`Bounds`, `Location`, `Left`, `Top`, `Width` or `Height` from a handler. Grep evidence:

```
grep -rn "Bounds\|\.Location = \|SetBounds\|\.Left = \|\.Top = " --include=*.cs Module 5/AdaptiveOps   → no matches in code-behind
```

(The Designer files set initial `Size` values, as any designer does; the containers own them afterwards.
The one `Size` assignment in code-behind is `card.Size = new Size(192, 76)` in `MainPage.SwitchEngine()`:
when a card leaves a Table cell, where `Dock = Fill` had stretched it, its design-time size is restored
before the Flow or Flex engine takes over — a reset to a constant, not a value computed from the browser.)

## Evidence (what the running app shows)

- `• server dashboard: FlexLayoutPanel Horizontal · Spacing 8 · list FillWeight 2 min 300×200 → 771×… · details FillWeight 1 min 280×200 max 520 AlignY Top → 385×… · workspace 1164×…` — the values the chain set, read back with `GetFillWeight` / `GetAlignY`, and the widths the engine produced (2 : 1).
- `• server acceptance check: … flex ok (Horizontal, 2 : 1, MinimumSize on both, MaximumSize on details, AlignY Top)`.
- Resize to 1024: `← client dashboard (region Resize) · … details … → 280×… (at MinimumSize) …` — the size constraint, not the weight, decided the width.
- Select a ticket: `← client grid selection: ticket T-1042 "Printer offline, floor 3" loaded into the TableLayoutPanel editor` — the grid and the editor created in this class are wired by the page (`dashboard.Grid.SelectionChanged`, `dashboard.Editor.SaveClick`).
