using System;
using System.Collections.Generic;
using System.Text;
using IconDesk.Icons;
using Wisej.Web;

namespace IconDesk
{
    /// <summary>
    /// The capstone. Every mechanism the course covered is on this one page, and the page can be
    /// walked through a theme QA pass without touching the code.
    ///
    /// Icon fonts are the one thing Module 7 adds. They are not an image mechanism at all: the
    /// glyphs are text, styled by a stylesheet the document loads, rendered inside content marked
    /// <c>AllowHtml</c>. That makes them free to recolour and resize with ordinary CSS, and it
    /// makes them invisible to every image property on the control.
    /// </summary>
    public partial class SummaryPage : Page
    {
        private static readonly string[] Themes = { "Bootstrap-4", "BootstrapDark-4", "Material-3", "Graphite-3" };

        private readonly CommandPage commands;

        public SummaryPage(CommandPage commands)
        {
            InitializeComponent();

            this.commands = commands;

            foreach (var theme in Themes)
                this.cboTheme.Items.Add(theme);
            this.cboTheme.SelectedItem = Application.Theme?.Name;
            if (this.cboTheme.SelectedIndex < 0)
                this.cboTheme.SelectedIndex = 0;

            ShowEveryMechanism();
            BuildIconFontToolbar();
            BuildGrid();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Application.MainPage = this.commands;
        }

        // ── the five image-source mechanisms ────────────────────────────────────

        private void ShowEveryMechanism()
        {
            this.picTheme.ImageSource = "icon-search";                              // the theme's own
            this.picOfficial.ImageSource = Wisej.Ext.FontAwesome.Icons.Cog;         // an official pack
            this.picCustom.ImageSource = AppIcons.Filter;                           // our own pack
            this.picEmbedded.ImageSource = "resource.wx/IconDesk/badge.gif";        // embedded in the app
            this.picRecoloured.ImageSource = AppIcons.Coloured(AppIcons.Warning, "invalid");
        }

        // ── the icon-font toolbar ───────────────────────────────────────────────

        /// <summary>
        /// One Label, no images. Each action is a <c>span</c> carrying the stylesheet's classes and
        /// a <c>title</c>, which is what a screen reader and a hovering mouse both read.
        /// </summary>
        private void BuildIconFontToolbar()
        {
            var html = new StringBuilder();

            html.Append("<span class='idi idi-star'></span> <b>Line 3 press</b> &nbsp;&nbsp; ")
                .Append(Glyph("edit", "Edit this asset"))
                .Append(Glyph("refresh", "Reload"))
                .Append(Glyph("export", "Export"))
                .Append(Glyph("settings", "Settings"))
                .Append("&nbsp;&nbsp;<span style='color:#7b8b9c'>")
                .Append("glyphs, not pictures - nothing here is an Image or an ImageSource")
                .Append("</span>");

            this.lblToolbar.Text = html.ToString();
        }

        private static string Glyph(string name, string title) =>
            $"<span class='idi idi-{name} idi-action' title='{title}'></span> ";

        // ── the grid, with icon-font actions in a cell ──────────────────────────

        private void BuildGrid()
        {
            // role= is the attribute Wisej.NET reports back through DataGridViewCellEventArgs.Role,
            // which is how a click lands on an action rather than merely on a row.
            string Actions() =>
                "<span class='idi idi-edit idi-action' role='edit' title='Edit'></span>" +
                "<span class='idi idi-export idi-action' role='export' title='Export'></span>" +
                "<span class='idi idi-delete idi-action idi-danger' role='delete' title='Delete'></span>";

            var rows = new List<AssetRow>
            {
                new AssetRow("icon-search", "Theme image", Actions()),
                new AssetRow("floppy-o.svg", "Official pack - FontAwesome", Actions()),
                new AssetRow("filter.svg", "Custom pack - IconDesk.Icons", Actions()),
                new AssetRow("badge.gif", "Embedded in the application", Actions()),
                new AssetRow("warning.svg?color=invalid", "Recoloured SVG", Actions()),
            };

            this.gridAssets.DataSource = rows;
        }

        /// <summary>
        /// The click. <c>e.Role</c> names the element the user actually hit; <c>e.X</c> and
        /// <c>e.Y</c> give the position inside the cell if a layout needs it. Without the role
        /// there is only "somebody clicked the actions cell", which is not enough to act on.
        /// </summary>
        private void gridAssets_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != this.colActions.Index)
                return;

            var asset = this.gridAssets.Rows[e.RowIndex].Cells[this.colAsset.Index].Value;

            this.lblStatus.Text = string.IsNullOrEmpty(e.Role)
                ? $"Clicked the actions cell for <b>{asset}</b> at x={e.X}, but not on a glyph - Role was empty."
                : $"<b>{e.Role}</b> on <b>{asset}</b> (cell x={e.X}, y={e.Y}). The role attribute is what made this actionable.";
        }

        /// <summary>One row of the capstone grid.</summary>
        public class AssetRow
        {
            public AssetRow(string asset, string kind, string actions)
            {
                Asset = asset;
                Kind = kind;
                Actions = actions;
            }

            public string Asset { get; }

            public string Kind { get; }

            public string Actions { get; }
        }

        // ── the theme QA pass ───────────────────────────────────────────────────

        private void cboTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            var theme = (string)this.cboTheme.SelectedItem;
            if (string.IsNullOrEmpty(theme) || theme == Application.Theme?.Name)
                return;

            Application.LoadTheme(theme);
            this.lblStatus.Text = $"Theme is now <b>{theme}</b>. Check every glyph and every icon before moving on.";
        }

        /// <summary>
        /// Walks the checklist the module asks for and reports what this page can answer
        /// automatically. The rest is a human looking at the screen in both themes, which is why
        /// the findings live in docs/ThemeQA.md rather than in an assertion.
        /// </summary>
        private void btnQa_Click(object sender, EventArgs e)
        {
            var findings = new StringBuilder();
            findings.Append("<b>Theme QA - ").Append(Application.Theme?.Name).Append("</b><br>");

            findings.Append("Image sources on this page: ")
                    .Append(this.picTheme.ImageSource).Append(" &middot; ")
                    .Append(this.picOfficial.ImageSource).Append(" &middot; ")
                    .Append(this.picCustom.ImageSource).Append(" &middot; ")
                    .Append(this.picEmbedded.ImageSource).Append(" &middot; ")
                    .Append(this.picRecoloured.ImageSource).Append("<br>");

            findings.Append("Icon-font glyphs take their colour from <code>icon-font.css</code>, not from the theme - ")
                    .Append("that is the finding this pass exists to catch. See docs/ThemeQA.md.");

            this.lblStatus.Text = findings.ToString();
        }
    }
}
