using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>An export waiting for a second person. Requester and approver are always different people.</summary>
    public sealed class ExportApproval
    {
        public int Id { get; set; }
        public string TenantId { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedUtc { get; set; }
        public int RowCount { get; set; }
        public string Reason { get; set; }
        public string ApprovedBy { get; set; }
        public bool Completed { get; set; }

        public override string ToString() => $"export #{Id} · {RowCount} rows · requested by {RequestedBy}";
    }

    public sealed class ExportResult : CommandResult
    {
        private ExportResult(bool succeeded, bool denied, string summary, string correlationId,
            IReadOnlyList<string> errors, ExportApproval pending, int rowsExported)
            : base(succeeded, denied, summary, correlationId, errors)
        {
            Pending = pending;
            RowsExported = rowsExported;
        }

        /// <summary>Set when the export needs a second pair of eyes. Nothing has left the server yet.</summary>
        public ExportApproval Pending { get; }

        public int RowsExported { get; }

        public static ExportResult Exported(int rows, string correlationId)
            => new ExportResult(true, false, $"{rows} rows exported", correlationId, null, null, rows);

        public static ExportResult AwaitingApproval(ExportApproval approval, string correlationId)
            => new ExportResult(false, false, "awaiting approval", correlationId,
                new List<string> { $"{approval.RowCount} rows is above the {ExportService.ApprovalThreshold}-row threshold — export #{approval.Id} is waiting for a second person." },
                approval, 0);

        public static new ExportResult Refused(string reason, string correlationId)
            => new ExportResult(false, true, "denied", correlationId, new List<string> { reason }, null, 0);

        public static new ExportResult Failed(string reason, string correlationId)
            => new ExportResult(false, false, "failed", correlationId, new List<string> { reason }, null, 0);
    }

    /// <summary>
    /// Export, with approval. The service that executes the action is where the permission is demanded — this is
    /// the class the walkthrough's failure path lands in when a wrongly-enabled button is clicked.
    ///
    /// Two controls, not one:
    ///  · <see cref="Permission.ExportData"/> decides who may ask at all;
    ///  · above <see cref="ApprovalThreshold"/> rows, a second person holding <see cref="Permission.ApproveExport"/>
    ///    must release it, and it may not be the requester. Separation of duties is a rule about **people**, so
    ///    it cannot live in the permission matrix — it lives here, next to the command, and it is audited.
    ///
    /// Nothing is written to disk: the "export" produces a row count. Real file handling belongs with the upload
    /// and download items on the hardening checklist.
    /// </summary>
    public sealed class ExportService
    {
        /// <summary>Above this many rows an export is a bulk data movement, and bulk needs a witness.</summary>
        public const int ApprovalThreshold = 25;

        private readonly IWorkOrderRepository _repository;
        private readonly IPermissionService _permissions;
        private readonly IAuditLog _audit;
        private readonly ActivityTrace _trace;

        // Application-scoped on purpose, like AuditLog.Shared: the requester and the approver are two people in
        // two sessions. Locked on every access, tenant-filtered on read.
        private static readonly List<ExportApproval> Pending = new List<ExportApproval>();
        private static readonly object PendingLock = new object();
        private static int _nextId = 3;   // #1 and #2 are in the seeded history

        public ExportService(IWorkOrderRepository repository, IPermissionService permissions, IAuditLog audit, ActivityTrace trace)
        {
            _repository = repository;
            _permissions = permissions;
            _audit = audit;
            _trace = trace;
        }

        /// <summary>Exports still waiting for a second person, for this tenant.</summary>
        public IReadOnlyList<ExportApproval> PendingFor(string tenantId)
        {
            lock (PendingLock)
                return Pending.Where(p => !p.Completed && StringComparer.Ordinal.Equals(p.TenantId, tenantId)).ToList();
        }

        /// <summary>
        /// The walkthrough's path. <c>Demand(ExportData)</c> runs here, where the action executes — so it runs
        /// whether the button was visible, hidden, disabled, re-enabled by a UI bug, or never rendered at all.
        /// </summary>
        public async Task<ExportResult> RequestExportAsync(CommandContext context)
        {
            int rowCount = _repository.ForTenant(context.TenantId).Count;
            _trace?.Service($"ExportService.RequestExportAsync — Demand(ExportData, resourceTenant '{context.TenantId}') before any row is read");

            try
            {
                _permissions.Demand(context, Permission.ExportData, context.TenantId);
            }
            catch (UnauthorizedAccessException ex)
            {
                _trace?.Service("ExportService — refused; nothing was read, nothing left the server");
                return ExportResult.Refused(ex.Message, context.CorrelationId);
            }

            await Task.Delay(150);

            if (rowCount > ApprovalThreshold)
            {
                var approval = new ExportApproval
                {
                    TenantId = context.TenantId,
                    RequestedBy = context.UserId,
                    RequestedUtc = DateTime.UtcNow,
                    RowCount = rowCount,
                    Reason = $"{rowCount} rows > {ApprovalThreshold}-row threshold",
                };
                lock (PendingLock)
                {
                    approval.Id = _nextId++;
                    Pending.Add(approval);
                }

                _audit.Write(context, "ExportData", AuditResult.Pending, $"export #{approval.Id} · {rowCount} rows",
                    $"awaiting a second approver ({approval.Reason})");
                _trace?.Service($"ExportService — export #{approval.Id} held: {approval.Reason}");

                return ExportResult.AwaitingApproval(approval, context.CorrelationId);
            }

            _audit.Write(context, "ExportData", AuditResult.Ok, $"{rowCount} rows", "below the approval threshold");
            return ExportResult.Exported(rowCount, context.CorrelationId);
        }

        /// <summary>
        /// Release a pending export. Two refusals live here, and both are audited: no
        /// <see cref="Permission.ApproveExport"/>, and "you are the person who asked for it".
        /// </summary>
        public async Task<ExportResult> ApproveExportAsync(CommandContext context, int approvalId)
        {
            ExportApproval approval;
            lock (PendingLock)
                approval = Pending.FirstOrDefault(p => p.Id == approvalId && !p.Completed
                                                    && StringComparer.Ordinal.Equals(p.TenantId, context.TenantId));
            if (approval == null)
                return ExportResult.Failed($"Export #{approvalId} is not waiting for approval.", context.CorrelationId);

            _trace?.Service($"ExportService.ApproveExportAsync(#{approval.Id}) — Demand(ApproveExport, resourceTenant '{approval.TenantId}')");

            try
            {
                _permissions.Demand(context, Permission.ApproveExport, approval.TenantId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ExportResult.Refused(ex.Message, context.CorrelationId);
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(approval.RequestedBy, context.UserId))
            {
                // The permission said yes; the rule about people says no. Refused, and recorded as refused.
                _audit.Write(context, "ApproveExport", AuditResult.Denied, $"export #{approval.Id}",
                    "separation of duties: the requester may not approve their own export");
                _trace?.Service("ExportService — separation of duties: requester and approver are the same person");
                return ExportResult.Refused("An export cannot be approved by the person who requested it.", context.CorrelationId);
            }

            await Task.Delay(150);

            lock (PendingLock)
            {
                if (approval.Completed)
                    return ExportResult.Failed($"Export #{approval.Id} was already released by {approval.ApprovedBy}.", context.CorrelationId);

                approval.ApprovedBy = context.UserId;
                approval.Completed = true;
            }

            _audit.Write(context, "ApproveExport", AuditResult.Ok, $"export #{approval.Id}",
                $"released {approval.RowCount} rows requested by {approval.RequestedBy}");
            _audit.Write(context, "ExportData", AuditResult.Ok, $"export #{approval.Id} · {approval.RowCount} rows",
                $"completed after approval by {context.UserId}");

            return ExportResult.Exported(approval.RowCount, context.CorrelationId);
        }
    }
}
