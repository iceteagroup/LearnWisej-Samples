# Northwind Dashboard · Wisej.NET 4.2: Build a Northwind Dashboard with AI · Module 6

Local lab build for **Module 6 · Explore Customers, Teams and Responsive Controls** (lab: *Build
Responsive Customer and Team Explorers*). **Customers** becomes the full explorer (map regions,
customer card with trend and recent orders, customer grid), **Team** opens with the organization
chart, and the shell follows the browser's width through client profiles: a rail of icons on a
tablet, a menu button and stacked cards on a phone.

This folder is the complete application at this point in the course, with its own solution and
port. The finished dashboard that every module works toward is in
[`../Wisej.NorthwindDashboard`](../Wisej.NorthwindDashboard).

## Run it

```bash
cd "Northwind Dashboard with AI Course/Module 6/NorthwindDashboard"
dotnet run -f net10.0 --urls http://localhost:6516
```

Open <http://localhost:6516>. In Visual Studio, open `NorthwindDashboard.slnx` and press F5.

Requirements: the .NET 10 SDK and a Wisej.NET development or trial license. The project restores
the Wisej.NET package from the course's `Wisej.NorthwindDashboard/packages` folder
(`NuGet.Config` maps `Wisej-4` to that folder and everything else to nuget.org) into
`.nuget-packages` in the course folder, a package cache shared by the seven module applications
and ignored by git. The customer map loads its tiles from OpenStreetMap, so it needs an internet
connection.

## What this module adds

Everything in [Module 5](../Module%205) is still here: Overview, Sales, Orders, Fulfillment and
Products. New in this module:

| Area | Change | Controls |
|---|---|---|
| Customers | Replaced by the full explorer: countries shaded by revenue, the customer card with figures, trend and recent orders, the customer grid | `MapView` (`MapMarker`, regions, `RegionClick`), `Avatar`, `Sparkline`, `Timeline`, `EmptyState`, `ChipLabel`, `DataGridView` avatar, sparkline and chip columns |
| Team | New view: organization chart, profile with a radar against the team average, top customers, leaderboard | `OrgChart`, `Avatar`, `AvatarGroup`, `ChartView` (radar), `DataGridView` avatar, meter, sparkline and chip columns |
| Shell | Client profiles (Default, Tablet, Phone), the menu button, employees in the palette, the user's avatar opens Team | `ClientProfiles.json`, responsive profiles in `MainPage.resx`, `ContextMenu`, `Views/CardLayout.cs` |
| Orders | The toolbar wraps on Tablet and Phone; the view now receives a customer and a sales rep | responsive profiles in `Views/OrdersView.resx` |
| Products | The five stock chips wrap on Tablet and Phone, and the card grows for them | a responsive `MinimumSize` in `Views/ProductsView.resx` |

All seven areas are real views from this module on.

## The screen

**Customers** is the full explorer: the `MapView` card (one marker per customer, the badge is the
period revenue, a gray marker has no orders in the period, a pulsing one has open orders; countries
shaded by revenue quintile), the customer card (`Avatar`, address, three figures, the `Trend`
`Sparkline`, the `Recent orders` `Timeline`), and the customer grid with avatar, sparkline and chip
columns.

**Team** shows the `OrgChart` of the nine employees, the profile card (`Avatar`, four figures,
territories, the radar against the team average, the `Top customers` `AvatarGroup`) and the
leaderboard grid.

The shell follows `ClientProfiles.json`: Default from 1200 pixels, Tablet to 1199 (the sidebar
becomes a rail of icons), Phone to 767 (the sidebar hides behind the menu button, the period
selector moves under the heading, cards stack).

## What to try (figures for **30D**)

