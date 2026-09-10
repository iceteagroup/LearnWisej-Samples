using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using TicketOpsLive.Models;
using TicketOpsLive.Services;
using Wisej.Web;

namespace TicketOpsLive
{
    public enum TraceDirection { Push, Request, Server }

    /// <summary>
    /// TicketOps Live — Module 5 · Live Ticket Board: Updating Data Without Rebuilding the Screen.
    ///
    /// Left card:    the board — a DataGridView bound through a BindingSource to a BindingList&lt;Ticket&gt;.
    ///               Rows arrive at the top, existing rows change in place, a temporary "updated" marker fades
    ///               after 3 s, the selected row survives every event, and a non-blocking conflict warning
    ///               appears when the ticket the user has selected changes underneath them.
    /// Right card:   every push, request and server decision, with the id, what happened and whether the
    ///               selection was kept.
    /// Bottom bar:   progress path (New ticket every second ×20), success path (Randomize statuses),
    ///               cancellation (Stop feed), the anti-pattern (Rebind whole grid) and the failure path
    ///               (Apply corrupt event → the event is validated and rejected, the grid stays intact).
    ///
    /// The rule of the lesson: change the LIST, then notify the SOURCE. The grid is bound exactly once, in the
    /// Designer; nothing in this file ever assigns ticketsGrid.DataSource again — except rebindButton_Click,
    /// which exists to show what that costs.
    /// </summary>
    public partial class MainPage : Page
    {
        // ---------------------------------------------------------------------------------------------
        // Per-session state. Every browser tab has its own MainPage, so it has its own bound list and its
        // own feed. A static list here would be shared by every user of the server: one user's filter would
        // hide another user's rows and two threads would fight over the same collection.
        // ---------------------------------------------------------------------------------------------

        /// <summary>The master list: every ticket this session knows about, newest first.</summary>
        private readonly List<Ticket> _allTickets = new List<Ticket>();

        /// <summary>What the grid shows: the master list, filtered. This is the list the BindingSource binds to.</summary>
        private readonly BindingList<Ticket> _tickets = new BindingList<Ticket>();

        /// <summary>When each ticket's "updated" marker was set — markerTimer clears markers older than 3 s.</summary>
        private readonly Dictionary<int, DateTime> _markerSince = new Dictionary<int, DateTime>();

        /// <summary>The simulated feed. Session-owned: created here, stopped when this page is disposed.</summary>
        private readonly TicketSimulator _feed;

        private int _pushes;
        private int _eventsApplied;
        private int _eventsRejected;
        private int _pushers;               // tasks that currently need out-of-bound delivery
        private bool _polling;              // the fallback is active (no WebSocket when the task started)
        private bool _restoringSelection;   // suppresses the trace while the code (not the user) selects a row
        private bool _inRequestEvent;       // the current TicketChanged was raised inside a click, not by the task

        private const int MarkerSeconds = 3;
        private const int SeedCount = 5;
        private const int FeedCount = 20;
        private const int FeedIntervalMs = 1000;
        private const int RandomizeCount = 3;

        public MainPage()
        {
            InitializeComponent();

            // The simulator is an INSTANCE of this page, never a global. It asks the page whether it is still
            // alive instead of holding a control reference, so it cannot keep a disposed page in memory.
            _feed = new TicketSimulator(() => !this.IsDisposed);
            _feed.TicketChanged += Feed_TicketChanged;
            _feed.FeedStopped += Feed_FeedStopped;

            // Cleanup rule: the page going away stops the feed and drops the handlers.
            this.Disposed += MainPage_Disposed;
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";

            // Bind ONCE. From here on the code changes _tickets and calls ResetBindings(false); the grid's
            // DataSource (set in the Designer to ticketsBindingSource) is never assigned again.
            ticketsBindingSource.DataSource = _tickets;

            AddTrace(TraceDirection.Request, "MainPage_Load", $"first request: the page is rendered and returned with the response · IsWebSocket={Low(Application.IsWebSocket)} (the socket opens after this response)");
            AddTrace(TraceDirection.Server, "bind once", $"ticketsBindingSource.DataSource = _tickets (BindingList<Ticket>) · ticketsGrid.DataSource = ticketsBindingSource · AutoGenerateColumns = false, {ticketsGrid.Columns.Count} columns");

            SeedBoard();

            markerTimer.Start();
            SetStatus("idle · " + _allTickets.Count + " tickets on the board", StatusKind.Normal);
            RenderSelection();
            UpdateState();
        }

