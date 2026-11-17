# Northwind Dashboard · Wisej.NET 4.2: Build a Northwind Dashboard with AI · Module 7

Local lab build for **Module 7 · Complete and Test the Wisej.NET 4.2 Dashboard** (lab: *Demonstrate
a Tested 4.2 Northwind Capstone*). The complete application: the Overview gains its category chart
and says when it refreshed from a change made in another view, and the palette finds every kind of
record. Everything else is what Modules 1 to 6 built, checked together.

This folder is the complete application of the course, with its own solution and port. The
application the lessons' source references point at is
[`../Wisej.NorthwindDashboard`](../Wisej.NorthwindDashboard); this one is the same dashboard as the
walkthrough videos show it.

## Run it

```bash
cd "Northwind Dashboard with AI Course/Module 7/NorthwindDashboard"
dotnet run -f net10.0 --urls http://localhost:6517
```

Open <http://localhost:6517>. In Visual Studio, open `NorthwindDashboard.slnx` and press F5.

Requirements: the .NET 10 SDK and a Wisej.NET development or trial license. The project restores
the Wisej.NET package from the course's `Wisej.NorthwindDashboard/packages` folder
(`NuGet.Config` maps `Wisej-4` to that folder and everything else to nuget.org) into
`.nuget-packages` in the course folder, a package cache shared by the seven module applications
and ignored by git. The customer map loads its tiles from OpenStreetMap, so it needs an internet
connection.

## What this module adds

Everything in [Module 6](../Module%206) is still here. New in this module:

| Area | Change | Controls |
|---|---|---|
| Overview | **Revenue by category**, a bound bar chart whose bars open Products; the status line `Hidden view refreshes from the shared context` after a change made elsewhere | `ChartView` (`Bar`) |
| Shell | The palette also finds products and order numbers | `CommandPalette` |

## The screen

The capstone **Overview**: the four cards, then **Order stages** with the notes, **Revenue by
category** (a bound `Bar` chart, largest category first) and **Activity** side by side, then
**Order activity**. Every figure comes from the session's one `DashboardContext`, so the page
follows the period selector, the Fulfillment board and the reorder cards without any code that
links it to them.

The seven areas and the new controls each one uses:

| Area | Controls |
|---|---|
| Shell | `SegmentedButton` (`periodSelector`), `CommandPalette`, `Avatar`, client profiles |
| Overview | `KpiPanel` (sparkline and gauge visuals), `MeterBar`, `ChartView` (bar), `Timeline`, `HeatmapCalendar` |
| Sales | `ChartView` (stacked column, waterfall, area, bar, treemap) |
| Orders | `ChipGroup`, `QueryBuilder`, `EmptyState`, `ChipLabel`, the six `DataGridView` column types |
| Order dialog | `Stepper`, `ChipLabel`, `Avatar`, a subtitle column |
| Fulfillment | `RadialGauge`, `SegmentedButton`, `KanbanBoard`, `SchedulerView`, `GanttView`, `Timeline` |
| Products | `MeterBar`, `ChipLabel`, `KpiPanel`, `ChipGroup`, `ActionCardList`, subtitle, sparkline, meter, chip and actions columns |
| Customers | `MapView`, `Avatar`, `Sparkline`, `Timeline`, `EmptyState`, `ChipLabel`, avatar, sparkline and chip columns |
| Team | `OrgChart`, `Avatar`, `AvatarGroup`, `ChartView` (radar), avatar, meter, sparkline and chip columns |

## What to try

Two workflows that cross every view, then the checks around them. Figures are for **30D** in a new
session.

**Sales investigation**

| Action | Expected result |
|---|---|
| Start the application | Revenue $118,307.15, 74 orders. Revenue by category from Dairy Products to Condiments, axis to 40,000. Status: `SegmentedButton → DashboardContext → every view`. |
| Select **All** | 830 orders, $1,265,793.29; the chart's axis goes to 320,000. Select **30D** again. |
| Click the **Revenue** card | Sales opens. Status: `KpiPanel.Click → Sales`. |
| On Sales, click the **UK** bar | Customers opens with `Country: UK`. Status: `PointClick → e.Point.Tag → Customers`. |
| Back on Overview, click the **Seafood** bar | Products opens with the Seafood chip selected. Status: `PointClick → e.Point.Tag → Products`. |
| Click the **Orders** card | Orders opens. Status: `KpiPanel.Click → Orders`. |
| **Open** on the first row | The order dialog of #11077. Status: `ActionClick → bound row → OrderDialog`. |
| Click the last cell of the heatmap | Orders for that day: chip `Day: 1998-05-06`, 4 orders, $2,779, `all periods`. Status: `HeatmapCalendar.CellClick → day scope`. |

**Operations workflow**

| Action | Expected result |
|---|---|
| On Overview, click the first Activity item, **Mark as shipped**, **Close** | Order #11077 ships. The visible Overview refreshes: `Order #11077 shipped` first, `8 new · 7 picking · 5 packed · 20 open`, `0 late of 54 shipped`. |
| Open **Fulfillment**, move **#11076** to Shipped (drag, or focus the card and press **Alt+Right** three times) | `Open orders · 19`. Status: `CardMoved → e.Column.Value → context.SetStage`. |
| Return to **Overview** | The hidden Overview refreshed when shown: `Order #11076 shipped` is the first Activity item, `7 new · 7 picking · 5 packed · 19 open`, `0 late of 55 shipped`. Status: `Hidden view refreshes from the shared context`. |
| Open **Orders** | `Open (19) · Shipped (55)`. |
| Open **Products**, click **Order 40** on *Outback Lager* | Status: `ActionCard.ActionClick → context.PlacePurchaseOrder`. |
| Return to **Overview** | `Purchase order for Outback Lager` (`40 units from Pavlova, Ltd.`) is the first Activity item. Status: `Hidden view refreshes from the shared context`. Clicking the item opens Products on that product (`Timeline.ItemClick → e.Item.Tag → Products`). |
| Open another area and return without changing anything | Nothing is reloaded; the status stays `Button.Click → Tag → Navigate(Overview)`. |
| Change the period while another area is shown, then return | Overview shows the new period. Status: `SegmentedButton → DashboardContext → every view`. |
| Open the application in a private window | 21 open orders, 53 shipped, no purchase order: the changes belong to the first session. |

