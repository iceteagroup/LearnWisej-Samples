using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using TicketOpsLive.Models;
using TicketOpsLive.Services;
using Wisej.Core;
using Wisej.Web;

namespace TicketOpsLive
{
    public enum TraceDirection { Push, Request, Server }

    /// <summary>
    /// TicketOps Live — Module 6 · From One Session to Many: TicketHub Events.
    ///
    /// The ownership model of the lesson, running:
    ///   the PAGE owns its controls   — ticketsGrid, notificationsList, the counters, all instance state;
    ///   the SESSION owns its context — _context = Application.Current, captured on Load, used by every push;
    ///   the GLOBAL SERVICE owns shared data and events — TicketHub.Instance, which never sees a control.
    ///
    /// Left top card:    the ticket board of THIS session — its own BindingList, its own tenant filter.
    /// Left bottom card: the per-session notifications (the extension challenge), the hub counters and SERVER STATE.
    /// Right card:       every request, every hub decision and every push this session made.
    /// Bottom bar:       success (Publish / Publish for the other tenant / Escalate), subscription lifecycle
    ///                   (Subscribe / Unsubscribe), failure (Publish invalid ticket → ArgumentException),
    ///                   progress + cadence (Burst 20 events from a task), and a second session (Open another session).
    ///
    /// The point of the module is what happens BETWEEN sessions, so run two or three tabs — see README.md.
    /// </summary>
    public partial class MainPage : Page
    {
        // The global service. One instance for the whole process; every session talks to the same one.
        private readonly TicketHub _hub = TicketHub.Instance;

        // THIS session's context, captured while we are inside a request (Load). Every out-of-bound update
        // this page makes goes through Application.Update(_context, …) — the hub's event arrives on a foreign
        // thread (the publisher's), with no session context of its own.
        private IWisejComponent _context;

        // Per-session state — INSTANCE fields. A static here would be shared by every user of the server:
        // one tenant for everybody, one notification count for everybody.
        private readonly BindingList<Ticket> _tickets = new BindingList<Ticket>();
        private string _tenant = "Contoso";
        private bool _subscribed;
        private bool _exitHooked;
        private int _notificationCount;
        private int _filteredOut;
        private int _pushes;
        private volatile bool _bursting;
        private int _pushers;               // tasks that currently need out-of-bound delivery
        private bool _polling;              // the fallback is active (no WebSocket when a task started)

        private const int BurstEvents = 20;
        private const int BurstIntervalMs = 150;

        private static readonly string[] Owners = { "dispatcher", "s.oliveira", "m.keller", "a.rossi", "j.novak", "escalation-desk" };

        public MainPage()
        {
            InitializeComponent();

            // The grid is bound to a list this session owns. The hub's tickets are never bound to anything.
            ticketsBindingSource.DataSource = _tickets;

            // Cleanup #2 (see docs/SubscriptionCleanup.md). ApplicationExit covers "the session ended";
            // Disposed covers "the page went away while the session lives on" (Application.MainPage replaced,
            // a navigation, a dialog-driven page swap). Unsubscribe() is idempotent, so both firing is fine.
            this.Disposed += MainPage_Disposed;
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";

            // Capture the session context while we are IN it. Later, the hub will call Hub_TicketChanged on the
            // publishing session's thread; without this component we would have nothing to push into.
            _context = Application.Current;

            AddTrace(TraceDirection.Request, "MainPage_Load", $"first request: this session's page is created · IsWebSocket={Low(Application.IsWebSocket)} (the socket opens after this response) · ClientId={Application.ClientId}");

            _tenant = (tenantComboBox.SelectedItem as string) ?? "Contoso";
            AddTrace(TraceDirection.Server, "session metadata", $"this session works for tenant \"{_tenant}\" — the filter lives HERE, not in the hub");

            LoadSnapshot(_hub.GetSnapshot(), "page load");

            // The subscription. From now on every AddOrUpdate anywhere on the server reaches this page.
            Subscribe("page load");

            if (!_exitHooked)
            {
                Application.ApplicationExit += Application_ApplicationExit;
                _exitHooked = true;
                AddTrace(TraceDirection.Server, "Application.ApplicationExit +=", "cleanup #1 armed — the session shutting down unsubscribes this page from the hub");
            }

            SetStatus("subscribed · waiting for hub events", StatusKind.Normal);
            RenderSelection();
            RenderCounters();
            UpdateState();
        }

