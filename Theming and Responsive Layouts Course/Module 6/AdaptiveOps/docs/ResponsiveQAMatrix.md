# Deliverable · Responsive QA matrix

One row per scenario the lab names, with the profile the status bar must show and the checks that prove
the **behaviour** (not just the width) is right. Test **both ways**: drag the browser for the width rules,
use devtools device emulation (+ **refresh after every device change**) for the device / orientation rules.
Every screenshot the lab asks for is taken by the learner and named in the last column.

## Scenarios

| # | Scenario | Window | Orientation | How to reach it | Expected `lblProfile` | Screenshot |
|---|---|---|---|---|---|---|
| 1 | Desktop | 1440 × 900 | landscape | plain browser window | `profile: Desktop · browser 1440 × 900 px · Desktop` | `qa-01-desktop.png` |
| 2 | Small desktop | 1024 × 768 | landscape | drag the window to ≤ 1024 px wide (no reload needed) | `profile: Small Desktop · browser 1024 × 768 px · Desktop` | `qa-02-small-desktop.png` |
| 3 | Tablet portrait | 768 × 1024 | portrait | devtools → iPad (768×1024) → refresh | `profile: Tablet · browser 768 × 1024 px · Tablet` | `qa-03-tablet.png` |
| 4 | Tablet landscape | 1024 × 768 | landscape | rotate the emulated iPad (no refresh needed) | `profile: Tablet (Landscape) · browser 1024 × 768 px · Tablet` | `qa-04-tablet-landscape.png` |
| 5 | Phone portrait | 390 × 844 | portrait | devtools → iPhone 12 Pro → refresh | `profile: Phone · browser 390 × 844 px · Mobile` | `qa-05-phone.png` |
| 6 | Phone landscape | 844 × 390 | landscape | rotate the emulated iPhone | `profile: Phone (Landscape) · browser 844 × 390 px · Mobile` | `qa-06-phone-landscape.png` |
| 7 | Back to desktop | 1440 × 900 | landscape | close the device toolbar → refresh | `profile: Desktop …` — one trace line per change on the way | `qa-07-back.png` |

## Profile × region × expected behaviour × how to test

| Region | Desktop | Small Desktop | Tablet | Tablet (Landscape) | Phone | Phone (Landscape) | How to test |
|---|---|---|---|---|---|---|---|
| Toolbar | 8 labelled buttons + full title | icon-only buttons | icon-only | icon-only | icon-only + **☰ Menu**, title hidden | icon-only + ☰, short title | hover an icon → tooltip is the label; tab through → accessible names read; nothing wraps or overflows the 56 px bar |
| Navigation | full rail 220 px, five labels | full rail | **icon-only rail 64 px** | icon-only 64 px | **hidden**; ☰ opens a 5-item menu | hidden; ☰ menu | click *Reports* in the rail / in the menu → workspace title "Reports", trace `← client navigation: Reports …` |
| Details editor | docked Right 340 px | docked Right 300 px | **docked UNDER the grid**, 320 px, scrolls | docked Right 300 px | **modal dialog**, maximized, opens on row tap | modal dialog, centred | select a row; on phones the dialog opens once; Close returns to the grid; typed text survives Desktop ↔ Phone |
| Validation feedback | red line in the editor + banner + toast | same | same (scroll if needed) | same | **red line inside the modal** (banner hidden behind) | same | blank the title → Save → `✖ Title is required.` visible where the editor is; grid unchanged |
| Success feedback | green line + toast + grid refresh | same | same | same | green line **inside the modal** + toast | same | Save a valid ticket → `✓ T-… saved on the server` |
| Metric cards | 4 × 1 | **2 × 2** | 4 × 1 | 4 × 1 | **1 × 4** stacked | 4 × 1 | count rows; no card clipped; values still readable |
| Ticket grid | 6 columns | 6 columns | Owner hidden | 6 columns | **Owner, Due hidden** | Owner, Due hidden | header row; hidden data still visible in the editor |
| Status bar | status · profile+size · theme | same | same | same | status (110 px) · profile+size; theme hidden | same | label always names the profile; no ellipsis on the profile name |
| Horizontal overflow | none | none (≥ 900 px) | none | none | none | none | page has no horizontal scrollbar; `workspacePanel` keeps `MinimumSize 320×240`, `gridTickets` `280×120` |
| Trace | one `• server profile change …` + one `→ render profile …` per change | same | same | same | same | same | count the pairs: dragging 1440 → 1000 → 1440 gives exactly two |
| Touch / scroll | – | – | rail icons are 32 px targets in a 36 px row; the details card scrolls by touch | same | grid scrolls by touch; dialog fields are 28 px tall with 10 px gaps | same | tap targets reachable without zoom |
| Font scaling | theme fonts only (no per-profile Font in this module) | same | same | same | same | same | browser zoom 125 %: labels ellipsise, nothing overlaps |

