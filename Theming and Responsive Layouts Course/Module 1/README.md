# AdaptiveOps · Theming & Responsive Layouts Course · Module 1

Local lab build for **Module 1 · The Wisej.NET Visual System**. It follows the walkthrough video
("Where a control's look and layout really come from"): the Adaptive Operations Console shell is five
`Panel`s docked on the main `Page` in the order the lab prescribes — toolbar Top, status Bottom,
navigation Left, details Right, workspace Fill with a `MinimumSize` of 320 × 240 — a built-in base theme
(Bootstrap-4) is selected for the *running* application in `Default.json`, and a status-bar label reports
the browser size on first load and on every resize from one `ReportWidth()` method wrapped in
`try / catch`. Nothing in the code-behind positions a control and no Resize handler sets `Bounds`; the
layout containers own position and size, the theme owns the look of every standard control, and the
"Layout & theme · live trace" card logs each decision so it can be read, not guessed. The stretch goal
(four metric cards above a ticket grid) is done too, with real ticket data and a working details editor,
because every later module of the course grows this same console.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 1/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5501
```

Then open <http://localhost:5501>. (Visual Studio: open `AdaptiveOps.slnx`, press F5 — `launchSettings.json`
uses the same port.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

The trace card at the bottom of the workspace is the lab's evidence: every line starts with a timestamp
and `→ render` (server to client), `← client` (client to server) or `• server` (a server-side decision).
On first load you should already see, in this order: `← client resize (constructor): browser W×H · page
W×H · workspace W×H` (the constructor fills the width label before anything else), `• server shell built: toolbar Dock=Top … ·
navigation Dock=Left 220×… · details Dock=Right 340×… · status Dock=Bottom … · workspace Dock=Fill … min
320×240`, `• server base theme: Bootstrap-4 (selected in Default.json; …)`, `• server dock order verified:
Top, Bottom, Left, Right, Fill …`, `→ render page load: 12 tickets · open 8 · overdue 3 · mine 4 · closed
this week 3`, `← client page load: ticket T-1031 "Label printer driver" loaded into the details editor`
and `← client resize (page load): browser W×H · page W×H · workspace W×H`. The status bar reads
`● ready` on the left, `Browser W × H px · Desktop` in the middle and `theme: Bootstrap-4` on the right.

| Action | Path | What you should see |
|---|---|---|
| **Refresh** (toolbar) | success | `→ render Refresh: 12 tickets · open 8 · overdue 3 · mine 4 · closed this week 3`, then `← client Refresh: ticket T-… loaded into the details editor` — the four metric cards, the grid and the editor are all re-read from `TicketRepository`; the selected row is kept; status `● ready` |
| **Simulate load** (toolbar) | progress | `• server simulated load started: 5 steps every 400 ms (Wisej.Web.Timer, no client code)`, then one `→ render progress n/5: …` line per tick (`reading the ticket repository`, `computing the four metrics`, `filling the ticket grid`, `refreshing the details editor`, `done`); the toolbar label counts `Simulated load · step n of 5 · …`, the status turns orange `● loading`, the button is disabled until the last tick, then `→ render simulated load: …`, `Simulated load complete · 5/5`, `● ready` |
| **Invalid ticket** (toolbar) | failure | the Title box is blanked and saved; the repository throws `TicketValidationException` on the server: a red banner `✖ Rejected on the server (Invalid ticket): Title is required. Nothing was written.` docks in above the grid (the grid moves down — no `Bounds` involved), `• server rejected T-… (Invalid ticket): Title is required.`, status `● validation error` in red, an `AlertBox` top-right; the grid and metrics still show the last good state |
| **Reset** (toolbar) | recovery | `• server repository reset to the seed tickets; banner cleared`, then `→ render Reset: …`; the banner disappears (the grid moves back up), the progress label is cleared, a running simulated load is stopped, the editor is refilled from the server, status `● ready` |
| **Clear trace** (toolbar) | – | empties the trace list (nothing is logged for it) |
| Click **Dashboard / Tickets / Reports / Settings / Help** in the rail | navigation | the workspace title changes to the button text (Dashboard shows "Tickets"); `← client navigation: Reports (rail = navigationPanel Dock=Left, five Buttons with Anchor Left\|Right)` — in Module 1 the rail is five plain `Button`s and a click only retitles |
| Select a row in the grid | – | `← client grid selection: ticket T-1042 "Printer offline, floor 3" loaded into the details editor`; the editor subtitle reads `T-1042 · Open · due yyyy-MM-dd` — the editor is filled from the repository copy, never from the grid cells |
| Edit a ticket and press **Save** (details) | success | `• server saved T-1042 "…" (High, Open, Dana, due …)`, `→ render Save: …`, status `● saved T-1042`, `AlertBox` "T-1042 saved."; the grid row and the metrics follow |
| Clear **Owner** (or **Title**) and press **Save** | failure | `• server rejected T-1042 (Save): Owner is required.` (or `Title is required.` / `Title must be 80 characters or fewer.`), the same red banner, `● validation error` — validation runs only in `TicketRepository.Save`, never in the browser |
| Press **Save** with nothing selected (a guard only: the grid always keeps a row selected, so this is not reachable from the UI) | failure | banner `Select a ticket in the grid before saving.`, status `● nothing selected`, `• server save skipped: no ticket selected` |
| Resize the browser window (desktop → 1024 → 768 → 390 px) | width label | the middle status label follows: `Browser 1024 × 768 px · Desktop`; two trace lines per resize, `← client resize (Application.BrowserSizeChanged): …` and `← client resize (Page.Resize): …`, each with the browser, page and workspace sizes. Below 220 + 320 + 340 = 880 px the page overflows horizontally — Module 1 is the desktop shell only (see `docs/VisualInventory.md`, breakpoints) |
| Width read fails (cannot be forced from the UI) | failure | the label falls back to `Browser size unavailable — …` and `• server width read failed (reason): …` is traced; the `catch` never leaves a stale number |

### Where the lab text and this build differ

- **Region widths.** The lab step says `navigationPanel` 240 and `detailsPanel` 360. This build uses the
  course-wide convention from `_template/COOKBOOK.md`: **navigation 220, details 340** (toolbar 56 and
  status 28 are as in the lab). Every module of the course shares these numbers so the samples feel like
  one application; change the two `Size` lines in `MainPage.Designer.cs` if you want the lab's values.
- **Width label.** The lab calls it `widthLabel` with `$"Width: {this.Width} px"`. Here it is
  `lblBrowserWidth` and shows `Browser W × H px · Device` from `Application.Browser.Size` /
  `Application.Browser.Device`, refreshed from **both** `Application.BrowserSizeChanged` and
  `Page.Resize`; `this.Width` is still reported — in the trace line, next to the workspace size, so the
  page-versus-browser relationship the lesson describes is visible.
- **Failure case.** The lab suggests an `AlertBox` when `ReportWidth()` fails; this build writes the
  honest fallback text into the label and logs the exception message to the trace instead (the
  `AlertBox`s are used for the save paths). The `try / catch` and the "never a stale number" rule are the
  same.
- **Card chrome.** The lab's acceptance criterion says no control sets `BackColor`, `ForeColor` or `Font`.
  The five region panels, the page docking and every standard interactive control (buttons, editors,
  grid, list) honour that; the white cards inside the regions, the 4-px metric strips, the muted caption
  labels and the status colours **do** set them, as control properties, with exactly the values the
  Module 2 theme tokens will carry. `docs/VisualInventory.md` lists every occurrence and
  `docs/VisualLayers.md` explains why they are the Module 1 stand-in for the theme.

## Where things live

```
Module 1/
├─ AdaptiveOps.slnx
├─ README.md                        this file
└─ AdaptiveOps/
   ├─ MainPage.Designer.cs          InitializeComponent(): the five docked regions, the cards, the grid,
   │                                the editor, the status bar, timerLoad — every position and size lives here
   ├─ MainPage.cs                   code-behind: ReportWidth(), VerifyShell(), the four toolbar paths, the
   │                                editor, AddTrace(); no layout code at all
   ├─ Models/Ticket.cs              Ticket, TicketPriority, TicketStatus
   ├─ Models/TicketRepository.cs    12 seed tickets, metrics, Save() with server-side validation
   ├─ Default.json                  "theme": "Bootstrap-4"  — the base theme selected for the RUNNING app
   ├─ Web.config                    Wisej.DefaultTheme = Bootstrap-4 (same choice for the IIS/Windows target)
   ├─ Default.html                  the page host (mobile viewport meta tag, wisej.wx)
   ├─ Program.cs                    Application.MainPage = new MainPage()
   ├─ Startup.cs                    Kestrel host: UseWisej(), static files from the project folder, no *.json
   ├─ Properties/launchSettings.json  http://localhost:5501
   └─ docs/
      ├─ VisualInventory.md         deliverable 3 — the four tables + the "what the shell does not set" audit
      └─ VisualLayers.md            deliverable 2 + 5 — base theme choice with its reason; which layer owns
                                    each visual decision, region by region
