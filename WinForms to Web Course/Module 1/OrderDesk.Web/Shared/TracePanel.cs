using System;
using System.Globalization;
using Wisej.Web;

namespace OrderDesk.Shared
{
    public enum TraceKind
    {
        /// <summary>A server-side decision or event (• server).</summary>
        Server,
        /// <summary>Something the server sent to the browser (→ .NET→JS).</summary>
        ServerToClient,
        /// <summary>Something the browser sent to the server (← JS→.NET).</summary>
        ClientToServer,
        /// <summary>A migration finding worth writing into migration-log.md (★ log).</summary>
        Finding,
        /// <summary>A failure path (✖ fail).</summary>
        Failure,
        /// <summary>A recovered/verified path (✓ ok).</summary>
        Ok
    }

    /// <summary>
    /// The right-hand "migration trace" card every module sample shows: a timestamped log of what
    /// the server did, what crossed the wire, and every migration finding. A lab prop, not part of
    /// the migrated product.
    /// </summary>
    public class TracePanel : Panel
    {
        private readonly Label _title;
        private readonly ListBox _list;
        private readonly Label _footer;
        private readonly Button _clear;

        public TracePanel()
        {
            this.BackColor = Palette.CardBackground;
            this.BorderStyle = BorderStyle.Solid;
            this.Padding = new Padding(0);

            _title = new Label
            {
                AutoSize = false,
                Text = "Server ⇄ Client  ·  migration trace",
                Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(20, 0, 0, 0),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };
            _clear = new Button
            {
                Text = "Clear",
                Size = new System.Drawing.Size(72, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                ToolTipText = "Empty the trace",
            };
            _clear.Click += (s, e) => Clear();
            _title.Controls.Add(_clear);

            _footer = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Bottom,
                Height = 30,
                Padding = new Padding(20, 0, 20, 0),
                ForeColor = Palette.MutedText,
                Font = new System.Drawing.Font("default", 9F),
                Text = "• server   → .NET→JS   ← JS→.NET   ★ migration finding   ✖ failure path   ✓ verified",
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };
            _list = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("monospace", 9F),
                BorderStyle = BorderStyle.None,
            };

            this.Controls.Add(_list);
            this.Controls.Add(_title);
            this.Controls.Add(_footer);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            _clear.Location = new System.Drawing.Point(_title.Width - _clear.Width - 16, (_title.Height - _clear.Height) / 2);
        }

        public string Title { get => _title.Text; set => _title.Text = value; }
        public string Legend { get => _footer.Text; set => _footer.Text = value; }
        public int Count => _list.Items.Count;

        public void Add(TraceKind kind, string name, string payload = "")
        {
            string prefix;
            switch (kind)
            {
                case TraceKind.ServerToClient: prefix = "→ .NET→JS "; break;
                case TraceKind.ClientToServer: prefix = "← JS→.NET "; break;
                case TraceKind.Finding: prefix = "★ log     "; break;
                case TraceKind.Failure: prefix = "✖ fail    "; break;
                case TraceKind.Ok: prefix = "✓ ok      "; break;
                default: prefix = "• server  "; break;
            }
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            _list.Items.Add(time + "  " + prefix + " " + name.PadRight(30) + " " + payload);
            _list.SelectedIndex = _list.Items.Count - 1;
        }

        public void Server(string name, string payload = "") => Add(TraceKind.Server, name, payload);
        public void Out(string name, string payload = "") => Add(TraceKind.ServerToClient, name, payload);
        public void In(string name, string payload = "") => Add(TraceKind.ClientToServer, name, payload);
        public void Finding(string name, string payload = "") => Add(TraceKind.Finding, name, payload);
        public void Fail(string name, string payload = "") => Add(TraceKind.Failure, name, payload);
        public void Ok(string name, string payload = "") => Add(TraceKind.Ok, name, payload);

        public void Clear() => _list.Items.Clear();
    }
}
