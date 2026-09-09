namespace WisejTrainingApp
{
    partial class ReleaseReviewWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelRelease = new Wisej.Web.Panel();
            this.labelReleaseCard = new Wisej.Web.Label();
            this.lblEnvironment = new Wisej.Web.Label();
            this.cboEnvironment = new Wisej.Web.ComboBox();
            this.lblTarget = new Wisej.Web.Label();
            this.cboTarget = new Wisej.Web.ComboBox();
            this.lblVersion = new Wisej.Web.Label();
            this.txtVersion = new Wisej.Web.TextBox();
            this.lblReviewer = new Wisej.Web.Label();
            this.txtReviewer = new Wisej.Web.TextBox();
            this.lblRole = new Wisej.Web.Label();
            this.cboRole = new Wisej.Web.ComboBox();
            this.lblPermission = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.lblHint = new Wisej.Web.Label();
            this.panelChecklist = new Wisej.Web.Panel();
            this.labelChecklistCard = new Wisej.Web.Label();
            this.lblRequiredHead = new Wisej.Web.Label();
            this.chkRequired = new Wisej.Web.CheckedListBox();
            this.lblOptionalHead = new Wisej.Web.Label();
            this.chkOptional = new Wisej.Web.CheckedListBox();
            this.lblPackageStatus = new Wisej.Web.Label();
            this.lblNotesHead = new Wisej.Web.Label();
            this.txtNotes = new Wisej.Web.TextBox();
            this.panelSecrets = new Wisej.Web.Panel();
            this.labelSecretsCard = new Wisej.Web.Label();
            this.lblLicenseKey = new Wisej.Web.Label();
            this.lblConnectionString = new Wisej.Web.Label();
            this.btnShowUnsafeExample = new Wisej.Web.Button();
            this.lblSecretsHint = new Wisej.Web.Label();
            this.panelSummary = new Wisej.Web.Panel();
            this.labelSummaryCard = new Wisej.Web.Label();
            this.txtSummary = new Wisej.Web.TextBox();
            this.panelLog = new Wisej.Web.Panel();
            this.labelLogCard = new Wisej.Web.Label();
            this.lstLog = new Wisej.Web.ListBox();
            this.labelLogFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.btnReviewPackage = new Wisej.Web.Button();
            this.btnTryAsAgent = new Wisej.Web.Button();
            this.btnCompleteRequired = new Wisej.Web.Button();
            this.btnResetChecks = new Wisej.Web.Button();
            this.chkSimulateError = new Wisej.Web.CheckBox();
            this.btnClearLog = new Wisej.Web.Button();
            this.panelRelease.SuspendLayout();
            this.panelChecklist.SuspendLayout();
            this.panelSecrets.SuspendLayout();
            this.panelSummary.SuspendLayout();
            this.panelLog.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelRelease  (Release Information — where the app is going, and who is reviewing)
            //
            this.panelRelease.BackColor = System.Drawing.Color.White;
            this.panelRelease.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelRelease.Controls.Add(this.labelReleaseCard);
            this.panelRelease.Controls.Add(this.lblEnvironment);
            this.panelRelease.Controls.Add(this.cboEnvironment);
            this.panelRelease.Controls.Add(this.lblTarget);
            this.panelRelease.Controls.Add(this.cboTarget);
            this.panelRelease.Controls.Add(this.lblVersion);
            this.panelRelease.Controls.Add(this.txtVersion);
            this.panelRelease.Controls.Add(this.lblReviewer);
            this.panelRelease.Controls.Add(this.txtReviewer);
            this.panelRelease.Controls.Add(this.lblRole);
            this.panelRelease.Controls.Add(this.cboRole);
            this.panelRelease.Controls.Add(this.lblPermission);
            this.panelRelease.Controls.Add(this.lblStatus);
            this.panelRelease.Controls.Add(this.lblHint);
            this.panelRelease.Location = new System.Drawing.Point(30, 30);
            this.panelRelease.Name = "panelRelease";
            this.panelRelease.Size = new System.Drawing.Size(560, 244);
            //
            // labelReleaseCard
            //
            this.labelReleaseCard.AutoSize = false;
            this.labelReleaseCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelReleaseCard.Location = new System.Drawing.Point(24, 14);
            this.labelReleaseCard.Name = "labelReleaseCard";
            this.labelReleaseCard.Size = new System.Drawing.Size(512, 28);
            this.labelReleaseCard.Text = "Release Information  ·  environment, target, version, reviewer, role";
            //
            // lblEnvironment
            //
            this.lblEnvironment.AutoSize = false;
            this.lblEnvironment.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblEnvironment.Location = new System.Drawing.Point(24, 54);
            this.lblEnvironment.Name = "lblEnvironment";
            this.lblEnvironment.Size = new System.Drawing.Size(100, 22);
            this.lblEnvironment.Text = "Environment";
            //
            // cboEnvironment
            //
            this.cboEnvironment.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboEnvironment.Items.AddRange(new object[] { "Debug", "Staging", "Production" });
            this.cboEnvironment.Location = new System.Drawing.Point(130, 50);
            this.cboEnvironment.Name = "cboEnvironment";
            this.cboEnvironment.Size = new System.Drawing.Size(150, 30);
            this.cboEnvironment.SelectedIndexChanged += new System.EventHandler(this.cboEnvironment_SelectedIndexChanged);
            //
            // lblTarget
            //
            this.lblTarget.AutoSize = false;
            this.lblTarget.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTarget.Location = new System.Drawing.Point(300, 54);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(70, 22);
            this.lblTarget.Text = "Target";
            //
            // cboTarget
            //
            this.cboTarget.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTarget.Items.AddRange(new object[] { "IIS", "Kestrel", "Cloud" });
            this.cboTarget.Location = new System.Drawing.Point(376, 50);
            this.cboTarget.Name = "cboTarget";
            this.cboTarget.Size = new System.Drawing.Size(160, 30);
            this.cboTarget.SelectedIndexChanged += new System.EventHandler(this.cboTarget_SelectedIndexChanged);
            //
            // lblVersion
            //
            this.lblVersion.AutoSize = false;
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblVersion.Location = new System.Drawing.Point(24, 92);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(100, 22);
            this.lblVersion.Text = "Version";
            //
            // txtVersion
            //
            this.txtVersion.Location = new System.Drawing.Point(130, 88);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(150, 30);
            this.txtVersion.Text = "1.4.0";
            this.txtVersion.Watermark = "major.minor.patch";
            //
            // lblReviewer
            //
            this.lblReviewer.AutoSize = false;
            this.lblReviewer.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblReviewer.Location = new System.Drawing.Point(300, 92);
            this.lblReviewer.Name = "lblReviewer";
            this.lblReviewer.Size = new System.Drawing.Size(70, 22);
            this.lblReviewer.Text = "Reviewer";
            //
            // txtReviewer
            //
            this.txtReviewer.Location = new System.Drawing.Point(376, 88);
            this.txtReviewer.Name = "txtReviewer";
            this.txtReviewer.Size = new System.Drawing.Size(160, 30);
            this.txtReviewer.Text = "Jamie Lee";
            this.txtReviewer.Watermark = "who signs off";
            //
            // lblRole
            //
            this.lblRole.AutoSize = false;
            this.lblRole.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblRole.Location = new System.Drawing.Point(24, 130);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(100, 22);
            this.lblRole.Text = "Your role";
            //
            // cboRole  (authorization: the role decides whether the review action is available)
            //
            this.cboRole.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboRole.Items.AddRange(new object[] { "Support Agent", "Team Lead", "Admin" });
            this.cboRole.Location = new System.Drawing.Point(130, 126);
            this.cboRole.Name = "cboRole";
            this.cboRole.Size = new System.Drawing.Size(150, 30);
            this.cboRole.ToolTipText = "Changes currentUser.Role — the UI gate and the server-side check both use it.";
            this.cboRole.SelectedIndexChanged += new System.EventHandler(this.cboRole_SelectedIndexChanged);
            //
            // lblPermission  (the lesson's lblPermission: available / restricted)
            //
            this.lblPermission.AutoSize = false;
            this.lblPermission.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblPermission.Location = new System.Drawing.Point(300, 130);
            this.lblPermission.Name = "lblPermission";
            this.lblPermission.Size = new System.Drawing.Size(236, 22);
            this.lblPermission.Text = "Review is restricted.";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.lblStatus.Location = new System.Drawing.Point(24, 166);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatus.Size = new System.Drawing.Size(512, 36);
            this.lblStatus.Text = "● Review not started.";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblHint
            //
            this.lblHint.AutoSize = false;
            this.lblHint.Font = new System.Drawing.Font("monospace", 8F);
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblHint.Location = new System.Drawing.Point(24, 206);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(512, 32);
            this.lblHint.Text = "UI gate:     canReviewDeployment = Role == \"Admin\" || Role == \"Team Lead\" → btnReviewPackage.Enabled\nServer gate: ReleaseReviewService.TryCreatePackage repeats the role + required-checks test itself";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelChecklist  (Release checklist — required gates the status, optional does not)
            //
            this.panelChecklist.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelChecklist.BackColor = System.Drawing.Color.White;
            this.panelChecklist.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelChecklist.Controls.Add(this.labelChecklistCard);
            this.panelChecklist.Controls.Add(this.lblRequiredHead);
            this.panelChecklist.Controls.Add(this.chkRequired);
            this.panelChecklist.Controls.Add(this.lblOptionalHead);
            this.panelChecklist.Controls.Add(this.chkOptional);
            this.panelChecklist.Controls.Add(this.lblPackageStatus);
            this.panelChecklist.Controls.Add(this.lblNotesHead);
            this.panelChecklist.Controls.Add(this.txtNotes);
            this.panelChecklist.Location = new System.Drawing.Point(30, 292);
            this.panelChecklist.Name = "panelChecklist";
            this.panelChecklist.Size = new System.Drawing.Size(560, 398);
            //
            // labelChecklistCard
            //
            this.labelChecklistCard.AutoSize = false;
            this.labelChecklistCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelChecklistCard.Location = new System.Drawing.Point(24, 14);
            this.labelChecklistCard.Name = "labelChecklistCard";
            this.labelChecklistCard.Size = new System.Drawing.Size(512, 28);
            this.labelChecklistCard.Text = "Release checklist  ·  required checks gate the status";
            //
            // lblRequiredHead
            //
            this.lblRequiredHead.AutoSize = false;
            this.lblRequiredHead.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblRequiredHead.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblRequiredHead.Location = new System.Drawing.Point(24, 46);
            this.lblRequiredHead.Name = "lblRequiredHead";
            this.lblRequiredHead.Size = new System.Drawing.Size(300, 20);
            this.lblRequiredHead.Text = "REQUIRED (lesson s42 §4)";
            //
            // chkRequired  (CheckOnClick: one click toggles; AfterItemCheck recomputes the package status)
            //
            this.chkRequired.CheckOnClick = true;
            this.chkRequired.Location = new System.Drawing.Point(24, 68);
            this.chkRequired.Name = "chkRequired";
            this.chkRequired.Size = new System.Drawing.Size(312, 186);
            this.chkRequired.ToolTipText = "Nine required checks — the package status turns green only at 9 / 9.";
            this.chkRequired.AfterItemCheck += new Wisej.Web.ItemCheckEventHandler(this.chkRequired_AfterItemCheck);
            //
            // lblOptionalHead
            //
            this.lblOptionalHead.AutoSize = false;
            this.lblOptionalHead.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblOptionalHead.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblOptionalHead.Location = new System.Drawing.Point(348, 46);
            this.lblOptionalHead.Name = "lblOptionalHead";
            this.lblOptionalHead.Size = new System.Drawing.Size(188, 20);
            this.lblOptionalHead.Text = "OPTIONAL (does not gate)";
            //
            // chkOptional
            //
            this.chkOptional.CheckOnClick = true;
            this.chkOptional.Location = new System.Drawing.Point(348, 68);
            this.chkOptional.Name = "chkOptional";
            this.chkOptional.Size = new System.Drawing.Size(188, 186);
            this.chkOptional.ToolTipText = "Nice-to-have items; unchecked ones are listed as open items in the summary.";
            this.chkOptional.AfterItemCheck += new Wisej.Web.ItemCheckEventHandler(this.chkOptional_AfterItemCheck);
            //
            // lblPackageStatus  (the Package Status message — red NOT READY → green READY)
            //
            this.lblPackageStatus.AutoSize = false;
            this.lblPackageStatus.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblPackageStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblPackageStatus.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblPackageStatus.Location = new System.Drawing.Point(24, 262);
            this.lblPackageStatus.Name = "lblPackageStatus";
            this.lblPackageStatus.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblPackageStatus.Size = new System.Drawing.Size(512, 30);
            this.lblPackageStatus.Text = "Package status: NOT READY · 0 / 9 required checks";
            this.lblPackageStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblNotesHead
            //
            this.lblNotesHead.AutoSize = false;
            this.lblNotesHead.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNotesHead.Location = new System.Drawing.Point(24, 300);
            this.lblNotesHead.Name = "lblNotesHead";
            this.lblNotesHead.Size = new System.Drawing.Size(512, 20);
            this.lblNotesHead.Text = "Deployment notes  ·  pre-filled per target (Services/DeploymentNotes.cs), edit freely";
            //
            // txtNotes  (multiline deployment notes)
            //
            this.txtNotes.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtNotes.Font = new System.Drawing.Font("default", 9F);
            this.txtNotes.Location = new System.Drawing.Point(24, 322);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(512, 62);
            //
            // panelSecrets  (Secrets — read from secure config, shown masked, never logged)
            //
            this.panelSecrets.BackColor = System.Drawing.Color.White;
            this.panelSecrets.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSecrets.Controls.Add(this.labelSecretsCard);
            this.panelSecrets.Controls.Add(this.lblLicenseKey);
            this.panelSecrets.Controls.Add(this.lblConnectionString);
            this.panelSecrets.Controls.Add(this.btnShowUnsafeExample);
            this.panelSecrets.Controls.Add(this.lblSecretsHint);
            this.panelSecrets.Location = new System.Drawing.Point(618, 30);
            this.panelSecrets.Name = "panelSecrets";
            this.panelSecrets.Size = new System.Drawing.Size(700, 150);
            //
            // labelSecretsCard
            //
            this.labelSecretsCard.AutoSize = false;
            this.labelSecretsCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSecretsCard.Location = new System.Drawing.Point(20, 14);
            this.labelSecretsCard.Name = "labelSecretsCard";
            this.labelSecretsCard.Size = new System.Drawing.Size(660, 28);
            this.labelSecretsCard.Text = "Secrets (secure config)  ·  environment variables → Web.config → missing";
            //
            // lblLicenseKey
            //
            this.lblLicenseKey.AutoSize = false;
            this.lblLicenseKey.Font = new System.Drawing.Font("monospace", 9F);
            this.lblLicenseKey.Location = new System.Drawing.Point(20, 48);
            this.lblLicenseKey.Name = "lblLicenseKey";
            this.lblLicenseKey.Size = new System.Drawing.Size(660, 22);
            this.lblLicenseKey.Text = "License key       : …";
            //
            // lblConnectionString
            //
            this.lblConnectionString.AutoSize = false;
            this.lblConnectionString.Font = new System.Drawing.Font("monospace", 9F);
            this.lblConnectionString.Location = new System.Drawing.Point(20, 72);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(660, 22);
            this.lblConnectionString.Text = "Database connection: …";
            //
            // btnShowUnsafeExample  ("What NOT to do" — explains the unsafe pattern without printing a secret)
            //
            this.btnShowUnsafeExample.Location = new System.Drawing.Point(20, 104);
            this.btnShowUnsafeExample.Name = "btnShowUnsafeExample";
            this.btnShowUnsafeExample.Size = new System.Drawing.Size(180, 32);
            this.btnShowUnsafeExample.Text = "What NOT to do";
            this.btnShowUnsafeExample.ToolTipText = "Logs a note about the unsafe pattern (hard-coded key in a button handler). Prints no secret.";
            this.btnShowUnsafeExample.Click += new System.EventHandler(this.btnShowUnsafeExample_Click);
            //
            // lblSecretsHint
            //
            this.lblSecretsHint.AutoSize = false;
            this.lblSecretsHint.Font = new System.Drawing.Font("default", 9F);
            this.lblSecretsHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSecretsHint.Location = new System.Drawing.Point(212, 104);
            this.lblSecretsHint.Name = "lblSecretsHint";
            this.lblSecretsHint.Size = new System.Drawing.Size(468, 32);
            this.lblSecretsHint.Text = "Services/SecureConfig.cs reads the value and returns only a masked status; nothing on this screen or in the log ever shows a value.";
            this.lblSecretsHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelSummary  (Review summary — the safe package text)
            //
            this.panelSummary.BackColor = System.Drawing.Color.White;
            this.panelSummary.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSummary.Controls.Add(this.labelSummaryCard);
            this.panelSummary.Controls.Add(this.txtSummary);
            this.panelSummary.Location = new System.Drawing.Point(618, 198);
            this.panelSummary.Name = "panelSummary";
            this.panelSummary.Size = new System.Drawing.Size(700, 232);
            //
            // labelSummaryCard
            //
            this.labelSummaryCard.AutoSize = false;
            this.labelSummaryCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSummaryCard.Location = new System.Drawing.Point(20, 14);
            this.labelSummaryCard.Name = "labelSummaryCard";
            this.labelSummaryCard.Size = new System.Drawing.Size(660, 28);
            this.labelSummaryCard.Text = "Review summary  ·  environment, target, version, reviewer, status, known issues, rollback";
            //
            // txtSummary
            //
            this.txtSummary.Font = new System.Drawing.Font("monospace", 9F);
            this.txtSummary.Location = new System.Drawing.Point(20, 48);
            this.txtSummary.Multiline = true;
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ReadOnly = true;
            this.txtSummary.Size = new System.Drawing.Size(660, 170);
            this.txtSummary.Watermark = "Create Review Package fills this in — a safe summary, no secrets, no stack traces.";
            //
            // panelLog  (Troubleshooting log — timestamps, redacted, no secrets)
            //
            this.panelLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelLog.BackColor = System.Drawing.Color.White;
            this.panelLog.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLog.Controls.Add(this.labelLogCard);
            this.panelLog.Controls.Add(this.lstLog);
            this.panelLog.Controls.Add(this.labelLogFooter);
            this.panelLog.Location = new System.Drawing.Point(618, 448);
            this.panelLog.Name = "panelLog";
            this.panelLog.Size = new System.Drawing.Size(700, 242);
            //
            // labelLogCard
            //
            this.labelLogCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogCard.AutoSize = false;
            this.labelLogCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelLogCard.Location = new System.Drawing.Point(20, 14);
            this.labelLogCard.Name = "labelLogCard";
            this.labelLogCard.Size = new System.Drawing.Size(660, 28);
            this.labelLogCard.Text = "Troubleshooting log  ·  every line passes through SafeLogger.Redact";
            //
            // lstLog
            //
            this.lstLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstLog.Location = new System.Drawing.Point(20, 48);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(660, 156);
            //
            // labelLogFooter
            //
            this.labelLogFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogFooter.AutoSize = false;
            this.labelLogFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLogFooter.Location = new System.Drawing.Point(20, 210);
            this.labelLogFooter.Name = "labelLogFooter";
            this.labelLogFooter.Size = new System.Drawing.Size(660, 24);
            this.labelLogFooter.Text = "users see a safe message · developers see the detail here (redacted) and in Trace";
            //
            // panelActions  (bottom bar: success, failure paths, recovery)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.btnReviewPackage);
            this.panelActions.Controls.Add(this.btnTryAsAgent);
            this.panelActions.Controls.Add(this.btnCompleteRequired);
            this.panelActions.Controls.Add(this.btnResetChecks);
            this.panelActions.Controls.Add(this.chkSimulateError);
            this.panelActions.Controls.Add(this.btnClearLog);
            this.panelActions.Location = new System.Drawing.Point(30, 706);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // btnReviewPackage  (the lab's handler: btnReviewPackage_Click)
            //
            this.btnReviewPackage.Enabled = false;
            this.btnReviewPackage.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnReviewPackage.Location = new System.Drawing.Point(0, 4);
            this.btnReviewPackage.Name = "btnReviewPackage";
            this.btnReviewPackage.Size = new System.Drawing.Size(200, 36);
            this.btnReviewPackage.Text = "Create Review Package";
            this.btnReviewPackage.ToolTipText = "UI gate (role) → required checks → ReleaseReviewService.TryCreatePackage (server-side checks) → summary.";
            this.btnReviewPackage.Click += new System.EventHandler(this.btnReviewPackage_Click);
            //
            // btnTryAsAgent  (failure path a: the server check protects the action, not the disabled button)
            //
            this.btnTryAsAgent.Location = new System.Drawing.Point(208, 4);
            this.btnTryAsAgent.Name = "btnTryAsAgent";
            this.btnTryAsAgent.Size = new System.Drawing.Size(290, 36);
            this.btnTryAsAgent.Text = "Try review as Support Agent (server check)";
            this.btnTryAsAgent.ToolTipText = "Calls the service directly with Role = Support Agent, whatever the UI says — the server answers \"Review is restricted.\"";
            this.btnTryAsAgent.Click += new System.EventHandler(this.btnTryAsAgent_Click);
            //
            // btnCompleteRequired  (recovery: tick all nine required checks)
            //
            this.btnCompleteRequired.Location = new System.Drawing.Point(506, 4);
            this.btnCompleteRequired.Name = "btnCompleteRequired";
            this.btnCompleteRequired.Size = new System.Drawing.Size(210, 36);
            this.btnCompleteRequired.Text = "Complete all required checks";
            this.btnCompleteRequired.ToolTipText = "Ticks the nine required items (as if reviewed) — the status flips to READY at 9 / 9.";
            this.btnCompleteRequired.Click += new System.EventHandler(this.btnCompleteRequired_Click);
            //
            // btnResetChecks
            //
            this.btnResetChecks.Location = new System.Drawing.Point(724, 4);
            this.btnResetChecks.Name = "btnResetChecks";
            this.btnResetChecks.Size = new System.Drawing.Size(130, 36);
            this.btnResetChecks.Text = "Reset checklist";
            this.btnResetChecks.Click += new System.EventHandler(this.btnResetChecks_Click);
            //
            // chkSimulateError  (failure path c: packaging throws; the user sees a safe message)
            //
            this.chkSimulateError.Location = new System.Drawing.Point(870, 10);
            this.chkSimulateError.Name = "chkSimulateError";
            this.chkSimulateError.Size = new System.Drawing.Size(200, 24);
            this.chkSimulateError.Text = "Simulate packaging error";
            this.chkSimulateError.ToolTipText = "The service throws an IOException; the user sees a safe message, the log gets the detail.";
            //
            // btnClearLog
            //
            this.btnClearLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearLog.Location = new System.Drawing.Point(1178, 4);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(110, 36);
            this.btnClearLog.Text = "Clear log";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // ReleaseReviewWindow
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 760);
            this.Controls.Add(this.panelRelease);
            this.Controls.Add(this.panelChecklist);
            this.Controls.Add(this.panelSecrets);
            this.Controls.Add(this.panelSummary);
            this.Controls.Add(this.panelLog);
            this.Controls.Add(this.panelActions);
            this.Name = "ReleaseReviewWindow";
            this.Text = "WisejTrainingApp — Deployment review (Module 9)";
            this.Load += new System.EventHandler(this.ReleaseReviewWindow_Load);
            this.panelRelease.ResumeLayout(false);
            this.panelChecklist.ResumeLayout(false);
            this.panelSecrets.ResumeLayout(false);
            this.panelSummary.ResumeLayout(false);
            this.panelLog.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelRelease;
        private Wisej.Web.Label labelReleaseCard;
        private Wisej.Web.Label lblEnvironment;
        private Wisej.Web.ComboBox cboEnvironment;
        private Wisej.Web.Label lblTarget;
        private Wisej.Web.ComboBox cboTarget;
        private Wisej.Web.Label lblVersion;
        private Wisej.Web.TextBox txtVersion;
        private Wisej.Web.Label lblReviewer;
        private Wisej.Web.TextBox txtReviewer;
        private Wisej.Web.Label lblRole;
        private Wisej.Web.ComboBox cboRole;
        private Wisej.Web.Label lblPermission;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblHint;
        private Wisej.Web.Panel panelChecklist;
        private Wisej.Web.Label labelChecklistCard;
        private Wisej.Web.Label lblRequiredHead;
        private Wisej.Web.CheckedListBox chkRequired;
        private Wisej.Web.Label lblOptionalHead;
        private Wisej.Web.CheckedListBox chkOptional;
        private Wisej.Web.Label lblPackageStatus;
        private Wisej.Web.Label lblNotesHead;
        private Wisej.Web.TextBox txtNotes;
        private Wisej.Web.Panel panelSecrets;
        private Wisej.Web.Label labelSecretsCard;
        private Wisej.Web.Label lblLicenseKey;
        private Wisej.Web.Label lblConnectionString;
        private Wisej.Web.Button btnShowUnsafeExample;
        private Wisej.Web.Label lblSecretsHint;
        private Wisej.Web.Panel panelSummary;
        private Wisej.Web.Label labelSummaryCard;
        private Wisej.Web.TextBox txtSummary;
        private Wisej.Web.Panel panelLog;
        private Wisej.Web.Label labelLogCard;
        private Wisej.Web.ListBox lstLog;
        private Wisej.Web.Label labelLogFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button btnReviewPackage;
        private Wisej.Web.Button btnTryAsAgent;
        private Wisej.Web.Button btnCompleteRequired;
        private Wisej.Web.Button btnResetChecks;
        private Wisej.Web.CheckBox chkSimulateError;
        private Wisej.Web.Button btnClearLog;
    }
}
