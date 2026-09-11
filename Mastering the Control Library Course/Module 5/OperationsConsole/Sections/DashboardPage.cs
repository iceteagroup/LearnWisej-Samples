using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// Placeholder for the <b>Dashboard</b> section. Module 6 replaces it.
    /// </summary>
    public partial class DashboardPage : UserControl, ISection
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public string Title => "Dashboard";

        /// <inheritdoc/>
        public void RefreshSection()
        {
        }
    }
}
