// IntegrationLab — gauge-init.broken.js: the BROKEN InitScript, kept on purpose for the lab.
//
// Identical to gauge-init.js except for ONE line in step 4: the vendor callback calls
// this.fireWidgetEvent(...). Inside that callback "this" is the <input> element the plugin
// dispatched "knobchange" on - not the widget - so the call throws
//
//     TypeError: this.fireWidgetEvent is not a function
//
// and valueChanged never reaches .NET. The catch block reports the failure to the server as a
// "contextError" widget event (deferred with setTimeout, fired from the captured reference) so the
// bug is visible in the UI and the trace instead of only in the console.
//
// The fixed version is gauge-init.js. Compare the two files side by side: the diff is the lesson.

window.app = window.app || {};
app.widgets = app.widgets || {};
app.getWidget = app.getWidget || function (name) { return app.widgets[name] || null; };

this.init = function (options) {

    // "me" is captured here too, but ONLY to report the failure. The listener in step 4
    // deliberately ignores it - that is the bug this file demonstrates.
    var me = this;

    this.container.innerHTML = "<input class='knob'/>";
    var input = this.container.firstChild;
    input.placeholder = "raw <input> - plugin not applied";
    this.inputEl = input;

    try {
        $(input).vendorKnob({
            value: options.value, min: options.min, max: options.max, step: options.step,
            label: options.label, units: options.units, color: options.color
        });
    }
    catch (ex) {
        this.instance = this.widget = null;
        this._reportError("init", ex.message);
        return;
    }
    this.instance = $(input).data("vendorKnob");
    this.widget = this.instance;

    var registryName = options.name || (typeof this.getName === "function" ? this.getName() : null) || "widget";
    this._registryName = registryName;
    app.widgets[registryName] = this;

    // 4. THE BUG. Written as if the widget were still in scope. It compiles, it runs, and it
    //    fails at the first callback with "not a function", because "this" is the <input>.
    this._onKnobChange = function (e) {
        try {
            this.fireWidgetEvent("valueChanged", { value: e.detail.value });     // <-- wrong "this"
        }
        catch (ex) {
            // Prove what "this" was, then tell the server. Note the deferred fire from "me":
            // the only reference to the widget that survives into this callback.
            var thisWas = Object.prototype.toString.call(this).slice(8, -1);   // "HTMLInputElement"
            var value = e.detail.value;
            setTimeout(function () {
                me.fireWidgetEvent("contextError", { message: ex.message, thisWas: thisWas, value: value });
            }, 0);
        }
    };
    $(input).on("knobchange", this._onKnobChange);

    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () {
            if (me.instance) me.instance.resize();
        });
        this._resizeObserver.observe(this.container);
    }

    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            if (me._resizeObserver) { me._resizeObserver.disconnect(); me._resizeObserver = null; }
            if (me.inputEl) $(me.inputEl).off("knobchange", me._onKnobChange);
            if (me.instance) { me.instance.destroy(); me.instance = null; me.widget = null; }
            if (me.inputEl && me.inputEl.parentNode) me.inputEl.parentNode.removeChild(me.inputEl);
            me.inputEl = null;
            if (me._registryName && app.widgets[me._registryName] === me) delete app.widgets[me._registryName];
        }
        finally {
            if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments);
        }
    };
};

// update() is unaffected by the bug: it runs as a widget method, so "this" is the widget.
this.update = function (options, old) {
    if (!this.instance) return;

    var changed = {}, keys = ["value", "min", "max", "step", "label", "units", "color"];
    for (var i = 0; i < keys.length; i++) {
        var k = keys[i];
        if (options[k] === undefined) continue;
        if (old && old[k] === options[k]) continue;
        changed[k] = options[k];
    }

    try {
        if (changed.value !== undefined && Object.keys(changed).length === 1)
            this.instance.setValue(changed.value, true);
        else
            this.instance.setOptions(changed);
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
};

this._addListener = function (name, handler) { };
this._removeListener = function (name, handler) { };

this._reportError = function (phase, message) {
    var me = this;
    setTimeout(function () { me.fireWidgetEvent("error", { phase: phase, message: message }); }, 0);
};

this.pulse = function () { if (this.instance) this.instance.pulse(); };
this.getState = function () {
    return { value: this.instance ? this.instance.getValue() : null, hasInstance: !!this.instance };
};

//# sourceURL=gauge-init.broken.js
