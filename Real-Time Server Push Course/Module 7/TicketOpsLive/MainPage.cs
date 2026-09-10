using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using TicketOpsLive.Models;
using TicketOpsLive.Services;
using Wisej.Core;
using Wisej.Web;

namespace TicketOpsLive
{
    public enum TraceDirection { Push, Request, Server }

    /// <summary>
    /// TicketOps Live — Module 7 · Production Review (the capstone).
    ///
    /// Everything the course built, in one session, on one page:
    ///   status strip   M01 · heartbeat on a task, pushed once per second
    ///   Import monitor M03 · 200 records, push every 10, cancel, fail at 87, UI restored in finally
    ///   Ticket board   M05 + M06 · a session-owned bound list fed by the GLOBAL TicketHub, tenant-filtered
    ///   Cadence        M04 · 50 ms model, dirty flag, one UI refresh per timer tick, polling fallback
    ///   Session        M02 · Application.Session vs the static trap, Application.Current, live session registry
    ///   Health         M07 · RealtimeHealthSnapshot + RenderHealth, effective configuration, the checklist
    ///
    /// The architectural rules the capstone is graded on are enforced in ONE place each:
    ///   - every loop has a stop condition and an IsDisposed guard;
    ///   - every subscription is removed by the single idempotent <see cref="Cleanup"/>;
    ///   - every push is counted, so the health panel reports a measured update rate;
    ///   - no page, control or context is ever handed to a global service.
    /// </summary>
    public partial class MainPage : Page
    {
        private const int TotalRecords = 200;
        private const int RecordMilliseconds = 25;
        private const int PushEvery = 10;
        private const int LogEvery = 50;
        private const int MinCadenceMs = 250;
        private const int SimulatedTickets = 10;

        // ── Per-session state: instance fields of THIS page (one MainPage per browser tab) ────────────────
        private IWisejComponent _context;          // captured on Load for out-of-bound code
        private string _sessionId = "";            // the registry / hub key: SessionId is the tab, ClientId is the browser
        private string _clientId = "";

        // M01 heartbeat
        private volatile bool _heartbeatRunning;
        private int _heartbeat;

        // M03 import
        private CancellationTokenSource _importCts;
        private volatile bool _importRunning;
        private ImportJob _currentJob;
        private int _completed, _cancelled, _failed;

        // M05/M06 board
        private readonly List<Ticket> _allTickets = new List<Ticket>();
        private readonly BindingList<Ticket> _tickets = new BindingList<Ticket>();
        private readonly BindingSource _ticketsBindingSource = new BindingSource();
        private string _tenant = TicketHub.Tenants[0];
        private bool _subscribedToHub;
        private bool _restoringSelection;
        private int _notificationCount, _filteredOut, _eventsApplied;
        private volatile bool _feedRunning;

        // M04 cadence
        private readonly DashboardModel _model = new DashboardModel(Environment.TickCount);
        private DashboardSimulator _simulator;
        private volatile bool _dashboardDirty;
        private bool _refreshInProgress;
        private bool _liveMode;
        private int _cadenceMs = 1000;
        private int _eventsReceived, _updatesApplied, _ticksRefused;
        private bool _wiringDone;                  // handlers are wired after the initial fills

        // M02 session
        private static int _sharedCounter;         // THE TRAP, kept on purpose: shared by every session of this server
        private bool _registered;

        // M07 delivery + health
        private int _pushers;                      // out-of-bound work in flight
        private bool _polling;
        private int _pushes;
        private readonly Queue<DateTime> _pushTimes = new Queue<DateTime>();
        private DateTime? _lastPushUtc;
        private bool _cleanedUp;

        public MainPage()
        {
            InitializeComponent();

            _simulator = new DashboardSimulator(_model, SimulatedModelChanged, () => this.IsDisposed, SimulatorFailed);

            // Closing the tab is a stop condition for every loop and the last chance to unsubscribe.
            this.Disposed += (s, e) => Cleanup("Disposed");
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";

            // The context of THIS session, captured while we are in it. Out-of-bound code (the hub fan-out and
            // the registry both run on thread-pool threads) has no context of its own and would not know which
            // browser to update; Application.Update(_context, …) restores this one.
            _context = Application.Current;
            _clientId = Application.ClientId ?? "";
            _sessionId = Application.SessionId ?? "";

            BuildTicketGrid();
            FillTenants();
            FillCadences();
            BuildChecklist();

            // Register first, then subscribe: the session must not receive its own "joined" event mid-Load.
            _registered = SessionRegistry.Instance.Register(_sessionId);
            SessionRegistry.Instance.SessionsChanged += Registry_SessionsChanged;
            SubscribeToHub("page load");
            Application.ApplicationExit += Application_ApplicationExit;
            Application.SessionTimeout += Application_SessionTimeout;

            LoadSnapshot("page load");
            LoadConfiguration();

            // The handlers are wired only now: filling the combos and the list above must not look like user input.
            tenantComboBox.SelectedIndexChanged += new EventHandler(this.tenantComboBox_SelectedIndexChanged);
            cadenceComboBox.SelectedIndexChanged += new EventHandler(this.cadenceComboBox_SelectedIndexChanged);
            escalatedOnlyCheckBox.CheckedChanged += new EventHandler(this.escalatedOnlyCheckBox_CheckedChanged);
            liveModeCheckBox.CheckedChanged += new EventHandler(this.liveModeCheckBox_CheckedChanged);
            ticketsGrid.SelectionChanged += new EventHandler(this.ticketsGrid_SelectionChanged);
            checklistBox.AfterItemCheck += new ItemCheckEventHandler(this.checklistBox_AfterItemCheck);
            _wiringDone = true;

            healthTimer.Start();

            LifecycleLog("Page loaded for this session.");
            AddTrace(TraceDirection.Request, "MainPage_Load", $"session {Short(_sessionId)} of client {Short(_clientId)} · IsWebSocket={Low(Application.IsWebSocket)} (the socket opens after this response)");
            AddTrace(TraceDirection.Server, "wiring", "registry registered · hub subscribed · ApplicationExit + SessionTimeout armed · healthTimer started (2 s)");
            SetStatus("capstone ready · every feature is one tab away", StatusKind.Normal);
            RenderInspector("request thread (MainPage_Load)");
            RenderLiveSessions();
            RenderBoardCounters();
            RenderHealth(BuildHealthSnapshot());
            UpdateState();
        }

        #region M01 — the heartbeat (status strip)

        private void startButton_Click(object sender, EventArgs e)
        {
            if (_heartbeatRunning)
            {
                AddTrace(TraceDirection.Server, "startButton_Click", "refused — the heartbeat is already running (_heartbeatRunning == true)");
                ShowBanner("⚠ Heartbeat already running — a second click must not start a competing loop.", BannerKind.Warn);
                return;
            }

            _heartbeatRunning = true;
            startButton.Enabled = false;
            stopButton.Enabled = true;
            HideBanner();
            activityLabel.Text = "Heartbeat starting…";
            AddTrace(TraceDirection.Request, "startButton_Click", "the handler returns immediately; the loop runs on a task");
            AddTrace(TraceDirection.Server, "Application.StartTask", "heartbeat: one push per second while _heartbeatRunning && !IsDisposed");
            SetStatus("heartbeat running", StatusKind.Normal);
            BeginPush();

            Application.StartTask(HeartbeatLoop);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _heartbeatRunning = false;
            stopButton.Enabled = false;
            activityLabel.Text = "Heartbeat stopping… (ends within one second)";
            AddTrace(TraceDirection.Request, "stopButton_Click", "_heartbeatRunning = false → the loop exits at its next check");
        }

