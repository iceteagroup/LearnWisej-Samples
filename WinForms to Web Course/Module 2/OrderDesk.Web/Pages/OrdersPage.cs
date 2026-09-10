using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Wisej.Web;                       // ✓ was: using System.Windows.Forms;
using OrderDesk.Domain;
using OrderDesk.Legacy;

namespace OrderDesk.Pages
{
    /// <summary>
    /// LegacyOrderDesk.OrdersForm moved into the Wisej.NET shell — the first form of the port
    /// (Module 2). The using directive and the base class changed; the fields, the event handlers
    /// and the business-logic calls are the WinForms ones. Everything marked ✕ is desktop plumbing
    /// that stays for parity in this module and becomes an explicit task in Modules 3–6.
    ///
    /// Form → UserControl: in this lab the ported form is hosted inside the lab's MainPage next to
    /// the migration trace. When the ported form IS the main view, the equivalent is
    /// <c>OrdersPage : Page</c> with <c>"mainWindow": "OrderDesk.Pages.OrdersPage, OrderDesk"</c>
    /// in Default.json (the video shows that variant).
    /// </summary>
    public partial class OrdersPage : UserControl            // was: Form
    {
        private static readonly CultureInfo Money = CultureInfo.GetCultureInfo("en-US");

        private readonly OrderService _service = new OrderService();   // ✓ business logic, reused as-is
        private List<Order> _rows = new List<Order>();                 // was: BindingList<Order> — the grid binds a List<Order>

        /// <summary>Lab hook: what the ported form does, for the host's trace panel. Not part of the product.</summary>
        public event Action<string, string> Activity;

        public OrdersPage()
        {
            InitializeComponent();
        }

        private void OrdersPage_Load(object sender, EventArgs e)      // was: OrdersForm_Load
        {
            Text = "LegacyOrderDesk — Orders  ·  " + (AppState.CurrentUser?.DisplayName ?? "(no login yet — Module 4)");   // ✕ static per-user state
            ReloadGrid();
        }

        /// <summary>✕ loads EVERY row — fine on a LAN desktop, the Module 5 problem on the web.</summary>
        public void ReloadGrid()
        {
            _rows = _service.GetAll();
            Report("OrderService.GetAll()", _rows.Count + " orders · first " + Describe(_rows[0]));
            ordersGrid.DataSource = null;
            ordersGrid.DataSource = _rows;                              // was: BindingList<Order>
            statusLabel.Text = "Ready · " + _rows.Count + " orders · single-user desktop";   // ✕ still says single-user (Module 4)
            if (ordersGrid.Rows.Count > 0) ordersGrid.Rows[0].Selected = true;
            ShowDetail();
        }

        public int RowCount => _rows.Count;

        private Order SelectedOrder
        {
            get
            {
                var row = ordersGrid.CurrentRow;
                if (row == null && ordersGrid.SelectedRows.Count > 0) row = ordersGrid.SelectedRows[0];
                return row?.DataBoundItem as Order ?? (_rows.Count > 0 ? _rows[0] : null);
            }
        }

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
            if (o != null) Report("detail panel", Describe(o) + " · AppState.CurrentOrder set (✕ static)");
        }

        private void ordersGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || SelectedOrder == null) return;
            // The desktop opened EditOrderDialog here (never disposed). The dialog is ported in Module 3;
            // in Module 2 the handler proves the other half of the video's point: MessageBox.Show works in Wisej.Web.
            Report("CellDoubleClick", "Order " + SelectedOrder.Id + " → EditOrderDialog is ported in Module 3 · MessageBox.Show works");
            MessageBox.Show("Edit Order " + SelectedOrder.Id + "\n\nEditOrderDialog is ported in Module 3 (Forms, Navigation & Modal Workflow).", "Edit Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void newOrderButton_Click(object sender, EventArgs e)
        {
            Report("New Order", "EditOrderDialog is ported in Module 3 · MessageBox.Show works");
            MessageBox.Show("New Order\n\nEditOrderDialog is ported in Module 3 (Forms, Navigation & Modal Workflow).", "New Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void printInvoiceButton_Click(object sender, EventArgs e)
        {
            if (SelectedOrder == null) return;
            // ✕ InvoicePrinter drew on a PrintDocument and sent it to the printer of the machine running the code.
            Report("Print Invoice ✕", "Order " + SelectedOrder.Id + " · PrintDocument targets the SERVER's printer — Module 6 makes a PDF");
            MessageBox.Show("Could not print: PrintDocument prints on the machine running the code — on the web server there is no user printer.\n\nModule 6 replaces it with a server-generated PDF shown in a PdfViewer.", "Print Invoice", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            // ✕ ExcelExport (Office Interop) with the C:\Orders CSV fallback — both are desktop boundaries.
            Report("Export to Excel ✕", "Excel Interop needs Excel in an interactive session · fallback wrote C:\\Orders on the SERVER — Module 6");
            MessageBox.Show("Excel is not available (Office Automation needs Excel installed in an interactive user session).\n\nThe old fallback wrote C:\\Orders\\out.csv on the machine running the code — the server. Module 6 replaces both with a managed .xlsx download.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void exitMenuItem_Click(object sender, EventArgs e)   // was: => Close();
        {
            // Close() ended the desktop process. A hosted web view has nothing to close: the session ends
            // when the browser tab closes or times out (Module 4 handles ApplicationExit / SessionTimeout).
            Report("File → Exit", "Close() removed — the session ends with the tab (Application.ApplicationExit, Module 4)");
            MessageBox.Show("Exit closed the desktop app. On the web the session ends when the tab closes or times out — see Module 4 (session cleanup).", "Exit", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            Report("Help → About", "MessageBox.Show works in Wisej.Web");
            MessageBox.Show("LegacyOrderDesk 3.2 — the desktop order desk the course migrates to Wisej.NET.\n\nNow running as OrderDesk.Web (Wisej-4 4.1.0, Module 2).", "About");
        }

        private static string Describe(Order o)
            => o.Id + " " + o.CustomerName + " " + o.Total.ToString("C2", Money) + " " + o.Status;

        private void Report(string name, string payload) => Activity?.Invoke(name, payload);
    }
}
