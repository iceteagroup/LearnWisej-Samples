# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 1

Local lab build for **Advanced Module 1 · Enterprise Wisej.NET architecture, technical leadership & governance**.
It is the **EnterpriseOps Command Center** baseline that every later module of the course builds on: the
folder-per-layer solution structure, ADR-001, and the designable `CommandCenterDashboard` that reads its KPIs from
`EnterpriseOps.Services` through a one-line handler.

Nothing here is deployed anywhere. In-memory data only — no database, no network, no cloud account.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Enterprise Wisej.NET Course/Module 1/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5201
```

Then open <http://localhost:5201>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — the launch profile is
already on 5201.) The project multi-targets `net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## What to try

| Control | What you should see |
|---|---|
| *(on load)* | The three KPI tiles (**Open incidents · SLA at risk · Deployments today**) fill in, `dgvIncidents` lists the contoso incidents, and the dark status bar reads `Loaded 12 incidents — EnterpriseOps.Services.Workflow · NN ms`. `Signed in: ana.ops · Manager` sits next to Refresh |
| **⟳ Refresh** (`btnRefresh`) | The workflow pulls the operations feed: `INC-1042` turns **mitigated** and is painted green, Open incidents drops to **11**, status bar `Refreshed — INC-1042 mitigated — EnterpriseOps.Services.Workflow · NN ms`. Click again for the next scripted batch |

Failure paths are handled in the normal UI: a policy denial (`DashboardPolicy`, Technicians may not open the
Command Center) shows an amber banner and leaves the data untouched; an exception from the feed or the store is
logged with the correlation id and the user sees a red banner with a generic message plus `(ref <id>)`. The
governance failure path — a 74-line handler with no service call — is caught by the review gate
(`ReviewGate.CheckEventHandler`), with `Architecture/Samples/OrderEntryLegacy.cs.txt` as the counter-example.

## Deliverables

| Lab deliverable | Where |
|---|---|
| Solution / folder structure tree | [`EnterpriseOps/docs/SolutionStructure.md`](EnterpriseOps/docs/SolutionStructure.md) + the layer diagram [`solution-structure.svg`](EnterpriseOps/docs/solution-structure.svg) |
| ADR-001 solution structure decision | [`EnterpriseOps/Architecture/ADR-001-SolutionStructure.md`](EnterpriseOps/Architecture/ADR-001-SolutionStructure.md) — the ADR folder is `Architecture/`, indexed from [`docs/AdrIndex.md`](EnterpriseOps/docs/AdrIndex.md) |
| Team coding standards page | [`EnterpriseOps/docs/CodingStandards.md`](EnterpriseOps/docs/CodingStandards.md) |
| Reference screen naming + service-boundary example | [`EnterpriseOps/docs/ReferenceScreen.md`](EnterpriseOps/docs/ReferenceScreen.md) |
| Code review checklist applied to the first screen | [`EnterpriseOps/docs/CodeReviewChecklist.md`](EnterpriseOps/docs/CodeReviewChecklist.md) |

## Where things live

```
Module 1/
├─ EnterpriseOps.slnx
└─ EnterpriseOps/
   ├─ Program.cs                    session composition root: SessionContext → Application.MainPage
   ├─ Startup.cs · Default.json · Default.html · Web.config · Properties/launchSettings.json (5201)
   ├─ UI/                CommandCenterDashboard.cs + .Designer.cs      ← UI state only
   ├─ Controls/          KpiTile.cs + .Designer.cs                     ← shared designable control
   ├─ Domain/            WorkOrder · WorkOrderStatus · Priority · Tenant
   ├─ Services/          SessionContext · CommandContext · CommandResult
   │  └─ Workflow/       DashboardWorkflow · DashboardResult (DashboardKpis, IncidentRow)
   ├─ Data/              IWorkOrderRepository · InMemoryWorkOrderRepository · SeedData
   ├─ Integrations/      OperationsFeed · ReleaseCalendar
   ├─ Security/          UserIdentity · UserRole · DashboardPolicy
   ├─ Diagnostics/       ActivityTrace (server log) · ErrorLog
   ├─ Resources/         UiText
   ├─ Architecture/      ADR-001-SolutionStructure.md · ArchitectureGovernancePatterns.cs (ReviewGate)
   │                     Samples/OrderEntryLegacy.cs.txt
   ├─ Deployment/        README.md — what lands here and in which module
   └─ docs/              the five deliverables above
```

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Start the solution — the baseline every later module builds on | `EnterpriseOps.slnx`, `EnterpriseOps.csproj` (`net10.0-windows;net10.0`, Wisej-4 4.1.0) |
| Projects **or folders** for UI, shared controls, domain, application services, data access, integrations, security, diagnostics, resources, deployment | the folders above, each its own namespace — `docs/SolutionStructure.md` maps folder → namespace → allowed references |
| Add an Architecture Decision Record folder, write ADR-001 | `EnterpriseOps/Architecture/`, `Architecture/ADR-001-SolutionStructure.md` |
| Reference screen naming and service-boundary example | `docs/ReferenceScreen.md`, demonstrated by `UI/CommandCenterDashboard.cs` ↔ `Services/Workflow/DashboardWorkflow.cs` |
| Code review checklist applied to the first screen | `docs/CodeReviewChecklist.md`, rules from `ReviewGate` in `Architecture/ArchitectureGovernancePatterns.cs` |
| Show every path without leaking internals | `DashboardWorkflow.RunAsync` (policy first → typed denial), `CommandCenterDashboard.ReportFailure` (log + generic message + correlation id) |
| Review & run: production-readiness note | `Deployment/README.md` and the section below |

## Student review questions

**Can a new senior developer find the workflow service for a screen in under one minute?**
Yes: `UI/<Screen>.cs` → `Services/Workflow/<Screen minus its kind>Workflow.cs`, so `CommandCenterDashboard` →
`Services/Workflow/DashboardWorkflow.cs`.

**Can the team explain why a boundary exists?**
Each boundary has a one-line reason in `docs/SolutionStructure.md`, a rule id in `docs/CodingStandards.md`, and the
decision behind all of them in ADR-001 — including the option not taken (five real projects) and its review date.

**Is the designer still usable after the architecture split?**
Yes. All layout lives in `CommandCenterDashboard.Designer.cs` and the parameterless constructor hands the Designer
a default `SessionContext`, so the page renders without a session, a repository or an integration.

## Production readiness — state, security, behaviour

**State ownership.** UI state (text, colour, `Enabled`, `Visible`, the grid's `DataSource`) belongs to the screen.
Session state (tenant, signed-in user, the current command and its correlation id) belongs to `SessionContext`,
created in `Program.Main` and stored in `Application.Session.Context` — one per browser session, never a `static`.
Business state belongs to `Data/`, reachable only through `IWorkOrderRepository`.

**Security implications.** Every permission decision is `Security/DashboardPolicy`, called by the workflow *before*
any integration or query. Every repository query is scoped to a tenant inside the repository. What the user reads
is `Resources/UiText`; exception details never leave `ErrorLog`.

**Production behaviour.** Every action mints a correlation id carried through result and log. Refresh is disabled
during a command and restored in `finally`; after each `await`, `Application.Update(this)` pushes the changes.
Known debt (T-1): the screen constructor is the composition root until Module 3's per-session registry.

## Verified

`dotnet build -nologo -v q` → Build succeeded, 0 warnings, 0 errors, both target frameworks.
