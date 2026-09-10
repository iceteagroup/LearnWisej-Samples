namespace SupportDesk.Data.Diagnostics;

/// <summary>
/// What the host decided at startup, for the page's "Environment &amp; diagnostics" panel and the server
/// console: which environment is running, whether migrations were applied from this process, whether
/// sensitive-data logging is on, which migrations are applied, and where the reviewed production script
/// lives. Registered as a singleton and filled in once, from <c>Program.cs</c>, after the host decides —
/// never mutated again, the same "read what actually happened, do not hard-code it" habit
/// <c>SchemaInfoService</c> uses for the Model &amp; migration card.
/// </summary>
public sealed class StartupDiagnostics
{
    public string EnvironmentName { get; set; } = "";

    /// <summary>True only when this process ran <c>MigrateAsync</c> itself — Development only. False everywhere else: a production instance never migrates its own database at startup (see docs/DeploymentNotes.md).</summary>
    public bool MigrationsAppliedAtStartup { get; set; }

    public bool SensitiveDataLoggingOn { get; set; }

    public IReadOnlyList<string> AppliedMigrations { get; set; } = Array.Empty<string>();

    /// <summary>Where the reviewed, idempotent release script lives, relative to the module folder.</summary>
    public string MigrationScriptPath { get; set; } = "artifacts/sql/supportdesk_migrations.sql";
}
