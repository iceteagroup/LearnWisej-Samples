# TicketOps · Production Architecture Deep Dive · Module 8

Local lab build for **Module 8 · Services, Dependency Injection & Testable UI Patterns**. It follows the
walkthrough video *Inject services for a testable UI*: Module 1's hand-written `AppComposition` factory is
replaced by `Application.Services` registrations with explicit lifetimes; the screen asks for five
**capabilities** — `ITicketService`, `IUserService`, `IPermissionService`, `INotificationService`,
`IAuditLogService` — through `[Inject]` properties instead of creating concrete classes; each contract has a
**fake** (in-memory) and a **production-shaped** implementation behind a registration profile chosen at startup;
and the screen's decisions live in a plain `TicketWorkflowPresenter` that tests drive with fakes and no browser.

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

## What to try in the Ticket Workflow window

| Action | What you should see |
|---|---|
| Select #1041, type a reason, **Close ticket** | status **● Ticket #1041 closed.**; the row leaves the grid |
| Select #1044, **Assign to me** | the Assignee column shows Dana Reyes |
| **Close ticket** with an empty reason | banner **Give a reason before closing.** |
| Select #1042 (no hours logged), give a reason, **Close ticket** | orange banner **Log hours before closing.** |
| **Signed in as** → Sam Okafor (Viewer), then **Close ticket** | red banner **Sam Okafor (Viewer) may not close ticket #1041.** — a second tab keeps its own operator |
| Open `?profile=production` and repeat | the same screen and results; the server console shows `would run: UPDATE dbo.Tickets …` |

## Deliverables (lab guide)

| # | Deliverable | Where |
|---|---|---|
| 1 | Service registration method (fake + production profiles) | `Infrastructure/ServiceRegistration.cs` — `RegisterFakeServices`, `RegisterProductionServices`, idempotent `Apply`; `Infrastructure/ActiveProfile.cs` describes the live profile; `Program.cs` picks it (`?profile=production`) |
| 2 | Five interfaces and fake implementations | `Services/ITicketService.cs` + `FakeTicketService.cs`, `IUserService.cs` + `FakeUserService.cs`, `IPermissionService.cs` + `FakePermissionService.cs`, `INotificationService.cs` + `FakeNotificationService.cs`, `IAuditLogService.cs` + `FakeAuditLogService.cs` (production-shaped: `SqlTicketService`, `DirectoryUserService`, `RolePermissionService`, `EmailNotificationService`, `SqlAuditLogService`) |
| 3 | Injected MainPage/Form | `Views/TicketWorkflow.cs` — `[Inject(Required = true)]` properties for the five services, `[Inject]` for `ILog`; `EnsureInjected` falls back to `Application.Services.Inject(this)` |
| 4 | Presenter / workflow service for one screen | `Services/TicketWorkflowPresenter.cs` + `Services/WorkflowResult.cs`; tested without a browser by `Diagnostics/PresenterTestRunner.cs` |
| 5 | Service lifetime table | [`docs/ServiceLifetimeTable.md`](TicketOps/docs/ServiceLifetimeTable.md) |
| 6 | Every path visible without leaking internals | `TicketWorkflow.ShowResult` / `ReportFailure`, `Resources/Strings.cs` |
| 7 | Production-readiness note | [`docs/ProductionReadinessNote.md`](TicketOps/docs/ProductionReadinessNote.md) |
| — | Testability notes (what moved out of the Form, the fakes, the eight tests) | [`docs/TestabilityNotes.md`](TicketOps/docs/TestabilityNotes.md) |

## Where things live

```
TicketOps/
├─ Views/TicketWorkflow             the screen: [Inject] properties, thin handlers → presenter, ShowResult / ReportFailure
├─ Controls/StatusBanner            reusable "● state" + banner UserControl (display only)
├─ Services/
│  ├─ ITicketService / FakeTicketService / SqlTicketService            load & transition tickets (Session)
│  ├─ IUserService / FakeUserService / DirectoryUserService             who is signed in (Session)
│  ├─ IPermissionService / FakePermissionService / RolePermissionService  what they may do (Session)
│  ├─ INotificationService / FakeNotificationService / EmailNotificationService  toasts & alerts (Transient)
│  ├─ IAuditLogService / FakeAuditLogService / SqlAuditLogService        who did what, when (Shared)
│  ├─ TicketWorkflowPresenter.cs    the screen's decisions: input → exists → permission → rule → audit → notify
│  └─ WorkflowResult.cs             Ok / Invalid / Denied / NotFound + a message the user may read
├─ Domain/                          Ticket (CanClose / Close / AssignTo), Operator, AuditEntry, Notification, OperationResult
├─ Data/                            SeedData, TicketTable (in-memory table)
├─ Infrastructure/                  ServiceRegistration, ActiveProfile, ILog / ActivityLog (Session service, server console)
├─ Diagnostics/PresenterTestRunner.cs  the eight presenter tests, against fakes
├─ Resources/Strings.cs             safe user-facing messages
├─ docs/                            the deliverables
├─ Program.cs                       session entry point: pick the profile, register, new TicketWorkflow().Show()
└─ Startup.cs                       Kestrel host (app.UseWisej())
```

## Self-check answers (lesson guide)

- **Which services must be session-scoped, and why?**
  `IUserService` (it *is* "who is signed in"), `ITicketService` (the open-ticket list and its changes are per user),
  `IPermissionService` (it holds the session `IUserService` — a service cannot outlive a collaborator it captured), plus the
  session log (`ILog`). `IAuditLogService` is the opposite case — one trail for the whole server — and stays Shared *because*
  it holds nothing per user: the operator id is a parameter of every call. `INotificationService` is stateless and cheap, so Transient.
- **What breaks if a shared/singleton service stores a selected ticket ID?**
  Every session on the server shares that one field. User B's click overwrites user A's selection, A closes B's ticket, and the
  bug appears only with two users — the `static` field from Module 1 wearing a DI costume.
- **For each screen, could you unit-test its decision by handing a presenter a fake service — with no browser?**
  Yes: `PresenterTestRunner` does exactly that eight times — `new TicketWorkflowPresenter(new FakeTicketService(log),
  new FakeUserService(), new FakePermissionService(users), new FakeNotificationService(log), new FakeAuditLogService(), log)` —
  and asserts on the `WorkflowResult` and on what the fakes recorded. The Form never appears in the fixture.
