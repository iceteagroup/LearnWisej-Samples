using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// Placeholder for the <b>Lists and Trees</b> section. Module 4 replaces it.
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
        }
    }
}
