# Field-mode usability checklist

**Deliverable 5 of Module 13.** Gloves, glare, one hand, no signal. Applied to
`UI/FieldTechnicianPage` — every row says whether the sample passes and where.

## A. The device and the hand holding it

| # | Check | Sample |
|---|---|---|
| A1 | Touch targets ≥ 44 px on a phone, ≥ 40 px on a tablet | ✅ `ApplyDeviceLayout()` sets `btnComplete.Height` / `btnScan.Height` from `DeviceInfo.TouchTargetHeight` |
| A2 | Primary action reachable with one thumb, at the bottom of the content | ✅ **Complete…** is the first control in `pnlFieldActions`, docked to the bottom of the cache card |
| A3 | No hover-only affordances | ✅ every action is a button; tooltips add detail, they never carry it |
| A4 | No drag, no double-click, no right-click in the field flow | ✅ single taps only |
| A5 | Layout works at phone width without horizontal scrolling | ✅ frame narrows to 380 px; `Site`, `v` and the scan result are hidden rather than squeezed |
| A6 | Text remains legible in daylight glare | ⚠️ partial — the sample uses the course's light theme with 9–14 pt and strong contrast, but a real field build needs a high-contrast theme switch (`Application.LoadTheme`) |
| A7 | Confirmation the technician can feel, not only read | ✅ `IDeviceServices.VibrateAsync()` on a queued completion |

## B. Connectivity is a first-class state

| # | Check | Sample |
|---|---|---|
| B1 | Connection state is visible at all times, without asking | ✅ the `ONLINE` / `OFFLINE` pill in the field header |
| B2 | Going offline never blocks the workflow | ✅ **Complete…** queues instead of failing; no modal, no spinner, no error |
| B3 | The technician is told *what happened to their work*, in their words | ✅ "No signal — WO-1041 is queued locally and will sync when you reconnect. It is not lost and it is not applied." |
| B4 | Pending work is countable at a glance | ✅ queue title: `COMPLETION QUEUE — 2 PENDING OF 3 · 1 942 BYTES ON THE DEVICE` |
| B5 | Every pending item shows its own state, not a global spinner | ✅ one card per command with a coloured `SyncState` pill |
| B6 | Sync progress is visible and paced, not a frozen screen | ✅ `Application.StartTask` + `Application.Update` push one step per command (700 ms) into `prgSync` and the status strip |
| B7 | A failed sync is recoverable without losing anything | ✅ the queue is untouched on failure; **Sync now** retries |
| B8 | Cache age is visible | ✅ `WORK ORDERS — LOCAL CACHE (SQLITE) · 12 ROWS · DOWNLOADED 09:14` |

## C. Data honesty

| # | Check | Sample |
|---|---|---|
| C1 | Never show local intent as if it were server fact | ✅ two columns: **Server status** (`Assigned`) and **On this device** (`Completed · pending sync`) |
| C2 | Conflicts show both versions with who and when | ✅ `SyncConflictPanel` |
| C3 | No silent overwrite, ever | ✅ resolution is an explicit choice; the override needs a permission |
| C4 | Rejected work stays visible with the reason | ✅ the card keeps the server's message; it is not deleted |
| C5 | The technician can enter what they did before it matters | ✅ completion notes are required — an empty note is rejected locally with a reason |

## D. Field realities

| # | Check | Sample |
|---|---|---|
| D1 | Nothing depends on a keyboard | ⚠️ partial — the notes field needs typing. A production build adds voice-to-text and canned reasons behind the same command. |
| D2 | Scanning replaces typing where it can | ✅ **Scan** appends the asset tag to the notes |
| D3 | Scanned/GPS values are treated as hints, never as proof | ✅ validated on the server; a mismatch warns instead of blocking |
| D4 | The screen survives an app kill mid-shift | ⚠️ by design, not in this sample — the store is in-memory here; the production `LocalStore.Sqlite` makes the queue survive |
| D5 | Battery: no polling loop while idle | ✅ no `Timer`; the replay runs only when the technician reconnects or presses **Sync now** |
| D6 | A wiped or doubted device recovers by re-provisioning | ✅ **Recover** wipes the local store and re-downloads cache + permissions |

## E. Security, from the technician's point of view

| # | Check | Sample |
|---|---|---|
| E1 | Local data is encrypted at rest | ✅ modelled (`LocalStore.EncryptedAtRest`), traced on every cache download |
| E2 | Logout / lock-out destroys local data | ✅ `LocalStore.Wipe()` — cache, queue and permission snapshot |
| E3 | Permissions are refreshed on reconnect, not merged | ✅ `SyncWorkflow.RefreshPermissions()` replaces the snapshot |
| E4 | A revoked user's queued work is rejected, not applied | ✅ **Revoke ben.tech's permission** → the queue replays into `Rejected` with an audit entry |
| E5 | The technician sees why something was refused | ✅ the server's message reaches the banner, the toast and the card |

## Scoring the checklist

25 checks: **21 pass**, **4 partial** (A6 high-contrast theme, D1 keyboard-free entry, D4 durable store,
plus C-series items that depend on the production `LocalStore.Sqlite`). The four partials are all
"production build" items, and each names what would close it — which is the point of a checklist that is
applied rather than filed.

## Evidence — what the running app shows

Walk the checklist with the app open: switch **Simulate device** to *Phone* (A1, A5), press **Go
offline** (B1, B2), complete two work orders (B3, C1), press **Go online** (B5, B6), resolve the conflict
(C2, C3), then **Revoke ben.tech's permission** and sync again (E4, E5).
