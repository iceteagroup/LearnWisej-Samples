namespace IconDesk
{
    partial class IconGalleryPage
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
            this.pnlBody = new Wisej.Web.Panel();
            this.pnlTheme = new Wisej.Web.Panel();
            this.lblThemeCaption = new Wisej.Web.Label();
            this.btnLightTheme = new Wisej.Web.Button();
            this.btnDarkTheme = new Wisej.Web.Button();
            this.layoutSlots = new Wisej.Web.TableLayoutPanel();
            this.slotThemeImage = new IconDesk.GallerySlot();
            this.slotProjectSvg = new IconDesk.GallerySlot();
            this.slotAbsoluteUrl = new IconDesk.GallerySlot();
            this.slotPackOne = new IconDesk.GallerySlot();
            this.slotPackTwo = new IconDesk.GallerySlot();
            this.SuspendLayout();
            //
            // appTitleBar
            //
            this.appTitleBar.Title = "IconDesk — IconGallery";
            //
            // the theme switch
            //
            this.lblThemeCaption.AutoSize = false;
            this.lblThemeCaption.CssStyle = "letter-spacing:.05em;";
            this.lblThemeCaption.Font = new System.Drawing.Font("default", 9.4F, System.Drawing.FontStyle.Bold);
            this.lblThemeCaption.Location = new System.Drawing.Point(0, 5);
            this.lblThemeCaption.Name = "lblThemeCaption";
            this.lblThemeCaption.Size = new System.Drawing.Size(60, 20);
            this.lblThemeCaption.Text = "THEME";
            this.lblThemeCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.btnLightTheme.Font = new System.Drawing.Font("default", 9.4F, System.Drawing.FontStyle.Bold);
            this.btnLightTheme.Location = new System.Drawing.Point(70, 0);
            this.btnLightTheme.Name = "btnLightTheme";
            this.btnLightTheme.Size = new System.Drawing.Size(110, 28);
            this.btnLightTheme.TabIndex = 0;
            this.btnLightTheme.Text = "Light theme";
            this.btnLightTheme.Click += this.btnLightTheme_Click;

            this.btnDarkTheme.Font = new System.Drawing.Font("default", 9.4F, System.Drawing.FontStyle.Bold);
            this.btnDarkTheme.Location = new System.Drawing.Point(190, 0);
            this.btnDarkTheme.Name = "btnDarkTheme";
            this.btnDarkTheme.Size = new System.Drawing.Size(110, 28);
            this.btnDarkTheme.TabIndex = 1;
            this.btnDarkTheme.Text = "Dark theme";
            this.btnDarkTheme.Click += this.btnDarkTheme_Click;
            //
            // pnlTheme
            //
            this.pnlTheme.Dock = Wisej.Web.DockStyle.Top;
            this.pnlTheme.Name = "pnlTheme";
            this.pnlTheme.Size = new System.Drawing.Size(960, 42);
            this.pnlTheme.Controls.Add(this.lblThemeCaption);
            this.pnlTheme.Controls.Add(this.btnLightTheme);
            this.pnlTheme.Controls.Add(this.btnDarkTheme);
            //
            // the five gallery slots - five mechanisms, one row
            //
            this.slotThemeImage.Caption = "Named theme image";
            this.slotThemeImage.Dock = Wisej.Web.DockStyle.Fill;
            this.slotThemeImage.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.slotThemeImage.Name = "slotThemeImage";
            this.slotThemeImage.Picture.Name = "picThemeImage";

            this.slotProjectSvg.Caption = "Relative URL";
            this.slotProjectSvg.Dock = Wisej.Web.DockStyle.Fill;
            this.slotProjectSvg.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.slotProjectSvg.Name = "slotProjectSvg";
            this.slotProjectSvg.Picture.Name = "picProjectSvg";

            this.slotAbsoluteUrl.Caption = "Absolute URL";
            this.slotAbsoluteUrl.Dock = Wisej.Web.DockStyle.Fill;
            this.slotAbsoluteUrl.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.slotAbsoluteUrl.Name = "slotAbsoluteUrl";
            this.slotAbsoluteUrl.Picture.Name = "picAbsoluteUrl";

            this.slotPackOne.Caption = "Pack resource";
            this.slotPackOne.Dock = Wisej.Web.DockStyle.Fill;
            this.slotPackOne.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.slotPackOne.Name = "slotPackOne";
            this.slotPackOne.Picture.Name = "picPackOne";

            this.slotPackTwo.Caption = "Recoloured SVG";
            this.slotPackTwo.Dock = Wisej.Web.DockStyle.Fill;
            this.slotPackTwo.Name = "slotPackTwo";
            this.slotPackTwo.Picture.Name = "picPackTwo";
            //
            // layoutSlots
            //
            this.layoutSlots.ColumnCount = 5;
            this.layoutSlots.RowCount = 1;
            this.layoutSlots.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutSlots.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutSlots.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutSlots.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutSlots.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutSlots.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 180F));
            this.layoutSlots.Dock = Wisej.Web.DockStyle.Top;
            this.layoutSlots.Name = "layoutSlots";
            this.layoutSlots.Size = new System.Drawing.Size(960, 180);
            this.layoutSlots.Controls.Add(this.slotThemeImage, 0, 0);
            this.layoutSlots.Controls.Add(this.slotProjectSvg, 1, 0);
            this.layoutSlots.Controls.Add(this.slotAbsoluteUrl, 2, 0);
            this.layoutSlots.Controls.Add(this.slotPackOne, 3, 0);
            this.layoutSlots.Controls.Add(this.slotPackTwo, 4, 0);
            //
            // pnlBody
            //
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new Wisej.Web.Padding(18, 16, 18, 18);
            this.pnlBody.Controls.Add(this.layoutSlots);
            this.pnlBody.Controls.Add(this.pnlTheme);
            //
            // IconGalleryPage
            //
            this.Name = "IconGalleryPage";
            this.Size = new System.Drawing.Size(1000, 640);
            this.Text = "IconDesk — IconGallery";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.appTitleBar);
            this.ResumeLayout(false);
        }

        #endregion

        private IconDesk.AppTitleBar appTitleBar;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Panel pnlTheme;
        private Wisej.Web.Label lblThemeCaption;
        private Wisej.Web.Button btnLightTheme;
        private Wisej.Web.Button btnDarkTheme;
        private Wisej.Web.TableLayoutPanel layoutSlots;
        private IconDesk.GallerySlot slotThemeImage;
        private IconDesk.GallerySlot slotProjectSvg;
        private IconDesk.GallerySlot slotAbsoluteUrl;
        private IconDesk.GallerySlot slotPackOne;
        private IconDesk.GallerySlot slotPackTwo;
    }
}
