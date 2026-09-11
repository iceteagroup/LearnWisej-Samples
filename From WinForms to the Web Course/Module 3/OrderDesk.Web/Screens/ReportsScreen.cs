using System;
using OrderDesk.Domain;
using OrderDesk.Reporting;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk.Screens
{
    /// <summary>
    /// Reports: Print Invoice (a server-generated PDF in a modal PdfViewer, disposed in the
    /// ShowDialog callback) and Export (a file streamed to the browser).
    /// </summary>
    public partial class ReportsScreen : ScreenBase
    {
        private readonly OrderService _orderService = new OrderService();

        public ReportsScreen()
        {
            InitializeComponent();
        }

        public override string ScreenName => "Reports";

        public override string StatusText => $"{gridReportOrders.Rows.Count} orders";

        public Order SelectedOrder => gridReportOrders.CurrentRow?.Tag as Order;

        public override void OnShown(bool first)
        {
            Reload();   // every time: another screen may have saved an order
        }

        public void Reload(int? selectId = null)
        {
            int? keep = selectId ?? SelectedOrder?.Id;
            var orders = _orderService.GetOrders();
            gridReportOrders.Rows.Clear();
            foreach (var order in orders)
            {
                int index = gridReportOrders.Rows.Add(order.Id, order.CustomerName, order.Total, order.Status.ToString());
                gridReportOrders.Rows[index].Tag = order;
            }
            SelectOrder(keep ?? (orders.Count > 0 ? orders[0].Id : 0));
            RaiseStatusChanged();
        }

        public void SelectOrder(int id)
        {
            for (int i = 0; i < gridReportOrders.Rows.Count; i++)
            {
                if ((gridReportOrders.Rows[i].Tag as Order)?.Id == id)
                {
                    gridReportOrders.Rows[i].Selected = true;
                    return;
                }
            }
            if (gridReportOrders.Rows.Count > 0)
                gridReportOrders.Rows[0].Selected = true;
        }

        private void buttonReportPrint_Click(object sender, EventArgs e) => PrintSelected();

        private void buttonReportExport_Click(object sender, EventArgs e) => Export();

        /// <summary>Print Invoice: the invoice lines as a PDF built on the server, in a modal PdfViewer.</summary>
        public void PrintSelected()
        {
            var order = SelectedOrder;
            if (order == null) return;

            var lines = InvoiceDocument.Build(order, _orderService);
            var pdf = InvoicePdfWriter.Write(lines, $"Invoice {order.Id}");

            var preview = new InvoicePreviewForm(pdf, $"Invoice-{order.Id}.pdf");
            preview.ShowDialog((form, result) => form.Dispose());
        }

        /// <summary>Export: no Office, no local path — bytes in memory streamed to the browser.</summary>
        public void Export()
        {
            var stream = CsvExport.OrdersStream(_orderService.GetOrders());
            Application.Download(stream, "orders.csv");
            Ui.Toast("orders.csv sent to the browser.");
        }
    }
}
