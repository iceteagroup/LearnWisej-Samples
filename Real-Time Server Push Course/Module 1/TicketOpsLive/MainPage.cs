using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Wisej.Web;

namespace TicketOpsLive
{
    public enum TraceDirection { Push, Request, Server }

    /// <summary>
    /// TicketOps Live — Module 1 · Live Status Strip.
    ///
    /// Status strip: clock, connection state (WebSocket vs HTTP-only), activity message, server load bar.
    /// Left card:    the three update mechanisms side by side — in-request (refreshButton), out-of-bound
    ///               WebSocket push (serverEventButton, the heartbeat) and the polling fallback — plus a
    ///               cadence experiment (100 pushes at 10 ms vs 10 batched pushes).
    /// Right card:   every push the server makes and every request the browser sends, with a timestamp.
    /// Bottom bar:   success path (Refresh, Server event ×5), progress path (heartbeat), failure path
    ///               (Inject fault → caught inside the task), recovery (Start again), the cadence experiment.
    /// </summary>
    public partial class MainPage : Page
    {
        // Per-session state lives in INSTANCE fields: every browser tab has its own MainPage, so every
        // tab has its own heartbeat. A static field here would be shared by every user of the server.
        private volatile bool _running;
        private volatile bool _faultRequested;
        private bool _busy;                 // the server-event / cadence experiments (one at a time)
        private int _heartbeat;
        private int _pushes;
        private int _pushers;               // tasks that currently need out-of-bound delivery
        private bool _polling;              // the fallback is active (no WebSocket when a task started)

        public MainPage()
        {
            InitializeComponent();

            // The page going away is a stop condition too: the loop checks _running and IsDisposed.
            this.Disposed += (s, e) => _running = false;
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";

            // Mechanism 3 — polling fallback — is NOT requested here. During Load the WebSocket is not
            // connected yet (IsWebSocket is false: the socket opens right after the first response), so a
            // StartPolling(1000) placed here starts real HTTP polling that keeps running after the socket
            // connects. The fallback is requested where the out-of-bound work starts: see BeginPush().
            AddTrace(TraceDirection.Request, "MainPage_Load", $"first request: the page is rendered and returned with the response · IsWebSocket={Low(Application.IsWebSocket)} (the socket opens after this response)");
            RenderConnection();
            SetStatus("idle · click a button", StatusKind.Normal);
            UpdateState();
        }

        #region Mechanism 1 — in-request update (the response carries the change)

        private void refreshButton_Click(object sender, EventArgs e)
        {
            // Nothing to push: the click IS a request, and Wisej.NET returns the changed control
            // properties in the response of that request.
            statusLabel.Text = "Refreshed at " + DateTime.Now.ToLongTimeString();
            AddTrace(TraceDirection.Request, "refreshButton_Click", "the browser sent the click");
            AddTrace(TraceDirection.Server, "in-request update", $"statusLabel.Text = \"{statusLabel.Text}\" → returned with the response, no Application.Update()");
            RenderConnection();
            SetStatus("in-request update delivered with the response", StatusKind.Normal);
            UpdateState();
        }

        #endregion

        #region Mechanism 2 — out-of-bound push (the walkthrough's 5-step server event)

