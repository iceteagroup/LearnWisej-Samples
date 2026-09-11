# Lab notes · every hidden control and its mobile alternative

The lab's acceptance criterion: the lab notes record every hidden control with its mobile alternative, and why
each change lives in the designer or in the profile handler.

## 1. Hidden or reduced controls, profile by profile

| Control | Hidden / reduced on | Alternative on that profile | Owner |
|---|---|---|---|
| `navigationPanel` + `NavigationRail` | **Phone, Phone (Landscape)**: `Visible = false` | `btnMenu` (☰) opens a `ContextMenu` with the same five sections; each raises `NavigationRail.SectionSelected`, so the navigation code is the same | designer value (`Visible` per profile), assigned in `ApplyProfile` here |
| `NavigationRail` labels | **Tablet, Tablet (Landscape)**: `Display.Icon` | `ToolTipText` and `AccessibleName` carry the label | designer value (`Display`) |
| `lblNavTitle` | Tablet profiles | none needed | designer value |
| `detailsPanel` (docked editor) | **Phone, Phone (Landscape)**: `Visible = false` | the same `TicketEditor` instance moves into `TicketEditorForm` and opens with `ShowDialog` when a row is selected; Save, validation and success messages live inside the editor; a Close button appears only in the dialog | **profile handler**: reparenting and `ShowDialog`, guarded to run once |
| `detailsPanel` position | **Tablet, Tablet (Landscape)**: `Dock.Right → Dock.Bottom`, 320 px | still docked; the card scrolls because the editor keeps its `MinimumSize` | designer value (`Dock`, `Size`) |
| `detailsPanel` width | Small Desktop: 340 → 300 px | fields are anchored; only Notes shrinks | designer value (`Size`) |
| Toolbar labels (`btnRefresh`, `btnReapply`, `btnClearTrace`) | every profile below Desktop: `Display.Icon`, 36 px | `ToolTipText` names the command; the `FlowLayoutPanel` re-flows | designer value (`Display`, `Size`) |
| `lblAppTitle` | Phone profiles: short text "AdaptiveOps" | the browser tab title still names the app | designer value (`Text`) |
| `colOwner`, `colDue` | Phone profiles | both are in the editor (the modal) | designer values on the columns |
| Metric cards | never hidden: two per row on Phone | – | profile handler (`LayoutMetrics`) |
| `btnClose` (editor) | visible only in the dialog | – | profile handler (`CloseButtonVisible`) |

Nothing on any profile is gone: every section, field and command has a named way to reach it.

## 2. Designer or profile handler

A **deterministic value per profile** (Visible, Display, Dock, Size, Font) belongs in the Designer's
responsive-profile dropdown (`Control.ResponsiveProfiles`): serialized with the form, visible to the next
developer, applied by the framework. **Code** (`ResponsiveProfileChanged`) is for what a value cannot express:
opening a Form, reparenting a control, building UI from data, logging.

In this hand-written sample both kinds live in `MainPage.ApplyProfile`, so the whole profile table reads in one
place. Swapping them: the modal editor as a designer value is impossible (a second designer-placed editor would
hold a second copy of the ticket); the tablet re-dock in code works but the Designer keeps showing the desktop
picture and a typo in the profile-name check skips it silently.

## 3. Before and after (screenshots taken by the learner)

- `before-desktop.png`: the Module 5 state, the desktop shell at every width.
- `after-desktop.png`: `Profile: Desktop · …` in the status bar.
- `after-phone.png`: 390×844 emulation after a refresh: ☰ Menu, cards two per row, four grid columns, the
  maximized editor dialog after tapping a row.
- `after-tablet.png`: 768×1024: icon-only rail, details under the grid.

## 4. One question the grounded AI assistant should answer

*"If I put `Desktop` first in `ClientProfiles.json`, why does my phone still get the desktop shell, and where would
Wisej.NET have warned me?"* Profiles are matched top to bottom and the first match wins; an entry without rules
would match everything and shadow the entries below it; nothing warns, because a shadowed profile is valid JSON.
Order the file narrow to broad and check the status bar on each device.
