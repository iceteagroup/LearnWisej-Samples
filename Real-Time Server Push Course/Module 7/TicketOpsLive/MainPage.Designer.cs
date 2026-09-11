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
            this.clockLabel = new Wisej.Web.Label();
            this.connectionLabel = new Wisej.Web.Label();
            this.activityLabel = new Wisej.Web.Label();
            this.labelLoadCaption = new Wisej.Web.Label();
            this.serverLoadBar = new Wisej.Web.ProgressBar();
            this.loadValueLabel = new Wisej.Web.Label();
            this.startButton = new Wisej.Web.Button();
            this.stopButton = new Wisej.Web.Button();
            this.featureTabs = new Wisej.Web.TabControl();
            this.tabImport = new Wisej.Web.TabPage();
            this.tabBoard = new Wisej.Web.TabPage();
            this.tabCadence = new Wisej.Web.TabPage();
            this.tabSession = new Wisej.Web.TabPage();
            this.tabHealth = new Wisej.Web.TabPage();
            this.importStatusLabel = new Wisej.Web.Label();
            this.elapsedLabel = new Wisej.Web.Label();
            this.importProgressBar = new Wisej.Web.ProgressBar();
            this.recordsImportedLabel = new Wisej.Web.Label();
            this.importLogListBox = new Wisej.Web.ListBox();
            this.startImportButton = new Wisej.Web.Button();
            this.cancelImportButton = new Wisej.Web.Button();
            this.failAt87Button = new Wisej.Web.Button();
            this.labelTenantCaption = new Wisej.Web.Label();
            this.tenantComboBox = new Wisej.Web.ComboBox();
            this.escalatedOnlyCheckBox = new Wisej.Web.CheckBox();
            this.ticketsGrid = new Wisej.Web.DataGridView();
            this.notificationCountLabel = new Wisej.Web.Label();
            this.notificationsList = new Wisej.Web.ListBox();
            this.publishButton = new Wisej.Web.Button();
            this.escalateButton = new Wisej.Web.Button();
            this.newTicketsButton = new Wisej.Web.Button();
            this.subscribeButton = new Wisej.Web.Button();
            this.unsubscribeButton = new Wisej.Web.Button();
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
            this.labelLastAppliedCaption = new Wisej.Web.Label();
            this.lastAppliedLabel = new Wisej.Web.Label();
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
            this.incrementButton = new Wisej.Web.Button();
            this.backgroundButton = new Wisej.Web.Button();
            this.openSessionButton = new Wisej.Web.Button();
            this.liveSessionsLabel = new Wisej.Web.Label();
            this.lifecycleListBox = new Wisej.Web.ListBox();
            this.labelHealthTitle = new Wisej.Web.Label();
            this.websocketModeLabel = new Wisej.Web.Label();
            this.pollingLabel = new Wisej.Web.Label();
            this.subscriptionCountLabel = new Wisej.Web.Label();
            this.updateRateLabel = new Wisej.Web.Label();
            this.labelConfigTitle = new Wisej.Web.Label();
            this.configListBox = new Wisej.Web.ListBox();
            this.labelChecklistTitle = new Wisej.Web.Label();
            this.checklistBox = new Wisej.Web.CheckedListBox();
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
            this.SuspendLayout();
            //
            // statusPanel
            //
            this.statusPanel.BackColor = System.Drawing.Color.White;
            this.statusPanel.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.statusPanel.Controls.Add(this.clockLabel);
            this.statusPanel.Controls.Add(this.connectionLabel);
            this.statusPanel.Controls.Add(this.activityLabel);
            this.statusPanel.Controls.Add(this.labelLoadCaption);
            this.statusPanel.Controls.Add(this.serverLoadBar);
            this.statusPanel.Controls.Add(this.loadValueLabel);
            this.statusPanel.Controls.Add(this.startButton);
            this.statusPanel.Controls.Add(this.stopButton);
            this.statusPanel.Location = new System.Drawing.Point(20, 18);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(1000, 80);
            //
            // clockLabel
            //
            this.clockLabel.AutoSize = false;
            this.clockLabel.Font = new System.Drawing.Font("monospace", 22F, System.Drawing.FontStyle.Bold);
            this.clockLabel.Location = new System.Drawing.Point(16, 16);
            this.clockLabel.Name = "clockLabel";
            this.clockLabel.Size = new System.Drawing.Size(170, 44);
            this.clockLabel.Text = "--:--:--";
            this.clockLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // connectionLabel
            //
            this.connectionLabel.AutoSize = false;
            this.connectionLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.connectionLabel.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.connectionLabel.Location = new System.Drawing.Point(200, 14);
            this.connectionLabel.Name = "connectionLabel";
            this.connectionLabel.Size = new System.Drawing.Size(380, 24);
            this.connectionLabel.Text = "○ connecting…";
            //
            // activityLabel
            //
            this.activityLabel.AutoSize = false;
            this.activityLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.activityLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.activityLabel.Location = new System.Drawing.Point(200, 42);
            this.activityLabel.Name = "activityLabel";
            this.activityLabel.Size = new System.Drawing.Size(380, 22);
            this.activityLabel.Text = "Heartbeat not running";
            //
            // labelLoadCaption
            //
            this.labelLoadCaption.AutoSize = false;
            this.labelLoadCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelLoadCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLoadCaption.Location = new System.Drawing.Point(596, 12);
            this.labelLoadCaption.Name = "labelLoadCaption";
            this.labelLoadCaption.Size = new System.Drawing.Size(180, 18);
            this.labelLoadCaption.Text = "SERVER LOAD";
            //
            // serverLoadBar
            //
            this.serverLoadBar.Location = new System.Drawing.Point(596, 36);
            this.serverLoadBar.Name = "serverLoadBar";
            this.serverLoadBar.Size = new System.Drawing.Size(180, 24);
            this.serverLoadBar.Value = 0;
            //
            // loadValueLabel
            //
            this.loadValueLabel.AutoSize = false;
            this.loadValueLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.loadValueLabel.Location = new System.Drawing.Point(780, 30);
            this.loadValueLabel.Name = "loadValueLabel";
            this.loadValueLabel.Size = new System.Drawing.Size(70, 36);
            this.loadValueLabel.Text = "0 %";
            this.loadValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // startButton
            //
            this.startButton.Location = new System.Drawing.Point(862, 8);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(124, 30);
            this.startButton.Text = "▶ Start heartbeat";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            //
            // stopButton
            //
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(862, 42);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(124, 30);
            this.stopButton.Text = "■ Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            //
            // featureTabs
            //
            this.featureTabs.Location = new System.Drawing.Point(20, 112);
            this.featureTabs.Name = "featureTabs";
            this.featureTabs.SelectedIndex = 0;
            this.featureTabs.Size = new System.Drawing.Size(1000, 470);
            this.featureTabs.TabPages.Add(this.tabImport);
            this.featureTabs.TabPages.Add(this.tabBoard);
            this.featureTabs.TabPages.Add(this.tabCadence);
            this.featureTabs.TabPages.Add(this.tabSession);
            this.featureTabs.TabPages.Add(this.tabHealth);
            this.featureTabs.SelectedIndexChanged += new System.EventHandler(this.featureTabs_SelectedIndexChanged);
            //
            // tabImport
            //
            this.tabImport.BackColor = System.Drawing.Color.White;
            this.tabImport.Controls.Add(this.importStatusLabel);
            this.tabImport.Controls.Add(this.elapsedLabel);
            this.tabImport.Controls.Add(this.importProgressBar);
            this.tabImport.Controls.Add(this.recordsImportedLabel);
            this.tabImport.Controls.Add(this.importLogListBox);
            this.tabImport.Controls.Add(this.startImportButton);
            this.tabImport.Controls.Add(this.cancelImportButton);
            this.tabImport.Controls.Add(this.failAt87Button);
            this.tabImport.Name = "tabImport";
            this.tabImport.Text = "Import monitor";
            //
            // importStatusLabel
            //
            this.importStatusLabel.AutoSize = false;
            this.importStatusLabel.Font = new System.Drawing.Font("default", 11F);
            this.importStatusLabel.Location = new System.Drawing.Point(18, 14);
            this.importStatusLabel.Name = "importStatusLabel";
            this.importStatusLabel.Size = new System.Drawing.Size(600, 26);
            this.importStatusLabel.Text = "Idle — ready to import";
            //
            // elapsedLabel
            //
            this.elapsedLabel.AutoSize = false;
            this.elapsedLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.elapsedLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.elapsedLabel.Location = new System.Drawing.Point(744, 16);
            this.elapsedLabel.Name = "elapsedLabel";
            this.elapsedLabel.Size = new System.Drawing.Size(220, 22);
            this.elapsedLabel.Text = "Elapsed 0.00 s";
            this.elapsedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // importProgressBar
            //
            this.importProgressBar.Location = new System.Drawing.Point(18, 50);
            this.importProgressBar.Name = "importProgressBar";
            this.importProgressBar.Size = new System.Drawing.Size(790, 24);
            this.importProgressBar.Value = 0;
            //
            // recordsImportedLabel
            //
            this.recordsImportedLabel.AutoSize = false;
            this.recordsImportedLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.recordsImportedLabel.Location = new System.Drawing.Point(824, 46);
            this.recordsImportedLabel.Name = "recordsImportedLabel";
            this.recordsImportedLabel.Size = new System.Drawing.Size(140, 30);
            this.recordsImportedLabel.Text = "0/200";
            this.recordsImportedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // importLogListBox
            //
            this.importLogListBox.Font = new System.Drawing.Font("monospace", 9F);
            this.importLogListBox.Location = new System.Drawing.Point(18, 88);
            this.importLogListBox.Name = "importLogListBox";
            this.importLogListBox.Size = new System.Drawing.Size(740, 330);
            //
            // startImportButton
            //
            this.startImportButton.Location = new System.Drawing.Point(776, 88);
            this.startImportButton.Name = "startImportButton";
            this.startImportButton.Size = new System.Drawing.Size(188, 36);
            this.startImportButton.Text = "Start Import";
            this.startImportButton.Click += new System.EventHandler(this.startImportButton_Click);
            //
            // cancelImportButton
            //
            this.cancelImportButton.Enabled = false;
            this.cancelImportButton.Location = new System.Drawing.Point(776, 132);
            this.cancelImportButton.Name = "cancelImportButton";
            this.cancelImportButton.Size = new System.Drawing.Size(188, 36);
            this.cancelImportButton.Text = "Cancel Import";
            this.cancelImportButton.Click += new System.EventHandler(this.cancelImportButton_Click);
            //
            // failAt87Button
            //
            this.failAt87Button.ForeColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.failAt87Button.Location = new System.Drawing.Point(776, 176);
            this.failAt87Button.Name = "failAt87Button";
            this.failAt87Button.Size = new System.Drawing.Size(188, 36);
            this.failAt87Button.Text = "Fail at 87";
            this.failAt87Button.Click += new System.EventHandler(this.failAt87Button_Click);
            //
            // tabBoard
            //
            this.tabBoard.BackColor = System.Drawing.Color.White;
            this.tabBoard.Controls.Add(this.labelTenantCaption);
            this.tabBoard.Controls.Add(this.tenantComboBox);
            this.tabBoard.Controls.Add(this.escalatedOnlyCheckBox);
            this.tabBoard.Controls.Add(this.ticketsGrid);
            this.tabBoard.Controls.Add(this.notificationCountLabel);
            this.tabBoard.Controls.Add(this.notificationsList);
            this.tabBoard.Controls.Add(this.publishButton);
            this.tabBoard.Controls.Add(this.escalateButton);
            this.tabBoard.Controls.Add(this.newTicketsButton);
            this.tabBoard.Controls.Add(this.subscribeButton);
            this.tabBoard.Controls.Add(this.unsubscribeButton);
            this.tabBoard.Name = "tabBoard";
            this.tabBoard.Text = "Ticket board";
            //
            // labelTenantCaption
            //
            this.labelTenantCaption.AutoSize = false;
            this.labelTenantCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelTenantCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTenantCaption.Location = new System.Drawing.Point(18, 16);
            this.labelTenantCaption.Name = "labelTenantCaption";
            this.labelTenantCaption.Size = new System.Drawing.Size(60, 28);
            this.labelTenantCaption.Text = "Tenant";
            this.labelTenantCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tenantComboBox
            //
            this.tenantComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.tenantComboBox.Location = new System.Drawing.Point(80, 14);
            this.tenantComboBox.Name = "tenantComboBox";
            this.tenantComboBox.Size = new System.Drawing.Size(150, 30);
            //
            // escalatedOnlyCheckBox
            //
            this.escalatedOnlyCheckBox.AutoSize = false;
            this.escalatedOnlyCheckBox.Location = new System.Drawing.Point(250, 18);
            this.escalatedOnlyCheckBox.Name = "escalatedOnlyCheckBox";
            this.escalatedOnlyCheckBox.Size = new System.Drawing.Size(160, 24);
            this.escalatedOnlyCheckBox.Text = "Escalated only";
            //
            // ticketsGrid
            //
            this.ticketsGrid.Location = new System.Drawing.Point(18, 54);
            this.ticketsGrid.Name = "ticketsGrid";
            this.ticketsGrid.Size = new System.Drawing.Size(620, 300);
            //
            // notificationCountLabel
            //
            this.notificationCountLabel.AutoSize = false;
            this.notificationCountLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.notificationCountLabel.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.notificationCountLabel.Location = new System.Drawing.Point(656, 18);
            this.notificationCountLabel.Name = "notificationCountLabel";
            this.notificationCountLabel.Size = new System.Drawing.Size(308, 24);
            this.notificationCountLabel.Text = "0 notifications in this session";
            //
            // notificationsList
            //
            this.notificationsList.Font = new System.Drawing.Font("monospace", 9F);
            this.notificationsList.Location = new System.Drawing.Point(656, 54);
            this.notificationsList.Name = "notificationsList";
            this.notificationsList.Size = new System.Drawing.Size(308, 300);
            //
            // publishButton
            //
            this.publishButton.Location = new System.Drawing.Point(18, 368);
            this.publishButton.Name = "publishButton";
            this.publishButton.Size = new System.Drawing.Size(150, 36);
            this.publishButton.Text = "Publish Event";
            this.publishButton.Click += new System.EventHandler(this.publishButton_Click);
            //
            // escalateButton
            //
            this.escalateButton.Location = new System.Drawing.Point(176, 368);
            this.escalateButton.Name = "escalateButton";
            this.escalateButton.Size = new System.Drawing.Size(150, 36);
            this.escalateButton.Text = "Escalate selected";
            this.escalateButton.Click += new System.EventHandler(this.escalateButton_Click);
            //
            // newTicketsButton
            //
            this.newTicketsButton.Location = new System.Drawing.Point(334, 368);
            this.newTicketsButton.Name = "newTicketsButton";
            this.newTicketsButton.Size = new System.Drawing.Size(180, 36);
            this.newTicketsButton.Text = "New ticket events (10)";
            this.newTicketsButton.Click += new System.EventHandler(this.newTicketsButton_Click);
            //
            // subscribeButton
            //
            this.subscribeButton.Enabled = false;
            this.subscribeButton.Location = new System.Drawing.Point(656, 368);
            this.subscribeButton.Name = "subscribeButton";
            this.subscribeButton.Size = new System.Drawing.Size(150, 36);
            this.subscribeButton.Text = "Subscribe";
            this.subscribeButton.Click += new System.EventHandler(this.subscribeButton_Click);
            //
            // unsubscribeButton
            //
            this.unsubscribeButton.Location = new System.Drawing.Point(814, 368);
            this.unsubscribeButton.Name = "unsubscribeButton";
            this.unsubscribeButton.Size = new System.Drawing.Size(150, 36);
            this.unsubscribeButton.Text = "Unsubscribe";
            this.unsubscribeButton.Click += new System.EventHandler(this.unsubscribeButton_Click);
            //
            // tabCadence
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
            this.tabCadence.Controls.Add(this.labelLastAppliedCaption);
            this.tabCadence.Controls.Add(this.lastAppliedLabel);
            this.tabCadence.Name = "tabCadence";
            this.tabCadence.Text = "Cadence";
            //
            // liveModeCheckBox
            //
            this.liveModeCheckBox.AutoSize = false;
            this.liveModeCheckBox.Location = new System.Drawing.Point(18, 18);
            this.liveModeCheckBox.Name = "liveModeCheckBox";
            this.liveModeCheckBox.Size = new System.Drawing.Size(160, 26);
            this.liveModeCheckBox.Text = "Live mode";
            //
            // labelCadenceCaption
            //
            this.labelCadenceCaption.AutoSize = false;
            this.labelCadenceCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelCadenceCaption.Location = new System.Drawing.Point(200, 16);
            this.labelCadenceCaption.Name = "labelCadenceCaption";
            this.labelCadenceCaption.Size = new System.Drawing.Size(80, 30);
            this.labelCadenceCaption.Text = "Cadence";
            this.labelCadenceCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cadenceComboBox
            //
            this.cadenceComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cadenceComboBox.Location = new System.Drawing.Point(284, 14);
            this.cadenceComboBox.Name = "cadenceComboBox";
            this.cadenceComboBox.Size = new System.Drawing.Size(130, 32);
            //
            // labelOpenCaption
            //
            this.labelOpenCaption.AutoSize = false;
            this.labelOpenCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelOpenCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelOpenCaption.Location = new System.Drawing.Point(18, 70);
            this.labelOpenCaption.Name = "labelOpenCaption";
            this.labelOpenCaption.Size = new System.Drawing.Size(200, 18);
            this.labelOpenCaption.Text = "OPEN TICKETS";
            //
            // openTicketsLabel
            //
            this.openTicketsLabel.AutoSize = false;
            this.openTicketsLabel.Font = new System.Drawing.Font("default", 20F, System.Drawing.FontStyle.Bold);
            this.openTicketsLabel.Location = new System.Drawing.Point(18, 90);
            this.openTicketsLabel.Name = "openTicketsLabel";
            this.openTicketsLabel.Size = new System.Drawing.Size(200, 36);
            this.openTicketsLabel.Text = "—";
            //
            // labelQueueCaption
            //
            this.labelQueueCaption.AutoSize = false;
            this.labelQueueCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelQueueCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelQueueCaption.Location = new System.Drawing.Point(240, 70);
            this.labelQueueCaption.Name = "labelQueueCaption";
            this.labelQueueCaption.Size = new System.Drawing.Size(200, 18);
            this.labelQueueCaption.Text = "QUEUE DEPTH";
            //
            // queueDepthLabel
            //
            this.queueDepthLabel.AutoSize = false;
            this.queueDepthLabel.Font = new System.Drawing.Font("default", 20F, System.Drawing.FontStyle.Bold);
            this.queueDepthLabel.Location = new System.Drawing.Point(240, 90);
            this.queueDepthLabel.Name = "queueDepthLabel";
            this.queueDepthLabel.Size = new System.Drawing.Size(200, 36);
            this.queueDepthLabel.Text = "—";
            //
            // labelWaitCaption
            //
            this.labelWaitCaption.AutoSize = false;
            this.labelWaitCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelWaitCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelWaitCaption.Location = new System.Drawing.Point(462, 70);
            this.labelWaitCaption.Name = "labelWaitCaption";
            this.labelWaitCaption.Size = new System.Drawing.Size(200, 18);
            this.labelWaitCaption.Text = "AVG WAIT";
            //
            // avgWaitLabel
            //
            this.avgWaitLabel.AutoSize = false;
            this.avgWaitLabel.Font = new System.Drawing.Font("default", 20F, System.Drawing.FontStyle.Bold);
            this.avgWaitLabel.Location = new System.Drawing.Point(462, 90);
            this.avgWaitLabel.Name = "avgWaitLabel";
            this.avgWaitLabel.Size = new System.Drawing.Size(200, 36);
            this.avgWaitLabel.Text = "—";
            //
            // labelEventsCaption
            //
            this.labelEventsCaption.AutoSize = false;
            this.labelEventsCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelEventsCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelEventsCaption.Location = new System.Drawing.Point(18, 150);
            this.labelEventsCaption.Name = "labelEventsCaption";
            this.labelEventsCaption.Size = new System.Drawing.Size(200, 18);
            this.labelEventsCaption.Text = "EVENTS RECEIVED";
            //
            // eventsReceivedLabel
            //
            this.eventsReceivedLabel.AutoSize = false;
            this.eventsReceivedLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.eventsReceivedLabel.Location = new System.Drawing.Point(18, 170);
            this.eventsReceivedLabel.Name = "eventsReceivedLabel";
            this.eventsReceivedLabel.Size = new System.Drawing.Size(200, 30);
            this.eventsReceivedLabel.Text = "0";
            //
            // labelUpdatesCaption
            //
            this.labelUpdatesCaption.AutoSize = false;
            this.labelUpdatesCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelUpdatesCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelUpdatesCaption.Location = new System.Drawing.Point(240, 150);
            this.labelUpdatesCaption.Name = "labelUpdatesCaption";
            this.labelUpdatesCaption.Size = new System.Drawing.Size(200, 18);
            this.labelUpdatesCaption.Text = "UPDATES APPLIED";
            //
            // updatesAppliedLabel
            //
            this.updatesAppliedLabel.AutoSize = false;
            this.updatesAppliedLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.updatesAppliedLabel.Location = new System.Drawing.Point(240, 170);
            this.updatesAppliedLabel.Name = "updatesAppliedLabel";
            this.updatesAppliedLabel.Size = new System.Drawing.Size(200, 30);
            this.updatesAppliedLabel.Text = "0";
            //
            // labelLastAppliedCaption
            //
            this.labelLastAppliedCaption.AutoSize = false;
            this.labelLastAppliedCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelLastAppliedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLastAppliedCaption.Location = new System.Drawing.Point(462, 150);
            this.labelLastAppliedCaption.Name = "labelLastAppliedCaption";
            this.labelLastAppliedCaption.Size = new System.Drawing.Size(200, 18);
            this.labelLastAppliedCaption.Text = "LAST APPLIED";
            //
            // lastAppliedLabel
            //
            this.lastAppliedLabel.AutoSize = false;
            this.lastAppliedLabel.Font = new System.Drawing.Font("monospace", 11F);
            this.lastAppliedLabel.Location = new System.Drawing.Point(462, 172);
            this.lastAppliedLabel.Name = "lastAppliedLabel";
            this.lastAppliedLabel.Size = new System.Drawing.Size(200, 26);
            this.lastAppliedLabel.Text = "—";
            //
            // tabSession
            //
            this.tabSession.BackColor = System.Drawing.Color.White;
            this.tabSession.Controls.Add(this.diagnosticsGroupBox);
            this.tabSession.Controls.Add(this.incrementButton);
            this.tabSession.Controls.Add(this.backgroundButton);
            this.tabSession.Controls.Add(this.openSessionButton);
            this.tabSession.Controls.Add(this.liveSessionsLabel);
            this.tabSession.Controls.Add(this.lifecycleListBox);
            this.tabSession.Name = "tabSession";
            this.tabSession.Text = "Session";
            //
            // diagnosticsGroupBox
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
            this.diagnosticsGroupBox.Size = new System.Drawing.Size(460, 200);
            this.diagnosticsGroupBox.Text = "Session Inspector";
            //
            // labelTimeCaption
            //
            this.labelTimeCaption.AutoSize = false;
            this.labelTimeCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelTimeCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTimeCaption.Location = new System.Drawing.Point(16, 30);
            this.labelTimeCaption.Name = "labelTimeCaption";
            this.labelTimeCaption.Size = new System.Drawing.Size(110, 22);
            this.labelTimeCaption.Text = "Time";
            //
            // timeLabel
            //
            this.timeLabel.AutoSize = false;
            this.timeLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.timeLabel.Location = new System.Drawing.Point(130, 30);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(314, 22);
            this.timeLabel.Text = "—";
            //
            // labelClientCaption
            //
            this.labelClientCaption.AutoSize = false;
            this.labelClientCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelClientCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelClientCaption.Location = new System.Drawing.Point(16, 56);
            this.labelClientCaption.Name = "labelClientCaption";
            this.labelClientCaption.Size = new System.Drawing.Size(110, 22);
            this.labelClientCaption.Text = "Client id";
            //
            // clientIdLabel
            //
            this.clientIdLabel.AutoSize = false;
            this.clientIdLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.clientIdLabel.Location = new System.Drawing.Point(130, 56);
            this.clientIdLabel.Name = "clientIdLabel";
            this.clientIdLabel.Size = new System.Drawing.Size(314, 22);
            this.clientIdLabel.Text = "—";
            //
            // labelSessionCaption
            //
            this.labelSessionCaption.AutoSize = false;
            this.labelSessionCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelSessionCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelSessionCaption.Location = new System.Drawing.Point(16, 82);
            this.labelSessionCaption.Name = "labelSessionCaption";
            this.labelSessionCaption.Size = new System.Drawing.Size(110, 22);
            this.labelSessionCaption.Text = "Session id";
            //
            // sessionIdLabel
            //
            this.sessionIdLabel.AutoSize = false;
            this.sessionIdLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.sessionIdLabel.Location = new System.Drawing.Point(130, 82);
            this.sessionIdLabel.Name = "sessionIdLabel";
            this.sessionIdLabel.Size = new System.Drawing.Size(314, 22);
            this.sessionIdLabel.Text = "—";
            //
            // labelBrowserCaption
            //
            this.labelBrowserCaption.AutoSize = false;
            this.labelBrowserCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBrowserCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelBrowserCaption.Location = new System.Drawing.Point(16, 108);
            this.labelBrowserCaption.Name = "labelBrowserCaption";
            this.labelBrowserCaption.Size = new System.Drawing.Size(110, 22);
            this.labelBrowserCaption.Text = "Browser";
            //
            // browserLabel
            //
            this.browserLabel.AutoSize = false;
            this.browserLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.browserLabel.Location = new System.Drawing.Point(130, 108);
            this.browserLabel.Name = "browserLabel";
            this.browserLabel.Size = new System.Drawing.Size(314, 22);
            this.browserLabel.Text = "—";
            //
            // labelThreadCaption
            //
            this.labelThreadCaption.AutoSize = false;
            this.labelThreadCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelThreadCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelThreadCaption.Location = new System.Drawing.Point(16, 134);
            this.labelThreadCaption.Name = "labelThreadCaption";
            this.labelThreadCaption.Size = new System.Drawing.Size(110, 22);
            this.labelThreadCaption.Text = "Server thread";
            //
            // threadLabel
            //
            this.threadLabel.AutoSize = false;
            this.threadLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.threadLabel.Location = new System.Drawing.Point(130, 134);
            this.threadLabel.Name = "threadLabel";
            this.threadLabel.Size = new System.Drawing.Size(314, 22);
            this.threadLabel.Text = "—";
            //
            // labelCounterCaption
            //
            this.labelCounterCaption.AutoSize = false;
            this.labelCounterCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelCounterCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelCounterCaption.Location = new System.Drawing.Point(16, 160);
            this.labelCounterCaption.Name = "labelCounterCaption";
            this.labelCounterCaption.Size = new System.Drawing.Size(110, 22);
            this.labelCounterCaption.Text = "Counter";
            //
            // counterLabel
            //
            this.counterLabel.AutoSize = false;
            this.counterLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.counterLabel.Location = new System.Drawing.Point(130, 160);
            this.counterLabel.Name = "counterLabel";
            this.counterLabel.Size = new System.Drawing.Size(314, 22);
            this.counterLabel.Text = "0";
            //
            // incrementButton
            //
            this.incrementButton.Location = new System.Drawing.Point(18, 228);
            this.incrementButton.Name = "incrementButton";
            this.incrementButton.Size = new System.Drawing.Size(130, 36);
            this.incrementButton.Text = "Counter +1";
            this.incrementButton.Click += new System.EventHandler(this.incrementButton_Click);
            //
            // backgroundButton
            //
            this.backgroundButton.Location = new System.Drawing.Point(156, 228);
            this.backgroundButton.Name = "backgroundButton";
            this.backgroundButton.Size = new System.Drawing.Size(190, 36);
            this.backgroundButton.Text = "Background update";
            this.backgroundButton.Click += new System.EventHandler(this.backgroundButton_Click);
            //
            // openSessionButton
            //
            this.openSessionButton.Location = new System.Drawing.Point(18, 272);
            this.openSessionButton.Name = "openSessionButton";
            this.openSessionButton.Size = new System.Drawing.Size(190, 36);
            this.openSessionButton.Text = "Open second window";
            this.openSessionButton.Click += new System.EventHandler(this.openSessionButton_Click);
            //
            // liveSessionsLabel
            //
            this.liveSessionsLabel.AutoSize = false;
            this.liveSessionsLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.liveSessionsLabel.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.liveSessionsLabel.Location = new System.Drawing.Point(500, 18);
            this.liveSessionsLabel.Name = "liveSessionsLabel";
            this.liveSessionsLabel.Size = new System.Drawing.Size(464, 20);
            this.liveSessionsLabel.Text = "Live sessions: —";
            //
            // lifecycleListBox
            //
            this.lifecycleListBox.Font = new System.Drawing.Font("monospace", 9F);
            this.lifecycleListBox.Location = new System.Drawing.Point(500, 44);
            this.lifecycleListBox.Name = "lifecycleListBox";
            this.lifecycleListBox.Size = new System.Drawing.Size(464, 374);
            //
            // tabHealth
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
            // labelHealthTitle
            //
            this.labelHealthTitle.AutoSize = false;
            this.labelHealthTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelHealthTitle.Location = new System.Drawing.Point(18, 12);
            this.labelHealthTitle.Name = "labelHealthTitle";
            this.labelHealthTitle.Size = new System.Drawing.Size(400, 26);
            this.labelHealthTitle.Text = "Real-Time Health";
            //
            // websocketModeLabel
            //
            this.websocketModeLabel.AutoSize = false;
            this.websocketModeLabel.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.websocketModeLabel.Location = new System.Drawing.Point(18, 44);
            this.websocketModeLabel.Name = "websocketModeLabel";
            this.websocketModeLabel.Size = new System.Drawing.Size(320, 26);
            this.websocketModeLabel.Text = "—";
            //
            // pollingLabel
            //
            this.pollingLabel.AutoSize = false;
            this.pollingLabel.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.pollingLabel.Location = new System.Drawing.Point(344, 44);
            this.pollingLabel.Name = "pollingLabel";
            this.pollingLabel.Size = new System.Drawing.Size(320, 26);
            this.pollingLabel.Text = "—";
            //
            // subscriptionCountLabel
            //
            this.subscriptionCountLabel.AutoSize = false;
            this.subscriptionCountLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.subscriptionCountLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.subscriptionCountLabel.Location = new System.Drawing.Point(18, 74);
            this.subscriptionCountLabel.Name = "subscriptionCountLabel";
            this.subscriptionCountLabel.Size = new System.Drawing.Size(320, 22);
            this.subscriptionCountLabel.Text = "—";
            //
            // updateRateLabel
            //
            this.updateRateLabel.AutoSize = false;
            this.updateRateLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.updateRateLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.updateRateLabel.Location = new System.Drawing.Point(344, 74);
            this.updateRateLabel.Name = "updateRateLabel";
            this.updateRateLabel.Size = new System.Drawing.Size(620, 22);
            this.updateRateLabel.Text = "—";
            //
            // labelConfigTitle
            //
            this.labelConfigTitle.AutoSize = false;
            this.labelConfigTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelConfigTitle.Location = new System.Drawing.Point(18, 108);
            this.labelConfigTitle.Name = "labelConfigTitle";
            this.labelConfigTitle.Size = new System.Drawing.Size(470, 24);
            this.labelConfigTitle.Text = "Configuration";
            //
            // configListBox
            //
            this.configListBox.Font = new System.Drawing.Font("monospace", 9F);
            this.configListBox.Location = new System.Drawing.Point(18, 134);
            this.configListBox.Name = "configListBox";
            this.configListBox.Size = new System.Drawing.Size(470, 284);
            //
            // labelChecklistTitle
            //
            this.labelChecklistTitle.AutoSize = false;
            this.labelChecklistTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelChecklistTitle.Location = new System.Drawing.Point(504, 108);
            this.labelChecklistTitle.Name = "labelChecklistTitle";
            this.labelChecklistTitle.Size = new System.Drawing.Size(460, 24);
            this.labelChecklistTitle.Text = "Production checklist";
            //
            // checklistBox
            //
            this.checklistBox.CheckOnClick = true;
            this.checklistBox.Location = new System.Drawing.Point(504, 134);
            this.checklistBox.Name = "checklistBox";
            this.checklistBox.Size = new System.Drawing.Size(460, 284);
            //
            // refreshTimer
            //
            this.refreshTimer.Interval = 1000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            //
            // healthTimer
            //
            this.healthTimer.Interval = 2000;
            this.healthTimer.Tick += new System.EventHandler(this.healthTimer_Tick);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.featureTabs);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1040, 600);
            this.Text = "TicketOps Live";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.statusPanel.ResumeLayout(false);
            this.tabImport.ResumeLayout(false);
            this.tabBoard.ResumeLayout(false);
            this.tabCadence.ResumeLayout(false);
            this.tabSession.ResumeLayout(false);
            this.tabHealth.ResumeLayout(false);
            this.diagnosticsGroupBox.ResumeLayout(false);
            this.featureTabs.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel statusPanel;
        private Wisej.Web.Label clockLabel;
        private Wisej.Web.Label connectionLabel;
        private Wisej.Web.Label activityLabel;
        private Wisej.Web.Label labelLoadCaption;
        private Wisej.Web.ProgressBar serverLoadBar;
        private Wisej.Web.Label loadValueLabel;
        private Wisej.Web.Button startButton;
        private Wisej.Web.Button stopButton;
        private Wisej.Web.TabControl featureTabs;
        private Wisej.Web.TabPage tabImport;
        private Wisej.Web.TabPage tabBoard;
        private Wisej.Web.TabPage tabCadence;
        private Wisej.Web.TabPage tabSession;
        private Wisej.Web.TabPage tabHealth;
        private Wisej.Web.Label importStatusLabel;
        private Wisej.Web.Label elapsedLabel;
        private Wisej.Web.ProgressBar importProgressBar;
        private Wisej.Web.Label recordsImportedLabel;
        private Wisej.Web.ListBox importLogListBox;
        private Wisej.Web.Button startImportButton;
        private Wisej.Web.Button cancelImportButton;
        private Wisej.Web.Button failAt87Button;
        private Wisej.Web.Label labelTenantCaption;
        private Wisej.Web.ComboBox tenantComboBox;
        private Wisej.Web.CheckBox escalatedOnlyCheckBox;
        private Wisej.Web.DataGridView ticketsGrid;
        private Wisej.Web.Label notificationCountLabel;
        private Wisej.Web.ListBox notificationsList;
        private Wisej.Web.Button publishButton;
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
        private Wisej.Web.Label labelLastAppliedCaption;
        private Wisej.Web.Label lastAppliedLabel;
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
        private Wisej.Web.Button incrementButton;
        private Wisej.Web.Button backgroundButton;
        private Wisej.Web.Button openSessionButton;
        private Wisej.Web.Label liveSessionsLabel;
        private Wisej.Web.ListBox lifecycleListBox;
        private Wisej.Web.Label labelHealthTitle;
        private Wisej.Web.Label websocketModeLabel;
        private Wisej.Web.Label pollingLabel;
        private Wisej.Web.Label subscriptionCountLabel;
        private Wisej.Web.Label updateRateLabel;
        private Wisej.Web.Label labelConfigTitle;
        private Wisej.Web.ListBox configListBox;
        private Wisej.Web.Label labelChecklistTitle;
        private Wisej.Web.CheckedListBox checklistBox;
        private Wisej.Web.Timer refreshTimer;
        private Wisej.Web.Timer healthTimer;
    }
}
