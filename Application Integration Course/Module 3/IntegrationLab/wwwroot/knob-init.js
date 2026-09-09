// IntegrationLab — knob-init.js
// InitScript of the "knob" Wisej.Web.Widget on DashboardPage (no wrapper class: a plain Widget).
//
// The knob is a jQuery-style plugin ($.fn.vendorKnob) that enhances an <input> in place —
// the "library that wants a specific element" case. Packages, in order:
//   1. wwwroot/jquery-lite.js   (vendor-knob.js throws "jQuery is not defined" if loaded first)
//   2. wwwroot/knob.css
//   3. wwwroot/vendor-knob.js
//
// Options contract (server → client, camel-cased by the Wisej.NET serializer):
//   level   number    current position — top level because it changes often (vendor name: value)
//   min, max, step    range
//   label, units      caption drawn by the vendor
//   color             arc color
//
// Events out (WiredEvents): valueChanged {value}   — user drag / wheel only
//                           error        {phase, message}
// Server → client calls (Call): pulse()

this._events = {};

// ---------------------------------------------------------------------------------------------
// init(options): runs once. Creates the element the plugin wants, applies the plugin,
// stores the instance somewhere predictable (this.instance / this.widget).
// ---------------------------------------------------------------------------------------------
this.init = function (options) {
    var me = this;
    options = options || {};

    this.container.innerHTML = "<div class='knob-host'></div>";
    var host = this.container.firstChild;
    var input = document.createElement("input");
    input.type = "text";
    input.className = "knob-input";
    input.setAttribute("aria-label", options.label || "knob");
    host.appendChild(input);
    this.host = host;
    this.input = input;

    try {
        $(input).vendorKnob(this._toVendorOptions(options));
        this.instance = $(input).data("vendorKnob");   // the vendor object, as the walkthrough names it
        this.widget = this.instance;                    // Wisej.NET convention: Instance.xxx() targets this.widget
    }
    catch (ex) {
        this.instance = this.widget = null;
        this._reportError("init", ex.message);
        return;
    }

    // vendor event → contract event. User-driven (drag / wheel), so a synchronous fire is fine;
    // server-driven changes go through update() with silent=true and never come back here.
    $(input).on("knobchange", function (e) {
        me.fireWidgetEvent("valueChanged", { value: e.detail.value });
    });

    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () { if (me.instance) me.instance.resize(); });
        this._resizeObserver.observe(host);
    }

    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            if (me._resizeObserver) { me._resizeObserver.disconnect(); me._resizeObserver = null; }
            if (me.input) $(me.input).off("knobchange");
            if (me.instance) { me.instance.destroy(); me.instance = me.widget = null; }
            if (me.host && me.host.parentNode) me.host.parentNode.removeChild(me.host);
            me.host = null; me.input = null;
        }
        finally {
            if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments);
        }
    };
};

// ---------------------------------------------------------------------------------------------
// update(options, old): every first-level Options change on the server. Only changed fields
// reach the vendor; the value is set silently so a server change does not echo valueChanged.
// ---------------------------------------------------------------------------------------------
this.update = function (options, old) {
    if (!this.instance) return;
    options = options || {};
    old = old || {};

    try {
        var changes = {};
        ["min", "max", "step", "label", "units", "color"].forEach(function (k) {
            if (options[k] !== old[k]) changes[k] = options[k];
        });
        if (Object.keys(changes).length)
            this.instance.setOptions(changes);

        if (options.level !== old.level)
            this.instance.setValue(options.level, /* silent */ true);
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
};

// reached from the server with knob.Call("pulse")
this.pulse = function () {
    if (this.widget) this.widget.pulse();
};

// ---------------------------------------------------------------------------------------------
// events plumbing (framework registers one handler per WiredEvents entry)
// ---------------------------------------------------------------------------------------------
this._addListener = function (name, handler) { this._events[name] = handler; };
this._removeListener = function (name, handler) { if (this._events[name] === handler) delete this._events[name]; };
this._getEventData = function (type, e) { return e; };

this._reportError = function (phase, message) {
    var me = this, data = { phase: phase, message: message };
    if (this._events.error) { this._events.error(data); return; }
    setTimeout(function () { me.fireWidgetEvent("error", data); }, 0);
};

// contract → vendor option names (level → value)
this._toVendorOptions = function (options) {
    return {
        value: options.level,
        min: options.min, max: options.max, step: options.step,
        label: options.label, units: options.units, color: options.color
    };
};

//# sourceURL=knob-init.js