        private void MainPage_Disposed(object sender, EventArgs e)
        {
            _feed.Stop();
            _feed.TicketChanged -= Feed_TicketChanged;
            _feed.FeedStopped -= Feed_FeedStopped;
        }

        /// <summary>Five tickets so the board is never empty. They go through the same code path as live events.</summary>
        private void SeedBoard()
        {
            foreach (Ticket seed in _feed.Seed(SeedCount))
            {
                _allTickets.Insert(0, seed);
                _markerSince[seed.Id] = DateTime.MinValue;      // seeded rows start without a marker
            }

            ApplyFilter();
            ResetBindingsQuietly();
            AddTrace(TraceDirection.Server, "seed", $"{SeedCount} tickets added to the bound list · ResetBindings(false) · the grid was never rebound");
        }

        #region The one update path — every event goes through here

        /// <summary>
        /// The simulator raises this on the task thread (the "new ticket every second" feed) or on the request
        /// thread (Randomize statuses / Apply corrupt event). Both cases are handled the same way: enter the
        /// session context, apply the change to the bound list, push once.
        /// </summary>
        private void Feed_TicketChanged(object sender, TicketChangedEventArgs e)
        {
            if (this.IsDisposed)
                return;                     // the tab went away between the raise and the handler

            try
            {
                // Application.Update(context, callback) runs the callback in THIS session's context and then
                // flushes the pending changes in ONE push. Never mutate the bound list outside this callback:
                // the grid is reading it from the request thread.
                Application.Update(this, () => ApplyTicketEvent(e));
            }
            catch (ObjectDisposedException)
            {
                // the page was disposed between the IsDisposed check and the push
            }
        }

        /// <summary>
        /// The lab's ApplyTicketEvent. Remember the selection, insert or update in place, reset bindings once,
        /// restore the selection. Nothing here rebinds the grid.
        /// </summary>
        private void ApplyTicketEvent(TicketChangedEventArgs e)
        {
            // ---- validation first: a malformed event must never reach the bound list --------------------
            if (e == null || e.Ticket == null || e.Ticket.Id <= 0)
            {
                _eventsRejected++;
                string why = e == null || e.Ticket == null ? "Ticket is null" : "Ticket.Id = " + e.Ticket.Id;
                AddTrace(TraceDirection.Server, "ApplyTicketEvent", $"REJECTED — {why} · the bound list was not touched, the grid is intact ({_tickets.Count} rows)");
                ShowBanner($"✖ A malformed ticket event was rejected ({why}). The board was not changed — {_tickets.Count} rows, selection unchanged. See the server log.", BannerKind.Error);
                SetStatus("corrupt event rejected — board intact", StatusKind.Error);
                Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} rejected ticket event ({why}) for client {Application.ClientId}");
                UpdateState();
                return;
            }

            // ---- 1. remember what the user was reading --------------------------------------------------
            int? selectedId = ticketsGrid.CurrentRow?.DataBoundItem is Ticket selected
                ? selected.Id
                : (int?)null;

            // ---- 2. change the LIST, not the binding ----------------------------------------------------
            Ticket existing = FindById(e.Ticket.Id);
            bool added = existing == null;

            if (added)
            {
                // New tickets sort to the top (row cue of the lesson). The session keeps its OWN instance:
                // the snapshot the service handed over is copied, never stored by reference.
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
                // Existing tickets change IN PLACE: the row keeps its position, its width and its identity.
                existing.Status = e.Ticket.Status;
                existing.Owner = e.Ticket.Owner;
                existing.UpdatedAt = DateTime.Now;
            }

            existing.RecentlyUpdated = true;
            _markerSince[existing.Id] = DateTime.Now;

