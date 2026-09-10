using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using OrderDesk.Domain;

namespace OrderDesk.Reporting
{
    /// <summary>
    /// The web replacement for "Export to Excel" in the first slice: build the file on the server
    /// in memory and let Application.Download hand it to the browser. No local path, no Interop.
    /// (Module 6 upgrades this to a real .xlsx written with a managed library.)
    /// </summary>
    public static class CsvExport
    {
        public static byte[] Orders(IEnumerable<Order> orders)
        {
            var ci = CultureInfo.InvariantCulture;
            var sb = new StringBuilder();
            sb.AppendLine("Order,Customer,Owner,PO,Status,Total");
            foreach (var o in orders)
                sb.AppendLine($"{o.Id},{Quote(o.CustomerName)},{Quote(o.Owner)},{Quote(o.PoNumber)},{o.Status},{o.Total.ToString("F2", ci)}");
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static MemoryStream OrdersStream(IEnumerable<Order> orders) => new MemoryStream(Orders(orders));

        private static string Quote(string s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
    }
}
