namespace IconDesk.Icons
{
    /// <summary>
    /// The IconDesk icon pack, as a catalog of image-source strings.
    ///
    /// Callers never type a path. A renamed icon file breaks the build here and nowhere else, a
    /// mistyped name is a compiler error rather than a blank control at run time, and
    /// "which icons do we have?" is answered by IntelliSense instead of by opening a folder.
    /// The official Wisej-4 packs expose exactly this shape, which is not a coincidence - it is
    /// the interface an icon pack should have.
    /// </summary>
    public static class AppIcons
    {
        /// <summary>
        /// The assembly this pack lives in. Wisej.NET serves an embedded resource at
        /// <c>resource.wx/&lt;assembly&gt;/&lt;file&gt;</c>, and qualifying the URL means nothing
        /// on a deployment's disk can quietly replace a shipped icon.
        /// </summary>
        public const string Assembly = "IconDesk.Icons";

        /// <summary>The pack's own version, shown in the gallery toolbar.</summary>
        public const string Version = "1.0.0";

        private const string Prefix = "resource.wx/" + Assembly + "/";

        // ── the vocabulary, in the order the gallery shows it ───────────────────

        public const string Save = Prefix + "save.svg";
        public const string Delete = Prefix + "delete.svg";
        public const string Search = Prefix + "search.svg";
        public const string Filter = Prefix + "filter.svg";
        public const string Print = Prefix + "print.svg";
        public const string Export = Prefix + "export.svg";
        public const string Customer = Prefix + "customer.svg";
        public const string Order = Prefix + "order.svg";
        public const string Settings = Prefix + "settings.svg";
        public const string Success = Prefix + "status-success.svg";
        public const string Warning = Prefix + "status-warning.svg";
        public const string Error = Prefix + "status-error.svg";

        /// <summary>
        /// Every icon in the pack, by name, in gallery order. Useful for a gallery screen and for
        /// the test that asserts the catalog and the embedded resources have not drifted apart.
        /// </summary>
        public static readonly System.Collections.Generic.IReadOnlyList<System.Collections.Generic.KeyValuePair<string, string>> All =
            new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>>
            {
                new System.Collections.Generic.KeyValuePair<string, string>(nameof(Save), Save),
                new System.Collections.Generic.KeyValuePair<string, string>(nameof(Delete), Delete),
                new System.Collections.Generic.KeyValuePair<string, string>(nameof(Search), Search),
                new System.Collections.Generic.KeyValuePair<string, string>(nameof(Filter), Filter),
                new System.Collections.Generic.KeyValuePair<string, string>(nameof(Print), Print),
                new System.Collections.Generic.KeyValuePair<string, string>(nameof(Export), Export),
                new System.Collections.Generic.KeyValuePair<string, string>(nameof(Customer), Customer),
                new System.Collections.Generic.KeyValuePair<string, string>(nameof(Order), Order),
                new System.Collections.Generic.KeyValuePair<string, string>(nameof(Settings), Settings),
                new System.Collections.Generic.KeyValuePair<string, string>(nameof(Success), Success),
                new System.Collections.Generic.KeyValuePair<string, string>(nameof(Warning), Warning),
                new System.Collections.Generic.KeyValuePair<string, string>(nameof(Error), Error),
            };

        /// <summary>
        /// Appends the recolour suffix. The pack's artwork is monochrome on purpose and every
        /// shape in it strokes with <c>currentColor</c>, so every icon accepts one - see
        /// ICON-PACK-RULES.md for the file that did not, and what was wrong with it.
        /// </summary>
        /// <param name="icon">One of the constants on this class.</param>
        /// <param name="colour">A theme colour name such as <c>highlight</c>, or a literal.</param>
        public static string Coloured(string icon, string colour)
        {
            return icon + "?color=" + colour;
        }
    }
}
