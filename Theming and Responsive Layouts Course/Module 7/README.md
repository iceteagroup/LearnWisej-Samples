# AdaptiveOps · Theming & Responsive Layouts Course · Module 7

Local lab build for **Module 7 · Mobile-Ready Capstone, Accessibility, Performance, and Production
Governance**. It is the finished Adaptive Operations Console the walkthrough video reviews "across every
profile": the custom `AdaptiveOps` theme (tokens, semantic appearances, restyled focus frame, invalid
state), the scoped stylesheet, a shell of Dock + Flow + Table + Flex regions with `MinimumSize` on every
fill-weighted region, `ClientProfiles.json` with six profiles and one idempotent `ApplyProfile`, the
phone editor as a modal dialog, a test-mode profile indicator — plus the capstone additions the lab asks
for: an accessibility pass you can operate (tab order, focus frame, tooltips on icon-only buttons,
validation announced as text), performance evidence (a "Layout cost" card, `SuspendLayout` timing,
a lazily created Reports view) and a **Governance review** dialog that computes the production checklist
against the running application.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 7/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5507
```

Then open <http://localhost:5507>. (Visual Studio: open `AdaptiveOps.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The profile
indicator in the status bar is shown while `ADAPTIVEOPS_TESTMODE` is unset or `1` (it is `1` in
`Properties/launchSettings.json`); set it to `0` for a "production" run without the indicator.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Open the page at desktop width | startup | Status bar: `● ready · governance green` · `Browser 1348 × 680 px · Desktop` · `theme: AdaptiveOps` · `profile: Desktop`. Trace: `shell built …`, `theme: AdaptiveOps — custom theme from Themes/AdaptiveOps.theme …`, `ApplyProfile(Desktop) in n ms: rail text rail · details docked Right · cards 1 row(s) · grid 6/6 columns · toolbar Both · content Horizontal`, `layout cost (page load): N controls · toolbar … · rail … · workspace … · details … · status …`, `governance review at startup: 14/14 rules pass` |
| **Governance review** | success | The dialog "All 14 rules pass — the console is release-ready on the Desktop profile." with one row per rule (G1 theme … G14 governed files) and its computed evidence; every row also lands in the trace |
| **Review all profiles** | progress | A `Wisej.Web.Timer` applies `Phone → Tablet → Small Desktop → Desktop` (900 ms apart), runs the 14 rules on each and logs `review k/4 · <profile>: 14/14 rules (all green) · rail … · details … · cards … · grid … · toolbar … · content …`; the shell visibly changes at every step and returns to the real profile at the end |
| **Inject violation** | failure | Trace lines starting with `✕ injected:` — `cardOpen.BackColor`, `cardOverdue.CssStyle`, `cardMine.CssClass += legacy-card`, `Application.LoadTheme("Bootstrap-4")` (global for every session), and `✕ armed: the next ApplyProfile throws once`. The review dialog reopens with `✕ FAIL` on G1, G2, G5, G6, G7; the status label is in the `error` state; a banner explains. Now press **Review all profiles**: the first step logs `ApplyProfile(Phone) FAILED half-way … re-applying Desktop` with the friendly banner, then the run continues |
| **Recover** | recovery | `ResetBackColor()`, CssStyle cleared, CssClass restored, `LoadTheme("AdaptiveOps")`, failure disarmed, repository reset; the review dialog is green again |
| **Bulk fill** | performance | `bulk fill 240 rows: plain a ms · SuspendLayout/ResumeLayout(true) b ms (Stopwatch, server side …)`; the grid then reloads the 12 tickets |
| Rail → **Reports** | performance | First time: `Reports view created lazily: +N controls (M data cells) in x ms · total now T` and the **LAYOUT COST** card rises; second time: `reused, re-bound … no controls created` |
| Hover the **LAYOUT COST** card | performance | Tooltip: `Layout cost: T controls of a 400 budget (p%). toolbar … · rail … · workspace … · details … · status … · created since load +k` — the bar is two Percent columns of a `TableLayoutPanel` |
| Select a ticket, blank the title, **Save** | validation | Title editor: 2 px danger border + tooltip "Title is required." (theme `invalid` state + `InvalidMessage`); footer label "Rejected: Title is required." with the error icon; banner; status `● validation error`. Select another ticket to clear |
| Tab from the page start | accessibility | Focus frame (3 px `focusFrame` border + halo) walks toolbar → rail → grid → trace → editor → **Save** in the documented order; on the blue action buttons the frame is white |
| Resize below 1024 px | profiles | `← client profile changed: Desktop → Small Desktop`, then one `ApplyProfile(Small Desktop)` line: rail icon-only, cards in 2 rows, Owner column hidden, compact editor |
| Devtools → tablet portrait 768 × 1024 (refresh once) | profiles | `profile: Tablet`: icon-only toolbar and rail, details **docked under the grid** |
| Devtools → phone 390 × 844 (refresh once) | profiles | `profile: Phone`: details hidden, **Open details** in the grid header opens the editor as a maximized dialog (`… opened as a modal DetailsDialog (maximized)`), cards 2 per row, grid with Id · Title · Status, trace below the grid |
| Clear trace | – | empties the trace list |

