// IntegrationLab.GaugeWidget — client adapter around VendorGauge (Module 7).
//
// "this" is the Wisej.NET widget wrapper (wisej.web.Widget); this.container is
// the DOM element the framework owns. The vendor instance lives in a child host.
//
// Client event handler #1 — thresholdCrossed
//   subscribed : ONE vendor callback, "rangechange" (fires when the reading moves
//                into another band: normal / warm / high)
//   forwarded  : thresholdCrossed { value: number, level: "warn" | "high" }
//                rising edge only, once per crossing (the lesson's "above" flag)
//   kept local : the vendor's needle animation, "thresholdexceeded" (we derive
//                our own edge so the payload does not depend on the vendor)
//
// Timing rule (verified in Module 1): the crossing is usually CAUSED by a server
// Options change, i.e. the vendor callback runs inside update(). A synchronous
// fireWidgetEvent there is silently dropped, so the event goes through the
// framework handler registered by _addListener (deferred to the next tick).

this.init = function (options) {
    var me = this;

    var host = document.createElement("div");
    host.className = "gauge-host";
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;

    this._readThresholds(options);
    try {
        this.widget = new VendorGauge(host, options);
    }
    catch (ex) {
        this.widget = null;
        this._reportError("init", ex.message);
        return;
    }

    // Lesson flags: are we already above each threshold? (no event on init)
    this.above = options.value >= this._threshold;
    this.warned = options.value >= this._warnAt;

    this._wire();       // the single place vendor callbacks are attached

    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () { if (me.widget) me.widget.resize(); });
        this._resizeObserver.observe(host);
    }

    // Every attach has a detach in the disposal path (no vendor handler outlives the widget).
    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            if (me._resizeObserver) { me._resizeObserver.disconnect(); me._resizeObserver = null; }
            me._unwire();
            if (me.widget) { me.widget.destroy(); me.widget = null; }
            if (me.host && me.host.parentNode) me.host.parentNode.removeChild(me.host);
            me.host = null;
        }
        finally {
            if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments);
        }
    };
};

// wire ONE vendor callback (called from init; a recreate path would call it again)
this._wire = function () {
    var me = this;
    this._onVendorRange = function (e) { me._onChange(e.value); };
    this.widget.on("rangechange", this._onVendorRange);
};

this._unwire = function () {
    if (this.widget && this._onVendorRange) this.widget.off("rangechange", this._onVendorRange);
    this._onVendorRange = null;
};

// The client decides when the event is meaningful and fires ONCE per crossing.
this._onChange = function (v) {
    var level = null;
    if (v >= this._threshold) {
        if (!this.above) level = "high";          // rising edge over Threshold
        this.above = true;
        this.warned = true;                        // one change → at most one event
    }
    else if (v >= this._warnAt) {
        if (!this.warned) level = "warn";          // rising edge over WarnAt
        this.above = false;
        this.warned = true;
    }
    else {
        this.above = false;                        // falling: reset, no event
        this.warned = false;
    }
    if (level) this._raise("thresholdCrossed", { value: v, level: level });
};

// Raise a contract event through the framework's deferred handler when it is
// registered; otherwise defer ourselves. Never fire synchronously inside update().
this._raise = function (name, data) {
    var handler = this._handlers && this._handlers[name];
    if (handler) { handler(data); return; }
    var me = this;
    setTimeout(function () { me.fireWidgetEvent(name, data); }, 0);
};

// Called by Wisej.NET when a first-level Options field changes on the server.
this.update = function (options, old) {
    if (!this.widget) return;
    this._readThresholds(options);
    try {
        this.widget.setOptions({
            min: options.min, max: options.max, warnAt: options.warnAt, threshold: options.threshold,
            label: options.label, units: options.units,
            value: options.value
        });
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
};

this._readThresholds = function (options) {
    this._threshold = typeof options.threshold === "number" ? options.threshold : 100;
    this._warnAt = typeof options.warnAt === "number" ? options.warnAt : 85;
};

// Wisej.NET calls this once per name in WiredEvents (after "loaded"). "handler"
// is the framework's deferred dispatcher: it calls _getEventData and then
// fireWidgetEvent on the next tick.
this._addListener = function (name, handler) {
    (this._handlers = this._handlers || {})[name] = handler;
};
this._removeListener = function (name, handler) {
    if (this._handlers && this._handlers[name] === handler) delete this._handlers[name];
};

// Translate into the contract payload: only the fields in docs/PayloadContract.md.
this._getEventData = function (type, e) {
    switch (type) {
        case "thresholdCrossed": return { value: e.value, level: e.level };
        case "error": return { phase: e.phase, message: e.message };
    }
    return null;
};

this._reportError = function (phase, message) {
    this._raise("error", { phase: phase, message: message });
};

//# sourceURL=integrationlab.widgets.GaugeWidget.js
