# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 1

Local lab build for **Advanced Module 1 · Enterprise Wisej.NET architecture, technical leadership & governance**.
It is the **EnterpriseOps Command Center** baseline that every later module of the course builds on: the
folder-per-layer solution structure, ADR-001, the designable `CommandCenterDashboard` that reads its KPIs from
`EnterpriseOps.Services` through a one-line handler, and the review gate that catches a 74-line handler with no
service call *before* it merges.

Nothing here is deployed anywhere. In-memory data only — no database, no network, no cloud account.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Enterprise Wisej.NET Course/Module 1/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5201
```

Then open <http://localhost:5201>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — the launch profile is
already on 5201.)

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package. The project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

**Run it from the project folder.** The review-gate buttons read real source files off disk
(`UI/CommandCenterDashboard.cs`, `Architecture/Samples/OrderEntryLegacy.cs.txt`); if the app is started from
somewhere else they report *"Source file not found"* instead of a report.

## What to click

Left card = the dashboard. Right card = **Server · live activity trace**, one line per layer decision. Bottom bar
= the failure paths and their recoveries.

| Action | Path | What you should see |
|---|---|---|
| *(on load)* | success | Trace opens with `Architecture: ADR-001 Solution structure — accepted … review 2027-03-09`, then `UI → … _Load`, `Service:`, `Security:`, `Data:`, `Service:`, `UI ←`. Tiles show **12 / 3 / 2**, the grid lists the contoso incidents, the dark footer reads `Loaded 12 incidents — EnterpriseOps.Services.Workflow · NN ms` |
| **⟳ Refresh** | success | The workflow pulls the operations feed: `INC-1042` turns **mitigated** and is painted green, Open incidents drops to **11**, footer `Refreshed — INC-1042 mitigated — EnterpriseOps.Services.Workflow · NN ms`. Click again for the next scripted batch (`INC-1046` escalated, then two more) |
| **Fail: ops feed down** | failure 1 — integration | `Integration: OperationsFeed OFFLINE (simulated outage)`, then `no response after 350 ms → IntegrationUnavailableException`. Red banner + toast: *The action could not be completed. Check the log for details.* **(ref a1b2c3d4)**. `Diagnostics: ERROR corr=a1b2c3d4 IntegrationUnavailableException: … ops-feed.internal:8443` stays in the trace — the host name never reaches the banner. Tiles and grid keep their last good values |
| **Recover: feed back online** | recovery 1 | `Integration: OperationsFeed back online`, the same `btnRefresh_Click` succeeds, banner clears, status back to green |
| **Refresh as ben.tech (Technician)** | failure 2 — permission | `Security: DashboardPolicy.CanViewCommandCenter(ben.tech · Technician) → DENIED`, then `Service: stopped: policy denied — no feed pull, no query`. **Amber** banner (not red — this is an expected result, not a crash): *The Command Center is available to Managers and Admins…*. The grid is untouched |
| **Back to ana.ops (Manager)** *(same button)* | recovery 2 | Policy allows, the refresh runs end to end again |
| **Review gate: legacy handler** | failure 3 — governance | `Architecture: ReviewGate.CheckEventHandler("btnSubmitOrder_Click", 74, callsService: false) → 2 issue(s)` and both issues. Red banner: *Review gate — btnSubmitOrder_Click · 2 issues · 74 lines · callsService: false — caught before merge, not in production* |
| **Review gate: this screen** | recovery 3 | The same two rules over `UI/CommandCenterDashboard.cs`: 8 handlers, every one ≤ 20 lines and calling a service. Green banner: *0 issues · may merge* |
| **Clear trace** | – | empties the trace list |

The header carries the tenant, the signed-in user and the **correlation id** of the command that is running; the
same id appears in the trace, in the error log and in the failure banner, so a user's screenshot is joinable with
the server's log.

## Deliverables

| Lab deliverable | Where |
|---|---|
| Solution / folder structure tree | [`EnterpriseOps/docs/SolutionStructure.md`](EnterpriseOps/docs/SolutionStructure.md) + the layer diagram [`solution-structure.svg`](EnterpriseOps/docs/solution-structure.svg) |
| ADR-001 solution structure decision | [`EnterpriseOps/Architecture/ADR-001-SolutionStructure.md`](EnterpriseOps/Architecture/ADR-001-SolutionStructure.md) — the ADR folder is `Architecture/`, indexed from [`docs/AdrIndex.md`](EnterpriseOps/docs/AdrIndex.md); the same decision as data in `Architecture/DecisionLog.cs` |
| Team coding standards page | [`EnterpriseOps/docs/CodingStandards.md`](EnterpriseOps/docs/CodingStandards.md) — rules N-*, S-*, A-*, D-*, E-*, T-* |
| Reference screen naming + service-boundary example | [`EnterpriseOps/docs/ReferenceScreen.md`](EnterpriseOps/docs/ReferenceScreen.md) |
| Code review checklist applied to the first screen | [`EnterpriseOps/docs/CodeReviewChecklist.md`](EnterpriseOps/docs/CodeReviewChecklist.md) — filled in against `CommandCenterDashboard`, 8 handlers / 0 issues |

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
   ├─ Integrations/      OperationsFeed (outage switch) · ReleaseCalendar
   ├─ Security/          UserIdentity · UserRole · DashboardPolicy
   ├─ Diagnostics/       ActivityTrace · ErrorLog
   ├─ Resources/         UiText
   ├─ Architecture/      ADR-001-SolutionStructure.md · ArchitectureGovernancePatterns.cs (ReviewGate)
   │                     DecisionLog.cs · ReviewGateService.cs · Samples/OrderEntryLegacy.cs.txt
   ├─ Deployment/        README.md — what lands here and in which module
   └─ docs/              the five deliverables above
```

