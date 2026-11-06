using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AccessOps.Services;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The signed-in console. Its navigation is built from <see cref="SessionContext.Permissions"/>:
    /// an entry the permission set does not allow is not created at all. That is a courtesy - every
    /// service the areas call checks the same permission again, on the server, on every call.
    /// </summary>
    internal sealed class ConsolePage : Page, IAreaHost
    {
        // area, and the permissions any one of which makes it appear
        private static readonly (string Area, Permission[] Needs)[] Areas =
        {
            ("Dashboard", Array.Empty<Permission>()),
            ("Documents", new[] { Permission.ViewDepartmentDocuments, Permission.ViewAllDocuments }),
            ("Approvals", new[] { Permission.SubmitRequest, Permission.ApproveRequest }),
            ("Employees", new[] { Permission.ApproveRequest, Permission.ManageUsers }),
            ("Admin console", new[] { Permission.ManageUsers }),
        };

        private readonly IAccessOpsSession _session = AppHost.Get<IAccessOpsSession>();
        private readonly AppBar _appBar = new AppBar();
        private readonly Panel _nav = new Panel();
        private readonly Label _navNote = new Label();
        private readonly Panel _host = new Panel();
        private readonly StatusStrip _status = new StatusStrip("STATUS");
        private readonly Dictionary<string, Label> _navLabels = new Dictionary<string, Label>();
        private string _wireNote = "";

        public ConsolePage()
        {
            var me = _session.Current;   // throws when the session holds no identity

            this.Name = "ConsolePage";
            this.Text = "AccessOps";
            this.BackColor = Color.White;

            _nav.Name = "pnlNav";
            _nav.Dock = DockStyle.Top;
            _nav.Height = 46;
            _nav.BackColor = Theme.Strip;
            _nav.Padding = new Padding(16, 8, 16, 8);
            _nav.CssStyle = "border-bottom:1px solid #e4eaf1;";

            _host.Name = "pnlArea";
            _host.Dock = DockStyle.Fill;

            this.Controls.Add(_host);
            this.Controls.Add(_status);
            this.Controls.Add(_nav);
            this.Controls.Add(_appBar);

            BuildNavigation(me);
            ShowArea(Areas.First(a => Allowed(me, a.Needs)).Area);
        }

        public void Report(string status, StatusTone tone, string note = null) => _status.Report(status, tone, note);

        public string WireNote
        {
            get => _wireNote;
            set => _wireNote = value;
        }

        private static bool Allowed(SessionContext me, Permission[] needs) => needs.Length == 0 || needs.Any(me.Has);

        private void BuildNavigation(SessionContext me)
        {
            _nav.Controls.Clear();
            _navLabels.Clear();

            var visible = Areas.Where(a => Allowed(me, a.Needs)).ToList();

            _navNote.Name = "lblNavNote";
            _navNote.Dock = DockStyle.Right;
            _navNote.AutoSize = true;
            _navNote.Padding = new Padding(0, 0, 14, 0);
            _navNote.Font = Theme.Mono(11.5f);
            _navNote.ForeColor = Theme.Faint;
            _navNote.TextAlign = ContentAlignment.MiddleRight;
            _navNote.Text = $"{me.DisplayName} · {visible.Count} of {Areas.Length} entries · from SessionContext.Permissions";
            _nav.Controls.Add(_navNote);

            var btnSignOut = new Button { Name = "btnSignOut", Text = "Sign out", Dock = DockStyle.Right, Width = 84 };
            Theme.StyleButton(btnSignOut, primary: false);
            btnSignOut.Click += (s, e) => SignOutFlow.SignOut(SignOutReason.Explicit);
            _nav.Controls.Add(btnSignOut);

            for (int i = visible.Count - 1; i >= 0; i--)
            {
                var area = visible[i].Area;
                var item = new Label
                {
                    Name = "nav" + area.Replace(" ", ""),
                    Text = area,
                    Dock = DockStyle.Left,
                    AutoSize = true,
                    Padding = new Padding(13, 0, 13, 0),
                    Font = Theme.Px(12.5f, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand
                };
                item.Click += (s, e) => ShowArea(area);
                _navLabels[area] = item;
                _nav.Controls.Add(item);
                if (i > 0)
                    _nav.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 8 });
            }
        }

        private void ShowArea(string area)
        {
            // an administrator's "force re-resolve" takes effect at this session's next action
            var reResolved = AppHost.Get<PermissionRefreshService>().RefreshIfRequested();
            if (reResolved)
            {
                var me = _session.Current;
                BuildNavigation(me);
                if (!_navLabels.ContainsKey(area))
                    area = "Dashboard";
            }

            _appBar.Area = area;
            foreach (var pair in _navLabels)
            {
                var active = pair.Key == area;
                pair.Value.ForeColor = active ? Theme.Accent : Theme.Muted;
                pair.Value.BackColor = active ? Theme.AccentWash : Color.Transparent;
                pair.Value.CssStyle = "border-radius:7px;";
            }

            foreach (Control old in _host.Controls)
                old.Dispose();
            _host.Controls.Clear();

            Control view;
            try
            {
                view = area switch
                {
                    "Dashboard" => new DashboardView(this, _session.Current),
                    "Employees" => new EmployeesView(this),
                    "Documents" => new DocumentsView(this),
                    "Approvals" => new ApprovalsView(this),
                    "Admin console" => new AdminView(this),
                    _ => new EmptyAreaView(this, area)
                };
            }
            catch (NotAuthorizedException ex)
            {
                // the navigation was stale or forged: the service said no
                view = new EmptyAreaView(this, area);
                _status.Report(ex.Message, StatusTone.Bad);
            }

            view.Dock = DockStyle.Fill;
            _host.Controls.Add(view);

            if (reResolved)
                _status.Report("Permissions re-resolved by an administrator: " + _session.Current.Permissions, StatusTone.Warn);
        }
    }
}
