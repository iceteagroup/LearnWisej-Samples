using System;
using System.Globalization;
using System.Threading;
using Wisej.Web;

namespace TicketOpsLive
{
    public partial class MainPage : Page
    {
        // Per-session state lives in instance fields: every browser tab has its own MainPage.
        private volatile bool _running;
        private bool _serverEventRunning;
        private int _heartbeat;
        private int _pushers;
        private bool _polling;

        public MainPage()
        {
            InitializeComponent();

            this.Disposed += (s, e) => _running = false;
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";
        }

        #region Refresh — in-request update

        private void refreshButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Refreshed at " + DateTime.Now.ToLongTimeString();
            RenderConnection();
            AddTrace("request", "Refresh → " + statusLabel.Text);
        }

        #endregion

        #region Server event — out-of-bound push

        private void serverEventButton_Click(object sender, EventArgs e)
        {
            if (_serverEventRunning)
                return;

            _serverEventRunning = true;
            serverEventButton.Enabled = false;
            pushStatusLabel.Text = "starting…";
            BeginPush();

            Application.StartTask(() =>
            {
                try
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        pushStatusLabel.Text = $"step {i}/5";
                        AddTrace("push", $"Server event step {i}/5");
                        Application.Update(this);
                        Thread.Sleep(600);
                    }
                    pushStatusLabel.Text = "completed";
                }
                catch (Exception ex)
                {
                    LogError("server event", ex);
                    pushStatusLabel.Text = "Server event failed. See the server log.";
                }
                finally
                {
                    _serverEventRunning = false;
                    serverEventButton.Enabled = true;
                    EndPush();
                    Application.Update(this);
                }
            });
        }

        #endregion

        #region Heartbeat

        private void startButton_Click(object sender, EventArgs e)
        {
            if (_running)
                return;

            _running = true;
            startButton.Enabled = false;
            stopButton.Enabled = true;
            activityLabel.Text = "Heartbeat starting…";
            AddTrace("request", "Heartbeat started");
            BeginPush();

            Application.StartTask(HeartbeatLoop);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _running = false;
            stopButton.Enabled = false;
            activityLabel.Text = "Heartbeat stopping…";
        }

        private void HeartbeatLoop()
        {
            var random = new Random();
            bool faulted = false;

            try
            {
                while (_running && !this.IsDisposed)
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
                _running = false;
                if (!this.IsDisposed)
                {
                    try
                    {
                        Application.Update(this, () =>
                        {
                            startButton.Enabled = true;
                            stopButton.Enabled = false;
                            activityLabel.Text = faulted ? "Heartbeat failed. See the server log." : "Heartbeat stopped";
                            AddTrace("push", activityLabel.Text);
                            EndPush();
                        });
                    }
                    catch (ObjectDisposedException) { }
                }
            }
        }

        #endregion

        #region Delivery: WebSocket push, or polling while a task runs

        // IsWebSocket is false during Load (the socket opens after the first response), so polling is
        // requested here, when out-of-bound work starts without a WebSocket, and ended when it finishes.
        private void BeginPush()
        {
            _pushers++;
            RenderConnection();
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
            RenderConnection();
        }

        #endregion

        #region Helpers

        private void RenderConnection()
        {
            bool ws = Application.IsWebSocket;
            connectionLabel.Text = ws
                ? "● WebSocket connected"
                : _polling ? "○ No WebSocket — polling every second" : "○ No WebSocket";
            connectionLabel.ForeColor = ws ? System.Drawing.Color.FromArgb(31, 157, 87) : System.Drawing.Color.FromArgb(232, 161, 60);
        }

        private void AddTrace(string kind, string text)
        {
            listTrace.Items.Add($"{DateTime.Now:HH:mm:ss}  {kind,-8} {text}");
            while (listTrace.Items.Count > 200)
                listTrace.Items.RemoveAt(0);
            listTrace.SelectedIndex = listTrace.Items.Count - 1;
        }

        private static void LogError(string operation, Exception ex)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} {operation} failed for client {Application.ClientId}: {ex}");
        }

        #endregion
    }
}
