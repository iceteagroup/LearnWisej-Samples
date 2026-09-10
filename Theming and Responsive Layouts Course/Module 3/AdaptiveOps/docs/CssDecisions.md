# CssDecisions — which mechanism owns each visual change (Module 3)

The Adaptive Operations Console had, at the end of Module 1, every visual decision patched from code:
`BackColor = White` on seven cards, four hard-coded strip colours, three `ForeColor` values for the
status label, a red banner with `BackColor`/`ForeColor`/`Font` set in the designer, and a `BackColor`
on the page. Module 3 moves each of those decisions to the layer that owns it and adds the two things
the lab asks for on top: a badge and an SLA bar.

## The decision tree

Walk down and stop at the first rung that fits. It is also the second line the trace writes at start-up.

1. **Theme appearance** (`Themes/AdaptiveOps.theme`) — is it the *identity* of a standard control, or a
   reusable *semantic variant* of one (`AppearanceKey`), or a *transient condition* a designer should be
   able to see and restyle (custom state)? Then it is the theme's. Colours, fonts, text colour, borders,
   radius and every hovered / pressed / disabled / focused state live here.
2. **CssClass + `Styles/AdaptiveOps.css`** — is it app-owned UI that the theme has no appearance for
   (elevation, a pill, a tinted banner surface, a progress track) and that a designer should restyle in
   one file? Then it is a class you own. Selectors start from your class and never reach into a widget's
   internal DOM.
3. **CssStyle** — is it *one* value computed from live data at runtime? Then, and only then, an inline
   style. Anything else that lands here is a review comment.

If none of the three fits, the change is data or layout, not styling.

## Each visual change and the mechanism chosen

