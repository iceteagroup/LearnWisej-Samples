# Northwind Dashboard · Wisej.NET 4.2 cookbook

Facts about the new Wisej.NET 4.2 controls that came out of building and running the seven module
applications. Each entry says what was observed and where the course's code uses it (file names are
relative to a module's `NorthwindDashboard` folder; the module in brackets is the first one that
has the code). The controls live in the namespace `Wisej.Web`.

## Theme colors

- **The new controls ignore the theme's `primary` color.** Each control has its own color keys, and
  their primary defaults to indigo `#4f46e5`. The application blue `#1565d8` is set key by key in
  theme mixins. Keys used:

  | Control | Color keys | Mixin |
  |---|---|---|
  | `SegmentedButton` | `segment-primary`, `segment-border`, `segment-text` | `Themes/Northwind.mixin.theme` [1] |
  | `KpiPanel` | `kpipanel-primary`, `-border`, `-text`, `-muted`, `-success`, `-danger` | `Northwind.mixin.theme` [1] |
  | `MapView` | `mapview-primary`, `mapview-background` | `Northwind.mixin.theme` [1] |
  | `ChipLabel`, `ChipGroup` | `chiplabel-primary`, `chiplabel-primary-soft` (and `-neutral`, `-success`, `-warning`, `-danger` with their `-soft` pairs) | `Northwind.mixin.theme` [1] |
  | `Avatar` | `avatar-focus`; the initials use six pairs `avatar-bg0`…`avatar-bg5` / `avatar-fg0`…`avatar-fg5` | `Northwind.mixin.theme` [1] |
  | `Sparkline` | `sparkline-primary` | `Northwind.OverviewSales.mixin.theme` [2] |
  | `MeterBar` | `meterbar-primary`, `-neutral`, `-success`, `-warning`, `-danger`, `-track` | `Northwind.OverviewSales.mixin.theme` [2] |
  | `RadialGauge` | `radialgauge-primary`, `-success`, `-warning`, `-danger`, `-neutral`, `-track`, `-value`, `-label` | `Northwind.Fulfillment.mixin.theme` [2] |
  | `ChartView` | `chartview-primary`, `-success`, `-danger`, `-neutral`, `-grid`, `-label`, `-text` | `Northwind.OverviewSales.mixin.theme` [3] |
  | Grid visual cells | `newcontrols-grid-primary`, `-success`, `-danger`, `-neutral`, `-border`, `-track`, `-text` | `Northwind.Orders.mixin.theme` [4] |
  | `QueryBuilder` | `querybuilder-primary`, `-soft`, `-border`, `-muted`, `-text` | `Northwind.Orders.mixin.theme` [4] |
  | `Stepper` | `stepper-primary`, `-success`, `-error`, `-muted`, `-track`, `-text` | `Northwind.Orders.mixin.theme` [4] |
  | `EmptyState` | `emptystate-border`, `-muted`, `-art` | `Northwind.Orders.mixin.theme` [4] |
  | `KanbanBoard` | `kanban-primary`, `-hover`, `-drop`, `-text`, `-muted`, `-border`, `-success`, `-warning`, `-danger` | `Northwind.Fulfillment.mixin.theme` [5] |
  | `SchedulerView` | `scheduler-primary`, `-soft`, `-text`, `-muted`, `-border`, `-success`, `-warning`, `-danger` | `Northwind.Fulfillment.mixin.theme` [5] |
  | `GanttView` | `ganttview-border`, `-grid`, `-muted` | `Northwind.Fulfillment.mixin.theme` [5] |
  | `Timeline` | `timeline-primary`, `-success`, `-danger`, `-neutral`, `-connector`, `-time`, `-detail` | `Northwind.OverviewSales.mixin.theme`, `Northwind.Fulfillment.mixin.theme` [5] |
  | `HeatmapCalendar` | `heatmapcalendar-primary`, `-selection`, `-empty`, `-label` | `Northwind.OverviewSales.mixin.theme` [5] |
  | `ActionCard` | `actioncard-primary`, `-primary-soft`, `-border`, `-text`, `-muted`, `-success`, `-warning`, `-danger` | `Northwind.Fulfillment.mixin.theme` [5] |
  | `OrgChart` | `orgchart-primary`, `-border`, `-connector`, `-text`, `-muted`, `-drop` | `Northwind.CustomersTeam.mixin.theme` [6] |

