using System;
using System.Collections.Generic;
using OrderDesk.Domain;
using OrderDesk.Screens;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk.Shell
{
    /// <summary>
    /// The application shell, ported from LegacyOrderDesk/OrdersForm: the MenuStrip became a
    /// <see cref="MenuBar"/>, the action buttons a <see cref="ToolBar"/>, the StatusStrip a
    /// <see cref="StatusBar"/>, and the single form a host that swaps <b>screens</b>
    /// (UserControls, one instance each, Dock = Fill) with <see cref="NavigateTo"/>.
    /// </summary>
    public partial class AppShell : UserControl
    {
        public const string OrdersScreenName = "Orders";
        public const string CustomersScreenName = "Customers";
        public const string ReportsScreenName = "Reports";

        private readonly Dictionary<string, ScreenBase> _screens = new Dictionary<string, ScreenBase>(StringComparer.OrdinalIgnoreCase);
        private ScreenBase _current;

        public AppShell()
        {
            InitializeComponent();
        }

        /// <summary>The name of the screen currently shown ("Orders", "Customers", "Reports").</summary>
        public string CurrentScreen => _current?.ScreenName;

        public OrdersScreen Orders => (OrdersScreen)GetScreen(OrdersScreenName);
        public CustomersScreen Customers => (CustomersScreen)GetScreen(CustomersScreenName);
        public ReportsScreen Reports => (ReportsScreen)GetScreen(ReportsScreenName);

        #region Navigation

        /// <summary>
        /// Shows one screen in the content host. One instance per screen for the life of the shell
        /// (i.e. of the session): created on first use, hidden when another screen takes over, shown
        /// again on the way back.
        /// </summary>
        public void NavigateTo(string screen)
        {
            bool created = !_screens.ContainsKey(screen);
            var target = GetScreen(screen);
            if (ReferenceEquals(target, _current))
                return;

            screenHost.SuspendLayout();
            if (_current != null)
                _current.Visible = false;
            target.Visible = true;
            target.BringToFront();
            screenHost.ResumeLayout();

            _current = target;
            target.OnShown(created);
            UpdateStatus();
        }

        private ScreenBase GetScreen(string name)
        {
            if (_screens.TryGetValue(name, out var existing))
                return existing;

            ScreenBase screen;
            switch (name)
            {
                case OrdersScreenName: screen = new OrdersScreen(); break;
                case CustomersScreenName: screen = new CustomersScreen(); break;
                case ReportsScreenName: screen = new ReportsScreen(); break;
                default: throw new ArgumentException($"Unknown screen '{name}'.", nameof(name));
            }

            screen.Dock = DockStyle.Fill;
            screen.Visible = false;
            screen.StatusChanged += (s, e) => UpdateStatus();
            if (screen is OrdersScreen orders)
                orders.PrintInvoiceRequested += (s, order) => PrintInvoice(order);

            screenHost.Controls.Add(screen);
            _screens[name] = screen;
            return screen;
        }

        private void UpdateStatus()
        {
            if (_current == null) return;
            statusPanel.Text = $"{_current.ScreenName} · {_current.StatusText}";
        }

        #endregion

        #region Menu bar (View · Reports · Help)

        private void menuViewOrders_Click(object sender, EventArgs e) => NavigateTo(OrdersScreenName);
        private void menuViewCustomers_Click(object sender, EventArgs e) => NavigateTo(CustomersScreenName);
        private void menuViewReports_Click(object sender, EventArgs e) => NavigateTo(ReportsScreenName);

        private void menuViewOpenOnly_Click(object sender, EventArgs e)
        {
            NavigateTo(OrdersScreenName);
            Orders.ApplyFilter(OrderStatus.Open);
        }

        private void menuViewAllOrders_Click(object sender, EventArgs e)
        {
            NavigateTo(OrdersScreenName);
            Orders.ApplyFilter(null);
        }

        private void menuReportsPrintInvoice_Click(object sender, EventArgs e) => PrintInvoice(Orders.SelectedOrder);
        private void menuReportsExport_Click(object sender, EventArgs e) => Reports.Export();

        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            // Was a modal MessageBox; nothing to decide, so a Toast is enough.
            Ui.Toast("OrderDesk.Web — LegacyOrderDesk 3.2 migrated to Wisej.NET.");
        }

        #endregion

        #region Tool bar (Orders · Customers · Reports | New Order · Print Invoice · Export)

        private void toolOrders_Click(object sender, EventArgs e) => NavigateTo(OrdersScreenName);
        private void toolCustomers_Click(object sender, EventArgs e) => NavigateTo(CustomersScreenName);
        private void toolReports_Click(object sender, EventArgs e) => NavigateTo(ReportsScreenName);

        private void toolNewOrder_Click(object sender, EventArgs e)
        {
            NavigateTo(OrdersScreenName);
            Orders.NewOrder();
        }

        private void toolPrintInvoice_Click(object sender, EventArgs e) => PrintInvoice(Orders.SelectedOrder);
        private void toolExport_Click(object sender, EventArgs e) => Reports.Export();

        /// <summary>Print Invoice from anywhere: go to Reports, select the order there, preview the PDF.</summary>
        private void PrintInvoice(Order order)
        {
            if (order == null) return;
            NavigateTo(ReportsScreenName);
            Reports.SelectOrder(order.Id);
            Reports.PrintSelected();
        }

        #endregion
    }
}
