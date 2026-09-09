using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using OrderDesk.Domain;

namespace LegacyOrderDesk.Reporting
{
    /// <summary>
    /// Prints an invoice to a printer attached to this PC. The content comes from the shared
    /// InvoiceDocument rule; only the delivery (PrintDocument → local printer) is desktop-bound.
    /// Module 6 keeps the content rule and replaces the delivery with a server-generated PDF.
    /// </summary>
    public static class InvoicePrinter
    {
        public static void Print(Order order, OrderService service)
        {
            var lines = InvoiceDocument.Build(order, service);
            var document = new PrintDocument { DocumentName = $"Invoice-{order.Id}" };
            document.PrintPage += (s, e) => Draw(e, lines);

            using (var preview = new PrintPreviewDialog { Document = document, Width = 800, Height = 600 })
                preview.ShowDialog();
        }

        private static void Draw(PrintPageEventArgs e, IList<string> lines)
        {
            using (var font = new Font("Consolas", 10f))
            {
                float y = e.MarginBounds.Top;
                foreach (var line in lines)
                {
                    e.Graphics.DrawString(line, font, Brushes.Black, e.MarginBounds.Left, y);
                    y += font.GetHeight(e.Graphics) + 2;
                }
            }
        }
    }
}
