# CssDecisions · which mechanism owns each visual change

At the end of Module 1 every visual decision in the Adaptive Operations Console was patched from code:
`BackColor = White` on the cards, hard-coded strip colours, `ForeColor` and `Font` on the labels, a
`BackColor` on the page. Module 3 moves each decision to the layer that owns it and adds a badge and an
SLA bar.

## The decision tree

Walk down and stop at the first rung that fits.

1. **Theme appearance** (`Themes/AdaptiveOps.theme`): the identity of a standard control, a reusable
   semantic variant (`AppearanceKey`), or a transient condition a designer should restyle (custom state).
2. **CssClass + `Styles/AdaptiveOps.css`**: app-owned UI the theme has no appearance for (elevation, a
   pill, a progress track), restyled in one file. Selectors start from your class.
3. **CssStyle**: one value computed from live data at runtime. Nothing else.

If none fits, the change is data or layout, not styling.

## Each visual change and the mechanism chosen

| Visual change | Mechanism | Where | Why |
|---|---|---|---|
| Card surface, border, radius on every card | theme appearance `card` (inherits `panel`) | `appearances.card` | a semantic variant of `Panel` used on every card; the dark copy recolours them through the `surface` token |
| Metric card look and its **stale** condition | theme appearance `metric-card` with a custom state `stale` (`warningBg` surface, `warning` border), plus `metric-value/stale` | `appearances.metric-card`, `appearances.metric-value`; code: `AddState("stale")` / `RemoveState` in `RefreshTickets()` | stale is a transient UI condition; the threshold (`TicketRepository.StaleAfterDays`) is data, the look is theme |
| Metric title and value typography | theme appearances `metric-title`, `metric-value` | `appearances.metric-title`, `appearances.metric-value` | text colour and font are written inline by the theme engine; a class cannot beat them |
| Muted secondary text | theme appearance `muted-label` | `appearances.muted-label` | a colour token instead of `ForeColor` values; flips with the dark copy |
| Primary command (Save) | `AppearanceKey = "action-button"` | `appearances.action-button`; `MainPage.Designer.cs` | C# says what the button is, the theme says how every state looks |
| Page background | theme appearance `page` (`surfaceAlt`) | `appearances.page` | the page is a standard control |
| Metric card elevation | `CssClass = "metric-card"`, `.metric-card` | `Styles/AdaptiveOps.css` | the theme has no notion of elevation; `box-shadow` is not written by the theme, so the class wins |
| Uppercase, letter-spaced metric title | `CssClass = "metric-title"`, `.metric-card .metric-title` | `Styles/AdaptiveOps.css` | text transform and letter spacing are not theme properties |
| The 4 px accent strip on each card | `CssClass = "metric-strip strip-open"` … `strip-closed` | `Styles/AdaptiveOps.css` | four modifier classes replace four `BackColor` values (the hex values mirror the theme tokens by hand) |
| Priority badge in the editor | `CssClass = "compact-badge priority-<level>"` | `Styles/AdaptiveOps.css`; `MainPage.UpdatePriorityBadge` | the pill is app UI; the text keeps the theme's font and colour |
| SLA track and fill look | `CssClass = "progress-track"` / `"progress-fill"` | `Styles/AdaptiveOps.css` | two plain Panels the theme draws nothing on |
| **SLA fill amount** | **`CssStyle`**, the single assignment in the project | `MainPage.UpdateSla()`: `barFill.CssStyle = $"clip-path:inset(0 {100 - slaPercent}% 0 0)"` | see below |

## The one CssStyle, and why it is the last resort

The value that changes with the data is the share of open tickets still within their due date
(`TicketRepository.SlaOnTimePercent`). A stylesheet has no rule for "63 %", and one class per percentage
would be the same inline style with more steps. So it is the module's single `CssStyle`, set in
`UpdateSla()`, which `RefreshTickets()` calls on load and after every save.

Why `clip-path` rather than `width`: the layout engine writes `width` inline on every layout pass (the
fill is docked Fill), so an inline width would be overwritten on the next resize. `clip-path` is never
written by the theme or the layout, so the reveal survives resizes and theme switches.

An inline style is an anonymous override: not in the theme, not searchable in the stylesheet, and it wins
over most of what the theme generates. The lab allows one, computed, with the reason written down.

## "The class does not work" checklist

1. Is the class on the element? Inspect the widget's root element.
2. Is a property set on the control? `BackColor`, `ForeColor`, `Font` are written inline and beat any class.
3. Is an inline style set? Search the project for `CssStyle =`: one hit, in `UpdateSla()`.
4. Is the rule scoped correctly? `.metric-card .metric-title` only matches inside a card.

## Evidence

- The four cards are white surfaces with a soft shadow, titles small uppercase muted text; the Overdue
  card is tinted amber with an amber number (`stale`: two seed tickets are at least 2 days past due); the
  SLA bar is filled to the computed share; Save is brand blue.
- Select the two stale tickets, move each due date to tomorrow and Save: the Overdue card turns white.
- Screenshots taken by the learner: `screenshots/m3-desktop-light.png`, `m3-desktop-dark.png`,
  `m3-phone-light.png`, `m3-phone-dark.png`.
