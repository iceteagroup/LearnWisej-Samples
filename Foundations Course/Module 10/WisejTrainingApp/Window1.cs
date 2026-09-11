using System;
using System.Collections.Generic;
using System.Drawing;
using Wisej.Web;
using WisejTrainingApp.Services;
using WisejTrainingApp.Views;

namespace WisejTrainingApp
{
    // The shell: header, left navigation, content area and status bar.
    public partial class Window1 : Form
    {
        private readonly TicketService ticketService = new TicketService();

        // Each screen is created once and swapped in and out of pnlContent.
        private readonly Dictionary<string, UserControl> views = new Dictionary<string, UserControl>();
        private readonly DashboardView dashboardView;

        public Window1()
        {
            InitializeComponent();

            dashboardView = new DashboardView(ticketService);
            views["Dashboard"] = dashboardView;
            views["Tickets"] = new TicketsView(ticketService, SetStatus);
            views["Architecture"] = new ArchitectureView();
            views["Code Review"] = new CodeReviewView();
            views["Deployment"] = new DeploymentView();
            views["Next Steps"] = new NextStepsView();

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

        private void btnArchitecture_Click(object sender, EventArgs e)
        {
            NavigateTo("Architecture");
        }

        private void btnCodeReview_Click(object sender, EventArgs e)
        {
            NavigateTo("Code Review");
        }

        private void btnDeployment_Click(object sender, EventArgs e)
        {
            NavigateTo("Deployment");
        }

        private void btnNextSteps_Click(object sender, EventArgs e)
        {
            NavigateTo("Next Steps");
        }

        private void NavigateTo(string screen)
        {
            UserControl view = views[screen];

            pnlContent.Controls.Clear();
            view.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(view);

            if (view == dashboardView)
                dashboardView.RefreshCards();

            SetActiveButton(screen);
            SetStatus("Opened " + screen + ".");
        }

        private void SetActiveButton(string screen)
        {
            foreach (Button button in new[] { btnDashboard, btnTickets, btnArchitecture, btnCodeReview, btnDeployment, btnNextSteps })
            {
                if (button.Text == screen)
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

        private void SetStatus(string message)
        {
            lblStatus.Text = DateTime.Now.ToString("HH:mm:ss") + " - " + message;
        }
    }
}
