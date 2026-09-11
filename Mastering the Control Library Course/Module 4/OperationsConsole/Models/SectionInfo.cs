namespace OperationsConsole.Models
{
    /// <summary>
    /// Catalog entry for one section: its stable key and the business name shown in the navigation and the status area.
    /// </summary>
    public sealed class SectionInfo
    {
        public SectionInfo(SectionKey key, string title)
        {
            Key = key;
            Title = title;
        }

        /// <summary>Stable identity of the section.</summary>
        public SectionKey Key { get; }

        /// <summary>The business name ("Lists and Trees").</summary>
        public string Title { get; }
    }
}
