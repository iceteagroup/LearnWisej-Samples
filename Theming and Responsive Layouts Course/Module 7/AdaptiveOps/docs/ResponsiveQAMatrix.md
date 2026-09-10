# Responsive QA matrix — Adaptive Operations Console

One row per scenario; every row is checked with the browser console open and the test-mode profile
indicator visible (`ADAPTIVEOPS_TESTMODE`, on by default under `dotnet run`). Widths are the browser's
CSS pixels; use the devtools device emulation for the phone and tablet rows (the profile is re-evaluated
on the next request/resize — refresh once after switching the device type). "Result" is what the running
app shows; the profile name is read from the status bar and the `ApplyProfile(...)` trace line.

Screenshots are taken by the learner with the file names in the last column.

## Profile × region × expected × result

| Scenario | Size | Expected profile | Toolbar | Rail | Cards | Grid | Trace | Details | Status | Result | Screenshot |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Desktop | 1440 × 900 | `Desktop` | text + icon, 6 commands, progress label fills | text, 220 px, 5 items | 1 row, 5 cards | 6 columns | right of grid (weight 2 of 5) | docked Right 340, TableLayoutPanel editor | browser size · theme AdaptiveOps · `profile: Desktop` | `ApplyProfile(Desktop) … rail text rail · details docked Right · cards 1 row(s) · grid 6/6 columns · toolbar Both · content Horizontal`; review 14/14 | `qa-desktop-1440.png` |
| Small desktop | 1024 × 768 | `Small Desktop` | text + icon | icon-only, 68 px, tooltips | 2 rows (3 + 2) | 5 columns (Owner hidden) | right of grid | docked Right 300, compact editor | `profile: Small Desktop` | `… rail icon-only rail · details docked Right · cards 2 row(s) · grid 5/6 columns · toolbar Both`; no horizontal overflow; review 14/14 | `qa-small-desktop-1024.png` |
| Tablet portrait | 768 × 1024 | `Tablet` | icon-only (40 px), tooltips | icon-only, 68 px | 2 rows | 5 columns | right of grid | **docked Bottom 300** under the grid (Min 240 / Max 360) | `profile: Tablet` | `… details docked Bottom · cards 2 row(s) · grid 5/6 columns · toolbar Icon`; review 14/14 incl. G9 with 11 icon-only buttons | `qa-tablet-portrait-768.png` |
| Tablet landscape | 1024 × 768 | `Tablet (Landscape)` | text + icon | icon-only, 68 px | 2 rows | 5 columns | right of grid | docked Right 300 | `profile: Tablet (Landscape)` | as Small Desktop; touch spacing 8 px between commands | `qa-tablet-landscape-1024.png` |
| Phone portrait | 390 × 844 | `Phone` | icon-only, title hidden | icon-only, 60 px | 3 rows (2 + 2 + 1) | 3 columns (Id, Title, Status) | **below** the grid (Flex Vertical) | hidden; **Open details** in the grid header opens the maximized dialog | `profile: Phone` | `… details hidden → Open details dialog · cards 3 row(s) · grid 3/6 columns · toolbar Icon · content Vertical`; dialog: `details for T-1042 opened as a modal DetailsDialog (maximized)`; review 14/14 | `qa-phone-portrait-390.png`, `qa-phone-dialog.png` |
| Phone landscape | 844 × 390 | `Phone (Landscape)` | icon-only | icon-only, 60 px | 1 row | 3 columns | right of grid (Flex Horizontal) | hidden → dialog (centered, not maximized) | `profile: Phone (Landscape)` | `… cards 1 row(s) · grid 3/6 columns · content Horizontal`; the primary command (Open details → Save) is reachable with Tab | `qa-phone-landscape-844.png` |

## Per-row checks (tick each)

For every row: layout (no region collapsed below its `MinimumSize`, no horizontal page scrollbar),
scrolling (the grid scrolls inside its card; the trace list scrolls; the phone dialog's Notes field grows),
focus (Tab walks toolbar → rail → grid → editor; the focus frame is visible on the current control),
invalid state (blank the title, Save: red 2 px border + tooltip, footer message with icon, banner, status
error), touch (every command ≥ 32 px, 8 px apart), console (no JavaScript errors, no designer errors).

| Row | Layout | Scrolling | Focus | Invalid state | Touch | Console clean |
|---|---|---|---|---|---|---|
| Desktop | ☐ | ☐ | ☐ | ☐ | n/a | ☐ |
| Small desktop | ☐ | ☐ | ☐ | ☐ | ☐ | ☐ |
| Tablet portrait | ☐ | ☐ | ☐ | ☐ | ☐ | ☐ |
| Tablet landscape | ☐ | ☐ | ☐ | ☐ | ☐ | ☐ |
| Phone portrait | ☐ | ☐ | ☐ | ☐ (in the dialog) | ☐ | ☐ |
| Phone landscape | ☐ | ☐ | ☐ | ☐ (in the dialog) | ☐ | ☐ |

## Automated pass: **Review all profiles**

The progress button runs the same matrix without an emulator: a `Wisej.Web.Timer` applies `Phone`,
`Tablet`, `Small Desktop` and `Desktop` in turn (simulated names through the same `ApplyProfile`), runs the
14 governance rules on each and logs one line per profile:

```
→ render review 1/4 · Phone: 14/14 rules (all green) · rail icon-only rail · 5 nav-item buttons · width 52 · details hidden → Open details dialog · cards 3 row(s) · grid 3/6 columns · toolbar Icon · content Vertical
→ render review 2/4 · Tablet: 14/14 rules (all green) · … details docked Bottom · cards 2 row(s) · grid 5/6 columns · toolbar Icon · content Horizontal
→ render review 3/4 · Small Desktop: 14/14 rules (all green) · … details docked Right · cards 2 row(s) · grid 5/6 columns · toolbar Both · content Horizontal
→ render review 4/4 · Desktop: 14/14 rules (all green) · rail text rail · … details docked Right · cards 1 row(s) · grid 6/6 columns · toolbar Both · content Horizontal
• server review across every profile finished: Review complete · Phone 14/14 · Tablet 14/14 · Small Desktop 14/14 · Desktop 14/14 · back on Desktop
```

It does not replace the emulator rows (only the browser proves touch scrolling and real widths), but it
is the part that reruns in seconds after every theme, CSS or profile change and after every upgrade.

## When to rerun

After any change to `Themes/src/AdaptiveOps.overrides.json`, `Styles/AdaptiveOps.css`,
`ClientProfiles.json` or `ApplyProfile`, and after every Wisej.NET package upgrade. The matrix is
repeatable, not archived.

## Evidence

- Status bar right side reads `profile: <name>` on every row; the same name appears in the `ApplyProfile(<name>)` trace line.
- Resizing across 1024 px logs `← client profile changed: Desktop → Small Desktop (Application.ResponsiveProfileChanged)` followed by one `ApplyProfile(Small Desktop) in n ms: …` line; resizing back logs the reverse. A refresh on the same profile logs `ApplyProfile(<name>) skipped: same profile, no work` after the first paint.
- Rotating the emulated phone logs `Phone → Phone (Landscape)`; the trace card moves from below the grid to its right.
