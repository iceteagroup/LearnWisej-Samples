using System.Collections.Generic;
using System.Globalization;
using AccessOps.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace AccessOps.Services
{
    public sealed class DocumentListItem
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Department { get; init; }
        public string Classification { get; init; }
        public string Size { get; init; }
        public string Updated { get; init; }
    }

    /// <summary>
    /// Document queries. The department rule is part of the SQL: rows of another department never
    /// leave the database, never appear in a count and never reach a grid. The repository takes the
    /// department as an argument - it does not know about sessions, so it is reusable and testable.
    /// </summary>
    public sealed class DocumentRepository
    {
        private readonly AccessOpsDatabase _db;
        private readonly ILogger<DocumentRepository> _log;

        public DocumentRepository(AccessOpsDatabase db, ILogger<DocumentRepository> log)
        {
            _db = db;
            _log = log;
        }

        /// <summary>The SQL text of the last list query (for the review step: the filter is in the query).</summary>
        public string LastSql { get; private set; }

        /// <param name="department">Only this department; null means every department (ViewAllDocuments).</param>
        public IReadOnlyList<DocumentListItem> List(string department)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT id, title, department, classification, size_bytes, updated_utc FROM documents"
                + (department == null ? "" : " WHERE department = $department")
                + " ORDER BY updated_utc DESC, id";
            if (department != null)
                cmd.Parameters.AddWithValue("$department", department);

            this.LastSql = cmd.CommandText;
            _log.LogInformation("Document query: {Sql} [department={Department}]", cmd.CommandText, department ?? "(all)");

            var rows = new List<DocumentListItem>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
                rows.Add(Read(r));
            return rows;
        }

        internal static DocumentListItem Read(SqliteDataReader r) => new DocumentListItem
        {
            Id = r.GetInt32(0),
            Title = r.GetString(1),
            Department = r.GetString(2),
            Classification = r.GetString(3),
            Size = Format.Size(r.GetInt64(4)),
            Updated = r.GetString(5)
        };
    }

    /// <summary>
    /// Every public method starts with the guard. The caller comes from the session, never from a
    /// parameter, so nobody can pass someone else's department and inherit their documents.
    /// </summary>
    public sealed class DocumentService
    {
        private readonly IAuthorize _authorize;
        private readonly DocumentRepository _documents;

        public DocumentService(IAuthorize authorize, DocumentRepository documents)
        {
            _authorize = authorize;
            _documents = documents;
        }

        public IReadOnlyList<DocumentListItem> List()
        {
            var me = _authorize.RequireAny(Permission.ViewAllDocuments, Permission.ViewDepartmentDocuments);
            return _documents.List(me.Has(Permission.ViewAllDocuments) ? null : me.Department);
        }
    }

    internal static class Format
    {
        public static string Size(long bytes)
            => bytes >= 1024 * 1024
                ? (bytes / 1048576.0).ToString("0.0", CultureInfo.InvariantCulture) + " MB"
                : (bytes / 1024).ToString(CultureInfo.InvariantCulture) + " KB";

        public static string Amount(double amount)
            => amount <= 0 ? "—" : "€ " + amount.ToString("#,##0", CultureInfo.InvariantCulture);
    }
}
