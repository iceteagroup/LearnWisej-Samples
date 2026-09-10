using System;
using System.Globalization;
using System.Threading;
using TicketOpsLive.Services;
using Wisej.Web;

namespace TicketOpsLive
{
    public enum TraceDirection { Push, Request, Server }

    /// <summary>
    /// TicketOps Live — Module 4 · When Push Is Not Enough: Cadence, Timers, and Fallbacks.
    ///
    /// Status strip: the simulated operations dashboard (open tickets, queue depth, average wait) — the
    ///               numbers the refresh timer renders, one coalesced snapshot per tick.
    /// Left card:    the Update Cadence panel — liveModeCheckBox, cadenceComboBox (250 ms / 1 sec / 5 sec),
    ///               the slow-tick switch and the instrumentation the lesson asks for: events received,
    ///               updates applied, ticks refused by the overlap guard, last applied time.
    /// Right card:   one "← request refreshTimer_Tick" line per APPLIED update, plus every server decision.
    /// Bottom bar:   coalescing (Burst 50 model events), the timer-ownership anti-pattern (Start timer again),
    ///               Clear trace.
    ///
    /// The shape of the lesson, exactly: the model event NEVER pushes. DashboardSimulator changes the model
    /// every 50 ms and calls SimulatedModelChanged(), which counts the event and sets _dashboardDirty. The
    /// page's own Wisej.Web.Timer applies ONE snapshot per tick at the cadence the user picked. Model cadence
    /// (50 ms) and UI cadence (250 ms … 5 s) are two different things.
    ///
    /// Delivery, and why there is almost no "push" in this module: a Wisej.Web.Timer tick is a normal browser
    /// request, so everything refreshTimer_Tick changes travels back in the response of that request — no
    /// Application.Update() is needed. Polling is requested with live mode only for the out-of-bound path
    /// (the simulator's fault banner), and only when there is no WebSocket.
    /// </summary>
    public partial class MainPage : Page
    {
        // Per-session state lives in INSTANCE fields: every browser tab has its own MainPage, its own
        // model, its own simulator and its own timer. A static here would make one user's cadence the
        // cadence of everybody on the server.
        private readonly DashboardModel _model = new DashboardModel(4);
        private readonly DashboardSimulator _simulator;

        private int _eventsReceived;            // written from the simulator thread → Interlocked
        private int _updatesApplied;            // written from the tick (a request) only
        private int _skippedTicks;              // ticks refused because a refresh was in progress
        private int _idleTicks;                 // ticks with nothing dirty to apply
        private bool _refreshInProgress;        // the overlap guard of the lesson
        private volatile bool _dashboardDirty;  // set by the model event, cleared by the tick
        private volatile bool _liveMode;        // stop condition for the simulator loop
        private bool _timerStarted;             // "start timers once" — the ownership guard
        private bool _polling;                  // the fallback is active (no WebSocket when live mode started)
        private bool _settingLiveModeCheckBox;  // the code, not the user, is moving the checkbox
        private int _cadenceMs = DefaultCadenceMs;
        private long _lastVersion;              // model version applied by the last refresh

        private const int MinCadenceMs = 250;
        private const int DefaultCadenceMs = 1000;
        private const int SlowTickMs = 1500;
        private const int BurstEvents = 50;
        private const int PollingMs = 1000;

        public MainPage()
        {
            InitializeComponent();

            // One simulator per page, created once. It stops when live mode goes off, and when the page
            // is disposed — a loop without a stop condition is the leak the lesson warns about.
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

            // No StartPolling here on purpose: Application.IsWebSocket is false during Load (the socket
            // opens right after this response), so a StartPolling(1000) placed here would start real HTTP
            // polling that keeps running for the whole session. Polling is requested with live mode.
            AddTrace(TraceDirection.Request, "MainPage_Load", $"first request: the page is rendered and returned with the response · IsWebSocket={Low(Application.IsWebSocket)} (the socket opens after this response)");

            _cadenceMs = SelectedCadenceMs();
            refreshTimer.Interval = _cadenceMs;
            ApplySnapshot(_model.Snapshot());
            RenderConnection();
            RenderCadence();
            SetStatus("idle · tick Live mode to start the model and the timer", StatusKind.Normal);
            UpdateState();
        }

        #region Live mode — the timer, the simulator and the polling fallback start and stop together

        private void liveModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // The fault handler switches the checkbox off itself; that assignment must not run the stop
            // path a second time (it has already run) and must not overwrite the error status.
            if (_settingLiveModeCheckBox)
                return;

