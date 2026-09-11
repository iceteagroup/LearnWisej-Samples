namespace IntegrationLab
{
    partial class OperationsPage
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
            this.tileWidgets = new Wisej.Web.Panel();
            this.labelWidgetsCaption = new Wisej.Web.Label();
            this.labelWidgetsValue = new Wisej.Web.Label();
            this.tileRequests = new Wisej.Web.Panel();
            this.labelRequestsCaption = new Wisej.Web.Label();
            this.labelRequestsValue = new Wisej.Web.Label();
            this.tileErrors = new Wisej.Web.Panel();
            this.labelErrorsCaption = new Wisej.Web.Label();
            this.labelErrorsValue = new Wisej.Web.Label();
            this.tileDisposed = new Wisej.Web.Panel();
            this.labelDisposedCaption = new Wisej.Web.Label();
            this.labelDisposedValue = new Wisej.Web.Label();
            this.panelGauge = new Wisej.Web.Panel();
            this.labelGaugeTitle = new Wisej.Web.Label();
            this.gauge = new IntegrationLab.Controls.TemperatureGauge();
            this.panelLeak = new Wisej.Web.Panel();
            this.labelLeakTitle = new Wisej.Web.Label();
            this.panelScratch = new Wisej.Web.Panel();
            this.panelHeatmap = new Wisej.Web.Panel();
            this.labelHeatmapTitle = new Wisej.Web.Label();
            this.heatmap = new IntegrationLab.Controls.HeatmapWidget();
            this.labelBanner = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonLive = new Wisej.Web.Button();
            this.buttonPeak = new Wisej.Web.Button();
            this.buttonReload = new Wisej.Web.Button();
            this.buttonCellCount = new Wisej.Web.Button();
            this.buttonLeak = new Wisej.Web.Button();
            this.timerLeak = new Wisej.Web.Timer(this.components);
            this.tileWidgets.SuspendLayout();
            this.tileRequests.SuspendLayout();
            this.tileErrors.SuspendLayout();
            this.tileDisposed.SuspendLayout();
            this.panelGauge.SuspendLayout();
            this.panelLeak.SuspendLayout();
            this.panelHeatmap.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // tileWidgets
            //
            this.tileWidgets.BackColor = System.Drawing.Color.White;
            this.tileWidgets.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.tileWidgets.Controls.Add(this.labelWidgetsCaption);
            this.tileWidgets.Controls.Add(this.labelWidgetsValue);
            this.tileWidgets.Location = new System.Drawing.Point(30, 18);
            this.tileWidgets.Name = "tileWidgets";
            this.tileWidgets.Size = new System.Drawing.Size(310, 64);
            //
            // labelWidgetsCaption
            //
            this.labelWidgetsCaption.AutoSize = false;
            this.labelWidgetsCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelWidgetsCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelWidgetsCaption.Location = new System.Drawing.Point(16, 8);
            this.labelWidgetsCaption.Name = "labelWidgetsCaption";
            this.labelWidgetsCaption.Size = new System.Drawing.Size(280, 18);
            this.labelWidgetsCaption.Text = "WIDGETS LIVE";
            //
            // labelWidgetsValue
            //
            this.labelWidgetsValue.AutoSize = false;
            this.labelWidgetsValue.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.labelWidgetsValue.Location = new System.Drawing.Point(16, 26);
            this.labelWidgetsValue.Name = "labelWidgetsValue";
            this.labelWidgetsValue.Size = new System.Drawing.Size(280, 30);
            this.labelWidgetsValue.Text = "0";
            //
            // tileRequests
            //
            this.tileRequests.BackColor = System.Drawing.Color.White;
            this.tileRequests.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.tileRequests.Controls.Add(this.labelRequestsCaption);
            this.tileRequests.Controls.Add(this.labelRequestsValue);
            this.tileRequests.Location = new System.Drawing.Point(356, 18);
            this.tileRequests.Name = "tileRequests";
            this.tileRequests.Size = new System.Drawing.Size(310, 64);
            //
            // labelRequestsCaption
            //
            this.labelRequestsCaption.AutoSize = false;
            this.labelRequestsCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelRequestsCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelRequestsCaption.Location = new System.Drawing.Point(16, 8);
            this.labelRequestsCaption.Name = "labelRequestsCaption";
            this.labelRequestsCaption.Size = new System.Drawing.Size(280, 18);
            this.labelRequestsCaption.Text = "REQUESTS/MIN";
            //
            // labelRequestsValue
            //
            this.labelRequestsValue.AutoSize = false;
            this.labelRequestsValue.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.labelRequestsValue.Location = new System.Drawing.Point(16, 26);
            this.labelRequestsValue.Name = "labelRequestsValue";
            this.labelRequestsValue.Size = new System.Drawing.Size(280, 30);
            this.labelRequestsValue.Text = "0";
            //
            // tileErrors
            //
            this.tileErrors.BackColor = System.Drawing.Color.White;
            this.tileErrors.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.tileErrors.Controls.Add(this.labelErrorsCaption);
            this.tileErrors.Controls.Add(this.labelErrorsValue);
            this.tileErrors.Location = new System.Drawing.Point(682, 18);
            this.tileErrors.Name = "tileErrors";
            this.tileErrors.Size = new System.Drawing.Size(310, 64);
            //
            // labelErrorsCaption
            //
            this.labelErrorsCaption.AutoSize = false;
            this.labelErrorsCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelErrorsCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelErrorsCaption.Location = new System.Drawing.Point(16, 8);
            this.labelErrorsCaption.Name = "labelErrorsCaption";
            this.labelErrorsCaption.Size = new System.Drawing.Size(280, 18);
            this.labelErrorsCaption.Text = "ERRORS";
            //
            // labelErrorsValue
            //
            this.labelErrorsValue.AutoSize = false;
            this.labelErrorsValue.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.labelErrorsValue.Location = new System.Drawing.Point(16, 26);
            this.labelErrorsValue.Name = "labelErrorsValue";
            this.labelErrorsValue.Size = new System.Drawing.Size(280, 30);
            this.labelErrorsValue.Text = "0";
            //
            // tileDisposed
            //
            this.tileDisposed.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.tileDisposed.BackColor = System.Drawing.Color.White;
            this.tileDisposed.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.tileDisposed.Controls.Add(this.labelDisposedCaption);
            this.tileDisposed.Controls.Add(this.labelDisposedValue);
            this.tileDisposed.Location = new System.Drawing.Point(1008, 18);
            this.tileDisposed.Name = "tileDisposed";
            this.tileDisposed.Size = new System.Drawing.Size(310, 64);
            //
            // labelDisposedCaption
            //
            this.labelDisposedCaption.AutoSize = false;
            this.labelDisposedCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelDisposedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelDisposedCaption.Location = new System.Drawing.Point(16, 8);
            this.labelDisposedCaption.Name = "labelDisposedCaption";
            this.labelDisposedCaption.Size = new System.Drawing.Size(280, 18);
            this.labelDisposedCaption.Text = "DISPOSED CLEANLY";
            //
            // labelDisposedValue
            //
            this.labelDisposedValue.AutoSize = false;
            this.labelDisposedValue.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.labelDisposedValue.Location = new System.Drawing.Point(16, 26);
            this.labelDisposedValue.Name = "labelDisposedValue";
            this.labelDisposedValue.Size = new System.Drawing.Size(280, 30);
            this.labelDisposedValue.Text = "—";
            //
            // panelGauge
            //
            this.panelGauge.BackColor = System.Drawing.Color.White;
            this.panelGauge.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGauge.Controls.Add(this.labelGaugeTitle);
            this.panelGauge.Controls.Add(this.gauge);
            this.panelGauge.Location = new System.Drawing.Point(30, 98);
            this.panelGauge.Name = "panelGauge";
            this.panelGauge.Size = new System.Drawing.Size(320, 260);
            //
            // labelGaugeTitle
            //
            this.labelGaugeTitle.AutoSize = false;
            this.labelGaugeTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.labelGaugeTitle.Location = new System.Drawing.Point(20, 12);
            this.labelGaugeTitle.Name = "labelGaugeTitle";
            this.labelGaugeTitle.Size = new System.Drawing.Size(280, 26);
            this.labelGaugeTitle.Text = "Boiler 3 — live temperature";
            //
            // gauge
            //
            this.gauge.Label = "VendorGauge";
            this.gauge.Location = new System.Drawing.Point(20, 44);
            this.gauge.Maximum = 120D;
            this.gauge.Minimum = 40D;
            this.gauge.Name = "gauge";
            this.gauge.Size = new System.Drawing.Size(280, 200);
            this.gauge.Threshold = 100D;
            this.gauge.Value = 72D;
            this.gauge.WarnAt = 85D;
            //
            // panelLeak
            //
            this.panelLeak.BackColor = System.Drawing.Color.White;
            this.panelLeak.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLeak.Controls.Add(this.labelLeakTitle);
            this.panelLeak.Controls.Add(this.panelScratch);
            this.panelLeak.Location = new System.Drawing.Point(30, 374);
            this.panelLeak.Name = "panelLeak";
            this.panelLeak.Size = new System.Drawing.Size(320, 256);
            //
            // labelLeakTitle
            //
            this.labelLeakTitle.AutoSize = false;
            this.labelLeakTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.labelLeakTitle.Location = new System.Drawing.Point(20, 12);
            this.labelLeakTitle.Name = "labelLeakTitle";
            this.labelLeakTitle.Size = new System.Drawing.Size(280, 26);
            this.labelLeakTitle.Text = "Create/dispose test";
            //
            // panelScratch
            //
            this.panelScratch.BackColor = System.Drawing.Color.FromArgb(246, 248, 251);
            this.panelScratch.BorderStyle = Wisej.Web.BorderStyle.Dashed;
            this.panelScratch.Location = new System.Drawing.Point(20, 44);
            this.panelScratch.Name = "panelScratch";
            this.panelScratch.Size = new System.Drawing.Size(280, 196);
            //
            // panelHeatmap
            //
            this.panelHeatmap.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelHeatmap.BackColor = System.Drawing.Color.White;
            this.panelHeatmap.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelHeatmap.Controls.Add(this.labelHeatmapTitle);
            this.panelHeatmap.Controls.Add(this.heatmap);
            this.panelHeatmap.Controls.Add(this.labelBanner);
            this.panelHeatmap.Location = new System.Drawing.Point(366, 98);
            this.panelHeatmap.Name = "panelHeatmap";
            this.panelHeatmap.Size = new System.Drawing.Size(952, 532);
            //
            // labelHeatmapTitle
            //
            this.labelHeatmapTitle.AutoSize = false;
            this.labelHeatmapTitle.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.labelHeatmapTitle.Location = new System.Drawing.Point(20, 12);
            this.labelHeatmapTitle.Name = "labelHeatmapTitle";
            this.labelHeatmapTitle.Size = new System.Drawing.Size(600, 28);
            this.labelHeatmapTitle.Text = "Line load — 7 days × 24 h (postback data source)";
            //
            // heatmap
            //
            this.heatmap.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.heatmap.Days = 7;
            this.heatmap.HighAt = 85D;
            this.heatmap.Hours = 24;
            this.heatmap.Location = new System.Drawing.Point(20, 46);
            this.heatmap.Name = "heatmap";
            this.heatmap.Size = new System.Drawing.Size(912, 420);
            this.heatmap.Title = "VendorHeatmap · load % per hour";
            this.heatmap.WarnAt = 60D;
            //
            // labelBanner
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(230, 240, 251);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.labelBanner.Location = new System.Drawing.Point(20, 476);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(912, 40);
            this.labelBanner.Text = "";
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonLive);
            this.panelActions.Controls.Add(this.buttonPeak);
            this.panelActions.Controls.Add(this.buttonReload);
            this.panelActions.Controls.Add(this.buttonCellCount);
            this.panelActions.Controls.Add(this.buttonLeak);
            this.panelActions.Location = new System.Drawing.Point(30, 646);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // buttonLive
            //
            this.buttonLive.Location = new System.Drawing.Point(0, 4);
            this.buttonLive.Name = "buttonLive";
            this.buttonLive.Size = new System.Drawing.Size(160, 36);
            this.buttonLive.Text = "▶ Start live updates";
            this.buttonLive.Click += new System.EventHandler(this.buttonLive_Click);
            //
            // buttonPeak
            //
            this.buttonPeak.Location = new System.Drawing.Point(168, 4);
            this.buttonPeak.Name = "buttonPeak";
            this.buttonPeak.Size = new System.Drawing.Size(120, 36);
            this.buttonPeak.Text = "Highlight peak";
            this.buttonPeak.Click += new System.EventHandler(this.buttonPeak_Click);
            //
            // buttonReload
            //
            this.buttonReload.Location = new System.Drawing.Point(296, 4);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(110, 36);
            this.buttonReload.Text = "Reload data";
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            //
            // buttonCellCount
            //
            this.buttonCellCount.Location = new System.Drawing.Point(414, 4);
            this.buttonCellCount.Name = "buttonCellCount";
            this.buttonCellCount.Size = new System.Drawing.Size(100, 36);
            this.buttonCellCount.Text = "Cell count";
            this.buttonCellCount.Click += new System.EventHandler(this.buttonCellCount_Click);
            //
            // buttonLeak
            //
            this.buttonLeak.Location = new System.Drawing.Point(522, 4);
            this.buttonLeak.Name = "buttonLeak";
            this.buttonLeak.Size = new System.Drawing.Size(150, 36);
            this.buttonLeak.Text = "Create/dispose ×25";
            this.buttonLeak.Click += new System.EventHandler(this.buttonLeak_Click);
            //
            // timerLeak
            //
            this.timerLeak.Interval = 200;
            this.timerLeak.Tick += new System.EventHandler(this.timerLeak_Tick);
            //
            // OperationsPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.tileWidgets);
            this.Controls.Add(this.tileRequests);
            this.Controls.Add(this.tileErrors);
            this.Controls.Add(this.tileDisposed);
            this.Controls.Add(this.panelGauge);
            this.Controls.Add(this.panelLeak);
            this.Controls.Add(this.panelHeatmap);
            this.Controls.Add(this.panelActions);
            this.Name = "OperationsPage";
            this.Size = new System.Drawing.Size(1348, 700);
            this.Text = "IntegrationLab — Operations Dashboard";
            this.Load += new System.EventHandler(this.OperationsPage_Load);
            this.tileWidgets.ResumeLayout(false);
            this.tileRequests.ResumeLayout(false);
            this.tileErrors.ResumeLayout(false);
            this.tileDisposed.ResumeLayout(false);
            this.panelGauge.ResumeLayout(false);
            this.panelLeak.ResumeLayout(false);
            this.panelHeatmap.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel tileWidgets;
        private Wisej.Web.Label labelWidgetsCaption;
        private Wisej.Web.Label labelWidgetsValue;
        private Wisej.Web.Panel tileRequests;
        private Wisej.Web.Label labelRequestsCaption;
        private Wisej.Web.Label labelRequestsValue;
        private Wisej.Web.Panel tileErrors;
        private Wisej.Web.Label labelErrorsCaption;
        private Wisej.Web.Label labelErrorsValue;
        private Wisej.Web.Panel tileDisposed;
        private Wisej.Web.Label labelDisposedCaption;
        private Wisej.Web.Label labelDisposedValue;
        private Wisej.Web.Panel panelGauge;
        private Wisej.Web.Label labelGaugeTitle;
        private IntegrationLab.Controls.TemperatureGauge gauge;
        private Wisej.Web.Panel panelLeak;
        private Wisej.Web.Label labelLeakTitle;
        private Wisej.Web.Panel panelScratch;
        private Wisej.Web.Panel panelHeatmap;
        private Wisej.Web.Label labelHeatmapTitle;
        private IntegrationLab.Controls.HeatmapWidget heatmap;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonLive;
        private Wisej.Web.Button buttonPeak;
        private Wisej.Web.Button buttonReload;
        private Wisej.Web.Button buttonCellCount;
        private Wisej.Web.Button buttonLeak;
        private Wisej.Web.Timer timerLeak;
    }
}
