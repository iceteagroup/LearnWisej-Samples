using System;
using System.Collections.Generic;
using System.Linq;
using OrderDesk.Dialogs;
using OrderDesk.Domain;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Pages
{
    /// <summary>
    /// LegacyOrderDesk.OrdersForm ported to a Wisej.NET page (a Panel hosted by the MainPage shell).
    /// Business logic goes through the same OrderService; the grid keeps its columns; the row
    /// double-click keeps opening the modal EditOrderDialog — now inside a using block, and
    /// "Saved." is a Toast instead of a blocking MessageBox.
    /// </summary>
    public partial class OrdersPage : ModulePage
    {
        private readonly OrderService _service;
        private List<Order> _rows = new List<Order>();        // BindingList<Order> → List<Order> (DataSource)
        private bool _loading;

        public OrdersPage(OrderService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            InitializeComponent();
        }

        public override string Title => "Orders";

        /// <summary>Rows currently bound to the grid.</summary>
        public int RowCount => _rows.Count;

        /// <summary>Raised after a Save so the shell can refresh the status bar.</summary>
        public event EventHandler OrdersChanged;

        /// <summary>✕ loads EVERY row — fine on a LAN desktop, the Module 5 problem on the web. Kept for parity.</summary>
        public void ReloadGrid()
        {
            _loading = true;
            try
            {
                _rows = _service.GetAll();                       // ✓ same service call the desktop form made
                ordersGrid.DataSource = _rows;
                if (ordersGrid.Rows.Count > 0)
                    ordersGrid.Rows[0].Selected = true;
            }
            finally
            {
                _loading = false;
            }
            Log(TraceKind.Server, "OrderService.GetAll()", _rows.Count + " orders → ordersGrid.DataSource (BindingList<Order> → List<Order>)");
            ShowDetail();
        }

        /// <summary>The order of the current row (falls back to the first row, 1042, when nothing is selected).</summary>
        public Order SelectedOrder
        {
            get
            {
                var row = ordersGrid.CurrentRow;
                if (row == null && ordersGrid.SelectedRows.Count > 0)
                    row = ordersGrid.SelectedRows[0];
                return row?.DataBoundItem as Order ?? _rows.FirstOrDefault();
            }
        }

        private void ordersGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            var o = SelectedOrder;
            if (o != null)
                Log(TraceKind.ClientToServer, "ordersGrid.SelectionChanged", "Order " + o.Id + " · " + o.CustomerName);
            ShowDetail();
        }

        private void ShowDetail()
        {
            var o = SelectedOrder;
            // AppState.CurrentOrder / AppState.CurrentCustomer (✕ statics) are NOT ported — Module 4 replaces them with UserContext.
            detailTitle.Text = o == null ? "" : "Order " + o.Id;
            detailCustomer.Text = o?.CustomerName ?? "";
            detailPo.Text = o?.PoNumber ?? "";
            detailLines.Text = o == null ? "" : o.Lines.Sum(l => l.Quantity) + " items · " + o.Lines.Count + " line(s)";
        }

        private void ordersGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var o = SelectedOrder;
            if (o == null) return;
            Log(TraceKind.ClientToServer, "ordersGrid.CellDoubleClick", "row " + e.RowIndex + " → Order " + o.Id);
            EditOrder(o, "ordersGrid.CellDoubleClick");
        }

        /// <summary>The shell's "Edit selected ✓" button: same path as the grid double-click.</summary>
        public bool EditSelected(string origin = "Edit selected ✓")
        {
            var o = SelectedOrder;
            if (o == null)
            {
                Log(TraceKind.Failure, "EditSelected", "no order selected");
                return false;
            }
            return EditOrder(o, origin);
        }

        /// <summary>
        /// The ported handler. WinForms: <c>var dlg = new EditOrderDialog(...); if (dlg.ShowDialog(this) == OK) { Save; MessageBox.Show("Saved."); }</c>
        /// — never disposed. Web: the using block disposes the dialog every time, and the confirmation is a Toast.
        /// </summary>
        private bool EditOrder(Order order, string origin)
        {
            Log(TraceKind.Server, "EditOrder(" + order.Id + ")", "from " + origin + " · LiveInstances=" + EditOrderDialog.LiveInstances);
            bool saved = false;
            EditOrderDialog disposedRef;
            using (var dlg = new EditOrderDialog(order, _service, Trace))
            {
                disposedRef = dlg;
                Log(TraceKind.ServerToClient, "dlg.ShowDialog()", "modal · this handler is suspended until the browser closes the dialog");
                var result = dlg.ShowDialog();                    // blocks the handler (Wisej modal workflow)
                Log(TraceKind.ClientToServer, "dlg.ShowDialog() returned", "DialogResult." + result + " · dlg.IsDisposed=" + dlg.IsDisposed + " (inside the using block)");
                if (result == DialogResult.OK)
                {
                    _service.Save(dlg.Order);                     // ✓ business logic, reused as-is
                    Log(TraceKind.Ok, "_service.Save(dlg.Order)", "Order " + dlg.Order.Id + " · " + dlg.Order.Owner + " · " + dlg.Order.Status);
                    Notify.Saved("Order " + dlg.Order.Id + " saved.");   // ✓ Toast — was MessageBox.Show("Saved.")
                    Log(TraceKind.ServerToClient, "Toast", "\"Order " + dlg.Order.Id + " saved.\" · non-blocking · AutoCloseDelay 3500 ms");
                    saved = true;
                }
            }
            Log(TraceKind.Ok, "using block exit", "dlg.IsDisposed=" + disposedRef.IsDisposed + " · LiveInstances=" + EditOrderDialog.LiveInstances);
            if (saved)
            {
                ReloadGrid();
                OrdersChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Log(TraceKind.Server, "EditOrder", "cancelled — nothing saved, dialog disposed anyway");
            }
            return saved;
        }

        private void newOrderButton_Click(object sender, EventArgs e)
        {
            Log(TraceKind.ClientToServer, "newOrderButton.Click", "");
            var order = new Order
            {
                Customer = _service.Customers[0],
                Owner = "Kelly",                                  // was AppState.CurrentUser?.UserName (✕ static) → UserContext in Module 4
                Status = OrderStatus.Open,
                Date = DateTime.Today,
                PoNumber = "NEW-" + DateTime.Now.ToString("HHmmss"),
            };
            order.Lines.Add(new OrderLine { Sku = "LAMP-01", Description = "Desk lamp", Quantity = 1, UnitPrice = 48m });
            Log(TraceKind.Finding, "AppState.CurrentUser not ported", "owner defaults to \"Kelly\" until Module 4 (UserContext)");
            EditOrder(order, "newOrderButton.Click");
        }
    }
}
