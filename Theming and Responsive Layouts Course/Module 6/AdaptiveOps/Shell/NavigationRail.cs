using System;
using System.Collections.Generic;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>How the rail renders: full labels (desktop) or icon-only buttons (tablet, small desktop).</summary>
    public enum RailMode
    {
        Full,
        IconOnly
    }

    public sealed class SectionEventArgs : EventArgs
    {
        public SectionEventArgs(string section)
        {
            Section = section;
        }

        public string Section { get; }
    }

    /// <summary>
    /// The navigation rail as a UserControl (layout in NavigationRail.Designer.cs).
    ///
    /// Module 6 gives it one behaviour switch, <see cref="SetMode"/>: in <see cref="RailMode.IconOnly"/> every
    /// button renders <c>Display = Display.Icon</c> and keeps its label as <c>ToolTipText</c> / <c>AccessibleName</c>,
    /// so the rail still names its sections for screen readers and on hover. Hiding the rail altogether on the
    /// Phone profiles is the page's decision (it owns the region panel) — the page then offers the same five
    /// sections through the toolbar "Menu" button (<see cref="Sections"/>), so nothing is lost on a phone.
    ///
    /// In Visual Studio the Display values below would be assigned per profile in the Designer's responsive
    /// profile dropdown (Control.ResponsiveProfiles); here they are written out so the mapping is readable.
    /// </summary>
    public partial class NavigationRail : UserControl
    {
        /// <summary>Width of the navigation region when the rail shows labels.</summary>
        public const int FullWidth = 220;

        /// <summary>Width of the navigation region when the rail is icon-only.</summary>
        public const int IconOnlyWidth = 64;

        private Button[] _buttons;

        /// <summary>Raised when the operator picks a section (button click or, on a phone, a menu item).</summary>
        public event EventHandler<SectionEventArgs> SectionSelected;

        public NavigationRail()
        {
            InitializeComponent();
            _buttons = new[] { this.btnNavDashboard, this.btnNavTickets, this.btnNavReports, this.btnNavSettings, this.btnNavHelp };
            Mode = RailMode.Full;
        }

        public RailMode Mode { get; private set; }

        /// <summary>The section names in rail order; the phone "Menu" button lists exactly these.</summary>
        public IReadOnlyList<string> Sections
        {
            get
            {
                var names = new List<string>(_buttons.Length);
                foreach (var b in _buttons)
                    names.Add(b.Text);
                return names;
            }
        }

        /// <summary>The width the hosting region should take for the given mode.</summary>
        public static int WidthFor(RailMode mode) => mode == RailMode.IconOnly ? IconOnlyWidth : FullWidth;

        /// <summary>
        /// Idempotent: applying the same mode twice changes nothing. Display.Icon drops the label from the
        /// rendered button; the label survives as the tooltip and the accessible name.
        /// </summary>
        public void SetMode(RailMode mode)
        {
            foreach (var b in _buttons)
            {
                b.Display = mode == RailMode.IconOnly ? Display.Icon : Display.Both;
                b.ToolTipText = b.Text;
                b.AccessibleName = b.Text;
            }

            this.lblNavTitle.Visible = mode == RailMode.Full;
            Mode = mode;
        }

        /// <summary>Raises <see cref="SectionSelected"/> for a section chosen elsewhere (the phone menu).</summary>
        public void Select(string section)
        {
            SectionSelected?.Invoke(this, new SectionEventArgs(section));
        }

        private void btnNav_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
                Select(button.Text);
        }
    }
}
