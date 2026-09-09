using System;
using System.Collections.Generic;
using System.IO;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp
{
    /// <summary>
    /// Deployment review — Module 9 lab window (Configuration, security and deployment review).
    ///
    /// Left column:  "Release Information" (environment, target, version, reviewer, role + the lesson's
    ///               permission gate) and "Release checklist" (nine required checks that gate the package
    ///               status, optional items that do not, and per-target deployment notes).
    /// Right column: "Secrets" (license key / connection read by SecureConfig, shown masked only),
    ///               "Review summary" (the safe package text) and the "Troubleshooting log"
    ///               (timestamped, every line redacted by SafeLogger).
    /// Bottom bar:   Create Review Package (success path), the three failure paths (wrong role via the
    ///               server check, required checks incomplete, simulated packaging error) and the recovery.
    /// </summary>
    public partial class ReleaseReviewWindow : Form
    {
        // Per-session state: one instance of each per window, never static.
        private readonly UserAccount currentUser = new UserAccount { Name = "Jamie Lee", Role = "Support Agent" };
        private readonly ReleaseReviewService _reviewService = new ReleaseReviewService();
        private readonly SafeLogger _logger = new SafeLogger();
        private SecureConfig _secureConfig;

        private bool _bulkUpdate;   // true while a button ticks/unticks many items at once

        // Lesson s42 §4 — the required release checklist, verbatim.
        private static readonly string[] RequiredItems =
        {
            "Web.config reviewed and debug mode set correctly",
            "Wisej.NET license key handled securely",
            "Default/startup window, theme and URL settings checked",
            "Themes and static resources included",
            "Logging destination configured",
            "Authentication and authorization plan reviewed",
            "Sensitive files are not publicly downloadable",
            "Release build tested locally before deployment",
            "Deployment target requirements checked",
        };

        private static readonly string[] OptionalItems =
        {
            "Theme Builder tweaks reviewed",
            "Screenshots redacted",
            "Staging smoke test done",
            "Rollback plan written",
        };

        public ReleaseReviewWindow()
        {
            InitializeComponent();
        }

        private void ReleaseReviewWindow_Load(object sender, EventArgs e)
        {
            AddLog("Program.Main → new ReleaseReviewWindow().Show()");

            foreach (string item in RequiredItems) chkRequired.Items.Add(item);
            foreach (string item in OptionalItems) chkOptional.Items.Add(item);

            // Secrets: read once, keep only the masked status.
            _secureConfig = new SecureConfig(Application.StartupPath);
            ShowSecretStatus();

            // Defaults — each SelectedIndexChanged handler logs what it did.
            cboEnvironment.SelectedIndex = 2;   // Production
            cboTarget.SelectedIndex = 0;        // IIS → fills txtNotes
            cboRole.SelectedIndex = 0;          // Support Agent → review restricted

            UpdatePackageStatus();
            SetStatus("Review not started.", StatusKind.Warn);
            AddLog("Ready — complete the required checks, pick an allowed role, then Create Review Package");
        }

        #region Authorization: the UI gate (lesson s42 §1)

        private void cboRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentUser.Role = cboRole.Text;
            ApplyPermission();
            AddLog($"Role changed → {currentUser.Role}: {lblPermission.Text}");
        }

        /// <summary>The lesson snippet: the role decides whether the review action is available in the UI.</summary>
        private void ApplyPermission()
        {
            bool canReviewDeployment =
                currentUser.Role == "Admin" || currentUser.Role == "Team Lead";

            btnReviewPackage.Enabled = canReviewDeployment;
            lblPermission.Text = canReviewDeployment
                ? "Deployment review available."
                : "Review is restricted.";
            lblPermission.ForeColor = canReviewDeployment
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(224, 86, 59);
        }

        #endregion

        #region The lab's handler: Create Review Package

        /// <summary>
        /// The handler the lab asks for. UI gate → required checks → the service repeats both checks
        /// server-side and creates the package. The user only ever sees a safe message.
        /// </summary>
        private void btnReviewPackage_Click(object sender, EventArgs e)
        {
            bool canReview = currentUser.Role == "Admin" || currentUser.Role == "Team Lead";
            if (!canReview)
            {
                lblStatus.Text = "Review is restricted.";
                SetStatus("Review is restricted.", StatusKind.Error);
                AddLog($"Review attempt by {currentUser.Role} → restricted (UI gate)");
                return;
            }

            if (!AllRequiredChecksComplete())
            {
                lblStatus.Text = "Required checks incomplete.";
                SetStatus("Required checks incomplete.", StatusKind.Warn);
                AddLog($"Review attempt → required checks incomplete ({chkRequired.CheckedItems.Count} / {chkRequired.Items.Count})");
                return;
            }

            ReviewResult result = TryCreatePackage(currentUser.Role);
            if (result == null)
                return;                                   // packaging failed — already reported safely

            if (!result.Success)
            {
                SetStatus(result.Message, StatusKind.Error);
                AddLog($"Review refused by the server ({result.Reason}): {result.Message}");
                return;
            }

            AddLog("Deployment package reviewed.");
            lblStatus.Text = "Package ready for deployment.";
            SetStatus("Package ready for deployment.", StatusKind.Ok);
            txtSummary.Text = result.Summary;
            AlertBox.Show("Review package created — see the summary.", MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>All nine required items ticked? (Optional items never count.)</summary>
        private bool AllRequiredChecksComplete()
        {
            return chkRequired.Items.Count > 0
                && chkRequired.CheckedItems.Count == chkRequired.Items.Count;
        }

        /// <summary>
        /// Calls the server-side service with the given role. A thrown exception becomes a safe status
        /// message for the user and a redacted line in the log; the method then returns null.
        /// </summary>
        private ReviewResult TryCreatePackage(string role)
        {
            ReleaseRequest request = BuildRequest(role);
            AddLog($"ReleaseReviewService.TryCreatePackage(role={role}, env={request.Environment}, target={request.Target}, version={request.Version})");

            try
            {
                return _reviewService.TryCreatePackage(request);
            }
            catch (IOException ex)
            {
                // Safe release rule: the user gets a useful message, the developer gets the detail in the log.
                SetStatus("Packaging failed. Please contact the release manager.", StatusKind.Error);
                AddLog($"Packaging failed: {ex.GetType().Name} — {ex.Message} (stack trace in Trace, not on screen)");
                return null;
            }
        }

        #endregion

        #region Failure paths and recovery (bottom bar)

        /// <summary>
        /// Failure path (a): the disabled button is not the protection. This calls the service directly,
        /// as Support Agent, whatever cboRole says — the server answers "Review is restricted."
        /// </summary>
        private void btnTryAsAgent_Click(object sender, EventArgs e)
        {
            AddLog("btnTryAsAgent → bypassing the UI gate, calling the service as Support Agent");
            ReviewResult result = TryCreatePackage("Support Agent");
            if (result == null)
                return;

            SetStatus(result.Message, result.Success ? StatusKind.Ok : StatusKind.Error);
            AddLog(result.Success
                ? "Server allowed the review (unexpected — check ReleaseReviewService.AllowedRoles)"
                : $"Server refused ({result.Reason}): {result.Message} — the action is protected server-side, not just by btnReviewPackage.Enabled");
        }

        /// <summary>Recovery: tick all nine required items; the status flips to READY at 9 / 9.</summary>
        private void btnCompleteRequired_Click(object sender, EventArgs e)
        {
            SetAllChecked(chkRequired, true);
            UpdatePackageStatus();
            AddLog($"All required checks completed ({chkRequired.CheckedItems.Count} / {chkRequired.Items.Count}) → {lblPackageStatus.Text}");
        }

        private void btnResetChecks_Click(object sender, EventArgs e)
        {
            SetAllChecked(chkRequired, false);
            SetAllChecked(chkOptional, false);
            txtSummary.Text = string.Empty;
            UpdatePackageStatus();
            SetStatus("Checklist reset.", StatusKind.Warn);
            AddLog("Checklist reset → " + lblPackageStatus.Text);
        }

        private void SetAllChecked(CheckedListBox list, bool value)
        {
            _bulkUpdate = true;
            try
            {
                for (int i = 0; i < list.Items.Count; i++)
                    list.SetItemChecked(i, value);
            }
            finally
            {
                _bulkUpdate = false;
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
        }

        #endregion

        #region Checklist and package status

        private void chkRequired_AfterItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_bulkUpdate)
                return;

            bool done = chkRequired.GetItemChecked(e.Index);
            string item = chkRequired.Items[e.Index].ToString();
            AddLog(done
                ? $"Required check {chkRequired.CheckedItems.Count}/{chkRequired.Items.Count} completed: {item}"
                : $"Required check unchecked ({chkRequired.CheckedItems.Count}/{chkRequired.Items.Count}): {item}");

            UpdatePackageStatus();
        }

        private void chkOptional_AfterItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_bulkUpdate)
                return;

            bool done = chkOptional.GetItemChecked(e.Index);
            AddLog($"Optional item {(done ? "checked" : "unchecked")}: {chkOptional.Items[e.Index]} (does not change the package status)");
        }

        /// <summary>The Package Status message: red until every required check is complete, then green.</summary>
        private void UpdatePackageStatus()
        {
            int done = chkRequired.CheckedItems.Count;
            int total = chkRequired.Items.Count;
            bool ready = AllRequiredChecksComplete();

            lblPackageStatus.Text = ready
                ? $"Package status: READY for deployment review · {done} / {total} required checks"
                : $"Package status: NOT READY · {done} / {total} required checks";
            lblPackageStatus.ForeColor = ready
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(224, 86, 59);
        }

        #endregion

        #region Release information: environment and target

        private void cboEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            string env = cboEnvironment.Text;
            string meaning = env switch
            {
                "Debug" => "detailed errors, local testing — not safe for public users",
                "Staging" => "production-like test area used before release",
                _ => "safe error messages, protected secrets, stable configuration, logging enabled",
            };
            AddLog($"Environment → {env}: {meaning}");

            if (env == "Debug")
                SetStatus("Debug settings are for building, not for release — pick Staging or Production before packaging.", StatusKind.Warn);
        }

        private void cboTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtNotes.Text = DeploymentNotes.For(cboTarget.Text);
            AddLog($"Target → {cboTarget.Text}: deployment notes loaded from Services/DeploymentNotes.cs");
        }

        /// <summary>Everything the service needs, read from the controls once. Secrets go in masked.</summary>
        private ReleaseRequest BuildRequest(string role)
        {
            var open = new List<string>();
            for (int i = 0; i < chkOptional.Items.Count; i++)
                if (!chkOptional.GetItemChecked(i))
                    open.Add(chkOptional.Items[i].ToString());

            int rollbackIndex = Array.IndexOf(OptionalItems, "Rollback plan written");

            return new ReleaseRequest
            {
                Environment = cboEnvironment.Text,
                Target = cboTarget.Text,
                Version = txtVersion.Text.Trim(),
                Reviewer = txtReviewer.Text,
                Role = role,
                RequiredChecked = chkRequired.CheckedItems.Count,
                RequiredTotal = chkRequired.Items.Count,
                OptionalChecked = chkOptional.CheckedItems.Count,
                OptionalTotal = chkOptional.Items.Count,
                OpenOptionalItems = open,
                RollbackPlanWritten = rollbackIndex >= 0 && chkOptional.GetItemChecked(rollbackIndex),
                LicenseKeyStatus = _secureConfig.LicenseKey.Display,
                ConnectionStatus = _secureConfig.Connection.Display,
                Notes = txtNotes.Text,
                SimulateError = chkSimulateError.Checked,
            };
        }

        #endregion

        #region Secrets (lesson s41 §3)

        /// <summary>Shows only the masked status of each secret; the raw value never reaches a control or the log.</summary>
        private void ShowSecretStatus()
        {
            SecretStatus license = _secureConfig.LicenseKey;
            SecretStatus connection = _secureConfig.Connection;

            lblLicenseKey.Text = "License key       : " + license.Display;
            lblConnectionString.Text = "Database connection: " + connection.Display;
            lblLicenseKey.ForeColor = license.IsConfigured
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(224, 86, 59);
            lblConnectionString.ForeColor = connection.IsConfigured
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(224, 86, 59);

            AddLog($"SecureConfig → {SecureConfig.LicenseKeyVariable}: {license.Display}"
                   + (license.IsConfigured ? $" (source: {license.Source})" : string.Empty));
            AddLog($"SecureConfig → {SecureConfig.ConnectionVariable}: {connection.Display}"
                   + (connection.IsConfigured ? $" (source: {connection.Source})" : string.Empty));
        }

        /// <summary>"What NOT to do" — describes the unsafe pattern; prints no secret, hard-codes none.</summary>
        private void btnShowUnsafeExample_Click(object sender, EventArgs e)
        {
            AddLog("What NOT to do: a literal license key or connection value inside a button handler or a Page —");
            AddLog("  it ends up in Git history, in screenshots, and one Label.Text away from the browser.");
            AddLog("Safer: SecureConfig reads WISEJ_LICENSE_KEY / TRAINING_CONNECTION_STRING (or Web.config) and returns a masked status only.");
            // Proof that the log redacts even a careless line: the value below is fake and is masked before it is stored.
            AddLog("Redaction demo (fake value): licensekey=DEMO-0000-FAKE-1234 → the stored line masks it");
            SetStatus("Unsafe pattern explained in the log — no secret was shown.", StatusKind.Ok);
        }

        #endregion

        #region Helpers

        private enum StatusKind { Ok, Warn, Error }

        private void SetStatus(string text, StatusKind kind)
        {
            lblStatus.Text = "● " + text;
            lblStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        /// <summary>One place for logging: timestamp + redaction come from SafeLogger, so every handler stays short.</summary>
        private void AddLog(string message)
        {
            lstLog.Items.Add(_logger.Log(message));
            lstLog.SelectedIndex = lstLog.Items.Count - 1;
        }

        #endregion
    }
}
