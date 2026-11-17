# Northwind Dashboard · Wisej.NET 4.2: Build a Northwind Dashboard with AI · Module 5

Local lab build for **Module 5 · Coordinate Workflow, Scheduling and Inventory Controls** (lab:
*Build a Coordinated Operations Workspace*). **Fulfillment** becomes a workspace with a board, a
calendar and a timeline of the same open orders; **Products** opens with the inventory and the
reorder cards; **Overview** gains the activity timeline and the order heatmap. From this module on
the session changes its own data: an order shipped on the board, or a purchase order placed from a
card, is visible in every view of that session and in no other session.

This folder is the complete application at this point in the course, with its own solution and
port. The finished dashboard that every module works toward is in
[`../Wisej.NorthwindDashboard`](../Wisej.NorthwindDashboard).

## Run it

```bash
cd "Northwind Dashboard with AI Course/Module 5/NorthwindDashboard"
dotnet run -f net10.0 --urls http://localhost:6515
```

Open <http://localhost:6515>. In Visual Studio, open `NorthwindDashboard.slnx` and press F5.

Requirements: the .NET 10 SDK and a Wisej.NET development or trial license. The project restores
the Wisej.NET package from the course's `Wisej.NorthwindDashboard/packages` folder
(`NuGet.Config` maps `Wisej-4` to that folder and everything else to nuget.org) into
`.nuget-packages` in the course folder, a package cache shared by the seven module applications
and ignored by git. The customer map loads its tiles from OpenStreetMap, so it needs an internet
connection.

## What this module adds

Everything in [Module 4](../Module%204) is still here: Sales, the Orders workbench and its dialog,
the customer map and list. New in this module:

| Area | Change | Controls |
|---|---|---|
| Fulfillment | Under the shipper gauges: **Open orders** as Board, Calendar or Timeline, and the **Activity** list | `SegmentedButton`, `KanbanBoard`, `SchedulerView`, `GanttView`, `Timeline` |
| Products | New view: inventory health, stock filters, the catalog and the reorder suggestions | `MeterBar`, `ChipLabel`, `KpiPanel`, `ChipGroup`, `ActionCardList`, `DataGridView` subtitle, sparkline, meter, chip and actions columns |
| Overview | The **Activity** timeline and the **Order activity** heatmap | `Timeline` (`Grouping`, `LoadMore`), `HeatmapCalendar` |
| Orders | Unchanged, and now receives a day from the heatmap and a product from Products | |
| Sales | Unchanged; its category rows and treemap tiles now open the real Products view | |

Team still says which module builds it.

## The screen

**Fulfillment** keeps the three shipper gauges and adds the **Open orders** card with a
`SegmentedButton` (Board, Calendar, Timeline) and, beside it, the **Activity** `Timeline`.

- **Board** is a `KanbanBoard` bound to one card per order (`StatusMember = "Stage"`), columns New,
  Picking (WIP limit 8), Packed and Shipped. It holds the 21 open orders and the six most recent
  shipments.
- **Calendar** is a `SchedulerView` with one `SchedulerResource` per shipper; every open order is a
  `Due · …` appointment on its required date and every shipment of the last five weeks a
  `Shipped · …` appointment, each with the shipper's `ResourceId` and the order in `Tag`.
- **Timeline** is a `GanttView`: one parent task per shipper with open orders and one child per
  order from its order date to its required date, `Progress` from the stage.
- **Activity** lists the session's activity newest first (orders shipped, purchase orders placed in
  Products), followed by the snapshot's six latest shipments.

**Products** shows the inventory `MeterBar` with five selectable `ChipLabel` stock filters, the
`NEEDS ATTENTION` `KpiPanel`, the catalog grid under a category `ChipGroup`, and **Reorder
suggestions**, an `ActionCardList` whose cards get an `Order <qty>` action in `CardCreated`.

**Overview** keeps the cards, the order stages and the notes, and adds two cards. **Activity** is a
`Timeline` grouped by day: the session's own events first, then the most recent orders, fifteen at
a time, with **Older orders** at the end. **Order activity** is a `HeatmapCalendar` of the orders
per day over the last twelve months.

## What to try

