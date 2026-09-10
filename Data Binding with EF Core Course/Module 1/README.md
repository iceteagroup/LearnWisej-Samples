# SupportDesk · Data Binding with EF Core · Module 1

Local lab build for **Module 1 · Architecture: EF Core inside a Wisej.NET application**. It follows the
walkthrough video: the Support Desk Data Console solution is created with **Web, Data, Services and Tests**
projects, `SupportDeskContext` is registered through `AddDbContextFactory` (with a design-time factory),
Microsoft's `IServiceProvider` is bridged into Wisej.NET, `TicketQueryService` arrives on the page through
`[Inject]`, and the first **async count** runs behind a loading guard and a friendly error message.
Nothing is bound yet — that starts in Module 3.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 solution on this machine with a local SQLite file.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 1/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5401
```

Then open <http://localhost:5401>. (Visual Studio: open `SupportDesk.slnx`, press F5 — the port and the
Development environment are in `SupportDesk.Web/Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package, EF Core 10.0.12
(`Microsoft.EntityFrameworkCore.Sqlite` + `Design`, restored from nuget.org). The web project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`. In Development the host creates
`SupportDesk.Web/App_Data/supportdesk.db` on first start (EnsureCreated, twelve sample tickets); delete the
file to start over.

Tests: `dotnet test SupportDesk.Tests` — three tests against SQLite in memory (count on empty, count on
rows, one context created and disposed per operation).

## What to click

| Action | Path | What you should see |
|---|---|---|
| **Count tickets** (the lab's `countButton`) | success | `statusLabel` → *12 tickets in the Support Desk database*; the trace shows `• countButton_Click TicketQueryService.CountTicketsAsync()` → `◦ context #n created` → `→ SQL SELECT COUNT(*) FROM "Tickets"` (ms) → `◦ context #n disposed (0 tracked)` → `← result 12 tickets · 1 statement · 1 context created, 1 disposed`; the lifetimes table counts one more context created **and** disposed |
| **Slow count (2.5 s)**, then click **Count tickets** while it runs | progress · guard | the buttons are disabled and `statusLabel` reads *Counting tickets… (simulated 2.5 s latency)*; a click during the wait is logged as `• guard … a count is already running — this click is ignored`; `finally` restores the buttons |
| **▶ Count ×3 rapid (guard)** | progress · guard | three counts started at once: `rapid click 1` runs the slow count, `rapid click 2` and `3` are dropped by `_loading` — the trace shows the two guard lines between `context created` and the SQL |
| **Break the database** | failure | `DevelopmentOutageSwitch.IsDown = true`, the count runs, the connection open throws: red banner *The Support Desk database is not reachable right now. Nothing was changed…*, status `● fault`, trace `• caught DatabaseUnavailableException … → friendly message shown, full exception logged server-side`; the context is still disposed |
| **Restore and count** | recovery | the switch goes off; the next click gets a **fresh** context and a working connection; `● ok` again |
| **Two ops, one context (anti-pattern)** | anti-pattern | one context, two threads counting on it at once (what a static/shared DbContext does with two sessions): `• caught InvalidOperationException: A second operation was started on this context instance before a previous operation completed…` and a red banner. Nothing in the real code path can do this — the demo lives in `SharedContextAntiPattern` |
| **Clear trace** | – | empties the right-hand list |

The right-hand card is the **Server ⇄ Database · EF Core lifetime & SQL trace**: every context created and
disposed, every SQL statement with its duration, and every handler decision — so the "one context per
operation" rule can be watched instead of read. The left card's **Four lifetimes** table keeps the running
totals: the session id, the page instance, the handler runs, and contexts created versus disposed (always
zero alive between clicks).

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | Solution with Web, Data, Services and Tests projects and the EF Core provider packages | [`docs/SolutionStructure.md`](docs/SolutionStructure.md) · `SupportDesk.slnx`, `SupportDesk.Data/SupportDesk.Data.csproj` (the only project with the provider) |
| 2 | `SupportDeskContext` registered through `AddDbContextFactory` with a design-time factory | [`docs/DbContextRegistration.md`](docs/DbContextRegistration.md) · `SupportDesk.Data/DependencyInjection/SupportDeskDataServiceCollectionExtensions.cs`, `SupportDesk.Data/SupportDeskDesignTimeFactory.cs` |
| 3 | Microsoft `IServiceProvider` exposed to Wisej.NET and a query service resolved from a Page | [`docs/DependencyInjectionBridge.md`](docs/DependencyInjectionBridge.md) · `SupportDesk.Web/Startup.cs`, `[Inject]` in `SupportDesk.Web/TicketBrowserPage.cs` |
| 4 | First async count query with a loading guard and an error message | [`docs/FirstAsyncQuery.md`](docs/FirstAsyncQuery.md) · `TicketBrowserPage.CountAsync`, `SupportDesk.Services/TicketQueryService.cs` |
| 5 | No static DbContext anywhere in the solution | [`docs/LifetimesAndSessionSafety.md`](docs/LifetimesAndSessionSafety.md) · the four-lifetimes table on the page and the anti-pattern demo |

