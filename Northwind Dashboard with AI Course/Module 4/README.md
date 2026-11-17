# Northwind Dashboard · Wisej.NET 4.2: Build a Northwind Dashboard with AI · Module 4

Local lab build for **Module 4 · Build Rich Grids, Filters and Order Details** (lab: *Create a 4.2
Orders Workbench*). **Orders** opens: status chips with counts, a search box, a typed rule builder,
a grid that uses all six new column types, an empty state with a recovery action, and the order
dialog with its four-step `Stepper`.

This folder is the complete application at this point in the course, with its own solution and
port. The finished dashboard that every module works toward is in
[`../Wisej.NorthwindDashboard`](../Wisej.NorthwindDashboard).

## Run it

```bash
cd "Northwind Dashboard with AI Course/Module 4/NorthwindDashboard"
dotnet run -f net10.0 --urls http://localhost:6514
```

Open <http://localhost:6514>. In Visual Studio, open `NorthwindDashboard.slnx` and press F5.

Requirements: the .NET 10 SDK and a Wisej.NET development or trial license. The project restores
the Wisej.NET package from the course's `Wisej.NorthwindDashboard/packages` folder
(`NuGet.Config` maps `Wisej-4` to that folder and everything else to nuget.org) into
`.nuget-packages` in the course folder, a package cache shared by the seven module applications
and ignored by git. The customer map loads its tiles from OpenStreetMap, so it needs an internet
connection.

## What this module adds

Everything in [Module 3](../Module%203) is still here: the Overview cards, Sales, the shipper gauges,
the customer map and list. New in this module:

| Area | Change | Controls |
|---|---|---|
| Orders | New view: the orders workbench | `ChipGroup`, `QueryBuilder`, `EmptyState`, `ChipLabel`, `DataGridView` with `DataGridViewSubtitleColumn`, `DataGridViewAvatarColumn`, `DataGridViewChipColumn`, `DataGridViewSparklineColumn`, `DataGridViewMeterColumn`, `DataGridViewActionsColumn` |
| Order dialog | New: `Dialogs/OrderDialog`, opened from the grid | `Stepper`, `ChipLabel`, `Avatar` |
| Overview | The Orders card now opens the real Orders view | |
| Shared | `Views/GridRows.cs`, the row classes every later grid binds to | |

Products and Team still say which module builds them.

## The screen

**Orders** is the workbench of the module. Above the list: the `statusFilter` `ChipGroup` (All,
Open, Overdue, Shipped, Shipped late, each with its count), the search box and the **Advanced
filter** button, which opens a card with the `QueryBuilder` and its nine typed fields. The list
card shows the scope as a `ChipLabel` (`Period: Last 30 days`) and a `DataGridView` whose seven
columns use all six new column types:

| Column | Type | Bound to (`OrderRow`) |
|---|---|---|
| Order | `DataGridViewSubtitleColumn` | `Number`, `SubtitleMember = DateText` |
| Product | `DataGridViewSubtitleColumn` | `Product` (the line with the highest value), `SubtitleMember = ProductDetail` (`Beverages · +2 more`) |
| Customer | `DataGridViewAvatarColumn` | `Customer`, `SubTextMember = CustomerPlace` |
| Status | `DataGridViewChipColumn` | `StatusText`, `ToneMember = StatusTone`, `ShowDot` |
| Trend | `DataGridViewSparklineColumn` | `CustomerTrend`: the net value of the customer's last six orders up to this one, oldest first, `ChartType = Bar`; `ShowValue` prints the last point, which is this order's amount |
| Progress | `DataGridViewMeterColumn` | `Progress` (1 New, 2 Picking, 3 Packed, 4 Shipped), `Maximum = 4`, `TextMember = ProgressText`, green from 4 |
| Actions | `DataGridViewActionsColumn` | one `CardAction` named `open`, text **Open** |

When the filters leave no order, an `EmptyState` (`No orders match`, **Clear filters**) covers the
grid. **Open**, or a double-click, shows the `OrderDialog`: a `ChipLabel` with the status, a
four-step `Stepper`, the customer and sales rep as `Avatar` controls, and the order lines.

The walkthrough video is a simulation with eight fixture rows; this application lists the real
orders. Last 30 days: 74 orders, $118,307 net (21 open, 53 shipped). All time: 830 orders,
$1,265,793 (21 open, 772 shipped, 37 shipped late). No order is overdue on the as-of date.

