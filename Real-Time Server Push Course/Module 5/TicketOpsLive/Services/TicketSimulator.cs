using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using TicketOpsLive.Models;
using Wisej.Web;

namespace TicketOpsLive.Services
{
    /// <summary>
    /// The simulated ticket feed of Module 5 — a stand-in for the service that would really watch a queue,
    /// a database or a message bus.
    ///
    /// It is <b>session-owned</b>: MainPage creates one instance in its constructor and keeps it in an instance
    /// field. Nothing here is static, so one browser tab can never start, stop or corrupt another tab's feed,
    /// and the simulator never holds a reference to a control — it only raises events, and the page decides in
    /// which context to apply them (<c>Application.Update(this, () =&gt; ApplyTicketEvent(e))</c>).
    ///
    /// The simulator owns the authoritative ticket <i>records</i>; the page owns its own bound copies. That is
    /// the shape of the real thing: the service is the source of truth, every session keeps a bound projection
    /// of it, and the two are connected by events — never by a shared list.
    /// </summary>
    public sealed class TicketSimulator
    {
        private static readonly string[] Titles =
        {
            "VPN drops every 20 minutes",
            "Invoice export fails with a timeout",
            "Login loop after password reset",
            "Printer queue stuck on floor 3",
            "Report shows yesterday's totals",
            "Mobile app crashes on photo upload",
            "SSO certificate expiry warning",
            "Order sync stopped at 14:02",
            "Nightly batch retried four times",
            "Email notifications not delivered",
            "Dashboard tiles blank after update",
            "License seat count is wrong",
            "API returns 429 during bulk import",
            "Search index missing new records",
            "Two-factor SMS never arrives",
            "Barcode scanner offline in branch 7",
            "Backup window exceeded by two hours",
            "Customer portal slow after 17:00",
            "Payment webhook delivered twice",
            "Time zone off by one hour in export",
        };

        private static readonly string[] Customers =
        {
            "Northwind Logistics",
            "Contoso Manufacturing",
            "Fabrikam Retail",
            "Adventure Works",
            "Tailspin Toys",
            "Litware Insurance",
            "Proseware Health",
            "Wide World Importers",
            "Blue Yonder Airlines",
            "Coho Vineyard",
        };

        private static readonly string[] Owners =
        {
            "unassigned",
            "M. Feldman",
            "A. Rossi",
            "K. Brandt",
            "S. Okafor",
            "L. Duarte",
            "J. Petrov",
            "R. Nakamura",
        };

        /// <summary>Tells the simulator whether the page that owns it is still alive (MainPage passes () =&gt; !IsDisposed).</summary>
        private readonly Func<bool> _isPageAlive;

        /// <summary>Guards <see cref="_records"/>: the feed task and the request thread both reach it.</summary>
        private readonly object _gate = new object();

        private readonly List<Ticket> _records = new List<Ticket>();
        private readonly Random _random = new Random();

        private volatile bool _running;
        private int _nextId = 4818;
        private int _corruptKind;

        public TicketSimulator(Func<bool> isPageAlive)
        {
            _isPageAlive = isPageAlive ?? (() => true);
        }

        /// <summary>
        /// One ticket was added or changed. Raised on the <b>task thread</b> by <see cref="StartNewTickets"/> and on
        /// the request thread by <see cref="RandomizeStatuses"/> — the page handles both the same way, inside
        /// <c>Application.Update(context, callback)</c>.
        /// </summary>
        public event EventHandler<TicketChangedEventArgs> TicketChanged;

        /// <summary>
        /// The completion callback of the feed task: raised from its <c>finally</c> block whatever happened
        /// (completed, stopped, page disposed, faulted) so the page can restore its buttons exactly once.
        /// </summary>
        public event EventHandler<FeedStoppedEventArgs> FeedStopped;

        /// <summary>True while the "new ticket every second" task is running.</summary>
        public bool IsRunning => _running;

        /// <summary>How many TicketChanged events this simulator has raised (valid ones only).</summary>
        public int EventsRaised { get; private set; }

        /// <summary>How many ticket records the simulator holds (its own source of truth, not the page's list).</summary>
        public int RecordCount
        {
            get { lock (_gate) return _records.Count; }
        }

