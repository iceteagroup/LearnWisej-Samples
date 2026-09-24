using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wisej.Web;
using Fa = Wisej.Ext.FontAwesome.Icons;
using Md = Wisej.Ext.MaterialDesign.Icons;

namespace IconDesk
{
    /// <summary>
    /// The same five concepts drawn by two official icon packs, so the choice between them is
    /// made by looking rather than by reading marketing copy.
    /// </summary>
    /// <remarks>
    /// Both routes into a pack are used on purpose. The FontAwesome column carries the literal
    /// <c>resource.wx</c> strings the Visual Studio image explorer writes into the designer file;
    /// the Material Design column is assigned here from the pack's own <c>Icons</c> catalog, a
    /// class of constants the pack assembly ships. The catalog is the better habit - a typo is a
    /// compiler error rather than a blank control - but the picker is what you reach for while
    /// laying a screen out.
    /// </remarks>
    public partial class IconComparePage : Page
    {
        /// <summary>
        /// The two icons the lab recolours, with theme colour <b>names</b> rather than literals so
        /// the colour resolves again under a different theme.
        /// </summary>
        /// <remarks>
        /// The name has to be one the theme actually defines. <c>invalid</c> and <c>info</c>
        /// resolve; <c>error</c> and <c>activeText</c> do not - Wisej.NET passes an unknown name
        /// straight through to the SVG's fill attribute, where the browser ignores it and the
        /// icon silently keeps the colour it already had. Verified by decoding the inlined SVG
        /// out of the page.
        /// </remarks>
        private const string DeleteColour = "?color=invalid";
        private const string SettingsColour = "?color=info";

        public IconComparePage()
        {
            InitializeComponent();

            AssignFromCatalog();
            Recolour();
            Describe();

            this.lblRecommendation.Text =
                "<b>Primary family: Wisej-4-FontAwesome</b> — it covers all five concepts, its artwork is " +
                "monochrome SVG that takes the <span style='font-family:Consolas,monospace'>?color=</span> " +
                "suffix, and it ships as one assembly this project already has a licence trail for.";
        }

        // ── the two routes into a pack ──────────────────────────────────────────

        /// <summary>
        /// The catalog route. Every field is a resource.wx string the pack assembly resolves out
        /// of its own embedded resources - nothing was copied into this project.
        /// </summary>
        private void AssignFromCatalog()
        {
            this.picMdSave.ImageSource = Md.SaveButton;
            this.picMdDelete.ImageSource = Md.RubbishBinDeleteButton;
            this.picMdSearch.ImageSource = Md.SearchingMagnifyingGlass;
            this.picMdUser.ImageSource = Md.UserAccountBox1;
            this.picMdSettings.ImageSource = Md.SettingsCogwheelButton;
        }

        /// <summary>
        /// The colour suffix, on one icon from each family. Both are flat monochrome shapes, so
        /// both take it; a theme colour <b>name</b> is used rather than a literal so the colour
        /// resolves again under a different theme.
        /// </summary>
        private void Recolour()
        {
            this.picFaDelete.ImageSource = Fa.Trash + DeleteColour;
            this.picMdSettings.ImageSource = Md.SettingsCogwheelButton + SettingsColour;
        }

        // ── the report ──────────────────────────────────────────────────────────

        /// <summary>
        /// Rebuilds the Material Design column from the catalog, re-applies the two colour
        /// suffixes, and reports what every control ended up carrying - including any source that
        /// names a file the pack assembly does not contain.
        /// </summary>
        private void btnCompare_Click(object sender, EventArgs e)
        {
            AssignFromCatalog();
            Recolour();
            Describe();
        }

        private void Describe()
        {
            var picked = new[] { this.picFaSave, this.picFaDelete, this.picFaSearch, this.picFaUser, this.picFaSettings };
            var catalog = new[] { this.picMdSave, this.picMdDelete, this.picMdSearch, this.picMdUser, this.picMdSettings };

            ShowFileNames(picked, new[] { this.lblFaSave, this.lblFaDelete, this.lblFaSearch, this.lblFaUser, this.lblFaSettings });
            ShowFileNames(catalog, new[] { this.lblMdSave, this.lblMdDelete, this.lblMdSearch, this.lblMdUser, this.lblMdSettings });

            var missing = picked.Concat(catalog).Where(box => !Resolves(box.ImageSource)).Select(box => box.Name).ToList();

            this.lblFooterNote.Text = missing.Count == 0
                ? "Five picked in the image explorer, five from the pack catalog. Two recoloured with the " +
                  "<span style='font-family:Consolas,monospace'>?color=</span> suffix and a theme colour name."
                : "<b style='color:#b3261e'>" + string.Join(", ", missing) +
                  "</b> names a file the pack assembly does not contain - the control will be blank, and no exception is thrown.";
        }

        private static void ShowFileNames(PictureBox[] boxes, Label[] labels)
        {
            for (var i = 0; i < boxes.Length; i++)
                labels[i].Text = FileNameOf(boxes[i].ImageSource);
        }

        /// <summary>The last path segment of a resource source, without the colour suffix.</summary>
        private static string FileNameOf(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            var withoutQuery = source.Split('?')[0];
            return withoutQuery.Substring(withoutQuery.LastIndexOf('/') + 1);
        }

        /// <summary>
        /// Asks the pack assembly whether it really holds that file. A misspelled resource source
        /// is not an exception and not a compiler error - the control simply stays blank - so the
        /// page checks rather than assuming.
        /// </summary>
        private static bool Resolves(string source)
        {
            if (string.IsNullOrEmpty(source) || !source.StartsWith("resource.wx/", StringComparison.Ordinal))
                return false;

            var parts = source.Split('?')[0].Split('/');
            if (parts.Length < 3)
                return false;

            var assemblyName = parts[1];
            var file = parts[parts.Length - 1];

            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => string.Equals(a.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase));

            if (assembly == null)
                return false;

            // Manifest names are <RootNamespace>.<folder>.<file>, so the URL's last segment is the
            // tail of the manifest name.
            return ManifestNames(assembly).Any(name => name.EndsWith("." + file, StringComparison.OrdinalIgnoreCase));
        }

        private static IEnumerable<string> ManifestNames(Assembly assembly)
        {
            try
            {
                return assembly.GetManifestResourceNames();
            }
            catch (NotSupportedException)
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}