## What to try

| Action | Expected result |
|---|---|
| Click **Orders** | 74 orders, newest first (#11077, May 6, 1998). Chips `All (74) · Open (21) · Overdue (0) · Shipped (53) · Shipped late (0)`. Summary `Showing 74 orders · $118,307 net · last 30 days (Apr 7 – May 6, 1998)`. Status: `Candidates → counts → selected status`. |
| On Overview, click the **Orders** card | The same view. Status: `KpiPanel.Click → Orders`. |
| Click the **Shipped** chip | 53 orders, $92,370. The other chips keep their counts (All 74, Open 21). Status: `ChipGroup.SelectionChanged → ApplyFilters`. |
| Select period **All** | `All (830) · Open (21) · Overdue (0) · Shipped (772) · Shipped late (37)`, $1,265,793. |
| **Advanced filter**, **+ Rule**, `Amount ($)`, `>`, `9000` | 13 orders, $148,348; chips `All (13) · Shipped (12) · Shipped late (1)`. The first row is #11030 with $12,615: a text comparison would have dropped it, because "12615" sorts before "9000". Status: `QueryBuilder rules → typed comparison`. |
| With that rule, click **Shipped late** | 1 order ($9,921); the counts stay 13 / 12 / 1. |
| Change the rule's value to `99999` | No rows: `No orders match`, `Try another status, search text or filter rule.`, **Clear filters**. All five counts are 0. |
| Click **Clear filters** | 830 orders again, the **All** chip selected, the rule gone (`No rules`, button `Advanced filter`), the search box empty. Status: `EmptyState.ActionClick → ClearFilters → ApplyFilters`. |
| Click the **Customer** header, then **Open** on the fourth row | The dialog shows the order of that row (30D: #11073 after sorting by Customer, #11062 by Product, #11056 by Order). Status: `ActionClick → bound row → OrderDialog`. |
| Double-click a row | Same dialog. Status: `CellDoubleClick → bound row → OrderDialog`. |
| Type `11077` in the search box, **Open** | Unshipped order: chip `Open`; steps Ordered ✓ `May 6, 1998`, Picked and packed **active** `Waiting`, Shipped pending `Not shipped`, Delivery due pending `Jun 3, 1998`; **Mark as shipped** is offered. |
| In that dialog click **Mark as shipped**, then **Close** | Chip `Shipped`, the button disappears. With the search box cleared the chips read `Open (20) · Shipped (54)`. On Overview (refreshed when shown): `8 new · 7 picking · 5 packed · 20 open`, `0 late of 54 shipped`; on Fulfillment, United Package has 27 shipped and 10 open now. |
| Search `11008`, **Open** | Packed order: Ordered ✓, Picked and packed ✓ `Packed`, Shipped **active** `Not shipped`, Delivery due pending `May 6, 1998`. |
| Search `11069`, **Open** | Shipped on time: chip `Shipped`, four completed steps (`May 6, 1998`, due `Jun 1, 1998`). |
| Search `10970` (period **All**), **Open** | Shipped late: chip `Shipped late`; Ordered ✓, Picked and packed ✓ `Done`, Shipped **error** `Apr 24, 1998 · 17 days late`, Delivery due **error** `Apr 7, 1998`. |
| Click a customer's avatar in the grid | The Customers view opens on that customer. Status: `AvatarClick → bound row → Customers`. |
| In the dialog, click the customer's avatar | The dialog closes and Customers opens on that customer. Status: `Avatar.Click → Navigate(Customers, customer)`. The sales rep's avatar opens Team (`This view is built in Module 6.`). |

## Stepper state table (the lab's four cases)

| Case | Order | Ordered | Picked and packed | Shipped | Delivery due | Status chip |
|---|---|---|---|---|---|---|
| Unshipped, new | #11077 | Completed | Active · `Waiting` | Pending · `Not shipped` | Pending | Open |
| Unshipped, packed | #11008 | Completed | Completed · `Packed` | Active · `Not shipped` | Pending | Open |
| Shipped on time | #11069 | Completed | Completed · `Done` | Completed · date | Completed | Shipped |
| Shipped late | #10970 | Completed | Completed · `Done` | Error · date · `17 days late` | Error | Shipped late |

An unshipped order in the Picking step shows `Active · Picking` on the second step by the same code
path. An overdue unshipped order would show `Error` on Delivery due; the data has none on the as-of
date.

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Six column configurations with fixture rows | The seven columns of `ordersGrid` in [Views/OrdersView.Designer.cs](NorthwindDashboard/Views/OrdersView.Designer.cs); the row properties they name in `OrderRow`, [Views/GridRows.cs](NorthwindDashboard/Views/GridRows.cs) |
| Sorted and filtered action identity evidence | `colActions_ActionClick` (`e.DataBoundItem`), `RowAt` and `ordersGrid_CellDoubleClick` in [Views/OrdersView.cs](NorthwindDashboard/Views/OrdersView.cs) |
| Typed rules and chip-count expectations | `CreateQueryFields`, the two `Evaluate` overloads, and stages 5 and 6 of `ApplyFilters` |
| `EmptyState` recovery test | `emptyState` in the constructor (`Kind`, `TargetControl`, `ActionText`), `emptyState_ActionClick`, `ClearFilters` |
| Four-case Stepper state table and reviewed patch | `UpdateStatus` in [Dialogs/OrderDialog.cs](NorthwindDashboard/Dialogs/OrderDialog.cs); the four `Step` objects in [Dialogs/OrderDialog.Designer.cs](NorthwindDashboard/Dialogs/OrderDialog.Designer.cs) |
| Every filter stage visible | `ApplyFilters`: scope → target → search → rules → chip counts → selected status → sort |
| A target as an explicit scope with its caption | `ShowTarget`, the first `Where` of `ApplyFilters`, `TargetScope`, `ShowScope`. The views that send a day, a product or a sales rep arrive in Modules 5 and 6; the grid's avatar and the dialog already send a `Customer` to Customers |
| Shipping an order refreshes every view | `shipButton_Click` → `DashboardContext.SetStage` → `OrdersChanged` → `DashboardView.OnOrdersChanged` (visible views refresh, hidden ones when shown) |
| Colors of the grid cells, the rule builder and the stepper | [Themes/Northwind.Orders.mixin.theme](NorthwindDashboard/Themes/Northwind.Orders.mixin.theme) |

## Differences from the video

- **Rows and figures.** The video lists eight fixture orders (10248–10255); the application lists
  the real orders of the period, newest first.
- **Status chips.** The video's chips are the warehouse steps (New, Picking, Packed, Shipped). The
  application's chips are the delivery states the lesson describes: All, Open, Overdue, Shipped,
  Shipped late. The warehouse step is the Progress column.
- **Chip captions.** `ChipGroup` prints a count in brackets: `Shipped (53)`.
- **Order column.** A subtitle column too: the order number with the order date under it. The
  list is sorted by date, so the date has to be visible.
- **Product column.** An order has several lines; the cell names the line with the highest value
  and says how many more there are (`Beverages · +2 more`).
- **Trend column.** Bars for the customer's last six orders, ending with this order, and this
  order's amount beside them (`ShowValue`, `ValueFormat = "{0:C0}"`). The video draws three bars.
- **Progress column.** A scale of four steps (`Maximum = 4`) with the step's name under the bar,
  not a percentage.
- **Query builder.** The video prints a caption (`QueryBuilder: Amount > 20`). The application has
  the real control in a card opened by **Advanced filter**; the button shows the number of rules.
  The amounts are real, so the example rule is `Amount > 9000`.
- **Search box.** The application has the finished application's search box; the video has none.
- **Scope chip.** Always shown: `Period: Last 30 days`. A target that another view sends adds a
  second, closable chip, or replaces it with `Day: 1998-05-06` (from Module 5).
- **Empty state.** The real `EmptyState` draws its own icon; the video's caption
  `EmptyState · TargetControl = ordersGrid` is an annotation and is not shown.
- **Dialog.** The steps are the finished application's (`Ordered`, `Picked and packed`, `Shipped`,
  `Delivery due`) with a date or note under each, and a late shipment marks both `Shipped` and
  `Delivery due` as errors. The chip shows the delivery state (`Open`, `Shipped`, `Shipped late`),
  not `On time` / `Late`. The dialog also lists the order lines and totals and offers
  **Mark as shipped** for an open order.
- **Sorting.** A header click sorts the visible rows; the next filter change returns the list to
  newest first, because the sort is the last stage of `ApplyFilters`.
- **Order numbers in the palette.** The lesson opens the same order from the palette; the palette
  finds order numbers in [Module 7](../Module%207).
