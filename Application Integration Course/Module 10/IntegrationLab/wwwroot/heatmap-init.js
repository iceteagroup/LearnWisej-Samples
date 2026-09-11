// IntegrationLab.Controls.HeatmapWidget — client adapter around VendorHeatmap (Module 10 capstone).
//
// "this" is the Wisej.NET widget wrapper (wisej.web.Widget); this.container is the DOM element
// the framework positions, sizes, shows, hides, themes and disposes.
//
// Contract (see docs/CapstoneImplementation.md):
//   state in  (Options → init/update): days, hours, thresholds {warn, high}, title, palette,
//                                      vendorVersion, sampleCells (design mode only), postbackUrl (fallback)
//   calls in  (Call/CallAsync):        highlight(day, hour), clearHighlight(), reload(), setCells(cells),
//                                      getCellCount() → number, getDiagnostics() → { created, disposed, vendorInstances }
//   data in   (postback):              GET this.getPostbackUrl() + "&action=load" → { cells: [...] }
//   events out (WiredEvents):          cellSelected {day, hour, value}
//                                      loaded       {count}
//                                      error        {phase, status, message}
//
// Production rules this adapter follows:
//   1. Guard clause first: a missing or mismatched vendor library throws ONE clear message
//      instead of leaving a blank widget ("VendorHeatmap not loaded — check Packages order.").
//   2. The vendor object lives in a CHILD element (this.host), never on this.container itself.
//   3. update() re-syncs in place through setOptions(); only a days/hours change recreates the
//      vendor instance, and every recreation goes through the same _wire() function.
//   4. dispose() destroys the vendor instance, disconnects observers, drops handlers and nulls
//      references; window.__integrationLabDisposed counts clean disposals for the leak test.
//   5. The noisy vendor "cellhover" event is never forwarded to the server.

// contract event name → vendor event name
this._vendorEvents = { cellSelected: "cellselect", loaded: "loaded", error: "error" };
this._handlers = {};        // contract name → framework handler, kept so a recreate can re-wire

this._assertVendor = function (options) {
    if (typeof VendorHeatmap === "undefined")
        throw new Error("VendorHeatmap not loaded — check Packages order.");
    if (options && options.vendorVersion && VendorHeatmap.version !== options.vendorVersion)
        throw new Error("VendorHeatmap " + VendorHeatmap.version + " is loaded but HeatmapWidget expects "
            + options.vendorVersion + " — update HeatmapWidget.VendorVersion or the package file.");
};

this.init = function (options) {
    var me = this;
    this._options = options || {};

    // Disposal is installed BEFORE anything that can fail, so a half-initialized widget is still safe to dispose.
    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            if (me._resizeObserver) { me._resizeObserver.disconnect(); me._resizeObserver = null; }
            if (me.widget) { me.widget.destroy(); me.widget = null; }
            me._handlers = {};
            if (me.host && me.host.parentNode) me.host.parentNode.removeChild(me.host);
            me.host = null;
            me._options = null;
            if (me._created) { me._created = false; window.__integrationLabDisposed = (window.__integrationLabDisposed || 0) + 1; }
        }
        finally {
            if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments);
        }
    };

    try {
        this._assertVendor(this._options);          // guard clause first: names the cause in one line

        var host = document.createElement("div");
        host.className = "heatmap-widget-host";
        host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;overflow:hidden;";
        this.container.innerHTML = "";
        this.container.appendChild(host);
        this.host = host;

        this._wire(this._options);
        this._created = true;
        window.__integrationLabCreated = (window.__integrationLabCreated || 0) + 1;
    }
    catch (ex) {
        this.widget = null;
        if (window.console && console.error) console.error("[HeatmapWidget] init failed:", ex);   // visible in DevTools with the sourceURL below
        this._reportError("init", ex.message, 0);
        return;
    }

    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () { if (me.widget) me.widget.resize(); });
        this._resizeObserver.observe(this.host);
    }
};

// One place creates the vendor instance and re-attaches every handler: init and recreate both use it.
this._wire = function (options) {
    var widget = new VendorHeatmap(this.host, this._vendorOptions(options));
    this.widget = widget;
    for (var name in this._handlers) {
        var vendorName = this._vendorEvents[name];
        if (vendorName) widget.on(vendorName, this._handlers[name]);
    }
    this._loadInitial(options);
    return widget;
};

this._vendorOptions = function (options) {
    return {
        days: options.days, hours: options.hours, thresholds: options.thresholds,
        title: options.title, palette: options.palette,
        dataUrl: options.sampleCells ? null : this._dataUrl("load")
    };
};

