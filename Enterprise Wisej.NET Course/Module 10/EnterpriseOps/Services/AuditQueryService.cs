using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>What the user asked to see. "any" means no filter on that column.</summary>
    public sealed class AuditFilter
    {
        public const string Any = "any";

        public string UserId { get; set; } = Any;
        public string Action { get; set; } = Any;
        public string Result { get; set; } = Any;

        public override string ToString() => $"user={UserId} permission={Action} result={Result}";
    }

    /// <summary>One row of dgvAudit. A projection: the grid never binds to <see cref="AuditEntry"/> itself.</summary>
    public sealed class AuditRow
    {
        public string Time { get; set; }
        public string User { get; set; }
        public string Tenant { get; set; }
        public string Action { get; set; }
        public string Result { get; set; }
        public string Detail { get; set; }
        public bool IsDenied { get; set; }
    }

    public sealed class AuditQueryResult : CommandResult
    {
        private AuditQueryResult(bool succeeded, string summary, string correlationId, IReadOnlyList<string> errors,
            IReadOnlyList<AuditRow> rows, bool wholeTenant, int totalBeforeFilter)
            : base(succeeded, false, summary, correlationId, errors)
        {
            Rows = rows ?? new List<AuditRow>();
            WholeTenant = wholeTenant;
            TotalBeforeFilter = totalBeforeFilter;
        }

        public IReadOnlyList<AuditRow> Rows { get; }

        /// <summary>True when the caller holds ViewAuditLog; false when they are only seeing their own entries.</summary>
        public bool WholeTenant { get; }

        public int TotalBeforeFilter { get; }

        public static AuditQueryResult Loaded(IReadOnlyList<AuditRow> rows, bool wholeTenant, int total, string correlationId)
            => new AuditQueryResult(true, $"{rows.Count} of {total} entries", correlationId, null, rows, wholeTenant, total);
    }

    /// <summary>
    /// Reading the audit log. The trail is a sensitive record in its own right, so the read is authorized too —
    /// with a deliberate nuance the lab explains:
    ///
    ///  · <see cref="Permission.ViewAuditLog"/> ⇒ every entry of the caller's tenant (managers, admins, auditors);
    ///  · without it ⇒ **the caller's own entries only**. Being able to see the security events recorded against
    ///    your own account needs no privilege, and it is why the walkthrough's technician can watch their denial
    ///    land in the grid.
    ///
    /// In both cases the tenant filter is absolute: <see cref="AuditLog.Snapshot"/> never returns another
    /// tenant's rows, so there is no query this screen could build that would leak across the boundary.
    /// </summary>
    public sealed class AuditQueryService
    {
        private readonly IAuditLog _audit;
        private readonly IPermissionService _permissions;
        private readonly ActivityTrace _trace;

        public AuditQueryService(IAuditLog audit, IPermissionService permissions, ActivityTrace trace)
        {
            _audit = audit;
            _permissions = permissions;
            _trace = trace;
        }

        public async Task<AuditQueryResult> QueryAsync(CommandContext context, AuditFilter filter)
        {
            filter = filter ?? new AuditFilter();

            // Has, not Demand: the question decides how much of the trail the caller sees, and asking it is not
            // itself an attempt at anything. The narrower answer is still a valid one, so nothing is refused.
            bool wholeTenant = _permissions.Has(context, Permission.ViewAuditLog);
            _trace?.Service($"AuditQueryService.QueryAsync — Has(ViewAuditLog)={wholeTenant} → scope: {(wholeTenant ? "tenant" : "own entries")}; filter {filter}");

            await Task.Delay(60);

            var entries = _audit.Snapshot(context.TenantId);
            if (!wholeTenant)
                entries = entries.Where(e => StringComparer.OrdinalIgnoreCase.Equals(e.UserId, context.UserId)).ToList();

            int total = entries.Count;

            var rows = entries
                .Where(e => filter.UserId == AuditFilter.Any || StringComparer.OrdinalIgnoreCase.Equals(e.UserId, filter.UserId))
                .Where(e => filter.Action == AuditFilter.Any || StringComparer.OrdinalIgnoreCase.Equals(e.Action, filter.Action))
                .Where(e => filter.Result == AuditFilter.Any || string.Equals(e.Result.ToString(), filter.Result, StringComparison.OrdinalIgnoreCase))
                .Select(Project)
                .ToList();

            _trace?.Audit($"{rows.Count} of {total} entries returned for {context.UserId}@{context.TenantId} (never another tenant's)");
            return AuditQueryResult.Loaded(rows, wholeTenant, total, context.CorrelationId);
        }

        /// <summary>Distinct values for the three filter combos, from the entries this caller may see.</summary>
        public (List<string> Users, List<string> Actions) FilterValues(CommandContext context)
        {
            var entries = _audit.Snapshot(context.TenantId);
            if (!_permissions.Has(context, Permission.ViewAuditLog))
                entries = entries.Where(e => StringComparer.OrdinalIgnoreCase.Equals(e.UserId, context.UserId)).ToList();

            return (
                entries.Select(e => e.UserId).Distinct().OrderBy(u => u, StringComparer.Ordinal).ToList(),
                entries.Select(e => e.Action).Distinct().OrderBy(a => a, StringComparer.Ordinal).ToList());
        }

        private static AuditRow Project(AuditEntry e) => new AuditRow
        {
            Time = e.Time,
            User = e.UserId,
            Tenant = e.TenantId,
            Action = e.Action,
            Result = e.Result == AuditResult.Ok ? "OK" : e.Result.ToString().ToUpperInvariant(),
            Detail = string.IsNullOrEmpty(e.Target) ? $"{e.Detail} · corr {e.CorrelationId}" : $"{e.Target} — {e.Detail} · corr {e.CorrelationId}",
            IsDenied = e.Result == AuditResult.Denied,
        };
    }
}
