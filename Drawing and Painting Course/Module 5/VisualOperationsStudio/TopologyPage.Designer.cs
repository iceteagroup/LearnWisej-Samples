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
            this.lblStatus = new Wisej.Web.Label();
            this.pnlNodes = new Wisej.Web.Panel();
            this.lblNodesHeader = new Wisej.Web.Label();
            this.listNodes = new Wisej.Web.ListBox();
            this.pnlCanvasHost = new Wisej.Web.Panel();
            this.canvasTopology = new Wisej.Web.Canvas();
            this.pnlZoom = new Wisej.Web.Panel();
            this.btnZoomOut = new Wisej.Web.Button();
            this.btnZoomIn = new Wisej.Web.Button();
            this.btnFit = new Wisej.Web.Button();
            this.lblZoom = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Padding = new Wisej.Web.Padding(16, 0, 0, 0);
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAppTitle.Text = "VisualOperationsStudio — Plant topology";
            //
            // glyphs
            //
            this.glyphs.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.glyphs.Dock = Wisej.Web.DockStyle.Right;
            this.glyphs.Inset = 16;
            this.glyphs.Name = "glyphs";
            this.glyphs.Size = new System.Drawing.Size(85, 36);
            //
            // pnlAppBar
            //
            this.pnlAppBar.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Size = new System.Drawing.Size(1400, 36);
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            this.pnlAppBar.Controls.Add(this.glyphs);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.lblStatus.CssStyle = "border-top:1px solid #e4eaf1";
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("default", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.lblStatus.Size = new System.Drawing.Size(1400, 32);
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.Text = "Ready · click a node to select it";
            //
            // lblNodesHeader
            //
            this.lblNodesHeader.AutoSize = false;
            this.lblNodesHeader.CssStyle = "letter-spacing:.05em";
            this.lblNodesHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblNodesHeader.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblNodesHeader.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblNodesHeader.Name = "lblNodesHeader";
            this.lblNodesHeader.Padding = new Wisej.Web.Padding(6, 0, 6, 0);
            this.lblNodesHeader.Size = new System.Drawing.Size(166, 24);
            this.lblNodesHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNodesHeader.Text = "NODES (KEYBOARD)";
            //
            // listNodes
            //
            this.listNodes.AllowHtml = true;
            this.listNodes.BackColor = System.Drawing.Color.FromArgb(251, 252, 254);
            this.listNodes.BorderStyle = Wisej.Web.BorderStyle.None;
            this.listNodes.Dock = Wisej.Web.DockStyle.Fill;
            this.listNodes.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.listNodes.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.listNodes.ItemHeight = 30;
            this.listNodes.Name = "listNodes";
            this.listNodes.SelectedIndexChanged += this.listNodes_SelectedIndexChanged;
            //
            // pnlNodes
            //
            this.pnlNodes.BackColor = System.Drawing.Color.FromArgb(251, 252, 254);
            this.pnlNodes.CssStyle = "border-left:1px solid #e4eaf1";
            this.pnlNodes.Dock = Wisej.Web.DockStyle.Right;
            this.pnlNodes.Name = "pnlNodes";
            this.pnlNodes.Padding = new Wisej.Web.Padding(10, 10, 10, 10);
            this.pnlNodes.Size = new System.Drawing.Size(186, 400);
            this.pnlNodes.Controls.Add(this.listNodes);
            this.pnlNodes.Controls.Add(this.lblNodesHeader);
            //
            // canvasTopology
            //
            this.canvasTopology.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
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
            // btnZoomOut
            //
            this.btnZoomOut.BackColor = System.Drawing.Color.White;
            this.btnZoomOut.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.btnZoomOut.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnZoomOut.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.btnZoomOut.Location = new System.Drawing.Point(0, 0);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(34, 28);
            this.btnZoomOut.TabIndex = 1;
            this.btnZoomOut.Text = "−";
            this.btnZoomOut.Click += this.btnZoomOut_Click;
            //
            // btnZoomIn
            //
            this.btnZoomIn.BackColor = System.Drawing.Color.White;
            this.btnZoomIn.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.btnZoomIn.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnZoomIn.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.btnZoomIn.Location = new System.Drawing.Point(42, 0);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(34, 28);
            this.btnZoomIn.TabIndex = 2;
            this.btnZoomIn.Text = "+";
            this.btnZoomIn.Click += this.btnZoomIn_Click;
            //
            // btnFit
            //
            this.btnFit.BackColor = System.Drawing.Color.White;
            this.btnFit.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.btnFit.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnFit.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.btnFit.Location = new System.Drawing.Point(84, 0);
            this.btnFit.Name = "btnFit";
            this.btnFit.Size = new System.Drawing.Size(44, 28);
            this.btnFit.TabIndex = 3;
            this.btnFit.Text = "Fit";
            this.btnFit.Click += this.btnFit_Click;
            //
            // lblZoom
            //
            this.lblZoom.AutoSize = false;
            this.lblZoom.BackColor = System.Drawing.Color.White;
            this.lblZoom.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.lblZoom.Font = new System.Drawing.Font("Consolas", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblZoom.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblZoom.Location = new System.Drawing.Point(136, 0);
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.Size = new System.Drawing.Size(62, 28);
            this.lblZoom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblZoom.Text = "0.72x";
            //
            // pnlZoom
            //
            this.pnlZoom.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlZoom.BackColor = System.Drawing.Color.Transparent;
            this.pnlZoom.Location = new System.Drawing.Point(12, 360);
            this.pnlZoom.Name = "pnlZoom";
            this.pnlZoom.Size = new System.Drawing.Size(200, 28);
            this.pnlZoom.Controls.Add(this.btnZoomOut);
            this.pnlZoom.Controls.Add(this.btnZoomIn);
            this.pnlZoom.Controls.Add(this.btnFit);
            this.pnlZoom.Controls.Add(this.lblZoom);
            //
            // pnlCanvasHost
            //
            this.pnlCanvasHost.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.pnlCanvasHost.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlCanvasHost.Name = "pnlCanvasHost";
            this.pnlCanvasHost.Controls.Add(this.canvasTopology);
            this.pnlCanvasHost.Controls.Add(this.pnlZoom);
            //
            // TopologyPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Name = "TopologyPage";
            this.Load += this.TopologyPage_Load;
            this.Size = new System.Drawing.Size(1400, 900);
            this.Text = "VisualOperationsStudio — Plant topology";
            this.Controls.Add(this.pnlCanvasHost);
            this.Controls.Add(this.pnlNodes);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlAppBar);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private VisualOperationsStudio.Controls.WindowGlyphs glyphs;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlNodes;
        private Wisej.Web.Label lblNodesHeader;
        private Wisej.Web.ListBox listNodes;
        private Wisej.Web.Panel pnlCanvasHost;
        private Wisej.Web.Canvas canvasTopology;
        private Wisej.Web.Panel pnlZoom;
        private Wisej.Web.Button btnZoomOut;
        private Wisej.Web.Button btnZoomIn;
        private Wisej.Web.Button btnFit;
        private Wisej.Web.Label lblZoom;
    }
}
