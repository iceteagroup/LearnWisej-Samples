using System;
using System.Globalization;
using System.Threading;
using TicketOpsLive.Services;
using Wisej.Web;

namespace TicketOpsLive
{
    public partial class MainPage : Page
    {
        // Per-session state: every browser tab has its own model, simulator and timer.
        private readonly DashboardModel _model = new DashboardModel(4);
        private readonly DashboardSimulator _simulator;

        private int _eventsReceived;            // written from the simulator thread → Interlocked
        private int _updatesApplied;            // written from the tick (a request) only
        private bool _refreshInProgress;        // prevents overlapping ticks
        private volatile bool _dashboardDirty;  // set by the model event, cleared by the tick
        private volatile bool _liveMode;
        private bool _timerStarted;             // start the timer once
        private bool _polling;
        private bool _settingLiveModeCheckBox;
        private int _cadenceMs = DefaultCadenceMs;

        private const int MinCadenceMs = 250;
        private const int DefaultCadenceMs = 1000;
        private const int PollingMs = 1000;

        public MainPage()
        {
            InitializeComponent();

            _simulator = new DashboardSimulator(
                _model,
                SimulatedModelChanged,
                () => this.IsDisposed || !_liveMode,
                SimulatorFailed);

            this.Disposed += (s, e) => StopEverythingOnDispose();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";

            _cadenceMs = SelectedCadenceMs();
            refreshTimer.Interval = _cadenceMs;
            ApplySnapshot(_model.Snapshot());
        }

        #region Live mode — timer, simulator and polling start and stop together

        private void liveModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // SimulatorFailed switches the checkbox off itself after it has already stopped everything.
            if (_settingLiveModeCheckBox)
                return;

            if (liveModeCheckBox.Checked)
                StartLiveMode();
            else
                StopLiveMode();

            RenderConnection();
        }

        private void StartLiveMode()
        {
            _liveMode = true;
            _cadenceMs = SelectedCadenceMs();

            StartPollingFallback();
            StartRefreshTimer();
            _simulator.Start();
        }

        private void StopLiveMode()
        {
            _liveMode = false;
            _simulator.Stop();
            StopRefreshTimer();
            StopPollingFallback();

            // This handler is a request: the final state travels back with its response.
            ApplySnapshot(_model.Snapshot());
        }

        private void StartRefreshTimer()
        {
            if (_timerStarted)
                return;

            _timerStarted = true;
            refreshTimer.Interval = _cadenceMs;
            refreshTimer.Start();
        }

        private void StopRefreshTimer()
        {
            if (!_timerStarted)
                return;

            refreshTimer.Stop();
            _timerStarted = false;
        }

        // StartPolling is not ignored when a WebSocket is up (it starts real HTTP polls), so polling is
        // requested only when there is no WebSocket.
        private void StartPollingFallback()
        {
            if (_polling || Application.IsWebSocket)
                return;

            Application.StartPolling(PollingMs);
            _polling = true;
        }

        private void StopPollingFallback()
        {
            if (!_polling)
                return;

            Application.EndPolling();
            _polling = false;
        }

        #endregion

        #region Cadence selector

        private void cadenceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reprograms the running timer; no second timer is created.
            _cadenceMs = SelectedCadenceMs();
            refreshTimer.Interval = _cadenceMs;
        }

        private int SelectedCadenceMs()
        {
            int ms = cadenceComboBox.SelectedIndex switch
            {
                0 => 250,
                2 => 5000,
                _ => DefaultCadenceMs,
            };
            return Math.Max(MinCadenceMs, ms);
        }

        #endregion

        #region The model event — counts and marks dirty, never pushes

        /// <summary>Called by the simulator on its task thread after every model change (about 20 per second).</summary>
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

            try
            {
                // Out-of-bound: no request is in flight, so the changes are pushed.
                Application.Update(this, () =>
                {
                    _liveMode = false;
                    _settingLiveModeCheckBox = true;
                    try { liveModeCheckBox.Checked = false; }
                    finally { _settingLiveModeCheckBox = false; }
                    StopRefreshTimer();
                    StopPollingFallback();
                    RenderConnection();
                    AlertBox.Show("The dashboard simulator failed. See the server log.", MessageBoxIcon.Error,
                        alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                });
            }
            catch (ObjectDisposedException) { }
        }

        #endregion

        #region The refresh timer — one coalesced update per tick

        // A Wisej.Web.Timer tick is a browser request: the changes made here travel back with its
        // response, so no Application.Update() is needed.
        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            if (_refreshInProgress || !_dashboardDirty)
                return;

            try
            {
                _refreshInProgress = true;
                _dashboardDirty = false;
                _updatesApplied++;

                ApplySnapshot(_model.Snapshot());
                eventsReceivedLabel.Text = Volatile.Read(ref _eventsReceived).ToString(CultureInfo.InvariantCulture);
                updatesAppliedLabel.Text = _updatesApplied.ToString(CultureInfo.InvariantCulture);
                lastAppliedLabel.Text = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                LogError("refresh tick", ex);
            }
            finally
            {
                _refreshInProgress = false;
                RenderConnection();
            }
        }

        #endregion

        #region Helpers

        private void ApplySnapshot(DashboardSnapshot snapshot)
        {
            openTicketsLabel.Text = snapshot.OpenTickets.ToString(CultureInfo.InvariantCulture);
            queueDepthLabel.Text = snapshot.QueueDepth.ToString(CultureInfo.InvariantCulture);
            avgWaitLabel.Text = snapshot.AvgWaitMinutes.ToString("0.0", CultureInfo.InvariantCulture) + " min";
        }

        private void RenderConnection()
        {
            bool ws = Application.IsWebSocket;
            connectionLabel.Text = ws
                ? "● WebSocket"
                : _polling ? $"○ Polling {PollingMs} ms" : "○ No WebSocket";
            connectionLabel.ForeColor = ws
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(232, 161, 60);
        }

        private static void LogError(string operation, Exception ex)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} {operation} failed for client {Application.ClientId}: {ex}");
        }

        #endregion

        #region Cleanup

        private void StopEverythingOnDispose()
        {
            _liveMode = false;
            _simulator.Stop();
            try
            {
                refreshTimer.Stop();
                _timerStarted = false;
                if (_polling)
                {
                    Application.EndPolling();
                    _polling = false;
                }
            }
            catch (Exception ex)
            {
                LogError("dispose cleanup", ex);
            }
        }

        #endregion
    }
}
