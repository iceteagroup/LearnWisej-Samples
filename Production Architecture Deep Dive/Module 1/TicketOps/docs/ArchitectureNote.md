# Architecture note — where new code goes

*Module 1 deliverable · TicketOps Console*

The TicketOps Console is a Wisej.NET 4 application. Every control is a server-side .NET object mirrored
as a browser widget; our C# runs on the server, so a handler is a trusted place to enforce rules — but
only if the rule is somewhere a second screen, a bulk job or a test can also reach it. The structure
below exists for that reason.

## The rule in one line

**Controls display and collect state; services decide what the application means.**
A click handler reads the screen, hands off to a service, and shows the result — nothing more.

## The boundaries and their direction

```
Views / Controls  ──►  Services  ──►  Domain
                            │
                            └────►  Data
Infrastructure (ILog, config)  ◄── available to every layer
Diagnostics (logging, health)   ◄── records what happened, never decides
```

Arrows only ever point "downward". UI depends on services; services depend on domain and data; the
domain depends on nothing. Never the other way around: `Domain/` and `Services/` would compile in a
class library that does not reference Wisej.NET.

## Where does this go?

| You are adding… | Put it in | Example in this app |
|---|---|---|
| A screen the user works on | `Views/` — a Form/Page + `.Designer.cs`, named after what the user does there | `Views/TicketEditor` (not `Form3`) |
| A piece of UI used by more than one screen | `Controls/` — a UserControl | `Controls/StatusBanner` |
| A decision, a workflow, a rule that two screens share | `Services/` — behind an `IXxxService` interface | `ITicketService.SaveAsync` (validation), `CloseAsync` |
| A business record and the rules that belong to it | `Domain/` | `Ticket.CanClose` ("log hours before closing") |
| The shape the screen collects / the answer it shows | `Domain/` | `TicketDraft`, `OperationResult<T>` |
| Persistence and queries | `Data/` — behind an `IXxxRepository` | `InMemoryTicketRepository` (today), `SqlTicketRepository` (tomorrow) |
| The logging contract, configuration, wiring, integration clients | `Infrastructure/` | `ILog`, `AppComposition` |
| Text the user reads, images, themes, `.resx` | `Resources/` | `Strings.ActionFailed` |
| Logging and error tracking, health checks, troubleshooting pages | `Diagnostics/` | `ActivityLog` |

## Three tests before you commit

1. **Does the boundary earn its keep?** Add a service when it removes duplication, isolates a
   dependency (database, HTTP API, license check) or makes a workflow testable without a control.
   A label that formats a value already on screen stays in the screen. "We might need it next
   quarter" is not a reason.
2. **Can the logic run without a Wisej.NET control?** If a rule secretly reads `textTitle.Text`, it is
   stuck in the UI. Move it behind the service and hand it a `TicketDraft`.
3. **Does the screen get its dependency, or reach for one?** `TicketEditor(ITicketService, …)` is
   right. `TicketService.Current` or `new TicketService()` inside the form is wrong: a static is
   shared by every session on the server, and a `new` hides the dependency from tests.
   `AppComposition` is the one place that knows who gets what — one instance per session.

## Naming

Line the pieces of a feature up by name so a teammate who finds one can guess the others:
`Ticket` · `TicketDraft` · `ITicketService` · `TicketService` · `ITicketRepository` · `TicketEditor`.
Name handlers after the control and the event (`buttonSave_Click`), and keep them under a dozen lines.

## Failure paths

- **Expected outcome** (invalid input, a rule says no): the service returns `OperationResult.Fail(message)`
  with a sentence the user may read. Nothing is thrown, nothing is persisted.
- **Unexpected failure** (data store down, bug): the exception reaches the handler's `catch`, goes to
  `ILog.Error` with its type and message, and the user sees `Strings.ActionFailed`. Host names, SQL
  and stack traces never cross into a label.

## Designer

Keep every Form/Page/UserControl designable. The `.Designer.cs` is regenerated whenever a control
moves; put code in the main partial class only. Keep a parameterless constructor for the Designer
and a real constructor that takes the services.
