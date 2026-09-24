using System;
using Wisej.Web;

namespace IconDesk
{
    /// <summary>
    /// Five image sources of five different kinds - all of them strings the designer Image
    /// Selector can write - and a theme switch that separates the ones the theme owns from the
    /// ones it does not.
    /// </summary>
    /// <remarks>
    /// Nothing on this page decodes an image on the server. Every slot prints the source string
    /// it was given, so the page and <c>IconGallery.Designer.cs</c> say the same thing.
    /// </remarks>
    public partial class IconGalleryPage : Page
    {
        /// <summary>The theme owns this artwork outright, so it changes when the theme does.</summary>
        private const string ThemeImage = "icon-print";

        /// <summary>
        /// A file deployed beside the application. Its paths carry a literal stroke colour and
        /// turn their own fill off, so Wisej.NET renders it exactly as drawn.
        /// </summary>
        private const string ProjectSvg = "Images/company-logo.svg";

        /// <summary>
        /// An official pack's icon, addressed the way the designer's icon-pack explorer writes it:
        /// resource.wx, the assembly, the file. The form stores this string, not a copy of the
        /// artwork.
        /// </summary>
        private const string PackIcon = "resource.wx/Wisej.Ext.MaterialDesign/android-logo.svg";

        /// <summary>
        /// The same kind of source with a colour suffix. <c>highlight</c> is a theme colour
        /// <b>name</b>, so it resolves again under the next theme; a literal would not.
        /// </summary>
        private const string RecolouredIcon = "resource.wx/Wisej.Ext.MaterialDesign/save-button.svg?color=highlight";

        /// <summary>
        /// Which slots move when the theme changes, in slot order - read off the running page
        /// under both themes, not guessed from the mechanism.
        /// </summary>
        /// <remarks>
        /// The intuitive rule - "the theme owns theme images, everything else is a fixed file" -
        /// is wrong, and this array is the evidence. What decides is whether Wisej.NET reads the
        /// artwork as a recolourable icon. The pack SVG carries no fill of its own, so the fill
        /// Wisej.NET injects on the root is what paints it, and it changes with the theme even
        /// though nothing about its source string mentions a theme. The project mark sits in the
        /// same folder as everything else and does not move, because its paths fix their own
        /// colours. Where the file came from is irrelevant.
        /// </remarks>
        private static readonly bool[] FollowsTheTheme = { true, false, false, true, true };

        private GalleryPalette palette = GalleryPalette.Light;
        private bool switched;

        public IconGalleryPage()
        {
            InitializeComponent();

            AssignImageSources();
            ApplyPalette();
        }

        // ── the five mechanisms ─────────────────────────────────────────────────

        private void AssignImageSources()
        {
            this.slotThemeImage.Picture.ImageSource = ThemeImage;
            this.slotThemeImage.SourceText = ThemeImage;

            this.slotProjectSvg.Picture.ImageSource = ProjectSvg;
            this.slotProjectSvg.SourceText = ProjectSvg;

            // A complete external address. It is built at run time so the lab works on whichever
            // port it was started on; in a real project the designer writes the literal address.
            var absolute = Application.Url.TrimEnd('/') + "/cdn/users/42.png";
            this.slotAbsoluteUrl.Picture.ImageSource = absolute;
            this.slotAbsoluteUrl.SourceText = absolute.Replace("http://", string.Empty).Replace("https://", string.Empty);

            this.slotPackOne.Picture.ImageSource = PackIcon;
            this.slotPackOne.SourceText = "resource.wx/…/android-logo.svg";
            this.slotPackOne.Picture.ToolTipText = PackIcon;

            this.slotPackTwo.Picture.ImageSource = RecolouredIcon;
            this.slotPackTwo.SourceText = "…/save-button.svg?color=highlight";
            this.slotPackTwo.Picture.ToolTipText = RecolouredIcon;
        }

        // ── the theme switch ────────────────────────────────────────────────────

        private void btnLightTheme_Click(object sender, EventArgs e)
        {
            SwitchTo(GalleryPalette.Light);
        }

        private void btnDarkTheme_Click(object sender, EventArgs e)
        {
            SwitchTo(GalleryPalette.Dark);
        }

        private void SwitchTo(GalleryPalette next)
        {
            if (next == this.palette)
                return;

            // Application.Theme is a ClientTheme object, not a string. LoadTheme takes the name.
            Application.LoadTheme(next.ThemeName);

            this.palette = next;
            this.switched = true;

            ApplyPalette();
        }

        /// <summary>
        /// Repaints the page, and - once the theme has been switched at least once - puts the
        /// verdict on every card.
        /// </summary>
        private void ApplyPalette()
        {
            var p = this.palette;

            this.BackColor = p.Surface;
            this.pnlBody.BackColor = p.Surface;
            this.pnlTheme.BackColor = p.Surface;
            this.layoutSlots.BackColor = p.Surface;
            this.appTitleBar.BackColor = p.Bar;
            this.lblThemeCaption.ForeColor = p.Sub;

            StyleChip(this.btnLightTheme, p == GalleryPalette.Light, p);
            StyleChip(this.btnDarkTheme, p == GalleryPalette.Dark, p);

            var slots = new[] { this.slotThemeImage, this.slotProjectSvg, this.slotAbsoluteUrl, this.slotPackOne, this.slotPackTwo };

            for (var i = 0; i < slots.Length; i++)
                slots[i].ApplyPalette(p, FollowsTheTheme[i], this.switched);
        }

        private static void StyleChip(Button chip, bool active, GalleryPalette p)
        {
            chip.BackColor = active ? p.Active : p.Surface;
            chip.ForeColor = active ? p.OnActive : p.Sub;
            chip.CssStyle = "border:1px solid " + GalleryPalette.Css(active ? p.Active : p.Line) + ";border-radius:999px;";
        }
    }
}
