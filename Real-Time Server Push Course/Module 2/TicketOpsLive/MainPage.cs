using System;
using System.Globalization;
using System.Text;
using System.Threading;
using Wisej.Core;
using Wisej.Web;

namespace TicketOpsLive
{
    public enum TraceDirection { Push, Request, Server }

    /// <summary>
    /// TicketOps Live — Module 2 · Who Owns the UI: Sessions, Context, Exit, and Timeout.
    ///
    /// Left card:    the Session inspector (diagnosticsGroupBox) — current time, Application.ClientId,
    ///               Application.SessionId, Application.Browser, the server thread id (refreshed on every
    ///               event: the thread shifts between events because it is reused, not owned),
    ///               Application.IsWebSocket, the per-session counter (Application.Session.Counter) and,
    ///               right below it, the same counter written to a static field — the trap. Open a second
    ///               tab and watch the static one leak across sessions while the session one does not.
    /// Second card:  the Lifecycle logger (lifecycleListBox) — page loaded, every button, the arrival of a
    ///               background update, ApplicationExit, SessionTimeout, Disposed — plus liveSessionsLabel,
    ///               fed by the global SessionRegistry through Application.Update(_context, …).
    /// Right card:   every push the server makes, every request the browser sends, every server decision.
    /// Bottom bar:   success (session counter), the trap (static counter), progress (background update),
    ///               failure (background fault, caught inside the task), a second session, and the end of
    ///               this one (Application.Exit → the ApplicationExit handler cleans everything up).
    /// </summary>
    public partial class MainPage : Page
    {
        // ── The trap, on purpose ───────────────────────────────────────────────────────────────────────
        // ONE field for the whole server process. Every session increments the same integer, so the value
        // a session shows is the sum of what every other session did. This is exactly what the lesson calls
        // the static-field trap; it is here to be seen failing, never to be copied.
        private static int _sharedCounter;

        // ── Per-session state: instance fields of THIS page (one MainPage per browser tab) ─────────────
        private IWisejComponent _context;        // captured on Load, used by out-of-bound code
        private string _clientId = "";           // cached: Application.ClientId identifies the BROWSER (shared by all its tabs)
        private string _sessionId = "";          // cached: Application.SessionId identifies THIS session (one per tab) — the registry key
        private bool _registered;                // this session is in the global SessionRegistry
        private bool _cleanedUp;                 // Cleanup() is idempotent: exit AND dispose both call it
        private int _liveSessions;
        private int _events;                     // how many times a server thread served this session
        private int _pushes;
        private int _tasks;                      // background tasks in flight
        private int _pushers;                    // tasks that currently need out-of-bound delivery
        private bool _polling;                   // the fallback is active (no WebSocket when a task started)

        public MainPage()
        {
            InitializeComponent();

            // Cleanup rule #1: whatever the page subscribed to, the page releases when it is disposed.
            // Disposed and ApplicationExit can both fire (in either order) — Cleanup() is idempotent.
            this.Disposed += MainPage_Disposed;
        }

        #region Load — capture the context, register the session, subscribe

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";

            // The lab snippet: the counter lives in the per-session bag, not in a static field.
            if (Application.Session.Counter == null)
                Application.Session.Counter = 0;

            // Reflection question 1: Application.Current is captured HERE, while this code runs because the
            // browser made a request and the session context is available. Out-of-bound code (a task, a
            // thread-pool callback of a global service) has no context of its own and would not know which
            // session to update; Application.Update(_context, …) restores this one.
            _context = Application.Current;
            _clientId = Application.ClientId ?? "";
            _sessionId = Application.SessionId ?? "";

            // Session identity — read once, in context.
            clientIdLabel.Text = _clientId;
            sessionIdLabel.Text = Application.SessionId ?? "";
            browserLabel.Text = DescribeBrowser();

            // The global service: register first, then subscribe. Registering first means this session does
            // not receive its own "Joined" event while Load is still running (the registry raises on a
            // thread-pool thread); it reads the count directly instead.
            _registered = SessionRegistry.Instance.Register(_sessionId);   // keyed on SessionId: two tabs of one browser share the ClientId
            SessionRegistry.Instance.SessionsChanged += Registry_SessionsChanged;

