using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp
{
    /// <summary>
    /// Background jobs — Module 6 lab window (Session state, background work and safe error handling).
    ///
    /// Top-left card:     "Job runner" — btnStartImport / btnStartExport / btnCancel / btnClearLog,
    ///                    chkSimulateError, progressBar, lblStatus (the SAFE message the user sees) and
    ///                    lblJobInfo (job name, step x/y, elapsed). The lesson's try / catch / finally
    ///                    handler lives here, unchanged.
    /// Bottom-left card:  "Session vs shared state" — Application.SessionId, an instance counter (per
    ///                    user), the Application.Session bag, and a deliberately static counter that every
    ///                    browser tab shares: the static-field trap, made visible on purpose.
    /// Right card:        "Job log" — every step, cancellation and failure with a timestamp and the job
    ///                    name (the DEVELOPER detail; the user never sees it in lblStatus).
    /// </summary>
    public partial class JobsWindow : Form
    {
        private readonly JobCatalog _jobs = new JobCatalog();

        // ── Per-user session state ────────────────────────────────────────────────────────────────────
        // This Form is created by Program.Main once per connected user session (see Default.json
        // "startup"), so every INSTANCE field below belongs to exactly one user. That is why the
        // CancellationTokenSource is an instance field: user A's Cancel must never cancel user B's job.
        private CancellationTokenSource _currentJob;      // the running job of THIS session, or null
        private int _jobsRunInThisSession;                // per user — starts at 0 in every new tab
        private string _currentJobName = "—";
        private int _currentStep;
        private int _totalSteps;
        private JobOutcome _outcome = JobOutcome.Idle;

        // ── The trap, on purpose ──────────────────────────────────────────────────────────────────────
        // `static` = ONE value for the whole server process, shared by every session. Open a second
        // browser tab: its "Jobs run in this session" starts at 0, but this counter keeps counting for
        // both tabs. Put a job, a selected ticket or a progress message in a static field and one user
        // sees (or cancels) another user's work. The lesson calls this one of the easiest server-side
        // mistakes; it is here so the difference is something you can watch, not just read about.
        private static int _jobsRunOnThisServer;

        private enum JobOutcome { Idle, Running, Completed, Cancelled, Failed }

        public JobsWindow()
        {
            // InitializeComponent() builds every control from JobsWindow.Designer.cs — the Designer owns that file.
            InitializeComponent();
        }

        private void JobsWindow_Load(object sender, EventArgs e)
        {
            // Session bag: a dynamic per-session store. Initialised here so the card can always read it.
            Application.Session.LastJob = "(none yet)";

            AddLog("Program.Main → new JobsWindow().Show()  — one JobsWindow per user session");
            AddLog($"JobsWindow_Load → session {ShortSessionId()}; jobs in this session = 0; static server counter = {_jobsRunOnThisServer}");
            if (_jobsRunOnThisServer > 0)
                AddLog("             ↑ already > 0: another tab ran jobs before this session existed — that is the static-field trap");

            SetJobRunning(false);
            SetStatus("ready — click Start Import or Start Export", StatusKind.Ok);
            ShowJobInfo("—", 0, 0, "waiting", TimeSpan.Zero);
            RefreshStateCard();
        }

        #region The lab's handlers — the lesson's try / catch / finally pattern, unchanged

        /// <summary>
        /// The button click starts the workflow. The background job does the long work (RunImportJobAsync).
        /// The UI shows progress (inside the job). The catch blocks log developer detail but show a short,
        /// safe message. finally runs whether the job succeeds, is cancelled or throws — so the buttons
        /// always come back and the page never gets stuck in "running" mode.
        /// </summary>
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
                _outcome = JobOutcome.Cancelled;
                lblStatus.Text = "Import cancelled.";
                AddLog($"Import: cancelled by the user at step {_currentStep}/{_totalSteps}");
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
                _outcome = JobOutcome.Cancelled;
                lblStatus.Text = "Export cancelled.";
                AddLog($"Export: cancelled by the user at step {_currentStep}/{_totalSteps}");
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

        /// <summary>
        /// Cancel only signals the token of THIS session's job (_currentJob is an instance field). The
        /// job notices at its next `await Task.Delay(700, token)`, which throws OperationCanceledException,
        /// and the Start handler's catch block turns that into "Import cancelled." + a log line.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_currentJob == null)
            {
                AddLog("btnCancel_Click → no job is running in this session (nothing to cancel)");
                return;
            }

            AddLog($"{_currentJobName}: btnCancel_Click → _currentJob.Cancel() requested at step {_currentStep}/{_totalSteps}");
            btnCancel.Enabled = false;                       // one cancel is enough; the job will confirm
            SetStatus($"{_currentJobName}: cancelling…", StatusKind.Warn);
            _currentJob.Cancel();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
        }

        private void chkSimulateError_CheckedChanged(object sender, EventArgs e)
        {
            AddLog(chkSimulateError.Checked
                ? "chkSimulateError → ON: the next job throws InvalidOperationException at step 4"
                : "chkSimulateError → OFF: jobs run to completion");
        }

        private void btnRefreshState_Click(object sender, EventArgs e)
        {
            RefreshStateCard();
            AddLog($"btnRefreshState_Click → session {ShortSessionId()}: instance counter = {_jobsRunInThisSession}, static counter = {_jobsRunOnThisServer}");
        }

        #endregion

        #region The background job

        private Task RunImportJobAsync() => RunJobAsync(_jobs.ImportJob());

        private Task RunExportJobAsync() => RunJobAsync(_jobs.ExportJob());

        private Task RunJobAsync(JobDefinition job) => RunJobAsync(job.Name, job.Steps, job.InputFile);

        /// <summary>
        /// One job runner for every job: loop the steps, await each one, push progress on purpose.
        /// Each `await` hands the server thread back, so the page stays responsive (Cancel still works);
        /// each `Application.Update(this)` sends the changed controls to the browser while this method
        /// is still awaiting — without it the user would see nothing until the handler completed.
        /// </summary>
        private async Task RunJobAsync(string jobName, string[] steps, string inputFile = null)
        {
            if (_currentJob != null)
            {
                // The disabled Start button already prevents this; the guard is the belt to its braces.
                throw new InvalidOperationException($"A job ({_currentJobName}) is already running in this session.");
            }

            _currentJob = new CancellationTokenSource();     // per session — see the field comment
            CancellationToken token = _currentJob.Token;
            Stopwatch elapsed = Stopwatch.StartNew();

            _currentJobName = jobName;
            _currentStep = 0;
            _totalSteps = steps.Length;
            _outcome = JobOutcome.Running;

            // Session state changes when a job starts: the per-user counter and the session bag …
            _jobsRunInThisSession++;
            Application.Session.LastJob = $"{jobName} (started {DateTime.Now:HH:mm:ss})";
            // … and the shared counter (Interlocked because every session on this server touches it).
            Interlocked.Increment(ref _jobsRunOnThisServer);
            RefreshStateCard();

            try
            {
                progressBar.Value = 0;
                AddLog($"{jobName}: started — {steps.Length} steps, {inputFile ?? "no input file"}, simulateError = {chkSimulateError.Checked}");

                for (int i = 0; i < steps.Length; i++)
                {
                    int stepNumber = i + 1;
                    _currentStep = stepNumber;

                    // 1. Tell the user what is happening now, then push it to the browser.
                    ShowProgress(jobName, stepNumber, steps.Length, steps[i], elapsed.Elapsed);
                    AddLog($"{jobName}: step {stepNumber}/{steps.Length} — {steps[i]}…");
                    Application.Update(this);

                    // 2. The "long work". Task.Delay honours the token: Cancel throws OperationCanceledException here.
                    await Task.Delay(700, token);

                    // 3. The failure path, on request: a realistic exception with developer detail in the message.
                    if (chkSimulateError.Checked && stepNumber == 4)
                    {
                        throw new InvalidOperationException(
                            $"Row 1,204: column 'Email' is not a valid address ({inputFile ?? jobName})");
                    }

                    // 4. The step is done: move the bar, then push again.
                    progressBar.Value = stepNumber * 100 / steps.Length;
                    ShowJobInfo(jobName, stepNumber, steps.Length, steps[i] + " ✓", elapsed.Elapsed);
                    Application.Update(this);
                }

                _outcome = JobOutcome.Completed;
                AddLog($"{jobName}: completed in {elapsed.Elapsed.TotalSeconds:0.0} s ({steps.Length}/{steps.Length} steps)");
            }
            finally
            {
                // Whatever happened, this session no longer has a running job.
                _currentJob.Dispose();
                _currentJob = null;
            }
        }

        #endregion

        #region Helpers (small, reusable — the habit the course teaches)

        /// <summary>
        /// Guards the UI while a job runs: Start is disabled (no duplicate jobs), Cancel is enabled, the
        /// error switch is frozen. Called from `finally`, so it runs after success, cancel AND failure —
        /// the page can never be left in "running" mode.
        /// </summary>
        private void SetJobRunning(bool running)
        {
            btnStartImport.Enabled = !running;
            btnStartExport.Enabled = !running;
            chkSimulateError.Enabled = !running;
            btnCancel.Enabled = running;

            if (running)
            {
                SetStatus("job running… Start is disabled, Cancel is enabled", StatusKind.Warn);
                lblStatusHint.Text = "state: RUNNING  ·  btnStartImport/btnStartExport.Enabled = false  ·  btnCancel.Enabled = true";
            }
            else
            {
                // The handler already set the safe text; only the colour and the hint depend on the outcome.
                lblStatus.ForeColor = ColourFor(_outcome == JobOutcome.Failed ? StatusKind.Error
                                              : _outcome == JobOutcome.Cancelled ? StatusKind.Warn
                                              : StatusKind.Ok);
                lblStatusHint.Text = $"state: {_outcome.ToString().ToUpperInvariant()}  ·  Start enabled again by SetJobRunning(false) in finally";
                AddLog($"SetJobRunning(false) → buttons reset (outcome: {_outcome}) — the UI is back in a safe state");
                RefreshStateCard();
            }

            // Harmless inside a request; needed when the caller is a continuation after an await.
            Application.Update(this);
        }

        /// <summary>
        /// Developer detail goes to the log (in production: ILogger, a file, Application Insights).
        /// The user never sees any of this — lblStatus gets the short safe message from the handler.
        /// </summary>
        private void LogError(Exception ex)
        {
            _outcome = JobOutcome.Failed;
            AddLog($"ERROR {ex.GetType().Name}: {ex.Message}");
            AddLog($"      job = {_currentJobName}, step = {_currentStep}/{_totalSteps}, session = {ShortSessionId()}, " +
                   $"simulateError = {chkSimulateError.Checked}, progress = {progressBar.Value}%");
            AddLog($"      user sees only: \"{_currentJobName} failed. Please try again or contact support.\"");
        }

        private void ShowProgress(string jobName, int step, int total, string stepName, TimeSpan elapsed)
        {
            SetStatus($"{jobName}: step {step}/{total} — {stepName}…", StatusKind.Warn);
            ShowJobInfo(jobName, step, total, stepName, elapsed);
        }

        private void ShowJobInfo(string jobName, int step, int total, string stepName, TimeSpan elapsed)
        {
            string stepText = total == 0 ? "—" : $"{step}/{total}  {stepName}";
            lblJobInfo.Text =
                $"Job:      {jobName}\n" +
                $"Step:     {stepText}\n" +
                $"Elapsed:  {elapsed.TotalSeconds:0.0} s\n" +
                $"Token:    {(_currentJob == null ? "none (no job in this session)" : _currentJob.IsCancellationRequested ? "cancel requested" : "live — Cancel Job signals it")}";
        }

        /// <summary>The "Session vs shared state" card — per-user values next to the shared one.</summary>
        private void RefreshStateCard()
        {
            string lastJob = Application.Session.LastJob as string ?? "(none yet)";
            lblSessionId.Text = $"Session: {ShortSessionId()}   (Application.SessionId — one per connected user)";
            lblSessionJobs.Text = $"Jobs run in this session: {_jobsRunInThisSession}   (instance field — per user)";
            lblLastJob.Text = $"Last job (session bag): {lastJob}   (Application.Session.LastJob)";
            lblServerJobs.Text = $"Jobs run on this server (static): {_jobsRunOnThisServer}";
        }

        private static string ShortSessionId()
        {
            string id = Application.SessionId ?? "";
            return id.Length > 8 ? id.Substring(0, 8) : id;
        }

        private enum StatusKind { Ok, Warn, Error }

        private static System.Drawing.Color ColourFor(StatusKind kind) => kind switch
        {
            StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
            StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
            _ => System.Drawing.Color.FromArgb(31, 157, 87),
        };

        private void SetStatus(string text, StatusKind kind)
        {
            lblStatus.Text = "● " + text;
            lblStatus.ForeColor = ColourFor(kind);
        }

        /// <summary>One place for logging: timestamp + message (the job name is part of every job message).</summary>
        private void AddLog(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            lstLog.Items.Add($"{time}  {message}");
            lstLog.SelectedIndex = lstLog.Items.Count - 1;
        }

        #endregion
    }
}
