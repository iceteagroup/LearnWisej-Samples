# LayoutNotes · the spacing audit: which engine honours Margin, where Padding had to be used

Lab step 7: give the cards and editors a Margin and confirm the flow, flex and table engines honour it, then
confirm the docked shell regions still get their gaps from Padding because the default layout engine does not
use margins for docking.

## The rule

`Padding` belongs to the **container** (the inner space around its children). `Margin` belongs to the **child**
(its distance to adjacent controls). Whether a Margin does anything depends on the engine:

| Engine (container) | Reads the child's `Margin`? | What creates the gap |
|---|---|---|
| Default layout engine (Dock, Anchor, Location) | **No** | `Padding` on the parent |
| `FlowLayoutPanel` | Yes | child `Margin` (+ container `Padding` for the frame) |
| `FlexLayoutPanel` | Yes | child `Margin`, plus the container's `Spacing` |
| `TableLayoutPanel` | Yes, inside the cell | child `Margin` (+ container `Padding`) |

## Finding 1 · Margin honoured

| Child | Container | Margin | Effect |
|---|---|---|---|
| `MetricCard` (the four cards) | `FilterBar : FlowLayoutPanel` | `4` all round | 8 px between neighbouring cards; each card takes a 200 × 84 slot |
| `lblAppTitle`, `txtSearch`, `cmbStatus`, `btnApply` | `FilterBar` | `(0, 0, 8, 6)` | 8 px between controls on a row, 6 px between rows |
| `lblNavTitle`, the rail buttons | `navigationRail : FlowLayoutPanel` (TopDown) | `(0, 0, 0, 10)` / `(0, 0, 0, 8)` | the gaps in the rail |
| captions and editors of the ticket editor | `TableLayoutPanel` in `TicketEditor` | captions `(0, 0, 8, 6)`, editors `(0, 0, 0, 6)` | the 6-px rhythm between rows (an AutoSize row is the tallest child plus its Margin) and the 8-px gap between a caption and its editor |
| `pnlList`, `detailsPanel` | `DashboardWorkspace : FlexLayoutPanel` | none | the gap between them is the container's `Spacing 8` |

## Finding 2 · Margin ignored: the docked shell regions use Padding

| Region panel | Dock | Padding | Gaps it creates |
|---|---|---|---|
| `toolbarPanel` | Top (142) | `(8, 8, 8, 4)` | 8 px to the page edges, 4 px above the workspace |
| `navigationPanel` | Left (220) | `(8, 4, 0, 4)` | 8 px to the left edge; the workspace's left padding gives the gap to the content |
| `workspacePanel` | Fill | `(8, 4, 8, 4)` | 8 px to the rail and the right edge |
| `statusPanel` | Bottom (28) | `(8, 0, 8, 4)` | 8 px to the sides, 4 px to the bottom edge |

A quick proof: set `navigationPanel.Margin = new Padding(8)` in `MainPage.Designer.cs` and nothing moves; set
its `Padding` to `(16, 4, 0, 4)` and the rail card moves 8 px to the right.

## A consistent spacing system

Three values throughout: **4** (card margins), **8** (gaps between regions and siblings, the dashboard
`Spacing`) and **12** (the inner frame of the rail and of the editor's table). No code computes a gap.