| Visual change | Mechanism | Where | Why this rung and not another |
|---|---|---|---|
| Card surface, 1 px border, 6 px radius on every card (toolbar, navigation, grid, trace, details, status, SLA row) | Theme appearance `card` (inherits `panel`), `AppearanceKey = "card"` | `Themes/AdaptiveOps.theme` → `appearances.card` | The card is a semantic variant of `Panel` used seven times. Module 1 set `BackColor = White` seven times; one appearance replaces all of them and the dark copy recolours them by changing the `surface` token. |
| Metric card look and its **stale** condition | Theme appearance `metric-card` (inherits `card`) with a custom state `stale` (`warningBg` surface, `warning` border), plus `metric-value/stale` for the number | `appearances.metric-card`, `appearances.metric-value` — code: `cardOverdue.AddState("stale")` / `RemoveState` in `MainPage.ApplyStaleState` | Stale is a transient UI condition the theme can represent, exactly what custom states are for. The threshold (`TicketRepository.StaleAfterDays = 2`) is data; the look is theme; code only adds/removes the state and never reads it back to decide anything. |
| Metric title and value typography and colour | Theme appearances `metric-title` (font `metricTitle`, colour `textMuted`) and `metric-value` (font `metricValue`, colour `textMain`) | `appearances.metric-title`, `appearances.metric-value`, `fonts.metricTitle`, `fonts.metricValue` | Text colour and font are widget properties the theme engine writes inline on the element; a class rule cannot beat them, so they belong to the theme. The designer used to carry `Font(8F, Bold)` + `ForeColor(103,112,133)` on eight labels. |
| Muted secondary text (nav title, details subtitle, hint, theme label, SLA caption) | Theme appearance `muted-label` (`textColor: textMuted`) | `appearances.muted-label` | Same reason: a colour token instead of five `ForeColor` values, and it flips with the dark copy. |
| Status "● ready / warn / error" colour | Theme appearance `status-label` with custom states `warn` and `error` (default = `success`) | `appearances.status-label`; `MainPage.SetStatus` adds/removes the states | Module 1 switched three `ForeColor` values in code. The condition is semantic and transient → states. |
| Primary command (Save) | `AppearanceKey = "action-button"` (inherits `button`; default / hovered / pressed / disabled overridden) | `appearances.action-button`; `MainPage.Designer.cs` | The lesson's `btnAssign` case. No `BackColor`, no hover handler: C# says *what* the button is, the theme says how every state looks, and a designer can change it without a build. |
| Page background | Theme appearance `page` default `backgroundColor: surfaceAlt` | `appearances.page` (edited in the Bootstrap-4 copy) | Module 1 set `this.BackColor` on the page. The page is a standard control; its identity is the theme's. |
| **Elevation** of a metric card (shadow + 2 px lift) | `CssClass` — `.metric-card.elevated` | `Styles/AdaptiveOps.css`; toggled by `AddCssClass("elevated")` in *Apply styles* | The theme has no notion of "elevated"; it is app UI. A class is searchable, reusable on all four cards and restyled in one rule. `box-shadow`/`transform` are properties the theme engine does not write, so the class actually wins. |
| Uppercase, letter-spaced metric title | `CssClass = "metric-title"` — `.metric-card .metric-title` | `Styles/AdaptiveOps.css` | Text transform and letter spacing are not theme properties; the rule is scoped to a class inside another class we own. Colour and font of the same label stay in the theme (see above). |
| The 4 px accent strip on each card | `CssClass = "metric-strip strip-open"` … `strip-closed` | `Styles/AdaptiveOps.css` | Four modifier classes replace four `BackColor` values. Known trade-off: the strip hex values mirror theme tokens by hand; a palette change touches both files. Acceptable because the strip is app-only decoration; the alternative (four appearances) was judged heavier than the gain. |
| Priority badge in the editor | `CssClass = "compact-badge priority-<level>"` — `.compact-badge`, `.compact-badge.priority-*` | `Styles/AdaptiveOps.css`; `MainPage.UpdatePriorityBadge` | The pill (radius 999 px, tinted background) is app UI. The text keeps the theme's default font and colour on purpose: tint the surface, do not fight the inline text colour. |
| Validation banner | Both layers, each owning its part: `AppearanceKey = "banner-label"` (red bold text) **and** `CssClass = "banner-danger"` (tinted surface, 4 px left border, radius) | `appearances.banner-label`; `Styles/AdaptiveOps.css` | Text colour/font are theme properties (inline) → theme. Background tint and border are surface → class. Module 1 had `BackColor`, `ForeColor` and `Font` on the label. |
| SLA track and fill look | `CssClass = "progress-track"` / `"progress-fill"` | `Styles/AdaptiveOps.css` | Two plain Panels with `BorderStyle = None`; the theme draws nothing on them, so the stylesheet owns their colour and radius. |
| **SLA fill amount** (63 % on the seed data) | **`CssStyle`** — the single assignment in the project | `MainPage.WriteFillStyle` (`barFill.CssStyle = css`), value computed in `SetSlaProgress`: `clip-path:inset(0 {100 - percent}% 0 0)` | See below. |

## What CssStyle was used for, and why it is the last resort

The one value in the console that changes with the data is the share of open tickets that are still
within their due date (`TicketRepository.SlaOnTimePercent`). It is rendered as how much of the fill
Panel is revealed. That value cannot be a class: a stylesheet has no rule for "63 %", and generating
one class per percentage would be the same inline style with more steps. So it is the module's single
`CssStyle`: the assignment `barFill.CssStyle = css` exists once, in `MainPage.WriteFillStyle`, and the
value is computed in `SetSlaProgress`, which is called from `LoadTickets` (page load, Save, Reset), from
*Apply styles* and from the *Animate SLA* timer. The failure button pushes an invalid string through the
same `WriteFillStyle` to show what a bad inline style does; nothing else in the project assigns `CssStyle`
(search for `CssStyle =`: one hit).

Why `clip-path` rather than the lesson's `width:63%`: the layout engine writes `width` inline on every
widget on every layout pass (the fill is `Dock = Fill`), so an inline width would be overwritten the
next time the row resizes. The single CssStyle must set something no other layer owns — `clip-path` is
never written by the theme or the layout, so the reveal survives resizes and theme swaps.