            // Cleanup rule #2: a global-service subscription needs an unsubscribe plan before it is made.
            Application.ApplicationExit += Application_ApplicationExit;
            Application.SessionTimeout += Application_SessionTimeout;

            _liveSessions = SessionRegistry.Instance.Count;

            Log("Page loaded for this session.");
            AddTrace(TraceDirection.Request, "MainPage_Load", $"first request: the page is rendered and returned with the response · IsWebSocket={Low(Application.IsWebSocket)} (the socket opens after this response)");
            AddTrace(TraceDirection.Server, "SessionRegistry.Register", $"session {Short(_sessionId)} registered (ClientId {Short(_clientId)} is the browser, shared by its tabs) · SessionsChanged subscribed · ApplicationExit + SessionTimeout subscribed");
            SetStatus("session ready · this tab owns this UI", StatusKind.Normal);
            RenderLiveSessions();
            RenderInspector("request thread (MainPage_Load)");
            UpdateState();
        }

        #endregion

        #region Success path — the per-session counter (Application.Session)

        private void incrementButton_Click(object sender, EventArgs e)
        {
            // The lab snippet, unchanged. Application.Session is a dynamic bag isolated per session: the
            // second tab has its own Counter, and a browser refresh starts a new session with Counter = 0.
            if (Application.Session.Counter == null)
                Application.Session.Counter = 0;
            Application.Session.Counter = (int)Application.Session.Counter + 1;
            counterLabel.Text = Application.Session.Counter.ToString();

            Log($"Session counter → {GetSessionCounter()} (Application.Session, this session only)");
            AddTrace(TraceDirection.Request, "incrementButton_Click", $"Application.Session.Counter = {GetSessionCounter()} — in-request update, no push needed");
            SetStatus("session counter incremented", StatusKind.Normal);
            RenderInspector("request thread (incrementButton_Click)");
            UpdateState();
        }

        #endregion

        #region The static-field trap — deliberately wrong

        private void incrementSharedButton_Click(object sender, EventArgs e)
        {
            // Interlocked makes this thread-safe — and it is still wrong. Thread safety is not the problem:
            // SHARING is. Every session on this server increments the same field, so this number is not
            // "your" number. Open a second tab, click there, then look at this label again.
            int shared = Interlocked.Increment(ref _sharedCounter);

            Log($"Static counter → {shared} (ONE field for the whole server — every session sees this)");
            AddTrace(TraceDirection.Server, "static int _sharedCounter", $"{shared} — shared by every session on this server; a per-user value must never live here");
            ShowBanner("⚠ The static counter is shared by every session on this server. Open a second session and watch it move without you clicking here.", BannerKind.Warn);
            SetStatus("static counter incremented — shared state", StatusKind.Warn);
            RenderInspector("request thread (incrementSharedButton_Click)");
            UpdateState();
        }

        #endregion

        #region Progress path — the simulated background update (the lab snippet)

        private void backgroundButton_Click(object sender, EventArgs e)
        {
            // Captured while we are in context — this is the whole point of the module.
            var context = Application.Current;

            _tasks++;
            backgroundButton.Enabled = false;
            HideBanner();
            Log("Background update requested (1500 ms on a task).");
            AddTrace(TraceDirection.Request, "backgroundButton_Click", "the handler returns now; the work continues on a task started with Application.StartTask");
            SetStatus("background update running…", StatusKind.Normal);
            BeginPush();
            UpdateState();

            Application.StartTask(() =>
            {
                try
                {
                    Thread.Sleep(1500);                       // the "work": a queue read, a service call, a report…

                    // Application.Update(context, …) runs the callback in THIS session's context and pushes
                    // the result in one flush. Without the context the task would not know which of the
                    // server's sessions owns lifecycleListBox.
                    Application.Update(context, () =>
                    {
                        if (this.IsDisposed)                  // the tab may be gone: the task outlives it
                            return;

                        lifecycleListBox.Items.Insert(0, "Background update at " + DateTime.Now);
                        _pushes++;
                        AddTrace(TraceDirection.Push, "Application.Update(context, …)", $"\"Background update at …\" delivered to {Short(_clientId)} — out-of-bound, no click");
                        SetStatus("background update delivered", StatusKind.Normal);
                        RenderInspector("task thread (Application.StartTask)");
                        UpdateState();
                    });
                }
                catch (Exception ex)
                {
                    LogError("background update", ex);
                }
                finally
                {
                    // The UI is restored whatever happened above — and only if the UI is still there.
                    FinishTask(context, "background update finished");
                }
            });
        }

