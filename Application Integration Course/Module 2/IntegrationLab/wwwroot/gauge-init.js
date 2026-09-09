// IntegrationLab — gauge-init.js: the CONTEXT-SAFE InitScript for the Pressure knob.
//
// "this" inside every function declared in this file is the Wisej.NET client widget
// (wisej.web.Widget). this.container is the DOM element the framework owns, and
// this.fireWidgetEvent(name, data) delivers an event to the server (Widget.WidgetEvent).
//
// Packages (declared on the server, loaded once per page, in THIS order, before init runs):
//   1. wwwroot/jquery-lite.js   -> window.$ / window.jQuery
//   2. wwwroot/vendor-knob.css  -> styles for .vendor-knob and input.knob
//   3. wwwroot/vendor-knob.js   -> $.fn.vendorKnob   (throws "jQuery is not defined" if 1 is missing)
//
// Contract:
//   state in  (Options -> init / update): name, value, min, max, step, label, units, color
//   events out (WiredEvents):             valueChanged { value }          the user turned the knob
//                                         error        { phase, message } the adapter caught a vendor failure
//
// Where the vendor instance is stored (docs/DebuggingNotes.md):
//   this.instance          -> the VendorKnob object      (DevTools: app.getWidget("gaugeKnob").instance)
//   this.widget            -> the same object; Wisej.NET's server-side Instance proxy targets this.widget,
//                             so this.gaugeKnob.Instance.pulse() reaches the vendor later without changes here
//   $(input).data("vendorKnob") -> where the plugin itself keeps it (the vendor's own convention)

// A tiny registry so the DevTools console can do what the walkthrough shows:
//     app.getWidget("gaugeKnob").instance
// Wisej.NET has no app.getWidget of its own; the registry is filled by init and cleared by dispose.
window.app = window.app || {};
app.widgets = app.widgets || {};
app.getWidget = app.getWidget || function (name) { return app.widgets[name] || null; };

this.init = function (options) {

    // 1. Capture the widget. Every vendor callback below runs with a different "this"
    //    (the <input> that dispatched the event), so only this reference survives into them.
    var me = this;

    // debugger;   // Uncomment, open DevTools and reload: execution pauses here with
                   //   this    = the Wisej.NET widget      options = the server Options as JSON
                   //   this.container = the host element   (after step 3) this.instance = VendorKnob
                   // Leave it commented in committed code: with DevTools closed it is a no-op,
                   // with DevTools open it stops every page load.

    // 2. The vendor wants an <input> to enhance ("the library that needs a specific element").
    //    Create it INSIDE this.container: the container stays the framework's element (layout,
    //    visibility, theme, disposal), the vendor gets a child of its own. Nothing else on the
    //    page is touched.
    this.container.innerHTML = "<input class='knob'/>";
    var input = this.container.firstChild;
    input.placeholder = "raw <input> - plugin not applied";   // visible only if the plugin never runs
    this.inputEl = input;

    // 3. Apply the plugin and keep the vendor instance ON THE WIDGET so update() and dispose()
    //    can find it later, and so DevTools can confirm it exists.
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

    // 4. Vendor event -> widget event. The plugin dispatches "knobchange" on the <input>, so inside
    //    this handler "this" is the <input> element, NOT the widget. "me" is the widget.
    //    The event is caused by the user (drag / wheel), so firing synchronously is fine.
    //    (An event caused by a server-side update() must be deferred - see the cookbook gotcha.)
    this._onKnobChange = function (e) {
        me.fireWidgetEvent("valueChanged", { value: e.detail.value });
    };
    $(input).on("knobchange", this._onKnobChange);

    // 5. Resize: forward to the vendor.
    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () {
            if (me.instance) me.instance.resize();
        });
        this._resizeObserver.observe(this.container);
    }

    // 6. Disposal: wrap (never replace) the framework dispose. Unhook the vendor event, destroy
    //    the vendor object, remove our child element, forget the registry entry, then let the
    //    framework dispose the wrapper as usual.
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

// Called by Wisej.NET when a first-level Options field changes on the server.
// Re-sync only what changed: the vendor's setOptions applies a value silently (no "knobchange"),
// so a server-driven change never bounces back to the server as a valueChanged event.
this.update = function (options, old) {
    if (!this.instance) return;

    var changed = {}, keys = ["value", "min", "max", "step", "label", "units", "color"];
    for (var i = 0; i < keys.length; i++) {
        var k = keys[i];
        if (options[k] === undefined) continue;                 // not in this update
        if (old && old[k] === options[k]) continue;              // unchanged
        changed[k] = options[k];
    }

    try {
        if (changed.value !== undefined && Object.keys(changed).length === 1)
            this.instance.setValue(changed.value, true);         // the common case: only the value moved
        else
            this.instance.setOptions(changed);
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
};

// Wisej.NET calls these once per WiredEvents entry (after "loaded"). This adapter fires its
// events itself with fireWidgetEvent, straight from the vendor callback, so there is nothing
// to wire here; the overrides only document that decision.
this._addListener = function (name, handler) { /* events are fired directly - see step 4 */ };
this._removeListener = function (name, handler) { };

// One contract event for every vendor failure the adapter catches. Deferred with setTimeout so
// it is delivered even when the failure happened inside update() (the cookbook gotcha).
this._reportError = function (phase, message) {
    var me = this;
    setTimeout(function () { me.fireWidgetEvent("error", { phase: phase, message: message }); }, 0);
};

// Functions the server can reach with Call("pulse") / CallAsync("getState").
this.pulse = function () { if (this.instance) this.instance.pulse(); };
this.getState = function () {
    return { value: this.instance ? this.instance.getValue() : null, hasInstance: !!this.instance };
};

// Names this injected script in the DevTools Sources panel so breakpoints can be set in it.
// Must stay the last line of the file.
//# sourceURL=gauge-init.js
