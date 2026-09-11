# AdaptiveOps · Theming & Responsive Layouts Course · Module 6

Lab build for **Module 6 · ClientProfile and Responsive Properties**: profiles that change behaviour, not just
widths. `ClientProfiles.json` in the project root defines six profiles narrow to broad (Phone, Phone
(Landscape), Tablet, Tablet (Landscape), Small Desktop, Desktop); the framework matches them into
`Application.ActiveProfile`. `ApplyProfile(string profileName)` runs at startup and on every
`Application.ResponsiveProfileChanged`: it writes the profile into the status bar (`lblProfile`), adds one line to
the profile log, hides the rail and the details panel on a phone and opens the ticket editor there as a modal
`Form`, docks the details under the grid on a tablet, and switches the toolbar and the rail to icon-only.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 6/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5506
```

Then open <http://localhost:5506> (or open `AdaptiveOps.slnx` in Visual Studio and press F5).

## What to try

- **Wide desktop window**: status bar `Profile: Desktop · 1348 px`; profile log `Startup: Desktop`.
- **Drag the window to ≤ 1024 px and back**: `Profile: Small Desktop`, the log adds `Desktop → Small Desktop`;
  the toolbar buttons become icons (their tooltip is the label) and the details panel narrows to 300 px.
- **Devtools device toolbar → iPad → refresh**: `Profile: Tablet`; the rail shows icons only and the details
  panel is docked under the grid (it scrolls).
- **Devtools → iPhone 12 Pro → refresh**: `Profile: Phone`; the rail and the details panel are hidden, the ☰
  **Menu** button lists the same sections, the metric cards go two per row, the grid hides Owner and Due. Tap a
  row: the editor opens as a maximized dialog, once. Save with an empty Title: the error shows inside the
  dialog; a valid Save shows a success line in the dialog and a toast.
- **Re-apply** reads `Application.ActiveProfile` again and applies it; **Refresh** reloads the tickets; **Clear
  log** empties the profile log.

## Where things live

```
AdaptiveOps/
├─ ClientProfiles.json            six profiles, narrow to broad (Content, copied to bin for both targets)
├─ MainPage.cs                    ApplyProfile(string), the event subscription, the phone dialog, the profile log
├─ MainPage.Designer.cs           the shell: toolbar FlowLayoutPanel, regions, metrics TableLayoutPanel, grid, log, status bar
├─ Shell/NavigationRail.cs        the rail UserControl: SetMode(Full | IconOnly), SectionSelected, Sections
├─ Shell/TicketEditor.cs          the editor UserControl: LoadTicket / ReadTicket / ShowMessage, Save and Close events
├─ Dialogs/TicketEditorForm.cs    the reusable modal host: HostEditor(control)
└─ docs/
   ├─ ProfileNotes.md             every profile, its rule, what changes per region, designer vs code, idempotency
   ├─ ResponsiveQAMatrix.md       the six scenarios, the checks per region, the event checks
   └─ LabNotes.md                 every hidden control with its mobile alternative
```

In Visual Studio the per-profile `Visible`, `Display` and `Dock` values would be set in the Designer's
responsive-profile dropdown (`Control.ResponsiveProfiles`). This hand-written sample assigns them in
`ApplyProfile`, next to what genuinely needs code (the modal editor, the Menu, the log).

## Lab step → code map

| Lab step | Where |
|---|---|
| `ClientProfiles.json` in the application root, six profiles in first-match order | `ClientProfiles.json`; the csproj `Content Update` |
| `NavigationRail` and `detailsPanel` `Visible` false on both Phone profiles; `gridTickets` keeps its `MinimumSize` | `ApplyProfile` (navigation and details); `gridTickets.MinimumSize = 280×120` in the Designer |
| `Display` of `btnMenu`, `btnRefresh`, `btnReapply`, `btnClearTrace` icon-only; `detailsPanel.Dock` Bottom on both Tablet profiles | `ApplyProfile` (toolbar and details) |
| Subscribe once to `Application.ResponsiveProfileChanged`; `ApplyProfile(string profileName)` sets `lblProfile.Text` and writes one log line; called in the constructor with `Application.ActiveProfile?.Name` and from the handler with `e.CurrentProfile?.Name`; unsubscribed in `Dispose` | `MainPage()`, `Application_ResponsiveProfileChanged`, `ApplyProfile`, `MainPage.Designer.cs` `Dispose(bool)` |
| On Phone, open `TicketEditorForm` with `ShowDialog`, reuse an open form, lose nothing typed when returning to a wider profile | `HostEditorInDialog`, `OpenEditorDialog` (`_editorForm.Visible` guard), `HostEditorInPanel` (the same editor instance moves back) |
| Responsive QA matrix | `docs/ResponsiveQAMatrix.md` |
| Lab notes: every hidden control and its mobile alternative | `docs/LabNotes.md` |

Where the lab and the video differ: the lab makes the toolbar icon-only on Phone and Tablet; the video (and this
build) also does it on Small Desktop. The video's phone shows the metric cards two per row, which this build follows.

## Self-check answers (lab guide)

- **If Small Desktop were moved to the top of `ClientProfiles.json`, which of the other five profiles could still
  be matched, and what would a tablet in landscape see?** All five: `Small Desktop` only claims clients whose
  `device` is `Desktop` and whose width is at most 1024 px, so phones and tablets fall through as before and wide
  desktops reach `Desktop`. A landscape tablet still sees `Tablet (Landscape)`: icon-only rail, details under the
  grid, icon-only toolbar. The dangerous move is putting a rule-less or broad entry at the top: it matches first
  and silently shadows everything below it.
- **The tablet re-dock lives in the designer and the phone modal editor in code. What decides which mechanism owns
  each change, and what would go wrong if you swapped them?** Whether the change is a *value* or an *action*. The
  re-dock is a property value that depends only on the profile, so it belongs in the Designer's responsive
  properties, visible in the property grid and applied by the framework. Opening the editor as a modal `Form`
  reparents a control and calls `ShowDialog`, guarded to run once, so it can only be code. In code the re-dock
  would drift out of sight of the Designer; as a designer value the modal editor is impossible, and a second
  designer-placed editor would hold a second copy of the ticket.
- **A tester rotated a phone four times and the handler ran four times. Which state must be identical after the
  fourth run and after the first, and how does the code guarantee it?** Exactly one `TicketEditor`, hosted in one
  `TicketEditorForm`, shown once, holding what the tester typed; the rail hidden; the Menu visible; the same grid
  columns and status text. `ApplyProfile` assigns target values instead of toggling, `HostEditorInDialog` returns
  when the editor is already in the form, and `OpenEditorDialog` returns when the form is already visible.
