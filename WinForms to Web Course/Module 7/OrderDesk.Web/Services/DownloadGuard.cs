using System.Collections.Generic;
using System.Globalization;
using System.Text;
using OrderDesk.Domain;

namespace OrderDesk.Services
{
    /// <summary>Result of a server-side authorization check for a download.</summary>
    public sealed class DownloadDecision
    {
        public bool Allowed { get; set; }
        public int StatusCode { get; set; }
        public string FileName { get; set; }
        public string Reason { get; set; }
        public override string ToString() => (Allowed ? "200 " : "✖ " + StatusCode + " ") + FileName + " " + Reason;
    }

    /// <summary>
    /// Downloads are server operations exposed to a browser: the check happens HERE, on the server, for
    /// every request — never in the UI that hides a button. The file is generated, never read from an
    /// arbitrary server path the browser supplied.
    /// </summary>
    public static class DownloadGuard
    {
        public static DownloadDecision Authorize(UserContext ctx, string requiredRole, string fileName)
        {
            if (ctx == null || ctx.User == null)
                return new DownloadDecision { Allowed = false, StatusCode = 401, FileName = fileName, Reason = "needs a signed-in user" };
            if (!ctx.IsInRole(requiredRole))
                return new DownloadDecision { Allowed = false, StatusCode = 403, FileName = fileName, Reason = "needs " + requiredRole + " (" + ctx.UserName + " is " + ctx.Role + ")" };
            return new DownloadDecision { Allowed = true, StatusCode = 200, FileName = fileName, Reason = "granted to " + ctx.UserName + " (" + ctx.Role + ")" };
        }

        /// <summary>The sensitive export: customers WITH TaxId. Manager only — that is why it is guarded.</summary>
        public static string CustomersWithTaxIdCsv(IEnumerable<Customer> customers)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Customer,Country,Tier,CreditLimit,TaxId");
            foreach (var c in customers)
                sb.AppendLine(string.Join(",", Quote(c.Name), c.Country, c.Tier, c.CreditLimit.ToString("0.00", CultureInfo.InvariantCulture), Quote(c.TaxId)));
            return sb.ToString();
        }

        private static string Quote(string s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
    }
}
