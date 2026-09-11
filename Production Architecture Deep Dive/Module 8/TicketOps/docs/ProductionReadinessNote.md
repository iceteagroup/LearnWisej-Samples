# Production-readiness note — services, registration profiles and injection

*Module 8 deliverable (lab step 9) · TicketOps Console*

## What changed since Module 1

| Module 1 | Module 8 |
|---|---|
| `AppComposition` — a hand-written factory, one per session, constructor injection into `TicketEditor` | `ServiceRegistration` — `Application.Services` registrations with explicit lifetimes; `[Inject]` properties on `TicketWorkflow` |
| one `ITicketService` | five contracts: `ITicketService`, `IUserService`, `IPermissionService`, `INotificationService`, `IAuditLogService` |
| one fake implementation | a **fake profile** and a **production profile** per contract, chosen at startup |
| decisions in `TicketService` | decisions in `TicketWorkflowPresenter` (workflow) + the services (rules) — the Form is passive |

The rule did not change: *controls display and collect state; services decide what the application means.* The container just
replaced the factory as the thing that hands services out.

## Registration profiles

`ServiceRegistration.RegisterFakeServices` and `RegisterProductionServices` register the **same five contracts** with different
implementations. `Program.Main` picks one:

```
http://localhost:5108                      → fake (default)
http://localhost:5108/?profile=production  → production-shaped stand-ins
```

Not one line of the handlers or the presenter changes between the two.

Production-shaped means: real class names and real call shapes (`SqlTicketService`, `DirectoryUserService`, `EmailNotificationService`,
`SqlAuditLogService`, `RolePermissionService`), but no database, network or SMTP in this lab — each one logs the statement or
request it would send and keeps working on local data. Replacing the body of those classes with real I/O is the whole
remaining distance to production; nothing above them moves.

## Readiness checklist

| Check | Status | Where |
|---|---|---|
| Services are resolved through DI, never `new`-ed inside a handler | ✔ | `grep -n "new Fake\|new Sql" Views/` finds nothing; the only `new` in the Form is `new TicketWorkflowPresenter(...)` fed with injected interfaces |
| Every registration states its lifetime, and the lifetime matches the state it holds | ✔ | `docs/ServiceLifetimeTable.md` |
| No per-user state in a Shared service or a static | ✔ | `IAuditLogService` takes the operator id per call; the only statics are `ServiceRegistration.Gate` (a lock), `SeedData` (pure functions), `RolePermissionService.Matrix` (a constant table), `Strings` |
| Registration is idempotent across sessions | ✔ | `ServiceRegistration.Apply` checks `HasService` / the live `ActiveProfile` before touching the container |
| Failure paths visible without leaking internals | ✔ | permission (`Denied`), domain rule and input (`Invalid`), unexpected exceptions (`Strings.ActionFailed`, details in `ILog`) |
| Decisions are unit-testable with fakes | ✔ | `docs/TestabilityNotes.md`; `Diagnostics/PresenterTestRunner.cs` |
| Designer-friendly | ✔ | `TicketWorkflow` keeps a parameterless constructor (the only one); layout in `TicketWorkflow.Designer.cs` |

## Things to decide before real production

1. **Profile selection is a deployment setting, not a URL parameter.** In production, pick the profile from configuration at
   startup. Because the registration table is application-wide, a different `?profile=` on a later session re-registers for
   every session on the server.
2. **Injection timing.** Wisej.NET injects top-level containers (Form/Page/Desktop) automatically; `TicketWorkflow.EnsureInjected`
   checks in `Load` and falls back to `Application.Services.Inject(this)`. Keep the fallback for UserControls you create yourself,
   which are not injected automatically.
3. **`Required = true`.** A required service that nobody registered fails at injection time (before `Load`), which is the right
   moment: the app should not start half-wired. Optional (`[Inject]`) properties stay null and the code must check them.
4. **Constructor injection stays explicit.** The factory lambdas call each constructor with collaborators from the same container.
   That keeps every service constructible in a test with `new`, and it never relies on the container guessing a constructor.
5. **Disposal.** Session and Thread instances that implement `IDisposable` are disposed by the container at the end of the
   session/request; Transient ones are the caller's responsibility. None of the lab services hold unmanaged resources; a real
   `SqlTicketService` would implement `IDisposable` and release its connection scope.