        /// <summary>Compute off the request thread, then apply and push in ONE flush. finally always restores the UI.</summary>
        private void HeartbeatLoop()
        {
            var random = new Random();
            string reason = "stopped by operator";
            bool faulted = false;

            try
            {
                while (_heartbeatRunning && !this.IsDisposed)
                {
                    int n = ++_heartbeat;
                    int load = random.Next(5, 95);
                    DateTime now = DateTime.Now;

                    Application.Update(this, () =>
                    {
                        clockLabel.Text = now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                        serverLoadBar.Value = load;
                        loadValueLabel.Text = load + " %";
                        activityLabel.Text = $"Heartbeat #{n} · load {load}% · pushed {now:HH:mm:ss.fff}";
                        NotePush();
                        RenderConnection();
                        AddTrace(TraceDirection.Push, "Application.Update(this, …)", $"heartbeat #{n}: clock, load {load}%, activity — one flush");
                        UpdateState();
                    });

                    Thread.Sleep(1000);      // cadence: once per second is plenty for a status strip
                }

                if (this.IsDisposed) reason = "page disposed";
            }
            catch (Exception ex)
            {
                faulted = true;
                reason = "fault: " + ex.Message;
                LogError("heartbeat", ex);
            }
            finally
            {
                _heartbeatRunning = false;
                if (!this.IsDisposed)
                {
                    try
                    {
                        Application.Update(this, () =>
                        {
                            startButton.Enabled = true;
                            stopButton.Enabled = false;
                            activityLabel.Text = faulted ? "Heartbeat stopped after a fault" : "Heartbeat stopped";
                            if (faulted)
                            {
                                ShowBanner($"✖ Heartbeat failed. See the server log for session {Short(_sessionId)}. Click ▶ Start heartbeat to recover.", BannerKind.Error);
                                SetStatus("heartbeat fault — UI restored in finally", StatusKind.Error);
                            }
                            else
                            {
                                SetStatus("heartbeat stopped", StatusKind.Normal);
                            }
                            NotePush();
                            EndPush();
                            AddTrace(TraceDirection.Push, "Application.Update(this, …)", $"heartbeat stopped — {reason} after {_heartbeat} beats (finally block)");
                            UpdateState();
                        });
                    }
                    catch (ObjectDisposedException) { /* the page went away between the check and the push */ }
                }
            }
        }

        #endregion

        #region M03 — the background import monitor

        private void startImportButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "startImportButton_Click", "the browser sent the click");

            if (_importRunning)
            {
                AddTrace(TraceDirection.Server, "startImportButton_Click", $"refused — job {_currentJob?.JobId} is still running");
                ShowBanner($"⚠ Import {_currentJob?.JobId} is still running — a second click must not start a competing job.", BannerKind.Warn);
                return;
            }

            bool failAt87 = failAt87CheckBox.Checked;
            var job = new ImportJob(TotalRecords, "background");
            _currentJob = job;
            _importRunning = true;

            // 1 — adjust the UI (these changes ride back on the click's own response).
            startImportButton.Enabled = false;
            cancelImportButton.Enabled = true;
            failAt87CheckBox.Enabled = false;
            importProgressBar.Value = 0;
            recordsImportedLabel.Text = "0";
            jobIdLabel.Text = "Job " + job.JobId + (failAt87 ? "  ·  will throw at record 87" : "");
            elapsedLabel.Text = "elapsed 0.00 s";
            importStatusLabel.Text = "Starting import…";
            HideBanner();
            SetStatus($"import {job.JobId} running", StatusKind.Normal);
            ImportLog(job, failAt87 ? "Started (simulated failure armed at record 87)" : "Started");

            // 2 — cancellation state, per session, never static.
            _importCts = new CancellationTokenSource();
            CancellationToken token = _importCts.Token;

            AddTrace(TraceDirection.Server, "Application.StartTask", $"job {job.JobId}: {TotalRecords} records × {RecordMilliseconds} ms, push every {PushEvery} · the handler returns now");
            UpdateState();
            BeginPush();

