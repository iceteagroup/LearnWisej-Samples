using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using AccessOps.Identity;
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
        /// <summary>Partner account A: partner.ana@fabrikam.test, tenant A, Reader.</summary>
        public const string AnaAccountId = "a4c19e07-6b2d-4f80-9e35-1d7b0c2e5a17";

        /// <summary>A disabled partner account (partner.old@fabrikam.test), tenant A.</summary>
        public const string OldAccountId = "0d5b7e21-93fa-4c06-b8e4-7a1f62c9d3b0";

        /// <summary>A partner account in tenant C, which AccessOps has not approved.</summary>
        public const string KaiAccountId = "6e2f8a13-c7d4-45b9-a0e1-3b9c5d7f2e64";

        /// <summary>Dana Ellis, tenant A, Reader: signs in through Contoso Identity.</summary>
        public const string DanaAccountId = "41c7a9e2-5d03-4b8f-a6e1-0f2d9c7b31e5";

        /// <summary>The named service account that may still use the password form.</summary>
        public const string ReportsAccountId = "9e0b4d71-2c58-4a3f-8b16-e7d5a2c04f91";

        /// <summary>The issuer of the lab's test identity provider (Contoso Identity).</summary>
        public const string ContosoIssuer = "http://localhost:6415";

        private readonly string _connectionString;

        /// <summary>Opens (and creates) the database in <paramref name="dataRoot"/>.</summary>
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

        /// <summary>Reads <c>AccessOps:DataRoot</c> (relative paths are relative to the content root).</summary>
        public static AccessOpsDatabase FromConfiguration(IConfiguration configuration, string contentRoot)
        {
            var root = configuration["AccessOps:DataRoot"] ?? "../AccessOpsData";
            if (!Path.IsPathRooted(root))
                root = Path.Combine(contentRoot, root);
            return new AccessOpsDatabase(root);
        }

        /// <summary>A new, open connection (Microsoft.Data.Sqlite turns foreign keys on).</summary>
        public SqliteConnection Open()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// The 32-byte key behind the opaque audit pseudonyms, created once in the data folder so
        /// they stay stable across restarts.
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

        /// <summary>
        /// Creates the schema and, on an empty database, the Module 5 fixtures. The demo password
        /// hashes are derived only by Debug builds; a Release build seeds the accounts without any
        /// password, so nobody can sign in with a fixture secret.
        /// </summary>
        public void EnsureCreated(PasswordHasher hasher)
        {
            using var db = Open();
            Execute(db, @"
CREATE TABLE IF NOT EXISTS tenants (
    id        TEXT PRIMARY KEY,
    name      TEXT NOT NULL,
    approved  INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS accounts (
    id            TEXT PRIMARY KEY,
    user_name     TEXT NOT NULL UNIQUE,
    display_name  TEXT NOT NULL,
    tenant        TEXT NOT NULL REFERENCES tenants(id),
    role          TEXT NOT NULL,
    kind          TEXT NOT NULL DEFAULT 'interactive',
    disabled      INTEGER NOT NULL DEFAULT 0,
    password_hash TEXT NULL
);
CREATE TABLE IF NOT EXISTS external_logins (
    issuer      TEXT NOT NULL,
    subject     TEXT NOT NULL,
    account_id  TEXT NOT NULL REFERENCES accounts(id),
    PRIMARY KEY (issuer, subject)
);
CREATE TABLE IF NOT EXISTS grants (
    id          TEXT PRIMARY KEY,
    account_id  TEXT NOT NULL REFERENCES accounts(id),
    tenant      TEXT NOT NULL REFERENCES tenants(id),
    role        TEXT NOT NULL,
    can_read    INTEGER NOT NULL,
    created_at  TEXT NOT NULL,
    expires_at  TEXT NOT NULL,
    revoked     INTEGER NOT NULL DEFAULT 0,
    revoked_at  TEXT NULL
);
CREATE TABLE IF NOT EXISTS documents (
    tenant         TEXT NOT NULL REFERENCES tenants(id),
    id             INTEGER NOT NULL,
    title          TEXT NOT NULL,
    classification TEXT NOT NULL,
    body           TEXT NOT NULL,
    PRIMARY KEY (tenant, id)
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
CREATE TABLE IF NOT EXISTS login_audit (
    seq            INTEGER PRIMARY KEY AUTOINCREMENT,
    at             TEXT NOT NULL,
    subject        TEXT NOT NULL,
    address        TEXT NOT NULL,
    outcome        TEXT NOT NULL,
    correlation_id TEXT NOT NULL
);
CREATE TRIGGER IF NOT EXISTS audit_no_update BEFORE UPDATE ON audit
BEGIN SELECT RAISE(ABORT, 'audit is append-only'); END;
CREATE TRIGGER IF NOT EXISTS audit_no_delete BEFORE DELETE ON audit
BEGIN SELECT RAISE(ABORT, 'audit is append-only'); END;
CREATE TRIGGER IF NOT EXISTS login_audit_no_update BEFORE UPDATE ON login_audit
BEGIN SELECT RAISE(ABORT, 'login_audit is append-only'); END;
CREATE TRIGGER IF NOT EXISTS login_audit_no_delete BEFORE DELETE ON login_audit
BEGIN SELECT RAISE(ABORT, 'login_audit is append-only'); END;");

            using var count = db.CreateCommand();
            count.CommandText = "SELECT COUNT(*) FROM tenants";
            if (Convert.ToInt64(count.ExecuteScalar()) > 0)
                return;

            Seed(db, hasher);
        }

        private static void Seed(SqliteConnection db, PasswordHasher hasher)
        {
            using var tx = db.BeginTransaction();

            // Order matters: foreign keys are on, so parents go first.
            const string tenant = "INSERT INTO tenants (id, name, approved) VALUES ($a, $b, $c)";
            Insert(db, tx, tenant, "A", "Tenant A", 1);
            Insert(db, tx, tenant, "B", "Tenant B", 1);
            Insert(db, tx, tenant, "C", "Tenant C", 0);

            const string account = "INSERT INTO accounts (id, user_name, display_name, tenant, role, kind, disabled, password_hash) VALUES ($a, $b, $c, $d, $e, $f, $g, $h)";
            Insert(db, tx, account, AnaAccountId, "partner.ana@fabrikam.test", "Ana Ruiz", "A", "Reader", "interactive", 0, DemoHash(hasher, "partner.ana@fabrikam.test"));
            Insert(db, tx, account, OldAccountId, "partner.old@fabrikam.test", "Former partner", "A", "Reader", "interactive", 1, DemoHash(hasher, "partner.old@fabrikam.test"));
            Insert(db, tx, account, KaiAccountId, "partner.kai@northwind.test", "Kai Berg", "C", "Reader", "interactive", 0, DemoHash(hasher, "partner.kai@northwind.test"));
            Insert(db, tx, account, DanaAccountId, "dana.ellis", "Dana Ellis", "A", "Reader", "interactive", 0, DBNull.Value);
            Insert(db, tx, account, ReportsAccountId, "svc.reports", "Reports service", "A", "Reader", "service", 0, DemoHash(hasher, "svc.reports"));

            // issuer + subject -> local account (never an e-mail address)
            const string login = "INSERT INTO external_logins (issuer, subject, account_id) VALUES ($a, $b, $c)";
            Insert(db, tx, login, ContosoIssuer, "4f21-9ac0-dana-ellis", DanaAccountId);
            Insert(db, tx, login, ContosoIssuer, "c77d-21b4-kai-berg", KaiAccountId);       // tenant C: not approved (F08)

            const string doc = "INSERT INTO documents (tenant, id, title, classification, body) VALUES ($a, $b, $c, $d, $e)";
            Insert(db, tx, doc, "A", 17, "Supplier contract", "Confidential",
                "Tenant A. Fabrikam supply framework, 36 months, price cap 4% a year, termination on 90 days' notice.");
            Insert(db, tx, doc, "A", 22, "Rate card 2026", "Internal",
                "Tenant A. Partner day rates for 2026, valid from 1 January, reviewed in June.");
            Insert(db, tx, doc, "A", 31, "Onboarding checklist", "Internal",
                "Tenant A. NDA on file, named contacts, tenant A only, quarterly access review.");
            Insert(db, tx, doc, "B", 17, "Supplier contract — Tailspin logistics", "Confidential",
                "Tenant B. Tailspin freight framework, 24 months, volume rebate above 1,200 pallets.");
            Insert(db, tx, doc, "B", 19, "Warehouse safety audit", "Internal",
                "Tenant B. Two findings: racking labels and the loading-bay mirror.");

            tx.Commit();
        }

        // Debug builds derive the demo hashes from the account's user name; a Release build stores no
        // password at all and does not even contain the demo password strings (a string literal passed
        // to a method whose body is #if DEBUG still ends up in the Release dll).
        private static object DemoHash(PasswordHasher hasher, string userName)
        {
#if DEBUG
            var demo = userName switch
            {
                "partner.ana@fabrikam.test" => "Fabrikam-demo-17",
                "partner.old@fabrikam.test" => "Fabrikam-demo-old",
                "partner.kai@northwind.test" => "Northwind-demo-23",
                "svc.reports" => "Reports-demo-31",
                _ => null
            };
            return demo == null ? DBNull.Value : hasher.Hash(demo);
#else
            return DBNull.Value;
#endif
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

        /// <summary>An ISO 8601 round-trip timestamp for storage.</summary>
        public static string Stamp(DateTimeOffset value) => value.ToString("o", CultureInfo.InvariantCulture);

        /// <summary>Parses a stored timestamp.</summary>
        public static DateTimeOffset ParseStamp(string value)
            => DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }
}
