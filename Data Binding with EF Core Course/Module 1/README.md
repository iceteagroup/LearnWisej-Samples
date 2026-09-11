# SupportDesk · Data Binding with EF Core · Module 1

Local lab build for **Module 1 · Architecture: EF Core inside a Wisej.NET application**. It follows the
walkthrough video: the Support Desk Data Console solution is created with **Web, Data, Services and Tests**
projects, `SupportDeskContext` is registered through `AddDbContextFactory` (with a design-time factory),
Microsoft's `IServiceProvider` is bridged into Wisej.NET, `TicketQueryService` arrives on the page through
`[Inject]`, and the first **async count** runs behind a loading guard and a friendly error message.
Nothing is bound yet; that starts in Module 3.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 1/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5401
```

Then open <http://localhost:5401>. (Visual Studio: open `SupportDesk.slnx`, press F5; the port and the
Development environment are in `SupportDesk.Web/Properties/launchSettings.json`.)

The web project multi-targets `net10.0-windows;net10.0`, so `dotnet run` needs `-f`. In Development the host
creates `SupportDesk.Web/App_Data/supportdesk.db` on first start (EnsureCreated, twelve sample tickets);
delete the file to start over.

Tests: `dotnet test SupportDesk.Tests`: three tests against SQLite in memory (count on empty, count on
rows, one context created and disposed per operation).

## What to try

The page has the lab's two controls: `countButton` (**Count tickets**) and `statusLabel`.

- **Count tickets**: the button is disabled, `statusLabel` reads *Counting tickets...*, then
  *12 tickets in the Support Desk database*, and the button comes back.
- **Click twice quickly**: the second click returns at the `_loading` guard; only one count runs.
- **Two sessions**: open the page in two browser tabs and count in both; each gets its own result.
- **Database unreachable**: start the app against a database it cannot open, for example
  `set ASPNETCORE_ENVIRONMENT=Production` and `set ConnectionStrings__SupportDesk=Data Source=Z:/missing/supportdesk.db`
  before `dotnet run`. **Count tickets** then shows a friendly `AlertBox`, `statusLabel` reads *Count failed*,
  the exception goes to the console log, and the button is enabled again.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | Solution with Web, Data, Services and Tests projects and the EF Core provider packages | [`docs/SolutionStructure.md`](docs/SolutionStructure.md) · `SupportDesk.slnx`, `SupportDesk.Data/SupportDesk.Data.csproj` (the only project with the provider) |
| 2 | `SupportDeskContext` registered through `AddDbContextFactory` with a design-time factory | [`docs/DbContextRegistration.md`](docs/DbContextRegistration.md) · `SupportDesk.Data/DependencyInjection/SupportDeskDataServiceCollectionExtensions.cs`, `SupportDesk.Data/SupportDeskDesignTimeFactory.cs` |
| 3 | Microsoft `IServiceProvider` exposed to Wisej.NET and a query service resolved from a Page | [`docs/DependencyInjectionBridge.md`](docs/DependencyInjectionBridge.md) · `SupportDesk.Web/Startup.cs`, `[Inject]` in `SupportDesk.Web/TicketBrowserPage.cs` |
| 4 | First async count query with a loading guard and an error message | [`docs/FirstAsyncQuery.md`](docs/FirstAsyncQuery.md) · `TicketBrowserPage.countButton_Click`, `SupportDesk.Services/TicketQueryService.cs` |
| 5 | No static DbContext anywhere in the solution | [`docs/LifetimesAndSessionSafety.md`](docs/LifetimesAndSessionSafety.md) |

## Where things live

```
Module 1/
├─ SupportDesk.slnx                 the four projects
├─ SupportDesk.Web/                 the Wisej.NET application (net10.0-windows;net10.0)
│  ├─ Startup.cs                    host: configuration → AddSupportDeskData → services → Build → IServiceProvider bridge → UseWisej
│  ├─ Program.cs                    Wisej.NET session entry point: Application.MainPage = new TicketBrowserPage()
│  ├─ TicketBrowserPage.cs          [Inject] TicketQueryService, countButton_Click with the loading guard
│  ├─ TicketBrowserPage.Designer.cs countButton, statusLabel
│  ├─ SupportDeskDevelopmentDatabase.cs  Development only: EnsureCreated + twelve sample tickets at host start
│  └─ appsettings.json              ConnectionStrings:SupportDesk = Data Source=App_Data/supportdesk.db
├─ SupportDesk.Data/                EF Core lives here (the only project referencing the provider)
│  ├─ SupportDeskContext.cs         the unit of work (Module 1: one minimal Ticket entity)
│  ├─ Entities/Ticket.cs
│  ├─ SupportDeskDesignTimeFactory.cs   IDesignTimeDbContextFactory for dotnet ef
│  ├─ SupportDeskPaths.cs           resolves the relative SQLite path for the app, the tests and dotnet ef
│  ├─ DependencyInjection/…Extensions.cs  AddSupportDeskData: AddDbContextFactory + UseSqlite + dev diagnostics
│  └─ Diagnostics/                  QueryTrace + QueryTraceInterceptor, used by the tests to count contexts and statements
├─ SupportDesk.Services/
│  └─ TicketQueryService.cs         CountTicketsAsync (one context per call)
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
| 7 · Loading flag and error message around the query | `countButton_Click`: `_loading`, `countButton.Enabled`, `catch` → `AlertBox`, `finally` |

## Self-check answers (lab guide)

- **Which lifetime did you choose for the DbContext behind `CountTicketsAsync`, and what would go wrong if the page kept that context alive between clicks?**
  One context per operation: `await using var db = await _dbFactory.CreateDbContextAsync()` inside the
  method, disposed before it returns. A context kept in the page would live as long as the session (hours),
  track every entity it ever loaded, hold a stale view of the database, and, because a Wisej.NET page can
  receive a second click while an awaited operation is still running, be used by two operations at once,
  which EF Core refuses.
- **Two users open the Support Desk console at the same time and both click Count. Which objects are separate per session, which are shared, and why is that split safe?**
  Separate per session: the `TicketBrowserPage`, its controls, the `_loading` flag, and every
  `SupportDeskContext` (created inside the click). Shared: the `IDbContextFactory` and its options, the
  transient `TicketQueryService` instances (stateless, they hold only the factory) and the SQLite file. The
  split is safe because everything shared is immutable or thread-safe and everything mutable (UI state,
  change tracker) belongs to exactly one session or one operation.
- **If the database is unreachable, what does the user see, what does the button do, and how does the loading flag end up false again?**
  The connection open throws (`SqliteException` here, `SqlException` on SQL Server). `catch` shows one
  friendly sentence in an `AlertBox`, sets `statusLabel` to *Count failed* and writes the exception to the
  server log; the button was disabled at the start and is re-enabled in `finally`, which also sets
  `_loading = false`, so the next click gets a fresh context and simply works once the database is back.

## Verified facts

- `Application.Services.AddService<IServiceProvider>(app.Services)` makes `[Inject]` on a Page resolve
  through Microsoft DI. Wisej.NET calls the **root** provider, so application services must be
  **Transient** (or Singleton): a `Scoped` registration fails at page construction in Development with
  *Cannot resolve scoped service … from root provider*.
- After an `await` the handler continues off the original request; changing controls there is fine, and
  `Application.Update(this)` in `finally` pushes the final state to the browser.
