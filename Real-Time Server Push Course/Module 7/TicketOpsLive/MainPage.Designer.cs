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
            this.statusPanel = new Wisej.Web.Panel();
            this.labelStripCaption = new Wisej.Web.Label();
            this.clockLabel = new Wisej.Web.Label();
            this.connectionLabel = new Wisej.Web.Label();
            this.activityLabel = new Wisej.Web.Label();
            this.labelLoadCaption = new Wisej.Web.Label();
            this.serverLoadBar = new Wisej.Web.ProgressBar();
            this.loadValueLabel = new Wisej.Web.Label();
            this.featureTabs = new Wisej.Web.TabControl();
            this.tabImport = new Wisej.Web.TabPage();
            this.tabBoard = new Wisej.Web.TabPage();
            this.tabCadence = new Wisej.Web.TabPage();
            this.tabSession = new Wisej.Web.TabPage();
            this.tabHealth = new Wisej.Web.TabPage();
            // M03 — import monitor
            this.jobIdLabel = new Wisej.Web.Label();
            this.elapsedLabel = new Wisej.Web.Label();
            this.importStatusLabel = new Wisej.Web.Label();
            this.importProgressBar = new Wisej.Web.ProgressBar();
            this.labelRecordsCaption = new Wisej.Web.Label();
            this.recordsImportedLabel = new Wisej.Web.Label();
            this.failAt87CheckBox = new Wisej.Web.CheckBox();
            this.importLogListBox = new Wisej.Web.ListBox();
            this.startImportButton = new Wisej.Web.Button();
            this.cancelImportButton = new Wisej.Web.Button();
            // M05 + M06 — ticket board
            this.labelTenantCaption = new Wisej.Web.Label();
            this.tenantComboBox = new Wisej.Web.ComboBox();
            this.subscribersLabel = new Wisej.Web.Label();
            this.ticketsGrid = new Wisej.Web.DataGridView();
            this.selectedLabel = new Wisej.Web.Label();
            this.escalatedOnlyCheckBox = new Wisej.Web.CheckBox();
            this.notificationsList = new Wisej.Web.ListBox();
            this.notificationCountLabel = new Wisej.Web.Label();
            this.filteredOutLabel = new Wisej.Web.Label();
            this.publishButton = new Wisej.Web.Button();
            this.publishOtherButton = new Wisej.Web.Button();
            this.escalateButton = new Wisej.Web.Button();
            this.newTicketsButton = new Wisej.Web.Button();
            this.subscribeButton = new Wisej.Web.Button();
            this.unsubscribeButton = new Wisej.Web.Button();
            // M04 — cadence
            this.liveModeCheckBox = new Wisej.Web.CheckBox();
            this.labelCadenceCaption = new Wisej.Web.Label();
            this.cadenceComboBox = new Wisej.Web.ComboBox();
            this.labelOpenCaption = new Wisej.Web.Label();
            this.openTicketsLabel = new Wisej.Web.Label();
            this.labelQueueCaption = new Wisej.Web.Label();
            this.queueDepthLabel = new Wisej.Web.Label();
            this.labelWaitCaption = new Wisej.Web.Label();
            this.avgWaitLabel = new Wisej.Web.Label();
            this.labelEventsCaption = new Wisej.Web.Label();
            this.eventsReceivedLabel = new Wisej.Web.Label();
            this.labelUpdatesCaption = new Wisej.Web.Label();
            this.updatesAppliedLabel = new Wisej.Web.Label();
            this.labelSkippedCaption = new Wisej.Web.Label();
            this.skippedTicksLabel = new Wisej.Web.Label();
            this.labelLastAppliedCaption = new Wisej.Web.Label();
            this.lastAppliedLabel = new Wisej.Web.Label();
            this.labelCadenceHint = new Wisej.Web.Label();
            // M02 — session
            this.diagnosticsGroupBox = new Wisej.Web.GroupBox();
            this.labelTimeCaption = new Wisej.Web.Label();
            this.timeLabel = new Wisej.Web.Label();
            this.labelClientCaption = new Wisej.Web.Label();
            this.clientIdLabel = new Wisej.Web.Label();
            this.labelSessionCaption = new Wisej.Web.Label();
            this.sessionIdLabel = new Wisej.Web.Label();
            this.labelBrowserCaption = new Wisej.Web.Label();
            this.browserLabel = new Wisej.Web.Label();
            this.labelThreadCaption = new Wisej.Web.Label();
            this.threadLabel = new Wisej.Web.Label();
            this.labelCounterCaption = new Wisej.Web.Label();
            this.counterLabel = new Wisej.Web.Label();
            this.liveSessionsLabel = new Wisej.Web.Label();
            this.lifecycleListBox = new Wisej.Web.ListBox();
            this.incrementButton = new Wisej.Web.Button();
            this.backgroundButton = new Wisej.Web.Button();
            this.faultButton = new Wisej.Web.Button();
            // M07 — health and configuration
            this.labelHealthTitle = new Wisej.Web.Label();
            this.websocketModeLabel = new Wisej.Web.Label();
            this.pollingLabel = new Wisej.Web.Label();
            this.subscriptionCountLabel = new Wisej.Web.Label();
            this.updateRateLabel = new Wisej.Web.Label();
            this.labelConfigTitle = new Wisej.Web.Label();
            this.configListBox = new Wisej.Web.ListBox();
            this.labelChecklistTitle = new Wisej.Web.Label();
            this.checklistBox = new Wisej.Web.CheckedListBox();
            // shared
            this.labelBanner = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.startButton = new Wisej.Web.Button();
            this.stopButton = new Wisej.Web.Button();
            this.openSessionButton = new Wisej.Web.Button();
            this.exitButton = new Wisej.Web.Button();
            this.clearButton = new Wisej.Web.Button();
            this.refreshTimer = new Wisej.Web.Timer(this.components);
            this.healthTimer = new Wisej.Web.Timer(this.components);
            this.statusPanel.SuspendLayout();
            this.featureTabs.SuspendLayout();
            this.tabImport.SuspendLayout();
            this.tabBoard.SuspendLayout();
            this.tabCadence.SuspendLayout();
            this.tabSession.SuspendLayout();
            this.tabHealth.SuspendLayout();
            this.diagnosticsGroupBox.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // statusPanel  (the live status strip from Module 1)
            //
            this.statusPanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusPanel.BackColor = System.Drawing.Color.White;
            this.statusPanel.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.statusPanel.Controls.Add(this.labelStripCaption);
            this.statusPanel.Controls.Add(this.clockLabel);
            this.statusPanel.Controls.Add(this.connectionLabel);
            this.statusPanel.Controls.Add(this.activityLabel);
            this.statusPanel.Controls.Add(this.labelLoadCaption);
            this.statusPanel.Controls.Add(this.serverLoadBar);
            this.statusPanel.Controls.Add(this.loadValueLabel);
            this.statusPanel.Location = new System.Drawing.Point(30, 18);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(1288, 92);
            //
            this.labelStripCaption.AutoSize = false;
            this.labelStripCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelStripCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelStripCaption.Location = new System.Drawing.Point(16, 8);
            this.labelStripCaption.Name = "labelStripCaption";
            this.labelStripCaption.Size = new System.Drawing.Size(300, 18);
            this.labelStripCaption.Text = "TICKETOPS LIVE · CAPSTONE · STATUS STRIP";
            //
            this.clockLabel.AutoSize = false;
            this.clockLabel.Font = new System.Drawing.Font("monospace", 22F, System.Drawing.FontStyle.Bold);
            this.clockLabel.Location = new System.Drawing.Point(16, 30);
            this.clockLabel.Name = "clockLabel";
            this.clockLabel.Size = new System.Drawing.Size(200, 44);
            this.clockLabel.Text = "--:--:--";
            this.clockLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            this.connectionLabel.AutoSize = false;
            this.connectionLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.connectionLabel.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.connectionLabel.Location = new System.Drawing.Point(236, 30);
            this.connectionLabel.Name = "connectionLabel";
            this.connectionLabel.Size = new System.Drawing.Size(470, 24);
            this.connectionLabel.Text = "○ connecting…";
            //
            this.activityLabel.AutoSize = false;
            this.activityLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.activityLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.activityLabel.Location = new System.Drawing.Point(236, 56);
            this.activityLabel.Name = "activityLabel";
            this.activityLabel.Size = new System.Drawing.Size(470, 22);
            this.activityLabel.Text = "Heartbeat not running";
            //
            this.labelLoadCaption.AutoSize = false;
            this.labelLoadCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelLoadCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLoadCaption.Location = new System.Drawing.Point(740, 8);
            this.labelLoadCaption.Name = "labelLoadCaption";
            this.labelLoadCaption.Size = new System.Drawing.Size(220, 18);
            this.labelLoadCaption.Text = "SERVER LOAD (simulated)";
            //
            this.serverLoadBar.Location = new System.Drawing.Point(740, 36);
            this.serverLoadBar.Name = "serverLoadBar";
            this.serverLoadBar.Size = new System.Drawing.Size(380, 24);
            this.serverLoadBar.Value = 0;
            //
            this.loadValueLabel.AutoSize = false;
            this.loadValueLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.loadValueLabel.Location = new System.Drawing.Point(1136, 28);
            this.loadValueLabel.Name = "loadValueLabel";
            this.loadValueLabel.Size = new System.Drawing.Size(130, 36);
            this.loadValueLabel.Text = "0 %";
            this.loadValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // featureTabs  (one tab per module of the course)
            //
            this.featureTabs.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.featureTabs.Location = new System.Drawing.Point(30, 120);
            this.featureTabs.Name = "featureTabs";
            this.featureTabs.SelectedIndex = 0;
            this.featureTabs.Size = new System.Drawing.Size(700, 568);
            this.featureTabs.TabPages.Add(this.tabImport);
            this.featureTabs.TabPages.Add(this.tabBoard);
            this.featureTabs.TabPages.Add(this.tabCadence);
            this.featureTabs.TabPages.Add(this.tabSession);
            this.featureTabs.TabPages.Add(this.tabHealth);
            this.featureTabs.SelectedIndexChanged += new System.EventHandler(this.featureTabs_SelectedIndexChanged);
            //
            // tabImport  (M03 · Background import monitor)
            //
            this.tabImport.BackColor = System.Drawing.Color.White;
            this.tabImport.Controls.Add(this.jobIdLabel);
            this.tabImport.Controls.Add(this.elapsedLabel);
            this.tabImport.Controls.Add(this.importStatusLabel);
            this.tabImport.Controls.Add(this.importProgressBar);
            this.tabImport.Controls.Add(this.labelRecordsCaption);
            this.tabImport.Controls.Add(this.recordsImportedLabel);
            this.tabImport.Controls.Add(this.failAt87CheckBox);
            this.tabImport.Controls.Add(this.importLogListBox);
            this.tabImport.Controls.Add(this.startImportButton);
            this.tabImport.Controls.Add(this.cancelImportButton);
            this.tabImport.Name = "tabImport";
            this.tabImport.Text = "Import monitor";
            //
            this.jobIdLabel.AutoSize = false;
            this.jobIdLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.jobIdLabel.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.jobIdLabel.Location = new System.Drawing.Point(18, 14);
            this.jobIdLabel.Name = "jobIdLabel";
            this.jobIdLabel.Size = new System.Drawing.Size(420, 22);
            this.jobIdLabel.Text = "Job —";
            //
            this.elapsedLabel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.elapsedLabel.AutoSize = false;
            this.elapsedLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.elapsedLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.elapsedLabel.Location = new System.Drawing.Point(444, 14);
            this.elapsedLabel.Name = "elapsedLabel";
            this.elapsedLabel.Size = new System.Drawing.Size(220, 22);
            this.elapsedLabel.Text = "elapsed 0.00 s";
            this.elapsedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            this.importStatusLabel.AutoSize = false;
            this.importStatusLabel.Font = new System.Drawing.Font("default", 11F);
            this.importStatusLabel.Location = new System.Drawing.Point(18, 40);
            this.importStatusLabel.Name = "importStatusLabel";
            this.importStatusLabel.Size = new System.Drawing.Size(646, 26);
            this.importStatusLabel.Text = "Idle — click ▶ Start import";
            //
            this.importProgressBar.Location = new System.Drawing.Point(18, 72);
            this.importProgressBar.Name = "importProgressBar";
            this.importProgressBar.Size = new System.Drawing.Size(490, 26);
            this.importProgressBar.Value = 0;
            //
            this.labelRecordsCaption.AutoSize = false;
            this.labelRecordsCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelRecordsCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelRecordsCaption.Location = new System.Drawing.Point(524, 68);
            this.labelRecordsCaption.Name = "labelRecordsCaption";
            this.labelRecordsCaption.Size = new System.Drawing.Size(140, 16);
            this.labelRecordsCaption.Text = "RECORDS IMPORTED";
            this.labelRecordsCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            this.recordsImportedLabel.AutoSize = false;
            this.recordsImportedLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.recordsImportedLabel.Location = new System.Drawing.Point(524, 82);
            this.recordsImportedLabel.Name = "recordsImportedLabel";
            this.recordsImportedLabel.Size = new System.Drawing.Size(140, 28);
            this.recordsImportedLabel.Text = "0";
            this.recordsImportedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            this.failAt87CheckBox.AutoSize = false;
            this.failAt87CheckBox.Location = new System.Drawing.Point(18, 108);
            this.failAt87CheckBox.Name = "failAt87CheckBox";
            this.failAt87CheckBox.Size = new System.Drawing.Size(490, 26);
            this.failAt87CheckBox.Text = "Throw at record 87 — simulated malformed record";
            this.failAt87CheckBox.ToolTipText = "Arms the failure path: the task throws, the detail goes to the server log, the UI gets a safe message.";
            //
            this.importLogListBox.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.importLogListBox.Font = new System.Drawing.Font("monospace", 9F);
            this.importLogListBox.Location = new System.Drawing.Point(18, 142);
            this.importLogListBox.Name = "importLogListBox";
            this.importLogListBox.Size = new System.Drawing.Size(646, 330);
            //
            this.startImportButton.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.startImportButton.Location = new System.Drawing.Point(18, 482);
            this.startImportButton.Name = "startImportButton";
            this.startImportButton.Size = new System.Drawing.Size(150, 36);
            this.startImportButton.Text = "▶ Start import";
            this.startImportButton.ToolTipText = "200 records on a task; progress pushed every 10 records.";
            this.startImportButton.Click += new System.EventHandler(this.startImportButton_Click);
            //
            this.cancelImportButton.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.cancelImportButton.Enabled = false;
            this.cancelImportButton.Location = new System.Drawing.Point(176, 482);
            this.cancelImportButton.Name = "cancelImportButton";
            this.cancelImportButton.Size = new System.Drawing.Size(120, 36);
            this.cancelImportButton.Text = "■ Cancel";
            this.cancelImportButton.ToolTipText = "Cooperative: the loop sees the token between two records.";
            this.cancelImportButton.Click += new System.EventHandler(this.cancelImportButton_Click);
            //
            // tabBoard  (M05 + M06 · Live ticket board fed by the global TicketHub)
            //
            this.tabBoard.BackColor = System.Drawing.Color.White;
            this.tabBoard.Controls.Add(this.labelTenantCaption);
            this.tabBoard.Controls.Add(this.tenantComboBox);
            this.tabBoard.Controls.Add(this.subscribersLabel);
            this.tabBoard.Controls.Add(this.ticketsGrid);
            this.tabBoard.Controls.Add(this.selectedLabel);
            this.tabBoard.Controls.Add(this.escalatedOnlyCheckBox);
            this.tabBoard.Controls.Add(this.notificationsList);
            this.tabBoard.Controls.Add(this.notificationCountLabel);
            this.tabBoard.Controls.Add(this.filteredOutLabel);
            this.tabBoard.Controls.Add(this.publishButton);
            this.tabBoard.Controls.Add(this.publishOtherButton);
            this.tabBoard.Controls.Add(this.escalateButton);
            this.tabBoard.Controls.Add(this.newTicketsButton);
            this.tabBoard.Controls.Add(this.subscribeButton);
            this.tabBoard.Controls.Add(this.unsubscribeButton);
            this.tabBoard.Name = "tabBoard";
            this.tabBoard.Text = "Ticket board";
            //
            this.labelTenantCaption.AutoSize = false;
            this.labelTenantCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelTenantCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTenantCaption.Location = new System.Drawing.Point(18, 20);
            this.labelTenantCaption.Name = "labelTenantCaption";
            this.labelTenantCaption.Size = new System.Drawing.Size(60, 22);
            this.labelTenantCaption.Text = "TENANT";
            //
            this.tenantComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.tenantComboBox.Location = new System.Drawing.Point(84, 14);
            this.tenantComboBox.Name = "tenantComboBox";
            this.tenantComboBox.Size = new System.Drawing.Size(140, 32);
            this.tenantComboBox.ToolTipText = "This session's tenant. The hub broadcasts everything; the SESSION decides what to render.";
            //
            this.subscribersLabel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.subscribersLabel.AutoSize = false;
            this.subscribersLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.subscribersLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.subscribersLabel.Location = new System.Drawing.Point(240, 20);
            this.subscribersLabel.Name = "subscribersLabel";
            this.subscribersLabel.Size = new System.Drawing.Size(424, 22);
            this.subscribersLabel.Text = "hub subscribers: —";
            this.subscribersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            this.ticketsGrid.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.ticketsGrid.Location = new System.Drawing.Point(18, 52);
            this.ticketsGrid.Name = "ticketsGrid";
            this.ticketsGrid.Size = new System.Drawing.Size(646, 220);
            //
            this.selectedLabel.AutoSize = false;
            this.selectedLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.selectedLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.selectedLabel.Location = new System.Drawing.Point(18, 278);
            this.selectedLabel.Name = "selectedLabel";
            this.selectedLabel.Size = new System.Drawing.Size(450, 22);
            this.selectedLabel.Text = "selected: none";
            //
            this.escalatedOnlyCheckBox.AutoSize = false;
            this.escalatedOnlyCheckBox.Location = new System.Drawing.Point(474, 274);
            this.escalatedOnlyCheckBox.Name = "escalatedOnlyCheckBox";
            this.escalatedOnlyCheckBox.Size = new System.Drawing.Size(190, 26);
            this.escalatedOnlyCheckBox.Text = "Escalated only";
            this.escalatedOnlyCheckBox.ToolTipText = "The filter is applied to the session's own list; live events keep arriving.";
            //
            this.notificationsList.Font = new System.Drawing.Font("monospace", 9F);
            this.notificationsList.Location = new System.Drawing.Point(18, 306);
            this.notificationsList.Name = "notificationsList";
            this.notificationsList.Size = new System.Drawing.Size(646, 108);
            //
            this.notificationCountLabel.AutoSize = false;
            this.notificationCountLabel.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.notificationCountLabel.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.notificationCountLabel.Location = new System.Drawing.Point(18, 418);
            this.notificationCountLabel.Name = "notificationCountLabel";
            this.notificationCountLabel.Size = new System.Drawing.Size(320, 22);
            this.notificationCountLabel.Text = "0 notifications in this session";
            //
            this.filteredOutLabel.AutoSize = false;
            this.filteredOutLabel.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.filteredOutLabel.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.filteredOutLabel.Location = new System.Drawing.Point(344, 418);
            this.filteredOutLabel.Name = "filteredOutLabel";
            this.filteredOutLabel.Size = new System.Drawing.Size(320, 22);
            this.filteredOutLabel.Text = "filtered out (wrong tenant): 0";
            this.filteredOutLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            this.publishButton.Location = new System.Drawing.Point(18, 446);
            this.publishButton.Name = "publishButton";
            this.publishButton.Size = new System.Drawing.Size(170, 34);
            this.publishButton.Text = "Publish (my tenant)";
            this.publishButton.Click += new System.EventHandler(this.publishButton_Click);
            //
            this.publishOtherButton.Location = new System.Drawing.Point(196, 446);
            this.publishOtherButton.Name = "publishOtherButton";
            this.publishOtherButton.Size = new System.Drawing.Size(170, 34);
            this.publishOtherButton.Text = "Publish (other tenant)";
            this.publishOtherButton.Click += new System.EventHandler(this.publishOtherButton_Click);
            //
            this.escalateButton.Location = new System.Drawing.Point(374, 446);
            this.escalateButton.Name = "escalateButton";
            this.escalateButton.Size = new System.Drawing.Size(150, 34);
            this.escalateButton.Text = "Escalate selected";
            this.escalateButton.ToolTipText = "The only ChangeType that also pops a toast in the sessions that watch this tenant.";
            this.escalateButton.Click += new System.EventHandler(this.escalateButton_Click);
            //
            this.newTicketsButton.Location = new System.Drawing.Point(532, 446);
            this.newTicketsButton.Name = "newTicketsButton";
            this.newTicketsButton.Size = new System.Drawing.Size(132, 34);
            this.newTicketsButton.Text = "Simulate 10 ↻";
            this.newTicketsButton.ToolTipText = "A task publishes 10 tickets through the hub, one per second.";
            this.newTicketsButton.Click += new System.EventHandler(this.newTicketsButton_Click);
            //
            this.subscribeButton.Enabled = false;
            this.subscribeButton.Location = new System.Drawing.Point(18, 486);
            this.subscribeButton.Name = "subscribeButton";
            this.subscribeButton.Size = new System.Drawing.Size(170, 34);
            this.subscribeButton.Text = "Subscribe";
            this.subscribeButton.Click += new System.EventHandler(this.subscribeButton_Click);
            //
            this.unsubscribeButton.Location = new System.Drawing.Point(196, 486);
            this.unsubscribeButton.Name = "unsubscribeButton";
            this.unsubscribeButton.Size = new System.Drawing.Size(170, 34);
            this.unsubscribeButton.Text = "Unsubscribe";
            this.unsubscribeButton.ToolTipText = "Every subscription needs an unsubscribe — here it is manual so the count is visible.";
            this.unsubscribeButton.Click += new System.EventHandler(this.unsubscribeButton_Click);
            //
            // tabCadence  (M04 · update cadence)
            //
            this.tabCadence.BackColor = System.Drawing.Color.White;
            this.tabCadence.Controls.Add(this.liveModeCheckBox);
            this.tabCadence.Controls.Add(this.labelCadenceCaption);
            this.tabCadence.Controls.Add(this.cadenceComboBox);
            this.tabCadence.Controls.Add(this.labelOpenCaption);
            this.tabCadence.Controls.Add(this.openTicketsLabel);
            this.tabCadence.Controls.Add(this.labelQueueCaption);
            this.tabCadence.Controls.Add(this.queueDepthLabel);
            this.tabCadence.Controls.Add(this.labelWaitCaption);
            this.tabCadence.Controls.Add(this.avgWaitLabel);
            this.tabCadence.Controls.Add(this.labelEventsCaption);
            this.tabCadence.Controls.Add(this.eventsReceivedLabel);
            this.tabCadence.Controls.Add(this.labelUpdatesCaption);
            this.tabCadence.Controls.Add(this.updatesAppliedLabel);
            this.tabCadence.Controls.Add(this.labelSkippedCaption);
            this.tabCadence.Controls.Add(this.skippedTicksLabel);
            this.tabCadence.Controls.Add(this.labelLastAppliedCaption);
            this.tabCadence.Controls.Add(this.lastAppliedLabel);
            this.tabCadence.Controls.Add(this.labelCadenceHint);
            this.tabCadence.Name = "tabCadence";
            this.tabCadence.Text = "Cadence";
            //
            this.liveModeCheckBox.AutoSize = false;
            this.liveModeCheckBox.Location = new System.Drawing.Point(18, 16);
            this.liveModeCheckBox.Name = "liveModeCheckBox";
            this.liveModeCheckBox.Size = new System.Drawing.Size(420, 26);
            this.liveModeCheckBox.Text = "Live mode — model simulator + refreshTimer + polling fallback";
            this.liveModeCheckBox.ToolTipText = "Starts the 50 ms model simulator and the UI refresh timer; requests polling only when there is no WebSocket.";
            //
            this.labelCadenceCaption.AutoSize = false;
            this.labelCadenceCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelCadenceCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelCadenceCaption.Location = new System.Drawing.Point(18, 56);
            this.labelCadenceCaption.Name = "labelCadenceCaption";
            this.labelCadenceCaption.Size = new System.Drawing.Size(90, 30);
            this.labelCadenceCaption.Text = "UI CADENCE";
            //
            this.cadenceComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cadenceComboBox.Location = new System.Drawing.Point(114, 52);
            this.cadenceComboBox.Name = "cadenceComboBox";
            this.cadenceComboBox.Size = new System.Drawing.Size(140, 32);
            this.cadenceComboBox.ToolTipText = "Reprograms refreshTimer.Interval on the running timer. The model keeps changing every 50 ms whatever you pick.";
            //
            this.labelOpenCaption.AutoSize = false;
            this.labelOpenCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelOpenCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelOpenCaption.Location = new System.Drawing.Point(18, 108);
            this.labelOpenCaption.Name = "labelOpenCaption";
            this.labelOpenCaption.Size = new System.Drawing.Size(200, 18);
            this.labelOpenCaption.Text = "OPEN TICKETS";
            this.openTicketsLabel.AutoSize = false;
            this.openTicketsLabel.Font = new System.Drawing.Font("default", 20F, System.Drawing.FontStyle.Bold);
            this.openTicketsLabel.Location = new System.Drawing.Point(18, 128);
            this.openTicketsLabel.Name = "openTicketsLabel";
            this.openTicketsLabel.Size = new System.Drawing.Size(200, 36);
            this.openTicketsLabel.Text = "—";
            //
            this.labelQueueCaption.AutoSize = false;
            this.labelQueueCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelQueueCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelQueueCaption.Location = new System.Drawing.Point(240, 108);
            this.labelQueueCaption.Name = "labelQueueCaption";
            this.labelQueueCaption.Size = new System.Drawing.Size(200, 18);
            this.labelQueueCaption.Text = "QUEUE DEPTH";
            this.queueDepthLabel.AutoSize = false;
            this.queueDepthLabel.Font = new System.Drawing.Font("default", 20F, System.Drawing.FontStyle.Bold);
            this.queueDepthLabel.Location = new System.Drawing.Point(240, 128);
            this.queueDepthLabel.Name = "queueDepthLabel";
            this.queueDepthLabel.Size = new System.Drawing.Size(200, 36);
            this.queueDepthLabel.Text = "—";
            //
            this.labelWaitCaption.AutoSize = false;
            this.labelWaitCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelWaitCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelWaitCaption.Location = new System.Drawing.Point(462, 108);
            this.labelWaitCaption.Name = "labelWaitCaption";
            this.labelWaitCaption.Size = new System.Drawing.Size(202, 18);
            this.labelWaitCaption.Text = "AVG WAIT";
            this.avgWaitLabel.AutoSize = false;
            this.avgWaitLabel.Font = new System.Drawing.Font("default", 20F, System.Drawing.FontStyle.Bold);
            this.avgWaitLabel.Location = new System.Drawing.Point(462, 128);
            this.avgWaitLabel.Name = "avgWaitLabel";
            this.avgWaitLabel.Size = new System.Drawing.Size(202, 36);
            this.avgWaitLabel.Text = "—";
            //
            this.labelEventsCaption.AutoSize = false;
            this.labelEventsCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelEventsCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelEventsCaption.Location = new System.Drawing.Point(18, 196);
            this.labelEventsCaption.Name = "labelEventsCaption";
            this.labelEventsCaption.Size = new System.Drawing.Size(200, 18);
            this.labelEventsCaption.Text = "EVENTS RECEIVED (model)";
            this.eventsReceivedLabel.AutoSize = false;
            this.eventsReceivedLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.eventsReceivedLabel.Location = new System.Drawing.Point(18, 214);
            this.eventsReceivedLabel.Name = "eventsReceivedLabel";
            this.eventsReceivedLabel.Size = new System.Drawing.Size(200, 30);
            this.eventsReceivedLabel.Text = "0";
            //
            this.labelUpdatesCaption.AutoSize = false;
            this.labelUpdatesCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelUpdatesCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelUpdatesCaption.Location = new System.Drawing.Point(240, 196);
            this.labelUpdatesCaption.Name = "labelUpdatesCaption";
            this.labelUpdatesCaption.Size = new System.Drawing.Size(200, 18);
            this.labelUpdatesCaption.Text = "UPDATES APPLIED (UI)";
            this.updatesAppliedLabel.AutoSize = false;
            this.updatesAppliedLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.updatesAppliedLabel.Location = new System.Drawing.Point(240, 214);
            this.updatesAppliedLabel.Name = "updatesAppliedLabel";
            this.updatesAppliedLabel.Size = new System.Drawing.Size(200, 30);
            this.updatesAppliedLabel.Text = "0";
            //
            this.labelSkippedCaption.AutoSize = false;
            this.labelSkippedCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelSkippedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelSkippedCaption.Location = new System.Drawing.Point(462, 196);
            this.labelSkippedCaption.Name = "labelSkippedCaption";
            this.labelSkippedCaption.Size = new System.Drawing.Size(202, 18);
            this.labelSkippedCaption.Text = "TICKS REFUSED (overlap)";
            this.skippedTicksLabel.AutoSize = false;
            this.skippedTicksLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.skippedTicksLabel.Location = new System.Drawing.Point(462, 214);
            this.skippedTicksLabel.Name = "skippedTicksLabel";
            this.skippedTicksLabel.Size = new System.Drawing.Size(202, 30);
            this.skippedTicksLabel.Text = "0";
            //
            this.labelLastAppliedCaption.AutoSize = false;
            this.labelLastAppliedCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelLastAppliedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLastAppliedCaption.Location = new System.Drawing.Point(18, 258);
            this.labelLastAppliedCaption.Name = "labelLastAppliedCaption";
            this.labelLastAppliedCaption.Size = new System.Drawing.Size(200, 18);
            this.labelLastAppliedCaption.Text = "LAST APPLIED";
            this.lastAppliedLabel.AutoSize = false;
            this.lastAppliedLabel.Font = new System.Drawing.Font("monospace", 11F);
            this.lastAppliedLabel.Location = new System.Drawing.Point(18, 276);
            this.lastAppliedLabel.Name = "lastAppliedLabel";
            this.lastAppliedLabel.Size = new System.Drawing.Size(300, 26);
            this.lastAppliedLabel.Text = "—";
            //
            this.labelCadenceHint.AutoSize = false;
            this.labelCadenceHint.Font = new System.Drawing.Font("default", 9F);
            this.labelCadenceHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelCadenceHint.Location = new System.Drawing.Point(18, 316);
            this.labelCadenceHint.Name = "labelCadenceHint";
            this.labelCadenceHint.Size = new System.Drawing.Size(646, 80);
            this.labelCadenceHint.Text = "The simulator changes the model every 50 ms and only sets a dirty flag; refreshTimer_Tick applies ONE snapshot per tick and its changes ride back on that timer request — no Application.Update() is needed for a timer refresh. Model cadence, UI cadence, push cadence and polling cadence are four different numbers.";
            this.labelCadenceHint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // tabSession  (M02 · session inspector and lifecycle)
            //
            this.tabSession.BackColor = System.Drawing.Color.White;
            this.tabSession.Controls.Add(this.diagnosticsGroupBox);
            this.tabSession.Controls.Add(this.liveSessionsLabel);
            this.tabSession.Controls.Add(this.lifecycleListBox);
            this.tabSession.Controls.Add(this.incrementButton);
            this.tabSession.Controls.Add(this.backgroundButton);
            this.tabSession.Controls.Add(this.faultButton);
            this.tabSession.Name = "tabSession";
            this.tabSession.Text = "Session";
            //
            this.diagnosticsGroupBox.Controls.Add(this.labelTimeCaption);
            this.diagnosticsGroupBox.Controls.Add(this.timeLabel);
            this.diagnosticsGroupBox.Controls.Add(this.labelClientCaption);
            this.diagnosticsGroupBox.Controls.Add(this.clientIdLabel);
            this.diagnosticsGroupBox.Controls.Add(this.labelSessionCaption);
            this.diagnosticsGroupBox.Controls.Add(this.sessionIdLabel);
            this.diagnosticsGroupBox.Controls.Add(this.labelBrowserCaption);
            this.diagnosticsGroupBox.Controls.Add(this.browserLabel);
            this.diagnosticsGroupBox.Controls.Add(this.labelThreadCaption);
            this.diagnosticsGroupBox.Controls.Add(this.threadLabel);
            this.diagnosticsGroupBox.Controls.Add(this.labelCounterCaption);
            this.diagnosticsGroupBox.Controls.Add(this.counterLabel);
            this.diagnosticsGroupBox.Location = new System.Drawing.Point(18, 14);
            this.diagnosticsGroupBox.Name = "diagnosticsGroupBox";
            this.diagnosticsGroupBox.Size = new System.Drawing.Size(646, 216);
            this.diagnosticsGroupBox.Text = "Session inspector";
            //
            this.labelTimeCaption.AutoSize = false;
            this.labelTimeCaption.Font = new System.Drawing.Font("monospace", 9F);
            this.labelTimeCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTimeCaption.Location = new System.Drawing.Point(20, 22);
            this.labelTimeCaption.Name = "labelTimeCaption";
            this.labelTimeCaption.Size = new System.Drawing.Size(190, 22);
            this.labelTimeCaption.Text = "current time";
            this.timeLabel.AutoSize = false;
            this.timeLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.timeLabel.Location = new System.Drawing.Point(216, 22);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(410, 22);
            this.timeLabel.Text = "—";
            //
            this.labelClientCaption.AutoSize = false;
            this.labelClientCaption.Font = new System.Drawing.Font("monospace", 9F);
            this.labelClientCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelClientCaption.Location = new System.Drawing.Point(20, 50);
            this.labelClientCaption.Name = "labelClientCaption";
            this.labelClientCaption.Size = new System.Drawing.Size(190, 22);
            this.labelClientCaption.Text = "ClientId (the browser)";
            this.clientIdLabel.AutoSize = false;
            this.clientIdLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.clientIdLabel.Location = new System.Drawing.Point(216, 50);
            this.clientIdLabel.Name = "clientIdLabel";
            this.clientIdLabel.Size = new System.Drawing.Size(410, 22);
            this.clientIdLabel.Text = "—";
            //
            this.labelSessionCaption.AutoSize = false;
            this.labelSessionCaption.Font = new System.Drawing.Font("monospace", 9F);
            this.labelSessionCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelSessionCaption.Location = new System.Drawing.Point(20, 78);
            this.labelSessionCaption.Name = "labelSessionCaption";
            this.labelSessionCaption.Size = new System.Drawing.Size(190, 22);
            this.labelSessionCaption.Text = "SessionId (this tab)";
            this.sessionIdLabel.AutoSize = false;
            this.sessionIdLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.sessionIdLabel.Location = new System.Drawing.Point(216, 78);
            this.sessionIdLabel.Name = "sessionIdLabel";
            this.sessionIdLabel.Size = new System.Drawing.Size(410, 22);
            this.sessionIdLabel.Text = "—";
            //
            this.labelBrowserCaption.AutoSize = false;
            this.labelBrowserCaption.Font = new System.Drawing.Font("monospace", 9F);
            this.labelBrowserCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelBrowserCaption.Location = new System.Drawing.Point(20, 106);
            this.labelBrowserCaption.Name = "labelBrowserCaption";
            this.labelBrowserCaption.Size = new System.Drawing.Size(190, 22);
            this.labelBrowserCaption.Text = "Application.Browser";
            this.browserLabel.AutoSize = false;
            this.browserLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.browserLabel.Location = new System.Drawing.Point(216, 106);
            this.browserLabel.Name = "browserLabel";
            this.browserLabel.Size = new System.Drawing.Size(410, 22);
            this.browserLabel.Text = "—";
            //
            this.labelThreadCaption.AutoSize = false;
            this.labelThreadCaption.Font = new System.Drawing.Font("monospace", 9F);
            this.labelThreadCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelThreadCaption.Location = new System.Drawing.Point(20, 134);
            this.labelThreadCaption.Name = "labelThreadCaption";
            this.labelThreadCaption.Size = new System.Drawing.Size(190, 22);
            this.labelThreadCaption.Text = "server thread";
            this.threadLabel.AutoSize = false;
            this.threadLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.threadLabel.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.threadLabel.Location = new System.Drawing.Point(216, 134);
            this.threadLabel.Name = "threadLabel";
            this.threadLabel.Size = new System.Drawing.Size(410, 22);
            this.threadLabel.Text = "—";
            //
            this.labelCounterCaption.AutoSize = false;
            this.labelCounterCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelCounterCaption.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelCounterCaption.Location = new System.Drawing.Point(20, 168);
            this.labelCounterCaption.Name = "labelCounterCaption";
            this.labelCounterCaption.Size = new System.Drawing.Size(190, 22);
            this.labelCounterCaption.Text = "Session.Counter ✓";
            this.counterLabel.AutoSize = false;
            this.counterLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.counterLabel.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.counterLabel.Location = new System.Drawing.Point(216, 168);
            this.counterLabel.Name = "counterLabel";
            this.counterLabel.Size = new System.Drawing.Size(410, 22);
            this.counterLabel.Text = "0 ← Application.Session.Counter (this session only)";
            //
            this.liveSessionsLabel.AutoSize = false;
            this.liveSessionsLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.liveSessionsLabel.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.liveSessionsLabel.Location = new System.Drawing.Point(18, 238);
            this.liveSessionsLabel.Name = "liveSessionsLabel";
            this.liveSessionsLabel.Size = new System.Drawing.Size(646, 22);
            this.liveSessionsLabel.Text = "live sessions: —";
            //
            this.lifecycleListBox.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lifecycleListBox.Font = new System.Drawing.Font("monospace", 9F);
            this.lifecycleListBox.Location = new System.Drawing.Point(18, 266);
            this.lifecycleListBox.Name = "lifecycleListBox";
            this.lifecycleListBox.Size = new System.Drawing.Size(646, 206);
            //
            this.incrementButton.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.incrementButton.Location = new System.Drawing.Point(18, 482);
            this.incrementButton.Name = "incrementButton";
            this.incrementButton.Size = new System.Drawing.Size(170, 36);
            this.incrementButton.Text = "Session counter +1";
            this.incrementButton.Click += new System.EventHandler(this.incrementButton_Click);
            //
            this.backgroundButton.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.backgroundButton.Location = new System.Drawing.Point(196, 482);
            this.backgroundButton.Name = "backgroundButton";
            this.backgroundButton.Size = new System.Drawing.Size(170, 36);
            this.backgroundButton.Text = "Background update";
            this.backgroundButton.ToolTipText = "Captures Application.Current, waits 1.5 s on a task, then pushes with Application.Update(context, …).";
            this.backgroundButton.Click += new System.EventHandler(this.backgroundButton_Click);
            //
            this.faultButton.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.faultButton.Location = new System.Drawing.Point(374, 482);
            this.faultButton.Name = "faultButton";
            this.faultButton.Size = new System.Drawing.Size(170, 36);
            this.faultButton.Text = "Background fault";
            this.faultButton.ToolTipText = "The task throws: caught inside, logged with the session id, safe message pushed.";
            this.faultButton.Click += new System.EventHandler(this.faultButton_Click);
            //
            // tabHealth  (M07 · real-time health, configuration and the production checklist)
            //
            this.tabHealth.BackColor = System.Drawing.Color.White;
            this.tabHealth.Controls.Add(this.labelHealthTitle);
            this.tabHealth.Controls.Add(this.websocketModeLabel);
            this.tabHealth.Controls.Add(this.pollingLabel);
            this.tabHealth.Controls.Add(this.subscriptionCountLabel);
            this.tabHealth.Controls.Add(this.updateRateLabel);
            this.tabHealth.Controls.Add(this.labelConfigTitle);
            this.tabHealth.Controls.Add(this.configListBox);
            this.tabHealth.Controls.Add(this.labelChecklistTitle);
            this.tabHealth.Controls.Add(this.checklistBox);
            this.tabHealth.Name = "tabHealth";
            this.tabHealth.Text = "Health · config";
            //
            this.labelHealthTitle.AutoSize = false;
            this.labelHealthTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelHealthTitle.Location = new System.Drawing.Point(18, 12);
            this.labelHealthTitle.Name = "labelHealthTitle";
            this.labelHealthTitle.Size = new System.Drawing.Size(646, 26);
            this.labelHealthTitle.Text = "Real-time health  ·  refreshed by healthTimer every 2 s";
            //
            this.websocketModeLabel.AutoSize = false;
            this.websocketModeLabel.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.websocketModeLabel.Location = new System.Drawing.Point(18, 44);
            this.websocketModeLabel.Name = "websocketModeLabel";
            this.websocketModeLabel.Size = new System.Drawing.Size(320, 26);
            this.websocketModeLabel.Text = "—";
            //
            this.pollingLabel.AutoSize = false;
            this.pollingLabel.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.pollingLabel.Location = new System.Drawing.Point(344, 44);
            this.pollingLabel.Name = "pollingLabel";
            this.pollingLabel.Size = new System.Drawing.Size(320, 26);
            this.pollingLabel.Text = "—";
            //
            this.subscriptionCountLabel.AutoSize = false;
            this.subscriptionCountLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.subscriptionCountLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.subscriptionCountLabel.Location = new System.Drawing.Point(18, 74);
            this.subscriptionCountLabel.Name = "subscriptionCountLabel";
            this.subscriptionCountLabel.Size = new System.Drawing.Size(320, 22);
            this.subscriptionCountLabel.Text = "—";
            //
            this.updateRateLabel.AutoSize = false;
            this.updateRateLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.updateRateLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.updateRateLabel.Location = new System.Drawing.Point(344, 74);
            this.updateRateLabel.Name = "updateRateLabel";
            this.updateRateLabel.Size = new System.Drawing.Size(320, 22);
            this.updateRateLabel.Text = "—";
            //
            this.labelConfigTitle.AutoSize = false;
            this.labelConfigTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelConfigTitle.Location = new System.Drawing.Point(18, 106);
            this.labelConfigTitle.Name = "labelConfigTitle";
            this.labelConfigTitle.Size = new System.Drawing.Size(646, 24);
            this.labelConfigTitle.Text = "Effective configuration  ·  Default.json + HealthCheck.json (read-only)";
            //
            this.configListBox.Font = new System.Drawing.Font("monospace", 9F);
            this.configListBox.Location = new System.Drawing.Point(18, 132);
            this.configListBox.Name = "configListBox";
            this.configListBox.Size = new System.Drawing.Size(646, 160);
            //
            this.labelChecklistTitle.AutoSize = false;
            this.labelChecklistTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelChecklistTitle.Location = new System.Drawing.Point(18, 300);
            this.labelChecklistTitle.Name = "labelChecklistTitle";
            this.labelChecklistTitle.Size = new System.Drawing.Size(646, 24);
            this.labelChecklistTitle.Text = "Performance checklist  ·  tick an item to log where this app satisfies it";
            //
            this.checklistBox.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.checklistBox.CheckOnClick = true;
            this.checklistBox.Font = new System.Drawing.Font("monospace", 9F);
            this.checklistBox.Location = new System.Drawing.Point(18, 326);
            this.checklistBox.Name = "checklistBox";
            this.checklistBox.Size = new System.Drawing.Size(646, 194);
            //
            // panelTrace  (Server → Browser live push trace)
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.labelStatus);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelBanner);
            this.panelTrace.Controls.Add(this.labelState);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(748, 120);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(570, 568);
            //
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 12);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(330, 28);
            this.labelTraceTitle.Text = "Server → Browser  ·  live push trace";
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(356, 14);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(194, 24);
            this.labelStatus.Text = "● ready";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 46);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(530, 350);
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(20, 404);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(530, 44);
            this.labelBanner.Text = "";
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            this.labelState.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelState.AutoSize = false;
            this.labelState.Font = new System.Drawing.Font("monospace", 9F);
            this.labelState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelState.Location = new System.Drawing.Point(20, 452);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(530, 82);
            this.labelState.Text = "";
            this.labelState.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 536);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(530, 22);
            this.labelTraceFooter.Text = "→ push = Application.Update   ·   ← request = the browser asked   ·   • server = decision";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.startButton);
            this.panelActions.Controls.Add(this.stopButton);
            this.panelActions.Controls.Add(this.openSessionButton);
            this.panelActions.Controls.Add(this.exitButton);
            this.panelActions.Controls.Add(this.clearButton);
            this.panelActions.Location = new System.Drawing.Point(30, 700);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            this.startButton.Location = new System.Drawing.Point(0, 4);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(160, 36);
            this.startButton.Text = "▶ Start heartbeat";
            this.startButton.ToolTipText = "One push per second from a task: clock, load, activity. A second click is refused.";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            //
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(168, 4);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(100, 36);
            this.stopButton.Text = "■ Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            //
            this.openSessionButton.Location = new System.Drawing.Point(292, 4);
            this.openSessionButton.Name = "openSessionButton";
            this.openSessionButton.Size = new System.Drawing.Size(200, 36);
            this.openSessionButton.Text = "Open another session ↗";
            this.openSessionButton.ToolTipText = "A second tab: same ClientId (the browser), a new SessionId, its own page and counters.";
            this.openSessionButton.Click += new System.EventHandler(this.openSessionButton_Click);
            //
            this.exitButton.Location = new System.Drawing.Point(500, 4);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(170, 36);
            this.exitButton.Text = "End this session";
            this.exitButton.ToolTipText = "Application.Exit() → ApplicationExit runs the one cleanup: hub and registry unsubscribed, loops stopped.";
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            //
            this.clearButton.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.clearButton.Location = new System.Drawing.Point(1178, 4);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(110, 36);
            this.clearButton.Text = "Clear trace";
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            //
            // refreshTimer  (UI cadence of the dashboard — a Component with no visual surface)
            //
            this.refreshTimer.Interval = 1000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            //
            // healthTimer  (refreshes the health snapshot every 2 s)
            //
            this.healthTimer.Interval = 2000;
            this.healthTimer.Tick += new System.EventHandler(this.healthTimer_Tick);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.featureTabs);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 760);
            this.Text = "TicketOps Live — Production Review";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.statusPanel.ResumeLayout(false);
            this.tabImport.ResumeLayout(false);
            this.tabBoard.ResumeLayout(false);
            this.tabCadence.ResumeLayout(false);
            this.tabSession.ResumeLayout(false);
            this.tabHealth.ResumeLayout(false);
            this.diagnosticsGroupBox.ResumeLayout(false);
            this.featureTabs.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel statusPanel;
        private Wisej.Web.Label labelStripCaption;
        private Wisej.Web.Label clockLabel;
        private Wisej.Web.Label connectionLabel;
        private Wisej.Web.Label activityLabel;
        private Wisej.Web.Label labelLoadCaption;
        private Wisej.Web.ProgressBar serverLoadBar;
        private Wisej.Web.Label loadValueLabel;
        private Wisej.Web.TabControl featureTabs;
        private Wisej.Web.TabPage tabImport;
        private Wisej.Web.TabPage tabBoard;
        private Wisej.Web.TabPage tabCadence;
        private Wisej.Web.TabPage tabSession;
        private Wisej.Web.TabPage tabHealth;
        private Wisej.Web.Label jobIdLabel;
        private Wisej.Web.Label elapsedLabel;
        private Wisej.Web.Label importStatusLabel;
        private Wisej.Web.ProgressBar importProgressBar;
        private Wisej.Web.Label labelRecordsCaption;
        private Wisej.Web.Label recordsImportedLabel;
        private Wisej.Web.CheckBox failAt87CheckBox;
        private Wisej.Web.ListBox importLogListBox;
        private Wisej.Web.Button startImportButton;
        private Wisej.Web.Button cancelImportButton;
        private Wisej.Web.Label labelTenantCaption;
        private Wisej.Web.ComboBox tenantComboBox;
        private Wisej.Web.Label subscribersLabel;
        private Wisej.Web.DataGridView ticketsGrid;
        private Wisej.Web.Label selectedLabel;
        private Wisej.Web.CheckBox escalatedOnlyCheckBox;
        private Wisej.Web.ListBox notificationsList;
        private Wisej.Web.Label notificationCountLabel;
        private Wisej.Web.Label filteredOutLabel;
        private Wisej.Web.Button publishButton;
        private Wisej.Web.Button publishOtherButton;
        private Wisej.Web.Button escalateButton;
        private Wisej.Web.Button newTicketsButton;
        private Wisej.Web.Button subscribeButton;
        private Wisej.Web.Button unsubscribeButton;
        private Wisej.Web.CheckBox liveModeCheckBox;
        private Wisej.Web.Label labelCadenceCaption;
        private Wisej.Web.ComboBox cadenceComboBox;
        private Wisej.Web.Label labelOpenCaption;
        private Wisej.Web.Label openTicketsLabel;
        private Wisej.Web.Label labelQueueCaption;
        private Wisej.Web.Label queueDepthLabel;
        private Wisej.Web.Label labelWaitCaption;
        private Wisej.Web.Label avgWaitLabel;
        private Wisej.Web.Label labelEventsCaption;
        private Wisej.Web.Label eventsReceivedLabel;
        private Wisej.Web.Label labelUpdatesCaption;
        private Wisej.Web.Label updatesAppliedLabel;
        private Wisej.Web.Label labelSkippedCaption;
        private Wisej.Web.Label skippedTicksLabel;
        private Wisej.Web.Label labelLastAppliedCaption;
        private Wisej.Web.Label lastAppliedLabel;
        private Wisej.Web.Label labelCadenceHint;
        private Wisej.Web.GroupBox diagnosticsGroupBox;
        private Wisej.Web.Label labelTimeCaption;
        private Wisej.Web.Label timeLabel;
        private Wisej.Web.Label labelClientCaption;
        private Wisej.Web.Label clientIdLabel;
        private Wisej.Web.Label labelSessionCaption;
        private Wisej.Web.Label sessionIdLabel;
        private Wisej.Web.Label labelBrowserCaption;
        private Wisej.Web.Label browserLabel;
        private Wisej.Web.Label labelThreadCaption;
        private Wisej.Web.Label threadLabel;
        private Wisej.Web.Label labelCounterCaption;
        private Wisej.Web.Label counterLabel;
        private Wisej.Web.Label liveSessionsLabel;
        private Wisej.Web.ListBox lifecycleListBox;
        private Wisej.Web.Button incrementButton;
        private Wisej.Web.Button backgroundButton;
        private Wisej.Web.Button faultButton;
        private Wisej.Web.Label labelHealthTitle;
        private Wisej.Web.Label websocketModeLabel;
        private Wisej.Web.Label pollingLabel;
        private Wisej.Web.Label subscriptionCountLabel;
        private Wisej.Web.Label updateRateLabel;
        private Wisej.Web.Label labelConfigTitle;
        private Wisej.Web.ListBox configListBox;
        private Wisej.Web.Label labelChecklistTitle;
        private Wisej.Web.CheckedListBox checklistBox;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button startButton;
        private Wisej.Web.Button stopButton;
        private Wisej.Web.Button openSessionButton;
        private Wisej.Web.Button exitButton;
        private Wisej.Web.Button clearButton;
        private Wisej.Web.Timer refreshTimer;
        private Wisej.Web.Timer healthTimer;
    }
}
