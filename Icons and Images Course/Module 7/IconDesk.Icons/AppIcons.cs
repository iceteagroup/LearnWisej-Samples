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

        private const string Prefix = "resource.wx/" + Assembly + "/";

        public const string Add = Prefix + "add.svg";
        public const string Delete = Prefix + "delete.svg";
        public const string Edit = Prefix + "edit.svg";
        public const string Export = Prefix + "export.svg";
        public const string Filter = Prefix + "filter.svg";
        public const string Info = Prefix + "info.svg";
        public const string Refresh = Prefix + "refresh.svg";
        public const string Save = Prefix + "save.svg";
        public const string Search = Prefix + "search.svg";
        public const string Settings = Prefix + "settings.svg";
        public const string User = Prefix + "user.svg";
        public const string Warning = Prefix + "warning.svg";

        /// <summary>
        /// Every icon in the pack, by name. Useful for a gallery screen and for the test that
        /// asserts the catalog and the embedded resources have not drifted apart.
        /// </summary>
        public static readonly System.Collections.Generic.IReadOnlyDictionary<string, string> All =
            new System.Collections.Generic.Dictionary<string, string>
            {
                { nameof(Add), Add },
                { nameof(Delete), Delete },
                { nameof(Edit), Edit },
                { nameof(Export), Export },
                { nameof(Filter), Filter },
                { nameof(Info), Info },
                { nameof(Refresh), Refresh },
                { nameof(Save), Save },
                { nameof(Search), Search },
                { nameof(Settings), Settings },
                { nameof(User), User },
                { nameof(Warning), Warning },
            };

        /// <summary>
        /// Appends the recolour suffix. The pack's artwork is monochrome on purpose, so every icon
        /// in it accepts one - see the note on SVG cleanup in the module docs.
        /// </summary>
        /// <param name="icon">One of the constants on this class.</param>
        /// <param name="colour">A theme colour name such as <c>highlight</c>, or a literal.</param>
        public static string Coloured(string icon, string colour) => icon + "?color=" + colour;
    }
}
