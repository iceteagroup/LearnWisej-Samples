using System.Drawing;
using Wisej.Web;

namespace IconDesk
{
    /// <summary>
    /// One IconGallery slot: the picture, the caption naming the mechanism, and the source string
    /// exactly as it appears in <c>IconGallery.Designer.cs</c>. After a theme switch it also
    /// carries the verdict - whether this slot followed the theme or stayed as drawn.
    /// </summary>
    public class GallerySlot : Panel
    {
        private readonly Label lblCaption = new Label();
        private readonly Label lblSource = new Label();
        private readonly Label lblVerdict = new Label();

        /// <summary>The control the lab names - picThemeImage, picProjectSvg and so on.</summary>
        public PictureBox Picture { get; } = new PictureBox();

        public GallerySlot()
        {
            this.Picture.Location = new Point(0, 12);
            this.Picture.Size = new Size(42, 42);
            this.Picture.SizeMode = PictureBoxSizeMode.Zoom;

            this.lblCaption.AutoSize = false;
            this.lblCaption.Font = new Font("default", 9.4F, FontStyle.Bold);
            this.lblCaption.Location = new Point(10, 66);
            this.lblCaption.TextAlign = ContentAlignment.TopCenter;

            this.lblSource.AutoSize = false;
            this.lblSource.Font = new Font("Consolas", 7.9F);
            this.lblSource.Location = new Point(10, 104);
            this.lblSource.TextAlign = ContentAlignment.TopCenter;

            this.lblVerdict.AutoSize = false;
            this.lblVerdict.CssStyle = "letter-spacing:.04em;";
            this.lblVerdict.Font = new Font("default", 7.9F, FontStyle.Bold);
            this.lblVerdict.Location = new Point(10, 150);
            this.lblVerdict.TextAlign = ContentAlignment.TopCenter;
            this.lblVerdict.Visible = false;

            this.Controls.Add(this.Picture);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.lblSource);
            this.Controls.Add(this.lblVerdict);
        }

        /// <summary>The mechanism this slot is here to show, in the learner's words.</summary>
        public string Caption
        {
            get { return this.lblCaption.Text; }
            set { this.lblCaption.Text = value; }
        }

        /// <summary>The source string as the card prints it - elided when it is a long one.</summary>
        public string SourceText
        {
            get { return this.lblSource.Text; }
            set { this.lblSource.Text = value; }
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);

            var width = this.ClientSize.Width;

            this.Picture.Left = (width - this.Picture.Width) / 2;
            this.lblCaption.Size = new Size(width - 20, 34);
            this.lblSource.Size = new Size(width - 20, 44);
            this.lblVerdict.Size = new Size(width - 20, 18);
        }

        /// <summary>Repaints the card in the palette of the theme now loaded.</summary>
        public void ApplyPalette(GalleryPalette palette, bool followedTheTheme, bool showVerdict)
        {
            var border = showVerdict && followedTheTheme ? palette.Active : palette.Line;

            this.BackColor = palette.Panel;
            this.CssStyle = "border:1.5px solid " + GalleryPalette.Css(border) + ";border-radius:10px;";

            this.lblCaption.ForeColor = palette.Ink;
            this.lblSource.ForeColor = palette.Sub;

            this.lblVerdict.Visible = showVerdict;
            this.lblVerdict.ForeColor = followedTheTheme ? palette.Active : palette.Sub;
            this.lblVerdict.Text = followedTheTheme ? "FOLLOWED THE THEME" : "UNCHANGED";
        }
    }
}