this._loadInitial = function (options) {
    if (options.sampleCells) {                      // design mode: sample data, no services, nothing thrown
        this.widget.setData(options.sampleCells);
        return;
    }
    this.widget.load().catch(function () { /* already reported through the vendor "error" event */ });
};

// Postback data source. (unverified) this.getPostbackUrl() is the wrapper API from the Wisej.NET docs;
// options.postbackUrl is the server-computed fallback pushed by HeatmapWidget.OnWebRender.
this._dataUrl = function (action) {
    var base = (typeof this.getPostbackUrl === "function") ? this.getPostbackUrl() : (this._options && this._options.postbackUrl);
    if (!base)
        throw new Error("HeatmapWidget: no postback URL is available — the wrapper must belong to a live Wisej.NET session.");
    return base + (base.indexOf("?") >= 0 ? "&" : "?") + "action=" + encodeURIComponent(action);
};

// Called by Wisej.NET when a first-level Options field changes on the server.
this.update = function (options, old) {
    var previous = this._options || {};
    var merged = {};
    for (var k in previous) merged[k] = previous[k];
    for (var j in options) if (options[j] !== undefined) merged[j] = options[j];
    this._options = merged;
    if (!this.widget) return;

    try {
        var recreate = (options.days !== undefined && options.days !== previous.days)
                    || (options.hours !== undefined && options.hours !== previous.hours);
        if (recreate) {                             // the vendor cannot resize its grid in place: destroy + recreate through _wire
            this.widget.destroy();
            this.widget = null;
            this._wire(merged);
            return;
        }
        this.widget.setOptions({ thresholds: options.thresholds, title: options.title, palette: options.palette });   // re-sync in place
        if (options.sampleCells && options.sampleCells !== previous.sampleCells) this.widget.setData(options.sampleCells);
    }
    catch (ex) {
        this._reportError("update", ex.message, 0);
    }
};

// Wisej.NET calls this once for every name in WiredEvents (after "loaded"). "handler" is the
// framework's deferred dispatcher: it calls _getEventData and fires the widget event on the next tick.
this._addListener = function (name, handler) {
    this._handlers[name] = handler;
    var vendorName = this._vendorEvents[name];
    if (vendorName && this.widget) this.widget.on(vendorName, handler);
};

this._removeListener = function (name, handler) {
    if (this._handlers[name] === handler) delete this._handlers[name];
    var vendorName = this._vendorEvents[name];
    if (vendorName && this.widget) this.widget.off(vendorName, handler);
};

// Translate the vendor payload into the contract payload: only meaningful data crosses the wire.
this._getEventData = function (type, e) {
    switch (type) {
        case "cellSelected": return { day: e.day, hour: e.hour, value: e.value };
        case "loaded": return { count: e.count };
        case "error": return { phase: e.phase || "load", status: e.status || 0, message: e.message };
    }
    return null;
};

// Report an adapter-level failure as one contract "error" event (same channel as vendor load errors).
this._reportError = function (phase, message, status) {
    var me = this, data = { phase: phase, status: status || 0, message: message };
    if (this._handlers.error) { this._handlers.error(data); return; }
    setTimeout(function () { me.fireWidgetEvent("error", data); }, 0);
};

// ---- functions the server reaches with Call(...) / CallAsync(...) ----------------------------

this.highlight = function (day, hour) {
    if (!this.widget) return;
    try { this.widget.highlight(day, hour); }
    catch (ex) { this._reportError("call", ex.message, 0); }
};

this.clearHighlight = function () {
    if (this.widget) this.widget.clearHighlight();
};

this.reload = function () {
    if (!this.widget) return;
    try {
        this.widget.setOptions({ dataUrl: this._dataUrl("load") });   // always recover onto the good endpoint
        this.widget.load().catch(function () { });
    }
    catch (ex) { this._reportError("call", ex.message, 0); }
};

// Background updates land here: Application.StartTask → Call("setCells", cells) → Application.Update.
this.setCells = function (cells) {
    if (!this.widget) return;
    try { this.widget.setData(cells); }
    catch (ex) { this._reportError("call", ex.message, 0); }
};

this.getCellCount = function () {
    return this.widget ? this.widget.getCellCount() : 0;
};

this.getDiagnostics = function () {
    return {
        created: window.__integrationLabCreated || 0,
        disposed: window.__integrationLabDisposed || 0,
        vendorInstances: (typeof VendorHeatmap !== "undefined" && VendorHeatmap.liveInstances) ? VendorHeatmap.liveInstances() : -1
    };
};

//# sourceURL=integrationlab.controls.HeatmapWidget.js
