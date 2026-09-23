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
            this.btnNewReading = new Wisej.Web.Button();
            this.layoutSurfaces = new Wisej.Web.TableLayoutPanel();
            this.pnlSurface1 = new Wisej.Web.Panel();
            this.lblSurface1 = new Wisej.Web.Label();
            this.pnlPlain = new Wisej.Web.Panel();
            this.lblReading = new Wisej.Web.Label();
            this.progressReading = new Wisej.Web.ProgressBar();
            this.pnlSurface2 = new Wisej.Web.Panel();
            this.lblSurface2 = new Wisej.Web.Label();
            this.panelPainted = new Wisej.Web.Panel();
            this.pnlSurface3 = new Wisej.Web.Panel();
            this.lblSurface3 = new Wisej.Web.Label();
            this.canvasSurface = new Wisej.Web.Canvas();
            this.pnlSurface4 = new Wisej.Web.Panel();
            this.lblSurface4 = new Wisej.Web.Label();
            this.picExport = new Wisej.Web.PictureBox();
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
            this.lblSubtitle.Size = new System.Drawing.Size(700, 22);
            this.lblSubtitle.Text = "TelemetrySample - one model, four surfaces";
            //
            // pnlHeader
            //
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(980, 72);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblMachine);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(20, 8, 20, 8);
            this.lblStatus.Size = new System.Drawing.Size(980, 34);
            this.lblStatus.Text = "Ready";
            //
            // btnNewReading
            //
            this.btnNewReading.Location = new System.Drawing.Point(20, 9);
            this.btnNewReading.Name = "btnNewReading";
            this.btnNewReading.Size = new System.Drawing.Size(178, 38);
            this.btnNewReading.TabIndex = 0;
            this.btnNewReading.Text = "New reading";
            this.btnNewReading.Click += this.btnNewReading_Click;
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(980, 56);
            this.pnlActions.Controls.Add(this.btnNewReading);
            //
            // lblSurface1
            //
            this.lblSurface1.AutoSize = false;
            this.lblSurface1.Dock = Wisej.Web.DockStyle.Top;
            this.lblSurface1.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSurface1.Name = "lblSurface1";
            this.lblSurface1.Size = new System.Drawing.Size(200, 26);
            this.lblSurface1.Text = "1 - Label + ProgressBar";
            //
            // lblReading
            //
            this.lblReading.AutoSize = false;
            this.lblReading.Dock = Wisej.Web.DockStyle.Top;
            this.lblReading.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.lblReading.Name = "lblReading";
            this.lblReading.Size = new System.Drawing.Size(200, 32);
            this.lblReading.Text = "42 %";
            //
            // progressReading
            //
            this.progressReading.Dock = Wisej.Web.DockStyle.Top;
            this.progressReading.Maximum = 100;
            this.progressReading.Minimum = 0;
            this.progressReading.Name = "progressReading";
            this.progressReading.Size = new System.Drawing.Size(200, 26);
            this.progressReading.Value = 42;
            //
            // pnlPlain
            //
            this.pnlPlain.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlPlain.Name = "pnlPlain";
            this.pnlPlain.Controls.Add(this.lblReading);
            this.pnlPlain.Controls.Add(this.progressReading);
            //
            // pnlSurface1
            //
            this.pnlSurface1.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlSurface1.Name = "pnlSurface1";
            this.pnlSurface1.Padding = new Wisej.Web.Padding(12);
            this.pnlSurface1.Controls.Add(this.pnlPlain);
            this.pnlSurface1.Controls.Add(this.lblSurface1);
            //
            // lblSurface2
            //
            this.lblSurface2.AutoSize = false;
            this.lblSurface2.Dock = Wisej.Web.DockStyle.Top;
            this.lblSurface2.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSurface2.Name = "lblSurface2";
            this.lblSurface2.Size = new System.Drawing.Size(200, 26);
            this.lblSurface2.Text = "2 - Panel.Paint";
            //
            // panelPainted
            //
            this.panelPainted.Dock = Wisej.Web.DockStyle.Fill;
            this.panelPainted.Name = "panelPainted";
            this.panelPainted.Paint += this.panelPainted_Paint;
            //
            // pnlSurface2
            //
            this.pnlSurface2.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlSurface2.Name = "pnlSurface2";
            this.pnlSurface2.Padding = new Wisej.Web.Padding(12);
            this.pnlSurface2.Controls.Add(this.panelPainted);
            this.pnlSurface2.Controls.Add(this.lblSurface2);
            //
            // lblSurface3
            //
            this.lblSurface3.AutoSize = false;
            this.lblSurface3.Dock = Wisej.Web.DockStyle.Top;
            this.lblSurface3.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSurface3.Name = "lblSurface3";
            this.lblSurface3.Size = new System.Drawing.Size(200, 26);
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
            this.pnlSurface3.Padding = new Wisej.Web.Padding(12);
            this.pnlSurface3.Controls.Add(this.canvasSurface);
            this.pnlSurface3.Controls.Add(this.lblSurface3);
            //
            // lblSurface4
            //
            this.lblSurface4.AutoSize = false;
            this.lblSurface4.Dock = Wisej.Web.DockStyle.Top;
            this.lblSurface4.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSurface4.Name = "lblSurface4";
            this.lblSurface4.Size = new System.Drawing.Size(200, 26);
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
            this.pnlSurface4.Padding = new Wisej.Web.Padding(12);
            this.pnlSurface4.Controls.Add(this.picExport);
            this.pnlSurface4.Controls.Add(this.lblSurface4);
            //
            // layoutSurfaces
            //
            this.layoutSurfaces.ColumnCount = 2;
            this.layoutSurfaces.RowCount = 2;
            this.layoutSurfaces.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 50F));
            this.layoutSurfaces.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 50F));
            this.layoutSurfaces.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 50F));
            this.layoutSurfaces.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 50F));
            this.layoutSurfaces.Dock = Wisej.Web.DockStyle.Fill;
            this.layoutSurfaces.Name = "layoutSurfaces";
            this.layoutSurfaces.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.layoutSurfaces.Controls.Add(this.pnlSurface1, 0, 0);
            this.layoutSurfaces.Controls.Add(this.pnlSurface2, 1, 0);
            this.layoutSurfaces.Controls.Add(this.pnlSurface3, 0, 1);
            this.layoutSurfaces.Controls.Add(this.pnlSurface4, 1, 1);
            //
            // VisualOperationsPage
            //
            this.Name = "VisualOperationsPage";
            this.Load += this.VisualOperationsPage_Load;
            this.Size = new System.Drawing.Size(980, 660);
            this.Text = "VisualOperationsStudio - Operations";
            this.Controls.Add(this.layoutSurfaces);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblMachine;
        private Wisej.Web.Label lblSubtitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnNewReading;
        private Wisej.Web.TableLayoutPanel layoutSurfaces;
        private Wisej.Web.Panel pnlSurface1;
        private Wisej.Web.Label lblSurface1;
        private Wisej.Web.Panel pnlPlain;
        private Wisej.Web.Label lblReading;
        private Wisej.Web.ProgressBar progressReading;
        private Wisej.Web.Panel pnlSurface2;
        private Wisej.Web.Label lblSurface2;
        private Wisej.Web.Panel panelPainted;
        private Wisej.Web.Panel pnlSurface3;
        private Wisej.Web.Label lblSurface3;
        private Wisej.Web.Canvas canvasSurface;
        private Wisej.Web.Panel pnlSurface4;
        private Wisej.Web.Label lblSurface4;
        private Wisej.Web.PictureBox picExport;
    }
}
