using OrderDesk.Domain;

namespace OrderDesk.Screens
{
    /// <summary>The customer lookup: standard controls on the reused CustomerService.</summary>
    public partial class CustomersScreen : ScreenBase
    {
        private readonly CustomerService _customerService = new CustomerService();

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
    }
}
