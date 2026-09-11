# ThemePreviewScreenshots · the Theme Builder preview and browser screenshots

The lab asks for "Theme Builder preview screenshots and a note on state order and inheritance decisions". The note is
`ThemeNotes.md` (§5 and §6). The screenshots are **taken by the learner** while working through
`ThemeBuilderSteps.md`; this file lists each one, its file name and what it must show, so the browser can be compared
against the preview point by point.

| File | Taken in | Must show |
|---|---|---|
| `shots/01-base-bootstrap4.png` | browser, `Default.json` set to `Bootstrap-4` (or Module 1) | the console on the base theme: blue caption bars, grey buttons, 4 px radius, status label `Theme "AdaptiveOps" was not found …` or the Module 1 shell (the *before*) |
| `shots/02-tb-new-from-theme.png` | Theme Builder | *New from theme…* with Bootstrap-4 selected and the name `AdaptiveOps` |
| `shots/03-tb-colors-tokens.png` | Theme Builder, tree + property grid | the `colors` node with the ten named colours (Live update on, preview already recoloured) |
| `shots/04-tb-fonts.png` | Theme Builder | `fonts` › `default`, `heading` (18 bold), `mono` (Consolas 12) as `{ size, family, bold }` objects |
| `shots/05-tb-last-clicked-captionbar.png` | Theme Builder, preview | a panel caption bar clicked; Last Clicked reads `panel/captionbar`; background `brandPrimary` |
| `shots/06-tb-last-clicked-header.png` | Theme Builder, preview | a grid column header clicked; Last Clicked reads `table-header-cell`; `surfaceAlt` background, `textMuted` text |
| `shots/07-tb-tab.png` | Theme Builder, preview | `tabview/page/button`: an unselected tab next to the selected one (`surface`, `brandPrimary`) |
| `shots/08-tb-textbox-invalid.png` | Theme Builder, tree | `textbox` › states with `invalid` as the **last** state, `color: danger` |
| `shots/09-tb-tooltip.png` | Theme Builder, preview | a tooltip open: `tooltip/atom` on `textMain` with `surface` text |
| `shots/10-tb-action-button.png` | Theme Builder, tree + property grid | `action-button` with `inherit = button` and three states in the order **default › hovered › pressed** |
| `shots/11-tb-action-button-states.png` | Theme Builder, preview | the button default / hovered / pressed / focused + disabled; the last frame proves focused and disabled come from `button` |
| `shots/12-tb-state-order-swapped.png` | Theme Builder | `pressed` dragged above `hovered`, mouse held: the *hovered* colour shows; drag back before saving |
| `shots/13-browser-adaptiveops.png` | browser | the console on AdaptiveOps: status label `Theme: AdaptiveOps`, Save in `brandPrimary`, caption bar, metric strips (the *after*) |
| `shots/14-browser-primary-states.png` | browser | Save default / hovered / pressed / focused (Tab), compared with 11 |
| `shots/15-browser-invalid-editor.png` | browser, Save with an empty Title | the Title editor with the `danger` border and the error tooltip |
| `shots/16-browser-tooltip-header.png` | browser | the Save tooltip open and the grid header visible in the same frame |
| `shots/17-browser-missing-theme.png` | browser, `Default.json` naming a theme with no file | the status message the lab asks for when the theme name matches no file |

Acceptance: the six restyled appearances (button, panel, tab, grid header, invalid editor, tooltip) look the same in
the preview frames and the browser frames, and no frame contains a control whose look was set in code.
