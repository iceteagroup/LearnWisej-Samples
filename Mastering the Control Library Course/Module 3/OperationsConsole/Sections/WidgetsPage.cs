using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// Placeholder for the <b>Widgets</b> section. Module 7 (Custom Widgets, Extensions, Theming, and Capstone) replaces the body of this page with
    /// the ratingWidget (Widget + rating.js / rating.css), WidgetEvent to RatingService, CallAsync("setSaved"), theming, capstone.
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
            ConsoleLog.Add("WidgetsPage.RefreshSection() — placeholder, nothing to reload yet (Module 7 fills this page)");
        }
    }
}
