using System;
using System.Collections.Generic;
using System.Globalization;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp
{
    /// <summary>
    /// System Dashboard — Module 2 lab window (Using the Wisej.NET Designer / Properties, events and Designer code).
    ///
    /// Left card:   the lab screen exactly as the lesson pictures it — lblTitle, lblStatus, a Panel (pnlServices)
    ///              with lblServerStatus / lblDatabaseStatus / lblApiStatus, and btnStart / btnStop / btnReset /
    ///              btnRefresh. Each button has one small Click handler below; the repeated logging is in AddLog().
    ///              chkSimulateOutage is the failure switch: Refresh then reports the API Service as Degraded.
    /// Below it:    "Where your code goes" — which file owns what, and the properties / events / methods vocabulary.
    /// Right card:  the Event Log — lstEventLog, fed only through AddLog(string) with a DateTime.Now timestamp.
    /// Bottom bar:  the failure path (outage + Refresh) and the recovery (outage cleared + Refresh).
    ///
    /// Everything that changes the screen runs here on the server; Wisej.NET sends the changed properties
    /// (label Text / ForeColor, new log items) to the browser when the handler returns.
    /// </summary>
    public partial class DashboardWindow : Form
    {
        // Per-session state: one monitor per user (an instance field, never static).
        private readonly ServiceMonitor monitor = new ServiceMonitor();

        public DashboardWindow()
        {
            // InitializeComponent() builds every control from DashboardWindow.Designer.cs — the Designer owns that file.
            InitializeComponent();
        }

        private void DashboardWindow_Load(object sender, EventArgs e)
        {
            // The beginner lifecycle: Program.Main → new DashboardWindow() → InitializeComponent() → Load → user events.
            AddLog("Program.Main → new DashboardWindow().Show()");
            AddLog("InitializeComponent() built lblTitle, lblStatus, pnlServices, btnStart…btnRefresh, lstEventLog from DashboardWindow.Designer.cs");
            AddLog("DashboardWindow_Load → ready; Status: Idle, every service Offline — waiting for btnStart.Click");
            ShowServices(monitor.CheckAll());
        }

        #region The lab's four Click handlers (double-click a button in the Designer to create one)

        /// <summary>Start: every service goes Online, the status says Running, and it is logged.</summary>
        private void btnStart_Click(object sender, EventArgs e)
        {
            monitor.Start();
            ShowServices(monitor.CheckAll());
            SetStatus("Status: Running", StatusKind.Ok);
            AddLog("Dashboard started.");
        }

        /// <summary>Stop: services drop to Offline and the stop is logged.</summary>
        private void btnStop_Click(object sender, EventArgs e)
        {
            monitor.Stop();
            ShowServices(monitor.CheckAll());
            SetStatus("Status: Stopped", StatusKind.Error);
            AddLog("Dashboard stopped.");
        }

        /// <summary>Reset: the log clears, services go Offline, status returns to Idle, then the reset is logged.</summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            chkSimulateOutage.Checked = false;   // before the clear, so its CheckedChanged line is wiped too
            lstEventLog.Items.Clear();
            monitor.Reset();
            ShowServices(monitor.CheckAll());
            SetStatus("Status: Idle", StatusKind.Idle);
            AddLog("Dashboard reset.");
        }

        /// <summary>Refresh: re-checks every service through the monitor and adds a fresh log entry.</summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            monitor.SimulateApiOutage = chkSimulateOutage.Checked;

            List<ServiceCheck> checks = monitor.CheckAll();
            ShowServices(checks);

            ServiceCheck failed = checks.Find(c => c.State == ServiceState.Degraded);
            if (failed != null)
            {
                // Failure path: one check timed out. The dashboard reports it instead of pretending all is well.
                SetStatus("Status: Degraded", StatusKind.Warn);
                AddLog($"{failed.Name} check failed: {failed.Error}");
                return;
            }

            // Success path (and the recovery after an outage): the status follows the monitor.
            if (monitor.IsRunning)
                SetStatus("Status: Running", StatusKind.Ok);

            AddLog("Status refreshed — " + string.Join(" · ", checks.ConvertAll(c => c.Summary)));
        }

        #endregion

        #region Failure switch and bottom bar (failure path + recovery through the same Refresh handler)

        private void chkSimulateOutage_CheckedChanged(object sender, EventArgs e)
        {
            AddLog(chkSimulateOutage.Checked
                ? "chkSimulateOutage checked → the next Refresh will time out the API Service check"
                : "chkSimulateOutage unchecked → the next Refresh checks the API Service normally");
        }

        private void btnSimulateOutage_Click(object sender, EventArgs e)
        {
            chkSimulateOutage.Checked = true;
            AddLog("btnSimulateOutage_Click → calling btnRefresh_Click with the outage switched on");
            btnRefresh_Click(sender, e);
        }

        private void btnRecover_Click(object sender, EventArgs e)
        {
            chkSimulateOutage.Checked = false;
            AddLog("btnRecover_Click → calling btnRefresh_Click with the outage switched off");
            btnRefresh_Click(sender, e);
        }

        #endregion

        #region Helpers (small, reusable — the habit the lesson teaches)

        private enum StatusKind { Idle, Ok, Warn, Error }

        /// <summary>Paints the three indicator labels from a set of checks — the only place that knows which label is which.</summary>
        private void ShowServices(List<ServiceCheck> checks)
        {
            foreach (ServiceCheck check in checks)
            {
                Label label = check.Name switch
                {
                    ServiceMonitor.DatabaseName => lblDatabaseStatus,
                    ServiceMonitor.ApiName => lblApiStatus,
                    _ => lblServerStatus,
                };

                label.Text = check.DisplayText;
                label.ForeColor = check.State switch
                {
                    ServiceState.Online => System.Drawing.Color.FromArgb(31, 157, 87),
                    ServiceState.Degraded => System.Drawing.Color.FromArgb(232, 161, 60),
                    _ => System.Drawing.Color.FromArgb(224, 86, 59),
                };
            }
        }

        private void SetStatus(string text, StatusKind kind)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                StatusKind.Ok => System.Drawing.Color.FromArgb(31, 157, 87),
                _ => System.Drawing.Color.FromArgb(90, 107, 125),
            };
        }

        /// <summary>One place for logging, so every handler stays short. lstEventLog is only ever written through here.</summary>
        private void AddLog(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            lstEventLog.Items.Add($"{time}  {message}");
            lstEventLog.SelectedIndex = lstEventLog.Items.Count - 1;
        }

        #endregion
    }
}
