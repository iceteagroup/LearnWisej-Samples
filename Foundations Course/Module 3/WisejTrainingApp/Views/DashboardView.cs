using System;
using System.Globalization;
using Wisej.Web;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Dashboard page: three overview cards (Open Tickets, Customers, System Status), the
    /// "Run Health Check" quick action, and the course's event log — here called "Recent activity",
    /// fed by the shell's Log() so it shows every navigation and permission decision of the session.
    /// </summary>
    public partial class DashboardView : UserControl
    {
        private readonly IShellHost shell;
        private readonly TicketService ticketService;
        private readonly CustomerService customerService;
        private readonly HealthCheckService healthCheck;

        public DashboardView(IShellHost shell, TicketService ticketService, CustomerService customerService, HealthCheckService healthCheck)
        {
            this.shell = shell;
            this.ticketService = ticketService;
            this.customerService = customerService;
            this.healthCheck = healthCheck;

            InitializeComponent();
        }

        private void DashboardView_Load(object sender, EventArgs e)
        {
            RefreshCards();

            // Replay what the shell logged before this view existed, so navigating back loses nothing.
            lstEventLog.Items.Clear();
            foreach (string line in shell.Activity)
                AddLog(line);
        }

        /// <summary>The numbers come from the services the shell owns — never from the view itself.</summary>
        private void RefreshCards()
        {
            lblOpenTicketsValue.Text = ticketService.CountOpen().ToString(CultureInfo.InvariantCulture);
            lblCustomersValue.Text = customerService.Count().ToString(CultureInfo.InvariantCulture);

            if (healthCheck.Runs == 0)
            {
                lblSystemStatusValue.Text = "Not checked yet";
                lblSystemStatusValue.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
                lblSystemStatusDetail.Text = "Click Run Health Check.";
            }
        }

        private void btnRunHealthCheck_Click(object sender, EventArgs e)
        {
            var report = healthCheck.Run(ticketService.CountOpen());

            lblSystemStatusValue.Text = report.Summary;
            lblSystemStatusValue.ForeColor = report.AllHealthy
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(232, 161, 60);
            lblSystemStatusDetail.Text = "Checked " + DateTime.Now.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture)
                + " · run #" + healthCheck.Runs;

            shell.Log($"Health check #{healthCheck.Runs}: {report.Summary} ({string.Join("; ", report.Details)})",
                report.AllHealthy ? LogKind.Info : LogKind.Warn);
        }

        /// <summary>One place that writes the activity list; the shell calls it after every Log().</summary>
        public void AddLog(string line)
        {
            lstEventLog.Items.Add(line);
            lstEventLog.SelectedIndex = lstEventLog.Items.Count - 1;
        }
    }
}
