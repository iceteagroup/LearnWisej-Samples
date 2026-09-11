using System;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// The navigation rail: five <c>nav-item</c> buttons docked Top inside a <c>rail-surface</c> panel.
    /// The phone, tablet and small-desktop profiles switch it to icon-only (<see cref="IconOnly"/> sets
    /// <see cref="Button.Display"/> = Icon — a property value, no resize code); every button keeps its
    /// <c>ToolTipText</c> and <c>AccessibleName</c> so an icon-only rail stays discoverable. The selected
    /// item is the theme state <c>selected</c>, not a colour set in code.
    /// </summary>
    public partial class NavigationRail : UserControl
    {
        /// <summary>Raised when the user picks a view: "Dashboard", "Tickets", "Reports", "Settings" or "Help".</summary>
        public event EventHandler<string> Navigated;

        private bool _iconOnly;
        private string _selected = "Dashboard";

        public NavigationRail()
        {
            InitializeComponent();
            Select(_selected);
        }

        /// <summary>The view currently highlighted.</summary>
        public string Selected => _selected;

        /// <summary>Icon-only rendering for narrow profiles. Idempotent: assigning the same value twice changes nothing.</summary>
        public bool IconOnly
        {
            get => _iconOnly;
            set
            {
                _iconOnly = value;
                var display = value ? Display.Icon : Display.Both;
                foreach (var b in Buttons())
                    b.Display = display;
                this.lblNavTitle.Visible = !value;
                this.Padding = value ? new Padding(4, 8, 4, 8) : new Padding(8);
            }
        }

        /// <summary>Highlights one item (theme state <c>selected</c>) and clears the others. Does not raise <see cref="Navigated"/>.</summary>
        public void Select(string view)
        {
            _selected = view;
            foreach (var b in Buttons())
            {
                if (string.Equals((string)b.Tag, view, StringComparison.Ordinal))
                    b.AddState("selected");
                else
                    b.RemoveState("selected");
            }
        }

        private Button[] Buttons() => new[] { this.btnDashboard, this.btnTickets, this.btnReports, this.btnSettings, this.btnHelp };

        private void btnNav_Click(object sender, EventArgs e)
        {
            if (!(sender is Button button) || !(button.Tag is string view))
                return;

            Select(view);
            Navigated?.Invoke(this, view);
        }
    }
}
