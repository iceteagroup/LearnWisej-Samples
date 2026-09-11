namespace WisejTrainingApp
{
    partial class StatusPage
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
            this.widStatus = new Wisej.Web.Widget();
            this.pnlData = new Wisej.Web.Panel();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.lblStatusValue = new Wisej.Web.Label();
            this.lblOpenCaption = new Wisej.Web.Label();
            this.lblOpen = new Wisej.Web.Label();
            this.lblClosedCaption = new Wisej.Web.Label();
            this.lblClosed = new Wisej.Web.Label();
            this.lblLoadCaption = new Wisej.Web.Label();
            this.lblLoad = new Wisej.Web.Label();
            this.btnRefresh = new Wisej.Web.Button();
            this.btnSetHealthy = new Wisej.Web.Button();
            this.btnSetWarning = new Wisej.Web.Button();
            this.btnSetCritical = new Wisej.Web.Button();
            this.lblEventLog = new Wisej.Web.Label();
            this.lstEventLog = new Wisej.Web.ListBox();
            this.pnlData.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(24, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "System Status";
            //
            // widStatus
            //
            this.widStatus.Location = new System.Drawing.Point(24, 60);
            this.widStatus.Name = "widStatus";
            this.widStatus.Size = new System.Drawing.Size(420, 140);
            //
            // pnlData
            //
            this.pnlData.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlData.Controls.Add(this.lblStatusCaption);
            this.pnlData.Controls.Add(this.lblStatusValue);
            this.pnlData.Controls.Add(this.lblOpenCaption);
            this.pnlData.Controls.Add(this.lblOpen);
            this.pnlData.Controls.Add(this.lblClosedCaption);
            this.pnlData.Controls.Add(this.lblClosed);
            this.pnlData.Controls.Add(this.lblLoadCaption);
            this.pnlData.Controls.Add(this.lblLoad);
            this.pnlData.Location = new System.Drawing.Point(460, 60);
            this.pnlData.Name = "pnlData";
            this.pnlData.Size = new System.Drawing.Size(280, 140);
            //
            // lblStatusCaption
            //
            this.lblStatusCaption.AutoSize = true;
            this.lblStatusCaption.Location = new System.Drawing.Point(16, 14);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Text = "Status";
            //
            // lblStatusValue
            //
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatusValue.Location = new System.Drawing.Point(140, 14);
            this.lblStatusValue.Name = "lblStatusValue";
            //
            // lblOpenCaption
            //
            this.lblOpenCaption.AutoSize = true;
            this.lblOpenCaption.Location = new System.Drawing.Point(16, 44);
            this.lblOpenCaption.Name = "lblOpenCaption";
            this.lblOpenCaption.Text = "Open tickets";
            //
            // lblOpen
            //
            this.lblOpen.AutoSize = true;
            this.lblOpen.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblOpen.Location = new System.Drawing.Point(140, 44);
            this.lblOpen.Name = "lblOpen";
            //
            // lblClosedCaption
            //
            this.lblClosedCaption.AutoSize = true;
            this.lblClosedCaption.Location = new System.Drawing.Point(16, 74);
            this.lblClosedCaption.Name = "lblClosedCaption";
            this.lblClosedCaption.Text = "Closed tickets";
            //
            // lblClosed
            //
            this.lblClosed.AutoSize = true;
            this.lblClosed.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblClosed.Location = new System.Drawing.Point(140, 74);
            this.lblClosed.Name = "lblClosed";
            //
            // lblLoadCaption
            //
            this.lblLoadCaption.AutoSize = true;
            this.lblLoadCaption.Location = new System.Drawing.Point(16, 104);
            this.lblLoadCaption.Name = "lblLoadCaption";
            this.lblLoadCaption.Text = "System load";
            //
            // lblLoad
            //
            this.lblLoad.AutoSize = true;
            this.lblLoad.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblLoad.Location = new System.Drawing.Point(140, 104);
            this.lblLoad.Name = "lblLoad";
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(24, 216);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(170, 34);
            this.btnRefresh.Text = "Refresh Server Data";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnSetHealthy
            //
            this.btnSetHealthy.Location = new System.Drawing.Point(206, 216);
            this.btnSetHealthy.Name = "btnSetHealthy";
            this.btnSetHealthy.Size = new System.Drawing.Size(170, 34);
            this.btnSetHealthy.Text = "Set Healthy";
            this.btnSetHealthy.Click += new System.EventHandler(this.btnSetHealthy_Click);
            //
            // btnSetWarning
            //
            this.btnSetWarning.Location = new System.Drawing.Point(388, 216);
            this.btnSetWarning.Name = "btnSetWarning";
            this.btnSetWarning.Size = new System.Drawing.Size(170, 34);
            this.btnSetWarning.Text = "Set Warning";
            this.btnSetWarning.Click += new System.EventHandler(this.btnSetWarning_Click);
            //
            // btnSetCritical
            //
            this.btnSetCritical.Location = new System.Drawing.Point(570, 216);
            this.btnSetCritical.Name = "btnSetCritical";
            this.btnSetCritical.Size = new System.Drawing.Size(170, 34);
            this.btnSetCritical.Text = "Set Critical";
            this.btnSetCritical.Click += new System.EventHandler(this.btnSetCritical_Click);
            //
            // lblEventLog
            //
            this.lblEventLog.AutoSize = true;
            this.lblEventLog.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblEventLog.Location = new System.Drawing.Point(24, 270);
            this.lblEventLog.Name = "lblEventLog";
            this.lblEventLog.Text = "Event Log";
            //
            // lstEventLog
            //
            this.lstEventLog.Location = new System.Drawing.Point(24, 298);
            this.lstEventLog.Name = "lstEventLog";
            this.lstEventLog.Size = new System.Drawing.Size(716, 220);
            //
            // StatusPage
            //
            this.Controls.Add(this.lstEventLog);
            this.Controls.Add(this.lblEventLog);
            this.Controls.Add(this.btnSetCritical);
            this.Controls.Add(this.btnSetWarning);
            this.Controls.Add(this.btnSetHealthy);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.pnlData);
            this.Controls.Add(this.widStatus);
            this.Controls.Add(this.lblTitle);
            this.Name = "StatusPage";
            this.Size = new System.Drawing.Size(1000, 600);
            this.Text = "System Status";
            this.Load += new System.EventHandler(this.StatusPage_Load);
            this.pnlData.ResumeLayout(false);
            this.pnlData.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Widget widStatus;
        private Wisej.Web.Panel pnlData;
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.Label lblStatusValue;
        private Wisej.Web.Label lblOpenCaption;
        private Wisej.Web.Label lblOpen;
        private Wisej.Web.Label lblClosedCaption;
        private Wisej.Web.Label lblClosed;
        private Wisej.Web.Label lblLoadCaption;
        private Wisej.Web.Label lblLoad;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Button btnSetHealthy;
        private Wisej.Web.Button btnSetWarning;
        private Wisej.Web.Button btnSetCritical;
        private Wisej.Web.Label lblEventLog;
        private Wisej.Web.ListBox lstEventLog;
    }
}
