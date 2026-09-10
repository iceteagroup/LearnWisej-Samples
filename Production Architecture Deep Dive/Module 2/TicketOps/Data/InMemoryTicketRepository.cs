using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Data
{
    /// <summary>
    /// The session's door to the shared store. One instance per session (created in AppComposition) so it
    /// can log into <b>this</b> session's trace; the data behind it is the process-wide, thread-safe
    /// <see cref="SharedTicketStore"/>. That split is the module's point: the <i>connection</i> is per
    /// session, the <i>data</i> is shared — like a per-request DbContext over one database.
    /// </summary>
    public sealed class InMemoryTicketRepository : ITicketRepository
    {
        private readonly SharedTicketStore _store;
        private readonly ILog _log;

        public InMemoryTicketRepository(SharedTicketStore store, ILog log)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Task<IReadOnlyList<Ticket>> GetByTenantAsync(string tenant)
        {
            var rows = _store.GetByTenant(tenant);
            _log.Info(LogLayer.Data, "InMemoryTicketRepository.GetByTenantAsync",
                $"SELECT * FROM Tickets WHERE Tenant='{tenant}' → {rows.Count} rows (shared store, {_store.Count} total)");
            return Task.FromResult(rows);
        }

        public Task<Ticket> UpsertAsync(Ticket ticket)
        {
            if (ticket == null) throw new ArgumentNullException(nameof(ticket));
            var saved = _store.Upsert(ticket);
            _log.Info(LogLayer.Data, "InMemoryTicketRepository.UpsertAsync",
                $"INSERT INTO Tickets → #{saved.Id} written for tenant {saved.Tenant} (shared store, {_store.Count} total)");
            return Task.FromResult(saved);
        }
    }
}
