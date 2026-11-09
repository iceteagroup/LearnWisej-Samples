using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace AccessOps.Infrastructure
{
    /// <summary>One confidential file of the catalog: the ID the browser may send and the server-side location.</summary>
    public sealed class ConfidentialFile
    {
        /// <summary>Document ID ("DOC-201"): the only thing the screen ever sends.</summary>
        public string DocumentId { get; init; }

        /// <summary>The tenant that owns the document; the policy compares it with the grant.</summary>
        public string Tenant { get; init; }

        /// <summary>Title shown in the list.</summary>
        public string Title { get; init; }

        /// <summary>Fixture folder under the approved root ("pool-only", "labuser-only", "tenant-b").</summary>
        public string Folder { get; init; }

        /// <summary>File name inside <see cref="Folder"/>. Server side only.</summary>
        public string FileName { get; init; }
    }

    /// <summary>
    /// The catalog rows (table <c>confidential_files</c>) and the one approved root every
    /// confidential file must live under. Default root: <c>AccessOpsData/confidential</c>, created
    /// with its ACLs by <c>tools/Setup-Fixtures.ps1</c>; override with <c>AccessOps:ApprovedRoot</c>.
    /// </summary>
    public sealed class ConfidentialFileStore
    {
        private readonly AccessOpsDatabase _db;

        public ConfidentialFileStore(AccessOpsDatabase db, IConfiguration configuration)
        {
            _db = db;
            var root = configuration["AccessOps:ApprovedRoot"];
            this.ApprovedRoot = Path.GetFullPath(string.IsNullOrEmpty(root) ? Path.Combine(db.DataRoot, "confidential") : root);
        }

        /// <summary>The canonical approved root (no trailing separator).</summary>
        public string ApprovedRoot { get; }

        /// <summary>The catalog row for <paramref name="documentId"/>, or null.</summary>
        public ConfidentialFile Find(string documentId)
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT doc_id, tenant, title, folder, file_name FROM confidential_files WHERE doc_id = $id";
            cmd.Parameters.AddWithValue("$id", documentId ?? "");
            using var r = cmd.ExecuteReader();
            return r.Read() ? Map(r) : null;
        }

        /// <summary>Every catalog row, for the Documents list (IDs, titles and folders; no paths).</summary>
        public IReadOnlyList<ConfidentialFile> List()
        {
            var rows = new List<ConfidentialFile>();
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT doc_id, tenant, title, folder, file_name FROM confidential_files ORDER BY doc_id";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                rows.Add(Map(r));
            return rows;
        }

        private static ConfidentialFile Map(Microsoft.Data.Sqlite.SqliteDataReader r) => new ConfidentialFile
        {
            DocumentId = r.GetString(0),
            Tenant = r.GetString(1),
            Title = r.GetString(2),
            Folder = r.GetString(3),
            FileName = r.GetString(4)
        };
    }
}
