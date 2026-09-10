using System;
using System.Collections.Generic;
using System.Linq;
using OrderDesk.Domain;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Pages
{
    /// <summary>
    /// The second ported screen: the customer list. The desktop app opened it from the menu as a
    /// Form; here it is a page in the shell. The grid binds OrderService.Customers with explicit
    /// columns only — Customer.TaxId is sensitive and must never travel to the browser as a cell.
    /// </summary>
    public partial class CustomersPage : ModulePage
    {
        private readonly OrderService _service;
        private List<Customer> _rows = new List<Customer>();
        private bool _loaded;

        public CustomersPage(OrderService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            InitializeComponent();
        }

        public override string Title => "Customers";

        public int RowCount => _rows.Count;

        /// <summary>Binds the customers once (the page is reused, not recreated, on navigation).</summary>
        public void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;
            _rows = _service.Customers.ToList();
            customersGrid.DataSource = _rows;
            footerLabel.Text = _rows.Count + " customers · TaxId is sensitive and is not bound to the grid (AutoGenerateColumns = false).";
            Log(TraceKind.Server, "OrderService.Customers", _rows.Count + " customers → customersGrid.DataSource");
            Log(TraceKind.Finding, "TaxId never leaves the server", "explicit columns Name/Country/Tier/CreditLimit · AutoGenerateColumns=false");
        }

        private void customersGrid_SelectionChanged(object sender, EventArgs e)
        {
            var c = customersGrid.CurrentRow?.DataBoundItem as Customer;
            if (c != null && _loaded)
                Log(TraceKind.ClientToServer, "customersGrid.SelectionChanged", c.Name + " · " + c.Country + " · " + c.Tier);
        }
    }
}
