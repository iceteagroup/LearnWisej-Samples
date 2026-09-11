# Responsive QA matrix: Adaptive Operations Console

One row per scenario, checked with the browser console open and the test-mode profile indicator visible
(`ADAPTIVEOPS_TESTMODE`, on under `dotnet run`). Use the devtools device emulation for the phone and tablet
rows and refresh once after switching the device type. The profile name is read from the status bar.
Screenshots are taken by the learner with the file names in the last column.

## Profile × region × expected

| Scenario | Size | Expected profile | Toolbar | Rail | Cards | Grid | Details | Screenshot |
|---|---|---|---|---|---|---|---|---|
| Desktop | 1440 × 900 | `Desktop` | title and Refresh with text | text, 220 px | 1 row | 6 columns | docked Right 340 | `qa-desktop-1440.png` |
| Small desktop | 1024 × 768 | `Small Desktop` | text and icon | icon-only, 68 px | 2 rows | 5 columns (no Owner) | docked Right 300, compact | `qa-small-desktop-1024.png` |
| Tablet portrait | 768 × 1024 | `Tablet` | icon-only Refresh | icon-only, 68 px | 2 rows | 5 columns | **docked Bottom 300** (Min 240, Max 360) | `qa-tablet-portrait-768.png` |
| Tablet landscape | 1024 × 768 | `Tablet (Landscape)` | text and icon | icon-only, 68 px | 2 rows | 5 columns | docked Right 300 | `qa-tablet-landscape-1024.png` |
| Phone portrait | 390 × 844 | `Phone` | "Adaptive Ops", icon-only Refresh | icon-only, 60 px | 2 rows | 3 columns (Id, Title, Status) | hidden; **Open details** opens the maximized dialog | `qa-phone-portrait-390.png`, `qa-phone-dialog.png` |
| Phone landscape | 844 × 390 | `Phone (Landscape)` | icon-only | icon-only, 60 px | 1 row | 3 columns | hidden; the dialog is centered | `qa-phone-landscape-844.png` |

## Per-row checks

For every row: layout (no region below its `MinimumSize`, no horizontal page scrollbar), scrolling (the
grid scrolls inside its card; the dialog's Notes field grows), focus (Tab walks toolbar, rail, grid,
editor; the focus frame is visible), invalid state (blank the title and Save: red border, tooltip, footer
message with icon), touch (every command at least 32 px, 8 px apart), console (no JavaScript errors).

| Row | Layout | Scrolling | Focus | Invalid state | Touch | Console clean |
|---|---|---|---|---|---|---|
| Desktop | ☐ | ☐ | ☐ | ☐ | n/a | ☐ |
| Small desktop | ☐ | ☐ | ☐ | ☐ | ☐ | ☐ |
| Tablet portrait | ☐ | ☐ | ☐ | ☐ | ☐ | ☐ |
| Tablet landscape | ☐ | ☐ | ☐ | ☐ | ☐ | ☐ |
| Phone portrait | ☐ | ☐ | ☐ | ☐ (in the dialog) | ☐ | ☐ |
| Phone landscape | ☐ | ☐ | ☐ | ☐ (in the dialog) | ☐ | ☐ |

## When to rerun

After any change to `Themes/src/AdaptiveOps.overrides.json`, `Styles/AdaptiveOps.css`,
`ClientProfiles.json` or `ApplyProfile`, and after every Wisej.NET package upgrade.
