# AdaptiveOps · Theming & Responsive Layouts Course · Module 3

Lab build for **Module 3 · CSS, CssClass, CssStyle, States, and Runtime Theme Changes**. The console ships
its theme (`Themes/AdaptiveOps.theme`, selected in `Default.json`) and a scoped stylesheet
(`Styles/AdaptiveOps.css`, linked from `Default.html`). The metric cards get `CssClass = "metric-card"`, the
priority badge `CssClass = "compact-badge"`, the SLA bar carries the project's only `CssStyle` (a computed
value), Save uses `AppearanceKey = "action-button"`, the Overdue card gets a themed `stale` state from data,
and a toolbar switch changes the theme for the current session only.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 3/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5503
```

Then open <http://localhost:5503>. For the two-session check open the same URL in a private window too.

## What to try

- **Page load**: white cards with a soft shadow and small uppercase titles; the **Overdue** card is tinted
  amber (`stale`: two seed tickets are at least 2 days past due); the SLA bar is filled to the computed
  share; Save is brand blue. Status bar: `Theme: AdaptiveOps (this session)` and `Width: N px`.
- **Dark theme** (toolbar): this session turns dark, the status label reads
  `Theme: AdaptiveOps-Dark (this session)`, the second browser stays light. Reload: still dark. Click
  **Light theme** to go back.
- **Stale comes and goes with data**: select T-1037 and T-1038, set each Due to tomorrow and Save; after the
  second save the Overdue card turns white and the SLA share rises.
- **Priority badge**: select tickets of different priority; the badge tint changes
  (`compact-badge priority-critical` / `-high` / `-medium` / `-low`).
- **Save**: hover and press it (the `action-button` states); clear the Title and Save to see the server
  reject it in the status label.

## Where things live

```
AdaptiveOps/
├─ Default.html                   <link rel="stylesheet" href="Styles/AdaptiveOps.css" />
├─ Default.json · Web.config      "theme": "AdaptiveOps" (startup theme)
├─ Themes/AdaptiveOps.theme       tokens, fonts, appearances card, metric-card (+ stale), metric-title,
│                                 metric-value (+ stale), muted-label, status-label, action-button
├─ Styles/AdaptiveOps.css         .metric-card, .metric-card .metric-title, .metric-strip.strip-*,
│                                 .compact-badge(.priority-*), .progress-track / .progress-fill
├─ MainPage.Designer.cs           AppearanceKey / CssClass on the styled controls, no BackColor/ForeColor/Font on them
├─ MainPage.cs                    RefreshTickets() (stale state), UpdateSla() (the CssStyle), btnTheme_Click
├─ Models/TicketRepository.cs     tickets + StaleAfterDays, CountStaleOverdue, SlaOnTimePercent
└─ docs/
   ├─ CssDecisions.md             each visual change → mechanism → why; the one CssStyle
   ├─ RuntimeThemeNotes.md        startup vs session vs shared theme; the switch; two-session evidence
   └─ LabNotes.md                 layer ownership table, the CssStyle reason, the AI-assistant question
```

## Lab step → code map

| Lab step | Where |
|---|---|
| `AdaptiveOps.css` loaded next to the theme, selectors rooted in owned classes | `Styles/AdaptiveOps.css`; `<link>` in `Default.html` |
| `CssClass = "metric-card"` on `cardOpen` / `cardOverdue` / `cardMine` / `cardClosed`, `"compact-badge"` on `lblPriorityBadge` | `MainPage.Designer.cs` |
| One computed `CssStyle` in `UpdateSla()`, reason in `LabNotes.md` | `MainPage.UpdateSla()`; `docs/LabNotes.md`, `docs/CssDecisions.md` |
| `btnSave.AppearanceKey = "action-button"` | `MainPage.Designer.cs`; `appearances.action-button` in the theme |
| `stale` state from the warning token, added / removed in `RefreshTickets()` | theme `metric-card.states.stale`, `metric-value.states.stale`; `MainPage.RefreshTickets()` |
| `btnTheme` in the toolbar: `AdaptiveOps-Dark` for this session, choice in `Application.Session`, shared theme untouched | `MainPage.btnTheme_Click` / `SetSessionTheme`; see `docs/RuntimeThemeNotes.md` for the in-memory dark copy |
| Show every path: confirmation in the status label, a failed switch reported without a half-themed session, stale gained and lost, second session stays light | `ShowThemeState()`; the `catch` in `btnTheme_Click`; `RefreshTickets()`; two browsers |

## Self-check answers (lab guide)

- **A designer asks for a smaller corner radius on every metric card. Which file changes, and how many
  places if the radius had been a `CssStyle`?** One file, one value: the `radius` of the `card` appearance
  in `Themes/AdaptiveOps.theme` (it resolves `$borderRadius`). As an inline `CssStyle` it would be written on
  each card: several anonymous strings to find, none visible in the theme or the stylesheet.
- **Why does Save need no hover or pressed code, and what would you lose with `BackColor` / `ForeColor`?**
  `AppearanceKey = "action-button"` renders a theme appearance whose default, hovered, pressed and disabled
  states the theme engine switches as the browser reports the widget's state. With `BackColor` you get one
  static colour pair: no hover or press, the wrong disabled look, colours that beat every theme switch (the
  button stays blue on the dark copy), and no designer change without a build.
- **Two people are signed in and one switches to dark. What does each see, and which line guarantees it?**
  The one who clicked sees the dark console and `Theme: AdaptiveOps-Dark (this session)`, still dark after a
  reload (`Application.Session`). The other sees nothing change. The guarantee is
  `new ClientTheme(DarkThemeName, current)` in `SetSessionTheme`: the tokens are written to a copy and only
  `Application.Theme = copy` follows, the per-session theme. Writing `Application.Theme.Colors["surface"]`
  on the shared object would have put both people in the dark.
