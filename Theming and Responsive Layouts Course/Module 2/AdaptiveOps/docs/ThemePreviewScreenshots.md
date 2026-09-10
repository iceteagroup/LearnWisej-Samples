# Deliverable · ThemePreviewScreenshots — the Theme Builder preview and browser screenshots

The lab asks for "Theme Builder preview screenshots and a note on state order and inheritance decisions". The note is
`ThemeNotes.md` (§5 and §6). The screenshots are **taken by the learner** while working through
`ThemeBuilderSteps.md`; this file lists each one, its file name and what it must show, so the browser can be compared
against the preview point by point (lab step 10).

| File | Taken in | Must show |
|---|---|---|
| `shots/01-base-bootstrap4.png` | browser, `Base theme ⇄` (or Module 1) | the console on Bootstrap-4: `#007AFF` caption bars, grey buttons with `#D1E0E5` hover, 4 px radius, status label `theme: Bootstrap-4` — the *before* |
| `shots/02-tb-new-from-theme.png` | Theme Builder | *New from theme…* dialog with Bootstrap-4 selected and the name `AdaptiveOps` |
| `shots/03-tb-colors-tokens.png` | Theme Builder, tree + property grid | the `colors` node with `brandPrimary #2454A6`, `brandAccent #F59E0B`, `surface`, `surfaceAlt`, `textMain`, `textMuted`, `danger`, `warning`, `success`, `focusFrame` (Live update **on**, preview already recoloured) |
| `shots/04-tb-fonts.png` | Theme Builder | `fonts` › `default`, `heading` (18 bold), `mono` (Consolas 12) as `{ size, family, bold }` objects |
| `shots/05-tb-last-clicked-captionbar.png` | Theme Builder, preview | a panel caption bar clicked; Last Clicked reads `panel/captionbar`; the tree is on that node; background `brandPrimary` |
| `shots/06-tb-last-clicked-header.png` | Theme Builder, preview | a grid column header clicked; Last Clicked reads `table-header-cell`; `surfaceAlt` background, `textMuted` text |
| `shots/07-tb-tab.png` | Theme Builder, preview | `tabview/page/button` — an unselected tab (`surfaceAlt`, `textMuted`) next to the selected one (`surface`, `brandPrimary`) |
| `shots/08-tb-textbox-invalid.png` | Theme Builder, tree | `textbox` › states with `invalid` as the **last** state, `color: danger`; the preview editor with the red border |
| `shots/09-tb-tooltip.png` | Theme Builder, preview | a tooltip open: `tooltip/atom` on `textMain` with `surface` text |
| `shots/10-tb-action-button.png` | Theme Builder, tree + property grid | `action-button` with `inherit = button` and exactly three states in the order **default › hovered › pressed** |
| `shots/11-tb-action-button-states.png` | Theme Builder, preview (four frames) | the same preview button default / hovered / pressed / focused+disabled — the last frame proves focused and disabled come from `button` |
| `shots/12-tb-state-order-swapped.png` | Theme Builder | `pressed` dragged above `hovered`, mouse held: the *hovered* colour shows — the "order is behaviour" frame; drag back before saving |
| `shots/13-browser-adaptiveops.png` | browser, after `Apply theme` | the console on AdaptiveOps: status label `theme: AdaptiveOps (Themes/AdaptiveOps.theme · Default.json)`, Save in `brandPrimary`, caption bars, metric strips, muted overlines, monospaced trace — the *after* |
| `shots/14-browser-primary-states.png` | browser | Save default / hovered / pressed / focused (Tab) / disabled — compare with 11 |
| `shots/15-browser-invalid-editor.png` | browser, after `Invalid ticket` | the Title editor with the `danger` border and the error tooltip; banner in `danger`; status `● validation error` |
| `shots/16-browser-tooltip-header.png` | browser | a toolbar tooltip open and the grid header visible in the same frame |
| `shots/17-browser-missing-theme.png` | browser, after `Missing theme` | the banner and trace lines for `Application.LoadTheme("Missing-Theme")` and `GetColor("brandPrimry") = Color.Empty` — the status message the lab asks for when the theme name matches no file |
| `shots/18-browser-token-inspector.png` | browser, Token inspector tab | 18 rows, *Defined as* = *Resolved* for every colour token |

Naming the frames after the appearance path (`captionbar`, `header`, `textbox-invalid`, `action-button`) keeps the
comparison honest: each browser frame has a preview frame with the same subject.

## Evidence

The frames the app can produce on demand (13–18) are driven by the toolbar: `Base theme ⇄` for 01, `Apply theme` for
13/14/18, `Invalid ticket` for 15, hovering any toolbar button for 16, `Missing theme` for 17. Frames 02–12 come from
the Theme Builder session in `ThemeBuilderSteps.md`. Acceptance: the six restyled appearances (button, panel, tab, grid
header, invalid editor, tooltip) look the same in the preview frames and the browser frames, and no frame contains a
control whose look was set in code.
