using System;
using System.Collections.Generic;
using System.Linq;
using OrderDesk.Domain;
using OrderDesk.Services;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// The Orders screen with its per-user state (user, company, current customer, filter) in
    /// <see cref="SessionContext.Current"/> instead of static fields. Open a second session and sign
    /// in as the other user: each session keeps its own context.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly OrderService _orderService = new OrderService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly IList<Customer> _customers;
        private readonly OrderStatus[] _statuses = (OrderStatus[])Enum.GetValues(typeof(OrderStatus));
        private bool _binding;   // true while code fills the combos — their events must not write back

        public MainPage()
        {
            InitializeComponent();
            _customers = _customerService.GetCustomers();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            FillCombos();
            RefreshAll();
        }

        #region Orders, filtered by the session context

        private void FillCombos()
        {
            _binding = true;
            comboFilter.Items.Clear();
            comboFilter.Items.Add("All");
            foreach (var status in _statuses) comboFilter.Items.Add(status.ToString());
            comboCustomer.Items.Clear();
            comboCustomer.Items.Add("All customers");
            foreach (var customer in _customers) comboCustomer.Items.Add(customer.Name);
            comboFilter.SelectedIndex = 0;
            comboCustomer.SelectedIndex = 0;
            _binding = false;
        }

        private OrderStatus? SelectedFilter => comboFilter.SelectedIndex <= 0 ? (OrderStatus?)null : _statuses[comboFilter.SelectedIndex - 1];
        private int? SelectedCustomerId => comboCustomer.SelectedIndex <= 0 ? (int?)null : _customers[comboCustomer.SelectedIndex - 1].Id;

        /// <summary>Shows this session's context in the combos without writing anything back.</summary>
        private void SyncCombosFromContext()
        {
            var context = SessionContext.Current;
            _binding = true;
            comboFilter.SelectedIndex = context.CurrentFilter.HasValue ? Array.IndexOf(_statuses, context.CurrentFilter.Value) + 1 : 0;
            int customerIndex = context.CurrentCustomerId.HasValue ? _customers.ToList().FindIndex(c => c.Id == context.CurrentCustomerId.Value) : -1;
            comboCustomer.SelectedIndex = customerIndex + 1;
            _binding = false;
        }

        private void comboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_binding) return;
            SessionContext.Current.CurrentFilter = SelectedFilter;
            BindOrders();
            RefreshSession();
        }

        private void comboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_binding) return;
            SessionContext.Current.CurrentCustomerId = SelectedCustomerId;
            BindOrders();
            RefreshSession();
        }

        private void BindOrders()
        {
            var context = SessionContext.Current;
            var orders = _orderService.Search(new OrderFilter { Status = context.CurrentFilter, Text = context.LastSearch });

            gridOrders.Rows.Clear();
            foreach (var order in orders)
            {
                int index = gridOrders.Rows.Add(order.Id, order.CustomerName, order.Total, order.Status.ToString());
                gridOrders.Rows[index].Tag = order;
                gridOrders.Rows[index].Cells[3].Style.ForeColor = StatusColor(order.Status);
                if (context.CurrentCustomerId.HasValue && order.CustomerId == context.CurrentCustomerId.Value)
                {
                    // the current customer is highlighted, the way LegacyOrderDesk preselects it in dialogs
                    gridOrders.Rows[index].Cells[1].Style.ForeColor = Ui.Accent;
                    gridOrders.Rows[index].Cells[1].Style.Font = Ui.SmallBold;
                }
            }
            labelOrdersCount.Text = $"{orders.Count} of {_orderService.GetOrders().Count} orders";
        }

        private static System.Drawing.Color StatusColor(OrderStatus status) => status switch
        {
            OrderStatus.Shipped => Ui.Ok,
            OrderStatus.Invoiced => Ui.Purple,
            OrderStatus.Hold => Ui.Warn,
            _ => Ui.Accent
        };

        #endregion

        #region Session: sign in, sign out, second session

        private void buttonSignInKelly_Click(object sender, EventArgs e) => SignIn("kelly", "Acme", 1, OrderStatus.Open);          // Northwind Traders
        private void buttonSignInSam_Click(object sender, EventArgs e) => SignIn("sam", "Globex", 3, OrderStatus.Invoiced);        // Fabrikam Inc

        private void SignIn(string user, string company, int customerId, OrderStatus filter)
        {
            // Was: AppState.CurrentUser = user; AppState.CurrentCompany = company; … (one slot for the whole server)
            var context = SessionContext.Current;
            context.UserName = user;
            context.DisplayName = char.ToUpperInvariant(user[0]) + user.Substring(1);
            context.Company = company;
            context.CurrentCustomerId = customerId;
            context.CurrentFilter = filter;
            context.LastSearch = null;
            context.Culture = Application.CurrentCulture?.Name;
            context.SignedInAt = DateTime.Now;

            try
            {
                SessionCleanup.CreateWorkspace(Application.SessionId, user);
            }
            catch (Exception ex)
            {
                Ui.Toast($"The session workspace could not be created: {ex.Message}", MessageBoxIcon.Warning);
            }

            RefreshAll();
        }

        private void buttonSignOut_Click(object sender, EventArgs e)
        {
            SessionCleanup.Run("logout", Application.SessionId);
            RefreshAll();
            Ui.Toast("Signed out.");
        }

        private void buttonReread_Click(object sender, EventArgs e) => RefreshAll();

        private void buttonSecondSession_Click(object sender, EventArgs e)
        {
            Application.Navigate(Application.Url, "_blank");
        }

        private void RefreshAll()
        {
            SyncCombosFromContext();
            BindOrders();
            RefreshSession();
        }

        private void RefreshSession()
        {
            var context = SessionContext.Current;
            labelSession.Text =
                $"Session            {SessionCleanup.ShortId(Application.SessionId)}   ({Application.SessionCount} open)\n" +
                $"User               {(context.IsSignedIn ? context.UserName : "(not signed in)")}\n" +
                $"Current customer   {CustomerName(context.CurrentCustomerId)}\n" +
                $"Active filter      {(context.CurrentFilter.HasValue ? context.CurrentFilter.Value.ToString() : "All")}\n" +
                $"Company            {context.Company ?? "—"}";
        }

        private string CustomerName(int? customerId) =>
            customerId.HasValue ? _customerService.Find(customerId.Value)?.Name ?? "?" : "all customers";

        #endregion
    }
}
