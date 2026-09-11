# Data Binding with EF Core Course · lab samples

One runnable Wisej.NET 4 solution per module, built from the course's lesson guide, lab guide and walkthrough
video. The project is the **Support Desk Data Console** (Customers, Agents, Categories, Tickets, TicketComments
behind EF Core 10 + SQLite). The modules are cumulative, and `Module 7` is the capstone (built on Module 5).
Each screen shows only what that module's lab and video build. Each folder has its own `README.md` (what to
try, self-check answers) and a `docs/` folder with the lab notes.

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package, EF Core 10.0.12 and the
`dotnet-ef` tool. The database is a local SQLite file under `SupportDesk.Web/App_Data` that the host creates
(and, from Module 2, migrates and seeds) at start in Development.

| Module | Folder | What it builds | Run |
|---|---|---|---|
| 1 · Architecture: EF Core inside a Wisej.NET app | `Module 1` | four-project solution, `AddDbContextFactory`, the Microsoft DI bridge, the first async count behind a loading guard | `http://localhost:5401` |
| 2 · Modeling, DbContext configuration, migrations | `Module 2` | the five entities with Fluent API rules, delete behaviours, RowVersion, indexes, `InitialCreate` migration, development seed | `http://localhost:5402` |
| 3 · Loading data into BindingSource, grid and lookups | `Module 3` | `SearchTicketsAsync` (no-tracking projection, filters, paging in SQL), grid bound through a `BindingSource`, lookup ComboBoxes, Search / Next / Previous | `http://localhost:5403` |
| 4 · Two-way binding and CRUD editors | `Module 4` | `TicketEditModel`, modal `TicketEditorForm` with `DataBindings`, save / cancel / delete, grid refresh on `DialogResult.OK` | `http://localhost:5404` |
| 5 · Validation, ErrorProvider and feedback | `Module 5` | DataAnnotations, `TicketValidator`, `ErrorProvider` + summary label, Save disabled while invalid, `DbUpdateException` → friendly message, negative tests | `http://localhost:5405` |
| 6 · Async loads, related data and performance | `Module 6` | EF Core logging, projection + `AsNoTracking` + paging of 50, related data by filtered read and explicit load, before/after notes | `http://localhost:5406` |
| 7 · Concurrency, deployment, diagnostics, capstone | `Module 7` | RowVersion round trip, two-session conflict → `ConflictDialog` (Reload / Overwrite by role), migration script, deployment notes, capstone review | `http://localhost:5407` |

Run any module from its `SupportDesk.Web` project folder. The web project multi-targets `net10.0-windows` and
`net10.0`, so `dotnet run` needs a framework:

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 3/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5403
```

or open `SupportDesk.slnx` in the module folder with Visual Studio and press F5. Tests:
`dotnet test SupportDesk.Tests` from the module folder (xunit, SQLite in memory).

## `_template`

The Module 1 solution as a scaffold, plus `COOKBOOK.md`: the Wisej.NET + EF Core facts verified while
building these samples (the DI bridge and its lifetime rule, the loading guard, RowVersion on SQLite,
dialogs, `BindingSource`/`DataGridView`/`ErrorProvider` usage, the migration commands).
