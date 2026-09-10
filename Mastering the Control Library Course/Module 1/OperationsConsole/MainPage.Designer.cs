namespace OperationsConsole
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
            this.commandPanel = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.btnRefresh = new Wisej.Web.Button();
            this.chkSimulateFailure = new Wisej.Web.CheckBox();
            this.lblCommandHint = new Wisej.Web.Label();
            this.statusPanel = new Wisej.Web.Panel();
            this.statusLabel = new Wisej.Web.Label();
            this.diagnosticPanel = new Wisej.Web.Panel();
            this.lblSelectedControl = new Wisej.Web.Label();
            this.lblSelectedRecord = new Wisej.Web.Label();
            this.lblActiveProfile = new Wisej.Web.Label();
            this.lblLastRefresh = new Wisej.Web.Label();
            this.navigationPanel = new Wisej.Web.Panel();
            this.lblSections = new Wisej.Web.Label();
            this.editorsButton = new Wisej.Web.Button();
            this.layoutsButton = new Wisej.Web.Button();
            this.listsTreesButton = new Wisej.Web.Button();
            this.gridButton = new Wisej.Web.Button();
            this.dashboardButton = new Wisej.Web.Button();
            this.widgetsButton = new Wisej.Web.Button();
            this.lblNavigationHint = new Wisej.Web.Label();
            this.pnlEventLog = new Wisej.Web.Panel();
            this.lblEventLogTitle = new Wisej.Web.Label();
            this.lstEventLog = new Wisej.Web.ListBox();
            this.btnClearLog = new Wisej.Web.Button();
            this.contentPanel = new Wisej.Web.Panel();
            this.commandPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.diagnosticPanel.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.pnlEventLog.SuspendLayout();
            this.SuspendLayout();
            //
            // commandPanel  (command area · Dock = Top)
            //
            this.commandPanel.BackColor = System.Drawing.Color.White;
            this.commandPanel.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.commandPanel.Controls.Add(this.lblAppTitle);
            this.commandPanel.Controls.Add(this.btnRefresh);
            this.commandPanel.Controls.Add(this.chkSimulateFailure);
            this.commandPanel.Controls.Add(this.lblCommandHint);
            this.commandPanel.Dock = Wisej.Web.DockStyle.Top;
            this.commandPanel.Name = "commandPanel";
            this.commandPanel.Size = new System.Drawing.Size(1280, 56);
            this.commandPanel.TabIndex = 0;
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.Location = new System.Drawing.Point(16, 12);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(250, 30);
            this.lblAppTitle.Text = "Operations Console";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnRefresh  (calls RefreshSection() on the visible section)
            //
            this.btnRefresh.AccessibleName = "Refresh the current section";
            this.btnRefresh.Location = new System.Drawing.Point(280, 11);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(110, 34);
            this.btnRefresh.TabIndex = 10;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.ToolTipText = "Calls RefreshSection() on the visible section and stamps the last-refresh time.";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // chkSimulateFailure  (failure path: the page factory throws for the next navigation)
            //
            this.chkSimulateFailure.AccessibleName = "Simulate a page that cannot be created";
            this.chkSimulateFailure.Location = new System.Drawing.Point(410, 16);
            this.chkSimulateFailure.Name = "chkSimulateFailure";
            this.chkSimulateFailure.Size = new System.Drawing.Size(200, 24);
            this.chkSimulateFailure.TabIndex = 11;
            this.chkSimulateFailure.Text = "Simulate page failure";
            this.chkSimulateFailure.ToolTipText = "The next navigation throws inside the page factory: the status area turns red, an AlertBox explains, the previous page stays.";
            this.chkSimulateFailure.CheckedChanged += new System.EventHandler(this.chkSimulateFailure_CheckedChanged);
            //
            // lblCommandHint
            //
            this.lblCommandHint.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblCommandHint.AutoSize = false;
            this.lblCommandHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCommandHint.Location = new System.Drawing.Point(630, 16);
            this.lblCommandHint.Name = "lblCommandHint";
            this.lblCommandHint.Size = new System.Drawing.Size(630, 24);
            this.lblCommandHint.Text = "commandPanel · Dock = Top — commands for the current section";
            this.lblCommandHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // statusPanel  (status area · Dock = Bottom)
            //
            this.statusPanel.BackColor = System.Drawing.Color.White;
            this.statusPanel.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.statusPanel.Controls.Add(this.statusLabel);
            this.statusPanel.Controls.Add(this.diagnosticPanel);
            this.statusPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(1280, 40);
            this.statusPanel.TabIndex = 1;
            //
            // statusLabel  (the status text, coloured by StatusLevel)
            //
            this.statusLabel.AutoSize = false;
            this.statusLabel.Dock = Wisej.Web.DockStyle.Fill;
            this.statusLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Padding = new Wisej.Web.Padding(16, 0, 8, 0);
            this.statusLabel.Text = "Ready";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // diagnosticPanel  (stretch goal: selected control, selected record, active profile, last refresh)
            //
            this.diagnosticPanel.Controls.Add(this.lblSelectedControl);
            this.diagnosticPanel.Controls.Add(this.lblSelectedRecord);
            this.diagnosticPanel.Controls.Add(this.lblActiveProfile);
            this.diagnosticPanel.Controls.Add(this.lblLastRefresh);
            this.diagnosticPanel.Dock = Wisej.Web.DockStyle.Right;
            this.diagnosticPanel.Name = "diagnosticPanel";
            this.diagnosticPanel.Size = new System.Drawing.Size(670, 38);
            //
            // lblSelectedControl
            //
            this.lblSelectedControl.AutoSize = false;
            this.lblSelectedControl.Font = new System.Drawing.Font("monospace", 9F);
            this.lblSelectedControl.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblSelectedControl.Location = new System.Drawing.Point(0, 0);
            this.lblSelectedControl.Name = "lblSelectedControl";
            this.lblSelectedControl.Size = new System.Drawing.Size(200, 38);
            this.lblSelectedControl.Text = "Control: —";
            this.lblSelectedControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSelectedControl.ToolTipText = "Name of the control that was used last (diagnostic).";
            //
            // lblSelectedRecord
            //
            this.lblSelectedRecord.AutoSize = false;
            this.lblSelectedRecord.Font = new System.Drawing.Font("monospace", 9F);
            this.lblSelectedRecord.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblSelectedRecord.Location = new System.Drawing.Point(200, 0);
            this.lblSelectedRecord.Name = "lblSelectedRecord";
            this.lblSelectedRecord.Size = new System.Drawing.Size(140, 38);
            this.lblSelectedRecord.Text = "Record: —";
            this.lblSelectedRecord.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSelectedRecord.ToolTipText = "ID of the selected record (none in Module 1: the placeholder pages hold no data yet).";
            //
            // lblActiveProfile
            //
            this.lblActiveProfile.AutoSize = false;
            this.lblActiveProfile.Font = new System.Drawing.Font("monospace", 9F);
            this.lblActiveProfile.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblActiveProfile.Location = new System.Drawing.Point(340, 0);
            this.lblActiveProfile.Name = "lblActiveProfile";
            this.lblActiveProfile.Size = new System.Drawing.Size(160, 38);
            this.lblActiveProfile.Text = "Profile: —";
            this.lblActiveProfile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblActiveProfile.ToolTipText = "Application.ActiveProfile.Name — Phone ≤ 600 px, Tablet ≤ 1024 px, Desktop (ClientProfiles.json). Resize the browser to see it change.";
            //
            // lblLastRefresh
            //
            this.lblLastRefresh.AutoSize = false;
            this.lblLastRefresh.Font = new System.Drawing.Font("monospace", 9F);
            this.lblLastRefresh.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblLastRefresh.Location = new System.Drawing.Point(500, 0);
            this.lblLastRefresh.Name = "lblLastRefresh";
            this.lblLastRefresh.Size = new System.Drawing.Size(170, 38);
            this.lblLastRefresh.Text = "Refreshed: —";
            this.lblLastRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblLastRefresh.ToolTipText = "Time of the last Refresh command.";
            //
            // navigationPanel  (navigation area · Dock = Left)
            //
            this.navigationPanel.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.navigationPanel.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.navigationPanel.Controls.Add(this.lblSections);
            this.navigationPanel.Controls.Add(this.editorsButton);
            this.navigationPanel.Controls.Add(this.layoutsButton);
            this.navigationPanel.Controls.Add(this.listsTreesButton);
            this.navigationPanel.Controls.Add(this.gridButton);
            this.navigationPanel.Controls.Add(this.dashboardButton);
            this.navigationPanel.Controls.Add(this.widgetsButton);
            this.navigationPanel.Controls.Add(this.lblNavigationHint);
            this.navigationPanel.Dock = Wisej.Web.DockStyle.Left;
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Size = new System.Drawing.Size(220, 704);
            this.navigationPanel.TabIndex = 2;
            //
            // lblSections
            //
            this.lblSections.AutoSize = false;
            this.lblSections.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSections.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSections.Location = new System.Drawing.Point(14, 12);
            this.lblSections.Name = "lblSections";
            this.lblSections.Size = new System.Drawing.Size(192, 22);
            this.lblSections.Text = "SECTIONS";
            this.lblSections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // editorsButton  (TabIndex 1 — first stop of the keyboard workflow)
            //
            this.editorsButton.AccessibleName = "Open the Editors section";
            this.editorsButton.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.editorsButton.Location = new System.Drawing.Point(14, 40);
            this.editorsButton.Name = "editorsButton";
            this.editorsButton.Size = new System.Drawing.Size(192, 40);
            this.editorsButton.TabIndex = 1;
            this.editorsButton.Text = "Editors";
            this.editorsButton.ToolTipText = "Module 2 · editors, buttons, validation and feedback";
            this.editorsButton.Click += new System.EventHandler(this.editorsButton_Click);
            //
            // layoutsButton
            //
            this.layoutsButton.AccessibleName = "Open the Layouts section";
            this.layoutsButton.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.layoutsButton.Location = new System.Drawing.Point(14, 88);
            this.layoutsButton.Name = "layoutsButton";
            this.layoutsButton.Size = new System.Drawing.Size(192, 40);
            this.layoutsButton.TabIndex = 2;
            this.layoutsButton.Text = "Layouts";
            this.layoutsButton.ToolTipText = "Module 3 · containers, layouts, navigation and reuse";
            this.layoutsButton.Click += new System.EventHandler(this.layoutsButton_Click);
            //
            // listsTreesButton
            //
            this.listsTreesButton.AccessibleName = "Open the Lists and Trees section";
            this.listsTreesButton.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listsTreesButton.Location = new System.Drawing.Point(14, 136);
            this.listsTreesButton.Name = "listsTreesButton";
            this.listsTreesButton.Size = new System.Drawing.Size(192, 40);
            this.listsTreesButton.TabIndex = 3;
            this.listsTreesButton.Text = "Lists and Trees";
            this.listsTreesButton.ToolTipText = "Module 4 · lists, trees, repeaters and hierarchical data";
            this.listsTreesButton.Click += new System.EventHandler(this.listsTreesButton_Click);
            //
            // gridButton
            //
            this.gridButton.AccessibleName = "Open the DataGridView section";
            this.gridButton.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridButton.Location = new System.Drawing.Point(14, 184);
            this.gridButton.Name = "gridButton";
            this.gridButton.Size = new System.Drawing.Size(192, 40);
            this.gridButton.TabIndex = 4;
            this.gridButton.Text = "DataGridView";
            this.gridButton.ToolTipText = "Module 5 · DataGridView mastery";
            this.gridButton.Click += new System.EventHandler(this.gridButton_Click);
            //
            // dashboardButton
            //
            this.dashboardButton.AccessibleName = "Open the Dashboard section";
            this.dashboardButton.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dashboardButton.Location = new System.Drawing.Point(14, 232);
            this.dashboardButton.Name = "dashboardButton";
            this.dashboardButton.Size = new System.Drawing.Size(192, 40);
            this.dashboardButton.TabIndex = 5;
            this.dashboardButton.Text = "Dashboard";
            this.dashboardButton.ToolTipText = "Module 6 · charts, dashboards, content, media and documents";
            this.dashboardButton.Click += new System.EventHandler(this.dashboardButton_Click);
            //
            // widgetsButton
            //
            this.widgetsButton.AccessibleName = "Open the Widgets section";
            this.widgetsButton.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.widgetsButton.Location = new System.Drawing.Point(14, 280);
            this.widgetsButton.Name = "widgetsButton";
            this.widgetsButton.Size = new System.Drawing.Size(192, 40);
            this.widgetsButton.TabIndex = 6;
            this.widgetsButton.Text = "Widgets";
            this.widgetsButton.ToolTipText = "Module 7 · custom widgets, extensions, theming and capstone";
            this.widgetsButton.Click += new System.EventHandler(this.widgetsButton_Click);
            //
            // lblNavigationHint
            //
            this.lblNavigationHint.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblNavigationHint.AutoSize = false;
            this.lblNavigationHint.Font = new System.Drawing.Font("monospace", 8F);
            this.lblNavigationHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNavigationHint.Location = new System.Drawing.Point(14, 664);
            this.lblNavigationHint.Name = "lblNavigationHint";
            this.lblNavigationHint.Size = new System.Drawing.Size(192, 28);
            this.lblNavigationHint.Text = "navigationPanel · Dock = Left";
            this.lblNavigationHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlEventLog  (Event log card · Dock = Right — part of the shell contract, docked before contentPanel)
            //
            this.pnlEventLog.BackColor = System.Drawing.Color.White;
            this.pnlEventLog.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlEventLog.Controls.Add(this.lblEventLogTitle);
            this.pnlEventLog.Controls.Add(this.lstEventLog);
            this.pnlEventLog.Controls.Add(this.btnClearLog);
            this.pnlEventLog.Dock = Wisej.Web.DockStyle.Right;
            this.pnlEventLog.Name = "pnlEventLog";
            this.pnlEventLog.Size = new System.Drawing.Size(320, 704);
            this.pnlEventLog.TabIndex = 3;
            //
            // lblEventLogTitle
            //
            this.lblEventLogTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblEventLogTitle.AutoSize = false;
            this.lblEventLogTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblEventLogTitle.Location = new System.Drawing.Point(16, 12);
            this.lblEventLogTitle.Name = "lblEventLogTitle";
            this.lblEventLogTitle.Size = new System.Drawing.Size(288, 30);
            this.lblEventLogTitle.Text = "Event log";
            this.lblEventLogTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lstEventLog  (every user action, service call and decision: "HH:mm:ss  message")
            //
            this.lstEventLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstEventLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstEventLog.Location = new System.Drawing.Point(16, 50);
            this.lstEventLog.Name = "lstEventLog";
            this.lstEventLog.Size = new System.Drawing.Size(288, 594);
            this.lstEventLog.TabIndex = 20;
            //
            // btnClearLog
            //
            this.btnClearLog.AccessibleName = "Clear the event log";
            this.btnClearLog.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.btnClearLog.Location = new System.Drawing.Point(16, 656);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(100, 32);
            this.btnClearLog.TabIndex = 21;
            this.btnClearLog.Text = "Clear";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // contentPanel  (content area · Dock = Fill — dropped LAST in the designer, so it is FIRST in the serialised Controls.Add list below)
            //
            this.contentPanel.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.contentPanel.Dock = Wisej.Web.DockStyle.Fill;
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(740, 704);
            this.contentPanel.TabIndex = 4;
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            // Docking is applied from the LAST added control to the FIRST (the designer serialises the reverse z-order):
            // contentPanel (Fill) first in this list, then the Right / Left cards, then the Bottom / Top bars, which claim the full width.
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.pnlEventLog);
            this.Controls.Add(this.navigationPanel);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.commandPanel);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1280, 800);
            this.Text = "Operations Console";
            this.commandPanel.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.diagnosticPanel.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
            this.pnlEventLog.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // command area
        private Wisej.Web.Panel commandPanel;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.CheckBox chkSimulateFailure;
        private Wisej.Web.Label lblCommandHint;

        // status area + diagnostic panel (stretch goal)
        private Wisej.Web.Panel statusPanel;
        private Wisej.Web.Label statusLabel;
        private Wisej.Web.Panel diagnosticPanel;
        private Wisej.Web.Label lblSelectedControl;
        private Wisej.Web.Label lblSelectedRecord;
        private Wisej.Web.Label lblActiveProfile;
        private Wisej.Web.Label lblLastRefresh;

        // navigation area
        private Wisej.Web.Panel navigationPanel;
        private Wisej.Web.Label lblSections;
        private Wisej.Web.Button editorsButton;
        private Wisej.Web.Button layoutsButton;
        private Wisej.Web.Button listsTreesButton;
        private Wisej.Web.Button gridButton;
        private Wisej.Web.Button dashboardButton;
        private Wisej.Web.Button widgetsButton;
        private Wisej.Web.Label lblNavigationHint;

        // Event log card (shell contract)
        private Wisej.Web.Panel pnlEventLog;
        private Wisej.Web.Label lblEventLogTitle;
        private Wisej.Web.ListBox lstEventLog;
        private Wisej.Web.Button btnClearLog;

        // content area
        private Wisej.Web.Panel contentPanel;
    }
}
