using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// Placeholder for the <b>Editors</b> section. Module 2 replaces it.
    /// </summary>
    public partial class EditorsPage : UserControl, ISection
    {
        public EditorsPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public string Title => "Editors";

        /// <inheritdoc/>
        public void RefreshSection()
        {
        }
    }
}
