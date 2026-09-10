# Deliverable 1 — Solution / folder structure

The lab asks for "projects or folders for UI, shared controls, domain, application services, data access,
integrations, security, diagnostics, resources, and deployment", plus an Architecture Decision Record folder.
This is that tree, as it exists in this sample. The *why* is [ADR-001](../Architecture/ADR-001-SolutionStructure.md);
the diagram of the same thing is [`solution-structure.svg`](solution-structure.svg).

## The tree

```
Module 1/
├─ EnterpriseOps.slnx                        the solution
├─ README.md                                 how to run it, what to click, lab steps → code
└─ EnterpriseOps/                            one web project, folder-per-layer
   ├─ EnterpriseOps.csproj                   net10.0-windows;net10.0 · Wisej-4 4.1.0
   ├─ Program.cs                             composition root of a session: SessionContext → Application.MainPage
   ├─ Startup.cs                             Kestrel host: app.UseWisej(), static files, never the .json files
   ├─ Default.html · Default.json · Web.config
   ├─ Properties/launchSettings.json         port 5201
   │
   ├─ UI/                            EnterpriseOps.UI              ← screens. UI state only.
   │  ├─ CommandCenterDashboard.cs            code-behind: thin handlers, one service call each
   │  └─ CommandCenterDashboard.Designer.cs   InitializeComponent() — opens in the Wisej Designer
   │
   ├─ Controls/                      EnterpriseOps.Controls        ← shared, designable controls
   │  ├─ KpiTile.cs                           Caption / Value / Accent — shows a number, never computes one
   │  └─ KpiTile.Designer.cs
   │
   ├─ Domain/                        EnterpriseOps.Domain          ← entities, value objects, enums
   │  ├─ WorkOrder.cs                         the entity + ChangeStatus (keeps Version and UpdatedUtc honest)
   │  ├─ WorkOrderStatus.cs · Priority.cs     the vocabulary every module of the course shares
   │  └─ Tenant.cs                            contoso · fabrikam · northwind
   │
   ├─ Services/                      EnterpriseOps.Services        ← application services. Every decision.
   │  ├─ SessionContext.cs                    per-session tenant + user; BeginCommand() mints a correlation id
   │  ├─ CommandContext.cs                    who is asking: tenant, user, role, correlation id
   │  ├─ CommandResult.cs                     Succeeded · Errors · CorrelationId — what a screen gets back
   │  └─ Workflow/                   EnterpriseOps.Services.Workflow   one workflow per screen
   │     ├─ DashboardWorkflow.cs              LoadAsync / RefreshAsync — security → integration → data → project
   │     └─ DashboardResult.cs                DashboardKpis · IncidentRow — the grid's projection
   │
   ├─ Data/                          EnterpriseOps.Data            ← persistence behind an interface
   │  ├─ IWorkOrderRepository.cs               the contract Module 4 re-implements with EF Core
   │  ├─ InMemoryWorkOrderRepository.cs        the fake store, tenant isolation enforced here
   │  └─ SeedData.cs                           ~40 deterministic work orders across three tenants
   │
   ├─ Integrations/                  EnterpriseOps.Integrations    ← external systems, behind fakes
   │  ├─ OperationsFeed.cs                     field changes + the outage switch (failure path 1)
   │  └─ ReleaseCalendar.cs                    the "Deployments today" number
   │
   ├─ Security/                      EnterpriseOps.Security        ← identity, roles, policies
   │  ├─ UserIdentity.cs · UserRole.cs         ana.ops · ben.tech · cara.admin
   │  └─ DashboardPolicy.cs                    CanViewCommandCenter → PolicyDecision (failure path 2)
   │
   ├─ Diagnostics/                   EnterpriseOps.Diagnostics     ← what the reviewer reads
   │  ├─ ActivityTrace.cs                      one line per layer decision → lstTrace
   │  └─ ErrorLog.cs                           the `_log.Error(ex)` of the handler shape
   │
   ├─ Resources/                     EnterpriseOps.Resources       ← user-facing strings
   │  └─ UiText.cs                             no exception type, host name or stack trace ever reaches a user
   │
   ├─ Architecture/                  EnterpriseOps.Architecture    ← governance. ADRs live HERE.
   │  ├─ ADR-001-SolutionStructure.md          the decision record (this folder is the ADR folder)
   │  ├─ ArchitectureGovernancePatterns.cs     ArchitectureDecision · IWorkflowScreen · ReviewGate (lesson code)
   │  ├─ DecisionLog.cs                        the ADRs as data, so a screen can list them with review dates
   │  ├─ ReviewGateService.cs                  runs ReviewGate over a real .cs file (failure path 3 + recovery)
   │  └─ Samples/OrderEntryLegacy.cs.txt       the 74-line handler the gate rejects (not compiled)
   │
   ├─ Deployment/                                                   ← deployment assets
   │  └─ README.md                             what lands here and in which module (Dockerfile, /healthz, runbook)
   │
   └─ docs/                                                         ← the lab deliverables
      ├─ SolutionStructure.md                  this file (deliverable 1)
      ├─ solution-structure.svg                the same tree as a layer diagram
      ├─ AdrIndex.md                           the ADR index → Architecture/ADR-NNN-*.md (deliverable 2)
      ├─ CodingStandards.md                    team coding standards (deliverable 3)
      ├─ ReferenceScreen.md                    screen naming + service-boundary example (deliverable 4)
      └─ CodeReviewChecklist.md                the checklist, applied to CommandCenterDashboard (deliverable 5)
```