            // 3 — start in the session context.
            Application.StartTask(() => RunImport(token, failAt87));
        }

        private void cancelImportButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "cancelImportButton_Click", $"_importCts.Cancel() → job {_currentJob?.JobId} sees the token at its next record (≤ {RecordMilliseconds} ms)");
            cancelImportButton.Enabled = false;
            importStatusLabel.Text = "Cancelling…";
            RequestImportCancel("user");
        }

        private void RequestImportCancel(string who)
        {
            var cts = _importCts;
            if (cts == null)
                return;
            try
            {
                cts.Cancel();
                if (_currentJob != null && !this.IsDisposed)
                    ImportLog(_currentJob, "Cancel requested by " + who);
            }
            catch (ObjectDisposedException) { /* the job finished between the check and the call */ }
        }

        /// <summary>200 records, the token checked between records, progress pushed every tenth, UI restored in finally.</summary>
        private void RunImport(CancellationToken token, bool failAt87)
        {
            ImportJob job = _currentJob;

            try
            {
                for (int i = 1; i <= TotalRecords; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(RecordMilliseconds);

                    if (failAt87 && i == 87)
                        throw new InvalidOperationException($"Simulated malformed record #{i}: field 'priority' has value 'urgentish' (job {job.JobId}).");

                    job.RecordsImported = i;
                    recordsImportedLabel.Text = i.ToString(CultureInfo.InvariantCulture);
                    importProgressBar.Value = i / 2;
                    importStatusLabel.Text = $"Importing… {i}/{TotalRecords}";
                    elapsedLabel.Text = "elapsed " + job.ElapsedText;

                    if (i % LogEvery == 0)
                        ImportLog(job, $"Imported {i} records");

                    if (i % PushEvery == 0)
                    {
                        job.Pushes++;
                        NotePush();
                        AddTrace(TraceDirection.Push, "Application.Update(this)", $"job {job.JobId} · {i}/{TotalRecords} · {i / 2}% · {job.ElapsedText}");
                        UpdateState();
                        Application.Update(this);
                    }
                }

                job.Complete();
                importStatusLabel.Text = "Import completed successfully.";
                ImportLog(job, "Completed");
                AddTrace(TraceDirection.Server, "import", $"job {job.JobId} completed — {job.RecordsImported} records");
            }
            catch (OperationCanceledException)
            {
                job.Cancel();
                importStatusLabel.Text = "Import cancelled by user.";
                ImportLog(job, $"Cancelled after record {job.RecordsImported}");
                AddTrace(TraceDirection.Server, "import", $"job {job.JobId} cancelled after record {job.RecordsImported} (OperationCanceledException caught inside the task)");
            }
            catch (Exception ex)
            {
                job.Fail(ex);
                LogError($"import job {job.JobId}", ex);
                importStatusLabel.Text = "Import failed. Review the server log.";
                ImportLog(job, $"Failed on record {job.RecordsImported + 1}");
                AddTrace(TraceDirection.Server, "import", $"job {job.JobId} FAILED on record {job.RecordsImported + 1}: {ex.GetType().Name} caught inside the task → server log, safe message");
            }
            finally
            {
                var cts = _importCts;
                _importCts = null;
                cts?.Dispose();
                _importRunning = false;
                switch (job.Outcome)
                {
                    case ImportOutcome.Completed: _completed++; break;
                    case ImportOutcome.Cancelled: _cancelled++; break;
                    case ImportOutcome.Failed: _failed++; break;
                }
                if (ReferenceEquals(_currentJob, job))
                    _currentJob = null;

                if (!this.IsDisposed)
                {
                    try
                    {
                        Application.Update(this, () =>
                        {
                            startImportButton.Enabled = true;
                            cancelImportButton.Enabled = false;
                            failAt87CheckBox.Enabled = true;
                            elapsedLabel.Text = "elapsed " + job.ElapsedText;
                            jobIdLabel.Text = "Job " + job.JobId + " · " + job.OutcomeText;
                            ImportLog(job, job.Summary);

                            switch (job.Outcome)
                            {
                                case ImportOutcome.Failed:
                                    ShowBanner($"✖ {job.Summary}. The detail is in the server log under job {job.JobId}. Click ▶ Start import to recover.", BannerKind.Error);
                                    SetStatus("import failed — UI restored in finally", StatusKind.Error);
                                    break;
                                case ImportOutcome.Cancelled:
                                    ShowBanner($"ⓘ {job.Summary}. Records 1–{job.RecordsImported} are in; nothing was half-written.", BannerKind.Info);
                                    SetStatus("import cancelled — UI restored in finally", StatusKind.Warn);
                                    break;
                                default:
                                    SetStatus("import completed", StatusKind.Normal);
                                    break;
                            }

                            job.Pushes++;
                            NotePush();
                            EndPush();
                            AddTrace(TraceDirection.Push, "Application.Update(this, …)", $"{job.Summary} · {job.Pushes} pushes (final state, finally block)");
                            UpdateState();
                        });
                    }
                    catch (ObjectDisposedException) { /* the page went away */ }
                }
            }
        }

        #endregion

        #region M05 + M06 — the live ticket board fed by the global TicketHub

        /// <summary>Columns in code so the file stays readable; the binding itself is set up once, on Load.</summary>
        private void BuildTicketGrid()
        {
            _ticketsBindingSource.DataSource = _tickets;

            ticketsGrid.AutoGenerateColumns = false;
            ticketsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ticketsGrid.MultiSelect = false;
            ticketsGrid.ReadOnly = true;
            ticketsGrid.AllowUserToAddRows = false;
            ticketsGrid.AllowUserToDeleteRows = false;

            ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", DataPropertyName = "Id", HeaderText = "Id", Width = 64 });
            ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTenant", DataPropertyName = "TenantId", HeaderText = "Tenant", Width = 92 });
            ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTitle", DataPropertyName = "Title", HeaderText = "Title", Width = 230 });
            ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOwner", DataPropertyName = "Owner", HeaderText = "Owner", Width = 110 });
            ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", DataPropertyName = "Status", HeaderText = "Status", Width = 90 });

            var updated = new DataGridViewTextBoxColumn { Name = "colUpdated", DataPropertyName = "UpdatedAt", HeaderText = "Updated", Width = 80 };
            updated.DefaultCellStyle.Format = "HH:mm:ss";
            ticketsGrid.Columns.Add(updated);

            ticketsGrid.DataSource = _ticketsBindingSource;
        }

        private void FillTenants()
        {
            foreach (string tenant in TicketHub.Tenants)
                tenantComboBox.Items.Add(tenant);
            tenantComboBox.SelectedIndex = 0;
            _tenant = TicketHub.Tenants[0];
        }

        /// <summary>Loads THIS session's list from the hub's snapshot (clones, never the hub's instances).</summary>
        private void LoadSnapshot(string why)
        {
            var snapshot = TicketHub.Instance.GetSnapshot();
            _allTickets.Clear();
            foreach (Ticket ticket in snapshot)
                _allTickets.Add(ticket);       // GetSnapshot already handed out clones

            ApplyFilter();
            ResetBindingsQuietly();
            AddTrace(TraceDirection.Server, "hub.GetSnapshot()", $"{snapshot.Count} ticket(s) in the hub → {_tickets.Count} shown for tenant \"{_tenant}\" ({why})");
        }

        private void SubscribeToHub(string why)
        {
            if (_subscribedToHub)
                return;
            TicketHub.Instance.TicketChanged += Hub_TicketChanged;
            _subscribedToHub = true;
            subscribeButton.Enabled = false;
            unsubscribeButton.Enabled = true;
            AddTrace(TraceDirection.Server, "hub.TicketChanged +=", $"subscribed ({why}) · hub subscribers = {TicketHub.Instance.SubscriberCount}");
        }

        private void UnsubscribeFromHub(string why)
        {
            if (!_subscribedToHub)
                return;
            TicketHub.Instance.TicketChanged -= Hub_TicketChanged;
            _subscribedToHub = false;
            if (!this.IsDisposed)
            {
                subscribeButton.Enabled = true;
                unsubscribeButton.Enabled = false;
                AddTrace(TraceDirection.Server, "hub.TicketChanged -=", $"unsubscribed ({why}) · hub subscribers = {TicketHub.Instance.SubscriberCount} · this session now receives nothing");
            }
        }

        private void subscribeButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "subscribeButton_Click", "the browser sent the click");
            SubscribeToHub("operator");
            LoadSnapshot("re-subscribe");
            RenderBoardCounters();
            SetStatus("subscribed to the hub", StatusKind.Normal);
            UpdateState();
        }

        private void unsubscribeButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "unsubscribeButton_Click", "the browser sent the click");
            UnsubscribeFromHub("operator");
            RenderBoardCounters();
            SetStatus("unsubscribed — this session receives no hub events", StatusKind.Warn);
            UpdateState();
        }

        private void publishButton_Click(object sender, EventArgs e) => Publish(_tenant, "the browser sent the click");

        private void publishOtherButton_Click(object sender, EventArgs e)
        {
            string other = string.Equals(_tenant, TicketHub.Tenants[0], StringComparison.Ordinal) ? TicketHub.Tenants[1] : TicketHub.Tenants[0];
            Publish(other, $"target tenant \"{other}\" (NOT this session's)");
        }

        private void Publish(string tenant, string why)
        {
            AddTrace(TraceDirection.Request, "publish", why);
            try
            {
                Ticket ticket = NewTicketFor(tenant);
                var published = TicketHub.Instance.AddOrUpdate(ticket, _sessionId);
                AddTrace(TraceDirection.Server, "hub.AddOrUpdate", $"#{ticket.Id} → TicketChanged (tenant {tenant}, subscribers {TicketHub.Instance.SubscriberCount}) · event {published.EventId} · fanned out on a thread-pool thread");
                SetStatus($"published #{ticket.Id} for {tenant}", StatusKind.Normal);
            }
            catch (ArgumentException ex)
            {
                LogError("hub.AddOrUpdate", ex);
                ShowBanner($"✖ The hub rejected the publish: {ex.Message} Nothing was stored and no session was notified.", BannerKind.Error);
                SetStatus("publish rejected by the hub — state unchanged", StatusKind.Error);
                AddTrace(TraceDirection.Server, "ArgumentException", "validated BEFORE the lock → the hub's list and its subscribers were never touched");
            }
            UpdateState();
        }

        private void escalateButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "escalateButton_Click", "the browser sent the click");
            Ticket selected = ticketsGrid.CurrentRow?.DataBoundItem as Ticket;
            if (selected == null)
            {
                ShowBanner("ⓘ Select a ticket first — escalation is the one event type that also pops a toast.", BannerKind.Info);
                return;
            }

            Ticket copy = selected.Clone();
            copy.Status = TicketStatus.Escalated;
            copy.Owner = "escalation-desk";
            copy.UpdatedAt = DateTime.Now;
            var published = TicketHub.Instance.AddOrUpdate(copy, _sessionId, "Escalated");
            AddTrace(TraceDirection.Server, "hub.AddOrUpdate", $"#{copy.Id} → TicketChanged (Escalated, tenant {copy.TenantId}, subscribers {TicketHub.Instance.SubscriberCount}) · event {published.EventId}");
            SetStatus($"escalated #{copy.Id}", StatusKind.Warn);
            UpdateState();
        }

        private void newTicketsButton_Click(object sender, EventArgs e)
        {
            if (_feedRunning)
            {
                AddTrace(TraceDirection.Server, "newTicketsButton_Click", "refused — the simulated feed is already running");
                ShowBanner("⚠ The feed is already running — one loop per session.", BannerKind.Warn);
                return;
            }

            _feedRunning = true;
            newTicketsButton.Enabled = false;
            AddTrace(TraceDirection.Request, "newTicketsButton_Click", $"the handler returns now; {SimulatedTickets} tickets are published from a task, one per second");
            BeginPush();

            Application.StartTask(() =>
            {
                int published = 0;
                try
                {
                    for (int i = 0; i < SimulatedTickets && _feedRunning && !this.IsDisposed; i++)
                    {
                        Ticket ticket = NewTicketFor(_tenant);
                        TicketHub.Instance.AddOrUpdate(ticket, _sessionId, "Added");
                        published++;
                        Thread.Sleep(1000);      // cadence belongs to the publisher
                    }
                }
                catch (Exception ex)
                {
                    LogError("simulated feed", ex);
                }
                finally
                {
                    _feedRunning = false;
                    if (!this.IsDisposed)
                    {
                        try
                        {
                            Application.Update(this, () =>
                            {
                                newTicketsButton.Enabled = true;
                                NotePush();
                                EndPush();
                                AddTrace(TraceDirection.Push, "Application.Update(this, …)", $"simulated feed finished — {published} ticket(s) published through the hub (finally block)");
                                UpdateState();
                            });
                        }
                        catch (ObjectDisposedException) { }
                    }
                }
            });
        }

        private Ticket NewTicketFor(string tenant)
        {
            string[] titles =
            {
                "Invoice export fails with a timeout", "SSO certificate expiry warning", "Payment webhook delivered twice",
                "Two-factor SMS never arrives", "Dashboard tiles blank after update", "Badge reader offline in B2",
            };
            string[] customers = { "Contoso Manufacturing", "Northwind Logistics", "Litware Insurance", "Proseware Health", "Tailspin Toys" };
            var random = new Random(Environment.TickCount + _pushes);
            return new Ticket
            {
                Id = TicketHub.Instance.NextTicketId(tenant),
                Title = titles[random.Next(titles.Length)],
                Customer = customers[random.Next(customers.Length)],
                TenantId = tenant,
                Owner = "dispatcher",
                Status = TicketStatus.New,
                UpdatedAt = DateTime.Now,
            };
        }

        /// <summary>
        /// Runs on a thread-pool thread (the hub fans out with Task.Run so no session context leaks into another
        /// session's response). Guard, filter, then re-enter THIS session with Application.Update(_context, …).
        /// </summary>
        private void Hub_TicketChanged(object sender, TicketChangedEventArgs e)
        {
            if (this.IsDisposed)
                return;

            if (!string.Equals(e.TenantId, _tenant, StringComparison.Ordinal))
            {
                SafeUpdate(() =>
                {
                    _filteredOut++;
                    AddTrace(TraceDirection.Server, "filtered out", $"(tenant {e.TenantId} ≠ this session's {_tenant}) event {e.EventId} ticket {e.Ticket?.Id} — dropped, nothing rendered");
                    RenderBoardCounters();
                    UpdateState();
                });
                return;
            }

            SafeUpdate(() =>
            {
                ApplyTicketEvent(e);

                notificationsList.Items.Insert(0, $"{DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}  {e.Message}");
                while (notificationsList.Items.Count > 200)
                    notificationsList.Items.RemoveAt(notificationsList.Items.Count - 1);

                _notificationCount++;
                NotePush();

                if (string.Equals(e.ChangeType, "Escalated", StringComparison.Ordinal))
                {
                    AlertBox.Show($"Ticket {e.Ticket?.Id} escalated — {e.Ticket?.Title}", MessageBoxIcon.Warning,
                        alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                    AddTrace(TraceDirection.Server, "event type filter", "ChangeType=Escalated → AlertBox toast (Added/Updated do not pop)");
                }

                bool mine = string.Equals(e.PublishedBy, _sessionId, StringComparison.Ordinal);
                AddTrace(TraceDirection.Push, "Application.Update(_context)", $"event {e.EventId} · {e.ChangeType} #{e.Ticket?.Id} · published by {(mine ? "THIS session" : "another session " + Short(e.PublishedBy))} — applied here");
                RenderBoardCounters();
                UpdateState();
            });
        }

        /// <summary>Applies one event to the session's own list, keeping the operator's selected row.</summary>
        private void ApplyTicketEvent(TicketChangedEventArgs e)
        {
            if (e?.Ticket == null || e.Ticket.Id <= 0)
            {
                AddTrace(TraceDirection.Server, "ApplyTicketEvent", "REJECTED — malformed event · the bound list was not touched");
                return;
            }

            int? selectedId = ticketsGrid.CurrentRow?.DataBoundItem is Ticket selected ? selected.Id : (int?)null;

            Ticket existing = null;
            for (int i = 0; i < _allTickets.Count; i++)
            {
                if (_allTickets[i].Id == e.Ticket.Id) { existing = _allTickets[i]; break; }
            }

            bool added = existing == null;
            if (added)
                _allTickets.Insert(0, e.Ticket.Clone());     // new rows sort to the top
            else
                existing.CopyFrom(e.Ticket);                 // existing rows change in place, keeping their position

            ApplyFilter();
            ResetBindingsQuietly();
            bool kept = selectedId.HasValue && RestoreSelection(selectedId.Value);
            _eventsApplied++;
            RenderSelection();

            if (selectedId.HasValue && !kept)
                AddTrace(TraceDirection.Server, "selection", $"#{selectedId.Value} is not in the filtered view any more");
        }

        /// <summary>
        /// ResetBindings(false) makes the grid re-evaluate its current row and raises SelectionChanged
        /// synchronously — that is not a user action, so the handler is muted for the duration.
        /// </summary>
        private void ResetBindingsQuietly()
        {
            _restoringSelection = true;
            try
            {
                _ticketsBindingSource.ResetBindings(false);
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

        /// <summary>Rebuilds the bound list from the master list (tenant + Escalated filter). Session-owned, always.</summary>
        private void ApplyFilter()
        {
            bool escalatedOnly = escalatedOnlyCheckBox.Checked;

            _tickets.RaiseListChangedEvents = false;
            _tickets.Clear();
            foreach (Ticket ticket in _allTickets)
            {
                if (!string.Equals(ticket.TenantId, _tenant, StringComparison.Ordinal))
                    continue;
                if (escalatedOnly && ticket.Status != TicketStatus.Escalated)
                    continue;
                _tickets.Add(ticket);
            }
            _tickets.RaiseListChangedEvents = true;
        }

        private void tenantComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _tenant = tenantComboBox.SelectedItem as string ?? TicketHub.Tenants[0];
            ApplyFilter();
            ResetBindingsQuietly();
            RenderSelection();
            RenderBoardCounters();
            AddTrace(TraceDirection.Request, "tenantComboBox_SelectedIndexChanged", $"this session now watches \"{_tenant}\" · {_tickets.Count} row(s) · the filter lives in the SESSION, not in the hub");
            SetStatus($"tenant {_tenant}", StatusKind.Normal);
            UpdateState();
        }

        private void escalatedOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            int? selectedId = ticketsGrid.CurrentRow?.DataBoundItem is Ticket t ? t.Id : (int?)null;
            ApplyFilter();
            ResetBindingsQuietly();
            bool kept = selectedId.HasValue && RestoreSelection(selectedId.Value);
            RenderSelection();
            AddTrace(TraceDirection.Request, "escalatedOnlyCheckBox_CheckedChanged", $"filter {(escalatedOnlyCheckBox.Checked ? "ON (Escalated only)" : "OFF")} · {_tickets.Count} row(s) shown · {(selectedId.HasValue ? (kept ? "selection kept" : "selection filtered out") : "no selection")}");
            UpdateState();
        }

        private void ticketsGrid_SelectionChanged(object sender, EventArgs e)
        {
            RenderSelection();
            if (_restoringSelection)
                return;
            AddTrace(TraceDirection.Request, "ticketsGrid_SelectionChanged", "the user selected a row — the state every update has to protect");
        }

        private void RenderSelection()
        {
            Ticket ticket = ticketsGrid.CurrentRow?.DataBoundItem as Ticket;
            selectedLabel.Text = ticket == null
                ? "selected: none — click a row, then publish or escalate"
                : $"selected: #{ticket.Id} · {ticket.TenantId} · {ticket.Status} · {ticket.Owner} · updated {ticket.UpdatedAt.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}";
        }

        private void RenderBoardCounters()
        {
            notificationCountLabel.Text = _notificationCount == 1 ? "1 notification in this session" : $"{_notificationCount} notifications in this session";
            filteredOutLabel.Text = $"filtered out (wrong tenant): {_filteredOut}";
            subscribersLabel.Text = $"hub subscribers: {TicketHub.Instance.SubscriberCount} · hub tickets: {TicketHub.Instance.TicketCount} · hub events: {TicketHub.Instance.EventsPublished}";
        }

        #endregion

        #region M04 — update cadence

        private void FillCadences()
        {
            cadenceComboBox.Items.Add("250 ms");
            cadenceComboBox.Items.Add("1 sec");
            cadenceComboBox.Items.Add("5 sec");
            cadenceComboBox.SelectedIndex = 1;
            _cadenceMs = 1000;
        }

        private void liveModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (liveModeCheckBox.Checked)
            {
                _liveMode = true;
                AddTrace(TraceDirection.Request, "liveModeCheckBox_CheckedChanged", "live mode ON — the browser sent the click");

                // The polling fallback belongs where out-of-bound work starts, not in Load: during Load
                // IsWebSocket is still false and StartPolling would poll forever.
                if (!Application.IsWebSocket && !_polling)
                {
                    Application.StartPolling(1000);
                    _polling = true;
                    AddTrace(TraceDirection.Server, "Application.StartPolling(1000)", "no WebSocket → fallback polling ON while live mode runs");
                }
                else
                {
                    AddTrace(TraceDirection.Server, "polling fallback", "not requested — IsWebSocket=true, the live channel is already there");
                }

                refreshTimer.Interval = _cadenceMs;
                refreshTimer.Start();
                _simulator.Start();
                AddTrace(TraceDirection.Server, "refreshTimer.Start()", $"UI cadence: one tick every {CadenceText(_cadenceMs)} · the page owns this timer");
                AddTrace(TraceDirection.Server, "DashboardSimulator.Start()", $"model cadence: one change every {DashboardSimulator.IntervalMs} ms on a task · it never calls Application.Update");
                SetStatus($"live · model {DashboardSimulator.IntervalMs} ms · UI {CadenceText(_cadenceMs)}", StatusKind.Normal);
            }
            else
            {
                _liveMode = false;
                _simulator.Stop();
                refreshTimer.Stop();
                AddTrace(TraceDirection.Request, "liveModeCheckBox_CheckedChanged", "live mode OFF");
                AddTrace(TraceDirection.Server, "stop", "DashboardSimulator.Stop() + refreshTimer.Stop() — a timer nobody stops keeps a session busy forever");
                if (_polling)
                {
                    Application.EndPolling();
                    _polling = false;
                    AddTrace(TraceDirection.Server, "Application.EndPolling()", "live mode off → fallback polling OFF");
                }
                SetStatus("live mode off · timer and simulator stopped", StatusKind.Normal);
            }
            UpdateState();
        }

        private void cadenceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int previous = _cadenceMs;
            switch (cadenceComboBox.SelectedIndex)
            {
                case 0: _cadenceMs = 250; break;
                case 2: _cadenceMs = 5000; break;
                default: _cadenceMs = 1000; break;
            }
            if (_cadenceMs < MinCadenceMs) _cadenceMs = MinCadenceMs;

            refreshTimer.Interval = _cadenceMs;    // reprograms the RUNNING timer; no second timer is created
            AddTrace(TraceDirection.Request, "cadenceComboBox_SelectedIndexChanged", $"UI cadence {CadenceText(previous)} → {CadenceText(_cadenceMs)} · refreshTimer.Interval reprogrammed on the running timer");
            UpdateState();
        }

        /// <summary>Called by the simulator task ~20×/s. It counts and marks dirty — it never touches a control.</summary>
        private void SimulatedModelChanged()
        {
            Interlocked.Increment(ref _eventsReceived);
            _dashboardDirty = true;
        }

        private void SimulatorFailed(Exception ex)
        {
            LogError("dashboard simulator", ex);
            if (this.IsDisposed)
                return;
            SafeUpdate(() =>
            {
                _liveMode = false;
                liveModeCheckBox.Checked = false;
                refreshTimer.Stop();
                NotePush();
                ShowBanner("✖ The dashboard simulator failed. Live mode was switched off; see the server log.", BannerKind.Error);
                SetStatus("simulator fault — live mode off", StatusKind.Error);
                UpdateState();
            });
        }

        /// <summary>
        /// The UI cadence. A timer tick IS a browser request, so whatever it changes travels back with that
        /// request — no Application.Update() here. One snapshot per tick, however many model events arrived.
        /// </summary>
        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            if (_refreshInProgress)
            {
                _ticksRefused++;
                AddTrace(TraceDirection.Server, "refreshTimer_Tick", "refused — a refresh is still in progress (no overlapping ticks)");
                return;
            }
            if (!_dashboardDirty)
                return;                              // nothing changed: no work, no traffic

            try
            {
                _refreshInProgress = true;
                _dashboardDirty = false;
                _updatesApplied++;

                DashboardSnapshot snapshot = _model.Snapshot();
                openTicketsLabel.Text = snapshot.OpenTickets.ToString(CultureInfo.InvariantCulture);
                queueDepthLabel.Text = snapshot.QueueDepth.ToString(CultureInfo.InvariantCulture);
                avgWaitLabel.Text = snapshot.AvgWaitMinutes.ToString("0.0", CultureInfo.InvariantCulture) + " min";
                eventsReceivedLabel.Text = _eventsReceived.ToString(CultureInfo.InvariantCulture);
                updatesAppliedLabel.Text = _updatesApplied.ToString(CultureInfo.InvariantCulture);
                skippedTicksLabel.Text = _ticksRefused.ToString(CultureInfo.InvariantCulture);
                lastAppliedLabel.Text = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);

                AddTrace(TraceDirection.Request, "refreshTimer_Tick", $"applied model v{snapshot.Version} · {_eventsReceived} event(s) coalesced into {_updatesApplied} update(s) → the changes ride back with THIS timer request");
                UpdateState();
            }
            finally
            {
                _refreshInProgress = false;
            }
        }

        private static string CadenceText(int ms) => ms >= 1000 ? (ms / 1000) + " s" : ms + " ms";

        #endregion

        #region M02 — session inspector and lifecycle

        private void incrementButton_Click(object sender, EventArgs e)
        {
            if (Application.Session.Counter == null)
                Application.Session.Counter = 0;
            Application.Session.Counter = (int)Application.Session.Counter + 1;
            _sharedCounter++;                       // the trap, on purpose: one field for the whole server

            AddTrace(TraceDirection.Request, "incrementButton_Click", $"Application.Session.Counter = {Application.Session.Counter} (this session) · static _sharedCounter = {_sharedCounter} (EVERY session)");
            LifecycleLog($"Session counter → {Application.Session.Counter}; static counter → {_sharedCounter}");
            RenderInspector("request thread (incrementButton_Click)");
            UpdateState();
        }

        private void backgroundButton_Click(object sender, EventArgs e)
        {
            var context = Application.Current;      // captured IN context, used from the task
            backgroundButton.Enabled = false;
            AddTrace(TraceDirection.Request, "backgroundButton_Click", "the handler returns now; the work continues on a task");
            BeginPush();

            Application.StartTask(() =>
            {
                try
                {
                    Thread.Sleep(1500);
                    Application.Update(context, () =>
                    {
                        LifecycleLog("Background update at " + DateTime.Now.ToString(CultureInfo.CurrentCulture));
                        NotePush();
                        AddTrace(TraceDirection.Push, "Application.Update(context, …)", $"delivered to session {Short(_sessionId)} — out-of-bound, no click");
                        RenderInspector("task thread (background update)");
                        UpdateState();
                    });
                }
                catch (Exception ex)
                {
                    LogError("background update", ex);
                }
                finally
                {
                    if (!this.IsDisposed)
                    {
                        try
                        {
                            Application.Update(context, () =>
                            {
                                backgroundButton.Enabled = true;
                                NotePush();
                                EndPush();
                                AddTrace(TraceDirection.Push, "Application.Update(context, …)", "background update finished — button re-enabled in finally");
                                UpdateState();
                            });
                        }
                        catch (ObjectDisposedException) { }
                    }
                }
            });
        }

        private void faultButton_Click(object sender, EventArgs e)
        {
            var context = Application.Current;
            faultButton.Enabled = false;
            AddTrace(TraceDirection.Request, "faultButton_Click", "the task will throw after 500 ms — the exception is caught INSIDE the task");
            BeginPush();

            Application.StartTask(() =>
            {
                try
                {
                    Thread.Sleep(500);
                    throw new InvalidOperationException($"Simulated failure in the background job of session {_sessionId}.");
                }
                catch (Exception ex)
                {
                    LogError("background job", ex);
                    try
                    {
                        Application.Update(context, () =>
                        {
                            LifecycleLog("Background job failed. See the server log.");
                            ShowBanner($"✖ The background job failed. The detail is in the server log for session {Short(_sessionId)}. Click “Background update” to run a healthy one.", BannerKind.Error);
                            SetStatus("background job failed — UI recovered", StatusKind.Error);
                            NotePush();
                            AddTrace(TraceDirection.Push, "Application.Update(context, …)", $"{ex.GetType().Name} caught inside the task → server log + safe message");
                            UpdateState();
                        });
                    }
                    catch (ObjectDisposedException) { }
                }
                finally
                {
                    if (!this.IsDisposed)
                    {
                        try
                        {
                            Application.Update(context, () =>
                            {
                                faultButton.Enabled = true;
                                NotePush();
                                EndPush();
                                AddTrace(TraceDirection.Push, "Application.Update(context, …)", "background fault handled — button re-enabled in finally");
                                UpdateState();
                            });
                        }
                        catch (ObjectDisposedException) { }
                    }
                }
            });
        }

        /// <summary>Raised by the global registry on a thread-pool thread: no context of its own, so restore ours.</summary>
        private void Registry_SessionsChanged(object sender, SessionsChangedEventArgs e)
        {
            if (this.IsDisposed)
                return;

            SafeUpdate(() =>
            {
                string verb = e.Change == SessionChange.Joined ? "joined" : "left";
                LifecycleLog($"Session {Short(e.SessionId)} {verb} · {e.Count} live session(s)");
                NotePush();
                RenderLiveSessions();
                AddTrace(TraceDirection.Push, "Application.Update(_context, …)", $"SessionRegistry: {Short(e.SessionId)} {verb} → {e.Count} live · pushed into session {Short(_sessionId)} from a thread-pool thread");
                RenderInspector("thread-pool thread (SessionRegistry.SessionsChanged)");
                UpdateState();
            });
        }

        private void RenderInspector(string threadKind)
        {
            timeLabel.Text = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture) + " (server time)";
            clientIdLabel.Text = _clientId;
            sessionIdLabel.Text = _sessionId;
            browserLabel.Text = DescribeBrowser();
            threadLabel.Text = $"#{Environment.CurrentManagedThreadId} · {threadKind} — reused, not owned";
            object counter = null;
            try { counter = Application.Session.Counter; } catch (Exception) { /* not in context */ }
            counterLabel.Text = $"{counter ?? 0} ← Application.Session.Counter (this session)   ·   static int = {_sharedCounter} ← EVERY session";
        }

        private static string DescribeBrowser()
        {
            try
            {
                var browser = Application.Browser;
                if (browser == null)
                    return "n/a";
                return $"{browser.Type} {browser.Version} · {browser.OS} · {browser.Device}";
            }
            catch (Exception)
            {
                return "n/a (not available on this thread)";
            }
        }

        private void RenderLiveSessions()
        {
            SessionInfo[] sessions = SessionRegistry.Instance.GetSnapshot();
            var text = new StringBuilder();
            text.Append("live sessions: ").Append(sessions.Length.ToString(CultureInfo.InvariantCulture)).Append("  ·  ");
            for (int i = 0; i < sessions.Length && i < 6; i++)
            {
                if (i > 0) text.Append("  ");
                text.Append(Short(sessions[i].SessionId));
                if (string.Equals(sessions[i].SessionId, _sessionId, StringComparison.Ordinal))
                    text.Append(" (this one)");
                text.Append('@').Append(sessions[i].StartedAt.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
            }
            if (sessions.Length > 6)
                text.Append("  +").Append(sessions.Length - 6);
            liveSessionsLabel.Text = text.ToString();
        }

        private void LifecycleLog(string message)
        {
            lifecycleListBox.Items.Insert(0, $"{DateTime.Now:T} - {message}");
            while (lifecycleListBox.Items.Count > 200)
                lifecycleListBox.Items.RemoveAt(lifecycleListBox.Items.Count - 1);
        }

        #endregion

        #region M07 — real-time health, configuration and the checklist

        private void healthTimer_Tick(object sender, EventArgs e)
        {
            RenderHealth(BuildHealthSnapshot());
            RenderBoardCounters();
            UpdateState();
        }

        /// <summary>Everything an operator needs, measured rather than guessed. Cheap on purpose.</summary>
        private RealtimeHealthSnapshot BuildHealthSnapshot()
        {
            // Prune the push window first: updates/min is a measurement, not an estimate.
            DateTime cutoff = DateTime.UtcNow.AddSeconds(-60);
            while (_pushTimes.Count > 0 && _pushTimes.Peek() < cutoff)
                _pushTimes.Dequeue();

            var loops = new List<string>();
            if (_heartbeatRunning) loops.Add("heartbeat");
            if (_importRunning) loops.Add("import " + (_currentJob?.JobId ?? ""));
            if (_simulator.IsRunning) loops.Add("dashboard simulator");
            if (_feedRunning) loops.Add("ticket feed");
            if (refreshTimer.Enabled) loops.Add("refreshTimer @ " + CadenceText(_cadenceMs));

            return new RealtimeHealthSnapshot
            {
                WebSocketExpected = Application.IsWebSocket,
                PollingFallbackEnabled = _polling,
                ActiveSubscriptions = TicketHub.Instance.SubscriberCount + SessionRegistry.Instance.Count,
                UpdatesPerMinute = _pushTimes.Count,
                LastServerEventUtc = _lastPushUtc,
                RunningLoops = loops.Count,
                RunningLoopNames = loops.Count == 0 ? "none" : string.Join(", ", loops),
            };
        }

        /// <summary>The walkthrough's RenderHealth: one snapshot in, four labels out.</summary>
        private void RenderHealth(RealtimeHealthSnapshot h)
        {
            websocketModeLabel.Text = h.WebSocketExpected ? "● Push expected (WebSocket)" : "○ Fallback mode (no WebSocket)";
            websocketModeLabel.ForeColor = h.WebSocketExpected
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(232, 161, 60);

            pollingLabel.Text = h.PollingFallbackEnabled ? "● Polling enabled (1000 ms)" : "○ Polling off";
            pollingLabel.ForeColor = h.PollingFallbackEnabled
                ? System.Drawing.Color.FromArgb(146, 64, 14)
                : System.Drawing.Color.FromArgb(90, 107, 125);

            subscriptionCountLabel.Text = $"active subscriptions: {h.ActiveSubscriptions}  (hub {TicketHub.Instance.SubscriberCount} + sessions {SessionRegistry.Instance.Count})";
            updateRateLabel.Text = $"{h.UpdatesPerMinute} updates/min · last {(h.SecondsSinceLastEvent < 0 ? "—" : h.SecondsSinceLastEvent + " s ago")} · loops: {h.RunningLoopNames}";

            RenderConnection();
        }

        /// <summary>
        /// The effective configuration of this application. Read through Application.Configuration so the panel
        /// shows what the SERVER resolved, not what the file says; each read is guarded because a member that
        /// does not exist in a given build must not take the page down.
        /// </summary>
        private void LoadConfiguration()
        {
            configListBox.Items.Clear();
            configListBox.Items.Add("Default.json — the values this session actually runs with");
            AddConfig("sessionTimeout", () => Application.Configuration.SessionTimeout.ToString(CultureInfo.InvariantCulture), "how long an idle session survives (seconds)");
            AddConfig("pollingInterval", () => Application.Configuration.PollingInterval.ToString(CultureInfo.InvariantCulture), "fallback poll rate when WebSocket is unavailable");
            AddConfig("enableWebSocket", () => Application.Configuration.EnableWebSocket.ToString(), "set false to rehearse the fallback path");
            AddConfig("debug", () => Application.Configuration.Debug.ToString(), "must be FALSE in production (bundled/minified client)");
            AddConfig("maxSessions", () => Application.Configuration.MaxSessions.ToString(CultureInfo.InvariantCulture), "-1 or 0 = unbounded; bound it before a load balancer does");
            AddConfig("theme", () => Application.Configuration.ThemeName ?? "n/a", "client theme");

            configListBox.Items.Add("");
            configListBox.Items.Add("HealthCheck.json — what a load balancer reads to drain a hot instance");
            try
            {
                string path = Path.Combine(Application.StartupPath ?? ".", "HealthCheck.json");
                if (File.Exists(path))
                {
                    foreach (string line in File.ReadAllLines(path))
                    {
                        string trimmed = line.Trim();
                        if (trimmed.Length > 0 && trimmed != "{" && trimmed != "}")
                            configListBox.Items.Add("  " + trimmed.TrimEnd(','));
                    }
                }
                else
                {
                    configListBox.Items.Add("  (not found next to the executable — it ships with the project folder)");
                }
            }
            catch (Exception ex)
            {
                configListBox.Items.Add("  (unreadable: " + ex.GetType().Name + ")");
            }

            configListBox.Items.Add("");
            configListBox.Items.Add("Deployment: sticky sessions REQUIRED — server-side session state lives in one process.");
            configListBox.Items.Add("Reverse proxy must forward the WebSocket upgrade (Connection/Upgrade headers).");
        }

        private void AddConfig(string key, Func<string> read, string why)
        {
            string value;
            try { value = read() ?? "n/a"; }
            catch (Exception) { value = "n/a (not exposed by this build)"; }
            configListBox.Items.Add($"  {key,-18} {value,-12} {why}");
        }

        /// <summary>The lesson's performance checklist — each item says where THIS app satisfies it.</summary>
        private void BuildChecklist()
        {
            string[] items =
            {
                "Every background loop has a stop condition — _heartbeatRunning / _feedRunning / token / IsDisposed",
                "Every global subscription unsubscribes — Cleanup(): hub and SessionRegistry, from Exit and Disposed",
                "UI updates are batched or throttled — import pushes every 10 records, heartbeat once per second",
                "High-frequency model events are coalesced — 50 ms model, dirty flag, one refresh per timer tick",
                "Bound collections are updated in the session context — Application.Update(_context) in Hub_TicketChanged",
                "Static fields hold global state only — TicketHub/SessionRegistry; _sharedCounter is the labelled trap",
                "Counters, filters and selection are per session — instance fields; tenant + Escalated filter",
                "Exceptions are caught and logged inside tasks — try/catch/finally in every loop, safe UI message",
                "The app degrades when WebSocket is gone — StartPolling only while work runs, EndPolling after",
                "The load balancer keeps a session on one instance — sticky sessions, see ConfigurationReview.md",
            };
            foreach (string item in items)
                checklistBox.Items.Add(item);
        }

        private void checklistBox_AfterItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!_wiringDone)
                return;
            string item = e.Index >= 0 && e.Index < checklistBox.Items.Count ? Convert.ToString(checklistBox.Items[e.Index], CultureInfo.InvariantCulture) : "";
            AddTrace(TraceDirection.Request, "checklist", $"{(e.NewValue == CheckState.Checked ? "✔" : "✖")} {item}");
            SetStatus($"checklist {checklistBox.CheckedItems.Count}/{checklistBox.Items.Count}", StatusKind.Normal);
        }

        private void featureTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = featureTabs.SelectedTab?.Text ?? "";
            AddTrace(TraceDirection.Request, "featureTabs_SelectedIndexChanged", $"showing \"{name}\" — every tab is the same session, the same context and the same trace");
            if (featureTabs.SelectedTab == tabHealth)
                RenderHealth(BuildHealthSnapshot());
            if (featureTabs.SelectedTab == tabSession)
                RenderInspector("request thread (tab switch)");
        }

        #endregion

        #region Delivery: WebSocket push, or the polling fallback while a task needs it

        private void BeginPush()
        {
            _pushers++;
            if (Application.IsWebSocket || _polling)
                return;

            Application.StartPolling(1000);
            _polling = true;
            AddTrace(TraceDirection.Server, "Application.StartPolling(1000)", "no WebSocket when the task started → fallback polling ON until the work ends");
        }

        private void EndPush()
        {
            if (_pushers > 0) _pushers--;
            if (_pushers > 0 || !_polling || _liveMode)
                return;                              // live mode owns the fallback while it is on

            Application.EndPolling();
            _polling = false;
            AddTrace(TraceDirection.Server, "Application.EndPolling()", "the last task ended → fallback polling OFF");
        }

        /// <summary>Counts one push for the health panel (updates/min) and stamps the last server event.</summary>
        private void NotePush()
        {
            _pushes++;
            _lastPushUtc = DateTime.UtcNow;
            _pushTimes.Enqueue(_lastPushUtc.Value);
            while (_pushTimes.Count > 600)
                _pushTimes.Dequeue();
        }

        /// <summary>Application.Update(_context, …) with the guards every out-of-bound caller needs.</summary>
        private void SafeUpdate(Action action)
        {
            if (this.IsDisposed || _context == null)
                return;
            try
            {
                Application.Update(_context, action);
            }
            catch (ObjectDisposedException) { /* the session went away between the guard and the push */ }
            catch (Exception ex) { LogError("Application.Update(context)", ex); }
        }

        #endregion

        #region Lifecycle: ONE cleanup, called from ApplicationExit and from Disposed

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            // Replace with your logging framework in production.
            Console.Error.WriteLine($"TicketOps session exited: {_sessionId} (client {_clientId})");
            Cleanup("ApplicationExit");
        }

        private void Application_SessionTimeout(object sender, HandledEventArgs e)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} session {_sessionId} is about to time out (Handled={e.Handled}).");
            // e.Handled stays false: the built-in "prolong the session?" dialog is the right default for a console.
        }

        /// <summary>
        /// Idempotent. Stops every loop this session owns and removes every subscription it made. Both
        /// ApplicationExit and Disposed call it, and whichever runs first does the work.
        /// </summary>
        private void Cleanup(string why)
        {
            if (_cleanedUp)
                return;
            _cleanedUp = true;

            _heartbeatRunning = false;
            _feedRunning = false;
            _liveMode = false;
            try { _simulator?.Stop(); } catch (Exception) { }
            try { RequestImportCancel(why); } catch (Exception) { }

            try { TicketHub.Instance.TicketChanged -= Hub_TicketChanged; } catch (Exception) { }
            _subscribedToHub = false;
            try { SessionRegistry.Instance.SessionsChanged -= Registry_SessionsChanged; } catch (Exception) { }
            if (_registered)
            {
                _registered = false;
                try { SessionRegistry.Instance.Unregister(_sessionId); } catch (Exception) { }
            }
            try { Application.ApplicationExit -= Application_ApplicationExit; } catch (Exception) { }
            try { Application.SessionTimeout -= Application_SessionTimeout; } catch (Exception) { }

            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} cleanup ({why}) for session {_sessionId}: loops stopped, hub and registry unsubscribed.");
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "exitButton_Click", "Application.Exit() → ApplicationExit runs the single cleanup");
            LifecycleLog("Application.Exit() requested by the operator.");
            Application.Exit();
        }

        private void openSessionButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "openSessionButton_Click", "a second tab: same ClientId (the browser), a new SessionId, its own page and counters");
            Application.Navigate("/", "_blank");
        }

        #endregion

        #region UI helpers

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
            listTrace.Items.Add($"{time}  {prefix} {name,-32} {payload}");
            while (listTrace.Items.Count > 400)
                listTrace.Items.RemoveAt(0);
            listTrace.SelectedIndex = listTrace.Items.Count - 1;
        }

        private void ImportLog(ImportJob job, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            importLogListBox.Items.Insert(0, $"{time}  [{job.JobId}] {message}");
            while (importLogListBox.Items.Count > 200)
                importLogListBox.Items.RemoveAt(importLogListBox.Items.Count - 1);
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            listTrace.Items.Clear();
            AddTrace(TraceDirection.Request, "clearButton_Click", "trace cleared — the counters keep counting");
        }

        private void RenderConnection()
        {
            bool ws = Application.IsWebSocket;
            connectionLabel.Text = ws
                ? "● WebSocket connected — Application.Update pushes out-of-bound"
                : "○ HTTP only — pushes wait for a poll or the next request";
            connectionLabel.ForeColor = ws
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(232, 161, 60);
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
            RealtimeHealthSnapshot h = BuildHealthSnapshot();
            labelState.Text =
                $"SERVER STATE (this session only, except the hub line)\n" +
                $"heartbeat={Low(_heartbeatRunning)} import={(_importRunning ? _currentJob?.JobId : "idle")} live={Low(_liveMode)} feed={Low(_feedRunning)} loops={h.RunningLoopNames}\n" +
                $"pushes={_pushes} ({h.UpdatesPerMinute}/min) tasks={_pushers} polling={Low(_polling)} jobs ✓{_completed} ⃠{_cancelled} ✖{_failed}\n" +
                $"tenant={_tenant} subscribed={Low(_subscribedToHub)} shown={_tickets.Count}/{_allTickets.Count} notif={_notificationCount} filtered={_filteredOut} applied={_eventsApplied}\n" +
                $"cadence={CadenceText(_cadenceMs)} events={_eventsReceived} updates={_updatesApplied} refused={_ticksRefused} · hub: {TicketHub.Instance.TicketCount} tickets, {TicketHub.Instance.EventsPublished} events, {TicketHub.Instance.SubscriberCount} subscribers\n" +
                $"IsWebSocket={Low(Application.IsWebSocket)} ClientId={Short(_clientId)} (browser) SessionId={Short(_sessionId)} (tab) · sessions={SessionRegistry.Instance.Count}";
        }

        private static void LogError(string operation, Exception ex)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} {operation} failed for session {Application.SessionId}: {ex}");
        }

        private static string Short(string id) => string.IsNullOrEmpty(id) ? "—" : (id.Length <= 8 ? id : id.Substring(0, 8));

        private static string Low(bool value) => value ? "true" : "false";

        #endregion
    }
}
