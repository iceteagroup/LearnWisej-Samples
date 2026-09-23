using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IconDesk.Icons;
using Wisej.Web;

namespace IconDesk
{
    /// <summary>
    /// The project's own icon pack, consumed the two ways a real application consumes one.
    ///
    /// The three buttons at the top carry literal <c>resource.wx/IconDesk.Icons/*.svg</c> strings
    /// written into the designer file, which is what the picker produces. Everything below is
    /// built from <see cref="AppIcons.All"/>, so adding an icon to the pack adds it here with no
    /// edit to this page.
    /// </summary>
    public partial class IconPackPage : Page
    {
        /// <summary>The three the lab recolours, and the colours it walks through.</summary>
        private static readonly string[] Recoloured = { nameof(AppIcons.Warning), nameof(AppIcons.Info), nameof(AppIcons.Delete) };

        private static readonly string[] Colours = { "", "highlight", "invalid", "hotTrack" };

        private readonly CommandPage commands;
        private readonly Dictionary<string, PictureBox> boxes = new Dictionary<string, PictureBox>();

        private int colourIndex;

        public IconPackPage(CommandPage commands)
        {
            InitializeComponent();

            this.commands = commands;

            BuildCatalogRow();
            Report();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Application.MainPage = this.commands;
        }

        /// <summary>
        /// One control per catalog entry. No path is typed here and no icon is named individually:
        /// the pack decides what it contains and the page follows.
        /// </summary>
        private void BuildCatalogRow()
        {
            foreach (var entry in AppIcons.All.OrderBy(e => e.Key, StringComparer.Ordinal))
            {
                var holder = new Panel { Size = new System.Drawing.Size(74, 74) };

                var box = new PictureBox
                {
                    Dock = DockStyle.Top,
                    ImageSource = entry.Value,
                    Name = "pic" + entry.Key,
                    Size = new System.Drawing.Size(74, 44),
                    SizeMode = PictureBoxSizeMode.Zoom,
                };

                var caption = new Label
                {
                    AutoSize = false,
                    Dock = DockStyle.Fill,
                    Name = "lbl" + entry.Key,
                    Text = entry.Key,
                    TextAlign = System.Drawing.ContentAlignment.TopCenter,
                };

                holder.Controls.Add(caption);
                holder.Controls.Add(box);

                this.pnlCatalog.Controls.Add(holder);
                this.boxes[entry.Key] = box;
            }
        }

        // ── the colour suffix on three of them ──────────────────────────────────

        private void btnRecolour_Click(object sender, EventArgs e)
        {
            this.colourIndex = (this.colourIndex + 1) % Colours.Length;
            var colour = Colours[this.colourIndex];

            foreach (var name in Recoloured)
            {
                var icon = AppIcons.All[name];
                this.boxes[name].ImageSource = colour.Length == 0 ? icon : AppIcons.Coloured(icon, colour);
            }

            this.lblStatus.Text = colour.Length == 0
                ? "Suffix removed. All twelve are back to the theme's icon colour."
                : $"Warning, Info and Delete carry <code>?color={colour}</code>. The other nine did not move.";

            Report();
        }

        // ── catalog versus assembly ─────────────────────────────────────────────

        /// <summary>
        /// The one test worth having for an icon pack: the catalog and the embedded resources have
        /// not drifted apart. A renamed file with a stale constant is otherwise found by a user.
        /// </summary>
        private void btnVerify_Click(object sender, EventArgs e)
        {
            var assembly = typeof(AppIcons).Assembly;
            var embedded = assembly.GetManifestResourceNames()
                .Select(n => n.Replace("IconDesk.Icons.Resources.", string.Empty))
                .OrderBy(n => n, StringComparer.Ordinal)
                .ToList();

            var catalogued = AppIcons.All.Values
                .Select(v => v.Substring(v.LastIndexOf('/') + 1))
                .OrderBy(n => n, StringComparer.Ordinal)
                .ToList();

            var missingFromCatalog = embedded.Except(catalogued).ToList();
            var missingFromAssembly = catalogued.Except(embedded).ToList();

            this.lblStatus.Text = missingFromCatalog.Count == 0 && missingFromAssembly.Count == 0
                ? $"<b>{embedded.Count} embedded icons, {catalogued.Count} catalog entries, no drift.</b>"
                : $"<b>Drift.</b> In the assembly but not the catalog: {Join(missingFromCatalog)}. " +
                  $"In the catalog but not the assembly: {Join(missingFromAssembly)}.";
        }

        private static string Join(IEnumerable<string> names)
        {
            var text = string.Join(", ", names);
            return text.Length == 0 ? "none" : text;
        }

        // ── the report ──────────────────────────────────────────────────────────

        private void Report()
        {
            var report = new StringBuilder();

            report.Append("<b>Two ways into the same pack</b><br><br>")
                  .Append("Designer: <code>").Append(this.btnAdd.ImageSource).Append("</code>, ")
                  .Append("<code>").Append(this.btnEdit.ImageSource).Append("</code>, ")
                  .Append("<code>").Append(this.btnRemove.ImageSource).Append("</code><br>")
                  .Append("Catalog: <code>AppIcons.Add</code> and friends, ")
                  .Append(AppIcons.All.Count).Append(" of them, all resolving through <code>")
                  .Append("resource.wx/").Append(AppIcons.Assembly).Append("/&lt;file&gt;.svg</code>.<br><br>")
                  .Append("The catalog constants are qualified with the assembly name, so nothing on a ")
                  .Append("deployment's disk can replace a shipped icon - see the Module 5 note.");

            this.lblReport.Text = report.ToString();
        }
    }
}