## Where things live

```
AdaptiveOps/
├─ Themes/
│  ├─ AdaptiveOps.theme                  the shipped theme (generated: Bootstrap-4 ⊕ overrides)
│  └─ src/AdaptiveOps.overrides.json     the source we edit: tokens, fonts, settings, appearances
│     └─ merge-theme.mjs                 node merge-theme.mjs <Bootstrap-4.theme> regenerates ../AdaptiveOps.theme
├─ Styles/AdaptiveOps.css                scoped app CSS (CssClass only), linked from Default.html
├─ ClientProfiles.json                   six profiles, narrow → broad; Content, copied to the output
├─ Default.json / Web.config             "theme": "AdaptiveOps"
├─ MainPage.cs / .Designer.cs            the shell: Dock regions, toolbar Flow, cards Flow, grid/trace Flex; ApplyProfile; lab buttons
├─ Shell/
│  ├─ NavigationRail.cs (.Designer)      rail-surface + nav-item buttons; IconOnly; selected state
│  ├─ DetailsEditor.cs (.Designer)       TableLayoutPanel editor; Invalid/InvalidMessage + validation label; Compact
│  ├─ MetricCard.cs (.Designer)          metric-card appearance + metric-card CSS class
│  └─ LayoutCostCard.cs (.Designer)      controls-per-region card with a TableLayoutPanel percent bar
├─ Views/ReportsView.cs (.Designer)      created lazily on first navigation
├─ Dialogs/
│  ├─ DetailsDialog.cs (.Designer)       the phone presentation of DetailsEditor (modal, maximized)
│  └─ GovernanceDialog.cs (.Designer)    the checklist grid + summary
├─ Governance/
│  ├─ GovernanceReview.cs                rules G1–G14 computed at runtime
│  └─ ControlTree.cs                     tree walk, counts, ShouldSerializeBackColor/ForeColor/Font by reflection
├─ Models/                               Ticket, TicketRepository (TicketValidationException now names the field)
└─ docs/                                 the lab deliverables (below)
```

## Deliverables

1. **Finished Adaptive Operations Console** — this project: theme + semantic appearances + scoped CSS + Flow/Table/Flex regions, described in [`AdaptiveOps/docs/ArchitectureNote.md`](AdaptiveOps/docs/ArchitectureNote.md)
2. **ClientProfiles.json, per-profile values and the idempotent handler with a test-mode indicator** — [`AdaptiveOps/ClientProfiles.json`](AdaptiveOps/ClientProfiles.json), `MainPage.ApplyProfile`, `lblProfile`; the per-profile table is in the architecture note (layer 6)
3. **Accessibility review** — [`AdaptiveOps/docs/AccessibilityReview.md`](AdaptiveOps/docs/AccessibilityReview.md) (focus frames, invalid states, contrast ratios, touch targets, tab order, hidden content)
4. **Responsive QA matrix** — [`AdaptiveOps/docs/ResponsiveQAMatrix.md`](AdaptiveOps/docs/ResponsiveQAMatrix.md) (six rows, profile × region × expected × result, screenshots by the learner)
5. **Architecture note and AI grounding pack** — [`AdaptiveOps/docs/ArchitectureNote.md`](AdaptiveOps/docs/ArchitectureNote.md), [`AdaptiveOps/docs/GroundingPack.md`](AdaptiveOps/docs/GroundingPack.md)
6. **Production checklist with evidence** — [`AdaptiveOps/docs/ProductionChecklist.md`](AdaptiveOps/docs/ProductionChecklist.md), executable as `Governance/GovernanceReview.cs`

## Lab step → code map