        #region The snapshot — what a session sees before any event arrives

        /// <summary>
        /// Copies the hub's snapshot into THIS session's bound list, keeping only the tenant this session watches.
        /// The snapshot is already a list of clones; the page clones again on the way in so that nothing it binds
        /// is ever an object the hub could hand to another session.
        /// </summary>
        private void LoadSnapshot(IReadOnlyList<Ticket> snapshot, string reason)
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

            AddTrace(TraceDirection.Server, "hub.GetSnapshot()", $"{snapshot.Count} ticket(s) in the hub → {_tickets.Count} shown for tenant \"{_tenant}\" ({reason}) · clones, never the hub's instances");
        }

        private void tenantComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tenant = (tenantComboBox.SelectedItem as string) ?? "Contoso";
            if (string.Equals(tenant, _tenant, StringComparison.Ordinal))
                return;

            _tenant = tenant;
            AddTrace(TraceDirection.Request, "tenantComboBox_SelectedIndexChanged", $"this session now works for tenant \"{_tenant}\" — same hub, same events, a different filter");
            HideBanner();
            LoadSnapshot(_hub.GetSnapshot(), "tenant changed");
            SetStatus($"tenant {_tenant} · {(_subscribed ? "subscribed" : "NOT subscribed")}", _subscribed ? StatusKind.Normal : StatusKind.Warn);
            RenderSelection();
            RenderCounters();
            UpdateState();
        }

        #endregion

        #region The subscription lifecycle — every subscription has an unsubscribe

        private void subscribeButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "subscribeButton_Click", "the browser sent the click");
            Subscribe("operator");
            RenderCounters();
            UpdateState();
        }

        private void unsubscribeButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "unsubscribeButton_Click", "the browser sent the click");
            Unsubscribe("operator");
            RenderCounters();
            UpdateState();
        }

        /// <summary>Adds the handler once. _subscribed makes a second call a no-op, so the count can never drift.</summary>
        private void Subscribe(string reason)
        {
            if (_subscribed)
            {
                AddTrace(TraceDirection.Server, "hub.TicketChanged +=", "refused — this session is already subscribed (_subscribed == true); a double subscription would render every event twice");
                return;
            }

            _hub.TicketChanged += Hub_TicketChanged;
            _subscribed = true;

            if (!this.IsDisposed)
            {
                subscribeButton.Enabled = false;
                unsubscribeButton.Enabled = true;
                SetStatus($"tenant {_tenant} · subscribed", StatusKind.Normal);
            }
            AddTrace(TraceDirection.Server, "hub.TicketChanged +=", $"subscribed ({reason}) · hub subscribers = {_hub.SubscriberCount}");
        }

        /// <summary>
        /// Removes the handler once. Idempotent on purpose: ApplicationExit and Disposed both call it, and the
        /// operator can click Unsubscribe first. The hub's counter only moves when a handler was really removed.
        /// </summary>
        private void Unsubscribe(string reason)
        {
            if (!_subscribed)
            {
                if (!this.IsDisposed)
                    AddTrace(TraceDirection.Server, "hub.TicketChanged -=", $"nothing to do ({reason}) — this session was not subscribed");
                return;
            }

            _hub.TicketChanged -= Hub_TicketChanged;
            _subscribed = false;

            if (!this.IsDisposed)
            {
                subscribeButton.Enabled = true;
                unsubscribeButton.Enabled = false;
                SetStatus($"tenant {_tenant} · NOT subscribed — hub events no longer reach this session", StatusKind.Warn);
                AddTrace(TraceDirection.Server, "hub.TicketChanged -=", $"unsubscribed ({reason}) · hub subscribers = {_hub.SubscriberCount} · this session now receives nothing");
            }
            else
            {
                // The page is gone: the trace list is gone with it, so the evidence goes to the server log.
                Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} TicketHub: unsubscribed ({reason}) · subscribers now {_hub.SubscriberCount}");
            }
        }

        /// <summary>Cleanup #1 — the session is shutting down (tab closed, timeout, application exit).</summary>
        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            AddTraceSafe(TraceDirection.Server, "Application.ApplicationExit", "the session is shutting down → hub.TicketChanged -= Hub_TicketChanged");
            Unsubscribe("ApplicationExit");
        }

        /// <summary>Cleanup #2 — the page went away, the session may still live. Both paths end in the same method.</summary>
        private void MainPage_Disposed(object sender, EventArgs e)
        {
            _bursting = false;                                  // stop any running burst at its next check
            Unsubscribe("page disposed");

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

        #region The subscriber — a hub event arrives on a FOREIGN thread, in no session context

        /// <summary>
        /// The heart of the module. Called by TicketHub on the PUBLISHING session's thread (or on a burst task
        /// thread), with no context of its own. Three things happen, in this order:
        ///
        ///   1. the dead-session guard: if this page is disposed we return immediately — the hub must never try
        ///      to update a session that is gone (that is what a leaked subscription looks like);
        ///   2. the FILTER: the event carries TenantId; this session compares it with its own tenant and drops
        ///      what is not for it. Targeted update, decided by the session, out of metadata the hub supplied;
        ///   3. the push: Application.Update(_context, …) restores THIS session's context, applies every control
        ///      change and flushes them to this browser in one push.
        /// </summary>
        private void Hub_TicketChanged(object sender, TicketChangedEventArgs e)
        {
            // 1 — dead-session guard.
            if (this.IsDisposed)
                return;

            // 2 — the tenant filter. Even the "I dropped it" counter is a control, so it is updated in context too.
            if (!string.Equals(e.TenantId, _tenant, StringComparison.Ordinal))
            {
                SafeUpdate(() =>
                {
                    _filteredOut++;
                    AddTrace(TraceDirection.Server, "filtered out", $"(tenant {e.TenantId} ≠ this session's {_tenant}) event {e.EventId} ticket {e.Ticket?.Id} — dropped, nothing rendered");
                    RenderCounters();
                    UpdateState();
                });
                return;
            }

            // 3 — the push.
            SafeUpdate(() =>
            {
                ApplyTicketEvent(e);

                notificationsList.Items.Insert(0, $"{DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}  {e.Message}");
                while (notificationsList.Items.Count > 200)
                    notificationsList.Items.RemoveAt(notificationsList.Items.Count - 1);

                // The extension challenge: the count is an instance field, so it counts what THIS session saw,
                // even though the events come from a service every session shares.
                _notificationCount++;
                _pushes++;

                // The one EVENT TYPE that is filtered further: only an escalation interrupts the operator.
                if (string.Equals(e.ChangeType, "Escalated", StringComparison.Ordinal))
                {
                    AlertBox.Show(
                        $"Ticket {e.Ticket?.Id} escalated — {e.Ticket?.Title}",
                        MessageBoxIcon.Warning,
                        alignment: System.Drawing.ContentAlignment.TopRight,
                        autoCloseDelay: 4000);
                    AddTrace(TraceDirection.Server, "event type filter", $"ChangeType=Escalated → AlertBox toast (Added/Updated do not pop)");
                }

                bool mine = string.Equals(e.PublishedBy, Application.SessionId, StringComparison.Ordinal);   // SessionId: ClientId is the browser, shared by its tabs
                AddTrace(TraceDirection.Push, "Application.Update(_context)", $"event {e.EventId} · {e.ChangeType} #{e.Ticket?.Id} · published by {(mine ? "THIS session" : "another session " + Short(e.PublishedBy))} — applied in THIS session");

                RenderSelection();
                RenderCounters();
                UpdateState();
            });
        }

        /// <summary>
        /// Applies one event to this session's bound list, preserving the operator's selection: an event that
        /// arrives while a row is selected must not move the selection under the operator's mouse.
        /// </summary>
        private void ApplyTicketEvent(TicketChangedEventArgs e)
        {
            Ticket incoming = e.Ticket;
            if (incoming == null)
                return;

            int selectedId = SelectedTicketId();

            Ticket existing = _tickets.FirstOrDefault(t => t.Id == incoming.Id);
            if (existing == null)
                _tickets.Insert(0, incoming.Clone());       // the page keeps its own copy
            else
                existing.CopyFrom(incoming);

            ticketsBindingSource.ResetBindings(false);
            RestoreSelection(selectedId);
        }

        /// <summary>
        /// Every out-of-bound UI change of this page goes through here: the session context is restored, the
        /// changes are applied and flushed in ONE push, and a session that died in the meantime is swallowed
        /// (the hub logs it as a failing subscriber, the other sessions are unaffected).
        /// </summary>
        private void SafeUpdate(Action action)
        {
            try
            {
                Application.Update(_context, action);
            }
            catch (ObjectDisposedException)
            {
                // The page went away between the IsDisposed check and the push. Nothing to do.
            }
        }

        #endregion

        #region Success path — publishing (broadcast to a tenant)

        private void publishButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "publishButton_Click", "the browser sent the click");
            HideBanner();

            Ticket selected = SelectedTicket();
            Ticket ticket;

            if (selected != null)
            {
                // Update: take the hub's authoritative copy so two sessions editing the same ticket do not
                // fight over a stale local one.
                ticket = _hub.Find(selected.Id) ?? selected.Clone();
                ticket.Status = NextStatus(ticket.Status);
                ticket.Owner = NextOwner(ticket.Owner);
                ticket.UpdatedAt = DateTime.Now;
            }
            else
            {
                ticket = NewTicket(_tenant);
            }

            PublishAndTrace(ticket, null, selected != null ? "update" : "new ticket");
        }

        private void publishOtherButton_Click(object sender, EventArgs e)
        {
            string other = OtherTenant();
            AddTrace(TraceDirection.Request, "publishOtherButton_Click", $"the browser sent the click · target tenant \"{other}\" (NOT this session's)");
            HideBanner();

            Ticket ticket = NewTicket(other);
            PublishAndTrace(ticket, null, "new ticket for the other tenant");

            ShowBanner($"ⓘ Ticket {ticket.Id} was published for tenant {other}. This session watches {_tenant}, so the event was received and dropped — see “filtered out”. A {other} session shows it right now.", BannerKind.Info);
        }

        private void escalateButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "escalateButton_Click", "the browser sent the click");
            HideBanner();

            Ticket selected = SelectedTicket();
            if (selected == null)
            {
                ShowBanner("ⓘ Select a ticket in the board first — Escalate publishes ChangeType = \"Escalated\" for the selected ticket.", BannerKind.Info);
                AddTrace(TraceDirection.Server, "escalate", "refused — no row selected");
                return;
            }

            Ticket ticket = _hub.Find(selected.Id) ?? selected.Clone();
            ticket.Status = TicketStatus.Escalated;
            ticket.Owner = "escalation-desk";
            ticket.UpdatedAt = DateTime.Now;

            PublishAndTrace(ticket, "Escalated", "escalation");
        }

        /// <summary>
        /// The one place that talks to the hub. It traces what it is about to fan out, publishes, and then traces
        /// what came back — the event id lets the reviewer follow the same event across the traces of two tabs.
        /// </summary>
        private void PublishAndTrace(Ticket ticket, string changeType, string what)
        {
            int subscribers = _hub.SubscriberCount;
            AddTrace(TraceDirection.Server, "hub.AddOrUpdate", $"#{ticket.Id} → TicketChanged (tenant {ticket.TenantId}, subscribers {subscribers}) · {what} · raised outside the hub lock, fanned out on a thread-pool thread (never on this request thread)");

            try
            {
                TicketChangedEventArgs published = _hub.AddOrUpdate(ticket, Application.SessionId, changeType);
                AddTrace(TraceDirection.Server, "fan-out complete", $"event {published.EventId} · {published.ChangeType} · delivered to {subscribers} subscriber(s) · hub events = {_hub.EventsPublished}");
                SetStatus($"tenant {_tenant} · published #{ticket.Id} to {subscribers} subscriber(s)", StatusKind.Normal);
            }
            catch (ArgumentException ex)
            {
                HandleRejectedPublish(ex);
            }

            RenderSelection();
            RenderCounters();
            UpdateState();
        }

        #endregion

        #region Failure path — the hub rejects an invalid ticket and its state stays intact

        private void invalidButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "invalidButton_Click", "the browser sent the click");
            HideBanner();

            int ticketsBefore = _hub.TicketCount;
            int eventsBefore = _hub.EventsPublished;

            // No title, no tenant: the hub cannot route this and must not store it.
            var broken = new Ticket
            {
                Id = 7777,
                Title = "",
                TenantId = "",
                Customer = "Unknown",
                Owner = "nobody",
                Status = TicketStatus.New,
                UpdatedAt = DateTime.Now
            };

            AddTrace(TraceDirection.Server, "hub.AddOrUpdate", "#7777 with an empty Title and an empty TenantId — the hub validates BEFORE it takes the lock");

            try
            {
                _hub.AddOrUpdate(broken, Application.SessionId);
                AddTrace(TraceDirection.Server, "hub.AddOrUpdate", "unexpected: the hub accepted an invalid ticket");
            }
            catch (ArgumentException ex)
            {
                HandleRejectedPublish(ex);
            }

            AddTrace(TraceDirection.Server, "hub state after the rejection", $"tickets {ticketsBefore} → {_hub.TicketCount}, events {eventsBefore} → {_hub.EventsPublished} — unchanged, and no subscriber was called");
            RenderCounters();
            UpdateState();
        }

        private void HandleRejectedPublish(ArgumentException ex)
        {
            // The detail goes to the server log; the operator gets a safe, actionable message and can keep working.
            LogError("hub.AddOrUpdate", ex);
            ShowBanner($"✖ The hub rejected the publish: {ex.Message.Split('(')[0].Trim()} Nothing was stored and no session was notified — fix the ticket and publish again.", BannerKind.Error);
            SetStatus("publish rejected by the hub — state unchanged", StatusKind.Error);
            AddTrace(TraceDirection.Server, "ArgumentException", $"{ex.GetType().Name} caught by the caller → banner · the hub's list and its subscribers were never touched");
        }

        #endregion

        #region Progress + cadence — 20 events from a task, 150 ms apart

        private void burstButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "burstButton_Click", "the browser sent the click");

            if (_bursting)
            {
                AddTrace(TraceDirection.Server, "burst", "refused — a burst is already running (_bursting == true)");
                ShowBanner("⚠ A burst is already running — a second click must not start a competing one.", BannerKind.Warn);
                return;
            }

            if (_tickets.Count == 0)
            {
                ShowBanner($"ⓘ There is no ticket for tenant {_tenant} to update — publish one first.", BannerKind.Info);
                return;
            }

            _bursting = true;
            burstButton.Enabled = false;
            HideBanner();
            SetStatus($"burst running · {BurstEvents} events, {BurstIntervalMs} ms apart", StatusKind.Normal);
            AddTrace(TraceDirection.Server, "Application.StartTask", $"burst: {BurstEvents} hub publishes, {BurstIntervalMs} ms apart · every subscribed {_tenant} session renders each one");
            UpdateState();
            BeginPush();

            // The ids are captured now, on the request thread, so the task never touches the bound list.
            int[] ids = _tickets.Select(t => t.Id).ToArray();
            string tenant = _tenant;
            string publisher = Application.SessionId;

            Application.StartTask(() => RunBurst(ids, tenant, publisher));
        }

        /// <summary>
        /// The out-of-bound publisher. It only talks to the hub; the UI of this session is updated by the very
        /// same Hub_TicketChanged handler that serves the other sessions — one code path for every subscriber.
        /// Bounded (20 iterations), stoppable (_bursting / IsDisposed), caught inside, restored in finally.
        /// </summary>
        private void RunBurst(int[] ids, string tenant, string publisher)
        {
            int published = 0;
            string outcome = "completed";

            try
            {
                for (int i = 1; i <= BurstEvents && _bursting && !this.IsDisposed; i++)
                {
                    int id = ids[(i - 1) % ids.Length];
                    Ticket ticket = _hub.Find(id);
                    if (ticket == null)
                        continue;

                    ticket.Status = NextStatus(ticket.Status);
                    ticket.Owner = NextOwner(ticket.Owner);
                    ticket.UpdatedAt = DateTime.Now;

                    // Cadence lives HERE, with the publisher. The hub raises one event per call, synchronously;
                    // if this loop had no Sleep it would flood every subscribed session of this tenant.
                    _hub.AddOrUpdate(ticket, publisher);
                    published++;

                    Thread.Sleep(BurstIntervalMs);
                }

                if (this.IsDisposed) outcome = "page disposed";
                else if (!_bursting) outcome = "stopped";
            }
            catch (Exception ex)
            {
                outcome = "faulted: " + ex.Message;
                LogError("burst", ex);
            }
            finally
            {
                _bursting = false;
                if (!this.IsDisposed)
                {
                    SafeUpdate(() =>
                    {
                        burstButton.Enabled = true;
                        _pushes++;
                        EndPush();
                        SetStatus($"burst {outcome} · {published} events published", outcome == "completed" ? StatusKind.Normal : StatusKind.Warn);
                        AddTrace(TraceDirection.Push, "Application.Update(_context)", $"burst {outcome} — {published}/{BurstEvents} events published to tenant {tenant} (final state, finally block)");
                        RenderCounters();
                        UpdateState();
                    });
                }
            }
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
            AddTrace(TraceDirection.Server, "Application.StartPolling(1000)", "no WebSocket when the task started → fallback polling ON until the task ends");
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

        #region A second session

        private void openSessionButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "openSessionButton_Click", "Application.Navigate(\"/\", \"_blank\") — a new tab is a NEW session with its own MainPage, its own context and its own subscription");
            Application.Navigate("/", "_blank");
        }

        #endregion

        #region Selection, counters, trace and UI helpers

        private enum StatusKind { Normal, Warn, Error }
        private enum BannerKind { Info, Warn, Error }

        private void ticketsGrid_SelectionChanged(object sender, EventArgs e)
        {
            RenderSelection();
        }

        /// <summary>The ticket behind the selected row, or null.</summary>
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

        /// <summary>Puts the selection back on the same ticket after a rebind — the operator keeps their row.</summary>
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

        private void RenderSelection()
        {
            Ticket selected = SelectedTicket();
            selectedLabel.Text = selected == null
                ? "no row selected — Publish opens a NEW ticket; select a row to update or escalate it"
                : $"selected #{selected.Id} · {selected.TenantId} · {selected.Status} · {selected.Owner} · updated {selected.UpdatedAt:HH:mm:ss}";
        }

        private void RenderCounters()
        {
            notificationCountLabel.Text = _notificationCount == 1
                ? "1 notification in this session"
                : $"{_notificationCount} notifications in this session";

            int subscribers = _hub.SubscriberCount;
            subscribersLabel.Text = $"hub subscribers: {subscribers}" + (_subscribed ? " (this session included)" : " (this session NOT subscribed)");
            subscribersLabel.ForeColor = _subscribed
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(224, 86, 59);

            filteredOutLabel.Text = $"filtered out (wrong tenant): {_filteredOut}";
            hubStatsLabel.Text = $"hub: {_hub.TicketCount} tickets · {_hub.EventsPublished} events published (global — every session sees these same numbers)";
        }

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

        /// <summary>AddTrace for the shutdown paths: the page may already be gone, and that is not an error.</summary>
        private void AddTraceSafe(TraceDirection direction, string name, string payload)
        {
            if (this.IsDisposed)
                return;
            try
            {
                AddTrace(direction, name, payload);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            listTrace.Items.Clear();
            notificationsList.Items.Clear();
            AddTrace(TraceDirection.Request, "clearButton_Click", "trace and notifications cleared (in-request update) — the counters keep counting");
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

            labelState.Text =
                $"SERVER STATE (authoritative · this session only, except the hub line)\n" +
                $"tenant={_tenant} subscribed={Low(_subscribed)} hub.subscribers={_hub.SubscriberCount} hub.events={_hub.EventsPublished}\n" +
                $"notifications(this session)={_notificationCount} filteredOut={_filteredOut} pushes={_pushes} ticketsShown={_tickets.Count} burst={Low(_bursting)} polling={Low(_polling)}\n" +
                $"IsWebSocket={Low(Application.IsWebSocket)} ClientId={Application.ClientId} SessionId={session}\n" +
                $"the tenant, the list and the counters are MainPage instance fields · TicketHub.Instance is the only shared object, and it holds no control";
        }

        #endregion

        #region Ticket helpers (simulated data — no database)

        private string OtherTenant()
        {
            return string.Equals(_tenant, "Contoso", StringComparison.Ordinal) ? "Northwind" : "Contoso";
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

        private static string Short(string id)
        {
            if (string.IsNullOrEmpty(id))
                return "?";
            return id.Length > 8 ? id.Substring(0, 8) + "…" : id;
        }

        /// <summary>Server log: the technical detail stays here; the UI only gets a safe message.</summary>
        private static void LogError(string operation, Exception ex)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} {operation} failed for client {Application.ClientId} session {Application.SessionId}: {ex}");
        }

        private static string Low(bool value) => value ? "true" : "false";

        #endregion
    }
}
