# Solution structure · Web, Data, Services, Tests

Deliverable 1 of the Module 1 lab: the Support Desk Data Console solution with four projects and the
EF Core provider packages in the data project only.

## The four projects

| Project | Kind | References | Holds |
|---|---|---|---|
| `SupportDesk.Web` | ASP.NET Core + Wisej.NET 4 (`net10.0-windows;net10.0`) | Data, Services, `Wisej-4`, `Microsoft.EntityFrameworkCore.Design` (tooling only, `PrivateAssets=all`) | `Startup.cs` (host, DI, bridge), `Program.cs` (session entry), the Pages |
| `SupportDesk.Data` | class library (`net10.0`) | `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design` | `SupportDeskContext`, entities, the design-time factory, migrations (from Module 2), the lab diagnostics |
| `SupportDesk.Services` | class library (`net10.0`) | Data | `TicketQueryService` (and, later, the command service, edit models and validators) |
| `SupportDesk.Tests` | xunit (`net10.0`) | Services | tests against SQLite in memory, no UI |

The dependency direction is one-way: Web → Services → Data. The Web project never references the provider
package itself; it gets EF Core transitively and calls one method, `AddSupportDeskData(connectionString,
isDevelopment)`, that lives in the Data project. A search for `Microsoft.EntityFrameworkCore.Sqlite` in the
solution finds exactly one `PackageReference`.

Why the `Design` package appears in Web too: `dotnet ef … --startup-project SupportDesk.Web` refuses to run
unless the startup project references it. It is marked `PrivateAssets=all`, so nothing flows to consumers and
nothing ships.

## Multi-targeting

`SupportDesk.Web.csproj` keeps the course-wide setting:

```xml
<TargetFrameworks>net10.0-windows;net10.0</TargetFrameworks>
```

The class libraries target `net10.0` only; a `net10.0-windows` application can reference them. Run with
`dotnet run -f net10.0 --urls http://localhost:5401` (or F5, which uses the launch profile).

## Configuration

`appsettings.json` carries the only connection string:

```json
"ConnectionStrings": { "SupportDesk": "Data Source=App_Data/supportdesk.db" }
```

`Startup.cs` reads it with `GetConnectionString("SupportDesk")` and throws if it is missing; the relative
path is resolved against the content root by `SupportDeskPaths.ResolveSqliteDataSource`, so the app, the
tests and `dotnet ef` all open the same file. A SQLite file needs no password; a SQL Server string would come
from the environment or a secret store, never from source (Module 7).

## Evidence

- `dotnet build SupportDesk.slnx` builds all four projects with no warnings.
- `dotnet test SupportDesk.Tests` runs three tests without starting the web host.
- `dotnet ef dbcontext info --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0`
  reports `SupportDeskContext` with the SQLite provider — the design-time factory builds it without the
  Wisej.NET host (see [DbContextRegistration.md](DbContextRegistration.md)).
