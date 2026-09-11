using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Wisej.Web;

namespace EnterpriseOps.Widgets
{
    /// <summary>
    /// <b>WorkOrderChartWidget</b> — the stable C# API in front of the third-party status chart.
    ///
    /// <para>
    /// <b>Which vendor library it wraps:</b> <c>EnterpriseOpsChart</c> 1.2
    /// (<c>Widgets/vendor-opschart.js</c>, global <c>EnterpriseOpsChart</c>) — an SVG chart that knows
    /// nothing about Wisej.NET, .NET or work orders. The adapter that bridges the two is the embedded
    /// resource <c>Widgets/opschart-init.js</c>, versioned with this class. When the vendor releases a new
    /// version, this class, the adapter and <see cref="ComponentResourcePackage"/> change; the screens that
    /// use the chart do not.
    /// </para>
    ///
    /// <para>
    /// <b>The public surface</b>: <see cref="SetSegments(ChartSegment[])"/>, <see cref="Caption"/>,
    /// <see cref="Palette"/>, <see cref="ShowLegend"/>, <see cref="SampleMode"/>, <see cref="SelectedKey"/>,
    /// <see cref="Select"/>, and the events <see cref="SegmentClicked"/> and <see cref="WidgetError"/>.
    /// Every method and property is named after what the <i>application</i> wants, never after a vendor
    /// function — that is what survives a vendor upgrade or a vendor replacement.
    /// </para>
    ///
    /// <para>
    /// <b>Hidden</b>: <c>Packages</c>, <c>InitScript</c>, <c>WiredEvents</c>, the raw <c>Options</c> object
    /// and <c>Call</c>/<c>CallAsync</c>. Once this class owns the setup, letting a screen change them on one
    /// instance is a bug waiting to happen.
    /// </para>
    ///
    /// <para>
    /// <b>State ownership:</b> the server owns the data and the selection. The browser owns nothing but the
    /// pixels. A click in the browser arrives as a <i>key</i> and the server re-validates it before it
    /// queries anything — <see cref="SegmentClicked"/> hands the screen untrusted input on purpose.
    /// </para>
    ///
    /// <para>
    /// <b>Failure:</b> the wrapper owns it. When the vendor package does not load, or the vendor throws, the
    /// adapter renders the component's embedded fallback list and reports the reason through the contract's
    /// <c>error</c> event, which arrives here as <see cref="WidgetError"/>. Nothing is thrown at the screen
    /// and the rest of the page keeps working.
    /// </para>
    /// </summary>
    [DefaultProperty("Caption")]
    [DefaultEvent("SegmentClicked")]
    [Description("Reusable chart component wrapping the EnterpriseOpsChart JavaScript library. Set typed properties and handle SegmentClicked; packages, scripts, options and failure handling belong to the class.")]
    public class WorkOrderChartWidget : Widget
    {
        #region Defaults and the closed set of palettes

        public const string DefaultCaption = "WORK-ORDER HISTORY";
        public const string DefaultPalette = "ops";
        public const bool DefaultShowLegend = true;

        /// <summary>
        /// The palettes the vendor ships. The set is closed <b>on the server</b>: an unknown palette is
        /// refused here, before anything is rendered, so a typo can never reach the browser and be
        /// swallowed by a JavaScript exception nobody sees.
        /// </summary>
        public static readonly IReadOnlyList<string> KnownPalettes = new[] { "ops", "mono", "highcontrast" };

        #endregion

        // ---- server-owned state --------------------------------------------------
        private readonly List<ChartSegment> _segments = new List<ChartSegment>();
        private string _caption = DefaultCaption;
        private string _palette = DefaultPalette;
        private bool _showLegend = DefaultShowLegend;
        private bool _sampleMode;
        private string _selectedKey;
        private bool _fallbackShown;

        public WorkOrderChartWidget()
        {
            // 1. Resources — from the one ordered list, never typed on a page.
            //    The vendor script first, the component's own stylesheet after it so its rules win.
            foreach (ComponentResource resource in ComponentResourcePackage.Packages)
            {
                base.Packages.Add(new Package
                {
                    Name = resource.Name,
                    Source = resource.Source,
                });
            }

            // 2. The client adapter ships inside this assembly (see EnterpriseOps.csproj).
            base.InitScript = GetResourceString(ComponentResourcePackage.Adapter.EmbeddedName);

            // 3. The events the adapter is allowed to raise — the client/server contract, fixed here.
            base.WiredEvents = new[] { "pointSelected", "error" };

            this.Size = new System.Drawing.Size(420, 210);

            // 4. Default Options, so a fresh instance renders sensibly before any property is set.
            dynamic options = base.Options;
            options.segments = ToWire(SampleSegments());
            options.palette = _palette;
            options.caption = _caption;
            options.showLegend = _showLegend;
            options.badge = "";
            options.selectedKey = "";

            _segments.AddRange(SampleSegments());
        }

        #region Typed properties — the public API

