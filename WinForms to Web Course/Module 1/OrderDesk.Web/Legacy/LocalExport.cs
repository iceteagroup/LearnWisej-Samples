using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using OrderDesk.Domain;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied from LegacyOrderDesk/Legacy/LocalExport.cs. Desktop assumption: "the file system" is the
    /// user's PC. C:\Orders is a folder on the machine running the code — fine for one desktop, wrong
    /// for a server shared by many browsers.
    ///
    /// Course copy: <see cref="WriteCsv"/> computes the path the desktop code would use and throws
    /// instead of writing — the sample never touches a disk outside its project folder. The pure
    /// formatting half, <see cref="ToCsv"/>, is reused unchanged as the body of a Download.
    /// </summary>
    public static class LocalExport
    {
        public const string ExportFolder = @"C:\Orders";                 // ✕ local path from App.config ExportFolder

        /// <summary>The file the desktop code writes: C:\Orders\out.csv (on the machine running the code).</summary>
        public static string TargetPath => ExportFolder + @"\out.csv";

        /// <summary>✕ the desktop version — Directory.CreateDirectory(ExportFolder); File.WriteAllText(path, ToCsv(orders)).</summary>
        public static string WriteCsv(IEnumerable<Order> orders)
        {
            var path = TargetPath;
            var csv = ToCsv(orders);
            // On the server this line would create C:\Orders on the web server's disk, under the service
            // account, and the browser would never see the file. The course copy stops here.
            throw new InvalidOperationException(
                "would write " + path + " on the SERVER (machine " + Environment.MachineName + ", account " + Environment.UserName +
                ") — the desktop code meant the user's PC. The browser never sees a file written to the server's disk; " +
                csv.Length + " characters were NOT written.");
        }

        /// <summary>✓ Pure formatting — reusable on the web as the body of a Download.</summary>
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