**Palette**

| Action | Expected result |
|---|---|
| Ctrl+K | The seven page commands and the five period commands. |
| `go to orders`, Enter | Orders opens. Status: `Candidates → counts → selected status`. |
| `12 months`, Enter | The selector moves to **12M**; Orders shows 549 orders, $856,885. Status: `CommandExecuted → Tag → periodSelector.SelectedIndex → context.SetPeriod(Last12Months)`. |
| `around`, Enter | Customers opens with Around the Horn. Status: `CommandExecuted → Tag → Customers`. |
| `leverling`, Enter | Team opens with Janet Leverling. Status: `CommandExecuted → Tag → Team`. |
| `chai`, Enter | `Open product Chai` (`Beverages · Exotic Liquids · $18.00`): Products opens with the row of Chai selected and scrolled into view. Status: `CommandExecuted → Tag → Products`. |
| `#1097` | `Open order #10970` to `#10975`. Enter opens the dialog of #10970 (`Shipped late`) over the current view. Status: `CommandExecuted → Tag → OrderDialog`. |
| `zzz` | `No matching commands`. |

**Responsive**: the trip of [Module 6](../Module%206) (1440 → 1200 → 1199 → 768 → 767 → 1440)
restores every control of all seven views.

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Sales investigation interaction trace | period → `RefreshView` → `UpdateKpis` / `UpdateCategories` in [Views/OverviewView.cs](NorthwindDashboard/Views/OverviewView.cs); `revenueKpi_Click` → Sales; `countriesChart_PointClick` in `SalesView` → `CustomersView.ShowTarget`; `categoryChart_PointClick` → Products; `ordersKpi_Click` and `ordersHeatmap_CellClick` → `OrdersView.ShowTarget`; the event paths are the status texts in the first table |
| Operations workflow and isolation trace | `OrderDialog.shipButton_Click` and `FulfillmentView.kanbanBoard_CardMoved` → `DashboardContext.SetStage` → `OrdersChanged`; `ProductsView.PlacePurchaseOrder` → `ProductsChanged`; `OnOrdersChanged` / `OnProductsChanged` in `OverviewView` (note a change made while hidden), the base class's stale flag and `OnVisibleChanged` in [Views/DashboardView.cs](NorthwindDashboard/Views/DashboardView.cs) |
| Completed 4.2 control coverage matrix | The table of areas and controls above. For each control: binding in the view's `Update…` methods, events in the handlers at the end of the view's file, empty input (`UpdateKpis` without shipments, `comparisonLabel` without a comparison, `EmptyState` on Orders and on the customer card, the bridge without a comparison), repeat (`Segments.Clear()`, `Items.Clear()`, `Resources.Clear()`, `Tasks.Clear()`) |
| Build, domain and browser verification evidence | `dotnet build` builds both target frameworks; the figures in the **What to try** tables of the seven module READMEs are the checks |
| Reproducible artifact, runbook and reviewed AI decisions | **Run it** above; `NuGet.Config` pins the package source; [docs/ARCHITECTURE.md](NorthwindDashboard/docs/ARCHITECTURE.md) is the contract the views were written against |
| Products and orders in the palette | `productCommands`, `OrderCommand`, `Search`, `case Product` and `case Order` in [MainPage.cs](NorthwindDashboard/MainPage.cs) |

## Differences from the video

- **Chart.** The video draws the Overview chart as stacked columns. The finished application's
  Overview chart is a bar per category whose click opens Products, and that is what the lessons
  describe, so the sample uses it; the stacked chart is on Sales.
- **Cards.** Four cards with trend, sparkline and gauge; the video's capstone frame shows three
  without trend (All time has none) and, in its last phase, a shipper gauge that belongs to
  Fulfillment.
- **Status after a hidden refresh.** The video shows `Hidden view refreshes from the shared context`
  only in its verification scene; the sample writes it whenever an order or stock change arrived
  while Overview was hidden.
- **Combined status lines.** The video joins two steps in one line
  (`KpiPanel.Click → Orders → ActionClick → OrderDialog`,
  `CardMoved → SetStage → gauges, Timeline, scheduler, Gantt`). The application writes the event
  path of the last interaction: `KpiPanel.Click → Orders`, then `ActionClick → bound row → OrderDialog`;
  `CardMoved → e.Column.Value → context.SetStage`.
- **Palette order.** Record results come before the page and period commands, so `around` + Enter
  opens the customer. A word that many records contain (`sales` is in the job title of most
  contacts) lists six customers first, and the page command `Go to Sales` after them.
- **Publishing.** The video's verification scene publishes the application and inspects the
  artifact. That is a command-line step of the lab, not a screen of the sample.
- **Against the finished application.** The module applications keep the shell of the videos (blue
  application bar, status strip, 150-pixel sidebar) and the screens the videos show; the finished
  application has its own layout and a few more cards per view.
