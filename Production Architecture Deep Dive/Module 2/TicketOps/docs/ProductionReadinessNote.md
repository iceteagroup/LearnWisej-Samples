# Production-readiness note — startup, configuration, session state & lifetime

*Module 2 deliverable · lab step 9*

## The three moments, and what TicketOps does in each

| Moment | Runs | TicketOps code | Holds |
|---|---|---|---|
| Process start | once per process | `Startup.cs` (Kestrel + `app.UseWisej()`), then lazily `ProcessScope.LoadSettings` on the first session | `AppSettings` (immutable), `SharedTicketStore.Instance` (thread-safe), `SharedCounters` |
| Session start | once per browser session | `Program.Main` → `new AppComposition()` → `CreateMainView().Show()` | `ActivityLog`, `SessionContext`, `InMemoryUserDirectory`, `InMemoryTicketRepository`, `SessionService`, `TicketService`, `DiagnosticsService`, `SessionDiagnostics` (the Form) |
| Session end / timeout | once per session | Wisej.NET drops the session's graph | nothing survives except the shared, process-scoped objects above |
| A click | once per request | the handler's locals (`user`, `tenant`, `result`) | nothing after the round-trip |

Confusing "once per process" with "once per session" is how a shared object ends up where a private one
belonged. The diagnostics page makes the difference visible: the *Application* panel is identical in every
tab, the *This session* panel is not.

## The five scopes, applied

| Scope | TicketOps example | Object |
|---|---|---|
| Application | environment, dispatch URL, upload limit, the ticket store, the session counter | `AppSettings`, `SharedTicketStore`, `SharedCounters` |
| Session | operator, tenant, theme, client profile, selected ticket, the log | `SessionContext`, `ActivityLog` |
| User | who Alice/Bob/Sara/Jae are and which tenants they belong to | `UserAccount` (from `IUserDirectory`) |
| Browser tab | one Wisej.NET session per tab in this app (two tabs = two sessions, like two launches of a desktop app) | — |
| Request / thread | the values a handler reads from the combos, a validation result | locals |

Durable business data is **not** in any of these: tickets live in the repository (the shared store today, a
database tomorrow). Session state is the working set of one live console — losing it on disconnect corrupts
nothing.

## What is production-ready

- One `SessionContext` per session, constructor-injected into the Form and every service; no
  `SessionContext.Current`, no static accessor, no per-user static anywhere (`StaticStateAudit.md`).
- Every change to the context goes through `ISessionService`, so the rules (tenant membership, allowed
  themes) live in one testable place and expected refusals are results, not exceptions.
- Shared state is either immutable (`AppSettings`) or thread-safe (`SharedTicketStore` under a lock,
  `SharedCounters` with `Interlocked`) and carries no per-user meaning.
- Configuration is a documented contract (`ConfigurationContract.md`), read once; secrets are not in it.
- Failure paths are visible and safe: a refused tenant switch shows the reason in a banner; an unexpected
  exception goes to `ILog` and the user reads only `Strings.ActionFailed`.

## Before this goes live

| Gap | Why it matters | Module |
|---|---|---|
| `CurrentUser` is seeded as "Alice Rivera" and switched from a combo | identity must come from authentication (`Application.User`), and switching operators must be a sign-out/sign-in | 11 |
| Hand-written `AppComposition` | fine at this size; a container with `ServiceLifetime.Session` scales the same rule to dozens of services and disposes `IDisposable` session objects for you | 8 |
| `Application.Theme = ThemeCatalog.Load(name)` reads the theme JSON from the framework's embedded resources each time | cache the parsed themes read-only at process scope if theme switching is frequent; keep the *choice* per session | 10 |
| `SharedTicketStore` is in-memory | it stands in for the database; with two web servers behind a load balancer each process has its own — real persistence is required | 12 |
| Session timeout is 20 minutes with the default "prolong?" dialog | decide the operational value and what happens to unsaved work (`Application.SessionTimeout` event) | 12 |
| `SessionContext` holds nothing disposable | if it ever holds a reader or a temp-file handle for an upload, implement `IDisposable` so it is released with the session, and never let a singleton keep a reference to it | — |

## Two sessions, no leak — the check the lab asks for

Open <http://localhost:5102> in two tabs and follow the steps in `StaticStateAudit.md` → *Evidence*.
Expected: everything changed through **Apply to this session** or by selecting a ticket differs between the
tabs; *Environment / build*, *Dispatch API base URL*, *Upload limit*, *Idle session timeout*, *Default theme*
and *Tickets in the store* are identical in both.
