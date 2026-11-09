using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// The AccessOps database: one SQLite file in the module's AccessOpsData folder, outside the
    /// web root. Every command is built with <see cref="SqliteParameter"/> values - never by
    /// concatenating text into SQL.
    /// </summary>
    public sealed class AccessOpsDatabase
    {
        /// <summary>Tenant A's Reader account, reader1 (the redacted subject reads "acct …7f3a").</summary>
        public const string ReaderAccountId = "3b9e61d0-52c4-4a8e-9d1f-0c6e2b8a7f3a";

        /// <summary>visitor1: an account with no grant (the application deny after host sign-in).</summary>
        public const string UnassignedAccountId = "c27f0a94-8e15-4b3d-a6c2-51d9e04b19c8";

        /// <summary>The administrator who revokes grants in the admin console.</summary>
        public const string AdminAccountId = "e8d14b62-07a3-4f9c-b5e1-6a2c93f0d4b7";

        /// <summary>The Reader grant for tenant A (shown as "…21").</summary>
        public const string ReaderGrantId = "7c1e2a40-0b9d-4e55-9a6f-5d3c8b0e2f21";

        /// <summary>The administrator's grant.</summary>
        public const string AdminGrantId = "0a9e3d57-b8c2-4e16-9f4a-72d5c18e3b44";

        private readonly string _connectionString;

        public AccessOpsDatabase(string dataRoot)
        {
            this.DataRoot = Path.GetFullPath(dataRoot);
            Directory.CreateDirectory(this.DataRoot);

            _connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(this.DataRoot, "accessops.db"),
                Mode = SqliteOpenMode.ReadWriteCreate,
                Pooling = false
            }.ToString();
        }

        /// <summary>The folder holding the database and the audit key; never below the web root.</summary>
        public string DataRoot { get; }

        public static AccessOpsDatabase FromConfiguration(IConfiguration configuration, string contentRoot)
        {
            var root = configuration["AccessOps:DataRoot"] ?? "../AccessOpsData";
            if (!Path.IsPathRooted(root))
                root = Path.Combine(contentRoot, root);
            return new AccessOpsDatabase(root);
        }

        public SqliteConnection Open()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// The 32-byte key behind <c>Subject.Opaque</c>, created once in the data folder so audit
        /// pseudonyms stay stable across restarts.
        /// </summary>
        public byte[] LoadOrCreateOpaqueKey()
        {
            var file = Path.Combine(this.DataRoot, "audit-subject.key");
            if (File.Exists(file))
                return File.ReadAllBytes(file);
            var key = RandomNumberGenerator.GetBytes(32);
            File.WriteAllBytes(file, key);
            return key;
        }

        /// <summary>Creates the schema and, on an empty database, the Module 3 fixtures.</summary>
        public void EnsureCreated()
        {
            using var db = Open();
            Execute(db, @"
CREATE TABLE IF NOT EXISTS tenants (
    id   TEXT PRIMARY KEY,
    name TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS accounts (
    id           TEXT PRIMARY KEY,
    user_name    TEXT NOT NULL UNIQUE,
    tenant       TEXT NOT NULL REFERENCES tenants(id),
    grant_id     TEXT NULL,
    roles        TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS grants (
    id          TEXT PRIMARY KEY,
    account_id  TEXT NOT NULL REFERENCES accounts(id),
    tenant      TEXT NOT NULL REFERENCES tenants(id),
    role        TEXT NOT NULL,
    can_read    INTEGER NOT NULL,
    can_admin   INTEGER NOT NULL,
    revoked     INTEGER NOT NULL DEFAULT 0,
    expires_at  TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS documents (
    tenant         TEXT NOT NULL REFERENCES tenants(id),
    id             INTEGER NOT NULL,
    title          TEXT NOT NULL,
    classification TEXT NOT NULL,
    body           TEXT NOT NULL,
    PRIMARY KEY (tenant, id)
);
CREATE TABLE IF NOT EXISTS windows_accounts (
    sid         TEXT PRIMARY KEY,
    account_id  TEXT NOT NULL REFERENCES accounts(id)
);
CREATE TABLE IF NOT EXISTS confidential_files (
    doc_id     TEXT PRIMARY KEY,
    tenant     TEXT NOT NULL REFERENCES tenants(id),
    title      TEXT NOT NULL,
    folder     TEXT NOT NULL,
    file_name  TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS audit (
    seq            INTEGER PRIMARY KEY AUTOINCREMENT,
    at             TEXT NOT NULL,
    subject        TEXT NOT NULL,
    action         TEXT NOT NULL,
    resource       TEXT NOT NULL,
    classification TEXT NOT NULL,
    decision       TEXT NOT NULL,
    reason         TEXT NULL,
    correlation_id TEXT NOT NULL
);
CREATE TRIGGER IF NOT EXISTS audit_no_update BEFORE UPDATE ON audit
BEGIN SELECT RAISE(ABORT, 'audit is append-only'); END;
CREATE TRIGGER IF NOT EXISTS audit_no_delete BEFORE DELETE ON audit
BEGIN SELECT RAISE(ABORT, 'audit is append-only'); END;");

            using var count = db.CreateCommand();
            count.CommandText = "SELECT COUNT(*) FROM tenants";
            if (Convert.ToInt64(count.ExecuteScalar()) > 0)
                return;

            Seed(db);
        }

        private static void Seed(SqliteConnection db)
        {
            using var tx = db.BeginTransaction();

            // Order matters: Microsoft.Data.Sqlite turns foreign keys on, so parents go first.
            Insert(db, tx, "INSERT INTO tenants (id, name) VALUES ($a, $b)", "A", "Tenant A");
            Insert(db, tx, "INSERT INTO tenants (id, name) VALUES ($a, $b)", "B", "Tenant B");

            Insert(db, tx, "INSERT INTO accounts (id, user_name, tenant, grant_id, roles) VALUES ($a, $b, $c, $d, $e)",
                ReaderAccountId, "reader1", "A", ReaderGrantId, "Reader");
            Insert(db, tx, "INSERT INTO accounts (id, user_name, tenant, grant_id, roles) VALUES ($a, $b, $c, $d, $e)",
                UnassignedAccountId, "visitor1", "A", DBNull.Value, "");
            Insert(db, tx, "INSERT INTO accounts (id, user_name, tenant, grant_id, roles) VALUES ($a, $b, $c, $d, $e)",
                AdminAccountId, "admin", "A", AdminGrantId, "Admin");

            var expires = DateTimeOffset.UtcNow.AddYears(1).ToString("o", CultureInfo.InvariantCulture);
            Insert(db, tx, "INSERT INTO grants (id, account_id, tenant, role, can_read, can_admin, expires_at) VALUES ($a, $b, $c, $d, $e, $f, $g)",
                ReaderGrantId, ReaderAccountId, "A", "Reader", 1, 0, expires);
            Insert(db, tx, "INSERT INTO grants (id, account_id, tenant, role, can_read, can_admin, expires_at) VALUES ($a, $b, $c, $d, $e, $f, $g)",
                AdminGrantId, AdminAccountId, "A", "Admin", 0, 1, expires);

            const string doc = "INSERT INTO documents (tenant, id, title, classification, body) VALUES ($a, $b, $c, $d, $e)";
            // Tenant A as the Module 2 walkthrough shows it.
            Insert(db, tx, doc, "A", 17, "Supplier contract renewal", "Confidential",
                "Renewal terms for the tenant A supplier contract, effective next quarter.");
            Insert(db, tx, doc, "A", 18, "Payroll adjustments Q3", "Restricted",
                "Tenant A. Q3 payroll corrections: two back-payments and one allowance change.");
            Insert(db, tx, doc, "A", 21, "Site access audit", "Internal",
                "Tenant A. Badge readers at the north entrance logged three expired passes.");
            Insert(db, tx, doc, "A", 24, "Department budget draft", "Confidential",
                "Tenant A. Draft operating budget for next year, pending the finance review.");
            Insert(db, tx, doc, "B", 17, "Supplier contract — Fabrikam logistics", "Confidential",
                "Tenant B. Fabrikam freight framework, 24 months, volume rebate above 1,200 pallets.");
            Insert(db, tx, doc, "B", 19, "Warehouse safety audit", "Internal",
                "Tenant B. Two findings: racking labels and the loading-bay mirror.");
            Insert(db, tx, doc, "B", 23, "Pricing review — Q4", "Confidential",
                "Tenant B. List prices up 3%, key accounts held flat until March.");

            // Module 3: the confidential files. The browser only ever sends the document ID; the
            // folder and file name stay on the server (DocumentCatalog.ResolveApprovedPath).
            const string file = "INSERT INTO confidential_files (doc_id, tenant, title, folder, file_name) VALUES ($a, $b, $c, $d, $e)";
            Insert(db, tx, file, "DOC-201", "A", "Salary bands 2027", "labuser-only", "DOC-201-salary-bands-2027.txt");
            Insert(db, tx, file, "DOC-202", "A", "Vendor contract archive", "pool-only", "DOC-202-vendor-contract-archive.txt");
            Insert(db, tx, file, "DOC-203", "A", "Audit sample (F03)", "labuser-only", "DOC-203-audit-sample.txt");   // F03: never created
            Insert(db, tx, file, "DOC-204", "B", "Tenant B legal hold", "tenant-b", "DOC-204-legal-hold.txt");

            tx.Commit();
        }

        private static void Insert(SqliteConnection db, SqliteTransaction tx, string sql, params object[] values)
        {
            using var cmd = db.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;
            for (int i = 0; i < values.Length; i++)
                cmd.Parameters.AddWithValue("$" + (char)('a' + i), values[i]);
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
