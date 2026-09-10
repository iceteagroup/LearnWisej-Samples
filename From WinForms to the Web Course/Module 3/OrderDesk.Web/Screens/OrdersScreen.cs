using System;
using System.Linq;
using OrderDesk.Dialogs;
using OrderDesk.Domain;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk.Screens
{
    /// <summary>
    /// The Orders list, ported from LegacyOrderDesk/OrdersForm.cs (grid + detail + actions). The
    /// business logic is the same <see cref="OrderService"/>; what changed is the desktop plumbing:
    ///
    ///  ✕ dialog.ShowDialog(this) == DialogResult.OK          → ✓ await dialog.ShowDialogAsync() (the call returns at once)
    ///  ✕ new EditOrderDialog(...) never disposed              → ✓ using block: Dispose runs after the await, every time
    ///  ✕ MessageBox.Show("Saved.")                            → ✓ Ui.Toast("Order 1042 saved.") — nothing to decide
    ///  ✕ static AppState.CurrentFilter shared by every user   → ✓ a field on this screen (one screen per session)
    ///  ✕ fixed 716×372 layout                                 → ✓ Dock/Anchor (see OrdersScreen.Designer.cs)
    /// </summary>
    public partial class OrdersScreen : ScreenBase
    {
        private readonly OrderService _orderService = new OrderService();          // ✓ reused unchanged
        private readonly CustomerService _customerService = new CustomerService();  // ✓ reused unchanged
        private OrderStatus? _filter;                                               // ✓ was: static AppState.CurrentFilter

        /// <summary>The Print Invoice button: the shell routes it to the Reports screen.</summary>
        public event EventHandler<Order> PrintInvoiceRequested;

        /// <summary>Raised after a save or a delete changed the list.</summary>
        public event EventHandler OrdersChanged;

        public OrdersScreen()
        {
            InitializeComponent();
        }

        public override string ScreenName => "Orders";

        public override string StatusText =>
            $"{gridOrders.Rows.Count} orders · filter {(_filter.HasValue ? _filter.Value.ToString() : "all")} · session {Short(Application.SessionId)}";

        public Order SelectedOrder => gridOrders.CurrentRow?.Tag as Order;

        public override void OnShown(bool first)
        {
            if (first) Reload();
        }

        #region List + detail (the same OrderService calls the desktop makes)

        public void Reload(int? selectId = null)
        {
            int? keep = selectId ?? SelectedOrder?.Id;
            var orders = _filter.HasValue
                ? _orderService.Search(new OrderFilter { Status = _filter })
                : _orderService.GetOrders();
            RaiseTrace(TraceKind.Server, _filter.HasValue ? "OrderService.Search" : "OrderService.GetOrders",
                $"{orders.Count} orders{(_filter.HasValue ? $" · status = {_filter}" : "")} (business logic reused, unchanged)");

            gridOrders.Rows.Clear();
            foreach (var order in orders)
            {
                int index = gridOrders.Rows.Add(order.Id, order.CustomerName, order.Total, order.Status.ToString());
                gridOrders.Rows[index].Tag = order;
                gridOrders.Rows[index].Cells[3].Style.ForeColor = StatusColor(order.Status);
            }

            int select = 0;
            if (keep.HasValue)
                for (int i = 0; i < gridOrders.Rows.Count; i++)
                    if ((gridOrders.Rows[i].Tag as Order)?.Id == keep.Value) select = i;
            if (gridOrders.Rows.Count > 0)
                gridOrders.Rows[select].Selected = true;
            ShowDetail(gridOrders.Rows.Count > 0 ? gridOrders.Rows[select].Tag as Order : null);

            labelHeading.Text = _filter.HasValue ? $"Orders · {_filter.Value} only" : "Orders · all";
            RaiseStatusChanged();
        }

        /// <summary>View › Open orders only / All orders. Per screen instance — never a static.</summary>
        public void ApplyFilter(OrderStatus? status)
        {
            _filter = status;
            RaiseTrace(TraceKind.FromClient, "View › filter", $"status = {(status.HasValue ? status.ToString() : "all")} (kept on this screen, not in a static)");
            Reload();
        }

        private static System.Drawing.Color StatusColor(OrderStatus status) => status switch
        {
            OrderStatus.Shipped => Ui.Ok,
            OrderStatus.Invoiced => Ui.Purple,
            OrderStatus.Hold => Ui.Warn,
            OrderStatus.InProgress => Ui.Muted,
            _ => Ui.Accent
        };

        private void gridOrders_SelectionChanged(object sender, EventArgs e) => ShowDetail(SelectedOrder);

        private void ShowDetail(Order order)
        {
            if (order == null)
            {
                labelDetailTitle.Text = "Order";
                labelDetail.Text = "";
                return;
            }
            labelDetailTitle.Text = $"Order {order.Id}";
            labelDetail.Text =
                $"Customer  {order.CustomerName}\n" +
                $"PO #      {order.PoNumber}\n" +
                $"Owner     {(string.IsNullOrEmpty(order.Owner) ? "—" : order.Owner)}\n" +
                $"Lines     {order.Lines.Count} items · {order.Lines.Sum(l => l.Quantity)} units\n" +
                $"Total     {N2(order.Total)}";
        }

        #endregion

        #region The modal edit workflow — the Module 3 rule

        private void gridOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || SelectedOrder == null) return;
            RaiseTrace(TraceKind.FromClient, "gridOrders.CellDoubleClick", $"row {e.RowIndex} · order {SelectedOrder.Id} → EditOrder");
            EditOrder(SelectedOrder);
        }

        private void buttonScreenEdit_Click(object sender, EventArgs e)
        {
            if (SelectedOrder == null) return;
            RaiseTrace(TraceKind.FromClient, "Edit…", $"order {SelectedOrder.Id} → EditOrder");
            EditOrder(SelectedOrder);
        }

        private void buttonScreenNewOrder_Click(object sender, EventArgs e) => NewOrder();

        /// <summary>New Order: the same dialog for an order with Id 0 — Save assigns the next id.</summary>
        public void NewOrder()
        {
            var customer = _customerService.Find(1);   // Northwind Traders
            var order = new Order
            {
                Customer = customer, CustomerId = customer.Id, Owner = "",
                PoNumber = "NEW", Status = OrderStatus.Open, CreatedOn = DateTime.Today
            };
            order.Lines.Add(new OrderLine { Sku = "WJ-DEV-SEAT", Description = "Developer seat", Quantity = 1, UnitPrice = 190m });
            RaiseTrace(TraceKind.FromClient, "New Order", "new Order (Northwind Traders · 1 × Developer seat) → EditOrder");
            EditOrder(order);
        }

        /// <summary>
        /// The right way to run a transient modal dialog on the server:
        ///
        ///   ✕ desktop (OrdersForm.cs):
        ///       var dialog = new EditOrderDialog(order, customers);
        ///       if (dialog.ShowDialog(this) == DialogResult.OK) { _orderService.Save(dialog.Order); MessageBox.Show("Saved."); }
        ///       // the dialog is never disposed; the process end cleaned up eventually
        ///
        ///   ✓ web (this method): ShowDialog never blocks, so the result is awaited; the using block disposes
        ///     the dialog when the await completes, i.e. when the user closed it — deterministically, every time.
        ///     DialogTracker (Dialogs/DialogTracker.cs) turns that into a number the console shows.
        /// </summary>
        public async void EditOrder(Order order)
        {
            if (order == null) return;
            string session = Application.SessionId;
            string label = order.Id == 0 ? "new order" : $"order {order.Id}";

            using (var dialog = new EditOrderDialog(order, _customerService.GetCustomers()))
            {
                RaiseTrace(TraceKind.Server, "new EditOrderDialog", $"{label} · live dialogs: session {DialogTracker.LiveFor(session)} · process {DialogTracker.LiveTotal}");
                RaiseTrace(TraceKind.ToClient, "ShowDialogAsync", "modal in the browser; the handler yields — this session can still answer other requests");

                DialogResult result = await dialog.ShowDialogAsync();

                RaiseTrace(TraceKind.FromClient, "EditOrderDialog closed", $"DialogResult = {result}");
                if (result == DialogResult.OK)
                {
                    var saved = _orderService.Save(dialog.Order);   // ✓ business logic reused, unchanged
                    RaiseTrace(TraceKind.Server, "OrderService.Save", $"order {saved.Id} · status {saved.Status} · CalculateOrderTotal = {N2(saved.Total)}");
                    Reload(saved.Id);
                    OrdersChanged?.Invoke(this, EventArgs.Empty);

                    // ✕ was: MessageBox.Show("Saved.", "LegacyOrderDesk") — one extra click, the whole page masked, nothing to decide.
                    Ui.Toast($"Order {saved.Id} saved.");
                    RaiseTrace(TraceKind.ToClient, "Ui.Toast", $"\"Order {saved.Id} saved.\"  (was MessageBox.Show(\"Saved.\") — informational → non-blocking)");
                }
            }   // ✓ Dispose runs here, after the await — the async spelling of the lesson's using block

            RaiseTrace(TraceKind.Server, "EditOrderDialog.Dispose", $"disposed by the caller · live dialogs: session {DialogTracker.LiveFor(session)} · process {DialogTracker.LiveTotal}");
        }

        #endregion

        #region Print Invoice / Attach file

        private void buttonScreenPrint_Click(object sender, EventArgs e)
        {
            if (SelectedOrder == null) return;
            RaiseTrace(TraceKind.FromClient, "Print Invoice", $"order {SelectedOrder.Id} → Reports screen");
            PrintInvoiceRequested?.Invoke(this, SelectedOrder);
        }

        private void buttonScreenAttach_Click(object sender, EventArgs e)
        {
            // ✕ desktop: OpenFileDialog + File.Copy to C:\Orders\Attachments — the user's disk. Logged, not faked (Module 6: Upload + storage root).
            RaiseTrace(TraceKind.Boundary, "Attach file", "OpenFileDialog + C:\\Orders\\Attachments cannot exist on the server ⇒ Upload control + storage root (Module 6)");
            Ui.Toast("Attach file: the user's disk is not on the server — Module 6 replaces it with an Upload control.", MessageBoxIcon.Warning);
        }

        #endregion

        private static string Short(string id) => string.IsNullOrEmpty(id) ? "?" : (id.Length > 8 ? id.Substring(0, 8) : id);

        // Text shown to the user is formatted invariantly: the server thread's culture is not the user's.
        private static string N2(decimal value) => value.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
    }
}
