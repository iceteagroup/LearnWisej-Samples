using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Wisej.Web;

namespace EnterpriseOps.Controls
{
    /// <summary>
    /// <b>StatusTimeline</b> — the reusable "what happened to this record, in order" component of the
    /// EnterpriseOps application framework.
    ///
    /// <para>
    /// <b>What it is for:</b> any screen that shows the ordered history of one record — the work-order
    /// history page, the audit log, the escalation wizard's review step, the field technician's job card.
    /// Drop it from the Toolbox, set <see cref="Caption"/>, hand it items, handle <see cref="ItemSelected"/>.
    /// </para>
    ///
    /// <para>
    /// <b>Why a UserControl and not an inherited control:</b> a timeline is a <i>new thing made of several
    /// parts</i> (a header, a scrolling list of rows, a per-row dot / time / status / message layout), not a
    /// specialised version of one existing control. The lesson's rule: compose when the component is new,
    /// inherit when it is a specialised version of one control. Mixing the two produces components that are
    /// hard to theme and hard to explain.
    /// </para>
    ///
    /// <para>
    /// <b>The public surface</b> — everything a screen developer needs and nothing else:
    /// <see cref="Items"/>, <see cref="SelectedItem"/>, <see cref="SelectedIndex"/>, <see cref="Caption"/>,
    /// <see cref="TimeFormat"/>, <see cref="EmptyText"/>, <see cref="SampleMode"/>,
    /// <see cref="SetItems(TimelineItem[])"/>, <see cref="Clear"/> and the events
    /// <see cref="ItemSelected"/> and <see cref="SelectionCleared"/>. The internal layout
    /// (<c>pnlFrame</c>, <c>pnlItems</c>, the row panels) is private and can change without touching a
    /// single screen — that is the first review question of the lab.
    /// </para>
    ///
    /// <para>
    /// <b>State ownership:</b> the component owns <i>presentation</i> state only — which entries it was
    /// given, which one is highlighted, whether it is showing sample data. It never fetches, never filters,
    /// never decides who may see an entry, and it has no reference to a service, a repository or the domain.
    /// A screen maps its own projection into <see cref="TimelineItem"/> first, so the component physically
    /// cannot leak the actor, the tenant or the version number of a work order.
    /// </para>
    ///
    /// <para>
    /// <b>Design time:</b> the Designer creates the control with no services and no data. When
    /// <see cref="SampleMode"/> is on — automatically at design time — the control renders a plausible
    /// four-entry timeline and an amber "DESIGN-TIME SAMPLE" badge, so a screen developer can lay out a form
    /// without running the application. Sample mode never calls a service and never throws.
    /// </para>
    /// </summary>
    [ToolboxItem(true)]
    [DefaultProperty("Caption")]
    [DefaultEvent("ItemSelected")]
    [Description("Reusable ordered-history component: dot, time, status and message per entry, with a selection event and a design-time sample mode.")]
    public partial class StatusTimeline : UserControl
    {
        #region Defaults (so the Designer knows what not to serialize)

        public const string DefaultCaption = "STATUS TIMELINE";
        public const string DefaultTimeFormat = "MMM dd, HH:mm";
        public const string DefaultEmptyText = "No history to show.";

        #endregion

        // ---- component state (presentation only) --------------------------------
        private readonly TimelineItemCollection _items = new TimelineItemCollection();
        private readonly Dictionary<TimelineItem, Panel> _rows = new Dictionary<TimelineItem, Panel>();
        private TimelineItem _selected;
        private string _timeFormat = DefaultTimeFormat;
        private bool _sampleMode;
        private bool _sampleLoaded;

        public StatusTimeline()
        {
            InitializeComponent();

            _items.Changed += (s, e) => RenderItems();
        }

        #region Typed properties — the public API

        /// <summary>
        /// The entries, oldest first. A live collection: adding, removing or replacing an item re-renders
        /// the control, so a screen never repaints anything by hand. Kept out of the Properties window
        /// because a timeline is filled from a service result, not typed into the Designer.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The timeline entries, oldest first. Mutating the collection re-renders the control.")]
        public IList<TimelineItem> Items => _items;

        /// <summary>
        /// The highlighted entry, or null. Setting it moves the highlight without raising
        /// <see cref="ItemSelected"/> — that event means "the user clicked", which is what a handler
        /// usually wants to distinguish from "the screen restored a selection".
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The highlighted entry, or null. Setting it does not raise ItemSelected.")]
        public TimelineItem SelectedItem
        {
            get => _selected;
            set
            {
                if (value != null && !_items.Contains(value))
                    throw new ArgumentException("The item is not in this timeline's Items collection.", nameof(SelectedItem));

                if (ReferenceEquals(_selected, value))
                    return;

                _selected = value;
                PaintSelection();
            }
        }

        /// <summary>The index of <see cref="SelectedItem"/> in <see cref="Items"/>, or -1.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex => _selected == null ? -1 : _items.IndexOf(_selected);

