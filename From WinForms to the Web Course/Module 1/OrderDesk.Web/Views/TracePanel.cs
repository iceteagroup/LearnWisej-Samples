using System;
using System.Globalization;
using Wisej.Web;

namespace OrderDesk.Views
{
    public enum TraceKind
    {
        /// <summary>Server-side decision or business-logic call.</summary>
        Server,
        /// <summary>Something rendered/pushed to the browser (state out).</summary>
        ToClient,
        /// <summary>An event that arrived from the browser (events in).</summary>
        FromClient,
        /// <summary>A desktop assumption that was hit and replaced.</summary>
        Boundary
    }

    /// <summary>
    /// The right-hand "Migration log · live trace" card every module sample shares: every user
    /// action, every business-logic call and every browser-boundary decision, with a timestamp.
    /// </summary>
    public class TracePanel : UserControl
    {
        private Label _title;
        private ListBox _list;
        private Label _footer;

        public TracePanel()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => _title.Text;
            set => _title.Text = value;
        }

        public string Footer
        {
            get => _footer.Text;
            set => _footer.Text = value;
        }

        public void Add(TraceKind kind, string name, string payload = "")
        {
            string prefix = kind switch
            {
                TraceKind.ToClient => "→ .NET→JS ",
                TraceKind.FromClient => "← JS→.NET ",
                TraceKind.Boundary => "⚠ boundary",
                _ => "• server  ",
            };
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            _list.Items.Add($"{time}  {prefix} {name,-34} {payload}");
            _list.SelectedIndex = _list.Items.Count - 1;
        }

        public void Clear() => _list.Items.Clear();

        public int Count => _list.Items.Count;

        private void InitializeComponent()
        {
            this._title = new Label();
            this._list = new ListBox();
            this._footer = new Label();
            this.SuspendLayout();
            //
            // _title
            //
            this._title.Dock = DockStyle.Top;
            this._title.AutoSize = false;
            this._title.Font = Ui.CardTitle;
            this._title.Height = 38;
            this._title.Padding = new Padding(16, 6, 16, 0);
            this._title.Text = "Migration log  ·  live trace";
            //
            // _list
            //
            this._list.Dock = DockStyle.Fill;
            this._list.Font = Ui.Mono;
            this._list.Margin = new Padding(16, 0, 16, 0);
            this._list.HorizontalScrollbar = true;
            //
            // _footer
            //
            this._footer.Dock = DockStyle.Bottom;
            this._footer.AutoSize = false;
            this._footer.ForeColor = Ui.Muted;
            this._footer.Height = 30;
            this._footer.Padding = new Padding(16, 6, 16, 0);
            this._footer.Text = "• server  business logic   ·   → .NET→JS state out   ·   ← JS→.NET events in   ·   ⚠ boundary desktop assumption replaced";
            //
            // TracePanel
            //
            this.BackColor = Ui.CardBack;
            this.BorderStyle = BorderStyle.Solid;
            this.Padding = new Padding(4, 4, 4, 4);
            this.Controls.Add(this._list);
            this.Controls.Add(this._title);
            this.Controls.Add(this._footer);
            this.Size = new System.Drawing.Size(600, 560);
            this.ResumeLayout(false);
        }
    }
}
