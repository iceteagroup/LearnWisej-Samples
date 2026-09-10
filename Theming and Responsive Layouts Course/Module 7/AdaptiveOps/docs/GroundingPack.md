# AI grounding pack — Adaptive Operations Console

Written for two readers: the next developer and a grounded assistant that answers implementation
questions about this console from this material first and from the official documentation in the source
map second.

## 1. Decision tree (apply in order, stop at the first rung that fits)

1. **Theme** — identity of standard controls (colours, fonts, metrics, states) → `Themes/src/AdaptiveOps.overrides.json`
2. **AppearanceKey** — a semantic variant of a standard control → add an appearance that `inherit`s the base one
3. **CssClass** — scoped, app-specific CSS or a third-party widget → `Styles/AdaptiveOps.css`, inherited properties only
4. **CssStyle** — one computed, one-off value; must be added to `GovernanceContext.CssStyleAllowList` with a reason
5. **Layout containers** — position and size: Dock/Anchor for the shell, Flow for wrapping, Table for aligned forms, Flex for proportional regions
6. **Responsive properties** — designer-set per-profile values (`Visible`, `Dock`, `Size`, `Display`, column visibility) → in this hand-written project: `ApplyProfile`
7. **ResponsiveProfileChanged** — behaviour a value cannot express (the phone editor dialog)
8. **ClientProfiles.json** — the profile definitions, narrow to broad
9. **Production governance** — review, checklist, QA matrix, source control

The tell that a rung was skipped: `BackColor`/`ForeColor`/`Font` on a standard control, a `CssStyle` that
is not dynamic, a handler that sets `Bounds`, a CSS selector into a widget's internal DOM.

## 2. Assistant answer rules

1. Answer from this pack and `docs/ArchitectureNote.md` first, then from the source map.
2. Prefer theme appearances for standard control identity; `AppearanceKey` for variants; `CssClass` for scoped app CSS; `CssStyle` only for local dynamic values.
3. Never propose CSS media queries as the primary responsive mechanism when responsive properties or a `ClientProfile` fit.
4. Warn about brittle selectors that depend on internal widget DOM; allow them only for custom/third-party widgets, tightly scoped and documented.
5. Warn that `Application.LoadTheme(name)` and mutating `Application.Theme.Colors.x` on a loaded theme change **every session**; a session-only change is `Application.Theme = new ClientTheme("name", Application.Theme)` with the edits on the copy.
6. Warn that direct `BackColor`, `ForeColor`, `Font` or inline styles override the theme and break branding; point to the governance review rule G5.
7. When discussing `ClientProfiles.json`: first-match order, `device` / `minWidth` / `maxWidth` / `landscape` fields, `Application.ActiveProfile`, refresh after changing the emulator device.
8. When a responsive problem is ambiguous, ask for the profile name and the screen size (and orientation) before answering.
9. A profile handler must be idempotent: return early for the same profile, assign final values, never toggle, never reload data, unsubscribe in `Dispose`.
10. A focus frame is restyled from the `focusFrame` token, never removed; invalid states pair colour with an icon or a message.

## 3. Source map (what each rule is grounded on)

| Topic | Page |
|---|---|
| Theme system overview, create from a base theme, elements, colors, appearances, states, styles, properties, stylesheet | https://docs.wisej.com/theme-builder/ and its sub-pages (getting-started/create-new-theme, theme-elements/*, user-interface/*, concepts/tips-and-tricks) |
| Startup theme via Web.config / Default.json, runtime theme objects | https://docs.wisej.com/docs/concepts/theming/ |
| Layouts: docking, anchoring, Flow/Table/Flex, margins | https://docs.wisej.com/docs/concepts/layouts/ |
| Common properties: AppearanceKey, Dock, Anchor, States, CssClass, CssStyle, TabIndex, ToolTipText | https://docs.wisej.com/docs/controls/general/common-properties/ |
| Responsive properties | https://docs.wisej.com/docs/controls/general/responsive-properties/ |
| Client profiles: ClientProfiles.json, matching order, ActiveProfile, ResponsiveProfileChanged | https://docs.wisej.com/docs/concepts/client-profiles/ |
| ClientProfile API | https://docs.wisej.com/api/wisej.web/general/application/wisej.core.clientprofile |
| Application API (ActiveProfile, Theme) | https://docs.wisej.com/api/3.5/wisej.web/general/application/ |
| FlowLayoutPanel / TableLayoutPanel / FlexLayoutPanel API | https://docs.wisej.com/api/wisej.web/containers/flowlayoutpanel/ · …/tablelayoutpanel/ · …/flexlayoutpanel/ |
| AutoSizing, AutoScroll | https://docs.wisej.com/docs/controls/general/autosizing/ · https://docs.wisej.com/docs/controls/general/autoscroll/ |
| Markup extensions (fluent layout) | https://docs.wisej.com/api/wisej.web.markup/extensions/ |

