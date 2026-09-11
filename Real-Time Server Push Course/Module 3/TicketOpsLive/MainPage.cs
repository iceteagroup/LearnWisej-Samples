using System;
using System.Globalization;
using System.Threading;
using TicketOpsLive.Services;
using Wisej.Web;

namespace TicketOpsLive
{
    public partial class MainPage : Page
    {
        // Per-session state: every browser tab has its own MainPage and its own CancellationTokenSource.
        private CancellationTokenSource _cts;
        private volatile bool _importRunning;
        private int _pushers;
        private bool _polling;

        private const int TotalRecords = 200;
        private const int RecordMilliseconds = 25;
        private const int PushEvery = 10;
        private const int LogEvery = 50;

        public MainPage()
        {
            InitializeComponent();

            // If the page goes away, the import that belongs to it is cancelled too.
            this.Disposed += (s, e) => RequestCancel();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            Application.Title = "TicketOps Live";
        }

        #region Start / Fail at 87 / Cancel

        private void startImportButton_Click(object sender, EventArgs e)
        {
            StartImport(failAt87: false);
        }

        private void failAt87Button_Click(object sender, EventArgs e)
        {
            StartImport(failAt87: true);
        }

        private void StartImport(bool failAt87)
        {
            if (_importRunning)
                return;

            var job = new ImportJob(TotalRecords);
            _importRunning = true;

            startImportButton.Enabled = false;
            failAt87Button.Enabled = false;
            cancelImportButton.Enabled = true;
            importProgressBar.Value = 0;
            recordsImportedLabel.Text = $"0/{TotalRecords}";
            elapsedLabel.Text = "Elapsed 0.00 s";
            importStatusLabel.Text = "Starting import…";
            Log(job, "Import started");

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;
            BeginPush();

            Application.StartTask(() => RunImport(job, token, failAt87));
        }

        private void cancelImportButton_Click(object sender, EventArgs e)
        {
            cancelImportButton.Enabled = false;
            importStatusLabel.Text = "Cancelling…";
            RequestCancel();
        }

        private void RequestCancel()
        {
            try
            {
                _cts?.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // the job finished between the check and the call
            }
        }

        #endregion

        #region The import loop (runs on the task thread)

        private void RunImport(ImportJob job, CancellationToken token, bool failAt87)
        {
            try
            {
                for (int i = 1; i <= TotalRecords; i++)
                {
                    token.ThrowIfCancellationRequested();

                    Thread.Sleep(RecordMilliseconds);        // the simulated record

                    if (failAt87 && i == 87)
                        throw new InvalidOperationException($"Simulated malformed record #{i} (job {job.JobId}).");

                    job.RecordsImported = i;
                    recordsImportedLabel.Text = $"{i}/{TotalRecords}";
                    importProgressBar.Value = i / 2;
                    importStatusLabel.Text = $"Importing records… {i}/{TotalRecords}";
                    elapsedLabel.Text = "Elapsed " + job.ElapsedText;

                    if (i % LogEvery == 0)
                        Log(job, $"Imported {i} records");

                    // The controls change on every record; the browser is updated every 10th.
                    if (i % PushEvery == 0)
                        Application.Update(this);
                }

                job.Complete();
                importStatusLabel.Text = "Import completed successfully.";
                Log(job, "Import completed");
            }
            catch (OperationCanceledException)
            {
                job.Cancel();
                importStatusLabel.Text = "Import cancelled by user.";
                Log(job, "Cancelled at " + DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + $" after record {job.RecordsImported}");
            }
            catch (Exception ex)
            {
                job.Fail();
                LogError(job.JobId, ex);
                importStatusLabel.Text = "Import failed. Review the server log.";
                Log(job, "Failed at " + DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + $" on record {job.RecordsImported + 1}");
            }
            finally
            {
                var cts = _cts;
                _cts = null;
                cts?.Dispose();
                _importRunning = false;

                if (!this.IsDisposed)
                {
                    try
                    {
                        Application.Update(this, () =>
                        {
                            startImportButton.Enabled = true;
                            failAt87Button.Enabled = true;
                            cancelImportButton.Enabled = false;
                            elapsedLabel.Text = "Elapsed " + job.ElapsedText;
                            Log(job, job.Summary);
                            EndPush();
                        });
                    }
                    catch (ObjectDisposedException) { }
                }
            }
        }

        #endregion

        #region Delivery: WebSocket push, or polling while a task runs

        // IsWebSocket is false during Load, so polling is requested when the import starts without a
        // WebSocket and ended when it finishes.
        private void BeginPush()
        {
            _pushers++;
            if (Application.IsWebSocket || _polling)
                return;

            Application.StartPolling(1000);
            _polling = true;
        }

        private void EndPush()
        {
            if (_pushers > 0) _pushers--;
            if (_pushers > 0 || !_polling)
                return;

            Application.EndPolling();
            _polling = false;
        }

        #endregion

        #region Helpers

        private void Log(ImportJob job, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            importLogListBox.Items.Insert(0, $"{time}  [{job.JobId}] {message}");
            while (importLogListBox.Items.Count > 200)
                importLogListBox.Items.RemoveAt(importLogListBox.Items.Count - 1);
        }

        private static void LogError(string jobId, Exception ex)
        {
            Console.Error.WriteLine($"[TicketOpsLive] {DateTime.Now:HH:mm:ss.fff} job {jobId} import failed for session {Application.SessionId}: {ex}");
        }

        #endregion
    }
}
