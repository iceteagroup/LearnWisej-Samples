using System;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// Left navigation region of the console. The rail owns its local layout (buttons anchored
    /// Top|Left|Right inside an AutoScroll container with hidden scrollbars) and exposes only
    /// <see cref="SelectedSection"/> and <see cref="SectionChanged"/>.
    /// </summary>
    public partial class NavigationRail : UserControl
    {
        private string _selectedSection = "Tickets";

        public NavigationRail()
        {
            InitializeComponent();
        }

        /// <summary>Raised after the user picks another section.</summary>
        public event EventHandler SectionChanged;

        /// <summary>The name of the selected section ("Dashboard", "Tickets", …).</summary>
        public string SelectedSection
        {
            get => _selectedSection;
            set
            {
                string section = string.IsNullOrWhiteSpace(value) ? "Tickets" : value.Trim();
                if (section == _selectedSection)
                    return;

                _selectedSection = section;
                SectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void btnNav_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
                SelectedSection = button.Text;
        }
    }
}
