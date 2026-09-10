# Deliverable · Lab notes — every hidden control and its mobile alternative

The lab's acceptance criterion: *"the lab notes record every hidden control with its mobile alternative"*,
and *"why each change lives in the designer or in the profile handler"*. This is that register, plus the
theme-vs-CSS-vs-layout decisions the module touched.

## 1. Hidden or reduced controls, profile by profile

| Control | Hidden / reduced on | Alternative on that profile | Owner |
|---|---|---|---|
| `navigationPanel` + `NavigationRail` (5 sections) | **Phone, Phone (Landscape)** — `Visible = false` | `btnMenu` (☰) in the toolbar opens a `ContextMenu` with the same five `MenuItem`s; each raises `NavigationRail.SectionSelected`, so the navigation code path is identical | designer value (`Visible` per profile) — expressed in `ApplyKind` here |
| `NavigationRail` labels | **Tablet, Tablet (Landscape)** — `Display.Icon` | `ToolTipText` and `AccessibleName` carry the label; icons are theme images (`icon-columns`, `icon-file`, `icon-print`, `icon-settings`, `icon-help`) | designer value (`Display` per profile) |
| `lblNavTitle` ("NAVIGATION") | Tablet profiles | none needed — it is a caption for a rail that is now self-explanatory | designer value |
| `detailsPanel` (docked editor) | **Phone, Phone (Landscape)** — `Visible = false` | the **same** `TicketEditor` instance is reparented into `TicketEditorForm` and opened with `ShowDialog` when a row is selected; Save / validation / success messages live inside the editor; a **Close** button appears only in the dialog | **profile handler** — not a value: reparenting + `ShowDialog`, guarded to run once |
| `detailsPanel` position | **Tablet** — `Dock.Right → Dock.Bottom`, 320 px | still docked; the card `AutoScroll`s because the editor keeps `MinimumSize` 440 | designer value (`Dock`, `Size` per profile) |
| `detailsPanel` width | Small Desktop, Tablet (Landscape) — 340 → 300 px | fields are anchored; only Notes shrinks | designer value (`Size`) |
| Toolbar labels (`btnRefresh`, Phone, Tablet, Desktop, Step, Unknown, Faulty region, Re-apply) | **every profile below Desktop** — `Display.Icon`, 36 px | `ToolTipText` (the full sentence) and `AccessibleName` (the label); the `FlowLayoutPanel` re-flows, so nothing overlaps | designer value (`Display`, `Size`) |
| `lblAppTitle` | Phone — hidden; Phone (Landscape) — "AdaptiveOps" | the browser tab title (`Page.Text`) still names the app; the grid card title names the section | designer value (`Visible`, `Text`) |
| `colOwner` | **Tablet, Phone, Phone (Landscape)** | Owner is in the editor (docked or modal) | designer value on the `DataGridViewColumn` |
| `colDue` | **Phone, Phone (Landscape)** | Due is in the editor; the Overdue metric card still counts it | designer value on the column |
| `lblTheme` (status bar) | Phone profiles | the theme is unchanged by profiles; the trace logs it at startup | designer value |
| Metric cards (`cardOpen` … `cardClosed`) | never hidden — **re-shaped** 4×1 / 2×2 / 1×4 | – | profile handler (`LayoutMetrics`: `ColumnCount`, `RowCount`, `SetCellPosition`) |
| `btnClose` (editor) | visible **only** in the dialog | on desktop the docked panel needs no Close | profile handler (`CloseButtonVisible`) |

Nothing on any profile is "gone": every section, field and command has a named way to reach it.

## 2. Designer or profile handler — the decision

Rule from the lesson: a **deterministic value per profile** (Visible, Display, Dock, Size, Location, Font …)
belongs in the Designer's responsive-profile dropdown (`Control.ResponsiveProfiles`), because it is serialized
with the form, visible to the next developer and applied by the framework. **Code** (`ResponsiveProfileChanged`)
is for adaptations a value cannot express: opening a Form, reparenting a control, building UI from data,
changing a debounce, loading fewer columns from a query, logging.

