# Accessibility review: Adaptive Operations Console

Done by operating the running console: Tab through every region, trigger a rejection, hover every
icon-only button, measure touch targets at phone width.

## 1. Focus frames: restyled, never removed

| Where | Mechanism | Value |
|---|---|---|
| Buttons (`button`, `action-button`, `nav-item`) | theme `focused` state: spread shadow `focusShadow`, border `focusFrame` (white on the filled action button) | `focusBorderSize = 3`, `focusBlurRadius = 0`, `focusFrame = #2454A6`, `focusShadow = #9DB8EA` |
| Text editors (`textbox`, `compact-editor`) | theme `focused` state: border `focusFrame` and the same shadow | as above |

Changing the look means editing those tokens in `Themes/src/AdaptiveOps.overrides.json`, never deleting the `focused` state.

## 2. Invalid state: text and icon, not colour alone

- The failing editor gets `Invalid = true` and `InvalidMessage = <server message>`
  (`DetailsEditor.ShowValidationError`). The theme `invalid` state draws a 2 px `danger` border on
  `dangerSurface`, and the framework shows the message as a tooltip.
- The footer label `lblValidation` (`validation-label`, `AccessibleName = "Validation status"`) shows the
  same message as text with the `icon-error` image and the theme state `error`.
- The status bar reads "Not saved: <message>".
- Selecting another ticket or saving a valid one calls `ClearValidation()`.

## 3. Disabled state

`button.disabled` and `action-button.disabled` set `opacity: 0.55`. **Save** is disabled while no ticket is
selected (`DetailsEditor.Clear()`).

## 4. Contrast (WCAG 2.1 ratios of the token pairs in use)

AA normal text needs 4.5:1; UI components need 3:1.

| Foreground | Background | Ratio | Used by | Verdict |
|---|---|---|---|---|
| `textMain #222222` | `surface #FFFFFF` | 15.91 | card, editor and grid text | AAA |
| `textMain #222222` | `surfaceAlt #F6F7FB` | 14.86 | page text, table headers | AAA |
| `textMain #222222` | `surfaceRail #EEF2F7` | 14.15 | rail items | AAA |
| `textMain #222222` | `surfaceHover #E3EBF7` | 13.25 | hovered buttons and rail items | AAA |
| `textMuted #5B647A` | `surface #FFFFFF` | 5.92 | captions, subtitles | AA |
| `textMuted #5B647A` | `surfaceAlt #F6F7FB` | 5.53 | table header text | AA |
| `textMuted #5B647A` | `surfaceRail #EEF2F7` | 5.26 | "NAVIGATION" caption | AA |
| `white` | `brandPrimary #2454A6` | 7.26 | `action-button` text, selected `nav-item` | AAA |
| `white` | `brandPrimaryHover #1F4A92` | 8.56 | hovered action button | AAA |
| `white` | `brandPrimaryPressed #183B75` | 10.93 | pressed action button | AAA |
| `danger #B42318` | `dangerSurface #FDECEA` | 5.75 | invalid editor text | AA |
| `danger #B42318` | `surface #FFFFFF` | 6.57 | validation label | AA |
| `focusFrame #2454A6` | `surface #FFFFFF` | 7.26 | focus border on white cards | passes 3:1 |
| `focusFrame #2454A6` | `surfaceAlt #F6F7FB` | 6.78 | focus border on the page background | passes 3:1 |

Two deliberate non-text exceptions: `focusShadow` on white (2.00) is the halo around the 3 px border, not
the indicator itself; `borderSoft #D7DCE5` on white (1.38) is a decorative card border.

## 5. Touch targets and spacing (phone, 390 × 844)

| Control | Size on Phone |
|---|---|
| Toolbar Refresh (icon-only) | 40 × 34 |
| Rail items (icon-only) | 52 × 40 |
| Grid rows | 32 px, full-row select |
| **Open details** | 128 × 32 |
| Editor fields in the dialog | 28 px in 34 px rows |
| Save (dialog) | 96 × 36 |

Nothing is smaller than 32 px in the tap direction, and targets sit at least 8 px apart.

## 6. Icon-only buttons

Every button that can become icon-only carries `ToolTipText` and `AccessibleName`: the toolbar Refresh
(`InitToolbarButton`), the five rail items (`InitNavButton`) and **Open details**. The profile switches
`Button.Display`; the labels stay.

## 7. Tab order

Regions: toolbar 0, navigation 1, workspace 2, details 3, status 4. Inside each:

1. Toolbar: Refresh (1)
2. Rail: Dashboard (1), Tickets (2), Reports (3), Settings (4), Help (5)
3. Workspace: Open details (1, phone only), ticket grid (2)
4. Details editor: Title (1), Priority (2), Status (3), Owner (4), Due (5), Notes (6), **Save** (7)

Labels, cards and region panels are `TabStop = false`.

## 8. Hidden content has an alternative

| Hidden on | Control | Alternative |
|---|---|---|
| Phone, Phone (Landscape) | details region | **Open details** opens `DetailsDialog` |
| Phone | Priority and Due columns | visible in the editor dialog |
| Narrow profiles | Owner column | visible in the editor |
| Narrow profiles | rail captions | icons, tooltips and accessible names |

Screenshots taken by the learner: `a11y-focus-desktop.png`, `a11y-invalid-state.png`, `a11y-icon-only-phone.png`.
