using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The ticket workflow, depending on <see cref="SessionContext"/>. Because the service is handed
    /// THIS session's context (instead of reaching into a static or reading a control), it can be
    /// unit-tested by passing a hand-made context — no browser, no session, no server required — and
    /// it stays correct under concurrent sessions: session A's tenant can never leak into session B's query.
    /// </summary>
    public sealed class TicketService : ITicketService
    {
        private readonly SessionContext _ctx;
        private readonly ITicketRepository _repository;

        public TicketService(SessionContext ctx, ITicketRepository repository)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync()
        {
            return _repository.GetByTenantAsync(_ctx.Tenant);
        }
    }
}
