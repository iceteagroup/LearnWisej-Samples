using System;
using System.Collections.Generic;
using System.Threading;
using TicketOpsLive.Models;
using Wisej.Web;

namespace TicketOpsLive.Services
{
    /// <summary>
    /// The simulated ticket feed — a stand-in for a service that would watch a queue, a database or a message bus.
    ///
    /// Session-owned: MainPage creates one instance and keeps it in an instance field. The simulator never holds
    /// a control; it raises events and the page applies them in its own context. The simulator owns the ticket
    /// records; the page owns its own bound copies.
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

        /// <summary>Tells the simulator whether the page that owns it is still alive.</summary>
        private readonly Func<bool> _isPageAlive;

        /// <summary>Guards <see cref="_records"/>: the feed task and the request thread both reach it.</summary>
        private readonly object _gate = new object();

        private readonly List<Ticket> _records = new List<Ticket>();
        private readonly Random _random = new Random();

        private volatile bool _running;
        private int _nextId = 4818;

        public TicketSimulator(Func<bool> isPageAlive)
        {
            _isPageAlive = isPageAlive ?? (() => true);
        }

        /// <summary>One ticket was added or changed.</summary>
        public event EventHandler<TicketChangedEventArgs> TicketChanged;

        /// <summary>Raised from the feed task's finally block, whatever happened (completed, stopped, disposed, faulted).</summary>
        public event EventHandler<FeedStoppedEventArgs> FeedStopped;

        /// <summary>True while the "new ticket every second" task is running.</summary>
        public bool IsRunning => _running;

        /// <summary>Creates <paramref name="count"/> records and returns snapshots of them, oldest first.</summary>
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

        #region A new ticket every second (Application.StartTask)

        /// <summary>Starts the feed. Returns false when a feed is already running.</summary>
        public bool StartNewTickets(int count = 20, int everyMs = 1000)
        {
            if (_running)
                return false;

            _running = true;
            Application.StartTask(() => NewTicketLoop(count, everyMs));
            return true;
        }

        /// <summary>Cooperative stop: the loop checks the flag before every ticket.</summary>
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
                        reason = "stopped";
                        break;
                    }
                    if (!_isPageAlive())
                    {
                        reason = "page disposed";
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
                        Thread.Sleep(everyMs);
                }

                if (reason == null)
                    reason = "completed";
            }
            catch (Exception ex)
            {
                faulted = true;
                reason = "fault: " + ex.Message;
                Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} ticket feed failed for client {Application.ClientId}: {ex}");
            }
            finally
            {
                _running = false;

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

        #region Randomize a few statuses, inside the request

        /// <summary>
        /// Changes the status of <paramref name="count"/> random records and raises one TicketChanged per record.
        /// Runs in the click's request, so the UI changes travel back with that response.
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

            // Raise outside the lock: the page's handler runs UI code and must never hold the data lock.
            foreach (TicketChangedEventArgs change in changes)
                Raise(change);

            return changes.Count;
        }

        #endregion

        #region Record helpers

        private void Raise(TicketChangedEventArgs e)
        {
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

        /// <summary>A copy of the record: the page never stores the simulator's instance in its bound list.</summary>
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

        #endregion
    }
}
