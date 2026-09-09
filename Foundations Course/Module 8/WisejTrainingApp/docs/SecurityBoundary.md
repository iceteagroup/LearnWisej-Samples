# Security boundary — what this page sends to the browser, what it never sends, and why

Anything assigned to `widStatus.Options` (or passed to `Call()`) is serialized to JSON and reaches the
browser; DevTools shows it. The Widget must therefore stay a *display* surface: C# prepares safe display
values and sends only what the gauge needs to draw. This note records the boundary for the Module 8 lab.

## 1. Bad pattern vs better pattern (lesson §1)

| Bad pattern | Better pattern | On this page |
|---|---|---|
| Business rules inside a large JavaScript block | Calculate business values in a C# service, send display values to the Widget | `StatusService.StatusFor(load)` decides `ok` / `warn` / `error`; `statusGauge.js` only maps the word to a colour class |
| Load random scripts from unknown sites | Use trusted libraries, version them, document why they are needed | No third-party script at all: one local JS file and one local CSS file, both in `Widgets/`, both `<Content>` in the project |
| Every page builds its own script strings in button clicks | Wrap integration code in one reusable helper / control | Files, not strings: `InitScript` is read from `statusGauge.js`; one `UpdateWidget(StatusInfo)` helper is the only code that talks to the widget after load |
| Expose credentials or internal server data to JavaScript | Send only safe display data | `Options` carries `percent`, `label`, `status` — nothing else (see §2) |

## 2. What this page sends — and what it never sends

**Sent to the widget (`widStatus.Options`) — the whole list:**

| Property | Type | Source | Why it is safe |
|---|---|---|---|
| `percent` | number 0–100 | `StatusInfo.Percent` (= system load) | A display figure the user already sees in `lblLoad` |
| `label` | string | `StatusInfo.Label` = "System load" | A caption |
| `status` | `"ok"` / `"warn"` / `"error"` | `StatusInfo.Status`, decided by `StatusService.StatusFor` | The *result* of the rule, not the rule |

Also sent, by design of the lab: the `gaugeClick` event goes **up** with `{ percent }` only.

**Never sent — stays in C#:**

| Value | Where it lives | How the page proves it |
|---|---|---|
| `ApiKey` (the monitoring credential) | `private readonly` field of `StatusService`, used only inside `CallMonitoringApi()` | It is not a property of `StatusInfo`, so `UpdateWidget` cannot copy it; the "Never sent to the browser" list on the data card names it |
| `ConnectionString` | `private readonly` field of `StatusService`, used only inside `ReadTicketCounters()` | same |
| The raw exception from a failed `GetStatus()` | the `catch` in `btnRefresh_Click` | `lblStatus` shows *Could not refresh status. Please try again.*; the exception type and message go to the event log only |
| The thresholds 60 / 85 | `StatusService.WarnFrom` / `ErrorAbove` | The JS never compares numbers; it receives the word |
| `Open` / `Closed` counters | `StatusInfo` → native labels only | The widget does not need them, so they are not in `Options` |

The `StatusInfo` model is the checkpoint: if a value is not a property there, it cannot reach the page;
if it is not one of the three names in the anonymous object inside `UpdateWidget`, it cannot reach the widget.

## 3. Third-party script risk checklist (lesson §2)

Every external script is a dependency that must be reviewed and maintained. Before adding one:

- [ ] **Trusted source** — a known library, not a script copied from a forum. Who publishes it? Is it maintained?
- [ ] **Local and versioned** — a file in the project (or a pinned version), so behaviour is predictable and the build is reproducible. (`Widget.Package` has an `Integrity` property for sub-resource integrity when a CDN is unavoidable.)
- [ ] **Documented** — what the script does and which screen uses it (this file, the README's "Where things live" tree).
- [ ] **No secrets in what it receives** — review the `Options` object and every `Call()` argument; nothing that is not display data.
- [ ] **Scoped CSS** — prefixed class names (`lw-gauge-…`) so the stylesheet cannot restyle the rest of the app.
- [ ] **Only the events you need** — one `fireWidgetEvent` name, one `if (e.Type == …)` in C#; unknown events are logged, never acted on.
- [ ] **Re-tested after changes** — theme changes (the page has a button for it), layout changes, browser updates, library updates.
- [ ] **A failure story** — what the user sees when the script or the service fails: a safe message, the last good values, a way to retry.

This module passes the checklist trivially because the "library" is a 60-line file we wrote; the
checklist matters the day someone proposes a charting library from a CDN.

## 4. Where each business rule lives (C#, one place each)

| Rule | Where | Called from |
|---|---|---|
| load `< 60` → `ok`; `60–85` → `warn`; `> 85` → `error` | `StatusService.StatusFor(int)` (+ the `WarnFrom` / `ErrorAbove` constants) | `StatusService.Build()` on every `GetStatus()` / `Refresh()` / `Set…()` |
| Which figure the gauge shows (`Percent` = system load, label "System load") | `StatusService.Build()` | same |
| How a refresh moves the data (deterministic step sequence, ticket counters drift) | `StatusService.Refresh()` | `btnRefresh_Click` |
| The three preset loads 35 / 72 / 93 | `StatusService.SetHealthy/SetWarning/SetCritical` | the three buttons |
| What the user sees on failure | `btnRefresh_Click` → `catch` | `Simulate service error` (sets `FailNextCall`) |
| What reaches the browser | `StatusPage.UpdateWidget` (the anonymous object) | every path above |

Nothing in `Widgets/statusGauge.js` decides anything: `render` clamps the number to 0–100 for drawing
safety and picks a class name from the word it was given. That is the whole of its "logic".

## Evidence

| Action | Event log | Screen |
|---|---|---|
| Page load | `… Options = { percent = 42, label = "System load", status = "ok" } …` | the "Sent to the widget" line shows exactly those three names; the "Never sent to the browser" list names `ApiKey`, `ConnectionString`, raw exception → *stays in C#* |
| **Simulate service error** | `btnSimulateError_Click → StatusService.FailNextCall = true → calling btnRefresh_Click`, `btnRefresh_Click → GetStatus() threw → widget keeps its last values → user sees the safe message`, `   server-side detail (log only, never sent to the widget): InvalidOperationException: Monitoring API returned 503 …` | `lblStatus` turns red: *Could not refresh status. Please try again.*; the gauge, `lblLoad` and `lblSent` are unchanged — nothing half-updated |
| **Refresh Server Data** after the error | `Widget refreshed with server data` | the recovery: the next sample arrives, the gauge moves and pulses, `lblStatus` is green/amber/red again according to the rule |
| Open DevTools → Network → the Wisej.NET response after a refresh | — | the JSON for `widStatus` contains `percent`, `label`, `status` and nothing else — the boundary is verifiable, not just documented |
