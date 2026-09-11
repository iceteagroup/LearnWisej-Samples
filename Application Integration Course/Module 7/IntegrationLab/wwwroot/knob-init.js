// IntegrationLab.KnobWidget — client adapter around the jQuery-style VendorKnob plugin (Module 7).
//
// Client event handler #2 — valueChanged
//   subscribed : ONE vendor callback, the "knobchange" DOM event the plugin
//                dispatches on its <input> (e.detail.value)
//   forwarded  : valueChanged { value: number, source: "user" | "server" }
//   kept local : pointer capture, wheel steps, the dial redraw, pulse()
//
// Two ways the same vendor callback can be reached, two ways to forward:
//   user drags the dial  → fireWidgetEvent(...) directly (user-driven events are
//                          never dropped, synchronous is fine — the video's code)
//   server sets Options.value → the vendor fires knobchange INSIDE update(); a
//                          synchronous fireWidgetEvent there is dropped, so the
//                          event goes through the _addListener handler with
//                          source: "server".

this.init = function (options) {
    var me = this;

    // The plugin wants an <input> to enhance in place: give it one inside our container.
    this.container.innerHTML = "";
    var input = document.createElement("input");
    input.type = "text";
    input.className = "knob";
    this.container.appendChild(input);
    this.input = input;

    try {
        $(input).vendorKnob({
            value: options.value, min: options.min, max: options.max, step: options.step,
            label: options.label, units: options.units, color: options.color
        });
        this.widget = $(input).data("vendorKnob");
    }
    catch (ex) {
        this.widget = null;
        this._reportError("init", ex.message);
        return;
    }

    this._wire();       // the single place the vendor callback is attached

    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () { if (me.widget) me.widget.resize(); });
        this._resizeObserver.observe(this.container);
    }

    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            if (me._resizeObserver) { me._resizeObserver.disconnect(); me._resizeObserver = null; }
            me._unwire();
            if (me.widget) { me.widget.destroy(); me.widget = null; }
            if (me.input && me.input.parentNode) me.input.parentNode.removeChild(me.input);
            me.input = null;
        }
        finally {
            if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments);
        }
    };
};

// wire ONE vendor callback
this._wire = function () {
    var me = this;                                   // vendor "this" is the <input>; keep the widget
    this._onKnobChange = function (e) {
        var value = e.detail.value;
        if (me._applying) {
            // caused by a server Options change (we are inside update()): deferred path
            me._raise("valueChanged", { value: value, source: "server" });
            return;
        }
        // user gesture: forward a compact payload, never the DOM event
        me.fireWidgetEvent("valueChanged", { value: value, source: "user" });
    };
    $(this.input).on("knobchange", this._onKnobChange);
};

this._unwire = function () {
    if (this.input && this._onKnobChange) $(this.input).off("knobchange", this._onKnobChange);
    this._onKnobChange = null;
};

this.update = function (options, old) {
    if (!this.widget) return;
    this._applying = true;                           // mark: vendor callbacks now come from the server
    try {
        this.widget.setOptions({ min: options.min, max: options.max, step: options.step, label: options.label, units: options.units, color: options.color });
        if (typeof options.value === "number")
            this.widget.setValue(options.value);     // emits knobchange only when the value really changes
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
    finally {
        this._applying = false;
    }
};

this._raise = function (name, data) {
    var handler = this._handlers && this._handlers[name];
    if (handler) { handler(data); return; }
    var me = this;
    setTimeout(function () { me.fireWidgetEvent(name, data); }, 0);
};

this._addListener = function (name, handler) {
    (this._handlers = this._handlers || {})[name] = handler;
};
this._removeListener = function (name, handler) {
    if (this._handlers && this._handlers[name] === handler) delete this._handlers[name];
};
this._getEventData = function (type, e) {
    switch (type) {
        case "valueChanged": return { value: e.value, source: e.source };
        case "error": return { phase: e.phase, message: e.message };
    }
    return null;
};
this._reportError = function (phase, message) {
    this._raise("error", { phase: phase, message: message });
};

// function the server reaches with Call("pulse")
this.pulse = function () { if (this.widget) this.widget.pulse(); };

//# sourceURL=integrationlab.widgets.KnobWidget.js
