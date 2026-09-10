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
            this.labelOpenCaption = new Wisej.Web.Label();
            this.openTicketsLabel = new Wisej.Web.Label();
            this.labelQueueCaption = new Wisej.Web.Label();
            this.queueDepthLabel = new Wisej.Web.Label();
            this.labelWaitCaption = new Wisej.Web.Label();
            this.avgWaitLabel = new Wisej.Web.Label();
            this.connectionLabel = new Wisej.Web.Label();
            this.modelLabel = new Wisej.Web.Label();
            this.panelCadence = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.liveModeCheckBox = new Wisej.Web.CheckBox();
            this.labelCadenceCaption = new Wisej.Web.Label();
            this.cadenceComboBox = new Wisej.Web.ComboBox();
            this.slowTickCheckBox = new Wisej.Web.CheckBox();
            this.labelInstrumentCaption = new Wisej.Web.Label();
            this.labelEventsCaption = new Wisej.Web.Label();
            this.eventsReceivedLabel = new Wisej.Web.Label();
            this.labelUpdatesCaption = new Wisej.Web.Label();
            this.updatesAppliedLabel = new Wisej.Web.Label();
            this.labelSkippedCaption = new Wisej.Web.Label();
            this.skippedTicksLabel = new Wisej.Web.Label();
            this.labelLastCaption = new Wisej.Web.Label();
            this.lastAppliedLabel = new Wisej.Web.Label();
            this.labelExplain = new Wisej.Web.Label();
            this.labelFourCadences = new Wisej.Web.Label();
            this.labelBanner = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.burstButton = new Wisej.Web.Button();
            this.duplicateTimerButton = new Wisej.Web.Button();
            this.clearButton = new Wisej.Web.Button();
            this.refreshTimer = new Wisej.Web.Timer(this.components);
            this.statusPanel.SuspendLayout();
            this.panelCadence.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // statusPanel  (the simulated dashboard: what the refresh timer renders, once per UI tick)
            //
            this.statusPanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusPanel.BackColor = System.Drawing.Color.White;
            this.statusPanel.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.statusPanel.Controls.Add(this.labelStripCaption);
            this.statusPanel.Controls.Add(this.labelOpenCaption);
            this.statusPanel.Controls.Add(this.openTicketsLabel);
            this.statusPanel.Controls.Add(this.labelQueueCaption);
            this.statusPanel.Controls.Add(this.queueDepthLabel);
            this.statusPanel.Controls.Add(this.labelWaitCaption);
            this.statusPanel.Controls.Add(this.avgWaitLabel);
            this.statusPanel.Controls.Add(this.connectionLabel);
            this.statusPanel.Controls.Add(this.modelLabel);
            this.statusPanel.Location = new System.Drawing.Point(30, 18);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(1288, 92);
            //
            // labelStripCaption
            //
            this.labelStripCaption.AutoSize = false;
            this.labelStripCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelStripCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelStripCaption.Location = new System.Drawing.Point(16, 8);
            this.labelStripCaption.Name = "labelStripCaption";
            this.labelStripCaption.Size = new System.Drawing.Size(420, 18);
            this.labelStripCaption.Text = "TICKETOPS LIVE · SIMULATED OPERATIONS DASHBOARD";
            //
            // labelOpenCaption / openTicketsLabel
            //
            this.labelOpenCaption.AutoSize = false;
            this.labelOpenCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelOpenCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelOpenCaption.Location = new System.Drawing.Point(16, 30);
            this.labelOpenCaption.Name = "labelOpenCaption";
            this.labelOpenCaption.Size = new System.Drawing.Size(200, 18);
            this.labelOpenCaption.Text = "OPEN TICKETS";
            this.openTicketsLabel.AutoSize = false;
            this.openTicketsLabel.Font = new System.Drawing.Font("monospace", 20F, System.Drawing.FontStyle.Bold);
            this.openTicketsLabel.Location = new System.Drawing.Point(16, 48);
            this.openTicketsLabel.Name = "openTicketsLabel";
            this.openTicketsLabel.Size = new System.Drawing.Size(200, 36);
            this.openTicketsLabel.Text = "—";
            this.openTicketsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelQueueCaption / queueDepthLabel
            //
            this.labelQueueCaption.AutoSize = false;
            this.labelQueueCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelQueueCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelQueueCaption.Location = new System.Drawing.Point(236, 30);
            this.labelQueueCaption.Name = "labelQueueCaption";
            this.labelQueueCaption.Size = new System.Drawing.Size(200, 18);
            this.labelQueueCaption.Text = "QUEUE DEPTH";
            this.queueDepthLabel.AutoSize = false;
            this.queueDepthLabel.Font = new System.Drawing.Font("monospace", 20F, System.Drawing.FontStyle.Bold);
            this.queueDepthLabel.Location = new System.Drawing.Point(236, 48);
            this.queueDepthLabel.Name = "queueDepthLabel";
            this.queueDepthLabel.Size = new System.Drawing.Size(200, 36);
            this.queueDepthLabel.Text = "—";
            this.queueDepthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelWaitCaption / avgWaitLabel
            //
            this.labelWaitCaption.AutoSize = false;
            this.labelWaitCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelWaitCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelWaitCaption.Location = new System.Drawing.Point(456, 30);
            this.labelWaitCaption.Name = "labelWaitCaption";
            this.labelWaitCaption.Size = new System.Drawing.Size(200, 18);
            this.labelWaitCaption.Text = "AVG WAIT";
            this.avgWaitLabel.AutoSize = false;
            this.avgWaitLabel.Font = new System.Drawing.Font("monospace", 20F, System.Drawing.FontStyle.Bold);
            this.avgWaitLabel.Location = new System.Drawing.Point(456, 48);
            this.avgWaitLabel.Name = "avgWaitLabel";
            this.avgWaitLabel.Size = new System.Drawing.Size(200, 36);
            this.avgWaitLabel.Text = "—";
            this.avgWaitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // connectionLabel  (WebSocket vs HTTP-only — decides whether the polling fallback is requested)
            //
            this.connectionLabel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.connectionLabel.AutoSize = false;
            this.connectionLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.connectionLabel.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.connectionLabel.Location = new System.Drawing.Point(700, 26);
            this.connectionLabel.Name = "connectionLabel";
            this.connectionLabel.Size = new System.Drawing.Size(570, 24);
            this.connectionLabel.Text = "○ connecting…";
            //
            // modelLabel  (model cadence vs UI cadence, in words)
            //
            this.modelLabel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.modelLabel.AutoSize = false;
            this.modelLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.modelLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.modelLabel.Location = new System.Drawing.Point(700, 52);
            this.modelLabel.Name = "modelLabel";
            this.modelLabel.Size = new System.Drawing.Size(570, 34);
            this.modelLabel.Text = "model stopped · turn Live mode on in the Update cadence card";
            this.modelLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelCadence  (the Update Cadence panel of the walkthrough)
            //
            this.panelCadence.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelCadence.BackColor = System.Drawing.Color.White;
            this.panelCadence.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelCadence.Controls.Add(this.labelTitle);
            this.panelCadence.Controls.Add(this.labelStatus);
            this.panelCadence.Controls.Add(this.liveModeCheckBox);
            this.panelCadence.Controls.Add(this.labelCadenceCaption);
            this.panelCadence.Controls.Add(this.cadenceComboBox);
            this.panelCadence.Controls.Add(this.slowTickCheckBox);
            this.panelCadence.Controls.Add(this.labelInstrumentCaption);
            this.panelCadence.Controls.Add(this.labelEventsCaption);
            this.panelCadence.Controls.Add(this.eventsReceivedLabel);
            this.panelCadence.Controls.Add(this.labelUpdatesCaption);
            this.panelCadence.Controls.Add(this.updatesAppliedLabel);
            this.panelCadence.Controls.Add(this.labelSkippedCaption);
            this.panelCadence.Controls.Add(this.skippedTicksLabel);
            this.panelCadence.Controls.Add(this.labelLastCaption);
            this.panelCadence.Controls.Add(this.lastAppliedLabel);
            this.panelCadence.Controls.Add(this.labelExplain);
            this.panelCadence.Controls.Add(this.labelFourCadences);
            this.panelCadence.Controls.Add(this.labelBanner);
            this.panelCadence.Controls.Add(this.labelState);
            this.panelCadence.Location = new System.Drawing.Point(30, 126);
            this.panelCadence.Name = "panelCadence";
            this.panelCadence.Size = new System.Drawing.Size(700, 472);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 16);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(360, 30);
            this.labelTitle.Text = "Update cadence";
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
            // liveModeCheckBox  (lab: start/stop polling and the refresh timer with live mode)
            //
            this.liveModeCheckBox.AutoSize = false;
            this.liveModeCheckBox.Font = new System.Drawing.Font("default", 10F);
            this.liveModeCheckBox.Location = new System.Drawing.Point(24, 56);
            this.liveModeCheckBox.Name = "liveModeCheckBox";
            this.liveModeCheckBox.Size = new System.Drawing.Size(410, 26);
            this.liveModeCheckBox.Text = "Live mode — model simulator + refreshTimer + polling fallback";
            this.liveModeCheckBox.ToolTipText = "ON: the simulator changes the model every 50 ms, the refreshTimer applies one coalesced update per tick, and StartPolling(1000) is requested only if there is no WebSocket. OFF: everything stops.";
            this.liveModeCheckBox.CheckedChanged += new System.EventHandler(this.liveModeCheckBox_CheckedChanged);
            //
            // labelCadenceCaption / cadenceComboBox  (the UI cadence selector)
            //
            this.labelCadenceCaption.AutoSize = false;
            this.labelCadenceCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelCadenceCaption.Location = new System.Drawing.Point(24, 88);
            this.labelCadenceCaption.Name = "labelCadenceCaption";
            this.labelCadenceCaption.Size = new System.Drawing.Size(90, 32);
            this.labelCadenceCaption.Text = "UI cadence";
            this.labelCadenceCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cadenceComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cadenceComboBox.Items.AddRange(new object[] {
            "250 ms",
            "1 sec",
            "5 sec"});
            this.cadenceComboBox.Location = new System.Drawing.Point(118, 86);
            this.cadenceComboBox.Name = "cadenceComboBox";
            this.cadenceComboBox.Size = new System.Drawing.Size(130, 32);
            this.cadenceComboBox.ToolTipText = "Reprograms refreshTimer.Interval live (floored at 250 ms). The model keeps changing every 50 ms whatever you pick.";
            this.cadenceComboBox.SelectedIndex = 1;
            this.cadenceComboBox.SelectedIndexChanged += new System.EventHandler(this.cadenceComboBox_SelectedIndexChanged);
            //
            // slowTickCheckBox  (the overlap guard demo: a refresh that takes longer than the interval)
            //
            this.slowTickCheckBox.AutoSize = false;
            this.slowTickCheckBox.Font = new System.Drawing.Font("default", 10F);
            this.slowTickCheckBox.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.slowTickCheckBox.Location = new System.Drawing.Point(268, 88);
            this.slowTickCheckBox.Name = "slowTickCheckBox";
            this.slowTickCheckBox.Size = new System.Drawing.Size(408, 26);
            this.slowTickCheckBox.Text = "Simulate slow tick (1.5 s) — long work inside the tick";
            this.slowTickCheckBox.ToolTipText = "Anti-pattern of the lesson: the tick sleeps 1.5 s while applying. _refreshInProgress refuses a tick that arrives during a refresh.";
            this.slowTickCheckBox.CheckedChanged += new System.EventHandler(this.slowTickCheckBox_CheckedChanged);
            //
            // labelInstrumentCaption
            //
            this.labelInstrumentCaption.AutoSize = false;
            this.labelInstrumentCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelInstrumentCaption.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.labelInstrumentCaption.Location = new System.Drawing.Point(24, 124);
            this.labelInstrumentCaption.Name = "labelInstrumentCaption";
            this.labelInstrumentCaption.Size = new System.Drawing.Size(652, 20);
            this.labelInstrumentCaption.Text = "instrumentation   model events in · UI updates out · ticks refused · last applied";
            //
            // labelEventsCaption / eventsReceivedLabel
            //
            this.labelEventsCaption.AutoSize = false;
            this.labelEventsCaption.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.labelEventsCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelEventsCaption.Location = new System.Drawing.Point(24, 148);
            this.labelEventsCaption.Name = "labelEventsCaption";
            this.labelEventsCaption.Size = new System.Drawing.Size(160, 18);
            this.labelEventsCaption.Text = "EVENTS RECEIVED";
            this.eventsReceivedLabel.AutoSize = false;
            this.eventsReceivedLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.eventsReceivedLabel.Location = new System.Drawing.Point(24, 166);
            this.eventsReceivedLabel.Name = "eventsReceivedLabel";
            this.eventsReceivedLabel.Size = new System.Drawing.Size(160, 34);
            this.eventsReceivedLabel.Text = "0";
            //
            // labelUpdatesCaption / updatesAppliedLabel
            //
            this.labelUpdatesCaption.AutoSize = false;
            this.labelUpdatesCaption.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.labelUpdatesCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelUpdatesCaption.Location = new System.Drawing.Point(188, 148);
            this.labelUpdatesCaption.Name = "labelUpdatesCaption";
            this.labelUpdatesCaption.Size = new System.Drawing.Size(160, 18);
            this.labelUpdatesCaption.Text = "UPDATES APPLIED";
            this.updatesAppliedLabel.AutoSize = false;
            this.updatesAppliedLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.updatesAppliedLabel.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.updatesAppliedLabel.Location = new System.Drawing.Point(188, 166);
            this.updatesAppliedLabel.Name = "updatesAppliedLabel";
            this.updatesAppliedLabel.Size = new System.Drawing.Size(160, 34);
            this.updatesAppliedLabel.Text = "0";
            //
            // labelSkippedCaption / skippedTicksLabel
            //
            this.labelSkippedCaption.AutoSize = false;
            this.labelSkippedCaption.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.labelSkippedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelSkippedCaption.Location = new System.Drawing.Point(352, 148);
            this.labelSkippedCaption.Name = "labelSkippedCaption";
            this.labelSkippedCaption.Size = new System.Drawing.Size(160, 18);
            this.labelSkippedCaption.Text = "TICKS REFUSED";
            this.skippedTicksLabel.AutoSize = false;
            this.skippedTicksLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.skippedTicksLabel.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.skippedTicksLabel.Location = new System.Drawing.Point(352, 166);
            this.skippedTicksLabel.Name = "skippedTicksLabel";
            this.skippedTicksLabel.Size = new System.Drawing.Size(160, 34);
            this.skippedTicksLabel.Text = "0";
            //
            // labelLastCaption / lastAppliedLabel
            //
            this.labelLastCaption.AutoSize = false;
            this.labelLastCaption.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.labelLastCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLastCaption.Location = new System.Drawing.Point(516, 148);
            this.labelLastCaption.Name = "labelLastCaption";
            this.labelLastCaption.Size = new System.Drawing.Size(160, 18);
            this.labelLastCaption.Text = "LAST APPLIED";
            this.lastAppliedLabel.AutoSize = false;
            this.lastAppliedLabel.Font = new System.Drawing.Font("monospace", 12F, System.Drawing.FontStyle.Bold);
            this.lastAppliedLabel.Location = new System.Drawing.Point(516, 166);
            this.lastAppliedLabel.Name = "lastAppliedLabel";
            this.lastAppliedLabel.Size = new System.Drawing.Size(160, 34);
            this.lastAppliedLabel.Text = "--:--:--.---";
            this.lastAppliedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelExplain
            //
            this.labelExplain.AutoSize = false;
            this.labelExplain.Font = new System.Drawing.Font("default", 10F);
            this.labelExplain.Location = new System.Drawing.Point(24, 208);
            this.labelExplain.Name = "labelExplain";
            this.labelExplain.Size = new System.Drawing.Size(652, 44);
            this.labelExplain.Text = "The simulator only sets a dirty flag; refreshTimer_Tick applies ONE snapshot per tick. Events grow ~20×/s, updates grow at the cadence you pick.";
            this.labelExplain.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelFourCadences
            //
            this.labelFourCadences.AutoSize = false;
            this.labelFourCadences.Font = new System.Drawing.Font("monospace", 9F);
            this.labelFourCadences.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelFourCadences.Location = new System.Drawing.Point(24, 256);
            this.labelFourCadences.Name = "labelFourCadences";
            this.labelFourCadences.Size = new System.Drawing.Size(652, 42);
            this.labelFourCadences.Text = "";
            this.labelFourCadences.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelBanner
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(24, 304);
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
            this.labelState.Location = new System.Drawing.Point(24, 368);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(652, 88);
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
            this.panelTrace.Location = new System.Drawing.Point(748, 126);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(570, 472);
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
            this.listTrace.Size = new System.Drawing.Size(530, 370);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 430);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(530, 26);
            this.labelTraceFooter.Text = "← request = the browser asked (a timer tick IS a request)   ·   → push = Application.Update from a task   ·   • server = decision";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.burstButton);
            this.panelActions.Controls.Add(this.duplicateTimerButton);
            this.panelActions.Controls.Add(this.clearButton);
            this.panelActions.Location = new System.Drawing.Point(30, 614);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // burstButton  (coalescing: 50 model events → one UI update)
            //
            this.burstButton.Location = new System.Drawing.Point(0, 4);
            this.burstButton.Name = "burstButton";
            this.burstButton.Size = new System.Drawing.Size(190, 36);
            this.burstButton.Text = "Burst 50 model events";
            this.burstButton.ToolTipText = "50 model changes at once. Events received grows by 50; the next refreshTimer_Tick applies ONE update.";
            this.burstButton.Click += new System.EventHandler(this.burstButton_Click);
            //
            // duplicateTimerButton  (anti-pattern: a second Start() on a running timer)
            //
            this.duplicateTimerButton.Location = new System.Drawing.Point(198, 4);
            this.duplicateTimerButton.Name = "duplicateTimerButton";
            this.duplicateTimerButton.Size = new System.Drawing.Size(240, 36);
            this.duplicateTimerButton.Text = "Start timer again (anti-pattern)";
            this.duplicateTimerButton.ToolTipText = "Refused by the ownership guard: refreshTimer is started once, by live mode, and stopped by live mode.";
            this.duplicateTimerButton.Click += new System.EventHandler(this.duplicateTimerButton_Click);
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
            // refreshTimer  (a Component with no visual surface: it lives in the Designer tray and is owned by this page)
            //
            this.refreshTimer.Interval = 1000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.panelCadence);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 700);
            this.Text = "TicketOps Live — Update Cadence";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.statusPanel.ResumeLayout(false);
            this.panelCadence.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel statusPanel;
        private Wisej.Web.Label labelStripCaption;
        private Wisej.Web.Label labelOpenCaption;
        private Wisej.Web.Label openTicketsLabel;
        private Wisej.Web.Label labelQueueCaption;
        private Wisej.Web.Label queueDepthLabel;
        private Wisej.Web.Label labelWaitCaption;
        private Wisej.Web.Label avgWaitLabel;
        private Wisej.Web.Label connectionLabel;
        private Wisej.Web.Label modelLabel;
        private Wisej.Web.Panel panelCadence;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.CheckBox liveModeCheckBox;
        private Wisej.Web.Label labelCadenceCaption;
        private Wisej.Web.ComboBox cadenceComboBox;
        private Wisej.Web.CheckBox slowTickCheckBox;
        private Wisej.Web.Label labelInstrumentCaption;
        private Wisej.Web.Label labelEventsCaption;
        private Wisej.Web.Label eventsReceivedLabel;
        private Wisej.Web.Label labelUpdatesCaption;
        private Wisej.Web.Label updatesAppliedLabel;
        private Wisej.Web.Label labelSkippedCaption;
        private Wisej.Web.Label skippedTicksLabel;
        private Wisej.Web.Label labelLastCaption;
        private Wisej.Web.Label lastAppliedLabel;
        private Wisej.Web.Label labelExplain;
        private Wisej.Web.Label labelFourCadences;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button burstButton;
        private Wisej.Web.Button duplicateTimerButton;
        private Wisej.Web.Button clearButton;
        private Wisej.Web.Timer refreshTimer;
    }
}
