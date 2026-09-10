# AdaptiveOps · Theming & Responsive Layouts Course · Module 3

Local lab build for **Module 3 · CSS, CssClass, CssStyle, States, and Runtime Theme Changes**. It follows
the walkthrough video and the lab: the same metric card is styled three ways — a theme appearance
(`AppearanceKey = "metric-card"`, with a custom `stale` state), a class from the scoped stylesheet
`Styles/AdaptiveOps.css` (`CssClass = "metric-card elevated"`), and exactly one computed `CssStyle`
(the SLA fill) — and then the theme is changed at runtime twice: globally with `Application.LoadTheme`
(every session) and for this session only with `Application.Theme = new ClientTheme("AdaptiveOps-Dark",
Application.Theme)`. The console ships its own theme, `Themes/AdaptiveOps.theme`, selected in
`Default.json`; every `BackColor`, `ForeColor` and `Font` the Module 1 shell carried on the styled controls
is gone, replaced by appearances, tokens, states and classes.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 3/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5503
```

Then open <http://localhost:5503>. (Visual Studio: open `AdaptiveOps.slnx`, press F5.) For the
two-session proof open the same URL in a second browser or a private window.

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

The trace card at the bottom of the workspace logs every decision (`→` server to client, `←` client to
server, `•` server decision). Its second line at start-up is the module's decision tree.

| Action | Path | What you should see |
|---|---|---|
| Page load | – | White cards on a light grey page (theme `card` appearance, page token `surfaceAlt`), small uppercase muted card titles, the **Overdue** card tinted amber with an amber border and number (`stale` state: two seed tickets are ≥ 2 days past due), a blue SLA pill filled to **63 %**, a brand-blue **Save** button (`action-button`). Trace: `• server theme: AdaptiveOps selected in Default.json …`, `• server theme file present …`, `• server stylesheet present: Styles/AdaptiveOps.css …`, `→ render theme state (page load): cardOverdue.AddState("stale") …`, `→ render CssStyle (page load): barFill.CssStyle = "clip-path:inset(0 37% 0 0)" …` |
| **Apply styles** | success | The Overdue card lifts 2 px with a shadow (`.metric-card.elevated`); click again and it settles back. Trace shows the three mechanisms in order — theme state (unchanged, still derived from data), `cardOverdue.AddCssClass("elevated") → CssClass = "metric-card elevated"`, the CssStyle — then `• decision: stale = theme state · elevated = CssClass · SLA fill = CssStyle` |
| **Animate SLA** | progress | The fill sweeps 0 → 100 % in 5 % steps every 120 ms (a `Wisej.Web.Timer`, no client code) and settles on 63 %; trace lines at 25/50/75/100 show the same single `CssStyle` changing |
| **Style miss** | failure | `cardMine.CssClass = "metric-card metric-card-glow"` — no visible change (no rule for the class); the SLA fill jumps to 100 % with a `?` label (the browser dropped `clip-path:nonsense(42); colr:red`); `LoadTheme("Nope") FAILED … theme restored for this session: AdaptiveOps`; red banner and `● style miss` |
| **Reset** | recovery | Classes back to `metric-card`, states removed and re-derived from the seed data (Overdue is stale again), fill back to 63 %, AdaptiveOps reloaded, banner gone |
| Select T-1037 and T-1038, set each **Due** to tomorrow, **Save** | data → state | After the second save: `cardOverdue.RemoveState("stale") — no ticket ≥ 2 days past due any more`; the card is white and the SLA rises. The threshold is data (`TicketRepository.StaleAfterDays`), the look is theme, the code never reads the state back |
| Select tickets of different priority | CssClass modifiers | The badge next to the subtitle changes tint: `compact-badge priority-critical` / `-high` / `-medium` / `-low` |
| Hover / press **Save** | AppearanceKey | Darker blue on hover, darker still on press — no code, the `action-button` states in the theme |
| **Session-only dark** | theme · session | This tab goes dark; status bar `theme: AdaptiveOps-Dark · this session only`; trace `… 27 colour tokens changed ON THE COPY … THIS SESSION ONLY` and `• server proof: shared theme 'AdaptiveOps' Colors.surface was #FFFFFF and is still #FFFFFF`. The second browser stays light. Press F5: the tab comes back dark (`Application.Session`) |
| **Global: MaterialDark-4** | theme · global | Both browsers turn dark on their next round-trip (click anything in the other one); status bar `theme: MaterialDark-4 · shared (every session)`. The cards lose their `card`/`metric-card` appearances (the stock theme has none) — the trace explains why |
| **Back to AdaptiveOps** | theme · global | Both browsers light again; the session choice is forgotten |
| Clear trace | – | empties the list |

## Where things live