            if (liveModeCheckBox.Checked)
                StartLiveMode();
            else
                StopLiveMode("live mode switched off");

            RenderConnection();
            RenderCadence();
            UpdateState();
        }

        private void StartLiveMode()
        {
            HideBanner();
            AddTrace(TraceDirection.Request, "liveModeCheckBox_CheckedChanged", "live mode ON — the browser sent the click");

            _liveMode = true;
            _cadenceMs = SelectedCadenceMs();

            // 1 · the fallback. StartPolling is NOT ignored when a WebSocket is up (verified in Module 1:
            // it starts real HTTP polls), so it is requested only when there is no socket — and it is
            // requested for the out-of-bound path, not for the timer: the timer drives its own requests.
            StartPollingFallback();

            // 2 · the timer — started ONCE, by the owner of the view.
            StartRefreshTimer("liveModeCheckBox_CheckedChanged");

            // 3 · the model. The simulator changes the model every 50 ms and only marks it dirty.
            if (_simulator.Start())
                AddTrace(TraceDirection.Server, "DashboardSimulator.Start()", $"model cadence: one change every {DashboardSimulator.IntervalMs} ms on a task (Application.StartTask) · it never calls Application.Update");
            else
                AddTrace(TraceDirection.Server, "DashboardSimulator.Start()", "refused — the simulator was already running");

            SetStatus($"live · model {DashboardSimulator.IntervalMs} ms · UI {CadenceText(_cadenceMs)}", StatusKind.Normal);
        }

        private void StopLiveMode(string reason)
        {
            AddTrace(TraceDirection.Request, "liveModeCheckBox_CheckedChanged", "live mode OFF — " + reason);

            _liveMode = false;
            _simulator.Stop();
            AddTrace(TraceDirection.Server, "DashboardSimulator.Stop()", $"cooperative stop — the loop exits at its next check (≤ {DashboardSimulator.IntervalMs} ms)");

            StopRefreshTimer(reason);
            StopPollingFallback();

            // The final state is always delivered: this handler is a request, so the snapshot below
            // travels back with its response. Nothing is left half-rendered when the timer stops.
            ApplySnapshot(_model.Snapshot());
            SetStatus("live mode off · timer stopped, polling stopped", StatusKind.Warn);
        }

        /// <summary>Start once. A second Start() on a running timer is the anti-pattern of the lesson.</summary>
        private bool StartRefreshTimer(string origin)
        {
            if (_timerStarted)
            {
                AddTrace(TraceDirection.Server, origin, $"refused — refreshTimer is already running at {CadenceText(_cadenceMs)} — start timers once");
                ShowBanner("⚠ refreshTimer is already running — start timers once. A second Start() on a page-owned timer doubles the update traffic and makes bugs look random.", BannerKind.Warn);
                return false;
            }

            _timerStarted = true;
            refreshTimer.Interval = _cadenceMs;
            refreshTimer.Start();
            AddTrace(TraceDirection.Server, "refreshTimer.Start()", $"UI cadence: one tick every {CadenceText(_cadenceMs)} · the page owns this timer (components tray) and is the only thing that starts or stops it");
            return true;
        }

        private void StopRefreshTimer(string reason)
        {
            if (!_timerStarted)
                return;

            refreshTimer.Stop();
            _timerStarted = false;
            AddTrace(TraceDirection.Server, "refreshTimer.Stop()", "the view is no longer live (" + reason + ") — a timer that nobody stops keeps a session busy forever");
        }

        /// <summary>
        /// The fallback of the lab: polling while live mode is on. Only when there is no WebSocket —
        /// with a socket the polls would be pure cost (one HTTP request per second per session).
        /// </summary>
        private void StartPollingFallback()
        {
            if (_polling)
                return;

            if (Application.IsWebSocket)
            {
                AddTrace(TraceDirection.Server, "polling fallback", "not requested — IsWebSocket=true, the out-of-bound path already has a live channel");
                return;
            }

            Application.StartPolling(PollingMs);
            _polling = true;
            AddTrace(TraceDirection.Server, $"Application.StartPolling({PollingMs})", "IsWebSocket=false → the browser polls every second while live mode is on, so out-of-bound changes still arrive");
        }

        private void StopPollingFallback()
        {
            if (!_polling)
                return;

            Application.EndPolling();
            _polling = false;
            AddTrace(TraceDirection.Server, "Application.EndPolling()", "live mode off → fallback polling OFF (an idle session must not poll for nothing)");
        }

