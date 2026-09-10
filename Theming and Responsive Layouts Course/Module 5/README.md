# AdaptiveOps · Theming & Responsive Layouts Course · Module 5

Local lab build for **Module 5 · FlowLayoutPanel, TableLayoutPanel, FlexLayoutPanel, and Extended Layout
Properties**. It follows the walkthrough video ("The same region in Flow, Table and Flex"): the Adaptive
Operations Console keeps its five docked shell regions, and the content *inside* them is rebuilt with the
three layout engines — the toolbar becomes a `FilterBar : FlowLayoutPanel` whose search box carries
`FillWeight 1` and whose Apply button carries a `FlowBreak`, the ticket editor becomes a `TicketEditor`
UserControl around a two-column `TableLayoutPanel` (`ColumnStyle(Absolute, 140)` for captions,
`ColumnStyle(Percent, 100)` for editors, the Notes box spanning both columns), and the list / details
split becomes a `DashboardWorkspace : FlexLayoutPanel` weighted 2 : 1 with `MinimumSize` on both regions,
`MaximumSize` and `AlignY Top` on the details — that last region written entirely in code as one
`Wisej.Web.Markup` fluent chain, so both notations are on screen. A "Same region, three ways" tab strip
puts the four metric cards in a Flow, a Table and a Flex host in turn, and the "Layout & theme · live
trace" card logs what each engine did with them at the current width, so the resize comparison the lab
asks for can be read, not guessed.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 5/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5505
```

Then open <http://localhost:5505>. (Visual Studio: open `AdaptiveOps.slnx`, press F5 — `launchSettings.json`
uses the same port.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

The trace card at the bottom is the lab's evidence: every line starts with a timestamp and `→ render`
(server to client), `← client` (client to server) or `• server` (a server-side decision). On first load
you should see, in this order: `• server shell built: toolbar Dock=Top … · navigation Dock=Left 220×… ·
trace Dock=Bottom … · status Dock=Bottom … · workspace Dock=Fill … min 320×240`, `• server filter bar:
FlowLayoutPanel LeftToRight · WrapContents True · … · FillWeight(txtSearch)=1 min 180 → … px ·
FlowBreak(btnApply)=True · … · 11 children in 2 row(s) …`, `• server ticket editor: TableLayoutPanel 2 cols
[Absolute 140 | Percent 100] · 10 rows · notes span 2 · col widths 140,… · row heights …`, `• server
dashboard: FlexLayoutPanel Horizontal · Spacing 8 · list FillWeight 2 min 300×200 → … · details FillWeight 1
min 280×200 max 520 AlignY Top → …`, `• server same region, three ways: Flow host: … | Table host: … | Flex
host: …`, `• server dock order verified: Top, Bottom, Bottom, Left, Fill …`, `• server acceptance check:
flow ok (…) · table ok (…) · flex ok (…)`, `→ render page load: 12 of 12 tickets (no filter) · open 8 · …`
and `← client page load: ticket T-1031 "Label printer driver" loaded into the TableLayoutPanel editor`. The
status bar reads `● ready`, `Browser W × H px · Desktop` and `engine: Flow · 4 cards · 1 row(s)`.

| Action | Path | What you should see |
|---|---|---|
| **Switch engine** (toolbar, row 2) | success | the cards move Flow → Table → Flex → Flow. Each click: `• server engine → Table (Switch engine): Table host: TableLayoutPanel 4 cols × 1 row(s), ColumnStyles Percent 25 ×4, RowStyles Absolute 84, GrowStyle AddRows; cards Dock=Fill in their cells → … · cardOpen@(0,0), cardOverdue@(1,0) …`, the tab strip follows, and ~300 ms later `← client card bounds (after switch to Table) · Table: 4 cards in 1 row(s) · host …×… · col widths …,…,…,… · row heights 84 · open 277×76 @(8,8) · …` — the sizes the engine produced. Flex: equal widths (`FillWeight 1 each`); Flow: `192×76` each. Clicking a tab does the same switch (`tab click`) |
| **Add cards** (toolbar) | progress | `• server add cards started: one card every 700 ms into the Flow host (Wisej.Web.Timer, no client code)`, then per tick `→ render card 5: cardExtra1 "Critical" added to the Flow host — 200 px each in … px: wraps when the row is full` and `← client card bounds (after card 5) · Flow: 5 cards in 2 row(s) …`; the progress label counts `Adding cards · n of 6 · …`, status `● adding cards`. Run it on the **Table** tab and each tick reads `… Controls.Add(card) — the table placed it at (0,1) (GrowStyle AddRows, RowCount now 2)`; on the **Flex** tab `… 5 equal shares of … px, none below 140` — the weights redistribute and the cards shrink to their `MinimumSize` |
| **Cell collision** (toolbar) | failure | the Table tab is selected; `• server cell collision: cell (0,0) holds cardOpen; calling tableHost.Controls.Add(cardCollision, 0, 0) …`, then either `… no exception — Add accepted the cell. cardCollision reports (0,0), cardOpen reports (…), GetControlFromPosition(0,0) = …` (look at the tab: the engine overlaps the two or pushes one to the next free cell) or `… Controls.Add threw …: …` if the framework refuses; red banner, status `● layout fault: cell collision` |
| **No-wrap overflow** (toolbar) | failure | the Flow tab is selected; `• server no-wrap: flowHost.WrapContents=false, AutoScroll=false → one row of 4 × 200 px = 800 px in a …-px host: anything past the edge is clipped · filterBar.WrapContents=false (AutoScroll stays true) → the FlowBreak is ignored, both toolbar rows become one and scroll horizontally`, then `← client card bounds (after WrapContents=false) · Flow: … · n card(s) past the right edge (CLIPPED) · toolbar overflows by … px (scrolled) …` (narrow the browser first to see the cards clipped); red banner, status `● layout fault: no wrap` |
| **Restore** (toolbar) | recovery | extra cards and the intruder are removed, `WrapContents` is back on for both flow panels, the filters are cleared: `• server engine → Flow (Restore): …`, `→ render Restore: 12 of 12 tickets (no filter) …`, `• server restored the default arrangement: Flow engine, 4 cards, WrapContents on, filters cleared, banner hidden`, status `● ready`, `engine: Flow · 4 cards · 1 row(s)` |
| Type `printer` and press **Apply** (or Enter) | command | `→ render Apply: 3 of 12 tickets (contains "printer", status any) · open 8 · …`; status `● filters applied · 3 of 12 tickets`; the metric cards keep counting all tickets. Pick a status in the drop-down to combine |
| Type `/[/` and press **Apply** | failure | the regular expression does not parse: `• server ApplyFilters rejected: Invalid pattern '[' at offset 1. … — grid left as it was (…)`, banner `✖ ApplyFilters failed: … The grid keeps its last good filter.`, status `● filter error: invalid /regex/` — nothing was half-updated |
| Resize the browser 1400 → 1024 → 700 px | the comparison | per resize: `← client resize (Page.Resize): browser 1024×… · … · filter bar 2 row(s) · card host … px wide · list 500 / details 280 (details at MinimumSize)`, then `← client card bounds (card Resize) · Flow: 4 cards in 2 row(s) …` and `← client dashboard (region Resize) · … details … → 280×… (at MinimumSize) …`. Watch the Flow tab wrap (4 → 3 + 1 → 2 + 2), the search box stretch and shrink to 180, the editor captions stay at 140 while the editors narrow, and the details region stop at 280 instead of vanishing. Switch to the Table and Flex tabs at 700 px to see the two "wrong containers" for a card strip: the Table's 25 % cells are narrower than the cards' `MinimumSize`, the Flex row overflows instead of wrapping. `docs/LayoutComparison.md` § 2–5 has the expected numbers |
| Click **Dashboard / Tickets / Reports / …** in the rail | navigation | the list title changes; `← client navigation: Reports (rail = FlowLayoutPanel TopDown, WrapContents False; the 8-px gaps are each button's Margin)` |
| Select a row, edit, **Save** / clear the Owner and **Save** | success / failure | as in Module 1: `• server saved T-1042 …` and an `AlertBox`, or `• server rejected T-1042 (Save): Owner is required.` with the red banner — validation is still only in `TicketRepository.Save` |
| **Clear trace** | – | empties the trace list |

### Where the lab text and this build differ

- **The details region moved into the workspace.** The lab's `DashboardWorkspace` splits the list and the
  details 2 : 1, so the details editor is now a child of the flex container, not the Module 4 `Dock=Right`
  panel. The console still has **five docked regions**: the "Layout & theme · live trace" card takes the
  fifth slot as a `Dock=Bottom` region (150 px) above the status bar; the dock order is verified at
  startup (`Top, Bottom, Bottom, Left, Fill`).
- **The toolbar is 100 px tall, not 56.** The `FlowBreak` on Apply puts the lab buttons on a second row,
  which is the point of the exercise. The region height is fixed (Module 6 makes it per-profile), so at
  700 px, where row 1 itself wraps, the bar scrolls vertically rather than growing.
- **Names.** The pack's `TableLayoutColumnStyle` does not exist in Wisej.NET; the build uses
  `Wisej.Web.ColumnStyle` / `RowStyle`. The pack's `PlaceholderText` is `TextBox.Watermark`. The lab's
  `cmbStatus` is `cboStatusFilter`, `pnlList` / `pnlDetails` are `ListRegion` / `DetailsRegion`.
- **Folders.** `FilterBar` and `DashboardWorkspace` are in `Layout/` as the lab says; `TicketEditor` and
  `MetricCard` are UserControls and live in `Shell/` with the course convention (`X.cs` + `X.Designer.cs`).
- **Two fluent calls fell back to properties.** `AutoSize` is ambiguous between `ControlExtensions` and
  `LabelExtensions` (CS0121) and `BorderStyle(...)` exists only for buttons, so those two are object
  initialisers in `DashboardWorkspace.cs`; every layout property is in the chain. See `docs/FluentMarkupRegion.md`.
- **"Same region, three ways" is an addition.** The lab compares regions across resizes; the tab strip
  additionally shows the *same* cards under all three engines so the decision rule can be watched live.
  The Flow tab is the winner and the default.

## Where things live

```
Module 5/
├─ AdaptiveOps.slnx
├─ README.md                          this file
└─ AdaptiveOps/
   ├─ MainPage.Designer.cs            InitializeComponent(): the five docked regions, the TopDown rail, the
   │                                  "Same region, three ways" TabControl with the Flow / Table / Flex hosts and
   │                                  the four MetricCards, banner, trace, status bar, the two Timers
   ├─ MainPage.cs                     code-behind: SwitchEngine(), the Add-cards timer, CellCollision(),
   │                                  NoWrapOverflow(), Restore(), ApplyFilters(), the debounced bounds trace,
   │                                  VerifyShell()/VerifyContainers(), AddTrace(); no layout arithmetic
   ├─ Layout/FilterBar.cs             FlowLayoutPanel toolbar: Command event, Describe(), CountRows(), Overflow()
   ├─ Layout/FilterBar.Designer.cs    the 11 children, SetFillWeight(txtSearch,1), SetFlowBreak(btnApply,true),
   │                                  SetFillWeight(lblProgress,1), Margins — no Location / Dock / Anchor
   ├─ Layout/DashboardWorkspace.cs    FlexLayoutPanel list/details split, built with Wisej.Web.Markup (no Designer)
   ├─ Shell/TicketEditor.cs           the TableLayoutPanel form: ShowTicket / Read / Clear, SaveClick, Describe()
   ├─ Shell/TicketEditor.Designer.cs  ColumnStyles Absolute 140 | Percent 100, RowStyles, Controls.Add(c, col, row),
   │                                  SetColumnSpan(txtNotes, 2), editors Dock=Fill in their cells
   ├─ Shell/MetricCard.cs / .Designer.cs   one card: Title / Value / Accent; Size 192×76, Margin 4, Min 140×76, Max 0×76
   ├─ Models/Ticket.cs                Ticket, TicketPriority, TicketStatus
   ├─ Models/TicketRepository.cs      12 seed tickets, metrics, Save() with server-side validation
   ├─ Default.json                    "theme": "Bootstrap-4" — the base theme owns every standard control
   ├─ Default.html · Program.cs · Startup.cs · Web.config
   ├─ Properties/launchSettings.json  http://localhost:5505
   └─ docs/
      ├─ LayoutComparison.md          deliverable 5 — container per region, the extended property that did the
      │                               work, expected geometry at 1400 / 1024 / 700, screenshot list, evidence
      ├─ LayoutNotes.md               lab step 7 — the Margin / Padding audit: honoured by flow, flex, table;
      │                               ignored by Dock; the spacing system
      └─ FluentMarkupRegion.md        deliverable 4 — the DashboardWorkspace chain, which extensions exist,
                                      the two fallbacks, one notation per container, no Resize/Bounds code
```

## Deliverables

1. **FlowLayoutPanel filter and metric-card area using WrapContents, FillWeight and FlowBreak** —
   [`AdaptiveOps/Layout/FilterBar.Designer.cs`](AdaptiveOps/Layout/FilterBar.Designer.cs) (the bar) and the
   Flow host + cards in [`AdaptiveOps/MainPage.Designer.cs`](AdaptiveOps/MainPage.Designer.cs) (`flowHost`, `cardOpen … cardClosed`)
2. **TableLayoutPanel ticket editor with RowStyles, ColumnStyles and spans for aligned fields** —
   [`AdaptiveOps/Shell/TicketEditor.Designer.cs`](AdaptiveOps/Shell/TicketEditor.Designer.cs)
3. **FlexLayoutPanel dashboard shell with FillWeight, AlignX, AlignY, MinimumSize and MaximumSize** —
   [`AdaptiveOps/Layout/DashboardWorkspace.cs`](AdaptiveOps/Layout/DashboardWorkspace.cs) (`AlignX` is the
   Vertical-layout counterpart; it is documented in `FluentMarkupRegion.md` and not needed in a Horizontal row)
4. **One region built in code with the Wisej.Web.Markup fluent extensions** — the same
   [`DashboardWorkspace.cs`](AdaptiveOps/Layout/DashboardWorkspace.cs), explained in
   [`AdaptiveOps/docs/FluentMarkupRegion.md`](AdaptiveOps/docs/FluentMarkupRegion.md)
5. **Resize comparison screenshots and a note on which container fits each region** —
   [`AdaptiveOps/docs/LayoutComparison.md`](AdaptiveOps/docs/LayoutComparison.md); screenshots taken by the
   learner as `AdaptiveOps/docs/screenshots/desktop-1400.png`, `tablet-1024.png`, `phone-700.png`
   (+ `table-700.png`, `flex-700.png`). The spacing audit of lab step 7 is
   [`AdaptiveOps/docs/LayoutNotes.md`](AdaptiveOps/docs/LayoutNotes.md)

## Lab step → code map

| Lab step | Where it is in this build |
|---|---|
| Open the Module 4 solution; the shell docks, the workspace content is still positioned by hand | starting point = Module 1's shell (five docked `Panel`s, cards docked Left in fixed slots, editor with `Location` + `Anchor`); everything below replaces that content |
| Lab goal: three versions of the content area, one in code with Markup, compared across a resize | `Layout/FilterBar`, `Shell/TicketEditor`, `Layout/DashboardWorkspace`; the "Same region, three ways" tabs; `docs/LayoutComparison.md` |
| `Layout` folder, `FilterBar : FlowLayoutPanel` — LeftToRight, `WrapContents`, `AutoScroll`, `Padding 8`; `txtSearch`, `cmbStatus`, `btnApply`; `SetFillWeight(txtSearch, 1)` + `MinimumSize` 180; `SetFlowBreak(btnApply, true)` | `Layout/FilterBar.Designer.cs`: `FlowDirection = LeftToRight`, `WrapContents = true`, `AutoScroll = true`, `Padding (8,8,8,2)`; `txtSearch.MinimumSize = (180, 0)`, `SetFillWeight(this.txtSearch, 1)`, `SetFlowBreak(this.btnApply, true)`; the metric cards that "follow" are the Flow host under the bar. `FilterBar.Describe()` traces the values back with `GetFillWeight` / `GetFlowBreak` |
| `TicketEditor : UserControl` around a `TableLayoutPanel` docked Fill, `ColumnCount 2`, Absolute 140 + Percent 100 styles, rows added with `Controls.Add(control, column, row)`, Notes spanning both columns with `MinimumSize` height 120, every editor Dock Fill in its cell | `Shell/TicketEditor.Designer.cs`: `table.Dock = Fill`, `ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F))`, `… (SizeType.Percent, 100F)`, `Controls.Add(this.txtTitle, 1, 2)` …, `SetColumnSpan(this.txtNotes, 2)`, `txtNotes.MinimumSize = (0, 120)`, editors `Dock = Fill`; `RowStyles` nine AutoSize + Percent 100 for Notes |
| `DashboardWorkspace : FlexLayoutPanel` docked Fill, `LayoutStyle Horizontal`, `Padding 8`; `pnlList` and `pnlDetails` with `MinimumSize`, `MaximumSize` on details; `SetFillWeight 2 / 1`, `SetAlignY(pnlDetails, Top)` | `Layout/DashboardWorkspace.cs`: `.Dock(Fill).LayoutStyle(Horizontal).Spacing(8).FillWeight(ListRegion, 2).FillWeight(DetailsRegion, 1).AlignY(DetailsRegion, Top)`; `ListRegion.MinimumSize(300, 200)`, `DetailsRegion.MinimumSize(280, 200).MaximumSize(520, 0)`. The lab's `Padding 8` is the workspace region's padding here, `Spacing 8` is the gap between the two regions |
| One region entirely in code with `using Wisej.Web.Markup;`, chaining `FillWeight`, `FlowBreak`, `AlignX`, `AlignY`, one notation, no designer serialisation | `Layout/DashboardWorkspace.cs` (no Designer file); `docs/FluentMarkupRegion.md` lists the chain, the available `FlowBreak` / `AlignX` extensions and why two non-layout properties are object initialisers |
| Audit spacing: Margin on cards and editors honoured by flow / flex / table; docked regions get gaps from Padding; record both in `LayoutNotes.md` | `MetricCard.Margin = 4`, filter-bar children `Margin (0,0,8,6)`, rail buttons `(0,0,0,8)`, editor rows `(0,0,0,6)`; region `Padding`s in `MainPage.Designer.cs`; `docs/LayoutNotes.md` |
| `btnApply_Click` → `ApplyFilters()` refreshes the list and the status label; thin handler; no Resize handler or `Bounds` arithmetic anywhere | `FilterBar.btnApply_Click` raises `Command(Apply)`; `MainPage.filterBar_Command` calls `ApplyFilters()`, which builds the filter, calls `LoadTickets("Apply")` and `SetStatus(...)`. The three `Resize` handlers only trace; grep evidence in `docs/FluentMarkupRegion.md` |
| Show every path at 1400 / 1024 / 700: cards wrap, search box stretches, editor columns aligned, details stops at `MinimumSize`, a failed `ApplyFilters()` reports in the status label | `← client resize …`, `← client card bounds …`, `← client dashboard … (at MinimumSize)` trace lines; `ApplyFilters` catch → `● filter error: invalid /regex/`; plus the four lab buttons (Switch engine, Add cards, Cell collision, No-wrap overflow) and Restore |
| Review & run: screenshots per region, the comparison note, the layout checklist | `docs/LayoutComparison.md` (§ 1 decision table, § 2–5 per-width geometry, § 7 screenshot list); `VerifyShell()` + `VerifyContainers()` trace the checklist at startup: containers over resize handlers, dock order, Padding on docked regions, flow for wrapping, table for the form, flex for the proportional split, `MinimumSize` on every fill-weighted control |

## Self-check answers (lab guide)

- **You gave the details region FillWeight 1 but no MinimumSize, and at 700 pixels it vanished. Explain
  what the flex engine did with the space and why the fix is a size constraint rather than a different
  weight.**
  The flex engine always fills its client area and hands the *remaining* space to the weighted children
  in proportion: with weights 2 : 1 the list gets two thirds and the details one third of whatever is
  left after the container's padding and spacing. At 700 px the workspace is about 464 px wide, so the
  details' third is roughly 150 px — and because the list region has a `MinimumSize` of 300 and the
  details does not, the engine takes the shortfall from the only child that may still shrink: the details
  region goes to 0 px and the user sees an empty strip where the editor was. A different weight cannot
  fix that, because a weight is a *ratio*, not a floor: 3 : 1, 1 : 1 or 1 : 2 all still describe a share
  of a shrinking total, and at a small enough width every share is too small. The fix is the other
  extended property the flex engine reads, `MinimumSize` — `DetailsRegion.MinimumSize(280, 200)` in
  `DashboardWorkspace.cs`. Now the engine distributes by weight *until* a child reaches its minimum, then
  holds it there and takes the loss from the others (at 1024 px the trace shows `list 500 / details 280
  (details at MinimumSize)`); when both children are at their minimum the row overflows the workspace
  instead of destroying a region, which is the honest failure the lesson prefers. `MaximumSize(520, 0)` is
  the same idea from the other side: it stops a 4K monitor from stretching the form into whitespace. The
  checklist rule — a `MinimumSize` on every fill-weighted control — exists because of exactly this case.
- **The Apply button wraps to the second row at 1024 pixels even though there is room for it. Which
  extended property did that, and how would the FilterBar behave differently if you removed it and made
  WrapContents false?**
  In this build it is the controls *after* Apply that start the second row, and the property is
  `FlowBreak`: `SetFlowBreak(btnApply, true)` tells the flow engine to end the current row after Apply
  regardless of the width, so the lab buttons always begin a fresh row (in the lab's own layout the break
  sits on the control before Apply and Apply itself opens the new row — same mechanism). The width has
  nothing to do with it; the trace prints `FlowBreak(btnApply)=True` and `11 children in 2 row(s)` at
  1400 px, where one row would have had plenty of room. Remove the break and the engine goes back to
  its default rule, wrap only when the next control does not fit: at 1400 px every button joins row 1
  and the search box's `FillWeight 1` absorbs less spare width; at 1024 px the row wraps wherever the
  width runs out, which may split the lab buttons in an unplanned place. Set `WrapContents = false` as
  well and the engine stops wrapping altogether: all eleven children sit on one row and everything past
  the right edge is clipped — or, because the FilterBar has `AutoScroll = true`, reachable only with a
  horizontal scrollbar. The **No-wrap overflow** button does exactly that so it can be seen: the trace
  reports `toolbar overflows by … px (scrolled)`, and on the Flow host, where `AutoScroll` is turned off
  too, `n card(s) past the right edge (CLIPPED)`. A single-line toolbar with a scrollbar is a legitimate
  design; a filter bar that must stay fully visible on a tablet wants `WrapContents` on and a `FlowBreak`
  where the designer, not the width, decides the line ends.
- **A Margin of 8 on the cards in the FlowLayoutPanel produced gaps, but the same Margin on the docked
  navigation rail did nothing. Which engine honours margins in each case, and which property creates the
  gap for the rail?**
  The cards are children of a `FlowLayoutPanel`, and the flow engine enforces margins: it places each
  child at the previous child's edge *plus* both margins, so a `Margin` of 4 on every card produces the
  8-px gaps you see in the Flow host (the trace shows `open 192×76 @(8,8) · overdue 192×76 @(208,8)` —
  200 = 192 + 4 + 4). The `FlexLayoutPanel` honours margins the same way (plus its `Spacing`), and the
  `TableLayoutPanel` uses the margin as the child's distance to its cell border (a 285-px cell shows a
  277-px card). The navigation rail is a child of the page laid out by the **default layout engine**
  with `Dock = Left`, and that engine does not use margins when docking: a docked control is placed
  flush against the remaining client rectangle, so its `Margin` is stored and ignored. The gap around the
  rail comes from **`Padding`**, and from the right owner: `navigationPanel.Padding = (8, 4, 0, 4)` inside
  the docked region leaves room for the white card, and the workspace's own left padding of 8 provides
  the space between the rail and the content — the container's decision about its inner space, not the
  child's about its neighbours. That is why the lesson treats the two properties as belonging to
  different owners and why `docs/LayoutNotes.md` records both findings: when a gap fails to appear, first
  ask which engine owns the child.
