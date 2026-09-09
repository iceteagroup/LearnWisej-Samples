// IntegrationLab.Controls.TemperatureGauge — client adapter around VendorGauge (copied from Module 1).
//
// "this" is the Wisej.NET widget wrapper (wisej.web.Widget); this.container is
// the DOM element the framework positions, sizes, shows, hides, themes and disposes.
//
// Contract (see docs/ClientServerContract.md):
//   state in  (Options → init/update): value, min, max, warnAt, threshold, label, units
//   events out (WiredEvents):          thresholdExceeded {value}
//                                      rangeChanged      {range, value}
//                                      error             {phase, message}
//
// Rules this adapter follows:
//   1. The vendor object is created inside a child element (this.host), never on
//      this.container itself, so Wisej.NET keeps ownership of its own element.
//   2. The vendor instance is stored in this.widget so Instance.xxx() calls from
//      the server reach the vendor object.
//   3. Events are wired through _addListener / _getEventData. The framework calls
//      _addListener once per name in WiredEvents and defers the server round-trip,
//      so events raised while a server update is being applied are not lost.
//      (Calling fireWidgetEvent synchronously from inside update() would be dropped.)

// contract event name → vendor event name
this._vendorEvents = {
    thresholdExceeded: "thresholdexceeded",
    rangeChanged: "rangechange"
};

this.init = function (options) {
    var me = this;

    var host = document.createElement("div");
    host.className = "temperature-gauge-host";
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;

    try {
        this.widget = new VendorGauge(host, options);
    }
    catch (ex) {
        this.widget = null;
        this._reportError("init", ex.message);
        return;
    }

    // Resize: forward to the vendor's resize method.
    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () {
            if (me.widget) me.widget.resize();
        });
        this._resizeObserver.observe(host);
    }

    // Disposal: destroy the vendor object, clear the host, then let the
    // framework dispose the wrapper as usual.
    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            if (me._resizeObserver) { me._resizeObserver.disconnect(); me._resizeObserver = null; }
            if (me.widget) { me.widget.destroy(); me.widget = null; }
            if (me.host && me.host.parentNode) me.host.parentNode.removeChild(me.host);
            me.host = null;
        }
        finally {
            if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments);
        }
    };
};

// Called by Wisej.NET when a first-level Options field changes on the server.
this.update = function (options, old) {
    if (!this.widget) return;
    try {
        this.widget.setOptions({
            min: options.min, max: options.max, warnAt: options.warnAt,
            threshold: options.threshold, label: options.label, units: options.units,
            value: options.value
        });
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
};

// Wisej.NET calls this once for every name in WiredEvents (after "loaded").
// "handler" is the framework's deferred dispatcher; it calls _getEventData and
// then fireWidgetEvent on the next tick.
this._addListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        if (this.widget) this.widget.on(vendorName, handler);
        return;
    }
    if (name === "error") {
        // "error" is raised by the adapter itself, not by the vendor.
        this._errorHandler = handler;
    }
};

this._removeListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        if (this.widget) this.widget.off(vendorName, handler);
        return;
    }
    if (name === "error" && this._errorHandler === handler)
        this._errorHandler = null;
};

// Translate the vendor payload into the contract payload: only meaningful data.
this._getEventData = function (type, e) {
    switch (type) {
        case "thresholdExceeded":
            return { value: e.value };
        case "rangeChanged":
            return { range: e.range, value: e.value };
        case "error":
            return { phase: e.phase, message: e.message };
    }
    return null;
};

// Report a vendor failure as one contract event. Goes through the framework's
// deferred handler when it is registered; otherwise defers the fire itself.
this._reportError = function (phase, message) {
    var me = this, data = { phase: phase, message: message };
    if (this._errorHandler) {
        this._errorHandler(data);
        return;
    }
    setTimeout(function () { me.fireWidgetEvent("error", data); }, 0);
};

//# sourceURL=integrationlab.controls.TemperatureGauge.js
