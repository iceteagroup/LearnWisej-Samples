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
    /// <summary>
    /// TicketOps Live: status strip, import monitor, live ticket board fed by the global TicketHub, update
    /// cadence, session diagnostics, and the real-time health and configuration review.
    ///
    /// Every loop has a stop condition and an IsDisposed guard; every subscription is removed by the single
    /// idempotent <see cref="Cleanup"/>; every push is counted for the health panel; no page, control or
    /// context is ever handed to a global service.
    /// </summary>
    public partial class MainPage : Page
    {
        private const int TotalRecords = 200;
        private const int RecordMilliseconds = 25;
        private const int PushEvery = 10;
        private const int LogEvery = 50;
        private const int MinCadenceMs = 250;
        private const int SimulatedTickets = 10;

        // Per-session state: instance fields of this page (one MainPage per browser tab).
        private IWisejComponent _context;          // captured on Load for out-of-bound code
        private string _sessionId = "";            // SessionId is the tab, ClientId is the browser
        private string _clientId = "";

        // Heartbeat
        private volatile bool _heartbeatRunning;
        private int _heartbeat;

        // Import
        private CancellationTokenSource _importCts;
        private volatile bool _importRunning;
        private ImportJob _currentJob;

        // Ticket board
        private readonly List<Ticket> _allTickets = new List<Ticket>();
        private readonly BindingList<Ticket> _tickets = new BindingList<Ticket>();
        private readonly BindingSource _ticketsBindingSource = new BindingSource();
        private string _tenant = TicketHub.Tenants[0];
        private bool _subscribedToHub;
        private int _notificationCount;
        private volatile bool _feedRunning;

        // Cadence
        private readonly DashboardModel _model = new DashboardModel(Environment.TickCount);
        private DashboardSimulator _simulator;
        private volatile bool _dashboardDirty;
        private bool _refreshInProgress;
        private bool _liveMode;
        private int _cadenceMs = 1000;
        private int _eventsReceived, _updatesApplied;

        // Session
        private bool _registered;

        // Delivery and health
        private int _pushers;                      // out-of-bound work in flight
        private bool _polling;
        private readonly Queue<DateTime> _pushTimes = new Queue<DateTime>();
        private DateTime? _lastPushUtc;
        private bool _cleanedUp;

        public MainPage()
        {
            InitializeComponent();

            _simulator = new DashboardSimulator(_model, SimulatedModelChanged, () => this.IsDisposed, SimulatorFailed);

            // Closing the tab stops every loop and removes every subscription.
            this.Disposed += (s, e) => Cleanup();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";

            _context = Application.Current;
            _clientId = Application.ClientId ?? "";
            _sessionId = Application.SessionId ?? "";

            if (Application.Session.Counter == null)
                Application.Session.Counter = 0;

            BuildTicketGrid();
            FillTenants();
            FillCadences();
            BuildChecklist();

            // Register first, then subscribe: the session must not receive its own "joined" event mid-Load.
            _registered = SessionRegistry.Instance.Register(_sessionId);
            SessionRegistry.Instance.SessionsChanged += Registry_SessionsChanged;
            SubscribeToHub();
            Application.ApplicationExit += Application_ApplicationExit;
            Application.SessionTimeout += Application_SessionTimeout;

            LoadSnapshot();
            LoadConfiguration();

            // Wired after the initial fills, so filling the combos does not look like user input.
            tenantComboBox.SelectedIndexChanged += new EventHandler(this.tenantComboBox_SelectedIndexChanged);
            cadenceComboBox.SelectedIndexChanged += new EventHandler(this.cadenceComboBox_SelectedIndexChanged);
            escalatedOnlyCheckBox.CheckedChanged += new EventHandler(this.escalatedOnlyCheckBox_CheckedChanged);
            liveModeCheckBox.CheckedChanged += new EventHandler(this.liveModeCheckBox_CheckedChanged);

            healthTimer.Start();

            LifecycleLog("Page loaded");
            RenderInspector();
            RenderLiveSessions();
            RenderNotificationCount();
            RenderHealth(BuildHealthSnapshot());
        }

        #region Heartbeat (status strip)

        private void startButton_Click(object sender, EventArgs e)
        {
            if (_heartbeatRunning)
                return;

            _heartbeatRunning = true;
            startButton.Enabled = false;
            stopButton.Enabled = true;
            activityLabel.Text = "Heartbeat starting…";
            BeginPush();

            Application.StartTask(HeartbeatLoop);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _heartbeatRunning = false;
            stopButton.Enabled = false;
            activityLabel.Text = "Heartbeat stopping…";
        }

        private void HeartbeatLoop()
        {
            var random = new Random();
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
                        activityLabel.Text = $"Heartbeat #{n} · load {load}%";
                        NotePush();
                        RenderConnection();
                    });

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                faulted = true;
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
                            activityLabel.Text = faulted ? "Heartbeat failed. See the server log." : "Heartbeat stopped";
                            NotePush();
                            EndPush();
                        });
                    }
                    catch (ObjectDisposedException) { }
                }
            }
        }

        #endregion

        #region Import monitor

        private void startImportButton_Click(object sender, EventArgs e)
        {
            StartImport(failAt87: false);
        }

        private void failAt87Button_Click(object sender, EventArgs e)
        {
            StartImport(failAt87: true);
        }

        private void StartImport(bool failAt87)
        {
            if (_importRunning)
                return;

            var job = new ImportJob(TotalRecords);
            _currentJob = job;
            _importRunning = true;

            startImportButton.Enabled = false;
            failAt87Button.Enabled = false;
            cancelImportButton.Enabled = true;
            importProgressBar.Value = 0;
            recordsImportedLabel.Text = $"0/{TotalRecords}";
            elapsedLabel.Text = "Elapsed 0.00 s";
            importStatusLabel.Text = "Starting import…";
            ImportLog(job, "Import started");

            _importCts = new CancellationTokenSource();
            CancellationToken token = _importCts.Token;
            BeginPush();

            Application.StartTask(() => RunImport(job, token, failAt87));
        }

        private void cancelImportButton_Click(object sender, EventArgs e)
        {
            cancelImportButton.Enabled = false;
            importStatusLabel.Text = "Cancelling…";
            RequestImportCancel();
        }

        private void RequestImportCancel()
        {
            try
            {
                _importCts?.Cancel();
            }
            catch (ObjectDisposedException) { }
        }

        /// <summary>200 records, the token checked between records, progress pushed every tenth, UI restored in finally.</summary>
        private void RunImport(ImportJob job, CancellationToken token, bool failAt87)
        {
            try
            {
                for (int i = 1; i <= TotalRecords; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(RecordMilliseconds);

                    if (failAt87 && i == 87)
                        throw new InvalidOperationException($"Simulated malformed record #{i} (job {job.JobId}).");

                    job.RecordsImported = i;
                    recordsImportedLabel.Text = $"{i}/{TotalRecords}";
                    importProgressBar.Value = i / 2;
                    importStatusLabel.Text = $"Importing records… {i}/{TotalRecords}";
                    elapsedLabel.Text = "Elapsed " + job.ElapsedText;

                    if (i % LogEvery == 0)
                        ImportLog(job, $"Imported {i} records");

                    if (i % PushEvery == 0)
                    {
                        NotePush();
                        Application.Update(this);
                    }
                }

                job.Complete();
                importStatusLabel.Text = "Import completed successfully.";
                ImportLog(job, "Import completed");
            }
            catch (OperationCanceledException)
            {
                job.Cancel();
                importStatusLabel.Text = "Import cancelled by user.";
                ImportLog(job, $"Cancelled after record {job.RecordsImported}");
            }
            catch (Exception ex)
            {
                job.Fail();
                LogError($"import job {job.JobId}", ex);
                importStatusLabel.Text = "Import failed. Review the server log.";
                ImportLog(job, $"Failed on record {job.RecordsImported + 1}");
            }
            finally
            {
                var cts = _importCts;
                _importCts = null;
                cts?.Dispose();
                _importRunning = false;
                if (ReferenceEquals(_currentJob, job))
                    _currentJob = null;

                if (!this.IsDisposed)
                {
                    try
                    {
                        Application.Update(this, () =>
                        {
                            startImportButton.Enabled = true;
                            failAt87Button.Enabled = true;
                            cancelImportButton.Enabled = false;
                            elapsedLabel.Text = "Elapsed " + job.ElapsedText;
                            ImportLog(job, job.Summary);
                            NotePush();
                            EndPush();
                        });
                    }
                    catch (ObjectDisposedException) { }
                }
            }
        }

        #endregion

        #region Ticket board fed by the global TicketHub

        private void BuildTicketGrid()
        {
            _ticketsBindingSource.DataSource = _tickets;

            ticketsGrid.AutoGenerateColumns = false;
            ticketsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ticketsGrid.MultiSelect = false;
            ticketsGrid.ReadOnly = true;
            ticketsGrid.AllowUserToAddRows = false;
            ticketsGrid.AllowUserToDeleteRows = false;

            ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", DataPropertyName = "Id", HeaderText = "Id", Width = 60 });
            ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTenant", DataPropertyName = "TenantId", HeaderText = "Tenant", Width = 84 });
            ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTitle", DataPropertyName = "Title", HeaderText = "Title", Width = 200 });
            ticketsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOwner", DataPropertyName = "Owner", HeaderText = "Owner", Width = 100 });
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

        /// <summary>Loads this session's list from the hub's snapshot (clones, never the hub's instances).</summary>
        private void LoadSnapshot()
        {
            _allTickets.Clear();
            _allTickets.AddRange(TicketHub.Instance.GetSnapshot());

            ApplyFilter();
            ResetBindingsQuietly();
        }

        private void SubscribeToHub()
        {
            if (_subscribedToHub)
                return;
            TicketHub.Instance.TicketChanged += Hub_TicketChanged;
            _subscribedToHub = true;
            subscribeButton.Enabled = false;
            unsubscribeButton.Enabled = true;
        }

        private void UnsubscribeFromHub()
        {
            if (!_subscribedToHub)
                return;
            TicketHub.Instance.TicketChanged -= Hub_TicketChanged;
            _subscribedToHub = false;
            if (!this.IsDisposed)
            {
                subscribeButton.Enabled = true;
                unsubscribeButton.Enabled = false;
            }
        }

        private void subscribeButton_Click(object sender, EventArgs e)
        {
            SubscribeToHub();
            LoadSnapshot();
        }

        private void unsubscribeButton_Click(object sender, EventArgs e)
        {
            UnsubscribeFromHub();
        }

        private void publishButton_Click(object sender, EventArgs e)
        {
            try
            {
                TicketHub.Instance.AddOrUpdate(NewTicketFor(_tenant));
            }
            catch (ArgumentException ex)
            {
                LogError("hub.AddOrUpdate", ex);
                AlertBox.Show("The ticket could not be published. See the server log.", MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
        }

        private void escalateButton_Click(object sender, EventArgs e)
        {
            Ticket selected = ticketsGrid.CurrentRow?.DataBoundItem as Ticket;
            if (selected == null)
            {
                AlertBox.Show("Select a ticket to escalate.", MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 3000);
                return;
            }

            Ticket copy = selected.Clone();
            copy.Status = TicketStatus.Escalated;
            copy.Owner = "escalation-desk";
            copy.UpdatedAt = DateTime.Now;
            TicketHub.Instance.AddOrUpdate(copy, "Escalated");
        }

        private void newTicketsButton_Click(object sender, EventArgs e)
        {
            if (_feedRunning)
                return;

            _feedRunning = true;
            newTicketsButton.Enabled = false;
            BeginPush();

            Application.StartTask(() =>
            {
                try
                {
                    for (int i = 0; i < SimulatedTickets && _feedRunning && !this.IsDisposed; i++)
                    {
                        TicketHub.Instance.AddOrUpdate(NewTicketFor(_tenant), "Added");
                        Thread.Sleep(1000);      // the publisher owns the cadence
                    }
                }
                catch (Exception ex)
                {
                    LogError("ticket feed", ex);
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
            var random = new Random(Environment.TickCount + _notificationCount);
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
        /// Runs on a thread-pool thread (the hub fans out with Task.Run). Guard, filter by tenant, then re-enter
        /// this session with Application.Update(_context, …).
        /// </summary>
        private void Hub_TicketChanged(object sender, TicketChangedEventArgs e)
        {
            if (this.IsDisposed)
                return;

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
                NotePush();

                if (string.Equals(e.ChangeType, "Escalated", StringComparison.Ordinal))
                {
                    AlertBox.Show($"Ticket {e.Ticket?.Id} escalated — {e.Ticket?.Title}", MessageBoxIcon.Warning,
                        alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                }
            });
        }

        /// <summary>Applies one event to the session's own list, keeping the selected row.</summary>
        private void ApplyTicketEvent(TicketChangedEventArgs e)
        {
            if (e?.Ticket == null)
                return;

            int? selectedId = ticketsGrid.CurrentRow?.DataBoundItem is Ticket selected ? selected.Id : (int?)null;

            Ticket existing = null;
            for (int i = 0; i < _allTickets.Count; i++)
            {
                if (_allTickets[i].Id == e.Ticket.Id) { existing = _allTickets[i]; break; }
            }

            if (existing == null)
                _allTickets.Insert(0, e.Ticket.Clone());     // new rows go to the top
            else
                existing.CopyFrom(e.Ticket);                 // existing rows change in place

            ApplyFilter();
            ResetBindingsQuietly();
            if (selectedId.HasValue)
                RestoreSelection(selectedId.Value);
        }

        private void ResetBindingsQuietly()
        {
            _ticketsBindingSource.ResetBindings(false);
        }

        private bool RestoreSelection(int id)
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

        /// <summary>Rebuilds the bound list from the master list (tenant + Escalated filter).</summary>
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
        }

        private void escalatedOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            int? selectedId = ticketsGrid.CurrentRow?.DataBoundItem is Ticket t ? t.Id : (int?)null;
            ApplyFilter();
            ResetBindingsQuietly();
            if (selectedId.HasValue)
                RestoreSelection(selectedId.Value);
        }

        private void RenderNotificationCount()
        {
            notificationCountLabel.Text = _notificationCount == 1 ? "1 notification in this session" : $"{_notificationCount} notifications in this session";
        }

        #endregion

        #region Update cadence

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

                // Polling is requested only when there is no WebSocket, and never in Load.
                if (!Application.IsWebSocket && !_polling)
                {
                    Application.StartPolling(1000);
                    _polling = true;
                }

                refreshTimer.Interval = _cadenceMs;
                refreshTimer.Start();
                _simulator.Start();
            }
            else
            {
                _liveMode = false;
                _simulator.Stop();
                refreshTimer.Stop();
                if (_polling && _pushers == 0)
                {
                    Application.EndPolling();
                    _polling = false;
                }
            }
            RenderConnection();
        }

        private void cadenceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cadenceComboBox.SelectedIndex)
            {
                case 0: _cadenceMs = 250; break;
                case 2: _cadenceMs = 5000; break;
                default: _cadenceMs = 1000; break;
            }
            if (_cadenceMs < MinCadenceMs) _cadenceMs = MinCadenceMs;

            refreshTimer.Interval = _cadenceMs;    // reprograms the running timer; no second timer is created
        }

        /// <summary>Called by the simulator task about 20 times per second. It counts and marks dirty, never touches a control.</summary>
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
                liveModeCheckBox.Checked = false;
                NotePush();
                AlertBox.Show("The dashboard simulator failed. See the server log.", MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            });
        }

        // A timer tick is a browser request: its changes ride back with the response, no Application.Update().
        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            if (_refreshInProgress || !_dashboardDirty)
                return;

            try
            {
                _refreshInProgress = true;
                _dashboardDirty = false;
                _updatesApplied++;

                DashboardSnapshot snapshot = _model.Snapshot();
                openTicketsLabel.Text = snapshot.OpenTickets.ToString(CultureInfo.InvariantCulture);
                queueDepthLabel.Text = snapshot.QueueDepth.ToString(CultureInfo.InvariantCulture);
                avgWaitLabel.Text = snapshot.AvgWaitMinutes.ToString("0.0", CultureInfo.InvariantCulture) + " min";
                eventsReceivedLabel.Text = Volatile.Read(ref _eventsReceived).ToString(CultureInfo.InvariantCulture);
                updatesAppliedLabel.Text = _updatesApplied.ToString(CultureInfo.InvariantCulture);
                lastAppliedLabel.Text = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            }
            finally
            {
                _refreshInProgress = false;
            }
        }

        #endregion

        #region Session diagnostics

        private void incrementButton_Click(object sender, EventArgs e)
        {
            Application.Session.Counter = (int)Application.Session.Counter + 1;
            LifecycleLog($"Counter → {Application.Session.Counter}");
            RenderInspector();
        }

        private void backgroundButton_Click(object sender, EventArgs e)
        {
            var context = Application.Current;
            backgroundButton.Enabled = false;
            LifecycleLog("Background update started");
            BeginPush();

            Application.StartTask(() =>
            {
                try
                {
                    Thread.Sleep(1500);
                    Application.Update(context, () =>
                    {
                        if (this.IsDisposed)
                            return;
                        LifecycleLog("Background update at " + DateTime.Now.ToString(CultureInfo.CurrentCulture));
                        NotePush();
                        RenderInspector();
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
                            });
                        }
                        catch (ObjectDisposedException) { }
                    }
                }
            });
        }

        private void openSessionButton_Click(object sender, EventArgs e)
        {
            // Same browser, second tab: a new session with its own page, counters and subscriptions.
            Application.Navigate("/", "_blank");
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
                RenderInspector();
            });
        }

        private void RenderInspector()
        {
            timeLabel.Text = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            clientIdLabel.Text = _clientId;
            sessionIdLabel.Text = _sessionId;
            browserLabel.Text = DescribeBrowser();
            threadLabel.Text = "#" + Environment.CurrentManagedThreadId.ToString(CultureInfo.InvariantCulture);
            object counter = null;
            try { counter = Application.Session.Counter; } catch (Exception) { /* not in context */ }
            counterLabel.Text = (counter ?? 0).ToString();
        }

        private static string DescribeBrowser()
        {
            try
            {
                var browser = Application.Browser;
                if (browser == null)
                    return "(not available)";
                return $"{browser.Type} {browser.Version} · {browser.OS} · {browser.Device}";
            }
            catch (Exception)
            {
                return "(not available)";
            }
        }

        private void RenderLiveSessions()
        {
            SessionInfo[] sessions = SessionRegistry.Instance.GetSnapshot();
            var text = new StringBuilder();
            text.Append("Live sessions: ").Append(sessions.Length.ToString(CultureInfo.InvariantCulture));
            for (int i = 0; i < sessions.Length && i < 6; i++)
            {
                text.Append(i == 0 ? " — " : ", ").Append(Short(sessions[i].SessionId));
                if (string.Equals(sessions[i].SessionId, _sessionId, StringComparison.Ordinal))
                    text.Append(" (this tab)");
            }
            if (sessions.Length > 6)
                text.Append(" …");
            liveSessionsLabel.Text = text.ToString();
        }

        private void LifecycleLog(string message)
        {
            lifecycleListBox.Items.Insert(0, $"{DateTime.Now:T} - {message}");
            while (lifecycleListBox.Items.Count > 200)
                lifecycleListBox.Items.RemoveAt(lifecycleListBox.Items.Count - 1);
        }

        #endregion

        #region Real-time health, configuration and the production checklist

        private void healthTimer_Tick(object sender, EventArgs e)
        {
            RenderHealth(BuildHealthSnapshot());
        }

        private RealtimeHealthSnapshot BuildHealthSnapshot()
        {
            // updates/min is measured over the last 60 seconds.
            DateTime cutoff = DateTime.UtcNow.AddSeconds(-60);
            while (_pushTimes.Count > 0 && _pushTimes.Peek() < cutoff)
                _pushTimes.Dequeue();

            var loops = new List<string>();
            if (_heartbeatRunning) loops.Add("heartbeat");
            if (_importRunning) loops.Add("import " + (_currentJob?.JobId ?? ""));
            if (_simulator.IsRunning) loops.Add("dashboard simulator");
            if (_feedRunning) loops.Add("ticket feed");
            if (refreshTimer.Enabled) loops.Add("refreshTimer @ " + (_cadenceMs >= 1000 ? (_cadenceMs / 1000) + " s" : _cadenceMs + " ms"));

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

        private void RenderHealth(RealtimeHealthSnapshot h)
        {
            websocketModeLabel.Text = h.WebSocketExpected ? "● Push expected (WebSocket)" : "○ Fallback mode";
            websocketModeLabel.ForeColor = h.WebSocketExpected
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(232, 161, 60);

            pollingLabel.Text = h.PollingFallbackEnabled ? "● Polling enabled (1000 ms)" : "○ Polling off";
            pollingLabel.ForeColor = h.PollingFallbackEnabled
                ? System.Drawing.Color.FromArgb(146, 64, 14)
                : System.Drawing.Color.FromArgb(90, 107, 125);

            subscriptionCountLabel.Text = $"Active subscriptions: {h.ActiveSubscriptions}";
            updateRateLabel.Text = $"{h.UpdatesPerMinute} updates/min · last {(h.SecondsSinceLastEvent < 0 ? "—" : h.SecondsSinceLastEvent + " s ago")} · running: {h.RunningLoopNames}";

            RenderConnection();
        }

        /// <summary>
        /// The effective configuration, read through Application.Configuration so the panel shows what the server
        /// resolved. Each read is guarded: a member missing from a given build must not take the page down.
        /// </summary>
        private void LoadConfiguration()
        {
            configListBox.Items.Clear();
            configListBox.Items.Add("Default.json");
            AddConfig("sessionTimeout", () => Application.Configuration.SessionTimeout.ToString(CultureInfo.InvariantCulture), "idle session lifetime (seconds)");
            AddConfig("pollingInterval", () => Application.Configuration.PollingInterval.ToString(CultureInfo.InvariantCulture), "fallback poll rate without WebSocket");
            AddConfig("enableWebSocket", () => Application.Configuration.EnableWebSocket.ToString(), "false rehearses the fallback path");
            AddConfig("debug", () => Application.Configuration.Debug.ToString(), "must be false in production");
            AddConfig("maxSessions", () => Application.Configuration.MaxSessions.ToString(CultureInfo.InvariantCulture), "-1 or 0 = unbounded");
            AddConfig("theme", () => Application.Configuration.ThemeName ?? "n/a", "client theme");

            configListBox.Items.Add("");
            configListBox.Items.Add("HealthCheck.json");
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
                    configListBox.Items.Add("  (not found next to the application)");
                }
            }
            catch (Exception ex)
            {
                configListBox.Items.Add("  (unreadable: " + ex.GetType().Name + ")");
            }

            configListBox.Items.Add("");
            configListBox.Items.Add("Deployment: sticky sessions required — session state lives in one process.");
            configListBox.Items.Add("Reverse proxy must forward the WebSocket upgrade (Connection/Upgrade headers).");
        }

        private void AddConfig(string key, Func<string> read, string why)
        {
            string value;
            try { value = read() ?? "n/a"; }
            catch (Exception) { value = "n/a"; }
            configListBox.Items.Add($"  {key,-18} {value,-12} {why}");
        }

        private void BuildChecklist()
        {
            string[] items =
            {
                "Every background loop has a stop condition",
                "Every global subscription unsubscribes",
                "UI updates are batched or throttled",
                "High-frequency model events are coalesced",
                "Bound collections are updated in the session context",
                "Static fields hold global state only",
                "Counters, filters and selections are per session",
                "Background tasks catch and log exceptions",
                "The app behaves when WebSocket is unavailable",
                "The load balancer keeps a session on one instance",
            };
            foreach (string item in items)
                checklistBox.Items.Add(item);
        }

        private void featureTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            // A tab's controls are created when it is first shown: refresh what it displays.
            if (featureTabs.SelectedTab == tabHealth)
                RenderHealth(BuildHealthSnapshot());
            if (featureTabs.SelectedTab == tabSession)
                RenderInspector();
        }

        #endregion

        #region Delivery: WebSocket push, or polling while work runs

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
            if (_pushers > 0 || !_polling || _liveMode)
                return;                              // live mode keeps the fallback while it is on

            Application.EndPolling();
            _polling = false;
        }

        /// <summary>Counts one push for the health panel (updates/min) and stamps the last server event.</summary>
        private void NotePush()
        {
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
            catch (ObjectDisposedException) { }
            catch (Exception ex) { LogError("Application.Update(context)", ex); }
        }

        #endregion

        #region Lifecycle: one cleanup, called from ApplicationExit and from Disposed

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            // Replace with your logging framework in production.
            Console.Error.WriteLine($"TicketOps session exited: {_sessionId} (client {_clientId})");
            Cleanup();
        }

        private void Application_SessionTimeout(object sender, HandledEventArgs e)
        {
            // e.Handled stays false: Wisej.NET shows its default dialog offering to prolong the session.
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} session {_sessionId} is about to time out.");
        }

        /// <summary>Idempotent: stops every loop this session owns and removes every subscription it made.</summary>
        private void Cleanup()
        {
            if (_cleanedUp)
                return;
            _cleanedUp = true;

            _heartbeatRunning = false;
            _feedRunning = false;
            _liveMode = false;
            try { _simulator?.Stop(); } catch (Exception) { }
            try { RequestImportCancel(); } catch (Exception) { }

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
        }

        #endregion

        #region Helpers

        private void ImportLog(ImportJob job, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            importLogListBox.Items.Insert(0, $"{time}  [{job.JobId}] {message}");
            while (importLogListBox.Items.Count > 200)
                importLogListBox.Items.RemoveAt(importLogListBox.Items.Count - 1);
        }

        private void RenderConnection()
        {
            bool ws = Application.IsWebSocket;
            connectionLabel.Text = ws
                ? "● WebSocket connected"
                : _polling ? "○ No WebSocket — polling every second" : "○ No WebSocket";
            connectionLabel.ForeColor = ws
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(232, 161, 60);
        }

        private static void LogError(string operation, Exception ex)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} {operation} failed for session {Application.SessionId}: {ex}");
        }

        private static string Short(string id) => string.IsNullOrEmpty(id) ? "—" : (id.Length <= 8 ? id : id.Substring(0, 8));

        #endregion
    }
}
