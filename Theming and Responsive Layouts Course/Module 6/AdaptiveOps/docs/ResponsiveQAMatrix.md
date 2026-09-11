# Responsive QA matrix

One row per scenario the lab names, with the profile the status bar must show and the checks that prove the
behaviour is right. Drag the browser for the width rules; use devtools device emulation (and **refresh after every
device change**) for the device and orientation rules. Screenshots are taken by the learner.

## Scenarios

| # | Scenario | Window | How to reach it | Expected `lblProfile` | Screenshot |
|---|---|---|---|---|---|
| 1 | Desktop | 1440 × 900 | plain browser window | `Profile: Desktop · 1440 px` | `qa-01-desktop.png` |
| 2 | Small desktop | 1024 × 768 | drag the window to ≤ 1024 px | `Profile: Small Desktop · 1024 px` | `qa-02-small-desktop.png` |
| 3 | Tablet portrait | 768 × 1024 | devtools → iPad → refresh | `Profile: Tablet · 768 px` | `qa-03-tablet.png` |
| 4 | Tablet landscape | 1024 × 768 | rotate the emulated iPad | `Profile: Tablet (Landscape) · 1024 px` | `qa-04-tablet-landscape.png` |
| 5 | Phone portrait | 390 × 844 | devtools → iPhone 12 Pro → refresh | `Profile: Phone · 390 px` | `qa-05-phone.png` |
| 6 | Phone landscape | 844 × 390 | rotate the emulated iPhone | `Profile: Phone (Landscape) · 844 px` | `qa-06-phone-landscape.png` |

## Checks per region

| Region | Desktop | Small Desktop | Tablet (both) | Phone (both) | How to test |
|---|---|---|---|---|---|
| Toolbar | labelled buttons, full title | icon-only | icon-only | icon-only, ☰ Menu, short title | hover an icon: the tooltip is the label; nothing overflows the 56 px bar |
| Navigation | full rail, 220 px | full rail | icon-only rail, 64 px | hidden; ☰ opens the five sections | pick *Reports* in the rail or the menu: the workspace heading reads "Reports" |
| Details editor | docked right, 340 px | docked right, 300 px | docked under the grid, 320 px, scrolls | modal dialog, opens on row tap | on a phone the dialog opens once; Close returns to the grid; typed text survives Desktop ↔ Phone |
| Validation feedback | red line in the editor | same | same | red line inside the modal | clear the Title, Save: the message is visible where the editor is; the grid is unchanged |
| Success feedback | success line, toast, grid refresh | same | same | success line inside the modal, toast | save a valid ticket |
| Metric cards | 4 × 1 | 4 × 1 | 4 × 1 | 2 × 2 | no card clipped |
| Ticket grid | 6 columns | 6 columns | 6 columns | Owner and Due hidden | hidden data is still in the editor |
| Horizontal overflow | none | none (≥ 900 px) | none | none | no horizontal page scrollbar |
| Profile log | one line per change | same | same | same | dragging 1440 → 1000 → 1440 adds exactly two lines |

## Event checks

| Check | How | Pass when |
|---|---|---|
| Applied once at startup | open the app | the profile log shows one `Startup: …` line |
| One log line per change | drag 1440 → 1000 → 1440 | two lines: `Desktop → Small Desktop`, `Small Desktop → Desktop` |
| Idempotent handler | rotate the emulated phone four times | one editor, one dialog |
| Unsubscribed in Dispose | read `MainPage.Designer.cs` `Dispose(bool)` | both `Application.*` events removed; the dialog disposed |
| Re-apply | **Re-apply** button | the layout for `Application.ActiveProfile` is applied again, nothing changes when it already matches |

## Known limits

- A desktop browser dragged below about 900 px is still `Small Desktop`: the shell (rail 220 + details 300 +
  workspace ≥ 320) is then wider than the window and the page scrolls horizontally. A production console would
  add a `minWidth` to `Small Desktop` or treat narrow desktop windows like tablets.
- A 1024-px desktop window and a 1024-px tablet report different profiles (`Small Desktop` vs `Tablet (Landscape)`):
  the device rule decides, as intended.
