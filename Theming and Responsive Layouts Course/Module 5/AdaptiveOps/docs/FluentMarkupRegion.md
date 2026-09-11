# FluentMarkupRegion · one region built in code with `Wisej.Web.Markup`

**File:** `Layout/DashboardWorkspace.cs`, the list / details split of the workspace, a `FlexLayoutPanel`
created entirely in code. It has no Designer file, no `SetFillWeight` / `SetAlignY` calls and no designer
serialisation: the container is described by one fluent chain.

## Why this region

Its two children are the ticket grid and the `TicketEditor` UserControl, both of which the page needs to reach
(`dashboard.Grid`, `dashboard.Editor`), and its behaviour ("2 : 1, never collapse, cap the form") is a handful
of extended properties that read better as a chain. The FilterBar (Flow) and the TicketEditor (Table) stay in
the Designer, so both notations are visible side by side and never in the same class.

## The chain

```csharp
this.pnlList = new Panel { BorderStyle = BorderStyle.Solid }
    .Name("pnlList").BackColor(Color.White).Padding(new Padding(8))
    .MinimumSize(300, 200)
    .Controls(new Control[] { this.Grid, this.ListTitle });

this.Editor = new TicketEditor().Name("ticketEditor").Dock(DockStyle.Fill);
this.detailsPanel = new Panel { BorderStyle = BorderStyle.Solid }
    .Name("detailsPanel").BackColor(Color.White).AutoScroll(true)
    .MinimumSize(280, 200)
    .MaximumSize(520, 0)
    .Controls(new Control[] { this.Editor });

this
    .Name("dashboard")
    .Dock(DockStyle.Fill)
    .MinimumSize(320, 240)
    .LayoutStyle(FlexLayoutStyle.Horizontal)
    .Spacing(8)
    .Controls(new Control[] { this.pnlList, this.detailsPanel })
    .FillWeight(this.pnlList, 2)
    .FillWeight(this.detailsPanel, 1)
    .AlignY(this.detailsPanel, VerticalAlignment.Top);
```

Every extension returns the container, so the chain is the layout description: "horizontal, 8 px apart, list
then details, 2 : 1, details aligned to the top".

## Which extension methods exist (Wisej-4 4.1.0)

| Class | Used here | Also available |
|---|---|---|
| `FlexLayoutPanelExtensions` | `LayoutStyle`, `Spacing`, `FillWeight`, `AlignY` | `AlignX` (for a Vertical layout) |
| `FlowLayoutPanelExtensions` | — (the FilterBar stays in the Designer) | `FlowDirection`, `WrapContents`, `FillWeight`, `FlowBreak` |
| `TableLayoutPanelExtensions` | — (the TicketEditor stays in the Designer) | `ColumnCount`, `RowCount`, `GrowStyle`, `RowStyles` |
| `ControlExtensions` | `Name`, `Text`, `Dock`, `Size`, `Font`, `BackColor`, `Padding`, `MinimumSize`, `MaximumSize`, `Controls` | `Margin`, `Anchor`, `Visible`, `OnClick` … |
| `ScrollableControlExtensions` | `AutoScroll` | `ScrollBars`, `AutoScrollMargin` |
| `LabelExtensions` | `TextAlign` | `AutoEllipsis`, `AllowHtml` … |

Two properties are object initialisers on purpose: `AutoSize` (ambiguous between `ControlExtensions` and
`LabelExtensions` for a Label, CS0121) and `BorderStyle` (the only extension is on buttons, CS0311). Neither is a
layout property, so the container still has one notation for its layout.

## One notation per container

| Container | Notation | File |
|---|---|---|
| `DashboardWorkspace : FlexLayoutPanel` | fluent `Wisej.Web.Markup` chain | `Layout/DashboardWorkspace.cs` |
| `FilterBar : FlowLayoutPanel` | Designer serialisation (`SetFillWeight`, `SetFlowBreak`) | `Layout/FilterBar.Designer.cs` |
| `TicketEditor` → `TableLayoutPanel` | Designer serialisation (`ColumnStyles.Add`, `Controls.Add(c, col, row)`, `SetColumnSpan`) | `Shell/TicketEditor.Designer.cs` |

## No Resize handler, no Bounds arithmetic

No line in the solution assigns `Bounds`, `Location`, `Left`, `Top`, `Width` or `Height` from a handler, and
there is no `Resize` handler. The status bar's width label is updated from `Application.BrowserSizeChanged` and
sets no size.
