using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using OrderDesk.Domain;

namespace OrderDesk.Files
{
    /// <summary>The outcome of one import: what was saved and what was skipped, line by line.</summary>
    public sealed class ImportResult
    {
        public string Format { get; set; }
        public int RowCount { get; set; }
        public List<Order> Saved { get; } = new List<Order>();
        public List<string> Skipped { get; } = new List<string>();
    }

    /// <summary>
    /// ✓ The migrated "Import orders": parses a CSV that arrived through the Upload control and
    /// saves orders through the reused business logic (CustomerService.FindByName + OrderService.Save,
    /// which computes the total). The importer never knows where the file came from — a stream is
    /// a stream — which is exactly what the desktop version (File.ReadAllLines(@"C:\Orders\in.csv"))
    /// got wrong.
    ///
    /// Two layouts are accepted:
    ///   Customer,PO,Sku,Qty,UnitPrice          one line per order line; rows with the same Customer + PO form one order
    ///   Order,Customer,Owner,PO,Status,Total   the CsvExport layout; one order per row with a single "imported total" line
    /// </summary>
    public static class OrdersCsvImporter
    {
        public static ImportResult Import(Stream csv, CustomerService customers, OrderService orders, string owner)
        {
            if (csv == null) throw new ArgumentNullException(nameof(csv));
            string text;
            using (var reader = new StreamReader(csv, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true))
                text = reader.ReadToEnd();
            return Import(text, customers, orders, owner);
        }

        public static ImportResult Import(string csv, CustomerService customers, OrderService orders, string owner)
        {
            var result = new ImportResult();
            var lines = (csv ?? "").Split('\n').Select(l => l.TrimEnd('\r')).Where(l => l.Trim().Length > 0).ToList();
            if (lines.Count == 0)
            {
                result.Format = "empty";
                return result;
            }

            var header = Split(lines[0]).Select(h => h.Trim()).ToList();
            var rows = lines.Skip(1).Select(Split).ToList();
            result.RowCount = rows.Count;

            if (header.Any(h => h.Equals("Sku", StringComparison.OrdinalIgnoreCase)))
            {
                result.Format = "Customer,PO,Sku,Qty,UnitPrice";
                string missing = MissingColumn(header, "Customer", "PO", "Sku", "Qty", "UnitPrice");
                if (missing != null)
                    result.Skipped.Add($"header is missing column '{missing}' — nothing imported (layout {result.Format})");
                else
                    ImportLineRows(header, rows, customers, orders, owner, result);
            }
            else if (header.Any(h => h.Equals("Total", StringComparison.OrdinalIgnoreCase)))
            {
                result.Format = "Order,Customer,Owner,PO,Status,Total";
                string missing = MissingColumn(header, "Customer", "PO", "Status", "Total");
                if (missing != null)
                    result.Skipped.Add($"header is missing column '{missing}' — nothing imported (layout {result.Format})");
                else
                    ImportSummaryRows(header, rows, customers, orders, owner, result);
            }
            else
            {
                result.Format = "unknown";
                result.Skipped.Add($"header '{lines[0]}' is neither the line layout (…,Sku,Qty,UnitPrice) nor the export layout (…,Status,Total)");
            }
            return result;
        }

        // ── Customer,PO,Sku,Qty,UnitPrice ────────────────────────────────────────────────────────

        private static void ImportLineRows(List<string> header, List<string[]> rows, CustomerService customers, OrderService orders, string owner, ImportResult result)
        {
            int iCustomer = Index(header, "Customer"), iPo = Index(header, "PO"), iSku = Index(header, "Sku"),
                iQty = Index(header, "Qty"), iPrice = Index(header, "UnitPrice"), iDesc = Index(header, "Description");

            var groups = new Dictionary<string, Order>(StringComparer.OrdinalIgnoreCase);
            var sequence = new List<Order>();
            for (int r = 0; r < rows.Count; r++)
            {
                var cells = rows[r];
                int lineNo = r + 2;
                if (cells.Length <= Math.Max(Math.Max(iCustomer, iPo), Math.Max(iSku, Math.Max(iQty, iPrice))))
                {
                    result.Skipped.Add($"line {lineNo}: expected {header.Count} columns, found {cells.Length}");
                    continue;
                }

                string customerName = cells[iCustomer].Trim();
                var customer = customers.FindByName(customerName);
                if (customer == null)
                {
                    result.Skipped.Add($"line {lineNo}: unknown customer '{customerName}'");
                    continue;
                }
                if (!int.TryParse(cells[iQty].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int qty) || qty <= 0)
                {
                    result.Skipped.Add($"line {lineNo}: quantity '{cells[iQty]}' is not a positive integer");
                    continue;
                }
                if (!decimal.TryParse(cells[iPrice].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price) || price < 0)
                {
                    result.Skipped.Add($"line {lineNo}: unit price '{cells[iPrice]}' is not a number");
                    continue;
                }

                string po = cells[iPo].Trim();
                string key = customer.Id + "|" + po;
                if (!groups.TryGetValue(key, out var order))
                {
                    order = new Order
                    {
                        Customer = customer, CustomerId = customer.Id, PoNumber = po, Owner = owner ?? "",
                        Status = OrderStatus.Open, CreatedOn = DateTime.Today
                    };
                    groups[key] = order;
                    sequence.Add(order);
                }
                string sku = cells[iSku].Trim();
                order.Lines.Add(new OrderLine
                {
                    Sku = sku,
                    Description = iDesc >= 0 && iDesc < cells.Length && cells[iDesc].Trim().Length > 0 ? cells[iDesc].Trim() : DescribeSku(sku),
                    Quantity = qty,
                    UnitPrice = price
                });
            }

            foreach (var order in sequence)
                result.Saved.Add(orders.Save(order));       // ✓ CalculateOrderTotal + NextId — the reused business rule
        }

