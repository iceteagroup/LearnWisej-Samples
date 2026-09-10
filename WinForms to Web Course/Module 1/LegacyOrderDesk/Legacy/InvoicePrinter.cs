using System.Drawing;
using System.Drawing.Printing;
using OrderDesk.Domain;

namespace LegacyOrderDesk
{
    /// <summary>
    /// Desktop assumption: "Print Invoice" draws on a PrintDocument and sends it to the printer
    /// attached to THIS machine. On a server there is no user printer; Module 6 generates a PDF
    /// instead and shows it in a PdfViewer or offers it as a download.
    /// </summary>
    public sealed class InvoicePrinter
    {
        private readonly Order _order;

        public InvoicePrinter(Order order) { _order = order; }

        public void Print()
        {
            var doc = new PrintDocument { DocumentName = "Invoice " + _order.Id };
            doc.PrintPage += (s, e) =>
            {
                var font = new Font("Segoe UI", 11f);
                float y = e.MarginBounds.Top;
                e.Graphics.DrawString("LegacyOrderDesk — Invoice " + _order.Id, new Font("Segoe UI", 16f, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left, y); y += 40;
                e.Graphics.DrawString("Customer: " + _order.CustomerName, font, Brushes.Black, e.MarginBounds.Left, y); y += 24;
                e.Graphics.DrawString("PO: " + _order.PoNumber, font, Brushes.Black, e.MarginBounds.Left, y); y += 32;
                foreach (var line in _order.Lines)
                {
                    e.Graphics.DrawString(line.Quantity + " × " + line.Description + "  @ " + line.UnitPrice.ToString("C") + "  = " + line.LineTotal.ToString("C"), font, Brushes.Black, e.MarginBounds.Left, y);
                    y += 22;
                }
                y += 12;
                e.Graphics.DrawString("Total: " + _order.Total.ToString("C"), new Font("Segoe UI", 12f, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left, y);
            };
            doc.Print();   // ✕ goes to the default printer of the machine running the code
        }
    }
}