## Event checks (the acceptance criteria that are not visual)

| Check | How | Pass when |
|---|---|---|
| Subscribed once, applied once at startup | open the app, read the first trace lines | exactly one `• server profile change (constructor)` line |
| One log line per change | drag 1440 → 1000 → 1440 | two `Application.ResponsiveProfileChanged` changes, two `→ render profile` lines, and two `← client Page.ResponsiveProfileChanged too` lines |
| Idempotent handler | Phone button five times in a row, or rotate the emulated phone four times | one editor, one dialog; trace says `editor dialog already open — reused, not opened again` |
| Unsubscribed in Dispose | read `MainPage.Designer.cs` `Dispose(bool)` | both `Application.*` events removed; `_editorForm.Dispose()` |
| Unknown profile | **Unknown** button | banner `✖ Profile "Kiosk" does not exist …`, layout unchanged, trace lists the six known names |
| Partial failure | **Faulty region** button | banner `✖ 1 region(s) failed while applying "…"; the other 5 applied`, trace `• server region "metrics" failed: injected fault …`; toolbar, rail, details, grid, status still applied |
| Recovery | **Re-apply** | banner cleared, `• server recovery (Re-apply button): Application.ActiveProfile re-read → "Desktop"`, "(simulated)" gone from the status bar |
| Progress | **Step profiles** | status counts `stepping 1/7 · Desktop` … `7/7 · Desktop`, one profile change per 1.5 s, then a recovery line |

## Known limits

- **Small Desktop below ~900 px** (a desktop browser dragged to, say, 700 px): the profile is right, but the
  shell (rail 220 + details 300 + workspace ≥ 320) is wider than the window and the page scrolls horizontally.
  A production console would add a `minWidth` to `Small Desktop` and a `Compact Desktop` profile above it, or
  treat narrow desktop windows like tablets. Documented rather than hidden.
- `Small Desktop` has no `minWidth`, so a 1024-px **desktop** window and a 1024-px **tablet** report different
  profiles (`Small Desktop` vs `Tablet (Landscape)`): the device rule wins because it is listed first. That is
  intended — the tablet gets the icon-only rail for touch, the small desktop keeps the full rail.

## Evidence

What the running app shows while the matrix is walked (learner screenshots are named above; the trace card
is the evidence that does not need a screenshot):

- Rows 1 → 2 → 1 by dragging: exactly two `• server profile change (Application.ResponsiveProfileChanged)` lines,
  two `→ render profile` lines and two `← client Page.ResponsiveProfileChanged too` lines; rows 3–6 through
  emulation each show the expected `lblProfile` text after the refresh / rotation described.
- Row 5: tapping `T-1036` opens the maximized dialog (`→ render editor dialog opened (row selected on a phone) …`);
  blanking the title and saving keeps the red line inside the dialog; Close returns to the stacked cards and the
  four-column grid.
- The Phone / Tablet / Desktop buttons reproduce rows 5, 3 and 1 in a desktop window with "(simulated)" in the
  status bar, and Re-apply restores row 1 with `• server recovery (Re-apply button): Application.ActiveProfile re-read → "Desktop"`.
