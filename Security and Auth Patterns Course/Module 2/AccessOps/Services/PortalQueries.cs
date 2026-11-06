using System.Collections.Generic;
using System.Globalization;
using AccessOps.Infrastructure;

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

    public sealed class ApprovalListItem
    {
        public int Id { get; init; }
        public string Request => "REQ-" + this.Id.ToString(CultureInfo.InvariantCulture);
        public string Requester { get; init; }
        public string Subject { get; init; }
        public string Department { get; init; }
        public string Amount { get; init; }
        public string Status { get; init; }
    }

    /// <summary>
    /// The document list and the approval queue as Module 2 leaves them: every row to every
    /// signed-in employee. There is no authorization here yet - that is exactly the gap Module 3
    /// closes with PermissionService and the record-level rules.
    /// </summary>
    public sealed class PortalQueries
    {
        private readonly AccessOpsDatabase _db;

        public PortalQueries(AccessOpsDatabase db) => _db = db;

        public int CountPendingApprovals() => Count("SELECT COUNT(*) FROM approval_requests WHERE status = 'Pending'");

        public int CountDocuments() => Count("SELECT COUNT(*) FROM documents");

        public IReadOnlyList<DocumentListItem> ListDocuments()
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT id, title, department, classification, size_bytes, updated_utc FROM documents ORDER BY updated_utc DESC, id";
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
                    Size = FormatSize(r.GetInt64(4)),
                    Updated = r.GetString(5)
                });
            }
            return rows;
        }

        public IReadOnlyList<ApprovalListItem> ListApprovals()
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT a.id, e.first_name || ' ' || e.last_name, a.subject, a.department, a.amount, a.status
FROM approval_requests a JOIN employees e ON e.id = a.requester_id
ORDER BY a.id DESC";
            var rows = new List<ApprovalListItem>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                rows.Add(new ApprovalListItem
                {
                    Id = r.GetInt32(0),
                    Requester = r.GetString(1),
                    Subject = r.GetString(2),
                    Department = r.GetString(3),
                    Amount = FormatAmount(r.GetDouble(4)),
                    Status = r.GetString(5)
                });
            }
            return rows;
        }

        internal static string FormatSize(long bytes)
            => bytes >= 1024 * 1024
                ? (bytes / 1048576.0).ToString("0.0", CultureInfo.InvariantCulture) + " MB"
                : (bytes / 1024).ToString(CultureInfo.InvariantCulture) + " KB";

        internal static string FormatAmount(double amount)
            => amount <= 0 ? "—" : "€ " + amount.ToString("#,##0", CultureInfo.InvariantCulture);

        private int Count(string sql)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            return System.Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
