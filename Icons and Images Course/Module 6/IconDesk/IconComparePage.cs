using System;
using System.Text;
using Wisej.Web;
using Fa = Wisej.Ext.FontAwesome.Icons;
using Md = Wisej.Ext.MaterialDesign.Icons;

namespace IconDesk
{
    /// <summary>
    /// The same five concepts drawn by two official icon packs, so the choice between them is made
    /// by looking rather than by reading marketing copy.
    ///
    /// Both routes into a pack are used on purpose. The FontAwesome row carries the literal
    /// <c>resource.wx</c> strings the Visual Studio image explorer writes into the designer file;
    /// the Material Design row is assigned here from the pack's own <c>Icons</c> catalog, which is
    /// a class of constants the pack assembly ships. The catalog is the better habit - a typo is a
    /// compiler error rather than a blank control - but the picker is what you reach for while
    /// laying a screen out.
    /// </summary>
    public partial class IconComparePage : Page
    {
        /// <summary>Theme colour names, then one literal, so the suffix can be compared fairly.</summary>
        private static readonly string[] Colours = { "", "?color=highlight", "?color=invalid", "?color=#7d5ae0" };

        private readonly CommandPage commands;

        private int colourIndex;

        public IconComparePage(CommandPage commands)
        {
            InitializeComponent();

            this.commands = commands;

            // The catalog route. Every field is a resource.wx string the pack assembly resolves
            // out of its own embedded resources - nothing was copied into this project.
            this.picMdSave.ImageSource = Md.SaveButton;
            this.picMdDelete.ImageSource = Md.RubbishBinDeleteButton;
            this.picMdSearch.ImageSource = Md.SearchingMagnifyingGlass;
            this.picMdUser.ImageSource = Md.UserAccountBox1;
            this.picMdSettings.ImageSource = Md.SettingsCogwheelButton;

            Report();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Application.MainPage = this.commands;
        }

        // ── recolouring pack artwork ────────────────────────────────────────────

        /// <summary>
        /// Puts the same colour suffix on one icon from each family. Pack icons are drawn as single
        /// flat shapes, so both accept it - which is precisely why an icon pack is easier to live
        /// with than a folder of hand-drawn SVGs.
        /// </summary>
        private void btnRecolour_Click(object sender, EventArgs e)
        {
            this.colourIndex = (this.colourIndex + 1) % Colours.Length;
            var suffix = Colours[this.colourIndex];

            this.picFaDelete.ImageSource = Fa.Trash + suffix;
            this.picMdDelete.ImageSource = Md.RubbishBinDeleteButton + suffix;

            this.lblStatus.Text = suffix.Length == 0
                ? "No suffix: both Delete icons are back to the theme's own icon colour."
                : $"Both Delete icons carry \"{suffix}\". Pack artwork is monochrome by design, so the suffix always lands.";

            Report();
        }

        // ── the report ──────────────────────────────────────────────────────────

        private void btnReport_Click(object sender, EventArgs e)
        {
            Report();
            this.lblStatus.Text = "Every pack URL on the page. Note that none of them names a file inside this project.";
        }

        private void Report()
        {
            var report = new StringBuilder();
            report.Append("<b>Where the ten icons come from</b><br><br>");

            report.Append("<b>FontAwesome</b> - assigned in the designer, as the Visual Studio image explorer writes it:<br>");
            AppendRow(report, this.picFaSave, this.picFaDelete, this.picFaSearch, this.picFaUser, this.picFaSettings);

            report.Append("<br><b>Material Design</b> - assigned from the pack catalog in C#:<br>");
            AppendRow(report, this.picMdSave, this.picMdDelete, this.picMdSearch, this.picMdUser, this.picMdSettings);

            report.Append("<br>Both families resolve through <code>resource.wx/&lt;assembly&gt;/&lt;icon&gt;.svg</code>. ")
                  .Append("Each icon is one request the first time it is needed and comes from the browser cache afterwards, ")
                  .Append("so a screen using twelve icons costs twelve small requests once, not on every page.");

            this.lblReport.Text = report.ToString();
        }

        private static void AppendRow(StringBuilder report, params PictureBox[] boxes)
        {
            foreach (var box in boxes)
                report.Append("&nbsp;&nbsp;&nbsp;&nbsp;<code>").Append(box.ImageSource).Append("</code><br>");
        }
    }
}
