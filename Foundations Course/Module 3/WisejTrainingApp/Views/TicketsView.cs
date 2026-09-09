using System;
using System.Collections.Generic;
using System.Globalization;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Tickets page: the ticket list in a DataGridView with the two actions lesson s12 names —
    /// New Ticket and Close Selected. Both roles may do both (see PermissionService); the failure
    /// path here is "nothing selected" / "already closed".
    /// </summary>
    public partial class TicketsView : UserControl
    {
        private static readonly string[] SampleCustomers =
            { "Northwind", "Contoso", "Fabrikam", "Adventure Works", "Tailspin", "Wide World Importers" };

        private readonly IShellHost shell;
        private readonly TicketService ticketService;

        public TicketsView(IShellHost shell, TicketService ticketService)
        {
            this.shell = shell;
            this.ticketService = ticketService;

            InitializeComponent();
        }

        private void TicketsView_Load(object sender, EventArgs e)
        {
            LoadTickets();
        }

        /// <summary>Fills the grid from the service (unbound rows — Module 4 introduces data binding).</summary>
        private void LoadTickets(int selectId = 0)
        {
            List<Ticket> tickets = ticketService.GetTickets();

            dgvTickets.Rows.Clear();
            foreach (Ticket t in tickets)
            {
                int rowIndex = dgvTickets.Rows.Add(new object[]
                {
                    t.Id, t.Title, t.Customer, t.Status, t.Priority,
                    string.IsNullOrEmpty(t.AssignedTo) ? "—" : t.AssignedTo,
                    t.CreatedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                });

                if (t.Id == selectId)
                    dgvTickets.Rows[rowIndex].Selected = true;
            }

            lblTicketsSummary.Text = $"{tickets.Count} tickets · {ticketService.CountOpen()} open";
        }

        private void btnNewTicket_Click(object sender, EventArgs e)
        {
            // A quick "new ticket" without a dialog — Module 5 adds the real TicketDialog.
            int n = ticketService.GetTickets().Count;
            var ticket = new Ticket
            {
                Title = "New ticket #" + (n + 1),
                Customer = SampleCustomers[n % SampleCustomers.Length],
                Status = "Open",
                Priority = "Medium",
                AssignedTo = "",
                CreatedDate = DateTime.Now,
                Description = "Created from the Tickets page.",
            };

            ticketService.AddTicket(ticket);
            LoadTickets(ticket.Id);
            shell.Log($"Ticket #{ticket.Id} created for {ticket.Customer}.");
        }

        private void btnCloseSelected_Click(object sender, EventArgs e)
        {
            Ticket ticket = GetSelectedTicket();

            if (ticket == null)
            {
                // Failure path: nothing selected.
                shell.Log("Close Selected: no ticket selected.", LogKind.Warn);
                AlertBox.Show("Select a ticket first.", MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            if (ticket.Status == "Closed")
            {
                shell.Log($"Ticket #{ticket.Id} is already closed.", LogKind.Warn);
                return;
            }

            ticket.Status = "Closed";
            ticketService.UpdateTicket(ticket);
            LoadTickets(ticket.Id);
            shell.Log($"Ticket #{ticket.Id} closed ({ticket.Customer}).");
        }

        /// <summary>The Id sits in the first column; the service owns the real Ticket.</summary>
        private Ticket GetSelectedTicket()
        {
            DataGridViewRow row = dgvTickets.CurrentRow;
            if (row == null && dgvTickets.SelectedRows.Count > 0)
                row = dgvTickets.SelectedRows[0];
            if (row == null || row.Cells[0].Value == null)
                return null;

            int id = Convert.ToInt32(row.Cells[0].Value, CultureInfo.InvariantCulture);
            return ticketService.GetTicket(id);
        }
    }
}