        #endregion

        #region Failure path — the background task throws

        private void faultButton_Click(object sender, EventArgs e)
        {
            var context = Application.Current;

            _tasks++;
            faultButton.Enabled = false;
            HideBanner();
            Log("Background fault armed: the task throws after 500 ms.");
            AddTrace(TraceDirection.Request, "faultButton_Click", "the task will throw after 500 ms — the exception is caught INSIDE the task");
            SetStatus("background fault running…", StatusKind.Warn);
            BeginPush();
            UpdateState();

            Application.StartTask(() =>
            {
                try
                {
                    Thread.Sleep(500);
                    throw new InvalidOperationException($"Simulated failure in the background job of client {_clientId}.");
                }
                catch (Exception ex)
                {
                    // A background task must never fail silently: the detail goes to the server log, the
                    // user gets a safe message — never a stack trace.
                    LogError("background job", ex);

                    try
                    {
                        Application.Update(context, () =>
                        {
                            if (this.IsDisposed)
                                return;

                            Log("Background job failed. See the server log.");
                            _pushes++;
                            ShowBanner($"✖ The background job failed. The detail is in the server log for client {Short(_clientId)}. Click “Background update” to run a healthy one.", BannerKind.Error);
                            SetStatus("background job failed — UI recovered", StatusKind.Error);
                            AddTrace(TraceDirection.Push, "Application.Update(context, …)", $"{ex.GetType().Name} caught inside the task → server log + safe message pushed to {Short(_clientId)}");
                            RenderInspector("task thread (Application.StartTask · catch)");
                            UpdateState();
                        });
                    }
                    catch (ObjectDisposedException) { /* the page went away between the check and the push */ }
                }
                finally
                {
                    FinishTask(context, "background fault handled");
                }
            });
        }

        /// <summary>Re-enables the action bar and pushes the final state — from the task's finally block.</summary>
        private void FinishTask(IWisejComponent context, string reason)
        {
            if (this.IsDisposed)
            {
                // No UI left to restore. The counters still have to come back to a sane state.
                _tasks = Math.Max(0, _tasks - 1);
                _pushers = Math.Max(0, _pushers - 1);
                return;
            }

            try
            {
                Application.Update(context, () =>
                {
                    if (this.IsDisposed)
                        return;

                    _tasks = Math.Max(0, _tasks - 1);
                    backgroundButton.Enabled = true;
                    faultButton.Enabled = true;
                    _pushes++;
                    EndPush();
                    AddTrace(TraceDirection.Push, "Application.Update(context, …)", reason + " — buttons re-enabled in finally (final state pushed)");
                    RenderInspector("task thread (finally)");
                    UpdateState();
                });
            }
            catch (ObjectDisposedException) { /* the page went away between the check and the push */ }
        }

        #endregion

        #region A second session, and the end of this one

        private void openSessionButton_Click(object sender, EventArgs e)
        {
            // Same user, same browser, second tab — and a DIFFERENT session: its own MainPage, its own
            // Application.Session.Counter, its own ClientId. The static counter, though, is the same one.
            Log("Opening a second session in a new browser tab.");
            AddTrace(TraceDirection.Request, "openSessionButton_Click", "Application.Navigate(\"/\", \"_blank\") — one login, two tabs, two sessions");
            SetStatus("second session requested", StatusKind.Normal);
            RenderInspector("request thread (openSessionButton_Click)");
            UpdateState();

            Application.Navigate("/", "_blank");
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Log("End of session requested (Application.Exit).");
            AddTrace(TraceDirection.Request, "exitButton_Click", "Application.Exit() — the session terminates; the ApplicationExit handler unsubscribes and unregisters it");
            SetStatus("session ending…", StatusKind.Warn);
            UpdateState();

            // Terminates this session (closer to quitting a desktop application than to closing a page).
            // ApplicationExit fires, Cleanup() runs, and the OTHER sessions get the "left" event.
            Application.Exit();
        }

        #endregion

