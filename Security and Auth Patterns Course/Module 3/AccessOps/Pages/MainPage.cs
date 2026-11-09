using System.Collections.Generic;
using System.Drawing;
using AccessOps.Identity;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The AccessOps window: the blue app bar, the area strip
    /// (Departments · Employees · Documents · Approvals · Admin console) and the screen on show.
    /// On the intranet route the page is created by <c>Program.Main</c> after the host signed the
    /// Windows user in; it keeps the <see cref="IntranetUser"/> found at start-up (masked account
    /// and subject), never a token, and every protected action is decided by the services.
    /// </summary>
    public class MainPage : Page
    {
        private static readonly string[] Areas = { "Departments", "Employees", "Documents", "Approvals", "Admin console" };

        private readonly AppBar _appBar = new AppBar();
        private readonly FlowLayoutPanel _strip = new FlowLayoutPanel();
        private readonly Panel _host = new Panel();
        private readonly Dictionary<string, Label> _tabs = new Dictionary<string, Label>();
        private string _area = "Documents";

        public MainPage(IntranetUser user)
        {
            this.SignedInUser = user;
            this.Name = "MainPage";
            this.Text = "AccessOps";
            this.BackColor = Color.White;

            _strip.Name = "pnlAreas";
            _strip.Dock = DockStyle.Top;
            _strip.Height = 34;
            _strip.BackColor = Theme.Strip;
            _strip.Padding = new Padding(10, 5, 10, 0);
            _strip.FlowDirection = FlowDirection.LeftToRight;
            _strip.WrapContents = false;
            _strip.CssStyle = "border-bottom:1px solid #e0e7ef;";

            foreach (var area in Areas)
            {
                var tab = new Label
                {
                    Name = "tab" + area.Replace(" ", ""),
                    Text = area,
                    AutoSize = true,
                    Height = 24,
                    Padding = new Padding(12, 0, 12, 0),
                    Margin = new Padding(0, 0, 4, 0),
                    Font = Theme.Px(12.5f, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand,
                    CssStyle = "border-radius:6px;"
                };
                tab.Click += (s, e) => ShowArea(area);
                _tabs[area] = tab;
                _strip.Controls.Add(tab);
            }

            _host.Name = "pnlScreen";
            _host.Dock = DockStyle.Fill;

            this.Controls.Add(_host);
            this.Controls.Add(_strip);
            this.Controls.Add(_appBar);

            ShowArea("Documents");
        }

        /// <summary>The Windows user of this session, as <c>Program.Main</c> found it.</summary>
        internal IntranetUser SignedInUser { get; }

        /// <summary>Puts a screen into the window and names it in the app bar.</summary>
        internal void ShowScreen(string screenTitle, Control screen)
        {
            _appBar.Screen = screenTitle;

            foreach (Control old in _host.Controls)
                old.Dispose();
            _host.Controls.Clear();

            screen.Dock = DockStyle.Fill;
            _host.Controls.Add(screen);
        }

        private void ShowArea(string area)
        {
            _area = area;
            foreach (var pair in _tabs)
            {
                var active = pair.Key == area;
                pair.Value.ForeColor = active ? Theme.Accent : Theme.Muted;
                pair.Value.BackColor = active ? Theme.AccentWash : Color.Transparent;
                pair.Value.Font = Theme.Px(12.5f, active ? FontStyle.Bold : FontStyle.Regular);
            }

            if (area == "Admin console")
                ShowScreen("Admin console", new AdminConsoleView(this));
            else if (area == "Documents")
                ShowScreen("Confidential documents", new DocumentsView(this.SignedInUser));
            else
                ShowScreen(area, new EmptyAreaView());
        }
    }
}
