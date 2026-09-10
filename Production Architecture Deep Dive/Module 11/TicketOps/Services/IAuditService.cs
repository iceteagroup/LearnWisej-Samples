using System;
using System.Collections.Generic;
using TicketOps.Security;

namespace TicketOps.Services
{
    public enum AuditOutcome
    {
        Success,
        Denied,
        Failed
    }

    /// <summary>
    /// One append-only audit record: who, what, which entity, when (server UTC), and how it ended.
    /// <see cref="Detail"/> is a short factual phrase — never a password, a token or a user-typed payload.
    /// </summary>
    public sealed class AuditEntry
    {
        public DateTime TimeUtc { get; init; }
        public string Actor { get; init; }
        public string ActorRoles { get; init; }
        public string Action { get; init; }
        public string Target { get; init; }
        public AuditOutcome Outcome { get; init; }
        public string Detail { get; init; }
    }

    /// <summary>
    /// The audit trail of sensitive actions. Services write to it whether the action succeeded or was
    /// denied — a stream of denials against one account is exactly the signal a reviewer wants to see.
    /// Plain C#: no UI type, so the same entries feed the trace, the in-app list and (in production) a SIEM.
    /// </summary>
    public interface IAuditService
    {
        IReadOnlyList<AuditEntry> Entries { get; }

        event EventHandler<AuditEntry> EntryAdded;

        void Success(IUserContext actor, string action, string target, string detail = null);
        void Denied(IUserContext actor, string action, string target, string reason);
        void Failed(IUserContext actor, string action, string target, string reason);
    }
}
