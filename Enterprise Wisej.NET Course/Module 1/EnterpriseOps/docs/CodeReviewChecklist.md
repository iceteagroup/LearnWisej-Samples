# Deliverable 5 — Code review checklist, applied to the first screen

The gate a change passes before it merges. Part 1 is the machine-checkable half; Part 2 is what a human still has
to read. Rule ids refer to [`CodingStandards.md`](CodingStandards.md).

The checklist is **applied below to `UI/CommandCenterDashboard.cs`**, the first screen of the course — a filled-in
review, not a blank form.

---

## Part 1 — The automated gate

`ReviewGate.CheckEventHandler(handlerName, approximateLines, callsService)` in
[`Architecture/ArchitectureGovernancePatterns.cs`](../Architecture/ArchitectureGovernancePatterns.cs) is the
lesson's code, verbatim. Two rules:

| Rule | Fails when | Standard |
|---|---|---|
| Handler size | more than 20 lines from signature to closing brace | S-1 |
| Service call | the handler body contains no call into a service | S-1, S-2 |

Run it for every event handler a change touches (by hand, from a unit test, or from a CI script that counts each
handler's lines and looks for a service call).

### Result on the first screen — `UI/CommandCenterDashboard.cs`

| Handler | Lines | Calls a service | Issues |
|---|---:|---|---|
| `CommandCenterDashboard_Load` | 5 | yes — `LoadScreen()` → `_workflow.LoadAsync` | 0 |
| `btnRefresh_Click` | 17 | yes — `_workflow.RefreshAsync` | 0 |

**2 handlers · 0 issues → PASS — may merge.**

### Result on the counter-example — `Architecture/Samples/OrderEntryLegacy.cs.txt`

| Handler | Lines | Calls a service | Issues |
|---|---:|---|---|
| `btnSubmitOrder_Click` | 74 | **no** | 2 |

```
✕ btnSubmitOrder_Click is too large; move decisions into a service.
✕ btnSubmitOrder_Click does not call an application service; verify business logic location.
```

**1 handler · 2 issues → FAIL — blocked before merge, not found in production.** The fix is architectural: move
the workflow into `EnterpriseOps.Services/Workflow` and the handler shrinks to the shape in `ReferenceScreen.md`.

---

## Part 2 — What a human still reads

Ticked against `UI/CommandCenterDashboard.cs` + `Services/Workflow/DashboardWorkflow.cs` at this commit.

### Boundaries

- [x] **N-4** — one workflow per screen, findable by name: `Services/Workflow/DashboardWorkflow.cs`.
- [x] **S-2** — the screen changes text, colour, `DataSource`, `Enabled`, `Visible`; it counts nothing.
      `ShowResult` assigns `result.Kpis.*` — it does not compute them.
- [x] **S-4** — no role test in the screen. The signed-in label only displays the role.
- [x] **S-5** — `Data/` and `Integrations/` types appear only in the constructor (the composition root).
- [x] **A-4** — the grid is bound to `List<IncidentRow>`, never to `WorkOrder`.
- [x] **A-3** — `DashboardWorkflow.RunAsync` calls the policy first; on denial it returns before the feed and the
      repository are touched.

### Failure paths

- [x] Permission denied is a typed result (`Succeeded = false` + user-safe reason) → amber banner, data untouched.
- [x] An exception from the integration or the store → red banner with `UiText.ActionFailed` plus
      `(ref <correlationId>)`; the exception type and message go to `ErrorLog` only. Grep for `ex.Message` in
      `UI/`: none.
- [x] The governance failure path — the review gate rejecting a fat handler — is the counter-example above.
- [x] The screen stays usable after a failure — Refresh is re-enabled in `finally`, tiles and grid keep their
      last good values.

### Session, state and production behaviour

- [x] **A-7** — no `static` field holds a user, a tenant or a feed. `SessionContext` is created in
      `Program.Main`, stored in `Application.Session.Context`, and passed to the screen's constructor.
- [x] **E-4** — one correlation id per action, minted in `BeginBusy` → `SessionContext.BeginCommand()`, carried
      on every `CommandResult` and every log entry, and shown to the user in the failure banner.
- [x] **S-7** — every `async void` handler ends with `Application.Update(this)` in `EndBusy()`, and Refresh is
      restored in `finally` even when the service threw.
- [x] **A-6** — every layer writes to the activity log (`Diagnostics/ActivityTrace` → `System.Diagnostics.Trace`).

### Designer and standards

- [x] **S-3** — layout is entirely in `CommandCenterDashboard.Designer.cs`; the code-behind adds no controls.
- [x] **S-6** — `CommandCenterDashboard()` gives the Designer a default `SessionContext`, so the page renders in
      the Designer without a session.
- [x] **N-2** — `btnRefresh`, `dgvIncidents`, `kpiSlaAtRisk`, `lblBanner`, `lblStatusBar` … no `button1`, no
      `label3`.
- [x] **A-5** — the SLA threshold is `DashboardWorkflow.SlaWarningWindow`, not a `4` in a handler.
- [x] **D-3** — no connection string, host name or environment literal in `UI/`, `Services/` or `Data/`.

### Documentation and debt

- [x] The ADR that this change obeys exists and is linked: `Architecture/ADR-001-SolutionStructure.md`,
      with an owner and a review date (2027-03-09).
- [x] **T-1** — the one accepted shortcut (the screen constructor as composition root) is written down in
      ADR-001 and in `SolutionStructure.md`, with the module that removes it (Module 3).
- [x] **T-3** — the anti-pattern is kept as a non-compiled sample with a header explaining the fix.

---

## Reviewer's verdict on the first screen

**Approved.** Automated gate: 2 handlers, 0 issues. Manual pass: no boundary violation; failure paths handled
without leaking internals; state ownership, security and production behaviour all explainable.

## How to run this checklist on your own screen

1. For every handler in your screen, feed its name, line count and "calls a service?" to `ReviewGate.CheckEventHandler`.
2. Walk Part 2 against the screen and its workflow service.
