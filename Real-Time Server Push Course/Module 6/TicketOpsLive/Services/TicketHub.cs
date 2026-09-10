using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TicketOpsLive.Models;

namespace TicketOpsLive.Services
{
    /// <summary>
    /// The event hub of the lesson: ONE instance for the whole server process, shared by every session.
    ///
    /// Ownership (the architecture change the module is about):
    ///   the page owns its controls · the session owns its context · the GLOBAL SERVICE owns shared data and events.
    ///
    /// What that means in code, and what this class is careful never to do:
    ///   - it stores tickets (values) and nothing else: no MainPage, no control, no IWisejComponent, no session id
    ///     mapped to a page. A "bag of form references" is exactly the anti-pattern the lesson warns about;
    ///   - it NEVER calls Application.Update(). It owns no session, so it has no context to push into. It announces
    ///     that something happened; each subscriber restores its own context and updates its own UI;
    ///   - every access to the list is under <see cref="_sync"/>, and <see cref="GetSnapshot"/> hands out CLONES so
    ///     a session can never mutate global state or enumerate a collection the hub is changing;
    ///   - the event is raised OUTSIDE the lock. Subscribers do real work (they render, they push) and no global
    ///     lock may be held while foreign sessions render — that is how a shared service deadlocks or stalls;
    ///   - a subscriber that throws is logged and skipped: one dead session never breaks the fan-out for the others.
    ///
    /// Cadence note: <see cref="AddOrUpdate"/> hands the event to a thread-pool thread right away (see <see cref="Raise"/>),
    /// one fan-out per publish, in publish order. A burst
    /// therefore has to be throttled by the publisher — see burstButton in MainPage.cs. Where a hub emits faster
    /// than sessions can render, add backpressure (see docs/BroadcastVsTargeted.md).
    ///
    /// Subscribers own their subscription: MainPage subscribes on Load and unsubscribes on ApplicationExit AND on
    /// Disposed (see docs/SubscriptionCleanup.md). "Every subscription must have an unsubscribe" is a course rule.
    /// </summary>
    public sealed class TicketHub
    {
        private static readonly Lazy<TicketHub> _instance =
            new Lazy<TicketHub>(() => new TicketHub(), LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>The single, lazily created, thread-safe instance.</summary>
        public static TicketHub Instance => _instance.Value;

        private readonly object _sync = new object();
        private readonly List<Ticket> _tickets = new List<Ticket>();

        private EventHandler<TicketChangedEventArgs> _ticketChanged;
        private int _subscriberCount;
        private int _eventsPublished;

        /// <summary>The tenants of the demo. Every published ticket belongs to exactly one of them.</summary>
        public static readonly string[] Tenants = { "Contoso", "Northwind" };

        private TicketHub()
        {
            // Seed data: what a session sees the moment it loads, before any event is published.
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
        /// Raised after a ticket was added or updated. It runs on a thread-pool thread (never on the publisher's request thread — see Raise), outside the hub's lock and
        /// OUTSIDE the subscriber's session context — every handler must restore its own context with
        /// Application.Update(context, …) before touching controls.
        ///
        /// The accessors are explicit so the hub can count its subscribers exactly (the demo shows the number, and a
        /// leaked subscription would show up here as a count that never goes down). += / -= are idempotent-safe under
        /// the lock: -= on a handler that is not subscribed leaves the count alone.
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

        /// <summary>How many handlers are subscribed right now. In this demo: one per live, subscribed session.</summary>
        public int SubscriberCount
        {
            get { lock (_sync) return _subscriberCount; }
        }

        /// <summary>How many events the hub has published since the process started (every session sees the same number).</summary>
        public int EventsPublished
        {
            get { lock (_sync) return _eventsPublished; }
        }

        /// <summary>How many tickets the hub holds, all tenants together.</summary>
        public int TicketCount
        {
            get { lock (_sync) return _tickets.Count; }
        }

        /// <summary>
        /// A snapshot of the shared list, newest change first — CLONES, under the lock.
        ///
        /// Bad:    public List&lt;Ticket&gt; Tickets =&gt; _tickets;      (the caller can mutate global state, and it
        ///                                                            enumerates a list the hub may be changing)
        /// Better: this. The ownership boundary is explicit and cheap to reason about.
        /// </summary>
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

        /// <summary>A clone of one ticket, or null. Used by "Escalate selected" to publish from the authoritative copy.</summary>
        public Ticket Find(int id)
        {
            lock (_sync)
            {
                var found = _tickets.FirstOrDefault(t => t.Id == id);
                return found?.Clone();
            }
        }

        /// <summary>
        /// Adds or updates a ticket and announces it. The one write path of the hub.
        ///
        /// Validation first (the failure path of the sample): an invalid ticket is rejected with an ArgumentException
        /// BEFORE the lock, so global state is left exactly as it was and no event is published. The caller catches it
        /// and shows a banner — a bad publish must never corrupt what other sessions see.
        /// </summary>
        /// <param name="ticket">The ticket to store. The hub clones it; the caller keeps its own instance.</param>
        /// <param name="publishedBy">Application.ClientId of the publishing session — metadata only, never a reference.</param>
        /// <param name="changeType">"Escalated" to force the escalation flavour; null lets the hub say Added/Updated.</param>
        /// <returns>The event that was published — the publisher uses it to trace what it just fanned out.</returns>
        public TicketChangedEventArgs AddOrUpdate(Ticket ticket, string publishedBy, string changeType = null)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));
            if (ticket.Id <= 0)
                throw new ArgumentException("Ticket.Id must be greater than zero.", nameof(ticket));
            if (string.IsNullOrWhiteSpace(ticket.Title))
                throw new ArgumentException("Ticket.Title must not be empty.", nameof(ticket));
            if (string.IsNullOrWhiteSpace(ticket.TenantId))
                throw new ArgumentException("Ticket.TenantId must not be empty — the hub cannot route an event without its tenant.", nameof(ticket));

