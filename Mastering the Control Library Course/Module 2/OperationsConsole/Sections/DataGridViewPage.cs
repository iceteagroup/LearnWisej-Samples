using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// Placeholder for the <b>DataGridView</b> section. Module 5 replaces it.
    /// </summary>
    public partial class DataGridViewPage : UserControl, ISection
    {
        public DataGridViewPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public string Title => "DataGridView";

        /// <inheritdoc/>
        public void RefreshSection()
        {
        }
    }
}
