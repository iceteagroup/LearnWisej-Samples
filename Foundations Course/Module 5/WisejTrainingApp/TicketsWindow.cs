using System;
using Wisej.Web;
using WisejTrainingApp.Dialogs;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp
{
    public partial class TicketsWindow : Form
    {
        private TicketService ticketService = new TicketService();
        private BindingSource ticketsBindingSource = new BindingSource();

        public TicketsWindow()
        {
            InitializeComponent();
            LoadTickets();
        }

        private void LoadTickets()
        {
            ticketsBindingSource.DataSource = ticketService.GetTickets();
            dgvTickets.DataSource = ticketsBindingSource;
        }

        private Ticket SelectedTicket
        {
            get { return ticketsBindingSource.Current as Ticket; }
        }

        // Wisej.NET dialogs are awaited: the handler continues when the dialog closes.
        private async void btnNew_Click(object sender, EventArgs e)
        {
            var dialog = new TicketDialog();
            if (await dialog.ShowDialogAsync() == DialogResult.OK)
            {
                ticketService.AddTicket(dialog.Ticket);
                RefreshGrid();
                AlertBox.Show("Ticket created.");
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            Ticket selected = SelectedTicket;
            if (selected == null)
            {
                MessageBox.Show("Select a ticket to edit.");
                return;
            }

            var dialog = new TicketDialog(selected);
            if (await dialog.ShowDialogAsync() == DialogResult.OK)
            {
                ticketService.UpdateTicket(dialog.Ticket);
                RefreshGrid();
                AlertBox.Show("Ticket saved.");
            }
        }

        private void RefreshGrid()
        {
            ticketsBindingSource.ResetBindings(false);
        }
    }
}
