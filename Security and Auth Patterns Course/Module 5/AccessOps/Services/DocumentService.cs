using System;
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

    /// <summary>A document record as the service needs it. StoredName never leaves the server.</summary>
    public sealed class DocumentRecord
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Department { get; init; }
        public string Classification { get; init; }
        public string StoredName { get; init; }
        public string ContentType { get; init; }
    }

    /// <summary>
    /// Document queries and commands, every one parameterised. The department rule is part of the
    /// SQL; the repository takes the department as an argument and knows nothing about sessions.
    /// </summary>
    public sealed class DocumentRepository
    {
        private const string ListColumns = "SELECT id, title, department, classification, size_bytes, updated_utc FROM documents";

        private readonly AccessOpsDatabase _db;
        private readonly ILogger<DocumentRepository> _log;

        public DocumentRepository(AccessOpsDatabase db, ILogger<DocumentRepository> log)
        {
            _db = db;
            _log = log;
        }

        /// <param name="department">Only this department; null means every department (ViewAllDocuments).</param>
        public IReadOnlyList<DocumentListItem> List(string department)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = ListColumns
                + (department == null ? "" : " WHERE department = $department")
                + " ORDER BY updated_utc DESC, id";
            if (department != null)
                cmd.Parameters.AddWithValue("$department", department);

            _log.LogInformation("Document query: {Sql} [department={Department}]", cmd.CommandText, department ?? "(all)");
            return ReadList(cmd);
        }

        /// <summary>A title search: the term is a parameter, so quotes and semicolons are search text.</summary>
        public IReadOnlyList<DocumentListItem> Search(string department, string term)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = ListColumns + " WHERE instr(lower(title), lower($term)) > 0"
                + (department == null ? "" : " AND department = $department")
                + " ORDER BY updated_utc DESC, id";
            cmd.Parameters.AddWithValue("$term", term ?? "");
            if (department != null)
                cmd.Parameters.AddWithValue("$department", department);
            return ReadList(cmd);
        }

        public DocumentRecord FindById(int id)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT id, title, department, classification, stored_name, content_type FROM documents WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return null;
            return new DocumentRecord
            {
                Id = r.GetInt32(0),
                Title = r.GetString(1),
                Department = r.GetString(2),
                Classification = r.GetString(3),
                StoredName = r.IsDBNull(4) ? null : r.GetString(4),
                ContentType = r.IsDBNull(5) ? "application/octet-stream" : r.GetString(5)
            };
        }

        public int Insert(string title, string department, string classification, long size, string storedName, string contentType, int uploadedBy)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
