using System;
using OrderDesk.Domain;
using OrderDesk.Reporting;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk.Screens
{
    /// <summary>
    /// Reports: Print Invoice and Export, the two boundaries Module 1 already crossed. The invoice
    /// preview is a transient modal window shown with the callback form of ShowDialog and disposed
    /// in that callback — the other half of the Module 3 rule (OrdersScreen shows the awaited half).
    /// </summary>
    public partial class ReportsScreen : ScreenBase
    {
        private readonly OrderService _orderService = new OrderService();   // ✓ reused unchanged

        public ReportsScreen()
        {
            InitializeComponent();
        }

        public override string ScreenName => "Reports";

        public override string StatusText => $"{gridReportOrders.Rows.Count} orders · Print Invoice → server PDF · Export → download";

        public Order SelectedOrder => gridReportOrders.CurrentRow?.Tag as Order;

        public override void OnShown(bool first)
        {
            Reload();   // every time: another screen may have saved or deleted an order
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

        private void buttonReportPrint_Click(object sender, EventArgs e)
        {
            RaiseTrace(TraceKind.FromClient, "Print Invoice (PDF)", $"order {SelectedOrder?.Id.ToString() ?? "—"}");
            PrintSelected();
        }

        private void buttonReportExport_Click(object sender, EventArgs e)
        {
            RaiseTrace(TraceKind.FromClient, "Export orders", "all orders → orders.csv");
            Export();
        }

        /// <summary>Print Invoice: the Module 1 replacement, with the caller-disposes rule spelled out.</summary>
        public void PrintSelected()
        {
            var order = SelectedOrder;
            if (order == null) return;

            // ✕ desktop: InvoicePrinter.Print(order) → PrintDocument → the printer on the user's desk.
            // ✓ web:     the same InvoiceDocument lines → a PDF built on the server → PdfViewer in a modal window.
            var lines = InvoiceDocument.Build(order, _orderService);
            var pdf = InvoicePdfWriter.Write(lines, $"Invoice {order.Id}");
            RaiseTrace(TraceKind.Boundary, "Print Invoice", $"PrintDocument → local printer  ⇒  server PDF ({pdf.Length:N0} bytes) → PdfViewer");

            var preview = new InvoicePreviewForm(pdf, $"Invoice-{order.Id}.pdf");
            RaiseTrace(TraceKind.ToClient, "InvoicePreviewForm", $"Invoice-{order.Id}.pdf · ShowDialog(callback) — returns at once, the callback runs on close");
            preview.ShowDialog((form, result) =>
            {
                // ✓ the caller disposes the transient dialog when it closes (callback form of the Module 3 rule)
                form.Dispose();
                RaiseTrace(TraceKind.Server, "InvoicePreviewForm.Dispose", $"closed with {result} · disposed in the ShowDialog callback");
            });
        }

        /// <summary>Export: no Office, no local path — bytes in memory streamed to the browser.</summary>
        public void Export()
        {
            var orders = _orderService.GetOrders();
            var stream = CsvExport.OrdersStream(orders);
            RaiseTrace(TraceKind.Boundary, "Export to Excel", $"Excel.Application + C:\\Orders\\out.xlsx  ⇒  {stream.Length:N0} bytes → Application.Download(\"orders.csv\")");
            Application.Download(stream, "orders.csv");
            RaiseTrace(TraceKind.ToClient, "Application.Download", "orders.csv — the browser saves it; the server never touched a local path");
            // ✕ was: MessageBox.Show("Exported to " + path) — informational → ✓ Toast
            Ui.Toast("orders.csv sent to the browser.");
        }
    }
}
