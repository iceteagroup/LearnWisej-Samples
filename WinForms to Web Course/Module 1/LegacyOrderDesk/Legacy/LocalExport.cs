using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using OrderDesk.Domain;

namespace LegacyOrderDesk
{
    /// <summary>
    /// Desktop assumption: "the file system" is the user's PC. C:\Orders is a folder on the machine
    /// running the code — fine for one desktop, wrong for a server shared by many browsers.
    /// </summary>
    public static class LocalExport
    {
        public const string ExportFolder = @"C:\Orders";

        public static string WriteCsv(IEnumerable<Order> orders)
        {
            Directory.CreateDirectory(ExportFolder);
            var path = Path.Combine(ExportFolder, "out.csv");
            File.WriteAllText(path, ToCsv(orders), Encoding.UTF8);
            return path;
        }

        /// <summary>Pure formatting — reusable on the web as the body of a Download.</summary>
        public static string ToCsv(IEnumerable<Order> orders)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Order,Customer,Owner,Total,Status,Date");
            foreach (var o in orders)
                sb.AppendLine(string.Join(",",
                    o.Id,
                    Quote(o.CustomerName),
                    Quote(o.Owner),
                    o.Total.ToString("0.00", CultureInfo.InvariantCulture),
                    o.Status,
                    o.Date.ToString("yyyy-MM-dd")));
            return sb.ToString();
        }

        private static string Quote(string s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
    }
}
