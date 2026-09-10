using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OrderDesk.Domain;

namespace OrderDesk.Services
{
    /// <summary>One line of an import result — what the Import card lists and the trace logs.</summary>
    public sealed class ImportRowResult
    {
        public int Line { get; set; }
        public int OrderId { get; set; }
        public bool Saved { get; set; }
        public string Message { get; set; }
        public override string ToString() => (Saved ? "✓ " : "✖ ") + "row " + Line + (OrderId != 0 ? " · order " + OrderId : "") + " · " + Message;
    }

    public sealed class ImportResult
    {
        public List<ImportRowResult> Rows { get; } = new List<ImportRowResult>();
        public List<int> SavedIds { get; } = new List<int>();
        public int SavedCount => Rows.Count(r => r.Saved);
        public int RejectedCount => Rows.Count(r => !r.Saved);
        public string Summary => SavedCount + " saved · " + RejectedCount + " rejected";
    }

    /// <summary>
    /// The server side of the upload → process → result pattern. It parses the CSV contract that
    /// LocalExport.ToCsv writes (Order,Customer,Owner,Total,Status,Date), turns each row into an
    /// Order and saves it through the reused business logic — OrderService.Save runs OrderValidator,
    /// so a row without an owner is rejected by the same rule the edit dialog enforces.
    /// </summary>
    public sealed class CsvImportService
    {
        private readonly OrderService _service;

        public CsvImportService(OrderService service) { _service = service ?? throw new ArgumentNullException(nameof(service)); }

        public ImportResult Import(string csv)
        {
            var result = new ImportResult();
            if (string.IsNullOrWhiteSpace(csv)) return result;

            var lines = csv.Replace("\r\n", "\n").Split('\n').Select(l => l.TrimEnd('\r')).ToList();
            int lineNo = 0;
            bool headerSeen = false;
            foreach (var raw in lines)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(raw)) continue;
                var fields = SplitCsv(raw);
                if (!headerSeen)
                {
                    headerSeen = true;
                    if (fields.Count > 0 && fields[0].Equals("Order", StringComparison.OrdinalIgnoreCase)) continue;   // header row
                }
                result.Rows.Add(ImportRow(lineNo, fields, result));
            }
            return result;
        }

        private ImportRowResult ImportRow(int lineNo, List<string> f, ImportResult result)
        {
            var r = new ImportRowResult { Line = lineNo };
            if (f.Count < 6) { r.Message = "expected 6 columns (Order,Customer,Owner,Total,Status,Date), found " + f.Count; return r; }

            int.TryParse(f[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var id);
            r.OrderId = id;

            var customer = _service.Customers.FirstOrDefault(c => string.Equals(c.Name, f[1].Trim(), StringComparison.OrdinalIgnoreCase));
            if (customer == null) { r.Message = "unknown customer '" + f[1] + "'"; return r; }

            if (!decimal.TryParse(f[3], NumberStyles.Number, CultureInfo.InvariantCulture, out var total) || total < 0)
            { r.Message = "Total '" + f[3] + "' is not a number"; return r; }

            if (!Enum.TryParse(f[4].Trim(), true, out OrderStatus status))
            { r.Message = "unknown status '" + f[4] + "' (Open, InProgress, Shipped, Invoiced, Hold)"; return r; }

            if (!DateTime.TryParseExact(f[5].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                date = DateTime.Today;

            var order = new Order
            {
                Id = id,
                Customer = customer,
                Owner = string.IsNullOrWhiteSpace(f[2]) ? null : f[2].Trim(),
                Status = status,
                Date = date,
                PoNumber = "IMP-" + (id != 0 ? id.ToString(CultureInfo.InvariantCulture) : lineNo.ToString(CultureInfo.InvariantCulture)),
                Notes = "Imported from CSV",
            };
            // The CSV carries a total, not lines: one line that reproduces it (tax-exempt customers).
            order.Lines.Add(new OrderLine { Sku = "IMPORT", Description = "Imported order total", Quantity = 1, UnitPrice = total });

            try
            {
                _service.Save(order);                          // ✓ reused business logic: OrderValidator runs here
                r.Saved = true;
                r.OrderId = order.Id;
                r.Message = customer.Name + " · " + order.Total.ToString("C", CultureInfo.GetCultureInfo("en-US")) + " · " + status;
                result.SavedIds.Add(order.Id);
            }
            catch (ValidationException ex)
            {
                r.Message = string.Join("; ", ex.Result.Errors.Select(e => e.Key + ": " + e.Value));
            }
            return r;
        }

        /// <summary>RFC-4180-ish: commas, double quotes, "" escapes. Enough for the export contract.</summary>
        public static List<string> SplitCsv(string line)
        {
            var fields = new List<string>();
            var sb = new System.Text.StringBuilder();
            bool quoted = false;
            for (int i = 0; i < line.Length; i++)
            {
                var ch = line[i];
                if (quoted)
                {
                    if (ch == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                        else quoted = false;
                    }
                    else sb.Append(ch);
                }
                else if (ch == '"') quoted = true;
                else if (ch == ',') { fields.Add(sb.ToString()); sb.Clear(); }
                else sb.Append(ch);
            }
            fields.Add(sb.ToString());
            return fields;
        }
    }
}
