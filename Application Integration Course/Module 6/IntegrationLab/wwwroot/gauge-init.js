// IntegrationLab.Controls.SimpleGauge — client adapter around VendorGauge (Module 6: commands).
//
// "this" is the Wisej.NET widget wrapper (wisej.web.Widget); this.container is the DOM
// element the framework positions, sizes, shows, hides, themes and disposes.
//
// Contract (see docs/ServerToClientCalls.md and docs/DtoContract.md):
//   state in    (Options → init/update):  value, min, max, warnAt, threshold, label, units
//   commands in (C# Call / CallAsync / EvalAsync → functions on THIS wrapper):
//       setValue(v)          Call         sweeps the needle to v, returns nothing
//       resetAnimation()     Call         cancels the sweep, restarts the settle animation
//       getRenderedSize()    CallAsync    → { width, height }                      (RenderedSize)
//       getSelectedState()   CallAsync    → { value, isAboveThreshold, width, height, isAnimating }  (GaugeStateDto)
//       measureWidth()       EvalAsync    "this.measureWidth()" → number
//   events out  (WiredEvents):            thresholdExceeded {value}
//                                         rangeChanged      {range, value}
//                                         error             {phase, message}
//                                         leakDetected      {bytes, keys, sample}
//
// Which object does a server call reach?
//   gauge.Call("setValue", 72)      → this.setValue(72)         the WRAPPER function below (this = wrapper)
//   gauge.Instance.setValue(72)     → this.widget.setValue(72)  the VENDOR method, bypassing the wrapper
//   gauge.EvalAsync("this.measureWidth()")  → evaluated with this = wrapper
// The wrapper functions are the documented contract; Instance.* is a shortcut that skips them.
//
// Rules this adapter follows:
//   1. The vendor object lives in a child element (this.host), never on this.container.
//   2. The vendor instance is stored in this.widget.
//   3. Events go through _addListener / _getEventData (the framework defers the round trip).
//   4. Nothing here looks up other components: no App.MainPage, no widget(id), no parent walking.
//   5. Return values are small plain objects (JSON), never this.widget, this.host or a DOM node.

this._vendorEvents = {
    thresholdExceeded: "thresholdexceeded",
    rangeChanged: "rangechange"
};

this.SWEEP_MS = 700;        // needle sweep length; long enough for "Get selected state" during a stream to catch isAnimating: true

this.init = function (options) {
    var me = this;

    this._ensureStyles();

    var host = document.createElement("div");
    host.className = "simple-gauge-host";
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;

    this._threshold = options.threshold;
    this._targetValue = options.value;
    this._raf = null;
    this._sweeping = false;
    this._settling = false;

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

    // End of the CSS settle animation started by resetAnimation().
    this._onSettleEnd = function () {
        host.classList.remove("settle");
        me._settling = false;
    };
    host.addEventListener("animationend", this._onSettleEnd);

    // Disposal: stop the sweep, destroy the vendor object, clear the host, then let the
    // framework dispose the wrapper as usual.
    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            if (me._raf) { cancelAnimationFrame(me._raf); me._raf = null; }
            if (me._resizeObserver) { me._resizeObserver.disconnect(); me._resizeObserver = null; }
            if (me.widget) { me.widget.destroy(); me.widget = null; }
            if (me.host) {
                me.host.removeEventListener("animationend", me._onSettleEnd);
                if (me.host.parentNode) me.host.parentNode.removeChild(me.host);
            }
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
        // Serialization discipline check: a domain object leaked into the options.
        // Report what arrived; never render it.
        this._checkForLeak(options);

        if (options.threshold !== undefined) this._threshold = options.threshold;

        this.widget.setOptions({
            min: options.min, max: options.max, warnAt: options.warnAt,
            threshold: options.threshold, label: options.label, units: options.units
        });

        // A value change through Options is animated like the setValue command. The two are
        // deduped by target, so "gauge.Value = 72" followed by Call("setValue", 72) in one response
        // produces one sweep, whichever the browser applies first.
        if (typeof options.value !== "undefined" && options.value !== this._targetValue)
            this._animateTo(options.value);
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
};

// ---- commands the server invokes with Call / CallAsync ----------------------------------

// Call("setValue", v): one-way. Nothing is returned; the server has already moved on.
this.setValue = function (v) {
    if (!this.widget) return;
    try { this._animateTo(v); }
    catch (ex) { this._reportError("setValue", ex.message); }
};

// Call("resetAnimation"): one-way. Cancels a running sweep (landing on the target value) and
// restarts the CSS settle animation on the host. Purely visual, purely client-side.
this.resetAnimation = function () {
    var host = this.host;
    if (!host) return;
    this._cancelSweep();
    host.classList.remove("settle");
    void host.offsetWidth;                       // force a reflow so the animation restarts
    this._settling = true;
    host.classList.add("settle");
};

// CallAsync("getRenderedSize"): returns a small plain object → RenderedSize on the server.
this.getRenderedSize = function () {
    var r = this.host ? this.host.getBoundingClientRect() : { width: 0, height: 0 };
    return { width: Math.round(r.width), height: Math.round(r.height) };
};

