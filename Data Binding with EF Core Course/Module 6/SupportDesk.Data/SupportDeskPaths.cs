using Microsoft.Data.Sqlite;

namespace SupportDesk.Data;

/// <summary>
/// Turns the relative SQLite path from configuration ("Data Source=App_Data/supportdesk.db") into an
/// absolute one so the application, the tests and <c>dotnet ef</c> all open the same file.
/// </summary>
public static class SupportDeskPaths
{
    public const string WebProjectFolder = "SupportDesk.Web";

    /// <summary>Resolves a relative Data Source against <paramref name="baseDirectory"/> and creates its folder.</summary>
    public static string ResolveSqliteDataSource(string connectionString, string baseDirectory)
    {
        var csb = new SqliteConnectionStringBuilder(connectionString);
        if (string.IsNullOrWhiteSpace(csb.DataSource) || csb.DataSource == ":memory:")
            return connectionString;

        if (!Path.IsPathRooted(csb.DataSource))
            csb.DataSource = Path.GetFullPath(Path.Combine(baseDirectory, csb.DataSource));

        Directory.CreateDirectory(Path.GetDirectoryName(csb.DataSource)!);
        return csb.ToString();
    }

    /// <summary>
    /// The development database used by the design-time factory: SupportDesk.Web/App_Data/supportdesk.db,
    /// found by walking up from the current directory (dotnet ef may run from the solution or a project folder).
    /// </summary>
    public static string DevelopmentDatabaseFile()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir is not null)
        {
            var web = Path.Combine(dir.FullName, WebProjectFolder);
            if (Directory.Exists(web))
            {
                var appData = Path.Combine(web, "App_Data");
                Directory.CreateDirectory(appData);
                return Path.Combine(appData, "supportdesk.db");
            }
            dir = dir.Parent;
        }

        // Fallback (running from somewhere unexpected): a file in the current directory.
        return Path.GetFullPath("supportdesk.db");
    }
}
