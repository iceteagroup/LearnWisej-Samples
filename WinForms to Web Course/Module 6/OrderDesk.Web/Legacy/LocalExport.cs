using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using OrderDesk.Domain;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied from LegacyOrderDesk/Legacy/LocalExport.cs so the desktop assumption can be shown
    /// breaking on the server. Desktop assumption: "the file system" is the user's PC. C:\Orders is
    /// a folder on the machine running the code — fine for one desktop, wrong for a server shared by
    /// many browsers. The pure formatting (<see cref="ToCsv"/>) is business logic and is reused as-is
    /// by the web export; only the two methods that touch C:\Orders are the migration problem.
    /// </summary>
    public static class LocalExport
    {
        public const string ExportFolder = @"C:\Orders";   // ✕ Windows-only, machine-local path

        /// <summary>
        /// ✕ The desktop "Export" fallback. On the server this would create C:\Orders on the SERVER
        /// disk under the service account. The lab copy never writes outside the project folder: it
        /// computes the path, reports where the file would have gone and throws.
        /// </summary>
        public static string WriteCsv(IEnumerable<Order> orders)
        {
            var path = Path.Combine(ExportFolder, "out.csv");            // ✕ hard-coded local folder
            var csv = ToCsv(orders);                                      // ✓ business logic, still fine
            throw new InvalidOperationException(
                "would write " + path + " on the SERVER (machine " + Environment.MachineName + ", account " + Environment.UserName +
                ") — " + csv.Length + " characters the browser user would never see. The web replacement hands the same bytes to Application.Download.");
        }

        /// <summary>
        /// ✕ The desktop "Import" path: File.ReadAllText(@"C:\Orders\in.csv"). Migrated unchanged, this
        /// reads the SERVER's disk — the browser user's C: drive is not reachable from server code.
        /// </summary>
        public static string ReadCsv()
        {
            var path = Path.Combine(ExportFolder, "in.csv");             // ✕ the user's disk? No: the server's.
            if (!File.Exists(path))
                throw new FileNotFoundException("Could not find file '" + path + "' — looked on the SERVER (machine " + Environment.MachineName + ", account " + Environment.UserName + "), not on the browser user's machine.", path);

            // Even when the folder happens to exist on the server it is the wrong file: nothing the
            // browser user selected can be here. Refuse instead of importing a stranger's data.
            throw new InvalidOperationException("'" + path + "' exists on the SERVER, but it is not the browser user's file. Server code needs an Upload to receive user files.");
        }

        /// <summary>✓ Pure formatting — reusable on the web as the body of a Download (and as the import contract).</summary>
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
                    o.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            return sb.ToString();
        }

        private static string Quote(string s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
    }
}
