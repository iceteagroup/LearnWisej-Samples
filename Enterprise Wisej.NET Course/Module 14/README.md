# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 14

Local lab build for **Advanced Module 14 · AI-Assisted Development, MCP-Ready Documentation & Capstone Delivery**.
It is the capstone package: a compact EnterpriseOps Command Center (KPIs and a work queue from
`EnterpriseOps.Services`, an approve command with a permission check, an optimistic version and an audit line, a
diagnostics card), plus a **Capstone Review** screen that presents and checks the deliverables — the AI prompt
library, the generated-code review checklist (run over an AI draft), the MCP-shaped documentation index, the
demo script, the defense deck outline and the production readiness statement. No AI service is called: the
"AI draft" is a committed sample and the review engine is deterministic.

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

| Control | What you should see |
|---|---|
| **⟳ Refresh** / status filter (`cboStatus`) | `DashboardService` KPIs (open · escalated · due today · overdue · done in 7 days) and the queue page; the dark footer names the service, the row count, the elapsed ms and the correlation id |
| select a row → **✓ Approve selected** | the service authorizes, validates, writes with the row's version and audits; the footer shows the audit line. A `New` row is refused as data ("only InProgress or Escalated can be approved"), a stale version as a red banner — never as an exception |
| **▶ Run health check** / **■ Cancel** | six probes fill in one at a time (session context, store, audit log, documentation index, capstone package, review engine), cancellable |
| **Capstone review →** | `Application.MainPage = CapstoneReviewPage` — same session, same services |

### Capstone Review (`UI/CapstoneReviewPage`)

| Tab / control | What you should see |
|---|---|
| **Capstone package** → **✓ Verify package** | `CapstonePackageService` checks every deliverable on disk (file present, size, a marker heading) and the index: *ready to submit* |
| **AI prompt library** | the prompts from `docs/PromptLibrary.md`, each expanded with the project-rules header |
| **Review checklist** | the ten questions from `docs/GeneratedCodeReviewChecklist.md` |
| **Generated-code review** → **Load AI draft #214** → **▶ Review generated code** | the failure path: `REJECTED` with eleven findings (static session state, undocumented Wisej API members caught against the catalog built from `Wisej.Framework.xml`, no permission check, a swallowed exception, …) |
| **Load rev 2 (fixed)** → **▶ Review generated code** | `ACCEPTED — no checklist finding` |
| **Sign the decision** | the decision is recorded (author `ai-assistant`, signed by `ana.ops`) and shown on the **AI usage notes** tab |
| **Documentation index** | `docs/index.json` read back in MCP `resources/list` shape, every path resolved on disk |
| **← Command Center** | back to the dashboard, session intact |

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | AI prompt library | [`EnterpriseOps/docs/PromptLibrary.md`](EnterpriseOps/docs/PromptLibrary.md) · read by `PromptLibraryService` |
| 2 | Generated-code review checklist | [`EnterpriseOps/docs/GeneratedCodeReviewChecklist.md`](EnterpriseOps/docs/GeneratedCodeReviewChecklist.md) · executed by `GeneratedCodeReviewService` + `DocumentedApiCatalog` |
| 3 | Project documentation index | [`EnterpriseOps/docs/DocumentationIndex.md`](EnterpriseOps/docs/DocumentationIndex.md) + [`docs/index.json`](EnterpriseOps/docs/index.json) · served by `DocumentationIndexService` |
| 4 | Capstone demo script | [`EnterpriseOps/docs/CapstoneDemoScript.md`](EnterpriseOps/docs/CapstoneDemoScript.md) |
| 5 | Architecture defense deck outline | [`EnterpriseOps/docs/DefenseDeckOutline.md`](EnterpriseOps/docs/DefenseDeckOutline.md) |
| 6 | Production readiness statement | [`EnterpriseOps/docs/ProductionReadiness.md`](EnterpriseOps/docs/ProductionReadiness.md) |
| — | AI usage notes (accepted / rejected generated code) | [`EnterpriseOps/docs/AIUsageNotes.md`](EnterpriseOps/docs/AIUsageNotes.md) |
| — | AI-assisted review patterns | [`EnterpriseOps/docs/AIAssistedReviewPatterns.md`](EnterpriseOps/docs/AIAssistedReviewPatterns.md) |
| — | Capstone architecture diagram | [`EnterpriseOps/docs/capstone-package.svg`](EnterpriseOps/docs/capstone-package.svg) |

## Where things live

```
EnterpriseOps/
├─ Program.cs                       composition root: SessionContext → Application.Session → CommandCenterDashboard
├─ UI/  CommandCenterDashboard.cs (+.Designer.cs) · CapstoneReviewPage.cs (+.Designer.cs)
├─ Controls/ KpiTile.cs             the reusable KPI tile
├─ Domain/  WorkOrder · Capstone (deliverables, checks) · Review (findings, verdicts)
├─ Services/ DashboardService · WorkOrderService (approve = authorize → validate → write with version → audit)
│            WorkQueue · CapstonePackageService · PromptLibraryService
│            GeneratedCodeReviewService · GeneratedCodeSamples (PR #214 draft + rev 2) · DocumentedApiCatalog
│            DocumentationIndexService · SessionContext · CommandContext · CommandResult
├─ Security/ Permissions (Role → Permission) · AuditLog (append-only)
├─ Data/     InMemoryWorkOrderStore (120 rows / 3 tenants, optimistic Version) · DocsFolder (locates docs/)
├─ Diagnostics/ ActivityTrace (server log → System.Diagnostics.Trace) · DiagnosticsService (the six probes)
└─ docs/     the deliverables above + index.json
```

## Instructor acceptance criteria

| Criterion | Evidence |
|---|---|
| Follows the course architecture baseline | folder-per-layer namespaces, per-session `SessionContext`, no static session state (checklist rule Q2 rejects it) |
| UI event handlers remain thin and explainable | every handler in both pages is a few lines that build a command and call a service |
| Service-level logic reviewable without the designer | `Services/*` have no control references |
| At least one failure path demonstrated | AI draft #214 rejected on eleven findings; stale-version and refusal results shown as banners |
| State ownership / security / production behaviour explained | `docs/ProductionReadiness.md` answers the four questions |
