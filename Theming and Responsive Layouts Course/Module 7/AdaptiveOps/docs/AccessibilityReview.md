# Accessibility review — Adaptive Operations Console (Module 7 capstone)

Done by operating the running console, not by reading the theme: Tab through every region, trigger a
rejection, hover every icon-only button, measure touch targets at phone width. Every finding below maps
to a control or a theme entry in this project, and the runtime governance review (rules G3, G9, G10, G11)
re-checks the mechanical parts on every run.

## 1. Focus frames — restyled, never removed

| Where | Mechanism | Value |
|---|---|---|
| Every button (`button`, `action-button`, `destructive-button`, `nav-item`) | theme `focused` state: `shadowSpreadRadius: $focusBorderSize`, `shadowColor: focusShadow`, border `color: focusFrame` (white on the filled action button) | `settings.focusBorderSize = 3`, `focusBlurRadius = 0`, `focusFrame = #2454A6`, `focusShadow = #9DB8EA` |
| Text editors (`textbox`, `compact-editor`) | theme `focused` state: border `color: focusFrame` + the same spread shadow | as above |
| Trace list (`trace-list` inherits `list`) | inherited `focused` state from the base `list` appearance | as above |

Rule: the focus frame is drawn from the `focusFrame` / `focusShadow` tokens and the `focusBorderSize`
setting. Changing its look means editing those tokens in `Themes/src/AdaptiveOps.overrides.json`; deleting
the `focused` state is a governance failure (rule G3 checks `button` and `action-button` still carry it).

## 2. Invalid state — text and icon, not colour alone

- The failing editor gets `Invalid = true` and `InvalidMessage = <server message>` (`DetailsEditor.ShowValidationError`).
  The theme `invalid` state on `textbox` draws a 2 px `danger` border on a `dangerSurface` background and
  the framework shows the message as the invalid tooltip.
- The footer label `lblValidation` (`validation-label` appearance, `AccessibleName = "Validation status"`)
  announces the same message as text with the `icon-error` image and the theme state `error`.
- The workspace banner (`banner-danger` appearance) repeats the message with `icon-error`, and the status
  label switches to the `error` state with the text "● validation error".
- Recovery: selecting another ticket or saving a valid one calls `ClearValidation()`, which resets all three.

Rule G11 checks the validation label exists and carries an accessible name.

## 3. Disabled state

`button.disabled` and `action-button.disabled` set `opacity: 0.55` and `cursor: default`; the framework
also removes the control from the tab sequence. Seen on **Review all profiles** while the Timer runs and on
**Save** while no ticket is selected (`DetailsEditor.Clear()` disables it).

## 4. Contrast (WCAG 2.1 ratios of the token pairs the theme actually uses)

Computed from the token values in `Themes/src/AdaptiveOps.overrides.json` (relative luminance formula);
AA normal text needs 4.5:1, AA large text / UI components 3:1.

| Foreground | Background | Ratio | Used by | Verdict |
|---|---|---|---|---|
| `textMain #222222` | `surface #FFFFFF` | 15.91 | card text, editor text, grid cells | AAA |
| `textMain #222222` | `surfaceAlt #F6F7FB` | 14.86 | page text on the page background, table headers | AAA |
| `textMain #222222` | `surfaceRail #EEF2F7` | 14.15 | rail items | AAA |
| `textMain #222222` | `surfaceHover #E3EBF7` | 13.25 | hovered buttons and rail items | AAA |
| `textMuted #5B647A` | `surface #FFFFFF` | 5.92 | captions, subtitles, footer text | AA |
| `textMuted #5B647A` | `surfaceAlt #F6F7FB` | 5.53 | table header text (`table-header-cell`) | AA |
| `textMuted #5B647A` | `surfaceRail #EEF2F7` | 5.26 | "NAVIGATION" caption on the rail | AA |
| `white` | `brandPrimary #2454A6` | 7.26 | `action-button` text, selected `nav-item` | AAA |
| `white` | `brandPrimaryHover #1F4A92` | 8.56 | hovered action button | AAA |
| `white` | `brandPrimaryPressed #183B75` | 10.93 | pressed action button | AAA |
| `white` | `danger #B42318` | 6.57 | `destructive-button` text | AA (AAA large) |
| `danger #B42318` | `dangerSurface #FDECEA` | 5.75 | banner text, invalid editor text | AA |
| `danger #B42318` | `surface #FFFFFF` | 6.57 | status "error", validation label | AA |
| `warning #B54708` | `surface #FFFFFF` | 5.43 | status "warn", stale card border | AA |
| `success #027A48` | `surface #FFFFFF` | 5.41 | status "ok" | AA |
| `focusFrame #2454A6` | `surface #FFFFFF` | 7.26 | focus border on white cards | passes 3:1 for UI components |
| `focusFrame #2454A6` | `surfaceAlt #F6F7FB` | 6.78 | focus border on the page background | passes 3:1 |

