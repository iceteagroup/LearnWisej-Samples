using System;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Customers: the companies behind the tickets (dgvCustomers) and a small inline form to add one.
    /// Same shape as Tickets, one size smaller: the grid binds through a BindingSource, the rules live in
    /// CustomerService.ValidateCustomer, and the handler only reads the fields, asks the service, refreshes.
    /// Selecting a customer shows how many tickets name that company — TicketService answers, not this screen.
    /// </summary>
    public partial class CustomersView : HelpdeskView
    {
        private readonly CustomerService customerService;
        private readonly TicketService ticketService;
        private readonly BindingSource customersBindingSource = new BindingSource();

        public CustomersView(IHelpdeskShell shell, CustomerService customerService, TicketService ticketService)
            : base(shell)
        {
            InitializeComponent();
            this.customerService = customerService;
            this.ticketService = ticketService;

            ConfigureGrid();
        }

        public override void ActivateScreen()
        {
            RefreshCustomerGrid();
        }

        private void ConfigureGrid()
        {
            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Id", Name = "colId", Width = 50 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Contact", Name = "colName", Width = 160 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Company", HeaderText = "Company", Name = "colCompany", Width = 170 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", Name = "colEmail", Width = 200 });
            dgvCustomers.DataSource = customersBindingSource;
        }

        private void RefreshCustomerGrid()
        {
            customersBindingSource.DataSource = customerService.GetCustomers();
            customersBindingSource.ResetBindings(false);
            dgvCustomers.ClearSelection();
            UpdateTicketCount();
        }

        private Customer SelectedCustomer
        {
            get
            {
                if (dgvCustomers.SelectedRows.Count == 0)
                    return null;
                return dgvCustomers.SelectedRows[0].DataBoundItem as Customer;
            }
        }

        private void dgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            UpdateTicketCount();
        }

        private void UpdateTicketCount()
        {
            Customer selected = SelectedCustomer;
            if (selected == null)
            {
                lblCustomerTickets.Text = "Select a customer to see its tickets.";
                return;
            }

            var tickets = ticketService.GetTicketsForCustomer(selected.Company);
            int open = tickets.FindAll(t => t.Status != "Closed").Count;
            lblCustomerTickets.Text = $"{selected.Company}: {tickets.Count} ticket(s), {open} not closed — contact {selected.Name} <{selected.Email}>";
        }

        #region Add customer — success, failure (validation) and recovery

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            var candidate = new Customer
            {
                Name = txtCustomerName.Text.Trim(),
                Company = txtCompany.Text.Trim(),
                Email = txtEmail.Text.Trim(),
            };

            ValidationResult result = customerService.ValidateCustomer(candidate);
            if (!result.IsValid)
            {
                // Failure path: the messages stay next to the form; nothing is stored.
                lblCustomerValidation.Text = result.Message;
                ShowStatus(lblStatus, "Customer not added — fix the fields marked below.", StatusKind.Warn);
                Shell.AddActivity($"btnAddCustomer_Click → CustomerService.ValidateCustomer rejected: {string.Join(" ", result.Errors)}");
                return;
            }

            customerService.AddCustomer(candidate);
            RefreshCustomerGrid();
            ClearForm();
            ShowStatus(lblStatus, $"Customer #{candidate.Id} {candidate.Company} added.", StatusKind.Ok);
            Shell.AddActivity($"btnAddCustomer_Click → CustomerService.AddCustomer(#{candidate.Id} {candidate.Company}) → RefreshCustomerGrid()");
        }

        /// <summary>Recovery: put a valid customer in the form so the next click succeeds.</summary>
        private void btnFillSampleCustomer_Click(object sender, EventArgs e)
        {
            txtCustomerName.Text = "Laura Callahan";
            txtCompany.Text = "Alpine Ski House";
            txtEmail.Text = "laura@alpineskihouse.example";
            lblCustomerValidation.Text = "";
            ShowStatus(lblStatus, "Sample customer filled in — click Add customer.", StatusKind.Ok);
            Shell.AddActivity("btnFillSampleCustomer_Click → form filled with a valid sample");
        }

        private void ClearForm()
        {
            txtCustomerName.Text = "";
            txtCompany.Text = "";
            txtEmail.Text = "";
            lblCustomerValidation.Text = "";
        }

        #endregion
    }
}