| Action | Expected result |
|---|---|
| Open **Fulfillment** | `Open orders · 21 · 2 due within 7 days`. New 9, Picking 7 / 8, Packed 5, Shipped 6. Activity: six shipments, newest `Order #11069 shipped`. Status: `KanbanBoard.CardMoving → validate the move`. |
| Drag **#11068 · Queen Cozinha** from New to Picking | New 8, Picking 8 / 8. Gauges and Activity unchanged. Status: `CardMoved → e.Column.Value → context.SetStage`. |
| Move it on to Packed (drag, or focus the card and press **Alt+Right**) | Picking 7 / 8, Packed 6. Still no activity entry. |
| Drag it to Shipped | `Open orders · 20`. Shipped 7. Activity gains `Order #11068 shipped`. United Package: 27 shipped · 0 late, 6.0 days to ship, 10 open now (before: 26, 6.2, 11). Alert `Order #11068 shipped via United Package.` |
| Drag the shipped card back to Packed (or **Alt+Left**) | The card stays in Shipped, alert `Order #11068 has already shipped and cannot move back.` Counts, gauges and Activity unchanged. Status: `CardMoving → e.Cancel = true · context unchanged`. |
| Reorder it inside Shipped (**Alt+Down**) | The card changes place in the column; no second activity entry, nothing else changes. |
| Drag **#11070** to Picking, then **#11071** | The first makes Picking 8 / 8; the second is refused with `Picking already holds 8 orders. Pack or ship one first.` Status: `KanbanBoard.WipLimitExceeded → e.Column.WipLimit · context unchanged`. |
| Select **Calendar** | One row per shipper for May 6, 1998. Summary: `111 appointments for 3 shippers · due dates and the shipments of the last five weeks` (21 due + 90 shipped before a shipment, 20 + 91 after). Status: `SchedulerAppointment.ResourceId → shipper`. |
| Click an appointment | `OrderDialog` opens for that order. Status: `SchedulerView.AppointmentClick → e.Appointment.Tag → OrderDialog`. |
| Select **Timeline** | Speedy Express (4), United Package (11) and Federal Shipping (6) with their orders. Summary: `21 open orders under 3 shippers · order date to required date` (20 after a shipment). Status: `GanttTask.ParentId → shipper`. Double-click a child: `GanttView.TaskDoubleClick → e.Task.Tag → OrderDialog`. |
| Change the period twice (90D, 30D) | Both summaries keep their counts: the collections are cleared before each rebuild. |
| Click `Order #11068 shipped` in Activity | `OrderDialog` opens. Status: `Timeline.ItemClick → e.Item.Tag → OrderDialog`. |
| Open **Products** | 77 products: 51 in stock, 15 incoming, 2 to reorder, 1 out of stock, 8 discontinued; `NEEDS ATTENTION 3 products`; `Reorder suggestions · 18`. Status: `ActionCardList.CardCreated → CardAction`. |
| Click **Order 20** on *Nord-Ost Matjeshering* | The card becomes `Incoming`, `10 in stock · reorder at 15 · 20 on order`, and its action `Order 10`. 16 incoming, 1 to reorder, `2 products`. Status: `ActionCard.ActionClick → context.PlacePurchaseOrder`. The view stays on Products. |
| Click **Order 10** on the same card | `30 on order`; stock on hand is still 10. |
| Click the card *Outback Lager* itself | Orders opens with the product as target: chips `Product: Outback Lager` and `Period: Last 30 days`, 4 orders, $7,261. Status: `ActionCard.Click → Navigate(Orders, product)`. |
| The **Show orders** icon of the row *Raclette Courdavault* | Orders: `Product: Raclette Courdavault`, 6 orders, $32,706. Status: `DataGridViewActionsColumn.ActionClick → Navigate(Orders, product)`. |
| Click the **Seafood** chip, then the **2 to reorder** chip | `Product catalog · 12 of 77 products`, then `1 of 77`: Nord-Ost Matjeshering. |
| Return to **Fulfillment** after a purchase | Activity starts with `Purchase order for Nord-Ost Matjeshering`: the hidden view was refreshed when shown. Clicking it opens Products with that product selected in the grid. Status: `Timeline.ItemClick → e.Item.Tag → Products`. |
| Open **Overview** | Activity starts on `Wednesday, May 6, 1998` with Rattlesnake Canyon Grocery, `Order #11077 · $1,255.72 · Nancy Davolio · Open`; 15 orders, down to #11063. The heatmap runs from May to May, legend 0 to 6. |
| Click the first Activity item, **Mark as shipped**, **Close** | The dialog opens on order #11077 (`Timeline.ItemClick → e.Item.Tag → OrderDialog`). After shipping, the visible Overview refreshes at once: `Order #11077 shipped` is the first item, the stages read `8 new · 7 picking · 5 packed · 20 open`, the on-time card `0 late of 54 shipped`. Orders then shows `Open (20) · Shipped (54)`. |
| Scroll the Activity to the end, click **Older orders** twice | 30, then 45 orders, none repeated. Status: `Timeline.LoadMore → next 15 orders · 30 shown`, then `· 45 shown`. |
| Click the last cell of the heatmap (May 6, 1998) | Orders opens for that day across all periods: `Orders placed on May 6, 1998`, chip `Day: 1998-05-06`, 4 orders, $2,779, summary ending in `all periods`. Status: `HeatmapCalendar.CellClick → day scope`. |
| Close the day chip (×) | Back to the period: `Period: Last 30 days`, 74 orders. Status: `ChipLabel.CloseClick → target cleared → ApplyFilters`. |
| On **Sales**, double-click the *Seafood* row; click the *Raclette Courdavault* tile | Products opens with the Seafood chip selected (12 of 77 products); with the tile, the product's row is selected. Status: `CellDoubleClick → DataBoundItem → Products` and `PointClick → e.Point.Tag → Products`. |
| Open the application in a private window | 21 open orders, #11068 in New, 26 shipped for United Package, Nord-Ost Matjeshering `Reorder` with nothing on order: the changes belong to the first session. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Kanban validation and state-change evidence | `kanbanBoard_CardMoving` (cancel), `kanbanBoard_CardMoved` (same-column return, `Enum.Parse(e.Column.Value)`, `Dashboard.SetStage`, `RefreshCard`), `kanbanBoard_WipLimitExceeded` in [Views/FulfillmentView.cs](NorthwindDashboard/Views/FulfillmentView.cs); column values and member mappings in [Views/FulfillmentView.Designer.cs](NorthwindDashboard/Views/FulfillmentView.Designer.cs); `FulfillmentCard` |
| The `moving` flag | `OnOrdersChanged` in `FulfillmentView` |
| Scheduler and Gantt identity/date fixture | `UpdateSchedule`: `Resources.Clear()` / `Appointments.Clear()` / `Tasks.Clear()`, `Id`, `ResourceId`, `ParentId`, `Tag`; `scheduler_AppointmentClick`, `gantt_TaskDoubleClick` |
| Board, Calendar and Timeline modes of the same orders | `viewSwitcher_SelectionChanged`, `Mode`, `ModeStatus`, `UpdateSummary` |
| Timeline and heatmap navigation checks | `activityTimeline_ItemClick` (dispatch on `e.Item.Tag`: `Order` → `OrderDialog`, `Product` → Products) and `ordersHeatmap_CellClick` (`e.Date` → Orders) in [Views/OverviewView.cs](NorthwindDashboard/Views/OverviewView.cs); `UpdateActivity`, `activityTimeline_ItemClick` in `FulfillmentView` |
| `Timeline.Items` rebuilt deliberately, `LoadMore` without duplicates | `UpdateActivity` (`Items.Clear()`, session events, first page), `AppendOrders` (`Skip(loadedOrders).Take(OrdersPerPage)`, `HasMoreItems`), `activityTimeline_LoadMore` in `OverviewView` |
| HeatmapCalendar bound to daily data | `UpdateHeatmap` (`SetDateRange`, `DataSource = SalesAnalytics.DailyOrders(...)`), loaded once |
| ActionCardList action and incoming-stock proof | `reorderList_CardCreated`, `PlacePurchaseOrder`, `Suggestion` in [Views/ProductsView.cs](NorthwindDashboard/Views/ProductsView.cs); `reorderList` in [Views/ProductsView.Designer.cs](NorthwindDashboard/Views/ProductsView.Designer.cs) |
| Card `Click` separate from `ActionClick` | the two subscriptions in `reorderList_CardCreated` |
| Repeated-action and session-isolation results | `DashboardContext.SetStage` / `PlacePurchaseOrder` (session dictionaries); `OrdersChanged` / `ProductsChanged` and the base view's refresh when a hidden view is shown |
| Navigation targets | `ProductsView.ShowTarget` (`Category`, `Product`); `OrdersView.ShowTarget` (`DateTime`, `Product`) |