- **The `KpiPanel` sparkline is a `Sparkline`.** Its color is `sparkline-primary`; `kpipanel-primary`
  does not reach it. `Views/OverviewView` [2].
- **Some colors are not keys.** The Gantt bar colors are the `barColors` property of the `ganttview`
  appearance (neutral, primary, success, warning, danger); the org chart's avatar background and
  the soft background of a primary chip in a grid cell (`#eeedff`) are literals inside an
  appearance. Override the appearance in the mixin.
- **`"inherit"` works for a top-level appearance, not for a component of one.** `"ganttview":
  { "inherit": "ganttview", … }` and `"orgchart-avatar": { "inherit": "orgchart-avatar", … }` keep
  everything they do not mention. `"newcontrols-grid/chip": { "inherit": "newcontrols-grid/chip", … }`
  with only the `soft-primary` state replaced the whole component: the chips lost their padding,
  radius and every other tone. The Orders mixin therefore carries the complete `chip` component
  with one value changed. `Themes/Northwind.Orders.mixin.theme` [4].
- **A key defined in two mixins must have one value.** Two mixins defined `timeline-connector` and
  `timeline-detail` differently; which one wins depends on the order the files are applied. The
  values are the same now [5].
- **The built-in mixins can be read.** They are embedded resources named
  `Wisej.Themes.<Control>.mixin.theme` in `Wisej.Framework.dll`.

## The status strip: ordering

- **A handler that calls into the context and then writes a status overwrites whatever a view wrote
  during that call.** The shell's `navButton_Click` and `periodSelector_SelectionChanged` did that
  to every view's opening line.
- **`Application.Post(() => …)` runs after the event handlers of the request, before the response is
  sent, in the order the callbacks were posted.** It also works while the page is being constructed.
  A view posts the line that names its data path (`Views/OverviewView` [2], `SalesView` [3],
  `OrdersView` [4], `FulfillmentView` [2], `ProductsView` [5]).
- **Because posted callbacks run in order, two of them can bracket a third.** When a view asks for a
  navigation, the shell posts one callback before the navigation that closes the status strip and
  one after it that reopens it; the opening line the target view posts in between is ignored, and
  the sender's event path (`KpiPanel.Click → Sales`) stays, whether the sender wrote it before or
  after calling `Navigate`. `dashboard_NavigationRequested` and `SetStatus` in `MainPage.cs` [1].
- **A context event that refreshes a view also posts its load line.** `PlacePurchaseOrder` raises
  `ProductsChanged`, which refreshes Products; a `purchasing` flag keeps that refresh from posting
  the load line over the purchase line. `Views/ProductsView` [5].

## SegmentedButton

- **Setting `SelectedIndex` in code raises `SelectionChanged`.** The shell uses it: the constructor
  and the period commands of the palette take the same route as a click. `MainPage.cs` [1]. Where it
  is not wanted, guard: `Quietly` in `Views/OrdersView` [4].
- **A right-anchored `SegmentedButton` needs a width near its natural one,** or the segments are
  clipped. `periodSelector` [1].
- **The segments are in `PeriodKind` order,** so the selected index is the period:
  `(PeriodKind)periodSelector.SelectedIndex` [1].

## CommandPalette

- **`FilterMode = Server` raises `QueryChanged` when the palette opens (empty query) and on every
  change;** assign `e.Results` each time. `commandPalette_QueryChanged` [1].
- **The `Tag` of a `PaletteCommand` is the record.** `CommandExecuted` switches on its type
  (`DashboardArea`, `PeriodKind`, `Customer`, `Employee` [6], `Product` and `Order` [7]); no caption
  is parsed.
- **`Enter` runs the first result.** The order of `e.Results` is the order shown, grouped under the
  commands' `Category`; put the results the user is most likely to want first.
- **`Width` is a property:** the Phone profile sets it to the browser's width less 24 pixels [6].

## KpiPanel

