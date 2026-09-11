using System;
using Wisej.Web;
using OrderDesk.Dialogs;
using OrderDesk.Domain;
using OrderDesk.Legacy;
using OrderDesk.Views;

namespace OrderDesk
{
    /// <summary>
    /// LegacyOrderDesk's OrdersForm, ported to Wisej.NET: System.Windows.Forms became Wisej.Web and the
    /// class is still a Form; the handlers and the business logic are the same. The desktop-only
    /// operations that are not ported yet (printing, Excel export, attachments, registry settings)
    /// tell the user so instead of failing.
    /// </summary>
    public partial class OrdersForm : Form
    {
        private readonly OrderService _orderService = new OrderService();
        private readonly CustomerService _customerService = new CustomerService();

        public OrdersForm()
        {
            InitializeComponent();
        }

        private void OrdersForm_Load(object sender, EventArgs e)
        {
            ReloadGrid();
            statusLabel.Text = $"Ready · {ordersGrid.Rows.Count} orders";
        }

        private void ReloadGrid()
        {
            var filter = new OrderFilter { Status = AppState.CurrentFilter, Text = AppState.LastSearch };
            ordersGrid.AutoGenerateColumns = false;
            ordersGrid.DataSource = _orderService.Search(filter);
            if (ordersGrid.Rows.Count > 0)
                ordersGrid.Rows[0].Selected = true;
            ShowDetail();
        }

        private Order CurrentOrder
        {
            get
            {
                if (ordersGrid.CurrentRow?.DataBoundItem is Order order)
                    return order;
                return null;
            }
        }

        private void ordersGrid_SelectionChanged(object sender, EventArgs e) => ShowDetail();

        private void ShowDetail()
        {
            var order = CurrentOrder;
            if (order == null)
            {
                detailGroup.Text = "Order";
                customerValue.Text = poValue.Text = linesValue.Text = "";
                return;
            }
            detailGroup.Text = $"Order {order.Id}";
            customerValue.Text = order.CustomerName;
            poValue.Text = order.PoNumber;
            linesValue.Text = $"{order.Lines.Count} items";
        }

        private void ordersGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || CurrentOrder == null) return;

            // ShowDialog does not block in Wisej.NET: the result arrives in the callback.
            var dialog = new EditOrderDialog(CurrentOrder, _customerService.GetCustomers());
            dialog.ShowDialog(this, (form, result) =>
            {
                if (result == DialogResult.OK)
                {
                    _orderService.Save(dialog.Order);
                    MessageBox.Show("Saved.", "LegacyOrderDesk");
                    ReloadGrid();
                }
                form.Dispose();
            });
        }

        private void newOrderButton_Click(object sender, EventArgs e)
        {
            var order = new Order
            {
                Customer = _customerService.Find(1), CustomerId = 1, Owner = AppState.CurrentUser,
                PoNumber = "NEW", Status = OrderStatus.Open, CreatedOn = DateTime.Today
            };
            order.Lines.Add(new OrderLine { Sku = "WJ-DEV-SEAT", Description = "Developer seat", Quantity = 1, UnitPrice = 190m });

            var dialog = new EditOrderDialog(order, _customerService.GetCustomers());
            dialog.ShowDialog(this, (form, result) =>
            {
                if (result == DialogResult.OK)
                {
                    _orderService.Save(dialog.Order);
                    ReloadGrid();
                }
                form.Dispose();
            });
        }

        // Was: InvoicePrinter.Print(CurrentOrder, _orderService) — PrintDocument needs a local printer.
        private void printInvoiceButton_Click(object sender, EventArgs e)
        {
            if (CurrentOrder == null) return;
            NotPortedYet("Print Invoice");
        }

        // Was: ExcelExport.ExportOrders(orders, @"C:\Orders\out.xlsx") — Excel Interop and a local path.
        private void exportButton_Click(object sender, EventArgs e) => NotPortedYet("Export to Excel");

        // Was: OpenFileDialog + C:\Orders\Attachments — the user's disk is not reachable from the server.
        private void attachButton_Click(object sender, EventArgs e) => NotPortedYet("Attach file");

        // Was: new SettingsForm().ShowDialog(this) — the settings live in HKCU.
        private void settingsMenuItem_Click(object sender, EventArgs e) => NotPortedYet("Settings");

        private void filterOpenMenuItem_Click(object sender, EventArgs e) { AppState.CurrentFilter = OrderStatus.Open; ReloadGrid(); }
        private void filterAllMenuItem_Click(object sender, EventArgs e) { AppState.CurrentFilter = null; ReloadGrid(); }

        private void exitMenuItem_Click(object sender, EventArgs e) => Close();

        private void aboutMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show("LegacyOrderDesk 3.2 — the WinForms application migrated in \"From WinForms to the Web\".", "About");

        private static void NotPortedYet(string feature) =>
            Ui.Toast($"{feature} is not available in the web version yet.", MessageBoxIcon.Warning);
    }
}
