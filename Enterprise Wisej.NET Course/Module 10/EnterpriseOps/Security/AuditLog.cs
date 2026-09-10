using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Security
{
    /// <summary>How the attempt ended. Denials matter as much as successes — they are how probing becomes visible.</summary>
    public enum AuditResult
    {
        /// <summary>The caller was allowed and the command ran.</summary>
        Ok,

        /// <summary>The caller was refused by the tenant guard or the permission store. Nothing happened.</summary>
        Denied,

        /// <summary>The caller was allowed, but the command failed for another reason.</summary>
        Failed,

        /// <summary>The command is waiting for a second person (an export above the threshold).</summary>
        Pending
    }

    /// <summary>
    /// One line of the trail: who did what, to which record, in which tenant, when, and how it ended.
    /// Immutable — an audit entry is never edited, only appended.
    /// </summary>
    public sealed class AuditEntry
    {
        public DateTime AtUtc { get; init; }
        public string UserId { get; init; }
        public string TenantId { get; init; }

        /// <summary>The permission that was demanded, or the command name for a non-permission event (SignIn…).</summary>
        public string Action { get; init; }

        public AuditResult Result { get; init; }

        /// <summary>The record the command touched: "WO-1042", "export #3", or empty.</summary>
        public string Target { get; init; }

        /// <summary>The named reason and the values that matter — never a token, a password or a full record.</summary>
        public string Detail { get; init; }

        /// <summary>Ties this entry to the trace lines and (Module 11) the structured logs of the same command.</summary>
        public string CorrelationId { get; init; }

        public string Time => AtUtc.ToLocalTime().ToString("HH:mm:ss");

        public override string ToString()
            => $"{Time} {UserId}@{TenantId} {Action} {Result} {Target} — {Detail} (corr {CorrelationId})";
    }

    public interface IAuditLog
    {
        void Write(CommandContext context, string action, AuditResult result, string target, string detail);

        /// <summary>Entries for one tenant, newest first. The trail is shared; a view of it never is.</summary>
        IReadOnlyList<AuditEntry> Snapshot(string tenantId);
    }

    /// <summary>
    /// The append-only audit log — the in-memory stand-in for an audit table.
    ///
    /// It is **application-scoped on purpose**: an audit that lived in one session would vanish with the session,
    /// which is the opposite of what an audit is for. Because it is shared by every session in the process, every
    /// write goes through one lock and every read is filtered by tenant.
    ///
    /// Three properties the lab cares about, and a production system needs:
    ///  · append-only — <see cref="Write"/> adds, nothing updates or deletes;
    ///  · written by the service that executes the command, inside the same operation, so an action cannot
    ///    succeed without being recorded;
    ///  · denials are recorded too, with the named reason.
    ///
    /// What is missing here and required in production: durable storage, a hash chain or signed sequence number
    /// so tampering is detectable, and a retention policy. The hardening checklist says so.
    /// </summary>
    public sealed class AuditLog : IAuditLog
    {
        /// <summary>
        /// Shared across sessions, like the audit table it stands in for. Never put per-user state in a static;
        /// this is the exception that proves the rule, and it is documented, locked and tenant-filtered on read.
        /// </summary>
        public static readonly AuditLog Shared = new AuditLog();

        private readonly object _gate = new object();
        private readonly List<AuditEntry> _entries = new List<AuditEntry>();

        public void Write(CommandContext context, string action, AuditResult result, string target, string detail)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var entry = new AuditEntry
            {
                AtUtc = DateTime.UtcNow,
                UserId = context.UserId,
                TenantId = context.TenantId,
                Action = action,
                Result = result,
                Target = target ?? string.Empty,
                Detail = detail ?? string.Empty,
                CorrelationId = context.CorrelationId,
            };

            lock (_gate)
                _entries.Add(entry);
        }

        /// <summary>Used once at start-up to give the screen a believable history. Not reachable from the UI.</summary>
        internal void Seed(AuditEntry entry)
        {
            lock (_gate)
                _entries.Add(entry);
        }

        public IReadOnlyList<AuditEntry> Snapshot(string tenantId)
        {
            lock (_gate)
                return _entries
                    .Where(e => StringComparer.Ordinal.Equals(e.TenantId, tenantId))
                    .OrderByDescending(e => e.AtUtc)
                    .ToList();
        }

        public int Count
        {
            get { lock (_gate) return _entries.Count; }
        }
    }
}