- **`Trend = null` hides the indicator entirely** (no dash). `TrendFormat` is a three-section numeric
  format (`0.0' pts';0.0' pts';0' pts'`): the arrow carries the sign. `UpdateKpis` [2].
- **`Visual = Gauge` with a `Caption` needs about 180 pixels of height;** at 156 the gauge is
  clipped [2].
- **`Clickable = true` raises `Click`** for the whole card; the Overview cards navigate [2].

## MeterBar

- **No segment legend.** Only `Label` (left) and `ValueText` (right) are shown; put the segment
  counts in `ValueText` (`9 new · 7 picking · 5 packed · 21 open`). A long `Label` is ellipsized
  against `ValueText` in a narrow card. `UpdateStages` [2].
- **`Segments` must be cleared before it is refilled,** or a refresh appends duplicates [2].

## RadialGauge

- **There is no "no value" state.** A `ValueFormat` without a placeholder (`"—"`) prints the dash
  whatever `Value` is, and `Value = Minimum` draws only the track, so the threshold tone never
  shows. `UpdateShippers` in `Views/FulfillmentView` [2].
- **`MeterThreshold.From` is on the gauge's scale** (0 to 100 here: pass 95.1, not 0.951) [2].

## ChartView

- **`LegendItemClick`: the control has already toggled `e.Series.Visible`** when the event is
  raised; the handler only records it. `YAxis.Max` is not recomputed on a stacked chart when you set
  it yourself: sum the visible series per bucket. `RescaleCategoryTrend` in `Views/SalesView` [3].
- **A chart whose series are all hidden shows its `EmptyState` text;** change the text for that
  case, or it reads as "no data" [3].
- **Bound points carry their row.** With `DataSource` and `CategoryMember`/`ValueMember`,
  `e.Point.Tag` in `PointClick` is the bound object; with explicit `ChartPoint`s you set `Tag`
  yourself. Binding to a private nested row class works. `countriesChart_PointClick` [3].
- **Waterfall.** `IsTotal` points are drawn in the neutral tone (`chartview-neutral`), deltas in
  `chartview-success` / `chartview-danger`; an empty `Points` collection shows `EmptyState`. Set
  `YAxis.Min = 0` and a `Max` from the running peak. `UpdateBridge` [3].
- **Treemap.** `ColorMember` names a numeric member: positive values are drawn in `UpTone`, negative
  ones in `DownTone` [3].
- **A categorical palette is the chart's `Palette` property,** not a theme key:
  `DashboardStyle.CategoryPalette` [3].
- **In a browser test,** each chart is an `svg[role=img]` inside the control's element, and the SVG
  is re-created on every update: locate a chart through its control's `name` attribute each time,
  never through a cached handle.

## DataGridView and the new column types

- **`DataGridViewActionsColumn.IconOnly` defaults to `true`.** An action with only a `Text` then
  renders as a 28-pixel button with the caption clipped. Set `IconOnly = false` for text actions.
  `colActions` in `Views/OrdersView.Designer.cs` [4].
- **`DataGridViewSparklineColumn.ValueFormat` is a composite format string** (default `{0:N0}`):
  `"C0"` prints the two characters `C0`; write `"{0:C0}"`. With `ShowValue = true` the cell prints
  the last point of the sequence [4].
- **`DataGridViewVisualCellEventArgs.DataBoundItem`** is the row's object in `ActionClick` and
  `AvatarClick`, and `e.Action.Name` says which `CardAction` was clicked, so the handler is right
  after sorting and filtering. For the ordinary grid events use `Rows[e.RowIndex].DataBoundItem`.
  `colActions_ActionClick` [4].
- **`MeterThreshold.From` in a meter column is on the scale of the bound value** and the column's
  `Maximum` (4 here, not 100) [4].
- **A bound grid sorts a `List<T>` by header click** without a sortable binding list; assigning a new
  `DataSource` clears the sort and its glyph [4].
- **The first row of a freshly bound grid looks selected** even after `CurrentCell = null`. Add
  `ClearSelection()`; in a dialog do both in `OnLoad`, because the selection is set when the form is
  shown. `Dialogs/OrderDialog` [4]. On the Sales table it can still show at first display and
  clears on the next refresh [3].
