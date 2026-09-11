# The migration workflow

Deliverable 4 of the Module 2 lab: the design-time factory, the `InitialCreate` migration, the model
snapshot, and the reviewed script that a production release would run.

## The design-time factory

`dotnet ef` has to construct a `SupportDeskContext` to scaffold or apply a migration. Without a
design-time factory it falls back to starting the application host — and the host of this project calls
`UseWisej()`, sets up the UI framework and would run the development database bootstrap. None of that
belongs in a build tool.

`SupportDesk.Data/SupportDeskDesignTimeFactory.cs`:

```csharp
public sealed class SupportDeskDesignTimeFactory : IDesignTimeDbContextFactory<SupportDeskContext>
{
    public SupportDeskContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SupportDeskContext>()
            .UseSqlite($"Data Source={SupportDeskPaths.DevelopmentDatabaseFile()}")
            .Options;

        return new SupportDeskContext(options);
    }
}
```

Same provider as the application, and a connection string with **no password and no server** —
`SupportDeskPaths.DevelopmentDatabaseFile()` walks up from the current directory to the folder that
contains `SupportDesk.Web` and returns `SupportDesk.Web/App_Data/supportdesk.db`, so the commands work
from the module folder or from any project folder and always hit the same file the application opens.
A SQL Server version of the same factory would use `(localdb)\MSSQLLocalDB` with a trusted connection —
the point is that nothing secret is ever committed with the factory.

## The commands

Run from the **module folder** (`Module 2/`). `--framework net10.0` is required because
`SupportDesk.Web` multi-targets `net10.0-windows;net10.0` and the tools refuse an ambiguous target.

```bash
# scaffold
dotnet ef migrations add InitialCreate \
    --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0

# review the generated Up/Down and the snapshot, then apply to the local file
dotnet ef database update \
    --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0

# what exists and what is applied
dotnet ef migrations list \
    --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0

# the reviewable script for a release
dotnet ef migrations script --idempotent \
    --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0 \
    --output artifacts/sql/supportdesk_migrations.sql
```

`dotnet ef migrations list` prints the migration id and marks anything the target database has not
applied with `(Pending)`:

```
20260910150534_InitialCreate
```

(no marker — this database is up to date; on a fresh checkout, before the first run, it reads
`20260910150534_InitialCreate (Pending)`).

## Reading the generated migration

`SupportDesk.Data/Migrations/20260910150534_InitialCreate.cs` is the part that matters — read it before
applying it, every time. `Up` creates the five tables in dependency order (`Agents`, `Categories`,
`Customers`, then `Tickets`, then `TicketComments`), then the eight indexes.

The columns carry the Fluent rules:

```csharp
Number      = table.Column<string>(type: "TEXT", maxLength: 20,   nullable: false),
Title       = table.Column<string>(type: "TEXT", maxLength: 180,  nullable: false),
Description = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
Status      = table.Column<string>(type: "TEXT", maxLength: 30,   nullable: false),
AgentId     = table.Column<int>(type: "INTEGER", nullable: true),
RowVersion  = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: false)
```

`rowVersion: true` is the concurrency token showing up in the schema — one of the four things the lesson
tells you to check for. `nullable: true` on `AgentId` and `false` on `CustomerId`/`CategoryId` is the
required-versus-optional decision, visible as a column property.

The constraints are on the table:

```csharp
constraints: table =>
{
    table.PrimaryKey("PK_Tickets", x => x.Id);
    table.CheckConstraint("CK_Tickets_Title_Length", "length(\"Title\") <= 180");
    table.ForeignKey(name: "FK_Tickets_Agents_AgentId",        column: x => x.AgentId,
        principalTable: "Agents",     principalColumn: "Id", onDelete: ReferentialAction.SetNull);
    table.ForeignKey(name: "FK_Tickets_Categories_CategoryId", column: x => x.CategoryId,
        principalTable: "Categories", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
    table.ForeignKey(name: "FK_Tickets_Customers_CustomerId",  column: x => x.CustomerId,
        principalTable: "Customers",  principalColumn: "Id", onDelete: ReferentialAction.Restrict);
});
```

and the cascade on the detail table:

```csharp
table.ForeignKey(name: "FK_TicketComments_Tickets_TicketId", column: x => x.TicketId,
    principalTable: "Tickets", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
```

The indexes, including the unique one:

```csharp
migrationBuilder.CreateIndex(name: "IX_Tickets_Number",         table: "Tickets", column: "Number", unique: true);
migrationBuilder.CreateIndex(name: "IX_Tickets_Status_DueDate", table: "Tickets", columns: new[] { "Status", "DueDate" });
migrationBuilder.CreateIndex(name: "IX_Tickets_UpdatedAt",      table: "Tickets", column: "UpdatedAt");
migrationBuilder.CreateIndex(name: "IX_Tickets_CustomerId",     table: "Tickets", column: "CustomerId");
migrationBuilder.CreateIndex(name: "IX_Customers_Name",         table: "Customers", column: "Name");
```

`Down` drops the tables in reverse dependency order — children first:

```csharp
migrationBuilder.DropTable(name: "TicketComments");
migrationBuilder.DropTable(name: "Tickets");
migrationBuilder.DropTable(name: "Agents");
migrationBuilder.DropTable(name: "Categories");
migrationBuilder.DropTable(name: "Customers");
```

For an initial create the `Down` is obvious. It stops being obvious the first time a migration renames a
column: EF Core often scaffolds that as a drop plus an add, which *loses the data*, and only reading the
file catches it. That is the whole reason the lesson insists a migration is a commit to review, not build
output.

### The snapshot

