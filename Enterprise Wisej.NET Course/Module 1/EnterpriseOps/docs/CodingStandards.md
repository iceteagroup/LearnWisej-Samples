# Deliverable 3 — Team coding standards (EnterpriseOps)

One page. Every rule has an id (used by review comments and by
[`CodeReviewChecklist.md`](CodeReviewChecklist.md)), a reason, and a line of this codebase that shows it.
Rules follow from [ADR-001](../Architecture/ADR-001-SolutionStructure.md); changing one means amending the ADR.

Governance without bureaucracy: a developer may add a button without asking anyone. These rules only say
**where things live** and **what a screen is not allowed to decide**.

---

## N — Naming

| Id | Rule | Why | Example in this sample |
|---|---|---|---|
| N-1 | Screens are `<Noun><Kind>`: `…Dashboard`, `…Page`, `…Wizard`, `…Dialog`. One `.cs` + one `.Designer.cs`. | The file name predicts the screen the user sees. | `UI/CommandCenterDashboard.cs` |
| N-2 | Every control is named for its job with a type prefix — `btn`, `lbl`, `txt`, `cbo`, `dgv`, `lst`, `pnl`, `chk`, `kpi`. **Never `button1`.** | A reviewer reads the handler without opening the Designer. | `btnRefresh`, `dgvIncidents`, `lblStatusBar`, `kpiSlaAtRisk` |
| N-3 | Handlers are `<control>_<Event>`: `btnRefresh_Click`. Nothing else calls them. | Grep for a button, find its behaviour. | `btnRefresh_Click` |
| N-4 | One workflow service per screen, named `<Screen minus its kind>Workflow`, in `Services/Workflow/`. | Answers the review question "find the workflow in under a minute" with a folder listing. | `CommandCenterDashboard` → `Services/Workflow/DashboardWorkflow.cs` |
| N-5 | Commands are `<Verb><Noun>Command`, results `<Noun>Result`, projections `<Noun>Row`. | The type name says whether it goes in or comes out. | `DashboardResult`, `IncidentRow`, `DashboardKpis` |
| N-6 | Folder = namespace, always. A file in `Security/` is in `EnterpriseOps.Security`. | The `using` list of a file is its dependency list, readable at a glance. | every file in this project |

## S — Screens

| Id | Rule | Why | Example |
|---|---|---|---|
| S-1 | An event handler is **≤ 20 lines** and makes **one** call into a service. The shape is fixed: `try { var result = await _service.XAsync(ctx); ShowResult(result); } catch (Exception ex) { _log.Error(ex); AlertBox.Show(generic); }`. | It is the rule the review gate can check mechanically. | `btnRefresh_Click` — 17 lines |
| S-2 | Screens own **UI state** only: text, colour, enabled, visible, selection, which card is showing. They own no rule, no threshold, no query, no permission. | "Designer-friendly boundaries" is UI state vs application decisions — not visual vs coded. | `ShowResult` paints; it never counts |
| S-3 | Layout lives in `.Designer.cs` and stays Designer-editable. Never hand-edit it to add behaviour; never build the layout in code. | The Designer is a Wisej.NET advantage; keeping it usable is a deliberate constraint, not an accident. | `CommandCenterDashboard.Designer.cs` |
| S-4 | A screen never tests a role or a permission (`if (user.Role == …)`). It shows what a policy decided. | One place to audit, one place to change, no screen that "forgot". | `DashboardPolicy.CanViewCommandCenter` → the screen only reads `result.Succeeded` |
| S-5 | A screen never touches `Data/` or `Integrations/` types outside its constructor. | Otherwise the boundary exists only on paper. | the dashboard's constructor is the only place it sees `OperationsFeed` |
| S-6 | Every screen gives the Designer a parameterless constructor with sane defaults. | A screen that cannot be instantiated cannot be designed. | `CommandCenterDashboard() : this(SessionContext.CreateDefault())` |
| S-7 | After an `await`, push changes with `Application.Update(this)`; reset button state in `finally`. | The request that started the handler is gone by then. | `EndBusy()` |

## A — Application services