- **A grid keeps its scroll offset when a shorter list is bound.** After filtering 91 rows to 9 with
  a row far down selected, the grid showed rows 4 to 9. Call `ScrollRowIntoView(0)` (or the selected
  row) after assigning `DataSource`. `ApplyFilter` in `Views/CustomersView` [3].
- **The DOM order of grid rows is not the visual order;** a browser test finds a row's button by
  position.

## ChipGroup and ChipLabel

- **Counts on chips:** `ChipGroup.ShowCounts = true` and `Segment.Count` (an `int?`); the chip
  renders `Text (Count)`, and `SelectedItem.Value` identifies the choice without parsing a caption.
  `statusFilter` [4].
- **`ChipLabel` fills its bounds and has no working `AutoSize`** (set to true, the chip collapses to
  a dot). The pill is as wide as the control, with the caption centered, so size the control for
  its caption: `ChipWidth` in `Views/OrdersView` [4].
- **`Closable = true` shows the ×** and raises `CloseClick`; the handler removes the filter the chip
  stands for. `countryChip_CloseClick` in `Views/CustomersView` [3].
- **The first focusable control of a dialog gets the focus ring;** for a status `ChipLabel` that
  looks like a selected chip. `statusChip.Focusable = false` moves the focus on [4].

## QueryBuilder

- **`Fields.Add` raises `QueryChanged`.** A handler that filters or writes a status needs a guard
  while the code changes the builder (`Quietly`), also around `Root.Rules.Clear()` and
  `Root.Groups.Clear()` [4].
