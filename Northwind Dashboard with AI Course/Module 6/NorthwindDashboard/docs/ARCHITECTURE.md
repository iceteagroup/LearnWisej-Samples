# Northwind Dashboard · how the shell and the views fit together

This is the contract every module of the course builds on. A module adds views and replaces table
entries; it does not change how the shell and a view talk to each other. Module 6 adds the responsive
layout to the shell; the last sections describe it.

## The pieces

| Piece | File | Role |
|---|---|---|
| `SessionStartup.Start` | `SessionStartup.cs` | Runs once per browser session (named in `Default.json`): sets the culture and creates `MainPage`. |
| `MainPage` | `MainPage.cs`, `MainPage.Designer.cs`, `MainPage.resx` | The shell. Owns the session's one `DashboardContext`, the navigation, the period selector, the command palette and the status strip. |
| `DashboardContext` | `Data/DashboardContext.cs` | Session state: the selected period, navigation requests, and (from Module 5) the session's order and stock changes. |
| `NorthwindDatabase` | `Data/NorthwindDatabase.cs` | The immutable Northwind snapshot shared by all sessions (`NorthwindDatabase.Instance`). |
| `SalesAnalytics` | `Data/SalesAnalytics.cs` | Every calculation a view shows; takes the context, returns plain objects. |
| `DashboardView` | `Views/DashboardView.cs` | Base class of every view. |
| `DashboardStyle` | `Views/DashboardStyle.cs` | Shared number, date, color and icon-path conventions. |
| `OrderDialog` | `Dialogs/OrderDialog.cs` | The order details. Every view opens it with `OrderDialog.ShowOrder(Dashboard, order)`. |
| Row classes | `Views/GridRows.cs` | The objects the grids bind to (`OrderRow`, `ProductRow`, `CustomerRow`, `EmployeeRow`, ...). |
| `CardLayout` | `Views/CardLayout.cs` | Reflows a view's cards for the Tablet and Phone profiles and restores the designed layout. |

The `Data` folder is identical to the finished application's except for the namespace
(`NorthwindDashboard.Data`). Do not change it in a module; if a view needs a figure, it is already
in `SalesAnalytics` or `DashboardContext`.

## Controls on MainPage

