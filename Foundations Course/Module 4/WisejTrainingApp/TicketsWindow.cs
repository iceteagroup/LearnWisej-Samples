using System;
using Wisej.Web;
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

            // Load first, then bind: the bindings need the BindingSource to already have its data.
            LoadTickets();
            BindDetailControls();
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

        private void BindDetailControls()
        {
            txtTitle.DataBindings.Add("Text", ticketsBindingSource, "Title", true,
                DataSourceUpdateMode.OnPropertyChanged);

            cmbStatus.DataBindings.Add("Text", ticketsBindingSource, "Status", true,
                DataSourceUpdateMode.OnPropertyChanged);

            cmbPriority.DataBindings.Add("Text", ticketsBindingSource, "Priority", true,
                DataSourceUpdateMode.OnPropertyChanged);

            dtpCreated.DataBindings.Add("Value", ticketsBindingSource, "CreatedDate", true,
                DataSourceUpdateMode.OnPropertyChanged);
        }

        private void btnSaveTicket_Click(object sender, EventArgs e)
        {
            Ticket ticket = SelectedTicket;
            if (ticket == null)
                return;

            ticketService.SaveTicket(ticket);
            ticketsBindingSource.ResetBindings(false);
            lblStatus.Text = "Saved ticket #" + ticket.Id;
        }
    }
}