        /// <summary>The small uppercase caption drawn above the bar.</summary>
        [Category("Chart")]
        [DefaultValue(DefaultCaption)]
        [Description("The small uppercase caption drawn above the bar.")]
        public string Caption
        {
            get => _caption;
            set
            {
                string caption = (value ?? "").ToUpperInvariant();
                if (_caption == caption)
                    return;

                _caption = caption;
                dynamic options = base.Options;
                options.caption = caption;
            }
        }

        /// <summary>
        /// Which colour set the vendor draws with. One of <see cref="KnownPalettes"/>; anything else is
        /// refused with an <see cref="ArgumentOutOfRangeException"/> and nothing is rendered.
        /// </summary>
        [Category("Chart")]
        [DefaultValue(DefaultPalette)]
        [Description("Which colour set the vendor draws with: ops, mono or highcontrast.")]
        public string Palette
        {
            get => _palette;
            set
            {
                // An option the component does not support is a programming error: it fails here, on the
                // server, with the list of what is supported — the bad value never reaches the browser.
                if (value == null || !KnownPalettes.Contains(value, StringComparer.Ordinal))
                {
                    throw new ArgumentOutOfRangeException(nameof(Palette), value,
                        $"Unknown palette '{value ?? "(null)"}'. Known palettes: {string.Join(", ", KnownPalettes)}.");
                }

                if (_palette == value)
                    return;

                _palette = value;
                dynamic options = base.Options;
                options.palette = value;
            }
        }

        /// <summary>Whether the legend under the bar is drawn.</summary>
        [Category("Chart")]
        [DefaultValue(DefaultShowLegend)]
        [Description("Whether the legend under the bar is drawn.")]
        public bool ShowLegend
        {
            get => _showLegend;
            set
            {
                if (_showLegend == value)
                    return;

                _showLegend = value;
                dynamic options = base.Options;
                options.showLegend = value;
            }
        }

        /// <summary>
        /// Design-time sample mode. Turned on automatically in the Designer, and available at run time so a
        /// screen can show exactly what the Designer shows. It fills a fixed, plausible breakdown and an
        /// amber badge. It never calls a service.
        /// </summary>
        [Category("Chart")]
        [DefaultValue(false)]
        [Description("Render a fixed sample breakdown (used automatically at design time). Never calls a service.")]
        public bool SampleMode
        {
            get => _sampleMode;
            set
            {
                if (_sampleMode == value)
                    return;

                _sampleMode = value;

                dynamic options = base.Options;
                options.badge = value ? "DESIGN-TIME SAMPLE" : "";

                if (value)
                    ApplySegments(SampleSegments());
            }
        }

        /// <summary>The key of the highlighted slice, or an empty string. Read-only: use <see cref="Select"/>.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedKey => _selectedKey ?? "";

        /// <summary>True while the client is showing the embedded fallback instead of the vendor chart.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFallbackRendered => _fallbackShown;

        /// <summary>The vendor version this wrapper was written against.</summary>
        [Browsable(false)]
        public string VendorVersion => ComponentResourcePackage.VendorVersion;

        #endregion

        #region Methods — named after what the application wants

        /// <summary>
        /// Replaces the whole breakdown: <c>chartWorkOrders.SetSegments(segments)</c>. The wrapper validates
        /// first, converts to the vendor's shape itself, and renders once.
        /// </summary>
        public void SetSegments(params ChartSegment[] segments)
            => SetSegments((IEnumerable<ChartSegment>)segments);

        /// <summary>Replaces the whole breakdown.</summary>
        public void SetSegments(IEnumerable<ChartSegment> segments)
        {
            var list = (segments ?? Enumerable.Empty<ChartSegment>()).ToList();

            // Validation belongs on the server, in application terms, before anything crosses.
            if (list.Count == 0)
                throw new ArgumentException("A chart needs at least one segment.", nameof(segments));

            foreach (ChartSegment segment in list)
            {
                if (segment == null || string.IsNullOrWhiteSpace(segment.Key))
                    throw new ArgumentException("Every segment needs a non-empty Key.", nameof(segments));
                if (segment.Value < 0)
                    throw new ArgumentOutOfRangeException(nameof(segments), segment.Value,
                        $"Segment '{segment.Key}' has a negative value.");
            }

            if (list.Select(s => s.Key).Distinct(StringComparer.Ordinal).Count() != list.Count)
                throw new ArgumentException("Two segments share the same Key.", nameof(segments));

            _sampleMode = false;
            dynamic options = base.Options;
            options.badge = "";

            ApplySegments(list);
        }

        /// <summary>
        /// Highlights one slice from the server (after a reload, or to mirror a selection made elsewhere).
        /// Unlike a user click it raises no <see cref="SegmentClicked"/> — the screen already knows.
        /// </summary>
        public void Select(string key)
        {
            if (key != null && key.Length > 0 && !_segments.Any(s => s.Key == key))
                throw new ArgumentException($"No segment with key '{key}'.", nameof(key));

            _selectedKey = key ?? "";
            dynamic options = base.Options;
            options.selectedKey = _selectedKey;
        }

        #endregion

        #region Events

