using System;
using System.Drawing;
using System.IO;
using System.Text;
using Wisej.Web;

namespace IconDesk
{
    /// <summary>
    /// The two pipelines, side by side and visible.
    ///
    /// The four size-mode boxes all show the same 360x200 raster through <c>ImageSource</c>, so
    /// the only difference between them is how the client fits it into an identical box. The
    /// remote JPEG arrives through <see cref="PictureBox.LoadAsync"/>, which returns immediately
    /// and raises <c>LoadCompleted</c> later. The SVG row shows one vector at 16, 24, 32 and 48
    /// pixels; the bitmap row shows a 16-pixel <c>Image</c> object blown up to the same four
    /// sizes, which is what makes raster interface icons fall apart.
    /// </summary>
    public partial class ImageLabPage : Page
    {
        /// <summary>The lab's raster asset, addressed the way the browser addresses everything else.</summary>
        private const string RasterSource = "Images/workbench.png";

        /// <summary>One monochrome SVG for the whole vector row.</summary>
        private const string VectorSource = "Images/glyph-save.svg";

        /// <summary>
        /// A URL that takes 2.5 seconds to answer, served by this application's own Startup.cs.
        /// It stands in for a far-away host: the lab needs the delay to be real and repeatable,
        /// and a machine with no internet access must still be able to run it.
        /// </summary>
        private const string SlowJpegUrl = "slow/hero.jpg";

        private DateTime requestedAt;

        /// <summary>
        /// What LoadCompleted found. The ImageLab screen has no status strip, so it goes on the
        /// picture's tooltip - and into the lab note.
        /// </summary>
        public string LoadAsyncFinding { get; private set; } = "LoadAsync has not completed yet.";

        public ImageLabPage()
        {
            InitializeComponent();

            ShowRasterInEverySizeMode();
            ShowVectorAtEverySize();
            ShowBitmapAtEverySize();
            Report();
        }

        // ── one raster, four size modes ─────────────────────────────────────────

        /// <summary>
        /// The same image source in four boxes of the same size. Nothing else differs, so every
        /// difference on screen is the size mode.
        /// </summary>
        private void ShowRasterInEverySizeMode()
        {
            // A string, not an object: the browser fetches Images/workbench.png once and reuses
            // it for all four boxes. Nothing is decoded on the server.
            this.picNormal.ImageSource = RasterSource;
            this.picZoom.ImageSource = RasterSource;
            this.picCover.ImageSource = RasterSource;
            this.picStretch.ImageSource = RasterSource;
        }

        // ── one vector, four sizes ──────────────────────────────────────────────

        /// <summary>
        /// The same string on four differently sized controls. Because an image source keeps the
        /// asset's identity, the SVG is drawn at each box's own size instead of being scaled from
        /// one rasterisation - which is the whole argument for vector interface icons.
        /// </summary>
        private void ShowVectorAtEverySize()
        {
            this.picSvg16.ImageSource = VectorSource;
            this.picSvg24.ImageSource = VectorSource;
            this.picSvg32.ImageSource = VectorSource;
            this.picSvg48.ImageSource = VectorSource;
        }

        /// <summary>
        /// The control group: one 16-pixel bitmap <b>object</b>, stretched to the same four sizes.
        /// Nothing regenerates raster artwork - it is only ever resampled, so 32 is soft and 48 is
        /// unusable.
        /// </summary>
        private void ShowBitmapAtEverySize()
        {
            var glyph = LoadServerImage("glyph-save-16.png");

            this.picLegacy16.Image = glyph;
            this.picLegacy24.Image = glyph;
            this.picLegacy32.Image = glyph;
            this.picLegacy48.Image = glyph;
        }

        private static Image LoadServerImage(string fileName)
        {
            var bytes = File.ReadAllBytes(Application.MapPath("Images/" + fileName));
            return Image.FromStream(new MemoryStream(bytes));
        }

        // ── the remote picture ──────────────────────────────────────────────────

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Started from Load rather than the constructor so the loader has a laid-out control
            // to appear in.
            this.requestedAt = DateTime.UtcNow;
            this.picRemote.ShowLoader = true;
            this.picRemote.LoadAsync(SlowJpegUrl);
        }

        /// <summary>
        /// LoadAsync returns at once and the page stays usable - every other control on the page
        /// is interactive while the spinner is up. <c>LoadCompleted</c> arrives later.
        ///
        /// Worth knowing, because it is not what the name suggests: <c>LoadAsync</c> sets
        /// <c>ImageSource</c> to the URL and lets the <b>browser</b> fetch it. Nothing is
        /// downloaded or decoded on the server, and <c>Image</c> is still null when
        /// <c>LoadCompleted</c> fires. A URL the server can reach but the browser cannot will
        /// therefore fail, and vice versa - which is the opposite of the usual assumption.
        /// </summary>
        private void picRemote_LoadCompleted(object sender, EventArgs e)
        {
            var elapsed = (DateTime.UtcNow - this.requestedAt).TotalMilliseconds;

            // The loader is ours to turn off: LoadCompleted is the signal, not the spinner.
            this.picRemote.ShowLoader = false;

            this.lblRemoteWaiting.Visible = false;
            this.picRemote.Visible = true;

            this.LoadAsyncFinding =
                $"LoadCompleted after {elapsed:0} ms. The picture is on ImageSource " +
                $"(\"{this.picRemote.ImageSource}\") and Image is " +
                $"{(this.picRemote.Image == null ? "still null" : "set")} - the browser fetched it, not the server.";

            this.picRemote.ToolTipText = this.LoadAsyncFinding;
        }

        // ── which property supplied each picture ────────────────────────────────

        /// <summary>
        /// One row per box. Five of them answer <c>ImageSource</c> and one answers <c>Image</c>;
        /// a box that ever reports the property you did not set means somebody assigned two image
        /// properties and one of them won silently.
        /// </summary>
        private void Report()
        {
            var html = new StringBuilder();

            foreach (var box in new[] { this.picNormal, this.picZoom, this.picCover, this.picStretch, this.picSvg48, this.picLegacy48 })
            {
                var mechanism = !string.IsNullOrEmpty(box.ImageSource) ? "ImageSource"
                    : box.Image != null ? "Image"
                    : "nothing yet";

                html.Append("<div style='display:flex;font-family:Consolas,\"Courier New\",monospace;")
                    .Append("font-size:13.5px;line-height:24px;color:#1f2d3a;'>")
                    .Append("<span style='display:inline-block;width:150px;'>").Append(box.Name).Append("</span>")
                    .Append("<span style='font-weight:700;color:")
                    .Append(mechanism == "Image" ? "#7d5ae0" : "#1565d8").Append(";'>")
                    .Append(mechanism).Append("</span></div>");
            }

            this.lblReport.Text = html.ToString();
        }
    }
}
