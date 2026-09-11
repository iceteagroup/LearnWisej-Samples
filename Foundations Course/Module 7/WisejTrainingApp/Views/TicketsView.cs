using System;
using Wisej.Web;
using WisejTrainingApp.Dialogs;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Views
{
    // The Module 5 ticket screen, unchanged in behaviour — only its place in the shell is new.
    public partial class TicketsView : UserControl
    {
        private readonly TicketService ticketService;
        private readonly Action<string> addActivity;
        private readonly BindingSource ticketsBindingSource = new BindingSource();

        public TicketsView(TicketService ticketService, Action<string> addActivity)
        {
            InitializeComponent();
            this.ticketService = ticketService;
            this.addActivity = addActivity;

            ticketsBindingSource.DataSource = ticketService.GetTickets();
            dgvTickets.DataSource = ticketsBindingSource;
        }

        private async void btnNew_Click(object sender, EventArgs e)
        {
            var dialog = new TicketDialog();
            if (await dialog.ShowDialogAsync() == DialogResult.OK)
            {
                ticketService.AddTicket(dialog.Ticket);
                ticketsBindingSource.ResetBindings(false);
                addActivity("Created ticket #" + dialog.Ticket.Id + ".");
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            Ticket selected = ticketsBindingSource.Current as Ticket;
            if (selected == null)
            {
                MessageBox.Show("Select a ticket to edit.");
                return;
            }

            var dialog = new TicketDialog(selected);
            if (await dialog.ShowDialogAsync() == DialogResult.OK)
            {
                ticketService.UpdateTicket(dialog.Ticket);
                ticketsBindingSource.ResetBindings(false);
                addActivity("Saved ticket #" + dialog.Ticket.Id + ".");
            }
        }
    }
}
