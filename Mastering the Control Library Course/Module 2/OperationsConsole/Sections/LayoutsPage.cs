using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// Placeholder for the <b>Layouts</b> section. Module 3 (Containers, Layouts, Navigation, and Reuse) replaces the body of this page with
    /// the real shell (SplitContainer, TabControl, ToolBar, StatusBar) plus the RecordHeader / StatusStrip UserControls and the narrow profile.
    /// </summary>
    public partial class LayoutsPage : UserControl, ISection
    {
        public LayoutsPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public string Title => "Layouts";

        /// <inheritdoc/>
        public void RefreshSection()
        {
            ConsoleLog.Add("LayoutsPage.RefreshSection() — placeholder, nothing to reload yet (Module 3 fills this page)");
        }
    }
}
