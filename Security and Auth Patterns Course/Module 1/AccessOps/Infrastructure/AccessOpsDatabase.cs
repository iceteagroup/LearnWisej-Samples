using System;
using System.IO;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// The AccessOps database: one SQLite file in the module's AccessOpsData folder, outside the
    /// web root. Every command in the application is built here or in a repository with
    /// <see cref="SqliteParameter"/> values - never by concatenating text into SQL.
    /// </summary>
    public sealed class AccessOpsDatabase
    {
        private readonly string _connectionString;

        public AccessOpsDatabase(string dataRoot, string seedFolder)
        {
            this.DataRoot = Path.GetFullPath(dataRoot);
            this.SeedFolder = seedFolder;
            Directory.CreateDirectory(this.DataRoot);

            _connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(this.DataRoot, "accessops.db"),
                Mode = SqliteOpenMode.ReadWriteCreate,
                Pooling = false
            }.ToString();
        }

        /// <summary>The folder holding the database; never below the web root.</summary>
        public string DataRoot { get; }

        /// <summary>The folder holding the seed files that fill an empty database.</summary>
        public string SeedFolder { get; }

        public static AccessOpsDatabase FromConfiguration(IConfiguration configuration, string contentRoot)
        {
            var root = configuration["AccessOps:DataRoot"] ?? "../AccessOpsData";
            if (!Path.IsPathRooted(root))
                root = Path.Combine(contentRoot, root);

            return new AccessOpsDatabase(root, Path.Combine(AppContext.BaseDirectory, "Data"));
        }

        public SqliteConnection Open()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Creates the schema and, on an empty database, loads the demo employees from
        /// Data/seed-employees.json.
        /// </summary>
        public void EnsureCreated()
        {
            using var db = Open();
            Execute(db, @"
CREATE TABLE IF NOT EXISTS employees (
    id            INTEGER PRIMARY KEY,
    first_name    TEXT NOT NULL,
    last_name     TEXT NOT NULL,
    email         TEXT NOT NULL,
    phone         TEXT NOT NULL,
    home_address  TEXT NOT NULL,
    date_of_birth TEXT NOT NULL,
    hire_date     TEXT NOT NULL,
    department    TEXT NOT NULL,
    role          TEXT NOT NULL,
    manager_id    INTEGER NULL REFERENCES employees(id),
    salary_band   TEXT NOT NULL,
    review_note   TEXT NOT NULL,
    cost_centre   TEXT NOT NULL
);");

            using var count = db.CreateCommand();
            count.CommandText = "SELECT COUNT(*) FROM employees";
            if (Convert.ToInt64(count.ExecuteScalar()) > 0)
                return;

            SeedEmployees(db);
        }

        private void SeedEmployees(SqliteConnection db)
        {
            var file = Path.Combine(this.SeedFolder, "seed-employees.json");
            using var json = JsonDocument.Parse(File.ReadAllText(file));
            using var tx = db.BeginTransaction();

            // managers are referenced before they are inserted: check the keys at commit
            using (var defer = db.CreateCommand())
            {
                defer.Transaction = tx;
                defer.CommandText = "PRAGMA defer_foreign_keys = ON;";
                defer.ExecuteNonQuery();
            }

            foreach (var e in json.RootElement.GetProperty("employees").EnumerateArray())
            {
                using var cmd = db.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"
INSERT INTO employees (id, first_name, last_name, email, phone, home_address, date_of_birth, hire_date,
                       department, role, manager_id, salary_band, review_note, cost_centre)
VALUES ($id, $first, $last, $email, $phone, $address, $dob, $hired, $dept, $role, $manager, $band, $note, $cc);";
                cmd.Parameters.AddWithValue("$id", e.GetProperty("id").GetInt32());
                cmd.Parameters.AddWithValue("$first", e.GetProperty("firstName").GetString());
                cmd.Parameters.AddWithValue("$last", e.GetProperty("lastName").GetString());
                cmd.Parameters.AddWithValue("$email", e.GetProperty("email").GetString());
                cmd.Parameters.AddWithValue("$phone", e.GetProperty("phone").GetString());
                cmd.Parameters.AddWithValue("$address", e.GetProperty("homeAddress").GetString());
                cmd.Parameters.AddWithValue("$dob", e.GetProperty("dateOfBirth").GetString());
                cmd.Parameters.AddWithValue("$hired", e.GetProperty("hireDate").GetString());
                cmd.Parameters.AddWithValue("$dept", e.GetProperty("department").GetString());
                cmd.Parameters.AddWithValue("$role", e.GetProperty("role").GetString());
                cmd.Parameters.AddWithValue("$manager",
                    e.TryGetProperty("managerId", out var m) && m.ValueKind == JsonValueKind.Number ? m.GetInt32() : DBNull.Value);
                cmd.Parameters.AddWithValue("$band", e.GetProperty("salaryBand").GetString());
                cmd.Parameters.AddWithValue("$note", e.GetProperty("reviewNote").GetString());
                cmd.Parameters.AddWithValue("$cc", e.GetProperty("costCentre").GetString());
                cmd.ExecuteNonQuery();
            }

            tx.Commit();
        }

        internal static void Execute(SqliteConnection db, string sql)
        {
            using var cmd = db.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
