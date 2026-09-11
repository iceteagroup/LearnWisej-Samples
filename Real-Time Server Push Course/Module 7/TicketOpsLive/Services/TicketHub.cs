using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketOpsLive.Models;

namespace TicketOpsLive.Services
{
    /// <summary>
    /// The event hub: one instance for the whole server process, shared by every session.
    ///
    ///   - it stores tickets (values) only: no page, no control, no application context;
    ///   - it never calls Application.Update(): it owns no session. It announces what happened and each
    ///     subscriber restores its own context and updates its own UI;
    ///   - every access to the list is under <see cref="_sync"/>, and <see cref="GetSnapshot"/> hands out clones;
    ///   - the event is raised outside the lock, on a thread-pool thread, and a subscriber that throws is
    ///     logged and skipped so one dead session never breaks the fan-out for the others.
    /// </summary>
    public sealed class TicketHub
    {
        private static readonly Lazy<TicketHub> _instance =
            new Lazy<TicketHub>(() => new TicketHub(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static TicketHub Instance => _instance.Value;

        /// <summary>The tenants of the demo. Every published ticket belongs to exactly one of them.</summary>
        public static readonly string[] Tenants = { "Contoso", "Northwind" };

        private readonly object _sync = new object();
        private readonly List<Ticket> _tickets = new List<Ticket>();

        private EventHandler<TicketChangedEventArgs> _ticketChanged;
        private int _subscriberCount;

        private TicketHub()
        {
            Seed(4801, "Contoso", "VPN drops every 20 minutes", "Fabrikam Ltd", "dispatcher", TicketStatus.New);
            Seed(4822, "Contoso", "Invoice batch stuck in queue", "Fabrikam Ltd", "s.oliveira", TicketStatus.Assigned);
            Seed(4830, "Contoso", "Badge reader offline in B2", "Contoso Facilities", "m.keller", TicketStatus.Waiting);
            Seed(9001, "Northwind", "Warehouse scanner not syncing", "Northwind Traders", "dispatcher", TicketStatus.New);
            Seed(9014, "Northwind", "Nightly price import failed", "Northwind Traders", "a.rossi", TicketStatus.Assigned);
            Seed(9022, "Northwind", "Customer portal login loop", "Northwind Retail", "j.novak", TicketStatus.Resolved);
        }

        private void Seed(int id, string tenant, string title, string customer, string owner, TicketStatus status)
        {
            _tickets.Add(new Ticket
            {
                Id = id,
                TenantId = tenant,
                Title = title,
                Customer = customer,
                Owner = owner,
                Status = status,
                UpdatedAt = DateTime.Now
            });
        }

        /// <summary>
        /// Raised after a ticket was added or updated, on a thread-pool thread and outside any session context.
        /// The accessors count the subscribers so the health panel can report active subscriptions.
        /// </summary>
        public event EventHandler<TicketChangedEventArgs> TicketChanged
        {
            add
            {
                if (value == null)
                    return;
                lock (_sync)
                {
                    _ticketChanged += value;
                    _subscriberCount++;
                }
            }
            remove
            {
                if (value == null)
                    return;
                lock (_sync)
                {
                    var before = _ticketChanged;
                    _ticketChanged -= value;
                    if (!ReferenceEquals(before, _ticketChanged) && _subscriberCount > 0)
                        _subscriberCount--;
                }
            }
        }

        /// <summary>How many handlers are subscribed right now: one per live, subscribed session.</summary>
        public int SubscriberCount
        {
            get { lock (_sync) return _subscriberCount; }
        }

        /// <summary>A snapshot of the shared list, newest change first — clones, taken under the lock.</summary>
        public IReadOnlyList<Ticket> GetSnapshot()
        {
            lock (_sync)
                return _tickets.Select(t => t.Clone()).ToList();
        }

        /// <summary>The next free ticket id for a tenant (Contoso ids stay in the 4000s, Northwind in the 9000s).</summary>
        public int NextTicketId(string tenantId)
        {
            lock (_sync)
            {
                int baseId = string.Equals(tenantId, "Northwind", StringComparison.Ordinal) ? 9000 : 4000;
                int highest = baseId;
                foreach (var t in _tickets)
                {
                    if (t.Id >= baseId && t.Id < baseId + 1000 && t.Id > highest)
                        highest = t.Id;
                }
                return highest + 1;
            }
        }

        /// <summary>
        /// Adds or updates a ticket and announces it. An invalid ticket is rejected before the lock, so the shared
        /// state is left as it was and no event is published.
        /// </summary>
        /// <param name="ticket">The ticket to store. The hub clones it; the caller keeps its own instance.</param>
        /// <param name="changeType">"Escalated" to force the escalation event; null lets the hub say Added/Updated.</param>
        public TicketChangedEventArgs AddOrUpdate(Ticket ticket, string changeType = null)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));
            if (ticket.Id <= 0)
                throw new ArgumentException("Ticket.Id must be greater than zero.", nameof(ticket));
            if (string.IsNullOrWhiteSpace(ticket.Title))
                throw new ArgumentException("Ticket.Title must not be empty.", nameof(ticket));
            if (string.IsNullOrWhiteSpace(ticket.TenantId))
                throw new ArgumentException("Ticket.TenantId must not be empty.", nameof(ticket));

            TicketChangedEventArgs args;

            lock (_sync)
            {
                var existing = _tickets.FirstOrDefault(t => t.Id == ticket.Id);
                bool added = existing == null;

                if (added)
                {
                    _tickets.Insert(0, ticket.Clone());
                }
                else
                {
                    existing.CopyFrom(ticket);
                    _tickets.Remove(existing);
                    _tickets.Insert(0, existing);              // newest change first
                }

                string type = string.IsNullOrEmpty(changeType) ? (added ? "Added" : "Updated") : changeType;

                args = new TicketChangedEventArgs
                {
                    Ticket = ticket.Clone(),                   // the event carries a snapshot, never a live reference
                    ChangeType = type,
                    TenantId = ticket.TenantId,
                    Message = BuildMessage(ticket, type)
                };
            }

            Raise(args);
            return args;
        }

        private static string BuildMessage(Ticket ticket, string changeType)
        {
            string verb = changeType switch
            {
                "Added" => "opened",
                "Escalated" => "ESCALATED",
                _ => "updated",
            };
            return $"Ticket {ticket.Id} {verb} · {ticket.Status} · {ticket.Title} · {ticket.Owner}";
        }

        /// <summary>
        /// Fans the event out on a thread-pool thread, never on the publisher's request thread: raised on the
        /// publisher's thread, another session's Application.Update(context, …) ran while the publisher's context
        /// was ambient and its control changes leaked into the publisher's response. From a thread with no session
        /// context every subscriber restores its own. Each handler is isolated: one that throws is logged.
        /// </summary>
        private void Raise(TicketChangedEventArgs e)
        {
            EventHandler<TicketChangedEventArgs> handlers;
            lock (_sync)
                handlers = _ticketChanged;

            if (handlers == null)
                return;

            Task.Run(() =>
            {
                foreach (EventHandler<TicketChangedEventArgs> handler in handlers.GetInvocationList())
                {
                    try
                    {
                        handler(this, e);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} TicketHub: subscriber failed while handling ticket {e.Ticket?.Id} (tenant {e.TenantId}): {ex}");
                    }
                }
            });
        }
    }
}
