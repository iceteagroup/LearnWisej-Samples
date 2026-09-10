using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// Placeholder for the <b>Dashboard</b> section. Module 6 (Charts, Dashboards, Content, Media, and Documents) replaces the body of this page with
    /// the dashboard: ChartJS trend, ProgressBar, PdfViewer / HtmlPanel preview, Upload workflow, one RefreshDashboard(model).
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
            ConsoleLog.Add("DashboardPage.RefreshSection() — placeholder, nothing to reload yet (Module 6 fills this page)");
        }
    }
}
