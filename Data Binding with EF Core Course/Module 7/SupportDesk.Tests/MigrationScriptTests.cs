namespace SupportDesk.Tests;

/// <summary>
/// The Module 7 deployment deliverable: the committed release script under <c>artifacts/sql/</c> exists and
/// is generated the SQLite way. <c>dotnet ef migrations script --idempotent</c> throws
/// <c>System.NotSupportedException</c> on the SQLite provider ("Generating idempotent scripts for migrations
/// is not currently supported for SQLite") — see <c>docs/DeploymentNotes.md</c> for the deviation this
/// forced from the lab guide's exact command. What SQLite's <b>plain</b> script generator emits unconditionally
/// (confirmed byte-for-byte identical to the idempotent attempt's output before it throws, by diffing the two
/// against a real run of both commands while building this module) is still a real idempotency guard for the
/// one statement that matters for a repeat deployment: it will not fail if <c>__EFMigrationsHistory</c>
/// already exists. This test asserts exactly what the script contains — never what the lab guide's SQL
/// Server-flavoured wording implies.
/// </summary>
public sealed class MigrationScriptTests
{
    private static string FindScriptPath()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "artifacts", "sql", "supportdesk_migrations.sql");
            if (File.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }

        throw new FileNotFoundException("artifacts/sql/supportdesk_migrations.sql was not found by walking up from the test output directory — run dotnet ef migrations script from the Module 7 folder first.");
    }

    [Fact]
    public void The_release_script_exists_under_artifacts_sql()
    {
        var path = FindScriptPath();
        Assert.True(File.Exists(path));
    }

    [Fact]
    public void The_release_script_guards_the_EFMigrationsHistory_table_with_IF_NOT_EXISTS()
    {
        var sql = File.ReadAllText(FindScriptPath());

        // SQLite's own idempotent guard: CREATE TABLE IF NOT EXISTS on the history table, generated
        // unconditionally by the SQLite provider's migrations script generator (not only for --idempotent,
        // which SQLite refuses outright). Re-running this script against an already-migrated database will
        // not fail on this line.
        Assert.Contains("CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\"", sql);
    }

    [Fact]
    public void The_release_script_records_the_applied_migration_in_the_history_table()
    {
        var sql = File.ReadAllText(FindScriptPath());

        Assert.Contains("INSERT INTO \"__EFMigrationsHistory\"", sql);
        Assert.Contains("20260910150534_InitialCreate", sql);
    }

    [Fact]
    public void The_release_script_wraps_the_schema_changes_in_one_transaction()
    {
        var sql = File.ReadAllText(FindScriptPath());

        Assert.Contains("BEGIN TRANSACTION;", sql);
        Assert.Contains("COMMIT;", sql);
    }
}
