using System.Collections.Generic;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// Records who did what, when. Lifetime: Shared — one thread-safe sink for the whole server. Because
    /// every session shares it, it takes the operator id as a parameter and holds no "current user",
    /// no selected ticket and no session-scoped collaborator (not even the session ILog).
    /// </summary>
    public interface IAuditLogService
    {
        AuditEntry Record(string action, int ticketId, int operatorId, string detail);

        IReadOnlyList<AuditEntry> Recent(int count);

        int Count { get; }
    }
}
