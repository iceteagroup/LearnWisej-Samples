# AI grounding pack: Adaptive Operations Console

Written for the next developer and for a grounded assistant that answers implementation questions about
this console from this material first, and from the official documentation in the source map second.

## 1. Decision tree (apply in order, stop at the first rung that fits)

1. **Theme**: identity of standard controls (colours, fonts, metrics, states), in `Themes/src/AdaptiveOps.overrides.json`
2. **AppearanceKey**: a semantic variant of a standard control, as an appearance that `inherit`s the base one
3. **CssClass**: scoped, app-specific CSS or a third-party widget, in `Styles/AdaptiveOps.css`, inherited properties only
4. **CssStyle**: one computed, one-off value, documented in `docs/ArchitectureNote.md` layer 4
5. **Layout containers**: Dock/Anchor for the shell, Flow for wrapping, Table for aligned forms, Flex for proportional regions
6. **Responsive properties**: per-profile values (`Visible`, `Dock`, `Size`, `Display`, column visibility); in this project, `ApplyProfile`
7. **ResponsiveProfileChanged**: behaviour a value cannot express (the phone editor dialog)
8. **ClientProfiles.json**: the profile definitions, narrow to broad
9. **Production governance**: checklist, QA matrix, source control

The tell that a rung was skipped: `BackColor`, `ForeColor` or `Font` on a standard control, a `CssStyle`
that is not dynamic, a handler that sets `Bounds`, a CSS selector into a widget's internal DOM.

## 2. Assistant answer rules

1. Answer from this pack and `docs/ArchitectureNote.md` first, then from the source map.
2. Prefer theme appearances for standard control identity, `AppearanceKey` for variants, `CssClass` for scoped app CSS, and `CssStyle` only for local dynamic values.
3. Never propose CSS media queries as the primary responsive mechanism when responsive properties or a `ClientProfile` fit.
4. Warn about brittle selectors that depend on internal widget DOM; allow them only for custom or third-party widgets, tightly scoped and documented.
5. Warn that `Application.LoadTheme(name)` and mutating a loaded theme change **every session**; a session-only change is `Application.Theme = new ClientTheme("name", Application.Theme)` with the edits on the copy.
6. Warn that direct `BackColor`, `ForeColor`, `Font` or inline styles override the theme and break branding.
7. When discussing `ClientProfiles.json`: first-match order, the `device`, `minWidth`, `maxWidth` and `landscape` fields, `Application.ActiveProfile`, and a refresh after changing the emulated device.
8. When a responsive problem is ambiguous, ask for the profile name and the screen size and orientation before answering.
9. A profile handler must be idempotent: return early for the same profile, assign final values, never toggle, never reload data, unsubscribe in `Dispose`.
10. A focus frame is restyled from the `focusFrame` token, never removed; invalid states pair colour with an icon or a message.

## 3. Source map

| Topic | Page |
|---|---|
| Theme system, create from a base theme, colors, appearances, states, properties | https://docs.wisej.com/theme-builder/ and its sub-pages |
| Startup theme and runtime theme objects | https://docs.wisej.com/docs/concepts/theming/ |
| Layouts: docking, anchoring, Flow, Table, Flex, margins | https://docs.wisej.com/docs/concepts/layouts/ |
| Common properties: AppearanceKey, Dock, Anchor, States, CssClass, CssStyle, TabIndex, ToolTipText | https://docs.wisej.com/docs/controls/general/common-properties/ |
| Responsive properties | https://docs.wisej.com/docs/controls/general/responsive-properties/ |
| Client profiles, ActiveProfile, ResponsiveProfileChanged | https://docs.wisej.com/docs/concepts/client-profiles/ |
| ClientProfile API | https://docs.wisej.com/api/wisej.web/general/application/wisej.core.clientprofile |
| FlowLayoutPanel, TableLayoutPanel, FlexLayoutPanel API | https://docs.wisej.com/api/wisej.web/containers/flowlayoutpanel/ and its siblings |
| AutoSizing, AutoScroll | https://docs.wisej.com/docs/controls/general/autosizing/ · https://docs.wisej.com/docs/controls/general/autoscroll/ |

Project-local sources: `docs/ArchitectureNote.md`, `docs/ProductionChecklist.md`,
`docs/AccessibilityReview.md`, `docs/ResponsiveQAMatrix.md`.

## 4. Canonical Q&A for this console

**Q: Why is the details region a modal on phone instead of a docked panel?**
A: On a 390 px phone the docked editor would push the grid below its `MinimumSize`. Hiding the region is a
property value (`detailsPanel.Visible = false`, rung 6), but presenting the editor another way is
behaviour (rung 7): **Open details** shows the same `DetailsEditor` in a maximized `DetailsDialog`.

**Q: Why is the metric card styled by both an appearance and a `CssClass`?**
A: Surface, border, shadow, fonts and colours are the `metric-card`, `metric-title` and `metric-value`
appearances, because they are theme properties. Uppercase, letter-spacing and tabular numerals are CSS,
because they are not theme properties and the card is app-specific content.

**Q: How do I make one button look like a delete action?**
A: `AppearanceKey = "destructive-button"`; the appearance inherits `action-button` and overrides the
background with the `danger` token. Do not set `BackColor`.

**Q: My grid header tweak only works with a selector into the widget's DOM.**
A: First try the `table-header-cell` appearance. If a selector is unavoidable, it must target a custom or
third-party widget, be scoped to a `CssClass` you own, and be written down in `docs/ArchitectureNote.md`
layer 3 with the reason and the upgrade risk.

**Q: Why did my theme not apply?**
A: Check, in order: a `BackColor`, `ForeColor`, `Font` or `CssStyle` on the control, the `AppearanceKey`
name, the state order in the appearance, and that `Default.json` selects `AdaptiveOps`.

**Q: The handler ran twice for `Tablet` after a refresh. What protected the UI?**
A: `if (name == _lastProfile) return;` at the top of `ApplyProfile`, plus final-value assignments
(`Visible = !phone`, not `Visible = !Visible`).

## 5. One question the assistant must answer from this material

> "At 1024 × 768 in landscape on a tablet, which profile is active, where is the details editor, and
> which mechanism decided that?"

Expected answer: `Tablet (Landscape)`. It is the fourth entry of `ClientProfiles.json` (`device: Tablet`,
`landscape: true`) and matches before `Small Desktop` because it is listed first (rung 8). The details
editor is docked Right at 300 px with the compact editor, because `ApplyProfile("Tablet (Landscape)")`
assigns those values (rung 6). Only portrait `Tablet` docks it Bottom; the dialog is phone-only (rung 7).

## 6. RAG chunk boundaries

Each of these stands alone as a retrievable chunk: the decision tree (§1), each answer rule (§2), each
source-map row (§3), each Q&A (§4), the per-profile table in `ArchitectureNote.md` layer 6, each checklist
row in `ProductionChecklist.md`, and each row of `ResponsiveQAMatrix.md`.
