using System;
using OrderDesk.Domain;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// Copied from LegacyOrderDesk/Legacy/InvoicePrinter.cs and reduced to a same-shaped stub.
    /// Desktop assumption: "Print Invoice" draws on a System.Drawing.Printing.PrintDocument and sends
    /// it to the printer attached to THIS machine. The web project also targets plain net10.0 (Linux
    /// containers), where System.Drawing.Printing does not exist at all — so the PrintDocument code
    /// cannot even compile there. On Windows servers it compiles and then fails at run time: the
    /// service account has no default printer and no interactive desktop. Module 6 generates a PDF
    /// on the server instead (Services/PdfWriter) and shows it in a PdfViewer or offers it as a download.
    /// </summary>
    public sealed class InvoicePrinter
    {
        private readonly Order _order;

        public InvoicePrinter(Order order) { _order = order ?? throw new ArgumentNullException(nameof(order)); }

        /// <summary>What the desktop code passed to PrintDocument.DocumentName.</summary>
        public string DocumentName => "Invoice " + _order.Id;

        /// <summary>
        /// The original body, kept for the record (it does not compile on net10.0):
        /// <code>
        /// var doc = new PrintDocument { DocumentName = "Invoice " + _order.Id };
        /// doc.PrintPage += (s, e) => { e.Graphics.DrawString("LegacyOrderDesk — Invoice " + _order.Id, ...); ... };
        /// doc.Print();   // ✕ goes to the default printer of the machine running the code
        /// </code>
        /// </summary>
        public void Print()
        {
            // ✕ On the server "the default printer" is the server's — usually none, never the user's.
            throw new InvalidOperationException("No printer is installed on the server (System.Drawing.Printing is a Windows desktop API). " +
                                                DocumentName + " cannot be printed from a web server process; generate a PDF and hand it to the browser instead.");
        }
    }
}
