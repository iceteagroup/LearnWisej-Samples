using System;
using System.Drawing;
using System.Linq;
using EnterpriseOps.Services.Jobs;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// The job detail screen — a UserControl so the Import Center hosts it and a future "Job detail" page can
    /// host the same thing. It is a pure observer: hand it a <see cref="JobRecord"/> copy and it renders it.
    /// It never calls a service, never starts anything and does not know a queue exists.
    ///
    /// It shows the two things a support engineer asks for at 09:41 on a Monday: when each transition happened
    /// (the status history the store recorded) and which rows failed and why (transient, retried and given up,
    /// or terminal).
    /// </summary>
    public partial class JobDetailPanel : UserControl
    {
        private static readonly Color Ok = Color.FromArgb(31, 157, 87);
        private static readonly Color Warn = Color.FromArgb(232, 161, 60);
        private static readonly Color Bad = Color.FromArgb(224, 86, 59);
        private static readonly Color Ink = Color.FromArgb(70, 88, 106);

        private Guid _shownJobId;
        private int _shownHistoryCount = -1;

        public JobDetailPanel()
        {
            InitializeComponent();
        }

        /// <summary>The retry policy in force, printed under the row errors so the two are read together.</summary>
        public void ShowPolicy(RetryPolicy policy)
        {
            this.labelPolicy.Text = policy == null
                ? "retry policy: —"
                : $"retry: {policy.MaxAttempts} attempts · {policy.BaseDelayMs} ms × 2ⁿ · cancel: {CancellationPolicy.Description}";
        }

        public void Clear()
        {
            _shownJobId = Guid.Empty;
            _shownHistoryCount = -1;
            this.labelDetailTitle.Text = "Job detail";
            this.labelResult.ForeColor = Ink;
            this.labelResult.Text = "Select a job in the queue to see its history and its per-row result.";
            this.listHistory.Items.Clear();
            this.listRowErrors.Items.Clear();
        }

        /// <summary>
        /// Renders a job. Called from the observer's push loop several times a second, so it repaints the two
        /// lists only when the history actually grew — a redraw is cheap, but "cheap × 3 per second × forever"
        /// is the habit this module is about.
        /// </summary>
        public void Show(JobRecord record)
        {
            if (record == null)
            {
                Clear();
                return;
            }

            bool sameJob = record.JobId == _shownJobId;
            this.labelDetailTitle.Text = $"Job detail — {record.Number} · {record.Description}";
            this.labelResult.Text = DescribeResult(record);
            this.labelResult.ForeColor = ResultColor(record.Status);

            if (!sameJob || record.History.Count != _shownHistoryCount)
            {
                _shownJobId = record.JobId;
                _shownHistoryCount = record.History.Count;

                this.listHistory.BeginUpdate();
                try
                {
                    this.listHistory.Items.Clear();
                    foreach (var entry in record.History)
                        this.listHistory.Items.Add($"{entry.AtUtc.ToLocalTime():HH:mm:ss}  {entry.Layer,-7} {ImportService.Describe(entry.Status),-19} {(entry.Percent >= 0 ? entry.Percent + "%" : ""),4}  {entry.Message}");
                    if (this.listHistory.Items.Count > 0)
                        this.listHistory.SelectedIndex = this.listHistory.Items.Count - 1;
                }
                finally
                {
                    this.listHistory.EndUpdate();
                }

                ShowRowErrors(record);
            }
        }

        private void ShowRowErrors(JobRecord record)
        {
            this.listRowErrors.BeginUpdate();
            try
            {
                this.listRowErrors.Items.Clear();

                if (record.Result?.TerminalError != null)
                {
                    this.listRowErrors.Items.Add("FILE  terminal — the job failed before any row was read:");
                    this.listRowErrors.Items.Add("      " + record.Result.TerminalError);
                    return;
                }

                var errors = record.Result?.RowErrors;
                if (errors == null || errors.Count == 0)
                {
                    this.listRowErrors.Items.Add(record.IsFinished ? "(no failed rows)" : "(none so far)");
                    return;
                }

                foreach (var error in errors)
                    this.listRowErrors.Items.Add(error.ToString());

                this.listRowErrors.Items.Add("");
                this.listRowErrors.Items.Add($"fix these {errors.Count} row(s) and re-import — the other");
                this.listRowErrors.Items.Add("rows are idempotent, so nothing is duplicated.");
            }
            finally
            {
                this.listRowErrors.EndUpdate();
            }
        }

        private static string DescribeResult(JobRecord record)
        {
            var result = record.Result;
            if (result == null)
                return $"{ImportService.Describe(record.Status)} · {record.Percent}% · {record.Message}";

            int terminal = result.RowErrors.Count(e => !e.Retryable);
            int exhausted = result.RowErrors.Count - terminal;

            string text = $"{ImportService.Describe(record.Status)} · {result.TotalRows:n0} rows · " +
                          $"✓ {result.Imported:n0} imported ({result.Created:n0} created, {result.Updated:n0} updated)" +
                          $" · ↻ {result.Retried} retried in {result.RetryAttempts} attempt(s)" +
                          $" · ✕ {terminal} terminal";
            if (exhausted > 0)
                text += $" · ⨯ {exhausted} gave up after retries";
            if (record.StartedUtc != null && record.FinishedUtc != null)
                text += $" · {(record.FinishedUtc.Value - record.StartedUtc.Value).TotalSeconds:n1} s";
            return text;
        }

        private static Color ResultColor(JobStatus status) => status switch
        {
            JobStatus.Completed => Ok,
            JobStatus.CompletedWithErrors => Warn,
            JobStatus.Canceled => Warn,
            JobStatus.Failed => Bad,
            _ => Ink,
        };
    }
}
