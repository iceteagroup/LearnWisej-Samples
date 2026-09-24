namespace IconDesk
{
    partial class IconPackPage
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
            this.appTitleBar = new IconDesk.AppTitleBar();
            this.pnlScroll = new Wisej.Web.Panel();
            this.pnlToolbar = new Wisej.Web.Panel();
            this.lblView = new Wisej.Web.Label();
            this.lblPackVersion = new Wisej.Web.Label();
            this.btnTheme = new Wisej.Web.Button();
            this.layoutGrid = new Wisej.Web.TableLayoutPanel();
            this.pnlGalleryStatus = new Wisej.Web.Panel();
            this.lblGalleryGlyph = new Wisej.Web.Label();
            this.lblGalleryStatus = new Wisej.Web.Label();
            this.pnlCheckHeader = new Wisej.Web.Panel();
            this.lblCheckTitle = new Wisej.Web.Label();
            this.pnlRecolour = new Wisej.Web.Panel();
            this.pnlStatus = new Wisej.Web.Panel();
            this.lblStatusGlyph = new Wisej.Web.Label();
            this.lblPackStatus = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // appTitleBar
            //
            this.appTitleBar.Size = new System.Drawing.Size(1000, 38);
            this.appTitleBar.Title = "IconDesk — Pack Gallery";
            //
            // lblView - the section's own name, exactly as the toolbar shows it
            //
            this.lblView.AutoSize = false;
            this.lblView.Font = new System.Drawing.Font("default", 11.3F, System.Drawing.FontStyle.Bold);
            this.lblView.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblView.Location = new System.Drawing.Point(16, 11);
            this.lblView.Name = "lblView";
            this.lblView.Size = new System.Drawing.Size(150, 30);
            this.lblView.Text = "Pack gallery";
            this.lblView.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblPackVersion
            //
            this.lblPackVersion.AutoSize = false;
            this.lblPackVersion.BackColor = System.Drawing.Color.White;
            this.lblPackVersion.CssStyle = "border:1px solid #cdd9e6;border-radius:7px;";
            this.lblPackVersion.Font = new System.Drawing.Font("default", 9.8F, System.Drawing.FontStyle.Bold);
            this.lblPackVersion.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblPackVersion.Location = new System.Drawing.Point(178, 12);
            this.lblPackVersion.Name = "lblPackVersion";
            this.lblPackVersion.Size = new System.Drawing.Size(170, 28);
            this.lblPackVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnTheme
            //
            this.btnTheme.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnTheme.BackColor = System.Drawing.Color.White;
            this.btnTheme.CssStyle = "border:1px solid #cdd9e6;border-radius:7px;";
            this.btnTheme.Font = new System.Drawing.Font("default", 9.8F, System.Drawing.FontStyle.Bold);
            this.btnTheme.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.btnTheme.Location = new System.Drawing.Point(844, 12);
            this.btnTheme.Name = "btnTheme";
            this.btnTheme.Size = new System.Drawing.Size(140, 28);
            this.btnTheme.TabIndex = 0;
            this.btnTheme.Text = "Light theme";
            this.btnTheme.Click += this.btnTheme_Click;
            //
            // pnlToolbar
            //
            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(251, 252, 254);
            this.pnlToolbar.CssStyle = "border-bottom:1px solid #e3e9f0;";
            this.pnlToolbar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Size = new System.Drawing.Size(1000, 52);
            this.pnlToolbar.Controls.Add(this.lblView);
            this.pnlToolbar.Controls.Add(this.lblPackVersion);
            this.pnlToolbar.Controls.Add(this.btnTheme);
            //
            // layoutGrid - the pack gallery, four tiles per row
            //
            this.layoutGrid.BackColor = System.Drawing.Color.FromArgb(246, 248, 251);
            this.layoutGrid.ColumnCount = 4;
            this.layoutGrid.RowCount = 3;
            for (var column = 0; column < 4; column++)
                this.layoutGrid.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 25F));
            for (var row = 0; row < 3; row++)
                this.layoutGrid.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 112F));
            this.layoutGrid.Dock = Wisej.Web.DockStyle.Top;
            this.layoutGrid.Name = "layoutGrid";
            this.layoutGrid.Padding = new Wisej.Web.Padding(18, 16, 18, 4);
            this.layoutGrid.Size = new System.Drawing.Size(1000, 356);
            //
            // pnlGalleryStatus - what the gallery loaded
            //
            this.lblGalleryGlyph.AutoSize = false;
            this.lblGalleryGlyph.Font = new System.Drawing.Font("default", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblGalleryGlyph.ForeColor = System.Drawing.Color.FromArgb(22, 119, 77);
            this.lblGalleryGlyph.Location = new System.Drawing.Point(16, 0);
            this.lblGalleryGlyph.Name = "lblGalleryGlyph";
            this.lblGalleryGlyph.Size = new System.Drawing.Size(16, 46);
            this.lblGalleryGlyph.Text = "✓";
            this.lblGalleryGlyph.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblGalleryStatus.AutoSize = false;
            this.lblGalleryStatus.Dock = Wisej.Web.DockStyle.Fill;
            this.lblGalleryStatus.Font = new System.Drawing.Font("default", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblGalleryStatus.ForeColor = System.Drawing.Color.FromArgb(22, 119, 77);
            this.lblGalleryStatus.Name = "lblGalleryStatus";
            this.lblGalleryStatus.Padding = new Wisej.Web.Padding(36, 0, 16, 0);
            this.lblGalleryStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.pnlGalleryStatus.BackColor = System.Drawing.Color.FromArgb(236, 248, 241);
            this.pnlGalleryStatus.CssStyle = "border-top:1px solid #a9e0c4;";
            this.pnlGalleryStatus.Dock = Wisej.Web.DockStyle.Top;
            this.pnlGalleryStatus.Name = "pnlGalleryStatus";
            this.pnlGalleryStatus.Size = new System.Drawing.Size(1000, 46);
            this.pnlGalleryStatus.Controls.Add(this.lblGalleryStatus);
            this.pnlGalleryStatus.Controls.Add(this.lblGalleryGlyph);
            //
            // pnlCheckHeader - the second section's toolbar
            //
            this.lblCheckTitle.AutoSize = false;
            this.lblCheckTitle.Font = new System.Drawing.Font("default", 11.3F, System.Drawing.FontStyle.Bold);
            this.lblCheckTitle.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblCheckTitle.Location = new System.Drawing.Point(16, 11);
            this.lblCheckTitle.Name = "lblCheckTitle";
            this.lblCheckTitle.Size = new System.Drawing.Size(400, 30);
            this.lblCheckTitle.Text = "Recolour check";
            this.lblCheckTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.pnlCheckHeader.BackColor = System.Drawing.Color.FromArgb(251, 252, 254);
            this.pnlCheckHeader.CssStyle = "border-bottom:1px solid #e3e9f0;";
            this.pnlCheckHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlCheckHeader.Name = "pnlCheckHeader";
            this.pnlCheckHeader.Size = new System.Drawing.Size(1000, 52);
            this.pnlCheckHeader.Controls.Add(this.lblCheckTitle);
            //
            // pnlRecolour - three cards side by side
            //
            this.pnlRecolour.BackColor = System.Drawing.Color.FromArgb(246, 248, 251);
            this.pnlRecolour.Dock = Wisej.Web.DockStyle.Top;
            this.pnlRecolour.Name = "pnlRecolour";
            this.pnlRecolour.Size = new System.Drawing.Size(1000, 214);
            //
            // pnlScroll
            //
            this.pnlScroll.AutoScroll = true;
            this.pnlScroll.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlScroll.Name = "pnlScroll";
            this.pnlScroll.Controls.Add(this.pnlRecolour);
            this.pnlScroll.Controls.Add(this.pnlCheckHeader);
            this.pnlScroll.Controls.Add(this.pnlGalleryStatus);
            this.pnlScroll.Controls.Add(this.layoutGrid);
            this.pnlScroll.Controls.Add(this.pnlToolbar);
            //
            // pnlStatus - the recolour verdict
            //
            this.lblStatusGlyph.AutoSize = false;
            this.lblStatusGlyph.Font = new System.Drawing.Font("default", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblStatusGlyph.Location = new System.Drawing.Point(16, 0);
            this.lblStatusGlyph.Name = "lblStatusGlyph";
            this.lblStatusGlyph.Size = new System.Drawing.Size(16, 46);
            this.lblStatusGlyph.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblPackStatus.AutoSize = false;
            this.lblPackStatus.Dock = Wisej.Web.DockStyle.Fill;
            this.lblPackStatus.Font = new System.Drawing.Font("default", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblPackStatus.Name = "lblPackStatus";
            this.lblPackStatus.Padding = new Wisej.Web.Padding(36, 0, 16, 0);
            this.lblPackStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.pnlStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(1000, 46);
            this.pnlStatus.Controls.Add(this.lblPackStatus);
            this.pnlStatus.Controls.Add(this.lblStatusGlyph);
            //
            // IconPackPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Name = "IconPackPage";
            this.Size = new System.Drawing.Size(1000, 640);
            this.Text = "IconDesk — Pack Gallery";
            this.Controls.Add(this.pnlScroll);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.appTitleBar);
            this.ResumeLayout(false);
        }

        #endregion

        private IconDesk.AppTitleBar appTitleBar;
        private Wisej.Web.Panel pnlScroll;
        private Wisej.Web.Panel pnlToolbar;
        private Wisej.Web.Label lblView;
        private Wisej.Web.Label lblPackVersion;
        private Wisej.Web.Button btnTheme;
        private Wisej.Web.TableLayoutPanel layoutGrid;
        private Wisej.Web.Panel pnlGalleryStatus;
        private Wisej.Web.Label lblGalleryGlyph;
        private Wisej.Web.Label lblGalleryStatus;
        private Wisej.Web.Panel pnlCheckHeader;
        private Wisej.Web.Label lblCheckTitle;
        private Wisej.Web.Panel pnlRecolour;
        private Wisej.Web.Panel pnlStatus;
        private Wisej.Web.Label lblStatusGlyph;
        private Wisej.Web.Label lblPackStatus;
    }
}
