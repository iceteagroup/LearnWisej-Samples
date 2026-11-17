# Northwind Dashboard · how the shell and the views fit together

This is the contract every module of the course builds on. A module adds views and replaces table
entries; it does not change how the shell and a view talk to each other.

## The pieces

| Piece | File | Role |
|---|---|---|
| `SessionStartup.Start` | `SessionStartup.cs` | Runs once per browser session (named in `Default.json`): sets the culture and creates `MainPage`. |
| `MainPage` | `MainPage.cs`, `MainPage.Designer.cs` | The shell. Owns the session's one `DashboardContext`, the navigation, the period selector, the command palette and the status strip. |
| `DashboardContext` | `Data/DashboardContext.cs` | Session state: the selected period, navigation requests, and (from Module 5) the session's order and stock changes. |
| `NorthwindDatabase` | `Data/NorthwindDatabase.cs` | The immutable Northwind snapshot shared by all sessions (`NorthwindDatabase.Instance`). |
| `SalesAnalytics` | `Data/SalesAnalytics.cs` | Every calculation a view shows; takes the context, returns plain objects. |
| `DashboardView` | `Views/DashboardView.cs` | Base class of every view. |
| `DashboardStyle` | `Views/DashboardStyle.cs` | Shared number, date, color and icon-path conventions. |

The `Data` folder is identical to the finished application's except for the namespace
(`NorthwindDashboard.Data`). Do not change it in a module; if a view needs a figure, it is already
in `SalesAnalytics` or `DashboardContext`.

## Controls on MainPage

| Field | Control | Notes |
|---|---|---|
| `appBar` | `Panel` (top, 46 px) | `brandLabel`, `versionLabel`; `toolsPanel` holds `asOfLabel`, `searchButton`, `userAvatar`. |
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
	[DashboardArea.Sales] = AreaEntry.BuiltIn(3),       // PlaceholderView: "This view is built in Module 3."
	[DashboardArea.Orders] = AreaEntry.BuiltIn(4),      // PlaceholderView: "This view is built in Module 4."
	[DashboardArea.Fulfillment] = AreaEntry.View<FulfillmentView>(),
	[DashboardArea.Products] = AreaEntry.BuiltIn(5),    // PlaceholderView: "This view is built in Module 5."
	[DashboardArea.Customers] = AreaEntry.View<CustomersView>(),
	[DashboardArea.Team] = AreaEntry.BuiltIn(6)        // PlaceholderView: "This view is built in Module 6."
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

- `navButton_Click` and `periodSelector_SelectionChanged` write the shell's line after `Navigate` and
  `SetPeriod` return, which is after the view was shown and refreshed.
- A view that wants its own opening line posts it:
  `Application.Post(() => SetStatus("…"))` runs after the handlers of the request, in the order the
  callbacks were posted. Overview and Fulfillment post the line that names their data path.
- A view that sends the user somewhere else writes the event path in its own handler, before or after
  it calls `Navigate` (`KpiPanel.Click → Sales`, `PointClick → e.Point.Tag → Customers`). That line
  stays: while the shell completes a navigation a view asked for, `SetStatus` ignores the
  posted opening line of the view that opens (`dashboard_NavigationRequested` posts one callback that
  closes the status strip before the navigation and one that reopens it after).
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

## The command palette

`commandPalette_QueryChanged` assigns `e.Results`; `commandPalette_CommandExecuted` switches on
`e.Command.Tag`: a `DashboardArea` navigates, a `PeriodKind` sets the selector, a `Customer` opens
the Customers view with that customer. To make a new record type searchable, add a command list in
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
`Themes/*.mixin.theme`, and this module has 2 more, each with the color keys of the controls its views use:

- `Northwind.OverviewSales.mixin.theme` (`sparkline-*`, `meterbar-*`)
- `Northwind.Fulfillment.mixin.theme` (`radialgauge-*`)

A color key defined in two mixins must have the same value in both: which file is applied last is
not something to rely on.
