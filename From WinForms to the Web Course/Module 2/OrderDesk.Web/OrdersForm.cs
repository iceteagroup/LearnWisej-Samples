using System;
using System.IO;
using Wisej.Web;                       // ✓ was: using System.Windows.Forms;
// ✕ compiler: Namespace/using — using LegacyOrderDesk.Reporting;  (CS0246 · InvoicePrinter/ExcelExport are desktop-only, not copied)
// ✕ compiler: Namespace/using — using LegacyOrderDesk.Settings;   (CS0246 · RegistrySettings is desktop-only, not copied)
using OrderDesk.Dialogs;
using OrderDesk.Domain;
using OrderDesk.Legacy;
using OrderDesk.Views;

namespace OrderDesk                    // ✓ was: namespace LegacyOrderDesk
{
    /// <summary>
    /// LegacyOrderDesk/OrdersForm.cs after the Module 2 port: <c>System.Windows.Forms</c> became
    /// <c>Wisej.Web</c>, the class is still a <c>Form</c>, the handlers and the business logic are the
    /// same lines. Every place the compiler stopped is kept as the ORIGINAL line, commented and tagged
    /// <c>✕ compiler: &lt;category&gt;</c>, with a one-line web-safe stand-in underneath so the form runs
    /// in the browser. The stand-ins are NOT the final replacements — those are Modules 3–6.
    /// (docs/FormVsPage.md explains why this stays a Form and how it becomes a Page in one line.)
    /// </summary>
    public partial class OrdersForm : Form
    {
        private readonly OrderService _orderService = new OrderService();          // ✓ reused unchanged
        private readonly CustomerService _customerService = new CustomerService();  // ✓ reused unchanged

        /// <summary>Raised by every stand-in so the lab console can log the boundary that was hit (name, message).</summary>
        public event Action<string, string> BoundaryHit;

        public OrdersForm()
        {
            InitializeComponent();
        }

        private void OrdersForm_Load(object sender, EventArgs e)
        {
            // ✕ compiler: Unsupported desktop op — RegistrySettings (CS0103) → per-user profile store (Module 4)
            // var settings = RegistrySettings.Load();
            // if (settings.WindowWidth > 0 && settings.WindowHeight > 0)
            //     Size = new System.Drawing.Size(settings.WindowWidth, settings.WindowHeight);
            Boundary("RegistrySettings.Load", "HKCU window size not restored — the browser window belongs to the user (Module 4 / Module 7)");

            ReloadGrid();
            statusLabel.Text = $"Ready · {ordersGrid.Rows.Count} orders · user {AppState.CurrentUser} · single-user desktop";
        }

