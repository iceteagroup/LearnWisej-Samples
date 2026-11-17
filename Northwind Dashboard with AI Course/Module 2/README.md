# Northwind Dashboard · Wisej.NET 4.2: Build a Northwind Dashboard with AI · Module 2

Local lab build for **Module 2 · Build KPI Cards, Sparklines and Meters** (lab: *Create the Northwind
Metric Row*). The Overview's cards gain their miniature trends, an on-time gauge and the order-stage
meter, and **Fulfillment** opens for the first time with one `RadialGauge` per shipper.

This folder is the complete application at this point in the course, with its own solution and
port. The finished dashboard that every module works toward is in
[`../Wisej.NorthwindDashboard`](../Wisej.NorthwindDashboard).

## Run it

```bash
cd "Northwind Dashboard with AI Course/Module 2/NorthwindDashboard"
dotnet run -f net10.0 --urls http://localhost:6512
```

Open <http://localhost:6512>. In Visual Studio, open `NorthwindDashboard.slnx` and press F5.

Requirements: the .NET 10 SDK and a Wisej.NET development or trial license. The project restores
the Wisej.NET package from the course's `Wisej.NorthwindDashboard/packages` folder
(`NuGet.Config` maps `Wisej-4` to that folder and everything else to nuget.org) into
`.nuget-packages` in the course folder, a package cache shared by the seven module applications
and ignored by git. The customer map loads its tiles from OpenStreetMap, so it needs an internet
connection.

## What this module adds

Everything in [Module 1](../Module%201) is still here: the shell, the period selector, the command
palette, the Customers map. New in this module:

| Area | Change | Controls |
|---|---|---|
| Overview | A sparkline on the Revenue, Orders and Average order cards; the **Shipped on time** card with a gauge; the **Order stages** meter; the comparison note | `KpiPanel` (`Visual = Sparkline`, `Visual = Gauge`), `MeterBar` with `MeterSegment` |
| Fulfillment | New view: the on-time rate of each shipper for the selected period | `RadialGauge` with `MeterThreshold` |

Sales, Orders, Products and Team still say which module builds them.

## The screen

**Overview** has four `KpiPanel` cards. Revenue, Orders and Average order keep their value and trend
from Module 1 and gain the miniature trend (`SparklineValues`, one value per bucket of the period).
**Shipped on time** uses `Visual = Gauge` with `ProgressValue`, a caption (`0 late of 53 shipped`)
and a tone that follows the rate: success from 95%, warning from 90%, danger below. Under the cards,
the **Order stages** card holds a `MeterBar` whose three segments (New, Picking, Packed) are the
open orders by warehouse step, and two note lines: the comparison the trends are measured against,
and the Module 1 period label. The cards are clickable.

**Fulfillment** shows one card per shipper: a `RadialGauge` with the on-time rate of the selected
period on a 0 to 100 scale, the shipper's name, and three lines (shipped and late orders, average
days to ship, orders open now). Each gauge has three `MeterThreshold` entries, danger from 0,
warning from 90 and success from 95; the note beside the cards repeats the boundaries
(`warning ≥ 90 · success ≥ 95`) and names the period (`Orders placed Apr 7 – May 6, 1998`).

## What to try

