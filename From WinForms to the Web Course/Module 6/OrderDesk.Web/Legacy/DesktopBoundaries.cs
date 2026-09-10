using System;
using System.IO;

namespace OrderDesk.Legacy
{
    /// <summary>
    /// The desktop file / printer assumptions of LegacyOrderDesk, kept here so the Module 6 console
    /// can run the ones that still compile and show them failing on the server, and quote the ones
    /// that do not even compile for net10.0 (System.Drawing.Printing, System.Windows.Forms).
    ///
    /// Every member is ✕: nothing in this file is part of the migrated path.
    /// </summary>
    public static class DesktopBoundaries
    {
        /// <summary>The path the desktop "Import orders" read: the user's C: drive.</summary>
        public const string LegacyImportPath = @"C:\Orders\in.csv";           // ✕ a Windows path on the USER's machine

        /// <summary>The path OrdersForm.exportButton_Click wrote through Excel Interop.</summary>
        public const string LegacyExportPath = @"C:\Orders\out.xlsx";         // ✕ same disk, same problem

        /// <summary>The folder OrdersForm.attachButton_Click copied attachments into.</summary>
        public const string LegacyAttachmentsRoot = @"C:\Orders\Attachments"; // ✕ per-PC folder, invisible to every other user

        /// <summary>
        /// ✕ "Import orders" the desktop way: read C:\Orders\in.csv. On the developer's PC the file
        /// is there; on the server the same code opens the SERVER's C: drive (or, on Linux, a
        /// relative path literally named "C:\Orders\in.csv" — backslashes are not separators there).
        /// The console calls this on purpose and catches the exception.
        /// </summary>
        public static string[] LocalFileImport()
        {
            return File.ReadAllLines(LegacyImportPath);                        // ✕ DirectoryNotFound / FileNotFound on the server
        }

        /// <summary>
        /// ✕ Probes the desktop export's first line — Type.GetTypeFromProgID("Excel.Application") —
        /// WITHOUT creating the COM object. Returns true when Office is registered on this machine,
        /// false when the ProgID is missing, and null when the platform has no COM at all (Linux).
        /// Either way the conclusion is the same: the server must not automate Excel.
        /// </summary>
        public static bool? ExcelProgIdInstalled()
        {
            if (!OperatingSystem.IsWindows())
                return null;                                                   // no registry, no COM, no Excel.Application
            try
            {
                return Type.GetTypeFromProgID("Excel.Application") != null;    // ✕ the check ExcelExport.ExportOrders makes
            }
            catch (PlatformNotSupportedException)
            {
                return null;
            }
        }

        // ── Documented excerpt: LegacyOrderDesk/Reporting/InvoicePrinter.cs ───────────────────────
        //
        // Quoted instead of copied because System.Drawing.Printing and System.Windows.Forms do not
        // exist for net10.0 (Linux/Kestrel) — the file cannot even be compiled into the web app.
        //
        //   public static void Print(Order order, OrderService service)
        //   {
        //       var lines = InvoiceDocument.Build(order, service);            // ✓ content rule — reused by the web app
        //       var document = new PrintDocument { DocumentName = $"Invoice-{order.Id}" };
        //       document.PrintPage += (s, e) => Draw(e, lines);                // ✕ PrintDocument → the SERVER's default printer
        //
        //       using (var preview = new PrintPreviewDialog { Document = document, Width = 800, Height = 600 })
        //           preview.ShowDialog();                                     // ✕ a WinForms window on the server's desktop, blocking the request
        //   }
        //
        //   private static void Draw(PrintPageEventArgs e, IList<string> lines)
        //   {
        //       using (var font = new Font("Consolas", 10f))                 // ✕ GDI+ font on the server, not the user's PC
        //       {
        //           float y = e.MarginBounds.Top;
        //           foreach (var line in lines)
        //           {
        //               e.Graphics.DrawString(line, font, Brushes.Black, e.MarginBounds.Left, y);
        //               y += font.GetHeight(e.Graphics) + 2;
        //           }
        //       }
        //   }
        //
        // Web replacement (Reporting/ReportService.CreateInvoicePdf): the same InvoiceDocument lines →
        // InvoicePdfWriter (server PDF bytes) → InvoicePreviewForm (PdfViewer) or Application.Download.
        //
        // ── Documented excerpt: LegacyOrderDesk/OrdersForm.attachButton_Click ────────────────────────
        //
        //   using (var open = new OpenFileDialog { Title = "Attach a file to the order" })   // ✕ a dialog on the server's desktop
        //   {
        //       if (open.ShowDialog(this) != DialogResult.OK || CurrentOrder == null) return;
        //       var folder = Path.Combine(@"C:\Orders\Attachments", CurrentOrder.Id.ToString());
        //       Directory.CreateDirectory(folder);                                          // ✕ the SERVER's C: drive
        //       File.Copy(open.FileName, Path.Combine(folder, Path.GetFileName(open.FileName)), overwrite: true);
        //   }
        //
        // Web replacement: Wisej.Web.Upload → Files/StorageRoot.Uploads (a configured server root).
    }
}