| Action | Expected result |
|---|---|
| Open **Customers** | The markers, the legend `Revenue quintile`, `No customer selected` in the card, `Customers · 91 · sales last 30 days` (first rows Save-a-lot Markets $19,339, Ernst Handel $16,649). Status: `Markers from customers · Regions shaded by quintile`. |
| Ctrl+K, `around`, Enter, then zoom in on London (**+** eight times) | The London customers stand on a small circle; each has its own marker. The card shows Around the Horn, `London · Thomas Hardy`, $492, 1 order. Status: `CommandExecuted → Tag → Customers`. |
| Click the marker **B's Beverages**, then **Consolidated Holdings** | The card follows each time (`London · Victoria Ashworth`, then `London · Elizabeth Brown` with `RECENT ORDERS · 3 IN TOTAL` and `TREND · REVENUE PER DAY · LAST 30 DAYS`). The clicked marker is orange. Status: `MapView.MarkerClick → e.Marker.Tag → customer`. |
| Click the pin tool of the map card, then the shape of **Brazil** | The map shows all markers again (`Panel.ToolClick → MapView.FitBounds()`). Then `Customers in Brazil · 9 · sales last 30 days` and the chip `Country: Brazil`. Status: `MapView.RegionClick → e.FeatureId → country`. The tooltip of the shape starts with `Brazil · $7,278 · 5 customers · 6 orders`. |
| Click a row of the list | The customer is selected on the map and in the card. Status: `DataGridView.CellClick → DataBoundItem → customer`. |
| Close the chip | All 91 customers again; the selection stays. Status: `ChipLabel.CloseClick → country filter removed`. |
| Select **All** | `Customers · 91 · sales all time`, `TREND · REVENUE PER MONTH · ALL TIME`; the selected customer stays. |
| Ctrl+K, `paris sp`, Enter | Paris spécialités has never ordered: the card says so, and the trend is flat. |
| Click an entry of **Recent orders** | The order dialog opens. Status: `Timeline.ItemClick → e.Item.Tag → OrderDialog`. |
| Click the tool of the customer card (Around the Horn selected) | Orders opens: `Orders from Around the Horn`, chips `Customer: Around the Horn` and `Period: Last 30 days`, 1 order, $492. Status: `Panel.ToolClick → Navigate(Orders, customer)`. |
| On **Sales**, click the **UK** bar | Customers opens with the chip `Country: UK`. Status: `PointClick → e.Point.Tag → Customers`. |
| Open **Team** | The chart shows Andrew Fuller and his five reports; `+ 3 reports` under Steven Buchanan. The period's top seller is selected (Robert King, $24,856, 21.0%). |
| Click **Nancy Davolio**, **Steven Buchanan**, **Andrew Fuller** | `Ms. Nancy Davolio`, `TOP CUSTOMERS · 10 IN THIS PERIOD` (six avatars and `+4`); `Mr. Steven Buchanan`, `TOP CUSTOMERS · 1 IN THIS PERIOD` (one avatar, no overflow); `Dr. Andrew Fuller`, `TOP CUSTOMERS · 16 IN THIS PERIOD` (six avatars and `+6`). Status: `OrgChart.NodeClick → Tag → employee · AvatarGroup items`. |
| Expand `+ 3 reports`, click **Michael Suyama** | His profile, `TOP CUSTOMERS · 4 IN THIS PERIOD`. |
| With Andrew Fuller selected, click the first avatar, return to Team, click the third | Customers opens with White Clover Markets, then with Königlich Essen: each avatar carries its own customer. Status: `AvatarGroup.ItemClick → e.Item.Tag → Customers`. |
| Click **+6** | Orders opens for the employee: `Sales rep: Andrew Fuller`, 16 orders, $24,018; with period **All**, 96 orders, $166,538. Status: `AvatarGroup.OverflowClick → Navigate(Orders, employee)`. |
| Click **Anne Dodsworth** in the leaderboard while Steven's branch is collapsed | Her profile; the branch opens so her node is visible. Status: `DataGridView.CellClick → DataBoundItem → employee · AvatarGroup items`. |
| Ctrl+K, `leverling`, Enter | Team opens with `Ms. Janet Leverling` ($12.6K, 9 orders). Status: `CommandExecuted → Tag → Team`. |
| Ctrl+K, `davolio`, Enter, then the tool of the profile card | Orders: `Orders booked by Nancy Davolio`, `Sales rep: Nancy Davolio`, 10 orders, $15,543. Status: `Panel.ToolClick → Navigate(Orders, employee)`. |
| Click the user's avatar in the application bar | Team opens with `Dr. Andrew Fuller`. Status: `Avatar.Click → Navigate(Team, employee)`. |
| In an order dialog, click the sales rep's avatar | The dialog closes and Team opens with that employee. Status: `Avatar.Click → Navigate(Team, employee)`. |
| Resize the browser 1440 → 1200 → 1199 → 768 → 767 → 1440 | 1440 and 1200: Default. 1199 and 768: Tablet, a 60-pixel rail of icons, every card full width (the Overview cards two by two), the Customers grid without TREND and LAST ORDER, the Orders chips above the search box, the Products stock chips on two rows at 768. 767: Phone, no sidebar, the menu button, the period selector under the heading, one card per row; the grids keep CUSTOMER and REVENUE (Team: EMPLOYEE, REVENUE, VS. PRIOR). Back at 1440 every control of Overview, Orders, Fulfillment, Customers, Sales, Products and Team has the position and size it had before the trip, and the selections are still shown. The status strip names the profile at each crossing: `Tablet profile · sidebar as icons · cards full width`, `Phone profile · sidebar collapsed · cards stacked`, `Default profile · designed layout restored`. |
| On a phone width press the menu button | A menu with the seven areas; an entry opens the area. Status: `ContextMenu.MenuItemClicked → Tag → Navigate(Customers)`. Ctrl+K: the palette is as wide as the browser less 24 pixels (343 at 375). |
| Start the application in a narrow window, then widen it | The views are created in the narrow arrangement and return to the designed one. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| Map marker and region selection evidence | `CreateMarkers` (`MapMarker` with `Location`, `Text` and the customer in `Tag`; same-city offsets), `BuildRegions` (quintiles, tooltip), `map_MarkerClick` → `Select` → `ShowDetails`, `map_RegionClick` → `ApplyFilter` in [Views/CustomersView.cs](NorthwindDashboard/Views/CustomersView.cs); `map.LegendText` in [Views/CustomersView.Designer.cs](NorthwindDashboard/Views/CustomersView.Designer.cs) |
| OrgChart hierarchy and employee identity test | `CreateNodes` (`OrgNode` `Id`/`ParentId`/`Text`/`SubText`/`Tag`), `orgChart_NodeClick` → `Select` in [Views/TeamView.cs](NorthwindDashboard/Views/TeamView.cs); `orgChart` (`CollapseBelow`) in [Views/TeamView.Designer.cs](NorthwindDashboard/Views/TeamView.Designer.cs) |
| Avatar and AvatarGroup navigation results | `Select` (`Items.Clear`, `AvatarItem`), `customersGroup_ItemClick`, `customersGroup_OverflowClick` in `TeamView`; `userAvatar_Click` in [MainPage.cs](NorthwindDashboard/MainPage.cs); `customerAvatar_Click`, `salesRepAvatar_Click` in [Dialogs/OrderDialog.cs](NorthwindDashboard/Dialogs/OrderDialog.cs) |
| Responsive restoration table and screenshots | [ClientProfiles.json](NorthwindDashboard/ClientProfiles.json), `MainPage.resx`, `OnResponsiveProfileChanged` in `MainPage.cs`, [Views/CardLayout.cs](NorthwindDashboard/Views/CardLayout.cs) (`Reflow` / `Restore`, `Stack`); the table below |
| Grid columns per profile | `Views/CustomersView.resx`, `Views/TeamView.resx` |
| A toolbar that wraps | `Views/OrdersView.resx` (the `Dock` of `statusFilter`, `searchBox`, `advancedButton` per profile), `OnResponsiveProfileChanged` and `PlaceToolbar` in [Views/OrdersView.cs](NorthwindDashboard/Views/OrdersView.cs) |
| A card that asks for more height on a profile | `Views/ProductsView.resx` (`inventoryCard`: `MinimumSize` 0, 170 on Tablet and 0, 204 on Phone), `PlaceStockChips` in [Views/ProductsView.cs](NorthwindDashboard/Views/ProductsView.cs), and the row minimum in `CardLayout.Reflow` |
| Radar interpretation and reviewed patch | `UpdateRadar` in `TeamView`: each axis is normalized to the team's best value, zero-safe |
| Employees in the palette | `BuildCommands`, `Search`, `case Employee` in `commandPalette_CommandExecuted` |
| The shell's contract after this module | [docs/ARCHITECTURE.md](NorthwindDashboard/docs/ARCHITECTURE.md): the profile table, the status rules, what a view needs for the responsive layout |