Full annotated tree, including which folder may reference which: `docs/SolutionStructure.md`.

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Start the solution — the baseline every later module builds on | `EnterpriseOps.slnx`, `EnterpriseOps.csproj` (`net10.0-windows;net10.0`, Wisej-4 4.1.0) |
| Projects **or folders** for UI, shared controls, domain, application services, data access, integrations, security, diagnostics, resources, deployment | the ten folders above, each its own namespace — `docs/SolutionStructure.md` maps folder → namespace → allowed references |
| Add an Architecture Decision Record folder | `EnterpriseOps/Architecture/` |
| Write ADR-001 for the chosen solution structure | `Architecture/ADR-001-SolutionStructure.md` (context · three options · decision · consequences · how to revisit) and `Architecture/DecisionLog.cs` (the same record as data, traced on load) |
| Deliverable · structure tree | `docs/SolutionStructure.md`, `docs/solution-structure.svg` |
| Deliverable · team coding standards page | `docs/CodingStandards.md` |
| Deliverable · reference screen naming and service-boundary example | `docs/ReferenceScreen.md`, demonstrated by `UI/CommandCenterDashboard.cs` ↔ `Services/Workflow/DashboardWorkflow.cs` |
| Deliverable · code review checklist applied to the first screen | `docs/CodeReviewChecklist.md`, runnable via `Architecture/ReviewGateService.cs` |
| Create the service/boundary first, then the screen | `DashboardWorkflow.RunAsync` orders the layers: Security → Integration → Data → projection; the screen only calls `LoadAsync` / `RefreshAsync` |
| Show every path: success, validation/permission and error, without leaking internals | success = Refresh · permission = *Refresh as ben.tech* (amber, typed result) · error = *Fail: ops feed down* (red, generic message + correlation id, details only in `ErrorLog`) |
| Add a failure-path demonstration, not only the happy path | three of them, each with a recovery — see the What-to-click table |
| Review & run: production-readiness note | `Deployment/README.md` and the *Production readiness* section below |

Lab code check (`labs.js` m1) — `btnRefresh_Click` in `UI/CommandCenterDashboard.cs` satisfies all five greps:
an event handler (`btnRefresh_Click`), thin & async (`async`/`await`/`Task`), a service call
(`_workflow.RefreshAsync(...)`), a failure path (`try` / `catch`), and architecture standards (the file's header
comment names ADR-001, the boundary and the review gate).

## Student review questions, answered against this sample

**Can a new senior developer find the workflow service for a screen in under one minute?**
Yes, without asking anyone: the rule is `UI/<Screen>.cs` → `Services/Workflow/<Screen minus its kind>Workflow.cs`.
`CommandCenterDashboard` → `Services/Workflow/DashboardWorkflow.cs`. The running app also says so: the trace line
`Service: DashboardWorkflow.RefreshAsync(tenant=contoso user=ana.ops corr=…)` names the class the moment you click.

**Can the team explain why a boundary exists?**
Each boundary has a one-line reason in the folder table of `docs/SolutionStructure.md`, a rule id in
`docs/CodingStandards.md`, and the decision behind all of them in ADR-001 — which also records the option that was
*not* taken (five real projects), what it would have cost, and the date the team revisits it. The costs are written
down too: more files per feature, and a boundary the compiler does not enforce yet.

**Is the designer still usable after the architecture split?**
Yes — that is the constraint the split was designed around. All layout lives in
`CommandCenterDashboard.Designer.cs` (panels, three `KpiTile`s, `dgvIncidents` with explicit columns, the banner,
the dark footer, the trace card, the button bar) and the parameterless constructor hands the Designer a default
`SessionContext`, so the page renders without a session, a repository or an integration. The boundary is not
*visual vs coded*; it is *UI state vs application decisions*.

