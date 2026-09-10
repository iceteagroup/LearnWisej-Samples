// EnterpriseOps.Widgets.WorkOrderChartWidget — client adapter around EnterpriseOpsChart 1.2.
//
// Embedded resource "EnterpriseOps.Widgets.opschart-init.js" (see EnterpriseOps.csproj). The
// WorkOrderChartWidget class hands this file to the widget as its InitScript; no page and no
// designer ever references it, and it is versioned with the class that ships it.
//
// "this" is the Wisej.NET widget wrapper (wisej.web.Widget). this.container is the DOM element
// the framework positions, sizes, shows, hides, themes and disposes.
//
// THE CONTRACT (docs/EventContract.md) — the only thing that crosses the wire:
//   down (Options → init/update):  segments[{key,label,value}] · palette · caption · showLegend
//                                  · badge · simulateBlockedVendor · simulateVendorFailure
//   up   (WiredEvents):            pointSelected {key, label, value, percent}
//                                  error         {phase, message}
//   calls (server → wrapper):      selectSegment(key) · getRenderState()
//
// Rules this adapter follows (Application Integration cookbook, verified):
//   1. The vendor object lives in a CHILD element (this.host), never on this.container.
//   2. The vendor instance is stored in this.widget so Instance.xxx() reaches it.
//   3. Events go through _addListener / _getEventData. The framework registers one handler per
//      WiredEvents entry and defers the round trip itself; a synchronous fireWidgetEvent raised
//      inside update() would be dropped.
//   4. Every vendor call is wrapped in try/catch. A vendor failure becomes the "error" event and
//      an embedded fallback rendering — never an exception thrown at the screen.
//   5. The wrapper never invents data. It reports what the vendor said, the server decides.

// contract event name → vendor event name
this._vendorEvents = {
    pointSelected: "pointclick"
};

this.init = function (options) {

    var me = this;

    var host = document.createElement("div");
    host.className = "eops-chart-host";
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;

    this._options = options;
    this._blocked = false;

    this._createVendor(options);

    // Keep the SVG in step with the Wisej layout.
    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () {
            if (me.widget) {
                try { me.widget.resize(); }
                catch (ex) { me._reportError("resize", ex.message); }
            }
        });
        this._resizeObserver.observe(host);
    }

    // Wrap — never replace — the framework's dispose.
    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            if (me._resizeObserver) { me._resizeObserver.disconnect(); me._resizeObserver = null; }
            me._destroyVendor();
            if (me.host && me.host.parentNode) me.host.parentNode.removeChild(me.host);
            me.host = null;
        }
        finally {
            if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments);
        }
    };
};

// Called by Wisej.NET whenever a first-level Options field changed on the server.
this.update = function (options, old) {
    if (options) options.segments = this._normalizeSegments(options.segments);

    this._options = options;

    // The proxy-block simulation flips the vendor in and out; treat it as a re-init.
    var blockedNow = options.simulateBlockedVendor === true;
    if (blockedNow !== this._blocked) {
        this._destroyVendor();
        this._createVendor(options);
        return;
    }

    if (!this.widget) {
        // Still on the fallback: redraw it so the numbers stay current.
        this._renderFallback(options, this._lastErrorMessage);
        return;
    }

    try {
        if (options.simulateVendorFailure === true) {
            // The lab's "the vendor throws" path: hand the vendor an option it rejects. The vendor
            // validates BEFORE it touches the screen, so the chart on screen is still the old one.
            this.widget.setOptions({ segments: null });
            return;
        }

        this.widget.setOptions({
            segments: options.segments,
            palette: options.palette,
            caption: options.caption,
            showLegend: options.showLegend,
            badge: options.badge
        });

        if (options.selectedKey)
            this.widget.select(options.selectedKey);
    }
    catch (ex) {
        // The vendor refused. Say so through the contract and fall back — the screen keeps working.
        this._reportError("update", ex && ex.message ? ex.message : String(ex));
        this._destroyVendor();
        this._renderFallback(options, ex && ex.message ? ex.message : String(ex));
    }
};

// ---- vendor lifecycle ------------------------------------------------------

// Wire tolerance: the server sends {key,label,value}; accept {Key,Label,Value} too so a casing slip
// never turns into a blank chart (it did once — see WorkOrderChartWidget.ToWire).
this._normalizeSegments = function (segments) {
    if (!segments || !segments.length) return segments;
    var out = [];
    for (var i = 0; i < segments.length; i++) {
        var s = segments[i] || {};
        out.push({
            key: s.key !== undefined ? s.key : s.Key,
            label: s.label !== undefined ? s.label : s.Label,
            value: s.value !== undefined ? s.value : s.Value
        });
    }
    return out;
};