        private void serverEventButton_Click(object sender, EventArgs e)
        {
            if (_busy) { ShowBanner("⚠ Another experiment is still running — wait for it to finish.", BannerKind.Warn); return; }

            _busy = true;
            serverEventButton.Enabled = false;
            pushStatusLabel.Text = "Server event started…";
            AddTrace(TraceDirection.Request, "serverEventButton_Click", "the handler returns immediately; the work continues on a task");
            AddTrace(TraceDirection.Server, "Application.StartTask", "5 steps, 600 ms apart, each pushed with Application.Update(this)");
            BeginPush();

            // Application.StartTask keeps THIS session's context on the new thread, so the task can change
            // the page's controls directly. Nothing reaches the browser until Application.Update(this)
            // pushes the pending changes over the WebSocket.
            Application.StartTask(() =>
            {
                try
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        pushStatusLabel.Text = $"Server event step {i}/5";
                        _pushes++;
                        AddTrace(TraceDirection.Push, "Application.Update(this)", $"pushStatusLabel = \"Server event step {i}/5\"");
                        UpdateState();
                        Application.Update(this);
                        Thread.Sleep(600);
                    }
                    pushStatusLabel.Text = "Server event done — 5 pushes, 0 clicks";
                    AddTrace(TraceDirection.Server, "server event", "completed");
                }
                catch (Exception ex)
                {
                    LogError("server event", ex);
                    pushStatusLabel.Text = "Server event failed. See the server log.";
                }
                finally
                {
                    _busy = false;
                    serverEventButton.Enabled = true;
                    _pushes++;
                    EndPush();
                    UpdateState();
                    Application.Update(this);          // the final state is pushed immediately
                }
            });
        }

        #endregion

        #region Progress path — the heartbeat (lab task)

        private void startButton_Click(object sender, EventArgs e)
        {
            if (_running)
            {
                // Acceptance criterion: starting twice does not create two competing loops.
                AddTrace(TraceDirection.Server, "startButton_Click", "refused — the heartbeat is already running (_running == true)");
                ShowBanner("⚠ Heartbeat already running — a second click must not start a competing loop.", BannerKind.Warn);
                return;
            }

            _running = true;
            _faultRequested = false;
            startButton.Enabled = false;
            stopButton.Enabled = true;
            HideBanner();
            activityLabel.Text = "Heartbeat starting…";
            AddTrace(TraceDirection.Request, "startButton_Click", "the handler returns immediately; the loop runs on a task");
            AddTrace(TraceDirection.Server, "Application.StartTask", "heartbeat: one push per second while _running && !IsDisposed");
            SetStatus("heartbeat running", StatusKind.Normal);
            BeginPush();

            Application.StartTask(HeartbeatLoop);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            // Cooperative stop: the loop checks the flag once per second and exits by itself.
            _running = false;
            stopButton.Enabled = false;
            activityLabel.Text = "Heartbeat stopping… (ends within one second)";
            AddTrace(TraceDirection.Request, "stopButton_Click", "_running = false → the loop exits at its next check");
        }

        private void faultButton_Click(object sender, EventArgs e)
        {
            if (!_running)
            {
                ShowBanner("ⓘ Start the heartbeat first — the fault is injected into the running loop.", BannerKind.Info);
                return;
            }
            _faultRequested = true;
            AddTrace(TraceDirection.Request, "faultButton_Click", "the next heartbeat throws InvalidOperationException inside the task");
        }

        /// <summary>
        /// The loop the lab asks for. It runs on the task thread (session context preserved by StartTask):
        /// compute off the request thread, then apply the UI changes and push them in ONE flush with
        /// Application.Update(this, callback). try/catch/finally guarantees the buttons are re-enabled.
        /// </summary>
        private void HeartbeatLoop()
        {
            var random = new Random();
            string reason = "stopped by operator";
            bool faulted = false;

            try
            {
                while (_running && !this.IsDisposed)
                {
                    int n = ++_heartbeat;
                    if (_faultRequested)
                    {
                        _faultRequested = false;
                        throw new InvalidOperationException($"Simulated sensor failure in heartbeat #{n}.");
                    }

                    // The "work": a fake load reading. Real code would read a queue, a service or a database here.
                    int load = random.Next(5, 95);
                    DateTime now = DateTime.Now;

                    Application.Update(this, () =>
                    {
                        clockLabel.Text = now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                        serverLoadBar.Value = load;
                        loadValueLabel.Text = load + " %";
                        activityLabel.Text = $"Heartbeat #{n} · load {load}% · pushed {now:HH:mm:ss.fff}";
                        _pushes++;
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
                // A background task must never fail silently: log the detail, show a safe message.
                faulted = true;
                reason = "fault: " + ex.Message;
                LogError("heartbeat", ex);
            }
            finally
            {
                _running = false;
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
                                ShowBanner($"✖ Heartbeat failed. See the server log (client {Application.ClientId}). Click ▶ Start heartbeat to recover.", BannerKind.Error);
                                SetStatus("fault — loop ended, UI re-enabled in finally", StatusKind.Error);
                            }
                            else
                            {
                                SetStatus("heartbeat stopped", StatusKind.Normal);
                            }
                            _pushes++;
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

        #region Cadence experiment — what 10 ms pushes cost

        private void burstButton_Click(object sender, EventArgs e) => RunCadenceExperiment(pushEvery: 1);

        private void batchedButton_Click(object sender, EventArgs e) => RunCadenceExperiment(pushEvery: 10);

        /// <summary>100 model changes, 10 ms apart. pushEvery=1 pushes all of them; pushEvery=10 coalesces to 10 pushes.</summary>
        private void RunCadenceExperiment(int pushEvery)
        {
            if (_busy) { ShowBanner("⚠ Another experiment is still running — wait for it to finish.", BannerKind.Warn); return; }

            _busy = true;
            burstButton.Enabled = false;
            batchedButton.Enabled = false;
            string mode = pushEvery == 1 ? "unthrottled (anti-pattern)" : $"batched (push every {pushEvery})";
            cadenceResultLabel.Text = $"{mode}: running…";
            AddTrace(TraceDirection.Request, pushEvery == 1 ? "burstButton_Click" : "batchedButton_Click", $"100 model changes 10 ms apart, {mode}");
            BeginPush();

            Application.StartTask(() =>
            {
                int pushes = 0;
                var watch = Stopwatch.StartNew();
                try
                {
                    for (int i = 1; i <= 100; i++)
                    {
                        serverLoadBar.Value = i;                                   // the model changed…
                        cadenceResultLabel.Text = $"{mode}: change {i}/100";
                        if (i % pushEvery == 0)
                        {
                            pushes++;                                              // …but only some changes are pushed
                            Application.Update(this);
                        }
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    LogError("cadence experiment", ex);
                }
                finally
                {
                    watch.Stop();
                    _busy = false;
                    _pushes += pushes + 1;
                    burstButton.Enabled = true;
                    batchedButton.Enabled = true;
                    EndPush();
                    cadenceResultLabel.Text = $"{mode}: 100 changes → {pushes} pushes in {watch.ElapsedMilliseconds} ms — every push costs serialization, a WebSocket frame and browser work";
                    AddTrace(TraceDirection.Push, "Application.Update(this)", $"{mode}: 100 changes → {pushes} pushes, {watch.ElapsedMilliseconds} ms (this line is the final push)");
                    UpdateState();
                    Application.Update(this);
                }
            });
        }

        #endregion

        #region Delivery: WebSocket push, or the polling fallback while a task needs it

        /// <summary>
        /// Called by every button that starts out-of-bound work. With a WebSocket the pushes flow by themselves.
        /// Without one, ask the browser to poll every second for the duration of the work — and only for that
        /// duration, because polling costs a request per second per session.
        /// </summary>
        private void BeginPush()
        {
            _pushers++;
            RenderConnection();
            if (Application.IsWebSocket || _polling)
                return;

            Application.StartPolling(1000);
            _polling = true;
            pollingLabel.Text = "IsWebSocket = false → Application.StartPolling(1000): the browser polls every second while a task runs";
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
            RenderConnection();
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
            listTrace.Items.Add($"{time}  {prefix} {name,-30} {payload}");
            while (listTrace.Items.Count > 400)
                listTrace.Items.RemoveAt(0);
            listTrace.SelectedIndex = listTrace.Items.Count - 1;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            listTrace.Items.Clear();
        }

        private void RenderConnection()
        {
            bool ws = Application.IsWebSocket;
            connectionLabel.Text = ws
                ? "● WebSocket connected — Application.Update pushes out-of-bound"
                : "○ HTTP only — no live channel; the polling fallback (1000 ms) delivers pending updates";
            connectionLabel.ForeColor = ws ? System.Drawing.Color.FromArgb(31, 157, 87) : System.Drawing.Color.FromArgb(232, 161, 60);
            if (_polling)
                pollingLabel.Text = "IsWebSocket = false → Application.StartPolling(1000): the browser polls every second while a task runs";
            else if (ws)
                pollingLabel.Text = "IsWebSocket = true → pushes arrive the moment Update() is called; no polling is requested";
            else
                pollingLabel.Text = "IsWebSocket = false → the next task will request StartPolling(1000) and EndPolling() when it ends";
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
                $"SERVER STATE (authoritative · this session only)\n" +
                $"heartbeat running={Low(_running)} beats={_heartbeat} pushes={_pushes} busy={Low(_busy)} tasks={_pushers} polling={Low(_polling)}\n" +
                $"IsWebSocket={Low(Application.IsWebSocket)} ClientId={Application.ClientId} SessionId={session}\n" +
                $"state lives in MainPage instance fields — one MainPage per browser tab, never static";
        }

        /// <summary>Server log: the technical detail stays here; the UI only gets a safe message.</summary>
        private static void LogError(string operation, Exception ex)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} {operation} failed for client {Application.ClientId}: {ex}");
        }

        private static string Low(bool value) => value ? "true" : "false";

        #endregion
    }
}
