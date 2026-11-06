using System;
using System.IO;
using System.Text.Json;
using AccessOps.Services;
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
        /// Creates the schema and, on an empty database, loads the demo data. Demo passwords are
        /// hashed through <see cref="PasswordService"/> on the way in; only the derivation is stored.
        /// </summary>
        public void EnsureCreated(PasswordService passwords)
        {
            using var db = Open();
            Execute(db, @"
CREATE TABLE IF NOT EXISTS employees (
    id            INTEGER PRIMARY KEY,
    user_name     TEXT NULL UNIQUE COLLATE BINARY,
    first_name    TEXT NOT NULL,
    last_name     TEXT NOT NULL,
    email         TEXT NOT NULL,
    phone         TEXT NOT NULL,
    home_address  TEXT NOT NULL,
    date_of_birth TEXT NOT NULL,
    hire_date     TEXT NOT NULL,
    department    TEXT NOT NULL,
    role          TEXT NOT NULL,
    roles         TEXT NOT NULL,
    is_disabled   INTEGER NOT NULL DEFAULT 0,
    is_service_account INTEGER NOT NULL DEFAULT 0,
    manager_id    INTEGER NULL REFERENCES employees(id),
    salary_band   TEXT NOT NULL,
    review_note   TEXT NOT NULL,
    cost_centre   TEXT NOT NULL,
    -- credential columns: algorithm, iteration count, salt, derived bytes. No password column.
    pw_algorithm  TEXT NULL,
    pw_iterations INTEGER NULL,
    pw_salt       BLOB NULL,
    pw_hash       BLOB NULL
);
CREATE TABLE IF NOT EXISTS login_audit (
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    at_utc          TEXT NOT NULL,
    user_name_tried TEXT NOT NULL,
    client_address  TEXT NOT NULL,
    outcome         TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS external_logins (
    issuer      TEXT NOT NULL COLLATE BINARY,
    subject     TEXT NOT NULL COLLATE BINARY,
    employee_id INTEGER NOT NULL REFERENCES employees(id),
    PRIMARY KEY (issuer, subject)
);
CREATE TABLE IF NOT EXISTS employee_claims (
    employee_id INTEGER NOT NULL REFERENCES employees(id),
    kind        TEXT NOT NULL CHECK (kind IN ('grant', 'deny')),
    permission  TEXT NOT NULL,
    PRIMARY KEY (employee_id, kind, permission)
);
CREATE TABLE IF NOT EXISTS documents (
    id             INTEGER PRIMARY KEY,
    title          TEXT NOT NULL,
    department     TEXT NOT NULL,
    classification TEXT NOT NULL,
    size_bytes     INTEGER NOT NULL,
    updated_utc    TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS approval_requests (
    id           INTEGER PRIMARY KEY,
    requester_id INTEGER NOT NULL REFERENCES employees(id),
    subject      TEXT NOT NULL,
    department   TEXT NOT NULL,
    amount       REAL NOT NULL,
    status       TEXT NOT NULL,
    decided_by   INTEGER NULL REFERENCES employees(id),
    decided_utc  TEXT NULL,
    note         TEXT NULL
);");

            using var count = db.CreateCommand();
            count.CommandText = "SELECT COUNT(*) FROM employees";
            if (Convert.ToInt64(count.ExecuteScalar()) > 0)
                return;

            using var tx = db.BeginTransaction();
            Run(db, tx, "PRAGMA defer_foreign_keys = ON;");
            SeedEmployees(db, tx);
            SeedCredentials(db, tx, passwords);
            SeedDocuments(db, tx);
            SeedApprovals(db, tx);
            SeedClaims(db, tx);
            SeedExternalLogins(db, tx);
            tx.Commit();
        }

        private JsonElement ReadSeed(string file, string property)
        {
            var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(this.SeedFolder, file)));
            return json.RootElement.GetProperty(property);
        }

        private void SeedEmployees(SqliteConnection db, SqliteTransaction tx)
        {
            foreach (var e in ReadSeed("seed-employees.json", "employees").EnumerateArray())
            {
                using var cmd = db.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"
INSERT INTO employees (id, user_name, first_name, last_name, email, phone, home_address, date_of_birth, hire_date,
                       department, role, roles, is_disabled, is_service_account, manager_id, salary_band, review_note, cost_centre)
VALUES ($id, $user, $first, $last, $email, $phone, $address, $dob, $hired, $dept, $role, $roles, $disabled,
        $service, $manager, $band, $note, $cc);";
                cmd.Parameters.AddWithValue("$id", e.GetProperty("id").GetInt32());
                cmd.Parameters.AddWithValue("$user", NullableString(e, "userName"));
                cmd.Parameters.AddWithValue("$first", e.GetProperty("firstName").GetString());
                cmd.Parameters.AddWithValue("$last", e.GetProperty("lastName").GetString());
                cmd.Parameters.AddWithValue("$email", e.GetProperty("email").GetString());
                cmd.Parameters.AddWithValue("$phone", e.GetProperty("phone").GetString());
                cmd.Parameters.AddWithValue("$address", e.GetProperty("homeAddress").GetString());
                cmd.Parameters.AddWithValue("$dob", e.GetProperty("dateOfBirth").GetString());
                cmd.Parameters.AddWithValue("$hired", e.GetProperty("hireDate").GetString());
                cmd.Parameters.AddWithValue("$dept", e.GetProperty("department").GetString());
                cmd.Parameters.AddWithValue("$role", e.GetProperty("role").GetString());
                cmd.Parameters.AddWithValue("$roles", e.GetProperty("roles").GetString());
                cmd.Parameters.AddWithValue("$disabled",
                    e.TryGetProperty("disabled", out var d) && d.ValueKind == JsonValueKind.True ? 1 : 0);
                cmd.Parameters.AddWithValue("$service",
                    e.TryGetProperty("serviceAccount", out var sa) && sa.ValueKind == JsonValueKind.True ? 1 : 0);
                cmd.Parameters.AddWithValue("$manager",
                    e.TryGetProperty("managerId", out var m) && m.ValueKind == JsonValueKind.Number ? m.GetInt32() : DBNull.Value);
                cmd.Parameters.AddWithValue("$band", e.GetProperty("salaryBand").GetString());
                cmd.Parameters.AddWithValue("$note", e.GetProperty("reviewNote").GetString());
                cmd.Parameters.AddWithValue("$cc", e.GetProperty("costCentre").GetString());
                cmd.ExecuteNonQuery();
            }
        }

        private void SeedCredentials(SqliteConnection db, SqliteTransaction tx, PasswordService passwords)
        {
            foreach (var a in ReadSeed("seed-credentials.json", "accounts").EnumerateArray())
            {
                using var find = db.CreateCommand();
                find.Transaction = tx;
                find.CommandText = "SELECT id FROM employees WHERE user_name = $user";
                find.Parameters.AddWithValue("$user", a.GetProperty("userName").GetString());
                var id = Convert.ToInt32(find.ExecuteScalar());

                var credential = passwords.Hash(id, a.GetProperty("password").GetString());

                using var cmd = db.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"
UPDATE employees SET pw_algorithm = $alg, pw_iterations = $iter, pw_salt = $salt, pw_hash = $hash
WHERE id = $id;";
                cmd.Parameters.AddWithValue("$alg", credential.Algorithm);
                cmd.Parameters.AddWithValue("$iter", credential.Iterations);
                cmd.Parameters.AddWithValue("$salt", credential.Salt);
                cmd.Parameters.AddWithValue("$hash", credential.Hash);
                cmd.Parameters.AddWithValue("$id", id);
                cmd.ExecuteNonQuery();
            }
        }

        private void SeedDocuments(SqliteConnection db, SqliteTransaction tx)
        {
            foreach (var d in ReadSeed("seed-documents.json", "documents").EnumerateArray())
            {
                using var cmd = db.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"
INSERT INTO documents (id, title, department, classification, size_bytes, updated_utc)
VALUES ($id, $title, $dept, $class, $size, $updated);";
                cmd.Parameters.AddWithValue("$id", d.GetProperty("id").GetInt32());
                cmd.Parameters.AddWithValue("$title", d.GetProperty("title").GetString());
                cmd.Parameters.AddWithValue("$dept", d.GetProperty("department").GetString());
                cmd.Parameters.AddWithValue("$class", d.GetProperty("classification").GetString());
                cmd.Parameters.AddWithValue("$size", d.GetProperty("sizeBytes").GetInt64());
                cmd.Parameters.AddWithValue("$updated", d.GetProperty("updated").GetString());
                cmd.ExecuteNonQuery();
            }
        }

        private void SeedApprovals(SqliteConnection db, SqliteTransaction tx)
        {
            foreach (var r in ReadSeed("seed-approvals.json", "requests").EnumerateArray())
            {
                using var cmd = db.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"
INSERT INTO approval_requests (id, requester_id, subject, department, amount, status)
VALUES ($id, $requester, $subject, $dept, $amount, $status);";
                cmd.Parameters.AddWithValue("$id", r.GetProperty("id").GetInt32());
                cmd.Parameters.AddWithValue("$requester", r.GetProperty("requesterId").GetInt32());
                cmd.Parameters.AddWithValue("$subject", r.GetProperty("subject").GetString());
                cmd.Parameters.AddWithValue("$dept", r.GetProperty("department").GetString());
                cmd.Parameters.AddWithValue("$amount", r.GetProperty("amount").GetDouble());
                cmd.Parameters.AddWithValue("$status", r.GetProperty("status").GetString());
                cmd.ExecuteNonQuery();
            }
        }

        private void SeedClaims(SqliteConnection db, SqliteTransaction tx)
        {
            foreach (var claim in ReadSeed("seed-claims.json", "claims").EnumerateArray())
            {
                using var cmd = db.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = "INSERT INTO employee_claims (employee_id, kind, permission) VALUES ($id, $kind, $permission);";
                cmd.Parameters.AddWithValue("$id", claim.GetProperty("employeeId").GetInt32());
                cmd.Parameters.AddWithValue("$kind", claim.GetProperty("kind").GetString());
                cmd.Parameters.AddWithValue("$permission", claim.GetProperty("permission").GetString());
                cmd.ExecuteNonQuery();
            }
        }

        private void SeedExternalLogins(SqliteConnection db, SqliteTransaction tx)
        {
            foreach (var l in ReadSeed("seed-external-logins.json", "logins").EnumerateArray())
            {
                using var cmd = db.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = "INSERT INTO external_logins (issuer, subject, employee_id) VALUES ($issuer, $subject, $id);";
                cmd.Parameters.AddWithValue("$issuer", l.GetProperty("issuer").GetString());
                cmd.Parameters.AddWithValue("$subject", l.GetProperty("subject").GetString());
                cmd.Parameters.AddWithValue("$id", l.GetProperty("employeeId").GetInt32());
                cmd.ExecuteNonQuery();
            }
        }

        private static object NullableString(JsonElement e, string name)
            => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : DBNull.Value;

        private static void Run(SqliteConnection db, SqliteTransaction tx, string sql)
        {
            using var cmd = db.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        internal static void Execute(SqliteConnection db, string sql)
        {
            using var cmd = db.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
