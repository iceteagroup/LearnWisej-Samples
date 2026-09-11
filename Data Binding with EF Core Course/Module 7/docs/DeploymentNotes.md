# Deployment notes

## Where each environment's connection string comes from

| Environment | Source | File |
|---|---|---|
| Development (this machine) | `ConnectionStrings:SupportDesk` in `appsettings.Development.json` / `appsettings.json` — a relative SQLite path resolved by `SupportDeskPaths.ResolveSqliteDataSource` against `App_Data/`. Committed on purpose — it is a local file path, not a secret. | `SupportDesk.Web/appsettings.Development.json`, `appsettings.json` |
| A shared test/staging environment | User secrets (`dotnet user-secrets`) on a developer machine, or the CI/CD pipeline's own secret store, injected the same way as production below. Never committed. | — |
| Production | The environment variable `ConnectionStrings__SupportDesk` (ASP.NET Core's configuration binder maps `__` to `:`), or the platform's managed secret store (Azure Key Vault, AWS Secrets Manager, a Kubernetes secret mounted as an env var) feeding that same variable. `appsettings.Production.json` carries only a **placeholder string** that says so — never a real value. | `SupportDesk.Web/appsettings.Production.json` |

`Startup.cs` reads the connection string once, the same way in every environment:

```csharp
var connectionString = builder.Configuration.GetConnectionString("SupportDesk")
    ?? throw new InvalidOperationException("Missing connection string 'SupportDesk'.");
```

Configuration precedence (ASP.NET Core default) means an environment variable set on the production host
always overrides `appsettings.Production.json`'s placeholder — the placeholder exists only so a developer
reading the repository can see immediately that a real value is expected from the environment, not typed in.

## Release steps: script reviewed → applied by the release pipeline, one instance, never from app startup

1. A developer runs `dotnet ef migrations add <Name> --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0` locally, commits `Migrations/*.cs` and the updated `SupportDeskContextModelSnapshot.cs`.
2. Before a release, `dotnet ef migrations script --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0 --output artifacts/sql/supportdesk_migrations.sql` regenerates the release script (see "The `--idempotent` deviation" below) and it is committed alongside the code change that needs it.
3. A human reviews the SQL diff in the pull request like any other change — this is the entire point of scripting migrations instead of calling `Migrate()` from the app: the exact statements that will run against production are visible before they run.
4. The release pipeline applies the reviewed script to the target database as **one step, from one place** — not from any running application instance. On SQLite this is normally "copy the file if it does not exist, then run the script against it" or an equivalent single controlled step for whatever hosts the file in that environment; on a networked database it would be `sqlite3`/the provider's own client run once by the pipeline, or a migrations bundle (`dotnet ef migrations bundle`) executed as its own release step.
5. Only after the script has applied does traffic switch to the new application version.

**Why never from application startup outside Development.** `Startup.cs` only calls
`SupportDeskDevelopmentDatabase.EnsureReadyAsync` (which runs `MigrateAsync`) `if (app.Environment.IsDevelopment())`. Production
never calls `Migrate()`. If it did: a typical production deployment starts more than one instance (for
availability or scale) at roughly the same moment; every one of them would race to `ALTER`/`CREATE` the same
tables, the loser(s) would see "table already exists" or a lock timeout as a *startup crash* rather than a
release-time failure someone is watching for, and there would be no reviewed diff to roll back from — only
whatever the currently-deployed code's model happened to compute at that instant. Migrations are source code,
reviewed like any other change; running them from every instance's own startup turns a one-time, reviewed,
rollback-able step into an unreviewed race that happens on every deploy and every restart.

## The `--idempotent` deviation

The lab guide and this module's own task both ask for
`dotnet ef migrations script --idempotent --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0 --output artifacts/sql/supportdesk_migrations.sql`.
Run verbatim, on this machine, against the SQLite provider, it throws:

```
System.NotSupportedException: Generating idempotent scripts for migrations is not currently supported for SQLite.
```

This is a real, current EF Core 10.0.12 + `Microsoft.EntityFrameworkCore.Sqlite` limitation, not a mistake in
the command. The script actually committed under `artifacts/sql/supportdesk_migrations.sql` was generated
**without** `--idempotent` — confirmed, by running both commands while building this module, to produce
**byte-for-byte identical output** up to the point the idempotent attempt throws, because SQLite's script
generator already emits one idempotency guard unconditionally, `--idempotent` or not:

```sql
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);
```

What this script is **not**: on SQL Server, `--idempotent` wraps every individual migration's statements in
`IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'…')` guards, so the whole
script can be re-run against a database that already has some (but not all) of the migrations applied, and
it only applies the missing ones. SQLite's script has no such per-migration guard around `CREATE TABLE
"Tickets"` and the rest — running it a second time against an already-migrated database would fail with
"table already exists". `SupportDesk.Tests/MigrationScriptTests.cs` asserts exactly what the script contains
(the history-table guard, the transaction wrapper, the history INSERT) and nothing more. The practical
consequence for the release process above: the pipeline step that applies this script must itself track
"has this migration already been applied to this database" — which it already does, by design, via the same
`__EFMigrationsHistory` table `MigrateAsync` and this script both use — so re-running the **wrong** (already
applied) script is prevented by checking that table before running the script, not by the script protecting
itself the way a SQL Server idempotent script would.

## Logging configuration

| Category | Development | Production |
|---|---|---|
| `Default` | `Information` | `Warning` |
| `Microsoft.AspNetCore` | `Warning` | `Warning` |
| `Microsoft.EntityFrameworkCore` | `Warning` | `Warning` |
| `Microsoft.EntityFrameworkCore.Database.Command` | (inherits `Microsoft.EntityFrameworkCore` = `Warning`, so SQL is not logged; set it to `Information` in `appsettings.Development.json` to see every statement, as Modules 3 to 6 do) | `Warning` — SQL text and parameter values never reach the production log at `Information` or below |
| `SupportDesk` (this application's own `ILogger<T>` categories) | (inherits `Default` = `Information`) | `Information` — enough to diagnose a failure without the database chatter |

`appsettings.Production.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning",
      "SupportDesk": "Information"
    }
  }
}
```

`EnableSensitiveDataLogging()` stays exactly where it always was, inside
`SupportDeskDataServiceCollectionExtensions.AddSupportDeskData`'s `if (isDevelopment)` block — Module 7 adds
no second copy of this flag anywhere. What Module 7 adds is *reporting* the decision: right after the DI
bridge, `Startup.cs` prints one line to the server console

```
[SupportDesk] environment: Development · sensitive-data logging: ON (Development)
```

(or `OFF`, outside Development). Outside Development it prints a second line instead of migrating:

```
[SupportDesk] migrations are not applied at startup outside Development: apply artifacts/sql/supportdesk_migrations.sql as a reviewed release step.
```

## Rollback notes

- **Schema rollback.** SQLite's `ALTER TABLE` support is limited (no `DROP COLUMN` before recent SQLite
  versions, no `ALTER COLUMN` at all — see the cookbook's "SQLite migration limits"), so `dotnet ef
  migrations remove` locally, before a migration ships, is the normal path; once a migration has shipped and
  data depends on it, the safer rollback is almost always a **new forward migration** that undoes the
  change, scripted and reviewed the same way, rather than attempting to run the old migration's `Down()`
  against a database that already has real rows shaped by the new one.
- **Application rollback.** Redeploying the previous application version is safe only if the schema the
  script applied is backward-compatible with it (additive columns, new tables, new indexes — everything this
  module's `InitialCreate` migration is). A destructive schema change (a dropped or renamed column) would
  need a two-step migration (add the new shape, deploy code that writes both, backfill, deploy code that
  only uses the new shape, then drop the old shape) — not needed here, but the pattern to reach for.
- **Data rollback.** Neither `MigrateAsync` nor the release script touches row data beyond what a migration's
  `Up()` does (this module's `InitialCreate` only creates tables/indexes — no data migration). A real data
  rollback needs a database backup/restore step outside EF Core's migration system entirely.

## Evidence

- `dotnet ef migrations script --idempotent …` — reproduced the `NotSupportedException` above, verbatim, on
  this machine.
- `dotnet ef migrations script …` (without `--idempotent`) — succeeded; output diffed byte-for-byte identical
  against a second run with `--idempotent` up to the point it throws, confirming the guard SQLite does emit
  is emitted regardless of the flag.
- `SupportDesk.Tests/MigrationScriptTests.cs` — the committed script exists under `artifacts/sql/`, contains
  the `IF NOT EXISTS` guard on `__EFMigrationsHistory`, records the applied migration, and wraps the schema
  changes in one transaction.
- `Startup.cs` — `if (app.Environment.IsDevelopment())` is the only path that calls
  `SupportDeskDevelopmentDatabase.EnsureReadyAsync` (and therefore `MigrateAsync`); read the file directly to
  confirm there is no other call site.
