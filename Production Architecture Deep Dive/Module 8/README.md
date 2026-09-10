# TicketOps · Production Architecture Deep Dive · Module 8

Local lab build for **Module 8 · Services, Dependency Injection & Testable UI Patterns**. It follows the
walkthrough video *Inject services for a testable UI*: Module 1's hand-written `AppComposition` factory is
replaced by `Application.Services` registrations with explicit lifetimes; the screen asks for five
**capabilities** — `ITicketService`, `IUserService`, `IPermissionService`, `INotificationService`,
`IAuditLogService` — through `[Inject]` properties instead of creating concrete classes; each contract has a
**fake** (in-memory) and a **production-shaped** implementation behind a registration profile you can switch;
and the screen's decisions live in a plain `TicketWorkflowPresenter` that the app can run against fakes
with no browser in the loop.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine. The "production"
services are stand-ins that log the SQL / directory / SMTP calls they would make.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Production Architecture Deep Dive\Module 8\TicketOps"
dotnet run -f net10.0 --urls http://localhost:5108
```

Then open <http://localhost:5108> (fake profile) or <http://localhost:5108/?profile=production>.
(Visual Studio: open `TicketOps.slnx`, press F5 — the port is in `Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with no warnings for both targets (`net10.0-windows`, `net10.0`).

## What to click in the Ticket Workflow window

The left card is the screen; the right card is the **Activity trace · UI → Presenter → Services → Data**.
When the page opens, the trace already holds the startup lines: `[SESSION] Program.Main — session … started ·
profile fake (in-memory) · registered now · 1 active session(s)`, seven `[INFRA] ServiceRegistration — IXxx → Impl · Lifetime`
lines, `[INFRA] TicketWorkflow.EnsureInjected — [Inject] properties were filled automatically …` (or the explicit
`Application.Services.Inject(this)` fallback, if the automatic path did not run), and the concrete type behind every
injected property. The **Injected services** grid resolves each contract twice: Session and Shared rows say
*same instance*, the Transient `INotificationService` row says *new instance #xxxx (transient)*.

