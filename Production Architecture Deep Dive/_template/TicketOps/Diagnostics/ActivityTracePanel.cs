using System;
using System.Globalization;
using TicketOps.Infrastructure;
using Wisej.Web;

namespace TicketOps.Diagnostics
{
    /// <summary>
    /// The right-hand card of every lab screen: "Activity trace · UI → Service → Data".
    /// It renders <see cref="ActivityLog"/> entries live so the reader can watch one click travel
    /// through the boundaries. Diagnostics code only reads the log — it never makes decisions.
    /// </summary>
    public partial class ActivityTracePanel : UserControl
    {
        private ActivityLog _log;

        public ActivityTracePanel()
        {
            InitializeComponent();
        }

        /// <summary>Renders every entry the log already holds and follows new ones.</summary>
        public void Attach(ActivityLog log)
        {
            if (_log != null)
                _log.EntryAdded -= Log_EntryAdded;

            _log = log;
            this.listTrace.Items.Clear();

            if (_log == null)
                return;

            foreach (var entry in _log.Entries)
                Append(entry);

            _log.EntryAdded += Log_EntryAdded;
        }

        public string Title
        {
            get => this.labelTitle.Text;
            set => this.labelTitle.Text = value;
        }

        public string Footer
        {
            get => this.labelFooter.Text;
            set => this.labelFooter.Text = value;
        }

        public void ClearTrace()
        {
            _log?.Clear();
            this.listTrace.Items.Clear();
        }

        private void Log_EntryAdded(object sender, ActivityEntry entry) => Append(entry);

        private void Append(ActivityEntry entry)
        {
            if (this.IsDisposed)
                return;

            this.listTrace.Items.Add(Format(entry));
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        /// <summary>
        /// One fixed-width line: time, level mark, layer tag, source, message.
        /// Errors keep the exception type in the trace; the message shown to users lives elsewhere.
        /// </summary>
        public static string Format(ActivityEntry entry)
        {
            string time = entry.Time.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            string mark = entry.Level switch
            {
                LogLevel.Warn => "⚠",
                LogLevel.Error => "✖",
                _ => " "
            };
            // The ListBox escapes item text (markup shows as characters) but still collapses runs of spaces: use explicit separators, not padding.
            string layer = entry.Layer switch
            {
                LogLayer.UI => "UI",
                LogLayer.Service => "SVC",
                LogLayer.Domain => "DOMAIN",
                LogLayer.Data => "DATA",
                LogLayer.Infrastructure => "INFRA",
                LogLayer.Session => "SESSION",
                LogLayer.Client => "CLIENT",
                _ => "?"
            };
            string detail = entry.Exception != null
                ? $"{entry.Message}  [{entry.Exception.GetType().Name}]"
                : entry.Message;
            return $"{time} {mark} [{layer}] {entry.Source} — {detail}";
        }
    }
}
