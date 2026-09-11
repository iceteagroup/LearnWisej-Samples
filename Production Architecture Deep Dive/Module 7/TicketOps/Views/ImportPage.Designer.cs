namespace TicketOps.Views
{
    partial class ImportPage
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
            this.panelScreen = new Wisej.Web.Panel();
            this.labelScreenTitle = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.labelFile = new Wisej.Web.Label();
            this.buttonStartImport = new Wisej.Web.Button();
            this.buttonCancelImport = new Wisej.Web.Button();
            this.buttonRefreshList = new Wisej.Web.Button();
            this.labelTicketCount = new Wisej.Web.Label();
            this.labelProgressCaption = new Wisej.Web.Label();
            this.labelRowsDone = new Wisej.Web.Label();
            this.labelPercent = new Wisej.Web.Label();
            this.progressImport = new Wisej.Web.ProgressBar();
            this.labelLogCaption = new Wisej.Web.Label();
            this.listImportLog = new Wisej.Web.ListBox();
            this.panelScreen.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.labelFile);
            this.panelScreen.Controls.Add(this.buttonStartImport);
            this.panelScreen.Controls.Add(this.buttonCancelImport);
            this.panelScreen.Controls.Add(this.buttonRefreshList);
            this.panelScreen.Controls.Add(this.labelTicketCount);
            this.panelScreen.Controls.Add(this.labelProgressCaption);
            this.panelScreen.Controls.Add(this.labelRowsDone);
            this.panelScreen.Controls.Add(this.labelPercent);
            this.panelScreen.Controls.Add(this.progressImport);
            this.panelScreen.Controls.Add(this.labelLogCaption);
            this.panelScreen.Controls.Add(this.listImportLog);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 520);
            //
            // labelScreenTitle
            //
            this.labelScreenTitle.AutoSize = false;
            this.labelScreenTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelScreenTitle.Location = new System.Drawing.Point(24, 18);
            this.labelScreenTitle.Name = "labelScreenTitle";
            this.labelScreenTitle.Size = new System.Drawing.Size(300, 30);
            this.labelScreenTitle.Text = "Ticket Import (CSV)";
            //
            // statusBanner
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // labelFile
            //
            this.labelFile.AutoSize = false;
            this.labelFile.Font = new System.Drawing.Font("monospace", 9F);
            this.labelFile.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelFile.Location = new System.Drawing.Point(24, 52);
            this.labelFile.Name = "labelFile";
            this.labelFile.Size = new System.Drawing.Size(712, 20);
            this.labelFile.Text = "opening the import file…";
            //
            // buttonStartImport
            //
            this.buttonStartImport.Location = new System.Drawing.Point(24, 86);
            this.buttonStartImport.Name = "buttonStartImport";
            this.buttonStartImport.Size = new System.Drawing.Size(180, 36);
            this.buttonStartImport.Text = "▶ Start import";
            this.buttonStartImport.Click += new System.EventHandler(this.buttonStartImport_Click);
            //
            // buttonCancelImport
            //
            this.buttonCancelImport.Enabled = false;
            this.buttonCancelImport.Location = new System.Drawing.Point(214, 86);
            this.buttonCancelImport.Name = "buttonCancelImport";
            this.buttonCancelImport.Size = new System.Drawing.Size(120, 36);
            this.buttonCancelImport.Text = "⏹ Cancel";
            this.buttonCancelImport.Click += new System.EventHandler(this.buttonCancelImport_Click);
            //
            // buttonRefreshList
            //
            this.buttonRefreshList.Location = new System.Drawing.Point(344, 86);
            this.buttonRefreshList.Name = "buttonRefreshList";
            this.buttonRefreshList.Size = new System.Drawing.Size(140, 36);
            this.buttonRefreshList.Text = "↻ Refresh list";
            this.buttonRefreshList.Click += new System.EventHandler(this.buttonRefreshList_Click);
            //
            // labelTicketCount
            //
            this.labelTicketCount.AutoSize = false;
            this.labelTicketCount.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelTicketCount.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.labelTicketCount.Location = new System.Drawing.Point(494, 92);
            this.labelTicketCount.Name = "labelTicketCount";
            this.labelTicketCount.Size = new System.Drawing.Size(242, 24);
            this.labelTicketCount.Text = "… tickets in the repository";
            this.labelTicketCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // labelProgressCaption
            //
            this.labelProgressCaption.AutoSize = false;
            this.labelProgressCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelProgressCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelProgressCaption.Location = new System.Drawing.Point(24, 136);
            this.labelProgressCaption.Name = "labelProgressCaption";
            this.labelProgressCaption.Size = new System.Drawing.Size(120, 20);
            this.labelProgressCaption.Text = "PROGRESS";
            //
            // labelRowsDone
            //
            this.labelRowsDone.AutoSize = false;
            this.labelRowsDone.Font = new System.Drawing.Font("monospace", 9F);
            this.labelRowsDone.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelRowsDone.Location = new System.Drawing.Point(150, 136);
            this.labelRowsDone.Name = "labelRowsDone";
            this.labelRowsDone.Size = new System.Drawing.Size(500, 20);
            this.labelRowsDone.Text = "0 of 0 rows";
            this.labelRowsDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // labelPercent
            //
            this.labelPercent.AutoSize = false;
            this.labelPercent.Font = new System.Drawing.Font("monospace", 10F, System.Drawing.FontStyle.Bold);
            this.labelPercent.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.labelPercent.Location = new System.Drawing.Point(656, 136);
            this.labelPercent.Name = "labelPercent";
            this.labelPercent.Size = new System.Drawing.Size(80, 20);
            this.labelPercent.Text = "0%";
            this.labelPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // progressImport
            //
            this.progressImport.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.progressImport.Location = new System.Drawing.Point(24, 160);
            this.progressImport.Maximum = 100;
            this.progressImport.Name = "progressImport";
            this.progressImport.Size = new System.Drawing.Size(712, 18);
            //
            // labelLogCaption
            //
            this.labelLogCaption.AutoSize = false;
            this.labelLogCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelLogCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelLogCaption.Location = new System.Drawing.Point(24, 190);
            this.labelLogCaption.Name = "labelLogCaption";
            this.labelLogCaption.Size = new System.Drawing.Size(400, 20);
            this.labelLogCaption.Text = "IMPORT LOG";
            //
            // listImportLog
            //
            this.listImportLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listImportLog.Font = new System.Drawing.Font("monospace", 9F);
            this.listImportLog.Location = new System.Drawing.Point(24, 212);
            this.listImportLog.Name = "listImportLog";
            this.listImportLog.Size = new System.Drawing.Size(712, 284);
            //
            // ImportPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(820, 580);
            this.Controls.Add(this.panelScreen);
            this.Name = "ImportPage";
            this.Text = "TicketOps Console";
            this.Load += new System.EventHandler(this.ImportPage_Load);
            this.panelScreen.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelFile;
        private Wisej.Web.Button buttonStartImport;
        private Wisej.Web.Button buttonCancelImport;
        private Wisej.Web.Button buttonRefreshList;
        private Wisej.Web.Label labelTicketCount;
        private Wisej.Web.Label labelProgressCaption;
        private Wisej.Web.Label labelRowsDone;
        private Wisej.Web.Label labelPercent;
        private Wisej.Web.ProgressBar progressImport;
        private Wisej.Web.Label labelLogCaption;
        private Wisej.Web.ListBox listImportLog;
    }
}
