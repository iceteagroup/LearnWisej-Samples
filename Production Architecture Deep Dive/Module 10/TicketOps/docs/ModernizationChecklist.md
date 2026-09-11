# Deliverable — UI modernization checklist, applied to two screens

*Module 10 deliverable · TicketOps Console · screens: `Views/OperationsDashboard` and `Views/WorkOrderDetail`*

The checklist from the walkthrough (spacing · hierarchy · contrast · icons · status messages · theme
exceptions) plus the applied guide's six questions, walked against both screens. ✔ = holds; ◐ = holds
with a documented exception.

## The checklist

| # | Check | Operations Dashboard | Work Order Detail | Evidence |
|---|---|---|---|---|
| 1 | **Spacing** — consistent rhythm, no cramped corners | ✔ 24 px card inset, 16 px between KPI cards, 12 px inside them, 10 px between buttons | ✔ 24 px inset, 30 px row pitch, 210 px value column | `*.Designer.cs`: every `Location` sits on the 24/16/12/10 scale |
| 2 | **Hierarchy** — titles, labels & values clearly ranked | ✔ title 14 bold → KPI value 22 bold → selected order 11 bold → body default → footer 9 | ✔ title 14 bold → captions 10 bold → values default | fonts are `new Font("default", …)`: the theme's font family, no other family |
| 3 | **Contrast** — readable in both themes | ✔ no `BackColor`/`ForeColor` on any surface or label; the theme pairs `window`/`windowText` itself; chip tints are translucent | ✔ same | switch **Theme → Dark**: cards, grid and dialog all turn dark with light text; nothing stays white |
| 4 | **Icons** — one set, one weight | ◐ no bitmap icons; the banner's ● and ✖ glyphs come from the theme font. A toolbar with `ImageSource = "resource.wx/…"` icons is the documented next step | ◐ same | `Views`: no `ImageSource`, no file paths |
| 5 | **Status messages** — chips & status bar, same vocabulary | ✔ `StatusChip` for work-order status; `StatusBanner` "● state" + banner line for the screen's own state; both read from `Status.*`/`State.*` resources | ✔ the same chip in the header; results via `AlertBox` top-right | `Resources/Strings.resx` `Status.*`, `State.*`, `Message.*` |
| 6 | **Theme exceptions** — documented, screen-level, rare | ◐ one, listed below | ✔ none | grep for `Color.` in `Views/`: 0 hits |

### The applied guide's six questions

| Question | Answer |
|---|---|
| Does every status colour come from the theme or a class token, with no inline `BackColor`? | Yes — `StatusChip` sets a theme **state** on the `chip` appearance; `Themes/TicketOps.mixin.theme` holds the palette (`chip-open` → `primary`, …). `grep BackColor Controls/StatusChip*` → 0. |
| Does the light/dark toggle re-theme the whole console, including chips? | Yes — `ThemeSwitcher.Apply` → `Application.Theme`; cards, grid, buttons, dialog and chips re-render without any chip code running. |
| Are toolbar icons resolved from managed resources rather than loose paths? | There are no bitmap icons in this build. If added: `Button.ImageSource = "resource.wx/TicketOps.Resources.refresh.svg"` behind one `Glyphs` constant class, never a path literal. |
| Is every visible label localizable, and does the panel format dates and currency by `CultureInfo`? | Yes — every caption is a key in `Strings.resx`; `LocalizationService.Format*` formats with the session culture; `docs/LocalizationNotes.md`. |
| Does the widest translated label still fit without clipping in both themes? | Yes — `In Bearbeitung` (chip 132 px) and `Details öffnen…` / `Nächster Status` (170 px buttons) checked in light and dark. |
| Could a future rebrand change the palette without editing individual screens? | Yes — the screens name no colour; a rebrand edits the theme (or the four `chip-*` aliases in the mixin) and nothing under `Views/`. |

## What was modernized (before → after)

| Area | Template / earlier modules | Module 10 |
|---|---|---|
| **Theme** | `Form.BackColor = Color.FromArgb(238,242,247)`, `panelScreen.BackColor = White` | no `BackColor` anywhere: the theme paints every surface (`window` in Bootstrap-4 = white, in BootstrapDark-4 = `#212429`); switching theme re-skins all of it |
| **Chips** | status shown as plain grid text | one `StatusChip` UserControl, themed through appearance + state, reused on three surfaces |
| **Spacing** | ad-hoc positions | 24/16/12/10 scale; KPI cards 166 × 90 on a 182 px pitch |
| **Icons** | — | glyphs from the theme font only; no loose image paths |
| **Resources** | `Strings.cs` with three `const` English sentences | `Strings.resx` + `Strings.de.resx` behind a `ResourceManager`; `Strings.cs` is the typed accessor; screens reference keys |
| **Culture** | server default culture, `ToString()` without a culture | `SessionContext.Culture` → `LocalizationService` → `Application.CurrentCulture`; every date/number/currency formatted with the session culture |

## Documented theme exception

**`Controls/StatusBanner`** keeps five semantic literals (`ColorFor(StatusKind)` — blue/green/orange/red/grey
for busy/success/warning/error/normal). It is the shared template control every module uses, and its
colours are *data-driven state*, the lesson's one legitimate inline case. Promotion path: alias them to
the theme's `primary`/`success`/`warning`/`danger`/`secondary` tokens (all exist in both Bootstrap themes)
through a mixin appearance, exactly as the chip does.

No `CssStyle` is used anywhere in the module; the chip's applied-guide `CssClass` variant is discussed
in `docs/StatusChip.md`.
