# Testability notes — how the Ticket Workflow screen became testable without a browser

*Module 8 deliverable · TicketOps Console*

## The seam

`Views/TicketWorkflow.cs` depends on five **interfaces** it did not create:

```csharp
[Inject(Required = true)] public ITicketService Tickets { get; set; }
[Inject(Required = true)] public IUserService Users { get; set; }
[Inject(Required = true)] public IPermissionService Permissions { get; set; }
[Inject(Required = true)] public INotificationService Notifications { get; set; }
[Inject(Required = true)] public IAuditLogService Audit { get; set; }
```

Wisej.NET creates the Form, so the Form cannot take them by constructor — they arrive by **property injection** from
`Application.Services`. Everything the Form does with them happens through one plain class:

```csharp
_presenter = new TicketWorkflowPresenter(Tickets, Users, Permissions, Notifications, Audit, _log);
```

`Services/TicketWorkflowPresenter.cs` takes its collaborators by **constructor injection**, references no Wisej.NET type, and
returns a `WorkflowResult` (`Ok` / `Invalid` / `Denied` / `NotFound` + message). The Form reads the controls, calls the presenter,
renders the result. That is the whole division of labour:

| Stays in the Form (appearance) | Moved to the presenter (decisions) |
|---|---|
| which grid row is selected, the reason text | is a reason required, does the ticket exist |
| enabling buttons, showing the banner, the toast | may this operator close / take this ticket |
| refreshing the grid after success | what gets audited and who gets notified |
| — | the domain rule (delegated to `Ticket.CanClose` through `ITicketService`) |

## The fakes

Every interface has a fake in `Services/Fake*.cs`. Two of them are deliberately more useful than the real thing in a test:

- `FakePermissionService.DenyEverything = true` reaches the presenter's "denied" branch without arranging roles and tickets.
- `FakeNotificationService.Sent` and `FakeAuditLogService.Recent(n)` expose what the presenter caused, so a test can verify the
  **interaction**, not only the return value.

The fakes are what the **fake registration profile** hands to the running app too, which is why the tests and the screen exercise
the same code path.

## The tests (Diagnostics/PresenterTestRunner.cs — "▶ Run presenter tests")

Each test builds a fresh fixture by constructor — `new FakeTicketService(log, health)`, `new FakeUserService(log)`,
`new FakePermissionService(users, log)`, `new FakeNotificationService(log)`, `new FakeAuditLogService()`,
`new TicketWorkflowPresenter(...)` — calls one presenter method and asserts. No container, no Form, no session.

| # | Test | Asserts |
|---|---|---|
| 1 | `Close_reports_success_and_records_an_audit_entry` | `Ok`; ticket `Closed`; one `close` audit entry; assignee notified |
| 2 | `Close_is_denied_for_a_viewer` | `Denied`; nothing audited; ticket still open |
| 3 | `Close_is_denied_when_the_permission_fake_says_no` | `Denied` via `DenyEverything`; nobody notified |
| 4 | `Close_without_hours_is_rejected_by_the_domain_rule` | `Invalid` with `"Log hours before closing."`; nothing audited |
| 5 | `Close_without_a_reason_never_reaches_a_service` | `Invalid`; ticket untouched; no side effects |
| 6 | `Close_of_an_unknown_ticket_reports_not_found` | `NotFound` |
| 7 | `Assign_to_me_notifies_the_new_assignee` | `Ok`; assignee = operator 1; notification + `assign` audit entry |
| 8 | `Data_outage_surfaces_as_an_exception_not_a_result` | `DataOutageException` propagates; nothing audited |

In a real solution these eight methods move to a test project under `[Test]`/`[Fact]` unchanged — the in-app runner exists so the
lab can show them without a test runner installed. The `Assert` helper throws `InvalidOperationException`; the runner turns it
into a ✖ line in the trace and a failed count in the status.

## What is *not* tested here, on purpose

- Whether `[Inject]` fills the properties: that is framework behaviour, verified by running the app (the trace prints which path
  ran — automatic injection or an explicit `Application.Services.Inject(this)`).
- Layout, colours, the Designer file.
- The production-shaped services: they are stand-ins; a real `SqlTicketService` would get integration tests against a database.

## Keeping the container out of the logic

Neither the presenter nor any service calls `Application.Services`. Only three places touch the container: `Program.Main`
(pick a profile), `ServiceRegistration` (the registrations), and `Diagnostics/ServiceProbe` + the Form's `EnsureInjected` /
profile-switch handler (diagnostics and the fallback `Inject(this)`). A service that fetched its own dependencies mid-method would
hide them from the constructor — and from the tests.

## Evidence

- **▶ Run presenter tests** → the progress bar advances 8 steps; the trace shows `[INFRA] PresenterTests — ✔ 1/8 …` through
  `✔ 8/8 …` interleaved with the `[SVC]`/`[DOMAIN]`/`[DATA]` lines the fakes wrote; status **● 8/8 tests passed**; green banner.
- **Close ticket** on #1041 with a reason → the same `[SVC] TicketWorkflowPresenter.CloseAsync` lines appear as in test 1, now
  driven by the injected services instead of the fixture.
- **Switch to production** → the same handlers produce `[DATA] SqlTicketService … would run: UPDATE dbo.Tickets …` — the Form and
  the presenter did not change.
