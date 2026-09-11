using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OrderDesk.Domain;
using OrderDesk.Files;
using OrderDesk.Reporting;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// OrderDesk documents: orders are imported through Upload into the configured storage root,
    /// invoices are rendered as PDFs on the server and shown in a PdfViewer, the order list is
    /// exported as .xlsx and downloaded, and long reports run in a process-wide queue that every
    /// session sees.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly OrderService _orderService = new OrderService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly ReportService _reportService;
        private IList<Order> _orders = new List<Order>();
        private string _queueSignature;

        public MainPage()
        {
            _reportService = new ReportService(_orderService);
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            // No login in this module: the first session acts as kelly, every later one as sam,
            // so the queue's Owner column shows who queued what (per session, never a static).
            if (SessionUser == null)
                SessionUser = Application.SessionCount <= 1 ? "kelly" : "sam";

            BindOrders();
            RefreshQueue(force: true);
            timerQueue.Start();
        }

        #region Import: Upload → storage root → importer

        private void upload_Uploaded(object sender, UploadedEventArgs e)
        {
            for (int i = 0; i < e.Files.Count; i++)
            {
                var file = e.Files[i];

                // AllowedFileTypes is a browser-side filter; the server checks again.
                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    Ui.Toast($"{file.FileName}: only .csv files can be imported.", MessageBoxIcon.Warning);
                    continue;
                }

                string path;
                try
                {
                    path = StorageRoot.UploadPath(file.FileName);      // sanitized leaf name under the configured root
                    using (var target = File.Create(path))
                        file.InputStream.CopyTo(target);
                }
                catch (Exception ex) when (ex is ArgumentException || ex is IOException)
                {
                    Ui.Toast($"{file.FileName} could not be stored: {ex.Message}", MessageBoxIcon.Warning);
                    continue;
                }

                ImportResult result;
                using (var stream = File.OpenRead(path))
                    result = OrdersCsvImporter.Import(stream, _customerService, _orderService, SessionUser);

                BindOrders();
                labelImportResult.Text = $"{file.FileName} → {result.Saved.Count} order(s) saved, {result.Skipped.Count} skipped";
                if (result.Saved.Count > 0)
                    Ui.Toast($"{result.Saved.Count} order(s) imported from {file.FileName}.");
                else
                    Ui.Toast($"Nothing imported from {file.FileName}: {(result.Skipped.Count > 0 ? result.Skipped[0] : "no data rows")}", MessageBoxIcon.Warning);
            }
        }

        private void upload_Error(object sender, UploadErrorEventArgs e)
        {
            string names = e.FileNames == null ? "" : string.Join(", ", e.FileNames);
            Ui.Toast($"Upload refused ({e.ErrorType}): {names}. {e.Message}", MessageBoxIcon.Warning);
        }

        #endregion

        #region Print Invoice (PDF) and Export (.xlsx)

        private void BindOrders()
        {
            _orders = _orderService.GetOrders();
            comboOrders.Items.Clear();
            foreach (var order in _orders)
                comboOrders.Items.Add($"{order.Id} · {order.CustomerName} · {order.Total:N2} · {order.Status}");
            if (comboOrders.Items.Count > 0)
                comboOrders.SelectedIndex = 0;
        }

        private Order CurrentOrder =>
            comboOrders.SelectedIndex >= 0 && comboOrders.SelectedIndex < _orders.Count ? _orders[comboOrders.SelectedIndex] : null;

        private void buttonPrintPdf_Click(object sender, EventArgs e)
        {
            var order = CurrentOrder;
            if (order == null) return;

            // Was PrintDocument → the local printer; now a PDF built on the server, shown in a PdfViewer.
            var pdf = _reportService.CreateInvoicePdf(order);
            var preview = new InvoicePreviewForm(pdf, $"Invoice-{order.Id}.pdf");
            preview.ShowDialog((form, result) => form.Dispose());
        }

        private void buttonExportXlsx_Click(object sender, EventArgs e)
        {
            // Was Excel Interop + C:\Orders\out.xlsx; now a managed .xlsx under the storage root, downloaded.
            var rows = _orderService.Search(new OrderFilter());
            var path = _reportService.CreateOrdersWorkbook(rows);
            Application.Download(path, "Orders.xlsx");
        }

        #endregion

        #region Report queue: one process, every session

        private ReportJob SelectedJob => gridJobs.CurrentRow?.Tag as ReportJob;

        private void buttonQueueBatch_Click(object sender, EventArgs e) =>
            Enqueue("Invoice batch × 1,204 orders", "Invoice-batch-1204.pdf", ReportBuilders.InvoiceBatch(_orders, 1204, _orderService));

        private void buttonQueueStatement_Click(object sender, EventArgs e) =>
            Enqueue("Monthly statement", "Monthly-statement.pdf", ReportBuilders.MonthlyStatement(_orders, _orderService));

        private void buttonQueueSummary_Click(object sender, EventArgs e) =>
            Enqueue("Q2 summary", "Q2-summary.pdf", ReportBuilders.QuarterSummary(_orders, _orderService));

        private void Enqueue(string name, string fileName, Func<Action<int>, System.Threading.CancellationToken, byte[]> work)
        {
            var job = ReportQueue.Enqueue(name, SessionUser, fileName, StorageRoot.Reports, work);
            RefreshQueue(force: true, selectId: job.Id);
        }

        private void buttonCancelJob_Click(object sender, EventArgs e)
        {
            var job = SelectedJob;
            if (job == null) return;
            ReportQueue.Cancel(job.Id);
            RefreshQueue(force: true, selectId: job.Id);
        }

        private void buttonViewResult_Click(object sender, EventArgs e)
        {
            var job = SelectedJob;
            if (!HasResult(job)) return;
            var preview = new InvoicePreviewForm(File.ReadAllBytes(job.ResultPath), job.ResultFileName);
            preview.ShowDialog((form, result) => form.Dispose());
        }

        private void buttonDownloadResult_Click(object sender, EventArgs e)
        {
            var job = SelectedJob;
            if (!HasResult(job)) return;
            Application.Download(job.ResultPath, job.ResultFileName);
        }

        private static bool HasResult(ReportJob job) =>
            job != null && job.Status == ReportJobStatus.Done && job.ResultPath != null && File.Exists(job.ResultPath);

        private void buttonSecondSession_Click(object sender, EventArgs e)
        {
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
                if (job.Id == keep) select = index;
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

        private static string SessionUser
        {
            get { dynamic session = Application.Session; return session.User as string; }
            set { dynamic session = Application.Session; session.User = value; }
        }
    }
}
