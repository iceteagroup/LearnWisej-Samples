using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using OrderDesk.Domain;
using OrderDesk.Files;
using OrderDesk.Reporting;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 6 · Files, Reports and Browser Boundaries.
    ///
    /// Left, card A:  every file / Office / printer touch of LegacyOrderDesk, classified
    ///                (server storage · upload · download · ClientFileSystem · redesign).
    /// Left, card B:  the import workflow — C:\Orders\in.csv becomes Upload → storage root → importer;
    ///                the legacy read is executed on purpose and fails on the server.
    /// Left, card C:  export and print — Excel Interop becomes a managed .xlsx + Download,
    ///                PrintDocument becomes a server PDF in a PdfViewer; both legacy paths explained.
    /// Right:         the migration log, and the process-wide report queue every session shares
    ///                (progress, cancel, view, download — open a second session to see the same jobs).
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly OrderService _orderService = new OrderService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly ReportService _reportService;
        private readonly Dictionary<int, ReportJobStatus> _seenJobStatus = new Dictionary<int, ReportJobStatus>();
        private BoundaryClass? _classFilter;
        private IList<Order> _orders = new List<Order>();
        private string _queueSignature;

        public MainPage()
        {
            _reportService = new ReportService(_orderService);
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            trace.Add(TraceKind.Server, "startup", "Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()");
            trace.Add(TraceKind.Server, "session", $"new browser session {Short(Application.SessionId)} · {Application.SessionCount} session(s) in this process");

            // No login in this module: the first session acts as kelly, every later one as sam,
            // so the queue's Owner column shows who queued what (per session, never a static).
            if (SessionUser == null)
                SessionUser = Application.SessionCount <= 1 ? "kelly" : "sam";
            trace.Add(TraceKind.Server, "Application.Session.User", $"= {SessionUser}  (this session only)");

            // ✓ the configured storage root replaces every C:\Orders literal
            trace.Add(TraceKind.Boundary, "storage root", $"C:\\Orders\\…  ⇒  {StorageRoot.Describe()}");
            trace.Add(TraceKind.Server, "StorageRoot", $"uploads/ exports/ reports/ under {Relative(StorageRoot.Root)} · OS {RuntimeInformation.OSDescription}");

            BindBoundaries();
            BindOrders();
            ShowUploadDetail(null, null);
            RefreshQueue(force: true);
            timerQueue.Start();
            trace.Add(TraceKind.Server, "Wisej.Web.Timer", "polling ReportQueue every 1 s (redraws only when a job changed)");
            Ui.SetStatus(labelStatus, $"storage root ready · {SessionUser} · queue polling", Ui.Ok);
        }

        #region Card A · File boundary classification (deliverable 1)

        private void BindBoundaries()
        {
            var rows = FileBoundaryClassifier.Items
                .Where(i => _classFilter == null || i.Class == _classFilter.Value)
                .ToList();

            gridBoundaries.Rows.Clear();
            foreach (var item in rows)
            {
                int index = gridBoundaries.Rows.Add(item.Feature, item.LegacyApi, item.ClassText, item.Replacement);
                gridBoundaries.Rows[index].Tag = item;
                gridBoundaries.Rows[index].Cells[2].Style.ForeColor = ClassColor(item.Class);
                gridBoundaries.Rows[index].Cells[2].Style.Font = Ui.SmallBold;
            }

            if (gridBoundaries.Rows.Count > 0)
            {
                gridBoundaries.Rows[0].Selected = true;
                ShowBoundaryDetail(gridBoundaries.Rows[0].Tag as FileBoundary);
            }
            else
            {
                ShowBoundaryDetail(null);
            }
        }

        private static System.Drawing.Color ClassColor(BoundaryClass value) => value switch
        {
            BoundaryClass.ServerStorage => Ui.Ok,
            BoundaryClass.Upload => Ui.Accent,
            BoundaryClass.Download => Ui.Purple,
            BoundaryClass.ClientFileSystem => Ui.Muted,
            _ => Ui.Warn
        };

        private void gridBoundaries_SelectionChanged(object sender, EventArgs e) =>
            ShowBoundaryDetail(gridBoundaries.CurrentRow?.Tag as FileBoundary);

        private void ShowBoundaryDetail(FileBoundary item)
        {
            if (item == null)
            {
                labelBoundaryDetail.Text = "";
                return;
            }
            int total = FileBoundaryClassifier.Items.Count;
            string count = _classFilter == null ? $"{total} paths" : $"{gridBoundaries.Rows.Count} of {total} · {FileBoundary.TextOf(_classFilter.Value)}";
            labelBoundaryDetail.Text = $"{count} · {item.Feature}: {item.LegacyApi}\n→ {item.Replacement}\nEvidence: {item.Evidence}";
        }

        private void buttonClassAll_Click(object sender, EventArgs e) => ApplyClassFilter(null);
        private void buttonClassServer_Click(object sender, EventArgs e) => ApplyClassFilter(BoundaryClass.ServerStorage);
        private void buttonClassUpload_Click(object sender, EventArgs e) => ApplyClassFilter(BoundaryClass.Upload);
        private void buttonClassDownload_Click(object sender, EventArgs e) => ApplyClassFilter(BoundaryClass.Download);
        private void buttonClassClient_Click(object sender, EventArgs e) => ApplyClassFilter(BoundaryClass.ClientFileSystem);
        private void buttonClassRedesign_Click(object sender, EventArgs e) => ApplyClassFilter(BoundaryClass.Redesign);

        private void ApplyClassFilter(BoundaryClass? value)
        {
            _classFilter = value;
            BindBoundaries();
            trace.Add(TraceKind.FromClient, "boundaries.filter", $"class = {(value == null ? "all" : FileBoundary.TextOf(value.Value))} → {gridBoundaries.Rows.Count} rows");
        }

        #endregion

        #region Card B · Import: upload + server storage (was C:\Orders\in.csv)

        private void upload_Uploaded(object sender, UploadedEventArgs e)
        {
            Ui.HideBanner(labelBanner);
            for (int i = 0; i < e.Files.Count; i++)
            {
                var file = e.Files[i];
                trace.Add(TraceKind.FromClient, "Upload.Uploaded", $"{file.FileName} · {file.ContentLength:N0} bytes · {file.ContentType}");

                // AllowedFileTypes is a browser-side filter; the server checks again.
                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    Ui.ShowBanner(labelBanner, $"✕ {file.FileName}: only .csv files are imported. The browser filter (AllowedFileTypes) is a convenience, the server decides.", Ui.BannerKind.Error);
                    Ui.SetStatus(labelStatus, "upload rejected", Ui.Error);
                    continue;
                }

                string path;
                try
                {
                    path = StorageRoot.UploadPath(file.FileName);      // ✓ sanitized leaf name under the configured root
                }
                catch (ArgumentException ex)
                {
                    Ui.ShowBanner(labelBanner, "✕ " + ex.Message, Ui.BannerKind.Error);
                    continue;
                }

                try
                {
                    using (var target = File.Create(path))
                        file.InputStream.CopyTo(target);               // ✓ client file → server storage
                }
                catch (IOException ex)
                {
                    // Two sessions uploading the same name at the same moment collide on the sanitized leaf name.
                    trace.Add(TraceKind.Boundary, "Upload collision", $"{ex.GetType().Name}: {ex.Message}");
                    Ui.ShowBanner(labelBanner, $"✕ {file.FileName} could not be stored: {ex.Message} — another session is writing the same name right now; a production import prefixes the stored name with the session or a timestamp (the export path already does).", Ui.BannerKind.Error);
                    Ui.SetStatus(labelStatus, "upload collided on the server", Ui.Error);
                    continue;
                }
                trace.Add(TraceKind.Boundary, "Import orders", $"File.ReadAllLines(C:\\Orders\\in.csv)  ⇒  Upload → {Relative(path)}");

                ImportResult result;
                using (var stream = File.OpenRead(path))
                    result = OrdersCsvImporter.Import(stream, _customerService, _orderService, SessionUser);
                trace.Add(TraceKind.Server, "OrdersCsvImporter.Import", $"layout {result.Format} · {result.RowCount} data row(s) → {result.Saved.Count} order(s), {result.Skipped.Count} skipped");
                foreach (var order in result.Saved)
                    trace.Add(TraceKind.Server, "OrderService.Save", $"order {order.Id} · {order.CustomerName} · PO {order.PoNumber} · {order.Lines.Count} line(s) · total {order.Total:N2}");
                foreach (var skipped in result.Skipped)
                    trace.Add(TraceKind.Server, "import.skipped", skipped);

                BindOrders();
                ShowUploadDetail(file.FileName, result);
                if (result.Saved.Count > 0)
                {
                    Ui.Toast($"{result.Saved.Count} order(s) imported from {file.FileName} — the file never had a C:\\ path.");
                    Ui.SetStatus(labelStatus, $"{result.Saved.Count} order(s) imported through Upload", Ui.Ok);
                }
                else
                {
                    Ui.ShowBanner(labelBanner, $"Nothing imported from {file.FileName}: {(result.Skipped.Count > 0 ? result.Skipped[0] : "no data rows")}. Download the sample CSV to see the accepted layout.", Ui.BannerKind.Warn);
                    Ui.SetStatus(labelStatus, "upload processed · no orders saved", Ui.Warn);
                }
            }
        }

        private void upload_Error(object sender, UploadErrorEventArgs e)
        {
            string names = e.FileNames == null ? "" : string.Join(", ", e.FileNames);
            trace.Add(TraceKind.FromClient, "Upload.Error", $"{e.ErrorType} · {names} · {e.Message}");
            Ui.ShowBanner(labelBanner, $"✕ Upload refused ({e.ErrorType}): {names} — {e.Message}. MaxFileSize is 1 MB; the limit is enforced before a byte reaches the importer.", Ui.BannerKind.Error);
            Ui.SetStatus(labelStatus, "upload refused", Ui.Error);
        }

        private void buttonSampleCsv_Click(object sender, EventArgs e)
        {
            // ✓ a file the server owns → the browser saves it (Download); re-upload it to run the import.
            string path = Path.Combine(Application.StartupPath, "wwwroot", "samples", "order-batch.csv");
            trace.Add(TraceKind.ToClient, "Application.Download", $"{Relative(path)} → order-batch.csv (8 rows, layout Customer,PO,Sku,Qty,UnitPrice)");
            Application.Download(path, "order-batch.csv");
            Ui.SetStatus(labelStatus, "sample CSV sent to the browser — upload it back", Ui.Ok);
        }

        private void buttonLegacyImport_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "legacy import", $"File.ReadAllLines(@\"{Legacy.DesktopBoundaries.LegacyImportPath}\") — as the desktop app did");
            try
            {
                var lines = Legacy.DesktopBoundaries.LocalFileImport();      // ✕ reads the SERVER's disk
                trace.Add(TraceKind.Boundary, "✕ legacy import", $"{lines.Length} line(s) read from {Legacy.DesktopBoundaries.LegacyImportPath} — on the SERVER's disk");
                Ui.ShowBanner(labelBanner, $"✕ {Legacy.DesktopBoundaries.LegacyImportPath} exists here only because the server IS your PC today. Deployed, the same call reads the server's C: drive: every user would import the same file, and nobody could put theirs there. Use the Upload control.", Ui.BannerKind.Warn);
                Ui.SetStatus(labelStatus, "legacy import read the server's disk", Ui.Warn);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // DirectoryNotFoundException on Windows; FileNotFoundException on Linux (the path is a relative name there).
                trace.Add(TraceKind.Boundary, "✕ legacy import", $"{ex.GetType().Name}: {ex.Message}");
                Ui.ShowBanner(labelBanner, $"✕ Legacy import failed with {ex.GetType().Name}: the path is on the user's PC; the server has no such disk. File.ReadAllLines runs where the code runs — on the server. Web replacement: Upload → {Relative(StorageRoot.Uploads)}{Path.DirectorySeparatorChar}… → OrdersCsvImporter.", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "desktop path assumption broke on the server", Ui.Error);
            }
        }

        private void ShowUploadDetail(string fileName, ImportResult result)
        {
            string root = Relative(StorageRoot.Root);
            string last = result == null
                ? "last import   —  (download the sample CSV, then upload it)"
                : $"last import   {fileName} → {result.Saved.Count} order(s) saved, {result.Skipped.Count} skipped · {string.Join(", ", result.Saved.Select(o => o.Id))}";
            labelUploadDetail.Text =
                $"storage root  {root}{Path.DirectorySeparatorChar}  (Web.config OrderDesk.StorageRoot = \"{StorageRoot.Configured}\")\n" +
                $"uploads       {Relative(StorageRoot.Uploads)}{Path.DirectorySeparatorChar}  ·  {Directory.EnumerateFiles(StorageRoot.Uploads).Count()} file(s)\n" +
                last;
        }

        #endregion

        #region Card C · Export and print: managed writers, Download, PdfViewer

        private void BindOrders()
        {
            _orders = _orderService.GetOrders();                       // ✓ business logic reused as-is
            comboOrders.Items.Clear();
            foreach (var order in _orders)
                comboOrders.Items.Add($"{order.Id} · {order.CustomerName} · {order.Total:N2} · {order.Status}");
            if (comboOrders.Items.Count > 0)
                comboOrders.SelectedIndex = 0;
            trace.Add(TraceKind.Server, "OrderService.GetOrders", $"{_orders.Count} orders in the shared repository");
        }

        private Order CurrentOrder =>
            comboOrders.SelectedIndex >= 0 && comboOrders.SelectedIndex < _orders.Count ? _orders[comboOrders.SelectedIndex] : null;

        private void buttonExportXlsx_Click(object sender, EventArgs e)
        {
            // The lesson's snippet, line for line: rows → reportService → Application.Download(path, …).
            var rows = _orderService.Search(new OrderFilter());
            var path = _reportService.CreateOrdersWorkbook(rows);
            long size = new FileInfo(path).Length;
            trace.Add(TraceKind.Boundary, "Export to Excel", $"Excel.Application + C:\\Orders\\out.xlsx  ⇒  XlsxWriter → {Relative(path)} ({size:N0} bytes)");
            Application.Download(path, "Orders.xlsx");
            trace.Add(TraceKind.ToClient, "Application.Download", $"Orders.xlsx ({rows.Count} rows) — no Excel.exe, no COM, no dialog, any number of sessions at once");
            Ui.HideBanner(labelBanner);
            Ui.SetStatus(labelStatus, "Orders.xlsx generated on the server and streamed to the browser", Ui.Ok);
        }

        private void buttonExportCsv_Click(object sender, EventArgs e)
        {
            // The Module 1 recovery is still valid — and its output is a layout the importer accepts (round-trip test).
            var stream = CsvExport.OrdersStream(_orders);
            trace.Add(TraceKind.ToClient, "Application.Download", $"orders.csv ({stream.Length:N0} bytes, layout Order,Customer,Owner,PO,Status,Total) — upload it back to test the second layout");
            Application.Download(stream, "orders.csv");
            Ui.SetStatus(labelStatus, "orders.csv streamed to the browser", Ui.Ok);
        }

        private void buttonLegacyExcel_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "legacy Excel export", "probe Type.GetTypeFromProgID(\"Excel.Application\") — the COM object is NOT created");
            bool? installed = Legacy.DesktopBoundaries.ExcelProgIdInstalled();
            if (installed == true)
            {
                trace.Add(TraceKind.Boundary, "✕ Excel Interop", "ProgID found: Office is installed on THIS machine — unsupported from a server process (KB 257757)");
                Ui.ShowBanner(labelBanner, "✕ Excel is installed on THIS machine, but automating it from a server process is unsupported (Microsoft KB 257757): one interactive Excel per export, no concurrency, hangs on dialogs, runs as the service account. The console did not create the COM object. Migrated path: Export .xlsx (XlsxWriter + Application.Download).", Ui.BannerKind.Warn);
                Ui.SetStatus(labelStatus, "Office present, Office Automation still not a server strategy", Ui.Warn);
            }
            else if (installed == false)
            {
                trace.Add(TraceKind.Boundary, "✕ Excel Interop", "ProgID missing → ExcelExport.ExportOrders throws InvalidOperationException before writing a byte");
                Ui.ShowBanner(labelBanner, "✕ ProgID Excel.Application not found → the desktop export throws InvalidOperationException (\"Excel is not installed on this machine\"). This is the real server case: a web server has no Office. Migrated path: Export .xlsx (XlsxWriter + Application.Download).", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "desktop export cannot run here", Ui.Error);
            }
            else
            {
                trace.Add(TraceKind.Boundary, "✕ Excel Interop", $"{RuntimeInformation.OSDescription}: no COM, no registry — Type.GetTypeFromProgID cannot be asked");
                Ui.ShowBanner(labelBanner, $"✕ {RuntimeInformation.OSDescription}: no COM, no registry, no Excel.Application — the desktop export cannot even ask for the ProgID. Migrated path: Export .xlsx (XlsxWriter + Application.Download).", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "no Office Automation on this platform", Ui.Error);
            }
        }

        private void buttonPrintPdf_Click(object sender, EventArgs e)
        {
            var order = CurrentOrder;
            if (order == null) return;

            // Desktop: InvoicePrinter.Print → PrintDocument → the printer on the user's desk.
            // Web:     same InvoiceDocument lines → PDF built on the server → PdfViewer / download.
            var pdf = _reportService.CreateInvoicePdf(order);
            trace.Add(TraceKind.Boundary, "Print Invoice", $"PrintDocument → local printer  ⇒  server PDF ({pdf.Length:N0} bytes) → PdfViewer");
            trace.Add(TraceKind.ToClient, "InvoicePreviewForm", $"Invoice-{order.Id}.pdf  (PdfViewer.PdfStream, modal)");

            var preview = new InvoicePreviewForm(pdf, $"Invoice-{order.Id}.pdf");
            preview.ShowDialog((form, result) =>
            {
                form.Dispose();                                        // the caller disposes a transient dialog
                trace.Add(TraceKind.Server, "InvoicePreviewForm", "closed and disposed");
            });
            Ui.HideBanner(labelBanner);
            Ui.SetStatus(labelStatus, $"invoice {order.Id} rendered on the server", Ui.Ok);
        }

        private void buttonLegacyPrint_Click(object sender, EventArgs e)
        {
            var row = FileBoundaryClassifier.Items.First(i => i.Feature == "Print Invoice");
            trace.Add(TraceKind.FromClient, "legacy print", "InvoicePrinter.Print(order) — PrintDocument + PrintPreviewDialog");
            trace.Add(TraceKind.Boundary, "✕ Print Invoice", $"{row.LegacyApi} → verdict {row.ClassText}: {row.Replacement}");
            Ui.ShowBanner(labelBanner, $"✕ Legacy print: PrintDocument targets the printer attached to the SERVER (a web server has none) and PrintPreviewDialog is a WinForms window on the server's desktop that would block this request. System.Drawing.Printing does not even compile for net10.0 — the code is quoted in Legacy/DesktopBoundaries.cs. Verdict {row.ClassText} → {row.Replacement}.", Ui.BannerKind.Error);
            Ui.SetStatus(labelStatus, "printer assumption explained — use Print Invoice → PDF", Ui.Error);
        }

        #endregion

        #region Card D · Report queue: one process, every session

        private ReportJob SelectedJob => gridJobs.CurrentRow?.Tag as ReportJob;

        private void buttonQueueBatch_Click(object sender, EventArgs e) =>
            Enqueue("Invoice batch × 1,204 orders", "Invoice-batch-1204.pdf", ReportBuilders.InvoiceBatch(_orders, 1204, _orderService),
                $"one page per order, ~{ReportBuilders.MillisecondsPerInvoice} ms each ≈ {1204 * ReportBuilders.MillisecondsPerInvoice / 1000} s");

        private void buttonQueueStatement_Click(object sender, EventArgs e) =>
            Enqueue("Monthly statement", "Monthly-statement.pdf", ReportBuilders.MonthlyStatement(_orders, _orderService), "≈ 15 s of simulated rendering");

        private void buttonQueueSummary_Click(object sender, EventArgs e) =>
            Enqueue("Q2 summary", "Q2-summary.pdf", ReportBuilders.QuarterSummary(_orders, _orderService), "≈ 3 s — Done first, try View ▸ and Download ⬇");

        private void Enqueue(string name, string fileName, Func<Action<int>, System.Threading.CancellationToken, byte[]> work, string note)
        {
            trace.Add(TraceKind.FromClient, "queue report", $"{name} · {note}");
            var job = ReportQueue.Enqueue(name, SessionUser, fileName, StorageRoot.Reports, work);   // ✓ process-wide queue, per-session owner
            trace.Add(TraceKind.Server, "ReportQueue.Enqueue", $"job {job.Id} · owner {job.Owner} · result → {Relative(StorageRoot.Reports)}{Path.DirectorySeparatorChar}{job.Id:D4}-{fileName}");
            _seenJobStatus[job.Id] = job.Status;
            RefreshQueue(force: true, selectId: job.Id);
            Ui.HideBanner(labelBanner);
            Ui.SetStatus(labelStatus, $"job {job.Id} queued · the page keeps responding while the worker renders", Ui.Ok);
        }

        private void buttonCancelJob_Click(object sender, EventArgs e)
        {
            var job = SelectedJob;
            if (job == null) return;
            bool cancelled = ReportQueue.Cancel(job.Id);
            trace.Add(TraceKind.FromClient, "ReportQueue.Cancel", $"job {job.Id} ({job.Name}, {job.Status}) → {(cancelled ? "cancellation requested" : "nothing to cancel")}");
            if (!cancelled)
                Ui.ShowBanner(labelBanner, $"Job {job.Id} is already {job.Status} — only Queued or Running jobs can be cancelled.", Ui.BannerKind.Warn);
            RefreshQueue(force: true, selectId: job.Id);
        }

        private void buttonViewResult_Click(object sender, EventArgs e)
        {
            var job = SelectedJob;
            if (job == null || job.Status != ReportJobStatus.Done || job.ResultPath == null || !File.Exists(job.ResultPath))
            {
                Ui.ShowBanner(labelBanner, job == null ? "Select a job first." : $"Job {job.Id} has no result yet ({job.Status}).", Ui.BannerKind.Warn);
                return;
            }
            var pdf = File.ReadAllBytes(job.ResultPath);                // ✓ server storage → PdfViewer
            trace.Add(TraceKind.ToClient, "InvoicePreviewForm", $"{job.ResultFileName} ({pdf.Length:N0} bytes) from {Relative(job.ResultPath)}");
            var preview = new InvoicePreviewForm(pdf, job.ResultFileName);
            preview.ShowDialog((form, result) =>
            {
                form.Dispose();
                trace.Add(TraceKind.Server, "InvoicePreviewForm", "closed and disposed");
            });
            Ui.HideBanner(labelBanner);
        }

        private void buttonDownloadResult_Click(object sender, EventArgs e)
        {
            var job = SelectedJob;
            if (job == null || job.Status != ReportJobStatus.Done || job.ResultPath == null || !File.Exists(job.ResultPath))
            {
                Ui.ShowBanner(labelBanner, job == null ? "Select a job first." : $"Job {job.Id} has no result yet ({job.Status}).", Ui.BannerKind.Warn);
                return;
            }
            trace.Add(TraceKind.ToClient, "Application.Download", $"{Relative(job.ResultPath)} → {job.ResultFileName}");
            Application.Download(job.ResultPath, job.ResultFileName);   // ✓ generated server file → the user
            Ui.HideBanner(labelBanner);
        }

        private void buttonSecondSession_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.ToClient, "Application.Navigate", "same URL, target _blank → a second browser session sees the same queue");
            Application.Navigate(Application.Url, "_blank");
        }

        private void timerQueue_Tick(object sender, EventArgs e) => RefreshQueue(force: false);

        private void gridJobs_SelectionChanged(object sender, EventArgs e) => ShowJobDetail();

        /// <summary>Re-reads the shared queue; redraws only when a job changed (or when forced).</summary>
        private void RefreshQueue(bool force, int? selectId = null)
        {
            if (IsDisposed) return;                                    // the Timer can tick after the page is gone
            string signature = ReportQueue.Signature();
            if (!force && signature == _queueSignature) return;
            _queueSignature = signature;

            int keep = selectId ?? SelectedJob?.Id ?? -1;
            var jobs = ReportQueue.Snapshot();
            gridJobs.Rows.Clear();
            int select = 0;
            foreach (var job in jobs)
            {
                int index = gridJobs.Rows.Add(job.Id, job.Name, job.Owner, job.Status.ToString(), job.ProgressText, job.ResultText);
                gridJobs.Rows[index].Tag = job;
                gridJobs.Rows[index].Cells[3].Style.ForeColor = StatusColor(job.Status);
                gridJobs.Rows[index].Cells[3].Style.Font = Ui.SmallBold;
                if (job.Id == keep) select = index;

                // Log every status transition once per session — including jobs another session queued.
                if (!_seenJobStatus.TryGetValue(job.Id, out var seen) || seen != job.Status)
                {
                    _seenJobStatus[job.Id] = job.Status;
                    trace.Add(TraceKind.Server, $"job {job.Id} ({job.Owner})", $"{job.Name} → {job.Status}{(job.Status == ReportJobStatus.Done ? " · " + job.ResultFileName : "")}{(job.Status == ReportJobStatus.Failed ? " · " + job.Error : "")}");
                }
            }
            if (gridJobs.Rows.Count > 0)
                gridJobs.Rows[select].Selected = true;

            labelQueueCounts.Text = ReportQueue.Counts().ToString();
            ShowJobDetail();
        }

        private void ShowJobDetail()
        {
            var job = SelectedJob;
            progressJob.Value = job?.Progress ?? 0;
            buttonCancelJob.Enabled = job != null && job.IsActive;
            buttonViewResult.Enabled = job != null && job.Status == ReportJobStatus.Done;
            buttonDownloadResult.Enabled = buttonViewResult.Enabled;
        }

        private static System.Drawing.Color StatusColor(ReportJobStatus status) => status switch
        {
            ReportJobStatus.Done => Ui.Ok,
            ReportJobStatus.Running => Ui.Accent,
            ReportJobStatus.Failed => Ui.Error,
            ReportJobStatus.Cancelled => Ui.Warn,
            _ => Ui.Muted
        };

        #endregion

        private void buttonClear_Click(object sender, EventArgs e) => trace.Clear();

        private static string SessionUser
        {
            get { dynamic session = Application.Session; return session.User as string; }
            set { dynamic session = Application.Session; session.User = value; }
        }

        /// <summary>A path relative to the project folder, for the trace (Linux-safe: Path does the separators).</summary>
        private static string Relative(string path) => Path.GetRelativePath(Application.StartupPath, path);

        private static string Short(string id) => string.IsNullOrEmpty(id) ? "?" : (id.Length > 8 ? id.Substring(0, 8) : id);
    }
}