            TicketChangedEventArgs args;

            lock (_sync)
            {
                var existing = _tickets.FirstOrDefault(t => t.Id == ticket.Id);
                bool added = existing == null;

                if (added)
                {
                    _tickets.Insert(0, ticket.Clone());        // a clone: the caller's instance stays the caller's
                }
                else
                {
                    existing.CopyFrom(ticket);
                    _tickets.Remove(existing);
                    _tickets.Insert(0, existing);              // newest change first
                }

                string type = string.IsNullOrEmpty(changeType) ? (added ? "Added" : "Updated") : changeType;
                _eventsPublished++;

                args = new TicketChangedEventArgs
                {
                    Ticket = ticket.Clone(),                   // the event carries a snapshot, never a live reference
                    ChangeType = type,
                    EventId = TicketChangedEventArgs.NewEventId(),
                    PublishedBy = publishedBy,
                    TenantId = ticket.TenantId,
                    Message = BuildMessage(ticket, type)
                };
            }

            // OUTSIDE the lock. Subscribers render and push here; holding the global lock across that work would
            // serialize every session behind the slowest one — and would deadlock the moment a handler called back in.
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
            return $"[{ticket.TenantId}] Ticket {ticket.Id} {verb} · {ticket.Status} · {ticket.Title} · {ticket.Owner} · {ticket.UpdatedAt:HH:mm:ss}";
        }

        /// <summary>
        /// Fans the event out on a THREAD-POOL thread, handler by handler, never on the publisher's request thread.
        ///
        /// Verified with two browser tabs: when the hub invoked the subscribers synchronously on the publishing
        /// session's request thread, the other session's Application.Update(context, …) callback ran while the
        /// publisher's session context was still ambient, and that session's pending control changes were flushed
        /// into the publisher's response — the publisher's trace and notification list showed the other tab's
        /// lines. Raising from a thread that has no session context of its own (like a queue or a file watcher
        /// would) makes every subscriber restore its own context, and the leak is gone. Every handler is isolated:
        /// one that throws (a session that died between the check and the call) is logged and the loop continues.
        /// </summary>
        private void Raise(TicketChangedEventArgs e)
        {
            EventHandler<TicketChangedEventArgs> handlers;
            lock (_sync)
                handlers = _ticketChanged;

            if (handlers == null)
                return;

            System.Threading.Tasks.Task.Run(() =>
            {
                foreach (EventHandler<TicketChangedEventArgs> handler in handlers.GetInvocationList())
                {
                    try
                    {
                        handler(this, e);
                    }
                    catch (Exception ex)
                    {
                        // One broken subscriber must not break the fan-out: log the detail, keep going.
                        Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} TicketHub: subscriber failed while handling event {e.EventId} (ticket {e.Ticket?.Id}, tenant {e.TenantId}): {ex}");
                    }
                }
            });
        }
    }
}
