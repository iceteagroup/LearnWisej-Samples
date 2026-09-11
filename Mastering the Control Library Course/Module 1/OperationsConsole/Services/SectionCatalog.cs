using System;
using System.Collections.Generic;
using OperationsConsole.Models;
using OperationsConsole.Sections;
using Wisej.Web;

namespace OperationsConsole.Services
{
    /// <summary>
    /// The page factory of the shell: knows the six sections and creates their pages.
    /// </summary>
    public sealed class SectionCatalog
    {
        private readonly List<SectionInfo> _sections = new List<SectionInfo>
        {
            new SectionInfo(SectionKey.Editors,      "Editors"),
            new SectionInfo(SectionKey.Layouts,      "Layouts"),
            new SectionInfo(SectionKey.ListsTrees,   "Lists and Trees"),
            new SectionInfo(SectionKey.DataGridView, "DataGridView"),
            new SectionInfo(SectionKey.Dashboard,    "Dashboard"),
            new SectionInfo(SectionKey.Widgets,      "Widgets"),
        };

        /// <summary>When true, <see cref="CreatePage"/> throws: the "page that cannot be created" path.</summary>
        public bool SimulateFailure { get; set; }

        /// <summary>The six sections in navigation order.</summary>
        public IReadOnlyList<SectionInfo> Sections => _sections;

        public SectionInfo Get(SectionKey key)
        {
            foreach (var s in _sections)
                if (s.Key == key)
                    return s;

            throw new ArgumentOutOfRangeException(nameof(key), key, "Unknown section.");
        }

        /// <summary>Creates the page for a section.</summary>
        public UserControl CreatePage(SectionKey key)
        {
            var info = Get(key);

            if (SimulateFailure)
                throw new InvalidOperationException("The " + info.Title + " page could not be created.");

            switch (key)
            {
                case SectionKey.Editors:      return new EditorsPage();
                case SectionKey.Layouts:      return new LayoutsPage();
                case SectionKey.ListsTrees:   return new ListsTreesPage();
                case SectionKey.DataGridView: return new DataGridViewPage();
                case SectionKey.Dashboard:    return new DashboardPage();
                case SectionKey.Widgets:      return new WidgetsPage();
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, "Unknown section.");
            }
        }
    }
}
