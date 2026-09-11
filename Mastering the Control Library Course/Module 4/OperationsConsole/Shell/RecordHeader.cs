using System;
using System.ComponentModel;
using Wisej.Web;

namespace OperationsConsole.Shell
{
    /// <summary>
    /// The record header of a section page: a title, a record count, the time of the last refresh and a Refresh
    /// button. Its public surface is <see cref="Title"/>, <see cref="RecordCount"/>, <see cref="LastRefresh"/> and
    /// <see cref="RefreshRequested"/>; every child control is private.
    /// </summary>
    public partial class RecordHeader : UserControl
    {
        private int _recordCount;
        private DateTime? _lastRefresh;

        public RecordHeader()
        {
            InitializeComponent();
        }

        /// <summary>The name of what the page is showing ("Layouts").</summary>
        [DefaultValue("Section")]
        public string Title
        {
            get => this.lblTitle.Text;
            set => this.lblTitle.Text = value;
        }

        /// <summary>How many records the page holds.</summary>
        [DefaultValue(0)]
        public int RecordCount
        {
            get => this._recordCount;
            set
            {
                this._recordCount = value;
                this.lblCount.Text = value == 1 ? "1 record" : value + " records";
            }
        }

        /// <summary>When the page was last refreshed; <c>null</c> means "never".</summary>
        public DateTime? LastRefresh
        {
            get => this._lastRefresh;
            set
            {
                this._lastRefresh = value;
                this.lblRefreshed.Text = value == null
                    ? "never refreshed"
                    : "refreshed " + value.Value.ToString("HH:mm:ss");
            }
        }

        /// <summary>Raised when the user asks for a refresh.</summary>
        public event EventHandler RefreshRequested;

        private void btnRefresh_Click(object sender, EventArgs e)
            => this.RefreshRequested?.Invoke(this, EventArgs.Empty);
    }
}
