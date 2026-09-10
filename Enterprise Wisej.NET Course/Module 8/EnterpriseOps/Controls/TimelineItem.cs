using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EnterpriseOps.Controls
{
    /// <summary>
    /// How prominent one timeline entry is. The <b>component</b> owns the colours, not the screens:
    /// a screen says "this entry is critical", never "this entry is #c0392b". That is what keeps five
    /// screens looking the same and lets a theme change happen in one file.
    /// </summary>
    public enum TimelineSeverity
    {
        /// <summary>Routine, grey (Created, Cancelled).</summary>
        Neutral = 0,
        /// <summary>Something moved forward, blue (Assigned, In progress).</summary>
        Info = 1,
        /// <summary>Something is waiting, amber (On hold).</summary>
        Warning = 2,
        /// <summary>Something needs a human now, red (Escalated).</summary>
        Critical = 3,
        /// <summary>Finished, green (Completed).</summary>
        Success = 4,
    }

    /// <summary>
    /// The whole contract of <see cref="StatusTimeline"/>: <b>when</b>, <b>what status</b>,
    /// <b>what happened</b>, and how prominent it is. Three lines of data — deliberately.
    ///
    /// <para>
    /// It is not a domain entity and it never will be. A screen maps its own projection
    /// (<c>HistoryEntryView</c>) into this type, which is why the timeline can be reused by the audit
    /// screen, the escalation wizard and the field-technician page without any of them sharing a
    /// domain model — and why the component cannot leak an actor, a tenant id or a version number
    /// it was never given.
    /// </para>
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class TimelineItem
    {
        /// <summary>Creates an empty item (the designer and serializers need a parameterless constructor).</summary>
        public TimelineItem()
        {
        }

        /// <summary>Creates an item. <paramref name="at"/> is displayed as given — convert to local time first if that is what the screen wants.</summary>
        public TimelineItem(DateTime at, string status, string message, TimelineSeverity severity = TimelineSeverity.Neutral)
        {
            this.At = at;
            this.Status = status;
            this.Message = message;
            this.Severity = severity;
        }

        /// <summary>When the transition happened.</summary>
        [Category("Timeline"), Description("When the transition happened.")]
        public DateTime At { get; set; }

        /// <summary>The short status name shown in the accent colour ("Created", "On hold", "Escalated").</summary>
        [Category("Timeline"), Description("The short status name shown in the accent colour.")]
        public string Status { get; set; }

        /// <summary>One line of human detail ("vendor part backordered").</summary>
        [Category("Timeline"), Description("One line of human detail about the transition.")]
        public string Message { get; set; }

        /// <summary>How prominent the entry is. The control maps this onto the colour; the screen never picks a colour.</summary>
        [Category("Timeline"), DefaultValue(TimelineSeverity.Neutral)]
        [Description("How prominent the entry is. The control maps this onto a colour.")]
        public TimelineSeverity Severity { get; set; } = TimelineSeverity.Neutral;

        /// <summary>Anything the screen wants to find its own object again in the ItemSelected handler. Never rendered.</summary>
        [Browsable(false)]
        public object Tag { get; set; }

        public override string ToString() => $"{this.At:g} — {this.Status}: {this.Message}";
    }

    /// <summary>
    /// The live <c>Items</c> collection of a <see cref="StatusTimeline"/>. Every mutation raises
    /// <see cref="Changed"/> so the control re-renders itself; a screen can therefore write
    /// <c>timeline.Items.Add(...)</c> and never think about repainting.
    /// </summary>
    public sealed class TimelineItemCollection : Collection<TimelineItem>
    {
        /// <summary>Raised after any change to the collection.</summary>
        public event EventHandler Changed;

        private bool _suspended;

        /// <summary>Adds many items and raises <see cref="Changed"/> once.</summary>
        public void AddRange(System.Collections.Generic.IEnumerable<TimelineItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _suspended = true;
            try
            {
                foreach (var item in items)
                    Add(item);
            }
            finally
            {
                _suspended = false;
            }
            OnChanged();
        }

        /// <summary>Replaces the whole content and raises <see cref="Changed"/> once.</summary>
        public void Reset(System.Collections.Generic.IEnumerable<TimelineItem> items)
        {
            _suspended = true;
            try
            {
                base.ClearItems();
                if (items != null)
                {
                    foreach (var item in items)
                        Add(item);
                }
            }
            finally
            {
                _suspended = false;
            }
            OnChanged();
        }

        protected override void InsertItem(int index, TimelineItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "A timeline item cannot be null.");
            base.InsertItem(index, item);
            OnChanged();
        }

        protected override void SetItem(int index, TimelineItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "A timeline item cannot be null.");
            base.SetItem(index, item);
            OnChanged();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            OnChanged();
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            OnChanged();
        }

        private void OnChanged()
        {
            if (_suspended)
                return;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Carries the selected entry to the screen's <c>ItemSelected</c> handler.</summary>
    public class TimelineItemEventArgs : EventArgs
    {
        public TimelineItemEventArgs(TimelineItem item, int index)
        {
            this.Item = item;
            this.Index = index;
        }

        /// <summary>The entry the user clicked. Never null.</summary>
        public TimelineItem Item { get; }

        /// <summary>Its position in <c>Items</c>, oldest first.</summary>
        public int Index { get; }
    }
}
