using System;
using TicketOps.Infrastructure;

namespace TicketOps.Data
{
    /// <summary>
    /// Raised by the data layer when the store is unreachable. Its message is deliberately internal
    /// (host names, table names): it belongs in the log, and the screen must not show it.
    /// </summary>
    public sealed class DataOutageException : Exception
    {
        public DataOutageException(string message) : base(message) { }
    }

    /// <summary>
    /// The lab's outage switch, registered as a Session service so one browser tab can break its own
    /// data store without touching another tab's. Both ticket implementations (fake and production-shaped)
    /// ask it before touching data, so the error path looks the same in both profiles.
    /// </summary>
    public sealed class DataStoreHealth
    {
        public bool SimulateOutage { get; set; }

        /// <summary>Throws like a real driver would when the outage switch is on; logs the internal detail first.</summary>
        public void EnsureAvailable(ILog log, string source, string statement)
        {
            if (!SimulateOutage)
                return;

            // What a real driver would say — and exactly what must not reach the user.
            log?.Error(LogLayer.Data, source, null,
                $"outage: {statement} failed — timeout connecting to sql01:1433 (TicketOps.dbo.Tickets)");
            throw new DataOutageException("Timeout connecting to sql01:1433 while executing: " + statement);
        }
    }
}
