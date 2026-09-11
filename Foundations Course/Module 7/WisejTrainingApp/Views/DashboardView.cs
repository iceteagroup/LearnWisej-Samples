using System;
using System.Collections.Generic;
using System.Linq;
using Wisej.Web;
using WisejTrainingApp.Dialogs;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Views
{
    public partial class DashboardView : UserControl
    {
        private readonly TicketService ticketService;

        public DashboardView(TicketService ticketService)
        {
            InitializeComponent();
            this.ticketService = ticketService;
        }

        public void RefreshCards()
        {
            List<Ticket> tickets = ticketService.GetTickets();
            lblOpenValue.Text = tickets.Count(t => t.Status == "Open").ToString();
            lblInProgressValue.Text = tickets.Count(t => t.Status == "In Progress").ToString();
            lblClosedValue.Text = tickets.Count(t => t.Status == "Closed").ToString();
        }

        public void AddActivity(string message)
        {
            lstActivity.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " - " + message);
        }

        private async void btnNewTicket_Click(object sender, EventArgs e)
        {
            var dialog = new TicketDialog();
            if (await dialog.ShowDialogAsync() == DialogResult.OK)
            {
                ticketService.AddTicket(dialog.Ticket);
                RefreshCards();
                AddActivity("Created ticket #" + dialog.Ticket.Id + ".");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshCards();
            AddActivity("Dashboard refreshed.");
        }
    }
}
