using System.Collections.Generic;

namespace OrderDesk.Files
{
    /// <summary>The five answers of the lesson's classification exercise.</summary>
    public enum BoundaryClass
    {
        /// <summary>The server owns the file: a configured root, a database, a blob store.</summary>
        ServerStorage,
        /// <summary>A client file must reach the server → Upload control.</summary>
        Upload,
        /// <summary>A server-generated file must reach the user → Application.Download.</summary>
        Download,
        /// <summary>The app must work with the user's files directly and the platform permits it.</summary>
        ClientFileSystem,
        /// <summary>No mapping exists — the feature is redesigned (Office Automation, local printer).</summary>
        Redesign
    }

    /// <summary>One file / Office / printer touch found in LegacyOrderDesk and its verdict.</summary>
    public sealed class FileBoundary
    {
        public string Feature { get; set; }        // where in LegacyOrderDesk
        public string LegacyApi { get; set; }      // the call and the path it assumed
        public BoundaryClass Class { get; set; }
        public string Replacement { get; set; }    // the web-safe pattern
        public string Evidence { get; set; }       // what the Module 6 console shows for it

        public string ClassText => TextOf(Class);

        public static string TextOf(BoundaryClass value) => value switch
        {
            BoundaryClass.ServerStorage => "Server storage",
            BoundaryClass.Upload => "Upload",
            BoundaryClass.Download => "Download",
            BoundaryClass.ClientFileSystem => "ClientFileSystem",
            _ => "Redesign"
        };
    }

    /// <summary>
    /// Deliverable 1 of the lab as data: every path, Office object and printer LegacyOrderDesk
    /// touches, classified. The same rows are in docs/FileBoundaryClassification.md.
    /// </summary>
    public static class FileBoundaryClassifier
    {
        public static readonly IReadOnlyList<FileBoundary> Items = new List<FileBoundary>
        {
            new FileBoundary
            {
                Feature = "Import orders",
                LegacyApi = @"File.ReadAllLines(@""C:\Orders\in.csv"")",
                Class = BoundaryClass.Upload,
                Replacement = "Wisej.Web.Upload (.csv, ≤ 1 MB) → StorageRoot.Uploads → OrdersCsvImporter → OrderService.Save",
                Evidence = "Card B: Upload order-batch.csv succeeds; \"Legacy import\" throws DirectoryNotFoundException on the server."
            },
            new FileBoundary
            {
                Feature = "Export to Excel",
                LegacyApi = @"Excel.Application (Interop) → workbook.SaveAs(@""C:\Orders\out.xlsx"")",
                Class = BoundaryClass.Download,
                Replacement = "Managed writer (XlsxWriter, no Office) → StorageRoot.Exports → Application.Download(path, \"Orders.xlsx\")",
                Evidence = "Card C: Export .xlsx streams a real workbook; \"Legacy Excel Interop\" only probes the ProgID and explains KB 257757."
            },
            new FileBoundary
            {
                Feature = "Office Automation itself",
                LegacyApi = "Activator.CreateInstance(Type.GetTypeFromProgID(\"Excel.Application\"))",
                Class = BoundaryClass.Redesign,
                Replacement = "Never on the server: EPPlus / Open XML SDK / Aspose / Syncfusion / DevExpress — here a hand-written .xlsx writer",
                Evidence = "Legacy/ExcelExport.cs is kept ✕ and never called; the console never creates the COM object."
            },
            new FileBoundary
            {
                Feature = "Print Invoice",
                LegacyApi = "PrintDocument + PrintPreviewDialog (System.Drawing.Printing)",
                Class = BoundaryClass.Redesign,
                Replacement = "InvoiceDocument lines → InvoicePdfWriter on the server → PdfViewer (InvoicePreviewForm) or Download",
                Evidence = "Card C: Print Invoice → PDF opens Invoice-1042.pdf; \"Legacy print\" explains the printer is the server's."
            },
            new FileBoundary
            {
                Feature = "Attach file",
                LegacyApi = @"OpenFileDialog → File.Copy(…, @""C:\Orders\Attachments\<order>"")",
                Class = BoundaryClass.Upload,
                Replacement = "Upload control → StorageRoot.Uploads/<order>/ (or a database blob); the server never sees the user's disk",
                Evidence = "Same Upload control as the import; the storage root is App_Data/uploads by configuration."
            },
            new FileBoundary
            {
                Feature = "Open an attachment",
                LegacyApi = "Process.Start(path)  — the default app on the user's PC",
                Class = BoundaryClass.ClientFileSystem,
                Replacement = "Application.Download (the browser opens it) — direct client file access only where the platform permits it",
                Evidence = "Every result in Card D has a Download button; nothing is opened on the server."
            },
            new FileBoundary
            {
                Feature = "Settings · Export folder",
                LegacyApi = @"HKCU\Software\LegacyOrderDesk\ExportFolder = C:\Orders",
                Class = BoundaryClass.ServerStorage,
                Replacement = "Web.config OrderDesk.StorageRoot resolved with Path.Combine(Application.StartupPath, …); the user never picks a server folder",
                Evidence = "Load trace: storage root = <project>/App_Data · uploads/ exports/ reports/ created on demand."
            },
            new FileBoundary
            {
                Feature = "Long reports (invoice batch)",
                LegacyApi = "for-each order → PrintDocument, UI frozen until the last page",
                Class = BoundaryClass.ServerStorage,
                Replacement = "ReportQueue job → PDF written to StorageRoot.Reports; pages poll status, View ▸ / Download ⬇ when Done",
                Evidence = "Card D: Invoice batch × 1,204 orders shows progress; a second session sees the same queue."
            },
            new FileBoundary
            {
                Feature = "Application log",
                LegacyApi = @"File.AppendAllText(%APPDATA%\LegacyOrderDesk\app.log)",
                Class = BoundaryClass.ServerStorage,
                Replacement = "Host logging (ILogger / console / files under a configured server folder) — one log for all sessions, tagged with the session id",
                Evidence = "Not exercised in the console; the TracePanel plays the per-session log role in the labs."
            },
        };
    }
}