| Button | Path | What you should see |
|---|---|---|
| **Close ticket** (select #1041, type a reason) | success | `[UI] buttonClose_Click → presenter.CloseAsync(#1041, "…") over the [Inject]ed ITicketService / …`, `[SVC] TicketWorkflowPresenter.CloseAsync — #1041 as Dana Reyes (Technician)`, `[DATA] FakeTicketService.FindAsync`, `[SVC] FakePermissionService.CanClose … → allowed`, `[DOMAIN] Ticket.Close — #1041 status → Closed`, `[DATA] … written to the in-memory table`, `[SVC] … IAuditLogService.Record(close, #1041, operator 1) → entry 1 of 1 (Shared …)`, `[SVC] FakeNotificationService#xxxx.Notify — to operator 1 …`, `[UI] OK · Ticket #1041 closed.`; the row leaves the grid; **Audit trail: 1 entries** |
| **Assign to me** (select #1044) | success | `presenter.AssignToMeAsync(#1044)` → `CanAssign … allowed` → `[DOMAIN] Ticket.AssignTo` → audit `assign` → notification; the Assignee column shows Dana Reyes |
| **Signed in as** combo | session state | `[UI] → IUserService.SignInAs(n)`, `[SESSION] FakeUserService.SignInAs — current operator → … (this session only)` — a second tab keeps its own operator |
| **Switch to production** / **Switch to fake** | infrastructure | `[INFRA] ServiceRegistration — ITicketService → SqlTicketService · Session` (×5), `[INFRA] … re-injected: ITicketService → SqlTicketService #xxxx · IAuditLogService → SqlAuditLogService #xxxx · ILog unchanged`; the grid's Implementation column changes; the refresh logs `[DATA] SqlTicketService … would run: SELECT … FROM dbo.Tickets …`; the handlers and the presenter are the same code |
| **▶ Run presenter tests** | progress | the progress bar advances 8 steps (one test per `Timer` tick); each test builds `new TicketWorkflowPresenter(fakes…)` and the trace shows `[INFRA] PresenterTests — ✔ 1/8 Close_reports_success_and_records_an_audit_entry` … `✔ 8/8 …` between the fakes' `[SVC]`/`[DOMAIN]`/`[DATA]` lines; status **● 8/8 tests passed**, green banner |
| **Close #1041 as Viewer** | failure 1 (permission) | the combo jumps to Sam Okafor (Viewer); `[SVC] FakePermissionService.CanClose — Sam Okafor (Viewer) on #1041 → denied`, `[SVC] ⚠ … IPermissionService.CanClose(#1041) → denied`; red banner **Sam Okafor (Viewer) may not close ticket #1041.**; no `[DOMAIN]`, no audit line |
| **Close #1042 without hours** | failure 2 (domain rule) | signs Dana back in; `CanClose … → allowed`, then `[DOMAIN] ⚠ Ticket.CanClose — #1042 rejected: Log hours before closing.`; orange banner **Log hours before closing.**; status **● invalid** |
| **Close ticket** with an empty reason | failure 3 (input) | `[SVC] ⚠ … rejected before any service call: no reason given`; banner **Give a reason before closing.** — no `[DATA]` line follows |
| **Resolve a missing service** | failure 4 (DI) | `[UI] → Application.Services.GetService<IExportService>()`, `[INFRA] ⚠ Application.Services — IExportService is not registered (HasService = False, GetService = null) …`; orange banner **Exporting is not available in this profile. Ask an administrator to check the service registration.**; status **● feature unavailable** |
| **Simulate data outage** | error path | `[DATA] ✖ FakeTicketService.GetOpenTicketsAsync — outage: read Tickets failed — timeout connecting to sql01:1433 …` stays in the trace; the user sees only the red banner **The action could not be completed. Check the log for details.** and a toast; status **● failed** |
| **Recover the data store** (same button) | recovery | the service answers again, the grid reloads, status **● ready** |
| **Clear trace** | — | empties the right-hand card |

### Two tabs (Session vs Shared)

Open <http://localhost:5108> in a second tab. Its trace starts with `… registration already in place (application-wide) ·
2 active session(s)` — the registration table is shared, the instances are not: the `ITicketService`, `IUserService` and `ILog`
rows show **different** instance ids from tab 1, the `IAuditLogService` row the **same** id, and **Audit trail** already counts
tab 1's entries. Close a ticket in tab 1 → after **↻ Refresh** tab 2 still lists it (per-session store) but its audit count went up.
Change the operator in tab 1 → tab 2's combo does not move.

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Service registration method (fake + production profiles) | `Infrastructure/ServiceRegistration.cs` — `RegisterFakeServices`, `RegisterProductionServices`, idempotent `Apply`; `Infrastructure/ActiveProfile.cs` describes the live profile; `Program.cs` picks it (`?profile=production`) |
| 2 | Five interfaces and fake implementations | `Services/ITicketService.cs` + `FakeTicketService.cs`, `IUserService.cs` + `FakeUserService.cs`, `IPermissionService.cs` + `FakePermissionService.cs`, `INotificationService.cs` + `FakeNotificationService.cs`, `IAuditLogService.cs` + `FakeAuditLogService.cs` (production-shaped: `SqlTicketService`, `DirectoryUserService`, `RolePermissionService`, `EmailNotificationService`, `SqlAuditLogService`) |
| 3 | Injected MainPage/Form | `Views/TicketWorkflow.cs` — `[Inject(Required = true)]` properties for the five services, `[Inject]` for `ILog`, `DataStoreHealth`, `ActiveProfile`; `EnsureInjected` documents the automatic path and the `Application.Services.Inject(this)` fallback |
| 4 | Presenter / workflow service for one screen | `Services/TicketWorkflowPresenter.cs` + `Services/WorkflowResult.cs`; driven without a browser by `Diagnostics/PresenterTestRunner.cs` |
| 5 | Service lifetime table | [`docs/ServiceLifetimeTable.md`](TicketOps/docs/ServiceLifetimeTable.md) — mirrored live in the **Injected services** grid (`Diagnostics/ServiceProbe.cs`) |
| 6 | Every path visible without leaking internals | `TicketWorkflow.ShowResult` / `ReportFailure`, `Resources/Strings.cs`, the trace panel — see the table above |
| 7 | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |
| — | Testability notes (what moved out of the Form, the fakes, the eight tests) | [`docs/TestabilityNotes.md`](TicketOps/docs/TestabilityNotes.md) |

## Where things live

```
TicketOps/
├─ Views/
│  ├─ TicketWorkflow.cs             the screen: [Inject] properties, thin handlers → presenter, ShowResult / ReportFailure
│  └─ TicketWorkflow.Designer.cs    GENERATED-style layout (opens in the Wisej Designer) — no logic here
├─ Controls/StatusBanner            reusable "● state" + banner UserControl (display only)
├─ Services/
│  ├─ ITicketService / FakeTicketService / SqlTicketService            load & transition tickets (Session)
│  ├─ IUserService / FakeUserService / DirectoryUserService             who is signed in (Session)
│  ├─ IPermissionService / FakePermissionService / RolePermissionService  what they may do (Session)
│  ├─ INotificationService / FakeNotificationService / EmailNotificationService  toasts & alerts (Transient)
│  ├─ IAuditLogService / FakeAuditLogService / SqlAuditLogService        who did what, when (Shared)
│  ├─ TicketWorkflowPresenter.cs    the screen's decisions: input → exists → permission → rule → audit → notify
│  └─ WorkflowResult.cs             Ok / Invalid / Denied / NotFound + a message the user may read
├─ Domain/
│  ├─ Ticket.cs                     record + its rule (CanClose / Close / AssignTo); no Wisej.NET dependency
│  ├─ Operator.cs, AuditEntry.cs, Notification.cs, OperationResult.cs
├─ Data/
│  ├─ SeedData.cs                   demo operators and tickets (pure functions)
│  ├─ TicketTable.cs                in-memory table used by the fake and as the production stand-in
│  └─ DataStoreHealth.cs            the session's outage switch + DataOutageException
├─ Infrastructure/
│  ├─ ServiceRegistration.cs        Application.Services registrations: two profiles, lifetimes, idempotent Apply
│  ├─ ActiveProfile.cs              which profile is live + the lifetime table as data
│  └─ ILog.cs / ActivityLog.cs      cross-cutting logging (registered as a Session service)
├─ Diagnostics/
│  ├─ ActivityTracePanel            the live trace card
│  ├─ ServiceProbe.cs               resolves each contract twice → the "Injected services" grid
│  └─ PresenterTestRunner.cs        the eight presenter tests, run against fakes from the bottom bar
├─ Resources/Strings.cs             safe user-facing messages
├─ docs/                            the deliverables
├─ Program.cs                       session entry point: pick the profile, register, new TicketWorkflow().Show()
└─ Startup.cs                       Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **Which services must be session-scoped, and why?**
  `IUserService` (it *is* "who is signed in"), `ITicketService` (the open-ticket list and its changes are per user),
  `IPermissionService` (it holds the session `IUserService` — a service cannot outlive a collaborator it captured), plus the
  infrastructure that is per tab: `ILog` (the trace) and `DataStoreHealth` (the outage switch). `IAuditLogService` is the
  opposite case — one trail for the whole server — and stays Shared *because* it holds nothing per user: the operator id is a
  parameter of every call. `INotificationService` is stateless and cheap, so Transient.
- **What breaks if a shared/singleton service stores a selected ticket ID?**
  Every session on the server shares that one field. User B's click overwrites user A's selection, A closes B's ticket, and the
  bug appears only with two users — the `static` field from Module 1 wearing a DI costume. The two-tabs step above shows the
  correct behaviour: tab 2's `IAuditLogService` is the same instance as tab 1's (Shared, safe because it is append-only and
  thread-safe), while its `ITicketService` and `IUserService` are different instances (Session).
- **For each screen, could you unit-test its decision by handing a presenter a fake service — with no browser?**
  Yes: **▶ Run presenter tests** does exactly that eight times — `new TicketWorkflowPresenter(new FakeTicketService(…),
  new FakeUserService(…), new FakePermissionService(…), new FakeNotificationService(…), new FakeAuditLogService(), log)` — and
  asserts on the `WorkflowResult` and on what the fakes recorded. The Form never appears in the fixture; the same methods move
  to a test project unchanged.
