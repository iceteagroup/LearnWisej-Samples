using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using TicketOpsLive.Models;
using TicketOpsLive.Services;
using Wisej.Web;

namespace TicketOpsLive
{
    public partial class MainPage : Page
    {
        // Per-session state: every browser tab has its own MainPage, its own bound list and its own feed.

        /// <summary>The master list: every ticket this session knows about, newest first.</summary>
        private readonly List<Ticket> _allTickets = new List<Ticket>();

        /// <summary>What the grid shows: the master list, filtered. The BindingSource binds to this list.</summary>
        private readonly BindingList<Ticket> _tickets = new BindingList<Ticket>();

        /// <summary>When each ticket's "updated" marker was set; markerTimer clears markers older than 3 s.</summary>
        private readonly Dictionary<int, DateTime> _markerSince = new Dictionary<int, DateTime>();

        private readonly TicketSimulator _feed;

        private int _pushers;
        private bool _polling;
        private bool _restoringSelection;   // the code, not the user, is selecting a row

        private const int MarkerSeconds = 3;
        private const int SeedCount = 5;
        private const int FeedCount = 20;
        private const int FeedIntervalMs = 1000;
        private const int RandomizeCount = 3;

        public MainPage()
        {
            InitializeComponent();

            _feed = new TicketSimulator(() => !this.IsDisposed);
            _feed.TicketChanged += Feed_TicketChanged;
            _feed.FeedStopped += Feed_FeedStopped;

            this.Disposed += MainPage_Disposed;
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";

            // Bind once. From here on the code changes _tickets and calls ResetBindings(false).
            ticketsBindingSource.DataSource = _tickets;

            SeedBoard();
            markerTimer.Start();
        }

        private void MainPage_Disposed(object sender, EventArgs e)
        {
            _feed.Stop();
            _feed.TicketChanged -= Feed_TicketChanged;
            _feed.FeedStopped -= Feed_FeedStopped;
        }

        private void SeedBoard()
        {
            foreach (Ticket seed in _feed.Seed(SeedCount))
            {
                _allTickets.Insert(0, seed);
                _markerSince[seed.Id] = DateTime.MinValue;
            }

            ApplyFilter();
            ResetBindingsQuietly();
        }

        #region The update path — every ticket event goes through here

        /// <summary>
        /// Raised on the feed's task thread (new tickets) or on the request thread (Randomize statuses).
        /// Both are applied the same way: in this session's context, then pushed once.
        /// </summary>
        private void Feed_TicketChanged(object sender, TicketChangedEventArgs e)
        {
            if (this.IsDisposed)
                return;

            try
            {
                Application.Update(this, () => ApplyTicketEvent(e));
            }
            catch (ObjectDisposedException) { }
        }

        private void ApplyTicketEvent(TicketChangedEventArgs e)
        {
            if (e?.Ticket == null)
                return;

            int? selectedId = CurrentSelectedId();

            Ticket existing = FindById(e.Ticket.Id);
            bool added = existing == null;

            if (added)
            {
                // New tickets go to the top. The session keeps its own instance, a copy of the service's snapshot.
                existing = new Ticket
                {
                    Id = e.Ticket.Id,
                    Title = e.Ticket.Title,
                    Customer = e.Ticket.Customer,
                    Owner = e.Ticket.Owner,
                    Status = e.Ticket.Status,
                    UpdatedAt = DateTime.Now,
                };
                _allTickets.Insert(0, existing);
            }
            else
            {
                // Existing tickets change in place: the row keeps its position.
                existing.Status = e.Ticket.Status;
                existing.Owner = e.Ticket.Owner;
                existing.UpdatedAt = DateTime.Now;
            }

            existing.RecentlyUpdated = true;
            _markerSince[existing.Id] = DateTime.Now;

            ApplyFilter();
            ResetBindingsQuietly();

            if (selectedId.HasValue)
                RestoreSelection(selectedId.Value);

            // The ticket the user has selected changed underneath them: warn, don't reload.
            if (!added && selectedId.HasValue && selectedId.Value == existing.Id)
                ShowTicketChangeWarning(existing);
        }

        /// <summary>
        /// ResetBindings(false) raises SelectionChanged synchronously, pointing at whatever now sits at the
        /// old row index. That is not a user action, so the handler is muted for the duration.
        /// </summary>
        private void ResetBindingsQuietly()
        {
            _restoringSelection = true;
            try
            {
                ticketsBindingSource.ResetBindings(false);
            }
            finally
            {
                _restoringSelection = false;
            }
        }

        private bool RestoreSelection(int id)
        {
            _restoringSelection = true;
            try
            {
                for (int i = 0; i < ticketsGrid.Rows.Count; i++)
                {
                    if (ticketsGrid.Rows[i].DataBoundItem is Ticket ticket && ticket.Id == id)
                    {
                        ticketsGrid.Rows[i].Selected = true;
                        ticketsGrid.CurrentCell = ticketsGrid.Rows[i].Cells[0];
                        return true;
                    }
                }
                return false;
            }
            finally
            {
                _restoringSelection = false;
            }
        }

