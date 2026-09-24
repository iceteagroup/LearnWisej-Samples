using System.Drawing;
using Wisej.Web;

namespace IconDesk
{
    /// <summary>
    /// The 40-pixel application bar every IconDesk page carries: the page title on the left and
    /// the three window glyphs on the right. Nothing else belongs in it.
    /// </summary>
    /// <remarks>
    /// IconDesk is a Web Page Application, so the browser window has no native title bar of its
    /// own. The bar is a painted <see cref="Panel"/>: a <see cref="Label"/> for the title and one
    /// AllowHtml label for the minimise / maximise / close glyphs, which are three plain boxes
    /// rather than pictures - no image mechanism is involved in the chrome.
    /// </remarks>
    public class AppTitleBar : Panel
    {
        private const string Glyphs =
            "<span style='display:inline-flex;align-items:center;gap:16px;'>" +
            "<span style='display:inline-block;width:11px;border-bottom:1.4px solid #ffffff;'></span>" +
            "<span style='display:inline-block;width:10px;height:10px;border:1.4px solid #ffffff;border-radius:2px;'></span>" +
            "<span style='font-size:13px;line-height:1;'>&#10005;</span>" +
            "</span>";

        private readonly Label lblAppTitle = new Label();
        private readonly Label lblWindowGlyphs = new Label();

        public AppTitleBar()
        {
            this.Name = "appTitleBar";
            this.Dock = DockStyle.Top;
            this.Size = new Size(1000, 40);
            this.BackColor = Color.FromArgb(0x15, 0x65, 0xD8);

            this.lblWindowGlyphs.Name = "lblWindowGlyphs";
            this.lblWindowGlyphs.AllowHtml = true;
            this.lblWindowGlyphs.AutoSize = false;
            this.lblWindowGlyphs.Dock = DockStyle.Right;
            this.lblWindowGlyphs.ForeColor = Color.White;
            this.lblWindowGlyphs.Size = new Size(88, 40);
            this.lblWindowGlyphs.Padding = new Padding(0, 0, 18, 0);
            this.lblWindowGlyphs.Text = Glyphs;
            this.lblWindowGlyphs.TextAlign = ContentAlignment.MiddleRight;

            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.AllowHtml = true;
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = DockStyle.Fill;
            this.lblAppTitle.Font = new Font("default", 11.25F, FontStyle.Bold);
            this.lblAppTitle.ForeColor = Color.White;
            this.lblAppTitle.Padding = new Padding(18, 0, 0, 0);
            this.lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;

            this.Controls.Add(this.lblAppTitle);
            this.Controls.Add(this.lblWindowGlyphs);
        }

        /// <summary>The page name shown on the left of the bar.</summary>
        public string Title
        {
            get { return this.lblAppTitle.Text; }
            set { this.lblAppTitle.Text = value; }
        }
    }
}
