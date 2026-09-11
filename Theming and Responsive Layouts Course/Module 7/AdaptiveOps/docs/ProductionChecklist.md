# Production checklist: Adaptive Operations Console

Adapted from the course pack's checklists (production governance, accessibility, layout, client profile,
CSS usage, theme builder, responsive properties, AI grounding), with the evidence in this repository.

## A. Theme

| Rule | Evidence |
|---|---|
| The custom theme is active, not a built-in theme edited in place | `Default.json` `"theme": "AdaptiveOps"`; `Themes/src/merge-theme.mjs` regenerates the merged file |
| Named tokens for brand, surfaces, text, danger, warning, success, focus | `colors` in `Themes/src/AdaptiveOps.overrides.json` |
| Focus frame restyled from the `focusFrame` token, never removed | `settings.focusBorderSize = 3`; `button` and `action-button` keep a `focused` state |
| At least four semantic appearances assigned through `AppearanceKey` | `action-button`, `nav-item`, `surface-card`, `metric-card`, `rail-surface`, `compact-editor`, label appearances |
| Fonts are tokens, referenced by name | `fonts` in the overrides; no `new Font(...)` in code |
| Default, hovered, pressed, focused, disabled and invalid states verified | `docs/AccessibilityReview.md` §1–3 |

## B. Colours, fonts and CSS discipline

| Rule | Evidence |
|---|---|
| No hard-coded `BackColor`, `ForeColor` or `Font` | none in the Designer files |
| `CssStyle` only for documented one-off dynamic values | none used |
| Every `CssClass` is defined in the scoped stylesheet | `metric-card`, `metric-title`, `metric-value` in `Styles/AdaptiveOps.css` |
| No CSS against internal widget DOM | two class rules, inherited text properties only |

## C. Layout

| Rule | Evidence |
|---|---|
| `MinimumSize` on fill-weighted and docked regions, `MaximumSize` on the details region, no `AutoSize` containers | workspace 320×240, gridCard 280×200, gridTickets 240×160, detailsPanel Min 280 / Max 420 (Min 240 / Max 360 when docked Bottom) |
| Containers over resize handlers; docking order checked | `MainPage.Designer.cs`; `ArchitectureNote.md` layer 5 |
| Flow for wrapping, Table for forms, Flex for proportional regions | toolbar and metric cards (Flow), editor (Table), grid region (Flex) |

## D. Client profiles and responsive properties

| Rule | Evidence |
|---|---|
| `ClientProfiles.json` shipped with the app, narrow to broad | copied to the output folder; order Phone to Desktop |
| Profile handler idempotent and cheap | `ApplyProfile` early return, final values, one `SuspendLayout/ResumeLayout` pair |
| A failure inside the handler reports friendly and leaves no half-applied layout | `ApplyProfile` catch: status message, then re-apply the last good profile |
| Profile name shown in test builds only | `lblProfile.Visible = TestMode` (`ADAPTIVEOPS_TESTMODE`) |
| Hidden controls and mobile alternatives documented | `AccessibilityReview.md` §8 |

## E. Accessibility

| Rule | Evidence |
|---|---|
| Icon-only buttons carry `ToolTipText` and `AccessibleName` | Refresh, the five rail items, Open details |
| Keyboard order declared | `AccessibilityReview.md` §7 |
| Invalid state announced as text and icon | `Invalid`/`InvalidMessage` and `lblValidation` with `icon-error` |
| Contrast of the token pairs in use at least 4.5:1 for text | `AccessibilityReview.md` §4 |
| Touch targets at least 32 px with 8 px spacing on phone | `AccessibilityReview.md` §5 |

## F. Source control and upgrades

| Rule | Evidence |
|---|---|
| Theme, stylesheet, profiles and review docs live in the project | files present |
| Theme and CSS changes reviewed as diffs | edit `Themes/src/AdaptiveOps.overrides.json`, regenerate `AdaptiveOps.theme`, commit both |
| Responsive matrix rerun | `docs/ResponsiveQAMatrix.md` after every theme, CSS or profile change and every upgrade |
| Upgrade test includes theme and responsive pages | run the app, rerun the matrix, inspect each custom appearance and the `invalid`, `selected` and `stale` states |

## G. AI grounding

| Rule | Evidence |
|---|---|
| Answers prefer official Wisej.NET mechanisms; decision rules and source map bundled; brittle CSS and global theme changes flagged | `docs/GroundingPack.md` |
