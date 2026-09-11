using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// Placeholder for the <b>Widgets</b> section. Module 7 replaces it.
    /// </summary>
    public partial class WidgetsPage : UserControl, ISection
    {
        public WidgetsPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public string Title => "Widgets";

        /// <inheritdoc/>
        public void RefreshSection()
        {
        }
    }
}
