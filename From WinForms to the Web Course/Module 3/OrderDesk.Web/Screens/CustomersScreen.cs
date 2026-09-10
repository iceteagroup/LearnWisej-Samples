using System;
using OrderDesk.Domain;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk.Screens
{
    /// <summary>
    /// The customer lookup — "standard controls, simple binding, no local dependency" in the Module 1
    /// workbook (verdict: direct-port). It exists here to be the second screen the shell navigates to.
    /// </summary>
    public partial class CustomersScreen : ScreenBase
    {
        private readonly CustomerService _customerService = new CustomerService();   // ✓ reused unchanged

        public CustomersScreen()
        {
            InitializeComponent();
        }

        public override string ScreenName => "Customers";

        public override string StatusText => $"{gridCustomers.Rows.Count} customers";

        public Customer SelectedCustomer => gridCustomers.CurrentRow?.Tag as Customer;

        public override void OnShown(bool first)
        {
            if (first) Reload();
        }

        public void Reload()
        {
            var customers = _customerService.GetCustomers();
            RaiseTrace(TraceKind.Server, "CustomerService.GetCustomers", $"{customers.Count} customers (business logic reused, unchanged)");

            gridCustomers.Rows.Clear();
            foreach (var customer in customers)
            {
                int index = gridCustomers.Rows.Add(customer.Name, customer.City, customer.Country,
                    customer.DiscountRate > 0 ? customer.DiscountRate.ToString("P0") : "—");
                gridCustomers.Rows[index].Tag = customer;
            }
            if (gridCustomers.Rows.Count > 0)
                gridCustomers.Rows[0].Selected = true;
            RaiseStatusChanged();
        }

        private void gridCustomers_SelectionChanged(object sender, EventArgs e)
        {
            var customer = SelectedCustomer;
            if (customer != null)
                RaiseTrace(TraceKind.FromClient, "gridCustomers.SelectionChanged", $"{customer.Name} ({customer.City}, {customer.Country})");
        }
    }
}
