namespace WisejTrainingApp.Views
{
    partial class JobsView
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
            this.pnlJob = new Wisej.Web.Panel();
            this.labelJobCard = new Wisej.Web.Label();
            this.lblJobDescription = new Wisej.Web.Label();
            this.btnStartJob = new Wisej.Web.Button();
            this.btnCancelJob = new Wisej.Web.Button();
            this.chkSimulateError = new Wisej.Web.CheckBox();
            this.progressBar = new Wisej.Web.ProgressBar();
            this.lblProgress = new Wisej.Web.Label();
            this.labelLogCaption = new Wisej.Web.Label();
            this.lstJobLog = new Wisej.Web.ListBox();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlJob.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlJob
            //
            this.pnlJob.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlJob.BackColor = System.Drawing.Color.White;
            this.pnlJob.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlJob.Controls.Add(this.labelJobCard);
            this.pnlJob.Controls.Add(this.lblJobDescription);
            this.pnlJob.Controls.Add(this.btnStartJob);
            this.pnlJob.Controls.Add(this.btnCancelJob);
            this.pnlJob.Controls.Add(this.chkSimulateError);
            this.pnlJob.Controls.Add(this.progressBar);
            this.pnlJob.Controls.Add(this.lblProgress);
            this.pnlJob.Controls.Add(this.labelLogCaption);
            this.pnlJob.Controls.Add(this.lstJobLog);
            this.pnlJob.Controls.Add(this.lblStatus);
            this.pnlJob.Location = new System.Drawing.Point(0, 0);
            this.pnlJob.Name = "pnlJob";
            this.pnlJob.Size = new System.Drawing.Size(1032, 532);
            //
            // labelJobCard
            //
            this.labelJobCard.AutoSize = false;
            this.labelJobCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelJobCard.Location = new System.Drawing.Point(24, 14);
            this.labelJobCard.Name = "labelJobCard";
            this.labelJobCard.Size = new System.Drawing.Size(600, 28);
            this.labelJobCard.Text = "Background job  ·  weekly ticket digest (async / await + Application.Update)";
            //
            // lblJobDescription
            //
            this.lblJobDescription.AutoSize = false;
            this.lblJobDescription.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblJobDescription.Location = new System.Drawing.Point(24, 44);
            this.lblJobDescription.Name = "lblJobDescription";
            this.lblJobDescription.Size = new System.Drawing.Size(984, 22);
            this.lblJobDescription.Text = "Ten steps, 450 ms each. The browser stays responsive; every step is pushed as it happens. Tick the box to see the failure path at step 6.";
            //
            // btnStartJob  (primary)
            //
            this.btnStartJob.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnStartJob.Location = new System.Drawing.Point(24, 80);
            this.btnStartJob.Name = "btnStartJob";
            this.btnStartJob.Size = new System.Drawing.Size(140, 36);
            this.btnStartJob.Text = "Start job";
            this.btnStartJob.ToolTipText = "async btnStartJob_Click: try / catch / finally, Application.Update(this) per step.";
            this.btnStartJob.Click += new System.EventHandler(this.btnStartJob_Click);
            //
            // btnCancelJob
            //
            this.btnCancelJob.Enabled = false;
            this.btnCancelJob.Location = new System.Drawing.Point(172, 80);
            this.btnCancelJob.Name = "btnCancelJob";
            this.btnCancelJob.Size = new System.Drawing.Size(120, 36);
            this.btnCancelJob.Text = "Cancel";
            this.btnCancelJob.ToolTipText = "CancellationTokenSource.Cancel() — the loop stops at its next await.";
            this.btnCancelJob.Click += new System.EventHandler(this.btnCancelJob_Click);
            //
            // chkSimulateError  (failure path)
            //
            this.chkSimulateError.Location = new System.Drawing.Point(316, 84);
            this.chkSimulateError.Name = "chkSimulateError";
            this.chkSimulateError.Size = new System.Drawing.Size(360, 28);
            this.chkSimulateError.Text = "Simulate a mail-relay error at step 6";
            //
            // progressBar
            //
            this.progressBar.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.progressBar.Location = new System.Drawing.Point(24, 132);
            this.progressBar.Maximum = 100;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(984, 24);
            //
            // lblProgress
            //
            this.lblProgress.AutoSize = false;
            this.lblProgress.Font = new System.Drawing.Font("monospace", 9F);
            this.lblProgress.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblProgress.Location = new System.Drawing.Point(24, 160);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(400, 22);
            this.lblProgress.Text = "0% — idle";
            //
            // labelLogCaption
            //
            this.labelLogCaption.AutoSize = false;
            this.labelLogCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelLogCaption.Location = new System.Drawing.Point(24, 192);
            this.labelLogCaption.Name = "labelLogCaption";
            this.labelLogCaption.Size = new System.Drawing.Size(600, 24);
            this.labelLogCaption.Text = "Job log  ·  the user gets the short message; the detail stays here";
            //
            // lstJobLog
            //
            this.lstJobLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstJobLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstJobLog.Location = new System.Drawing.Point(24, 220);
            this.lstJobLog.Name = "lstJobLog";
            this.lstJobLog.Size = new System.Drawing.Size(984, 244);
            //
            // lblStatus
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(24, 476);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(984, 26);
            this.lblStatus.Text = "● idle — click Start job";
            //
            // JobsView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlJob);
            this.Name = "JobsView";
            this.Size = new System.Drawing.Size(1032, 532);
            this.pnlJob.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlJob;
        private Wisej.Web.Label labelJobCard;
        private Wisej.Web.Label lblJobDescription;
        private Wisej.Web.Button btnStartJob;
        private Wisej.Web.Button btnCancelJob;
        private Wisej.Web.CheckBox chkSimulateError;
        private Wisej.Web.ProgressBar progressBar;
        private Wisej.Web.Label lblProgress;
        private Wisej.Web.Label labelLogCaption;
        private Wisej.Web.ListBox lstJobLog;
        private Wisej.Web.Label lblStatus;
    }
}
