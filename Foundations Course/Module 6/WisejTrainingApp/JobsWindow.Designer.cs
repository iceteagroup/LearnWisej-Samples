namespace WisejTrainingApp
{
    partial class JobsWindow
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
            this.btnStartImport = new Wisej.Web.Button();
            this.btnStartExport = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.btnClearLog = new Wisej.Web.Button();
            this.chkSimulateError = new Wisej.Web.CheckBox();
            this.progressBar = new Wisej.Web.ProgressBar();
            this.lblStatus = new Wisej.Web.Label();
            this.lstLog = new Wisej.Web.ListBox();
            this.SuspendLayout();
            //
            // btnStartImport
            //
            this.btnStartImport.Location = new System.Drawing.Point(20, 20);
            this.btnStartImport.Name = "btnStartImport";
            this.btnStartImport.Size = new System.Drawing.Size(130, 34);
            this.btnStartImport.Text = "Start Import";
            this.btnStartImport.Click += new System.EventHandler(this.btnStartImport_Click);
            //
            // btnStartExport
            //
            this.btnStartExport.Location = new System.Drawing.Point(158, 20);
            this.btnStartExport.Name = "btnStartExport";
            this.btnStartExport.Size = new System.Drawing.Size(130, 34);
            this.btnStartExport.Text = "Start Export";
            this.btnStartExport.Click += new System.EventHandler(this.btnStartExport_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(296, 20);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(130, 34);
            this.btnCancel.Text = "Cancel Job";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnClearLog
            //
            this.btnClearLog.Location = new System.Drawing.Point(490, 20);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(130, 34);
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // chkSimulateError
            //
            this.chkSimulateError.AutoSize = true;
            this.chkSimulateError.Location = new System.Drawing.Point(20, 66);
            this.chkSimulateError.Name = "chkSimulateError";
            this.chkSimulateError.Text = "Simulate Error";
            //
            // progressBar
            //
            this.progressBar.Location = new System.Drawing.Point(20, 100);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(600, 20);
            //
            // lblStatus
            //
            this.lblStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(20, 130);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(600, 24);
            this.lblStatus.Text = "Ready.";
            //
            // lstLog
            //
            this.lstLog.Location = new System.Drawing.Point(20, 164);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(600, 240);
            //
            // JobsWindow
            //
            this.ClientSize = new System.Drawing.Size(640, 424);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.chkSimulateError);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnStartExport);
            this.Controls.Add(this.btnStartImport);
            this.Name = "JobsWindow";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "Background Jobs";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Button btnStartImport;
        private Wisej.Web.Button btnStartExport;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Button btnClearLog;
        private Wisej.Web.CheckBox chkSimulateError;
        private Wisej.Web.ProgressBar progressBar;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.ListBox lstLog;
    }
}
