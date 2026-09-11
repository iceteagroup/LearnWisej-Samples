using System;
using System.Collections.Generic;
using EnterpriseOps.Services;

namespace EnterpriseOps.Security
{
    public enum AuditOutcome { Allowed, Denied, Rejected }

    /// <summary>One line per boundary crossing: who asked, from which tenant, for what, and what the server answered.</summary>
    public sealed class AuditEntry
    {
        public DateTime Utc { get; set; }
        public string UserName { get; set; }
        public string TenantId { get; set; }
        public string CorrelationId { get; set; }
        public string CommandName { get; set; }
        public string EntityId { get; set; }
        public AuditOutcome Outcome { get; set; }
        public string Detail { get; set; }
    }

    /// <summary>
    /// Append-only, per session. Every WebMethod call is attributable: user, tenant, correlation id,
    /// command name and outcome — the lesson's "a support engineer can reconstruct what the browser
    /// asked for and what the server answered".
    /// </summary>
    public sealed class AuditLog
    {
        private readonly List<AuditEntry> _entries = new List<AuditEntry>();
        private readonly ActivityTrace _trace;

        public AuditLog(ActivityTrace trace)
        {
            _trace = trace;
        }

        public IReadOnlyList<AuditEntry> Entries => _entries;

        public AuditEntry Record(CommandContext context, string commandName, string entityId, AuditOutcome outcome, string detail)
        {
            var entry = new AuditEntry
            {
                Utc = DateTime.UtcNow,
                UserName = context.UserName,
                TenantId = context.TenantId,
                CorrelationId = context.CorrelationId,
                CommandName = commandName ?? "",
                EntityId = entityId ?? "",
                Outcome = outcome,
                Detail = detail,
            };
            _entries.Add(entry);
            _trace.Audit($"{outcome.ToString().ToUpperInvariant()} user={entry.UserName} tenant={entry.TenantId} corr={entry.CorrelationId} cmd={entry.CommandName} entity={(entry.EntityId.Length == 0 ? "—" : entry.EntityId)} · {detail}");
            return entry;
        }
    }
}