```

## Deliverables

1. **Adaptive Operations Console solution with a docked desktop shell** — [`AdaptiveOps/MainPage.Designer.cs`](AdaptiveOps/MainPage.Designer.cs)
   (toolbar Top 56, status Bottom 28, navigation Left 220, details Right 340, workspace Fill min 320 × 240;
   the `Controls.Add` order at the end of `InitializeComponent()` is the dock order)
2. **Base theme chosen and configured, with the reason recorded** — [`AdaptiveOps/Default.json`](AdaptiveOps/Default.json)
   (`"theme": "Bootstrap-4"`, i.e. selected for the running application) and
   [`AdaptiveOps/docs/VisualLayers.md`](AdaptiveOps/docs/VisualLayers.md) § Base theme
3. **VisualInventory.md** — [`AdaptiveOps/docs/VisualInventory.md`](AdaptiveOps/docs/VisualInventory.md):
   controls needing appearances, the ten colour and three font tokens, the five layout regions, the
   breakpoints, each row with its owning layer
4. **Status label that reports the current browser width** — `lblBrowserWidth` in the status bar,
   filled by `ReportWidth()` in [`AdaptiveOps/MainPage.cs`](AdaptiveOps/MainPage.cs) from the constructor,
   `Load`, `Application.BrowserSizeChanged` and `Page.Resize`
5. **Desktop screenshot and the layer-ownership note** — screenshot taken by the learner as
   `AdaptiveOps/docs/screenshots/desktop-1440.png` (the video shows the same shell); the note is
   [`AdaptiveOps/docs/VisualLayers.md`](AdaptiveOps/docs/VisualLayers.md)

The lab guide does not name a separate theme-choice document, so the base-theme decision is recorded
inside `VisualLayers.md` (and echoed in the first table of `VisualInventory.md`).

## Lab step → code map

| Lab step | Where it is in this build |
|---|---|
| Start the solution: `AdaptiveOperationsConsole`, empty main `Page` opens | `AdaptiveOps.slnx`, `Program.cs` (`Application.MainPage = new MainPage()`), `Default.json` `startup`; the project keeps the course name `AdaptiveOps` |
| Five `Panel`s docked in order: toolbar Top 56, status Bottom 28, navigation Left, details Right, workspace Fill + `MinimumSize` 320 × 240 | `MainPage.Designer.cs`: `toolbarPanel` (Top, 56), `statusPanel` (Bottom, 28), `navigationPanel` (Left, 220), `detailsPanel` (Right, 340), `workspacePanel` (Fill, `MinimumSize` 320 × 240); the `this.Controls.Add(...)` block at the end (workspace, details, navigation, status, toolbar — added last docks first). `VerifyShell()` in `MainPage.cs` checks it at startup and traces `• server dock order verified …` |
| Choose a built-in theme and select it for the running application; record the reason | `Default.json` `"theme": "Bootstrap-4"` (Kestrel) and `Web.config` `Wisej.DefaultTheme` (Windows target); `MainPage_Load` traces `• server base theme: Bootstrap-4 …` and `lblTheme` shows `theme: Bootstrap-4` (`Application.Theme.Name`); reason in `docs/VisualLayers.md` |
| `widthLabel` in `statusPanel`, `Resize` handled, one `ReportWidth()` called from the constructor too | `lblBrowserWidth` (`statusCard`, `Dock=Fill`); `MainPage.cs`: `ReportWidth(string reason)` called from the constructor (`"constructor"`), `MainPage_Load`, `Application_BrowserSizeChanged` and `MainPage_Resize`; `Dispose(bool)` in the Designer unsubscribes the session-level event |
| `VisualInventory.md` with four tables (controls needing appearances, ten colour + three font tokens, five regions, breakpoints) | `docs/VisualInventory.md` §1–§4 |
| "Owned by" column on every row; confirm the shell sets no `BackColor`, `ForeColor`, `Font`, `CssStyle` and no Resize code sets `Bounds` | `docs/VisualInventory.md` (every table has the column) and §5 "What the shell does and does not set", with the grep evidence; `MainPage_Resize` only calls `ReportWidth` |
| Stretch: placeholder workspace content — four metric-card `Panel`s above a `DataGridView` | `metricsPanel` (Top, 84) with `slotOpen / slotOverdue / slotMine / slotClosed` (Left, 192 each) and `gridCard` (Fill) with `gridTickets`; `LoadTickets()` fills both from `TicketRepository` |
| Show every path: first-load width, live update on resize, failure with an honest fallback | `ReportWidth("constructor")` and `ReportWidth("page load")`; `Application.BrowserSizeChanged` + `Page.Resize`; the `catch` writes `Browser size unavailable — …` and traces `• server width read failed …` |
| Review & run: resize desktop → tablet → phone, take the desktop screenshot, write the layer note | the `← client resize (…)` trace lines; screenshot `docs/screenshots/desktop-1440.png` (taken by the learner); `docs/VisualLayers.md` |

## Self-check answers (lab guide)

- **The toolbar's primary command needs the brand colour on every screen. Which of the three layers owns
  that decision, and what would go wrong if you set `BackColor` on the button instead?**
  The theme owns it, through a semantic appearance: Module 2 adds an `action-button` appearance to the
  AdaptiveOps theme (inheriting `button`) whose `default` state paints `brandPrimary` and whose
  `hovered`, `pressed`, `focused` and `disabled` states are defined once, and the toolbar button selects
  it with `AppearanceKey = "action-button"`. A `BackColor` on the button is a layer-two control property
  that overrides the theme decorator for that one widget in *every* state: the hover and pressed shades
  disappear (the button looks frozen), the disabled state stays brand-blue, the designer renders it
  differently from the runtime, a later brand change means hunting every button that carries the colour,
  and swapping the base theme leaves the hard-coded blue behind. In this build the toolbar buttons
  deliberately carry no colour at all; the brand blue you can already see is the 4-px strip on the "Open"
  card, and `VisualInventory.md` marks it as the value the theme takes over in Module 2.
- **Your width label reads 1024 px and the details panel is still visible. Which mechanism from a later
  module will decide whether it should be, and why is a CSS media query the wrong first answer in
  Wisej.NET?**
  Client profiles decide it: `ClientProfiles.json` names the breakpoints (`Small Desktop` has
  `maxWidth 1024` in the framework's default file), `Application.ActiveProfile` tells the server which
  profile the request matched, the Designer's responsive properties store a per-profile `Visible`
  (or `Dock`, `Size`, `Font`) for `detailsPanel`, and `ResponsiveProfileChanged` handles the behaviour a
  property value cannot express (stacking the editor under the grid on the phone, Module 6). A media
  query is the wrong first answer because the details panel is not a CSS box the application owns: it is
  a server-side control whose size and visibility are decided by the Dock layout engine on the server
  and pushed to a generated widget. A `display:none` from a stylesheet would hide the DOM element while
  the server still believes the panel is 340 px wide, so the workspace would not grow into the space,
  `Visible` would still read `true`, the code that fills the editor would keep running, and the rule would
  fight the theme engine's generated classes and break on the next upgrade. Wisej.NET resolves the
  profile on the server so the layout engine, the control model and the code-behind all agree.
- **If the base theme were swapped for a different built-in theme tomorrow, which rows of your
  `VisualInventory.md` would change and which would not, and what does that tell you about where the
  inventory's decisions live?**
  Only the rows owned by the **theme** would change: the look of every standard control in table 1
  (buttons, text boxes, combo boxes, the date picker, the grid header and rows, the list, tooltips, the
  `invalid` state), the *values* the ten colour tokens and three fonts resolve to in table 2, and the
  base-theme row itself. The rows owned by a **layout container** would not move a pixel: the five
  regions keep `Dock = Top / Bottom / Left / Right / Fill`, 56 / 28 / 220 / 340 and the 320 × 240
  minimum, the metric slots stay 192 px wide, the banner still docks in above the grid. The breakpoint
  rows in table 4 would not change either, because they belong to client profiles and responsive
  properties, not to the theme. The rows owned by a **control property** (the white card `BackColor`,
  the muted `ForeColor`, the label `Font`s) would keep their hard-coded values and therefore might clash
  with the new theme — which is exactly the argument for moving them into theme tokens in Module 2. The
  inventory thus shows that the decisions live in three independent places: identity in the theme,
  geometry in the containers, device behaviour in the profiles; a decision that is hard-coded on a
  control lives nowhere the system can manage, and that is the row to fix.
