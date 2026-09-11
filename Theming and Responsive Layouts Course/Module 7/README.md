# AdaptiveOps · Theming and Responsive Layouts Course · Module 7

Local lab build for **Module 7 · Mobile-Ready Capstone, Accessibility, Performance, and Production
Governance**: the finished Adaptive Operations Console. It includes:

- the custom `AdaptiveOps` theme (tokens, semantic appearances, restyled focus frame, invalid state) and the scoped stylesheet;
- a shell of Dock, Flow, Table and Flex regions, with `MinimumSize` on every fill-weighted region;
- `ClientProfiles.json` with six profiles and one idempotent `ApplyProfile`;
- the phone editor as a modal dialog;
- a profile indicator shown in test mode only.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Theming and Responsive Layouts Course/Module 7/AdaptiveOps"
dotnet run -f net10.0 --urls http://localhost:5507
```

Open <http://localhost:5507> (or open `AdaptiveOps.slnx` in Visual Studio and press F5). The profile
indicator is shown while `ADAPTIVEOPS_TESTMODE` is unset or `1` (it is `1` in
`Properties/launchSettings.json`). Set it to `0` for a production run.

## What to check

| Action | What you should see |
|---|---|
| Open the page at desktop width | Status bar: `Ready`, `Width: N px`, `Profile: Desktop` |
| Select a ticket, blank the title, **Save** | Title editor with the danger border and the tooltip "Title is required."; footer "Rejected: Title is required." with the error icon; status "Not saved: …" |
| Save a valid ticket | Status "Saved T-…" and a toast; the grid and cards refresh |
| Tab from the page start | The focus frame walks toolbar, rail, grid, editor, **Save** |
| Resize below 1024 px | `Profile: Small Desktop`: icon-only rail, cards in 2 rows, Owner column hidden, compact editor |
| Devtools, tablet portrait 768 × 1024 (refresh once) | `Profile: Tablet`: icon-only toolbar and rail, details docked under the grid |
| Devtools, phone 390 × 844 (refresh once) | `Profile: Phone`: title "Adaptive Ops", details hidden, **Open details** opens the editor as a maximized dialog, grid shows Id, Title and Status |

A failure inside `ApplyProfile` is caught: the status bar says the layout could not be applied and the
last good profile is re-applied.

## Where things live

```
AdaptiveOps/
├─ Themes/AdaptiveOps.theme              shipped theme (generated: Bootstrap-4 plus overrides)
├─ Themes/src/AdaptiveOps.overrides.json the source we edit; merge-theme.mjs regenerates the theme
├─ Styles/AdaptiveOps.css                scoped app CSS (CssClass only), linked from Default.html
├─ ClientProfiles.json                   six profiles, narrow to broad
├─ MainPage.cs / .Designer.cs            shell, ApplyProfile, grid, save
├─ Shell/                                NavigationRail, DetailsEditor (TableLayoutPanel), MetricCard
├─ Dialogs/DetailsDialog.cs              the phone presentation of DetailsEditor
├─ Models/                               Ticket, TicketRepository (server-side validation)
└─ docs/                                 the lab deliverables
```

## Deliverables

1. Finished console: this project, described in [`docs/ArchitectureNote.md`](AdaptiveOps/docs/ArchitectureNote.md)
2. `ClientProfiles.json`, per-profile values and the idempotent handler: `ClientProfiles.json`, `MainPage.ApplyProfile`, `lblProfile`
3. [`docs/AccessibilityReview.md`](AdaptiveOps/docs/AccessibilityReview.md)
4. [`docs/ResponsiveQAMatrix.md`](AdaptiveOps/docs/ResponsiveQAMatrix.md)
5. [`docs/ArchitectureNote.md`](AdaptiveOps/docs/ArchitectureNote.md) and [`docs/GroundingPack.md`](AdaptiveOps/docs/GroundingPack.md)
6. [`docs/ProductionChecklist.md`](AdaptiveOps/docs/ProductionChecklist.md)

## Self-check answers (lab guide)

- **The handler runs twice for Tablet after a refresh. Which line guarantees the second run changes nothing?**
  The first line of `ApplyProfile`: `if (!force && string.Equals(name, _lastProfile, StringComparison.Ordinal)) return;`.
  The body also assigns final values (`detailsPanel.Visible = !phone`, `Dock = Bottom`), so even a re-run
  repeats the same layout. A toggling handler (`Visible = !Visible`) would flip the rail and the details
  region on every duplicate event.

- **A reviewer wants the focus frame removed from `action-button`. What do you change instead?**
  Restyle it, never delete it. Edit the `focusFrame` and `focusShadow` tokens (and `settings.focusBorderSize`)
  in `Themes/src/AdaptiveOps.overrides.json` and regenerate the theme. Then re-verify the **hovered** and
  **pressed** states, and check that **disabled** shows no frame.

- **A grid header tweak needs a selector into the widget's internal DOM. What do you try first?**
  The `table-header-cell` appearance, then a `CssClass` rule with inherited properties. A DOM selector is
  allowed only for a custom or third-party widget, scoped under a class you own, and written down in
  `docs/ArchitectureNote.md` layer 3 with the Wisej.NET version it was verified against. Rerun the QA
  matrix after the next upgrade.
