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
            this.tileDisposed = new Wisej.Web.Panel();
            this.labelDisposedCaption = new Wisej.Web.Label();
            this.labelDisposedValue = new Wisej.Web.Label();
            this.tileLive = new Wisej.Web.Panel();
            this.labelLiveCaption = new Wisej.Web.Label();
            this.labelLiveValue = new Wisej.Web.Label();
            this.panelHeatmap = new Wisej.Web.Panel();
            this.labelHeatmapTitle = new Wisej.Web.Label();
            this.labelHeatStatus = new Wisej.Web.Label();
            this.heatmap = new IntegrationLab.Controls.HeatmapWidget();
            this.labelBanner = new Wisej.Web.Label();
            this.panelGauge = new Wisej.Web.Panel();
            this.labelGaugeTitle = new Wisej.Web.Label();
            this.gauge = new IntegrationLab.Controls.TemperatureGauge();
            this.labelState = new Wisej.Web.Label();
            this.labelScratch = new Wisej.Web.Label();
            this.panelScratch = new Wisej.Web.Panel();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonLive = new Wisej.Web.Button();
            this.buttonPeak = new Wisej.Web.Button();
            this.buttonReload = new Wisej.Web.Button();
            this.buttonCellCount = new Wisej.Web.Button();
            this.buttonLeak = new Wisej.Web.Button();
            this.buttonMissingVendor = new Wisej.Web.Button();
            this.buttonMalformed = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerLeak = new Wisej.Web.Timer(this.components);
            this.tileWidgets.SuspendLayout();
            this.tileRequests.SuspendLayout();
            this.tileDisposed.SuspendLayout();
            this.tileLive.SuspendLayout();
            this.panelHeatmap.SuspendLayout();
            this.panelGauge.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // stats strip: four tiles
            //
            this.tileWidgets.BackColor = System.Drawing.Color.White;
            this.tileWidgets.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.tileWidgets.Controls.Add(this.labelWidgetsCaption);
            this.tileWidgets.Controls.Add(this.labelWidgetsValue);
            this.tileWidgets.Location = new System.Drawing.Point(30, 18);
            this.tileWidgets.Name = "tileWidgets";
            this.tileWidgets.Size = new System.Drawing.Size(310, 64);
            this.labelWidgetsCaption.AutoSize = false;
            this.labelWidgetsCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelWidgetsCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelWidgetsCaption.Location = new System.Drawing.Point(16, 8);
            this.labelWidgetsCaption.Name = "labelWidgetsCaption";
            this.labelWidgetsCaption.Size = new System.Drawing.Size(280, 18);
            this.labelWidgetsCaption.Text = "WIDGETS LIVE";
            this.labelWidgetsValue.AutoSize = false;
            this.labelWidgetsValue.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.labelWidgetsValue.Location = new System.Drawing.Point(16, 26);
            this.labelWidgetsValue.Name = "labelWidgetsValue";
            this.labelWidgetsValue.Size = new System.Drawing.Size(280, 30);
            this.labelWidgetsValue.Text = "0";

            this.tileRequests.BackColor = System.Drawing.Color.White;
            this.tileRequests.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.tileRequests.Controls.Add(this.labelRequestsCaption);
            this.tileRequests.Controls.Add(this.labelRequestsValue);
            this.tileRequests.Location = new System.Drawing.Point(356, 18);
            this.tileRequests.Name = "tileRequests";
            this.tileRequests.Size = new System.Drawing.Size(310, 64);
            this.labelRequestsCaption.AutoSize = false;
            this.labelRequestsCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelRequestsCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelRequestsCaption.Location = new System.Drawing.Point(16, 8);
            this.labelRequestsCaption.Name = "labelRequestsCaption";
            this.labelRequestsCaption.Size = new System.Drawing.Size(280, 18);
            this.labelRequestsCaption.Text = "REQUESTS/MIN  (postback endpoint)";
            this.labelRequestsValue.AutoSize = false;
            this.labelRequestsValue.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.labelRequestsValue.Location = new System.Drawing.Point(16, 26);
            this.labelRequestsValue.Name = "labelRequestsValue";
            this.labelRequestsValue.Size = new System.Drawing.Size(280, 30);
            this.labelRequestsValue.Text = "0";

            this.tileDisposed.BackColor = System.Drawing.Color.White;
            this.tileDisposed.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.tileDisposed.Controls.Add(this.labelDisposedCaption);
            this.tileDisposed.Controls.Add(this.labelDisposedValue);
            this.tileDisposed.Location = new System.Drawing.Point(682, 18);
            this.tileDisposed.Name = "tileDisposed";
            this.tileDisposed.Size = new System.Drawing.Size(310, 64);
            this.labelDisposedCaption.AutoSize = false;
            this.labelDisposedCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelDisposedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelDisposedCaption.Location = new System.Drawing.Point(16, 8);
            this.labelDisposedCaption.Name = "labelDisposedCaption";
            this.labelDisposedCaption.Size = new System.Drawing.Size(280, 18);
            this.labelDisposedCaption.Text = "DISPOSED CLEANLY  (create/dispose test)";
            this.labelDisposedValue.AutoSize = false;
            this.labelDisposedValue.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.labelDisposedValue.Location = new System.Drawing.Point(16, 26);
            this.labelDisposedValue.Name = "labelDisposedValue";
            this.labelDisposedValue.Size = new System.Drawing.Size(280, 30);
            this.labelDisposedValue.Text = "—";

            this.tileLive.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.tileLive.BackColor = System.Drawing.Color.White;
            this.tileLive.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.tileLive.Controls.Add(this.labelLiveCaption);
            this.tileLive.Controls.Add(this.labelLiveValue);
            this.tileLive.Location = new System.Drawing.Point(1008, 18);
            this.tileLive.Name = "tileLive";
            this.tileLive.Size = new System.Drawing.Size(310, 64);
            this.labelLiveCaption.AutoSize = false;
            this.labelLiveCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelLiveCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLiveCaption.Location = new System.Drawing.Point(16, 8);
            this.labelLiveCaption.Name = "labelLiveCaption";
            this.labelLiveCaption.Size = new System.Drawing.Size(280, 18);
            this.labelLiveCaption.Text = "LIVE UPDATES  (StartTask → Call → Update)";
            this.labelLiveValue.AutoSize = false;
            this.labelLiveValue.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelLiveValue.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLiveValue.Location = new System.Drawing.Point(16, 28);
            this.labelLiveValue.Name = "labelLiveValue";
            this.labelLiveValue.Size = new System.Drawing.Size(280, 28);
            this.labelLiveValue.Text = "● stopped";
            //
            // panelHeatmap  (the capstone widget card)
            //
            this.panelHeatmap.BackColor = System.Drawing.Color.White;
            this.panelHeatmap.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelHeatmap.Controls.Add(this.labelHeatmapTitle);
            this.panelHeatmap.Controls.Add(this.labelHeatStatus);
            this.panelHeatmap.Controls.Add(this.heatmap);
            this.panelHeatmap.Controls.Add(this.labelBanner);
            this.panelHeatmap.Location = new System.Drawing.Point(30, 98);
            this.panelHeatmap.Name = "panelHeatmap";
            this.panelHeatmap.Size = new System.Drawing.Size(700, 336);
            //
            // labelHeatmapTitle
            //
            this.labelHeatmapTitle.AutoSize = false;
            this.labelHeatmapTitle.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.labelHeatmapTitle.Location = new System.Drawing.Point(20, 12);
            this.labelHeatmapTitle.Name = "labelHeatmapTitle";
            this.labelHeatmapTitle.Size = new System.Drawing.Size(500, 28);
            this.labelHeatmapTitle.Text = "Line load — 7 days × 24 h (postback data source)";
            //
            // labelHeatStatus
            //
            this.labelHeatStatus.AutoSize = false;
            this.labelHeatStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelHeatStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelHeatStatus.Location = new System.Drawing.Point(520, 14);
            this.labelHeatStatus.Name = "labelHeatStatus";
            this.labelHeatStatus.Size = new System.Drawing.Size(160, 24);
            this.labelHeatStatus.Text = "● idle";
            this.labelHeatStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // heatmap  (Widget host: the VendorHeatmap instance lives inside its container)
            //
            this.heatmap.Days = 7;
            this.heatmap.HighAt = 85D;
            this.heatmap.Hours = 24;
            this.heatmap.Location = new System.Drawing.Point(20, 46);
            this.heatmap.Name = "heatmap";
            this.heatmap.Size = new System.Drawing.Size(660, 236);
            this.heatmap.Title = "VendorHeatmap · load % per hour";
            this.heatmap.WarnAt = 60D;
            //
            // labelBanner
            //
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(230, 240, 251);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.labelBanner.Location = new System.Drawing.Point(20, 290);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(660, 36);
            this.labelBanner.Text = "";
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // panelGauge  (a second integrated widget fed by the same background task)
            //
            this.panelGauge.BackColor = System.Drawing.Color.White;
            this.panelGauge.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGauge.Controls.Add(this.labelGaugeTitle);
            this.panelGauge.Controls.Add(this.gauge);
            this.panelGauge.Controls.Add(this.labelState);
            this.panelGauge.Controls.Add(this.labelScratch);
            this.panelGauge.Controls.Add(this.panelScratch);
            this.panelGauge.Location = new System.Drawing.Point(30, 450);
            this.panelGauge.Name = "panelGauge";
            this.panelGauge.Size = new System.Drawing.Size(700, 180);
            //
            // labelGaugeTitle
            //
            this.labelGaugeTitle.AutoSize = false;
            this.labelGaugeTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.labelGaugeTitle.Location = new System.Drawing.Point(20, 10);
            this.labelGaugeTitle.Name = "labelGaugeTitle";
            this.labelGaugeTitle.Size = new System.Drawing.Size(460, 26);
            this.labelGaugeTitle.Text = "Boiler 3 — live temperature (Module 1 wrapper, same background task)";
            //
            // gauge
            //
            this.gauge.Label = "VendorGauge";
            this.gauge.Location = new System.Drawing.Point(20, 38);
            this.gauge.Maximum = 120D;
            this.gauge.Minimum = 40D;
            this.gauge.Name = "gauge";
            this.gauge.Size = new System.Drawing.Size(180, 132);
            this.gauge.Threshold = 100D;
            this.gauge.Value = 72D;
            this.gauge.WarnAt = 85D;
            //
            // labelState  (what the server owns right now)
            //
            this.labelState.AutoSize = false;
            this.labelState.Font = new System.Drawing.Font("monospace", 8F);
            this.labelState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelState.Location = new System.Drawing.Point(214, 40);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(270, 128);
            this.labelState.Text = "";
            this.labelState.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelScratch + panelScratch  (create/dispose target and failure-simulation host)
            //
            this.labelScratch.AutoSize = false;
            this.labelScratch.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelScratch.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelScratch.Location = new System.Drawing.Point(500, 18);
            this.labelScratch.Name = "labelScratch";
            this.labelScratch.Size = new System.Drawing.Size(180, 18);
            this.labelScratch.Text = "SCRATCH · create/dispose ×25";
            this.panelScratch.BackColor = System.Drawing.Color.FromArgb(246, 248, 251);
            this.panelScratch.BorderStyle = Wisej.Web.BorderStyle.Dashed;
            this.panelScratch.Location = new System.Drawing.Point(500, 38);
            this.panelScratch.Name = "panelScratch";
            this.panelScratch.Size = new System.Drawing.Size(180, 132);
            //
            // panelTrace  (Server ⇄ Client live message trace)
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(746, 98);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(572, 532);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(532, 30);
            this.labelTraceTitle.Text = "Server ⇄ Client  ·  live message trace";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(532, 428);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 490);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(532, 26);
            this.labelTraceFooter.Text = "state out: compact JSON · calls: Call/CallAsync · data: postback · → .NET→JS   ← JS→.NET   • server";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonLive);
            this.panelActions.Controls.Add(this.buttonPeak);
            this.panelActions.Controls.Add(this.buttonReload);
            this.panelActions.Controls.Add(this.buttonCellCount);
            this.panelActions.Controls.Add(this.buttonLeak);
            this.panelActions.Controls.Add(this.buttonMissingVendor);
            this.panelActions.Controls.Add(this.buttonMalformed);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 646);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // buttonLive (progress path: background task)
            //
            this.buttonLive.Location = new System.Drawing.Point(0, 4);
            this.buttonLive.Name = "buttonLive";
            this.buttonLive.Size = new System.Drawing.Size(160, 36);
            this.buttonLive.Text = "▶ Start live updates";
            this.buttonLive.ToolTipText = "Application.StartTask → heatmap.Call(\"setCells\") + gauge.Value → Application.Update(page). Bounded: 1500 ms, 40 pushes.";
            this.buttonLive.Click += new System.EventHandler(this.buttonLive_Click);
            //
            // buttons: server calls (success path)
            //
            this.buttonPeak.Location = new System.Drawing.Point(168, 4);
            this.buttonPeak.Name = "buttonPeak";
            this.buttonPeak.Size = new System.Drawing.Size(120, 36);
            this.buttonPeak.Text = "Highlight peak";
            this.buttonPeak.ToolTipText = "The server finds the max cell in its copy of the data and calls highlight(day, hour) on the client.";
            this.buttonPeak.Click += new System.EventHandler(this.buttonPeak_Click);
            this.buttonReload.Location = new System.Drawing.Point(296, 4);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(110, 36);
            this.buttonReload.Text = "Reload data";
            this.buttonReload.ToolTipText = "Call(\"reload\"): the client fetches the postback endpoint again (recovery path).";
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            this.buttonCellCount.Location = new System.Drawing.Point(414, 4);
            this.buttonCellCount.Name = "buttonCellCount";
            this.buttonCellCount.Size = new System.Drawing.Size(100, 36);
            this.buttonCellCount.Text = "Cell count";
            this.buttonCellCount.ToolTipText = "await CallAsync(\"getCellCount\"): a round trip with a return value.";
            this.buttonCellCount.Click += new System.EventHandler(this.buttonCellCount_Click);
            //
            // buttonLeak (create/dispose test)
            //
            this.buttonLeak.Location = new System.Drawing.Point(522, 4);
            this.buttonLeak.Name = "buttonLeak";
            this.buttonLeak.Size = new System.Drawing.Size(150, 36);
            this.buttonLeak.Text = "Create/dispose ×25";
            this.buttonLeak.ToolTipText = "Creates and disposes a HeatmapWidget 25 times, then reads window.__integrationLabDisposed with EvalAsync.";
            this.buttonLeak.Click += new System.EventHandler(this.buttonLeak_Click);
            //
            // failure paths
            //
            this.buttonMissingVendor.Location = new System.Drawing.Point(680, 4);
            this.buttonMissingVendor.Name = "buttonMissingVendor";
            this.buttonMissingVendor.Size = new System.Drawing.Size(160, 36);
            this.buttonMissingVendor.Text = "Simulate missing vendor";
            this.buttonMissingVendor.ToolTipText = "A second HeatmapWidget without the vendor-heatmap package: the guard clause must throw a clear message.";
            this.buttonMissingVendor.Click += new System.EventHandler(this.buttonMissingVendor_Click);
            this.buttonMalformed.Location = new System.Drawing.Point(848, 4);
            this.buttonMalformed.Name = "buttonMalformed";
            this.buttonMalformed.Size = new System.Drawing.Size(130, 36);
            this.buttonMalformed.Text = "Malformed data";
            this.buttonMalformed.ToolTipText = "The endpoint answers action=corrupt with invalid JSON: the vendor throws, the adapter reports one error event.";
            this.buttonMalformed.Click += new System.EventHandler(this.buttonMalformed_Click);
            //
            // buttonClear
            //
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(1178, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(110, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timerLeak  (a Component with no visual surface: paces the create/dispose test)
            //
            this.timerLeak.Interval = 200;
            this.timerLeak.Tick += new System.EventHandler(this.timerLeak_Tick);
            //
            // OperationsPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.tileWidgets);
            this.Controls.Add(this.tileRequests);
            this.Controls.Add(this.tileDisposed);
            this.Controls.Add(this.tileLive);
            this.Controls.Add(this.panelHeatmap);
            this.Controls.Add(this.panelGauge);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "OperationsPage";
            this.Size = new System.Drawing.Size(1348, 700);
            this.Text = "IntegrationLab — Operations Dashboard";
            this.Load += new System.EventHandler(this.OperationsPage_Load);
            this.tileWidgets.ResumeLayout(false);
            this.tileRequests.ResumeLayout(false);
            this.tileDisposed.ResumeLayout(false);
            this.tileLive.ResumeLayout(false);
            this.panelHeatmap.ResumeLayout(false);
            this.panelGauge.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
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
        private Wisej.Web.Panel tileDisposed;
        private Wisej.Web.Label labelDisposedCaption;
        private Wisej.Web.Label labelDisposedValue;
        private Wisej.Web.Panel tileLive;
        private Wisej.Web.Label labelLiveCaption;
        private Wisej.Web.Label labelLiveValue;
        private Wisej.Web.Panel panelHeatmap;
        private Wisej.Web.Label labelHeatmapTitle;
        private Wisej.Web.Label labelHeatStatus;
        private IntegrationLab.Controls.HeatmapWidget heatmap;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Panel panelGauge;
        private Wisej.Web.Label labelGaugeTitle;
        private IntegrationLab.Controls.TemperatureGauge gauge;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Label labelScratch;
        private Wisej.Web.Panel panelScratch;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonLive;
        private Wisej.Web.Button buttonPeak;
        private Wisej.Web.Button buttonReload;
        private Wisej.Web.Button buttonCellCount;
        private Wisej.Web.Button buttonLeak;
        private Wisej.Web.Button buttonMissingVendor;
        private Wisej.Web.Button buttonMalformed;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerLeak;
    }
}
