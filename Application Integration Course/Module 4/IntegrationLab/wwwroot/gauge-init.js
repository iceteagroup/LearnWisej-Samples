// IntegrationLab.Controls.SimpleGauge — client adapter around VendorGauge 1.0.
//
// Embedded resource "IntegrationLab.wwwroot.gauge-init.js": the SimpleGauge class hands
// this file to the widget as its InitScript. No page ever references it.
//
// "this" is the Wisej.NET widget wrapper (wisej.web.Widget); this.container is the DOM
// element the framework positions, sizes, shows, hides, themes and disposes.
//
// Contract (see docs/ReusableWidgetClass.md):
//   state in  (Options → init/update): value, min, max, threshold, warnAt, label, units, animationEnabled
//   events out (WiredEvents):          valueChanged      {value, previous}   (the browser rendered a new value)
//                                      thresholdExceeded {value, threshold}  (rising edge, from the vendor)
//                                      error             {phase, message}    (vendor failure caught by the adapter)
//
// Rules this adapter follows:
//   1. The vendor object lives in a child element (this.host), never on this.container.
//   2. The vendor instance is stored in this.widget so Instance.xxx() calls reach it.
//   3. Events go through _addListener / _getEventData. The framework registers one handler
//      per WiredEvents entry and defers the round trip itself, so events raised while a
//      server update is being applied are not lost (a synchronous fireWidgetEvent inside
//      update() would be dropped).
//   4. The vendor has no value-change event of its own (its "rangechange" is about bands),
//      so valueChanged is reported by the adapter once the target value is on screen.

// contract event name → vendor event name
this._vendorEvents = {
    thresholdExceeded: "thresholdexceeded"
};

this.init = function (options) {
    var me = this;

    var host = document.createElement("div");
    host.className = "simple-gauge-host";
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;

    this._animationEnabled = options.animationEnabled !== false;
    this._target = options.value;
    this._animation = null;

    try {
        this.widget = new VendorGauge(host, this._vendorOptions(options));
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

    // Disposal: stop any sweep, destroy the vendor object, clear the host, then let the
    // framework dispose the wrapper as usual.
    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            me._cancelAnimation();
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

    this._animationEnabled = options.animationEnabled !== false;

    try {
        // 1. Scale, bands and caption: instant.
        this.widget.setOptions({
            min: options.min, max: options.max,
            warnAt: options.warnAt, threshold: options.threshold,
            label: options.label, units: options.units
        });

        // 2. The reading: sweep or jump, then acknowledge with valueChanged.
        if (options.value !== this._target)
            this._moveTo(options.value);
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
};

// Only the options the vendor understands, by the vendor's names.
this._vendorOptions = function (o) {
    return {
        value: o.value, min: o.min, max: o.max,
        warnAt: o.warnAt, threshold: o.threshold,
        label: o.label, units: o.units
    };
};

// Move the needle to "target". With animationEnabled the value is tweened through the
// vendor frame by frame (so the vendor still sees the threshold crossing and raises its
// own rising-edge event exactly once); otherwise it jumps.
this._moveTo = function (target) {
    var me = this;
    var from = this.widget.getValue();
    var previous = this._target;
    this._target = target;
    this._cancelAnimation();

    if (!this._animationEnabled || typeof from !== "number" || from === target) {
        this.widget.setValue(target);
        this._acknowledge(target, previous);
        return;
    }

    var start = performance.now(), duration = 600;
    var step = function (now) {
        if (!me.widget) { me._animation = null; return; }
        var f = Math.min(1, (now - start) / duration);
        var eased = f < 0.5 ? 4 * f * f * f : 1 - Math.pow(-2 * f + 2, 3) / 2;
        var v = f >= 1 ? target : from + (target - from) * eased;
        try {
            me.widget.setValue(v);
        }
        catch (ex) {
            me._animation = null;
            me._reportError("animate", ex.message);
            return;
        }
        if (f < 1) {
            me._animation = requestAnimationFrame(step);
        }
        else {
            me._animation = null;
            me._acknowledge(target, previous);
        }
    };
    this._animation = requestAnimationFrame(step);
};

this._cancelAnimation = function () {
    if (this._animation !== null && this._animation !== undefined) {
        cancelAnimationFrame(this._animation);
        this._animation = null;
    }
};

// Report "the browser now shows this value" as one contract event.
this._acknowledge = function (value, previous) {
    var me = this, data = { value: value, previous: previous };
    if (this._valueChangedHandler) {
        this._valueChangedHandler(data);          // the framework's deferred dispatcher
        return;
    }
    setTimeout(function () { me.fireWidgetEvent("valueChanged", data); }, 0);
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
    if (name === "valueChanged") this._valueChangedHandler = handler;
    if (name === "error") this._errorHandler = handler;
};

this._removeListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        if (this.widget) this.widget.off(vendorName, handler);
        return;
    }
    if (name === "valueChanged" && this._valueChangedHandler === handler) this._valueChangedHandler = null;
    if (name === "error" && this._errorHandler === handler) this._errorHandler = null;
};

// Translate the vendor payload into the contract payload: only meaningful data.
this._getEventData = function (type, e) {
    switch (type) {
        case "valueChanged":
            return { value: e.value, previous: e.previous };
        case "thresholdExceeded":
            return { value: e.value, threshold: e.threshold };
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

// Reachable from the wrapper with CallAsync("getRenderedSize") — never from a page.
this.getRenderedSize = function () {
    var r = this.host ? this.host.getBoundingClientRect() : { width: 0, height: 0 };
    return { width: Math.round(r.width), height: Math.round(r.height) };
};

//# sourceURL=integrationlab.controls.SimpleGauge.js
