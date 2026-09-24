namespace VisualOperationsStudio
{
    partial class VisualOperationsPage
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
            this.pnlAppBar = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.lblAppSuffix = new Wisej.Web.Label();
            this.glyphs = new VisualOperationsStudio.Controls.WindowGlyphs();
            this.pnlBody = new Wisej.Web.Panel();
            this.cardGauge = new Wisej.Web.Panel();
            this.lblGaugeHeader = new Wisej.Web.Label();
            this.pnlGaugeGap = new Wisej.Web.Panel();
            this.gaugeCpu = new VisualOperationsStudio.Controls.TelemetryGauge();
            this.lblAccessibleReading = new Wisej.Web.Label();
            this.pnlGaugeFiller = new Wisej.Web.Panel();
            this.pnlGapLeft = new Wisej.Web.Panel();
            this.cardGrid = new Wisej.Web.Panel();
            this.lblGridHeader = new Wisej.Web.Label();
            this.gridAssets = new Wisej.Web.DataGridView();
            this.colAsset = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colHealth = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTrend = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colState = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblAccessibleTable = new Wisej.Web.Label();
            this.pnlGapRight = new Wisej.Web.Panel();
            this.cardCanvas = new Wisej.Web.Panel();
            this.lblCanvasHeader = new Wisej.Web.Label();
            this.canvasTopology = new Wisej.Web.Canvas();
            this.pnlCanvasFiller = new Wisej.Web.Panel();
            this.pnlCanvasActions = new Wisej.Web.Panel();
            this.btnExport = new Wisej.Web.Button();
            this.btnResetView = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Location = new System.Drawing.Point(18, 0);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(188, 42);
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAppTitle.Text = "Visual Operations Studio";
            //
            // lblAppSuffix
            //
            this.lblAppSuffix.AutoSize = false;
            this.lblAppSuffix.CssStyle = "opacity:.85";
            this.lblAppSuffix.Font = new System.Drawing.Font("default", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblAppSuffix.ForeColor = System.Drawing.Color.White;
            this.lblAppSuffix.Location = new System.Drawing.Point(209, 0);
            this.lblAppSuffix.Name = "lblAppSuffix";
            this.lblAppSuffix.Size = new System.Drawing.Size(160, 42);
            this.lblAppSuffix.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAppSuffix.Text = "· Capstone";
            //
            // glyphs
            //
            this.glyphs.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.glyphs.Dock = Wisej.Web.DockStyle.Right;
            this.glyphs.Inset = 18;
            this.glyphs.Name = "glyphs";
            this.glyphs.Size = new System.Drawing.Size(89, 42);
            //
            // pnlAppBar
            //
            this.pnlAppBar.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Size = new System.Drawing.Size(1400, 42);
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            this.pnlAppBar.Controls.Add(this.lblAppSuffix);
            this.pnlAppBar.Controls.Add(this.glyphs);
            //
            // lblGaugeHeader
            //
            this.lblGaugeHeader.AutoSize = false;
            this.lblGaugeHeader.CssStyle = "letter-spacing:.04em";
            this.lblGaugeHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblGaugeHeader.Font = new System.Drawing.Font("default", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblGaugeHeader.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblGaugeHeader.Name = "lblGaugeHeader";
            this.lblGaugeHeader.Size = new System.Drawing.Size(218, 18);
            this.lblGaugeHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblGaugeHeader.Text = "CONTROL PAINT · SERVER";
            //
            // pnlGaugeGap
            //
            this.pnlGaugeGap.Dock = Wisej.Web.DockStyle.Top;
            this.pnlGaugeGap.Name = "pnlGaugeGap";
            this.pnlGaugeGap.Size = new System.Drawing.Size(218, 10);
            //
            // gaugeCpu
            //
            this.gaugeCpu.Dock = Wisej.Web.DockStyle.Top;
            this.gaugeCpu.Name = "gaugeCpu";
            this.gaugeCpu.Size = new System.Drawing.Size(218, 210);
            //
            // lblAccessibleReading
            //
            this.lblAccessibleReading.AllowHtml = true;
            this.lblAccessibleReading.AutoSize = false;
            this.lblAccessibleReading.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.lblAccessibleReading.CssStyle = "border:1px solid #e4eaf1;border-radius:8px;white-space:normal;line-height:1.4";
            this.lblAccessibleReading.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblAccessibleReading.Font = new System.Drawing.Font("default", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblAccessibleReading.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.lblAccessibleReading.Name = "lblAccessibleReading";
            this.lblAccessibleReading.Padding = new Wisej.Web.Padding(10, 7, 10, 7);
            this.lblAccessibleReading.Size = new System.Drawing.Size(218, 58);
            this.lblAccessibleReading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlGaugeFiller
            //
            this.pnlGaugeFiller.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlGaugeFiller.Name = "pnlGaugeFiller";
            //
            // cardGauge
            //
            this.cardGauge.BackColor = System.Drawing.Color.White;
            this.cardGauge.CssStyle = "border:1px solid #e4eaf1;border-radius:12px";
            this.cardGauge.Dock = Wisej.Web.DockStyle.Left;
            this.cardGauge.Name = "cardGauge";
            this.cardGauge.Padding = new Wisej.Web.Padding(16, 14, 16, 14);
            this.cardGauge.Size = new System.Drawing.Size(250, 600);
            this.cardGauge.Controls.Add(this.pnlGaugeFiller);
            this.cardGauge.Controls.Add(this.lblAccessibleReading);
            this.cardGauge.Controls.Add(this.gaugeCpu);
            this.cardGauge.Controls.Add(this.pnlGaugeGap);
            this.cardGauge.Controls.Add(this.lblGaugeHeader);
            //
            // pnlGapLeft
            //
            this.pnlGapLeft.Dock = Wisej.Web.DockStyle.Left;
            this.pnlGapLeft.Name = "pnlGapLeft";
            this.pnlGapLeft.Size = new System.Drawing.Size(16, 600);
            //
            // lblGridHeader
            //
            this.lblGridHeader.AutoSize = false;
            this.lblGridHeader.CssStyle = "letter-spacing:.04em;border-bottom:1px solid #e4eaf1";
            this.lblGridHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblGridHeader.Font = new System.Drawing.Font("default", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblGridHeader.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblGridHeader.Name = "lblGridHeader";
            this.lblGridHeader.Padding = new Wisej.Web.Padding(14, 0, 14, 0);
            this.lblGridHeader.Size = new System.Drawing.Size(500, 36);
            this.lblGridHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblGridHeader.Text = "CELLPAINT · USERPAINT COLUMNS";
            //
            // colAsset
            //
            this.colAsset.DataPropertyName = "Asset";
            this.colAsset.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.colAsset.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.colAsset.HeaderText = "Asset";
            this.colAsset.Name = "colAsset";
            this.colAsset.Width = 104;
            //
            // colHealth
            //
            this.colHealth.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colHealth.HeaderText = "Health";
            this.colHealth.Name = "colHealth";
            //
            // colTrend
            //
            this.colTrend.HeaderText = "Trend";
            this.colTrend.Name = "colTrend";
            this.colTrend.Width = 112;
            //
            // colState
            //
            this.colState.AllowHtml = true;
            this.colState.DataPropertyName = "StateHtml";
            this.colState.HeaderText = "State";
            this.colState.Name = "colState";
            this.colState.Width = 92;
            //
            // gridAssets
            //
            this.gridAssets.AllowUserToAddRows = false;
            this.gridAssets.AllowUserToDeleteRows = false;
            this.gridAssets.AllowUserToOrderColumns = false;
            this.gridAssets.AutoGenerateColumns = false;
            this.gridAssets.BorderStyle = Wisej.Web.BorderStyle.None;
            this.gridAssets.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.gridAssets.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("default", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.gridAssets.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.gridAssets.ColumnHeadersHeight = 26;
            this.gridAssets.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.gridAssets.DefaultCellStyle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.gridAssets.Dock = Wisej.Web.DockStyle.Fill;
            this.gridAssets.Name = "gridAssets";
            this.gridAssets.ReadOnly = true;
            this.gridAssets.RowHeadersVisible = false;
            this.gridAssets.RowTemplate.Height = 42;
            this.gridAssets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridAssets.Columns.Add(this.colAsset);
            this.gridAssets.Columns.Add(this.colHealth);
            this.gridAssets.Columns.Add(this.colTrend);
            this.gridAssets.Columns.Add(this.colState);
            //
            // lblAccessibleTable
            //
            this.lblAccessibleTable.AllowHtml = true;
            this.lblAccessibleTable.AutoSize = false;
            this.lblAccessibleTable.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.lblAccessibleTable.CssStyle = "border:1px dashed #9db9db;border-radius:8px;white-space:normal;line-height:1.45";
            this.lblAccessibleTable.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblAccessibleTable.Font = new System.Drawing.Font("default", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblAccessibleTable.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.lblAccessibleTable.Margin = new Wisej.Web.Padding(12, 10, 12, 10);
            this.lblAccessibleTable.Name = "lblAccessibleTable";
            this.lblAccessibleTable.Padding = new Wisej.Web.Padding(12, 9, 12, 9);
            this.lblAccessibleTable.Size = new System.Drawing.Size(500, 46);
            this.lblAccessibleTable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cardGrid
            //
            this.cardGrid.BackColor = System.Drawing.Color.White;
            this.cardGrid.CssStyle = "border:1px solid #e4eaf1;border-radius:12px;overflow:hidden";
            this.cardGrid.Dock = Wisej.Web.DockStyle.Fill;
            this.cardGrid.Name = "cardGrid";
            this.cardGrid.Controls.Add(this.gridAssets);
            this.cardGrid.Controls.Add(this.lblAccessibleTable);
            this.cardGrid.Controls.Add(this.lblGridHeader);
            //
            // pnlGapRight
            //
            this.pnlGapRight.Dock = Wisej.Web.DockStyle.Right;
            this.pnlGapRight.Name = "pnlGapRight";
            this.pnlGapRight.Size = new System.Drawing.Size(16, 600);
            //
            // lblCanvasHeader
            //
            this.lblCanvasHeader.AutoSize = false;
            this.lblCanvasHeader.CssStyle = "letter-spacing:.04em;border-bottom:1px solid #e4eaf1";
            this.lblCanvasHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblCanvasHeader.Font = new System.Drawing.Font("default", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblCanvasHeader.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCanvasHeader.Name = "lblCanvasHeader";
            this.lblCanvasHeader.Padding = new Wisej.Web.Padding(14, 0, 14, 0);
            this.lblCanvasHeader.Size = new System.Drawing.Size(330, 36);
            this.lblCanvasHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCanvasHeader.Text = "WISEJ.WEB.CANVAS · BROWSER";
            //
            // canvasTopology
            //
            this.canvasTopology.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.canvasTopology.Dock = Wisej.Web.DockStyle.Top;
            this.canvasTopology.LiveUpdate = false;
            this.canvasTopology.Name = "canvasTopology";
            this.canvasTopology.Size = new System.Drawing.Size(330, 258);
            this.canvasTopology.Redraw += this.canvasTopology_Redraw;
            this.canvasTopology.MouseDown += this.canvasTopology_MouseDown;
            this.canvasTopology.MouseMove += this.canvasTopology_MouseMove;
            this.canvasTopology.MouseUp += this.canvasTopology_MouseUp;
            //
            // pnlCanvasFiller
            //
            this.pnlCanvasFiller.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlCanvasFiller.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlCanvasFiller.Name = "pnlCanvasFiller";
            //
            // btnExport
            //
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.btnExport.CssStyle = "border-radius:7px;border:none";
            this.btnExport.Font = new System.Drawing.Font("default", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(12, 9);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(96, 28);
            this.btnExport.TabIndex = 0;
            this.btnExport.Text = "Export PNG";
            this.btnExport.Click += this.btnExport_Click;
            //
            // btnResetView
            //
            this.btnResetView.BackColor = System.Drawing.Color.White;
            this.btnResetView.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.btnResetView.Font = new System.Drawing.Font("default", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnResetView.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.btnResetView.Location = new System.Drawing.Point(116, 9);
            this.btnResetView.Name = "btnResetView";
            this.btnResetView.Size = new System.Drawing.Size(92, 28);
            this.btnResetView.TabIndex = 1;
            this.btnResetView.Text = "Reset view";
            this.btnResetView.Click += this.btnResetView_Click;
            //
            // pnlCanvasActions
            //
            this.pnlCanvasActions.BackColor = System.Drawing.Color.White;
            this.pnlCanvasActions.CssStyle = "border-top:1px solid #e4eaf1";
            this.pnlCanvasActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlCanvasActions.Name = "pnlCanvasActions";
            this.pnlCanvasActions.Size = new System.Drawing.Size(330, 46);
            this.pnlCanvasActions.Controls.Add(this.btnExport);
            this.pnlCanvasActions.Controls.Add(this.btnResetView);
            //
            // cardCanvas
            //
            this.cardCanvas.BackColor = System.Drawing.Color.White;
            this.cardCanvas.CssStyle = "border:1px solid #e4eaf1;border-radius:12px;overflow:hidden";
            this.cardCanvas.Dock = Wisej.Web.DockStyle.Right;
            this.cardCanvas.Name = "cardCanvas";
            this.cardCanvas.Size = new System.Drawing.Size(330, 600);
            this.cardCanvas.Controls.Add(this.pnlCanvasFiller);
            this.cardCanvas.Controls.Add(this.canvasTopology);
            this.cardCanvas.Controls.Add(this.pnlCanvasActions);
            this.cardCanvas.Controls.Add(this.lblCanvasHeader);
            //
            // pnlBody
            //
            this.pnlBody.BackColor = System.Drawing.Color.White;
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new Wisej.Web.Padding(16, 16, 16, 16);
            this.pnlBody.Controls.Add(this.cardGrid);
            this.pnlBody.Controls.Add(this.pnlGapRight);
            this.pnlBody.Controls.Add(this.cardCanvas);
            this.pnlBody.Controls.Add(this.pnlGapLeft);
            this.pnlBody.Controls.Add(this.cardGauge);
            //
            // VisualOperationsPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Name = "VisualOperationsPage";
            this.Load += this.VisualOperationsPage_Load;
            this.Size = new System.Drawing.Size(1400, 900);
            this.Text = "Visual Operations Studio · Capstone";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlAppBar);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Label lblAppSuffix;
        private VisualOperationsStudio.Controls.WindowGlyphs glyphs;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Panel cardGauge;
        private Wisej.Web.Label lblGaugeHeader;
        private Wisej.Web.Panel pnlGaugeGap;
        private VisualOperationsStudio.Controls.TelemetryGauge gaugeCpu;
        private Wisej.Web.Label lblAccessibleReading;
        private Wisej.Web.Panel pnlGaugeFiller;
        private Wisej.Web.Panel pnlGapLeft;
        private Wisej.Web.Panel cardGrid;
        private Wisej.Web.Label lblGridHeader;
        private Wisej.Web.DataGridView gridAssets;
        private Wisej.Web.DataGridViewTextBoxColumn colAsset;
        private Wisej.Web.DataGridViewTextBoxColumn colHealth;
        private Wisej.Web.DataGridViewTextBoxColumn colTrend;
        private Wisej.Web.DataGridViewTextBoxColumn colState;
        private Wisej.Web.Label lblAccessibleTable;
        private Wisej.Web.Panel pnlGapRight;
        private Wisej.Web.Panel cardCanvas;
        private Wisej.Web.Label lblCanvasHeader;
        private Wisej.Web.Canvas canvasTopology;
        private Wisej.Web.Panel pnlCanvasFiller;
        private Wisej.Web.Panel pnlCanvasActions;
        private Wisej.Web.Button btnExport;
        private Wisej.Web.Button btnResetView;
    }
}