        // ── Order,Customer,Owner,PO,Status,Total ─────────────────────────────────────────────────

        private static void ImportSummaryRows(List<string> header, List<string[]> rows, CustomerService customers, OrderService orders, string owner, ImportResult result)
        {
            int iCustomer = Index(header, "Customer"), iOwner = Index(header, "Owner"), iPo = Index(header, "PO"),
                iStatus = Index(header, "Status"), iTotal = Index(header, "Total");

            for (int r = 0; r < rows.Count; r++)
            {
                var cells = rows[r];
                int lineNo = r + 2;
                if (cells.Length <= Math.Max(Math.Max(iCustomer, iPo), Math.Max(iStatus, iTotal)))
                {
                    result.Skipped.Add($"line {lineNo}: expected {header.Count} columns, found {cells.Length}");
                    continue;
                }
                var customer = customers.FindByName(cells[iCustomer].Trim());
                if (customer == null)
                {
                    result.Skipped.Add($"line {lineNo}: unknown customer '{cells[iCustomer]}'");
                    continue;
                }
                if (!decimal.TryParse(cells[iTotal].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal total) || total < 0)
                {
                    result.Skipped.Add($"line {lineNo}: total '{cells[iTotal]}' is not a number");
                    continue;
                }
                if (!Enum.TryParse(cells[iStatus].Trim(), ignoreCase: true, out OrderStatus status))
                    status = OrderStatus.Open;

                // The export layout carries totals, not lines: keep the total as one imported line so
                // OrderService.Save still owns the arithmetic. Save applies DiscountFor again, so a
                // customer with a contract discount (Tailspin Toys) round-trips LOWER than exported —
                // the export layout is a report, not a backup; the line layout is the lossless one.
                var order = new Order
                {
                    Customer = customer, CustomerId = customer.Id, PoNumber = cells[iPo].Trim(),
                    Owner = iOwner >= 0 && iOwner < cells.Length && cells[iOwner].Trim().Length > 0 ? cells[iOwner].Trim() : owner ?? "",
                    Status = status, CreatedOn = DateTime.Today
                };
                order.Lines.Add(new OrderLine { Sku = "IMPORT", Description = "Imported order total", Quantity = 1, UnitPrice = total });
                result.Saved.Add(orders.Save(order));
            }
        }

        // ── helpers ──────────────────────────────────────────────────────────────────────────────

        /// <summary>The first required column the header lacks, or null when all are present (a missing column is a skipped file, never an exception).</summary>
        private static string MissingColumn(List<string> header, params string[] required)
        {
            foreach (var name in required)
                if (Index(header, name) < 0) return name;
            return null;
        }

        private static int Index(List<string> header, string name) =>
            header.FindIndex(h => h.Equals(name, StringComparison.OrdinalIgnoreCase));

        private static string DescribeSku(string sku) => sku switch
        {
            "WJ-DEV-SEAT" => "Developer seat",
            "WJ-CTRL-STD" => "Control library seat",
            "WJ-ENT-SEAT" => "Enterprise seat",
            "WJ-SRV-CORE" => "Server core license",
            "WJ-SUP-GOLD" => "Gold support, 1 year",
            "WJ-SUP-PLAT" => "Platinum support",
            "WJ-TRN-DAY" => "On-site training day",
            "WJ-THEME-PK" => "Theme pack",
            _ => sku
        };

        /// <summary>RFC 4180-style split: commas inside double quotes are kept, "" is an escaped quote.</summary>
        internal static string[] Split(string line)
        {
            var cells = new List<string>();
            var cell = new StringBuilder();
            bool quoted = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (quoted)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"') { cell.Append('"'); i++; }
                        else quoted = false;
                    }
                    else cell.Append(c);
                }
                else if (c == '"') quoted = true;
                else if (c == ',') { cells.Add(cell.ToString()); cell.Clear(); }
                else cell.Append(c);
            }
            cells.Add(cell.ToString());
            return cells.ToArray();
        }
    }
}
