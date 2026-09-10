using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using OrderDesk.Domain;

namespace LegacyOrderDesk
{
    /// <summary>
    /// The main WinForms screen of LegacyOrderDesk: the orders grid, a detail panel and the three
    /// buttons the assessment flags. Business logic goes through OrderService (reused as-is on the
    /// web); everything marked ✕ is desktop plumbing that becomes an explicit migration task.
    /// </summary>
    public partial class OrdersForm : Form
    {
        private readonly OrderService _service = new OrderService();
        private readonly BindingList<Order> _rows = new BindingList<Order>();

        public OrdersForm()
        {
            InitializeComponent();
        }

        private void OrdersForm_Load(object sender, EventArgs e)
        {
            Text = "LegacyOrderDesk — Orders  ·  " + AppState.CurrentUser?.DisplayName;
            ReloadGrid();
        }

        /// <summary>✕ loads EVERY row — fine on a LAN desktop, the Module 5 problem on the web.</summary>
        private void ReloadGrid()
        {
            _rows.Clear();
            foreach (var o in _service.GetAll())
                _rows.Add(o);
            ordersGrid.DataSource = _rows;
            statusLabel.Text = "Ready · " + _rows.Count + " orders · single-user desktop";
            if (ordersGrid.Rows.Count > 0) ordersGrid.Rows[0].Selected = true;
            ShowDetail();
        }

        private Order SelectedOrder => ordersGrid.CurrentRow?.DataBoundItem as Order;

        private void ordersGrid_SelectionChanged(object sender, EventArgs e) => ShowDetail();

        private void ShowDetail()
        {
            var o = SelectedOrder;
            AppState.CurrentOrder = o;                                  // ✕ static per-user state
            AppState.CurrentCustomer = o?.Customer;                     // ✕ static per-user state
            detailTitle.Text = o == null ? "" : "Order " + o.Id;
            detailCustomer.Text = o?.CustomerName ?? "";
            detailPo.Text = o?.PoNumber ?? "";
            detailLines.Text = o == null ? "" : o.Lines.Sum(l => l.Quantity) + " items";
        }

        private void ordersGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || SelectedOrder == null) return;
            var dlg = new EditOrderDialog(SelectedOrder, _service);   // ✕ never disposed — the process exit hides it
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _service.Save(dlg.Order);                            // ✓ business logic, reused as-is
                MessageBox.Show("Saved.");                           // ✕ blocking confirmation for an informational message
                ReloadGrid();
            }
        }

        private void newOrderButton_Click(object sender, EventArgs e)
        {
            var order = new Order
            {
                Customer = _service.Customers[0],
                Owner = AppState.CurrentUser?.UserName,
                Status = OrderStatus.Open,
                Date = DateTime.Today,
                PoNumber = "NEW-" + DateTime.Now.ToString("HHmmss"),
            };
            order.Lines.Add(new OrderLine { Sku = "LAMP-01", Description = "Desk lamp", Quantity = 1, UnitPrice = 48m });
            var dlg = new EditOrderDialog(order, _service);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _service.Save(dlg.Order);
                MessageBox.Show("Saved.");
                ReloadGrid();
            }
        }

        private void printInvoiceButton_Click(object sender, EventArgs e)
        {
            if (SelectedOrder == null) return;
            try
            {
                new InvoicePrinter(SelectedOrder).Print();           // ✕ local printer
                statusLabel.Text = "Invoice " + SelectedOrder.Id + " sent to the default printer";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not print: " + ex.Message, "Print Invoice", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            try
            {
                var path = ExcelExport.ExportToExcel(_rows);         // ✕ Excel Interop
                MessageBox.Show("Exported to " + path);
            }
            catch (Exception ex)
            {
                // The desktop fallback the team added years ago: write a CSV to C:\Orders instead.
                var csv = LocalExport.WriteCsv(_rows);               // ✕ local file path
                MessageBox.Show("Excel is not available (" + ex.Message + ").\n\nWrote " + csv + " instead.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e) => Close();

        private void aboutMenuItem_Click(object sender, EventArgs e)
            => MessageBox.Show("LegacyOrderDesk 3.2 — the desktop order desk the course migrates to Wisej.NET.", "About");
    }
}
