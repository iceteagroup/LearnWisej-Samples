# LabNotes — Module 3 · CSS, CssClass, CssStyle, States, and Runtime Theme Changes

Short notes the lab guide asks for: which layer owns each visual decision, why the single `CssStyle`
could not be a class, what was removed, and the question the grounded AI assistant should answer.

## Which layer owns what

| Layer | Owns in this console | Not allowed to own |
|---|---|---|
| Theme (`Themes/AdaptiveOps.theme`) | identity of every standard control; the `card`, `metric-card`, `action-button`, `metric-title`, `metric-value`, `muted-label`, `status-label`, `banner-label` variants; the `stale`, `warn`, `error` custom states; the page background; every hovered/pressed/disabled/focused look | nothing app-specific that has no appearance (elevation, pills, progress track) |
| `AppearanceKey` (code) | *which* appearance a control renders with — intent, no pixels | colours, fonts |
| States (`AddState` / `RemoveState`) | transient UI conditions derived from data (`stale` when a ticket is ≥ 2 days past due; `warn`/`error` on the status label) | application data — code never reads a state back to decide anything |
| `CssClass` + `Styles/AdaptiveOps.css` | `.metric-card` transition and `.elevated` shadow, `.metric-title` uppercase/letter-spacing, `.metric-strip.strip-*`, `.compact-badge.priority-*`, `.banner-danger`, `.progress-track` / `.progress-fill` | anything a standard widget's internal DOM renders; text colour and font (the theme writes them inline and wins) |
| `CssStyle` | one computed value: `clip-path:inset(0 <100-pct>% 0 0)` on the SLA fill, computed in `MainPage.SetSlaProgress`, written by the single assignment in `MainPage.WriteFillStyle` | everything else |
| Layout containers | position and size (Dock, Padding, Anchor) — unchanged from Module 1 | — |

## Why the SLA fill could not be a class

The value is the on-time share of open tickets, recomputed on every load and every save (63 % on the
seed data). A stylesheet rule is static: it cannot express a number that arrives with the data, and a
class per percentage point is an inline style in disguise. That is the lab's "single computed one-off
value" and the only `CssStyle` in the project. It sets `clip-path` rather than `width` because the
layout engine owns `width` and rewrites it inline on every layout pass; the one inline value has to be a
property no other layer touches.

## What was removed from the styled controls

Every `BackColor`, `ForeColor` and `Font` on the seven cards, the four strips, the four metric titles and
values, the banner, the status label and the muted labels, plus `MainPage.BackColor`. They became
appearances and tokens in the theme or classes in the stylesheet (see `CssDecisions.md` for the table).
Fonts on the remaining shell titles (app title, section titles) are Module 1 shell typography and are
out of this module's scope.

## Paths shown

| Path | Button | What happens |
|---|---|---|
| success | Apply styles | `stale` re-derived from data (theme state), `.elevated` toggled (class), SLA fill set (the one CssStyle); the decision line is written to the trace |
| progress | Animate SLA | a `Wisej.Web.Timer` moves the fill 0 → 100 % through the same single `CssStyle`, then settles on the data value |
| failure | Style miss | undefined class on `cardMine`, invalid `CssStyle` string on the fill, `LoadTheme("Nope")`: nothing throws to the user, nothing renders, the theme is restored |
| recovery | Reset | classes and states removed, AdaptiveOps theme reloaded, repository reset, everything re-derived from the server |
| theme · global | Global: MaterialDark-4 / Back to AdaptiveOps | `Application.LoadTheme` — every session |
| theme · session | Session-only dark | `Application.Theme = new ClientTheme("AdaptiveOps-Dark", Application.Theme)` with dark tokens — this session; remembered in `Application.Session` |

## One question the grounded AI assistant should answer from the reading

"Two operators are signed in. One clicks the dark-mode toggle. Which Wisej.NET member must the toggle
assign so that only that operator's session changes, and what would go wrong if the code changed a
colour on `Application.Theme` directly after a `LoadTheme`?"

Expected answer: assign a *copy* (`new ClientTheme(name, Application.Theme)` with the changed tokens) to
`Application.Theme` for the current session and remember the choice in `Application.Session`; changing a
colour on the theme installed by `LoadTheme` mutates the object shared by every session, so the second
operator would go dark too.

## Evidence

Described per path in `CssDecisions.md` (styling) and `RuntimeThemeNotes.md` (theme switches). The
learner's screenshots: `screenshots/m3-desktop-light.png`, `m3-desktop-dark.png`, `m3-phone-light.png`,
`m3-phone-dark.png`, `m3-two-sessions.png`.
