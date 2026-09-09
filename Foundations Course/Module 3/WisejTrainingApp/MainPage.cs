using System;
using System.Collections.Generic;
using System.Globalization;
using Wisej.Web;
using WisejTrainingApp.Services;
using WisejTrainingApp.Views;

namespace WisejTrainingApp
{
    /// <summary>
    /// The application shell — Module 3 (Application shell and navigation / Reusable navigation and permissions).
    ///
    /// Four regions that never change (all docked, see MainPage.Designer.cs):
    ///   pnlHeader  Dock.Top     app title, "Signed in as", breadcrumb, and the role picker (cboRole)
    ///   pnlNav     Dock.Left    btnDashboard / btnTickets / btnCustomers / btnSettings
    ///   pnlContent Dock.Fill    the current view (a UserControl) — the only thing that swaps
    ///   lblStatus  Dock.Bottom  "hh:mm:ss tt - Opened Tickets page."
    ///
    /// One shared NavigateTo(pageName) does all the page changing; every nav button only calls it.
    /// The shell is the IShellHost the views talk to: Log() writes the status bar and the session's
    /// recent-activity list that DashboardView shows; CurrentRole is what the header says.
    /// </summary>
    public partial class MainPage : Page, IShellHost
    {
        // Per-session state lives in instance fields (never static): each browser gets its own MainPage.
        private string currentRole = PermissionService.SupportAgent;
        private string currentPage = "";
        private readonly List<string> activity = new List<string>();

        private readonly PermissionService permissions = new PermissionService();
        private readonly TicketService ticketService = new TicketService();
        private readonly CustomerService customerService = new CustomerService();
        private readonly HealthCheckService healthCheck = new HealthCheckService();

        public MainPage()
        {
            InitializeComponent();

            // The header ComboBox mirrors currentRole; selecting it here does not log (same role).
            cboRole.SelectedItem = currentRole;

            Log("Application shell loaded.");
            ApplyPermissions();
            NavigateTo("Dashboard");
        }

        #region IShellHost — what the views may ask of the shell

        public string CurrentRole => currentRole;

        public IReadOnlyList<string> Activity => activity;

        /// <summary>
        /// The one place that writes the status bar. Every navigation and every permission decision
        /// goes through here, so the bottom bar and the Dashboard's "Recent activity" always agree.
        /// </summary>
        public void Log(string message, LogKind kind = LogKind.Info)
        {
            string line = DateTime.Now.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture) + " - " + message;

            lblStatus.Text = line;
            lblStatus.ForeColor = kind switch
            {
                LogKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                LogKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };

            activity.Add(line);

            // If the Dashboard is on screen, its Recent activity card follows the shell live.
            if (pnlContent.Controls.Count > 0 && pnlContent.Controls[0] is DashboardView dashboard)
                dashboard.AddLog(line);
        }

        #endregion

        #region Navigation — one shared method, four one-line handlers

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            NavigateTo("Dashboard");
        }

        private void btnTickets_Click(object sender, EventArgs e)
        {
            NavigateTo("Tickets");
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            NavigateTo("Customers");
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            NavigateTo("Settings");
        }

        /// <summary>
        /// The only method that changes the page: clear the content panel, create the matching view,
        /// dock it to fill, update the breadcrumb and the status bar, highlight the nav button.
        /// </summary>
        private void NavigateTo(string pageName)
        {
            pnlContent.Controls.Clear();

            UserControl view;
            switch (pageName)
            {
                case "Tickets": view = new TicketsView(this, ticketService); break;
                case "Customers": view = new CustomersView(this, customerService, ticketService, permissions); break;
                case "Settings": view = new SettingsView(currentRole, this, permissions); break;
                default: pageName = "Dashboard"; view = new DashboardView(this, ticketService, customerService, healthCheck); break;
            }

            view.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(view);

            currentPage = pageName;
            lblBreadcrumb.Text = "Home / " + pageName;
            SetActiveButton(pageName);
            Log("Opened " + pageName + " page.");
        }

        /// <summary>Highlights the nav button for the current page and resets the other three.</summary>
        private void SetActiveButton(string pageName)
        {
            var buttons = new Dictionary<string, Button>
            {
                { "Dashboard", btnDashboard },
                { "Tickets", btnTickets },
                { "Customers", btnCustomers },
                { "Settings", btnSettings },
            };

            foreach (var pair in buttons)
            {
                bool active = pair.Key == pageName;
                pair.Value.BackColor = active ? System.Drawing.Color.FromArgb(37, 99, 235) : System.Drawing.Color.White;
                pair.Value.ForeColor = active ? System.Drawing.Color.White : System.Drawing.Color.FromArgb(31, 41, 55);
                pair.Value.Font = new System.Drawing.Font("default", 10F, active ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular);
            }
        }

        #endregion

        #region Permissions — the role picker and ApplyPermissions

        private void cboRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            string role = cboRole.SelectedItem as string ?? currentRole;
            if (role == currentRole)
                return;

            currentRole = role;
            Log("Role changed to " + currentRole + " - permissions re-applied.");
            ApplyPermissions();

            // The open view was built for the old role; rebuild it so what it enables matches the new one.
            if (currentPage != "")
                NavigateTo(currentPage);
        }

        /// <summary>
        /// Beginner version of authorization: no login, just a role. The header says who is signed in and
        /// the nav buttons explain what the role may do; the views ask PermissionService again before
        /// they enable or run a protected action. Later this grows into real authentication and authorization.
        /// </summary>
        private void ApplyPermissions()
        {
            lblUser.Text = "Signed in as: " + currentRole;

            btnSettings.ToolTipText = permissions.CanSaveSettings(currentRole)
                ? "Settings — this role can save changes."
                : "Settings — view only for this role.";
            btnCustomers.ToolTipText = permissions.CanEditCustomers(currentRole)
                ? "Customers — this role can add customers."
                : "Customers — view only for this role.";
            btnTickets.ToolTipText = "Tickets — create and close tickets.";
            btnDashboard.ToolTipText = "Dashboard — overview cards, health check and recent activity.";
        }

        #endregion
    }
}
