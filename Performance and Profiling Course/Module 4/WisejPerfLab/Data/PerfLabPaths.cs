using System.IO;
using Microsoft.Data.Sqlite;

namespace WisejPerfLab.Data
{
    /// <summary>
    /// Turns the relative SQLite path from configuration ("Data Source=App_Data/perflab.db") into an
    /// absolute one, so the app opens the same file whatever the working directory is.
    /// </summary>
    public static class PerfLabPaths
    {
        public static string ResolveSqliteDataSource(string connectionString, string baseDirectory)
        {
            var csb = new SqliteConnectionStringBuilder(connectionString);
            if (string.IsNullOrWhiteSpace(csb.DataSource) || csb.DataSource == ":memory:")
                return connectionString;

            if (!Path.IsPathRooted(csb.DataSource))
                csb.DataSource = Path.GetFullPath(Path.Combine(baseDirectory, csb.DataSource));

            Directory.CreateDirectory(Path.GetDirectoryName(csb.DataSource));
            return csb.ToString();
        }
    }
}
