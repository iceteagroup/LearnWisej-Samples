# AdaptiveOps · Theming & Responsive Layouts Course · Module 1

Lab build for **Module 1 · The Wisej.NET Visual System**: the Adaptive Operations Console shell as five
`Panel`s docked on the main `Page` (toolbar Top, status Bottom, navigation Left, details Right, workspace
Fill with a `MinimumSize` of 320 × 240), a built-in base theme (Bootstrap-4) selected for the running
application in `Default.json`, and a status-bar `widthLabel` that reports the browser width on first load
and on every resize. The stretch goal (four metric cards above a ticket grid) is included, with a working
details editor, because every later module grows this same console.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 1/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5501
```

Then open <http://localhost:5501> (or open `AdaptiveOps.slnx` in Visual Studio and press F5).

## What to try

- **Resize the browser** from desktop to tablet to phone width: `widthLabel` in the status bar follows
  (`Width: 1024 px`). The docked regions hold; below 220 + 320 + 340 px the details panel is pushed out.
  The desktop shell does not adapt yet, which is what later modules fix.
- **Select a row** in the grid: the details editor on the right shows that ticket.
- **Edit and Save**: the grid and the metric cards follow, the status label reads `Saved T-1042`.
  Clear the Title or Owner and Save: the server rejects it and the status label says why.
- **Refresh** reloads the metric cards, the grid and the editor from the repository.
- Click a navigation button: the workspace heading changes.

## Where things live

```
AdaptiveOps/
├─ MainPage.Designer.cs          the five docked regions, metric cards, grid, details editor, status bar
├─ MainPage.cs                   ReportWidth() (constructor, Load, Resize), grid and editor code
├─ Models/Ticket.cs, TicketRepository.cs   12 seed tickets, metrics, Save() with server-side validation
├─ Default.json                  "theme": "Bootstrap-4" (the base theme for the running app)
├─ Web.config                    Wisej.DefaultTheme = Bootstrap-4 (Windows/IIS target)
└─ docs/
   ├─ VisualInventory.md         controls, tokens, regions, breakpoints, each with its owning layer
   └─ VisualLayers.md            base theme choice and reason; which layer owns each visual decision
```

Region sizes follow the course-wide convention (navigation 220, details 340); the lab text says 240 / 360.
Change the two `Size` lines in `MainPage.Designer.cs` to use the lab's values.

## Lab step → code map

| Lab step | Where |
|---|---|
| Five `Panel`s docked in order, workspace `MinimumSize` 320 × 240 | `MainPage.Designer.cs`: the `this.Controls.Add(...)` block at the end of `InitializeComponent()` (workspace, details, navigation, status, toolbar; the control added last docks first) |
| Base theme selected for the running application, reason recorded | `Default.json`, `Web.config`; reason in `docs/VisualLayers.md` |
| `widthLabel` in `statusPanel`, `Resize` handled, one `ReportWidth()` also called from the constructor | `MainPage.cs`: `ReportWidth()` from the constructor, `MainPage_Load` and `MainPage_Resize` |
| `VisualInventory.md` with four tables and an "owned by" column | `docs/VisualInventory.md` |
| Stretch: four metric-card `Panel`s above a `DataGridView` | `metricsPanel` with `cardOpen`, `cardOverdue`, `cardMine`, `cardClosed`; `gridCard` with `gridTickets` |
| Failure case: try / catch with an honest fallback and an `AlertBox` | the `catch` in `ReportWidth()` writes `Width: unavailable` and shows an `AlertBox` |
| Desktop screenshot and layer note | screenshot taken by the learner; note in `docs/VisualLayers.md` |

## Self-check answers (lab guide)

- **The toolbar's primary command needs the brand colour on every screen. Which layer owns that decision,
  and what goes wrong with `BackColor` on the button?** The theme, through a semantic appearance: Module 2
  adds `action-button` (inheriting `button`) and the button selects it with
  `AppearanceKey = "action-button"`. A `BackColor` overrides the theme for that one widget in every state:
  hover and pressed disappear, the disabled state stays blue, and a brand change or a new base theme means
  hunting every button that carries the colour.
- **Your width label reads 1024 px and the details panel is still visible. Which later mechanism decides
  whether it should be, and why is a CSS media query the wrong first answer?** Client profiles:
  `ClientProfiles.json` names the breakpoints, `Application.ActiveProfile` tells the server which one
  matched, responsive properties hold a per-profile `Visible` / `Dock` / `Size`, and
  `ResponsiveProfileChanged` handles what a value cannot express (Module 6). A media query hides a DOM
  element while the server still lays the panel out at 340 px, `Visible` still reads `true`, and the rule
  fights the theme's generated classes.
- **If the base theme were swapped tomorrow, which inventory rows would change?** Only the rows owned by
  the theme: the look of standard controls, the token values and the base-theme row. Rows owned by layout
  containers (regions, docking, minimum sizes) and the breakpoint rows (profiles) would not move. Rows
  that hard-code a colour or font on a control would keep their values and might clash, which is why
  Module 2 moves them into theme tokens.