## Folder → namespace → what may reference what

| Folder | Namespace | Owns | May reference |
|---|---|---|---|
| `UI/` | `EnterpriseOps.UI` | screen layout and UI state | Services, Controls, Resources, Diagnostics, Architecture |
| `Controls/` | `EnterpriseOps.Controls` | reusable visuals | Resources |
| `Domain/` | `EnterpriseOps.Domain` | entities and their invariants | nothing (the innermost layer) |
| `Services/` | `EnterpriseOps.Services` | orchestration, validation, projection | Domain, Data, Integrations, Security, Diagnostics |
| `Data/` | `EnterpriseOps.Data` | persistence | Domain, Diagnostics |
| `Integrations/` | `EnterpriseOps.Integrations` | external systems | Domain, Diagnostics |
| `Security/` | `EnterpriseOps.Security` | identity and permission decisions | Domain, Services (contexts), Diagnostics |
| `Diagnostics/` | `EnterpriseOps.Diagnostics` | the trace and the error log | nothing |
| `Resources/` | `EnterpriseOps.Resources` | user-facing strings | nothing |
| `Architecture/` | `EnterpriseOps.Architecture` | ADRs, governance patterns, the review gate | Services, Diagnostics |
| `Deployment/` | — (assets, not code) | launch profiles, env config, later Dockerfile | — |

The one deliberate exception: `UI/CommandCenterDashboard`'s **constructor** news up the repository, the feed,
the calendar and the policy to build its workflow. That constructor is the screen's composition root and is
allowed to see the whole graph; no other line of the screen may. Module 3 replaces it with a per-session registry.

## Why one project instead of five

The walkthrough video's Solution Explorer shows five projects (`EnterpriseOps.UI`, `.Domain`, `.Services`,
`.Data`, `.Integrations`). This sample keeps the same names and the same boundaries as **folders and namespaces**
inside one web project, so the whole module runs with a single `dotnet run`. Because the namespaces already match
the project names, promoting a folder to a project later is a move-files-and-add-a-reference change, not a
redesign. ADR-001 records that trade-off, its cost (the boundary is enforced by review, not by the compiler)
and the date the team revisits it.

## Evidence in the running app

- The header of every trace line names the layer that decided: `UI →`, `Security:`, `Integration:`, `Data:`,
  `Service:`, `Diagnostics:`, `Architecture:`, `UI ←`. Click **Refresh** and read the trace top to bottom: the
  order is exactly the order of this tree's dependency arrows.
- On load, the first trace line is `Architecture: ADR-001 Solution structure — accepted … · review 2027-03-09`
  (from `DecisionLog`), so the decision is visible in the product, not only in a document.
- **Review gate: legacy handler** proves the boundary is checkable: `Architecture/Samples/OrderEntryLegacy.cs.txt`
  fails on 74 lines and no service call. **Review gate: this screen** passes on `UI/CommandCenterDashboard.cs`.
