# DbContext registration · AddDbContextFactory and the design-time factory

Deliverable 2 of the Module 1 lab: `SupportDeskContext` registered through `AddDbContextFactory` in
Microsoft DI, plus a design-time factory for `dotnet ef`.

## Why a factory and not `AddDbContext`

`AddDbContext` gives one context per DI scope — per HTTP request in a classic web app. A Wisej.NET page is
not a request: it lives for the whole session and receives many clicks, each of which may `await`. There is
no natural scope that matches "one click", so the code asks for a context exactly when it does database work:

```csharp
await using var db = await _dbFactory.CreateDbContextAsync(token);
return await db.Tickets.CountAsync(token);
```

The factory is a singleton and thread-safe; each `CreateDbContextAsync` returns a new, independent context
with its own change tracker and connection. That is the recommended default for stateful server-side UI
frameworks (Blazor Server and Wisej.NET alike).

## The registration (`SupportDesk.Data/DependencyInjection/SupportDeskDataServiceCollectionExtensions.cs`)

```csharp
services.AddDbContextFactory<SupportDeskContext>((provider, options) =>
{
    options.UseSqlite(connectionString);
    options.AddInterceptors(new QueryTraceInterceptor(), new OutageInterceptor(...));   // lab instruments
    if (isDevelopment)
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();   // parameter values in logs: local development only
    }
});
```

`Startup.cs` calls `builder.Services.AddSupportDeskData(connectionString, builder.Environment.IsDevelopment())`
— step 2 of the host. The two interceptors are the lab's teaching instruments (the SQL trace shown on the
page and the outage switch); a production registration would drop them and add `EnableRetryOnFailure()`
on a SQL Server provider.

## The design-time factory (`SupportDesk.Data/SupportDeskDesignTimeFactory.cs`)

`dotnet ef` needs to build the context to scaffold a migration. Without a design-time factory it would try
to start the application host — and the Wisej.NET startup is not something to run from a build tool. The
factory gives it the same provider and a safe local file:

```csharp
public SupportDeskContext CreateDbContext(string[] args)
{
    var options = new DbContextOptionsBuilder<SupportDeskContext>()
        .UseSqlite($"Data Source={SupportDeskPaths.DevelopmentDatabaseFile()}")
        .Options;
    return new SupportDeskContext(options);
}
```

`DevelopmentDatabaseFile()` walks up from the current directory to the folder that contains
`SupportDesk.Web` and returns `SupportDesk.Web/App_Data/supportdesk.db` — the same file the application
opens — so the commands work from the solution folder or from any project folder. No password, no
environment variable, nothing to leak.

Commands (from the `Module 1` folder; the migrations themselves start in Module 2):

```bash
dotnet ef dbcontext info      --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0
dotnet ef migrations add X    --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0
dotnet ef database update     --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0
```

## The context itself (`SupportDeskContext.cs`)

Module 1 maps one minimal `Ticket` (Id, Number, Title, Status, CreatedAt) with three Fluent rules so the
count has something to count; the full model arrives in Module 2. The constructor and `Dispose` report to
`QueryTrace` — that is how the page can print `◦ context #n created` / `◦ context #n disposed (0 tracked
entities released)` around each statement. Remove those two lines and nothing else changes.

## Development-only startup

`SupportDeskDevelopmentDatabase.EnsureReadyAsync` (Web project) runs once at host start **in Development
only**: it creates the SQLite file with `EnsureCreated` and inserts twelve sample tickets when the table is
empty. It uses the factory like any other code (`await using var db = …`). It is not session code and it is
not how production databases are created — Module 2 introduces migrations and Module 7 the reviewed
script.

## Evidence

- Every operation in the page trace is bracketed by `created` / `disposed` lines with the same context
  number and `0 alive between clicks` in the lifetimes table.
- `dotnet ef dbcontext info` succeeds from the module folder without starting Kestrel.
- The tests build their own `IDbContextFactory<SupportDeskContext>` (`SqliteTestFactory`) over an in-memory
  connection and pass it to `TicketQueryService` — the service does not care where the factory comes from.