        /// <summary>The small uppercase caption above the entries.</summary>
        [Category("Timeline")]
        [DefaultValue(DefaultCaption)]
        [Description("The small uppercase caption above the entries.")]
        public string Caption
        {
            get => lblCaption.Text;
            set => lblCaption.Text = (value ?? "").ToUpperInvariant();
        }

        /// <summary>
        /// How each entry's timestamp is formatted (a standard .NET date format string).
        /// Changing it re-renders the rows.
        /// </summary>
        [Category("Timeline")]
        [DefaultValue(DefaultTimeFormat)]
        [Description("How each entry's timestamp is formatted (a .NET date format string).")]
        public string TimeFormat
        {
            get => _timeFormat;
            set
            {
                string format = string.IsNullOrWhiteSpace(value) ? DefaultTimeFormat : value;

                // Fail here, on the property, with a message that names the property — not later,
                // deep inside a render loop, with a FormatException nobody can place.
                try
                {
                    DateTime.Now.ToString(format);
                }
                catch (FormatException ex)
                {
                    throw new ArgumentException($"'{value}' is not a valid date format string.", nameof(TimeFormat), ex);
                }

                if (_timeFormat == format)
                    return;

                _timeFormat = format;
                RenderItems();
            }
        }

        /// <summary>What the control says when it has no entries.</summary>
        [Category("Timeline")]
        [DefaultValue(DefaultEmptyText)]
        [Description("What the control shows when it has no entries.")]
        public string EmptyText
        {
            get => lblEmpty.Text;
            set => lblEmpty.Text = value ?? "";
        }

        /// <summary>
        /// Design-time sample mode. Turned on automatically in the Designer; a screen can also turn it on at
        /// run time to show what the Designer shows. While it is on the control renders a fixed four-entry
        /// sample and an amber badge, and it never calls anything.
        /// </summary>
        [Category("Timeline")]
        [DefaultValue(false)]
        [Description("Render a fixed sample timeline (used automatically at design time). Never calls a service.")]
        public bool SampleMode
        {
            get => _sampleMode;
            set
            {
                if (_sampleMode == value)
                    return;

                _sampleMode = value;
                lblSampleBadge.Visible = value;

                if (value)
                    LoadSample();
                else if (_sampleLoaded)
                {
                    _sampleLoaded = false;
                    _items.Reset(null);
                }
            }
        }

        #endregion

        #region Events

        /// <summary>The user clicked an entry. The screen decides what that means.</summary>
        [Category("Timeline")]
        [Description("Raised when the user clicks an entry.")]
        public event EventHandler<TimelineItemEventArgs> ItemSelected;

        /// <summary>The highlight was dropped because the entries were replaced.</summary>
        [Category("Timeline")]
        [Description("Raised when the highlight is dropped because the entries were replaced.")]
        public event EventHandler SelectionCleared;

        protected virtual void OnItemSelected(TimelineItemEventArgs e) => ItemSelected?.Invoke(this, e);

        protected virtual void OnSelectionCleared(EventArgs e) => SelectionCleared?.Invoke(this, e);

        #endregion

        #region Methods

        /// <summary>
        /// Replaces every entry in one call and drops the highlight — the API the walkthrough uses:
        /// <c>statusTimeline.SetItems(items)</c>. One render, one round trip.
        /// </summary>
        public void SetItems(params TimelineItem[] items)
        {
            SetItems((IEnumerable<TimelineItem>)items);
        }

        /// <summary>Replaces every entry in one call and drops the highlight.</summary>
        public void SetItems(IEnumerable<TimelineItem> items)
        {
            _sampleMode = false;
            _sampleLoaded = false;
            lblSampleBadge.Visible = false;

            ClearSelection();
            _items.Reset(items);
        }

        /// <summary>Removes every entry.</summary>
        public void Clear()
        {
            ClearSelection();
            _items.Reset(null);
        }

        /// <summary>Drops the highlight (without raising <see cref="ItemSelected"/>).</summary>
        public void ClearSelection()
        {
            if (_selected == null)
                return;

            _selected = null;
            PaintSelection();
            OnSelectionCleared(EventArgs.Empty);
        }

        #endregion

        #region Design time

        /// <summary>
        /// The Designer builds the control with no services and no data, so this is where sample mode is
        /// switched on. It is the third review question of the lab: drop the control on an empty form and
        /// it must render something plausible and never throw.
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (this.DesignMode && _items.Count == 0)
                this.SampleMode = true;
        }

        /// <summary>The fixed sample: a work order that was created, assigned, put on hold and escalated.</summary>
        private void LoadSample()
        {
            var day = new DateTime(DateTime.Today.Year, 6, 10, 9, 12, 0);

            _items.Reset(new[]
            {
                new TimelineItem(day, "Created", "Imported from fabrikam_q2.csv", TimelineSeverity.Neutral),
                new TimelineItem(day.AddDays(1).AddHours(4).AddMinutes(53), "Assigned", "t.nguyen — dock crew", TimelineSeverity.Info),
                new TimelineItem(day.AddDays(1).AddHours(23).AddMinutes(18), "On hold", "vendor part backordered", TimelineSeverity.Warning),
                new TimelineItem(day.AddDays(2).AddHours(1).AddMinutes(29), "Escalated", "approver m.weber · due Jun 14", TimelineSeverity.Critical),
            });

            _sampleLoaded = true;
        }

