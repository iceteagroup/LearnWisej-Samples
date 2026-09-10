using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// Placeholder for the <b>Editors</b> section. Module 2 (Editors, Buttons, Validation, and Feedback) replaces the body of this page with
    /// the CustomerEditor UserControl: value-matched editors, ErrorProvider validators, Save / Reset / Validate with busy state, Toast feedback.
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
            ConsoleLog.Add("EditorsPage.RefreshSection() — placeholder, nothing to reload yet (Module 2 fills this page)");
        }
    }
}
