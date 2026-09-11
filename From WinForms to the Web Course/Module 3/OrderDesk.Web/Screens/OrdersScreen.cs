using System;
using System.Globalization;
using System.Linq;
using OrderDesk.Dialogs;
using OrderDesk.Domain;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk.Screens
{
    /// <summary>
    /// The Orders list, ported from LegacyOrderDesk/OrdersForm.cs (grid + detail + actions), on the
    /// same <see cref="OrderService"/>. The edit dialog is awaited and disposed by this screen, and
    /// the "Saved." MessageBox is a Toast.
    /// </summary>
    public partial class OrdersScreen : ScreenBase
    {
        private readonly OrderService _orderService = new OrderService();
        private readonly CustomerService _customerService = new CustomerService();
        private OrderStatus? _filter;   // was the static AppState.CurrentFilter

        /// <summary>The Print Invoice button: the shell routes it to the Reports screen.</summary>
        public event EventHandler<Order> PrintInvoiceRequested;

        public OrdersScreen()
        {
            InitializeComponent();
        }

        public override string ScreenName => "Orders";

        public override string StatusText =>
            $"{gridOrders.Rows.Count} orders · filter {(_filter.HasValue ? _filter.Value.ToString() : "all")}";

        public Order SelectedOrder => gridOrders.CurrentRow?.Tag as Order;

        public override void OnShown(bool first)
        {
            if (first) Reload();
        }

        #region List + detail

        public void Reload(int? selectId = null)
        {
            int? keep = selectId ?? SelectedOrder?.Id;
            var orders = _filter.HasValue
                ? _orderService.Search(new OrderFilter { Status = _filter })
                : _orderService.GetOrders();

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

        /// <summary>View › Open orders only / All orders.</summary>
        public void ApplyFilter(OrderStatus? status)
        {
            _filter = status;
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

        #region Edit dialog

        private void gridOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || SelectedOrder == null) return;
            EditOrder(SelectedOrder);
        }

        private void buttonScreenEdit_Click(object sender, EventArgs e) => EditOrder(SelectedOrder);

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
            EditOrder(order);
        }

        /// <summary>
        /// ShowDialog does not block in Wisej.NET, so the result is awaited; the using block disposes
        /// the dialog when the await completes, i.e. when the user closed it.
        /// </summary>
        public async void EditOrder(Order order)
        {
            if (order == null) return;

            using (var dialog = new EditOrderDialog(order, _customerService.GetCustomers()))
            {
                if (await dialog.ShowDialogAsync() == DialogResult.OK)
                {
                    var saved = _orderService.Save(dialog.Order);
                    Reload(saved.Id);
                    Ui.Toast($"Order {saved.Id} saved.");
                    // UI changed after an await: push it, in case the continuation runs after the close request returned.
                    Application.Update(this);
                }
            }
        }

        #endregion

        private void buttonScreenPrint_Click(object sender, EventArgs e)
        {
            if (SelectedOrder == null) return;
            PrintInvoiceRequested?.Invoke(this, SelectedOrder);
        }

        // Text shown to the user is formatted invariantly: the server thread's culture is not the user's.
        private static string N2(decimal value) => value.ToString("N2", CultureInfo.InvariantCulture);
    }
}
