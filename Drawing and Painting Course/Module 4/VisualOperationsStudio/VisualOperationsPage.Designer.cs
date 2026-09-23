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
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblMachine = new Wisej.Web.Label();
            this.lblSubtitle = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnTakeReading = new Wisej.Web.Button();
            this.btnReset = new Wisej.Web.Button();
            this.btnPlayground = new Wisej.Web.Button();
            this.pnlBody = new Wisej.Web.Panel();
            this.splitMain = new Wisej.Web.SplitContainer();
            this.pnlText = new Wisej.Web.Panel();
            this.lblTextHeader = new Wisej.Web.Label();
            this.lblCaption1 = new Wisej.Web.Label();
            this.lblValue1 = new Wisej.Web.Label();
            this.lblCaption2 = new Wisej.Web.Label();
            this.lblValue2 = new Wisej.Web.Label();
            this.lblCaption3 = new Wisej.Web.Label();
            this.lblValue3 = new Wisej.Web.Label();
            this.pnlGauges = new Wisej.Web.Panel();
            this.gaugeSpindle = new VisualOperationsStudio.Controls.TelemetryGauge();
            this.gaugeCoolant = new VisualOperationsStudio.Controls.TelemetryGauge();
            this.gaugeCycle = new VisualOperationsStudio.Controls.TelemetryGauge();
            this.gridOperations = new Wisej.Web.DataGridView();
            this.colMachine = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colSite = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colLastReading = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colHealth = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTrend = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatusHtml = new Wisej.Web.DataGridViewTextBoxColumn();
            this.SuspendLayout();
            //
            // lblMachine
            //
            this.lblMachine.AutoSize = false;
            this.lblMachine.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold);
            this.lblMachine.Location = new System.Drawing.Point(20, 12);
            this.lblMachine.Name = "lblMachine";
            this.lblMachine.Size = new System.Drawing.Size(360, 28);
            this.lblMachine.Text = "Line 3 Press";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSubtitle.Location = new System.Drawing.Point(20, 42);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(760, 22);
            this.lblSubtitle.Text = "TelemetryGauge - three painted gauges over the same TelemetrySample model";
            //
            // pnlHeader
            //
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1080, 72);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblMachine);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(20, 8, 20, 8);
            this.lblStatus.Size = new System.Drawing.Size(1080, 34);
            this.lblStatus.Text = "Opening VisualOperationsStudio...";
            //
            // btnTakeReading
            //
            this.btnTakeReading.Location = new System.Drawing.Point(20, 9);
            this.btnTakeReading.Name = "btnTakeReading";
            this.btnTakeReading.Size = new System.Drawing.Size(160, 38);
            this.btnTakeReading.TabIndex = 0;
            this.btnTakeReading.Text = "Take reading";
            this.btnTakeReading.Click += this.btnTakeReading_Click;
            //
            // btnReset
            //
            this.btnReset.Location = new System.Drawing.Point(192, 9);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(110, 38);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "Reset";
            this.btnReset.Click += this.btnReset_Click;
            //
            // btnPlayground
            //
            this.btnPlayground.Location = new System.Drawing.Point(314, 9);
            this.btnPlayground.Name = "btnPlayground";
            this.btnPlayground.Size = new System.Drawing.Size(180, 38);
            this.btnPlayground.TabIndex = 2;
            this.btnPlayground.Text = "Canvas playground";
            this.btnPlayground.Click += this.btnPlayground_Click;
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1080, 56);
            this.pnlActions.Controls.Add(this.btnPlayground);
            this.pnlActions.Controls.Add(this.btnReset);
            this.pnlActions.Controls.Add(this.btnTakeReading);
            //
            // lblTextHeader
            //
            this.lblTextHeader.AutoSize = false;
            this.lblTextHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblTextHeader.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblTextHeader.Name = "lblTextHeader";
            this.lblTextHeader.Size = new System.Drawing.Size(300, 30);
            this.lblTextHeader.Text = "The same values, as text";
            //
            // lblCaption1
            //
            this.lblCaption1.AutoSize = false;
            this.lblCaption1.Location = new System.Drawing.Point(14, 48);
            this.lblCaption1.Name = "lblCaption1";
            this.lblCaption1.Size = new System.Drawing.Size(170, 26);
            //
            // lblValue1
            //
            this.lblValue1.AutoSize = false;
            this.lblValue1.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblValue1.Location = new System.Drawing.Point(186, 48);
            this.lblValue1.Name = "lblValue1";
            this.lblValue1.Size = new System.Drawing.Size(110, 26);
            this.lblValue1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCaption2
            //
            this.lblCaption2.AutoSize = false;
            this.lblCaption2.Location = new System.Drawing.Point(14, 80);
            this.lblCaption2.Name = "lblCaption2";
            this.lblCaption2.Size = new System.Drawing.Size(170, 26);
            //
            // lblValue2
            //
            this.lblValue2.AutoSize = false;
            this.lblValue2.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblValue2.Location = new System.Drawing.Point(186, 80);
            this.lblValue2.Name = "lblValue2";
            this.lblValue2.Size = new System.Drawing.Size(110, 26);
            this.lblValue2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCaption3
            //
            this.lblCaption3.AutoSize = false;
            this.lblCaption3.Location = new System.Drawing.Point(14, 112);
            this.lblCaption3.Name = "lblCaption3";
            this.lblCaption3.Size = new System.Drawing.Size(170, 26);
            //
            // lblValue3
            //
            this.lblValue3.AutoSize = false;
            this.lblValue3.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblValue3.Location = new System.Drawing.Point(186, 112);
            this.lblValue3.Name = "lblValue3";
            this.lblValue3.Size = new System.Drawing.Size(110, 26);
            this.lblValue3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlText
            //
            this.pnlText.Dock = Wisej.Web.DockStyle.Right;
            this.pnlText.Name = "pnlText";
            this.pnlText.Size = new System.Drawing.Size(320, 400);
            this.pnlText.Controls.Add(this.lblValue3);
            this.pnlText.Controls.Add(this.lblCaption3);
            this.pnlText.Controls.Add(this.lblValue2);
            this.pnlText.Controls.Add(this.lblCaption2);
            this.pnlText.Controls.Add(this.lblValue1);
            this.pnlText.Controls.Add(this.lblCaption1);
            this.pnlText.Controls.Add(this.lblTextHeader);
            //
            // gaugeSpindle
            //
            this.gaugeSpindle.Dock = Wisej.Web.DockStyle.Left;
            this.gaugeSpindle.Name = "gaugeSpindle";
            this.gaugeSpindle.Size = new System.Drawing.Size(250, 400);
            //
            // gaugeCoolant
            //
            this.gaugeCoolant.Dock = Wisej.Web.DockStyle.Left;
            this.gaugeCoolant.Name = "gaugeCoolant";
            this.gaugeCoolant.Size = new System.Drawing.Size(250, 400);
            //
            // gaugeCycle
            //
            this.gaugeCycle.Dock = Wisej.Web.DockStyle.Fill;
            this.gaugeCycle.Name = "gaugeCycle";
            //
            // pnlGauges
            //
            this.pnlGauges.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlGauges.Name = "pnlGauges";
            this.pnlGauges.Padding = new Wisej.Web.Padding(12, 6, 12, 6);
            this.pnlGauges.Resize += this.pnlGauges_Resize;
            this.pnlGauges.Controls.Add(this.gaugeCycle);
            this.pnlGauges.Controls.Add(this.gaugeCoolant);
            this.pnlGauges.Controls.Add(this.gaugeSpindle);
            //
            // pnlBody
            //
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Controls.Add(this.pnlGauges);
            this.pnlBody.Controls.Add(this.pnlText);
            //
            // colMachine
            //
            this.colMachine.DataPropertyName = "Machine";
            this.colMachine.HeaderText = "Machine";
            this.colMachine.Name = "colMachine";
            this.colMachine.Width = 130;
            //
            // colSite
            //
            this.colSite.DataPropertyName = "Site";
            this.colSite.HeaderText = "Site";
            this.colSite.Name = "colSite";
            this.colSite.Width = 110;
            //
            // colLastReading
            //
            this.colLastReading.DataPropertyName = "LastReadingText";
            this.colLastReading.HeaderText = "Last reading";
            this.colLastReading.Name = "colLastReading";
            this.colLastReading.Width = 110;
            //
            // colHealth
            //
            this.colHealth.HeaderText = "Health (painted)";
            this.colHealth.Name = "colHealth";
            this.colHealth.Width = 200;
            //
            // colTrend
            //
            this.colTrend.HeaderText = "Trend (painted)";
            this.colTrend.Name = "colTrend";
            this.colTrend.Width = 160;
            //
            // colStatusHtml
            //
            this.colStatusHtml.AllowHtml = true;
            this.colStatusHtml.DataPropertyName = "SeverityHtml";
            this.colStatusHtml.HeaderText = "Status (AllowHtml)";
            this.colStatusHtml.Name = "colStatusHtml";
            this.colStatusHtml.Width = 180;
            //
            // gridOperations
            //
            this.gridOperations.AllowUserToAddRows = false;
            this.gridOperations.AutoGenerateColumns = false;
            this.gridOperations.Dock = Wisej.Web.DockStyle.Fill;
            this.gridOperations.Name = "gridOperations";
            this.gridOperations.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOperations.Columns.Add(this.colMachine);
            this.gridOperations.Columns.Add(this.colSite);
            this.gridOperations.Columns.Add(this.colLastReading);
            this.gridOperations.Columns.Add(this.colHealth);
            this.gridOperations.Columns.Add(this.colTrend);
            this.gridOperations.Columns.Add(this.colStatusHtml);
            //
            // splitMain
            //
            this.splitMain.Dock = Wisej.Web.DockStyle.Fill;
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = Wisej.Web.Orientation.Horizontal;
            this.splitMain.SplitterDistance = 300;
            this.splitMain.Panel1.Controls.Add(this.pnlBody);
            this.splitMain.Panel2.Controls.Add(this.gridOperations);
            //
            // VisualOperationsPage
            //
            this.Name = "VisualOperationsPage";
            this.Size = new System.Drawing.Size(1080, 660);
            this.Text = "VisualOperationsStudio - Operations";
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblMachine;
        private Wisej.Web.Label lblSubtitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnTakeReading;
        private Wisej.Web.Button btnReset;
        private Wisej.Web.Button btnPlayground;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.SplitContainer splitMain;
        private Wisej.Web.Panel pnlText;
        private Wisej.Web.Label lblTextHeader;
        private Wisej.Web.Label lblCaption1;
        private Wisej.Web.Label lblValue1;
        private Wisej.Web.Label lblCaption2;
        private Wisej.Web.Label lblValue2;
        private Wisej.Web.Label lblCaption3;
        private Wisej.Web.Label lblValue3;
        private Wisej.Web.Panel pnlGauges;
        private VisualOperationsStudio.Controls.TelemetryGauge gaugeSpindle;
        private VisualOperationsStudio.Controls.TelemetryGauge gaugeCoolant;
        private VisualOperationsStudio.Controls.TelemetryGauge gaugeCycle;
        private Wisej.Web.DataGridView gridOperations;
        private Wisej.Web.DataGridViewTextBoxColumn colMachine;
        private Wisej.Web.DataGridViewTextBoxColumn colSite;
        private Wisej.Web.DataGridViewTextBoxColumn colLastReading;
        private Wisej.Web.DataGridViewTextBoxColumn colHealth;
        private Wisej.Web.DataGridViewTextBoxColumn colTrend;
        private Wisej.Web.DataGridViewTextBoxColumn colStatusHtml;
    }
}
