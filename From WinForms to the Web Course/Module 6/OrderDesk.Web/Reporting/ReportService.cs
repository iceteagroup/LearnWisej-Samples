using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OrderDesk.Domain;
using OrderDesk.Files;

namespace OrderDesk.Reporting
{
    /// <summary>
    /// ✓ The lesson's reportService: turns domain objects into documents on the server. It knows the
    /// storage root and the managed writers; it knows nothing about printers, Excel or the user's PC.
    ///
    ///   var rows = orderService.Search(currentFilter);
    ///   var path = reportService.CreateOrdersWorkbook(rows);
    ///   Application.Download(path, "Orders.xlsx");
    /// </summary>
    public sealed class ReportService
    {
        private readonly OrderService _orders;

        public ReportService(OrderService orders)
        {
            _orders = orders ?? throw new ArgumentNullException(nameof(orders));
        }

        /// <summary>
        /// Writes the orders as an .xlsx under StorageRoot.Exports and returns the server path —
        /// the input for Application.Download(path, "Orders.xlsx"). The file name carries a
        /// timestamp so two sessions exporting at the same moment never collide.
        /// </summary>
        public string CreateOrdersWorkbook(IList<Order> rows)
        {
            var bytes = XlsxWriter.OrdersWorkbook(rows ?? Array.Empty<Order>());
            string stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff", CultureInfo.InvariantCulture);
            string path = StorageRoot.ExportPath($"Orders-{stamp}.xlsx");
            File.WriteAllBytes(path, bytes);
            return path;
        }

        /// <summary>The invoice as PDF bytes: InvoiceDocument (shared rule) → InvoicePdfWriter (server).</summary>
        public byte[] CreateInvoicePdf(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            var lines = InvoiceDocument.Build(order, _orders);
            return InvoicePdfWriter.Write(lines, $"Invoice {order.Id}");
        }
    }
}
