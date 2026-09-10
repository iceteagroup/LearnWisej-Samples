using System;
using System.ComponentModel;
using Wisej.Web;

namespace OperationsConsole.Shell
{
    /// <summary>
    /// The record header that used to be copied onto every section page: a title, a record count, the time of the
    /// last refresh and a Refresh button.
    /// <para>
    /// This is the module's <b>reuse boundary</b>. Its whole public surface is three properties
    /// (<see cref="Title"/>, <see cref="RecordCount"/>, <see cref="LastRefresh"/>) and one event
    /// (<see cref="RefreshRequested"/>). <c>lblTitle</c>, <c>lblCount</c>, <c>lblRefreshed</c> and
    /// <c>btnRefresh</c> stay private, so they can be renamed, restyled or replaced tomorrow without any
    /// consuming page noticing — which is exactly what a consumer could <i>not</i> do if the controls were public.
    /// </para>
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

        /// <summary>How many records the page holds. The control decides how to word it.</summary>
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

        /// <summary>Raised when the user asks for a refresh. What that means is the consuming page's business.</summary>
        public event EventHandler RefreshRequested;

        private void btnRefresh_Click(object sender, EventArgs e)
            => this.RefreshRequested?.Invoke(this, EventArgs.Empty);
    }
}
