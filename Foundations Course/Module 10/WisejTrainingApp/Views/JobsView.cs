using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Jobs: one small background job (Module 7) — "build the weekly ticket digest". It walks the tickets
    /// in ten steps with await Task.Delay, pushes each step to the browser with Application.Update(this),
    /// can be cancelled through a per-session CancellationTokenSource, and shows what a safe error looks
    /// like: the user sees a short message, the job log keeps the detail (LogError).
    ///
    /// try / catch / finally — SetJobRunning(false) in finally, so the buttons are never left disabled.
    /// </summary>
    public partial class JobsView : HelpdeskView
    {
        private readonly TicketService ticketService;

        // Instance field: one token source per user session. Never static.
        private CancellationTokenSource jobCancellation;

        public JobsView(IHelpdeskShell shell, TicketService ticketService)
            : base(shell)
        {
            InitializeComponent();
            this.ticketService = ticketService;
        }

        private async void btnStartJob_Click(object sender, EventArgs e)
        {
            jobCancellation = new CancellationTokenSource();
            CancellationToken token = jobCancellation.Token;

            SetJobRunning(true);
            lstJobLog.Items.Clear();
            LogJob("Job started: weekly ticket digest");
            Shell.AddActivity("btnStartJob_Click → digest job started (async, cancellable)");

            try
            {
                var tickets = ticketService.GetTickets();
                var customers = tickets.Select(t => t.Customer).Distinct().OrderBy(c => c).ToList();

                for (int step = 1; step <= 10; step++)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(450, token);       // the "work" — the request has already returned

                    DescribeStep(step, tickets, customers);

                    if (chkSimulateError.Checked && step == 6)
                        throw new InvalidOperationException("SMTP relay smtp.internal.example:587 refused the connection (simulated).");

                    progressBar.Value = step * 10;
                    lblProgress.Text = $"{step * 10}% — step {step} of 10";
                    Application.Update(this);           // push this step's changes to the browser now
                }

                ShowStatus(lblStatus, "Digest built and sent — job completed.", StatusKind.Ok);
                LogJob("Job completed");
                Shell.AddActivity("digest job completed (10/10 steps)");
            }
            catch (OperationCanceledException)
            {
                ShowStatus(lblStatus, "Job cancelled — nothing was sent.", StatusKind.Warn);
                LogJob("Job cancelled by the user");
                Shell.AddActivity("digest job cancelled by btnCancelJob");
            }
            catch (Exception ex)
            {
                // Safe message for the user; the detail goes to the log, not the screen.
                ShowStatus(lblStatus, "The digest could not be sent. Support has the details in the job log.", StatusKind.Error);
                LogError(ex);
                Shell.AddActivity($"digest job failed: {ex.GetType().Name} (detail in lstJobLog)");
            }
            finally
            {
                SetJobRunning(false);
                jobCancellation.Dispose();
                jobCancellation = null;
            }
        }

        private void btnCancelJob_Click(object sender, EventArgs e)
        {
            jobCancellation?.Cancel();
            LogJob("Cancel requested…");
            Shell.AddActivity("btnCancelJob_Click → CancellationTokenSource.Cancel()");
        }

        /// <summary>One line per step, so the log reads like the job's story.</summary>
        private void DescribeStep(int step, System.Collections.Generic.List<Ticket> tickets, System.Collections.Generic.List<string> customers)
        {
            switch (step)
            {
                case 1: LogJob($"Loaded {tickets.Count} tickets from TicketService"); break;
                case 2: LogJob($"Grouped by customer: {customers.Count} companies"); break;
                case 3: LogJob($"Open: {tickets.Count(t => t.Status == "Open")} · In Progress: {tickets.Count(t => t.Status == "In Progress")} · Closed: {tickets.Count(t => t.Status == "Closed")}"); break;
                case 4: LogJob($"High priority still open: {tickets.Count(t => t.Priority == "High" && t.Status != "Closed")}"); break;
                case 5: LogJob("Rendered the digest table (HTML)"); break;
                case 6: LogJob("Connecting to the mail relay…"); break;
                case 7: LogJob($"Sent digest to {customers.Count} customer contacts"); break;
                case 8: LogJob("Sent the internal summary to the support channel"); break;
                case 9: LogJob("Archived the digest under /digests/" + DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)); break;
                case 10: LogJob("Cleaned up temporary files"); break;
            }
        }

        /// <summary>Buttons and progress in one place, called from the start and from finally.</summary>
        private void SetJobRunning(bool running)
        {
            btnStartJob.Enabled = !running;
            btnCancelJob.Enabled = running;
            chkSimulateError.Enabled = !running;

            if (running)
            {
                progressBar.Value = 0;
                lblProgress.Text = "0% — starting";
                ShowStatus(lblStatus, "Job running…", StatusKind.Warn);
            }
        }

        /// <summary>Developer detail (type, message, where) — in the log, never in the user-facing status.</summary>
        private void LogError(Exception ex)
        {
            LogJob($"ERROR {ex.GetType().Name}: {ex.Message}");
            LogJob("      at JobsView.btnStartJob_Click (step 6 — mail relay)");
        }

        private void LogJob(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            lstJobLog.Items.Add($"{time}  {message}");
            lstJobLog.SelectedIndex = lstJobLog.Items.Count - 1;
        }
    }
}
