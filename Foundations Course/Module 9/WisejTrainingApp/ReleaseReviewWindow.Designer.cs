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
            this.lblTitle = new Wisej.Web.Label();
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
            this.lblLicenseKey = new Wisej.Web.Label();
            this.lblNotesHead = new Wisej.Web.Label();
            this.txtNotes = new Wisej.Web.TextBox();
            this.lblRequiredHead = new Wisej.Web.Label();
            this.chkRequired = new Wisej.Web.CheckedListBox();
            this.lblOptionalHead = new Wisej.Web.Label();
            this.chkOptional = new Wisej.Web.CheckedListBox();
            this.lblPackageStatus = new Wisej.Web.Label();
            this.btnReviewPackage = new Wisej.Web.Button();
            this.txtSummary = new Wisej.Web.TextBox();
            this.lblLogHead = new Wisej.Web.Label();
            this.lstLog = new Wisej.Web.ListBox();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "Deployment Review";
            //
            // lblEnvironment
            //
            this.lblEnvironment.AutoSize = true;
            this.lblEnvironment.Location = new System.Drawing.Point(20, 62);
            this.lblEnvironment.Name = "lblEnvironment";
            this.lblEnvironment.Text = "Environment";
            //
            // cboEnvironment
            //
            this.cboEnvironment.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboEnvironment.Items.AddRange(new object[] { "Debug", "Staging", "Production" });
            this.cboEnvironment.Location = new System.Drawing.Point(140, 56);
            this.cboEnvironment.Name = "cboEnvironment";
            this.cboEnvironment.Size = new System.Drawing.Size(280, 30);
            //
            // lblTarget
            //
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(20, 98);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Text = "Target";
            //
            // cboTarget
            //
            this.cboTarget.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTarget.Items.AddRange(new object[] { "IIS", "Kestrel", "Cloud" });
            this.cboTarget.Location = new System.Drawing.Point(140, 92);
            this.cboTarget.Name = "cboTarget";
            this.cboTarget.Size = new System.Drawing.Size(280, 30);
            this.cboTarget.SelectedIndexChanged += new System.EventHandler(this.cboTarget_SelectedIndexChanged);
            //
            // lblVersion
            //
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(20, 134);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Text = "Version";
            //
            // txtVersion
            //
            this.txtVersion.Location = new System.Drawing.Point(140, 128);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(280, 30);
            this.txtVersion.Text = "1.0.0";
            //
            // lblReviewer
            //
            this.lblReviewer.AutoSize = true;
            this.lblReviewer.Location = new System.Drawing.Point(20, 170);
            this.lblReviewer.Name = "lblReviewer";
            this.lblReviewer.Text = "Reviewer";
            //
            // txtReviewer
            //
            this.txtReviewer.Location = new System.Drawing.Point(140, 164);
            this.txtReviewer.Name = "txtReviewer";
            this.txtReviewer.Size = new System.Drawing.Size(280, 30);
            this.txtReviewer.Text = "Jamie Lee";
            //
            // lblRole
            //
            this.lblRole.AutoSize = true;
            this.lblRole.Location = new System.Drawing.Point(20, 206);
            this.lblRole.Name = "lblRole";
            this.lblRole.Text = "Role";
            //
            // cboRole
            //
            this.cboRole.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboRole.Items.AddRange(new object[] { "Support Agent", "Team Lead", "Admin" });
            this.cboRole.Location = new System.Drawing.Point(140, 200);
            this.cboRole.Name = "cboRole";
            this.cboRole.Size = new System.Drawing.Size(280, 30);
            this.cboRole.SelectedIndexChanged += new System.EventHandler(this.cboRole_SelectedIndexChanged);
            //
            // lblPermission
            //
            this.lblPermission.AutoSize = true;
            this.lblPermission.Location = new System.Drawing.Point(140, 238);
            this.lblPermission.Name = "lblPermission";
            //
            // lblLicenseKey
            //
            this.lblLicenseKey.Location = new System.Drawing.Point(20, 270);
            this.lblLicenseKey.Name = "lblLicenseKey";
            this.lblLicenseKey.Size = new System.Drawing.Size(400, 22);
            //
            // lblNotesHead
            //
            this.lblNotesHead.AutoSize = true;
            this.lblNotesHead.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblNotesHead.Location = new System.Drawing.Point(20, 304);
            this.lblNotesHead.Name = "lblNotesHead";
            this.lblNotesHead.Text = "Deployment notes";
            //
            // txtNotes
            //
            this.txtNotes.Location = new System.Drawing.Point(20, 328);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(400, 110);
            //
            // lblRequiredHead
            //
            this.lblRequiredHead.AutoSize = true;
            this.lblRequiredHead.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblRequiredHead.Location = new System.Drawing.Point(460, 56);
            this.lblRequiredHead.Name = "lblRequiredHead";
            this.lblRequiredHead.Text = "Required checks";
            //
            // chkRequired
            //
            this.chkRequired.CheckOnClick = true;
            this.chkRequired.Items.AddRange(new object[] {
            "Web.config reviewed and debug mode set correctly",
            "Wisej.NET license key handled securely",
            "Default/startup window, theme and URL settings checked",
            "Themes and static resources included",
            "Logging destination configured",
            "Authentication and authorization plan reviewed",
            "Sensitive files are not publicly downloadable",
            "Release build tested locally before deployment",
            "Deployment target requirements checked"});
            this.chkRequired.Location = new System.Drawing.Point(460, 80);
            this.chkRequired.Name = "chkRequired";
            this.chkRequired.Size = new System.Drawing.Size(420, 204);
            this.chkRequired.AfterItemCheck += new Wisej.Web.ItemCheckEventHandler(this.chkRequired_AfterItemCheck);
            //
            // lblOptionalHead
            //
            this.lblOptionalHead.AutoSize = true;
            this.lblOptionalHead.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblOptionalHead.Location = new System.Drawing.Point(460, 296);
            this.lblOptionalHead.Name = "lblOptionalHead";
            this.lblOptionalHead.Text = "Optional checks";
            //
            // chkOptional
            //
            this.chkOptional.CheckOnClick = true;
            this.chkOptional.Items.AddRange(new object[] {
            "Theme Builder tweaks reviewed",
            "Screenshots redacted",
            "Staging smoke test done",
            "Rollback plan written"});
            this.chkOptional.Location = new System.Drawing.Point(460, 320);
            this.chkOptional.Name = "chkOptional";
            this.chkOptional.Size = new System.Drawing.Size(420, 100);
            //
            // lblPackageStatus
            //
            this.lblPackageStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblPackageStatus.Location = new System.Drawing.Point(460, 428);
            this.lblPackageStatus.Name = "lblPackageStatus";
            this.lblPackageStatus.Size = new System.Drawing.Size(420, 22);
            //
            // btnReviewPackage
            //
            this.btnReviewPackage.Location = new System.Drawing.Point(20, 456);
            this.btnReviewPackage.Name = "btnReviewPackage";
            this.btnReviewPackage.Size = new System.Drawing.Size(200, 34);
            this.btnReviewPackage.Text = "Create Review Package";
            this.btnReviewPackage.Click += new System.EventHandler(this.btnReviewPackage_Click);
            //
            // txtSummary
            //
            this.txtSummary.Location = new System.Drawing.Point(20, 500);
            this.txtSummary.Multiline = true;
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ReadOnly = true;
            this.txtSummary.Size = new System.Drawing.Size(400, 140);
            //
            // lblLogHead
            //
            this.lblLogHead.AutoSize = true;
            this.lblLogHead.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblLogHead.Location = new System.Drawing.Point(460, 468);
            this.lblLogHead.Name = "lblLogHead";
            this.lblLogHead.Text = "Troubleshooting log";
            //
            // lstLog
            //
            this.lstLog.Location = new System.Drawing.Point(460, 492);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(420, 148);
            //
            // ReleaseReviewWindow
            //
            this.ClientSize = new System.Drawing.Size(900, 660);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.lblLogHead);
            this.Controls.Add(this.txtSummary);
            this.Controls.Add(this.btnReviewPackage);
            this.Controls.Add(this.lblPackageStatus);
            this.Controls.Add(this.chkOptional);
            this.Controls.Add(this.lblOptionalHead);
            this.Controls.Add(this.chkRequired);
            this.Controls.Add(this.lblRequiredHead);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.lblNotesHead);
            this.Controls.Add(this.lblLicenseKey);
            this.Controls.Add(this.lblPermission);
            this.Controls.Add(this.cboRole);
            this.Controls.Add(this.lblRole);
            this.Controls.Add(this.txtReviewer);
            this.Controls.Add(this.lblReviewer);
            this.Controls.Add(this.txtVersion);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.cboTarget);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.cboEnvironment);
            this.Controls.Add(this.lblEnvironment);
            this.Controls.Add(this.lblTitle);
            this.Name = "ReleaseReviewWindow";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "Deployment Review";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblTitle;
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
        private Wisej.Web.Label lblLicenseKey;
        private Wisej.Web.Label lblNotesHead;
        private Wisej.Web.TextBox txtNotes;
        private Wisej.Web.Label lblRequiredHead;
        private Wisej.Web.CheckedListBox chkRequired;
        private Wisej.Web.Label lblOptionalHead;
        private Wisej.Web.CheckedListBox chkOptional;
        private Wisej.Web.Label lblPackageStatus;
        private Wisej.Web.Button btnReviewPackage;
        private Wisej.Web.TextBox txtSummary;
        private Wisej.Web.Label lblLogHead;
        private Wisej.Web.ListBox lstLog;
    }
}
