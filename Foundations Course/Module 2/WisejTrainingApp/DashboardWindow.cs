using System;
using Wisej.Web;

namespace WisejTrainingApp
{
    public partial class DashboardWindow : Form
    {
        public DashboardWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Status: Running";
            lblServerStatus.Text = "Server: Online";
            lblDatabaseStatus.Text = "Database: Online";
            lblApiStatus.Text = "API Service: Online";
            AddLog("Dashboard started.");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Status: Stopped";
            lblServerStatus.Text = "Server: Offline";
            lblDatabaseStatus.Text = "Database: Offline";
            lblApiStatus.Text = "API Service: Offline";
            AddLog("Dashboard stopped.");
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            lstEventLog.Items.Clear();
            lblStatus.Text = "Status: Idle";
            lblServerStatus.Text = "Server: Offline";
            lblDatabaseStatus.Text = "Database: Offline";
            lblApiStatus.Text = "API Service: Offline";
            AddLog("Dashboard reset.");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            AddLog("Status refreshed.");
        }

        private void AddLog(string message)
        {
            lstEventLog.Items.Add(DateTime.Now.ToString("hh:mm tt") + " - " + message);
        }
    }
}
