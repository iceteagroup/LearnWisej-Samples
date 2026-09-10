# Profile notes — how each Client Profile changes the Ticket Workspace

*Module 3 deliverable · "profile screenshots or notes" · TicketOps Console*

Screenshots are taken by the learner while running the sample (`desktop.png`, `tablet.png`,
`phone-list.png`, `phone-task.png`); the notes below say what each one must show and how to
reproduce it — with the real browser and with the on-demand preview.

## `ClientProfiles.json` (project root, copied to `/bin`)

```json
{
  "profiles": [
    { "name": "Phone",   "maxWidth": 600 },
    { "name": "Tablet",  "minWidth": 601, "maxWidth": 1024 },
    { "name": "Desktop", "minWidth": 1025 }
  ]
}
```

- `minWidth` / `maxWidth` are **browser** widths in CSS pixels, so resizing a desktop browser switches
  profiles — that is what makes the lab testable without a phone. (`minScreenWidth` / `maxScreenWidth`
  would match the *device* width instead; `device` and `userAgent` match strings or regexes.)
- Profiles are matched top to bottom, first match wins: order them narrow → broad.
- Names matching the framework's built-in profiles (`Phone`, `Tablet`) **override** them; the framework
  also ships `Phone (Landscape)`, `Tablet (Landscape)`, `Small Desktop` (Desktop device, ≤ 1024 px) and
  `Default`. If a reviewer ever sees one of those names in the strip, the workspace treats it as unknown,
  falls back to the desktop layout and says so in the banner — the layout never just breaks.
- The csproj copies the file next to the binaries for both targets:
  `<Content Update="ClientProfiles.json"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></Content>`.
  The video shows `"default": true` and `"theme"` keys on the profiles; the framework's profile
  properties are `name`, `minWidth`, `maxWidth`, `minScreenWidth`, `maxScreenWidth`, `device`,
  `userAgent`, `landscape`, so those two are left out here.
- `Startup.cs` never serves `*.json`, so the file is not downloadable from the browser.

`Infrastructure/ClientProfileCatalog` reads the same file back so the screen can print
`Phone ≤ 600 · Tablet 601–1024 · Desktop ≥ 1025` and tell a defined name from an undefined one.

## Desktop — `desktop.png` (browser ≥ 1025 px, e.g. 1400 × 760)

- Header **TicketOps — Ticket Workspace**, toolbar with the ticket SearchBar and **+ New Ticket**.
- Navigation rail 150 px with **▦ Dashboard · ◳ Tickets · ▤ Reports · ⚙ Settings**.
- Ticket list (weight 3) with **Id · Title · Priority · Assignee · Status** and the six chips in one row.
- Side pane (weight 2): **TICKET DETAIL** card above the **ACTIVITY** card (3 : 2), both visible.
- Status bar: `Active profile: Desktop`. Profile strip above the workspace: `Active profile: Desktop · browser 1400×760 px · device Desktop · Phone ≤ 600 · Tablet 601–1024 · Desktop ≥ 1025`.
- Frame: trace card on the right, one-row button bar.

## Tablet — `tablet.png` (browser 601–1024 px, e.g. 900 × 760)

- Navigation rail collapses to **56 px icons**; toolbar stays.
- List and side pane split **1 : 1**; the **Assignee column is hidden** (a server-side `Visible`).
- The side pane is a **TabControl**: **Details | Activity**. Switch to Activity: the same feed lines
  (same `ListBox`, re-parented — nothing was reloaded).
- Chips wrap to a second line in the narrower card.
- Status bar: `Active profile: Tablet`. Frame: the trace card moves **below** the workspace.

## Phone — `phone-list.png` and `phone-task.png` (browser ≤ 600 px, e.g. 400 × 760)

- `phone-list.png`: header **Tickets**, no navigation, no toolbar; the list fills the width with
  **Id · Title · Priority** only; chips wrap.
- Tap a row → `phone-task.png`: header **← Back  Ticket #1003**, the Details | Activity tabs fill the
  screen, Save / Close at the bottom of the detail card. **← Back** returns to the list; the draft typed
  in Title is still there when the row is opened again.
- Status bar: `Active profile: Phone`. Frame: the trace card is hidden, the button bar wraps to 3–4 rows.

## Without resizing — the preview path

The bottom bar's **Preview as desktop / tablet / phone** applies the arrangement to the workspace in a
device-like column (700 px for tablet, 400 px for phone) while the browser keeps its real profile; the
banner says `Previewing the phone layout — the browser is really on the Desktop profile`, the strip adds
`preview pinned: Phone`, and real `ResponsiveProfileChanged` events are logged but not applied until
**Live — follow the browser** is chosen again. **▶ Tour all profiles** steps through all three with a
Timer and returns to Live.

## Round trip = proof of "moved, not rebuilt"

1. Select **#1003**, type `Safari 17 regression` into Title, click the **High** chip.
2. Switch Desktop → Tablet → Phone → Desktop (resize, or Tour).
3. Title still reads `Safari 17 regression`, #1003 is still selected, the chip is still on, the activity
   feed still lists the same events; the trace shows `state kept (moved, not rebuilt): selected #1003,
   draft title "Safari 17 regression", …` before each switch.
