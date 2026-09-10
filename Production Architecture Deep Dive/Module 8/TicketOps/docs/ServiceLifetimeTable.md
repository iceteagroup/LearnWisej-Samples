# Service lifetime table

*Module 8 deliverable · TicketOps Console*

Registered in `Infrastructure/ServiceRegistration.cs` through `Application.Services` (a `Wisej.Services.ServiceProvider`).
The same table is built in code (`ActiveProfile.Entries`) and shown live in the **Injected services** grid, where every
contract is resolved twice so the lifetime can be seen, not just read.

## The question to ask for every service

> If two users hit this at once, is sharing one instance correct?
> Yes and it holds no per-user state → **Shared**. It holds per-user state (or a per-user collaborator) → **Session**.
> Cheap one-shot worker → **Transient**. One per request thread → **Thread** (rare; not used here).

## The table

| Contract | Fake profile | Production profile | Lifetime | Why this lifetime | Never put here |
|---|---|---|---|---|---|
| `ITicketService` | `FakeTicketService` (in-memory table) | `SqlTicketService` (stand-in: logs the T-SQL) | **Session** | the open-ticket list is per user: closing #1041 in one tab must not close it in another user's tab; the session disposes it when the user leaves | a server-wide cache of "the" tickets — that is a database's job, not a service instance's |
| `IUserService` | `FakeUserService` | `DirectoryUserService` (stand-in: logs the directory query) | **Session** | `Current` is "who is signed in" — the definition of per-session state | anything Shared: one `Current` field would be overwritten by every login on the server |
| `IPermissionService` | `FakePermissionService` (+ `DenyEverything` for tests) | `RolePermissionService` (role → action matrix) | **Session** | it takes the Session `IUserService` in its constructor; a service can never outlive a collaborator it captured (a Shared permission service holding a Session user service would answer for the wrong user) | a cached "last decision" |
| `INotificationService` | `FakeNotificationService` (records `Sent`) | `EmailNotificationService` (stand-in: logs the SMTP send) | **Transient** | stateless, cheap, one-shot; a fresh instance per resolve cannot leak anything between callers — the trace shows a different `#id` per resolve | an outbox that must survive the call — that would need Session or a store |
| `IAuditLogService` | `FakeAuditLogService` | `SqlAuditLogService` (stand-in: builds the INSERT) | **Shared** | one append-only trail for the whole server, thread-safe (`lock`); the operator id travels with every `Record` call, so the service never needs to know "the current user" | the current user, a selected ticket, the session `ILog` |
| `ILog` | `ActivityLog` | `ActivityLog` | **Session** | the activity trace is per browser tab; a Shared log would interleave every user's clicks | — |
| `DataStoreHealth` | `DataStoreHealth` | `DataStoreHealth` | **Session** | the lab's outage switch breaks one tab's data store, not the server's | — |
| `ActiveProfile` | (describes the fake registrations) | (describes the production registrations) | **Shared** | the registration table itself is application-wide; this object only describes it and holds nothing per user | — |

Registration forms used (all on `Application.Services`):

- `AddService<TService, TImpl>(ServiceLifetime.Session)` — contract → type, for classes with a parameterless constructor (`ActivityLog`, `DataStoreHealth`).
- `AddService<TService>(Func<Type, object> factory, lifetime)` — contract → factory, so the factory can call the implementation's constructor with its collaborators (`new FakeTicketService(log, health)`); the constructor stays the honest list of what the class needs.
- `AddService<TService>(object instance, ServiceLifetime.Shared)` — contract → the one instance every session shares (`FakeAuditLogService`, `ActiveProfile`).
- `AddOrReplaceService…` (same three shapes) when a profile is switched; `HasService<T>()` decides which of the two to call; `GetService<T>()` resolves; `Inject(object)` fills `[Inject]` properties on demand.

## What the lifetime is *not*

The registration **table** is application-wide (one `ServiceProvider` per server process, `Program.Main` runs per session, and
`AddService` throws on a duplicate — hence the idempotent `ServiceRegistration.Apply`). The **instances** follow the lifetime:
a Session registration still yields one object per browser session. So "registered once for the whole app" and "one instance
per user" are both true of `ITicketService`, and neither requires a static field.

## The classic mistake, on this screen

If `IAuditLogService` (Shared) held a `CurrentOperator` property that `TicketWorkflow` set on sign-in, then operator A signing in
in one tab would make every other tab's audit entries carry A's id — the `static` bug from Module 1 wearing a DI costume. That is
why `Record(action, ticketId, operatorId, detail)` takes the operator explicitly, and why the shared service also refuses the
session `ILog`: the presenter (Session-scoped) logs on its behalf.

## Evidence

- On load the **Injected services** grid shows seven rows: `ITicketService … Session … same instance`,
  `INotificationService … Transient … new instance #xxxx (transient)`, `IAuditLogService … Shared … same instance`.
- Open a second browser tab: its `ITicketService` row shows a **different** instance id (Session), its `IAuditLogService` row the
  **same** id as the first tab (Shared), and **Audit trail: n entries** counts what the first tab did.
- Close a ticket in tab 1; tab 2's list still shows it open after **↻ Refresh** (per-session ticket store), while tab 2's audit
  count went up by one (shared trail).
- Sign in as a different operator in tab 1; tab 2's combo does not move (Session `IUserService`).