INSERT INTO documents (title, department, classification, size_bytes, updated_utc, stored_name, content_type, uploaded_by)
VALUES ($title, $department, $class, $size, $updated, $stored, $type, $by);
SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$title", title);
            cmd.Parameters.AddWithValue("$department", department);
            cmd.Parameters.AddWithValue("$class", classification);
            cmd.Parameters.AddWithValue("$size", size);
            cmd.Parameters.AddWithValue("$updated", DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            cmd.Parameters.AddWithValue("$stored", storedName);
            cmd.Parameters.AddWithValue("$type", contentType);
            cmd.Parameters.AddWithValue("$by", uploadedBy);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /// <summary>First run: gives every seeded record a generated demo file in the store.</summary>
        public void EnsureSeedFiles(DocumentStore store)
        {
            using var connection = _db.Open();
            var missing = new List<(int Id, string Title)>();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, title FROM documents WHERE stored_name IS NULL";
                using var r = cmd.ExecuteReader();
                while (r.Read())
                    missing.Add((r.GetInt32(0), r.GetString(1)));
            }

            foreach (var (id, title) in missing)
            {
                var (content, contentType) = DemoFiles.For(title);
                var stored = DocumentStore.NewStoredName();
                store.WriteDocument(stored, content);
                using var update = connection.CreateCommand();
                update.CommandText = "UPDATE documents SET stored_name = $stored, content_type = $type, size_bytes = $size WHERE id = $id";
                update.Parameters.AddWithValue("$stored", stored);
                update.Parameters.AddWithValue("$type", contentType);
                update.Parameters.AddWithValue("$size", content.LongLength);
                update.Parameters.AddWithValue("$id", id);
                update.ExecuteNonQuery();
            }
        }

        private static IReadOnlyList<DocumentListItem> ReadList(SqliteCommand cmd)
        {
            var rows = new List<DocumentListItem>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                rows.Add(new DocumentListItem
                {
                    Id = r.GetInt32(0),
                    Title = r.GetString(1),
                    Department = r.GetString(2),
                    Classification = r.GetString(3),
                    Size = Format.Size(r.GetInt64(4)),
                    Updated = r.GetString(5)
                });
            }
            return rows;
        }
    }

    /// <summary>Sends a file to the current session. Wisej.NET: Application.Download; a test supplies its own.</summary>
    public interface IFileDelivery
    {
        void Send(byte[] content, string fileName);
    }

    public sealed record UploadResult(UploadDecision Decision, int? DocumentId);

    /// <summary>
    /// The only way a document enters or leaves AccessOps. Every public method starts with the guard;
    /// the caller comes from the session.
    /// </summary>
    public sealed class DocumentService
    {
        public const string NotAvailable = "That document is not available to you. The attempt has been recorded.";

        private readonly IAuthorize _authorize;
        private readonly DocumentRepository _documents;
        private readonly DocumentStore _store;
        private readonly UploadGate _gate;
        private readonly IFileDelivery _delivery;
        private readonly ILogger<DocumentService> _log;

        public DocumentService(IAuthorize authorize, DocumentRepository documents, DocumentStore store, UploadGate gate,
            IFileDelivery delivery, ILogger<DocumentService> log)
        {
            _authorize = authorize;
            _documents = documents;
            _store = store;
            _gate = gate;
            _delivery = delivery;
            _log = log;
        }

        public IReadOnlyList<DocumentListItem> List()
        {
            var me = _authorize.RequireAny(Permission.ViewAllDocuments, Permission.ViewDepartmentDocuments);
            return _documents.List(me.Has(Permission.ViewAllDocuments) ? null : me.Department);
        }

        public IReadOnlyList<DocumentListItem> Search(string term)
        {
            var me = _authorize.RequireAny(Permission.ViewAllDocuments, Permission.ViewDepartmentDocuments);
            return _documents.Search(me.Has(Permission.ViewAllDocuments) ? null : me.Department, term);
        }

        /// <summary>Find, authorize, record, then stream. A missing and a forbidden document get the same answer.</summary>
        public void Download(int documentId)
        {
            var me = _authorize.RequireAny(Permission.ViewAllDocuments, Permission.ViewDepartmentDocuments);
            var doc = Authorized(documentId, "download");
            var content = _store.ReadDocument(doc.StoredName);
            _log.LogInformation("Download {DocumentId} by employee {EmployeeId}: allowed", doc.Id, me.EmployeeId);
            _delivery.Send(content, doc.Title);
        }

        /// <summary>The record behind a share link, if this caller may read it (share links are issued for readable documents only).</summary>
        public DocumentRecord Authorized(int documentId, string action)
        {
            var me = _authorize.RequireAny(Permission.ViewAllDocuments, Permission.ViewDepartmentDocuments);
            var doc = _documents.FindById(documentId);
            if (doc == null || doc.StoredName == null || !CanRead(me, doc))
            {
                _log.LogWarning("Refused {Action} of document {DocumentId} for employee {EmployeeId} ({Department}): {Reason}",
                    action, documentId, me.EmployeeId, me.Department, doc == null ? "no such document" : "outside the caller's scope");
                throw new NotAuthorizedException(NotAvailable);
            }
            return doc;
        }

        public UploadResult Upload(string clientName, byte[] content)
        {
            var me = _authorize.Require(Permission.UploadDocument, "upload");
            var decision = _gate.Check(clientName, content);
            if (!decision.Accepted)
            {
                _log.LogWarning("Upload refused for employee {EmployeeId}: {Check} ({Detail})",
                    me.EmployeeId, decision.FailedCheck, decision.Steps[^1].Detail);
                return new UploadResult(decision, null);
            }

            // quarantine under a generated name, then promote and link to the uploader's department
            var stored = _store.WriteToQuarantine(content);
            try
            {
                _store.Promote(stored);
            }
            catch
            {
                _store.Discard(stored);
                throw;
            }

            var id = _documents.Insert(decision.DisplayName, me.Department, "Confidential", content.LongLength, stored, decision.ContentType, me.EmployeeId);
            _log.LogInformation("Upload accepted: document {DocumentId} for {Department}, stored as {StoredName}", id, me.Department, stored);
            return new UploadResult(decision, id);
        }

        internal static bool CanRead(SessionContext me, DocumentRecord doc)
            => me.Has(Permission.ViewAllDocuments) || (me.Has(Permission.ViewDepartmentDocuments) && doc.Department == me.Department);
    }

    internal static class Format
    {
        public static string Size(long bytes)
            => bytes >= 1024 * 1024
                ? (bytes / 1048576.0).ToString("0.0", CultureInfo.InvariantCulture) + " MB"
                : Math.Max(1, bytes / 1024).ToString(CultureInfo.InvariantCulture) + " KB";

        public static string Amount(double amount)
            => amount <= 0 ? "—" : "€ " + amount.ToString("#,##0", CultureInfo.InvariantCulture);
    }
}
