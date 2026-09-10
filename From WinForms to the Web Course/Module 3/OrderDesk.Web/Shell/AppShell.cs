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
    /// <see cref="MenuBar"/>, the action buttons became a <see cref="ToolBar"/>, the StatusStrip a
    /// <see cref="StatusBar"/>, and the single form became a host that swaps <b>screens</b>
    /// (UserControls, one instance each, Dock = Fill) with <see cref="NavigateTo"/>.
    ///
    /// ✕ Desktop: every screen was a Form; "navigation" meant opening another window (or a modal
    ///   SettingsForm) and the operating system kept them apart.
    /// ✓ Web:     one Page per browser tab; the shell owns the chrome and one content host. Screens are
    ///   created on first use and kept, so navigating back does not rebuild the grid.
    ///
    /// The shell knows nothing about the lab console: it raises <see cref="Trace"/> and the console
    /// forwards it to the TracePanel. Remove the subscription and the shell is the product.
    /// </summary>
    public partial class AppShell : UserControl
    {
        public const string OrdersScreenName = "Orders";
        public const string CustomersScreenName = "Customers";
        public const string ReportsScreenName = "Reports";

        private readonly Dictionary<string, ScreenBase> _screens = new Dictionary<string, ScreenBase>(StringComparer.OrdinalIgnoreCase);
        private ScreenBase _current;

        /// <summary>Everything the shell and its screens do, for the migration log.</summary>
        public event EventHandler<TraceEventArgs> Trace;

        /// <summary>Raised after <see cref="NavigateTo"/> switched screens.</summary>
        public event EventHandler Navigated;

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
        /// (i.e. of the session): created on first use, hidden — never disposed — when another screen
        /// takes over, shown again on the way back.
        /// </summary>
        public void NavigateTo(string screen)
        {
            bool created = !_screens.ContainsKey(screen);
            var target = GetScreen(screen);

            if (ReferenceEquals(target, _current))
            {
                RaiseTrace(TraceKind.FromClient, "navigate", $"{target.ScreenName} (already the current screen — nothing rebuilt)");
                return;
            }

            screenHost.SuspendLayout();
            if (_current != null)
                _current.Visible = false;
            target.Visible = true;
            target.BringToFront();
            screenHost.ResumeLayout();

            _current = target;
            RaiseTrace(TraceKind.FromClient, "navigate", $"{target.ScreenName} → {(created ? "created" : "reused")} {target.GetType().Name} in screenHost (Dock = Fill)");

            target.OnShown(created);
            UpdateStatus();
            Navigated?.Invoke(this, EventArgs.Empty);
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
            screen.Trace += (s, e) => Trace?.Invoke(this, e);
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

        #region Menu bar (File · View · Reports · Help)

        private void menuFileSettings_Click(object sender, EventArgs e)
        {
            // ✕ desktop: using (var dialog = new SettingsForm()) dialog.ShowDialog(this);  — HKCU on the user's PC.
            // ✓ web:     HKCU on the server is the service account's registry, shared by everyone. Settings become a
            //            per-user store behind Application.Session (Module 4); nothing is faked here.
            RaiseTrace(TraceKind.Boundary, "File › Settings…", "modal SettingsForm + HKCU registry ⇒ per-user profile store (Module 4) — not ported in this module");
            Ui.Toast("Settings move to a per-user profile store in Module 4 — the registry is the server's, not yours.", MessageBoxIcon.Warning);
        }

        private void menuFileExit_Click(object sender, EventArgs e)
        {
            // ✕ desktop: Close() ended the process (and disposed every forgotten dialog with it).
            // ✓ web:     the browser tab belongs to the user; there is no process to end. Exit becomes sign-out (Module 4).
            RaiseTrace(TraceKind.Boundary, "File › Exit", "Close() ended the process on the desktop ⇒ nothing to end on the server; becomes sign-out (Module 4)");
            Ui.Toast("Exit: there is no process to end in the browser — sign-out arrives with sessions in Module 4.", MessageBoxIcon.Information);
        }

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
            // ✕ was: MessageBox.Show("LegacyOrderDesk 3.2 — …", "About") — informational, nothing to decide → ✓ Toast (docs/NotificationsReview.md).
            RaiseTrace(TraceKind.ToClient, "Help › About", "MessageBox.Show(\"About\") ⇒ Ui.Toast (informational, no decision)");
            Ui.Toast("OrderDesk.Web — LegacyOrderDesk 3.2 migrated in \"From WinForms to the Web\" (Module 3).");
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
            if (order == null)
            {
                RaiseTrace(TraceKind.Server, "Print Invoice", "no order selected");
                return;
            }
            NavigateTo(ReportsScreenName);
            Reports.SelectOrder(order.Id);
            Reports.PrintSelected();
        }

        #endregion

        private void RaiseTrace(TraceKind kind, string name, string payload = "") =>
            Trace?.Invoke(this, new TraceEventArgs(kind, name, payload));
    }
}
