using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AccessOps.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace AccessOps.Services
{
    /// <summary>An approval request as the repository loads it.</summary>
    public sealed class ApprovalRequest
    {
        public int Id { get; init; }
        public int RequesterId { get; init; }
        public string RequesterName { get; init; }
        public string RequesterDepartment { get; init; }
        public string Subject { get; init; }

        /// <summary>The department the request is charged to - the scope of the approval queue.</summary>
        public string Department { get; init; }

        public double Amount { get; init; }
        public string Status { get; private set; }
        public int? DecidedBy { get; private set; }
        public DateTime? DecidedUtc { get; private set; }
        public string Note { get; private set; }

        internal void SetState(string status, int? decidedBy, DateTime? decidedUtc, string note)
        {
            this.Status = status;
            this.DecidedBy = decidedBy;
            this.DecidedUtc = decidedUtc;
            this.Note = note;
        }

        public void Approve(int approverId, string note, DateTime utc) => Decide("Approved", approverId, note, utc);

        public void Reject(int approverId, string note, DateTime utc) => Decide("Rejected", approverId, note, utc);

        private void Decide(string status, int approverId, string note, DateTime utc)
        {
            if (this.Status != "Pending")
                throw new InvalidOperationException("Only a pending request can be decided.");
            SetState(status, approverId, utc, note);
        }
    }

    /// <summary>What the approvals screen shows for one request.</summary>
    public sealed class ApprovalListItem
    {
        public int Id { get; init; }
        public string Request => "REQ-" + this.Id.ToString(CultureInfo.InvariantCulture);
        public string Requester { get; init; }
        public string Department { get; init; }
        public string Subject { get; init; }
        public string Amount { get; init; }
        public string Status { get; init; }

        /// <summary>A courtesy for the UI. The service re-checks on every call; this is never the control.</summary>
        public bool CanApprove { get; init; }

        /// <summary>"not your report", "no approval rights", or null.</summary>
        public string WhyNot { get; init; }
    }

    /// <summary>
    /// Approval queries and commands. The manager rule lives in the query:
    /// <see cref="GetForApprover"/> returns null when the request was not raised by one of that
    /// manager's direct reports. Commands take the caller's connection and transaction, so the
    /// business change and its audit row commit together.
    /// </summary>
    public sealed class ApprovalRepository
    {
        private const string Select = @"
SELECT a.id, a.requester_id, e.first_name || ' ' || e.last_name, e.department, a.subject, a.department,
       a.amount, a.status, a.decided_by, a.decided_utc, a.note
FROM approval_requests a
JOIN employees e ON e.id = a.requester_id";

        private readonly AccessOpsDatabase _db;

        public ApprovalRepository(AccessOpsDatabase db) => _db = db;

        public AccessOpsDatabase Database => _db;

        public Task<IReadOnlyList<ApprovalRequest>> ListForDepartmentAsync(string department)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = Select + " WHERE a.department = $department ORDER BY a.id DESC";
            cmd.Parameters.AddWithValue("$department", department);
            return Task.FromResult<IReadOnlyList<ApprovalRequest>>(ReadAll(cmd));
        }

        public ApprovalRequest GetForApprover(SqliteConnection connection, SqliteTransaction tx, int requestId, int approverId)
        {
            using var cmd = connection.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = Select + " WHERE a.id = $id AND e.manager_id = $approver";
            cmd.Parameters.AddWithValue("$id", requestId);
            cmd.Parameters.AddWithValue("$approver", approverId);
            return ReadAll(cmd).SingleOrDefault();
        }

        public Task<ApprovalRequest> GetAsync(int requestId)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = Select + " WHERE a.id = $id";
            cmd.Parameters.AddWithValue("$id", requestId);
            return Task.FromResult(ReadAll(cmd).SingleOrDefault());
        }

        public void Save(SqliteConnection connection, SqliteTransaction tx, ApprovalRequest request)
        {
            using var cmd = connection.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"
UPDATE approval_requests SET status = $status, decided_by = $by, decided_utc = $at, note = $note
WHERE id = $id;";
            cmd.Parameters.AddWithValue("$status", request.Status);
            cmd.Parameters.AddWithValue("$by", (object)request.DecidedBy ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$at", request.DecidedUtc?.ToString("O", CultureInfo.InvariantCulture) ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$note", (object)request.Note ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$id", request.Id);
            cmd.ExecuteNonQuery();
        }

        private static List<ApprovalRequest> ReadAll(SqliteCommand cmd)
        {
            var rows = new List<ApprovalRequest>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var request = new ApprovalRequest
                {
                    Id = r.GetInt32(0),
                    RequesterId = r.GetInt32(1),
                    RequesterName = r.GetString(2),
                    RequesterDepartment = r.GetString(3),
                    Subject = r.GetString(4),
                    Department = r.GetString(5),
                    Amount = r.GetDouble(6)
                };
                request.SetState(
                    r.GetString(7),
                    r.IsDBNull(8) ? null : r.GetInt32(8),
                    r.IsDBNull(9) ? null : DateTime.Parse(r.GetString(9), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    r.IsDBNull(10) ? null : r.GetString(10));
                rows.Add(request);
            }
            return rows;
        }
    }

    /// <summary>
    /// Guard, then scope, then act - and account for it: every command takes the
    /// <see cref="CommandContext"/> minted by the event handler, and writes its audit row in the same
    /// transaction as the business change. Refusals are audited too.
    /// </summary>
    public sealed class ApprovalService
    {
        private readonly IAuthorize _authorize;
        private readonly ApprovalRepository _requests;
        private readonly AuditService _audit;
        private readonly ILogger<ApprovalService> _log;

        public ApprovalService(IAuthorize authorize, ApprovalRepository requests, AuditService audit, ILogger<ApprovalService> log)
        {
            _authorize = authorize;
            _requests = requests;
            _audit = audit;
            _log = log;
        }

        /// <summary>The queue of the caller's own department (employees and managers alike).</summary>
        public async Task<IReadOnlyList<ApprovalListItem>> ListAsync()
        {
            var me = _authorize.RequireAny(Permission.SubmitRequest, Permission.ApproveRequest);
            var canApproveAny = me.Has(Permission.ApproveRequest);
            var rows = await _requests.ListForDepartmentAsync(me.Department);

            return rows.Select(r =>
            {
                var mine = me.DirectReports.Contains(r.RequesterId);
                var pending = r.Status == "Pending";
                return new ApprovalListItem
                {
                    Id = r.Id,
                    Requester = r.RequesterName,
                    Department = r.RequesterDepartment,
                    Subject = r.Subject,
                    Amount = Format.Amount(r.Amount),
                    Status = r.Status,
                    CanApprove = pending && canApproveAny && mine,
                    WhyNot = !pending ? null : !canApproveAny ? "no approval rights" : !mine ? "not your report" : null
                };
            }).ToList();
        }

        /// <summary>Returns the audit sequence number of the decision.</summary>
        public Task<long> ApproveAsync(CommandContext ctx, int requestId, string note) => DecideAsync(ctx, requestId, note, approve: true);

        public Task<long> RejectAsync(CommandContext ctx, int requestId, string note) => DecideAsync(ctx, requestId, note, approve: false);

        private Task<long> DecideAsync(CommandContext ctx, int requestId, string note, bool approve)
        {
            var action = approve ? "Approval.Approve" : "Approval.Reject";
            var target = requestId.ToString(CultureInfo.InvariantCulture);

            SessionContext me;
            try
            {
                me = _authorize.Require(Permission.ApproveRequest, "REQ-" + target);
            }
            catch (NotAuthorizedException)
            {
                _audit.Record(ctx, action, "ApprovalRequest", target, "Denied");
                throw;
            }

            using var connection = _requests.Database.Open();
            using var tx = connection.BeginTransaction();

            var request = _requests.GetForApprover(connection, tx, requestId, me.EmployeeId);
            if (request == null)
            {
                tx.Rollback();
                _audit.Record(ctx, action, "ApprovalRequest", target, "Denied");
                throw _authorize.Refuse(Permission.ApproveRequest, "REQ-" + target, "request is outside the approver's reports (or does not exist)");
            }

            try
            {
                if (approve)
                    request.Approve(me.EmployeeId, note, DateTime.UtcNow);
                else
                    request.Reject(me.EmployeeId, note, DateTime.UtcNow);
            }
            catch (InvalidOperationException ex)
            {
                // a failure, not a refusal: the full detail goes to the log under the correlation ID,
                // the user sees one sentence and that ID as the reference
                tx.Rollback();
                _audit.Record(ctx, action, "ApprovalRequest", target, "Failed");
                _log.LogWarning(ex, "Request {RequestId} is {Status}; {Action} not applied", requestId, request.Status, action);
                throw;
            }

            _requests.Save(connection, tx, request);                                         // the business change
            var seq = _audit.Record(connection, tx, ctx, action, "ApprovalRequest", target, "Allowed");  // and its record
            tx.Commit();                                                                     // both, or neither

            _log.LogInformation("Request {RequestId} moved Pending → {Status}", requestId, request.Status);
            _log.LogInformation("Audit entry {Seq} written for {Action}", seq, action);
            return Task.FromResult(seq);
        }
    }
}