In this hand-written sample **both** kinds live in `MainPage.ApplyKind`, one region per block, so the whole
profile table is readable in one place (see `docs/ProfileNotes.md` § 4 for the row-by-row mapping). The
README says so explicitly. What would go wrong if the two were swapped:

- Putting the **modal editor in the designer** is impossible — there is no property whose value is "show a
  Form". Trying to approximate it (a second, designer-placed editor made `Visible` on Phone) would create two
  editors with two copies of the ticket and no way to keep them in sync.
- Putting the **tablet re-dock in code** works, but the Designer would keep showing the desktop picture, the
  next developer would not find the tablet layout in the property grid, and a typo in the profile-name
  comparison would silently skip it. That is the drift the course warns about.

## 3. Theme vs CSS vs layout vs profile (module decision record)

| Decision | Mechanism | Why not the others |
|---|---|---|
| Rail / details hidden on phone, icon-only toolbar, Owner/Due hidden | **responsive property values** (server) | CSS `display:none` leaves server controls alive and shipping data; a layout container cannot decide *whether* a region exists |
| Editor as modal on phone | **`ResponsiveProfileChanged`** | not a value (see above) |
| Cards 4×1 / 2×2 / 1×4 | **TableLayoutPanel shape** driven by the profile | Dock/Anchor cannot express a grid; a FlowLayoutPanel would wrap by width, not by profile (fine visually, but it would not be a *profile* decision and would need a height computation) |
| Toolbar re-flow when buttons shrink | **FlowLayoutPanel** | a `Location` per button per profile is the "resize arithmetic" the course forbids |
| Editor scrolls under the grid on tablet | `MinimumSize` on the editor + `AutoScroll` on the card | shrinking the Notes box to 2 px (Anchor only) is not usable |
| Icons on buttons | **theme images** (`ImageSource = "icon-…"`) | inline SVG/data URIs would bypass the theme; the theme owns identity |
| Status colours (green / amber / red) | control `ForeColor` — Module 1 carry-over | Module 2 moves them into theme colour tokens; Module 6 does not touch the visual layers |
| Card backgrounds, borders | `BackColor White` + `BorderStyle.Solid` — Module 1 carry-over | Module 2 (theme) / Module 3 (CssClass) own these; not this module's topic |

## 4. Before / after (what the learner captures)

- `before-desktop.png` — Module 5 state: every width shows the desktop shell, status bar reports only the
  browser width, `Application.ActiveProfile` is null (the framework defaults still match, but nothing reacts).
- `after-desktop.png` — the same window with `profile: Desktop · …` in the status bar and the six-profile list
  in the trace.
- `after-phone.png` — 390×844 emulation after refresh: ☰ Menu, stacked cards, 4-column grid, and the
  maximized editor dialog after tapping a row, with the profile label visible in the status bar behind it.
- `after-tablet.png` — 768×1024: icon-only rail, details under the grid.

## 5. One question the grounded AI assistant should answer from the reading

*"If I put `Desktop` first in `ClientProfiles.json`, why does my phone still get the desktop shell, and where
would Wisej.NET have warned me?"* — Expected answer: profiles are matched top to bottom and the first match
wins; `Desktop` has no width rule (only `device: Desktop`) so on a phone it does **not** match, but if it
were written without any rule it would match everything and shadow every entry below it; nothing warns
because a shadowed profile is valid JSON. Order narrow → broad and read `Application.Browser.Profiles` at
startup to see the effective order.

## Evidence

- Phone profile (button or emulator): the trace line `→ render profile "Phone": toolbar: icon-only, Menu button
  shown · navigation: rail hidden → Menu button · details: editor → modal TicketEditorForm (opens on row
  selection) · metrics: cards 1×4 · grid: hidden Owner, Due (still in the editor) · status: profile + size
  (theme label hidden)` is the register above, generated by the code.
- ☰ Menu → *Settings* retitles the workspace exactly as clicking the rail button does on desktop.
- Selecting `T-1040` on Phone opens the dialog; its Owner and Due fields show what the grid no longer does.
- Tablet: hovering the icon-only rail shows "Dashboard", "Tickets", … as tooltips; the details card under the
  grid scrolls to the Notes box and the Save button.
