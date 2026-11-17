# Northwind Dashboard · Wisej.NET 4.2: Build a Northwind Dashboard with AI · Module 3

Local lab build for **Module 3 · Visualize Northwind Sales with ChartView** (lab: *Build Interactive
Sales Charts*). **Sales** opens with five `ChartView` charts and the table of the rows behind the
first one, and a click on a country bar opens **Customers** with that country as a removable filter.

This folder is the complete application at this point in the course, with its own solution and
port. The finished dashboard that every module works toward is in
[`../Wisej.NorthwindDashboard`](../Wisej.NorthwindDashboard).

## Run it

```bash
cd "Northwind Dashboard with AI Course/Module 3/NorthwindDashboard"
dotnet run -f net10.0 --urls http://localhost:6513
```

Open <http://localhost:6513>. In Visual Studio, open `NorthwindDashboard.slnx` and press F5.

Requirements: the .NET 10 SDK and a Wisej.NET development or trial license. The project restores
the Wisej.NET package from the course's `Wisej.NorthwindDashboard/packages` folder
(`NuGet.Config` maps `Wisej-4` to that folder and everything else to nuget.org) into
`.nuget-packages` in the course folder, a package cache shared by the seven module applications
and ignored by git. The customer map loads its tiles from OpenStreetMap, so it needs an internet
connection.

## What this module adds

Everything in [Module 2](../Module%202) is still here: the Overview cards and meter, the shipper
gauges. New in this module:

| Area | Change | Controls |
|---|---|---|
| Sales | New view: revenue by category over time with its table, the revenue bridge, the revenue trend against the comparison period, the top countries and the product mix | `ChartView` (`StackedColumn`, `Waterfall`, `Area`, `Bar`, `Treemap`), `DataGridView` |
| Customers | The customer list beside the map and the card; a country that arrives from Sales filters it | `ChipLabel` (`Closable`), `DataGridView` |
| Overview | The Revenue and Average order cards now open the real Sales view | |

Orders, Products and Team still say which module builds them.

## The screen

