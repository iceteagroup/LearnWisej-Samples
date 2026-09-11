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
            this.labelOpenCaption = new Wisej.Web.Label();
            this.openTicketsLabel = new Wisej.Web.Label();
            this.labelQueueCaption = new Wisej.Web.Label();
            this.queueDepthLabel = new Wisej.Web.Label();
            this.labelWaitCaption = new Wisej.Web.Label();
            this.avgWaitLabel = new Wisej.Web.Label();
            this.connectionLabel = new Wisej.Web.Label();
            this.panelCadence = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.liveModeCheckBox = new Wisej.Web.CheckBox();
            this.labelCadenceCaption = new Wisej.Web.Label();
            this.cadenceComboBox = new Wisej.Web.ComboBox();
            this.labelEventsCaption = new Wisej.Web.Label();
            this.eventsReceivedLabel = new Wisej.Web.Label();
            this.labelUpdatesCaption = new Wisej.Web.Label();
            this.updatesAppliedLabel = new Wisej.Web.Label();
            this.labelLastCaption = new Wisej.Web.Label();
            this.lastAppliedLabel = new Wisej.Web.Label();
            this.refreshTimer = new Wisej.Web.Timer(this.components);
            this.statusPanel.SuspendLayout();
            this.panelCadence.SuspendLayout();
            this.SuspendLayout();
            //
            // statusPanel
            //
            this.statusPanel.BackColor = System.Drawing.Color.White;
            this.statusPanel.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.statusPanel.Controls.Add(this.labelOpenCaption);
            this.statusPanel.Controls.Add(this.openTicketsLabel);
            this.statusPanel.Controls.Add(this.labelQueueCaption);
            this.statusPanel.Controls.Add(this.queueDepthLabel);
            this.statusPanel.Controls.Add(this.labelWaitCaption);
            this.statusPanel.Controls.Add(this.avgWaitLabel);
            this.statusPanel.Controls.Add(this.connectionLabel);
            this.statusPanel.Location = new System.Drawing.Point(20, 18);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(700, 80);
            //
            // labelOpenCaption
            //
            this.labelOpenCaption.AutoSize = false;
            this.labelOpenCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelOpenCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelOpenCaption.Location = new System.Drawing.Point(16, 12);
            this.labelOpenCaption.Name = "labelOpenCaption";
            this.labelOpenCaption.Size = new System.Drawing.Size(150, 18);
            this.labelOpenCaption.Text = "OPEN TICKETS";
            //
            // openTicketsLabel
            //
            this.openTicketsLabel.AutoSize = false;
            this.openTicketsLabel.Font = new System.Drawing.Font("monospace", 20F, System.Drawing.FontStyle.Bold);
            this.openTicketsLabel.Location = new System.Drawing.Point(16, 32);
            this.openTicketsLabel.Name = "openTicketsLabel";
            this.openTicketsLabel.Size = new System.Drawing.Size(150, 36);
            this.openTicketsLabel.Text = "—";
            this.openTicketsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelQueueCaption
            //
            this.labelQueueCaption.AutoSize = false;
            this.labelQueueCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelQueueCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelQueueCaption.Location = new System.Drawing.Point(176, 12);
            this.labelQueueCaption.Name = "labelQueueCaption";
            this.labelQueueCaption.Size = new System.Drawing.Size(150, 18);
            this.labelQueueCaption.Text = "QUEUE DEPTH";
            //
            // queueDepthLabel
            //
            this.queueDepthLabel.AutoSize = false;
            this.queueDepthLabel.Font = new System.Drawing.Font("monospace", 20F, System.Drawing.FontStyle.Bold);
            this.queueDepthLabel.Location = new System.Drawing.Point(176, 32);
            this.queueDepthLabel.Name = "queueDepthLabel";
            this.queueDepthLabel.Size = new System.Drawing.Size(150, 36);
            this.queueDepthLabel.Text = "—";
            this.queueDepthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelWaitCaption
            //
            this.labelWaitCaption.AutoSize = false;
            this.labelWaitCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelWaitCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelWaitCaption.Location = new System.Drawing.Point(336, 12);
            this.labelWaitCaption.Name = "labelWaitCaption";
            this.labelWaitCaption.Size = new System.Drawing.Size(150, 18);
            this.labelWaitCaption.Text = "AVG WAIT";
            //
            // avgWaitLabel
            //
            this.avgWaitLabel.AutoSize = false;
            this.avgWaitLabel.Font = new System.Drawing.Font("monospace", 20F, System.Drawing.FontStyle.Bold);
            this.avgWaitLabel.Location = new System.Drawing.Point(336, 32);
            this.avgWaitLabel.Name = "avgWaitLabel";
            this.avgWaitLabel.Size = new System.Drawing.Size(170, 36);
            this.avgWaitLabel.Text = "—";
            this.avgWaitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // connectionLabel
            //
            this.connectionLabel.AutoSize = false;
            this.connectionLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.connectionLabel.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.connectionLabel.Location = new System.Drawing.Point(516, 28);
            this.connectionLabel.Name = "connectionLabel";
            this.connectionLabel.Size = new System.Drawing.Size(168, 24);
            this.connectionLabel.Text = "";
            this.connectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // panelCadence
            //
            this.panelCadence.BackColor = System.Drawing.Color.White;
            this.panelCadence.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelCadence.Controls.Add(this.labelTitle);
            this.panelCadence.Controls.Add(this.liveModeCheckBox);
            this.panelCadence.Controls.Add(this.labelCadenceCaption);
            this.panelCadence.Controls.Add(this.cadenceComboBox);
            this.panelCadence.Controls.Add(this.labelEventsCaption);
            this.panelCadence.Controls.Add(this.eventsReceivedLabel);
            this.panelCadence.Controls.Add(this.labelUpdatesCaption);
            this.panelCadence.Controls.Add(this.updatesAppliedLabel);
            this.panelCadence.Controls.Add(this.labelLastCaption);
            this.panelCadence.Controls.Add(this.lastAppliedLabel);
            this.panelCadence.Location = new System.Drawing.Point(20, 112);
            this.panelCadence.Name = "panelCadence";
            this.panelCadence.Size = new System.Drawing.Size(700, 180);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 16);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(360, 30);
            this.labelTitle.Text = "Update Cadence";
            //
            // liveModeCheckBox
            //
            this.liveModeCheckBox.AutoSize = false;
            this.liveModeCheckBox.Font = new System.Drawing.Font("default", 10F);
            this.liveModeCheckBox.Location = new System.Drawing.Point(24, 58);
            this.liveModeCheckBox.Name = "liveModeCheckBox";
            this.liveModeCheckBox.Size = new System.Drawing.Size(160, 26);
            this.liveModeCheckBox.Text = "Live mode";
            this.liveModeCheckBox.CheckedChanged += new System.EventHandler(this.liveModeCheckBox_CheckedChanged);
            //
            // labelCadenceCaption
            //
            this.labelCadenceCaption.AutoSize = false;
            this.labelCadenceCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelCadenceCaption.Location = new System.Drawing.Point(244, 56);
            this.labelCadenceCaption.Name = "labelCadenceCaption";
            this.labelCadenceCaption.Size = new System.Drawing.Size(80, 32);
            this.labelCadenceCaption.Text = "Cadence";
            this.labelCadenceCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cadenceComboBox
            //
            this.cadenceComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cadenceComboBox.Items.AddRange(new object[] {
            "250 ms",
            "1 sec",
            "5 sec"});
            this.cadenceComboBox.Location = new System.Drawing.Point(328, 56);
            this.cadenceComboBox.Name = "cadenceComboBox";
            this.cadenceComboBox.Size = new System.Drawing.Size(130, 32);
            this.cadenceComboBox.SelectedIndex = 1;
            this.cadenceComboBox.SelectedIndexChanged += new System.EventHandler(this.cadenceComboBox_SelectedIndexChanged);
            //
            // labelEventsCaption
            //
            this.labelEventsCaption.AutoSize = false;
            this.labelEventsCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelEventsCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelEventsCaption.Location = new System.Drawing.Point(24, 106);
            this.labelEventsCaption.Name = "labelEventsCaption";
            this.labelEventsCaption.Size = new System.Drawing.Size(210, 18);
            this.labelEventsCaption.Text = "EVENTS RECEIVED";
            //
            // eventsReceivedLabel
            //
            this.eventsReceivedLabel.AutoSize = false;
            this.eventsReceivedLabel.Font = new System.Drawing.Font("monospace", 18F, System.Drawing.FontStyle.Bold);
            this.eventsReceivedLabel.Location = new System.Drawing.Point(24, 126);
            this.eventsReceivedLabel.Name = "eventsReceivedLabel";
            this.eventsReceivedLabel.Size = new System.Drawing.Size(210, 36);
            this.eventsReceivedLabel.Text = "0";
            this.eventsReceivedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelUpdatesCaption
            //
            this.labelUpdatesCaption.AutoSize = false;
            this.labelUpdatesCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelUpdatesCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelUpdatesCaption.Location = new System.Drawing.Point(244, 106);
            this.labelUpdatesCaption.Name = "labelUpdatesCaption";
            this.labelUpdatesCaption.Size = new System.Drawing.Size(210, 18);
            this.labelUpdatesCaption.Text = "UPDATES APPLIED";
            //
            // updatesAppliedLabel
            //
            this.updatesAppliedLabel.AutoSize = false;
            this.updatesAppliedLabel.Font = new System.Drawing.Font("monospace", 18F, System.Drawing.FontStyle.Bold);
            this.updatesAppliedLabel.Location = new System.Drawing.Point(244, 126);
            this.updatesAppliedLabel.Name = "updatesAppliedLabel";
            this.updatesAppliedLabel.Size = new System.Drawing.Size(210, 36);
            this.updatesAppliedLabel.Text = "0";
            this.updatesAppliedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelLastCaption
            //
            this.labelLastCaption.AutoSize = false;
            this.labelLastCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelLastCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLastCaption.Location = new System.Drawing.Point(464, 106);
            this.labelLastCaption.Name = "labelLastCaption";
            this.labelLastCaption.Size = new System.Drawing.Size(210, 18);
            this.labelLastCaption.Text = "LAST APPLIED";
            //
            // lastAppliedLabel
            //
            this.lastAppliedLabel.AutoSize = false;
            this.lastAppliedLabel.Font = new System.Drawing.Font("monospace", 14F, System.Drawing.FontStyle.Bold);
            this.lastAppliedLabel.Location = new System.Drawing.Point(464, 126);
            this.lastAppliedLabel.Name = "lastAppliedLabel";
            this.lastAppliedLabel.Size = new System.Drawing.Size(210, 36);
            this.lastAppliedLabel.Text = "—";
            this.lastAppliedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // refreshTimer
            //
            this.refreshTimer.Interval = 1000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.panelCadence);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(740, 312);
            this.Text = "TicketOps Live — Update Cadence";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.statusPanel.ResumeLayout(false);
            this.panelCadence.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel statusPanel;
        private Wisej.Web.Label labelOpenCaption;
        private Wisej.Web.Label openTicketsLabel;
        private Wisej.Web.Label labelQueueCaption;
        private Wisej.Web.Label queueDepthLabel;
        private Wisej.Web.Label labelWaitCaption;
        private Wisej.Web.Label avgWaitLabel;
        private Wisej.Web.Label connectionLabel;
        private Wisej.Web.Panel panelCadence;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.CheckBox liveModeCheckBox;
        private Wisej.Web.Label labelCadenceCaption;
        private Wisej.Web.ComboBox cadenceComboBox;
        private Wisej.Web.Label labelEventsCaption;
        private Wisej.Web.Label eventsReceivedLabel;
        private Wisej.Web.Label labelUpdatesCaption;
        private Wisej.Web.Label updatesAppliedLabel;
        private Wisej.Web.Label labelLastCaption;
        private Wisej.Web.Label lastAppliedLabel;
        private Wisej.Web.Timer refreshTimer;
    }
}