## Differences from the video

- **Where things are.** The Module 5 video stays on Fulfillment and Orders and draws its activity
  `Timeline` and the reorder cards on the Fulfillment page. The lesson puts `Timeline.Items`,
  `ItemClick` and `LoadMore` on Overview and the reorder cards in Products, as the finished
  application does, and the sample follows the lesson: Activity is on Overview and on Fulfillment,
  the cards are in Products.
- **Status texts for the modes.** The video shows one combined line,
  `SchedulerAppointment.ResourceId · CardCreated → CardAction`. The application shows
  `SchedulerAppointment.ResourceId → shipper` for Calendar, `GanttTask.ParentId → shipper` for
  Timeline and `ActionCardList.CardCreated → CardAction` on Products.
- **Calendar.** The video draws five day columns with one bar per shipper. `SchedulerView` opens in
  its Timeline mode: one row per shipper for the as-of day, with hour columns, plus the control's
  own `Unassigned` row. Day, Week, Month and Agenda are one click away in the control's header.
- **Timeline.** The video's Gantt shows one order with Pick, Pack and Ship children. The
  application follows the lesson: a parent per shipper and a child per open order.
- **Activity.** The video lists `shipped` and `packed` events. The context records an activity only
  when an order ships (and for purchase orders), so there are no `packed` entries. On Overview the
  items show the customer as title and the order as description, grouped under day headings.
- **Gauges.** The video shows one small Speedy Express gauge under the activity list; the
  application keeps Module 2's three gauge cards above the board. With real data the 30-day rate
  stays 100.0% after a shipment (27 of 27), so the visible change is in the counts.
- **Feedback.** A refused or completed move also shows an `AlertBox`; the video draws a red label on
  the board.
- **Board size.** A lane shows at most eight cards and then `+ 1 more` (`MaxVisibleCards`).
- **Day scope.** The video's `Day: 1998-05-06` chip is the Orders view's scope chip; the card title
  adds `Orders placed on May 6, 1998`.