        private Ticket FindById(int id)
        {
            for (int i = 0; i < _allTickets.Count; i++)
            {
                if (_allTickets[i].Id == id)
                    return _allTickets[i];
            }
            return null;
        }

        #endregion

        #region New ticket every second for 20 seconds

        private void newTicketsButton_Click(object sender, EventArgs e)
        {
            if (_feed.IsRunning)
                return;

            newTicketsButton.Enabled = false;
            stopButton.Enabled = true;
            BeginPush();

            if (!_feed.StartNewTickets(FeedCount, FeedIntervalMs))
            {
                EndPush();
                newTicketsButton.Enabled = true;
                stopButton.Enabled = false;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _feed.Stop();
            stopButton.Enabled = false;
        }

        /// <summary>Raised from the feed task's finally block, whatever happened: restore the buttons once.</summary>
        private void Feed_FeedStopped(object sender, FeedStoppedEventArgs e)
        {
            if (this.IsDisposed)
                return;

            try
            {
                Application.Update(this, () =>
                {
                    newTicketsButton.Enabled = true;
                    stopButton.Enabled = false;
                    EndPush();

                    if (e.Faulted)
                    {
                        AlertBox.Show("The ticket feed failed. See the server log.", MessageBoxIcon.Error,
                            alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                    }
                });
            }
            catch (ObjectDisposedException) { }
        }

        #endregion

        #region Randomize statuses (in the request, no push needed)

        private void changeStatusButton_Click(object sender, EventArgs e)
        {
            _feed.RandomizeStatuses(RandomizeCount);
        }

        #endregion

        #region Escalated filter

        private void escalatedOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            int? selectedId = CurrentSelectedId();

            ApplyFilter();
            ResetBindingsQuietly();

            if (selectedId.HasValue)
                RestoreSelection(selectedId.Value);
        }

        /// <summary>Rebuilds the bound list from the master list, only when the result actually differs.</summary>
        private bool ApplyFilter()
        {
            bool filtered = escalatedOnlyCheckBox.Checked;

            var wanted = new List<Ticket>();
            foreach (Ticket ticket in _allTickets)
            {
                if (!filtered || ticket.Status == TicketStatus.Escalated)
                    wanted.Add(ticket);
            }

            if (SameSequence(_tickets, wanted))
                return false;

            _tickets.RaiseListChangedEvents = false;
            _tickets.Clear();
            foreach (Ticket ticket in wanted)
                _tickets.Add(ticket);
            _tickets.RaiseListChangedEvents = true;
            return true;
        }

        private static bool SameSequence(IList<Ticket> current, IList<Ticket> wanted)
        {
            if (current.Count != wanted.Count)
                return false;
            for (int i = 0; i < current.Count; i++)
            {
                if (!ReferenceEquals(current[i], wanted[i]))
                    return false;
            }
            return true;
        }

        #endregion

        #region The temporary "updated" marker

        // A timer tick is a normal request: its changes are returned with it, no Application.Update() needed.
        private void markerTimer_Tick(object sender, EventArgs e)
        {
            DateTime cutoff = DateTime.Now.AddSeconds(-MarkerSeconds);
            var expired = new List<int>();

            foreach (Ticket ticket in _allTickets)
            {
                if (!ticket.RecentlyUpdated)
                    continue;
                if (_markerSince.TryGetValue(ticket.Id, out DateTime since) && since > cutoff)
                    continue;

                ticket.RecentlyUpdated = false;
                expired.Add(ticket.Id);
            }

            if (expired.Count == 0)
                return;

            foreach (int id in expired)
                _markerSince.Remove(id);

            int? selectedId = CurrentSelectedId();
            ResetBindingsQuietly();
            if (selectedId.HasValue)
                RestoreSelection(selectedId.Value);
        }

        #endregion

        #region Selection and the conflict warning

        private void ticketsGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (_restoringSelection)
                return;

            HideTicketChangeWarning();
        }

        private int? CurrentSelectedId()
        {
            return ticketsGrid.CurrentRow?.DataBoundItem is Ticket ticket ? ticket.Id : (int?)null;
        }

        private void ShowTicketChangeWarning(Ticket ticket)
        {
            ticketChangeLabel.Text =
                $"⚠ Ticket #{ticket.Id} changed on the server while you have it open — now {ticket.Status}, " +
                $"{ticket.Owner}, at {ticket.UpdatedAt.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}.";
            ticketChangeLabel.Visible = true;
            dismissButton.Visible = true;
        }

        private void HideTicketChangeWarning()
        {
            ticketChangeLabel.Visible = false;
            dismissButton.Visible = false;
        }

        private void dismissButton_Click(object sender, EventArgs e)
        {
            HideTicketChangeWarning();
        }

        #endregion

        #region Delivery: WebSocket push, or polling while the feed runs

        private void BeginPush()
        {
            _pushers++;
            if (Application.IsWebSocket || _polling)
                return;

            Application.StartPolling(1000);
            _polling = true;
        }

        private void EndPush()
        {
            if (_pushers > 0) _pushers--;
            if (_pushers > 0 || !_polling)
                return;

            Application.EndPolling();
            _polling = false;
        }

        #endregion
    }
}
