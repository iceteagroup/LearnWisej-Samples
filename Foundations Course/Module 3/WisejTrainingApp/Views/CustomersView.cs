using System;
using System.Collections.Generic;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Customers page: the customer list with Add Customer and View Selected (lesson s12).
    /// The role matrix says a Support Agent may only view customers, a Manager may add them —
    /// so this is the second place the permission idea shows up, enforced the same way as Settings:
    /// the button is disabled for the role AND the handler asks PermissionService again.
    /// </summary>
    public partial class CustomersView : UserControl
    {
        private static readonly string[] NewCustomerPool =
            { "Litware", "Proseware", "Woodgrove Bank", "Alpine Ski House", "Coho Winery" };

        private readonly IShellHost shell;
        private readonly CustomerService customerService;
        private readonly TicketService ticketService;
        private readonly PermissionService permissions;

        private List<Customer> customers = new List<Customer>();

        public CustomersView(IShellHost shell, CustomerService customerService, TicketService ticketService, PermissionService permissions)
        {
            this.shell = shell;
            this.customerService = customerService;
            this.ticketService = ticketService;
            this.permissions = permissions;

            InitializeComponent();
        }

        private void CustomersView_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            ApplyPermissions();
        }

        /// <summary>What this role may do here; the same check runs again inside btnAddCustomer_Click.</summary>
        private void ApplyPermissions()
        {
            if (permissions.CanEditCustomers(shell.CurrentRole))
            {
                btnAddCustomer.Enabled = true;
                lblPermissionNote.Text = shell.CurrentRole + " can add and edit customers.";
                lblPermissionNote.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            }
            else
            {
                btnAddCustomer.Enabled = false;
                lblPermissionNote.Text = "Customers are view-only for this role (" + shell.CurrentRole + ").";
                lblPermissionNote.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            }
        }

        private void LoadCustomers(int selectId = 0)
        {
            customers = customerService.GetCustomers();

            lstCustomers.Items.Clear();
            foreach (Customer c in customers)
            {
                int open = ticketService.CountOpenFor(c.Name);
                lstCustomers.Items.Add($"{c.Id,2}  {c.Name,-22} {c.Contact,-16} {c.City,-10} {open} open ticket{(open == 1 ? "" : "s")}");
                if (c.Id == selectId)
                    lstCustomers.SelectedIndex = lstCustomers.Items.Count - 1;
            }

            lblCustomersSummary.Text = customers.Count + " customers";
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            // The server-side check — a disabled button is UI courtesy, this is the rule.
            if (!permissions.CanEditCustomers(shell.CurrentRole))
            {
                shell.Log($"Permission denied: {shell.CurrentRole} cannot add customers.", LogKind.Error);
                return;
            }

            int n = customerService.Count();
            string name = NewCustomerPool[n % NewCustomerPool.Length];
            if (customers.Exists(c => c.Name == name))
                name += " " + (n + 1);

            var customer = new Customer
            {
                Name = name,
                Contact = "New contact",
                City = "Unknown",
            };

            customerService.AddCustomer(customer);
            LoadCustomers(customer.Id);
            shell.Log($"Customer #{customer.Id} {customer.Name} added by {shell.CurrentRole}.");
        }

        private void btnViewSelected_Click(object sender, EventArgs e)
        {
            int index = lstCustomers.SelectedIndex;
            if (index < 0 || index >= customers.Count)
            {
                // Failure path: nothing selected.
                shell.Log("View Selected: no customer selected.", LogKind.Warn);
                AlertBox.Show("Select a customer first.", MessageBoxIcon.Warning,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            Customer c = customers[index];
            int open = ticketService.CountOpenFor(c.Name);

            AlertBox.Show($"{c.Name}\nContact: {c.Contact}\nCity: {c.City}\nOpen tickets: {open}",
                MessageBoxIcon.Information, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 5000);
            shell.Log($"Viewed customer {c.Name} ({open} open ticket{(open == 1 ? "" : "s")}).");
        }
    }
}
