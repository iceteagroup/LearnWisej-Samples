using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OrderDesk.Domain;
using OrderDesk.Services;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 6 — Files, Reports &amp; Browser Boundaries. The Documents screen of OrderDesk.Web:
    /// the three desktop assumptions of LegacyOrderDesk (local file paths, Excel Interop, a local
    /// printer) fail on the server on purpose, and their web-safe replacements run next to them —
    /// Upload into a configured storage root, a managed .xlsx writer + Application.Download, a
    /// server-generated PDF in a PdfViewer, and a queued report worker that pushes progress.
    /// </summary>
    public partial class MainPage : Page
    {
        private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en-US");

        private readonly OrderService _service = new OrderService();
        private readonly CsvImportService _import;
        private readonly List<int> _importedIds = new List<int>();     // this session's imports (shown under the five)
        private string _sessionTag = "--------";
        private byte[] _pdf;
        private string _pdfName;

        public MainPage()
        {
            InitializeComponent();
            _import = new CsvImportService(_service);
            ReportQueue.Changed += OnQueueChanged;
            this.Disposed += (s, e) => ReportQueue.Changed -= OnQueueChanged;
        }

        // ── startup ──────────────────────────────────────────────────────────────

        private void MainPage_Load(object sender, EventArgs e)
        {
            _sessionTag = (Application.SessionId ?? "--------").Substring(0, 8);
            trace.Server("Program.Main", "Application.MainPage = new MainPage()  · session " + _sessionTag + " · sessions " + Application.SessionCount);
            trace.Server("AppConfig (Web.config)", AppConfig.StorageRootKey + " = " + AppConfig.StorageRootSetting + "  · " + DocumentStorage.Display(AppConfig.WebConfigPath));

            var root = DocumentStorage.Root;
            trace.Server("DocumentStorage.Root", root);
            trace.Finding("C:\\Orders → storage root", "Path.Combine(app folder, \"" + AppConfig.StorageRootSetting + "\") · created on demand · '" + Path.DirectorySeparatorChar + "' separator on this OS");
            storageLabel.Text = "staged under " + DocumentStorage.Display(DocumentStorage.Imports) + "/ (Web.config " + AppConfig.StorageRootKey + ")";

            var sample = DocumentStorage.EnsureSampleImport();
            trace.Server("sample-import.csv", DocumentStorage.Display(sample) + " · 5 rows · download it, then upload it");

            BindOrders();
            RefreshJobs();
            trace.Ok("Ready", "5 orders · Upload a CSV, or click a button below (✕ = desktop assumption on the server, ✓ = web-safe replacement)");
        }

        // ── orders grid ──────────────────────────────────────────────────────────

        /// <summary>The five walkthrough orders first, then this session's imported rows.</summary>
        private List<Order> VisibleOrders()
        {
            var five = _service.GetAll().Where(o => o.Id >= 1038 && o.Id <= 1042).OrderByDescending(o => o.Id).ToList();
            foreach (var id in _importedIds)
            {
                var o = _service.Find(id);
                if (o != null && five.All(x => x.Id != o.Id)) five.Add(o);
            }
            return five;
        }

        private void BindOrders()
        {
            var rows = VisibleOrders();
            ordersGrid.DataSource = null;
            ordersGrid.DataSource = rows;
            if (ordersGrid.Rows.Count > 0 && ordersGrid.SelectedRows.Count == 0)
                ordersGrid.Rows[0].Selected = true;
            trace.Server("OrderService.GetAll()", rows.Count + " rows bound · " + string.Join(", ", rows.Take(5).Select(o => o.Id)) + (rows.Count > 5 ? " + " + (rows.Count - 5) + " imported" : ""));
        }

        private Order SelectedOrder
        {
            get
            {
                var row = ordersGrid.CurrentRow ?? (ordersGrid.SelectedRows.Count > 0 ? ordersGrid.SelectedRows[0] : null);
                return row?.DataBoundItem as Order ?? VisibleOrders().FirstOrDefault();
            }
        }

        private void ordersGrid_SelectionChanged(object sender, EventArgs e)
        {
            var o = SelectedOrder;
            if (o != null) trace.In("ordersGrid.SelectionChanged", "order " + o.Id + " · " + o.CustomerName + " · " + o.Total.ToString("C", En));
        }

        // ── status / banner ──────────────────────────────────────────────────────

        private void SetStatus(string state)
        {
            statusLabel.Text = "● " + state;
            statusLabel.ForeColor = state == "alarm" ? Palette.Bad : state == "working" ? Palette.Accent : Palette.MutedText;
        }

        private void Banner(string text, bool bad)
        {
            bannerLabel.Text = text;
            bannerLabel.BackColor = bad ? Palette.BadSoft : Palette.GoodSoft;
            bannerLabel.ForeColor = bad ? Palette.Bad : Palette.Good;
            bannerLabel.ToolTipText = text;
            bannerLabel.Visible = true;
        }

        // ── Download sample CSV ──────────────────────────────────────────────────

        private void downloadSampleButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "Download sample CSV");
            var path = DocumentStorage.EnsureSampleImport();
            var bytes = File.ReadAllBytes(path);
            trace.Server("File.ReadAllBytes", DocumentStorage.Display(path) + " · " + bytes.Length + " bytes (server storage: a template we own)");
            Application.Download(new MemoryStream(bytes), "order-batch.csv");
            trace.Out("Application.Download", "order-batch.csv → browser · now pick it in \"Upload CSV…\"");
            Banner("✓ order-batch.csv downloaded — upload it with the Upload button to run the import", false);
            SetStatus("idle");
        }

        // ── Import (C:\Orders) ✕ ─────────────────────────────────────────────────

        private void importLegacyButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "Import (C:\\Orders) ✕");
            SetStatus("working");
            try
            {
                trace.Server("LocalExport.ReadCsv", "File.ReadAllText(@\"C:\\Orders\\in.csv\")  ← the desktop code, unchanged");
                var csv = Legacy.LocalExport.ReadCsv();
                trace.Ok("LocalExport.ReadCsv", csv.Length + " characters");   // never reached on a server
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is InvalidOperationException || ex is IOException || ex is UnauthorizedAccessException)
            {
                trace.Fail("File.ReadAllText", ex.GetType().Name + ": " + ex.Message);
                trace.Finding("local read → upload", "server code reads the SERVER's disk (" + Environment.MachineName + " as " + Environment.UserName + "); the user's C: drive is reachable only through Upload");
                Banner("✖ C:\\Orders\\in.csv was looked up on the SERVER (" + Environment.MachineName + "), not on the user's PC — the browser's files arrive only through Upload", true);
                SetStatus("alarm");
            }
        }

        // ── Upload → import ──────────────────────────────────────────────────────

        private void upload_Uploading(object sender, UploadingEventArgs e)
        {
            trace.In("Upload.Uploading", string.Join(", ", e.FileNames ?? new string[0]) + " · " + Sum(e.FileSizes) + " bytes");
            SetStatus("working");
        }

        private void upload_Progress(object sender, UploadProgressEventArgs e)
        {
            trace.In("Upload.Progress", e.Loaded + " / " + e.Total + " bytes");
        }

        private void upload_Error(object sender, UploadErrorEventArgs e)
        {
            trace.Fail("Upload.Error", e.ErrorType + " · " + e.Message + " · " + string.Join(", ", e.FileNames ?? new string[0]));
            Banner("✖ upload rejected: " + e.Message, true);
            SetStatus("alarm");
        }

        private void upload_Uploaded(object sender, UploadedEventArgs e)
        {
            SetStatus("working");
            importResults.Items.Clear();
            int totalSaved = 0, totalRejected = 0;
            string lastName = "upload";
            for (int i = 0; i < e.Files.Count; i++)
            {
                var file = e.Files[i];
                var name = DocumentStorage.SafeFileName(file.FileName);
                lastName = name;
                trace.In("Upload.Uploaded", name + " · " + file.ContentLength + " bytes · " + file.ContentType);

                byte[] bytes;
                using (var ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }

                // 1. stage the user's file under the configured root (never a user path, never C:\)
                var dest = Path.Combine(DocumentStorage.Imports, DocumentStorage.TimeStamp() + "-" + name);
                File.WriteAllBytes(dest, bytes);
                trace.Server("File.WriteAllBytes", DocumentStorage.Display(dest) + " · " + bytes.Length + " bytes");
                trace.Finding("upload + configured root", "user file → Upload → Path.Combine(storage root, \"imports\", name) → processed on the server");

                // 2. process it with the reused business logic
                var text = Encoding.UTF8.GetString(bytes);
                var result = _import.Import(text);
                trace.Server("CsvImportService.Import", result.Rows.Count + " data rows · columns Order,Customer,Owner,Total,Status,Date (the LocalExport.ToCsv contract)");
                foreach (var row in result.Rows)
                {
                    importResults.Items.Add(row.ToString());
                    if (row.Saved) trace.Ok("OrderService.Save", "row " + row.Line + " · order " + row.OrderId + " · " + row.Message);
                    else trace.Fail("OrderValidator / lookup", "row " + row.Line + " · " + row.Message);
                }
                foreach (var id in result.SavedIds)
                    if (!_importedIds.Contains(id)) _importedIds.Add(id);
                totalSaved += result.SavedCount;
                totalRejected += result.RejectedCount;
            }

            importSummary.Text = lastName + ": " + totalSaved + " saved · " + totalRejected + " rejected";
            importSummary.ForeColor = totalRejected > 0 ? Palette.Warn : Palette.Good;
            BindOrders();
            Banner("✓ " + lastName + " imported on the server: " + totalSaved + " saved · " + totalRejected + " rejected (unknown customer / missing owner — listed in the Import card)", false);
            Notify.Saved(lastName + " imported: " + totalSaved + " saved, " + totalRejected + " rejected.");
            trace.Out("Toast", "\"" + lastName + " imported: " + totalSaved + " saved, " + totalRejected + " rejected.\"");
            SetStatus("idle");
        }

        private static long Sum(long[] sizes) => sizes == null ? 0 : sizes.Sum();

        // ── Export (Excel Interop) ✕ ─────────────────────────────────────────────

        private void exportInteropButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "Export (Excel Interop) ✕");
            SetStatus("working");
            try
            {
                trace.Server("ExcelExport.ExportToExcel", "new Excel.Application()  ← Microsoft.Office.Interop.Excel shape, on the SERVER");
                var path = Legacy.ExcelExport.ExportToExcel(VisibleOrders());
                trace.Ok("ExcelExport", path);                                  // never reached on a server
            }
            catch (COMException ex)
            {
                trace.Fail("Excel.Application (COM)", "HRESULT 0x" + ex.HResult.ToString("X8") + " · " + ex.Message);
                trace.Finding("Office Interop → managed writer", "Office needs an interactive desktop + user profile; unattended on a server it fails or deadlocks (unsupported by Microsoft). Replace with a spreadsheet library + Download.");
                Banner("✖ Excel Interop failed on the server (0x" + ex.HResult.ToString("X8") + " Excel.Application) — Office Automation is not a server strategy", true);
                SetStatus("alarm");
            }
            catch (Exception ex)
            {
                trace.Fail("ExcelExport", ex.GetType().Name + ": " + ex.Message);
                Banner("✖ Excel export failed on the server: " + ex.Message, true);
                SetStatus("alarm");
            }
        }

        // ── Export (managed .xlsx) ✓ ─────────────────────────────────────────────

        private void exportXlsxButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "Export (managed .xlsx) ✓");
            SetStatus("working");
            var rows = VisibleOrders();
            var bytes = XlsxWriter.OrdersWorkbook(rows);
            trace.Server("XlsxWriter.OrdersWorkbook", rows.Count + " rows · " + bytes.Length + " bytes · ZipArchive parts: " + string.Join(", ", XlsxWriter.Parts));
            var copy = Path.Combine(DocumentStorage.Reports, "orders.xlsx");
            File.WriteAllBytes(copy, bytes);
            trace.Server("File.WriteAllBytes", DocumentStorage.Display(copy) + " (server export, kept under the storage root)");
            Application.Download(new MemoryStream(bytes), "orders.xlsx");
            trace.Out("Application.Download", "orders.xlsx → browser (Content-Disposition attachment)");
            trace.Finding("Excel Interop → XlsxWriter + Download", "same rows, no Office on the server; the download crosses the browser boundary, the file never touches a user path");
            Banner("✓ orders.xlsx generated on the server (managed OpenXML writer) and downloaded — open it in Excel or LibreOffice", false);
            SetStatus("idle");
        }

        // ── Print invoice ✕ ─────────────────────────────────────────────────────

        private void printInvoiceButton_Click(object sender, EventArgs e)
        {
            var order = SelectedOrder;
            trace.In("click", "Print invoice ✕ · order " + order.Id);
            SetStatus("working");
            try
            {
                var printer = new Legacy.InvoicePrinter(order);
                trace.Server("InvoicePrinter.Print", "PrintDocument \"" + printer.DocumentName + "\" → default printer of the machine running the code");
                printer.Print();
                trace.Ok("InvoicePrinter", "printed");                          // never reached on a server
            }
            catch (InvalidOperationException ex)
            {
                trace.Fail("PrintDocument.Print", ex.Message);
                trace.Finding("PrintDocument → server PDF", "the server has no user printer (and System.Drawing.Printing does not exist on net10.0 Linux); render a PDF and show it in PdfViewer / Download");
                Banner("✖ Invoice " + order.Id + ": no printer on the server — PrintDocument is a desktop API; use Invoice PDF ✓", true);
                SetStatus("alarm");
            }
        }

        // ── Invoice PDF ✓ ────────────────────────────────────────────────────────

        private void invoicePdfButton_Click(object sender, EventArgs e)
        {
            var order = SelectedOrder;
            trace.In("click", "Invoice PDF ✓ · order " + order.Id);
            SetStatus("working");

            _pdf = PdfWriter.Invoice(order, _sessionTag);
            _pdfName = "Invoice-" + order.Id + ".pdf";
            trace.Server("PdfWriter.Invoice", _pdfName + " · " + _pdf.Length + " bytes · PDF 1.4 · 1 page · Helvetica · " + order.Lines.Count + " line(s) · total " + order.Total.ToString("C", En));

            var path = Path.Combine(DocumentStorage.Reports, _pdfName);
            File.WriteAllBytes(path, _pdf);
            trace.Server("File.WriteAllBytes", DocumentStorage.Display(path));

            pdfViewer.FileName = _pdfName;
            pdfViewer.PdfStream = new MemoryStream(_pdf);
            pdfPlaceholder.Visible = false;
            pdfViewer.Visible = true;
            pdfTitle.Text = "PdfViewer  ·  " + _pdfName;
            pdfDownloadButton.Visible = true;
            trace.Out("PdfViewer.PdfStream", _pdfName + " → browser PDF viewer (ViewerType.Auto)");
            trace.Finding("printed report → server PDF", "generated for any session on any OS; shown in PdfViewer, downloadable, stored under " + DocumentStorage.Display(DocumentStorage.Reports) + "/");
            Banner("✓ " + _pdfName + " rendered on the server and shown in the PdfViewer — ⬇ Download PDF saves it", false);
            SetStatus("idle");
        }

        private void pdfDownloadButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "⬇ Download PDF");
            if (_pdf == null) return;
            Application.Download(new MemoryStream(_pdf), _pdfName);
            trace.Out("Application.Download", _pdfName + " → browser");
        }

        // ── Queue report (all invoices) ──────────────────────────────────────────

        private void queueReportButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "Queue report (all invoices)");
            SetStatus("working");
            var five = VisibleOrders().Take(5).ToList();
            foreach (var o in five)
            {
                var job = ReportQueue.Enqueue(o.Id, _sessionTag);
                trace.Server("ReportQueue.Enqueue", "job #" + job.Id + " · " + job.FileName + " · Queued · session " + _sessionTag);
            }
            trace.Server("ReportQueue worker", ReportQueue.IsWorkerRunning ? "one worker (Application.StartTask) is draining the queue" : "worker starting");
            trace.Finding("long reports → ReportQueue", "one worker serializes the engine; every status change is pushed to the page with Application.Update — the UI never freezes");
            Banner("● " + five.Count + " invoice jobs queued — watch Queued → Running → Done in the Report queue card", false);
            RefreshJobs();
        }

        /// <summary>Runs on the worker thread for every status change of every job (all sessions).</summary>
        private void OnQueueChanged(ReportJob job)
        {
            if (this.IsDisposed) return;
            Application.Update(this, () =>
            {
                if (this.IsDisposed) return;
                RefreshJobs();
                if (job.Session == _sessionTag)
                {
                    if (job.Status == ReportJobStatus.Running)
                        trace.Out("Application.Update", "job #" + job.Id + " · " + job.FileName + " · Running");
                    else if (job.Status == ReportJobStatus.Done)
                        trace.Out("Application.Update", "job #" + job.Id + " · " + job.FileName + " · Done → " + DocumentStorage.Display(job.Path) + " (" + job.Bytes + " bytes)");
                    else if (job.Status == ReportJobStatus.Failed)
                        trace.Fail("ReportQueue job #" + job.Id, job.Error);
                }
                else
                {
                    trace.Server("ReportQueue.Changed", "job #" + job.Id + " from session " + job.Session + " · " + job.StatusText + " (another session shares the worker)");
                }

                if (ReportQueue.PendingCount == 0 && !ReportQueue.IsWorkerRunning || job.Status != ReportJobStatus.Running && ReportQueue.PendingCount == 0)
                {
                    var mine = ReportQueue.Snapshot().Where(j => j.Session == _sessionTag).ToList();
                    if (mine.Count > 0 && mine.All(j => j.Status == ReportJobStatus.Done || j.Status == ReportJobStatus.Failed))
                    {
                        Banner("✓ " + mine.Count(j => j.Status == ReportJobStatus.Done) + " invoice PDFs written under " + DocumentStorage.Display(DocumentStorage.Reports) + "/ by the queue worker", false);
                        SetStatus("idle");
                    }
                }
            });
        }

        private sealed class JobRow
        {
            public string Job { get; set; }
            public string File { get; set; }
            public string Status { get; set; }
            public string Session { get; set; }
        }

        private void RefreshJobs()
        {
            var jobs = ReportQueue.Snapshot(12);
            jobsGrid.DataSource = null;
            jobsGrid.DataSource = jobs.Select(j => new JobRow { Job = "#" + j.Id, File = j.FileName, Status = j.StatusText, Session = j.Session }).ToList();
            var all = ReportQueue.Snapshot(1000);
            queueProgress.Maximum = Math.Max(1, all.Count);
            queueProgress.Value = Math.Min(queueProgress.Maximum, all.Count(j => j.Status == ReportJobStatus.Done || j.Status == ReportJobStatus.Failed));
            queueLabel.Text = ReportQueue.Summary() + "  ·  files: " + DocumentStorage.Display(DocumentStorage.Reports) + "/";
        }

        // ── Show storage root ────────────────────────────────────────────────────

        private void storageRootButton_Click(object sender, EventArgs e)
        {
            trace.In("click", "Show storage root");
            trace.Server("AppConfig.AppRoot", AppConfig.AppRoot);
            trace.Server("Web.config", AppConfig.StorageRootKey + " = \"" + AppConfig.StorageRootSetting + "\"  → " + DocumentStorage.Root);
            trace.Server("Web.config", "connectionStrings[OrderDesk] = " + (AppConfig.ConnectionString("OrderDesk") ?? "(none)"));
            trace.Server("Environment", Environment.MachineName + " · " + Environment.UserName + " · " + RuntimeInformation.OSDescription + " · separator '" + Path.DirectorySeparatorChar + "'");
            trace.Finding("Path.Combine, not C:\\", "Linux deployments: forward slashes and case-sensitive names — one configured root, every path combined from it");
            var files = DocumentStorage.ListFiles();
            trace.Server("Directory.EnumerateFiles", files.Count + " file(s) under " + DocumentStorage.Display(DocumentStorage.Root) + "/");
            foreach (var f in files.Take(12)) trace.Server("  file", f);
            if (files.Count > 12) trace.Server("  …", (files.Count - 12) + " more");
            Banner("storage root = " + DocumentStorage.Root + " · " + files.Count + " file(s) (imports/, reports/, sample-import.csv)", false);
            SetStatus("idle");
        }
    }
}
