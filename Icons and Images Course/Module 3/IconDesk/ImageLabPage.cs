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
    /// The four size-mode boxes all show the same 480x300 raster through <c>Image</c>, so the only
    /// difference between them is how the client fits it into an identical box. The remote JPEG
    /// arrives through <see cref="PictureBox.LoadAsync"/>, which returns immediately and raises
    /// <c>LoadCompleted</c> later. The four small boxes show one SVG through <c>ImageSource</c>:
    /// the resource keeps its identity all the way to the browser, so it is re-rendered at each
    /// size rather than resampled.
    /// </summary>
    public partial class ImageLabPage : Page
    {
        /// <summary>
        /// A URL that takes 2.5 seconds to answer, served by this application's own Startup.cs.
        /// It stands in for a far-away host: the lab needs the delay to be real and repeatable,
        /// and a machine with no internet access must still be able to run it.
        /// </summary>
        private const string SlowJpegUrl = "slow/photo.jpg";

        /// <summary>
        /// A genuinely remote JPEG, for the second button. This one is allowed to fail - on a
        /// machine with no outbound access it simply never completes, and the page says so
        /// without anything else on it being affected.
        /// </summary>
        private const string RemoteJpegUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/320px-Cat03.jpg";

        private const string VectorSource = "Images/logo.svg";

        private readonly CommandPage commands;

        private DateTime requestedAt;
        private string pending = "nothing";

        public ImageLabPage(CommandPage commands)
        {
            InitializeComponent();

            this.commands = commands;

            ShowRasterInEverySizeMode();
            ShowVectorAtEverySize();
            Report();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Application.MainPage = this.commands;
        }

        // ── one raster, four size modes ─────────────────────────────────────────

        /// <summary>
        /// The same image object in four boxes of the same size. Nothing else differs, so every
        /// difference on screen is the size mode.
        /// </summary>
        private void ShowRasterInEverySizeMode()
        {
            // One decode, four references. Assigning the same Image to several controls is fine:
            // Wisej.NET sends it once and the browser caches it under one URL.
            var photo = LoadServerImage("photo.png");

            this.picNormal.Image = photo;
            this.picZoom.Image = photo;
            this.picCover.Image = photo;
            this.picStretch.Image = photo;
        }

        private static Image LoadServerImage(string fileName)
        {
            var bytes = File.ReadAllBytes(Application.MapPath("Images/" + fileName));
            return Image.FromStream(new MemoryStream(bytes));
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

        // ── the remote picture ──────────────────────────────────────────────────

        private void btnFetchSlow_Click(object sender, EventArgs e)
        {
            Fetch(SlowJpegUrl, "the slow local endpoint");
        }

        private void btnFetchRemote_Click(object sender, EventArgs e)
        {
            Fetch(RemoteJpegUrl, "a genuinely remote host");
        }

        /// <summary>
        /// LoadAsync returns at once and the page stays usable - press any other button while the
        /// spinner is up and it answers immediately. <c>LoadCompleted</c> arrives later.
        ///
        /// Worth knowing, because it is not what the name suggests: <c>LoadAsync</c> sets
        /// <c>ImageSource</c> to the URL and lets the <b>browser</b> fetch it. Nothing is
        /// downloaded or decoded on the server, and <c>Image</c> is still null when
        /// <c>LoadCompleted</c> fires. A URL the server can reach but the browser cannot will
        /// therefore fail, and vice versa - which is the opposite of the usual assumption.
        /// <c>PictureBox.Load(url)</c> is the blocking alternative, and it is what makes a page
        /// feel broken when the far end is slow.
        /// </summary>
        private void Fetch(string url, string what)
        {
            this.pending = what;
            this.requestedAt = DateTime.UtcNow;

            this.lblStatus.Text =
                $"LoadAsync called for {what}. This handler has already returned - nothing on the page is waiting for the picture.";

            this.picRemote.ShowLoader = true;
            this.picRemote.LoadAsync(url);
        }

        private void picRemote_LoadCompleted(object sender, EventArgs e)
        {
            var elapsed = (DateTime.UtcNow - this.requestedAt).TotalMilliseconds;

            // The loader is ours to turn off: LoadCompleted is the signal, not the spinner.
            this.picRemote.ShowLoader = false;

            this.lblStatus.Text =
                $"{this.pending}: LoadCompleted after {elapsed:0} ms. The picture is on ImageSource " +
                $"(\"{this.picRemote.ImageSource}\") and Image is {(this.picRemote.Image == null ? "still null" : "set")} - " +
                "the browser fetched it, not the server.";

            Report();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Started from Load rather than the constructor so the spinner has a laid-out
            // control to appear in.
            Fetch(SlowJpegUrl, "the slow local endpoint");
        }

        // ── which property supplied each picture ────────────────────────────────

        private void btnReport_Click(object sender, EventArgs e)
        {
            Report();
            this.lblStatus.Text = "Every PictureBox on the page, reported from its own properties.";
        }

        private void Report()
        {
            var boxes = new[]
            {
                this.picNormal, this.picZoom, this.picCover, this.picStretch,
                this.picRemote,
                this.picSvg16, this.picSvg24, this.picSvg32, this.picSvg48,
            };

            var report = new StringBuilder();
            report.Append("<b>Where each picture came from</b><br><br>");

            foreach (var box in boxes)
            {
                report.Append("<b>").Append(box.Name).Append("</b> &mdash; ");

                if (!string.IsNullOrEmpty(box.ImageSource))
                {
                    report.Append("ImageSource \"").Append(box.ImageSource).Append("\" - a string the client resolved; no decoding happened on the server");
                    if (box == this.picRemote)
                        report.Append(" (set by LoadAsync, which is why this one is not an Image)");
                }
                else if (box.Image != null)
                    report.Append("Image - a ").Append(box.Image.Width).Append('x').Append(box.Image.Height)
                          .Append(" object in server memory, sent as PNG, drawn with SizeMode.").Append(box.SizeMode);
                else
                    report.Append("nothing yet");

                report.Append("<br>");
            }

            this.lblReport.Text = report.ToString();
        }
    }
}
