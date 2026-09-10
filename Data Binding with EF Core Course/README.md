# Data Binding with EF Core Course · lab samples

One runnable Wisej.NET 4 solution per module, built from the course's lesson guide, lab guide and walkthrough
video. The project is the **Support Desk Data Console** (Customers, Agents, Categories, Tickets, TicketComments
behind EF Core 10 + SQLite). The modules are cumulative: each module folder is the previous one plus that module's
lab, so `Module 7` is the finished capstone. Every sample keeps the same layout: the lab's controls on the left, a
**Server ⇄ Database · EF Core lifetime & SQL trace** on the right (every context created and disposed, every SQL
statement with its duration), and a button bar that exercises the success path, a progress path, at least one
failure path and the recovery. Each folder has its own `README.md` (what to click, self-check answers) and a
`docs/` folder with the lab deliverables.

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package, EF Core 10.0.12 and the
`dotnet-ef` tool. Nothing is deployed anywhere; the database is a local SQLite file under `SupportDesk.Web/App_Data`
that the host creates (and, from Module 2, migrates and seeds) at start in Development.

| Module | Folder | What it builds | Run |
|---|---|---|---|
| 1 · Architecture: EF Core inside a Wisej.NET app | `Module 1` | four-project solution, `AddDbContextFactory`, the Microsoft DI bridge, first async count behind a loading guard, the four-lifetimes table and the shared-context anti-pattern | `http://localhost:5401` |
| 2 · Modeling, DbContext configuration, migrations | `Module 2` | the five entities with Fluent API rules, delete behaviours, RowVersion, indexes, `InitialCreate` migration, development seed (5 customers, 3 agents, categories, 50+ tickets) | `http://localhost:5402` |
| 3 · Loading data into BindingSource, grid and lookups | `Module 3` | `TicketListItem`/`TicketSearchCriteria`/`PagedResult`, `SearchTicketsAsync` (no-tracking projection, filters, paging in SQL), grid bound through a `BindingSource`, lookup ComboBoxes, Search / Next / Previous | `http://localhost:5403` |
| 4 · Two-way binding and CRUD editors | `Module 4` | `TicketEditModel`, modal `TicketEditorForm` with `DataBindings`, `EndEdit`, save / cancel / delete flows through a fresh context, grid refresh on `DialogResult.OK` | `http://localhost:5404` |
| 5 · Validation, ErrorProvider and feedback | `Module 5` | DataAnnotations, `TicketValidator`, `ErrorProvider` + summary label, Save disabled while invalid, `DbUpdateException` → friendly message, five negative tests | `http://localhost:5405` |
| 6 · Async loads, related data and performance | `Module 6` | naive vs optimised search measured in the trace, projection + `AsNoTracking` + paging of 50, related data by `Include` / filtered `Include` / explicit load, before/after notes | `http://localhost:5406` |
| 7 · Concurrency, deployment, diagnostics, capstone | `Module 7` | RowVersion round trip, two-session conflict → `ConflictDialog` (Reload / Overwrite by role), idempotent migration script, deployment notes, capstone review | `http://localhost:5407` |

Run any module from its `SupportDesk.Web` project folder. The web project multi-targets `net10.0-windows` and
`net10.0`, so `dotnet run` needs a framework:

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 3/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5403
```

or open the `SupportDesk.slnx` in the module folder with Visual Studio and press F5. Tests:
`dotnet test SupportDesk.Tests` from the module folder (xunit, SQLite in memory).

## Verification status

Every module was built (`dotnet build`, 0 warnings), tested (`dotnet test`: 3 / 18 / 40 / 52 / 68 / 83 / 82 passing for Modules 1–7)
and then driven in the Browser pane on this machine on 2026-09-10; the **Browser results** section at the end of each module
README lists what was clicked, the trace lines it produced, and the defects found and fixed on the way (the DI lifetime rule,
the ticked date pickers, the four editor-binding facts, the modal-blocks-the-page rule). Those facts are collected in
`_template/COOKBOOK.md` so the next sample does not rediscover them.

## `_template`

The Module 1 solution as a scaffold, plus `COOKBOOK.md`: the Wisej.NET + EF Core conventions verified while
building and running these samples (the DI bridge and its lifetime rule, the loading guard, the AsyncLocal SQL
trace, RowVersion on SQLite, dialogs, `BindingSource`/`DataGridView`/`ErrorProvider` usage, the migration
commands and the gotchas found along the way). Read it before writing a new sample.
