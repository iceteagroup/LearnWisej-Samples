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
            this.glyphs = new VisualOperationsStudio.Controls.WindowGlyphs();
            this.pnlBody = new Wisej.Web.Panel();
            this.pnlGauges = new Wisej.Web.Panel();
            this.pnlCard1 = new Wisej.Web.Panel();
            this.gaugeSpindle = new VisualOperationsStudio.Controls.TelemetryGauge();
            this.pnlCardGap1 = new Wisej.Web.Panel();
            this.pnlCard2 = new Wisej.Web.Panel();
            this.gaugeCoolant = new VisualOperationsStudio.Controls.TelemetryGauge();
            this.pnlCardGap2 = new Wisej.Web.Panel();
            this.pnlCard3 = new Wisej.Web.Panel();
            this.gaugeCycle = new VisualOperationsStudio.Controls.TelemetryGauge();
            this.pnlActionGap = new Wisej.Web.Panel();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnTakeReading = new Wisej.Web.Button();
            this.btnReset = new Wisej.Web.Button();
            this.lblRepaints = new Wisej.Web.Label();
            this.pnlFiller = new Wisej.Web.Panel();
            this.SuspendLayout();
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Padding = new Wisej.Web.Padding(20, 0, 0, 0);
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAppTitle.Text = "VisualOperationsStudio — Operations";
            //
            // glyphs
            //
            this.glyphs.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.glyphs.Dock = Wisej.Web.DockStyle.Right;
            this.glyphs.Inset = 20;
            this.glyphs.Name = "glyphs";
            this.glyphs.Size = new System.Drawing.Size(91, 42);
            //
            // pnlAppBar
            //
            this.pnlAppBar.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Size = new System.Drawing.Size(1400, 42);
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            this.pnlAppBar.Controls.Add(this.glyphs);
            //
            // gaugeSpindle
            //
            this.gaugeSpindle.Dock = Wisej.Web.DockStyle.Fill;
            this.gaugeSpindle.Name = "gaugeSpindle";
            //
            // pnlCard1
            //
            this.pnlCard1.BackColor = System.Drawing.Color.White;
            this.pnlCard1.CssStyle = "border:1px solid #e2e9f1;border-radius:12px;box-shadow:0 6px 18px rgba(13,40,80,.06)";
            this.pnlCard1.Dock = Wisej.Web.DockStyle.Left;
            this.pnlCard1.Name = "pnlCard1";
            this.pnlCard1.Padding = new Wisej.Web.Padding(10, 16, 10, 12);
            this.pnlCard1.Size = new System.Drawing.Size(437, 389);
            this.pnlCard1.Controls.Add(this.gaugeSpindle);
            //
            // pnlCardGap1
            //
            this.pnlCardGap1.Dock = Wisej.Web.DockStyle.Left;
            this.pnlCardGap1.Name = "pnlCardGap1";
            this.pnlCardGap1.Size = new System.Drawing.Size(18, 389);
            //
            // gaugeCoolant
            //
            this.gaugeCoolant.Dock = Wisej.Web.DockStyle.Fill;
            this.gaugeCoolant.Name = "gaugeCoolant";
            //
            // pnlCard2
            //
            this.pnlCard2.BackColor = System.Drawing.Color.White;
            this.pnlCard2.CssStyle = "border:1px solid #e2e9f1;border-radius:12px;box-shadow:0 6px 18px rgba(13,40,80,.06)";
            this.pnlCard2.Dock = Wisej.Web.DockStyle.Left;
            this.pnlCard2.Name = "pnlCard2";
            this.pnlCard2.Padding = new Wisej.Web.Padding(10, 16, 10, 12);
            this.pnlCard2.Size = new System.Drawing.Size(437, 389);
            this.pnlCard2.Controls.Add(this.gaugeCoolant);
            //
            // pnlCardGap2
            //
            this.pnlCardGap2.Dock = Wisej.Web.DockStyle.Left;
            this.pnlCardGap2.Name = "pnlCardGap2";
            this.pnlCardGap2.Size = new System.Drawing.Size(18, 389);
            //
            // gaugeCycle
            //
            this.gaugeCycle.Dock = Wisej.Web.DockStyle.Fill;
            this.gaugeCycle.Name = "gaugeCycle";
            //
            // pnlCard3
            //
            this.pnlCard3.BackColor = System.Drawing.Color.White;
            this.pnlCard3.CssStyle = "border:1px solid #e2e9f1;border-radius:12px;box-shadow:0 6px 18px rgba(13,40,80,.06)";
            this.pnlCard3.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlCard3.Name = "pnlCard3";
            this.pnlCard3.Padding = new Wisej.Web.Padding(10, 16, 10, 12);
            this.pnlCard3.Controls.Add(this.gaugeCycle);
            //
            // pnlGauges
            //
            this.pnlGauges.Dock = Wisej.Web.DockStyle.Top;
            this.pnlGauges.Name = "pnlGauges";
            this.pnlGauges.Size = new System.Drawing.Size(1348, 389);
            this.pnlGauges.Controls.Add(this.pnlCard3);
            this.pnlGauges.Controls.Add(this.pnlCardGap2);
            this.pnlGauges.Controls.Add(this.pnlCard2);
            this.pnlGauges.Controls.Add(this.pnlCardGap1);
            this.pnlGauges.Controls.Add(this.pnlCard1);
            //
            // pnlActionGap
            //
            this.pnlActionGap.Dock = Wisej.Web.DockStyle.Top;
            this.pnlActionGap.Name = "pnlActionGap";
            this.pnlActionGap.Size = new System.Drawing.Size(1348, 20);
            //
            // btnTakeReading
            //
            this.btnTakeReading.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.btnTakeReading.CssStyle = "border-radius:7px;border:none";
            this.btnTakeReading.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnTakeReading.ForeColor = System.Drawing.Color.White;
            this.btnTakeReading.Location = new System.Drawing.Point(0, 0);
            this.btnTakeReading.Name = "btnTakeReading";
            this.btnTakeReading.Size = new System.Drawing.Size(136, 38);
            this.btnTakeReading.TabIndex = 0;
            this.btnTakeReading.Text = "Take reading";
            this.btnTakeReading.Click += this.btnTakeReading_Click;
            //
            // btnReset
            //
            this.btnReset.BackColor = System.Drawing.Color.White;
            this.btnReset.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.btnReset.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnReset.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.btnReset.Location = new System.Drawing.Point(150, 0);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(96, 38);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "Reset";
            this.btnReset.Click += this.btnReset_Click;
            //
            // lblRepaints
            //
            this.lblRepaints.AutoSize = false;
            this.lblRepaints.Dock = Wisej.Web.DockStyle.Right;
            this.lblRepaints.Font = new System.Drawing.Font("default", 13.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblRepaints.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblRepaints.Name = "lblRepaints";
            this.lblRepaints.Size = new System.Drawing.Size(300, 38);
            this.lblRepaints.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRepaints.Text = "Repaints this request: 0";
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Top;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1348, 38);
            this.pnlActions.Controls.Add(this.btnTakeReading);
            this.pnlActions.Controls.Add(this.btnReset);
            this.pnlActions.Controls.Add(this.lblRepaints);
            //
            // pnlFiller
            //
            this.pnlFiller.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlFiller.Name = "pnlFiller";
            //
            // pnlBody
            //
            this.pnlBody.BackColor = System.Drawing.Color.White;
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new Wisej.Web.Padding(26, 24, 26, 22);
            this.pnlBody.Resize += this.pnlBody_Resize;
            this.pnlBody.Controls.Add(this.pnlFiller);
            this.pnlBody.Controls.Add(this.pnlActions);
            this.pnlBody.Controls.Add(this.pnlActionGap);
            this.pnlBody.Controls.Add(this.pnlGauges);
            //
            // VisualOperationsPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Name = "VisualOperationsPage";
            this.Load += this.VisualOperationsPage_Load;
            this.Size = new System.Drawing.Size(1400, 900);
            this.Text = "VisualOperationsStudio — Operations";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlAppBar);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private VisualOperationsStudio.Controls.WindowGlyphs glyphs;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Panel pnlGauges;
        private Wisej.Web.Panel pnlCard1;
        private Wisej.Web.Panel pnlCardGap1;
        private Wisej.Web.Panel pnlCard2;
        private Wisej.Web.Panel pnlCardGap2;
        private Wisej.Web.Panel pnlCard3;
        private VisualOperationsStudio.Controls.TelemetryGauge gaugeSpindle;
        private VisualOperationsStudio.Controls.TelemetryGauge gaugeCoolant;
        private VisualOperationsStudio.Controls.TelemetryGauge gaugeCycle;
        private Wisej.Web.Panel pnlActionGap;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnTakeReading;
        private Wisej.Web.Button btnReset;
        private Wisej.Web.Label lblRepaints;
        private Wisej.Web.Panel pnlFiller;
    }
}
