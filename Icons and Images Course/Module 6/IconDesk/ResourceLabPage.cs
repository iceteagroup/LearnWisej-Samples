using System;
using System.IO;
using System.Text;
using Wisej.Web;

namespace IconDesk
{
    /// <summary>
    /// Five assets compiled into the assembly and served through <c>resource.wx</c>.
    ///
    /// An embedded resource cannot go missing on a target machine, cannot be edited by accident and
    /// does not have to be remembered by whoever writes the deployment script. The cost is that
    /// changing one means rebuilding - unless the URL is left unqualified, which is what the
    /// override in this lab is about.
    ///
    /// Two URL shapes, and the difference between them is not cosmetic:
    ///   resource.wx/IconDesk/logo.svg  - qualified: always the embedded resource,
    ///   resource.wx/logo.svg           - unqualified: a file of that name in the application
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

        private readonly CommandPage commands;

        public ResourceLabPage(CommandPage commands)
        {
            InitializeComponent();

            this.commands = commands;

            AssignSources();
            Report();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Application.MainPage = this.commands;
        }

        /// <summary>
        /// One qualified source and four unqualified ones. All five resolve - the short form is
        /// not a convenience that only works sometimes - but they do not behave the same way when
        /// a file of the same name is deployed beside the application.
        /// </summary>
        private void AssignSources()
        {
            this.picLogo.ImageSource = Bust(QualifiedLogo);
            this.picOk.ImageSource = Bust(UnqualifiedOk);
            this.picWarning.ImageSource = Bust("resource.wx/status-warning.svg");
            this.picPhoto.ImageSource = Bust("resource.wx/photo.png");
            this.picBadge.ImageSource = Bust("resource.wx/badge.gif");
        }

        /// <summary>
        /// The browser caches a resource URL, correctly - the whole point is that these addresses
        /// are stable. When the bytes behind one change under the lab's feet, the URL has to
        /// change too or the old picture stays on screen.
        /// </summary>
        private static string Bust(string url) => url + "?v=" + DateTime.UtcNow.Ticks;

        // ── overriding an embedded resource from disk ───────────────────────────

        /// <summary>
        /// Where a deployment-time replacement goes: the application root, named exactly like the
        /// resource. Nothing is rebuilt and nothing in the assembly is touched.
        /// </summary>
        private static string OverridePath => Application.MapPath(OverrideFile);

        private void btnOverride_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(OverridePath, OverrideSvg);
                AssignSources();

                this.lblStatus.Text =
                    $"Wrote <code>{OverrideFile}</code> to the application root. <b>picOk changed and picLogo did not</b> - " +
                    "one is unqualified and one is qualified, and that is the only difference between them.";
            }
            catch (Exception ex)
            {
                this.lblStatus.Text = "Could not write the override: " + ex.Message;
            }

            Report();
        }

        private void btnRemoveOverride_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(OverridePath))
                    File.Delete(OverridePath);

                AssignSources();
                this.lblStatus.Text = "Override removed. picOk falls back to the embedded resource with no code change.";
            }
            catch (Exception ex)
            {
                this.lblStatus.Text = "Could not remove the override: " + ex.Message;
            }

            Report();
        }

        /// <summary>A deliberately different picture, so "did the override take?" is not a judgement call.</summary>
        private const string OverrideSvg =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
            "<!-- Written at run time by the Module 5 lab to override the embedded status-ok.svg. -->\n" +
            "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" width=\"24\" height=\"24\">\n" +
            "  <rect x=\"2\" y=\"2\" width=\"20\" height=\"20\" rx=\"4\" fill=\"#7d5ae0\"/>\n" +
            "  <path d=\"M8 12h8\" stroke=\"#ffffff\" stroke-width=\"2.6\" stroke-linecap=\"round\"/>\n" +
            "</svg>\n";

        // ── the misspelling ─────────────────────────────────────────────────────

        /// <summary>
        /// Points one control at a resource that does not exist. Nothing throws: the control simply
        /// shows nothing, which is why a typo in a resource URL is found in the network panel
        /// rather than in a stack trace.
        /// </summary>
        private void btnMisspell_Click(object sender, EventArgs e)
        {
            this.picWarning.ImageSource = "resource.wx/IconDesk/status-warnning.svg";

            this.lblStatus.Text =
                "picWarning now points at <code>status-warnning.svg</code>, which does not exist. " +
                "No exception and no log entry - just an empty control. The browser network panel has the failed request.";

            Report();
        }

        // ── the report ──────────────────────────────────────────────────────────

        private void btnReport_Click(object sender, EventArgs e)
        {
            AssignSources();
            Report();
            this.lblStatus.Text = "Sources reset and reported.";
        }

        private void Report()
        {
            var report = new StringBuilder();
            report.Append("<b>The five resource URLs</b><br><br>");

            foreach (var box in new[] { this.picLogo, this.picOk, this.picWarning, this.picPhoto, this.picBadge })
            {
                var source = box.ImageSource ?? string.Empty;
                var shown = source.Contains("?v=") ? source.Substring(0, source.IndexOf("?v=", StringComparison.Ordinal)) : source;

                report.Append("<b>").Append(box.Name).Append("</b> &mdash; <code>").Append(shown).Append("</code> &mdash; ")
                      .Append(shown.StartsWith("resource.wx/IconDesk/", StringComparison.Ordinal)
                          ? "qualified: the assembly, always"
                          : "unqualified: the application root first, then the assembly")
                      .Append("<br>");
            }

            report.Append("<br>Embedded as <code>IconDesk.Resources.&lt;file&gt;</code> from the <code>Assets</code> folder. ")
                  .Append("The override file is ")
                  .Append(File.Exists(OverridePath) ? "<b>present</b>" : "absent")
                  .Append(" at <code>").Append(OverridePath).Append("</code>.");

            this.lblReport.Text = report.ToString();
        }
    }
}
