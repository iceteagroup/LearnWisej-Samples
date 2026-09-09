using System;
using System.IO;
using System.Windows.Forms;
using LegacyOrderDesk.Reporting;
using LegacyOrderDesk.Settings;
using OrderDesk.Domain;

namespace LegacyOrderDesk
{
    /// <summary>
    /// The main screen of LegacyOrderDesk: an orders grid, a detail panel and the four actions
    /// the course keeps coming back to (New Order, Print Invoice, Export to Excel, Attach file).
    /// The business logic lives in OrderDesk.Domain and is reused as-is by the web app; the
    /// desktop plumbing in this file is what the migration has to replace.
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
            // ✕ registry-backed user preference (server registry ≠ user's registry on the web)
            var settings = RegistrySettings.Load();
            if (settings.WindowWidth > 0 && settings.WindowHeight > 0)
                Size = new System.Drawing.Size(settings.WindowWidth, settings.WindowHeight);

            ReloadGrid();
            statusLabel.Text = $"Ready · {ordersGrid.Rows.Count} orders · user {AppState.CurrentUser} · single-user desktop";
        }

        private void ReloadGrid()
        {
            var filter = new OrderFilter { Status = AppState.CurrentFilter, Text = AppState.LastSearch };
            ordersGrid.AutoGenerateColumns = false;
            ordersGrid.DataSource = _orderService.Search(filter);   // ✓ business logic reused
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

            // ✕ desktop habit: the dialog is never disposed (Module 3 fixes it with a using block)
            var dialog = new EditOrderDialog(CurrentOrder, _customerService.GetCustomers());
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _orderService.Save(dialog.Order);                 // ✓ business logic reused
                MessageBox.Show("Saved.", "LegacyOrderDesk");      // Module 3: a Toast is enough
                ReloadGrid();
            }
        }

        private void newOrderButton_Click(object sender, EventArgs e)
        {
            var order = new Order
            {
                Customer = _customerService.Find(1), CustomerId = 1, Owner = AppState.CurrentUser,
                PoNumber = "NEW", Status = OrderStatus.Open, CreatedOn = DateTime.Today
            };
            order.Lines.Add(new OrderLine { Sku = "WJ-DEV-SEAT", Description = "Developer seat", Quantity = 1, UnitPrice = 190m });
            using (var dialog = new EditOrderDialog(order, _customerService.GetCustomers()))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _orderService.Save(dialog.Order);
                    ReloadGrid();
                }
            }
        }

        private void printInvoiceButton_Click(object sender, EventArgs e)
        {
            if (CurrentOrder == null) return;
            // ✕ prints to a printer attached to THIS machine — on a server there is none
            InvoicePrinter.Print(CurrentOrder, _orderService);
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            try
            {
                // ✕ Excel Interop + a local path — neither exists on a shared server
                var path = ExcelExport.ExportOrders(_orderService.GetOrders(), @"C:\Orders\out.xlsx");
                MessageBox.Show("Exported to " + path, "LegacyOrderDesk");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed: " + ex.Message, "LegacyOrderDesk", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void attachButton_Click(object sender, EventArgs e)
        {
            // ✕ local file dialog + local attachments folder on the user's PC
            using (var open = new OpenFileDialog { Title = "Attach a file to the order" })
            {
                if (open.ShowDialog(this) != DialogResult.OK || CurrentOrder == null) return;
                var folder = Path.Combine(@"C:\Orders\Attachments", CurrentOrder.Id.ToString());
                Directory.CreateDirectory(folder);
                File.Copy(open.FileName, Path.Combine(folder, Path.GetFileName(open.FileName)), overwrite: true);
                MessageBox.Show("Attached to " + folder, "LegacyOrderDesk");
            }
        }

        private void filterOpenMenuItem_Click(object sender, EventArgs e) { AppState.CurrentFilter = OrderStatus.Open; ReloadGrid(); }
        private void filterAllMenuItem_Click(object sender, EventArgs e) { AppState.CurrentFilter = null; ReloadGrid(); }

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new SettingsForm())
                dialog.ShowDialog(this);
        }

        private void exitMenuItem_Click(object sender, EventArgs e) => Close();

        private void aboutMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show("LegacyOrderDesk 3.2 — the WinForms application migrated in \"From WinForms to the Web\".", "About");

        private void OrdersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var settings = RegistrySettings.Load();
            settings.WindowWidth = Width;
            settings.WindowHeight = Height;
            RegistrySettings.Save(settings);
        }
    }
}