Restoration table (leaving Phone or Tablet for Default):

| Changed by | Property | Restored by |
|---|---|---|
| `MainPage.resx` profiles | `Visible`, `Size`, `Location`, `Padding`, `Anchor`, `Display` of the shell controls | Wisej, when the profile no longer applies |
| `OnResponsiveProfileChanged` (shell) | navigation `ToolTipText`, `commandPalette.Width` | the same method: both are assigned for every profile |
| `CardLayout.Reflow` | each card's cell, column span, row span and right margin; the table's row styles, `Padding`, `Dock`, `Height`; the view's `AutoScroll` | `CardLayout.Restore`, from the values recorded in the constructor |
| `CardLayout.Stack` | `Bounds` and `Anchor` of each control placed by location; the view's `AutoScroll` | `CardLayout.Stack`, from the recorded values |
| view `.resx` profiles | column `Visible` and `Width`; `Dock`, `Size` and `Display` of the Orders toolbar controls | Wisej |
| `OrdersView.OnResponsiveProfileChanged` | the toolbar row's height | the same method, for every profile |
| Wisej, undocking a right-anchored control | `Location` of the Orders search box and filter button | `OrdersView.PlaceToolbar`, from the toolbar's width |
| `ProductsView.PlaceStockChips` | `Location` of the five stock chips | the same method, on every resize of the card: at the designed width they fit one row |

