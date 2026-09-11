using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// Placeholder for the <b>Layouts</b> section. Module 3 replaces it.
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
        }
    }
}