## Instructor acceptance criteria, answered

| Criterion | Evidence |
|---|---|
| Follows the course architecture baseline | Folder-per-layer with namespaces matching (`EnterpriseOps.UI/.Controls/.Domain/.Services/.Data/.Integrations/.Security/.Diagnostics/.Resources/.Architecture`), typed commands and results, `CommandContext`/`SessionContext`, per-session services |
| UI event handlers remain thin and explainable | 8 handlers, 4–19 lines, one service call each; proved mechanically by **Review gate: this screen** (0 issues) and tabulated in `docs/CodeReviewChecklist.md` |
| Service-level logic can be reviewed without opening the designer | `Services/Workflow/DashboardWorkflow.cs` reads top-to-bottom as *security → integration → data → projection*; no Wisej.NET control type appears in `Services/`, `Data/`, `Integrations/`, `Security/` or `Domain/` |
| At least one failure path is demonstrated | Three, each with a recovery: integration outage (exception → log + generic message + correlation id), permission denied (typed result → amber banner, nothing queried), review-gate rejection (74-line handler blocked before merge) |
| The student can explain state ownership, security implications and production behaviour | See the section below |

## Production readiness — state, security, behaviour

**State ownership.** UI state (text, colour, `Enabled`, `Visible`, the grid's `DataSource`, the trace list) belongs
to the screen. Session state (tenant, signed-in user, the current command and its correlation id) belongs to
`SessionContext`, created in `Program.Main`, stored in `Application.Session.Context` and injected into the screen —
one instance per browser session, never a `static`. Business state (work orders and their `Version`) belongs to
`Data/`, reachable only through `IWorkOrderRepository`. No layer caches another layer's state.

**Security implications.** Every permission decision is `Security/DashboardPolicy`, called by the workflow *before*
any integration or query — a denial costs one policy call and zero rows read. Screens never test a role (rule S-4).
Every repository query is scoped to a tenant inside the repository, so a caller cannot forget (D-2). What the user
reads is `Resources/UiText`; exception types, the feed's host name and the simulated SQL never leave `ErrorLog`
(E-1). The one thing the user *is* given is the correlation id, which is useless to an attacker and essential to
support.

**Production behaviour.** Every action mints a correlation id and carries it through result, trace and log (E-4).
Buttons are disabled during a command and restored in `finally`, so a thrown service leaves a usable screen.
After each `await`, `Application.Update(this)` pushes the pending changes over the WebSocket. Expected failures
are typed results and unexpected ones are exceptions, so the screen can say *"you may not"* in amber and
*"something broke"* in red. The simulated latency (60 ms repository, 350 ms feed) makes the busy state visible.

**Known debt, written down rather than hidden** (T-1): the screen constructor is the composition root and is the
one place `UI/` sees `Data/` and `Integrations/` — Module 3 replaces it with a per-session registry. The layer
boundary is enforced by review, not by the compiler, until the folders become projects — ADR-001 revisits that on
2027-03-09. Authentication is simulated (`SessionContext.SignInAs`) until Module 7.

## Verified / unverified

Built on this machine against Wisej-4 4.1.0 / .NET 10: `dotnet build -nologo -v q` → **Build succeeded, 0 warnings,
0 errors**, both target frameworks. The app was **not** run by the builder (per the brief); the reviewer runs it.

Cookbook facts used that the cookbook marks **(unverified)**, worth confirming at runtime:

- `Application.Session.<name>` as a dynamic per-user bag — `Program.Main` stores the `SessionContext` in
  `Application.Session.Context`. Compiles; the screen does not read it back (it receives the instance through its
  constructor), so a failure here would not break the sample.
- `DataGridView` bound to a `List<T>` with `AutoGenerateColumns = false` and explicit
  `DataPropertyName` columns, plus per-row `DefaultCellStyle.BackColor/ForeColor` for the green "mitigated" row.

Verified elsewhere in the course and relied on here: `AlertBox.Show(…, alignment: ContentAlignment.TopRight,
autoCloseDelay: 4000)`; `async void` handlers with `await` followed by `Application.Update(this)`;
`Font("default", …)` / `Font("monospace", 9F)`; `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`;
`Application.MainPage = new <Page>()`.

One API in this sample is outside the cookbook: **`Application.StartupPath`**, used by
`Architecture/ReviewGateService.Resolve` as one of the candidate roots when locating a source file (with
`Directory.GetCurrentDirectory()` and a walk up from `AppContext.BaseDirectory` as fallbacks). It compiles against
Wisej-4 4.1.0; if it behaves unexpectedly at runtime the fallbacks still find the file when the app is started
from the project folder, and the gate reports a clean *"Source file not found"* result rather than throwing.
