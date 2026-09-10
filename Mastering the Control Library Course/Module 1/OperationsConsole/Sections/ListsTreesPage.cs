using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// Placeholder for the <b>Lists and Trees</b> section. Module 4 (Lists, Trees, Repeaters, and Hierarchical Data) replaces the body of this page with
    /// the document explorer: lazy TreeView, virtual-mode ListView, shared ImageList, DocumentDetailControl.
    /// </summary>
    public partial class ListsTreesPage : UserControl, ISection
    {
        public ListsTreesPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public string Title => "Lists and Trees";

        /// <inheritdoc/>
        public void RefreshSection()
        {
            ConsoleLog.Add("ListsTreesPage.RefreshSection() — placeholder, nothing to reload yet (Module 4 fills this page)");
        }
    }
}
