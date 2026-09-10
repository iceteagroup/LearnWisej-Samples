using System;
using System.Collections.Generic;
using OperationsConsole.Models;
using OperationsConsole.Sections;
using Wisej.Web;

namespace OperationsConsole.Services
{
    /// <summary>
    /// The page factory of the shell: knows the six sections and creates their pages.
    /// <see cref="SimulateFailure"/> makes <see cref="CreatePage"/> throw, so the "page that cannot be created"
    /// path of the lab is reproducible from the UI (the <c>Simulate page failure</c> CheckBox in the command area).
    /// </summary>
    public sealed class SectionCatalog
    {
        private readonly List<SectionInfo> _sections = new List<SectionInfo>
        {
            new SectionInfo(SectionKey.Editors,      "Editors",         "editorsButton",    nameof(EditorsPage),      2),
            new SectionInfo(SectionKey.Layouts,      "Layouts",         "layoutsButton",    nameof(LayoutsPage),      3),
            new SectionInfo(SectionKey.ListsTrees,   "Lists and Trees", "listsTreesButton", nameof(ListsTreesPage),   4),
            new SectionInfo(SectionKey.DataGridView, "DataGridView",    "gridButton",       nameof(DataGridViewPage), 5),
            new SectionInfo(SectionKey.Dashboard,    "Dashboard",       "dashboardButton",  nameof(DashboardPage),    6),
            new SectionInfo(SectionKey.Widgets,      "Widgets",         "widgetsButton",    nameof(WidgetsPage),      7),
        };

        /// <summary>When true every page factory throws — the failure path of the lab.</summary>
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

        /// <summary>
        /// Creates the page for a section. Throws when <see cref="SimulateFailure"/> is set —
        /// the exception message is written to the Event log only, never shown to the user as-is.
        /// </summary>
        public UserControl CreatePage(SectionKey key)
        {
            var info = Get(key);

            if (SimulateFailure)
                throw new InvalidOperationException(
                    info.PageTypeName + " could not be created: simulated failure (\"Simulate page failure\" is checked). " +
                    "In a real application this is the exception a page constructor throws when a service or a package is missing.");

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
