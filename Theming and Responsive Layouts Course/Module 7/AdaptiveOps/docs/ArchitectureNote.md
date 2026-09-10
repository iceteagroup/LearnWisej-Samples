# Architecture note — the layered visual system as built

The Adaptive Operations Console is one visual system in nine layers. For every visible decision this
note names the rung of the course decision tree that owns it and why the rungs above did not fit. The
runtime **Governance review** (`Governance/GovernanceReview.cs`) enforces the boundaries between the
rungs; `docs/GroundingPack.md` turns this note into answer rules an assistant can cite.

## The decision tree, applied in order

| # | Rung | Owns in this app |
|---|---|---|
| 1 | **Theme** (`Themes/AdaptiveOps.theme`) | identity of every standard control: colours, fonts, radius, focus frame, invalid state, grid header, tooltip, page background |
| 2 | **AppearanceKey** | every business variant: `action-button`, `destructive-button`, `nav-item`, `metric-card`, `surface-card`, `rail-surface`, `banner-danger`, `compact-editor`, `heading/subheading/muted/mono/metric-title/metric-value/status/validation-label`, `trace-list`, `cost-bar-fill/track` |
| 3 | **CssClass** (`Styles/AdaptiveOps.css`) | app-specific typography on app-specific content: uppercase + letter-spacing on card titles, tabular numerals, trace ligatures, review summary emphasis |
| 4 | **CssStyle** | nothing — the allow-list is empty; rule G6 fails on the first use |
| 5 | **Layout containers** | Dock (five regions, workspace stack, cards' inner layout), FlowLayoutPanel (toolbar, metric cards), TableLayoutPanel (editor form, cost bar, reports), FlexLayoutPanel (grid/trace split) |
| 6 | **Responsive properties** | the per-profile values `ApplyProfile` assigns: `Visible`, `Dock`, `Width/Height`, `MinimumSize/MaximumSize`, `Display`, `DataGridViewColumn.Visible`, `FlexLayoutStyle` |
| 7 | **ResponsiveProfileChanged** | the one behaviour a property cannot express: the details editor opens as a modal `DetailsDialog` on phone |
| 8 | **ClientProfiles.json** | the six profile definitions, narrow to broad |
| 9 | **Production governance** | the review dialog, this note, the checklist, the QA matrix, source control |

## Layer 1 — Theme: `Themes/AdaptiveOps.theme`

- **Derived from a base theme.** `Themes/src/AdaptiveOps.overrides.json` is the source we edit: `name`,
  `inherit: "Bootstrap-4"`, `settings`, our `colors`, `fonts` and `appearances`. `Themes/src/merge-theme.mjs`
  deep-merges it over the embedded Bootstrap-4 theme (dumped from `Wisej.Framework.dll`) into the shipped
  `AdaptiveOps.theme`, so the runtime gets a complete theme whether or not it merges the top-level `inherit`
  itself. Never edit the built-in theme; never edit the merged file by hand — regenerate it.
- **Tokens** (`colors`): `brandPrimary/Hover/Pressed`, `brandAccent`, `surface`, `surfaceAlt`, `surfaceRail`,
  `surfaceHover`, `borderSoft`, `textMain`, `textMuted`, `danger`, `dangerHover`, `dangerSurface`, `warning`,
  `success`, `focusFrame`, `focusShadow`, `invalid`. The Bootstrap system names the base appearances reference
  (`primary`, `window`, `windowText`, `windowFrame`, `buttonFace`, `buttonText`, `highlight`, `hotTrack`,
  `table-row-background-*`, `text-placeholder`, `grayText`) are repointed to the same values, so inherited
  appearances we never touch (combobox, datefield, scrollbars, menus) already wear the brand.
- **Fonts**: `default`, `defaultBold`, `heading` 18, `subheading` 14, `cardTitle` 11 bold, `cardValue` 26 bold,
  `mono` 12 — objects `{size, family[], bold}` as the framework reads them; referenced by name from
  `properties.font` in appearances. No `System.Drawing.Font` is constructed anywhere in code.
- **Settings**: `borderRadius 6`, `focusBorderSize 3`, `focusBlurRadius 0` — the focus frame geometry.
- **Overridden base appearances**: `page` (background `surfaceAlt`), `button` (border `borderSoft`, hover
  `surfaceHover`, focus from the tokens), `textbox` (32 px, `invalid` state = 2 px `danger` border on
  `dangerSurface`), `table-header-cell` (`surfaceAlt` background, `cardTitle` font, `textMuted` text),
  `tooltip/atom` (`textMain` background, 6/10 padding).

Why the theme and not CSS: these are standard controls; the theme reaches their states (hovered, focused,
pressed, disabled, invalid, selected) and their child components without knowing the widget's DOM.

## Layer 2 — AppearanceKey: semantic variants

| Appearance | Inherits | Used by | Why not rung 1 or 3 |
|---|---|---|---|
| `action-button` | `button` | Governance review, Save, Open details | one business variant of a standard control; states (hover/pressed/focus/disabled) must follow |
| `destructive-button` | `action-button` | Inject violation | inherits and overrides the colour only |
| `nav-item` | `button` | the five rail buttons; custom state `selected` | left-aligned, borderless, selected state — still a button |
| `surface-card` | `panel` | toolbar, grid, trace, status cards, editor, reports | the white card is the standard surface of this app |
| `metric-card` | `surface-card` | the four metric cards + Layout cost; custom state `stale` | adds shadow and a dashed warning state |
| `rail-surface` | `panel` | the navigation rail | a second surface colour |
| `banner-danger` | `panel` | the rejection banner | danger surface + border + text colour as one unit |
| `compact-editor` | `textbox` | text editors in the phone dialog and the narrow docked editor | same editor, tighter metrics |
| `heading-label`, `subheading-label`, `muted-label`, `mono-label`, `metric-title`, `metric-value` | `textlabel` | every label | typography from theme fonts, never `Label.Font` |
| `status-label` | `textlabel` | status bar, review summary; states `ok` / `warn` / `error` | colour by state, never `ForeColor` |
| `validation-label` | `textlabel` | editor footer; state `error` | colour + bold by state |
| `trace-list` | `list` | the live trace | mono font on a ListBox |
| `cost-bar-fill`, `cost-bar-track` | `panel` | the two cells of the cost bar | filled panels without borders |

The Module 1 strip colours on the metric cards became the `metric-card` border/shadow; the status colours
that Module 1 set with `ForeColor` became the `ok`/`warn`/`error` states.

## Layer 3 — CssClass: `Styles/AdaptiveOps.css`

| Class | On | Rule | Why CSS and not the theme |
|---|---|---|---|
| `metric-card .metric-title` | card titles | uppercase, letter-spacing | `text-transform` and `letter-spacing` are not theme properties; the content is app-specific |
| `metric-card .metric-value` | card values | tabular numerals | same |
| `cost-card .cost-bar` | the cost bar | opacity | app-specific widget |
| `trace-list` | the trace ListBox | no ligatures | app-specific content |
| `gov-summary`, `gov-summary-pass`, `gov-summary-fail` | review summary label | tabular numerals, underline on fail | app-specific dialog |

Every rule uses inherited text properties on classes we put on the widget root, so the stylesheet never
touches a widget's internal DOM — the upgrade-safety rule. The legacy `Wisej.Web.StyleSheet` extender
would load the same file; the `<link>` in `Default.html` is the simpler, documented route.

## Layer 4 — CssStyle: none

No control carries a `CssStyle`. The one candidate — the proportional cost bar — is expressed by two
Percent `ColumnStyle`s of a `TableLayoutPanel` instead (rung 5). "Inject violation" adds a `CssStyle` to
prove rule G6 catches it.

## Layer 5 — Layout containers, no resize code

```
MainPage (Page)                              Dock: toolbar Top 56 · status Bottom 28 · navigation Left · details Right · workspace Fill
├─ toolbarPanel › toolbarCard › toolbarFlow  FlowLayoutPanel LeftToRight, no wrap; lblProgress FillWeight 1
├─ navigationPanel › NavigationRail          rail-surface; five nav-item buttons Dock Top; IconOnly switches Display
├─ workspacePanel (MinimumSize 320×240)      Dock stack
│  ├─ metricsFlow                            FlowLayoutPanel, wraps; the profile sets the row budget as Height (92 / 184 / 276)
│  │  └─ 4 × MetricCard + LayoutCostCard     UserControls 140×84, Margin 0 0 8 8 (Flow honours Margin)
│  ├─ bannerPanel                            Dock Top, hidden until a rejection
│  ├─ contentFlex                            FlexLayoutPanel Horizontal (Vertical on portrait phone), Spacing 8
│  │  ├─ gridCard  (FillWeight 3, Min 280×200) › gridHeader (title + Open details) + DataGridView (Min 240×160)
│  │  └─ traceCard (FillWeight 2, Min 200×120) › title + trace ListBox
│  └─ ReportsView                            lazy, Dock Fill, TableLayoutPanel 3 columns
├─ detailsPanel (Min 280, Max 420 wide; Bottom on portrait tablet: Min 240, Max 360 tall) › DetailsEditor
│  └─ TableLayoutPanel 2 columns (Absolute 84 · Percent 100), rows 5 × Absolute 40 + Notes Percent 100; footer Save + validation label
└─ statusPanel › statusCard                  labels docked Left · Fill · Right · Right
```

Docking priority is the child order (the control added last docks first); `Padding` on the region panels
is the gap between regions because Dock ignores `Margin`. Nothing sets `Bounds` or `Location` in response
to a size or profile change; the only sizes code assigns are the per-profile final values in
`ApplyProfile` (`navigationPanel.Width`, `detailsPanel.Width/Height`, `metricsFlow.Height`, button widths).

## Layer 6 — per-profile values (`ApplyProfile`)

Written by hand here; in Visual Studio the same values would be set per profile in the Designer's
responsive-properties dropdown (they live in `Control.ResponsiveProfiles`).

| Value | Desktop | Small Desktop / Tablet (Landscape) | Tablet | Phone (Landscape) | Phone |
|---|---|---|---|---|---|
| rail | text, 220 | icon-only, 68 | icon-only, 68 | icon-only, 60 | icon-only, 60 |
| details | Right 340 (Max 420) | Right 300 | **Bottom 300** (Min 240 / Max 360) | hidden | hidden → dialog |
| editor | normal | compact | compact | dialog, compact | dialog, compact, maximized |
| card rows | 1 (92) | 2 (184) | 2 (184) | 1 (92) | 3 (276) |
| grid columns | 6 | 5 (no Owner) | 5 | 3 (Id, Title, Status) | 3 |
| toolbar | text + icon | text + icon | icon-only | icon-only | icon-only |
| grid/trace | side by side | side by side | side by side | side by side | stacked |
| app title | shown | shown | shown | hidden | hidden |

`ApplyProfile(name)` returns early when `name == _lastProfile`, assigns final values (never toggles),
never reloads data, and is called once from the constructor with `Application.ActiveProfile` and from
`Application.ResponsiveProfileChanged`; `Dispose` unsubscribes. A failure inside it is caught, reported
in the status region and undone by re-applying the last good profile (rule G13 proves idempotence by
re-running it and comparing snapshots).

## Layer 7 — ResponsiveProfileChanged behaviour

Only one: on phone profiles the editor is not docked anywhere; **Open details** creates a `DetailsDialog`
(the same `DetailsEditor`, `Compact = true`, `WindowState = Maximized`) and shows it with the non-blocking
`ShowDialog(callback)`. Saving inside the dialog goes through the same `TrySave` as the docked editor.

## Layer 8 — `ClientProfiles.json`

`Phone` → `Phone (Landscape)` → `Tablet` → `Tablet (Landscape)` → `Small Desktop (maxWidth 1024)` →
`Desktop (minWidth 1025)`; first match wins, so the narrow entries come first and `Desktop` is explicit
(the framework's `Default` is treated as Desktop by `ProfileNameOf`). The file is Content, copied to the
output for both targets; rule G8 checks the copy, the order and that the active profile is one of the names.

## Layer 9 — Production governance

`GovernanceReview.Run` computes fourteen rules against the live control tree and the theme objects
(reflection over `Control.ShouldSerializeBackColor/ForeColor/Font`, `Application.Theme.Colors/Settings/Appearances`
as JSON, the stylesheet's class list, the profiles file, the project files). `docs/ProductionChecklist.md`
is the human version of the same list with evidence; `docs/ResponsiveQAMatrix.md` is rerun after every
theme, CSS or profile change and after every Wisej.NET upgrade.

## Performance decisions

- **Layout cost card**: `ControlTree.Count` per region; the page holds roughly 150 controls at load
  (exact number in the card and the trace) against a 400-control budget for one page.
- **Bulk fill**: 240 rows added plain and inside `SuspendLayout()/ResumeLayout(true)`, both timed with a
  `Stopwatch` on the server. The framework batches client updates per request either way; the timing shows
  the server-side cost of the handler, which is what a profile change must keep small.
- **Lazy Reports view**: created on the first navigation only (trace `Reports view created lazily: +N controls`),
  re-bound and reused afterwards.
- **Cheap profile handler**: `ApplyProfile` assigns a dozen properties inside one `SuspendLayout/ResumeLayout`
  pair; its duration is traced in ms.

## Evidence

- Trace at startup: `shell built: …`, `theme: AdaptiveOps — custom theme from Themes/AdaptiveOps.theme …`,
  `ApplyProfile(Desktop) in n ms: rail text rail · details docked Right · cards 1 row(s) · grid 6/6 columns …`,
  `layout cost (page load): N controls · toolbar … · rail … · workspace … · details … · status …`,
  `governance review at startup: 14/14 rules pass`.
- Rail → **Reports**: `Reports view created lazily: +N controls (M data cells) in x ms`; the Layout cost card value rises.
- **Bulk fill**: `bulk fill 240 rows: plain a ms · SuspendLayout/ResumeLayout(true) b ms`.
- **Inject violation** → review shows G1, G2, G5, G6, G7 red; **Recover** → all green again.