this._createVendor = function (options) {

    var me = this;
    if (options) options.segments = this._normalizeSegments(options.segments);
    this._blocked = options.simulateBlockedVendor === true;

    // The real guard: the package may not have loaded at all (proxy, CSP, offline, 404).
    var available = (typeof EnterpriseOpsChart !== "undefined") && !this._blocked;
    if (!available) {
        this.widget = null;
        var why = this._blocked
            ? "vendor-opschart.js was blocked before it could define EnterpriseOpsChart"
            : "EnterpriseOpsChart is not defined — Widgets/vendor-opschart.js did not load";
        this._reportError("init", why);
        this._renderFallback(options, why);
        return;
    }

    try {
        this.host.innerHTML = "";
        this.widget = new EnterpriseOpsChart(this.host, {
            segments: options.segments,
            palette: options.palette,
            caption: options.caption,
            showLegend: options.showLegend,
            badge: options.badge
        });

        // The vendor's own "drawing failed" channel maps onto the same contract event.
        this.widget.on("renderfail", function (e) {
            me._reportError("render", e && e.message ? e.message : "render failed");
        });

        if (options.selectedKey)
            this.widget.select(options.selectedKey);

        // Re-attach the contract listener the framework registered before this re-init.
        if (this._pointHandler)
            this.widget.on("pointclick", this._pointHandler);
    }
    catch (ex) {
        this.widget = null;
        var message = ex && ex.message ? ex.message : String(ex);
        this._reportError("init", message);
        this._renderFallback(options, message);
    }
};

this._destroyVendor = function () {
    if (!this.widget) return;
    try { this.widget.destroy(); }
    catch (ignored) { /* a broken vendor must not break disposal */ }
    this.widget = null;
};

// ---- the polite fallback (ships with the component, no vendor needed) -------

this._renderFallback = function (options, message) {

    this._lastErrorMessage = message || "";
    if (!this.host) return;

    var segments = (options && options.segments) || [];
    var total = 0, i;
    for (i = 0; i < segments.length; i++) total += segments[i].value;
    if (total <= 0) total = 1;

    var html = '<div class="eops-chart-fallback">' +
        '<div class="eops-chart-head">' +
        '<span class="eops-chart-title">' + this._escape((options && options.caption) || "Work-order history") + ' — fallback</span>' +
        '<span class="eops-chart-vendor">vendor-opschart.js unavailable</span>' +
        '</div>';

    var colors = { open: "#1a86ff", onhold: "#b9770e", escalated: "#c0392b", done: "#1f8a4c" };
    for (i = 0; i < segments.length; i++) {
        var s = segments[i];
        var percent = Math.round((s.value / total) * 100);
        html += '<div class="eops-fallback-row">' +
            '<span class="eops-chart-swatch" style="background:' + (colors[s.key] || "#8a97a4") + '"></span>' +
            '<span class="eops-fallback-label">' + this._escape(s.label) + '</span>' +
            '<span class="eops-fallback-value">' + s.value + ' · ' + percent + '%</span>' +
            '</div>';
    }

    html += '</div>';
    this.host.innerHTML = html;
};

this._escape = function (text) {
    return String(text == null ? "" : text)
        .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
};

// ---- the event contract ----------------------------------------------------

// The framework calls this once per WiredEvents entry, after "loaded".
this._addListener = function (name, handler) {

    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        this._pointHandler = handler;                       // remembered across a vendor re-init
        if (this.widget) this.widget.on(vendorName, handler);
        return;
    }

    if (name === "error") {
        this._errorHandler = handler;
        // An error raised before the listener existed (a blocked package) is delivered now.
        if (this._pendingError) {
            var pending = this._pendingError;
            this._pendingError = null;
            handler(pending);
        }
    }
};

this._removeListener = function (name, handler) {

    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        if (this._pointHandler === handler) this._pointHandler = null;
        if (this.widget) this.widget.off(vendorName, handler);
        return;
    }

    if (name === "error" && this._errorHandler === handler)
        this._errorHandler = null;
};

// Translate the vendor payload into the small contract payload. Nothing else crosses.
this._getEventData = function (type, e) {
    switch (type) {
        case "pointSelected":
            return { key: e.key, label: e.label, value: e.value, percent: e.percent };
        case "error":
            return { phase: e.phase, message: e.message };
    }
    return null;
};

// Always deferred: an error raised synchronously inside init() or update() would be dropped by
// the framework, so it leaves on the next tick. If "error" has not been wired yet (init failures
// happen before _addListener runs) the payload waits in _pendingError and _addListener delivers it.
this._reportError = function (phase, message) {
    var me = this, data = { phase: phase, message: message };
    setTimeout(function () {
        if (me._errorHandler) { me._errorHandler(data); return; }
        me._pendingError = data;
    }, 0);
};

// ---- functions the server reaches with Call(...) / CallAsync(...) -----------

this.selectSegment = function (key) {
    if (!this.widget) return;
    try { this.widget.select(key); }
    catch (ex) { this._reportError("select", ex.message); }
};

this.getRenderState = function () {
    return {
        vendorLoaded: !!this.widget,
        vendorVersion: this.widget ? this.widget.version : null,
        selectedKey: this.widget ? this.widget.getSelectedKey() : null,
        fallback: !this.widget
    };
};

//# sourceURL=enterpriseops.widgets.WorkOrderChartWidget.js
