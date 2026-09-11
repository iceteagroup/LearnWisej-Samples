using System;
using System.Globalization;
using System.Text;
using System.Threading;
using Wisej.Core;
using Wisej.Web;

namespace TicketOpsLive
{
    public partial class MainPage : Page
    {
        // Per-session state: instance fields of this page (one MainPage per browser tab).
        private IWisejComponent _context;        // captured on Load, used by out-of-bound code
        private string _clientId = "";           // identifies the browser (shared by its tabs)
        private string _sessionId = "";          // identifies this session (one per tab)
        private bool _registered;
        private bool _cleanedUp;
        private int _pushers;
        private bool _polling;

        public MainPage()
        {
            InitializeComponent();

            this.Disposed += MainPage_Disposed;
        }

        #region Load

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";

            if (Application.Session.Counter == null)
                Application.Session.Counter = 0;

            _context = Application.Current;
            _clientId = Application.ClientId ?? "";
            _sessionId = Application.SessionId ?? "";

            clientIdLabel.Text = _clientId;
            sessionIdLabel.Text = _sessionId;
            browserLabel.Text = DescribeBrowser();

            // Register first, then subscribe: this session does not receive its own "joined" event.
            _registered = SessionRegistry.Instance.Register(_sessionId);
            SessionRegistry.Instance.SessionsChanged += Registry_SessionsChanged;

            Application.ApplicationExit += Application_ApplicationExit;
            Application.SessionTimeout += Application_SessionTimeout;

            Log("Page loaded");
            RenderLiveSessions();
            RenderInspector();
        }

        #endregion

        #region Counter and background update

        private void incrementButton_Click(object sender, EventArgs e)
        {
            Application.Session.Counter = (int)Application.Session.Counter + 1;
            counterLabel.Text = Application.Session.Counter.ToString();

            Log($"Counter → {Application.Session.Counter}");
            RenderInspector();
        }

        private void backgroundButton_Click(object sender, EventArgs e)
        {
            var context = Application.Current;

            backgroundButton.Enabled = false;
            labelStatus.Text = "";
            Log("Background update started");
            RenderInspector();
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

                        lifecycleListBox.Items.Insert(0, "Background update at " + DateTime.Now);
                        RenderInspector();
                    });
                }
                catch (Exception ex)
                {
                    LogError("background update", ex);
                    try
                    {
                        Application.Update(context, () =>
                        {
                            if (!this.IsDisposed)
                                labelStatus.Text = "The background update failed. See the server log.";
                        });
                    }
                    catch (ObjectDisposedException) { }
                }
                finally
                {
                    FinishTask(context);
                }
            });
        }

        private void FinishTask(IWisejComponent context)
        {
            if (this.IsDisposed)
            {
                _pushers = Math.Max(0, _pushers - 1);
                return;
            }

            try
            {
                Application.Update(context, () =>
                {
                    if (this.IsDisposed)
                        return;

                    backgroundButton.Enabled = true;
                    EndPush();
                });
            }
            catch (ObjectDisposedException) { }
        }

        #endregion

        #region Second window, clear log

        private void openSessionButton_Click(object sender, EventArgs e)
        {
            // Same browser, second tab: a new session with its own MainPage and its own Application.Session.
            Log("Opening a second window");
            Application.Navigate("/", "_blank");
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            lifecycleListBox.Items.Clear();
            RenderInspector();
        }

        #endregion

        #region Lifecycle — ApplicationExit, SessionTimeout, Disposed

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            // Replace with your logging framework in production.
            Console.Error.WriteLine("TicketOps session exited: " + _clientId);

            if (!this.IsDisposed)
                Log("TicketOps session exited: " + _clientId);

            Cleanup();
        }

        private void Application_SessionTimeout(object sender, System.ComponentModel.HandledEventArgs e)
        {
            // e.Handled stays false: Wisej.NET shows its default dialog offering to prolong the session.
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} session {_sessionId} is about to time out.");

            if (!this.IsDisposed)
                Log("Session is about to time out");
        }

        private void MainPage_Disposed(object sender, EventArgs e)
        {
            Cleanup();
        }

        /// <summary>
        /// Releases everything subscribed in Load. Idempotent: ApplicationExit and Disposed can both fire, in either order.
        /// </summary>
        private void Cleanup()
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

            try { Application.ApplicationExit -= Application_ApplicationExit; } catch (Exception) { }
            try { Application.SessionTimeout -= Application_SessionTimeout; } catch (Exception) { }
        }

        #endregion

        #region Global service subscription

        /// <summary>
        /// Raised by the global <see cref="SessionRegistry"/> on a thread-pool thread with no session context:
        /// check IsDisposed, restore the captured context, and touch only this session's controls.
        /// </summary>
        private void Registry_SessionsChanged(object sender, SessionsChangedEventArgs e)
        {
            if (this.IsDisposed)
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

                    string verb = e.Change == SessionChange.Joined ? "joined" : "left";
                    Log($"Session {Short(e.SessionId)} {verb} · {e.Count} live session(s)");
                    RenderLiveSessions();
                    RenderInspector();
                });
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                LogError("SessionsChanged", ex);
            }
        }

        #endregion

        #region Delivery: WebSocket push, or polling while a task runs

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

        #region Helpers

        private void Log(string message)
        {
            lifecycleListBox.Items.Insert(0, $"{DateTime.Now:T} - {message}");
            while (lifecycleListBox.Items.Count > 200)
                lifecycleListBox.Items.RemoveAt(lifecycleListBox.Items.Count - 1);
        }

        /// <summary>Refreshes the time, the server thread serving this event, and the session counter.</summary>
        private void RenderInspector()
        {
            timeLabel.Text = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            threadLabel.Text = "#" + Environment.CurrentManagedThreadId.ToString(CultureInfo.InvariantCulture);
            counterLabel.Text = GetSessionCounter().ToString(CultureInfo.InvariantCulture);
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

        private static int GetSessionCounter()
        {
            try
            {
                object value = Application.Session.Counter;
                return value is int n ? n : 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

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
            catch (Exception)
            {
                return "(not available)";
            }
        }

        private void LogError(string operation, Exception ex)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} {operation} failed for session {_sessionId}: {ex}");
        }

        private static string Short(string id)
        {
            if (string.IsNullOrEmpty(id))
                return "—";
            return id.Length <= 8 ? id : id.Substring(0, 8);
        }

        #endregion
    }
}
