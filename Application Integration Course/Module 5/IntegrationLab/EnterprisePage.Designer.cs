namespace IntegrationLab
{
    partial class EnterprisePage
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
            this.labelTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.panelKpiLines = new Wisej.Web.Panel();
            this.stripKpiLines = new Wisej.Web.Panel();
            this.labelKpiLinesTitle = new Wisej.Web.Label();
            this.labelKpiLinesValue = new Wisej.Web.Label();
            this.panelKpiThroughput = new Wisej.Web.Panel();
            this.stripKpiThroughput = new Wisej.Web.Panel();
            this.labelKpiThroughputTitle = new Wisej.Web.Label();
            this.labelKpiThroughputValue = new Wisej.Web.Label();
            this.panelKpiAlerts = new Wisej.Web.Panel();
            this.stripKpiAlerts = new Wisej.Web.Panel();
            this.labelKpiAlertsTitle = new Wisej.Web.Label();
            this.labelKpiAlertsValue = new Wisej.Web.Label();
            this.panelKpiUptime = new Wisej.Web.Panel();
            this.stripKpiUptime = new Wisej.Web.Panel();
            this.labelKpiUptimeTitle = new Wisej.Web.Label();
            this.labelKpiUptimeValue = new Wisej.Web.Label();
            this.panelTile1 = new Wisej.Web.Panel();
            this.labelTile1 = new Wisej.Web.Label();
            this.gaugeBoiler1 = new IntegrationLab.Controls.SimpleGaugeControl();
            this.labelBanner1 = new Wisej.Web.Label();
            this.panelTile2 = new Wisej.Web.Panel();
            this.labelTile2 = new Wisej.Web.Label();
            this.gaugeBoiler2 = new IntegrationLab.Controls.SimpleGaugeControl();
            this.labelBanner2 = new Wisej.Web.Label();
            this.panelTile3 = new Wisej.Web.Panel();
            this.labelTile3 = new Wisej.Web.Label();
            this.gaugeTurbine = new IntegrationLab.Controls.SimpleGaugeControl();
            this.labelBanner3 = new Wisej.Web.Label();
            this.panelTile4 = new Wisej.Web.Panel();
            this.labelTile4 = new Wisej.Web.Label();
            this.gaugeCoolant = new IntegrationLab.Controls.SimpleGaugeControl();
            this.labelBanner4 = new Wisej.Web.Label();
            this.timerStream = new Wisej.Web.Timer(this.components);
            this.panelKpiLines.SuspendLayout();
            this.panelKpiThroughput.SuspendLayout();
            this.panelKpiAlerts.SuspendLayout();
            this.panelKpiUptime.SuspendLayout();
            this.panelTile1.SuspendLayout();
            this.panelTile2.SuspendLayout();
            this.panelTile3.SuspendLayout();
            this.panelTile4.SuspendLayout();
            this.SuspendLayout();
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(30, 14);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(700, 32);
            this.labelTitle.Text = "IntegrationLab — Operations Dashboard";
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(918, 16);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(400, 28);
            this.labelStatus.Text = "● live";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // KPI strip: Active lines
            //
            this.panelKpiLines.BackColor = System.Drawing.Color.White;
            this.panelKpiLines.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelKpiLines.Controls.Add(this.stripKpiLines);
            this.panelKpiLines.Controls.Add(this.labelKpiLinesTitle);
            this.panelKpiLines.Controls.Add(this.labelKpiLinesValue);
            this.panelKpiLines.Location = new System.Drawing.Point(30, 56);
            this.panelKpiLines.Name = "panelKpiLines";
            this.panelKpiLines.Size = new System.Drawing.Size(307, 64);
            this.stripKpiLines.BackColor = System.Drawing.Color.FromArgb(26, 134, 255);
            this.stripKpiLines.Location = new System.Drawing.Point(0, 0);
            this.stripKpiLines.Name = "stripKpiLines";
            this.stripKpiLines.Size = new System.Drawing.Size(4, 62);
            this.labelKpiLinesTitle.AutoSize = false;
            this.labelKpiLinesTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelKpiLinesTitle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelKpiLinesTitle.Location = new System.Drawing.Point(16, 8);
            this.labelKpiLinesTitle.Name = "labelKpiLinesTitle";
            this.labelKpiLinesTitle.Size = new System.Drawing.Size(280, 16);
            this.labelKpiLinesTitle.Text = "ACTIVE LINES";
            this.labelKpiLinesValue.AutoSize = false;
            this.labelKpiLinesValue.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.labelKpiLinesValue.Location = new System.Drawing.Point(16, 26);
            this.labelKpiLinesValue.Name = "labelKpiLinesValue";
            this.labelKpiLinesValue.Size = new System.Drawing.Size(280, 30);
            this.labelKpiLinesValue.Text = "12";
            //
            // KPI strip: Avg. throughput
            //
            this.panelKpiThroughput.BackColor = System.Drawing.Color.White;
            this.panelKpiThroughput.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelKpiThroughput.Controls.Add(this.stripKpiThroughput);
            this.panelKpiThroughput.Controls.Add(this.labelKpiThroughputTitle);
            this.panelKpiThroughput.Controls.Add(this.labelKpiThroughputValue);
            this.panelKpiThroughput.Location = new System.Drawing.Point(357, 56);
            this.panelKpiThroughput.Name = "panelKpiThroughput";
            this.panelKpiThroughput.Size = new System.Drawing.Size(307, 64);
            this.stripKpiThroughput.BackColor = System.Drawing.Color.FromArgb(31, 157, 107);
            this.stripKpiThroughput.Location = new System.Drawing.Point(0, 0);
            this.stripKpiThroughput.Name = "stripKpiThroughput";
            this.stripKpiThroughput.Size = new System.Drawing.Size(4, 62);
            this.labelKpiThroughputTitle.AutoSize = false;
            this.labelKpiThroughputTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelKpiThroughputTitle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelKpiThroughputTitle.Location = new System.Drawing.Point(16, 8);
            this.labelKpiThroughputTitle.Name = "labelKpiThroughputTitle";
            this.labelKpiThroughputTitle.Size = new System.Drawing.Size(280, 16);
            this.labelKpiThroughputTitle.Text = "AVG. THROUGHPUT";
            this.labelKpiThroughputValue.AutoSize = false;
            this.labelKpiThroughputValue.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.labelKpiThroughputValue.Location = new System.Drawing.Point(16, 26);
            this.labelKpiThroughputValue.Name = "labelKpiThroughputValue";
            this.labelKpiThroughputValue.Size = new System.Drawing.Size(280, 30);
            this.labelKpiThroughputValue.Text = "847/min";
            //
            // KPI strip: Alerts
            //
            this.panelKpiAlerts.BackColor = System.Drawing.Color.White;
            this.panelKpiAlerts.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelKpiAlerts.Controls.Add(this.stripKpiAlerts);
            this.panelKpiAlerts.Controls.Add(this.labelKpiAlertsTitle);
            this.panelKpiAlerts.Controls.Add(this.labelKpiAlertsValue);
            this.panelKpiAlerts.Location = new System.Drawing.Point(684, 56);
            this.panelKpiAlerts.Name = "panelKpiAlerts";
            this.panelKpiAlerts.Size = new System.Drawing.Size(307, 64);
            this.stripKpiAlerts.BackColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.stripKpiAlerts.Location = new System.Drawing.Point(0, 0);
            this.stripKpiAlerts.Name = "stripKpiAlerts";
            this.stripKpiAlerts.Size = new System.Drawing.Size(4, 62);
            this.labelKpiAlertsTitle.AutoSize = false;
            this.labelKpiAlertsTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelKpiAlertsTitle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelKpiAlertsTitle.Location = new System.Drawing.Point(16, 8);
            this.labelKpiAlertsTitle.Name = "labelKpiAlertsTitle";
            this.labelKpiAlertsTitle.Size = new System.Drawing.Size(280, 16);
            this.labelKpiAlertsTitle.Text = "ALERTS";
            this.labelKpiAlertsValue.AutoSize = false;
            this.labelKpiAlertsValue.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.labelKpiAlertsValue.Location = new System.Drawing.Point(16, 26);
            this.labelKpiAlertsValue.Name = "labelKpiAlertsValue";
            this.labelKpiAlertsValue.Size = new System.Drawing.Size(280, 30);
            this.labelKpiAlertsValue.Text = "0";
            //
            // KPI strip: Uptime
            //
            this.panelKpiUptime.BackColor = System.Drawing.Color.White;
            this.panelKpiUptime.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelKpiUptime.Controls.Add(this.stripKpiUptime);
            this.panelKpiUptime.Controls.Add(this.labelKpiUptimeTitle);
            this.panelKpiUptime.Controls.Add(this.labelKpiUptimeValue);
            this.panelKpiUptime.Location = new System.Drawing.Point(1011, 56);
            this.panelKpiUptime.Name = "panelKpiUptime";
            this.panelKpiUptime.Size = new System.Drawing.Size(307, 64);
            this.stripKpiUptime.BackColor = System.Drawing.Color.FromArgb(125, 90, 224);
            this.stripKpiUptime.Location = new System.Drawing.Point(0, 0);
            this.stripKpiUptime.Name = "stripKpiUptime";
            this.stripKpiUptime.Size = new System.Drawing.Size(4, 62);
            this.labelKpiUptimeTitle.AutoSize = false;
            this.labelKpiUptimeTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelKpiUptimeTitle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelKpiUptimeTitle.Location = new System.Drawing.Point(16, 8);
            this.labelKpiUptimeTitle.Name = "labelKpiUptimeTitle";
            this.labelKpiUptimeTitle.Size = new System.Drawing.Size(280, 16);
            this.labelKpiUptimeTitle.Text = "UPTIME";
            this.labelKpiUptimeValue.AutoSize = false;
            this.labelKpiUptimeValue.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.labelKpiUptimeValue.Location = new System.Drawing.Point(16, 26);
            this.labelKpiUptimeValue.Name = "labelKpiUptimeValue";
            this.labelKpiUptimeValue.Size = new System.Drawing.Size(280, 30);
            this.labelKpiUptimeValue.Text = "99.98%";
            //
            // Tile 1: Boiler 1 — pressure
            //
            this.panelTile1.BackColor = System.Drawing.Color.White;
            this.panelTile1.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTile1.Controls.Add(this.labelTile1);
            this.panelTile1.Controls.Add(this.gaugeBoiler1);
            this.panelTile1.Controls.Add(this.labelBanner1);
            this.panelTile1.Location = new System.Drawing.Point(30, 136);
            this.panelTile1.Name = "panelTile1";
            this.panelTile1.Size = new System.Drawing.Size(307, 300);
            this.labelTile1.AutoSize = false;
            this.labelTile1.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelTile1.Location = new System.Drawing.Point(14, 8);
            this.labelTile1.Name = "labelTile1";
            this.labelTile1.Size = new System.Drawing.Size(279, 24);
            this.labelTile1.Text = "Boiler 1 — pressure";
            this.gaugeBoiler1.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gaugeBoiler1.Caption = "Boiler 1";
            this.gaugeBoiler1.Location = new System.Drawing.Point(14, 34);
            this.gaugeBoiler1.Maximum = 150D;
            this.gaugeBoiler1.Minimum = 0D;
            this.gaugeBoiler1.Name = "gaugeBoiler1";
            this.gaugeBoiler1.Size = new System.Drawing.Size(279, 226);
            this.gaugeBoiler1.Threshold = 120D;
            this.gaugeBoiler1.Units = " psi";
            this.gaugeBoiler1.Value = 70D;
            this.labelBanner1.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner1.AutoSize = false;
            this.labelBanner1.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner1.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner1.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner1.Location = new System.Drawing.Point(14, 268);
            this.labelBanner1.Name = "labelBanner1";
            this.labelBanner1.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.labelBanner1.Size = new System.Drawing.Size(279, 22);
            this.labelBanner1.Text = "";
            this.labelBanner1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner1.Visible = false;
            //
            // Tile 2: Boiler 2 — pressure
            //
            this.panelTile2.BackColor = System.Drawing.Color.White;
            this.panelTile2.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTile2.Controls.Add(this.labelTile2);
            this.panelTile2.Controls.Add(this.gaugeBoiler2);
            this.panelTile2.Controls.Add(this.labelBanner2);
            this.panelTile2.Location = new System.Drawing.Point(357, 136);
            this.panelTile2.Name = "panelTile2";
            this.panelTile2.Size = new System.Drawing.Size(307, 300);
            this.labelTile2.AutoSize = false;
            this.labelTile2.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelTile2.Location = new System.Drawing.Point(14, 8);
            this.labelTile2.Name = "labelTile2";
            this.labelTile2.Size = new System.Drawing.Size(279, 24);
            this.labelTile2.Text = "Boiler 2 — pressure";
            this.gaugeBoiler2.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gaugeBoiler2.Caption = "Boiler 2";
            this.gaugeBoiler2.Location = new System.Drawing.Point(14, 34);
            this.gaugeBoiler2.Maximum = 150D;
            this.gaugeBoiler2.Minimum = 0D;
            this.gaugeBoiler2.Name = "gaugeBoiler2";
            this.gaugeBoiler2.Size = new System.Drawing.Size(279, 226);
            this.gaugeBoiler2.Threshold = 105D;
            this.gaugeBoiler2.Units = " psi";
            this.gaugeBoiler2.Value = 92D;
            this.labelBanner2.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner2.AutoSize = false;
            this.labelBanner2.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner2.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner2.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner2.Location = new System.Drawing.Point(14, 268);
            this.labelBanner2.Name = "labelBanner2";
            this.labelBanner2.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.labelBanner2.Size = new System.Drawing.Size(279, 22);
            this.labelBanner2.Text = "";
            this.labelBanner2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner2.Visible = false;
            //
            // Tile 3: Turbine — load
            //
            this.panelTile3.BackColor = System.Drawing.Color.White;
            this.panelTile3.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTile3.Controls.Add(this.labelTile3);
            this.panelTile3.Controls.Add(this.gaugeTurbine);
            this.panelTile3.Controls.Add(this.labelBanner3);
            this.panelTile3.Location = new System.Drawing.Point(684, 136);
            this.panelTile3.Name = "panelTile3";
            this.panelTile3.Size = new System.Drawing.Size(307, 300);
            this.labelTile3.AutoSize = false;
            this.labelTile3.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelTile3.Location = new System.Drawing.Point(14, 8);
            this.labelTile3.Name = "labelTile3";
            this.labelTile3.Size = new System.Drawing.Size(279, 24);
            this.labelTile3.Text = "Turbine — load";
            this.gaugeTurbine.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gaugeTurbine.Caption = "Turbine";
            this.gaugeTurbine.Location = new System.Drawing.Point(14, 34);
            this.gaugeTurbine.Maximum = 100D;
            this.gaugeTurbine.Minimum = 0D;
            this.gaugeTurbine.Name = "gaugeTurbine";
            this.gaugeTurbine.Size = new System.Drawing.Size(279, 226);
            this.gaugeTurbine.Threshold = 90D;
            this.gaugeTurbine.Units = "%";
            this.gaugeTurbine.Value = 64D;
            this.labelBanner3.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner3.AutoSize = false;
            this.labelBanner3.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner3.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner3.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner3.Location = new System.Drawing.Point(14, 268);
            this.labelBanner3.Name = "labelBanner3";
            this.labelBanner3.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.labelBanner3.Size = new System.Drawing.Size(279, 22);
            this.labelBanner3.Text = "";
            this.labelBanner3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner3.Visible = false;
            //
            // Tile 4: Coolant — temp
            //
            this.panelTile4.BackColor = System.Drawing.Color.White;
            this.panelTile4.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTile4.Controls.Add(this.labelTile4);
            this.panelTile4.Controls.Add(this.gaugeCoolant);
            this.panelTile4.Controls.Add(this.labelBanner4);
            this.panelTile4.Location = new System.Drawing.Point(1011, 136);
            this.panelTile4.Name = "panelTile4";
            this.panelTile4.Size = new System.Drawing.Size(307, 300);
            this.labelTile4.AutoSize = false;
            this.labelTile4.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelTile4.Location = new System.Drawing.Point(14, 8);
            this.labelTile4.Name = "labelTile4";
            this.labelTile4.Size = new System.Drawing.Size(279, 24);
            this.labelTile4.Text = "Coolant — temp";
            this.gaugeCoolant.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gaugeCoolant.Caption = "Coolant";
            this.gaugeCoolant.Location = new System.Drawing.Point(14, 34);
            this.gaugeCoolant.Maximum = 140D;
            this.gaugeCoolant.Minimum = 40D;
            this.gaugeCoolant.Name = "gaugeCoolant";
            this.gaugeCoolant.Size = new System.Drawing.Size(279, 226);
            this.gaugeCoolant.Threshold = 115D;
            this.gaugeCoolant.Units = "°F";
            this.gaugeCoolant.Value = 108D;
            this.labelBanner4.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner4.AutoSize = false;
            this.labelBanner4.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner4.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner4.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner4.Location = new System.Drawing.Point(14, 268);
            this.labelBanner4.Name = "labelBanner4";
            this.labelBanner4.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.labelBanner4.Size = new System.Drawing.Size(279, 22);
            this.labelBanner4.Text = "";
            this.labelBanner4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner4.Visible = false;
            //
            // timerStream
            //
            this.timerStream.Interval = 800;
            this.timerStream.Tick += new System.EventHandler(this.timerStream_Tick);
            //
            // EnterprisePage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.panelKpiLines);
            this.Controls.Add(this.panelKpiThroughput);
            this.Controls.Add(this.panelKpiAlerts);
            this.Controls.Add(this.panelKpiUptime);
            this.Controls.Add(this.panelTile1);
            this.Controls.Add(this.panelTile2);
            this.Controls.Add(this.panelTile3);
            this.Controls.Add(this.panelTile4);
            this.Name = "EnterprisePage";
            this.Size = new System.Drawing.Size(1348, 460);
            this.Text = "IntegrationLab — Operations Dashboard";
            this.Load += new System.EventHandler(this.EnterprisePage_Load);
            this.panelKpiLines.ResumeLayout(false);
            this.panelKpiThroughput.ResumeLayout(false);
            this.panelKpiAlerts.ResumeLayout(false);
            this.panelKpiUptime.ResumeLayout(false);
            this.panelTile1.ResumeLayout(false);
            this.panelTile2.ResumeLayout(false);
            this.panelTile3.ResumeLayout(false);
            this.panelTile4.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Panel panelKpiLines;
        private Wisej.Web.Panel stripKpiLines;
        private Wisej.Web.Label labelKpiLinesTitle;
        private Wisej.Web.Label labelKpiLinesValue;
        private Wisej.Web.Panel panelKpiThroughput;
        private Wisej.Web.Panel stripKpiThroughput;
        private Wisej.Web.Label labelKpiThroughputTitle;
        private Wisej.Web.Label labelKpiThroughputValue;
        private Wisej.Web.Panel panelKpiAlerts;
        private Wisej.Web.Panel stripKpiAlerts;
        private Wisej.Web.Label labelKpiAlertsTitle;
        private Wisej.Web.Label labelKpiAlertsValue;
        private Wisej.Web.Panel panelKpiUptime;
        private Wisej.Web.Panel stripKpiUptime;
        private Wisej.Web.Label labelKpiUptimeTitle;
        private Wisej.Web.Label labelKpiUptimeValue;
        private Wisej.Web.Panel panelTile1;
        private Wisej.Web.Label labelTile1;
        private IntegrationLab.Controls.SimpleGaugeControl gaugeBoiler1;
        private Wisej.Web.Label labelBanner1;
        private Wisej.Web.Panel panelTile2;
        private Wisej.Web.Label labelTile2;
        private IntegrationLab.Controls.SimpleGaugeControl gaugeBoiler2;
        private Wisej.Web.Label labelBanner2;
        private Wisej.Web.Panel panelTile3;
        private Wisej.Web.Label labelTile3;
        private IntegrationLab.Controls.SimpleGaugeControl gaugeTurbine;
        private Wisej.Web.Label labelBanner3;
        private Wisej.Web.Panel panelTile4;
        private Wisej.Web.Label labelTile4;
        private IntegrationLab.Controls.SimpleGaugeControl gaugeCoolant;
        private Wisej.Web.Label labelBanner4;
        private Wisej.Web.Timer timerStream;
    }
}
