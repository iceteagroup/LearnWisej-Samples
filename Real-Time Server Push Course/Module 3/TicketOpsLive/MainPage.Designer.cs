namespace TicketOpsLive
{
    partial class MainPage
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
            this.panelImport = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.jobIdLabel = new Wisej.Web.Label();
            this.elapsedLabel = new Wisej.Web.Label();
            this.importStatusLabel = new Wisej.Web.Label();
            this.importProgressBar = new Wisej.Web.ProgressBar();
            this.labelRecordsCaption = new Wisej.Web.Label();
            this.recordsImportedLabel = new Wisej.Web.Label();
            this.failAt87CheckBox = new Wisej.Web.CheckBox();
            this.labelLogCaption = new Wisej.Web.Label();
            this.importLogListBox = new Wisej.Web.ListBox();
            this.labelBanner = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.startImportButton = new Wisej.Web.Button();
            this.cancelImportButton = new Wisej.Web.Button();
            this.blockingImportButton = new Wisej.Web.Button();
            this.clearLogButton = new Wisej.Web.Button();
            this.clearButton = new Wisej.Web.Button();
            this.panelImport.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelImport  (the Import Monitor — lab UI requirements)
            //
            this.panelImport.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelImport.BackColor = System.Drawing.Color.White;
            this.panelImport.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelImport.Controls.Add(this.labelTitle);
            this.panelImport.Controls.Add(this.labelStatus);
            this.panelImport.Controls.Add(this.jobIdLabel);
            this.panelImport.Controls.Add(this.elapsedLabel);
            this.panelImport.Controls.Add(this.importStatusLabel);
            this.panelImport.Controls.Add(this.importProgressBar);
            this.panelImport.Controls.Add(this.labelRecordsCaption);
            this.panelImport.Controls.Add(this.recordsImportedLabel);
            this.panelImport.Controls.Add(this.failAt87CheckBox);
            this.panelImport.Controls.Add(this.labelLogCaption);
            this.panelImport.Controls.Add(this.importLogListBox);
            this.panelImport.Controls.Add(this.labelBanner);
            this.panelImport.Controls.Add(this.labelState);
            this.panelImport.Location = new System.Drawing.Point(30, 18);
            this.panelImport.Name = "panelImport";
            this.panelImport.Size = new System.Drawing.Size(700, 580);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 16);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(360, 30);
            this.labelTitle.Text = "Import Monitor";
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(390, 18);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(286, 26);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // jobIdLabel / elapsedLabel  (extension challenge: the JobId; lab: elapsed time)
            //
            this.jobIdLabel.AutoSize = false;
            this.jobIdLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.jobIdLabel.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.jobIdLabel.Location = new System.Drawing.Point(24, 58);
            this.jobIdLabel.Name = "jobIdLabel";
            this.jobIdLabel.Size = new System.Drawing.Size(420, 20);
            this.jobIdLabel.Text = "Job —";
            this.elapsedLabel.AutoSize = false;
            this.elapsedLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.elapsedLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.elapsedLabel.Location = new System.Drawing.Point(456, 58);
            this.elapsedLabel.Name = "elapsedLabel";
            this.elapsedLabel.Size = new System.Drawing.Size(220, 20);
            this.elapsedLabel.Text = "elapsed 0.00 s";
            this.elapsedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // importStatusLabel  (pushed every 10 records + the final state)
            //
            this.importStatusLabel.AutoSize = false;
            this.importStatusLabel.Font = new System.Drawing.Font("default", 11F);
            this.importStatusLabel.Location = new System.Drawing.Point(24, 82);
            this.importStatusLabel.Name = "importStatusLabel";
            this.importStatusLabel.Size = new System.Drawing.Size(652, 26);
            this.importStatusLabel.Text = "Idle — click ▶ Start import";
            //
            // importProgressBar  (Value = i / 2 → 0..100 for 200 records)
            //
            this.importProgressBar.Location = new System.Drawing.Point(24, 118);
            this.importProgressBar.Name = "importProgressBar";
            this.importProgressBar.Size = new System.Drawing.Size(500, 26);
            this.importProgressBar.Value = 0;
            //
            // labelRecordsCaption / recordsImportedLabel
            //
            this.labelRecordsCaption.AutoSize = false;
            this.labelRecordsCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelRecordsCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelRecordsCaption.Location = new System.Drawing.Point(536, 104);
            this.labelRecordsCaption.Name = "labelRecordsCaption";
            this.labelRecordsCaption.Size = new System.Drawing.Size(140, 16);
            this.labelRecordsCaption.Text = "RECORDS IMPORTED";
            this.labelRecordsCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.recordsImportedLabel.AutoSize = false;
            this.recordsImportedLabel.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.recordsImportedLabel.Location = new System.Drawing.Point(536, 118);
            this.recordsImportedLabel.Name = "recordsImportedLabel";
            this.recordsImportedLabel.Size = new System.Drawing.Size(140, 30);
            this.recordsImportedLabel.Text = "0";
            this.recordsImportedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // failAt87CheckBox  (failure path — lab task 5)
            //
            this.failAt87CheckBox.AutoSize = false;
            this.failAt87CheckBox.Font = new System.Drawing.Font("default", 10F);
            this.failAt87CheckBox.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.failAt87CheckBox.Location = new System.Drawing.Point(24, 156);
            this.failAt87CheckBox.Name = "failAt87CheckBox";
            this.failAt87CheckBox.Size = new System.Drawing.Size(652, 24);
            this.failAt87CheckBox.Text = "Throw at record 87 — simulated malformed record";
            this.failAt87CheckBox.ToolTipText = "The next Start import throws InvalidOperationException inside the task at record 87: caught, logged with the JobId, safe message, UI restored in finally.";
            //
            // labelLogCaption / importLogListBox  (newest line first; every line carries the JobId)
            //
            this.labelLogCaption.AutoSize = false;
            this.labelLogCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelLogCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLogCaption.Location = new System.Drawing.Point(24, 190);
            this.labelLogCaption.Name = "labelLogCaption";
            this.labelLogCaption.Size = new System.Drawing.Size(652, 20);
            this.labelLogCaption.Text = "import log   newest first · start, every 50 records, cancellation, failure, completion, summary — all with the JobId";
            this.importLogListBox.Font = new System.Drawing.Font("monospace", 9F);
            this.importLogListBox.Location = new System.Drawing.Point(24, 212);
            this.importLogListBox.Name = "importLogListBox";
            this.importLogListBox.Size = new System.Drawing.Size(652, 150);
            //
            // labelBanner
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(24, 372);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(652, 54);
            this.labelBanner.Text = "";
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // labelState  (what the server owns right now)
            //
            this.labelState.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelState.AutoSize = false;
            this.labelState.Font = new System.Drawing.Font("monospace", 9F);
            this.labelState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelState.Location = new System.Drawing.Point(24, 436);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(652, 128);
            this.labelState.Text = "";
            this.labelState.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelTrace  (Server → Browser live push trace)
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(748, 18);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(570, 580);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(530, 30);
            this.labelTraceTitle.Text = "Server → Browser  ·  live push trace";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(530, 478);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 538);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(530, 26);
            this.labelTraceFooter.Text = "→ push = Application.Update from a task   ·   ← request = the browser asked   ·   • server = decision";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.startImportButton);
            this.panelActions.Controls.Add(this.cancelImportButton);
            this.panelActions.Controls.Add(this.blockingImportButton);
            this.panelActions.Controls.Add(this.clearLogButton);
            this.panelActions.Controls.Add(this.clearButton);
            this.panelActions.Location = new System.Drawing.Point(30, 614);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // startImportButton  (success / progress path — steps 1-3 of the pattern)
            //
            this.startImportButton.Location = new System.Drawing.Point(0, 4);
            this.startImportButton.Name = "startImportButton";
            this.startImportButton.Size = new System.Drawing.Size(150, 36);
            this.startImportButton.Text = "▶ Start import";
            this.startImportButton.ToolTipText = "200 records × 25 ms on Application.StartTask; the browser is updated every 10 records. Clicking twice does not start a second job.";
            this.startImportButton.Click += new System.EventHandler(this.startImportButton_Click);
            //
            // cancelImportButton  (cancellation path — cooperative)
            //
            this.cancelImportButton.Enabled = false;
            this.cancelImportButton.Location = new System.Drawing.Point(158, 4);
            this.cancelImportButton.Name = "cancelImportButton";
            this.cancelImportButton.Size = new System.Drawing.Size(110, 36);
            this.cancelImportButton.Text = "■ Cancel";
            this.cancelImportButton.ToolTipText = "_cts.Cancel(): the loop checks the token before every record and stops between two records.";
            this.cancelImportButton.Click += new System.EventHandler(this.cancelImportButton_Click);
            //
            // blockingImportButton  (anti-pattern — the import inside the click handler)
            //
            this.blockingImportButton.Location = new System.Drawing.Point(292, 4);
            this.blockingImportButton.Name = "blockingImportButton";
            this.blockingImportButton.Size = new System.Drawing.Size(230, 36);
            this.blockingImportButton.Text = "Blocking import (anti-pattern)";
            this.blockingImportButton.ToolTipText = "60 records × 25 ms synchronously INSIDE the click handler: the browser sees nothing for ~1.5 s, then everything at once.";
            this.blockingImportButton.Click += new System.EventHandler(this.blockingImportButton_Click);
            //
            // clearLogButton
            //
            this.clearLogButton.Location = new System.Drawing.Point(546, 4);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(110, 36);
            this.clearLogButton.Text = "Clear log";
            this.clearLogButton.ToolTipText = "Empties the import log (in-request update).";
            this.clearLogButton.Click += new System.EventHandler(this.clearLogButton_Click);
            //
            // clearButton
            //
            this.clearButton.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.clearButton.Location = new System.Drawing.Point(1178, 4);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(110, 36);
            this.clearButton.Text = "Clear trace";
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelImport);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 700);
            this.Text = "TicketOps Live — Background Import Monitor";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelImport.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelImport;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Label jobIdLabel;
        private Wisej.Web.Label elapsedLabel;
        private Wisej.Web.Label importStatusLabel;
        private Wisej.Web.ProgressBar importProgressBar;
        private Wisej.Web.Label labelRecordsCaption;
        private Wisej.Web.Label recordsImportedLabel;
        private Wisej.Web.CheckBox failAt87CheckBox;
        private Wisej.Web.Label labelLogCaption;
        private Wisej.Web.ListBox importLogListBox;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button startImportButton;
        private Wisej.Web.Button cancelImportButton;
        private Wisej.Web.Button blockingImportButton;
        private Wisej.Web.Button clearLogButton;
        private Wisej.Web.Button clearButton;
    }
}