        private void ReloadGrid()
        {
            var filter = new OrderFilter { Status = AppState.CurrentFilter, Text = AppState.LastSearch };   // ✕ runtime: Unclear/deferred — static AppState is one slot for every session (Module 4)
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

            // ✕ runtime: Unclear/deferred — compiles, but ShowDialog(this) returns immediately on the web, so the
            //   "== DialogResult.OK" branch never runs; the result arrives in a callback (Module 3)
            // var dialog = new EditOrderDialog(CurrentOrder, _customerService.GetCustomers());
            // if (dialog.ShowDialog(this) == DialogResult.OK)
            // {
            //     _orderService.Save(dialog.Order);                 // ✓ business logic reused
            //     MessageBox.Show("Saved.", "LegacyOrderDesk");      // Module 3: a Toast is enough
            //     ReloadGrid();
            // }
            var dialog = new EditOrderDialog(CurrentOrder, _customerService.GetCustomers());
            dialog.ShowDialog(this, (form, result) =>
            {
                if (result == DialogResult.OK)
                {
                    _orderService.Save(dialog.Order);                 // ✓ business logic reused
                    MessageBox.Show("Saved.", "LegacyOrderDesk");      // ✓ even MessageBox.Show works — Module 3: a Toast is enough
                    ReloadGrid();
                }
                form.Dispose();                                       // ✓ the desktop habit never disposed it
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

            // ✕ runtime: Unclear/deferred — compiles, but "using" disposes the dialog the moment ShowDialog returns,
            //   i.e. while it is still open in the browser; dispose in the callback instead (Module 3)
            // using (var dialog = new EditOrderDialog(order, _customerService.GetCustomers()))
            // {
            //     if (dialog.ShowDialog(this) == DialogResult.OK)
            //     {
            //         _orderService.Save(dialog.Order);
            //         ReloadGrid();
            //     }
            // }
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

        private void printInvoiceButton_Click(object sender, EventArgs e)
        {
            if (CurrentOrder == null) return;
            // ✕ compiler: Unsupported desktop op — InvoicePrinter (CS0103: PrintDocument + PrintPreviewDialog) → server PDF in a PdfViewer (Module 6)
            // InvoicePrinter.Print(CurrentOrder, _orderService);
            Boundary("InvoicePrinter.Print", $"no printer on the server for order {CurrentOrder.Id} — InvoiceDocument stays, delivery becomes a server PDF (Module 6)");
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            try
            {
                // ✕ compiler: Unsupported desktop op — ExcelExport (CS0103: Excel Interop + C:\Orders\out.xlsx) → managed writer + Application.Download (Module 6)
                // var path = ExcelExport.ExportOrders(_orderService.GetOrders(), @"C:\Orders\out.xlsx");
                // MessageBox.Show("Exported to " + path, "LegacyOrderDesk");
                Boundary("ExcelExport.ExportOrders", $"no Excel and no C:\\Orders on the server — {_orderService.GetOrders().Count} orders will stream as a download (Module 6)");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed: " + ex.Message, "LegacyOrderDesk", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void attachButton_Click(object sender, EventArgs e)
        {
            // ✕ runtime: Unsupported desktop op — compiles (Wisej.Web.OpenFileDialog exists) but it browses the SERVER's
            //   file system, and C:\Orders\Attachments would be a folder on the server → Upload control + storage root (Module 6)
            // using (var open = new OpenFileDialog { Title = "Attach a file to the order" })
            // {
            //     if (open.ShowDialog(this) != DialogResult.OK || CurrentOrder == null) return;
            //     var folder = Path.Combine(@"C:\Orders\Attachments", CurrentOrder.Id.ToString());
            //     Directory.CreateDirectory(folder);
            //     File.Copy(open.FileName, Path.Combine(folder, Path.GetFileName(open.FileName)), overwrite: true);
            //     MessageBox.Show("Attached to " + folder, "LegacyOrderDesk");
            // }
            if (CurrentOrder == null) return;
            Boundary("OpenFileDialog", $"the user's disk is unreachable from the server — order {CurrentOrder.Id} attachments go through Upload → {Path.Combine("App_Data", "Attachments")} (Module 6)");
        }

        private void filterOpenMenuItem_Click(object sender, EventArgs e) { AppState.CurrentFilter = OrderStatus.Open; ReloadGrid(); }
        private void filterAllMenuItem_Click(object sender, EventArgs e) { AppState.CurrentFilter = null; ReloadGrid(); }

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            // ✕ compiler: Unsupported desktop op — SettingsForm (CS0246: not copied, it edits HKCU) → per-user settings dialog (Module 4)
            // using (var dialog = new SettingsForm())
            //     dialog.ShowDialog(this);
            Boundary("SettingsForm", "not ported yet — grid density and export folder move from HKCU to a per-user store (Module 4)");
        }

        private void exitMenuItem_Click(object sender, EventArgs e) => Close();

        private void aboutMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show("LegacyOrderDesk 3.2 — the WinForms application migrated in \"From WinForms to the Web\".", "About");

        private void OrdersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ✕ compiler: Unsupported desktop op — RegistrySettings (CS0103 ×2) → nothing to save: the browser owns the window size
            // var settings = RegistrySettings.Load();
            // settings.WindowWidth = Width;
            // settings.WindowHeight = Height;
            // RegistrySettings.Save(settings);
            Boundary("RegistrySettings.Save", $"window size {Width}×{Height} not written to HKCU — the service account's registry is shared by every visitor");
        }

        /// <summary>The one-line stand-in every commented desktop call uses: tell the user, tell the console.</summary>
        private void Boundary(string name, string message)
        {
            Ui.Toast($"{name}: {message}", MessageBoxIcon.Warning);
            BoundaryHit?.Invoke(name, message);
        }
    }
}
