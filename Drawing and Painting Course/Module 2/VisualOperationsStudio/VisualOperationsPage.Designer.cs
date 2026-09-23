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
            this.pnlSurfaces = new Wisej.Web.Panel();
            this.lblSurfacesHeader = new Wisej.Web.Label();
            this.layoutSurfaces = new Wisej.Web.TableLayoutPanel();
            this.pnlSurface1 = new Wisej.Web.Panel();
            this.lblSurface1 = new Wisej.Web.Label();
            this.pnlPlain = new Wisej.Web.Panel();
            this.lblReading = new Wisej.Web.Label();
            this.progressReading = new Wisej.Web.ProgressBar();
            this.pnlSurface3 = new Wisej.Web.Panel();
            this.lblSurface3 = new Wisej.Web.Label();
            this.canvasSurface = new Wisej.Web.Canvas();
            this.pnlSurface4 = new Wisej.Web.Panel();
            this.lblSurface4 = new Wisej.Web.Label();
            this.picExport = new Wisej.Web.PictureBox();
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblMachine = new Wisej.Web.Label();
            this.lblSubtitle = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnTakeReading = new Wisej.Web.Button();
            this.btnReset = new Wisej.Web.Button();
            this.pnlBody = new Wisej.Web.Panel();
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
            this.lblStatus.Text = "Repaints this request: 0";
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
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1080, 56);
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
            // lblSurfacesHeader
            //
            this.lblSurfacesHeader.AutoSize = false;
            this.lblSurfacesHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblSurfacesHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSurfacesHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSurfacesHeader.Name = "lblSurfacesHeader";
            this.lblSurfacesHeader.Size = new System.Drawing.Size(1080, 24);
            this.lblSurfacesHeader.Text = "The same spindle reading on the other three surfaces - surface 2 is the painted TelemetryGauge above";
            //
            // lblSurface1
            //
            this.lblSurface1.AutoSize = false;
            this.lblSurface1.Dock = Wisej.Web.DockStyle.Top;
            this.lblSurface1.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSurface1.Name = "lblSurface1";
            this.lblSurface1.Size = new System.Drawing.Size(200, 22);
            this.lblSurface1.Text = "1 - Label + ProgressBar";
            //
            // progressReading
            //
            this.progressReading.Dock = Wisej.Web.DockStyle.Top;
            this.progressReading.Maximum = 100;
            this.progressReading.Minimum = 0;
            this.progressReading.Name = "progressReading";
            this.progressReading.Size = new System.Drawing.Size(200, 24);
            this.progressReading.Value = 34;
            //
            // lblReading
            //
            this.lblReading.AutoSize = false;
            this.lblReading.Dock = Wisej.Web.DockStyle.Top;
            this.lblReading.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.lblReading.Name = "lblReading";
            this.lblReading.Size = new System.Drawing.Size(200, 30);
            this.lblReading.Text = "34 %";
            //
            // pnlPlain
            //
            this.pnlPlain.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlPlain.Name = "pnlPlain";
            this.pnlPlain.Controls.Add(this.progressReading);
            this.pnlPlain.Controls.Add(this.lblReading);
            //
            // pnlSurface1
            //
            this.pnlSurface1.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlSurface1.Name = "pnlSurface1";
            this.pnlSurface1.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.pnlSurface1.Controls.Add(this.pnlPlain);
            this.pnlSurface1.Controls.Add(this.lblSurface1);
            //
            // lblSurface3
            //
            this.lblSurface3.AutoSize = false;
            this.lblSurface3.Dock = Wisej.Web.DockStyle.Top;
            this.lblSurface3.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSurface3.Name = "lblSurface3";
            this.lblSurface3.Size = new System.Drawing.Size(200, 22);
            this.lblSurface3.Text = "3 - Wisej.Web.Canvas";
            //
            // canvasSurface
            //
            this.canvasSurface.Dock = Wisej.Web.DockStyle.Fill;
            this.canvasSurface.Name = "canvasSurface";
            this.canvasSurface.Redraw += this.canvasSurface_Redraw;
            //
            // pnlSurface3
            //
            this.pnlSurface3.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlSurface3.Name = "pnlSurface3";
            this.pnlSurface3.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.pnlSurface3.Controls.Add(this.canvasSurface);
            this.pnlSurface3.Controls.Add(this.lblSurface3);
            //
            // lblSurface4
            //
            this.lblSurface4.AutoSize = false;
            this.lblSurface4.Dock = Wisej.Web.DockStyle.Top;
            this.lblSurface4.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSurface4.Name = "lblSurface4";
            this.lblSurface4.Size = new System.Drawing.Size(200, 22);
            this.lblSurface4.Text = "4 - Bitmap + Graphics.FromImage";
            //
            // picExport
            //
            this.picExport.Dock = Wisej.Web.DockStyle.Fill;
            this.picExport.Name = "picExport";
            this.picExport.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            //
            // pnlSurface4
            //
            this.pnlSurface4.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlSurface4.Name = "pnlSurface4";
            this.pnlSurface4.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.pnlSurface4.Controls.Add(this.picExport);
            this.pnlSurface4.Controls.Add(this.lblSurface4);
            //
            // layoutSurfaces
            //
            this.layoutSurfaces.ColumnCount = 3;
            this.layoutSurfaces.RowCount = 1;
            this.layoutSurfaces.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 33.34F));
            this.layoutSurfaces.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 33.33F));
            this.layoutSurfaces.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 33.33F));
            this.layoutSurfaces.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
            this.layoutSurfaces.Dock = Wisej.Web.DockStyle.Fill;
            this.layoutSurfaces.Name = "layoutSurfaces";
            this.layoutSurfaces.Controls.Add(this.pnlSurface1, 0, 0);
            this.layoutSurfaces.Controls.Add(this.pnlSurface3, 1, 0);
            this.layoutSurfaces.Controls.Add(this.pnlSurface4, 2, 0);
            //
            // pnlSurfaces
            //
            this.pnlSurfaces.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlSurfaces.Name = "pnlSurfaces";
            this.pnlSurfaces.Padding = new Wisej.Web.Padding(12, 4, 12, 8);
            this.pnlSurfaces.Size = new System.Drawing.Size(1080, 150);
            this.pnlSurfaces.Controls.Add(this.layoutSurfaces);
            this.pnlSurfaces.Controls.Add(this.lblSurfacesHeader);
            //
            // VisualOperationsPage
            //
            this.Name = "VisualOperationsPage";
            this.Load += this.VisualOperationsPage_Load;
            this.Size = new System.Drawing.Size(1080, 660);
            this.Text = "VisualOperationsStudio - Operations";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlSurfaces);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Panel pnlSurfaces;
        private Wisej.Web.Label lblSurfacesHeader;
        private Wisej.Web.TableLayoutPanel layoutSurfaces;
        private Wisej.Web.Panel pnlSurface1;
        private Wisej.Web.Label lblSurface1;
        private Wisej.Web.Panel pnlPlain;
        private Wisej.Web.Label lblReading;
        private Wisej.Web.ProgressBar progressReading;
        private Wisej.Web.Panel pnlSurface3;
        private Wisej.Web.Label lblSurface3;
        private Wisej.Web.Canvas canvasSurface;
        private Wisej.Web.Panel pnlSurface4;
        private Wisej.Web.Label lblSurface4;
        private Wisej.Web.PictureBox picExport;
        private Wisej.Web.Label lblMachine;
        private Wisej.Web.Label lblSubtitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnTakeReading;
        private Wisej.Web.Button btnReset;
        private Wisej.Web.Panel pnlBody;
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
    }
}
