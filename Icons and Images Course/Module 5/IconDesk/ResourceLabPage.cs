using System;
using System.Drawing;
using System.IO;
using Wisej.Web;

namespace IconDesk
{
    /// <summary>
    /// Five assets compiled into the assembly and served through <c>resource.wx</c>.
    ///
    /// An embedded resource cannot go missing on a target machine, cannot be edited by accident
    /// and does not have to be remembered by whoever writes the deployment script. The cost is
    /// that changing one means rebuilding - unless the URL is left unqualified, which is what the
    /// override in this lab is about.
    ///
    /// Two URL shapes, and the difference between them is not cosmetic:
    ///   resource.wx/IconDesk/logo.svg  - qualified: always the embedded resource,
    ///   resource.wx/status-ok.svg      - unqualified: a file of that name in the application
    ///                                    root wins, and the embedded resource is the fallback.
    /// </summary>
    public partial class ResourceLabPage : Page
    {
        /// <summary>Qualified, so nothing on disk can replace it. See the lab note.</summary>
        private const string QualifiedLogo = "resource.wx/IconDesk/logo.svg";

        /// <summary>Unqualified, so a file beside the application takes precedence.</summary>
        private const string UnqualifiedOk = "resource.wx/status-ok.svg";

        /// <summary>The file name the override is written as - the same name as the resource.</summary>
        private const string OverrideFile = "status-ok.svg";

        /// <summary>The misspelling the lab tracks down. One doubled letter.</summary>
        private const string MisspelledWarning = "resource.wx/IconDesk/status-warnning.svg";

        private static readonly Color Idle = Color.FromArgb(0x5A, 0x6B, 0x7D);
        private static readonly Color Ok = Color.FromArgb(0x1F, 0x9D, 0x6B);
        private static readonly Color Alt = Color.FromArgb(0x5B, 0x3F, 0xA8);
        private static readonly Color Bad = Color.FromArgb(0xC0, 0x39, 0x2B);

        /// <summary>Which step of the lab the one button performs next.</summary>
        private int stage;

        public ResourceLabPage()
        {
            InitializeComponent();

            AssignSources();
            ShowAll(AssetTile.TileState.Embedded);

            SetReport("Resolving image sources…", Idle);
        }

        // ── the five embedded assets ────────────────────────────────────────────

        /// <summary>
        /// One qualified source and four unqualified ones. All five resolve - the short form is
        /// not a convenience that only works sometimes - but they do not behave the same way when
        /// a file of the same name is deployed beside the application.
        /// </summary>
        private void AssignSources()
        {
            this.tileLogo.Picture.ImageSource = Bust(QualifiedLogo);
            this.tileOk.Picture.ImageSource = Bust(UnqualifiedOk);
            this.tileWarning.Picture.ImageSource = Bust("resource.wx/status-warning.svg");
            this.tilePhoto.Picture.ImageSource = Bust("resource.wx/photo.png");
            this.tileBadge.Picture.ImageSource = Bust("resource.wx/badge.gif");
        }

        /// <summary>
        /// The browser caches a resource URL, correctly - the whole point is that these addresses
        /// are stable. When the bytes behind one change under the lab's feet, the URL has to
        /// change too or the old picture stays on screen.
        /// </summary>
        private static string Bust(string url)
        {
            return url + "?v=" + DateTime.UtcNow.Ticks;
        }

        private void ShowAll(AssetTile.TileState state)
        {
            foreach (var tile in new[] { this.tileLogo, this.tileOk, this.tileWarning, this.tilePhoto, this.tileBadge })
                tile.Show(state);
        }

        // ── the four steps, on one button ───────────────────────────────────────

        private void btnShowResources_Click(object sender, EventArgs e)
        {
            switch (this.stage)
            {
                case 0:
                    AssignSources();
                    ShowAll(AssetTile.TileState.Embedded);
                    SetReport("Ready — 5 embedded assets served by resource.wx", Ok);
                    Advance("Deploy an override", 1);
                    break;

                case 1:
                    DeployOverride();
                    break;

                case 2:
                    RemoveOverride();
                    break;

                case 3:
                    Misspell();
                    break;

                default:
                    this.tileWarning.FileName = "status-warning.svg";
                    AssignSources();
                    ShowAll(AssetTile.TileState.Embedded);
                    SetReport("Ready — 5 embedded assets served by resource.wx", Ok);
                    Advance("Deploy an override", 1);
                    break;
            }
        }

        private void Advance(string caption, int next)
        {
            this.btnShowResources.Text = caption;
            this.stage = next;
        }

        // ── overriding an embedded resource from disk ───────────────────────────

        /// <summary>
        /// Where a deployment-time replacement goes: the application root, named exactly like the
        /// resource. Nothing is rebuilt and nothing in the assembly is touched.
        /// </summary>
        private static string OverridePath
        {
            get { return Application.MapPath(OverrideFile); }
        }

        private void DeployOverride()
        {
            try
            {
                File.WriteAllText(OverridePath, OverrideSvg);
                AssignSources();

                this.tileOk.Show(AssetTile.TileState.Overridden);
                SetReport("status-ok.svg served from the deployment folder — logo.svg did not move, because its source names the assembly", Alt);
                this.appTitleBar.Title = "IconDesk — ResourceLab (customer deployment)";
                Advance("Remove the override", 2);
            }
            catch (Exception ex)
            {
                SetReport("Could not write the override: " + ex.Message, Bad);
            }
        }

        private void RemoveOverride()
        {
            try
            {
                if (File.Exists(OverridePath))
                    File.Delete(OverridePath);

                AssignSources();
                ShowAll(AssetTile.TileState.Embedded);

                this.appTitleBar.Title = "IconDesk — ResourceLab";
                SetReport("Ready — 5 embedded assets", Ok);
                Advance("Misspell one URL", 3);
            }
            catch (Exception ex)
            {
                SetReport("Could not remove the override: " + ex.Message, Bad);
            }
        }

        /// <summary>A deliberately different picture, so "did the override take?" is not a judgement call.</summary>
        private const string OverrideSvg =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
            "<!-- Written at run time by the Module 5 lab to override the embedded status-ok.svg. -->\n" +
            "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 48 48\" width=\"48\" height=\"48\">\n" +
            "  <rect x=\"5\" y=\"5\" width=\"38\" height=\"38\" rx=\"9\" fill=\"none\" stroke=\"#7d5ae0\" stroke-width=\"3\"/>\n" +
            "  <path d=\"M24 13 L34 24 L24 35 L14 24 Z\" fill=\"#7d5ae0\"/>\n" +
            "</svg>\n";

        // ── the misspelling ─────────────────────────────────────────────────────

        /// <summary>
        /// Points one control at a resource that does not exist. Nothing throws: the control
        /// simply shows nothing, which is why a typo in a resource URL is found in the network
        /// panel rather than in a stack trace. The tile says so instead of staying blank.
        /// </summary>
        private void Misspell()
        {
            this.tileWarning.Picture.ImageSource = MisspelledWarning;
            this.tileWarning.FileName = "status-warnning.svg";
            this.tileWarning.Show(AssetTile.TileState.Missing);

            SetReport("status-warnning.svg → 404 (no such embedded resource)", Bad);
            Advance("Reset", 4);
        }

        private void SetReport(string text, Color color)
        {
            this.lblResourceReport.Text = text;
            this.lblResourceReport.ForeColor = color;
        }
    }
}
