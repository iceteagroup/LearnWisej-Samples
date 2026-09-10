# Production checklist — Adaptive Operations Console

Adapted from the course pack's checklists (production governance, accessibility visual, layout, client
profile, CSS usage, theme builder, responsive properties, AI grounding) to this application. The **G1–G14**
rules are the ones `Governance/GovernanceReview.cs` computes at runtime and the **Governance review**
dialog shows; the remaining items are process rules with their evidence in this repository.

## A. Theme (`Themes/AdaptiveOps.theme`)

| # | Rule | Evidence |
|---|---|---|
| G1 | The custom theme is active, not a built-in theme edited in place | `Application.Theme.Name == "AdaptiveOps"`; `Default.json` `"theme": "AdaptiveOps"`, `Web.config` `Wisej.DefaultTheme`; the base theme is never edited — `Themes/src/merge-theme.mjs` regenerates the merged file |
| G2 | Named tokens for brand, surfaces, text, danger, warning, success, focus | `Application.Theme.Colors` contains `brandPrimary surface surfaceAlt textMain textMuted danger warning success focusFrame` |
| G3 | Focus frame restyled from the `focusFrame` token, never removed | `settings.focusBorderSize = 3`; `button` and `action-button` keep a `focused` state |
| G4 | At least four semantic appearances assigned through `AppearanceKey` | 20 keys in use at desktop width (list in the review evidence) |
| — | Fonts are tokens (`{size, family[], bold}`), referenced by name | `fonts.default/defaultBold/heading/subheading/cardTitle/cardValue/mono`; no `new Font(...)` in code |
| — | Default, hovered, pressed, focused, disabled and invalid states verified | see `docs/AccessibilityReview.md` §1–3; Theme Builder: open `Themes/AdaptiveOps.theme`, select `action-button`, step through the states |
| — | Custom appearances and child components documented | `docs/ArchitectureNote.md` layer 2 table |

## B. Colours, fonts and CSS discipline

| # | Rule | Evidence |
|---|---|---|
| G5 | No hard-coded `BackColor` / `ForeColor` / `Font` on any control | reflection over `Control.ShouldSerializeBackColor/ForeColor/Font` across the whole tree: 0 explicit values; the documented exception list is empty |
| G6 | `CssStyle` only for documented one-off dynamic values | 0 uses; allow-list empty |
| G7 | Every `CssClass` is defined in the scoped stylesheet | classes in use ⊆ classes defined in `Styles/AdaptiveOps.css` |
| — | No global CSS against standard controls through internal-DOM selectors | the stylesheet has five class rules using inherited text properties only; no descendant selectors into widget markup |
| — | Third-party widget CSS kept scoped | no third-party widgets in this app |

## C. Layout

| # | Rule | Evidence |
|---|---|---|
| G12 | `MinimumSize` on fill-weighted and docked regions, `MaximumSize` on the details region, no `AutoSize` on container hierarchies | workspace 320×240, gridCard 280×200, gridTickets 240×160, traceCard 200×120, detailsPanel Min 280 / Max 420 (Min 240 / Max 360 tall when docked Bottom); 0 AutoSize containers |
| — | Containers over resize handlers; docking order checked; Padding on docked regions | `MainPage.Designer.cs` comments on child order; `ArchitectureNote.md` layer 5; no `Bounds`/`Location` assignments in handlers |
| — | Flow for wrapping, Table for forms, Flex for proportional regions | toolbar + metric cards (Flow), editor + cost bar + reports (Table), grid/trace (Flex) |

## D. Client profiles and responsive properties

| # | Rule | Evidence |
|---|---|---|
| G8 | `ClientProfiles.json` shipped with the app, narrow to broad, active profile recognised | copied to the output folder for both targets; order Phone → … → Desktop |
| G13 | Profile handler idempotent and cheap | `ApplyProfile` re-run under the review: identical snapshot; duration traced (a few ms) |
| — | Responsive values are final values, not toggles; `ResponsiveProfileChanged` only for real behaviour | `ApplyProfile` assigns; the dialog is the only behaviour change |
| — | Profile name shown in test builds only | `lblProfile.Visible = TestMode` (`ADAPTIVEOPS_TESTMODE`, on in `launchSettings.json`) |
| — | Hidden controls and mobile alternatives documented | `AccessibilityReview.md` §8 |

## E. Accessibility

| # | Rule | Evidence |
|---|---|---|
| G9 | Icon-only buttons carry `ToolTipText` and `AccessibleName` | all 12 buttons that can become icon-only |
| G10 | Keyboard order declared: unique `TabIndex` per container on every focusable command | `AccessibilityReview.md` §7 |
| G11 | Invalid state announced as text and icon, not colour alone | `lblValidation` + `Invalid`/`InvalidMessage` + banner |
| — | Contrast of the token pairs in use ≥ 4.5:1 for text | `AccessibilityReview.md` §4 table |
| — | Touch targets ≥ 32 px with ≥ 8 px spacing at phone width | `AccessibilityReview.md` §5 |

## F. Source control and upgrades

| # | Rule | Evidence |
|---|---|---|
| G14 | Theme, stylesheet, profiles and review docs live in the project; `Default.json` selects the custom theme | files present; `Default.json` theme = AdaptiveOps |
| — | Theme and CSS changes reviewed as diffs | edit `Themes/src/AdaptiveOps.overrides.json` (small, readable), regenerate `AdaptiveOps.theme`, commit both; review the overrides diff |
| — | Responsive screenshot matrix complete and rerun | `docs/ResponsiveQAMatrix.md`, six rows, rerun after every theme/CSS/profile change and every Wisej.NET upgrade |
| — | Upgrade test includes theme and responsive pages | after an upgrade: run the app, press **Governance review** (must be 14/14), press **Review all profiles** (every profile 14/14), rerun the matrix, inspect each custom appearance key and the `invalid`/`selected`/`stale`/`ok`/`warn`/`error` states |
| — | Rules for developers: when `BackColor`/`ForeColor`/`Font` may be used | never on shipped controls; add the control name to `GovernanceContext.ExplicitValueAllowList` with a comment in this file if a documented exception is ever needed |

## G. AI grounding

| Rule | Evidence |
|---|---|
| Course answers prefer official Wisej.NET mechanisms; decision rules in the pack; source map bundled; assistant warns about brittle CSS and global theme changes; ambiguous responsive issues trigger a question | `docs/GroundingPack.md` |

## Evidence in the running app

- **Governance review** (success): dialog "All 14 rules pass — the console is release-ready on the Desktop
  profile."; every rule line also lands in the trace.
- **Review all profiles** (progress): four trace lines `review k/4 · <profile>: 14/14 rules (all green) · rail … · details … · cards … · grid …`,
  then `back on Desktop`.
- **Inject violation** (failure): the dialog lists `G1` (theme is Bootstrap-4), `G2` (tokens missing),
  `G5` (`MetricCard cardOpen (BackColor)`), `G6` (`cardOverdue = "border-color: #F59E0B"`), `G7`
  (`legacy-card on MetricCard cardMine`) as ✕ FAIL; the status label is in the `error` state. Running
  **Review all profiles** now also shows `ApplyProfile(Phone) FAILED half-way … re-applying Desktop` and the
  banner "Profile change to Phone failed …", after which the run continues.
- **Recover**: `recovered: ResetBackColor(), CssStyle cleared, CssClass restored, AdaptiveOps theme reloaded …`
  and the dialog is green again.
- Note: `Application.LoadTheme` swaps the theme for **every** session of the running app; the failure and
  recovery paths say so in the trace.