| Action | Expected result |
|---|---|
| Start the application | 30D: Revenue $118,307.15 (▲ 6.4%), Orders 74 (▲ 5.7%), Average order $1,598.75 (▲ 0.6%), each with a sparkline. Shipped on time 100.0% (▲ 2.9 pts), `0 late of 53 shipped`, a full green gauge. Order stages `9 new · 7 picking · 5 packed · 21 open`. `Comparison: previous 30 days · Mar 8 – Apr 6, 1998`. Status: `SalesAnalytics.Summary → KpiPanel.Value, Trend, SparklineValues`. |
| Select **90D** | $320,348.28 (▲ 43.0%), 205 (▲ 48.6%), $1,562.67 (▼ 3.8%), 97.8% (▲ 3.6 pts, `4 late of 184 shipped`). `Comparison: previous 90 days · Nov 8, 1997 – Feb 5, 1998`. |
| Select **YTD** | $440,623.95 (▲ 119.4%), 270 orders, 96.8% on time. `Comparison: same dates one year earlier · Jan 1 – May 6, 1997`. |
| Select **12M**, then **All** | 12M: $856,885.44, 549 orders, 95.6% (`23 late of 528 shipped`). All: $1,265,793.29, 830 orders, $1,525.05, 95.4% (`37 late of 809 shipped`). Neither has a trend on any card: `No comparison period: Trend suppressed`. |
| Change the period several times | The meter keeps its three segments and `21 open`: the open orders do not depend on the period, and the segments are cleared before they are added. |
| Click the **Orders** card | Orders opens (`This view is built in Module 4.`). Status: `KpiPanel.Click → Orders`. |
| Click **Revenue** or **Average order** | Sales opens (`This view is built in Module 3.`). Status: `KpiPanel.Click → Sales`. |
| Click **Shipped on time** | Fulfillment opens. Status: `KpiPanel.Click → Fulfillment`: the card's event path stays, the view does not replace it with its own opening line. |
| Click **Fulfillment** in the sidebar (30D) | Speedy Express 100.0% (18 shipped · 0 late, 6.4 days to ship, 4 open now), United Package 100.0% (26 shipped · 0 late, 6.2 days, 11 open now), Federal Shipping 100.0% (9 shipped · 0 late, 5.7 days, 6 open now). All three green. Status: `RadialGauge.Value = onTime / shipped`. |
| Select **90D** | Speedy Express 94.3% in the warning color (53 shipped · 3 late), United Package 98.7% (79 · 1), Federal Shipping 100.0% (52 · 0). |
| Select **YTD**, then **12M** | YTD: 94.6% warning (74 · 4), 97.2% (106 · 3), 98.6% (69 · 1). 12M: 94.7% warning (170 · 9), 94.6% warning (203 · 11), 98.1% (155 · 3). |
| Select **All** | Speedy Express 95.1% success (245 shipped · 12 late, 8.6 days), United Package 94.9% warning (315 · 16, 9.2 days), Federal Shipping 96.4% success (249 · 9, 7.5 days). 94.9 is just below the 95 boundary and 95.1 just above it. |
| Open Overview and return to Fulfillment | The gauges keep the period; the status returns to `RadialGauge.Value = onTime / shipped`. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| KpiPanel configuration and displayed values | `revenueKpi`, `ordersKpi`, `averageKpi`, `onTimeKpi` in [Views/OverviewView.Designer.cs](NorthwindDashboard/Views/OverviewView.Designer.cs) (`Title`, `TrendFormat`, `Visual`, `Clickable`); `UpdateKpis` in [Views/OverviewView.cs](NorthwindDashboard/Views/OverviewView.cs) assigns `Value`, `Trend`, `SparklineValues` / `ProgressValue`, `Tone` and `Caption` from one `PeriodSummary` |
| Ordered Sparkline fixture and customer binding | `SparklineValues = summary.RevenueTrend`: one value per `DashboardPeriod.Buckets` entry, oldest first. The standalone customer `Sparkline` arrives with the customer card in Module 6 |
| MeterBar segment totals after repeated refresh | `UpdateStages`: `Maximum`, `Value`, `Segments.Clear()`, then three `MeterSegment` objects and `ValueText` |
| RadialGauge boundary and empty-state evidence | `UpdateShippers` and the constructor (`WarningFrom`, `SuccessFrom`) in [Views/FulfillmentView.cs](NorthwindDashboard/Views/FulfillmentView.cs); the `shipper.Shipped == 0` branch (`NoRateFormat`); `ValueFormat` and `Label` in the designer. On the Overview card: the dash, the neutral tone and `No shipments yet` when `summary.Shipped == 0` |
| The rate passed as 95.1, not 0.951 | `UpdateShippers`: the gauge's scale is 0 to 100 |
| Missing comparison | `comparisonLabel` in `RefreshView`; a null `Trend` hides the indicator |
| Independent metric calculations and reviewed patch | `SalesAnalytics.Summarize` and `SalesAnalytics.ShipperPerformance` in the shared `Data` layer; the views only format |
| Refresh twice without duplicating collections | thresholds are added once in the constructor; `UpdateShippers` only assigns values; `UpdateStages` clears before it adds |
| Colors that match the application | [Themes/Northwind.OverviewSales.mixin.theme](NorthwindDashboard/Themes/Northwind.OverviewSales.mixin.theme) (`sparkline-primary`, `meterbar-*`) and [Themes/Northwind.Fulfillment.mixin.theme](NorthwindDashboard/Themes/Northwind.Fulfillment.mixin.theme) (`radialgauge-*`) |

## Differences from the video

- **Figures** are the real ones (74 orders, $118,307.15), not the video's rounded fixture. The video
  shows shipper rates of 94% and 97% and a shipper without shipments; the real 30-day rates are
  100.0% for all three, and the warning tone appears from 90D on.
- **Fourth card.** The video's Overview shows three cards; the lesson and the finished application
  have the on-time card with the gauge, so the sample has it.
- **Sparkline.** `KpiPanel` draws a filled line with an end marker and no highlighted point; the
  video highlights one point in orange.
- **Trend.** Arrow and percentage (▲ 6.4%), and nothing at all when there is no comparison, where
  the video prints `+6.2%` and a dash.
- **Meter.** `MeterBar` has no legend row: the segment counts are in its `ValueText`
  (`9 new · 7 picking · 5 packed · 21 open`). The real open orders have three steps (New, Picking,
  Packed); the video's fixture shows two.
- **No empty shipper in the data.** Every shipper ships in every period of the Northwind snapshot,
  so the dash on an empty track (`No shipments in this period`) is implemented but never shown by
  the real data. To see it, change the data in a copy, for example move one shipper's orders of the
  last 30 days to another shipper in `Data/northwind.json`.
- **Period on Fulfillment.** The finished application measures the gauges over the last 12 months
  and hides the period selector on this view. Here the gauges follow the period selector, as the
  video's header shows and the lab's period checks require.
- **Gauge layout.** Each gauge sits in a card with the shipper name and three figures beside it; the
  video draws the name under the gauge.
- **Captions.** `Order stages` and `On-time thresholds`: the control names in the video's captions
  (`· MeterBar`, `RadialGauge thresholds`) are annotations for the viewer.
