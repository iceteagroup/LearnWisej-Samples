using System;
using System.Collections.Generic;
using System.Drawing;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// The AccessOps shell: app bar, the five areas, the area on show and the "server said"
    /// strip. Module 1 builds the navigation only; Departments, Documents, Approvals and the
    /// admin console are empty screens that later modules fill.
    /// </summary>
    public class MainPage : Page
    {
        private static readonly string[] Areas = { "Departments", "Employees", "Documents", "Approvals", "Admin console" };

        private readonly AppBar _appBar = new AppBar();
        private readonly Panel _tabs = new Panel();
        private readonly Label _wireNote = new Label();
        private readonly Panel _host = new Panel();
        private readonly StatusStrip _status = new StatusStrip("SERVER SAID");
        private readonly Dictionary<string, Label> _tabLabels = new Dictionary<string, Label>();

        public MainPage(string initialStatus = null)
        {
            this.Name = "MainPage";
            this.Text = "AccessOps";
            this.BackColor = Color.White;

            _tabs.Name = "pnlAreas";
            _tabs.Dock = DockStyle.Top;
            _tabs.Height = 46;
            _tabs.BackColor = Theme.Strip;
            _tabs.Padding = new Padding(14, 8, 14, 8);
            _tabs.CssStyle = "border-bottom:1px solid #e4eaf1;";

            _wireNote.Name = "lblWireNote";
            _wireNote.Dock = DockStyle.Right;
            _wireNote.AutoSize = true;
            _wireNote.Font = Theme.Mono(11.5f);
            _wireNote.ForeColor = Theme.Muted;
            _wireNote.TextAlign = ContentAlignment.MiddleRight;
            _tabs.Controls.Add(_wireNote);

            // Dock order is the reverse of Controls.Add, so the tabs are added right to left.
            for (int i = Areas.Length - 1; i >= 0; i--)
            {
                var area = Areas[i];
                var tab = new Label
                {
                    Name = "tab" + area.Replace(" ", ""),
                    Text = area,
                    AutoSize = true,
                    Dock = DockStyle.Left,
                    Padding = new Padding(14, 0, 14, 0),
                    Margin = new Padding(0, 0, 6, 0),
                    Font = Theme.Px(12.5f, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand
                };
                tab.Click += (s, e) => ShowArea(area);
                _tabLabels[area] = tab;
                _tabs.Controls.Add(tab);
                if (i > 0)
                    _tabs.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 6 });
            }

            _host.Name = "pnlArea";
            _host.Dock = DockStyle.Fill;

            this.Controls.Add(_host);
            this.Controls.Add(_status);
            this.Controls.Add(_tabs);
            this.Controls.Add(_appBar);

            ShowArea("Employees");

            if (initialStatus != null)
                _status.Report(initialStatus, StatusTone.Idle);
        }

        internal void Report(string status, StatusTone tone, string note = null) => _status.Report(status, tone, note);

        internal string WireNote
        {
            get => _wireNote.Text;
            set => _wireNote.Text = value;
        }

        private void ShowArea(string area)
        {
            _appBar.Area = area;

            foreach (var pair in _tabLabels)
            {
                var active = pair.Key == area;
                var admin = pair.Key == "Admin console";
                pair.Value.ForeColor = admin ? Theme.BadInk : active ? Theme.Accent : Theme.Muted;
                pair.Value.BackColor = admin ? Theme.BadWash : active ? Theme.AccentWash : Color.Transparent;
                pair.Value.CssStyle = admin
                    ? "border-radius:7px; border:1px dashed #e0563b;"
                    : "border-radius:7px;";
            }

            foreach (Control old in _host.Controls)
                old.Dispose();
            _host.Controls.Clear();

            Control view = area == "Employees"
                ? new EmployeesView(this)
                : new EmptyAreaView(this, area);
            view.Dock = DockStyle.Fill;
            _host.Controls.Add(view);
        }
    }
}
