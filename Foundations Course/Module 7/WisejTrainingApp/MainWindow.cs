using System;
using System.Collections.Generic;
using System.Drawing;
using Wisej.Web;
using WisejTrainingApp.Services;
using WisejTrainingApp.Views;

namespace WisejTrainingApp
{
    public partial class MainWindow : Form
    {
        private readonly TicketService ticketService = new TicketService();

        // The pages are created once and swapped in and out of panelContent.
        private readonly Dictionary<string, UserControl> views = new Dictionary<string, UserControl>();
        private readonly DashboardView dashboardView;

        public MainWindow()
        {
            InitializeComponent();

            dashboardView = new DashboardView(ticketService);
            views["Dashboard"] = dashboardView;
            views["Tickets"] = new TicketsView(ticketService, AddActivity);
            views["Customers"] = new CustomersView();
            views["Reports"] = new ReportsView();
            views["Settings"] = new SettingsView();

            cboTheme.SelectedItem = "Bootstrap-4";
            NavigateTo("Dashboard");
        }

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

        private void btnReports_Click(object sender, EventArgs e)
        {
            NavigateTo("Reports");
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            NavigateTo("Settings");
        }

        private void NavigateTo(string pageName)
        {
            UserControl view = views[pageName];

            panelContent.Controls.Clear();
            view.Dock = DockStyle.Fill;
            panelContent.Controls.Add(view);

            if (view == dashboardView)
                dashboardView.RefreshCards();

            SetActiveButton(pageName);
            AddActivity("Opened " + pageName + ".");
        }

        // One consistent active state: the selected nav button stands out, the others stay plain.
        private void SetActiveButton(string pageName)
        {
            foreach (Button button in new[] { btnDashboard, btnTickets, btnCustomers, btnReports, btnSettings })
            {
                if (button.Text == pageName)
                {
                    button.BackColor = Color.FromArgb(37, 99, 235);
                    button.ForeColor = Color.White;
                    button.Font = new Font("default", 10F, FontStyle.Bold);
                }
                else
                {
                    button.ResetBackColor();
                    button.ResetForeColor();
                    button.Font = new Font("default", 10F);
                }
            }
        }

        private void cboTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTheme = cboTheme.Text;

            // Apply the selected theme — keep this UI setting separate
            // from ticket service logic.
            Application.LoadTheme(selectedTheme);
            lblCurrentTheme.Text = "Current theme: " + selectedTheme;
            AddActivity("Theme changed to " + selectedTheme + ".");
        }

        private void AddActivity(string message)
        {
            dashboardView.AddActivity(message);
        }
    }
}