- **`QueryRule.Value` is always text,** and the control evaluates nothing. A number field delivers
  what was typed (`09000` when the digits follow the editor's initial 0): parse numbers with the
  invariant culture and dates as `yyyy-MM-dd`. The two `Evaluate` overloads [4].
- **An incomplete rule** (no value yet) should be ignored by the filter, not treated as "matches
  nothing" [4].

## EmptyState

- **`TargetControl` is enough:** the placeholder covers the grid when it has no rows and goes away
  when rows return; no `Visible` code. Create it in the constructor, after `InitializeComponent`.
  `ActionText` adds the button, `ActionClick` is its event. `Views/OrdersView` [4].

## Stepper

- **Each `Step` has a state** (completed, active, pending, error) and a description line; the dialog
  sets all four from the order in one method. A late shipment marks two steps as errors.
  `UpdateStatus` in `Dialogs/OrderDialog` [4].

## KanbanBoard

- **`ColumnWidth` is the minimum lane width** (default 260) and `AutoSizeColumns` (default true)
  only lets lanes grow. Four lanes in an 870-pixel card need `ColumnWidth = 190`, or the last lane
  is cut off behind a horizontal scroll. `kanbanBoard` [5].
- **Keyboard moves.** Alt+Left / Alt+Right move the focused card to the neighboring lane and
  Alt+Up / Alt+Down reorder it; a lane change raises the same `CardMoving` / `CardMoved` as a drag,
  so validation covers both. A reorder inside a lane must leave the data alone: the handler returns
  when `OldColumn == Column`. `kanbanBoard_CardMoved` [5].
- **`CardMoving` with `e.Cancel = true` refuses a move;** `WipLimitExceeded` is raised when a lane's
  `WipLimit` would be passed, and the card stays where it was [5].
- **`MaxVisibleCards`** hides the rest of a lane behind `+ n more`; a hidden card has no element to
  drag [5].
- **A view that changes the context from `CardMoved` must not rebuild the board from the context's
  event** it raised itself: the `moving` flag in `OnOrdersChanged` [5].
- **Drag in a browser test** works with real mouse events: press on the card, move in steps to the
  target lane, pause, release. Select cards by `[role=button][aria-label^="#11068 "]`: the element's
  CSS class changes while hovered.

## SchedulerView

- **Resources appear as rows only in `SchedulerViewMode.Timeline`.** Week and Month show the resource
  name inside each appointment instead. Timeline always adds an `Unassigned` row. `scheduler` [5].
- **An appointment's `ResourceId` is the resource's `Id`,** and its `Tag` carries the order:
  `AppointmentClick` → `e.Appointment.Tag` [5].
- **Clear `Resources` and `Appointments` before a rebuild;** the summary line counts them to prove
  it [5].

## GanttView

- **`GanttTask.ParentId` of a top-level task is an empty string, not null:** test it with
  `string.IsNullOrEmpty` [5].
- **`ScrollTo` scrolls the whole pane, task list included:** called while the control is visible it
  pushes the task names out of view. The chart already starts at the week of the first task, so the
  view does not call it [5].

## Timeline

- **`Grouping = ByDay` with `TimePosition = Hidden`** gives day headings and clean rows.
  `activityTimeline` in `Views/OverviewView` [5].
- **`HasMoreItems = true` shows the `LoadMoreText` button after the last item;** `LoadMore` must
  append only (`Skip(loaded).Take(page)`), or items repeat. `AppendOrders` [5].
- **`ItemClick` gives `e.Item`; its `Tag` is the record.** One timeline can hold orders and
  products: the handler dispatches on the type [5].

## HeatmapCalendar

- **Call `SetDateRange` before assigning `DataSource`.** 53 weeks at `CellSize = 13` need about
  860 x 175 pixels, and the control scrolls when its area is shorter. `UpdateHeatmap` [5].
- **`CellClick` is raised for empty days too** (`e.Value` 0); `e.Date` is the day. `ToolTipFormat`
  takes `{0}` date and `{1}` value [5].

## ActionCardList

- **Assigning a new `DataSource` recreates the cards and raises `CardCreated` again,** so per-card
  handlers subscribed there are never duplicated. The event data is `e.Card` and `e.DataItem`.
  `reorderList_CardCreated` in `Views/ProductsView` [5].
- **`ActionClick` (a `CardAction`) and `Click` (the card) are separate events;** an action click does
  not also raise the card's click [5].

## MapView

- **`Cluster` defaults to `true`.** It is off in the course's views so that every customer keeps a
  marker; markers are drawn in collection order, so the selected one is moved last. [1].
- **Markers at the same coordinates cover each other.** The six London customers are placed on a
  small circle around the city. `CreateMarkers` in `Views/CustomersView` [6].
- **Regions are GeoJSON features;** `RegionClick` gives `e.FeatureId`. The control appends the
  region's value to the tooltip text it is given. `BuildRegions` [6].
- **The mouse wheel over a `MapView` zooms the map,** it does not scroll the view; scroll with the
  pointer over a card.
- **`FitBounds()`** over all Northwind markers ends further out than the designed view (`Center`
  38, -35 and `Zoom` 2) in the 740-pixel map card [6].

## OrgChart, Avatar and AvatarGroup

- **`OrgChart` has no selected-node property.** The node the user clicks gets the theme's `selected`
  border on the client; a selection made in code (leaderboard, palette) can only be shown with
  `OrgNode.Tone`, which colors the node's avatar. `SetExpanded(node, true)` opens a collapsed
  branch. `Select` in `Views/TeamView` [6].
- **`OrgChart` scrolls sideways when the tree is wider than the control:** nine nodes with both
  levels open need about 1,450 pixels; `CollapseBelow = 1` starts with the second level closed [6].
- **`AvatarGroup` draws `MaxVisible` avatars and one `+N` element;** `ItemClick` gives the
  `AvatarItem` and its index, `OverflowClick` gives nothing, so the overflow needs a destination of
  its own. `customersGroup_ItemClick`, `customersGroup_OverflowClick` [6].

## Responsive profiles

- **Give every profile of a control the same set of properties.** `sidebar` had `Visible` in the
  Phone profile and `Visible` + `Size` in the Tablet profile. A session that started on a phone
  width, went to a tablet width and then to Default kept the 60-pixel sidebar: Wisej restored only
  the properties of the first profile it had applied. With `Size` in both profiles every route
  restores. `MainPage.resx` [6].
- **Check every route, not one round trip.** 1440 → … → 767 → 1440 passed with the bug above; only a
  session started narrow showed it.
- **`ResponsiveProfile` entries in a `.resx`** are
  `application/x-microsoft.net.object.bytearray.base64` of plain XML:
  `<ResponsiveProfile Name="Phone"><Visible Type="System.Boolean">False</Visible><Size
  Type="Size">150, 814</Size></ResponsiveProfile>`. Types seen: `System.Boolean`, `System.Int32`,
  `Size`, `Point`, `Wisej.Web.Padding, Wisej.Framework`, `Wisej.Web.DockStyle, Wisej.Framework`,
  `Wisej.Web.Display, Wisej.Framework`, `Wisej.Web.AnchorStyles, Wisej.Framework`. The designer line
  is `control.ResponsiveProfiles.Add((Wisej.Base.ResponsiveProfile)resources.GetObject(
  "control.ResponsiveProfiles"))`, the second profile `…ResponsiveProfiles1`.
- **`OnResponsiveProfileChanged` is raised for a view created while a narrow profile is active,**
  after Wisej applied the profile's property values, and `ResponsiveProfileChanged` can be handled
  from outside (`view.ResponsiveProfileChanged += …`). The shell reflows the views' cards that way.
  `GetView` in `MainPage.cs` [6].
- **What a property cannot express goes in `OnResponsiveProfileChanged`, assigned for every
  profile,** so each change is also the restore: a row height of a `TableLayoutPanel`
  (`Views/OrdersView` [6]), tooltips, the palette's width (`MainPage.cs` [6]).
- **A control anchored to the right that a profile docks does not come back to its place.** When the
  profile no longer applies, Wisej restores `Dock` and `Size`; the `Location` is the designed one,
  kept at the distance from the right edge that the container had at that moment, and a hidden view
  has not been laid out for the new browser width yet. The Orders search box and filter button
  ended 38 pixels, or several hundred, from their place. Fix: while they are undocked, keep them
  against the container's edge from its `Resize` event. `PlaceToolbar` in `Views/OrdersView` [6].
- **A card that spans two rows needs both heights when the table is reflowed to one card per row.**
  The Overview's chart and activity cards span two designed rows and collapsed to the height of the
  first. `Reflow` in `Views/CardLayout.cs` [6].
- **`MinimumSize` is a responsive property, and a layout helper can read it.** The Products
  inventory card asks for 170 pixels on Tablet and 204 on Phone, where its five stock chips wrap to
  two or three rows; `CardLayout.Reflow` gives the row at least that height. For a view created
  while a narrow profile is active, the profile's values arrive after the view was added to its
  parent: a layout computed at that moment still sees the designed `MinimumSize`, and the next card
  overlapped the grown one. The narrow arrangement is therefore computed again when the view's
  `ResponsiveProfileChanged` is raised. `Views/ProductsView.resx`, `PlaceStockChips` in
  `Views/ProductsView`, `Apply` in `Views/CardLayout.cs` [6].
- **`Button.Display = Label` with an `ImageSource`** keeps the icon out of the Default layout and
  lets a profile switch to `Display = Icon` with no code. The navigation buttons [6].
- **`ClientProfiles.json` must be copied to the output** next to `Default.json`
  (`<Content Update="ClientProfiles.json" CopyToOutputDirectory="PreserveNewest" … />`); without it
  the application only ever runs in the Default profile [6].

## General

- **Docking order is the reverse of `Controls.Add`:** the control added last is docked first.
- **A view loads data in `RefreshView()`, never in its constructor:** the designer and the
  constructor have no context yet. Collections are cleared before they are filled, because a view
  is refreshed on every period change and whenever the session's data changes.
- **`TextBox.Text` set from code raises `TextChanged`,** like the selection properties above.
- **Every control's `Name` is the `name` attribute of its element,** tools included
  (`[name="fit"]`), which makes browser checks independent of captions. Two views may use the same
  control name (`activityTimeline` on Overview and on Fulfillment): scope the selector with the
  view's name (`[name=FulfillmentView] [name=activityTimeline]`).
- **A double hyphen inside an XML comment breaks a project file.**
- **The package's XML documentation does not cover the new controls.** Take signatures from the
  finished application's source, or reflect over a built `Wisej.Framework.dll` (load it from a
  built `bin` folder with an `AssemblyResolve` handler).
