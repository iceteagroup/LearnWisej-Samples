namespace VisualOperationsStudio
{
    partial class CanvasPlaygroundPage
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
            this.btnRender = new Wisej.Web.Button();
            this.btnResizeSurface = new Wisej.Web.Button();
            this.btnProgressive = new Wisej.Web.Button();
            this.lblLiveUpdate = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlSurface = new Wisej.Web.Panel();
            this.canvasPlayground = new Wisej.Web.Canvas();
            this.SuspendLayout();
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Padding = new Wisej.Web.Padding(18, 0, 0, 0);
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAppTitle.Text = "VisualOperationsStudio — Canvas playground";
            //
            // glyphs
            //
            this.glyphs.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.glyphs.Dock = Wisej.Web.DockStyle.Right;
            this.glyphs.Inset = 18;
            this.glyphs.Name = "glyphs";
            this.glyphs.Size = new System.Drawing.Size(89, 40);
            //
            // pnlAppBar
            //
            this.pnlAppBar.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Size = new System.Drawing.Size(1400, 40);
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            this.pnlAppBar.Controls.Add(this.glyphs);
            //
            // btnRender
            //
            this.btnRender.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.btnRender.CssStyle = "border-radius:7px;border:none";
            this.btnRender.Font = new System.Drawing.Font("default", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnRender.ForeColor = System.Drawing.Color.White;
            this.btnRender.Location = new System.Drawing.Point(20, 6);
            this.btnRender.Name = "btnRender";
            this.btnRender.Size = new System.Drawing.Size(80, 26);
            this.btnRender.TabIndex = 0;
            this.btnRender.Text = "Render";
            this.btnRender.Click += this.btnRender_Click;
            //
            // btnResizeSurface
            //
            this.btnResizeSurface.BackColor = System.Drawing.Color.White;
            this.btnResizeSurface.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.btnResizeSurface.Font = new System.Drawing.Font("default", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnResizeSurface.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.btnResizeSurface.Location = new System.Drawing.Point(110, 6);
            this.btnResizeSurface.Name = "btnResizeSurface";
            this.btnResizeSurface.Size = new System.Drawing.Size(116, 26);
            this.btnResizeSurface.TabIndex = 1;
            this.btnResizeSurface.Text = "Resize surface";
            this.btnResizeSurface.Click += this.btnResizeSurface_Click;
            //
            // btnProgressive
            //
            this.btnProgressive.BackColor = System.Drawing.Color.White;
            this.btnProgressive.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.btnProgressive.Font = new System.Drawing.Font("default", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.btnProgressive.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.btnProgressive.Location = new System.Drawing.Point(236, 6);
            this.btnProgressive.Name = "btnProgressive";
            this.btnProgressive.Size = new System.Drawing.Size(178, 26);
            this.btnProgressive.TabIndex = 2;
            this.btnProgressive.Text = "Progressive (LiveUpdate)";
            this.btnProgressive.Click += this.btnProgressive_Click;
            //
            // lblLiveUpdate
            //
            this.lblLiveUpdate.AutoSize = false;
            this.lblLiveUpdate.Dock = Wisej.Web.DockStyle.Right;
            this.lblLiveUpdate.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblLiveUpdate.ForeColor = System.Drawing.Color.FromArgb(107, 125, 144);
            this.lblLiveUpdate.Name = "lblLiveUpdate";
            this.lblLiveUpdate.Padding = new Wisej.Web.Padding(0, 0, 20, 0);
            this.lblLiveUpdate.Size = new System.Drawing.Size(200, 38);
            this.lblLiveUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblLiveUpdate.Text = "LiveUpdate = false";
            //
            // pnlToolbar
            //
            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.pnlToolbar.CssStyle = "border-bottom:1px solid #e0e7ef";
            this.pnlToolbar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Size = new System.Drawing.Size(1400, 38);
            this.pnlToolbar.Controls.Add(this.btnRender);
            this.pnlToolbar.Controls.Add(this.btnResizeSurface);
            this.pnlToolbar.Controls.Add(this.btnProgressive);
            this.pnlToolbar.Controls.Add(this.lblLiveUpdate);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(16, 27, 43);
            this.lblStatus.CssStyle = "border-top:1px solid #24303f";
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("Consolas", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(143, 169, 196);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(20, 0, 20, 0);
            this.lblStatus.Size = new System.Drawing.Size(1400, 42);
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.Text = "Ready · DrawPlayground() wired to canvasPlayground.Redraw";
            //
            // canvasPlayground
            //
            this.canvasPlayground.BackColor = System.Drawing.Color.FromArgb(14, 21, 32);
            this.canvasPlayground.CssStyle = "border:1px solid #24303f;border-radius:8px";
            this.canvasPlayground.Dock = Wisej.Web.DockStyle.Fill;
            this.canvasPlayground.LiveUpdate = false;
            this.canvasPlayground.Name = "canvasPlayground";
            this.canvasPlayground.Redraw += this.canvasPlayground_Redraw;
            //
            // pnlSurface
            //
            this.pnlSurface.BackColor = System.Drawing.Color.FromArgb(11, 18, 32);
            this.pnlSurface.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlSurface.Name = "pnlSurface";
            this.pnlSurface.Padding = new Wisej.Web.Padding(20, 4, 20, 8);
            this.pnlSurface.Controls.Add(this.canvasPlayground);
            //
            // CanvasPlaygroundPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(11, 18, 32);
            this.Name = "CanvasPlaygroundPage";
            this.Load += this.CanvasPlaygroundPage_Load;
            this.Size = new System.Drawing.Size(1400, 900);
            this.Text = "VisualOperationsStudio — Canvas playground";
            this.Controls.Add(this.pnlSurface);
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
        private Wisej.Web.Button btnRender;
        private Wisej.Web.Button btnResizeSurface;
        private Wisej.Web.Button btnProgressive;
        private Wisej.Web.Label lblLiveUpdate;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlSurface;
        private Wisej.Web.Canvas canvasPlayground;
    }
}
