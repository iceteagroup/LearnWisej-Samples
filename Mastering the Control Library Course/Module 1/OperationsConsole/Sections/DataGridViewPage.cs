using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// Placeholder for the <b>DataGridView</b> section. Module 5 (DataGridView Mastery) replaces the body of this page with
    /// the Orders grid: BindingSource + explicit columns, HTML status badge, custom editor, virtual mode with a cache, filter and status strips.
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
            ConsoleLog.Add("DataGridViewPage.RefreshSection() — placeholder, nothing to reload yet (Module 5 fills this page)");
        }
    }
}