        #endregion

        #region Cadence selector — reprogram the interval live

        private void cadenceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int previous = _cadenceMs;
            _cadenceMs = SelectedCadenceMs();

            // The interval is reprogrammed on the live timer; the timer is NOT restarted and no second
            // timer is created. Ownership does not change because the cadence did.
            refreshTimer.Interval = _cadenceMs;

            AddTrace(TraceDirection.Request, "cadenceComboBox_SelectedIndexChanged", $"UI cadence {CadenceText(previous)} → {CadenceText(_cadenceMs)} (floor {MinCadenceMs} ms) · refreshTimer.Interval reprogrammed on the running timer, no new timer");
            if (_timerStarted)
                SetStatus($"live · model {DashboardSimulator.IntervalMs} ms · UI {CadenceText(_cadenceMs)}", StatusKind.Normal);

            RenderCadence();
            UpdateState();
        }

        /// <summary>"250 ms" / "1 sec" / "5 sec" → milliseconds, never below the floor.</summary>
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

        /// <summary>
        /// Called by the simulator on the task thread after every model change (≈20 times per second).
        /// It does the two cheapest things possible: count the event and raise the dirty flag. No control
        /// is touched, no Application.Update is called — otherwise the model cadence would become the push
        /// cadence and the browser would receive 20 frames per second nobody can read.
        /// </summary>
        private void SimulatedModelChanged()
        {
            Interlocked.Increment(ref _eventsReceived);
            _dashboardDirty = true;
        }

        /// <summary>The simulator faulted: log the detail, show a safe message. This is the one out-of-bound push of the module.</summary>
        private void SimulatorFailed(Exception ex)
        {
            LogError("dashboard simulator", ex);
            if (this.IsDisposed)
                return;

            try
            {
                // Out-of-bound: no request is in flight, so the changes must be pushed (over the WebSocket,
                // or delivered by the fallback polls requested when live mode started).
                Application.Update(this, () =>
                {
                    _liveMode = false;
                    _settingLiveModeCheckBox = true;
                    try { liveModeCheckBox.Checked = false; }
                    finally { _settingLiveModeCheckBox = false; }
                    StopRefreshTimer("the simulator faulted");
                    StopPollingFallback();
                    ShowBanner($"✖ The model simulator failed. The detail is in the server log (client {Application.ClientId}). Tick Live mode again to recover.", BannerKind.Error);
                    SetStatus("simulator faulted — live mode off, UI restored", StatusKind.Error);
                    AddTrace(TraceDirection.Push, "Application.Update(this, …)", "simulator faulted — caught inside the task, live mode switched off, safe message pushed");
                    UpdateState();
                });
            }
            catch (ObjectDisposedException) { /* the page went away between the check and the push */ }
        }

        #endregion

        #region The refresh timer — one coalesced update per tick

        /// <summary>
        /// The heart of the module. A Wisej.Web.Timer tick is a browser request, so every control change
        /// made here travels back with the response of that request — that is why there is no
        /// Application.Update() in this method.
        ///
        /// Two guards, exactly as the lesson writes them:
        ///   _refreshInProgress — a tick that arrives while a refresh is running is counted and dropped;
        ///   _dashboardDirty    — a tick with nothing to show does no work at all.
        /// </summary>
        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            if (_refreshInProgress)
            {
                // Overlap: the previous refresh has not finished (see the slow-tick switch). Count it and
                // drop it — a queue of half-applied refreshes is worse than a missing frame.
                _skippedTicks++;
                skippedTicksLabel.Text = _skippedTicks.ToString(CultureInfo.InvariantCulture);
                AddTrace(TraceDirection.Server, "refreshTimer_Tick", $"refused — a refresh is still in progress (_refreshInProgress) · ticks refused = {_skippedTicks}");
                UpdateState();
                return;
            }

            if (!_dashboardDirty)
            {
                // Nothing changed since the last refresh: no snapshot, no labels, no traffic.
                _idleTicks++;
                if (_idleTicks % 10 == 1)
                    AddTrace(TraceDirection.Server, "refreshTimer_Tick", $"nothing to do — the model has not changed since the last refresh (idle ticks = {_idleTicks})");
                UpdateState();
                return;
            }

            try
            {
                _refreshInProgress = true;
                _dashboardDirty = false;
                _updatesApplied++;

                // One consistent copy of the model, taken under its lock. The number of model changes that
                // this single update represents is the coalescing ratio the lesson talks about.
                DashboardSnapshot snapshot = _model.Snapshot();
                long coalesced = snapshot.Version - _lastVersion;
                _lastVersion = snapshot.Version;

                if (slowTickCheckBox.Checked)
                {
                    // Anti-pattern on purpose: long work inside a tick. The lesson's rule is "start work or
                    // schedule it instead" — a tick should apply state, not produce it.
                    Thread.Sleep(SlowTickMs);
                }

                ApplySnapshot(snapshot);
                eventsReceivedLabel.Text = Volatile.Read(ref _eventsReceived).ToString(CultureInfo.InvariantCulture);
                updatesAppliedLabel.Text = _updatesApplied.ToString(CultureInfo.InvariantCulture);
                skippedTicksLabel.Text = _skippedTicks.ToString(CultureInfo.InvariantCulture);
                lastAppliedLabel.Text = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);

                AddTrace(TraceDirection.Request, "refreshTimer_Tick", $"applied model v{snapshot.Version} · {coalesced} model change(s) coalesced into 1 update · open={snapshot.OpenTickets} queue={snapshot.QueueDepth} wait={FormatWait(snapshot.AvgWaitMinutes)}{(slowTickCheckBox.Checked ? $" · slow tick {SlowTickMs} ms" : "")} → the changes travel back with THIS timer request, no Application.Update()");
                RenderCadence();
            }
            catch (Exception ex)
            {
                // A tick must never take the session down: log the detail, show a safe message, keep going.
                LogError("refresh tick", ex);
                ShowBanner($"✖ The refresh tick failed. The detail is in the server log (client {Application.ClientId}). The timer keeps running.", BannerKind.Error);
                SetStatus("refresh tick failed — see the server log", StatusKind.Error);
                AddTrace(TraceDirection.Server, "refreshTimer_Tick", $"FAILED: {ex.GetType().Name} caught inside the tick → server log, safe message, timer untouched");
            }
            finally
            {
                _refreshInProgress = false;
                RenderConnection();
                UpdateState();
            }
        }

        private void slowTickCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (slowTickCheckBox.Checked)
            {
                AddTrace(TraceDirection.Request, "slowTickCheckBox_CheckedChanged", $"the tick will sleep {SlowTickMs} ms while applying — long work inside a tick (anti-pattern)");
                ShowBanner($"⚠ Slow tick armed: every refresh now takes {SlowTickMs} ms. At 250 ms cadence the ticks pile up — Wisej.NET serialises the requests of one session, and _refreshInProgress refuses whatever still overlaps.", BannerKind.Warn);
            }
            else
            {
                AddTrace(TraceDirection.Request, "slowTickCheckBox_CheckedChanged", "the tick applies the snapshot immediately again");
                HideBanner();
            }
            UpdateState();
        }

        #endregion

        #region Coalescing demo — a burst of model events becomes ONE update

        private void burstButton_Click(object sender, EventArgs e)
        {
            int before = Volatile.Read(ref _eventsReceived);
            int updatesBefore = _updatesApplied;

            AddTrace(TraceDirection.Request, "burstButton_Click", $"{BurstEvents} model changes at once — the queue burst of the lesson");

            for (int i = 0; i < BurstEvents; i++)
            {
                _model.Step();              // the model changed…
                SimulatedModelChanged();    // …and only says "I am dirty". No control, no push.
            }

            AddTrace(TraceDirection.Server, "burst", $"events received {before} → {Volatile.Read(ref _eventsReceived)} (+{BurstEvents}), updates applied still {updatesBefore} · the next refreshTimer_Tick applies ONE update with the final state");

            if (!_timerStarted)
                ShowBanner("ⓘ The burst is in the model. Tick Live mode to let refreshTimer apply it — one update, not 50.", BannerKind.Info);

            // Deliberately NOT refreshing the dashboard labels here: they belong to the UI cadence. The
            // SERVER STATE line below is the authoritative model — watch it run ahead of the labels.
            RenderCadence();
            UpdateState();
        }

        #endregion

        #region Timer ownership — the anti-pattern the guard refuses

        private void duplicateTimerButton_Click(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.Request, "duplicateTimerButton_Click", "the browser asked for a second refreshTimer.Start()");

            if (!_timerStarted)
            {
                AddTrace(TraceDirection.Server, "duplicateTimerButton_Click", "nothing to duplicate — refreshTimer is stopped (live mode is off)");
                ShowBanner("ⓘ Turn Live mode on first. The anti-pattern is a SECOND Start() on a timer that is already running.", BannerKind.Info);
                UpdateState();
                return;
            }

            StartRefreshTimer("duplicateTimerButton_Click");   // refused by the guard, with the trace line
            SetStatus("second timer refused — the page owns refreshTimer", StatusKind.Warn);
            UpdateState();
        }

        #endregion

        #region Rendering helpers

        private enum StatusKind { Normal, Warn, Error }
        private enum BannerKind { Info, Warn, Error }

        /// <summary>Renders a model snapshot into the dashboard labels. Called only at the UI cadence.</summary>
        private void ApplySnapshot(DashboardSnapshot snapshot)
        {
            openTicketsLabel.Text = snapshot.OpenTickets.ToString(CultureInfo.InvariantCulture);
            queueDepthLabel.Text = snapshot.QueueDepth.ToString(CultureInfo.InvariantCulture);
            avgWaitLabel.Text = FormatWait(snapshot.AvgWaitMinutes);
        }

        private void RenderConnection()
        {
            bool ws = Application.IsWebSocket;
            connectionLabel.Text = ws
                ? "● WebSocket connected — out-of-bound changes can be pushed; the timer refresh does not need it"
                : "○ HTTP only — live mode requests StartPolling(1000) so out-of-bound changes still arrive";
            connectionLabel.ForeColor = ws
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(232, 161, 60);
        }

        /// <summary>The four cadences of the lesson, with the numbers this session is actually running.</summary>
        private void RenderCadence()
        {
            int events = Volatile.Read(ref _eventsReceived);
            string model = _liveMode
                ? $"every {DashboardSimulator.IntervalMs} ms (simulator task) · v{_model.Snapshot().Version}"
                : $"stopped · v{_model.Snapshot().Version}";

            modelLabel.Text =
                $"model cadence {model}\n" +
                $"UI cadence    {CadenceText(_cadenceMs)} (refreshTimer) · events {events} → updates {_updatesApplied}";

            labelFourCadences.Text =
                $"model   {(_liveMode ? DashboardSimulator.IntervalMs + " ms" : "stopped")}   how often the data changes\n" +
                $"UI      {CadenceText(_cadenceMs)}   how often the user needs to see it (refreshTimer)\n" +
                $"push    none — a timer tick IS a request, its changes ride the response   ·   polling   {(_polling ? PollingMs + " ms (fallback ON)" : "off")}";
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

            int events = Volatile.Read(ref _eventsReceived);
            DashboardSnapshot snapshot = _model.Snapshot();
            string ratio = _updatesApplied == 0 ? "—" : (events / (double)_updatesApplied).ToString("0.0", CultureInfo.InvariantCulture) + " events per update";

            labelState.Text =
                $"SERVER STATE (authoritative · this session only)\n" +
                $"live={Low(_liveMode)} timer={(_timerStarted ? "running @ " + CadenceText(_cadenceMs) : "stopped")} slow tick={Low(slowTickCheckBox.Checked)} refresh in progress={Low(_refreshInProgress)} dirty={Low(_dashboardDirty)}\n" +
                $"events={events} updates={_updatesApplied} refused={_skippedTicks} idle ticks={_idleTicks} model v={snapshot.Version} ({ratio})\n" +
                $"IsWebSocket={Low(Application.IsWebSocket)} polling={Low(_polling)} ClientId={Application.ClientId} SessionId={session}\n" +
                $"model, simulator, timer and counters are MainPage instance fields — one per browser tab, never static";
        }

        private static string CadenceText(int milliseconds) =>
            milliseconds >= 1000
                ? (milliseconds / 1000d).ToString("0.###", CultureInfo.InvariantCulture) + " s"
                : milliseconds + " ms";

        private static string FormatWait(double minutes) =>
            minutes.ToString("0.0", CultureInfo.InvariantCulture) + " min";

        /// <summary>Server log: the technical detail stays here; the UI only gets a safe message.</summary>
        private static void LogError(string operation, Exception ex)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} {operation} failed for client {Application.ClientId}: {ex}");
        }

        private static string Low(bool value) => value ? "true" : "false";

        #endregion

        #region Cleanup — the view is gone, so nothing it started may survive it

        /// <summary>
        /// Ownership means cleanup. The page started the simulator and the timer, so the page stops them.
        /// Called from the Disposed handler wired in the constructor.
        /// </summary>
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
                // The session may already be tearing down: log it, never throw from Disposed.
                LogError("dispose cleanup", ex);
            }
        }

        #endregion
    }
}
