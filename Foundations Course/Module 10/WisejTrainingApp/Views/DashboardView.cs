using System;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Dashboard: four summary cards (Total / Open / In Progress / Closed) computed by TicketService.GetSummary(),
    /// and the recent-activity card (lstActivity) every screen logs into through the shell's AddActivity().
    /// The cards never count anything themselves — the numbers come from the service, so they always agree
    /// with the Tickets grid.
    /// </summary>
    public partial class DashboardView : HelpdeskView
    {
        private readonly TicketService ticketService;

        public DashboardView(IHelpdeskShell shell, TicketService ticketService)
            : base(shell)
        {
            InitializeComponent();
            this.ticketService = ticketService;
        }

        public override void ActivateScreen()
        {
            RefreshSummary();
        }

        /// <summary>Recomputed on every change (Window1.TicketsChanged) and every time the screen opens.</summary>
        public void RefreshSummary()
        {
            TicketSummary summary = ticketService.GetSummary();

            lblTotal.Text = summary.Total.ToString();
            lblOpen.Text = summary.Open.ToString();
            lblInProgress.Text = summary.InProgress.ToString();
            lblClosed.Text = summary.Closed.ToString();

            ShowStatus(lblStatus, $"summary refreshed from TicketService.GetSummary() — {summary.Total} tickets", StatusKind.Ok);
        }

        /// <summary>Called by Window1.AddActivity — the line already carries its timestamp.</summary>
        public void AppendActivity(string line)
        {
            lstActivity.Items.Add(line);
            lstActivity.SelectedIndex = lstActivity.Items.Count - 1;
        }

        private void btnRefreshSummary_Click(object sender, EventArgs e)
        {
            RefreshSummary();
            Shell.AddActivity("btnRefreshSummary_Click → DashboardView.RefreshSummary()");
        }

        private void btnOpenTickets_Click(object sender, EventArgs e)
        {
            Shell.NavigateTo("Tickets");
        }

        private void btnClearActivity_Click(object sender, EventArgs e)
        {
            lstActivity.Items.Clear();
        }
    }
}