| Id | Rule | Why | Example |
|---|---|---|---|
| A-1 | A service method takes a `CommandContext` (tenant · user · role · correlation id) and returns a `CommandResult` subclass. It never reads session state on its own. | Testable without a browser; every log line correlates. | `DashboardWorkflow.RefreshAsync(CommandContext)` |
| A-2 | **Expected** failures — validation, policy, stale version — are `Succeeded = false` plus user-safe `Errors`. **Unexpected** failures are exceptions. | The screen can tell "you may not" from "something broke". | `DashboardResult.Denied(...)` vs an exception from the feed or the store |
| A-3 | Security decides first. If the policy denies, nothing else runs — no integration call, no query. | A denial that still hits the database is a leak and a bill. | `RunAsync` step 1 returns `DashboardResult.Denied` before the feed and the repository |
| A-4 | Domain entities never cross into the UI. Services project them into rows/DTOs. | The grid cannot accidentally write to the store, and the projection can change without touching the entity. | `IncidentRow`, built in step 4 |
| A-5 | Thresholds and business constants are named, `public` and on the service — not literals in a handler. | Reviewable, and the same number everywhere. | `DashboardWorkflow.SlaWarningWindow = 4 hours` |
| A-6 | Every layer writes one log line for the decision it took, tagged with its layer. | Production issues can be followed layer by layer without a debugger. | `_trace.Service(…)`, `_trace.Data(…)`, `_trace.Security(…)` |
| A-7 | Services are **per-session instances**, created at a composition root. Never `static` state that holds a user, a tenant or a connection. | Every browser session shares the process. | `Program.Main` → `SessionContext`, then the screen's constructor |

## D — Data, integrations, deployment

| Id | Rule | Why | Example |
|---|---|---|---|
| D-1 | Persistence sits behind an interface in `Data/`; services depend on the interface. | Module 4 swaps in EF Core with no service change. | `IWorkOrderRepository` |
| D-2 | Every query is scoped to a tenant inside the repository — there is no "all tenants" overload. | Tenant isolation cannot be forgotten by a caller. | `QueryAsync(string tenantId, …)` |
| D-3 | No connection string, host name, path or environment name as a literal in `UI/`, `Services/` or `Data/`. Configuration comes from `Default.json` / `appsettings.{Environment}.json`; deployment assets live in `Deployment/`. | The same binary must run in three environments. | the violation is `Architecture/Samples/OrderEntryLegacy.cs.txt` (`Server=ops-sql01;…` in a click handler) |
| D-4 | External systems live behind a class in `Integrations/`, replaceable by a fake. | The workflow can be tested, including its failure path, without the real system. | `OperationsFeed` |

## E — Errors, text, diagnostics

| Id | Rule | Why | Example |
|---|---|---|---|
| E-1 | A user message never contains an exception type, a stack trace, a host name or SQL. It carries a correlation id instead. | Security, and a support call that can be traced. | `UiText.ActionFailed` + `(ref 7f3a1c92)` |
| E-2 | User-facing strings live in `Resources/`, not inline in handlers. | Localisation later, consistency now. | `Resources/UiText.cs` |
| E-3 | Every caught exception is logged with its correlation id before the generic message is shown. | Otherwise the id in the banner leads nowhere. | `_log.Error(ex, CurrentContext.CorrelationId)` |
| E-4 | One correlation id per user action, minted at the start of the command and shown to the user when it fails. | The log and the user's screenshot are joinable. | `SessionContext.BeginCommand()` → `(ref …)` in the failure banner |

## T — Technical debt controls

| Id | Rule | Why |
|---|---|---|
| T-1 | A shortcut merges only with an ADR or a dated `// DEBT:` note naming an owner. Undated debt is invisible debt. |
| T-2 | Every ADR has a review date; the sweep is a calendar item. |
| T-3 | A deliberately-bad example is kept as a non-compiled sample (`.cs.txt`) with a comment saying what is wrong and what the fix would be — teams learn faster from the anti-pattern than from the rule. |
| T-4 | The review gate runs on the branch, not after the release. |

---

## What this page is not

It says nothing about braces, `var`, or line width — `.editorconfig` and the formatter own that, and arguing
about it in review is the bureaucracy this page exists to avoid. Every rule here is about **where a decision
lives**, which is the only thing that is expensive to change later.
