// IntegrationLab — gauge-init.js
// InitScript of the "gauge" Wisej.Web.Widget on DashboardPage (no wrapper class: a plain Widget).
//
// "this" is the Wisej.NET widget wrapper (wisej.web.Widget); this.container is the DOM element
// the framework positions, sizes, shows, hides, themes and disposes.
//
// Options contract (server → client). The Wisej.NET serializer camel-cases property names, so
// the C# spellings on the right arrive as the JavaScript spellings on the left:
//
//   value   number                     current reading — top level because it changes often
//   range   { minValue, maxValue }     C#  Options.range = new { MinValue = 0, MaxValue = 120 }
//   bands   [ { from, to, color } ]    C#  Options.bands = new[] { Band(0, 85, "#1f9d57"), ... }
//   label   { text, units }            C#  Options.label = new { Text = "Boiler 3", Units = "°F" }
//   style   "card" | "compact"         which host the vendor is bound to → requires recreation
//
// Events out (WiredEvents on the server): error {phase, message}
// Server → client calls (Call): flash()
//
// VendorGauge has no "bands" API — it knows warnAt / threshold. The adapter translates:
//   warnAt    = start of the 2nd band (sorted by "from")
//   threshold = start of the last band
// and draws the band colors itself as a small legend under the gauge (div.gauge-bands).
// That translation is the kind of vendor-specific detail that must stay inside the adapter.

this._events = {};                       // adapter-raised event name → framework handler (see _addListener)

// ---------------------------------------------------------------------------------------------
// init(options): runs ONCE, after the Packages have loaded and this.container exists.
// ---------------------------------------------------------------------------------------------
this.init = function (options) {
    var me = this;
    options = options || {};
    this._options = options;

    this._buildHost(options.style);

    try {
        // the vendor lives in a CHILD element (this.host), never on this.container itself
        this.widget = new VendorGauge(this.host, this._toVendorOptions(options));
        this._renderBands(options.bands);
    }
    catch (ex) {
        this.widget = null;
        this._reportError("init", ex.message);
        return;
    }

    // resize: forward to the vendor
    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () { if (me.widget) me.widget.resize(); });
        this._resizeObserver.observe(this.host);
    }

    // dispose: wrap, never replace, the framework dispose
    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            if (me._resizeObserver) { me._resizeObserver.disconnect(); me._resizeObserver = null; }
            if (me.widget) { me.widget.destroy(); me.widget = null; }
            if (me.frame && me.frame.parentNode) me.frame.parentNode.removeChild(me.frame);
            me.frame = null; me.host = null; me.legend = null;
        }
        finally {
            if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments);
        }
    };
};

