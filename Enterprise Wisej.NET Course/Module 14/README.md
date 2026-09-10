# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 14

Local lab build for **Advanced Module 14 · AI-Assisted Development, MCP-Ready Documentation & Capstone Delivery**.
It is the capstone package: a compact EnterpriseOps Command Center that ties the course together (KPIs and a work
queue from `EnterpriseOps.Services`, an approve command with a permission check, an optimistic version and an audit
line, a diagnostics/health card), plus a **Capstone Review** screen that presents the deliverables and checks them
in front of the reviewer — the AI prompt library, the generated-code review checklist (run live over an AI draft),
the MCP-shaped documentation index, the demo script, the defense deck outline and the production readiness
statement. No AI service is called: the "AI draft" is a committed sample and the review engine is deterministic.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Enterprise Wisej.NET Course/Module 14/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5214
```

Then open <http://localhost:5214>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.) The project multi-targets `net10.0-windows;net10.0`, so `dotnet run` needs `-f`.
Start it from the project folder: the Capstone Review reads `docs/` relative to `Application.StartupPath`.

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to click

### Command Center (`UI/CommandCenterDashboard`)

| Button | Path | What you should see |
|---|---|---|
| **⟳ Refresh** / status filter | success | `DashboardService` KPIs (open 33 · escalated 8 · due today 1 · overdue 5 for contoso) and the queue page; the dark footer names the service, the row count, the elapsed ms and the correlation id |
| select a row → **✓ Approve selected** | success / refusal | `ApproveWorkOrderCommand #id vN` → `Security: ApproveWorkOrder granted` → `update work_order → vN+1` → `audit ←` line; a `New` row is refused as data ("only InProgress or Escalated can be approved"), never as an exception |
| **▶ Run health check** / **■ Cancel** | progress | six probes stream in on a background task (session context, store, audit log, documentation index, capstone package, review engine) → `health check complete: 6/6 healthy` |
| **Fail: stale version** | failure | another session bumps the row's version, the same approve runs with the old one → `update rejected — … you saw v1, it is now v2` + an audit row with `false` |
| **Switch to ben.tech** → Approve | failure | `Security: ApproveWorkOrder denied` from the *service* (the button is only a convenience), audited |
| **Recover: reload queue** | recovery | the queue is re-read and the versions on screen are current again |
| **Capstone review →** | navigation | `Application.MainPage = CapstoneReviewPage` — same session, same services |

### Capstone Review (`UI/CapstoneReviewPage`)

| Button / tab | Path | What you should see |
|---|---|---|
| **✓ Verify package** | success | `CapstonePackageService` checks every deliverable on disk (file present, size, a marker heading) and the index: `12/12 checks pass, 0 required missing` |
| same button as ben.tech | failure | `Security: VerifyCapstonePackage denied — ben.tech (Technician)` — the Technician sees the screen, not the verdict |
| **Load AI draft #214** → **▶ Review generated code** | failure | the ten-rule checklist runs over the committed draft: `Rejected — 11 finding(s)` (static session state, undocumented Wisej API members caught against the 3,752-member catalog built from `Wisej.Framework.xml`, no permission check, a swallowed exception, the tenant taken from the caller, a background task without an `IsDisposed` guard, logic inside the handler) |
| **Load rev 2 (fixed)** → **▶ Review generated code** | recovery | `Accepted — 0 finding(s), 10 rules run` |
| **Sign the decision** | success | the decision is written to `docs/AIUsageNotes.md` (author `ai-assistant`, signed by `ana.ops`) and shown on the *AI usage notes* tab |
| **Fail: missing document** / **Recover: document written** | failure / recovery | the index gains a deliverable that has no file → the package check fails on that row; the recovery writes the file and the check passes again |
| **Documentation index** tab | reading | `docs/index.json` read back in MCP `resources/list` shape, every path resolved on disk |
| **← Command Center** | navigation | back to the dashboard, trace and session intact |

The right-hand card is the **Server · live activity trace**: every user action, every service decision, every
security verdict and every document check, with a timestamp and the correlation id, so the reviewer can see that
no screen decided anything.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | AI prompt library | [`EnterpriseOps/docs/PromptLibrary.md`](EnterpriseOps/docs/PromptLibrary.md) · read live by `PromptLibraryService` (the *AI prompt library* tab) |
| 2 | Generated-code review checklist | [`EnterpriseOps/docs/GeneratedCodeReviewChecklist.md`](EnterpriseOps/docs/GeneratedCodeReviewChecklist.md) · executed by `GeneratedCodeReviewService` + `DocumentedApiCatalog` (the *Generated-code review* tab) |
| 3 | Project documentation index | [`EnterpriseOps/docs/DocumentationIndex.md`](EnterpriseOps/docs/DocumentationIndex.md) + [`docs/index.json`](EnterpriseOps/docs/index.json) (MCP `resources/list` shape) · served by `DocumentationIndexService` |
| 4 | Capstone demo script | [`EnterpriseOps/docs/CapstoneDemoScript.md`](EnterpriseOps/docs/CapstoneDemoScript.md) — seven stops, each a working screen paired with its failure path |
| 5 | Architecture defense deck outline | [`EnterpriseOps/docs/DefenseDeckOutline.md`](EnterpriseOps/docs/DefenseDeckOutline.md) — twelve slides: one claim, one piece of evidence, one risk each |
| 6 | Production readiness statement | [`EnterpriseOps/docs/ProductionReadiness.md`](EnterpriseOps/docs/ProductionReadiness.md) |
| — | AI usage notes (accepted / rejected generated code) | [`EnterpriseOps/docs/AIUsageNotes.md`](EnterpriseOps/docs/AIUsageNotes.md) — appended by *Sign the decision* |
| — | AI-assisted review patterns | [`EnterpriseOps/docs/AIAssistedReviewPatterns.md`](EnterpriseOps/docs/AIAssistedReviewPatterns.md) |
| — | Capstone architecture diagram | [`EnterpriseOps/docs/capstone-package.svg`](EnterpriseOps/docs/capstone-package.svg) |