Two deliberate non-text exceptions: `focusShadow #9DB8EA` on white is 2.00 — it is the soft halo *around*
the 3 px `focusFrame` border, not the indicator itself; `borderSoft #D7DCE5` on white is 1.38 — a decorative
card border, never the only boundary (cards also carry a shadow and the page background differs).

`textMuted` was darkened from the pack's `#677085` (4.7:1 on `surfaceAlt`) to `#5B647A` (5.5:1) so the
uppercase 11 px card titles clear AA with margin.

## 5. Touch targets and spacing (phone width, 390 × 844)

| Control | Size on Phone | Note |
|---|---|---|
| Toolbar commands (icon-only) | 40 × 34, 8 px gap | 34 px is the theme button height; the 8 px flow margin keeps neighbours apart |
| Rail items (icon-only) | 52 × 40 | `nav-item.height = 40`, rail padding 4 |
| Metric cards | 140 × 84, 2 per row | not interactive; tooltip carries the title |
| Grid rows | 32 px (`table.rowHeight`) | full-row select, 3 columns |
| **Open details** | 128 × 32 | the primary command on phone, right of the grid title |
| Editor fields in the dialog | 28 px (`compact-editor`) in 34 px rows | the dialog is maximized on phone; Notes fills the rest |
| Save (dialog) | 96 × 36 | action-button |

Nothing is smaller than 32 px in the direction a finger taps, and nothing sits closer than 8 px to another target.

## 6. Icon-only buttons — discoverable

Every button that can become icon-only carries both `ToolTipText` and `AccessibleName` in the Designer:
the six toolbar commands (`MainPage.Designer.cs`, `InitToolbarButton`), the five rail items
(`NavigationRail.Designer.cs`, `InitNavButton`) and the phone-only **Open details**. `Button.Display = Icon`
is what the profile switches; the labels stay. Rule G9 fails when any icon-only button lacks either.

## 7. Tab order — walked with the keyboard

Region containers: toolbar 0 → navigation 1 → workspace 2 → details 3 → status 4. Inside each:

1. Toolbar: Governance review (1) → Review all profiles (2) → Inject violation (3) → Recover (4) → Bulk fill (5) → Clear trace (6)
2. Rail: Dashboard (1) → Tickets (2) → Reports (3) → Settings (4) → Help (5)
3. Workspace: Open details (1, phone only) → ticket grid (2) → trace list (3)
4. Details editor: Title (1) → Priority (2) → Status (3) → Owner (4) → Due (5) → Notes (6) → **Save** (7)

The primary command (Save) is the last stop of the editor, where a form user expects it. Labels, cards
and region panels are `TabStop = false`. Rule G10 fails on a focusable command with `TabIndex 0` or a
duplicate index in one container.

## 8. Hidden content has a story

| Hidden on | Control | Alternative |
|---|---|---|
| Phone, Phone (Landscape) | details region | **Open details** in the grid header → `DetailsDialog` (maximized) |
| Phone | app title label | the browser tab title "Adaptive Operations Console" |
| Phone | Priority, Due grid columns | visible in the editor dialog |
| Narrow profiles | Owner column | visible in the editor |
| Narrow profiles | rail captions ("NAVIGATION", item texts) | icons + tooltips + accessible names |

## Evidence

- **Startup**: the trace shows `governance review at startup: 14/14 rules pass`; the status label reads
  "● ready · governance green" in the `ok` state.
- **Tab walk**: press Tab from the page start; the focus frame (3 px blue border + halo) moves through the
  order in section 7; on **Governance review** the frame is white-on-blue.
- **Rejection**: select a ticket, blank the title, press Save (or use the dialog on phone) — the title
  editor turns red-bordered with a tooltip, the footer label reads "Rejected: Title is required." with the
  error icon, the banner appears, the status label turns to "● validation error".
- **Icon-only**: switch the browser to a phone or portrait-tablet emulation — every toolbar and rail button
  shows only its icon; hovering shows the tooltip; **Governance review** still reports rule G9 green.
- Screenshots taken by the learner: `a11y-focus-desktop.png`, `a11y-invalid-state.png`, `a11y-icon-only-phone.png`.
