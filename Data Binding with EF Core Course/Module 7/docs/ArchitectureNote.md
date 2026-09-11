# Architecture note

One page, for the capstone hand-in: the four lifetimes, the service boundary, the DI bridge, and where every
`DbContext` in this solution is created and disposed.

## The four lifetimes

| Lifetime | Owner | Scope |
|---|---|---|
| Session state | Wisej.NET | `TicketBrowserPage` — created once per browser session (`Program.Main`), lives as long as the tab. Holds `ticketBindingSource` (the `List<TicketListItem>` the last search returned), the current page index and total, and `cboRole`'s selection. |
| UI object | `this` (a `Page` or `Form`) | `TicketBrowserPage` itself; `TicketEditorForm` and `ConflictDialog` for as long as their modal is open — created, shown with `ShowDialogAsync`, disposed (`using`) when it closes. |
| Request / thread | One click | One `async void` handler run. Continuations after an `await` may resume on any thread; `Application.Update(this)` in every handler's `finally` pushes the final UI state regardless of which thread got there. |
| Unit of work | `DbContext` | Created from `IDbContextFactory<SupportDeskContext>`, used for exactly one query or one save, disposed (`await using`) before the owning method returns. **Never** session state, **never** a field on a `Page`/`Form`. |

The habit the whole course teaches, unchanged since Module 1: **forms, pages, `BindingSource`s and the
selected row live for the session; a `DbContext` lives for one operation.**

## The service boundary

```
SupportDesk.Web (Wisej.NET forms/pages)
    │  [Inject] properties resolved through the Microsoft DI bridge (see below)
    ▼
SupportDesk.Services (stateless, UI-free — no reference to Wisej.Web)
    TicketQueryService        read-side: search, count, lookups (no-tracking)
    TicketCommandService      write-side: load-for-edit, save (including Overwrite), delete
    TicketValidator           DataAnnotations + the cross-field rule — no database, no UI
    ConflictResolution        turns DbUpdateConcurrencyException into ConflictField/ConflictSet — no UI
    DevelopmentSeeder, SchemaInfoService, ModelDemoService, …
    │  holds only IDbContextFactory<SupportDeskContext>
    ▼
SupportDesk.Data (EF Core — the only project referencing the SQLite provider)
    SupportDeskContext, Entities/, Migrations/, Diagnostics/ (QueryTrace, QueryTraceInterceptor —
    test instrumentation: SqliteTestFactory adds the interceptor, the application does not register it)
```

Every service in `SupportDesk.Services` is **Transient** and holds only the factory (`TicketCommandService`
also takes the stateless `ConflictResolution`) — never a `DbContext`, never mutable session state of its
own. `TicketValidator` and `ConflictResolution` do not even reference `Microsoft.EntityFrameworkCore.Sqlite`
or `Wisej.Web` — they are UI-free and database-free by construction, which is what makes
`TicketValidatorTests` and the `ConflictResolution` tests in
`SupportDesk.Tests/ConcurrencyAndTransactionsTests.cs` able to exercise them with no page, no session and (for
`TicketValidator`) no database at all.

## The DI bridge

Wisej.NET has its own dependency injection for `[Inject]`, separate from ASP.NET Core's. `Startup.cs` wires
them together in one line, right after `builder.Build()`:

```csharp
var app = builder.Build();
Wisej.Web.Application.Services.AddService<IServiceProvider>(app.Services);
```

From that point on, every `[Inject]` property on a `Page` or `Form` — `TicketQueries`, `Commands`,
`Validator`; `ConflictResolution` is used directly inside `TicketCommandService`, not `[Inject]`ed into the
Web layer — resolves through the **root** Microsoft DI container. Wisej.NET asks the root provider, not a
per-request scope, which is why every application service must be **Transient** or **Singleton**: a
`Scoped` registration throws *Cannot resolve scoped service … from root provider* the moment a page or form
that injects it is constructed. `IDbContextFactory<SupportDeskContext>` is itself a singleton (EF Core's own
registration) and resolves fine; what it *hands out* — each `SupportDeskContext` instance — is what has the
short, one-operation lifetime.

## Where every `DbContext` is created and disposed

Every one of these is `await using var db = await _dbFactory.CreateDbContextAsync(token);` at the top of the
method, disposed when that method returns (success, exception, or early return alike):

| Method | Purpose |
|---|---|
| `TicketQueryService.SearchTicketsAsync` | one context, two statements (COUNT + paged SELECT) |
| `TicketQueryService.CountTicketsAsync`, `GetCustomersAsync` | one context each (`GetStatusesAsync` returns the fixed list and opens none) |
| `TicketCommandService.LoadEditModelAsync` | one context, one no-tracking read |
| `TicketCommandService.GetLookupsAsync` | one context, three no-tracking reads |
| `TicketCommandService.SaveAsync` (add/update/Overwrite) | one context: tracked read (update) or `Add` (new), map, `SaveChangesAsync` — on a concurrency conflict, `ConflictResolution.BuildConflictListAsync` runs **inside this same context**, before it disposes (see `docs/ConcurrencyResolution.md`) |
| `TicketCommandService.DeleteAsync` | one context: tracked read by key, business rule, `Remove` + `SaveChangesAsync` |
| `SchemaInfoService.DescribeAsync`, `DevelopmentSeeder.*`, `ModelDemoService.*` | one context per call, same rule |
| `SupportDeskDevelopmentDatabase.EnsureReadyAsync` (`Startup.cs`, Development only) | one context for `MigrateAsync` + `GetAppliedMigrationsAsync`, then the seeder's own context |

The page and every form never hold a `DbContext` reference at any point — not as a field, not across an
`await`, not between handlers. `TicketBrowserPage`, `TicketEditorForm` and `ConflictDialog` call into
`SupportDesk.Services` and get back plain data (`TicketListItem`, `TicketEditData`, `ConflictSet`,
`SaveTicketResult`, …), never an entity, never a context.

## Evidence

- `Startup.cs` — the bridge line and the service registrations (all `AddTransient`), with EF Core registered
  inside `AddSupportDeskData`.
- Every method named above, read directly — each opens with `await using var db = await
  _dbFactory.CreateDbContextAsync(...)`. `TicketQueryServiceTests.Each_operation_creates_and_disposes_its_own_context`
  and the statement-count tests in `TicketSearchTests` and `TicketCommandServiceTests` count the contexts
  created and disposed through `QueryTrace`.
- `SupportDesk.Tests/ConcurrencyAndTransactionsTests.cs` and the inherited Module 1–5 tests all construct
  services directly against `SqliteTestFactory` with no page, no session, and (for `TicketValidator`) no
  database — proof the service boundary really has no UI or session dependency baked in.