```
Module 3/
├─ AdaptiveOps.slnx
├─ README.md                         this file
└─ AdaptiveOps/
   ├─ Default.html                   <link rel="stylesheet" href="Styles/AdaptiveOps.css" /> next to wisej.wx
   ├─ Default.json · Web.config      "theme": "AdaptiveOps" / Wisej.DefaultTheme = AdaptiveOps (startup theme)
   ├─ Themes/AdaptiveOps.theme       the console's theme: Bootstrap-4 as the base definition + AdaptiveOps tokens,
   │                                 fonts (metricTitle, metricValue, heading) and appearances action-button, card,
   │                                 metric-card (+ stale), metric-title, metric-value (+ stale), muted-label,
   │                                 status-label (+ warn, error), banner-label; page background → surfaceAlt
   ├─ Styles/AdaptiveOps.css         scoped stylesheet: .metric-card(.elevated), .metric-card .metric-title,
   │                                 .metric-strip.strip-*, .compact-badge(.priority-*), .banner-danger, .progress-track/-fill
   ├─ MainPage.Designer.cs           the shell: AppearanceKey / CssClass on every styled control, no BackColor/ForeColor/Font on them
   ├─ MainPage.cs                    ApplyStaleState (states), SetSlaProgress (the one CssStyle), SwitchGlobalTheme,
   │                                 ApplySessionDarkTheme, Session choice, lab buttons, trace
   ├─ Models/TicketRepository.cs     tickets + the data rules: StaleAfterDays, CountStaleOverdue, SlaOnTimePercent
   ├─ Models/Ticket.cs
   ├─ Program.cs · Startup.cs        session entry point; Kestrel host serving the project folder (Styles/, Themes/), never *.json
   └─ docs/
      ├─ CssDecisions.md             deliverable: each visual change → mechanism → why; the decision tree; the one CssStyle
      ├─ RuntimeThemeNotes.md        deliverable: startup vs global vs session theme change, what the trace showed
      └─ LabNotes.md                 lab notes: layer ownership table, the CssStyle reason, paths, the AI-assistant question
```

## Deliverables

1. **Scoped AdaptiveOps.css with metric-card and compact-badge classes applied through CssClass** —
   [`AdaptiveOps/Styles/AdaptiveOps.css`](AdaptiveOps/Styles/AdaptiveOps.css); applied in
   [`AdaptiveOps/MainPage.Designer.cs`](AdaptiveOps/MainPage.Designer.cs) (`cardOpen … cardClosed`, `lblPriorityBadge`)
   and toggled in `MainPage.btnApplyStyles_Click`. Explained in [`AdaptiveOps/docs/CssDecisions.md`](AdaptiveOps/docs/CssDecisions.md).
2. **CssStyle used for exactly one computed one-off value, with the reason documented** —
   `MainPage.SetSlaProgress` computes it, `MainPage.WriteFillStyle` holds the only `CssStyle =` in the project; reason in
   [`CssDecisions.md`](AdaptiveOps/docs/CssDecisions.md) and [`LabNotes.md`](AdaptiveOps/docs/LabNotes.md).
3. **Primary command using the action-button AppearanceKey** — `btnSave.AppearanceKey = "action-button"`
   in the designer; the appearance in [`AdaptiveOps/Themes/AdaptiveOps.theme`](AdaptiveOps/Themes/AdaptiveOps.theme).
4. **Custom stale state added through States and styled in the theme** — `MainPage.ApplyStaleState`
   (`AddState("stale")` / `RemoveState`); `metric-card/stale` and `metric-value/stale` in the theme;
   threshold in `TicketRepository.StaleAfterDays`.
5. **Session-only light/dark theme switch that leaves the shared theme untouched** —
   `MainPage.ApplySessionDarkTheme` (+ `RememberThemeChoice` / `RestoreSessionTheme`), contrasted with
   `SwitchGlobalTheme`; explained in [`AdaptiveOps/docs/RuntimeThemeNotes.md`](AdaptiveOps/docs/RuntimeThemeNotes.md).

## Lab step → code map