| Lab step | Where |
|---|---|
| Open the project, confirm the status label shows the profile and the theme loads | `MainPage_Load` traces theme and test mode; `lblProfile`, `lblTheme` in the status bar |
| Audit the theme: derives from a base theme, named tokens, ≥ 4 semantic appearances via `AppearanceKey`, no stray `BackColor`/`ForeColor`/`Font` | `Themes/src/AdaptiveOps.overrides.json` (`inherit`, `colors`, `fonts`, 20 appearances); every label/card/button sets `AppearanceKey` in the Designers; rules G1–G5 |
| Harden the shell: `MinimumSize` on workspace, grid and fill-weighted children, `MaximumSize` on details, no `AutoSize` hierarchies, Flow filter bar and Table editor survive resize | `MainPage.Designer.cs` (`workspacePanel`, `gridCard`, `gridTickets`, `traceCard`, `detailsPanel`), `ApplyProfile` (details Min/Max per dock), rule G12 |
| One `ApplyProfile(name)` with early return and final values; called from the constructor and the handler; unsubscribe in `Dispose`; `lblProfile` only in test mode | `MainPage.ApplyProfile`, constructor, `Application_ResponsiveProfileChanged`, `MainPage.Designer.cs Dispose`, `TestMode` |
| Accessibility review: Tab order, focus frame on `action-button` and the invalid editor state, icon or message for invalid, `ToolTipText` on icon-only buttons, touch targets | `TabIndex` in every Designer; theme `focused`/`invalid` states; `DetailsEditor.ShowValidationError`; `InitToolbarButton`/`InitNavButton`; `docs/AccessibilityReview.md` |
| Responsive QA matrix, six rows | `docs/ResponsiveQAMatrix.md`; **Review all profiles** automates the checklist part |
| Architecture note and grounding pack | `docs/ArchitectureNote.md`, `docs/GroundingPack.md` |
| Show every path: indicator on every profile, invalid state with icon + message, keyboard reaches the primary command, a failure inside `ApplyProfile` reports friendly | status bar; Save with a blank title; Tab to **Save**; **Inject violation** then **Review all profiles** |
| Review & run: console open across the matrix, theme/CSS/profiles committed with reviewed diffs | `docs/ProductionChecklist.md` section F; `Themes/src` is the reviewable diff |

## Self-check answers (lab guide)

- **Your `ResponsiveProfileChanged` handler runs twice for the Tablet profile after a browser refresh. Which line guarantees the second run changes nothing, and what would the user see if it were missing?**
  The first line of `ApplyProfile`: `if (!force && string.Equals(name, _lastProfile, StringComparison.Ordinal)) return;` — the
  second run logs `ApplyProfile(Tablet) skipped: same profile, no work` and touches nothing. Even without
  the guard the body assigns final values (`detailsPanel.Visible = !phone`, `Dock = Bottom`,
  `metricsFlow.Height = 184`), so a re-run would repeat the same layout at the cost of one more
  `SuspendLayout/ResumeLayout` pass. What would break is a *toggling* handler
  (`navRail.Visible = !navRail.Visible`): the user would see the rail vanish and the details region jump
  between Right and Bottom on every duplicate event, and a refresh would leave the shell in whichever state
  the last flip produced. The governance review proves the property with rule G13: it re-runs
  `ApplyProfile` with `force: true` and compares two snapshots of every value the method assigns.

- **A reviewer asks you to remove the focus frame from `action-button` because it "looks dated". What do you change instead, which token drives it, and which two other states do you re-verify afterwards?**
  Restyle, never delete: the `focused` state of `action-button` in `Themes/src/AdaptiveOps.overrides.json`
  keeps its `shadowSpreadRadius: "$focusBorderSize"` and `shadowColor: "focusShadow"` and sets the border
  `color` to `white` so the frame reads on the filled blue button; the geometry comes from
  `settings.focusBorderSize` (3) and `focusBlurRadius` (0), the colours from the `focusFrame` and
  `focusShadow` tokens — so a better-looking frame is a token edit, regenerated with `merge-theme.mjs`.
  Afterwards re-verify in the Theme Builder (or by hovering/pressing in the running app) the **hovered**
  and **pressed** states, because `action-button` inherits `button` and overrides the background per state:
  a new focus shadow must still be visible when the button is hovered (`brandPrimaryHover`) and pressed
  (`brandPrimaryPressed`), and the **disabled** state must not show a frame at all. Rule G3 fails if the
  `focused` state disappears from `button` or `action-button`.

- **The ticket grid header needs a tweak that only a selector on the widget's internal DOM can reach. What do you try first, what must be true before the selector is allowed, and what do you write down?**
  First the theme: the header is the `table-header-cell` appearance (states `default`, `hovered`, `sorted`,
  components `label`, `sort-icon`, `icon`), and this theme already restyles it — background `surfaceAlt`,
  border `borderSoft`, `cardTitle` font, `textMuted` text, padding. Most "header tweaks" (colour, font,
  padding, sort icon, hover) are theme properties or component styles there; the second attempt is a
  `CssClass` on the grid with a rule that uses inherited properties only. A selector into the widget's
  internal DOM is allowed only when the target is a custom or third-party widget the theme does not model,
  the selector is scoped under a class you own (`.tickets-grid …`, never a global `.qx-table-header`), and
  the reason is written down. What you record, in `docs/ArchitectureNote.md` layer 3 and in
  `docs/ProductionChecklist.md` section B: the selector, why the theme and `CssClass` could not own it,
  the Wisej.NET version it was verified against, and the instruction to rerun the responsive QA matrix and
  the governance review after the next upgrade so a changed widget DOM is caught before release.
