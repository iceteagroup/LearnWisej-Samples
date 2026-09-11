using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TicketOpsLive.Models;
using TicketOpsLive.Services;
using Wisej.Core;
using Wisej.Web;

namespace TicketOpsLive
{
    public partial class MainPage : Page
    {
        // The global service: one instance for the whole process, shared by every session.
        private readonly TicketHub _hub = TicketHub.Instance;

        // This session's context, captured on Load. The hub's event arrives on a thread-pool thread with no
        // session context of its own; every update goes through Application.Update(_context, …).
        private IWisejComponent _context;

        // Per-session state: instance fields, one set per browser tab.
        private readonly BindingList<Ticket> _tickets = new BindingList<Ticket>();
        private string _tenant = "Contoso";
        private bool _subscribed;
        private bool _exitHooked;
        private int _notificationCount;

        private static readonly string[] Owners = { "dispatcher", "s.oliveira", "m.keller", "a.rossi", "j.novak", "escalation-desk" };

        public MainPage()
        {
            InitializeComponent();

            // The grid is bound to a list this session owns. The hub's tickets are never bound to anything.
            ticketsBindingSource.DataSource = _tickets;

            this.Disposed += MainPage_Disposed;
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";

            _context = Application.Current;
            _tenant = (tenantComboBox.SelectedItem as string) ?? "Contoso";

            LoadSnapshot(_hub.GetSnapshot());
            Subscribe();

            if (!_exitHooked)
            {
                Application.ApplicationExit += Application_ApplicationExit;
                _exitHooked = true;
            }

            RenderNotificationCount();
        }

        #region Snapshot and tenant

        /// <summary>Copies the hub's snapshot into this session's bound list, keeping only this session's tenant.</summary>
        private void LoadSnapshot(System.Collections.Generic.IReadOnlyList<Ticket> snapshot)
        {
            int selectedId = SelectedTicketId();

            _tickets.Clear();
            foreach (var ticket in snapshot)
            {
                if (string.Equals(ticket.TenantId, _tenant, StringComparison.Ordinal))
                    _tickets.Add(ticket.Clone());
            }
            ticketsBindingSource.ResetBindings(false);
            RestoreSelection(selectedId);
        }

        private void tenantComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tenant = (tenantComboBox.SelectedItem as string) ?? "Contoso";
            if (string.Equals(tenant, _tenant, StringComparison.Ordinal))
                return;

            _tenant = tenant;
            LoadSnapshot(_hub.GetSnapshot());
        }

        #endregion

        #region Subscription lifecycle

        private void subscribeButton_Click(object sender, EventArgs e)
        {
            Subscribe();
        }

        private void unsubscribeButton_Click(object sender, EventArgs e)
        {
            Unsubscribe();
        }

        /// <summary>Adds the handler once: a second subscription would render every event twice.</summary>
        private void Subscribe()
        {
            if (_subscribed)
                return;

            _hub.TicketChanged += Hub_TicketChanged;
            _subscribed = true;

            if (!this.IsDisposed)
            {
                subscribeButton.Enabled = false;
                unsubscribeButton.Enabled = true;
            }
        }

        /// <summary>Removes the handler once. ApplicationExit, Disposed and the Unsubscribe button can all call it.</summary>
        private void Unsubscribe()
        {
            if (!_subscribed)
                return;

            _hub.TicketChanged -= Hub_TicketChanged;
            _subscribed = false;

            if (!this.IsDisposed)
            {
                subscribeButton.Enabled = true;
                unsubscribeButton.Enabled = false;
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            Unsubscribe();
        }

        private void MainPage_Disposed(object sender, EventArgs e)
        {
            Unsubscribe();

            if (_exitHooked)
            {
                try
                {
                    Application.ApplicationExit -= Application_ApplicationExit;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} could not detach ApplicationExit: {ex.Message}");
                }
                _exitHooked = false;
            }
        }

        #endregion

        #region The subscriber — hub events arrive on a thread-pool thread

        private void Hub_TicketChanged(object sender, TicketChangedEventArgs e)
        {
            if (this.IsDisposed)
                return;

            // Tenant filter: the event carries the metadata, this session decides.
            if (!string.Equals(e.TenantId, _tenant, StringComparison.Ordinal))
                return;

            SafeUpdate(() =>
            {
                ApplyTicketEvent(e);

                notificationsList.Items.Insert(0, $"{DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}  {e.Message}");
                while (notificationsList.Items.Count > 200)
                    notificationsList.Items.RemoveAt(notificationsList.Items.Count - 1);

                _notificationCount++;
                RenderNotificationCount();

                // Event-type filter: only an escalation interrupts the operator.
                if (string.Equals(e.ChangeType, "Escalated", StringComparison.Ordinal))
                {
                    AlertBox.Show(
                        $"Ticket {e.Ticket?.Id} escalated — {e.Ticket?.Title}",
                        MessageBoxIcon.Warning,
                        alignment: System.Drawing.ContentAlignment.TopRight,
                        autoCloseDelay: 4000);
                }
            });
        }

