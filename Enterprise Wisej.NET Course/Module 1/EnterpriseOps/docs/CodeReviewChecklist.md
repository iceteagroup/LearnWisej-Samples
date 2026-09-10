# Deliverable 5 — Code review checklist, applied to the first screen

The gate a change passes before it merges. Part 1 is the machine-checkable half (it runs in the app); Part 2 is
what a human still has to read. Rule ids refer to [`CodingStandards.md`](CodingStandards.md).

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

`Architecture/ReviewGateService.cs` runs it over a **real file**: it finds every `(object sender, … e)` handler,
counts lines by brace depth, and looks for `_field.Method(`, a `*Service` type, or an `…Async(` call.
Two buttons on the dashboard run it live.

### Result on the first screen — `UI/CommandCenterDashboard.cs`

| Handler | Lines | Calls a service | Issues |
|---|---:|---|---|
| `CommandCenterDashboard_Load` | 9 | yes — `_trace`, `LoadScreen()` | 0 |
| `btnRefresh_Click` | 19 | yes — `_workflow.RefreshAsync` | 0 |
| `btnFeedDown_Click` | 6 | yes — `_feed.SetAvailable` | 0 |
| `btnFeedRestore_Click` | 6 | yes — `_feed.SetAvailable` | 0 |
| `btnSwitchUser_Click` | 8 | yes — `_session.SignInAs` | 0 |
| `btnGateLegacy_Click` | 17 | yes — `_gate.InspectAsync` | 0 |
| `btnGateScreen_Click` | 17 | yes — `_gate.InspectAsync` | 0 |
| `btnClearTrace_Click` | 4 | yes — `_trace.Clear` | 0 |

**8 handlers · 0 issues → PASS — may merge.** Click **Review gate: this screen** to reproduce it.

`btnRefresh_Click` at 19 lines is the closest to the limit and is worth watching: the next line of UI state added
to it must move into `BeginBusy`/`ShowResult`, not into the handler. That is the rule doing its job.

### Result on the counter-example — `Architecture/Samples/OrderEntryLegacy.cs.txt`

| Handler | Lines | Calls a service | Issues |
|---|---:|---|---|
| `btnSubmitOrder_Click` | 74 | **no** | 2 |

```
✕ btnSubmitOrder_Click is too large; move decisions into a service.
✕ btnSubmitOrder_Click does not call an application service; verify business logic location.
```

**1 handler · 2 issues → FAIL — blocked before merge, not found in production.** Click
**Review gate: legacy handler**. The fix is architectural: move the workflow into
`EnterpriseOps.Services/Workflow` and the handler shrinks to the shape in `ReferenceScreen.md`.

---

## Part 2 — What a human still reads

Ticked against `UI/CommandCenterDashboard.cs` + `Services/Workflow/DashboardWorkflow.cs` at this commit.

### Boundaries

- [x] **N-4** — one workflow per screen, findable by name: `Services/Workflow/DashboardWorkflow.cs`.
- [x] **S-2** — the screen changes text, colour, `DataSource`, `Enabled`, `Visible`; it counts nothing.
      `ShowResult` assigns `result.Kpis.*` — it does not compute them.
- [x] **S-4** — no role test in the screen. The only `UserRole` mention is `btnSwitchUser_Click` choosing *which
      user to sign in as*, which is the demo's input, not a permission decision.
- [x] **S-5** — `Data/` and `Integrations/` types appear only in the constructor (the composition root) and in
      `_feed.SetAvailable`, which is the failure-path switch the lab asks for. Noted and accepted in review.
- [x] **A-4** — the grid is bound to `List<IncidentRow>`, never to `WorkOrder`.
- [x] **A-3** — `DashboardWorkflow.RunAsync` calls the policy first; on denial it returns before the feed and the
      repository are touched. Verified in the trace: `stopped: policy denied — no feed pull, no query`.

### Failure paths

- [x] At least one failure path is demonstrated — three are: integration outage (exception), permission denied
      (typed result), and the review gate rejecting a fat handler.
- [x] Each has a recovery: feed restored, sign back in as `ana.ops`, gate re-run on the compliant screen.
- [x] **A-2** — denial is `Succeeded = false` with a user-safe reason; the outage is an exception. The screen
      distinguishes them: amber banner vs red banner.
- [x] **E-1 / E-3** — the outage message the user sees is `UiText.ActionFailed` plus `(ref <correlationId>)`.
      `ops-feed.internal:8443` and the exception type go to `ErrorLog` only. Grep for `ex.Message` in `UI/`: none.
- [x] The screen stays usable after a failure — buttons are re-enabled in `finally`, tiles and grid keep their
      last good values.

### Session, state and production behaviour

- [x] **A-7** — no `static` field holds a user, a tenant, a feed or a trace. `SessionContext` is created in
      `Program.Main`, stored in `Application.Session.Context`, and passed to the screen's constructor.
- [x] **E-4** — one correlation id per action, minted in `BeginBusy` → `SessionContext.BeginCommand()`, shown in
      `lblCorrelation`, carried on every `CommandResult` and every log entry.
- [x] **S-7** — every `async void` handler ends with `Application.Update(this)` in `EndBusy()`, and buttons are
      restored in `finally` even when the service threw.
- [x] **A-6** — every layer traces: load produces `Architecture:` → `UI →` → `Service:` → `Security:` → `Data:`
      → `Service:` → `UI ←`. A reviewer can prove the handler was thin without a debugger.

### Designer and standards

- [x] **S-3** — layout is entirely in `CommandCenterDashboard.Designer.cs`; the code-behind adds no controls.
- [x] **S-6** — `CommandCenterDashboard()` gives the Designer a default `SessionContext`, so the page renders in
      the Designer without a session.
- [x] **N-2** — `btnRefresh`, `dgvIncidents`, `lstTrace`, `kpiSlaAtRisk`, `lblBanner`, `pnlActions` … no
      `button1`, no `label3`.
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

**Approved.** Automated gate: 8 handlers, 0 issues. Manual pass: no boundary violation; three failure paths with
recoveries; state ownership, security and production behaviour all explainable from the running screen's trace.

One follow-up, non-blocking: `btnRefresh_Click` is 19 of 20 permitted lines. Before the next change to it, move
its UI-state lines into `BeginBusy`/`ShowResult`.

## How to run this checklist on your own screen

1. Open the module and press **Review gate: this screen** — it inspects `UI/CommandCenterDashboard.cs`.
2. Point `SourceFiles` (bottom of `Architecture/ReviewGateService.cs`) at your file and press it again.
3. Walk Part 2 with the trace card open; every line you tick should have a trace line behind it.
