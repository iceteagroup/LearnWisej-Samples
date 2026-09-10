using System.Collections.Generic;
using System.Linq;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// A tiny in-memory ticket table seeded with the demo rows. The fake ticket service uses it as its
    /// whole storage; the production-shaped SqlTicketService uses it as the local stand-in for the
    /// database it would talk to. One table per service instance, so one per session — never static.
    /// </summary>
    public sealed class TicketTable
    {
        private readonly Dictionary<int, Ticket> _rows = new Dictionary<int, Ticket>();

        public TicketTable()
        {
            foreach (var t in SeedData.Tickets())
                _rows[t.Id] = t;
        }

        public int Count => _rows.Count;

        public IReadOnlyList<Ticket> All() => _rows.Values.Select(t => t.Clone()).ToList();

        public Ticket Find(int id) => _rows.TryGetValue(id, out var t) ? t.Clone() : null;

        public Ticket Save(Ticket ticket)
        {
            _rows[ticket.Id] = ticket.Clone();
            return ticket.Clone();
        }
    }
}
