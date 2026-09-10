using System;
using System.Collections.Generic;
using System.Globalization;

namespace OrderDesk.Domain
{
    /// <summary>
    /// The text of an invoice, line by line. LegacyOrderDesk hands these lines to a PrintDocument;
    /// OrderDesk.Web hands them to a server-side PDF writer. The content rule is shared.
    /// </summary>
    public static class InvoiceDocument
    {
        public static IList<string> Build(Order order, OrderService service)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            var ci = CultureInfo.InvariantCulture;
            var lines = new List<string>
            {
                $"INVOICE  #{order.Id}",
                $"Date: {DateTime.Today.ToString("yyyy-MM-dd", ci)}    PO: {order.PoNumber}",
                $"Bill to: {order.CustomerName} — {order.Customer?.City}, {order.Customer?.Country}",
                "",
                $"{"SKU",-14}{"Description",-28}{"Qty",5}{"Unit",12}{"Line",12}",
                new string('-', 71),
            };
            decimal subtotal = 0;
            foreach (var l in order.Lines)
            {
                subtotal += l.LineTotal;
                lines.Add($"{l.Sku,-14}{Trunc(l.Description, 27),-28}{l.Quantity,5}{l.UnitPrice.ToString("N2", ci),12}{l.LineTotal.ToString("N2", ci),12}");
            }
            var discount = service.DiscountFor(order.Customer, subtotal);
            var tax = service.Tax(subtotal);
            lines.Add(new string('-', 71));
            lines.Add($"{"Subtotal",59}{subtotal.ToString("N2", ci),12}");
            if (discount != 0) lines.Add($"{"Discount",59}{(-discount).ToString("N2", ci),12}");
            if (tax != 0) lines.Add($"{"Tax",59}{tax.ToString("N2", ci),12}");
            lines.Add($"{"TOTAL",59}{service.CalculateOrderTotal(order).ToString("N2", ci),12}");
            lines.Add("");
            lines.Add("Thank you for your business.");
            return lines;
        }

        private static string Trunc(string s, int max) => string.IsNullOrEmpty(s) || s.Length <= max ? s ?? "" : s.Substring(0, max - 1) + "…";
    }
}
