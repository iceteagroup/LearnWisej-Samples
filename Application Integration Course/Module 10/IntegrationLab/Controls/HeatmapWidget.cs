using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IntegrationLab.Data;
using Wisej.Core;
using Wisej.Web;

namespace IntegrationLab.Controls
{
    /// <summary>
    /// Reusable Wisej.NET wrapper around the "unfamiliar" VendorHeatmap library (Module 10 capstone).
    ///
    /// Server side (this class): typed properties, validation, the postback data endpoint, server calls,
    /// .NET events and the versions that are pinned for this integration.
    /// Client side (wwwroot/heatmap-init.js, embedded): the vendor instance, DOM, resize, disposal.
    ///
    /// Everything that crosses the wire is data: compact JSON out (Options / Call arguments / the postback
    /// body) and small event payloads back. The browser never holds the authoritative dataset; the server
    /// keeps <see cref="Cells"/> and uses it for decisions (e.g. <see cref="FindPeak"/>).
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("CellSelected")]
    [DefaultProperty("Title")]
    [Description("Hosts the VendorHeatmap calendar heatmap inside a Wisej.NET widget container, loading its data through a postback endpoint.")]
    public class HeatmapWidget : Widget
    {
        // ---- pinned versions and resource paths (production checklist: resources + versioning) --------------
        /// <summary>Vendor library version this wrapper was written against; the adapter refuses any other.</summary>
        public const string VendorVersion = "1.2.0";
        /// <summary>Version of this wrapper (bump when the contract changes; see docs/CapstoneImplementation.md).</summary>
        public const string WrapperVersion = "1.0.0";
        /// <summary>Application paths (served by the static file server); no CDN is involved.</summary>
        public const string VendorStylePath = "wwwroot/vendor-heatmap.css";
        public const string VendorScriptPath = "wwwroot/vendor-heatmap.js";
        private const string AdapterResourceName = "IntegrationLab.wwwroot.heatmap-init.js";

        /// <summary>The only postback actions the endpoint answers (security checklist: validate every endpoint).</summary>
        private static readonly HashSet<string> AllowedActions = new HashSet<string>(StringComparer.Ordinal) { "load" };
        private const int MaxDays = 14;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static readonly string[] DefaultPalette = { "#dce9f8", "#1a86ff", "#e8a13c", "#e0563b" };

        // ---- server-owned state ---------------------------------------------------------------------------
        private int _days = 7;
        private int _hours = 24;
        private double _warnAt = 60;
        private double _highAt = 85;
        private string _title = "";
        private IReadOnlyList<HeatmapCell> _cells = Array.Empty<HeatmapCell>();
        private readonly object _requestLock = new object();
        private readonly Queue<DateTime> _requestTimes = new Queue<DateTime>();
        private bool _fallbackUrlPushed;
        private bool _designSamplePushed;
        private readonly bool _simulateMissingVendor;

        public HeatmapWidget() : this(false)
        {
        }

        private HeatmapWidget(bool simulateMissingVendor)
        {
            _simulateMissingVendor = simulateMissingVendor;

            // Packages load once per page, in list order: stylesheet first, then the library the adapter needs.
            base.Packages.Add(new Package { Name = "vendor-heatmap-css", Source = VendorStylePath });
            if (!simulateMissingVendor)
                base.Packages.Add(new Package { Name = "vendor-heatmap", Source = VendorScriptPath });

            // The client adapter is embedded in this assembly (see IntegrationLab.csproj) and handed to the wrapper.
            string adapter = GetResourceString(AdapterResourceName);
            base.InitScript = simulateMissingVendor ? MissingVendorPreamble + adapter : adapter;

            // The events this wrapper is allowed to raise: the documented contract. "cellhover" is deliberately absent.
            base.WiredEvents = new[] { "cellSelected", "loaded", "error" };

            this.Size = new System.Drawing.Size(660, 250);
            PushState();
        }

        #region Typed properties (server state → Options)

        /// <summary>Number of day rows (1..14). Changing it recreates the vendor grid on the client.</summary>
        [DefaultValue(7)]
        [Description("Number of day rows (1..14).")]
        public int Days
        {
            get => _days;
            set
            {
                if (value < 1 || value > MaxDays)
                    throw new ArgumentOutOfRangeException(nameof(Days), value, $"Days must be between 1 and {MaxDays}.");
                if (_days == value) return;
                _days = value;
                dynamic options = base.Options;
                options.days = value;
            }
        }

        /// <summary>Number of hour columns (1..24). Changing it recreates the vendor grid on the client.</summary>
        [DefaultValue(24)]
        [Description("Number of hour columns (1..24).")]
        public int Hours
        {
            get => _hours;
            set
            {
                if (value < 1 || value > 24)
                    throw new ArgumentOutOfRangeException(nameof(Hours), value, "Hours must be between 1 and 24.");
                if (_hours == value) return;
                _hours = value;
                dynamic options = base.Options;
                options.hours = value;
            }
        }

        /// <summary>Start of the "warn" colour band (0..100). Must stay below <see cref="HighAt"/>.</summary>
        [DefaultValue(60.0)]
        [Description("Start of the warn band (0..100).")]
        public double WarnAt
        {
            get => _warnAt;
            set
            {
                if (value < 0 || value >= _highAt)
                    throw new ArgumentOutOfRangeException(nameof(WarnAt), value, $"WarnAt must be between 0 and HighAt ({F(_highAt)}).");
                if (_warnAt == value) return;
                _warnAt = value;
                PushThresholds();
            }
        }

        /// <summary>Start of the "high" colour band (0..100). Must stay above <see cref="WarnAt"/>.</summary>
        [DefaultValue(85.0)]
        [Description("Start of the high band (0..100).")]
        public double HighAt
        {
            get => _highAt;
            set
            {
                if (value <= _warnAt || value > LoadSampleService.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(HighAt), value, $"HighAt must be between WarnAt ({F(_warnAt)}) and {F(LoadSampleService.MaxValue)}.");
                if (_highAt == value) return;
                _highAt = value;
                PushThresholds();
            }
        }

        /// <summary>Small caption the vendor draws above the grid.</summary>
        [DefaultValue("")]
        [Description("Caption drawn above the grid.")]
        public string Title
        {
            get => _title;
            set
            {
                value = value ?? "";
                if (_title == value) return;
                _title = value;
                dynamic options = base.Options;
                options.title = value;
            }
        }

        /// <summary>The dataset the server considers current (what the endpoint last served or the task last pushed).</summary>
        [Browsable(false)]
        public IReadOnlyList<HeatmapCell> Cells => _cells;

        /// <summary>True once the vendor reported a successful load (or a background push replaced the data).</summary>
        [Browsable(false)]
        public bool IsDataLoaded { get; private set; }

        /// <summary>Postback requests answered in the last 60 seconds.</summary>
        [Browsable(false)]
        public int RequestsPerMinute
        {
            get
            {
                lock (_requestLock)
                {
                    TrimRequests();
                    return _requestTimes.Count;
                }
            }
        }

        /// <summary>The postback URL the client uses (for the trace); null until the component can compute it.</summary>
        [Browsable(false)]
        public string PostbackUrl
        {
            get
            {
                // (unverified) IWisejHandlerExtension.GetPostbackURL — from the Wisej docs, see COOKBOOK.md.
                try { return ((IWisejHandler)this).GetPostbackURL(); }
                catch { return null; }
            }
        }

        #endregion

        #region Escape hatches hidden from the Designer and IntelliSense

        // The raw Widget surface is still reachable for advanced callers, but it is not part of the
        // wrapper's public contract: typed properties above are the supported way in.

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override List<Package> Packages => base.Packages;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string InitScript
        {
            get => base.InitScript;
            set => base.InitScript = value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override object Options
        {
            get => base.Options;
            set => base.Options = value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string[] WiredEvents
        {
            get => base.WiredEvents;
            set => base.WiredEvents = value;
        }

        #endregion

        #region Events (.NET events raised from widget events)

        /// <summary>Raised when the user clicks a cell (vendor "cellselect").</summary>
        [Description("Raised when the user clicks a cell.")]
        public event EventHandler<HeatmapCellEventArgs> CellSelected;

        /// <summary>Raised when the postback data was fetched and accepted by the vendor (vendor "loaded").</summary>
        [Description("Raised when the postback data was loaded.")]
        public event EventHandler<HeatmapLoadedEventArgs> DataLoaded;

        /// <summary>Raised when the adapter or the vendor reported a failure: init guard, load, update or call (vendor "error").</summary>
        [Description("Raised when the client adapter or the vendor reported a failure.")]
        public event EventHandler<HeatmapErrorEventArgs> LoadFailed;

        /// <summary>Every message that crosses the wire in either direction, for the lab's live trace.</summary>
        [Browsable(false)]
        public event EventHandler<TraceEventArgs> Trace;

        #endregion

        #region Server calls (→ client functions in heatmap-init.js)

        /// <summary>Pulses one cell. Validated on the server before anything is sent.</summary>
        public void Highlight(int day, int hour)
        {
            if (day < 0 || day >= _days) throw new ArgumentOutOfRangeException(nameof(day), day, $"day must be between 0 and {_days - 1}.");
            if (hour < 0 || hour >= _hours) throw new ArgumentOutOfRangeException(nameof(hour), hour, $"hour must be between 0 and {_hours - 1}.");

            // (unverified beyond the docs) Control.Call runs the named wrapper function with these arguments.
            this.Call("highlight", day, hour);
            RaiseTrace(TraceDirection.ServerToClient, $"Call highlight({day},{hour})", $"[{day},{hour}]");
        }

        public void ClearHighlight()
        {
            this.Call("clearHighlight");
            RaiseTrace(TraceDirection.ServerToClient, "Call clearHighlight()", "[]");
        }

        /// <summary>Tells the client to fetch the postback endpoint again (recovery path: always the good "load" action).</summary>
        public void Reload()
        {
            this.Call("reload");
            RaiseTrace(TraceDirection.ServerToClient, "Call reload()", "[]");
        }

        /// <summary>
        /// Replaces the dataset without a fetch: used by the background task
        /// (Application.StartTask → SetCells → Application.Update). Validated first; the server copy is updated too.
        /// </summary>
        public void SetCells(IReadOnlyList<HeatmapCell> cells)
        {
            if (cells == null) throw new ArgumentNullException(nameof(cells));
            foreach (var c in cells)
            {
                if (c.Day < 0 || c.Day >= _days || c.Hour < 0 || c.Hour >= _hours || double.IsNaN(c.Value) || double.IsInfinity(c.Value))
                    throw new ArgumentOutOfRangeException(nameof(cells), c, "Cell outside the grid or with a non-finite value.");
            }

            _cells = cells;
            this.IsDataLoaded = true;
            object payload = ToClientCells(cells);
            this.Call("setCells", new object[] { payload });      // one argument: the whole array
            RaiseTrace(TraceDirection.ServerToClient, "Call setCells(cells)", $"[{cells.Count} cells, peak {F(LoadSampleService.FindPeak(cells).Value)}]");
        }

        /// <summary>Asks the client how many cells hold data (round trip with a return value).</summary>
        public async Task<int> GetCellCountAsync()
        {
            RaiseTrace(TraceDirection.ServerToClient, "CallAsync getCellCount()", "[]");
            // (unverified beyond the docs) Control.CallAsync returns the function's return value.
            object result = await this.CallAsync("getCellCount");
            int count = result == null ? 0 : Convert.ToInt32(result, CultureInfo.InvariantCulture);
            RaiseTrace(TraceDirection.ClientToServer, "getCellCount → return", count.ToString(CultureInfo.InvariantCulture));
            return count;
        }

        /// <summary>The cell with the highest value in the server's copy of the data.</summary>
        public HeatmapCell FindPeak() => LoadSampleService.FindPeak(_cells);

#if DEBUG
        /// <summary>
        /// DEBUG failure path: loads from the postback endpoint with a non-standard action. The endpoint answers
        /// "corrupt" with invalid JSON, so the vendor throws inside load() and the adapter reports one "error" event.
        /// </summary>
        public void LoadWithActionForTesting(string action)
        {
            this.Call("loadWithAction", action);
            RaiseTrace(TraceDirection.ServerToClient, $"Call loadWithAction(\"{action}\")", $"[\"{action}\"]  (failure path on purpose)");
        }

        /// <summary>
        /// DEBUG failure path: a wrapper whose Packages omit vendor-heatmap.js. The adapter's guard clause must throw
        /// "VendorHeatmap not loaded — check Packages order." and report it as an init error instead of a blank widget.
        /// </summary>
        public static HeatmapWidget CreateWithMissingVendorScript() => new HeatmapWidget(true);

        /// <summary>
        /// Wisej.NET caches packages per page and the dashboard has already loaded the library, so "forgetting the
        /// package" cannot be reproduced literally on the same page. This preamble hides the global for this one
        /// widget; OperationsPage restores it through window.__restoreVendorHeatmap() once the error has been reported.
        /// </summary>
        private const string MissingVendorPreamble =
            "/* DEBUG simulation: hide the vendor global so the guard clause sees the page as if the package were missing. */\n" +
            "(function () { if (window.__restoreVendorHeatmap) return; var real = window.VendorHeatmap; window.VendorHeatmap = undefined;\n" +
            "  window.__restoreVendorHeatmap = function () { window.VendorHeatmap = real; delete window.__restoreVendorHeatmap; }; })();\n";
#else
        private const string MissingVendorPreamble = "";
#endif

        [Browsable(false)]
        public bool SimulatesMissingVendor => _simulateMissingVendor;

        #endregion

        #region Postback data endpoint (client: this.getPostbackUrl() + "&action=load")

        /// <summary>
        /// The widget's postback request lands here. (unverified beyond the docs) Widget implements IWisejHandler;
        /// the WebRequest event carries the Wisej.Core.HttpRequest / HttpResponse pair.
        /// </summary>
        protected override void OnWebRequest(WebRequestEventArgs e)
        {
            var request = e.Request;
            var response = e.Response;
            string action = request.QueryString["action"] ?? "";
            string daysArg = request.QueryString["days"];
            RaiseTrace(TraceDirection.ClientToServer, "HTTP GET postback",
                $"?action={action}{(daysArg != null ? "&days=" + daysArg : "")}");
            RecordRequest();

#if DEBUG
            if (action == "corrupt")
            {
                // Deliberately malformed body with the right content type: the vendor must fail loudly, not blank.
                const string broken = "{\"cells\": [ {\"day\": 0, \"hour\": 1, \"value\": }";
                response.ContentType = "application/json";
                response.Write(broken);
                RaiseTrace(TraceDirection.ServerToClient, "HTTP 200 application/json", broken + "  (malformed on purpose)");
                base.OnWebRequest(e);
                return;
            }
#endif

            if (!AllowedActions.Contains(action))
            {
                Reject(response, 400, $"Unknown action \"{action}\".");
                base.OnWebRequest(e);
                return;
            }

            int days = _days;
            if (daysArg != null)
            {
                if (!int.TryParse(daysArg, NumberStyles.Integer, CultureInfo.InvariantCulture, out days) || days < 1 || days > MaxDays)
                {
                    Reject(response, 400, $"days must be an integer between 1 and {MaxDays}.");
                    base.OnWebRequest(e);
                    return;
                }
            }

            // One page of data: the endpoint never serialises a domain object, only HeatmapCell records.
            var cells = LoadSampleService.Generate(days, _hours);
            _cells = cells;
            string json = JsonSerializer.Serialize(new { cells }, JsonOptions);
            response.ContentType = "application/json";
            response.Write(json);
            RaiseTrace(TraceDirection.ServerToClient, "HTTP 200 application/json", $"{{\"cells\":[…{cells.Count} cells…]}}  ({json.Length} bytes)");

            base.OnWebRequest(e);
        }

        private void Reject(Wisej.Core.HttpResponse response, int status, string reason)
        {
            response.StatusCode = status;
            response.ContentType = "text/plain";
            response.Write(reason);
            RaiseTrace(TraceDirection.ServerToClient, $"HTTP {status} text/plain", reason);
        }

        private void RecordRequest()
        {
            lock (_requestLock)
            {
                _requestTimes.Enqueue(DateTime.UtcNow);
                TrimRequests();
            }
        }

        private void TrimRequests()
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-1);
            while (_requestTimes.Count > 0 && _requestTimes.Peek() < cutoff)
                _requestTimes.Dequeue();
        }

        #endregion

        #region Rendering hooks (design mode, postback URL fallback)

        protected override void OnWebRender(dynamic config)
        {
            if (this.DesignMode)
            {
                // Design time: sample data through Options, no endpoint, nothing that can throw in the Designer.
                if (!_designSamplePushed)
                {
                    _designSamplePushed = true;
                    dynamic options = base.Options;
                    options.sampleCells = ToClientCells(LoadSampleService.DesignSample(_days, _hours));
                }
            }
            else if (!_fallbackUrlPushed)
            {
                // Fallback for the adapter in case the wrapper has no getPostbackUrl(): the server-computed URL.
                string url = this.PostbackUrl;
                if (url != null)
                {
                    _fallbackUrlPushed = true;
                    dynamic options = base.Options;
                    options.postbackUrl = url;
                }
            }

            base.OnWebRender((object)config);
        }

        #endregion

        #region Widget events → .NET events

        protected override void OnWidgetEvent(WidgetEventArgs e)
        {
            dynamic data = e.Data;
            switch (e.Type)
            {
                case "cellSelected":
                    {
                        int day = ToInt(data?.day), hour = ToInt(data?.hour);
                        double reported = ToDouble(data?.value);
                        RaiseTrace(TraceDirection.ClientToServer, "cellSelected", $"{{\"day\":{day},\"hour\":{hour},\"value\":{F(reported)}}}");
                        if (day < 0 || day >= _days || hour < 0 || hour >= _hours)
                        {
                            RaiseTrace(TraceDirection.Server, "contract check", $"cell ({day},{hour}) is outside the {_days}×{_hours} grid: ignored");
                            break;
                        }
                        double serverValue = ServerValueAt(day, hour, reported);
                        if (Math.Abs(serverValue - reported) > 0.001)
                            RaiseTrace(TraceDirection.Server, "contract check", $"client reported {F(reported)} but server has {F(serverValue)}: server wins");
                        CellSelected?.Invoke(this, new HeatmapCellEventArgs(day, hour, serverValue, reported));
                        break;
                    }
                case "loaded":
                    {
                        int count = ToInt(data?.count);
                        this.IsDataLoaded = true;
                        RaiseTrace(TraceDirection.ClientToServer, "loaded", $"{{\"count\":{count}}}");
                        DataLoaded?.Invoke(this, new HeatmapLoadedEventArgs(count));
                        break;
                    }
                case "error":
                    {
                        string phase = (string)(data?.phase ?? "unknown");
                        int status = ToInt(data?.status);
                        string message = (string)(data?.message ?? "");
                        RaiseTrace(TraceDirection.ClientToServer, "error", $"{{\"phase\":\"{phase}\",\"status\":{status},\"message\":\"{message}\"}}");
                        LoadFailed?.Invoke(this, new HeatmapErrorEventArgs(phase, status, message));
                        break;
                    }
                default:
                    base.OnWidgetEvent(e);      // never swallow unknown events
                    break;
            }
        }

        #endregion

        #region Helpers

        /// <summary>Compact JSON of the state this component owns (what init(options) receives).</summary>
        public string ToJson()
            => $"{{\"days\":{_days},\"hours\":{_hours},\"thresholds\":{{\"warn\":{F(_warnAt)},\"high\":{F(_highAt)}}},\"title\":\"{_title}\",\"vendorVersion\":\"{VendorVersion}\"}}";

        /// <summary>Called by the UI after it changes something, so the trace shows what went out.</summary>
        public void TraceStateOut(string what, string json) => RaiseTrace(TraceDirection.ServerToClient, what, json);

        private void PushState()
        {
            dynamic options = base.Options;
            options.days = _days;
            options.hours = _hours;
            options.thresholds = new { warn = _warnAt, high = _highAt };
            options.title = _title;
            options.palette = DefaultPalette;
            options.vendorVersion = VendorVersion;
        }

        private void PushThresholds()
        {
            // Nested object: replacing it whole counts as a first-level change (no Notify needed).
            dynamic options = base.Options;
            options.thresholds = new { warn = _warnAt, high = _highAt };
        }

        private static object[] ToClientCells(IReadOnlyList<HeatmapCell> cells)
            => cells.Select(c => (object)new { day = c.Day, hour = c.Hour, value = c.Value }).ToArray();

        private double ServerValueAt(int day, int hour, double fallback)
        {
            foreach (var c in _cells)
                if (c.Day == day && c.Hour == hour) return c.Value;
            return fallback;
        }

        private void RaiseTrace(TraceDirection direction, string name, string payload)
            => Trace?.Invoke(this, new TraceEventArgs(direction, name, payload));

        private static string F(double value) => value.ToString(CultureInfo.InvariantCulture);

        private static double ToDouble(object value)
        {
            try { return value == null ? double.NaN : Convert.ToDouble(value, CultureInfo.InvariantCulture); }
            catch { return double.NaN; }
        }

        private static int ToInt(object value)
        {
            try { return value == null ? -1 : Convert.ToInt32(value, CultureInfo.InvariantCulture); }
            catch { return -1; }
        }

        #endregion
    }
}