// ---------------------------------------------------------------------------------------------
// update(options, old): runs EVERY time the server changes a first-level field of Options.
// Compare against "old" and push only what changed — an unrelated option must not restart an
// animation, and the one option that needs recreation (style) recreates only when it changed.
// ---------------------------------------------------------------------------------------------
this.update = function (options, old) {
    if (!this.widget) return;
    options = options || {};
    old = old || {};
    this._options = options;

    try {
        // 1. the option the vendor cannot take after construction → destroy & recreate
        if (options.style !== old.style) {
            this._recreate(options);
            return;                                   // the new instance was built from the full options
        }

        // 2. options with a clean setOptions path — only the changed ones
        var changes = {};
        if (!this._same(options.range, old.range)) {
            changes.min = options.range ? options.range.minValue : undefined;
            changes.max = options.range ? options.range.maxValue : undefined;
        }
        if (!this._same(options.label, old.label)) {
            changes.label = options.label ? options.label.text : "";
            changes.units = options.label ? options.label.units : "";
        }
        if (!this._same(options.bands, old.bands)) {   // nested objects compare as whole values
            var t = this._thresholds(options.bands, options.range);
            changes.warnAt = t.warnAt;
            changes.threshold = t.threshold;
            this._renderBands(options.bands);
        }
        if (Object.keys(changes).length)
            this.widget.setOptions(changes);

        // 3. the frequently changing top-level value
        if (options.value !== old.value)
            this.widget.setValue(options.value);
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
};

// ---------------------------------------------------------------------------------------------
// functions the server reaches with Call("flash"): "this" is this wrapper, return value ignored
// ---------------------------------------------------------------------------------------------
this.flash = function () {
    var host = this.host;
    if (!host) return;
    host.classList.remove("gauge-host--flash");
    void host.offsetWidth;                            // restart the CSS animation
    host.classList.add("gauge-host--flash");
    setTimeout(function () { host.classList.remove("gauge-host--flash"); }, 700);
};

// ---------------------------------------------------------------------------------------------
// events: the framework calls _addListener once per WiredEvents entry (after "loaded"); the
// handler it passes defers the round trip and calls _getEventData for the payload.
// ---------------------------------------------------------------------------------------------
this._addListener = function (name, handler) { this._events[name] = handler; };
this._removeListener = function (name, handler) { if (this._events[name] === handler) delete this._events[name]; };
this._getEventData = function (type, e) { return e; };   // adapter events already carry the contract payload

this._raise = function (name, data) {
    var me = this;
    if (this._events[name]) { this._events[name](data); return; }
    setTimeout(function () { me.fireWidgetEvent(name, data); }, 0);
};
this._reportError = function (phase, message) {
    this._raise("error", { phase: phase, message: message });
};

// ---------------------------------------------------------------------------------------------
// internals
// ---------------------------------------------------------------------------------------------

// The vendor is bound to its element in the constructor and has no re-parent API, so a style
// that needs a different host element (compact: dark, padded box) means a new instance.
this._buildHost = function (style) {
    var frame = document.createElement("div");
    frame.className = "gauge-frame" + (style === "compact" ? " gauge-frame--compact" : "");
    var host = document.createElement("div");
    host.className = "gauge-host";
    var legend = document.createElement("div");
    legend.className = "gauge-bands";
    frame.appendChild(host);
    frame.appendChild(legend);

    this.container.innerHTML = "";
    this.container.appendChild(frame);
    this.frame = frame; this.host = host; this.legend = legend;
};

this._recreate = function (options) {
    var me = this;
    if (this._resizeObserver) { this._resizeObserver.disconnect(); this._resizeObserver = null; }
    if (this.widget) { this.widget.destroy(); this.widget = null; }
    if (this.frame && this.frame.parentNode) this.frame.parentNode.removeChild(this.frame);

    this._buildHost(options.style);
    this.widget = new VendorGauge(this.host, this._toVendorOptions(options));
    this._renderBands(options.bands);

    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () { if (me.widget) me.widget.resize(); });
        this._resizeObserver.observe(this.host);
    }
};

// Options (contract) → VendorGauge constructor options (vendor)
this._toVendorOptions = function (options) {
    var o = { value: options.value };
    if (options.range) { o.min = options.range.minValue; o.max = options.range.maxValue; }
    if (options.label) { o.label = options.label.text; o.units = options.label.units; }
    var t = this._thresholds(options.bands, options.range);
    if (t.warnAt !== undefined) o.warnAt = t.warnAt;
    if (t.threshold !== undefined) o.threshold = t.threshold;
    return o;
};

this._thresholds = function (bands, range) {
    if (!bands || !bands.length) return {};
    var sorted = bands.slice().sort(function (a, b) { return a.from - b.from; });
    var max = range ? range.maxValue : sorted[sorted.length - 1].to;
    return {
        warnAt: sorted.length > 1 ? sorted[1].from : max,
        threshold: sorted.length > 2 ? sorted[sorted.length - 1].from : max
    };
};

this._renderBands = function (bands) {
    if (!this.legend) return;
    this.legend.innerHTML = "";
    (bands || []).forEach(function (b) {
        var chip = document.createElement("span");
        chip.className = "gauge-band";
        var dot = document.createElement("i");
        dot.style.background = b.color || "#999";
        chip.appendChild(dot);
        chip.appendChild(document.createTextNode(b.from + "–" + b.to));
        this.legend.appendChild(chip);
    }, this);
};

this._same = function (a, b) { return JSON.stringify(a) === JSON.stringify(b); };

//# sourceURL=gauge-init.js
