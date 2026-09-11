using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// The session's door to the shared store. One instance per session (created in AppComposition); the
    /// data behind it is the process-wide, thread-safe <see cref="SharedTicketStore"/>. That split is the
    /// module's point: the <i>connection</i> is per session, the <i>data</i> is shared — like a per-request
    /// DbContext over one database.
    /// </summary>
    public sealed class InMemoryTicketRepository : ITicketRepository
    {
        private readonly SharedTicketStore _store;

        public InMemoryTicketRepository(SharedTicketStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public Task<IReadOnlyList<Ticket>> GetByTenantAsync(string tenant)
        {
            return Task.FromResult(_store.GetByTenant(tenant));
        }
    }
}
