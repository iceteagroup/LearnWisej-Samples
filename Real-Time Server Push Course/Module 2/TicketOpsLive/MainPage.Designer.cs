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
            this.labelWebSocketCaption = new Wisej.Web.Label();
            this.websocketLabel = new Wisej.Web.Label();
            this.labelCounterCaption = new Wisej.Web.Label();
            this.counterLabel = new Wisej.Web.Label();
            this.labelSharedCaption = new Wisej.Web.Label();
            this.sharedCounterLabel = new Wisej.Web.Label();
            this.panelLifecycle = new Wisej.Web.Panel();
            this.labelLifecycleTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.liveSessionsLabel = new Wisej.Web.Label();
            this.lifecycleListBox = new Wisej.Web.ListBox();
            this.labelBanner = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.incrementButton = new Wisej.Web.Button();
            this.incrementSharedButton = new Wisej.Web.Button();
            this.backgroundButton = new Wisej.Web.Button();
            this.faultButton = new Wisej.Web.Button();
            this.openSessionButton = new Wisej.Web.Button();
            this.exitButton = new Wisej.Web.Button();
            this.clearButton = new Wisej.Web.Button();
            this.diagnosticsGroupBox.SuspendLayout();
            this.panelLifecycle.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // diagnosticsGroupBox  (the lab: "Add a diagnostics group box to MainPage")
            //
            this.diagnosticsGroupBox.BackColor = System.Drawing.Color.White;
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
            this.diagnosticsGroupBox.Controls.Add(this.labelWebSocketCaption);
            this.diagnosticsGroupBox.Controls.Add(this.websocketLabel);
            this.diagnosticsGroupBox.Controls.Add(this.labelCounterCaption);
            this.diagnosticsGroupBox.Controls.Add(this.counterLabel);
            this.diagnosticsGroupBox.Controls.Add(this.labelSharedCaption);
            this.diagnosticsGroupBox.Controls.Add(this.sharedCounterLabel);
            this.diagnosticsGroupBox.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.diagnosticsGroupBox.Location = new System.Drawing.Point(30, 18);
            this.diagnosticsGroupBox.Name = "diagnosticsGroupBox";
            this.diagnosticsGroupBox.Size = new System.Drawing.Size(700, 280);
            this.diagnosticsGroupBox.Text = "Session inspector";
            //
            // labelTimeCaption / timeLabel
            //
            this.labelTimeCaption.AutoSize = false;
            this.labelTimeCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelTimeCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTimeCaption.Location = new System.Drawing.Point(20, 26);
            this.labelTimeCaption.Name = "labelTimeCaption";
            this.labelTimeCaption.Size = new System.Drawing.Size(176, 22);
            this.labelTimeCaption.Text = "current time";
            this.timeLabel.AutoSize = false;
            this.timeLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.timeLabel.Location = new System.Drawing.Point(204, 26);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(470, 22);
            this.timeLabel.Text = "--:--:--";
            //
            // labelClientCaption / clientIdLabel
            //
            this.labelClientCaption.AutoSize = false;
            this.labelClientCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelClientCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelClientCaption.Location = new System.Drawing.Point(20, 54);
            this.labelClientCaption.Name = "labelClientCaption";
            this.labelClientCaption.Size = new System.Drawing.Size(176, 22);
            this.labelClientCaption.Text = "Application.ClientId";
            this.clientIdLabel.AutoSize = false;
            this.clientIdLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.clientIdLabel.Location = new System.Drawing.Point(204, 54);
            this.clientIdLabel.Name = "clientIdLabel";
            this.clientIdLabel.Size = new System.Drawing.Size(470, 22);
            this.clientIdLabel.Text = "";
            //
            // labelSessionCaption / sessionIdLabel
            //
            this.labelSessionCaption.AutoSize = false;
            this.labelSessionCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelSessionCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelSessionCaption.Location = new System.Drawing.Point(20, 82);
            this.labelSessionCaption.Name = "labelSessionCaption";
            this.labelSessionCaption.Size = new System.Drawing.Size(176, 22);
            this.labelSessionCaption.Text = "Application.SessionId";
            this.sessionIdLabel.AutoSize = false;
            this.sessionIdLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.sessionIdLabel.Location = new System.Drawing.Point(204, 82);
            this.sessionIdLabel.Name = "sessionIdLabel";
            this.sessionIdLabel.Size = new System.Drawing.Size(470, 22);
            this.sessionIdLabel.Text = "";
            //
            // labelBrowserCaption / browserLabel
            //
            this.labelBrowserCaption.AutoSize = false;
            this.labelBrowserCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelBrowserCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelBrowserCaption.Location = new System.Drawing.Point(20, 110);
            this.labelBrowserCaption.Name = "labelBrowserCaption";
            this.labelBrowserCaption.Size = new System.Drawing.Size(176, 22);
            this.labelBrowserCaption.Text = "Application.Browser";
            this.browserLabel.AutoSize = false;
            this.browserLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.browserLabel.Location = new System.Drawing.Point(204, 110);
            this.browserLabel.Name = "browserLabel";
            this.browserLabel.Size = new System.Drawing.Size(470, 22);
            this.browserLabel.Text = "";
            //
            // labelThreadCaption / threadLabel  (the walkthrough: the thread shifts between events)
            //
            this.labelThreadCaption.AutoSize = false;
            this.labelThreadCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelThreadCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelThreadCaption.Location = new System.Drawing.Point(20, 138);
            this.labelThreadCaption.Name = "labelThreadCaption";
            this.labelThreadCaption.Size = new System.Drawing.Size(176, 22);
            this.labelThreadCaption.Text = "server thread";
            this.threadLabel.AutoSize = false;
            this.threadLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.threadLabel.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.threadLabel.Location = new System.Drawing.Point(204, 138);
            this.threadLabel.Name = "threadLabel";
            this.threadLabel.Size = new System.Drawing.Size(470, 22);
            this.threadLabel.Text = "";
            //
            // labelWebSocketCaption / websocketLabel
            //
            this.labelWebSocketCaption.AutoSize = false;
            this.labelWebSocketCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelWebSocketCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelWebSocketCaption.Location = new System.Drawing.Point(20, 166);
            this.labelWebSocketCaption.Name = "labelWebSocketCaption";
            this.labelWebSocketCaption.Size = new System.Drawing.Size(176, 22);
            this.labelWebSocketCaption.Text = "Application.IsWebSocket";
            this.websocketLabel.AutoSize = false;
            this.websocketLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.websocketLabel.Location = new System.Drawing.Point(204, 166);
            this.websocketLabel.Name = "websocketLabel";
            this.websocketLabel.Size = new System.Drawing.Size(470, 22);
            this.websocketLabel.Text = "";
            //
            // labelCounterCaption / counterLabel  (per-session state — the right way)
            //
            this.labelCounterCaption.AutoSize = false;
            this.labelCounterCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelCounterCaption.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelCounterCaption.Location = new System.Drawing.Point(20, 194);
            this.labelCounterCaption.Name = "labelCounterCaption";
            this.labelCounterCaption.Size = new System.Drawing.Size(176, 22);
            this.labelCounterCaption.Text = "Session.Counter ✓";
            this.counterLabel.AutoSize = false;
            this.counterLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.counterLabel.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.counterLabel.Location = new System.Drawing.Point(204, 194);
            this.counterLabel.Name = "counterLabel";
            this.counterLabel.Size = new System.Drawing.Size(470, 22);
            this.counterLabel.Text = "0";
            //
            // labelSharedCaption / sharedCounterLabel  (the static-field trap — deliberately wrong)
            //
            this.labelSharedCaption.AutoSize = false;
            this.labelSharedCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelSharedCaption.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelSharedCaption.Location = new System.Drawing.Point(20, 222);
            this.labelSharedCaption.Name = "labelSharedCaption";
            this.labelSharedCaption.Size = new System.Drawing.Size(176, 22);
            this.labelSharedCaption.Text = "static int ✖ (the trap)";
            this.sharedCounterLabel.AutoSize = false;
            this.sharedCounterLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.sharedCounterLabel.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.sharedCounterLabel.Location = new System.Drawing.Point(204, 222);
            this.sharedCounterLabel.Name = "sharedCounterLabel";
            this.sharedCounterLabel.Size = new System.Drawing.Size(470, 22);
            this.sharedCounterLabel.Text = "0";
            //
            // panelLifecycle  (the lifecycle logger card)
            //
            this.panelLifecycle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelLifecycle.BackColor = System.Drawing.Color.White;
            this.panelLifecycle.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLifecycle.Controls.Add(this.labelLifecycleTitle);
            this.panelLifecycle.Controls.Add(this.labelStatus);
            this.panelLifecycle.Controls.Add(this.liveSessionsLabel);
            this.panelLifecycle.Controls.Add(this.lifecycleListBox);
            this.panelLifecycle.Controls.Add(this.labelBanner);
            this.panelLifecycle.Controls.Add(this.labelState);
            this.panelLifecycle.Location = new System.Drawing.Point(30, 308);
            this.panelLifecycle.Name = "panelLifecycle";
            this.panelLifecycle.Size = new System.Drawing.Size(700, 290);
            //
            // labelLifecycleTitle
            //
            this.labelLifecycleTitle.AutoSize = false;
            this.labelLifecycleTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelLifecycleTitle.Location = new System.Drawing.Point(20, 12);
            this.labelLifecycleTitle.Name = "labelLifecycleTitle";
            this.labelLifecycleTitle.Size = new System.Drawing.Size(330, 30);
            this.labelLifecycleTitle.Text = "Lifecycle logger";
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(356, 14);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(320, 26);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // liveSessionsLabel  (fed by the global SessionRegistry)
            //
            this.liveSessionsLabel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.liveSessionsLabel.AutoSize = false;
            this.liveSessionsLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.liveSessionsLabel.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.liveSessionsLabel.Location = new System.Drawing.Point(20, 46);
            this.liveSessionsLabel.Name = "liveSessionsLabel";
            this.liveSessionsLabel.Size = new System.Drawing.Size(656, 22);
            this.liveSessionsLabel.Text = "live sessions: —";
            //
            // lifecycleListBox  (page loaded, every button, background arrival, exit, timeout, disposed)
            //
            this.lifecycleListBox.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lifecycleListBox.Font = new System.Drawing.Font("monospace", 9F);
            this.lifecycleListBox.Location = new System.Drawing.Point(20, 72);
            this.lifecycleListBox.Name = "lifecycleListBox";
            this.lifecycleListBox.Size = new System.Drawing.Size(656, 104);
            //
            // labelBanner
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(20, 184);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(656, 36);
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
            this.labelState.Location = new System.Drawing.Point(20, 226);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(656, 58);
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
            this.panelActions.Controls.Add(this.incrementButton);
            this.panelActions.Controls.Add(this.incrementSharedButton);
            this.panelActions.Controls.Add(this.backgroundButton);
            this.panelActions.Controls.Add(this.faultButton);
            this.panelActions.Controls.Add(this.openSessionButton);
            this.panelActions.Controls.Add(this.exitButton);
            this.panelActions.Controls.Add(this.clearButton);
            this.panelActions.Location = new System.Drawing.Point(30, 614);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // incrementButton  (success path — per-session state)
            //
            this.incrementButton.Location = new System.Drawing.Point(0, 4);
            this.incrementButton.Name = "incrementButton";
            this.incrementButton.Size = new System.Drawing.Size(170, 36);
            this.incrementButton.Text = "Session counter +1";
            this.incrementButton.ToolTipText = "Application.Session.Counter — one value per session; a second tab has its own.";
            this.incrementButton.Click += new System.EventHandler(this.incrementButton_Click);
            //
            // incrementSharedButton  (the static-field trap)
            //
            this.incrementSharedButton.Location = new System.Drawing.Point(178, 4);
            this.incrementSharedButton.Name = "incrementSharedButton";
            this.incrementSharedButton.Size = new System.Drawing.Size(200, 36);
            this.incrementSharedButton.Text = "Static counter +1 (trap)";
            this.incrementSharedButton.ToolTipText = "A static int is ONE field for the whole server: open a second tab and watch it leak across sessions.";
            this.incrementSharedButton.Click += new System.EventHandler(this.incrementSharedButton_Click);
            //
            // backgroundButton  (progress path — the lab's simulated background update)
            //
            this.backgroundButton.Location = new System.Drawing.Point(386, 4);
            this.backgroundButton.Name = "backgroundButton";
            this.backgroundButton.Size = new System.Drawing.Size(170, 36);
            this.backgroundButton.Text = "Background update";
            this.backgroundButton.ToolTipText = "Captures Application.Current, sleeps 1500 ms on a task, then Application.Update(context, …).";
            this.backgroundButton.Click += new System.EventHandler(this.backgroundButton_Click);
            //
            // faultButton  (failure path)
            //
            this.faultButton.Location = new System.Drawing.Point(564, 4);
            this.faultButton.Name = "faultButton";
            this.faultButton.Size = new System.Drawing.Size(160, 36);
            this.faultButton.Text = "Background fault";
            this.faultButton.ToolTipText = "The task throws after 500 ms: caught inside the task, detail to the server log, safe message in the UI.";
            this.faultButton.Click += new System.EventHandler(this.faultButton_Click);
            //
            // openSessionButton  (a second session, in a second tab)
            //
            this.openSessionButton.Location = new System.Drawing.Point(732, 4);
            this.openSessionButton.Name = "openSessionButton";
            this.openSessionButton.Size = new System.Drawing.Size(200, 36);
            this.openSessionButton.Text = "Open second session ↗";
            this.openSessionButton.ToolTipText = "Application.Navigate(\"/\", \"_blank\") — same user, second tab, DIFFERENT session.";
            this.openSessionButton.Click += new System.EventHandler(this.openSessionButton_Click);
            //
            // exitButton  (lifecycle — ApplicationExit)
            //
            this.exitButton.Location = new System.Drawing.Point(940, 4);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(160, 36);
            this.exitButton.Text = "End this session";
            this.exitButton.ToolTipText = "Application.Exit() — the ApplicationExit handler unsubscribes and unregisters this session.";
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
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
            this.Controls.Add(this.diagnosticsGroupBox);
            this.Controls.Add(this.panelLifecycle);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 700);
            this.Text = "TicketOps Live — Who Owns the UI";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.diagnosticsGroupBox.ResumeLayout(false);
            this.panelLifecycle.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

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
        private Wisej.Web.Label labelWebSocketCaption;
        private Wisej.Web.Label websocketLabel;
        private Wisej.Web.Label labelCounterCaption;
        private Wisej.Web.Label counterLabel;
        private Wisej.Web.Label labelSharedCaption;
        private Wisej.Web.Label sharedCounterLabel;
        private Wisej.Web.Panel panelLifecycle;
        private Wisej.Web.Label labelLifecycleTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Label liveSessionsLabel;
        private Wisej.Web.ListBox lifecycleListBox;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button incrementButton;
        private Wisej.Web.Button incrementSharedButton;
        private Wisej.Web.Button backgroundButton;
        private Wisej.Web.Button faultButton;
        private Wisej.Web.Button openSessionButton;
        private Wisej.Web.Button exitButton;
        private Wisej.Web.Button clearButton;
    }
}
