using System.Drawing;

namespace IconDesk
{
    /// <summary>
    /// The colours IconGallery paints itself with, one set per Wisej.NET theme it offers.
    /// </summary>
    /// <remarks>
    /// The page sets its own surface, panel and text colours rather than inheriting them, because
    /// the lab has to be able to say which pictures moved when the theme changed. If the whole
    /// page redrew itself from the theme there would be nothing to compare. Only the five image
    /// sources are left to the theme, and they are the subject of the exercise.
    /// </remarks>
    public sealed class GalleryPalette
    {
        public string ThemeName;
        public Color Surface;
        public Color Panel;
        public Color Ink;
        public Color Sub;
        public Color Line;
        public Color Bar;
        public Color Active;
        public Color OnActive;

        public static readonly GalleryPalette Light = new GalleryPalette
        {
            ThemeName = "Bootstrap-4",
            Surface = Color.FromArgb(0xFF, 0xFF, 0xFF),
            Panel = Color.FromArgb(0xF3, 0xF7, 0xFC),
            Ink = Color.FromArgb(0x1F, 0x2D, 0x3A),
            Sub = Color.FromArgb(0x5A, 0x6B, 0x7D),
            Line = Color.FromArgb(0xDC, 0xE5, 0xEF),
            Bar = Color.FromArgb(0x15, 0x65, 0xD8),
            Active = Color.FromArgb(0x15, 0x65, 0xD8),
            OnActive = Color.FromArgb(0xFF, 0xFF, 0xFF),
        };

        public static readonly GalleryPalette Dark = new GalleryPalette
        {
            ThemeName = "BootstrapDark-4",
            Surface = Color.FromArgb(0x1D, 0x25, 0x32),
            Panel = Color.FromArgb(0x26, 0x31, 0x41),
            Ink = Color.FromArgb(0xEA, 0xF1, 0xF9),
            Sub = Color.FromArgb(0x9F, 0xB2, 0xC7),
            Line = Color.FromArgb(0x36, 0x43, 0x5A),
            Bar = Color.FromArgb(0x10, 0x18, 0x23),
            Active = Color.FromArgb(0x6F, 0xB3, 0xFF),
            OnActive = Color.FromArgb(0x0F, 0x18, 0x22),
        };

        /// <summary>A CSS colour literal, for the places that need a border colour.</summary>
        public static string Css(Color color)
        {
            return "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");
        }
    }
}
