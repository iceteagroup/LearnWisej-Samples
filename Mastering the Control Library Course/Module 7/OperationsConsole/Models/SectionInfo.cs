namespace OperationsConsole.Models
{
    /// <summary>
    /// Catalog entry for one section: what the navigation shows, which page type opens it and which module builds it.
    /// </summary>
    public sealed class SectionInfo
    {
        public SectionInfo(SectionKey key, string title, string buttonName, string pageTypeName, int module)
        {
            Key = key;
            Title = title;
            ButtonName = buttonName;
            PageTypeName = pageTypeName;
            Module = module;
        }

        /// <summary>Stable identity of the section.</summary>
        public SectionKey Key { get; }

        /// <summary>The business name shown in the navigation and the status area ("Lists and Trees").</summary>
        public string Title { get; }

        /// <summary>Name of the navigation button that opens it (<c>listsTreesButton</c>).</summary>
        public string ButtonName { get; }

        /// <summary>The UserControl that renders it (<c>ListsTreesPage</c>).</summary>
        public string PageTypeName { get; }

        /// <summary>The course module that replaces the placeholder with the real section.</summary>
        public int Module { get; }
    }
}