| Lab step (tr3s2 / labs/m3.json) | Where |
|---|---|
| Open the console, confirm the AdaptiveOps theme is applied through Web.config / Default.json | `Default.json` `"theme"`, `Web.config` `Wisej.DefaultTheme`, `Themes/AdaptiveOps.theme`; trace line `• server theme: AdaptiveOps selected in …` |
| Create `AdaptiveOps.css`, load it next to the theme, selectors rooted in classes you own (`.metric-card`, `.metric-card .metric-title`, `.compact-badge`) | `Styles/AdaptiveOps.css`; `<link>` in `Default.html`; `Startup.cs` serves the project folder |
| Apply `CssClass = "metric-card"` to the four metric panels and `"compact-badge"` to the badge; delete every `BackColor`/`ForeColor`/`Font` override on those controls | `MainPage.Designer.cs`: `cardOpen/cardOverdue/cardMine/cardClosed.CssClass`, `lblPriorityBadge.CssClass`; no colour or font property on any of them (the theme appearances `metric-card`, `metric-title`, `metric-value` own those) |
| Set the single computed `CssStyle` (progress width) in one method, remove every other `CssStyle`, write the reason in the lab notes | `MainPage.SetSlaProgress` computes `clip-path:inset(0 {100 - percent}% 0 0)`; `MainPage.WriteFillStyle` is the only `CssStyle =` in the project; `docs/LabNotes.md`, `docs/CssDecisions.md` |
| `AppearanceKey = "action-button"` on the primary command; default/hovered/pressed/disabled from the theme | `btnSave` in `MainPage.Designer.cs`; `appearances.action-button` in the theme (inherits `button`, four states) |
| Add a `stale` state styled from the warning token; add/remove it from data | theme: `metric-card.states.stale` (`warningBg`, `warning`), `metric-value.states.stale`; code: `MainPage.ApplyStaleState`, data: `TicketRepository.CountStaleOverdue` / `StaleAfterDays` |
| Toolbar theme toggle: load the second theme, assign `Application.Theme` for the current session, store the choice in `Application.Session`, never modify the shared theme | `btnThemeSession_Click` → `ApplySessionDarkTheme` (copy via `new ClientTheme(name, Application.Theme)`, tokens changed on the copy, `Application.Theme = dark`, `RememberThemeChoice`); `RestoreSessionTheme` on load |
| Show every path: confirmation in the status label, missing/invalid theme caught without a half-themed session, stale gained and lost as data changes, second session keeps the default theme | `SetStatus` after each switch; `SwitchGlobalTheme("Nope")` in *Style miss* with restore; `ApplyStaleState` on every `LoadTickets`; second browser stays on `AdaptiveOps` |
| Two sessions side by side, desktop and phone screenshots of both themes, a note on which layer owns each decision | `docs/RuntimeThemeNotes.md` (two-session evidence), `docs/LabNotes.md` (ownership table), screenshots listed there as taken by the learner |

Note on the theme file: the framework's `ClientTheme` does not merge a top-level `"inherit"` from a
theme file (checked in-process against Wisej-4 4.1.0), so `Themes/AdaptiveOps.theme` carries the full
Bootstrap-4 definition as its base with the AdaptiveOps tokens and appearances placed first in `colors`,
`fonts` and `appearances`. Appearance-level `"inherit": "button"` / `"panel"` / `"textlabel"` works as the
lesson describes.

## Self-check answers (lab guide)

- **A designer asks for a smaller corner radius on every metric card. Which file changes, and how many
  places would you have had to edit if the radius had been set with `CssStyle`?**
  One file, one value: the radius of a metric card is the `radius` style of the `card` appearance in
  `Themes/AdaptiveOps.theme`, which resolves `$borderRadius` from `settings.borderRadius` (6). Changing
  that one number changes the seven cards and every other radius the theme derives from it; changing the
  `card` appearance alone changes the seven cards. Had the radius been an inline `CssStyle`, it would have
  been written on each of the four metric panels (plus the seven other cards if they had been styled the
  same way) — eleven anonymous strings to find and edit, none of them visible in the theme or the
  stylesheet, each a candidate to be missed and to drift.
- **Why does the Save (Assign) button need no hover or pressed code, and what exactly would you lose by
  replacing its `AppearanceKey` with `BackColor` and `ForeColor` values that match the palette?**
  Because `AppearanceKey = "action-button"` makes the control render with a named appearance of the
  active theme, and that appearance defines the `default`, `hovered`, `pressed` and `disabled` states —
  the theme engine switches between them as the browser reports the widget's state, so no C# runs.
  Replacing it with `BackColor`/`ForeColor` would give one static colour pair: hover and press would stop
  changing (or need a `MouseEnter`/`MouseLeave` handler pair per button), the disabled look would be the
  same blue at half opacity instead of the theme's grey, the colours would be written inline and beat
  every theme swap (the button would stay AdaptiveOps blue on MaterialDark-4 and on the dark session copy),
  and a designer could no longer change it without a build. You would lose the states, the theme
  switch and the separation between intent (code) and pixels (theme).
- **Two people are signed in and one of them switches to dark mode. Describe what each sees, and point
  at the line of your code that guarantees it.**
  The one who clicked sees the whole console re-rendered with the dark tokens (dark surfaces, light
  text, dark grid rows) and the status bar reads `theme: AdaptiveOps-Dark · this session only`; after an
  F5 it is still dark because the choice is in `Application.Session`. The other person sees nothing change:
  their session keeps rendering `AdaptiveOps` and their status bar still says `shared (every session)`.
  The guarantee is the constructor call in `MainPage.ApplySessionDarkTheme`:
  `var dark = new ClientTheme(SessionDarkThemeName, shared);` — the 27 colour tokens are written to
  `dark.Colors`, a copy, and only `Application.Theme = dark;` follows, which is the per-session theme.
  The shared theme object that `LoadTheme` installed is never assigned to; the trace proves it by reading
  the shared `surface` token before and after (`was #FFFFFF and is still #FFFFFF`). The opposite line —
  `Application.Theme.Colors["surface"] = …` without the copy — is the one that would have put both
  people in the dark.
