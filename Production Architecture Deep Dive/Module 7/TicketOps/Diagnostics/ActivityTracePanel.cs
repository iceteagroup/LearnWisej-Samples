using System;
using System.Collections.Generic;
using System.Globalization;
using TicketOps.Infrastructure;
using Wisej.Web;

namespace TicketOps.Diagnostics
{
    /// <summary>
    /// The right-hand card of every lab screen: "Activity trace · UI → Service → Data".
    /// It renders <see cref="ActivityLog"/> entries live so the reader can watch one click travel
    /// through the boundaries. Diagnostics code only reads the log — it never makes decisions.
    ///
    /// Module 7 addition — deferred mode. While a background task runs, the log is written from a worker
    /// thread; a control must not be touched from there. The page calls <see cref="BeginDeferred"/> when it
    /// starts the task: from then on entries are queued under a lock, and the page drains them with
    /// <see cref="FlushPending"/> inside each <c>Application.Update(this, …)</c> callback (session context)
    /// and at the end of its own request handlers. <see cref="EndDeferred"/> drains once more and returns to
    /// the direct mode. The list is capped at <see cref="MaxLines"/> lines (a 600-row import writes ~650).
    /// </summary>
    public partial class ActivityTracePanel : UserControl
    {
        public const int MaxLines = 800;

        private ActivityLog _log;
        private readonly object _gate = new object();
        private readonly List<ActivityEntry> _pending = new List<ActivityEntry>();
        private volatile bool _deferred;

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
                AddLine(entry);

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

        /// <summary>True while entries are queued instead of rendered (a background task is running).</summary>
        public bool IsDeferred => _deferred;

        /// <summary>Queue new entries instead of touching the list: call it on the request thread before starting a task.</summary>
        public void BeginDeferred()
        {
            _deferred = true;
        }

        /// <summary>Back to direct rendering; drains what is queued. Call it in session context (a push callback or a request).</summary>
        public void EndDeferred()
        {
            _deferred = false;
            FlushPending();
        }

        /// <summary>
        /// Renders the queued entries. Must run in the session context — inside <c>Application.Update(this, …)</c>
        /// from a worker, or on a request thread. The lock is held only to copy the queue, never while the list changes.
        /// </summary>
        public void FlushPending()
        {
            ActivityEntry[] batch;
            lock (_gate)
            {
                if (_pending.Count == 0)
                    return;
                batch = _pending.ToArray();
                _pending.Clear();
            }

            foreach (var entry in batch)
                AddLine(entry);
        }

        public void ClearTrace()
        {
            _log?.Clear();
            lock (_gate) _pending.Clear();
            this.listTrace.Items.Clear();
        }

        private void Log_EntryAdded(object sender, ActivityEntry entry) => Append(entry);

        private void Append(ActivityEntry entry)
        {
            if (_deferred)
            {
                // Worker thread (or a request racing with one): never touch the ListBox here.
                lock (_gate) _pending.Add(entry);
                return;
            }

            AddLine(entry);
        }

        private void AddLine(ActivityEntry entry)
        {
            if (this.IsDisposed)
                return;

            this.listTrace.Items.Add(Format(entry));
            while (this.listTrace.Items.Count > MaxLines)
                this.listTrace.Items.RemoveAt(0);
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