// CallAsync("getSelectedState"): the selected client-side state → GaugeStateDto on the server.
// Only what the server asked for, camelCase, primitives. Never this.widget or this.host.
this.getSelectedState = function () {
    var value = this.widget ? this.widget.getValue() : null;
    var size = this.getRenderedSize();
    return {
        value: value,
        isAboveThreshold: value !== null && typeof this._threshold === "number" && value >= this._threshold,
        width: size.width,
        height: size.height,
        isAnimating: !!(this._sweeping || this._settling)
    };
};

// EvalAsync("this.measureWidth()"): a primitive back from an expression.
this.measureWidth = function () {
    return this.host ? this.host.getBoundingClientRect().width : 0;
};

// ---- needle sweep (client-only visual state) ----------------------------------------------

this._animateTo = function (target) {
    var me = this, w = this.widget;
    if (typeof target !== "number" || !isFinite(target))
        throw new Error("SimpleGauge.setValue: value must be a finite number, received " + JSON.stringify(target) + ".");

    this._cancelSweep();                         // lands any interrupted sweep on its own target first
    var from = w.getValue();
    if (from === undefined) from = target;
    this._targetValue = target;
    if (from === target) return;

    var start = performance.now(), dur = this.SWEEP_MS;
    this._sweepFrom = from;
    this._sweeping = true;

    function ease(p) { return 1 - Math.pow(1 - p, 3); }
    function frame(now) {
        var p = Math.min(1, (now - start) / dur);
        if (p < 1) {
            w.setValue(from + (target - from) * ease(p), true);     // silent: no vendor events per frame
            me._raf = requestAnimationFrame(frame);
            return;
        }
        me._raf = null;
        me._land();
    }
    this._raf = requestAnimationFrame(frame);
};

// Finish a sweep: restore the pre-sweep vendor state silently, then apply the target with
// events on, so the vendor's rising-edge / range detection sees exactly one transition from → target.
this._land = function () {
    if (!this._sweeping || !this.widget) return;
    this._sweeping = false;
    this.widget.setValue(this._sweepFrom, true);
    this.widget.setValue(this._targetValue);
};

this._cancelSweep = function () {
    if (this._raf) { cancelAnimationFrame(this._raf); this._raf = null; }
    this._land();
};

// ---- serialization discipline: detect a leaked domain object in the options ---------------

this._checkForLeak = function (options) {
    if (!("debugDump" in options)) return;
    if (options.debugDump === null || options.debugDump === undefined) { this._lastLeakJson = null; return; }

    var dump = options.debugDump, json;
    try { json = JSON.stringify(dump); } catch (ex) { json = "[unserializable]"; }
    if (json === this._lastLeakJson) return;      // already reported this exact payload
    this._lastLeakJson = json;

    var keys = [], k;
    try {
        for (k in dump) keys.push(k);
        if (dump.customer) for (k in dump.customer) keys.push("customer." + k);
    } catch (ex) { /* ignore */ }

    var sensitive = keys.filter(function (n) { return /taxId|creditLimit|internalRemarks|email|approvedBy/i.test(n); });
    var data = { bytes: json.length, keys: keys.length, sample: sensitive.slice(0, 5).join(", ") };
    if (this._leakHandler) { this._leakHandler(data); return; }
    var me = this;
    setTimeout(function () { me.fireWidgetEvent("leakDetected", data); }, 0);
};

// ---- events -------------------------------------------------------------------------------

// Wisej.NET calls this once for every name in WiredEvents (after "loaded").
// "handler" is the framework's deferred dispatcher; it calls _getEventData and then
// fireWidgetEvent on the next tick.
this._addListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        if (this.widget) this.widget.on(vendorName, handler);
        return;
    }
    if (name === "error") this._errorHandler = handler;          // raised by the adapter itself
    if (name === "leakDetected") this._leakHandler = handler;    // raised by the adapter itself
};

this._removeListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        if (this.widget) this.widget.off(vendorName, handler);
        return;
    }
    if (name === "error" && this._errorHandler === handler) this._errorHandler = null;
    if (name === "leakDetected" && this._leakHandler === handler) this._leakHandler = null;
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
        case "leakDetected":
            return { bytes: e.bytes, keys: e.keys, sample: e.sample };
    }
    return null;
};

// Report a vendor failure as one contract event.
this._reportError = function (phase, message) {
    var me = this, data = { phase: phase, message: message };
    if (this._errorHandler) {
        this._errorHandler(data);
        return;
    }
    setTimeout(function () { me.fireWidgetEvent("error", data); }, 0);
};

// One <style> per page for the settle animation used by resetAnimation().
this._ensureStyles = function () {
    if (document.getElementById("simple-gauge-styles")) return;
    var style = document.createElement("style");
    style.id = "simple-gauge-styles";
    style.textContent =
        "@keyframes simple-gauge-settle{0%{transform:scale(.94);opacity:.35}60%{transform:scale(1.02);opacity:1}100%{transform:none}}" +
        ".simple-gauge-host.settle{animation:simple-gauge-settle 600ms ease-out;transform-origin:50% 70%;}";
    document.head.appendChild(style);
};

//# sourceURL=integrationlab.controls.SimpleGauge.js