        #region Seeding — so the grid is never empty on Load

        /// <summary>
        /// Creates <paramref name="count"/> records and returns snapshots of them, oldest first. The page inserts
        /// each snapshot at position 0 of its bound list, so the newest ticket ends up on top.
        /// </summary>
        public IList<Ticket> Seed(int count)
        {
            var seeded = new List<Ticket>();
            lock (_gate)
            {
                for (int i = 0; i < count; i++)
                {
                    Ticket record = CreateRecord(i == 0 ? TicketStatus.Escalated : RandomStatus());
                    _records.Add(record);
                    seeded.Add(Snapshot(record));
                }
            }
            return seeded;
        }

        #endregion

        #region Progress path — a new ticket every second for 20 seconds (Application.StartTask)

        /// <summary>
        /// Starts the feed: one new ticket per <paramref name="everyMs"/> milliseconds, <paramref name="count"/> times.
        /// Returns false when a feed is already running — a second click must never start a competing loop.
        /// </summary>
        public bool StartNewTickets(int count = 20, int everyMs = 1000)
        {
            if (_running)
                return false;

            _running = true;

            // StartTask keeps THIS session's context on the new thread. The task itself never touches a control:
            // it raises TicketChanged, and the page applies the change inside Application.Update(page, callback).
            Application.StartTask(() => NewTicketLoop(count, everyMs));
            return true;
        }

        /// <summary>Cooperative stop. The loop checks the flag before every ticket and exits by itself.</summary>
        public void Stop()
        {
            _running = false;
        }