Why it is the last resort: an inline style is an anonymous override. It is not in the theme, so a
designer cannot see it; it is not in the stylesheet, so nobody can search for it; it wins over most of
what the theme generates, so it is the first suspect when "the class does not work"; and it is stored
per control, so ten of them are ten places to edit. The lab allows one, asks that it be computed, and
asks that the reason be written down — this is that note (also in `LabNotes.md`).

## What defeats a class (the "class does not work" checklist)

1. **Is the class on the element?** Inspect the widget's root element. `cardMine.AddCssClass("metric-card-glow")`
   in *Style miss* puts a class on the element that no rule matches — nothing renders, nothing breaks.
2. **Is a property set on the control?** `BackColor`, `ForeColor`, `Font` are written inline by the theme
   engine and beat any class. That is why this module removed every one of them from the styled controls
   and why colour/font decisions were moved to appearances, not to CSS.
3. **Is an inline style set?** A leftover `CssStyle` beats everything. Search the project for `CssStyle =`:
   there is one hit, in `WriteFillStyle`.
4. **Is the rule scoped to your container?** `.metric-card .metric-title` only matches inside a card;
   `.compact-badge.priority-high` only when both classes are present.

Two older mechanisms were deliberately not used: the `Wisej.Web.StyleSheet` extender (`StyleSheetSource`,
`SetCssClass`) is a migration aid — every control already has `CssClass`; and the theme's `stylesheet`
section (kept empty in `AdaptiveOps.theme`) is for third-party widgets without a Wisej appearance, not
for standard controls, where appearance-generated rules may override it.

## Evidence

- **Page load** — trace: `• decision tree: 1 theme appearance … → 2 CssClass … → 3 CssStyle …`,
  `• server theme: AdaptiveOps selected in Default.json …`, `• server theme file present: Themes/AdaptiveOps.theme (… bytes)`,
  `• server stylesheet present: Styles/AdaptiveOps.css (… bytes)`. The four cards are white surfaces on
  a light grey page, titles are small uppercase muted text, the Overdue card is tinted amber with an amber
  border (`stale`, because two seed tickets are ≥ 2 days past due) and its number is amber; the SLA row
  shows a blue pill filled to 63 %; the Save button is brand blue.
- **Apply styles** — trace: `→ render theme state … cardOverdue stale=True`, `→ render CssClass (Apply styles):
  cardOverdue.AddCssClass("elevated") → CssClass = "metric-card elevated"`, `→ render CssStyle (Apply styles):
  barFill.CssStyle = "clip-path:inset(0 37% 0 0)"`, then `• decision: stale = theme state · elevated = CssClass · SLA fill = CssStyle`.
  The Overdue card lifts 2 px with a shadow; clicking again removes the lift.
- **Stale comes and goes with data** — select T-1037 and T-1038, move each due date to tomorrow, Save:
  after the second save the trace shows `cardOverdue.RemoveState("stale") — no ticket ≥ 2 days past due any more`
  and the card is white again. Reset brings the seed data and the state back.
- **Style miss** — `cardMine.CssClass = "metric-card metric-card-glow"` with no visible change,
  `barFill.CssStyle = "clip-path:nonsense(42); colr:red"` after which the fill shows 100 % with a `?` label
  (the browser dropped both declarations), and `LoadTheme("Nope") FAILED … theme restored for this session`.
  Reset restores the class list, the computed clip-path and the AdaptiveOps theme.
- Screenshots for the lab (taken by the learner): `screenshots/m3-desktop-light.png`,
  `screenshots/m3-desktop-dark.png`, `screenshots/m3-phone-light.png`, `screenshots/m3-phone-dark.png`
  (browser device emulation ≤ 480 px; the shell has no phone profile until Module 6, so the details panel
  simply overflows — record that on the screenshot).
