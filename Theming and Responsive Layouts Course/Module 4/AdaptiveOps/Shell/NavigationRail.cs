using System;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// Left navigation region of the console (Dock = Left in MainPage). The rail owns its local
    /// layout — seven buttons anchored Top|Left|Right inside an AutoScroll container with hidden
    /// scrollbars (NavigationRail.Designer.cs) — and exposes exactly two members to the shell:
    /// <see cref="SelectedSection"/> and <see cref="SectionChanged"/>. Every child control is
    /// private, so Module 6 can dock this region anywhere, or collapse it to an icon strip,
    /// without the page referencing a single button.
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
                this.lblSelected.Text = "Section: " + section;
                SectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Height of the rail's content (last control's bottom + AutoScrollMargin), so the shell can
        /// log whether the rail scrolls at the current browser height. Computed from the layout the
        /// designer declared, not measured in the browser.
        /// </summary>
        public int ContentHeight => this.lblSelected.Bottom + this.AutoScrollMargin.Height;

        /// <summary>One line for the trace: "rail 212×588 · content 396 px → fits" or "… → scrolls by N px (AutoScroll, ScrollBars=Hidden)".</summary>
        public string DescribeScroll()
        {
            int content = ContentHeight;
            int visible = this.ClientSize.Height;
            return $"rail {this.Width}×{this.Height} · content {content} px → " +
                   (content > visible
                        ? $"scrolls by {content - visible} px (AutoScroll, ScrollBars={this.ScrollBars})"
                        : "fits");
        }

        private void btnNav_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
                SelectedSection = button.Text;
        }
    }
}