## Where things live

```
Module 1/
├─ SupportDesk.slnx                 the four projects
├─ SupportDesk.Web/                 the Wisej.NET application (net10.0-windows;net10.0)
│  ├─ Startup.cs                    Kestrel host: configuration → AddSupportDeskData → services → Build → IServiceProvider bridge → UseWisej
│  ├─ Program.cs                    Wisej.NET session entry point: Application.MainPage = new TicketBrowserPage()
│  ├─ TicketBrowserPage.cs          the lab page: [Inject] TicketQueryService, countButton_Click, the guard, the trace
│  ├─ TicketBrowserPage.Designer.cs Designer-generated layout (cards, buttons, trace list)
│  ├─ SupportDeskDevelopmentDatabase.cs  Development only: EnsureCreated + twelve sample tickets at host start
│  ├─ appsettings.json              ConnectionStrings:SupportDesk = Data Source=App_Data/supportdesk.db
│  └─ Default.json / Default.html / Web.config / Properties/launchSettings.json
├─ SupportDesk.Data/                EF Core lives here (the only project referencing the provider)
│  ├─ SupportDeskContext.cs         the unit of work (Module 1: one minimal Ticket entity)
│  ├─ Entities/Ticket.cs
│  ├─ SupportDeskDesignTimeFactory.cs   IDesignTimeDbContextFactory for dotnet ef
│  ├─ SupportDeskPaths.cs           resolves the relative SQLite path for the app, the tests and dotnet ef
│  ├─ DependencyInjection/…Extensions.cs  AddSupportDeskData: AddDbContextFactory + UseSqlite + dev diagnostics
│  └─ Diagnostics/                  lab instruments: QueryTrace (AsyncLocal sink), QueryTraceInterceptor, DevelopmentOutageSwitch
├─ SupportDesk.Services/
│  ├─ TicketQueryService.cs         CountTicketsAsync (one context per call) + CountTicketsSlowlyAsync (lab prop)
│  └─ SharedContextAntiPattern.cs   deliberately wrong: two threads on one context
├─ SupportDesk.Tests/               xunit, SQLite in memory (SqliteTestFactory)
└─ docs/                            the five deliverables
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Solution folders Web, Data, Services, Tests | `SupportDesk.slnx` and the four project folders |
| 2 · EF Core provider packages in the data project | `SupportDesk.Data.csproj` (`Microsoft.EntityFrameworkCore.Sqlite`, `…Design`) |
| 3 · `SupportDeskContext` and a design-time factory | `SupportDesk.Data/SupportDeskContext.cs`, `SupportDeskDesignTimeFactory.cs` |
| 4 · `AddDbContextFactory` in ASP.NET Core startup | `Startup.cs` step 2 → `AddSupportDeskData(...)` |
| 5 · Expose Microsoft `IServiceProvider` to Wisej.NET | `Startup.cs` step 4: `Application.Services.AddService<IServiceProvider>(app.Services)` |
| 6 · Resolve a query service from a Page, run a count | `[Inject] TicketQueryService TicketQueries` + `countButton_Click` in `TicketBrowserPage.cs` |
| 7 · Loading flag and error message around the query | `TicketBrowserPage.CountAsync`: `_loading`, `SetBusy`, `catch` → banner, `finally` |

Lab code check (`labs.js` m1): the count goes through the service's async method, the handler returns
early while `_loading` is true and disables `countButton` until `finally` restores it, and failures show a
friendly message instead of a raw exception.

## Self-check answers (lab guide)

- **Which lifetime did you choose for the DbContext behind `CountTicketsAsync`, and what would go wrong if the page kept that context alive between clicks?**
  One context per operation: `await using var db = await _dbFactory.CreateDbContextAsync()` inside the
  method, disposed before it returns — the trace shows `created` and `disposed` around every `SELECT COUNT`.
  A context kept in the page would live as long as the session (hours), track every entity it ever loaded,
  hold a stale view of the database, and — because a Wisej.NET page can receive a second click while an
  awaited operation is still running — be used by two operations at once, which EF Core refuses (the
  anti-pattern button shows the exception).
- **Two users open the Support Desk console at the same time and both click Count. Which objects are separate per session, which are shared, and why is that split safe?**
  Separate per session: the `TicketBrowserPage`, its labels and trace list, the `_loading` flag, and every
  `SupportDeskContext` (created inside the click). Shared: the `IDbContextFactory` and its options, the
  transient `TicketQueryService` instances (stateless, they hold only the factory), the outage switch (a
  lab prop) and the SQLite file. The split is safe because everything shared is immutable or thread-safe
  and everything mutable (UI state, change tracker) belongs to exactly one session or one operation.
- **If the database is unreachable, what does the user see, what does the button do, and how does the loading flag end up false again?**
  The connection open throws (`DatabaseUnavailableException` from the lab switch; `SqliteException` /
  `SqlException` in real life). `catch` sets `statusLabel` to *Count unavailable*, shows the red banner
  with one friendly sentence and logs the exception with its type on the server; the button was disabled
  by `SetBusy(true)` at the start and is re-enabled in `finally`, which also sets `_loading = false` —
  so the next click gets a fresh context and simply works once the database is back (**Restore and count**).

## Verified / unverified

Built, tested (`dotnet test`) and run in the browser on this machine (Wisej-4 4.1.0, .NET 10, EF Core 10.0.12,
SQLite): every path in the table above. Two facts learned here and used by every later module:

- `Application.Services.AddService<IServiceProvider>(app.Services)` makes `[Inject]` on a Page resolve
  through Microsoft DI — Wisej.NET calls the **root** provider, so application services must be
  **Transient** (or Singleton): a `Scoped` registration fails at page construction in Development with
  *Cannot resolve scoped service … from root provider*.
- After an `await` the handler continues off the original request; changing controls there is fine, and
  `Application.Update(this)` in `finally` pushes the final state to the browser.

## Browser results (reviewer, 2026-09-10)

Run on this machine at <http://localhost:5401> in the Browser pane; trace read back from the page:

- Page load: `• session page created for session … · TicketQueryService through [Inject] → resolved from Microsoft DI`.
  (The first run, with `AddScoped`, died at page construction with *Cannot resolve scoped service 'SupportDesk.Services.TicketQueryService' from root provider* — the Transient rule above comes from that.)
- **Count tickets** → `◦ context #2 created`, `→ SQL SELECT COUNT(*) FROM "Tickets" AS "t" (0.1 ms)`, `◦ context #2 disposed (0 tracked entities released)`,
  `← result 12 tickets · 1 statement(s) · 0.1 ms in the database · 1 context created, 1 disposed`; the lifetimes table shows `1 created · 1 disposed · 0 alive between clicks`.
- **▶ Count ×3 rapid (guard)** → `rapid click 1` runs the 2.5 s count; `• guard rapid click 2: a count is already running — this click is ignored` and the same for click 3, both between `◦ context #6 created` and the SQL.
- **Break the database** → `◦ context #4 created`, `◦ context #4 disposed`, `• caught DatabaseUnavailableException: Simulated outage … → friendly message shown, full exception logged server-side`, red banner, `● fault`.
- **Restore and count** → a normal count on a fresh context, `● ok`.
- **Two ops, one context (anti-pattern)** → `• caught InvalidOperationException: An attempt was made to use the context instance while it is being configured … (round 1)` and the red banner
  (the SQLite COUNT is so fast that the collision needs the two-thread rounds `SharedContextAntiPattern` runs; it hit on the first round here).
- The lifetimes table renders as an HTML table (`AllowHtml`), the ampersand in the old "Restore & count" caption was swallowed as a mnemonic and the button is now "Restore and count".
