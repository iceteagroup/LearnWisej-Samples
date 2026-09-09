using System;
using Wisej.Web;
using WisejTrainingApp.Dialogs;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Tickets: the DataGridView (dgvTickets, bound through a BindingSource) and the four commands
    /// Create / Edit / Delete / Refresh. This is the data flow from lesson s46 §3 in one file:
    /// click → TicketDialog → validation → TicketService → RefreshTicketGrid() → status + activity log.
    ///
    /// Each handler has the shape the lab asks for: open the dialog, await it, call the service, refresh,
    /// say what happened. No rule about tickets lives here — TicketValidator and TicketService own those.
    /// </summary>
    public partial class TicketsView : HelpdeskView
    {
        private readonly TicketService ticketService;
        private readonly BindingSource ticketsBindingSource = new BindingSource();

        public TicketsView(IHelpdeskShell shell, TicketService ticketService)
            : base(shell)
        {
            InitializeComponent();
            this.ticketService = ticketService;

            ConfigureGrid();
        }

        public override void ActivateScreen()
        {
            RefreshTicketGrid();
        }

        /// <summary>Columns are declared once, by hand, so the grid shows exactly Id / Title / Customer / Status / Priority / Assigned To / Created.</summary>
        private void ConfigureGrid()
        {
            dgvTickets.AutoGenerateColumns = false;
            dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Id", Name = "colId", Width = 60 });
            dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "Title", Name = "colTitle", Width = 300 });
            dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Customer", HeaderText = "Customer", Name = "colCustomer", Width = 170 });
            dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status", Name = "colStatus", Width = 110 });
            dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Priority", HeaderText = "Priority", Name = "colPriority", Width = 90 });
            dgvTickets.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "AssignedTo", HeaderText = "Assigned To", Name = "colAssignedTo", Width = 120 });

            var colCreated = new DataGridViewTextBoxColumn { DataPropertyName = "CreatedDate", HeaderText = "Created", Name = "colCreated", Width = 110 };
            colCreated.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvTickets.Columns.Add(colCreated);

            dgvTickets.DataSource = ticketsBindingSource;
        }

        /// <summary>The one refresh (s47 §1 "intentional refresh"): re-read from the service, rebind, clear the selection.</summary>
        private void RefreshTicketGrid()
        {
            ticketsBindingSource.DataSource = ticketService.GetTickets();
            ticketsBindingSource.ResetBindings(false);
            dgvTickets.ClearSelection();
            UpdateSelectionHint();
        }

        /// <summary>The ticket behind the selected row, or null when nothing is selected.</summary>
        private Ticket SelectedTicket
        {
            get
            {
                if (dgvTickets.SelectedRows.Count == 0)
                    return null;
                return dgvTickets.SelectedRows[0].DataBoundItem as Ticket;
            }
        }

        #region Commands — the lab's handler shape

        private async void btnCreateTicket_Click(object sender, EventArgs e)
        {
            var dialog = new TicketDialog();
            if (await dialog.ShowDialogAsync() == DialogResult.OK)
            {
                ticketService.AddTicket(dialog.TicketResult);
                RefreshTicketGrid();
                lblStatus.Text = "Ticket created successfully.";
                lblStatus.ForeColor = OkColor;
                Shell.AddActivity($"btnCreateTicket_Click → TicketDialog OK → TicketService.AddTicket(#{dialog.TicketResult.Id} \"{dialog.TicketResult.Title}\") → RefreshTicketGrid()");
                Shell.TicketsChanged();
            }
            else
            {
                ShowStatus(lblStatus, "Create cancelled — nothing changed.", StatusKind.Warn);
                Shell.AddActivity("btnCreateTicket_Click → TicketDialog Cancel → nothing changed");
            }
        }

        private async void btnEditTicket_Click(object sender, EventArgs e)
        {
            Ticket selected = SelectedTicket;
            if (selected == null)
            {
                // Guard: a friendly message instead of a null reference.
                ShowStatus(lblStatus, "Select a ticket in the grid first, then click Edit.", StatusKind.Warn);
                Shell.AddActivity("btnEditTicket_Click → no row selected → guard message");
                return;
            }

            var dialog = new TicketDialog(selected.Clone());
            if (await dialog.ShowDialogAsync() == DialogResult.OK)
            {
                ticketService.UpdateTicket(dialog.TicketResult);
                RefreshTicketGrid();
                ShowStatus(lblStatus, $"Ticket #{dialog.TicketResult.Id} updated successfully.", StatusKind.Ok);
                Shell.AddActivity($"btnEditTicket_Click → TicketDialog OK → TicketService.UpdateTicket(#{dialog.TicketResult.Id}) → RefreshTicketGrid()");
                Shell.TicketsChanged();
            }
            else
            {
                ShowStatus(lblStatus, $"Edit of ticket #{selected.Id} cancelled — nothing changed.", StatusKind.Warn);
                Shell.AddActivity($"btnEditTicket_Click → TicketDialog Cancel → ticket #{selected.Id} unchanged");
            }
        }

        private async void btnDeleteTicket_Click(object sender, EventArgs e)
        {
            Ticket selected = SelectedTicket;
            if (selected == null)
            {
                ShowStatus(lblStatus, "Select a ticket in the grid first, then click Delete.", StatusKind.Warn);
                Shell.AddActivity("btnDeleteTicket_Click → no row selected → guard message");
                return;
            }

            DialogResult answer = await MessageBox.ShowAsync(
                $"Delete ticket #{selected.Id} \"{selected.Title}\"?\nThis cannot be undone.",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
            {
                ShowStatus(lblStatus, $"Delete of ticket #{selected.Id} cancelled.", StatusKind.Warn);
                Shell.AddActivity($"btnDeleteTicket_Click → MessageBox No → ticket #{selected.Id} kept");
                return;
            }

            ticketService.DeleteTicket(selected.Id);
            RefreshTicketGrid();
            ShowStatus(lblStatus, $"Ticket #{selected.Id} deleted.", StatusKind.Ok);
            Shell.AddActivity($"btnDeleteTicket_Click → MessageBox Yes → TicketService.DeleteTicket({selected.Id}) → RefreshTicketGrid()");
            Shell.TicketsChanged();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshTicketGrid();
            ShowStatus(lblStatus, "Grid refreshed from TicketService.GetTickets().", StatusKind.Ok);
            Shell.AddActivity("btnRefresh_Click → RefreshTicketGrid()");
        }

        #endregion

        private void dgvTickets_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectionHint();
        }

        private void UpdateSelectionHint()
        {
            Ticket selected = SelectedTicket;
            lblSelection.Text = selected == null
                ? "No ticket selected — Edit and Delete will ask you to select one."
                : $"Selected: #{selected.Id} · {selected.Title} · {selected.Customer} · {selected.Status} / {selected.Priority}";
        }
    }
}