        #endregion

        #region Rendering (private — no screen depends on any of this)

        private static readonly Color RowBack = Color.White;
        private static readonly Color RowBackSelected = Color.FromArgb(234, 243, 255);
        private static readonly Color TimeColor = Color.FromArgb(122, 138, 153);
        private static readonly Color MessageColor = Color.FromArgb(70, 88, 106);

        /// <summary>The severity → colour map. One place: change it and every screen changes with it.</summary>
        private static Color ColorFor(TimelineSeverity severity)
        {
            switch (severity)
            {
                case TimelineSeverity.Info: return Color.FromArgb(26, 134, 255);
                case TimelineSeverity.Warning: return Color.FromArgb(185, 119, 14);
                case TimelineSeverity.Critical: return Color.FromArgb(192, 57, 43);
                case TimelineSeverity.Success: return Color.FromArgb(31, 138, 76);
                default: return Color.FromArgb(138, 151, 164);
            }
        }

        private void RenderItems()
        {
            pnlItems.SuspendLayout();
            try
            {
                // Copy first: disposing a control removes it from Controls, and mutating the collection
                // while enumerating it throws.
                var existing = new List<Control>();
                foreach (Control control in pnlItems.Controls)
                    existing.Add(control);

                pnlItems.Controls.Clear();
                foreach (Control control in existing)
                    control.Dispose();

                _rows.Clear();

                for (int i = 0; i < _items.Count; i++)
                    pnlItems.Controls.Add(BuildRow(_items[i], i));
            }
            finally
            {
                pnlItems.ResumeLayout(true);
            }

            lblEmpty.Visible = _items.Count == 0;
            pnlItems.Visible = _items.Count > 0;

            if (_selected != null && !_items.Contains(_selected))
                _selected = null;

            PaintSelection();
        }

        private Panel BuildRow(TimelineItem item, int index)
        {
            var row = new Panel
            {
                Name = "pnlRow" + index,
                Size = new Size(372, 26),
                Margin = new Padding(0, 0, 0, 2),
                BackColor = RowBack,
                Cursor = Cursors.Hand,
                Tag = item,
                ToolTipText = item.Message,
            };

            Color accent = ColorFor(item.Severity);

            var dot = new Label
            {
                Name = "lblDot" + index,
                AutoSize = false,
                Text = "●",
                Font = new Font("default", 11F),
                ForeColor = accent,
                Location = new Point(0, 4),
                Size = new Size(14, 18),
                TextAlign = ContentAlignment.MiddleCenter,
            };

            var when = new Label
            {
                Name = "lblWhen" + index,
                AutoSize = false,
                Text = SafeFormat(item.At),
                Font = new Font("monospace", 8.25F),
                ForeColor = TimeColor,
                Location = new Point(16, 4),
                Size = new Size(104, 18),
                TextAlign = ContentAlignment.MiddleLeft,
            };

            var status = new Label
            {
                Name = "lblStatus" + index,
                AutoSize = false,
                Text = item.Status ?? "",
                Font = new Font("default", 8.25F, FontStyle.Bold),
                ForeColor = accent,
                Location = new Point(124, 4),
                Size = new Size(78, 18),
                TextAlign = ContentAlignment.MiddleLeft,
            };

            var message = new Label
            {
                Name = "lblMessage" + index,
                AutoSize = false,
                Text = item.Message ?? "",
                Font = new Font("default", 8.25F),
                ForeColor = MessageColor,
                Location = new Point(206, 4),
                Size = new Size(164, 18),
                TextAlign = ContentAlignment.MiddleLeft,
            };

            row.Controls.Add(dot);
            row.Controls.Add(when);
            row.Controls.Add(status);
            row.Controls.Add(message);

            // A click anywhere on the row selects it — the labels sit on top of the panel, so each of
            // them forwards to the same private handler.
            row.Click += row_Click;
            dot.Click += row_Click;
            when.Click += row_Click;
            status.Click += row_Click;
            message.Click += row_Click;

            _rows[item] = row;
            return row;
        }

        /// <summary>
        /// The only handler in the component: find the item, move the highlight, raise the named event.
        /// Everything else is the screen's business.
        /// </summary>
        private void row_Click(object sender, EventArgs e)
        {
            var control = sender as Control;
            var row = control as Panel ?? control?.Parent as Panel;
            var item = row?.Tag as TimelineItem;
            if (item == null)
                return;

            _selected = item;
            PaintSelection();
            OnItemSelected(new TimelineItemEventArgs(item, _items.IndexOf(item)));
        }

        private void PaintSelection()
        {
            foreach (var pair in _rows)
                pair.Value.BackColor = ReferenceEquals(pair.Key, _selected) ? RowBackSelected : RowBack;
        }

        /// <summary>A bad format string must never take the screen down; the property setter already validated it.</summary>
        private string SafeFormat(DateTime at)
        {
            try { return at.ToString(_timeFormat); }
            catch (FormatException) { return at.ToString(DefaultTimeFormat); }
        }

        #endregion
    }
}
