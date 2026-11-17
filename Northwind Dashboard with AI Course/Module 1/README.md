# Northwind Dashboard · Wisej.NET 4.2: Build a Northwind Dashboard with AI · Module 1

Local lab build for **Module 1 · Start a Wisej.NET 4.2 Northwind Application** (lab: *Build the 4.2
Navigation Shell*). The shell of the dashboard with its two new-control interactions: a
`SegmentedButton` that selects the reporting period for the whole session, and a `CommandPalette`
(Ctrl+K) whose commands carry a typed `Tag` - a page, a period or a customer - and are dispatched
by that type.

This folder is the complete application at this point in the course, with its own solution and
port. The finished dashboard that every module works toward is in
[`../Wisej.NorthwindDashboard`](../Wisej.NorthwindDashboard).

## Run it

```bash
cd "Northwind Dashboard with AI Course/Module 1/NorthwindDashboard"
dotnet run -f net10.0 --urls http://localhost:6511
```

Open <http://localhost:6511>. In Visual Studio, open `NorthwindDashboard.slnx` and press F5.

Requirements: the .NET 10 SDK and a Wisej.NET development or trial license. The project restores
the Wisej.NET package from the course's `Wisej.NorthwindDashboard/packages` folder
(`NuGet.Config` maps `Wisej-4` to that folder and everything else to nuget.org) into
`.nuget-packages` in the course folder, a package cache shared by the seven module applications
and ignored by git. The customer map loads its tiles from OpenStreetMap, so it needs an internet
connection.

## The screen

A 46-pixel blue application bar (`Northwind Dashboard`, `Wisej.NET 4.2`, `As of 1998-05-06`, the
search button and the user's `Avatar`), a 150-pixel sidebar with the seven areas, the content
area with the view's name and the period selector (30D, 90D, YTD, 12M, All), and a status strip
that names the event path of the last interaction.

- **Overview** shows three `KpiPanel` cards (Revenue, Orders, Average order) with the change
  against the previous period, and the `Period label` line.
- **Customers** shows the `MapView` with one marker per customer and the card of the selected
  customer.
- **Sales, Orders, Fulfillment, Products** and **Team** open, and say which module builds them.

The walkthrough video is a simulation with rounded figures; this application calculates the real
ones from the Northwind data. Last 30 days: 74 orders, $118,307.15. All time: 830 orders,
$1,265,793.29.

## What to try

| Action | Expected result |
|---|---|
| Start the application | Overview, **30D** selected: Revenue $118,307.15 (▲ 6.4%), Orders 74 (▲ 5.7%), Average order $1,598.75 (▲ 0.6%). Period label `Last 30 days · 1998-04-07 to 1998-05-06`. Status: `SegmentedButton.SelectionChanged → context.SetPeriod(Last30Days)`. |
| Select **All** | Revenue $1,265,793.29, Orders 830, Average order $1,525.05, and no trend: all time has no earlier period to compare with. Period label `All time · 1996-07-04 to 1998-05-06`. Status ends in `SetPeriod(AllTime)`. |
| Press **Ctrl+K** (or the search button) | The palette opens with the seven page commands and the five period commands. Status: `CommandPalette.QueryChanged → e.Results`. |
| Type `around` | Two results: **Open customer Around the Horn** and **Go to Customers**. Mixed case (`ARound`) finds the same; `zzz` finds nothing. |
| Press **Enter** on the customer command | The Customers view opens, the map moves to the United Kingdom, the marker of Around the Horn turns orange and the card shows `Around the Horn`, `London · UK`, `Selected through CommandExecuted`. Status: `CommandExecuted → Tag → Customers`. |
| Click another marker | The card follows, with `Selected through MarkerClick`. |
| Ctrl+K, type `90`, Enter | The period command sets the selector to **90D**; the Overview shows 205 orders and $320,348.28. |
| Click each sidebar entry | All seven open; five show `This view is built in Module N.` |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| One session context owned by the page and passed to cached views | `MainPage()` constructor and `GetView` in [MainPage.cs](NorthwindDashboard/MainPage.cs) |
| `periodSelector` segments in `PeriodKind` order | `periodSelector` in [MainPage.Designer.cs](NorthwindDashboard/MainPage.Designer.cs) |
| One `SelectionChanged` subscription, guarded by `SelectedIndex >= 0`, calling `SetPeriod` | `periodSelector_SelectionChanged` |
| The selected period's name and range shown next to the data | `RefreshView` in [Views/OverviewView.cs](NorthwindDashboard/Views/OverviewView.cs) |
| `commandPalette` with `FilterMode = Server` and a placeholder text | `commandPalette` in the designer |
| `PaletteCommand` objects with `Text`, `Description`, `Category`, `Keywords` and a typed `Tag` | `BuildCommands` |
| Empty query returns page and period commands; a query adds matching customers | `commandPalette_QueryChanged`, `Search` |
| Dispatch by `Tag` type: page, period, customer | `commandPalette_CommandExecuted` |
| A period command updates the selector as well as the view | the `PeriodKind` case sets `periodSelector.SelectedIndex` |
| `userAvatar` in the application bar | `userAvatar` in the designer, `userAvatar_Click` |
| A customer target selected on the map | `ShowTarget` and `Select` in [Views/CustomersView.cs](NorthwindDashboard/Views/CustomersView.cs) |
| How the shell and the views fit together | [docs/ARCHITECTURE.md](NorthwindDashboard/docs/ARCHITECTURE.md) |

## Differences from the video

- **Figures.** The video rounds; the application shows the real 30-day values ($118,307.15,
  +6.4%, +5.7%, +0.6%; average order $1,598.75).
- **Trend.** `KpiPanel` draws the change as an arrow and a percentage (▲ 6.4%) and shows nothing
  when there is no comparison, where the video prints `+6.2%` and a dash.
- **Status text.** The status strip names the `PeriodKind` passed to the context
  (`SetPeriod(Last30Days)`, `SetPeriod(AllTime)`), not the segment caption (`30D`, `All`). The
  sidebar buttons and the marker click write their own event path too.
- **Palette rows.** The video prints each command's `Tag` at the right of its row. The real
  palette shows the command's `Description` under its text (`Thomas Hardy · London, UK`) and
  groups the results under `Customers`, `Pages` and `Reporting period`.
- **Palette targets.** Products, employees and order numbers are not searchable yet: their
  destination views do not exist in this module. They are added with the views.
- **Country.** The card reads `London · UK`: Northwind stores the country as `UK`.
- **Map.** `MapView` draws OpenStreetMap tiles under the shaded countries, and each marker
  carries the customer's name. The selected marker is orange and adds the city
  (`Around the Horn · London`).
- **Application bar.** The search button and the user's avatar sit beside the date; the video's
  bar shows the date only.

## Notes for anyone extending the shell

- A view loads data in `RefreshView()`, never in its constructor: the designer and the constructor
  have no context yet.
- Setting `periodSelector.SelectedIndex` in code raises `SelectionChanged`, so the constructor and
  the period commands use the same route as a click.
- `CommandPalette.FilterMode = Server` raises `QueryChanged` when the palette opens (empty query)
  and on each change; assign `e.Results` every time.
- The new controls take their colors from their own theme keys, not from the theme's `primary`.
  `Themes/Northwind.mixin.theme` sets `segment-primary`, `kpipanel-primary`, `mapview-primary` and
  `chiplabel-primary` to the application blue.
- `MapView.Cluster` is off here so that the selected customer's marker stays visible among the
  six London customers; markers are drawn in collection order, so the selected one is moved last.
