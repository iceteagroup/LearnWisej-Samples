# ADR-001 — Solution structure

| | |
|---|---|
| **Status** | Accepted |
| **Owner** | Tech lead (EnterpriseOps) |
| **Date** | 2026-09-09 |
| **Review date** | 2027-03-09 (six months — revisit when the first second team joins or the first module needs a separate deployable) |
| **Shape** | `Architecture/ArchitectureGovernancePatterns.cs` → `ArchitectureDecision` record |

## Context

EnterpriseOps will grow: more screens, more integrations, multiple teams, several deployment environments.
The Intermediate course showed that one Wisej.NET project with mixed concerns is fast for one developer and one
screen. It does not survive growth: workflow logic lands in click handlers, the same pricing rule is copied into
three screens, security checks depend on which developer wrote the screen, and nobody can say where a decision
lives without opening the Designer.

The advanced question is not "can the screen be built" but "can a team evolve it for years". The Wisej.NET
advantage — rich server-side C# and a designable UI — must be preserved, not replaced with a generic web stack.

## Options considered

| Option | Description | Why not / why |
|---|---|---|
| A. One project, one folder per screen | Everything a screen needs next to its `.cs`/`.Designer.cs` | Fast to start; workflow, security and data access end up per screen and get duplicated. Nobody can find "the" workflow. Rejected. |
| B. Multi-project split (`EnterpriseOps.UI`, `.Domain`, `.Services`, `.Data`, `.Integrations`) | The five-project solution the walkthrough video shows | The right end state for multiple teams: compile-time boundaries, separate versioning, separate test projects. Costs five projects to run one lab sample. Deferred, not rejected. |
| C. One web project, **folder-per-layer, namespaces follow folders** | `UI/`, `Controls/`, `Domain/`, `Services/`, `Data/`, `Integrations/`, `Security/`, `Diagnostics/`, `Resources/`, `Architecture/`, `Deployment/` | Same boundaries and the same names as B, runnable with one `dotnet run`. Moving a folder into its own project later is a mechanical change because the namespaces are already right. **Chosen.** |

## Decision

Split by responsibility, one folder and one namespace per layer, in one web project:

```
EnterpriseOps.UI            screens (Page/Form + .Designer.cs) and dialogs — UI state only
EnterpriseOps.Controls      shared, designable controls (KpiTile) — never compute a number
EnterpriseOps.Domain        entities, value objects, enums (WorkOrder, Tenant, WorkOrderStatus, Priority)
EnterpriseOps.Services      application services, commands, contexts, results; Services.Workflow per screen
EnterpriseOps.Data          repositories behind interfaces (in-memory now, EF Core in Module 4)
EnterpriseOps.Integrations  external systems behind fakes (OperationsFeed, ReleaseCalendar)
EnterpriseOps.Security      identity, roles, policies — every permission decision
EnterpriseOps.Diagnostics   the activity log and the error log
EnterpriseOps.Resources     user-facing strings (UiText) — never an exception in a message
EnterpriseOps.Architecture  ADRs (this folder), the governance patterns, the review gate
Deployment/                 launch profiles, environment notes, later the Dockerfile and health probe
```

Rules that follow from it (the full list is `docs/CodingStandards.md`):

1. A screen may reference `Services`, `Controls`, `Resources`, `Diagnostics` and `Architecture`. It never references
   `Data`, `Integrations` or `Security` directly — those are the workflow's business. (The dashboard's constructor
   is the one exception: it is the composition root until Module 3 introduces a registry.)
2. Every screen has exactly one workflow service, named `<Screen>Workflow` in `Services/Workflow/`, returning a
   typed `CommandResult`. Handlers are `try { await _workflow.XAsync(ctx); ShowResult(result); } catch { log; generic message }`.
3. Entities never cross into the UI. Services project them (`IncidentRow`, `DashboardKpis`).
4. Security decides first: a policy runs before any integration or query; a denial is a result, not an exception.
5. Screens stay designable: layout, tiles, grid columns, banners and the status bar live in `.Designer.cs` and open
   in the Wisej Designer. The parameterless constructor gives the Designer a default `SessionContext`.

## Consequences

- **Findable.** A new senior developer opens `Services/Workflow/` and finds `DashboardWorkflow.cs` for
  `UI/CommandCenterDashboard.cs` by name — the review question "under one minute" is answered by the folder tree.
- **Explainable.** Every boundary has a one-line reason in the tree above and a longer one in
  `docs/SolutionStructure.md`; the activity log records the layers in the order they decide.
- **Designer keeps working.** Orchestration never lives in event handlers, so the Designer never has to load a
  repository or an integration to render the page.
- **Reviewable without the Designer.** `ReviewGate.CheckEventHandler` (lesson code, verbatim): > 20 lines or no
  service call blocks the merge. `Architecture/Samples/OrderEntryLegacy.cs.txt` is the handler it rejects.
- **Cost.** More files per feature (screen + workflow + result + policy). Accepted: each file has one reason to change.
- **Cost.** Folder discipline is enforced by review, not by the compiler, until the split into projects (option B).
  The review checklist carries the rule; the six-month review decides whether the compiler should.

## How to revisit

Re-open this ADR when any of these happens: a second team owns screens; a layer needs its own release cadence;
the Data layer needs to be tested without Wisej.NET loaded; the deployment (Module 12) needs a separate worker.
The expected outcome is option B with the same folder names promoted to projects.
