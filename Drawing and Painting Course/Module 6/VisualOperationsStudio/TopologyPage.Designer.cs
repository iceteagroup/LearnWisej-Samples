namespace VisualOperationsStudio
{
    partial class TopologyPage
    {
        private System.ComponentModel.IContainer components = null;

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
            this.pnlToolbar = new Wisej.Web.Panel();
            this.lblToolbarTitle = new Wisej.Web.Label();
            this.btnFitToView = new Wisej.Web.Button();
            this.btnResetZoom = new Wisej.Web.Button();
            this.btnExport = new Wisej.Web.Button();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlCanvasHost = new Wisej.Web.Panel();
            this.canvasTopology = new Wisej.Web.Canvas();
            this.lblSurfaceNote = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 14.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Padding = new Wisej.Web.Padding(16, 0, 0, 0);
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAppTitle.Text = "VisualOperationsStudio — Topology";
            //
            // glyphs
            //
            this.glyphs.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.glyphs.Dock = Wisej.Web.DockStyle.Right;
            this.glyphs.Inset = 16;
            this.glyphs.Name = "glyphs";
            this.glyphs.Size = new System.Drawing.Size(85, 38);
            //
            // pnlAppBar
            //
            this.pnlAppBar.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Size = new System.Drawing.Size(1400, 38);
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            this.pnlAppBar.Controls.Add(this.glyphs);
            //
            // lblToolbarTitle
            //
            this.lblToolbarTitle.AutoSize = false;
            this.lblToolbarTitle.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblToolbarTitle.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblToolbarTitle.Location = new System.Drawing.Point(16, 0);
            this.lblToolbarTitle.Name = "lblToolbarTitle";
            this.lblToolbarTitle.Size = new System.Drawing.Size(80, 52);
            this.lblToolbarTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblToolbarTitle.Text = "Topology";
            //
            // btnFitToView
            //
            this.btnFitToView.BackColor = System.Drawing.Color.White;
            this.btnFitToView.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.btnFitToView.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnFitToView.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.btnFitToView.Location = new System.Drawing.Point(108, 12);
            this.btnFitToView.Name = "btnFitToView";
            this.btnFitToView.Size = new System.Drawing.Size(98, 30);
            this.btnFitToView.TabIndex = 0;
            this.btnFitToView.Text = "Fit to view";
            this.btnFitToView.Click += this.btnFitToView_Click;
            //
            // btnResetZoom
            //
            this.btnResetZoom.BackColor = System.Drawing.Color.White;
            this.btnResetZoom.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.btnResetZoom.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnResetZoom.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.btnResetZoom.Location = new System.Drawing.Point(216, 12);
            this.btnResetZoom.Name = "btnResetZoom";
            this.btnResetZoom.Size = new System.Drawing.Size(106, 30);
            this.btnResetZoom.TabIndex = 1;
            this.btnResetZoom.Text = "Reset zoom";
            this.btnResetZoom.Click += this.btnResetZoom_Click;
            //
            // btnExport
            //
            this.btnExport.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.btnExport.CssStyle = "border-radius:7px;border:none";
            this.btnExport.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(1268, 12);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(116, 30);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export PNG";
            this.btnExport.Click += this.btnExport_Click;
            //
            // pnlToolbar
            //
            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(251, 252, 254);
            this.pnlToolbar.CssStyle = "border-bottom:1px solid #e3e9f0";
            this.pnlToolbar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Size = new System.Drawing.Size(1400, 52);
            this.pnlToolbar.Controls.Add(this.lblToolbarTitle);
            this.pnlToolbar.Controls.Add(this.btnFitToView);
            this.pnlToolbar.Controls.Add(this.btnResetZoom);
            this.pnlToolbar.Controls.Add(this.btnExport);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(246, 248, 251);
            this.lblStatus.CssStyle = "border-top:1px solid #e3e9f0";
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("default", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.lblStatus.Size = new System.Drawing.Size(1400, 44);
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.Text = "Drag a node, or export the scene as a PNG";
            //
            // canvasTopology
            //
            this.canvasTopology.BackColor = System.Drawing.Color.FromArgb(247, 250, 253);
            this.canvasTopology.Dock = Wisej.Web.DockStyle.Fill;
            this.canvasTopology.Focusable = true;
            this.canvasTopology.LiveUpdate = false;
            this.canvasTopology.Name = "canvasTopology";
            this.canvasTopology.TabStop = true;
            this.canvasTopology.Redraw += this.canvasTopology_Redraw;
            this.canvasTopology.MouseDown += this.canvasTopology_MouseDown;
            this.canvasTopology.MouseMove += this.canvasTopology_MouseMove;
            this.canvasTopology.MouseUp += this.canvasTopology_MouseUp;
            this.canvasTopology.MouseWheel += this.canvasTopology_MouseWheel;
            this.canvasTopology.KeyDown += this.canvasTopology_KeyDown;
            //
            // lblSurfaceNote
            //
            this.lblSurfaceNote.AutoSize = false;
            this.lblSurfaceNote.BackColor = System.Drawing.Color.FromArgb(247, 250, 253);
            this.lblSurfaceNote.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblSurfaceNote.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblSurfaceNote.ForeColor = System.Drawing.Color.FromArgb(122, 143, 165);
            this.lblSurfaceNote.Name = "lblSurfaceNote";
            this.lblSurfaceNote.Padding = new Wisej.Web.Padding(14, 0, 14, 0);
            this.lblSurfaceNote.Size = new System.Drawing.Size(320, 30);
            this.lblSurfaceNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSurfaceNote.Text = "Wisej.Web.Canvas · browser-side commands";
            //
            // pnlCanvasHost
            //
            this.pnlCanvasHost.BackColor = System.Drawing.Color.FromArgb(247, 250, 253);
            this.pnlCanvasHost.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlCanvasHost.Name = "pnlCanvasHost";
            this.pnlCanvasHost.Controls.Add(this.canvasTopology);
            this.pnlCanvasHost.Controls.Add(this.lblSurfaceNote);
            //
            // TopologyPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Name = "TopologyPage";
            this.Load += this.TopologyPage_Load;
            this.Size = new System.Drawing.Size(1400, 900);
            this.Text = "VisualOperationsStudio — Topology";
            this.Controls.Add(this.pnlCanvasHost);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlAppBar);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private VisualOperationsStudio.Controls.WindowGlyphs glyphs;
        private Wisej.Web.Panel pnlToolbar;
        private Wisej.Web.Label lblToolbarTitle;
        private Wisej.Web.Button btnFitToView;
        private Wisej.Web.Button btnResetZoom;
        private Wisej.Web.Button btnExport;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlCanvasHost;
        private Wisej.Web.Canvas canvasTopology;
        private Wisej.Web.Label lblSurfaceNote;
    }
}
