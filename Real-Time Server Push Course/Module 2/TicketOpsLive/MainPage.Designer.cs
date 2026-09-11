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
            this.labelCounterCaption = new Wisej.Web.Label();
            this.counterLabel = new Wisej.Web.Label();
            this.incrementButton = new Wisej.Web.Button();
            this.backgroundButton = new Wisej.Web.Button();
            this.openSessionButton = new Wisej.Web.Button();
            this.clearButton = new Wisej.Web.Button();
            this.panelLifecycle = new Wisej.Web.Panel();
            this.labelLifecycleTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.liveSessionsLabel = new Wisej.Web.Label();
            this.lifecycleListBox = new Wisej.Web.ListBox();
            this.diagnosticsGroupBox.SuspendLayout();
            this.panelLifecycle.SuspendLayout();
            this.SuspendLayout();
            //
            // diagnosticsGroupBox
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
            this.diagnosticsGroupBox.Controls.Add(this.labelCounterCaption);
            this.diagnosticsGroupBox.Controls.Add(this.counterLabel);
            this.diagnosticsGroupBox.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.diagnosticsGroupBox.Location = new System.Drawing.Point(20, 18);
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
            this.timeLabel.Text = "--:--:--";
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
            this.clientIdLabel.Text = "";
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
            this.sessionIdLabel.Text = "";
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
            this.browserLabel.Text = "";
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
            this.threadLabel.Text = "";
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
            this.incrementButton.Location = new System.Drawing.Point(20, 232);
            this.incrementButton.Name = "incrementButton";
            this.incrementButton.Size = new System.Drawing.Size(130, 36);
            this.incrementButton.Text = "Counter +1";
            this.incrementButton.Click += new System.EventHandler(this.incrementButton_Click);
            //
            // backgroundButton
            //
            this.backgroundButton.Location = new System.Drawing.Point(158, 232);
            this.backgroundButton.Name = "backgroundButton";
            this.backgroundButton.Size = new System.Drawing.Size(190, 36);
            this.backgroundButton.Text = "Background update";
            this.backgroundButton.Click += new System.EventHandler(this.backgroundButton_Click);
            //
            // openSessionButton
            //
            this.openSessionButton.Location = new System.Drawing.Point(20, 276);
            this.openSessionButton.Name = "openSessionButton";
            this.openSessionButton.Size = new System.Drawing.Size(190, 36);
            this.openSessionButton.Text = "Open second window";
            this.openSessionButton.Click += new System.EventHandler(this.openSessionButton_Click);
            //
            // clearButton
            //
            this.clearButton.Location = new System.Drawing.Point(218, 276);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(130, 36);
            this.clearButton.Text = "Clear log";
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            //
            // panelLifecycle
            //
            this.panelLifecycle.BackColor = System.Drawing.Color.White;
            this.panelLifecycle.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLifecycle.Controls.Add(this.labelLifecycleTitle);
            this.panelLifecycle.Controls.Add(this.labelStatus);
            this.panelLifecycle.Controls.Add(this.liveSessionsLabel);
            this.panelLifecycle.Controls.Add(this.lifecycleListBox);
            this.panelLifecycle.Location = new System.Drawing.Point(500, 18);
            this.panelLifecycle.Name = "panelLifecycle";
            this.panelLifecycle.Size = new System.Drawing.Size(480, 294);
            //
            // labelLifecycleTitle
            //
            this.labelLifecycleTitle.AutoSize = false;
            this.labelLifecycleTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelLifecycleTitle.Location = new System.Drawing.Point(16, 12);
            this.labelLifecycleTitle.Name = "labelLifecycleTitle";
            this.labelLifecycleTitle.Size = new System.Drawing.Size(200, 28);
            this.labelLifecycleTitle.Text = "Lifecycle log";
            //
            // labelStatus
            //
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.labelStatus.Location = new System.Drawing.Point(216, 16);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(248, 22);
            this.labelStatus.Text = "";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // liveSessionsLabel
            //
            this.liveSessionsLabel.AutoSize = false;
            this.liveSessionsLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.liveSessionsLabel.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.liveSessionsLabel.Location = new System.Drawing.Point(16, 44);
            this.liveSessionsLabel.Name = "liveSessionsLabel";
            this.liveSessionsLabel.Size = new System.Drawing.Size(448, 20);
            this.liveSessionsLabel.Text = "Live sessions: —";
            //
            // lifecycleListBox
            //
            this.lifecycleListBox.Font = new System.Drawing.Font("monospace", 9F);
            this.lifecycleListBox.Location = new System.Drawing.Point(16, 70);
            this.lifecycleListBox.Name = "lifecycleListBox";
            this.lifecycleListBox.Size = new System.Drawing.Size(448, 208);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.diagnosticsGroupBox);
            this.Controls.Add(this.incrementButton);
            this.Controls.Add(this.backgroundButton);
            this.Controls.Add(this.openSessionButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.panelLifecycle);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1000, 332);
            this.Text = "TicketOps Live — Session";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.diagnosticsGroupBox.ResumeLayout(false);
            this.panelLifecycle.ResumeLayout(false);
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
        private Wisej.Web.Label labelCounterCaption;
        private Wisej.Web.Label counterLabel;
        private Wisej.Web.Button incrementButton;
        private Wisej.Web.Button backgroundButton;
        private Wisej.Web.Button openSessionButton;
        private Wisej.Web.Button clearButton;
        private Wisej.Web.Panel panelLifecycle;
        private Wisej.Web.Label labelLifecycleTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Label liveSessionsLabel;
        private Wisej.Web.ListBox lifecycleListBox;
    }
}