        private void NewTicketLoop(int count, int everyMs)
        {
            int raised = 0;
            bool faulted = false;
            string reason = null;

            try
            {
                for (int i = 1; i <= count; i++)
                {
                    if (!_running)
                    {
                        reason = $"stopped by operator after {raised} of {count} tickets";
                        break;
                    }
                    if (!_isPageAlive())
                    {
                        // The page went away (tab closed, session ended): stop immediately, push nothing.
                        reason = $"page disposed after {raised} of {count} tickets";
                        break;
                    }

                    Ticket record;
                    lock (_gate)
                    {
                        record = CreateRecord(TicketStatus.New);
                        _records.Add(record);
                    }

                    raised++;
                    Raise(new TicketChangedEventArgs
                    {
                        Ticket = Snapshot(record),
                        ChangeType = "Added",
                        Message = $"#{record.Id} {record.Title} · {record.Customer}",
                    });

                    if (i < count)
                        Thread.Sleep(everyMs);          // cadence: one ticket per second, never a tight loop
                }

                if (reason == null)
                    reason = $"completed — {raised} tickets, one every {everyMs} ms";
            }
            catch (Exception ex)
            {
                // A background task must never fail silently: the detail goes to the server log, the page only
                // gets a safe message through FeedStopped.
                faulted = true;
                reason = "fault: " + ex.Message;
                Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} ticket feed failed for client {Application.ClientId}: {ex}");
            }
            finally
            {
                _running = false;

                // The completion callback: the page restores its buttons and ends the polling fallback here,
                // whatever happened above.
                var args = new FeedStoppedEventArgs
                {
                    Operation = "StartNewTickets",
                    Reason = reason,
                    Faulted = faulted,
                    EventsRaised = raised,
                };

                try
                {
                    FeedStopped?.Invoke(this, args);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} FeedStopped handler failed: {ex}");
                }
            }
        }

        #endregion

        #region Success path — randomize a few statuses, synchronously, inside the request

        /// <summary>
        /// Changes the status (and sometimes the owner) of <paramref name="count"/> random existing records and
        /// raises one TicketChanged per record. This runs <b>in the request</b> that handled the click, so the
        /// resulting UI changes travel back with that response — no push is needed for them.
        /// Returns the number of records that actually changed.
        /// </summary>
        public int RandomizeStatuses(int count = 3)
        {
            var changes = new List<TicketChangedEventArgs>();

            lock (_gate)
            {
                if (_records.Count == 0)
                    return 0;

                int wanted = Math.Min(count, _records.Count);
                var picked = new HashSet<int>();
                int guard = 0;

                while (picked.Count < wanted && guard++ < 200)
                {
                    int index = _random.Next(_records.Count);
                    if (!picked.Add(index))
                        continue;

                    Ticket record = _records[index];
                    record.Status = NextStatus(record.Status);
                    if (record.Status != TicketStatus.New && record.Owner == "unassigned")
                        record.Owner = Owners[_random.Next(1, Owners.Length)];
                    record.UpdatedAt = DateTime.Now;

                    changes.Add(new TicketChangedEventArgs
                    {
                        Ticket = Snapshot(record),
                        ChangeType = "Updated",
                        Message = $"#{record.Id} Status → {record.Status}",
                    });
                }
            }

            // Raise outside the lock: the page's handler runs arbitrary UI code and must never hold the data lock.
            foreach (TicketChangedEventArgs change in changes)
                Raise(change);

            return changes.Count;
        }

        #endregion

        #region Failure path — a malformed event the page has to reject

        /// <summary>
        /// Raises an event the page must refuse: alternately one with no Ticket at all and one with an invalid Id.
        /// Nothing is changed in the records — the point is that <c>ApplyTicketEvent</c> validates before it touches
        /// the bound list, so a bad event can never corrupt the grid. Returns the description of what was sent.
        /// </summary>
        public string SendCorruptEvent()
        {
            _corruptKind = 1 - _corruptKind;

            if (_corruptKind == 1)
            {
                Raise(new TicketChangedEventArgs
                {
                    Ticket = null,
                    ChangeType = "Updated",
                    Message = "corrupt event: Ticket is null",
                }, counted: false);
                return "Ticket = null";
            }

            Raise(new TicketChangedEventArgs
            {
                Ticket = new Ticket { Id = 0, Title = "(no title)", Customer = "(no customer)" },
                ChangeType = "Updated",
                Message = "corrupt event: Ticket.Id = 0",
            }, counted: false);
            return "Ticket.Id = 0";
        }

        #endregion

        #region Record helpers

        private void Raise(TicketChangedEventArgs e, bool counted = true)
        {
            if (counted)
                EventsRaised++;
            TicketChanged?.Invoke(this, e);
        }

        private Ticket CreateRecord(TicketStatus status)
        {
            int id = _nextId++;
            var record = new Ticket
            {
                Id = id,
                Title = Titles[_random.Next(Titles.Length)],
                Customer = Customers[_random.Next(Customers.Length)],
                UpdatedAt = DateTime.Now,
            };
            record.Owner = status == TicketStatus.New ? "unassigned" : Owners[_random.Next(1, Owners.Length)];
            record.Status = status;
            return record;
        }

        /// <summary>
        /// A copy of the record. The page never stores the simulator's instance in its bound list: it copies the
        /// fields into its own <see cref="Ticket"/>, so the service can keep changing its records on another
        /// thread without the grid reading a half-written object.
        /// </summary>
        private static Ticket Snapshot(Ticket record)
        {
            return new Ticket
            {
                Id = record.Id,
                Title = record.Title,
                Customer = record.Customer,
                Owner = record.Owner,
                Status = record.Status,
                UpdatedAt = record.UpdatedAt,
            };
        }

        private TicketStatus RandomStatus()
        {
            switch (_random.Next(4))
            {
                case 0: return TicketStatus.New;
                case 1: return TicketStatus.Assigned;
                case 2: return TicketStatus.Waiting;
                default: return TicketStatus.Resolved;
            }
        }

        /// <summary>The lifecycle a ticket walks through; a small chance of an escalation at every step.</summary>
        private TicketStatus NextStatus(TicketStatus current)
        {
            if (_random.Next(6) == 0)
                return TicketStatus.Escalated;

            switch (current)
            {
                case TicketStatus.New: return TicketStatus.Assigned;
                case TicketStatus.Assigned: return _random.Next(2) == 0 ? TicketStatus.Waiting : TicketStatus.Resolved;
                case TicketStatus.Waiting: return TicketStatus.Resolved;
                case TicketStatus.Escalated: return TicketStatus.Assigned;
                default: return TicketStatus.Waiting;
            }
        }

        /// <summary>A short, readable description of the feed for the SERVER STATE block.</summary>
        public string Describe()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "records={0} events={1} running={2}",
                RecordCount,
                EventsRaised,
                _running ? "true" : "false");
        }

        #endregion
    }
}
