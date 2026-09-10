using System.Collections.Generic;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The server-side record of what users did. Interop rule: an audit entry is written by the server
    /// after it has confirmed the action — never by (or on the word of) JavaScript alone.
    /// </summary>
    public interface IAuditLogService
    {
        void Record(string action, int workOrderId, SessionUser user);

        IReadOnlyList<AuditEntry> Entries { get; }
    }
}
