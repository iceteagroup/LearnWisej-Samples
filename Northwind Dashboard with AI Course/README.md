# Wisej.NET 4.2: Build a Northwind Dashboard with AI - lab samples

One runnable application per module, built cumulatively. **Northwind Dashboard** starts as a shell
with a period selector and a command palette and ends as a seven-view sales dashboard over the
classic Northwind Traders data, built with the new Wisej.NET 4.2 controls. Each module application
is the screen its walkthrough video shows, with the real Northwind figures in place of the video's
fixture numbers.

Beside the seven module applications, the course has a finished reference application,
**Wisej.NorthwindDashboard**: the dashboard the lessons' source references point at. It is described
[below](#the-reference-application), and [`REFERENCE-APPLICATION.md`](REFERENCE-APPLICATION.md) shows,
file by file, where the work of the seven modules lives in it.

| Module | Folder | Port | What it adds |
|---|---|---|---|
| 1 | `Module 1` | 6511 | The shell: `SegmentedButton` period selector, `CommandPalette`, Overview `KpiPanel` cards, the customer `MapView` |
| 2 | `Module 2` | 6512 | Sparklines and an on-time gauge on the cards, the order-stage `MeterBar`, shipper `RadialGauge`s on Fulfillment |
| 3 | `Module 3` | 6513 | Sales with five `ChartView` charts; a country bar opens Customers with a closable country chip |
| 4 | `Module 4` | 6514 | Orders: `ChipGroup`, `QueryBuilder`, the six new grid column types, `EmptyState`, and the order dialog with its `Stepper` |
| 5 | `Module 5` | 6515 | Fulfillment as `KanbanBoard`, `SchedulerView` and `GanttView`; Products with `ActionCardList`; `Timeline` and `HeatmapCalendar` on Overview |
| 6 | `Module 6` | 6516 | The Customers explorer, Team with `OrgChart` and `AvatarGroup`, and the responsive shell (client profiles) |
| 7 | `Module 7` | 6517 | The capstone: the Overview category chart, products and order numbers in the palette, every view checked together |

Each folder is the complete application at that point in the course, with its own solution, its
`README.md` (the screen, what to try with the figures to expect, where each lab task is in the
code, how the application differs from the video) and `NorthwindDashboard/docs/ARCHITECTURE.md`, the
contract between the shell and the views. [`COOKBOOK.md`](COOKBOOK.md) collects the control facts
that came out of building and running them.

```bash
cd "Northwind Dashboard with AI Course/Module 1/NorthwindDashboard"
dotnet run -f net10.0 --urls http://localhost:6511
```

Then open <http://localhost:6511>. In Visual Studio, open the module's `NorthwindDashboard.slnx` and
press F5. Use the module's own port: several modules can run side by side.

Requirements: the .NET 10 SDK and a Wisej.NET development or trial license. Every module builds for
`net10.0-windows` (for the Visual Studio designer) and `net10.0` (for cross-platform hosting). The
customer map loads its tiles from OpenStreetMap, so it needs an internet connection.

## The Wisej.NET package

The new controls are not in a released Wisej.NET package yet, so the applications restore `Wisej-4`
from the local `Wisej.NorthwindDashboard/packages` folder instead of nuget.org: each `NuGet.Config`
maps that one package to the folder and everything else to nuget.org. The package keeps the version
number 4.1.4 but is an unsigned development build that carries the 4.2 controls. Because the
released `Wisej-4` 4.1.4 may already be in your NuGet cache, `NuGet.Config` also gives the samples a
package cache of their own (`.nuget-packages`, ignored by git): one in the course folder, shared by
the seven module applications, and one in the reference application's folder. Keep the package and
the `NuGet.Config` files together. See
[`Wisej.NorthwindDashboard/packages/README.md`](Wisej.NorthwindDashboard/packages/README.md).

Clone the repository into a short path such as `C:\src`. The package cache is inside the course
folder, so in a deeply nested folder some package paths pass the 260-character limit of Windows and
the build stops with "Could not copy the file ... because it was not found". Moving the clone to a
shorter path and building again fixes it.

## Data and session state

The Northwind data (`Data/northwind.json`, converted from Microsoft's `instnwnd.sql`, MIT license)
and the outlines of the 21 customer countries (`Data/countries.geojson`, from Natural Earth) are
embedded resources, loaded once into an immutable shared `NorthwindDatabase`. The dashboard's
"today" is the last recorded order date, **May 6, 1998**. The `Data` folder of every module
application is the reference application's, with the namespace `NorthwindDashboard.Data`.

The shared data is never modified. Each session has its own `DashboardContext` that records the
warehouse steps on the fulfillment board and the purchase orders placed from the reorder
suggestions; those changes last for the session only. A second browser session, or a private
window, starts from the snapshot again.

## The findings worth knowing before you start

These came out of running the code. [`COOKBOOK.md`](COOKBOOK.md) has them with the evidence and
where each one is used.

- **The new controls ignore the theme's primary color.** Each has its own color keys, indigo by
  default. An application sets them in a theme mixin, control by control; a few colors are literals
  inside an appearance and need the appearance overridden.
- **Setting a selection in code raises the change event.** `SegmentedButton.SelectedIndex`,
  `TextBox.Text` and `QueryBuilder.Fields.Add` all do. Use that (one route for clicks and code) or
  guard against it; do not assume silence.
- **The last write to a status text wins, and the shell usually writes last.** A view that must have
  the last word posts its line with `Application.Post`; posted callbacks run in order, after the
  handlers of the request.
- **`MapView` clusters markers by default,** and markers in the same city are drawn on top of each
  other. Turn `Cluster` off and offset same-city markers when every record needs its own marker.
- **A grid keeps its scroll offset when a shorter list is bound.** Call `ScrollRowIntoView` after
  assigning the `DataSource`.
- **A `KanbanBoard` card moves with the keyboard too** (Alt+arrows) and raises the same events as a
  drag, so validation in `CardMoving` covers both.
- **`SchedulerView` shows resources as rows only in its Timeline mode.**
- **Give every responsive profile of a control the same set of properties,** and test a session that
  starts narrow, not only a round trip from the designed width.
- **A card that spans two rows, a control anchored to the right:** both need attention when a layout
  is reflowed for a narrow profile and restored afterwards.

## The reference application

**Wisej.NorthwindDashboard** is the finished dashboard: the same seven views and the same data layer
with its own, denser layout. The lessons quote its source, and each module of the course studies
and rebuilds a different part of it.

![Overview: KPI cards, revenue trend, category bars, top products, recent orders](Wisej.NorthwindDashboard/docs/overview.png)

```bash
cd "Northwind Dashboard with AI Course/Wisej.NorthwindDashboard"
dotnet run -f net10.0 --urls http://localhost:6501
```

Then open <http://localhost:6501/Default.html>. In Visual Studio, open
`Wisej.NorthwindDashboard.slnx` and press F5.

A sidebar shell (`MainPage`) hosts seven views. The period selector (30D, 90D, YTD, 12M, All) drives
every view that reports on a period, and **Ctrl+K** opens a search palette over pages, periods,
customers, products, people and order numbers.

### Where each course module lives in the reference application

| Course module | Where to look | New controls it uses |
|---|---|---|
| 1 · Start a Wisej.NET 4.2 Northwind Application | `MainPage.cs`, `SessionStartup.cs`, `Data/DashboardContext.cs` | SegmentedButton, CommandPalette |
| 2 · Build KPI Cards, Sparklines and Meters | `Views/OverviewView.cs`, `Views/FulfillmentView.cs`, `Views/ProductsView.cs` | KpiPanel, Sparkline, MeterBar, RadialGauge |
| 3 · Visualize Northwind Sales with ChartView | `Views/SalesView.cs`, `Views/OverviewView.cs` | ChartView (area, stacked column, bar, waterfall, treemap) |
| 4 · Build Rich Grids, Filters and Order Details | `Views/OrdersView.cs`, `Dialogs/OrderDialog.cs`, `Views/GridRows.cs` | The six new DataGridView column types, ChipGroup, QueryBuilder, EmptyState, ChipLabel, Avatar, Stepper |
| 5 · Coordinate Workflow, Scheduling and Inventory Controls | `Views/FulfillmentView.cs`, `Views/ProductsView.cs`, `Views/OverviewView.cs` | KanbanBoard, SchedulerView, GanttView, Timeline, HeatmapCalendar, ActionCardList |
| 6 · Explore Customers, Teams and Responsive Controls | `Views/CustomersView.cs`, `Views/TeamView.cs`, `ClientProfiles.json`, `Views/CardLayout.cs` | MapView, OrgChart, Avatar, AvatarGroup, responsive profiles |
| 7 · Complete and Test the Wisej.NET 4.2 Dashboard | The whole application | Every control above, checked together |

### What the reference application shows

| View | What it shows | New controls used |
|---|---|---|
| Overview | Revenue, orders, average order and on-time KPIs; weekly revenue against the comparison period; revenue by category; top products; recent orders; 12-month order heatmap; open orders by warehouse step | KpiPanel, ChartView, DataGridView subtitle/sparkline/meter/chip columns, Timeline, HeatmapCalendar, MeterBar |
| Sales | Category revenue over time with its table view, revenue bridge, top countries, product-mix treemap, revenue per bucket | ChartView (stacked column, waterfall, bar, treemap), DataGridView chip/subtitle columns |
| Orders | Status filters with counts, search, advanced rule builder, drill-down from other views | ChipGroup, QueryBuilder, EmptyState, DataGridView avatar/chip/subtitle/actions columns |
| Fulfillment | Shipper on-time rates and lead time; open orders as a board, a delivery calendar or a lead-time timeline | RadialGauge, KpiPanel, KanbanBoard, SchedulerView, GanttView, SegmentedButton |
| Products | Inventory health, stock status filters, catalog with stock coverage, reorder suggestions | MeterBar segments, ChipLabel, ChipGroup, ActionCardList, DataGridView sparkline/meter/chip/actions columns |
| Customers | Countries shaded by revenue quintile, clustered customer markers, account details, customer list | MapView, Avatar, Sparkline, Timeline, EmptyState, DataGridView avatar/sparkline/chip columns |
| Team | Organization chart, profile with a radar against the team average, top customers, leaderboard | OrgChart, Avatar, AvatarGroup, ChartView (radar), DataGridView avatar/meter/sparkline/chip columns |

The order details dialog (`Dialogs/OrderDialog`) uses a Stepper, ChipLabel and Avatars. Every view
has a `.Designer.cs` file, so the layouts can be opened and edited in the Wisej designer.

The reference application's own [`README.md`](Wisej.NorthwindDashboard/README.md) covers the data
rules, the theme mixin and the responsive layout (Phone and Tablet client profiles) in detail.

### How the module applications differ from it

[`REFERENCE-APPLICATION.md`](REFERENCE-APPLICATION.md) is the full comparison: the module that builds
each part, the files and members to look for in the reference application, what each view has in
one application only, and what to try with the two running side by side. In short:

The module applications keep the shell of the walkthrough videos: a blue application bar, a
150-pixel sidebar and a status strip that names the event path of the last interaction. Their views
show what the module's video and lab build, and each module README lists the differences from the
video. Navigation between views uses the same target objects as the reference application, and the
order dialog and the row classes are the reference application's.