`Migrations/SupportDeskContextModelSnapshot.cs` is the model as EF Core last saw it. The next
`migrations add` diffs the current model against this file to decide what changed, so it is committed
**together with** the migration; a migration without its snapshot makes the next one wrong.
`Migrations/20260910150534_InitialCreate.Designer.cs` carries the same model attached to this specific
migration.

## The reviewed script (`artifacts/sql/supportdesk_migrations.sql`)

`--idempotent` produces a script that can be run against a database at any migration level. It starts by
making sure the history table exists and finishes by recording the migration:

```sql
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "Agents" ( … );
…
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260910150534_InitialCreate', '10.0.12');

COMMIT;
```

`__EFMigrationsHistory` is how any database knows which migrations it has: one row per applied migration.
`MigrateAsync`, `dotnet ef database update` and this script all write the same row, which is why the
three are interchangeable ways of reaching the same schema.

One provider caveat worth knowing: on SQL Server the idempotent script wraps each migration in an
`IF NOT EXISTS(SELECT … FROM [__EFMigrationsHistory] …)` block, so re-running it is genuinely a no-op.
SQLite has no procedural `IF`, so the generated script guards only the history table and then runs the
migration body unconditionally — on SQLite the guard you actually rely on is `MigrateAsync` /
`database update`, and the script is the *reviewable artefact* rather than a re-runnable one. Say this
out loud in a review rather than discovering it on a second deployment.

### Bundles

`dotnet ef migrations bundle` produces a self-contained executable (`efbundle`) that applies pending
migrations without the SDK, the source tree or the `dotnet ef` tool being present. It is the option to
reach for when the pipeline runs on a machine that only has the runtime, or when the release wants one
artefact it can re-run and log. Script and bundle answer the same question — *who applies the schema
change, and when* — with different amounts of ceremony:

| Option | Use it when |
|---|---|
| `MigrateAsync()` at start | local development, single instance, throwaway data |
| `dotnet ef database update` | a developer machine or a CI integration database |
| `--idempotent` script | a DBA reviews and applies the change; rollback planning is required |
| migration bundle | a release pipeline applies it before switching traffic, with no SDK on the box |

## What is committed

| Committed | Ignored |
|---|---|
| `SupportDesk.Data/Migrations/20260910150534_InitialCreate.cs` | `SupportDesk.Web/App_Data/` (the SQLite file) |
| `SupportDesk.Data/Migrations/20260910150534_InitialCreate.Designer.cs` | `bin/`, `obj/` |
| `SupportDesk.Data/Migrations/SupportDeskContextModelSnapshot.cs` | everything else under `artifacts/` |
| `artifacts/sql/supportdesk_migrations.sql` | |

The `.gitignore` has an explicit exception so the reviewed script survives the blanket `artifacts/` rule:

```gitignore
# Build artifacts are ignored, except the reviewed migration script (a Module 2 deliverable).
!artifacts/
artifacts/*
!artifacts/sql/
```

## Development start versus production release

In Development the host brings the local file up to date before the first session
(`SupportDesk.Web/Startup.cs`, step 5 → `SupportDeskDevelopmentDatabase.EnsureReadyAsync`):

```csharp
await using (var db = await factory.CreateDbContextAsync())
{
    var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
    await db.Database.MigrateAsync();
    var applied = (await db.Database.GetAppliedMigrationsAsync()).ToList();
    Console.Error.WriteLine($"[SupportDesk] MigrateAsync: {pending.Count} pending migration(s) applied, …");
}
```

It is guarded by `if (app.Environment.IsDevelopment())` and it replaces Module 1's `EnsureCreated` —
`EnsureCreated` builds the schema from the model and bypasses the migration history entirely, so the two
must never be mixed in one database.

In production, `Migrate()` at start is the thing to avoid: several instances starting at once race to
apply the same migration, one wins, the others fail or hang, and nobody reviewed the SQL. The release
runs the reviewed script or the bundle **before** traffic moves, and the application starts against a
schema it can only read. The page shows the difference honestly: the counts line under the buttons ends
with *Pending migrations N*, read through `SchemaInfoService.DescribeAsync()` (which calls
`GetPendingMigrationsAsync()`) on load and after every operation, so a pending migration would show up
there instead of being applied behind the operator's back.

## Evidence

Server console at start (Development):

```
[SupportDesk] MigrateAsync: 1 pending migration(s) applied, 1 applied in total (20260910150534_InitialCreate)
[SupportDesk] Development seed: seeded 5 customers, 3 agents, 6 categories, 60 tickets, 16 comments in 354 ms
```

On the second start the same two lines read `0 pending migration(s) applied, 1 applied in total` and
`nothing to seed: 60 tickets already exist`.

The counts line on the page ends with `Pending migrations 0` once the host has migrated the file.

Tests (`SupportDesk.Tests`):

- `MigrationTests.MigrateAsync_applies_InitialCreate_once_and_the_seeder_fills_the_schema` — the whole
  start-up path without Wisej.NET: `GetPendingMigrationsAsync()` returns `20260910150534_InitialCreate`
  on an empty database, `MigrateAsync()` applies it, the pending list empties, a **second**
  `MigrateAsync()` is a no-op (still one applied migration), the seeder then fills the migrated schema,
  and `sqlite_master` is read back to confirm the CHECK constraint, the three `ON DELETE` clauses and
  `"RowVersion" BLOB NOT NULL` are really in the created tables.
- `ModelRulesTests.The_model_carries_the_planned_indexes_delete_behaviours_and_check_constraint` — ends
  with `Assert.Empty(info.AppliedMigrations)`, which is the counter-example: the test database is built
  with `EnsureCreated`, so it has the same schema and **no** migration history.