## Differences from the video

- **Customers has three cards.** The video shows the map and a narrow profile; the application adds
  the customer grid (the lesson's "list") and, on the card, the address, three figures and the
  customer's tool button for the orders.
- **Marker text.** A marker shows the company and its period revenue (`Around the Horn · $492`); the
  video's dark tooltip `Around the Horn · London` is the mock's. The selected marker is orange, as in
  the video.
- **Profile captions.** The video's `Trend · Sparkline` and `Recent orders` are
  `TREND · REVENUE PER DAY · LAST 30 DAYS` and `RECENT ORDERS · 3 IN TOTAL`; order numbers are
  written `#11016`, dates `Apr 10, 1998`.
- **Region tooltip.** `MapView` appends the region's value to the name it is given
  (`Brazil · $7,278 · 5 customers · 6 orders · 5`); the last figure is the quintile.
- **No country without sales.** Every one of the 21 Northwind countries has an order in each of the
  five periods, so no shape is ever without a tone with this data. The branch exists
  (`· no orders in this period`, no value): such a shape keeps the map's neutral fill and still
  filters the list.
- **Org chart.** `OrgChart` draws an avatar with initials in every node; the video's nodes are text
  only. Steven Buchanan's three reports start collapsed (`CollapseBelow = 1`), because all nine nodes
  are wider than the card; the video shows two of them open. The selected employee's node has a blue
  avatar (`OrgNode.Tone = Primary`); the video fills the node blue. The node captions are the job
  titles, without the mock's `Id`/`ParentId` annotations.
- **Team has more than the video's two sections.** The profile also shows four figures, the
  territories and the radar, and the leaderboard is beside it, as in the finished application. The
  group's caption is `TOP CUSTOMERS · 10 IN THIS PERIOD`; the video's `AvatarGroup · 9 customers ·
  Tag = customer` is an annotation.
- **First employee.** The period's top seller is selected when Team opens (Robert King for 30D), not
  Nancy Davolio.
- **Phone.** The video's phone screen is the Overview; the status strip says
  `Phone profile · sidebar collapsed · cards stacked` when the profile is applied (in the video it is
  a caption inside the view). The application bar keeps the search button, as an icon.
- **Tablet.** The video has no tablet screen; the application shows the sidebar as icons there.
- **Views without profile-specific columns.** Sales, Fulfillment and Products reflow their cards
  like the others but keep every grid column: on a tablet or a phone the Orders and Products grids
  scroll sideways, and the board's fourth lane is reached by scrolling the board.
- **Against Module 3's Customers view.** Everything is still there (map, selection by marker, row
  and target, the country chip, the list) except the card's `Selected through …` line; the status
  strip names the path instead.