## Where things live

```
EnterpriseOps/
├─ Program.cs                       composition root: SessionContext → Application.Session → CommandCenterDashboard
├─ UI/  CommandCenterDashboard.cs (+.Designer.cs) · CapstoneReviewPage.cs (+.Designer.cs)
├─ Controls/ KpiTile.cs             the reusable KPI tile (Module 8 lesson applied)
├─ Domain/  WorkOrder · Capstone (deliverables, checks) · Review (findings, verdicts)
├─ Services/ DashboardService · WorkOrderService (approve = authorize → validate → write with version → audit)
│            WorkQueue (paged projection) · CapstonePackageService · PromptLibraryService
│            GeneratedCodeReviewService · GeneratedCodeSamples (PR #214 draft + rev 2) · DocumentedApiCatalog
│            DocumentationIndexService · SessionContext · CommandContext · CommandResult
├─ Security/ Permissions (Role → Permission) · AuditLog (append-only)
├─ Data/     InMemoryWorkOrderStore (120 rows / 3 tenants, optimistic Version) · DocsFolder (locates docs/)
├─ Diagnostics/ ActivityTrace · DiagnosticsService (the six probes)
└─ docs/     the deliverables above + index.json
```

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Finalize the capstone package | `Services/CapstonePackageService.cs` (`Verify`) + `docs/` — the *Capstone package* tab runs it |
| AI usage notes | `docs/AIUsageNotes.md`; `CapstoneReviewPage.btnSignDecision_Click` → `GeneratedCodeReviewService.RecordDecision` |
| Prompt library | `docs/PromptLibrary.md`; `Services/PromptLibraryService.cs` parses the header and the task prompts |
| Documentation links / index | `docs/index.json` + `docs/DocumentationIndex.md`; `Services/DocumentationIndexService.cs` (`resources/list`) |
| Code review checklist | `docs/GeneratedCodeReviewChecklist.md` (Q1–Q10) ⇄ `Services/GeneratedCodeReviewService.cs` (one rule per row) |
| Final demo script | `docs/CapstoneDemoScript.md` — every stop maps to a button in the two screens |
| Architecture defense deck outline | `docs/DefenseDeckOutline.md` |
| Production readiness statement | `docs/ProductionReadiness.md`; the *health check* card is its live evidence |

## Student review questions

- **Can the team explain every AI-generated line?** Yes — the review tab shows the draft, the rule that fired, the
  line and the reason, and the signed decision names who accepted rev 2 and why (`docs/AIUsageNotes.md`).
- **Is the documentation usable by a new maintainer and by a tool?** The Markdown index is for the person, the
  JSON index is the same list in `resources/list` shape; the app verifies both against the disk on every check.
- **Does the capstone still follow the architecture baseline?** Screens call services; `WorkOrderService.ApproveAsync`
  authorizes, validates, writes with the expected version and audits before it returns a typed result — the trace
  shows each step.

## Instructor acceptance criteria

| Criterion | Evidence |
|---|---|
| Follows the course architecture baseline | folder-per-layer namespaces, per-session `SessionContext`, no static session state (checklist rule Q2 rejects it) |
| UI event handlers remain thin and explainable | every handler in both pages is a few lines that build a command and call a service |
| Service-level logic reviewable without the designer | `Services/*` have no control references; `GeneratedCodeReviewService` is exercised from the page but runs on plain strings |
| At least one failure path demonstrated | stale version, Technician denial, AI draft rejected on 11 findings, missing deliverable |
| State ownership / security / production behaviour explained | `docs/ProductionReadiness.md` answers the four questions; the health check and the audit log show the running answers |

## Verified / unverified

Built and run in the browser on this machine (Wisej-4 4.1.0, .NET 10, 2026-09-10): dashboard load, refresh, health
check (6/6), Technician denial, package verification (12/12), the AI draft rejected on 11 findings, rev 2 accepted,
the signed decision on the *AI usage notes* tab, the missing-document failure and its recovery, and navigation both
ways. `dotnet build -nologo -v q` passes for both target frameworks with 0 warnings.

Cookbook items marked (unverified) that this sample relies on: `Application.Session.<name>` as the per-session bag
holding the `SessionContext`; `DataGridView` bound to a `List<T>` with explicit columns; `Application.StartupPath`
as the first candidate for locating `docs/` (with `AppContext.BaseDirectory` walk-up as the fallback).