**Sales** opens on the **Revenue by category** card: a `StackedColumn` chart with one series per
category, built from explicit `ChartPoint` objects (one point per bucket of the period, `Tag` = the
category's figures), and beside it the **Table of the same rows** (category, revenue, share, change).
Below: the **Revenue bridge**, a `Waterfall` from the comparison period's total through each
category's signed change to this period's total; the **Revenue trend**, an `Area` series bound with
`CategoryMember`/`ValueMember` plus a dashed comparison series that exists only when the period has
a comparison; **Top countries**, a bound `Bar` chart; and **Product mix**, a bound `Treemap` of the
twenty best-selling products.

**Customers** keeps Module 1's map and customer card and gains the customer list on the right:
every customer with `City · Country`, 91 rows. A country arriving from the country chart (the
navigation target is the Northwind country name, a `string`) shows as a closable `ChipLabel`
(`Country: UK`) above the list and reduces the list to that country.

## What to try

| Action | Expected result |
|---|---|
| Click **Sales** | 30D: eight category series stacked per day, axis to 32,000. Table: Dairy Products $32,562 · 27.5% · +124.5% first, Condiments $8,113 · 6.9% · -25.1% last. Status: `Series.DataSource = rows · CategoryMember, ValueMember`. |
| Click **Dairy Products** in the legend | The series disappears and the axis drops from 32,000 to 20,000. Status: `LegendItemClick → hidden series → axis max`. |
| Hide all eight categories | The chart shows `Every category is hidden · select a legend entry to show it`. Click the entries again to bring them back. |
| With Dairy Products hidden, select **All** | Dairy Products stays hidden (the choice survives the refresh); monthly buckets, axis to 100,000; the note adds `* partial month`. The bridge shows `Choose 30D, 90D or YTD to compare with an earlier period`; the trend loses its dashed series; the table loses VS. PRIOR and starts with Beverages $267,868 · 21.2%. |
| Select **30D** again | The bridge is back: `From $111,224 (Mar 8 – Apr 6, 1998) to $118,307 (Apr 7 – May 6, 1998) · +6.4%`, gray totals, green rises, red falls. The trend has exactly one `Comparison period` series again. |
| Click a bar of the bridge | Status: `ChartPoint.IsTotal · signed deltas`. |
| Scroll down, click the **UK** bar of **Top countries** | Customers opens with the chip `Country: UK`, `CUSTOMERS · 7` and seven rows: Around the Horn, B's Beverages, Consolidated Holdings, Eastern Connection, Island Trading (`Cowes · UK`), North/South, Seven Seas Imports (the others `London · UK`). The status strip shows the sender's `PointClick → e.Point.Tag → Customers`. |
| Click the row **Island Trading** | The card shows `Island Trading`, `Cowes · UK`, `Selected through the customer list`; the marker turns orange and the map moves to it. Status: `DataGridView.CellClick → DataBoundItem → customer`. |
| Back on Sales, click the **Brazil** bar | The chip changes to `Country: Brazil`, `CUSTOMERS · 9`. Island Trading stays in the card. |
| Click the **×** of the chip | The chip disappears, `CUSTOMERS · 91`. Status: `ChipLabel.CloseClick → country filter removed`. |
| Ctrl+K, `around`, Enter | Around the Horn is selected (`London · UK`, `Selected through a navigation target`). Status: `CommandExecuted → Tag → Customers`. |
| Ctrl+K, `wolski`, Enter, then the **USA** bar on Sales | `Country: USA`, `CUSTOMERS · 13`, and the short list starts at its first row, Great Lakes Food Market. |
| Change the period while a country is set | The chip, the list and the selection stay. |
| On Sales, click the **Raclette Courdavault** tile ($15,345) | Products opens (its placeholder until Module 5). Status: `PointClick → e.Point.Tag → Products`. |
| Double-click a row of the category table | Products opens. Status: `CellDoubleClick → DataBoundItem → Products`. |
| On Overview select **90D**, then open Sales | The hidden Sales view caught up: `Stacked net revenue per week`, bridge `From $224,090 (Nov 8, 1997 – Feb 5, 1998) to $320,348 (Feb 6 – May 6, 1998) · +43.0%`. |
| On Overview click the **Revenue** card | Sales opens. Status: `KpiPanel.Click → Sales`. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Bound ChartView and matching data table | `revenueChart` in [Views/SalesView.Designer.cs](NorthwindDashboard/Views/SalesView.Designer.cs) (`CategoryMember = "Label"`, `ValueMember = "Revenue"` / `"PreviousRevenue"`), `UpdateRevenue` in [Views/SalesView.cs](NorthwindDashboard/Views/SalesView.cs); `categoryGrid` and `UpdateCategoryTable` |
| Comparison series restored exactly once | `comparisonSeries` kept in the constructor; `UpdateRevenue` adds or removes that one object |
| Stacked-series and legend boundary checks | `UpdateCategoryTrend` (one series per category, aligned buckets, `Tag`), `categoryTrendChart_LegendItemClick`, `RescaleCategoryTrend` (largest sum of the visible series per bucket), `hiddenCategories` |
| Waterfall arithmetic and missing-comparison state | `UpdateBridge`: `IsTotal` endpoints, signed deltas, empty series and `EmptyState` without a comparison |
| Bar or treemap drill-down event trace | `countriesChart_PointClick`, `mixChart_PointClick`: the bound row is in `e.Point.Tag` |
| A country drill-down opens Customers | `ShowTarget` (`case string name`) in [Views/CustomersView.cs](NorthwindDashboard/Views/CustomersView.cs) |
| The filter is visible and removable | `countryChip` (`ChipLabel`, `Closable`) in [Views/CustomersView.Designer.cs](NorthwindDashboard/Views/CustomersView.Designer.cs), `ApplyFilter`, `countryChip_CloseClick` |
| The same business rows behind chart and list | `ApplyFilter` filters `Dashboard.Database.Customers` by the country name the chart row carries |
| Repeated-refresh evidence and reviewed patch | every `Update…` method of `SalesView` clears or reassigns; `RefreshView` is safe to call repeatedly |
| The sender's event path stays after a drill-down | `dashboard_NavigationRequested` and `SetStatus` in [MainPage.cs](NorthwindDashboard/MainPage.cs); see [docs/ARCHITECTURE.md](NorthwindDashboard/docs/ARCHITECTURE.md) |

## Differences from the video

- **Figures and size.** Eight real categories over 30 daily buckets instead of three categories over
  three weeks.
- **Table.** The video's table lists three week values; the sample's table lists the category rows
  the chart is built from (as the finished application does), with plain text columns: the rich grid
  column types are Module 4's subject.
- **Bridge.** `ChartView` draws the two totals in the neutral gray and adds connectors; the video
  draws them blue. The bridge is on the same page under the stacked chart, not a separate phase.
- **Revenue trend.** Not in the video. The lab asks for a bound area chart with a comparison series
  (the lesson shows it as `revenueChart`), so it sits beside the bridge.
- **Status.** `ChartPoint.IsTotal · signed deltas` is written when a bridge bar is clicked. The
  data-binding line is written when the view loads a period, not continuously.
- **Customers keeps its map and card.** The video's Customers screen in this module is only the chip
  and the table; the application keeps Module 1's map and card and puts the list beside them, and
  before a country arrives the list shows all 91 customers.
- **Chip.** `ChipLabel` draws a soft blue chip with a close button; the video draws a filled one
  without it.
- **Source line.** The card's line reads `Selected through a navigation target` where Module 1 said
  `Selected through CommandExecuted`: from this module on a customer can also arrive from other views.
- **Order and names.** The list is in company order; the country names are Northwind's (`UK`, `USA`).
- **First row of the table.** The first row of the category table can look selected when the view
  is first shown; it is not a selection the code made, and it clears with the next refresh.
- Captions such as `· StackedColumn` and `· Waterfall` are annotations and left out.