            // ---- 3. the filter is part of the list, not of the binding ----------------------------------
            ApplyFilter();

            // ---- 4. one notification per event ----------------------------------------------------------
            ResetBindingsQuietly();

            // ---- 5. give the user their row back --------------------------------------------------------
            bool kept = false;
            if (selectedId.HasValue)
                kept = RestoreSelection(selectedId.Value);

            _eventsApplied++;
            RenderSelection();

            string selectionText = !selectedId.HasValue
                ? "no selection"
                : kept
                    ? "selection kept #" + selectedId.Value
                    : "selection #" + selectedId.Value + " filtered out (row not shown)";

            if (_inRequestEvent)
            {
                // Raised inside the click: the change travels back with the response of that click.
                AddTrace(TraceDirection.Server, "in-request update", $"#{existing.Id} {(added ? "added" : "updated")} · {selectionText} · ResetBindings(false)");
            }
            else
            {
                _pushes++;
                AddTrace(TraceDirection.Push, "Application.Update(this, …)", $"#{existing.Id} {(added ? "added" : "updated")} · {selectionText} · ResetBindings(false)");
            }

            // ---- 6. the conflict rule: warn, do not reload ----------------------------------------------
            if (!added && selectedId.HasValue && selectedId.Value == existing.Id)
                ShowTicketChangeWarning(existing);

            SetStatus($"{_allTickets.Count} tickets · {_eventsApplied} events applied", StatusKind.Normal);
            UpdateState();
        }

        /// <summary>
        /// ResetBindings(false) makes the grid re-evaluate its current row against the new rows and raises
        /// SelectionChanged synchronously (verified: twice per reset, pointing at whatever now sits at the old row
        /// index). That is not a user action, so the handler is muted for the duration; RestoreSelection then puts
        /// the user back on their ticket.
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

        /// <summary>Walks the rows and re-selects the ticket the user was on. Returns false when the row is gone.</summary>
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

        #region Progress path — a new ticket every second for 20 seconds

