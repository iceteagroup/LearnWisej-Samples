# SupportDesk · Data Binding with EF Core · Module 2

Local lab build for **Module 2 · Modeling, DbContext Configuration, and Migrations**. The Support Desk model
arrives: `Customer`, `Agent`, `Category`, `Ticket` and `TicketComment`, configured with the Fluent API
(required strings and lengths, a `RowVersion` concurrency token, the planned indexes, one `OnDelete` rule per
relationship and a CHECK constraint on the title), scaffolded into the `InitialCreate` migration through the
design-time factory, applied with `MigrateAsync` at start in Development, and filled by a development-only
seeder.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 2/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5402
```

In Development the host migrates and seeds `SupportDesk.Web/App_Data/supportdesk.db` before the first
session (five customers, three agents, six categories, sixty tickets, sixteen comments). Delete the file
(and its `-wal` / `-shm` companions) to start over. Tests: `dotnet test SupportDesk.Tests`.

Migration commands, from the `Module 2` folder (`--framework` is needed because the web project multi-targets):

```bash
dotnet ef migrations list   --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0
dotnet ef migrations script --idempotent --project SupportDesk.Data --startup-project SupportDesk.Web \
    --framework net10.0 --output artifacts/sql/supportdesk_migrations.sql
```

## What to try

The page has `countButton`, `btnSeed`, `statusLabel`, a counts line (customers, agents, categories,
tickets, pending migrations) and the two failure paths lab step 8 asks for.

- **Count tickets**: *60 tickets in the Support Desk database*.
- **Seed development data** (`btnSeed`): the database is seeded at start, so it reports *nothing to seed:
  60 tickets already exist*. On an empty database it seeds and the counts line updates.
- **Save a 200-character title**: the CHECK constraint refuses the insert; a friendly `AlertBox` says the
  database rejected the change, the counts do not move, and the exception goes to the console log.
- **Delete a customer with tickets**: `DeleteBehavior.Restrict` refuses it; *This customer still has
  tickets and cannot be deleted.*

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | The five entities with relationships and delete behaviours | [`docs/ModelAndDeleteBehaviours.md`](docs/ModelAndDeleteBehaviours.md) · `SupportDesk.Data/Entities/*.cs`, `SupportDeskContext.OnModelCreating` |
| 2 | `RowVersion` configured with `IsRowVersion` | [`docs/RowVersionAndIndexes.md`](docs/RowVersionAndIndexes.md) · `SupportDeskContext` (`IsRowVersion()` and `StampTickets` in the `SaveChanges` overrides) |
| 3 | Indexes for `Status`, `DueDate`, `CustomerId` and `UpdatedAt` | [`docs/RowVersionAndIndexes.md`](docs/RowVersionAndIndexes.md) · the `HasIndex` calls, the `CreateIndex` operations in the migration |
| 4 | Design-time factory and the `InitialCreate` migration | [`docs/MigrationWorkflow.md`](docs/MigrationWorkflow.md) · `SupportDeskDesignTimeFactory.cs`, `Migrations/`, `artifacts/sql/supportdesk_migrations.sql` |
| 5 | Seed data: five customers, three agents, categories and at least fifty tickets | [`docs/SeedData.md`](docs/SeedData.md) · `DevelopmentSeeder.cs`, `SupportDeskDevelopmentDatabase.cs`, `btnSeed_Click` |

Schema constraints versus UI validation: [`docs/SchemaVsUiValidation.md`](docs/SchemaVsUiValidation.md).

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the Module 1 solution and confirm the count still runs | `countButton_Click` in `TicketBrowserPage.cs` |
| 2 · The five entity classes, no mapping attributes | `SupportDesk.Data/Entities/` |
| 3 · Relationships and `OnDelete` per relationship; `HasMaxLength`/`IsRequired` | `SupportDeskContext.OnModelCreating` |
| 4 · `RowVersion` with `IsRowVersion()` and the planned indexes | `SupportDeskContext.OnModelCreating` + `StampTickets` (SQLite has no `rowversion` type) |
| 5 · Design-time factory with a password-free local connection | `SupportDeskDesignTimeFactory.cs`, `SupportDeskPaths.DevelopmentDatabaseFile()` |
| 6 · `migrations add InitialCreate`, read `Up`/`Down`, `database update` | `SupportDesk.Data/Migrations/`; commands in [`docs/MigrationWorkflow.md`](docs/MigrationWorkflow.md) |
| 7 · `SeedDevelopmentDataAsync` called from `btnSeed_Click` behind the guard | `DevelopmentSeeder.cs`, `btnSeed_Click` → `RunAsync` |
| 8 · Show every path: seed, second seed, failed save, refused customer delete | `btnSeed`, `buttonOverlongTitle`, `buttonDeleteCustomer` (`ModelDemoService`) |
| 9 · Script produced, count shows at least fifty, note on delete and index choices | `artifacts/sql/supportdesk_migrations.sql`, the docs above |

## Self-check answers

- **You delete a customer who still has open tickets. What happens, and what would `Cascade` have done?**
  `Ticket.CustomerId` is `OnDelete(DeleteBehavior.Restrict)`, which becomes `ON DELETE RESTRICT`. SQLite
  refuses the `DELETE` (*FOREIGN KEY constraint failed*), EF Core wraps it in `DbUpdateException`, and the
  user reads *This customer still has tickets and cannot be deleted.* Nothing changes. With `Cascade` the
  delete would have taken all of that customer's tickets and their comments with it, silently. Only
  `TicketComment` is `Cascade`; the agent relationship is `SetNull`, because people leave and their tickets
  must not.
- **Two agents load SD-1042 and both click Save a second apart. Which save throws, and what does the `UPDATE` contain?**
  The second. `RowVersion` is a concurrency token, so the statement is
  `UPDATE "Tickets" SET … WHERE "Id" = @p2 AND "RowVersion" = @p3`. The first save matches and stamps a
  new token (in `SupportDeskContext.SaveChanges`, since SQLite does not generate one); the second carries the
  old token, matches zero rows and raises `DbUpdateConcurrencyException`. Module 7 turns that into a
  conflict dialog.
- **Why does `IsRequired()` on `Title` not give the user an "Enter a title" message?**
  `IsRequired()` and `HasMaxLength(180)` are mapping rules enforced by the database after a round trip, as a
  `DbUpdateException` no user should read. On SQLite `HasMaxLength` is not even enforced, hence
  `CK_Tickets_Title_Length`. The user-facing message comes in Module 5 from `TicketValidator` and an
  `ErrorProvider`. Both layers stay.
