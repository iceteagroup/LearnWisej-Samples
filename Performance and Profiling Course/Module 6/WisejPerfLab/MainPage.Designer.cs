namespace WisejPerfLab
{
    partial class MainPage
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Designer generated code

        private void InitializeComponent()
        {
            this.panelHeader = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.lblSubtitle = new Wisej.Web.Label();
            this.lblDataset = new Wisej.Web.Label();
            this.tabScreens = new Wisej.Web.TabControl();
            this.tabDashboard = new Wisej.Web.TabPage();
            this.tabTickets = new Wisej.Web.TabPage();
            this.tabCustomers = new Wisej.Web.TabPage();
            this.panelPerf = new Wisej.Web.Panel();
            this.lblPerfTitle = new Wisej.Web.Label();
            this.listPerf = new Wisej.Web.ListBox();
            this.lblPerfLegend = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.btnWarmUp = new Wisej.Web.Button();
            this.btnRunThree = new Wisej.Web.Button();
            this.btnBreakDatabase = new Wisej.Web.Button();
            this.btnRestoreDatabase = new Wisej.Web.Button();
            this.btnTraceNote = new Wisej.Web.Button();
            this.btnClearLog = new Wisej.Web.Button();
            this.panelActions2 = new Wisej.Web.Panel();
            this.btnSnapshotA = new Wisej.Web.Button();
            this.btnOpenCloseFifty = new Wisej.Web.Button();
            this.btnSnapshotB = new Wisej.Web.Button();
            this.lblMemory = new Wisej.Web.Label();
            this.lblState = new Wisej.Web.Label();
            this.lblBanner = new Wisej.Web.Label();
            this.panelHeader.SuspendLayout();
            this.tabScreens.SuspendLayout();
            this.panelPerf.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.panelActions2.SuspendLayout();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelHeader.Controls.Add(this.lblDataset);
            this.panelHeader.Controls.Add(this.lblSubtitle);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Location = new System.Drawing.Point(20, 16);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1308, 64);
            this.panelHeader.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(18, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(420, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "WisejPerfLab — Support operations";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSubtitle.Location = new System.Drawing.Point(20, 38);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(640, 16);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Module 6 — waiting: one statement per page, an index that earns itself, and an export that never blocks.";
            //
            // lblDataset
            //
            this.lblDataset.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
            this.lblDataset.Font = new System.Drawing.Font("monospace", 9F);
            this.lblDataset.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblDataset.Location = new System.Drawing.Point(786, 12);
            this.lblDataset.Name = "lblDataset";
            this.lblDataset.Size = new System.Drawing.Size(500, 40);
            this.lblDataset.TabIndex = 2;
            this.lblDataset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // tabScreens
            //
            this.tabScreens.Controls.Add(this.tabDashboard);
            this.tabScreens.Controls.Add(this.tabTickets);
            this.tabScreens.Controls.Add(this.tabCustomers);
            this.tabScreens.Location = new System.Drawing.Point(20, 92);
            this.tabScreens.Name = "tabScreens";
            this.tabScreens.SelectedIndex = 0;
            this.tabScreens.Size = new System.Drawing.Size(880, 470);
            this.tabScreens.TabIndex = 1;
            //
            // tabDashboard
            //
            this.tabDashboard.BackColor = System.Drawing.Color.White;
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Size = new System.Drawing.Size(876, 440);
            this.tabDashboard.TabIndex = 0;
            this.tabDashboard.Text = "Dashboard";
            //
            // tabTickets
            //
            this.tabTickets.BackColor = System.Drawing.Color.White;
            this.tabTickets.Name = "tabTickets";
            this.tabTickets.Size = new System.Drawing.Size(876, 440);
            this.tabTickets.TabIndex = 1;
            this.tabTickets.Text = "Tickets";
            //
            // tabCustomers
            //
            this.tabCustomers.BackColor = System.Drawing.Color.White;
            this.tabCustomers.Name = "tabCustomers";
            this.tabCustomers.Size = new System.Drawing.Size(876, 440);
            this.tabCustomers.TabIndex = 2;
            this.tabCustomers.Text = "Customers";
            //
            // panelPerf
            //
            this.panelPerf.BackColor = System.Drawing.Color.White;
            this.panelPerf.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelPerf.Controls.Add(this.lblPerfLegend);
            this.panelPerf.Controls.Add(this.listPerf);
            this.panelPerf.Controls.Add(this.lblPerfTitle);
            this.panelPerf.Location = new System.Drawing.Point(912, 92);
            this.panelPerf.Name = "panelPerf";
            this.panelPerf.Size = new System.Drawing.Size(416, 470);
            this.panelPerf.TabIndex = 2;
            //
            // lblPerfTitle
            //
            this.lblPerfTitle.AutoSize = true;
            this.lblPerfTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblPerfTitle.Location = new System.Drawing.Point(14, 12);
            this.lblPerfTitle.Name = "lblPerfTitle";
            this.lblPerfTitle.Size = new System.Drawing.Size(380, 20);
            this.lblPerfTitle.TabIndex = 0;
            this.lblPerfTitle.Text = "PERF log · ScenarioProbe records";
            //
            // listPerf
            //
            this.listPerf.Font = new System.Drawing.Font("monospace", 9F);
            this.listPerf.Location = new System.Drawing.Point(14, 42);
            this.listPerf.Name = "listPerf";
            this.listPerf.Size = new System.Drawing.Size(386, 374);
            this.listPerf.TabIndex = 1;
            //
            // lblPerfLegend
            //
            this.lblPerfLegend.Font = new System.Drawing.Font("monospace", 8F);
            this.lblPerfLegend.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPerfLegend.Location = new System.Drawing.Point(14, 422);
            this.lblPerfLegend.Name = "lblPerfLegend";
            this.lblPerfLegend.Size = new System.Drawing.Size(386, 36);
            this.lblPerfLegend.TabIndex = 2;
            this.lblPerfLegend.Text = "PERF start = scope entered   PERF end = scope disposed\r\nelapsedMs = whole user action   rows = what it moved";
            //
            // panelActions
            //
            this.panelActions.Controls.Add(this.btnClearLog);
            this.panelActions.Controls.Add(this.btnTraceNote);
            this.panelActions.Controls.Add(this.btnRestoreDatabase);
            this.panelActions.Controls.Add(this.btnBreakDatabase);
            this.panelActions.Controls.Add(this.btnRunThree);
            this.panelActions.Controls.Add(this.btnWarmUp);
            this.panelActions.Location = new System.Drawing.Point(20, 574);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1308, 44);
            this.panelActions.TabIndex = 3;
            //
            // btnWarmUp
            //
            this.btnWarmUp.Location = new System.Drawing.Point(0, 4);
            this.btnWarmUp.Name = "btnWarmUp";
            this.btnWarmUp.Size = new System.Drawing.Size(190, 36);
            this.btnWarmUp.TabIndex = 0;
            this.btnWarmUp.Text = "Warm up (discarded run)";
            this.btnWarmUp.Click += this.btnWarmUp_Click;
            //
            // btnRunThree
            //
            this.btnRunThree.Location = new System.Drawing.Point(198, 4);
            this.btnRunThree.Name = "btnRunThree";
            this.btnRunThree.Size = new System.Drawing.Size(230, 36);
            this.btnRunThree.TabIndex = 1;
            this.btnRunThree.Text = "Run the three scenarios ×3";
            this.btnRunThree.Click += this.btnRunThree_Click;
            //
            // btnBreakDatabase
            //
            this.btnBreakDatabase.Location = new System.Drawing.Point(436, 4);
            this.btnBreakDatabase.Name = "btnBreakDatabase";
            this.btnBreakDatabase.Size = new System.Drawing.Size(170, 36);
            this.btnBreakDatabase.TabIndex = 2;
            this.btnBreakDatabase.Text = "Break the database";
            this.btnBreakDatabase.Click += this.btnBreakDatabase_Click;
            //
            // btnRestoreDatabase
            //
            this.btnRestoreDatabase.Location = new System.Drawing.Point(614, 4);
            this.btnRestoreDatabase.Name = "btnRestoreDatabase";
            this.btnRestoreDatabase.Size = new System.Drawing.Size(150, 36);
            this.btnRestoreDatabase.TabIndex = 3;
            this.btnRestoreDatabase.Text = "Restore";
            this.btnRestoreDatabase.Click += this.btnRestoreDatabase_Click;
            //
            // btnTraceNote
            //
            this.btnTraceNote.Location = new System.Drawing.Point(772, 4);
            this.btnTraceNote.Name = "btnTraceNote";
            this.btnTraceNote.Size = new System.Drawing.Size(230, 36);
            this.btnTraceNote.TabIndex = 4;
            this.btnTraceNote.Text = "Trace note for the last run";
            this.btnTraceNote.Click += this.btnTraceNote_Click;
            //
            // btnClearLog
            //
            this.btnClearLog.Anchor = ((Wisej.Web.AnchorStyles)((Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right)));
            this.btnClearLog.Location = new System.Drawing.Point(1198, 4);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(110, 36);
            this.btnClearLog.TabIndex = 4;
            this.btnClearLog.Text = "Clear log";
            this.btnClearLog.Click += this.btnClearLog_Click;
            //
            // panelActions2
            //
            this.panelActions2.Controls.Add(this.lblMemory);
            this.panelActions2.Controls.Add(this.btnSnapshotB);
            this.panelActions2.Controls.Add(this.btnOpenCloseFifty);
            this.panelActions2.Controls.Add(this.btnSnapshotA);
            this.panelActions2.Location = new System.Drawing.Point(20, 622);
            this.panelActions2.Name = "panelActions2";
            this.panelActions2.Size = new System.Drawing.Size(1308, 44);
            this.panelActions2.TabIndex = 4;
            //
            // btnSnapshotA
            //
            this.btnSnapshotA.Location = new System.Drawing.Point(0, 4);
            this.btnSnapshotA.Name = "btnSnapshotA";
            this.btnSnapshotA.Size = new System.Drawing.Size(190, 36);
            this.btnSnapshotA.TabIndex = 0;
            this.btnSnapshotA.Text = "Memory snapshot A";
            this.btnSnapshotA.Click += this.btnSnapshotA_Click;
            //
            // btnOpenCloseFifty
            //
            this.btnOpenCloseFifty.Location = new System.Drawing.Point(198, 4);
            this.btnOpenCloseFifty.Name = "btnOpenCloseFifty";
            this.btnOpenCloseFifty.Size = new System.Drawing.Size(280, 36);
            this.btnOpenCloseFifty.TabIndex = 1;
            this.btnOpenCloseFifty.Text = "Open and close the detail form x50";
            this.btnOpenCloseFifty.Click += this.btnOpenCloseFifty_Click;
            //
            // btnSnapshotB
            //
            this.btnSnapshotB.Location = new System.Drawing.Point(486, 4);
            this.btnSnapshotB.Name = "btnSnapshotB";
            this.btnSnapshotB.Size = new System.Drawing.Size(230, 36);
            this.btnSnapshotB.TabIndex = 2;
            this.btnSnapshotB.Text = "Snapshot B and compare";
            this.btnSnapshotB.Click += this.btnSnapshotB_Click;
            //
            // lblMemory
            //
            this.lblMemory.Font = new System.Drawing.Font("monospace", 9F);
            this.lblMemory.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblMemory.Location = new System.Drawing.Point(726, 4);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(580, 36);
            this.lblMemory.TabIndex = 3;
            this.lblMemory.Text = "no snapshot yet";
            //
            // lblState
            //
            this.lblState.Font = new System.Drawing.Font("monospace", 9F);
            this.lblState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblState.Location = new System.Drawing.Point(22, 674);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(1250, 16);
            this.lblState.TabIndex = 4;
            this.lblState.Text = "● idle";
            //
            // lblBanner
            //
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 698);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(10, 6, 10, 6);
            this.lblBanner.Size = new System.Drawing.Size(1308, 32);
            this.lblBanner.TabIndex = 5;
            this.lblBanner.Visible = false;
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.lblBanner);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.panelActions2);
            this.Controls.Add(this.panelActions);
            this.Controls.Add(this.panelPerf);
            this.Controls.Add(this.tabScreens);
            this.Controls.Add(this.panelHeader);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 748);
            this.Load += this.MainPage_Load;
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.tabScreens.ResumeLayout(false);
            this.panelPerf.ResumeLayout(false);
            this.panelPerf.PerformLayout();
            this.panelActions.ResumeLayout(false);
            this.panelActions2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Panel panelHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblSubtitle;
        private Wisej.Web.Label lblDataset;
        private Wisej.Web.TabControl tabScreens;
        private Wisej.Web.TabPage tabDashboard;
        private Wisej.Web.TabPage tabTickets;
        private Wisej.Web.TabPage tabCustomers;
        private Wisej.Web.Panel panelPerf;
        private Wisej.Web.Label lblPerfTitle;
        private Wisej.Web.ListBox listPerf;
        private Wisej.Web.Label lblPerfLegend;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button btnWarmUp;
        private Wisej.Web.Button btnRunThree;
        private Wisej.Web.Button btnBreakDatabase;
        private Wisej.Web.Button btnRestoreDatabase;
        private Wisej.Web.Button btnTraceNote;
        private Wisej.Web.Button btnClearLog;
        private Wisej.Web.Panel panelActions2;
        private Wisej.Web.Button btnSnapshotA;
        private Wisej.Web.Button btnOpenCloseFifty;
        private Wisej.Web.Button btnSnapshotB;
        private Wisej.Web.Label lblMemory;
        private Wisej.Web.Label lblState;
        private Wisej.Web.Label lblBanner;
    }
}
