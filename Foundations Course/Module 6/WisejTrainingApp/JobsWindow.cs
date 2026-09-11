using System;
using System.Threading;
using System.Threading.Tasks;
using Wisej.Web;

namespace WisejTrainingApp
{
    public partial class JobsWindow : Form
    {
        // Per-user state: every user session gets its own JobsWindow, so this instance field belongs
        // to one user. Never make it static — then every user would share (and cancel) the same job.
        private CancellationTokenSource currentJob;

        public JobsWindow()
        {
            InitializeComponent();
            SetJobRunning(false);
        }

        private async void btnStartImport_Click(object sender, EventArgs e)
        {
            try
            {
                SetJobRunning(true);
                await RunImportJobAsync();
                lblStatus.Text = "Import completed successfully.";
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "Import cancelled.";
            }
            catch (Exception ex)
            {
                LogError(ex);
                lblStatus.Text = "Import failed. Please try again or contact support.";
            }
            finally
            {
                SetJobRunning(false);
            }
        }

        private async void btnStartExport_Click(object sender, EventArgs e)
        {
            try
            {
                SetJobRunning(true);
                await RunExportJobAsync();
                lblStatus.Text = "Export completed successfully.";
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "Export cancelled.";
            }
            catch (Exception ex)
            {
                LogError(ex);
                lblStatus.Text = "Export failed. Please try again or contact support.";
            }
            finally
            {
                SetJobRunning(false);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            currentJob?.Cancel();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
        }

        private Task RunImportJobAsync()
        {
            return RunJobAsync("Import", 5);
        }

        private Task RunExportJobAsync()
        {
            return RunJobAsync("Export", 5);
        }

        private async Task RunJobAsync(string jobName, int steps)
        {
            currentJob = new CancellationTokenSource();
            progressBar.Value = 0;
            AddLog(jobName + " started.");

            for (int step = 1; step <= steps; step++)
            {
                // The "long work" — awaiting keeps the page responsive.
                await Task.Delay(700, currentJob.Token);

                if (chkSimulateError.Checked && step == 3)
                    throw new InvalidOperationException($"{jobName} failed at step {step}: row 42 has an invalid date.");

                progressBar.Value = step * 100 / steps;
                lblStatus.Text = $"{jobName}: step {step} of {steps}";
                AddLog($"{jobName}: step {step} of {steps} done.");

                // Push the progress to the browser now, not only when the job ends.
                Application.Update(this);
            }
        }

        // Disable Start while a job runs so the user can't start a second one.
        private void SetJobRunning(bool running)
        {
            btnStartImport.Enabled = !running;
            btnStartExport.Enabled = !running;
            btnCancel.Enabled = running;

            if (!running)
                currentJob = null;

            // The job finishes in the background, after the click request ended — push the final
            // status and button state to the browser so the page doesn't stay "running".
            Application.Update(this);
        }

        // The technical detail goes to the log for developers; the user only sees a short, safe message.
        private void LogError(Exception ex)
        {
            AddLog("ERROR: " + ex.GetType().Name + " - " + ex.Message);
        }

        private void AddLog(string message)
        {
            lstLog.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " - " + message);
        }
    }
}
