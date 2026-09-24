using System.Drawing;
using Wisej.Web;

namespace IconDesk
{
    /// <summary>
    /// One ResourceLab tile: the asset, its file name and the shape of the URL it came from.
    /// A tile has three states - served from the assembly, served from a file deployed beside the
    /// application, and not found at all.
    /// </summary>
    public class AssetTile : Panel
    {
        /// <summary>What the tile is showing.</summary>
        public enum TileState
        {
            /// <summary>Served from the assembly, through resource.wx.</summary>
            Embedded,

            /// <summary>A file beside the application answered the same URL.</summary>
            Overridden,

            /// <summary>The URL resolved to nothing. No exception, no log entry, no picture.</summary>
            Missing,
        }

        private static readonly Color Ink = Color.FromArgb(0x1F, 0x2D, 0x3A);
        private static readonly Color Bad = Color.FromArgb(0xC0, 0x39, 0x2B);

        private readonly Label lblFile = new Label();
        private readonly Label lblSource = new Label();
        private readonly Label lblMissing = new Label();

        /// <summary>The control the lab names - picLogo, picOk and so on.</summary>
        public PictureBox Picture { get; } = new PictureBox();

        public AssetTile()
        {
            this.BackColor = Color.White;

            this.Picture.Location = new Point(0, 12);
            this.Picture.Size = new Size(58, 58);
            this.Picture.SizeMode = PictureBoxSizeMode.Zoom;

            // Stands in for the picture when the URL resolves to nothing, so the failure is
            // visible to the user instead of being an empty rectangle.
            this.lblMissing.AutoSize = false;
            this.lblMissing.CssStyle = "border:2px dashed #d64545;border-radius:8px;";
            this.lblMissing.Font = new Font("Consolas", 11.3F, FontStyle.Bold);
            this.lblMissing.ForeColor = Bad;
            this.lblMissing.Location = new Point(0, 12);
            this.lblMissing.Size = new Size(58, 58);
            this.lblMissing.Text = "404";
            this.lblMissing.TextAlign = ContentAlignment.MiddleCenter;
            this.lblMissing.Visible = false;

            this.lblFile.AutoSize = false;
            this.lblFile.Font = new Font("Consolas", 9.4F, FontStyle.Bold);
            this.lblFile.ForeColor = Ink;
            this.lblFile.Location = new Point(6, 82);
            this.lblFile.TextAlign = ContentAlignment.TopCenter;

            this.lblSource.AutoSize = false;
            this.lblSource.Font = new Font("Consolas", 7.9F, FontStyle.Bold);
            this.lblSource.Location = new Point(6, 106);
            this.lblSource.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(this.Picture);
            this.Controls.Add(this.lblMissing);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.lblSource);
        }

        /// <summary>The file name, as it is written in the project and in the URL.</summary>
        public string FileName
        {
            get { return this.lblFile.Text; }
            set { this.lblFile.Text = value; }
        }

        /// <summary>The shape of the URL, elided the way the lesson writes it.</summary>
        public string SourceShape { get; set; } = "resource.wx/…";

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);

            var width = this.ClientSize.Width;

            this.Picture.Left = (width - this.Picture.Width) / 2;
            this.lblMissing.Left = this.Picture.Left;
            this.lblFile.Size = new Size(width - 12, 22);
            this.lblSource.Size = new Size(width - 12, 20);
        }

        /// <summary>Repaints the tile for the state it is now in.</summary>
        public void Show(TileState state)
        {
            this.Picture.Visible = state != TileState.Missing;
            this.lblMissing.Visible = state == TileState.Missing;

            switch (state)
            {
                case TileState.Missing:
                    this.BackColor = Color.FromArgb(0xFD, 0xF4, 0xF3);
                    this.CssStyle = "border:1.5px solid #d64545;border-radius:10px;";
                    this.lblFile.ForeColor = Bad;
                    Pill(this.SourceShape, "#c0392b", "#fbe4e1", "#f0b4ac");
                    break;

                case TileState.Overridden:
                    this.BackColor = Color.White;
                    this.CssStyle = "border:1.5px solid #7d5ae0;border-radius:10px;";
                    this.lblFile.ForeColor = Ink;
                    Pill("deployed file wins", "#5b3fa8", "#f1ecfd", "#d4c6f7");
                    break;

                default:
                    this.BackColor = Color.White;
                    this.CssStyle = "border:1.5px solid #e0e7ef;border-radius:10px;";
                    this.lblFile.ForeColor = Ink;
                    Pill(this.SourceShape, "#1565d8", "#eaf3ff", "#c5ddff");
                    break;
            }
        }

        private void Pill(string text, string fore, string back, string border)
        {
            this.lblSource.Text = text;
            this.lblSource.ForeColor = ColorTranslator.FromHtml(fore);
            this.lblSource.BackColor = ColorTranslator.FromHtml(back);
            this.lblSource.CssStyle = "border:1px solid " + border + ";border-radius:999px;";
        }
    }
}
