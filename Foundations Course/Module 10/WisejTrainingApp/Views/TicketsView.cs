using System;
using Wisej.Web;
using WisejTrainingApp.Dialogs;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Views
{
    public partial class TicketsView : UserControl
    {
        private readonly TicketService ticketService;
        private readonly Action<string> setStatus;
        private readonly BindingSource ticketsBindingSource = new BindingSource();

        public TicketsView(TicketService ticketService, Action<string> setStatus)
        {
            InitializeComponent();
            this.ticketService = ticketService;
            this.setStatus = setStatus;

            dgvTickets.DataSource = ticketsBindingSource;
            RefreshTicketGrid();
        }

        private Ticket SelectedTicket
        {
            get { return ticketsBindingSource.Current as Ticket; }
        }

        private async void btnCreateTicket_Click(object sender, EventArgs e)
        {
            var dialog = new TicketDialog();
            if (await dialog.ShowDialogAsync() != DialogResult.OK)
                return;

            ticketService.AddTicket(dialog.Ticket);
            RefreshTicketGrid();
            setStatus("Ticket #" + dialog.Ticket.Id + " created.");
        }

        private async void btnEditTicket_Click(object sender, EventArgs e)
        {
            Ticket selected = SelectedTicket;
            if (selected == null)
            {
                MessageBox.Show("Select a ticket to edit.");
                return;
            }

            var dialog = new TicketDialog(selected);
            if (await dialog.ShowDialogAsync() != DialogResult.OK)
                return;

            ticketService.UpdateTicket(dialog.Ticket);
            RefreshTicketGrid();
            setStatus("Ticket #" + dialog.Ticket.Id + " saved.");
        }

        private async void btnDeleteTicket_Click(object sender, EventArgs e)
        {
            Ticket selected = SelectedTicket;
            if (selected == null)
            {
                MessageBox.Show("Select a ticket to delete.");
                return;
            }

            DialogResult answer = await MessageBox.ShowAsync(
                "Delete ticket #" + selected.Id + "?",
                "Delete Ticket",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
                return;

            ticketService.DeleteTicket(selected.Id);
            RefreshTicketGrid();
            setStatus("Ticket #" + selected.Id + " deleted.");
        }

        // Refresh on purpose after every change.
        private void RefreshTicketGrid()
        {
            ticketsBindingSource.DataSource = ticketService.GetTickets();
        }
    }
}