        private void newTicketsButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "newTicketsButton_Click", "the browser sent the click");

            if (_feed.IsRunning)
            {
                // Acceptance criterion: a second click must not start a competing feed.
                AddTrace(TraceDirection.Server, "newTicketsButton_Click", "refused — the feed is already running (IsRunning == true)");
                ShowBanner("⚠ The ticket feed is already running — a second click must not start a competing loop.", BannerKind.Warn);
                return;
            }

            HideBanner();
            newTicketsButton.Enabled = false;
            stopButton.Enabled = true;
            SetStatus("feed running · one new ticket per second", StatusKind.Normal);
            AddTrace(TraceDirection.Server, "Application.StartTask", $"{FeedCount} tickets, one every {FeedIntervalMs} ms · each event is applied inside Application.Update(this, …)");
            BeginPush();

            if (!_feed.StartNewTickets(FeedCount, FeedIntervalMs))
            {
                // Lost the race with another click: undo what was set above.
                EndPush();
                newTicketsButton.Enabled = true;
                stopButton.Enabled = false;
                AddTrace(TraceDirection.Server, "StartNewTickets", "refused by the simulator — a feed was already running");
            }

            UpdateState();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            // Cooperative stop: the loop checks the flag before the next ticket and ends by itself.
            _feed.Stop();
            stopButton.Enabled = false;
            SetStatus("feed stopping… (ends within one second)", StatusKind.Warn);
            AddTrace(TraceDirection.Request, "stopButton_Click", "_feed.Stop() → the loop exits at its next check (≤ 1 s)");
            UpdateState();
        }

        /// <summary>
        /// The simulator's completion callback, raised from the task's finally block whatever happened. This is
        /// where the page restores its UI — exactly once, for every ending (completed, stopped, disposed, faulted).
        /// </summary>
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

                    if (e.Faulted)
                    {
                        ShowBanner($"✖ The ticket feed failed after {e.EventsRaised} tickets. The detail is in the server log (client {Application.ClientId}). Click “New ticket every second ×20” to start again.", BannerKind.Error);
                        SetStatus("feed failed — UI restored in finally", StatusKind.Error);
                    }
                    else
                    {
                        SetStatus($"{_allTickets.Count} tickets · feed idle", StatusKind.Normal);
                    }

                    _pushes++;
                    EndPush();
                    AddTrace(TraceDirection.Push, "Application.Update(this, …)", $"feed stopped — {e.Reason} (finally block of {e.Operation})");
                    UpdateState();
                });
            }
            catch (ObjectDisposedException)
            {
                // the page went away between the check and the push
            }
        }

        #endregion

        #region Success path — randomize statuses (in the request, no push needed)

        private void changeStatusButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "changeStatusButton_Click", $"{RandomizeCount} random tickets change in place — synchronously, inside this request");
            HideBanner();

            _inRequestEvent = true;
            int changed;
            try
            {
                changed = _feed.RandomizeStatuses(RandomizeCount);
            }
            finally
            {
                _inRequestEvent = false;
            }

            if (changed == 0)
            {
                ShowBanner("ⓘ There are no tickets yet — start the feed first.", BannerKind.Info);
                AddTrace(TraceDirection.Server, "RandomizeStatuses", "nothing to change — the simulator holds no records");
                return;
            }

            AddTrace(TraceDirection.Server, "RandomizeStatuses", $"{changed} tickets updated in place · no row was added, no row moved · the changes travel back with THIS response");
            SetStatus($"{changed} tickets updated in place", StatusKind.Normal);
            UpdateState();
        }

        #endregion

        #region Failure path — an event the page must reject

        private void corruptButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "corruptButton_Click", "the simulator raises a malformed TicketChanged event");

            _inRequestEvent = true;
            string what;
            try
            {
                what = _feed.SendCorruptEvent();
            }
            finally
            {
                _inRequestEvent = false;
            }

            AddTrace(TraceDirection.Server, "SendCorruptEvent", $"raised with {what} → ApplyTicketEvent validated it before touching the list");
            UpdateState();
        }

        #endregion

        #region Anti-pattern — rebind the whole grid

        /// <summary>
        /// What the lesson calls "Bad": ticketsGrid.DataSource = null; ticketsGrid.DataSource = LoadAllTicketsAgain().
        /// The same rows come back and everything around them is gone — the selection first of all.
        /// </summary>
        private void rebindButton_Click(object sender, EventArgs e)
        {
            int? before = CurrentSelectedId();
            AddTrace(TraceDirection.Request, "rebindButton_Click", $"the anti-pattern: DataSource = null, reload every ticket, bind again · selection before = {Describe(before)}");

            // 1 — throw the binding away. Every row, the selection, the scroll position and the sort go with it.
            ticketsGrid.DataSource = null;

            // 2 — "LoadAllTicketsAgain()": fresh instances, as a reload from a service would produce. The objects
            //     the grid used to point at do not exist any more, so nothing can be matched back.
            var reloaded = new List<Ticket>();
            foreach (Ticket ticket in _allTickets)
            {
                reloaded.Add(new Ticket
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Customer = ticket.Customer,
                    Owner = ticket.Owner,
                    Status = ticket.Status,
                    UpdatedAt = ticket.UpdatedAt,
                });
            }
            _allTickets.Clear();
            _allTickets.AddRange(reloaded);
            _markerSince.Clear();

            _tickets.RaiseListChangedEvents = false;
            _tickets.Clear();
            foreach (Ticket ticket in reloaded)
            {
                if (!escalatedOnlyCheckBox.Checked || ticket.Status == TicketStatus.Escalated)
                    _tickets.Add(ticket);
            }
            _tickets.RaiseListChangedEvents = true;

            // 3 — bind again.
            ticketsGrid.DataSource = ticketsBindingSource;
            ResetBindingsQuietly();

            int? after = CurrentSelectedId();
            RenderSelection();

            AddTrace(TraceDirection.Server, "rebind (anti-pattern)", $"{_tickets.Count} rows rebuilt · selection {Describe(before)} → {Describe(after)} · scroll, sort and the “updated” markers reset");
            ShowBanner($"⚠ Anti-pattern: the grid was rebound. The rows are the same, but the selection ({Describe(before)} → {Describe(after)}), the scroll position, the sort and every “updated” marker are gone. Incremental updates keep all of it.", BannerKind.Warn);
            SetStatus("grid rebound — screen state lost", StatusKind.Warn);
            UpdateState();
        }

        #endregion

        #region Extension challenge — the Escalated filter, live

        private void escalatedOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            int? selectedId = CurrentSelectedId();

            ApplyFilter();
            ResetBindingsQuietly();

            bool kept = selectedId.HasValue && RestoreSelection(selectedId.Value);
            RenderSelection();

            AddTrace(TraceDirection.Request, "escalatedOnlyCheckBox_CheckedChanged",
                $"filter {(escalatedOnlyCheckBox.Checked ? "ON (Escalated only)" : "OFF")} · {_tickets.Count} of {_allTickets.Count} rows shown · " +
                (selectedId.HasValue ? (kept ? "selection kept #" + selectedId.Value : "selection #" + selectedId.Value + " is filtered out") : "no selection"));
            SetStatus(escalatedOnlyCheckBox.Checked ? $"filter on · {_tickets.Count} escalated" : $"{_allTickets.Count} tickets on the board", StatusKind.Normal);
            UpdateState();
        }

        /// <summary>
        /// Rebuilds the bound list from the master list. The bound list is only touched when the result actually
        /// differs, so a status change that does not cross the filter costs nothing. Returns true when it changed.
        /// </summary>
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

            // One ListChanged notification instead of 2×N: the grid is refreshed by ResetBindings(false) below.
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

        #region Row cues — the temporary "updated" marker

        /// <summary>
        /// Runs as a normal request every second, so whatever it changes is returned with that request — no
        /// Application.Update() needed. It only resets the bindings when a marker actually expired.
        /// </summary>
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
                return;                      // nothing changed → no ResetBindings, no traffic

            foreach (int id in expired)
                _markerSince.Remove(id);

            int? selectedId = CurrentSelectedId();
            ResetBindingsQuietly();
            if (selectedId.HasValue)
                RestoreSelection(selectedId.Value);

            AddTrace(TraceDirection.Server, "markerTimer_Tick", $"{expired.Count} “updated” marker(s) older than {MarkerSeconds} s cleared · ResetBindings(false) · selection {Describe(selectedId)} kept");
            UpdateState();
        }

        #endregion

        #region Selection and the conflict warning

        private void ticketsGrid_SelectionChanged(object sender, EventArgs e)
        {
            RenderSelection();

            if (_restoringSelection)
                return;                      // the code re-selected the row after an event: not a user action

            AddTrace(TraceDirection.Request, "ticketsGrid_SelectionChanged", $"the user selected {Describe(CurrentSelectedId())} — this is the state every update has to protect");
            HideTicketChangeWarning();
            UpdateState();
        }

        private int? CurrentSelectedId()
        {
            return ticketsGrid.CurrentRow?.DataBoundItem is Ticket ticket ? ticket.Id : (int?)null;
        }

        private void RenderSelection()
        {
            Ticket ticket = ticketsGrid.CurrentRow?.DataBoundItem as Ticket;
            selectedLabel.Text = ticket == null
                ? "selected: none — click a row, then send updates"
                : $"selected: #{ticket.Id} · {ticket.Status} · {ticket.Owner} · updated {ticket.UpdatedAt.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// The conflict rule of the lesson: the ticket the user is on changed underneath them. Warn without
        /// blocking, never reload the row from under their eyes — let them choose.
        /// </summary>
        private void ShowTicketChangeWarning(Ticket ticket)
        {
            ticketChangeLabel.Text =
                $"⚠ Ticket #{ticket.Id} changed while selected — status {ticket.Status}, owner {ticket.Owner}, at {ticket.UpdatedAt.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}. " +
                "The row was updated in place; nothing was reloaded and your selection did not move.";
            ticketChangeLabel.Visible = true;
            dismissButton.Visible = true;
            AddTrace(TraceDirection.Server, "conflict", $"#{ticket.Id} changed while selected → warn, don't reload (ticketChangeLabel, non-blocking)");
        }

        private void HideTicketChangeWarning()
        {
            ticketChangeLabel.Visible = false;
            dismissButton.Visible = false;
        }

        private void dismissButton_Click(object sender, EventArgs e)
        {
            HideTicketChangeWarning();
            AddTrace(TraceDirection.Request, "dismissButton_Click", "the user dismissed the conflict warning — the row was never reloaded");
        }

        #endregion

        #region Delivery: WebSocket push, or the polling fallback while a task needs it

        /// <summary>
        /// Called where out-of-bound work starts. With a WebSocket the pushes flow by themselves. Without one,
        /// ask the browser to poll every second for the duration of the work — and only for that duration.
        /// </summary>
        private void BeginPush()
        {
            _pushers++;
            if (Application.IsWebSocket || _polling)
                return;

            Application.StartPolling(1000);
            _polling = true;
            AddTrace(TraceDirection.Server, "Application.StartPolling(1000)", "no WebSocket when the task started → fallback polling ON until the feed ends");
        }

        private void EndPush()
        {
            if (_pushers > 0) _pushers--;
            if (_pushers > 0 || !_polling)
                return;

            Application.EndPolling();
            _polling = false;
            AddTrace(TraceDirection.Server, "Application.EndPolling()", "the last task ended → fallback polling OFF");
        }

        #endregion

        #region Trace and UI helpers

        private enum StatusKind { Normal, Warn, Error }
        private enum BannerKind { Info, Warn, Error }

        private void AddTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction switch
            {
                TraceDirection.Push => "→ push    ",
                TraceDirection.Request => "← request ",
                _ => "• server  ",
            };
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            listTrace.Items.Add($"{time}  {prefix} {name,-30} {payload}");
            while (listTrace.Items.Count > 400)
                listTrace.Items.RemoveAt(0);
            listTrace.SelectedIndex = listTrace.Items.Count - 1;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            listTrace.Items.Clear();
        }

        private void SetStatus(string text, StatusKind kind)
        {
            labelStatus.Text = "● " + text;
            labelStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        private void ShowBanner(string text, BannerKind kind)
        {
            labelBanner.Text = text;
            switch (kind)
            {
                case BannerKind.Error:
                    labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
                    labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
                    break;
                case BannerKind.Warn:
                    labelBanner.BackColor = System.Drawing.Color.FromArgb(255, 244, 229);
                    labelBanner.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
                    break;
                default:
                    labelBanner.BackColor = System.Drawing.Color.FromArgb(230, 240, 251);
                    labelBanner.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
                    break;
            }
            labelBanner.Visible = true;
        }

        private void HideBanner()
        {
            labelBanner.Visible = false;
        }

        private void UpdateState()
        {
            string session = Application.SessionId ?? "";
            if (session.Length > 8) session = session.Substring(0, 8) + "…";

            int escalated = 0;
            foreach (Ticket ticket in _allTickets)
            {
                if (ticket.Status == TicketStatus.Escalated)
                    escalated++;
            }

            labelState.Text =
                $"SERVER STATE (authoritative · this session only)\n" +
                $"tickets={_allTickets.Count} shown={_tickets.Count} escalated={escalated} filter={(escalatedOnlyCheckBox.Checked ? "on" : "off")} selected={Describe(CurrentSelectedId())}\n" +
                $"feed running={Low(_feed.IsRunning)} events applied={_eventsApplied} rejected={_eventsRejected} pushes={_pushes} tasks={_pushers} polling={Low(_polling)} · simulator {_feed.Describe()}\n" +
                $"IsWebSocket={Low(Application.IsWebSocket)} ClientId={Application.ClientId} SessionId={session}\n" +
                $"_allTickets, _tickets (BindingList) and the simulator are MainPage instance fields — one bound list per browser tab, never static";
        }

        private static string Describe(int? id) => id.HasValue ? "#" + id.Value.ToString(CultureInfo.InvariantCulture) : "none";

        private static string Low(bool value) => value ? "true" : "false";

        #endregion
    }
}
