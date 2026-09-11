# LabNotes · CSS, CssClass, CssStyle, States, and Runtime Theme Changes

## Which layer owns what

| Layer | Owns in this console | Not allowed to own |
|---|---|---|
| Theme (`Themes/AdaptiveOps.theme`) | identity of every standard control; the `card`, `metric-card`, `action-button`, `metric-title`, `metric-value`, `muted-label`, `status-label` variants; the `stale` state; the page background; every hovered / pressed / disabled / focused look | app-specific UI with no appearance (elevation, pills, progress track) |
| `AppearanceKey` (code) | which appearance a control renders with | colours, fonts |
| States (`AddState` / `RemoveState`) | `stale` on the Overdue card when a ticket is at least 2 days past due | application data: code never reads a state back to decide anything |
| `CssClass` + `Styles/AdaptiveOps.css` | `.metric-card` elevation, `.metric-title` uppercase, `.metric-strip.strip-*`, `.compact-badge.priority-*`, `.progress-track` / `.progress-fill` | a standard widget's internal DOM; text colour and font |
| `CssStyle` | one computed value: `clip-path:inset(0 <100-pct>% 0 0)` on `barFill`, set in `UpdateSla()` | everything else |
| Layout containers | position and size (Dock, Padding, Anchor), unchanged from Module 1 | — |

## Why the SLA fill could not be a class

The value is the on-time share of open tickets, recomputed on every refresh. A stylesheet rule is static:
it cannot express a number that arrives with the data. It sets `clip-path` rather than `width` because the
layout engine owns `width` and rewrites it on every layout pass.

## What was removed from the styled controls

Every `BackColor`, `ForeColor` and `Font` on the cards, strips, metric titles and values, the status label
and the muted labels, plus `MainPage.BackColor`. They became appearances and tokens in the theme or classes
in the stylesheet (see `CssDecisions.md`). Fonts on the remaining shell titles are Module 1 typography.

## One question the grounded AI assistant should answer

"Two operators are signed in. One clicks the dark-mode toggle. Which member must the toggle assign so that
only that operator's session changes, and what goes wrong if the code changes a colour on
`Application.Theme` directly after a `LoadTheme`?"

Expected answer: assign a copy (`new ClientTheme(name, Application.Theme)` with the changed tokens) to
`Application.Theme` and remember the choice in `Application.Session`; changing a colour on the theme
installed by `LoadTheme` mutates the object every session shares, so the second operator goes dark too.

Screenshots taken by the learner: `screenshots/m3-desktop-light.png`, `m3-desktop-dark.png`,
`m3-phone-light.png`, `m3-phone-dark.png`, `m3-two-sessions.png`.
