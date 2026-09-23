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

        //
        // Everything in this region is what the designer Image Selector writes. The picker never
        // produces an Image object for these controls - it produces the ImageSource strings below,
        // which is why they are readable, diffable and reviewable in a pull request.
        //
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.lblSubtitle = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnBack = new Wisej.Web.Button();
            this.cboTheme = new Wisej.Web.ComboBox();
            this.btnRecolour = new Wisej.Web.Button();
            this.btnReport = new Wisej.Web.Button();
            this.pnlBody = new Wisej.Web.Panel();
            this.lblGalleryHeader = new Wisej.Web.Label();
            this.layoutGallery = new Wisej.Web.TableLayoutPanel();
            this.picTheme = new Wisej.Web.PictureBox();
            this.lblTheme = new Wisej.Web.Label();
            this.picProject = new Wisej.Web.PictureBox();
            this.lblProject = new Wisej.Web.Label();
            this.picAbsolute = new Wisej.Web.PictureBox();
            this.lblAbsolute = new Wisej.Web.Label();
            this.picRecoloured = new Wisej.Web.PictureBox();
            this.lblRecoloured = new Wisej.Web.Label();
            this.picThemeSecond = new Wisej.Web.PictureBox();
            this.lblThemeSecond = new Wisej.Web.Label();
            this.lblReport = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(360, 28);
            this.lblTitle.Text = "Icon gallery";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblSubtitle.Location = new System.Drawing.Point(20, 42);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(900, 22);
            this.lblSubtitle.Text = "Five image sources of five different kinds. Switch the theme and watch which ones change.";
            //
            // pnlHeader
            //
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1000, 72);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(20, 8, 20, 8);
            this.lblStatus.Size = new System.Drawing.Size(1000, 34);
            this.lblStatus.Text = "Ready.";
            //
            // btnBack
            //
            this.btnBack.Location = new System.Drawing.Point(20, 9);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(170, 38);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "Back to commands";
            this.btnBack.Click += this.btnBack_Click;
            //
            // cboTheme
            //
            this.cboTheme.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTheme.Location = new System.Drawing.Point(202, 9);
            this.cboTheme.Name = "cboTheme";
            this.cboTheme.Size = new System.Drawing.Size(200, 38);
            this.cboTheme.TabIndex = 1;
            this.cboTheme.SelectedIndexChanged += this.cboTheme_SelectedIndexChanged;
            //
            // btnRecolour
            //
            this.btnRecolour.Location = new System.Drawing.Point(414, 9);
            this.btnRecolour.Name = "btnRecolour";
            this.btnRecolour.Size = new System.Drawing.Size(230, 38);
            this.btnRecolour.TabIndex = 2;
            this.btnRecolour.Text = "Cycle the colour suffix";
            this.btnRecolour.Click += this.btnRecolour_Click;
            //
            // btnReport
            //
            this.btnReport.Location = new System.Drawing.Point(656, 9);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(230, 38);
            this.btnReport.TabIndex = 3;
            this.btnReport.Text = "Report the image sources";
            this.btnReport.Click += this.btnReport_Click;
            //
            // pnlActions
            //
            this.pnlActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1000, 56);
            this.pnlActions.Controls.Add(this.btnReport);
            this.pnlActions.Controls.Add(this.btnRecolour);
            this.pnlActions.Controls.Add(this.cboTheme);
            this.pnlActions.Controls.Add(this.btnBack);
            //
            // lblGalleryHeader
            //
            this.lblGalleryHeader.AutoSize = false;
            this.lblGalleryHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblGalleryHeader.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblGalleryHeader.ForeColor = System.Drawing.Color.FromArgb(123, 139, 156);
            this.lblGalleryHeader.Name = "lblGalleryHeader";
            this.lblGalleryHeader.Size = new System.Drawing.Size(960, 26);
            this.lblGalleryHeader.Text = "Five controls, five kinds of image source";
            //
            // picTheme - a NAMED THEME IMAGE. No path, no extension: the client looks the name up
            // in the theme it has already loaded, so this one follows a theme switch.
            //
            this.picTheme.ImageSource = "icon-search";
            this.picTheme.Name = "picTheme";
            this.picTheme.Size = new System.Drawing.Size(48, 48);
            this.picTheme.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.lblTheme.AutoSize = false;
            this.lblTheme.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(180, 40);
            this.lblTheme.Text = "Theme image\r\n\"icon-search\"";
            //
            // picProject - a RELATIVE URL to a file deployed beside the application.
            //
            this.picProject.ImageSource = "Images/logo.svg";
            this.picProject.Name = "picProject";
            this.picProject.Size = new System.Drawing.Size(48, 48);
            this.picProject.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.lblProject.AutoSize = false;
            this.lblProject.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblProject.Name = "lblProject";
            this.lblProject.Size = new System.Drawing.Size(180, 40);
            this.lblProject.Text = "Project SVG\r\n\"Images/logo.svg\"";
            //
            // picAbsolute - an ABSOLUTE URL. Assigned in code so the lab works on any port; a real
            // project would have the designer write the literal address.
            //
            this.picAbsolute.Name = "picAbsolute";
            this.picAbsolute.Size = new System.Drawing.Size(48, 48);
            this.picAbsolute.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.lblAbsolute.AutoSize = false;
            this.lblAbsolute.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblAbsolute.Name = "lblAbsolute";
            this.lblAbsolute.Size = new System.Drawing.Size(180, 40);
            this.lblAbsolute.Text = "Absolute URL\r\n(built from Application.Url)";
            //
            // picRecoloured - the same monochrome SVG, with the ?color= suffix naming a THEME
            // COLOUR rather than a literal. It therefore follows a theme switch as well.
            //
            this.picRecoloured.ImageSource = "Images/pin.svg?color=highlight";
            this.picRecoloured.Name = "picRecoloured";
            this.picRecoloured.Size = new System.Drawing.Size(48, 48);
            this.picRecoloured.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.lblRecoloured.AutoSize = false;
            this.lblRecoloured.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblRecoloured.Name = "lblRecoloured";
            this.lblRecoloured.Size = new System.Drawing.Size(180, 40);
            this.lblRecoloured.Text = "Recoloured SVG\r\n\"...pin.svg?color=highlight\"";
            //
            // picThemeSecond - a second theme image, so the theme switch has two things to move.
            //
            this.picThemeSecond.ImageSource = "icon-settings";
            this.picThemeSecond.Name = "picThemeSecond";
            this.picThemeSecond.Size = new System.Drawing.Size(48, 48);
            this.picThemeSecond.SizeMode = Wisej.Web.PictureBoxSizeMode.Zoom;
            this.lblThemeSecond.AutoSize = false;
            this.lblThemeSecond.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblThemeSecond.Name = "lblThemeSecond";
            this.lblThemeSecond.Size = new System.Drawing.Size(180, 40);
            this.lblThemeSecond.Text = "Theme image\r\n\"icon-settings\"";
            //
            // layoutGallery
            //
            this.layoutGallery.ColumnCount = 5;
            this.layoutGallery.RowCount = 2;
            this.layoutGallery.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutGallery.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutGallery.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutGallery.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutGallery.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutGallery.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 70F));
            this.layoutGallery.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 46F));
            this.layoutGallery.Dock = Wisej.Web.DockStyle.Top;
            this.layoutGallery.Name = "layoutGallery";
            this.layoutGallery.Size = new System.Drawing.Size(960, 120);
            this.layoutGallery.Controls.Add(this.picTheme, 0, 0);
            this.layoutGallery.Controls.Add(this.picProject, 1, 0);
            this.layoutGallery.Controls.Add(this.picAbsolute, 2, 0);
            this.layoutGallery.Controls.Add(this.picRecoloured, 3, 0);
            this.layoutGallery.Controls.Add(this.picThemeSecond, 4, 0);
            this.layoutGallery.Controls.Add(this.lblTheme, 0, 1);
            this.layoutGallery.Controls.Add(this.lblProject, 1, 1);
            this.layoutGallery.Controls.Add(this.lblAbsolute, 2, 1);
            this.layoutGallery.Controls.Add(this.lblRecoloured, 3, 1);
            this.layoutGallery.Controls.Add(this.lblThemeSecond, 4, 1);
            //
            // lblReport
            //
            this.lblReport.AllowHtml = true;
            this.lblReport.AutoSize = false;
            this.lblReport.Dock = Wisej.Web.DockStyle.Fill;
            this.lblReport.Name = "lblReport";
            this.lblReport.Padding = new Wisej.Web.Padding(0, 14, 0, 0);
            this.lblReport.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // pnlBody
            //
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new Wisej.Web.Padding(20);
            this.pnlBody.Controls.Add(this.lblReport);
            this.pnlBody.Controls.Add(this.layoutGallery);
            this.pnlBody.Controls.Add(this.lblGalleryHeader);
            //
            // IconGalleryPage
            //
            this.Name = "IconGalleryPage";
            this.Size = new System.Drawing.Size(1000, 640);
            this.Text = "IconDesk - Icon gallery";
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblSubtitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnBack;
        private Wisej.Web.ComboBox cboTheme;
        private Wisej.Web.Button btnRecolour;
        private Wisej.Web.Button btnReport;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblGalleryHeader;
        private Wisej.Web.TableLayoutPanel layoutGallery;
        private Wisej.Web.PictureBox picTheme;
        private Wisej.Web.Label lblTheme;
        private Wisej.Web.PictureBox picProject;
        private Wisej.Web.Label lblProject;
        private Wisej.Web.PictureBox picAbsolute;
        private Wisej.Web.Label lblAbsolute;
        private Wisej.Web.PictureBox picRecoloured;
        private Wisej.Web.Label lblRecoloured;
        private Wisej.Web.PictureBox picThemeSecond;
        private Wisej.Web.Label lblThemeSecond;
        private Wisej.Web.Label lblReport;
    }
}
