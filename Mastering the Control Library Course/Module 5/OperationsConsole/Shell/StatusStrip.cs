using System;
using System.ComponentModel;
using Wisej.Web;

namespace OperationsConsole.Shell
{
    /// <summary>
    /// The compact status strip of a section page. Same public surface as <see cref="RecordHeader"/> —
    /// <see cref="Title"/>, <see cref="RecordCount"/>, <see cref="LastRefresh"/> and <see cref="RefreshRequested"/> —
    /// rendered as one line with a small refresh button.
    /// </summary>
    public partial class StatusStrip : UserControl
    {
        private string _title = "Section";
        private int _recordCount;
        private DateTime? _lastRefresh;

        public StatusStrip()
        {
            InitializeComponent();
            UpdateStrip();
        }

        /// <summary>The name of what the page is showing ("Layouts").</summary>
        [DefaultValue("Section")]
        public string Title
        {
            get => this._title;
            set { this._title = value; UpdateStrip(); }
        }

        /// <summary>How many records the page holds.</summary>
        [DefaultValue(0)]
        public int RecordCount
        {
            get => this._recordCount;
            set { this._recordCount = value; UpdateStrip(); }
        }

        /// <summary>When the page was last refreshed; <c>null</c> means "never".</summary>
        public DateTime? LastRefresh
        {
            get => this._lastRefresh;
            set { this._lastRefresh = value; UpdateStrip(); }
        }

        /// <summary>Raised when the user asks for a refresh.</summary>
        public event EventHandler RefreshRequested;

        private void UpdateStrip()
        {
            this.lblStrip.Text = this._title
                + " · " + (this._recordCount == 1 ? "1 record" : this._recordCount + " records")
                + " · " + (this._lastRefresh == null ? "never refreshed" : "refreshed " + this._lastRefresh.Value.ToString("HH:mm:ss"));
        }

        private void btnStripRefresh_Click(object sender, EventArgs e)
            => this.RefreshRequested?.Invoke(this, EventArgs.Empty);
    }
}
