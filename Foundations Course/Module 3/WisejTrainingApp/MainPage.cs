using System;
using Wisej.Web;
using WisejTrainingApp.Views;

namespace WisejTrainingApp
{
    // Main shell owns the navigation buttons and the content panel.
    public partial class MainPage : Page
    {
        private string currentRole = "Support Agent";

        public MainPage()
        {
            InitializeComponent();
            NavigateTo("Dashboard");
            ApplyPermissions();
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

        private void btnSettings_Click(object sender, EventArgs e)
        {
            NavigateTo("Settings");
        }

        private void NavigateTo(string pageName)
        {
            pnlContent.Controls.Clear();

            UserControl view;
            switch (pageName)
            {
                case "Tickets":   view = new TicketsView();   break;
                case "Customers": view = new CustomersView(); break;
                case "Settings":  view = new SettingsView(currentRole); break;
                default:          view = new DashboardView(); break;
            }

            view.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(view);
            lblBreadcrumb.Text = "Home / " + pageName;
            lblStatus.Text = DateTime.Now.ToString("hh:mm:ss tt") + " - Opened " + pageName + " page.";
        }

        private void ApplyPermissions()
        {
            // Beginner version: a Support Agent can view Settings but not save.
            // Later this grows into real authentication and authorization.
            lblUser.Text = "Signed in as: " + currentRole;
        }
    }
}