        /// <summary>
        /// The user clicked a slice. The payload carries the slice <b>key</b> and what the browser rendered
        /// — never a work order, never a tenant. Validate the key before you query with it.
        /// </summary>
        [Category("Chart")]
        [Description("Raised when the user clicks a slice. The key comes from the browser: validate it.")]
        public event EventHandler<ChartSegmentEventArgs> SegmentClicked;

        /// <summary>
        /// The client adapter caught a failure and reported it. The chart has already fallen back to the
        /// embedded list; this is the screen's chance to tell the user and to log the reason.
        /// </summary>
        [Category("Chart")]
        [Description("Raised when the client adapter reports a vendor failure. The fallback is already on screen.")]
        public event EventHandler<WidgetErrorEventArgs> WidgetError;

        protected virtual void OnSegmentClicked(ChartSegmentEventArgs e) => SegmentClicked?.Invoke(this, e);

        protected virtual void OnWidgetError(WidgetErrorEventArgs e) => WidgetError?.Invoke(this, e);

        /// <summary>
        /// Every event the adapter raises lands here and is translated into a <b>named server event</b>.
        /// The server decides what it means; it never lets the browser hold the authoritative state.
        /// </summary>
        protected override void OnWidgetEvent(WidgetEventArgs e)
        {
            dynamic data = e.Data;

            switch (e.Type)
            {
                case "pointSelected":
                    {
                        string key = ToText(data?.key);
                        string label = ToText(data?.label);
                        int value = ToInt(data?.value);
                        int percent = ToInt(data?.percent);

                        // The browser may send anything. A key the server never rendered is dropped here,
                        // in the wrapper, so no screen ever has to remember to check.
                        if (!_segments.Any(s => s.Key == key))
                            break;

                        _selectedKey = key;
                        _fallbackShown = false;
                        OnSegmentClicked(new ChartSegmentEventArgs(key, label, value, percent));
                        break;
                    }

                case "error":
                    {
                        string phase = ToText(data?.phase);
                        string message = ToText(data?.message);

                        _fallbackShown = true;
                        OnWidgetError(new WidgetErrorEventArgs(phase, message) { FallbackRendered = true });
                        break;
                    }

                default:
                    base.OnWidgetEvent(e);          // never swallow an event the contract does not know
                    break;
            }
        }

        #endregion

        #region Design time

        /// <summary>
        /// The Designer creates the widget with no services and no data. Sample mode gives a screen
        /// developer a plausible chart to lay out against, and never calls anything.
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (this.DesignMode)
                this.SampleMode = true;
        }

        /// <summary>The fixed design-time breakdown (the same shape the running screen shows).</summary>
        public static IReadOnlyList<ChartSegment> SampleSegments() => new[]
        {
            new ChartSegment("open", "Open", 38),
            new ChartSegment("onhold", "On hold", 22),
            new ChartSegment("escalated", "Escalated", 12),
            new ChartSegment("done", "Done", 28),
        };

        #endregion

        #region Hidden escape hatches (see docs/WidgetWrapper.md)

        // Once the class owns the setup, letting a screen change these on one instance breaks that instance
        // only — the worst kind of bug. They are shadowed read-only and hidden from the Properties window,
        // IntelliSense and designer serialization. The class itself always goes through "base.".

        /// <summary>Hidden. Packages come from <see cref="ComponentResourcePackage"/>.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new List<Package> Packages => base.Packages;

        /// <summary>Hidden. The client adapter is an embedded resource versioned with this class.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string InitScript => base.InitScript;

        /// <summary>Hidden. Use the typed properties; the wrapper writes the options.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new object Options => base.Options;

        /// <summary>Hidden. The event contract is fixed by this class and its adapter.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string[] WiredEvents => base.WiredEvents;

        /// <summary>Hidden from IntelliSense. Low-level client calls belong inside the wrapper.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Control Call(string function, params object[] args) => base.Call(function, args);

        /// <summary>Hidden from IntelliSense. Low-level client calls belong inside the wrapper.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Task<dynamic> CallAsync(string function, params object[] args) => base.CallAsync(function, args);

        #endregion

        #region Internals

        private void ApplySegments(IEnumerable<ChartSegment> segments)
        {
            _segments.Clear();
            _segments.AddRange(segments);

            if (_selectedKey != null && !_segments.Any(s => s.Key == _selectedKey))
                _selectedKey = "";

            dynamic options = base.Options;
            options.segments = ToWire(_segments);       // replacing the array is a first-level change → update()
            options.selectedKey = SelectedKey;
        }

        /// <summary>
        /// The only place the domain becomes wire data. Wisej.NET camel-cases the first-level Options
        /// fields, but objects nested inside an Options array keep their C# names — so the wire names are
        /// spelled out in the vendor's shape here, once.
        /// </summary>
        private static object[] ToWire(IEnumerable<ChartSegment> segments)
            => segments.Select(s => (object)new { key = s.Key, label = s.Label, value = s.Value }).ToArray();

        private static string ToText(object value)
            => value == null ? "" : Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);

        private static int ToInt(object value)
        {
            try { return value == null ? 0 : Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture); }
            catch { return 0; }
        }

        #endregion
    }
}
