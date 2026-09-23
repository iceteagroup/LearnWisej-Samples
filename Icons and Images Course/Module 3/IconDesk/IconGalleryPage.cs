using System;
using System.Text;
using Wisej.Web;

namespace IconDesk
{
    /// <summary>
    /// Five image sources of five different kinds, all of them strings the designer Image Selector
    /// can write, and a theme switch that separates the ones the theme owns from the ones it does
    /// not.
    ///
    /// The point of the page is the report at the bottom: every picture here is identified by a
    /// readable string that survives a code review, and nothing on this page decodes an image on
    /// the server.
    /// </summary>
    public partial class IconGalleryPage : Page
    {
        /// <summary>
        /// Themes that ship with Wisej.NET 4. A light and a dark one from the same family make the
        /// difference easiest to see.
        /// </summary>
        private static readonly string[] Themes = { "Bootstrap-4", "BootstrapDark-4", "Material-3", "Graphite-3" };

        /// <summary>
        /// Two theme colour names and one literal. A theme colour follows the theme; a literal
        /// does not, which is the whole argument for naming colours rather than typing them.
        /// </summary>
        private static readonly string[] Colours = { "highlight", "hotTrack", "invalid", "#7d5ae0" };

        private readonly CommandPage commands;

        private int colourIndex;

        public IconGalleryPage(CommandPage commands)
        {
            InitializeComponent();

            this.commands = commands;

            foreach (var theme in Themes)
                this.cboTheme.Items.Add(theme);

            this.cboTheme.SelectedItem = Application.Theme?.Name;
            if (this.cboTheme.SelectedIndex < 0)
                this.cboTheme.SelectedIndex = 0;

            // An absolute URL, built at run time so the lab works on whichever port it was started
            // on. In a real project the designer writes the literal address into the file above.
            this.picAbsolute.ImageSource = Application.Url.TrimEnd('/') + "/Images/status-ok.svg";

            Report();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Application.MainPage = this.commands;
        }

        // ── the theme switch ────────────────────────────────────────────────────

        private void cboTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            var theme = (string)this.cboTheme.SelectedItem;
            if (string.IsNullOrEmpty(theme) || theme == Application.Theme?.Name)
                return;

            // Application.Theme is a ClientTheme object, not a string. LoadTheme takes the name.
            Application.LoadTheme(theme);

            this.lblStatus.Text =
                $"Theme is now {theme}. Four of the five moved. The multi-coloured project logo did not, " +
                "and that - not where the file came from - is the rule: Wisej.NET recolours an SVG it reads as an icon.";

            Report();
        }

        // ── the colour suffix ───────────────────────────────────────────────────

        /// <summary>
        /// The suffix recolours a monochrome SVG. It works on artwork with a single fill and no
        /// gradient; it cannot do anything useful to the multi-coloured project logo, which is why
        /// only <c>pin.svg</c> is wired to this button.
        /// </summary>
        private void btnRecolour_Click(object sender, EventArgs e)
        {
            this.colourIndex = (this.colourIndex + 1) % Colours.Length;
            var colour = Colours[this.colourIndex];

            this.picRecoloured.ImageSource = "Images/pin.svg?color=" + colour;
            this.lblRecoloured.Text = "Recoloured SVG\r\n\"...pin.svg?color=" + colour + "\"";

            this.lblStatus.Text = colour.StartsWith("#")
                ? $"Colour is the literal {colour}. It will look exactly the same under every theme, which is usually wrong."
                : $"Colour is the theme name \"{colour}\". Switch the theme and this icon moves with it.";

            Report();
        }

        // ── the report ──────────────────────────────────────────────────────────

        private void btnReport_Click(object sender, EventArgs e)
        {
            Report();
            this.lblStatus.Text = "Every image source on the page, exactly as the designer would have written it.";
        }

        private void Report()
        {
            // The third column is the one the lab is actually about, and it is not the obvious
            // answer. What decides whether an asset follows the theme is not where it came from -
            // it is whether Wisej.NET decided the artwork is a recolourable icon. See
            // docs/ThemeSwitch.md for how that was established.
            var rows = new (PictureBox Box, string Kind, string Verdict)[]
            {
                (this.picTheme, "named theme image",
                    "<b>follows the theme</b> - the theme owns the artwork outright"),
                (this.picProject, "relative URL to a file deployed beside the application",
                    "<b>fixed</b> - six fills, so it is treated as artwork and passed through untouched"),
                (this.picAbsolute, "absolute URL",
                    "<b>follows the theme</b> - one fill plus white, so it is treated as an icon and recoloured"),
                (this.picRecoloured, "relative URL plus the ?color= suffix",
                    this.colourIndex == Colours.Length - 1
                        ? "<b>fixed</b> - the suffix names a literal, which no theme can move"
                        : "<b>follows the theme</b> - the suffix names a theme colour"),
                (this.picThemeSecond, "named theme image",
                    "<b>follows the theme</b> - the theme owns the artwork outright"),
            };

            var report = new StringBuilder();
            report.Append("<b>The five image sources, and which of them the theme moves</b><br><br>");

            foreach (var row in rows)
            {
                report.Append("<b>").Append(row.Box.Name).Append("</b> &mdash; ")
                      .Append(row.Kind).Append(": <code>").Append(row.Box.ImageSource).Append("</code><br>&nbsp;&nbsp;&nbsp;&nbsp;")
                      .Append(row.Verdict)
                      .Append("<br>");
            }

            report.Append("<br>Current theme: <b>").Append(Application.Theme?.Name).Append("</b>.");
            this.lblReport.Text = report.ToString();
        }
    }
}
