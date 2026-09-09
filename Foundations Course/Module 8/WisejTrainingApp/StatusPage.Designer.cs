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
            this.components = new System.ComponentModel.Container();
            this.panelWidget = new Wisej.Web.Panel();
            this.labelWidgetCard = new Wisej.Web.Label();
            this.widStatus = new Wisej.Web.Widget();
            this.lblWidgetCaption = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.panelData = new Wisej.Web.Panel();
            this.labelDataCard = new Wisej.Web.Label();
            this.lblOpenCaption = new Wisej.Web.Label();
            this.lblOpen = new Wisej.Web.Label();
            this.lblClosedCaption = new Wisej.Web.Label();
            this.lblClosed = new Wisej.Web.Label();
            this.lblLoadCaption = new Wisej.Web.Label();
            this.lblLoad = new Wisej.Web.Label();
            this.lblStatusValueCaption = new Wisej.Web.Label();
            this.lblStatusValue = new Wisej.Web.Label();
            this.lblRule = new Wisej.Web.Label();
            this.lblSentCaption = new Wisej.Web.Label();
            this.lblSent = new Wisej.Web.Label();
            this.lblNeverSentCaption = new Wisej.Web.Label();
            this.lblNeverSent = new Wisej.Web.Label();
            this.panelLog = new Wisej.Web.Panel();
            this.labelLogCard = new Wisej.Web.Label();
            this.lstEventLog = new Wisej.Web.ListBox();
            this.labelLogFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.btnRefresh = new Wisej.Web.Button();
            this.btnSetHealthy = new Wisej.Web.Button();
            this.btnSetWarning = new Wisej.Web.Button();
            this.btnSetCritical = new Wisej.Web.Button();
            this.btnSimulateError = new Wisej.Web.Button();
            this.btnToggleTheme = new Wisej.Web.Button();
            this.btnClearLog = new Wisej.Web.Button();
            this.panelWidget.SuspendLayout();
            this.panelData.SuspendLayout();
            this.panelLog.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelWidget  (the "Status widget" card — the lab's Widget control lives here)
            //
            this.panelWidget.BackColor = System.Drawing.Color.White;
            this.panelWidget.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelWidget.Controls.Add(this.labelWidgetCard);
            this.panelWidget.Controls.Add(this.widStatus);
            this.panelWidget.Controls.Add(this.lblWidgetCaption);
            this.panelWidget.Controls.Add(this.lblStatus);
            this.panelWidget.Location = new System.Drawing.Point(30, 30);
            this.panelWidget.Name = "panelWidget";
            this.panelWidget.Size = new System.Drawing.Size(560, 250);
            //
            // labelWidgetCard
            //
            this.labelWidgetCard.AutoSize = false;
            this.labelWidgetCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelWidgetCard.Location = new System.Drawing.Point(24, 14);
            this.labelWidgetCard.Name = "labelWidgetCard";
            this.labelWidgetCard.Size = new System.Drawing.Size(512, 28);
            this.labelWidgetCard.Text = "Status widget  ·  Wisej.Web.Widget fed by C#";
            //
            // widStatus  (Wisej.Web.Widget — Packages, InitScript and Options are set in StatusPage_Load)
            //
            this.widStatus.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.widStatus.Location = new System.Drawing.Point(24, 50);
            this.widStatus.Name = "widStatus";
            this.widStatus.Size = new System.Drawing.Size(420, 140);
            this.widStatus.ToolTipText = "Click the gauge: the JS fires gaugeClick → C# widStatus_WidgetEvent logs it.";
            //
            // lblWidgetCaption
            //
            this.lblWidgetCaption.AutoSize = false;
            this.lblWidgetCaption.Font = new System.Drawing.Font("monospace", 9F);
            this.lblWidgetCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblWidgetCaption.Location = new System.Drawing.Point(24, 196);
            this.lblWidgetCaption.Name = "lblWidgetCaption";
            this.lblWidgetCaption.Size = new System.Drawing.Size(512, 22);
            this.lblWidgetCaption.Text = "Packages: statusGauge.css · InitScript: statusGauge.js · click the gauge → gaugeClick";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(24, 220);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(512, 24);
            this.lblStatus.Text = "● ready";
            //
            // panelData  (lab step 6: the native "Server-side data" card)
            //
            this.panelData.BackColor = System.Drawing.Color.White;
            this.panelData.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelData.Controls.Add(this.labelDataCard);
            this.panelData.Controls.Add(this.lblOpenCaption);
            this.panelData.Controls.Add(this.lblOpen);
            this.panelData.Controls.Add(this.lblClosedCaption);
            this.panelData.Controls.Add(this.lblClosed);
            this.panelData.Controls.Add(this.lblLoadCaption);
            this.panelData.Controls.Add(this.lblLoad);
            this.panelData.Controls.Add(this.lblStatusValueCaption);
            this.panelData.Controls.Add(this.lblStatusValue);
            this.panelData.Controls.Add(this.lblRule);
            this.panelData.Controls.Add(this.lblSentCaption);
            this.panelData.Controls.Add(this.lblSent);
            this.panelData.Controls.Add(this.lblNeverSentCaption);
            this.panelData.Controls.Add(this.lblNeverSent);
            this.panelData.Location = new System.Drawing.Point(30, 298);
            this.panelData.Name = "panelData";
            this.panelData.Size = new System.Drawing.Size(560, 312);
            //
            // labelDataCard
            //
            this.labelDataCard.AutoSize = false;
            this.labelDataCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelDataCard.Location = new System.Drawing.Point(24, 14);
            this.labelDataCard.Name = "labelDataCard";
            this.labelDataCard.Size = new System.Drawing.Size(512, 28);
            this.labelDataCard.Text = "Server-side data  ·  what C# holds right now";
            //
            // lblOpenCaption / lblOpen
            //
            this.lblOpenCaption.AutoSize = false;
            this.lblOpenCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblOpenCaption.Location = new System.Drawing.Point(24, 50);
            this.lblOpenCaption.Name = "lblOpenCaption";
            this.lblOpenCaption.Size = new System.Drawing.Size(160, 22);
            this.lblOpenCaption.Text = "Open tickets";
            this.lblOpen.AutoSize = false;
            this.lblOpen.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblOpen.Location = new System.Drawing.Point(190, 50);
            this.lblOpen.Name = "lblOpen";
            this.lblOpen.Size = new System.Drawing.Size(120, 22);
            this.lblOpen.Text = "–";
            //
            // lblClosedCaption / lblClosed
            //
            this.lblClosedCaption.AutoSize = false;
            this.lblClosedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblClosedCaption.Location = new System.Drawing.Point(24, 76);
            this.lblClosedCaption.Name = "lblClosedCaption";
            this.lblClosedCaption.Size = new System.Drawing.Size(160, 22);
            this.lblClosedCaption.Text = "Closed tickets";
            this.lblClosed.AutoSize = false;
            this.lblClosed.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblClosed.Location = new System.Drawing.Point(190, 76);
            this.lblClosed.Name = "lblClosed";
            this.lblClosed.Size = new System.Drawing.Size(120, 22);
            this.lblClosed.Text = "–";
            //
            // lblLoadCaption / lblLoad
            //
            this.lblLoadCaption.AutoSize = false;
            this.lblLoadCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblLoadCaption.Location = new System.Drawing.Point(24, 102);
            this.lblLoadCaption.Name = "lblLoadCaption";
            this.lblLoadCaption.Size = new System.Drawing.Size(160, 22);
            this.lblLoadCaption.Text = "System load";
            this.lblLoad.AutoSize = false;
            this.lblLoad.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblLoad.Location = new System.Drawing.Point(190, 102);
            this.lblLoad.Name = "lblLoad";
            this.lblLoad.Size = new System.Drawing.Size(120, 22);
            this.lblLoad.Text = "–";
            //
            // lblStatusValueCaption / lblStatusValue
            //
            this.lblStatusValueCaption.AutoSize = false;
            this.lblStatusValueCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStatusValueCaption.Location = new System.Drawing.Point(24, 128);
            this.lblStatusValueCaption.Name = "lblStatusValueCaption";
            this.lblStatusValueCaption.Size = new System.Drawing.Size(160, 22);
            this.lblStatusValueCaption.Text = "Status (C# rule)";
            this.lblStatusValue.AutoSize = false;
            this.lblStatusValue.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblStatusValue.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatusValue.Location = new System.Drawing.Point(190, 128);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(120, 22);
            this.lblStatusValue.Text = "–";
            //
            // lblRule  (where the rule lives: StatusService.StatusFor)
            //
            this.lblRule.AutoSize = false;
            this.lblRule.Font = new System.Drawing.Font("monospace", 8F);
            this.lblRule.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblRule.Location = new System.Drawing.Point(330, 50);
            this.lblRule.Name = "lblRule";
            this.lblRule.Size = new System.Drawing.Size(206, 100);
            this.lblRule.Text = "rule: StatusService.StatusFor\n  load <  60  → \"ok\"\n  load 60–85  → \"warn\"\n  load >  85  → \"error\"\nJS only picks the colour.";
            this.lblRule.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // lblSentCaption / lblSent  (the exact Options object last sent)
            //
            this.lblSentCaption.AutoSize = false;
            this.lblSentCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSentCaption.Location = new System.Drawing.Point(24, 158);
            this.lblSentCaption.Name = "lblSentCaption";
            this.lblSentCaption.Size = new System.Drawing.Size(512, 20);
            this.lblSentCaption.Text = "Sent to the widget — widStatus.Options (the only values that reach the browser)";
            this.lblSent.AutoSize = false;
            this.lblSent.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblSent.Font = new System.Drawing.Font("monospace", 9F);
            this.lblSent.Location = new System.Drawing.Point(24, 180);
            this.lblSent.Name = "lblSent";
            this.lblSent.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.lblSent.Size = new System.Drawing.Size(512, 28);
            this.lblSent.Text = "{ }";
            this.lblSent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblNeverSentCaption / lblNeverSent  (lab step 9)
            //
            this.lblNeverSentCaption.AutoSize = false;
            this.lblNeverSentCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNeverSentCaption.Location = new System.Drawing.Point(24, 216);
            this.lblNeverSentCaption.Name = "lblNeverSentCaption";
            this.lblNeverSentCaption.Size = new System.Drawing.Size(512, 20);
            this.lblNeverSentCaption.Text = "Never sent to the browser";
            this.lblNeverSent.AutoSize = false;
            this.lblNeverSent.Font = new System.Drawing.Font("monospace", 8F);
            this.lblNeverSent.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNeverSent.Location = new System.Drawing.Point(24, 238);
            this.lblNeverSent.Name = "lblNeverSent";
            this.lblNeverSent.Size = new System.Drawing.Size(512, 62);
            this.lblNeverSent.Text = "";
            this.lblNeverSent.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelLog  (Event log · what the server-side code did)
            //
            this.panelLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelLog.BackColor = System.Drawing.Color.White;
            this.panelLog.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLog.Controls.Add(this.labelLogCard);
            this.panelLog.Controls.Add(this.lstEventLog);
            this.panelLog.Controls.Add(this.labelLogFooter);
            this.panelLog.Location = new System.Drawing.Point(618, 30);
            this.panelLog.Name = "panelLog";
            this.panelLog.Size = new System.Drawing.Size(700, 580);
            //
            // labelLogCard
            //
            this.labelLogCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogCard.AutoSize = false;
            this.labelLogCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelLogCard.Location = new System.Drawing.Point(20, 14);
            this.labelLogCard.Name = "labelLogCard";
            this.labelLogCard.Size = new System.Drawing.Size(660, 30);
            this.labelLogCard.Text = "Event log  ·  what the server-side code did";
            //
            // lstEventLog
            //
            this.lstEventLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstEventLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstEventLog.Location = new System.Drawing.Point(20, 52);
            this.lstEventLog.Name = "lstEventLog";
            this.lstEventLog.Size = new System.Drawing.Size(660, 476);
            //
            // labelLogFooter
            //
            this.labelLogFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogFooter.AutoSize = false;
            this.labelLogFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLogFooter.Location = new System.Drawing.Point(20, 538);
            this.labelLogFooter.Name = "labelLogFooter";
            this.labelLogFooter.Size = new System.Drawing.Size(660, 26);
            this.labelLogFooter.Text = "C# service → Options → Update() / Call(\"pulse\") → browser draws  ·  click → fireWidgetEvent → WidgetEvent in C#";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.btnRefresh);
            this.panelActions.Controls.Add(this.btnSetHealthy);
            this.panelActions.Controls.Add(this.btnSetWarning);
            this.panelActions.Controls.Add(this.btnSetCritical);
            this.panelActions.Controls.Add(this.btnSimulateError);
            this.panelActions.Controls.Add(this.btnToggleTheme);
            this.panelActions.Controls.Add(this.btnClearLog);
            this.panelActions.Location = new System.Drawing.Point(30, 626);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // btnRefresh  (lab step 7: success path)
            //
            this.btnRefresh.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Location = new System.Drawing.Point(0, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(170, 36);
            this.btnRefresh.Text = "Refresh Server Data";
            this.btnRefresh.ToolTipText = "StatusService.Refresh() + GetStatus() on the server → UpdateWidget(data): Options, Update(), Call(\"pulse\") + the native card.";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnSetHealthy / btnSetWarning / btnSetCritical  (the C# rule recolours the widget)
            //
            this.btnSetHealthy.Location = new System.Drawing.Point(178, 4);
            this.btnSetHealthy.Name = "btnSetHealthy";
            this.btnSetHealthy.Size = new System.Drawing.Size(110, 36);
            this.btnSetHealthy.Text = "Set Healthy";
            this.btnSetHealthy.ToolTipText = "Load 35 % → the C# rule says \"ok\" → the widget turns green.";
            this.btnSetHealthy.Click += new System.EventHandler(this.btnSetHealthy_Click);
            this.btnSetWarning.Location = new System.Drawing.Point(296, 4);
            this.btnSetWarning.Name = "btnSetWarning";
            this.btnSetWarning.Size = new System.Drawing.Size(110, 36);
            this.btnSetWarning.Text = "Set Warning";
            this.btnSetWarning.ToolTipText = "Load 72 % → the C# rule says \"warn\" → the widget turns amber.";
            this.btnSetWarning.Click += new System.EventHandler(this.btnSetWarning_Click);
            this.btnSetCritical.Location = new System.Drawing.Point(414, 4);
            this.btnSetCritical.Name = "btnSetCritical";
            this.btnSetCritical.Size = new System.Drawing.Size(110, 36);
            this.btnSetCritical.Text = "Set Critical";
            this.btnSetCritical.ToolTipText = "Load 93 % → the C# rule says \"error\" → the widget turns red.";
            this.btnSetCritical.Click += new System.EventHandler(this.btnSetCritical_Click);
            //
            // btnSimulateError  (failure path: GetStatus() throws once)
            //
            this.btnSimulateError.Location = new System.Drawing.Point(532, 4);
            this.btnSimulateError.Name = "btnSimulateError";
            this.btnSimulateError.Size = new System.Drawing.Size(170, 36);
            this.btnSimulateError.Text = "Simulate service error";
            this.btnSimulateError.ToolTipText = "The next GetStatus() throws: safe message in lblStatus, detail in the log, the widget keeps its last values. Refresh again to recover.";
            this.btnSimulateError.Click += new System.EventHandler(this.btnSimulateError_Click);
            //
            // btnToggleTheme  (lab step 10: re-test after a theme change)
            //
            this.btnToggleTheme.Location = new System.Drawing.Point(710, 4);
            this.btnToggleTheme.Name = "btnToggleTheme";
            this.btnToggleTheme.Size = new System.Drawing.Size(300, 36);
            this.btnToggleTheme.Text = "Switch theme (Bootstrap-4 ⇄ Material-3)";
            this.btnToggleTheme.ToolTipText = "Application.LoadTheme(...) restyles the native controls; the widget's scoped CSS must survive it — click Refresh again to prove it.";
            this.btnToggleTheme.Click += new System.EventHandler(this.btnToggleTheme_Click);
            //
            // btnClearLog
            //
            this.btnClearLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearLog.Location = new System.Drawing.Point(1178, 4);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(110, 36);
            this.btnClearLog.Text = "Clear log";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // StatusPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelWidget);
            this.Controls.Add(this.panelData);
            this.Controls.Add(this.panelLog);
            this.Controls.Add(this.panelActions);
            this.Name = "StatusPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "WisejTrainingApp — JavaScript widget (Module 8)";
            this.Load += new System.EventHandler(this.StatusPage_Load);
            this.panelWidget.ResumeLayout(false);
            this.panelData.ResumeLayout(false);
            this.panelLog.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelWidget;
        private Wisej.Web.Label labelWidgetCard;
        private Wisej.Web.Widget widStatus;
        private Wisej.Web.Label lblWidgetCaption;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel panelData;
        private Wisej.Web.Label labelDataCard;
        private Wisej.Web.Label lblOpenCaption;
        private Wisej.Web.Label lblOpen;
        private Wisej.Web.Label lblClosedCaption;
        private Wisej.Web.Label lblClosed;
        private Wisej.Web.Label lblLoadCaption;
        private Wisej.Web.Label lblLoad;
        private Wisej.Web.Label lblStatusValueCaption;
        private Wisej.Web.Label lblStatusValue;
        private Wisej.Web.Label lblRule;
        private Wisej.Web.Label lblSentCaption;
        private Wisej.Web.Label lblSent;
        private Wisej.Web.Label lblNeverSentCaption;
        private Wisej.Web.Label lblNeverSent;
        private Wisej.Web.Panel panelLog;
        private Wisej.Web.Label labelLogCard;
        private Wisej.Web.ListBox lstEventLog;
        private Wisej.Web.Label labelLogFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Button btnSetHealthy;
        private Wisej.Web.Button btnSetWarning;
        private Wisej.Web.Button btnSetCritical;
        private Wisej.Web.Button btnSimulateError;
        private Wisej.Web.Button btnToggleTheme;
        private Wisej.Web.Button btnClearLog;
    }
}
