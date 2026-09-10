using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// The read side the screen uses on the request thread. It is called while the import task is writing
    /// to the same repository, which is why the repository — not this class — owns the lock: the service
    /// has no state of its own to protect.
    /// </summary>
    public sealed class TicketService : ITicketService
    {
        private readonly ITicketRepository _repository;
        private readonly ILog _log;

        public TicketService(ITicketRepository repository, ILog log)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync()
        {
            _log.Info(LogLayer.Service, "TicketService.GetOpenTicketsAsync", "→ ITicketRepository.GetAllAsync()");
            var all = await _repository.GetAllAsync();
            return all.Where(t => t.Status != TicketStatus.Closed)
                      .OrderByDescending(t => t.Priority)
                      .ThenBy(t => t.Id)
                      .ToList();
        }

        public async Task<int> CountAsync()
        {
            _log.Info(LogLayer.Service, "TicketService.CountAsync", "→ ITicketRepository.CountAsync() (request thread, store may be mid-import)");
            return await _repository.CountAsync();
        }
    }
}