        /// <summary>Applies one event to this session's bound list, keeping the selected row.</summary>
        private void ApplyTicketEvent(TicketChangedEventArgs e)
        {
            Ticket incoming = e.Ticket;
            if (incoming == null)
                return;

            int selectedId = SelectedTicketId();

            Ticket existing = _tickets.FirstOrDefault(t => t.Id == incoming.Id);
            if (existing == null)
                _tickets.Insert(0, incoming.Clone());
            else
                existing.CopyFrom(incoming);

            ticketsBindingSource.ResetBindings(false);
            RestoreSelection(selectedId);
        }

        private void SafeUpdate(Action action)
        {
            try
            {
                Application.Update(_context, action);
            }
            catch (ObjectDisposedException)
            {
                // the page went away between the IsDisposed check and the push
            }
        }

        #endregion

        #region Publish and escalate

        private void publishButton_Click(object sender, EventArgs e)
        {
            Ticket selected = SelectedTicket();
            Ticket ticket;

            if (selected != null)
            {
                // Update the hub's authoritative copy, not a possibly stale local one.
                ticket = _hub.Find(selected.Id) ?? selected.Clone();
                ticket.Status = NextStatus(ticket.Status);
                ticket.Owner = NextOwner(ticket.Owner);
                ticket.UpdatedAt = DateTime.Now;
            }
            else
            {
                ticket = NewTicket(_tenant);
            }

            Publish(ticket, null);
        }

        private void escalateButton_Click(object sender, EventArgs e)
        {
            Ticket selected = SelectedTicket();
            if (selected == null)
            {
                AlertBox.Show("Select a ticket to escalate.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 3000);
                return;
            }

            Ticket ticket = _hub.Find(selected.Id) ?? selected.Clone();
            ticket.Status = TicketStatus.Escalated;
            ticket.Owner = "escalation-desk";
            ticket.UpdatedAt = DateTime.Now;

            Publish(ticket, "Escalated");
        }

        private void Publish(Ticket ticket, string changeType)
        {
            try
            {
                _hub.AddOrUpdate(ticket, changeType);
            }
            catch (ArgumentException ex)
            {
                LogError("hub.AddOrUpdate", ex);
                AlertBox.Show("The ticket could not be published. See the server log.", MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
        }

        #endregion

        #region Helpers

        private Ticket SelectedTicket()
        {
            if (ticketsGrid.SelectedRows.Count == 0)
                return null;
            return ticketsGrid.SelectedRows[0].DataBoundItem as Ticket;
        }

        private int SelectedTicketId()
        {
            Ticket selected = SelectedTicket();
            return selected == null ? -1 : selected.Id;
        }

        private void RestoreSelection(int ticketId)
        {
            if (ticketId <= 0)
                return;

            for (int i = 0; i < ticketsGrid.Rows.Count; i++)
            {
                if (ticketsGrid.Rows[i].DataBoundItem is Ticket ticket && ticket.Id == ticketId)
                {
                    ticketsGrid.Rows[i].Selected = true;
                    if (ticketsGrid.Rows[i].Cells.Count > 0)
                        ticketsGrid.CurrentCell = ticketsGrid.Rows[i].Cells[0];
                    return;
                }
            }
        }

        private void RenderNotificationCount()
        {
            notificationCountLabel.Text = _notificationCount == 1
                ? "1 notification in this session"
                : $"{_notificationCount} notifications in this session";
        }

        private Ticket NewTicket(string tenant)
        {
            string[] titles = tenant == "Northwind"
                ? new[] { "Label printer jammed in bay 4", "Stock count mismatch after sync", "Courier API returns 502", "Returns portal shows stale orders" }
                : new[] { "Outlook profile corrupt on rebuild", "SharePoint sync stuck at 94%", "Payment gateway timeout at checkout", "Meeting-room panel unresponsive" };

            int id = _hub.NextTicketId(tenant);
            return new Ticket
            {
                Id = id,
                TenantId = tenant,
                Title = titles[id % titles.Length],
                Customer = tenant == "Northwind" ? "Northwind Traders" : "Fabrikam Ltd",
                Owner = "dispatcher",
                Status = TicketStatus.New,
                UpdatedAt = DateTime.Now
            };
        }

        private static TicketStatus NextStatus(TicketStatus status)
        {
            return status switch
            {
                TicketStatus.New => TicketStatus.Assigned,
                TicketStatus.Assigned => TicketStatus.Waiting,
                TicketStatus.Waiting => TicketStatus.Resolved,
                TicketStatus.Resolved => TicketStatus.New,
                _ => TicketStatus.Assigned,
            };
        }

        private static string NextOwner(string owner)
        {
            int index = Array.IndexOf(Owners, owner);
            return Owners[(index + 1) % (Owners.Length - 1)];      // never rotates into "escalation-desk"
        }

        private static void LogError(string operation, Exception ex)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} {operation} failed for session {Application.SessionId}: {ex}");
        }

        #endregion
    }
}
