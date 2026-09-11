namespace WisejTrainingApp
{
    partial class DashboardWindow
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
            this.lblStatus = new Wisej.Web.Label();
            this.pnlServices = new Wisej.Web.Panel();
            this.lblServerStatus = new Wisej.Web.Label();
            this.lblDatabaseStatus = new Wisej.Web.Label();
            this.lblApiStatus = new Wisej.Web.Label();
            this.btnStart = new Wisej.Web.Button();
            this.btnStop = new Wisej.Web.Button();
            this.btnReset = new Wisej.Web.Button();
            this.btnRefresh = new Wisej.Web.Button();
            this.lblEventLog = new Wisej.Web.Label();
            this.lstEventLog = new Wisej.Web.ListBox();
            this.pnlServices.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(24, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(512, 40);
            this.lblTitle.Text = "System Dashboard";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(24, 66);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(512, 26);
            this.lblStatus.Text = "Status: Idle";
            //
            // pnlServices
            //
            this.pnlServices.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlServices.Controls.Add(this.lblServerStatus);
            this.pnlServices.Controls.Add(this.lblDatabaseStatus);
            this.pnlServices.Controls.Add(this.lblApiStatus);
            this.pnlServices.Location = new System.Drawing.Point(24, 100);
            this.pnlServices.Name = "pnlServices";
            this.pnlServices.Size = new System.Drawing.Size(512, 118);
            //
            // lblServerStatus
            //
            this.lblServerStatus.AutoSize = false;
            this.lblServerStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblServerStatus.Location = new System.Drawing.Point(16, 12);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(480, 26);
            this.lblServerStatus.Text = "Server: Offline";
            //
            // lblDatabaseStatus
            //
            this.lblDatabaseStatus.AutoSize = false;
            this.lblDatabaseStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblDatabaseStatus.Location = new System.Drawing.Point(16, 46);
            this.lblDatabaseStatus.Name = "lblDatabaseStatus";
            this.lblDatabaseStatus.Size = new System.Drawing.Size(480, 26);
            this.lblDatabaseStatus.Text = "Database: Offline";
            //
            // lblApiStatus
            //
            this.lblApiStatus.AutoSize = false;
            this.lblApiStatus.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblApiStatus.Location = new System.Drawing.Point(16, 80);
            this.lblApiStatus.Name = "lblApiStatus";
            this.lblApiStatus.Size = new System.Drawing.Size(480, 26);
            this.lblApiStatus.Text = "API Service: Offline";
            //
            // btnStart
            //
            this.btnStart.Location = new System.Drawing.Point(24, 232);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(122, 36);
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            //
            // btnStop
            //
            this.btnStop.Location = new System.Drawing.Point(154, 232);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(122, 36);
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            //
            // btnReset
            //
            this.btnReset.Location = new System.Drawing.Point(284, 232);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(122, 36);
            this.btnReset.Text = "Reset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(414, 232);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(122, 36);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // lblEventLog
            //
            this.lblEventLog.AutoSize = false;
            this.lblEventLog.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblEventLog.Location = new System.Drawing.Point(568, 66);
            this.lblEventLog.Name = "lblEventLog";
            this.lblEventLog.Size = new System.Drawing.Size(420, 26);
            this.lblEventLog.Text = "Event Log";
            //
            // lstEventLog
            //
            this.lstEventLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstEventLog.Location = new System.Drawing.Point(568, 100);
            this.lstEventLog.Name = "lstEventLog";
            this.lstEventLog.Size = new System.Drawing.Size(420, 168);
            //
            // DashboardWindow
            //
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1012, 292);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlServices);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lblEventLog);
            this.Controls.Add(this.lstEventLog);
            this.Name = "DashboardWindow";
            this.Text = "System Dashboard";
            this.pnlServices.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlServices;
        private Wisej.Web.Label lblServerStatus;
        private Wisej.Web.Label lblDatabaseStatus;
        private Wisej.Web.Label lblApiStatus;
        private Wisej.Web.Button btnStart;
        private Wisej.Web.Button btnStop;
        private Wisej.Web.Button btnReset;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Label lblEventLog;
        private Wisej.Web.ListBox lstEventLog;
    }
}