Project-local sources: `docs/ArchitectureNote.md` (every decision and its rung), `docs/ProductionChecklist.md`
(rules G1–G14 with evidence), `docs/AccessibilityReview.md`, `docs/ResponsiveQAMatrix.md`,
`Governance/GovernanceReview.cs` (the executable form of the rules).

## 4. Canonical Q&A for this console

**Q: Why is the details region a modal on phone instead of a docked panel?**
A: On a 390 px phone the docked editor would push the grid below its `MinimumSize`; hiding the region is a
property value (`detailsPanel.Visible = false`, rung 6), but *presenting the editor another way* is
behaviour, so it is the one `ResponsiveProfileChanged`-class decision (rung 7): **Open details** shows the
same `DetailsEditor` in a maximized `DetailsDialog`. The trace logs
`details … opened as a modal DetailsDialog (maximized)`.

**Q: Why is the metric card a `CssClass` and not (only) an appearance?**
A: It is both, on purpose. Surface, border, shadow, fonts and colours are the `metric-card`, `metric-title`
and `metric-value` **appearances** (rung 1–2) because they are theme properties. The uppercase +
letter-spacing treatment of the title and tabular numerals are **CSS** (rung 3) because `text-transform`,
`letter-spacing` and `font-variant-numeric` are not theme properties and the card is app-specific content.

**Q: How do I make one button look like a delete action?**
A: `AppearanceKey = "destructive-button"`; the appearance inherits `action-button` and overrides the
background with the `danger` token. Do not set `BackColor` (rule G5 fails).

**Q: My tweak to the grid header only works with a selector into the widget's DOM.**
A: First try the `table-header-cell` appearance (this theme already restyles it: `surfaceAlt` background,
`cardTitle` font, `textMuted` text). If a selector is unavoidable, it must target a custom/third-party
widget, be scoped to a `CssClass` you own, and be written down in `docs/ArchitectureNote.md` layer 3 with
the reason and the upgrade risk; rerun the QA matrix after the next upgrade.

**Q: Why did my theme not apply?**
A: Check, in order: a `BackColor`/`ForeColor`/`Font`/`CssStyle` on the control (G5/G6), the `AppearanceKey`
name, the state order in the appearance, and that `Default.json` selects `AdaptiveOps` (G1/G14).

**Q: The handler ran twice for `Tablet` after a refresh — what protected the UI?**
A: `if (name == _lastProfile) return;` at the top of `ApplyProfile`, plus final-value assignments
(`Visible = !phone`, not `Visible = !Visible`). Without the guard a toggle would flip the rail on every
duplicate event; with final values even a re-run is harmless (rule G13 re-runs it and compares snapshots).

## 5. One implementation question the assistant must answer from this material

> "At 1024 × 768 in landscape on a tablet, which profile is active, where is the details editor, and
> which mechanism decided that?"

Expected answer: `Tablet (Landscape)` — it is the fourth entry of `ClientProfiles.json` (`device: Tablet`,
`landscape: true`) and matches before `Small Desktop` because it is listed first and the device is a tablet
(rung 8, first match wins). The details editor is **docked Right at 300 px** with the compact editor
appearance, because `ApplyProfile("Tablet (Landscape)")` assigns `detailsPanel.Dock = Right`,
`Width = 300`, `detailsEditor.Compact = true` (rung 6, a per-profile property value). Only portrait
`Tablet` docks it Bottom. No `ResponsiveProfileChanged` behaviour is involved on tablets; the dialog is
phone-only (rung 7).

## 6. RAG chunk boundaries

Each of the following stands alone as a retrievable chunk: the decision tree (§1), each answer rule (§2),
each source-map row (§3), each Q&A (§4), the per-profile value table in `ArchitectureNote.md` layer 6,
each rule row G1–G14 in `ProductionChecklist.md`, and each row of `ResponsiveQAMatrix.md`.