| Field | Control | Notes |
|---|---|---|
| `appBar` | `Panel` (top, 46 px) | `menuButton` (hidden except on Phone), `brandLabel`, `versionLabel`; `toolsPanel` holds `asOfLabel`, `searchButton`, `userAvatar`. |
| `sidebar` | `Panel` (left, 150 px) | `overviewButton` … `teamButton`; each button's `Tag` is its `DashboardArea`. |
| `headerPanel` | `Panel` | `headingLabel` (the view's title) and `periodSelector` (`SegmentedButton`). |
| `viewHost` | `Panel` (fill) | Parent of every view; views are docked `Fill` and only the current one is visible. |
| `statusBar` / `statusLabel` | `Panel` + `Label` (bottom, 40 px) | The event path of the last interaction. |
| `commandPalette` | `CommandPalette` | `FilterMode = Server`, `Shortcut = Ctrl+K`. |

## MainPage members a module uses

```csharp
public void Navigate(DashboardArea area, object target = null);
public void SetStatus(string text);
```

The area table, in `MainPage.Areas.cs`, decides what `Navigate` shows. In this module:

```csharp
private static readonly Dictionary<DashboardArea, AreaEntry> areas = new Dictionary<DashboardArea, AreaEntry>
{
	[DashboardArea.Overview] = AreaEntry.View<OverviewView>(),
	[DashboardArea.Sales] = AreaEntry.View<SalesView>(),
	[DashboardArea.Orders] = AreaEntry.View<OrdersView>(),
	[DashboardArea.Fulfillment] = AreaEntry.View<FulfillmentView>(),
	[DashboardArea.Products] = AreaEntry.View<ProductsView>(),
	[DashboardArea.Customers] = AreaEntry.View<CustomersView>(),
	[DashboardArea.Team] = AreaEntry.View<TeamView>()
};
```

`AreaEntry.View<T>()` needs `T : DashboardView, new()`. `AreaEntry.BuiltIn(n)` shows a
`PlaceholderView` naming module `n`. Views are created the first time their area is shown and kept
for the session.

`Navigate` in order: creates or fetches the view, hides the others, shows it, moves the sidebar
highlight (`AppearanceKey` `nw-nav-active` / `nw-nav`), sets `headingLabel.Text = view.Title` and
`periodSelector.Visible = view.UsesPeriod`, then calls `view.ShowTarget(target)` when a target was
given.

## DashboardView members

```csharp
public DashboardContext Dashboard { get; set; }      // assigned by MainPage; assigning it calls RefreshView()
public MainPage Shell { get; set; }                  // assigned by MainPage before Dashboard
public virtual string Title => Text;                 // the heading; set Text in the designer
public virtual bool UsesPeriod => true;              // false hides the period selector for this view
public virtual void ShowTarget(object target);       // a navigation target arrived (Customer, Order, string, ...)
protected virtual void RefreshView();                // load everything from Dashboard here, never in the constructor
protected void SetStatus(string text);               // writes the status strip
protected void Navigate(DashboardArea area, object target = null);   // goes through Dashboard.Navigate
protected virtual void OnOrdersChanged();            // default: refresh
protected virtual void OnProductsChanged();          // default: refresh
```

Rules a view follows:

- The constructor only calls `InitializeComponent()` and creates objects the designer cannot. The
  context is null there, and in the designer.
- `RefreshView()` runs when the context is assigned and on every `PeriodChanged` while the view is
  visible. A hidden view is marked stale and refreshed when it is shown again, so `RefreshView()`
  must be safe to call repeatedly (clear before adding).
- A view never reaches into the shell's controls. It reads `Dashboard.Period`, calls
  `Dashboard.SetPeriod(kind)` to change the period (the shell moves the selector to match), and
  calls `Navigate` to leave.
- Status texts name the event path with arrows, as in
  `MapView.MarkerClick → e.Marker.Tag → customer`.

## The status strip: who has the last word

One request can write the status strip several times; the last write is what the user reads.

- A sidebar button, the menu and a palette command write the shell's line first and navigate second
  (`Button.Click → Tag → Navigate(Team)`), so a view that describes itself when it opens has the last
  word. A view that says nothing leaves the shell's line.
- `periodSelector_SelectionChanged` writes its line after `SetPeriod` returns, which is after the visible
  view refreshed.
- A view that wants its own opening line after a period change, or whose line must survive both shells of the course, posts it:
  `Application.Post(() => SetStatus("…"))` runs after the handlers of the request, in the order the
  callbacks were posted. Overview, Sales, Orders, Fulfillment and Products post the line that names their data path.
- A view that sends the user somewhere else writes the event path in its own handler, before or after
  it calls `Navigate` (`KpiPanel.Click → Sales`, `PointClick → e.Point.Tag → Customers`). That line
  stays: while the shell completes a navigation a view asked for, or one that carries a record, `SetStatus` ignores the
  posted opening line of the view that opens (`dashboard_NavigationRequested` posts one callback that
  closes the status strip before the navigation and one that reopens it after).
- `Navigate(area, target)` with a target also puts back the text it found on entry before it calls
  `ShowTarget`, so a view that writes its opening line directly (Customers, in `OnVisibleChanged`)
  does not replace the sender's line either.
- A view that receives a target does not write the status strip in `ShowTarget`.

## The period, in one direction

```
periodSelector.SelectionChanged → dashboard.SetPeriod(kind) → DashboardContext.PeriodChanged → each view's RefreshView()
```

The five segments are in `PeriodKind` order, so `(PeriodKind)periodSelector.SelectedIndex` is the
kind. Anything else that changes the period (a palette command, a view) sets
`periodSelector.SelectedIndex` or calls `Dashboard.SetPeriod`; both end in the same event. The
period label on the Overview is the view's own `periodLabel`, written in `RefreshView()` from
`Dashboard.Period`.

## Navigation targets

A view passes the record itself; the receiving view's `ShowTarget` dispatches on its type.

| Target | Sent by | Received by |
|---|---|---|
| `Customer` | the palette, the Orders grid's avatar, `OrderDialog`, Team's `AvatarGroup` | `CustomersView`: selects the customer |
| `string` (a country name) | Sales, the country chart | `CustomersView`: filters the list, shows the chip |
| `Category`, `Product` | Sales (table row, treemap), Overview and Fulfillment (a purchase in Activity) | `ProductsView`: selects the category chip or the row |
| `DateTime` | Overview, a `HeatmapCalendar` day | `OrdersView`: the day scope, across all periods |
| `Product` | Products (a reorder card, the row's Show orders action) | `OrdersView`: refines the period |
| `Customer`, `Employee` | Customers and Team (the card's tool), Team's `AvatarGroup` overflow | `OrdersView`: refines the period |
| `Employee` | the palette, the user's avatar, `OrderDialog` | `TeamView`: selects the employee |

A navigation without a target (a `KpiPanel` click, the orders card) shows the view as it was left.

## The command palette

`commandPalette_QueryChanged` assigns `e.Results`; `commandPalette_CommandExecuted` switches on
`e.Command.Tag`: a `DashboardArea` navigates, a `PeriodKind` sets the selector, a `Customer` opens
the Customers view with that customer and an `Employee` opens Team with that employee. To make a new record type searchable, add a command list in
`BuildCommands`, include it in `Search`, and add a `case` for its `Tag` type.

## Adding or replacing a view in a later module

1. Add `Views/SalesView.cs` and `Views/SalesView.Designer.cs` (class `SalesView : DashboardView`,
   `AppearanceKey = "nw-view"`, `Text = "Sales"`), or replace both files of an existing view.
2. Change the area's table entry in `MainPage.Areas.cs` to `AreaEntry.View<SalesView>()`.

Theme keys shared by the views are in `Themes/Northwind.mixin.theme` (`nw-card`, `nw-text`,
`nw-strong`, `nw-subtitle`, `nw-caption`, `nw-overline`, `nw-metric`, `nw-button`,
`nw-button-primary`, `nw-icon-button`). The same file sets the application blue (`#1565d8`) on the
new controls through their own color keys (`segment-primary`, `kpipanel-primary`,
`mapview-primary`, `chiplabel-primary`). The project file includes every
`Themes/*.mixin.theme`, and this module has 4 more, each with the color keys of the controls its views use:

- `Northwind.OverviewSales.mixin.theme` (`sparkline-*`, `meterbar-*`, `chartview-*`, `timeline-*`, `heatmapcalendar-*`)
- `Northwind.Fulfillment.mixin.theme` (`radialgauge-*`, `kanban-*`, `scheduler-*`, `ganttview-*`, `actioncard-*`)
- `Northwind.Orders.mixin.theme` (`newcontrols-grid-*`, `querybuilder-*`, `stepper-*`, `emptystate-*`, and the grid chip's soft background)
- `Northwind.CustomersTeam.mixin.theme` (`orgchart-*`, the menu button)

A color key defined in two mixins must have the same value in both: which file is applied last is
not something to rely on.

## Client profiles

`ClientProfiles.json` (copied to the output next to `Default.json`) defines Phone, to 767 pixels,
and Tablet, 768 to 1199. The designer holds each profile's property values in `MainPage.resx`:

| Control | Phone (≤ 767) | Tablet (768 – 1199) |
|---|---|---|
| `sidebar` | `Visible = false` (`Size` 150, 814) | `Visible = true`, `Size` 60, 814 |
| navigation buttons | | `Display = Icon` |
| `menuButton` | `Visible = true` | |
| `brandLabel` | `Location` 52, 0 | |
| `versionLabel` | `Location` 248, 2 · `Size` 90, 44 | |
| `toolsPanel` | `Padding` 0, 0, 10, 0 · `Size` 60, 46 | |
| `asOfLabel`, `userAvatar` | `Visible = false` | |
| `searchButton` | `Display = Icon` · `Size` 36, 28 | |
| `contentPanel` | `Padding` 12, 10, 12, 10 | |
| `headerPanel` | `Size` 1206, 84 | |
| `periodSelector` | `Anchor` Top, Left · `Location` 0, 46 | |

`OnResponsiveProfileChanged` adds what a property value cannot express, and assigns it for every
profile so that each change is also the restore: the navigation tooltips (Tablet only), the
palette's width (`Math.Min(600, Application.Browser.Size.Width - 24)` on Phone), and the status
strip (`Phone profile · sidebar collapsed · cards stacked`,
`Tablet profile · sidebar as icons · cards full width`, `Default profile · designed layout restored`).
`menuButton_Click` shows a `ContextMenu` with the seven areas.

## What a view must do to take part in the responsive layout

Nothing is required. `GetView` asks `CardLayout.For(view)` when it creates a view, keeps the
result, subscribes to the view's `ResponsiveProfileChanged` and applies the active profile. By what
the view contains:

- **A 12-column `TableLayoutPanel` directly in the view** (`layout`; Overview, Sales, Fulfillment,
  Products, Customers, Team): reflowed by `CardLayout`. Tablet: a row of cards that each span 3
  columns or fewer becomes rows of two, every other card spans the width. Phone: one card per row. A
  card keeps the height of the designed rows it spans; a row sized in percent gets `FillRowHeight`
  (620). The table is docked to the top, sized to its rows, and the view scrolls.
- **Controls placed by location** (no `Dock`, `Anchor` = Top, Left): stacked at full width on
  Phone, left as designed on Tablet, where the view scrolls.
- **Docked or edge-anchored controls** (`PlaceholderView`, Orders): left alone; they follow the width.

A view's own responsive properties go in its `.resx`: the grid columns of Customers and Team, and
the Orders toolbar, whose chips dock above the search box on Tablet and Phone.
`OrdersView.OnResponsiveProfileChanged` sets the toolbar row's height, which is not a property of a
control. The inventory card of Products asks for more height through a responsive `MinimumSize`,
which `CardLayout` takes as the row's minimum, and wraps its stock chips in `PlaceStockChips`.

Wisej applies a view's responsive values before it raises the view's `ResponsiveProfileChanged`.
For a view created while a narrow profile is active that is after `GetView` applied the profile
the first time, so `CardLayout.Apply` computes a narrow arrangement again each time it is asked.