        #region Lifecycle — ApplicationExit, SessionTimeout, Disposed (the cleanup rules)

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            // Replace with your logging framework in production.
            Console.Error.WriteLine("TicketOps session exited: " + _clientId);

            if (!this.IsDisposed)
            {
                Log("TicketOps session exited: " + _clientId);
                AddTrace(TraceDirection.Server, "Application.ApplicationExit", $"{Short(_clientId)} is shutting down → unsubscribe + unregister");
            }

            Cleanup("ApplicationExit");
        }

        private void Application_SessionTimeout(object sender, System.ComponentModel.HandledEventArgs e)
        {
            // e.Handled stays false: Wisej.NET's default dialog offering to prolong the session is exactly
            // what a real operator console wants. Setting it to true would suppress the dialog and let the
            // session die silently. A timed-out session is REMOVED from memory — the page, its controls and
            // its Application.Session bag go with it, which is why background work must never assume the UI
            // is still there.
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} session {_clientId} is about to time out (Handled={e.Handled}).");

            if (this.IsDisposed)
                return;

            Log("SessionTimeout: this session is about to expire.");
            AddTrace(TraceDirection.Server, "Application.SessionTimeout", "the session is about to expire — the default 'prolong the session?' dialog is left enabled (e.Handled = false)");
            ShowBanner("⚠ This session is about to time out. When it does, the session is removed from memory: page, controls and Application.Session go with it.", BannerKind.Warn);
            SetStatus("session about to time out", StatusKind.Warn);
        }

        private void MainPage_Disposed(object sender, EventArgs e)
        {
            Cleanup("Disposed");
        }

        /// <summary>
        /// The one cleanup path of this sample — idempotent, because ApplicationExit and Disposed can both
        /// fire, in either order. Every subscription made in Load is released here (see docs/CleanupRules.md).
        /// </summary>
        private void Cleanup(string reason)
        {
            if (_cleanedUp)
                return;
            _cleanedUp = true;

            try { SessionRegistry.Instance.SessionsChanged -= Registry_SessionsChanged; } catch (Exception ex) { LogError("unsubscribe SessionsChanged", ex); }

            if (_registered)
            {
                _registered = false;
                try { SessionRegistry.Instance.Unregister(_sessionId); } catch (Exception ex) { LogError("unregister session", ex); }
            }

            try { Application.ApplicationExit -= Application_ApplicationExit; } catch (Exception) { /* no context left */ }
            try { Application.SessionTimeout -= Application_SessionTimeout; } catch (Exception) { /* no context left */ }

            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} cleanup ({reason}) for client {_clientId}: SessionsChanged unsubscribed, session unregistered, ApplicationExit/SessionTimeout released.");
        }

        #endregion

        #region The global service — one subscription, one handler, one cleanup rule

        /// <summary>
        /// Raised by the global <see cref="SessionRegistry"/> when ANY session joins or leaves. It arrives on
        /// a thread-pool thread with no session context at all — the classic shape of a hub, a queue reader or
        /// a file watcher. Three rules: check IsDisposed, restore the context that was captured on Load, and
        /// touch only this session's own controls inside the callback.
        /// </summary>
        private void Registry_SessionsChanged(object sender, SessionsChangedEventArgs e)
        {
            if (this.IsDisposed)                     // the page may be gone; the registry event outlives it
                return;

            IWisejComponent context = _context;
            if (context == null)
                return;

            try
            {
                Application.Update(context, () =>
                {
                    if (this.IsDisposed)
                        return;

                    _liveSessions = e.Count;
                    _pushes++;
                    string verb = e.Change == SessionChange.Joined ? "joined" : "left";
                    Log($"Session {Short(e.SessionId)} {verb} · {e.Count} live session(s)");
                    RenderLiveSessions();
                    AddTrace(TraceDirection.Push, "Application.Update(_context, …)", $"SessionRegistry: {Short(e.SessionId)} {verb} → {e.Count} live · pushed into session {Short(_sessionId)} from a thread-pool thread");
                    RenderInspector("thread-pool thread (SessionRegistry.SessionsChanged)");
                    UpdateState();
                });
            }
            catch (ObjectDisposedException)
            {
                // The page went away between the IsDisposed check and the push: nothing to do, the
                // subscription is released by Cleanup().
            }
            catch (Exception ex)
            {
                LogError("SessionsChanged", ex);
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

        #region Session state helpers

        /// <summary>Reads the per-session counter out of the dynamic <c>Application.Session</c> bag.</summary>
        private static int GetSessionCounter()
        {
            try
            {
                object value = Application.Session.Counter;      // Application.Session is dynamic
                return value is int n ? n : 0;
            }
            catch (Exception)
            {
                return 0;                                        // no session context on this thread
            }
        }

        /// <summary>Application.Browser: what the client is. Guarded — the values come from the client.</summary>
        private static string DescribeBrowser()
        {
            try
            {
                ClientBrowser browser = Application.Browser;
                if (browser == null)
                    return "(not available)";

                string type = string.IsNullOrEmpty(browser.Type) ? "?" : browser.Type;
                string os = string.IsNullOrEmpty(browser.OS) ? "?" : browser.OS;
                string device = string.IsNullOrEmpty(browser.Device) ? "?" : browser.Device;
                return $"{type} {browser.Version.ToString(CultureInfo.InvariantCulture)} · {os} · {device}";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[TicketOpsLive] Application.Browser could not be read: {ex.Message}");
                return "(not available)";
            }
        }

        #endregion

        #region UI helpers

        private enum StatusKind { Normal, Warn, Error }
        private enum BannerKind { Info, Warn, Error }

        /// <summary>
        /// The lifecycle logger, newest first. Every event of this session lands here: load, clicks, the
        /// arrival of a background update, exit, timeout, disposal.
        /// </summary>
        private void Log(string message)
        {
            lifecycleListBox.Items.Insert(0, $"{DateTime.Now:T} - {message}");
            while (lifecycleListBox.Items.Count > 200)
                lifecycleListBox.Items.RemoveAt(lifecycleListBox.Items.Count - 1);
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

        /// <summary>
        /// Refreshes the volatile half of the inspector. <paramref name="origin"/> says which kind of thread
        /// is running right now: the id changes between events because the server thread is a pool thread —
        /// it is reused, not owned by the session. Nothing about the session lives on it.
        /// </summary>
        private void RenderInspector(string origin)
        {
            _events++;
            timeLabel.Text = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture) + "   (server time)";
            threadLabel.Text = $"#{Environment.CurrentManagedThreadId} · {origin} · event {_events} — reused, not owned";
            websocketLabel.Text = Application.IsWebSocket
                ? "true — Application.Update pushes out-of-bound"
                : "false — no live channel yet; a task requests StartPolling(1000)";
            counterLabel.Text = $"{GetSessionCounter()}   ← Application.Session.Counter (this session only)";
            sharedCounterLabel.Text = $"{Volatile.Read(ref _sharedCounter)}   ← static int, shared by EVERY session of this server";
        }

        private void RenderLiveSessions()
        {
            SessionInfo[] sessions = SessionRegistry.Instance.GetSnapshot();
            _liveSessions = sessions.Length;

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
                text.Append("  (+").Append((sessions.Length - 6).ToString(CultureInfo.InvariantCulture)).Append(" more)");

            liveSessionsLabel.Text = text.ToString();
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
                "SERVER STATE (authoritative · this session only)\n" +
                $"session.Counter={GetSessionCounter()} static _sharedCounter={Volatile.Read(ref _sharedCounter)} (server-wide!) liveSessions={_liveSessions} tasks={_tasks} pushes={_pushes} polling={Low(_polling)}\n" +
                $"IsWebSocket={Low(Application.IsWebSocket)} ClientId={_clientId} SessionId={session} thread=#{Environment.CurrentManagedThreadId}\n" +
                "the counter lives in Application.Session (per session) · the static int lives in the process (every session) · the page owns its controls";
        }

        /// <summary>Server log: the technical detail stays here; the UI only gets a safe message.</summary>
        private void LogError(string operation, Exception ex)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} {operation} failed for client {_clientId}: {ex}");
        }

        private static string Short(string id)
        {
            if (string.IsNullOrEmpty(id))
                return "—";
            return id.Length <= 8 ? id : id.Substring(0, 8);
        }

        private static string Low(bool value) => value ? "true" : "false";

        #endregion
    }
}
