using System;
using System.Collections.Generic;
using System.Drawing;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The signed-in console: app bar, the navigation column, the area on show and the status
    /// strip. It is built from <see cref="SessionContext"/> - the typed accessor throws when the
    /// session holds no identity, so this page cannot exist for an anonymous session.
    /// </summary>
    internal sealed class ConsolePage : Page, IAreaHost
    {
        private static readonly string[] Areas = { "Dashboard", "Employees", "Documents", "Approvals", "Admin console" };

        private readonly SessionContext _me;
        private readonly AppBar _appBar = new AppBar();
        private readonly Panel _nav = new Panel();
        private readonly Panel _host = new Panel();
        private readonly StatusStrip _status = new StatusStrip("SERVER SAID");
        private readonly Dictionary<string, Label> _navLabels = new Dictionary<string, Label>();
        private string _wireNote = "";

        public ConsolePage()
        {
            _me = AppHost.Get<IAccessOpsSession>().Current;

            this.Name = "ConsolePage";
            this.Text = "AccessOps";
            this.BackColor = Color.White;
            _appBar.Area = "Console";

            BuildNavigation();

            _host.Name = "pnlArea";
            _host.Dock = DockStyle.Fill;

            this.Controls.Add(_host);
            this.Controls.Add(_status);
            this.Controls.Add(_nav);
            this.Controls.Add(_appBar);

            ShowArea("Dashboard");
        }

        public void Report(string status, StatusTone tone, string note = null) => _status.Report(status, tone, note);

        public string WireNote
        {
            get => _wireNote;
            set => _wireNote = value;
        }

        private void BuildNavigation()
        {
            _nav.Name = "pnlNav";
            _nav.Dock = DockStyle.Left;
            _nav.Width = 176;
            _nav.BackColor = Theme.HeaderBack;
            _nav.Padding = new Padding(11, 15, 11, 15);
            _nav.CssStyle = "border-right:1px solid #e4eaf1;";

            // bottom block, top to bottom: the admin note, who is signed in, Sign out.
            // Docking runs in reverse of Controls.Add, so the control added last sits lowest.
            if (!_me.IsManager)
            {
                _nav.Controls.Add(new Label
                {
                    Name = "lblAdminNote",
                    Text = "Admin console hidden — the SessionContext says this employee does not manage anyone.",
                    Dock = DockStyle.Bottom,
                    AutoSize = false,
                    Height = 64,
                    Font = Theme.Px(11f),
                    ForeColor = ColorTranslator.FromHtml("#8a9aab")
                });
            }

            _nav.Controls.Add(new Label
            {
                Name = "lblSignedInAs",
                Text = _me.DisplayName + " · " + _me.Department,
                Dock = DockStyle.Bottom,
                AutoSize = false,
                Height = 30,
                Font = Theme.Px(12f, FontStyle.Bold),
                ForeColor = Theme.Text,
                TextAlign = ContentAlignment.MiddleLeft
            });

            var btnSignOut = new Button { Name = "btnSignOut", Text = "Sign out", Dock = DockStyle.Bottom, Height = 32 };
            Theme.StyleButton(btnSignOut, primary: false);
            btnSignOut.Click += (s, e) => SignOutFlow.SignOut(SignOutReason.Explicit);
            _nav.Controls.Add(btnSignOut);

            for (int i = Areas.Length - 1; i >= 0; i--)
            {
                var area = Areas[i];
                var enabled = area != "Admin console" || _me.IsManager;
                var item = new Label
                {
                    Name = "nav" + area.Replace(" ", ""),
                    Text = area,
                    Dock = DockStyle.Top,
                    AutoSize = false,
                    Height = 36,
                    Padding = new Padding(13, 0, 0, 0),
                    Font = Theme.Px(13f),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = enabled ? Cursors.Hand : Cursors.Default,
                    Enabled = enabled
                };
                if (enabled)
                    item.Click += (s, e) => ShowArea(area);
                _navLabels[area] = item;
                _nav.Controls.Add(item);
                _nav.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 4 });
            }
        }

        private void ShowArea(string area)
        {
            foreach (var pair in _navLabels)
            {
                var active = pair.Key == area;
                pair.Value.Font = Theme.Px(13f, active ? FontStyle.Bold : FontStyle.Regular);
                pair.Value.ForeColor = !pair.Value.Enabled ? ColorTranslator.FromHtml("#a9b8c8") : active ? Theme.Accent : Theme.Muted;
                pair.Value.BackColor = active ? Theme.AccentWash : Color.Transparent;
                pair.Value.CssStyle = "border-radius:8px;";
            }

            foreach (Control old in _host.Controls)
                old.Dispose();
            _host.Controls.Clear();

            Control view = area switch
            {
                "Dashboard" => new DashboardView(this, _me),
                "Employees" => new EmployeesView(this),
                "Documents" => new DocumentsView(this),
                "Approvals" => new ApprovalsView(this),
                _ => new EmptyAreaView(this, area)
            };
            view.Dock = DockStyle.Fill;
            _host.Controls.Add(view);
        }
    }
}
