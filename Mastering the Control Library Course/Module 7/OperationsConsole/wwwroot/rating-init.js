/* ============================================================================================
   rating-init.js — the InitScript of ratingWidget (Wisej.Web.Widget).
   OperationsConsole · Mastering the Control Library · Module 7

   This is the ADAPTER: the only file that knows both worlds. rating.js knows nothing about
   Wisej.NET, WidgetsPage.cs knows nothing about the DOM, and the contract between them lives
   here (and is written down in docs/WidgetContract.md).

   "this" in every function declared in this file is the Wisej.NET client widget
   (wisej.web.Widget). Two members of it matter:
       this.container            the DOM element the framework owns (layout, theme, disposal)
       this.fireWidgetEvent(n,d) delivers an event to the server → Widget.WidgetEvent

   It is delivered as an EMBEDDED RESOURCE (see OperationsConsole.csproj and
   Widgets/RatingInitScript.cs), never as a <script> in Default.html: a script in the startup
   HTML runs before the Wisej.NET widget exists, which is exactly the "initialisation timing"
   pitfall the module reading warns about.

   Packages, declared on the server and loaded once per page IN THIS ORDER before init runs:
       1. wwwroot/rating.css   → every colour and size (theme variants, .saved, .is-narrow)
       2. wwwroot/rating.js    → window.RatingWidget

   Contract:
       state in   (Options → init / update):  value, max, label, saved, theme, profile
       events out (WiredEvents):              ratingChanged { value }
       calls in   (Call / CallAsync):         setSaved(value), ratingClearSaved(),
                                              ratingSetBusy(busy), getState(), reinit(),
                                              sendRawPayload(value)

   Where the instance lives, for the browser console (docs/WidgetContract.md → Debugging):
       this.widget      the RatingWidget object — Wisej's server-side Instance proxy targets it
       this.host        the <div> created inside this.container that RatingWidget renders into
       app.getWidget("ratingWidget")      a tiny registry filled by init and cleared by dispose
   ============================================================================================ */

// A registry so the DevTools console can reach the adapter without hunting through qooxdoo.
// Wisej.NET has no app.getWidget of its own; init fills it, dispose clears it.
window.app = window.app || {};
app.widgets = app.widgets || {};
app.getWidget = app.getWidget || function (name) { return app.widgets[name] || null; };

// --------------------------------------------------------------------------------------------
// init — called once the packages are loaded, and AGAIN whenever the framework re-creates the
// client widget (browser refresh, the Widgets tab hidden and shown again, a layout change).
// Everything below is therefore idempotent: it tears down anything a previous run left behind
// before it builds. That is the "widget renders blank after the tab is re-shown" pitfall.
// --------------------------------------------------------------------------------------------
this.init = function (options) {

    // 1. Capture the widget. Inside the RatingWidget "change" callback "this" is the library's
    //    own object, NOT the Wisej.NET widget — "me" is the only reference that survives.
    var me = this;
    options = options || {};
    this._options = options;                       // kept so reinit() can replay the same state

    // 2. Idempotent teardown: a second init must not leave two libraries on two host elements.
    this._teardownRating();

    // 3. The library gets a CHILD element of its own. this.container stays the framework's
    //    element, so layout, visibility, theming and disposal keep working as usual.
    var host = document.createElement("div");
    host.className = "opc-rating-host";
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;

    // 4. Create the library object and keep it on the widget. this.widget is also what
    //    Wisej.NET's server-side Instance proxy targets, so ratingWidget.Instance.setValue(4)
    //    would reach the library later without touching this file.
    try {
        this.widget = new RatingWidget(host, {
            value: options.value,
            max: options.max,
            label: options.label,
            saved: options.saved,
            theme: options.theme || "bootstrap",
            narrow: options.profile === "Phone"
        });
    }
    catch (ex) {
        this.widget = null;
        if (window.console && console.error) console.error("ratingWidget: init failed —", ex);
        this.container.innerHTML = "<div class='opc-rating'><div class='opc-rating__caption'>" +
            "The rating widget could not be created. Check the browser console.</div></div>";
        return;
    }

    // 5. Library event → server event. The gesture is the user's (a click / a key), so firing
    //    synchronously is correct and reliable here. An event caused by a SERVER-side update()
    //    must be deferred instead — see _fireDeferred() and the cookbook gotcha.
    this._onRatingChange = function (e) {
        me.widget.setBusy(true);
        me.fireWidgetEvent("ratingChanged", { value: e.value });
    };
    this.widget.on("change", this._onRatingChange);

    // 6. Registry entry for the console.
    this._registryName = options.name || "ratingWidget";
    app.widgets[this._registryName] = this;

    // 7. Disposal: WRAP the framework's dispose, never replace it (replacing a qooxdoo method
    //    breaks the widget). Wrap once, even if init runs again.
    if (!this._disposeWrapped) {
        this._disposeWrapped = true;
        var frameworkDispose = this.dispose;
        this.dispose = function () {
            try { me._teardownRating(); }
            finally { if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments); }
        };
    }
};

// --------------------------------------------------------------------------------------------
// update — called by Wisej.NET when a FIRST-LEVEL field of Options changed on the server
// (ratingWidget.Options.value = 5; then ratingWidget.Update()).
// The library is updated SILENTLY, so a server-driven change never bounces back to the server
// as a ratingChanged event.
// --------------------------------------------------------------------------------------------
this.update = function (options, old) {
    if (!this.widget) return;
    options = options || {};
    this._options = options;

    try {
        this.widget.setOptions({
            value: options.value,
            label: options.label,
            saved: options.saved,
            theme: options.theme,
            narrow: options.profile === "Phone"
        });
    }
    catch (ex) {
        if (window.console && console.error) console.error("ratingWidget: update failed —", ex);
    }
};

// --------------------------------------------------------------------------------------------
// WiredEvents plumbing.
// Wisej.NET calls _addListener once per WiredEvents entry, after "loaded". This adapter raises
// its single contract event itself, straight from the user's gesture (step 5), so there is
// nothing to wire here; the two overrides only document that decision.
//
// The alternative shape — used by the Application Integration samples' TemperatureGauge — is to
// hand the framework handler to the library here and translate the payload in _getEventData:
//     this._addListener   = function (n, h) { if (n === "ratingChanged") this.widget.on("change", h); };
//     this._getEventData  = function (n, e) { return n === "ratingChanged" ? { value: e.value } : null; };
// Both reach WidgetEvent on the server. This file keeps the explicit fireWidgetEvent call
// because that is the line the module reading and the lab ask for, and because the adapter
// wants to flip the widget into its busy state in the same place.
// --------------------------------------------------------------------------------------------
this._addListener = function (name, handler) { /* events are fired directly — see init step 5 */ };
this._removeListener = function (name, handler) { };

// --------------------------------------------------------------------------------------------
// Functions the SERVER calls with Call(...) / CallAsync(...).
// None of them may be named like a qooxdoo method (getWidth, setValue, destroy, resize, show …):
// a function declared here with such a name REPLACES the framework's own and breaks the widget.
// --------------------------------------------------------------------------------------------

/** ratingWidget.Call("setSaved", value) — the server accepted and stored the value. */
this.setSaved = function (value) {
    if (this.widget) this.widget.setSaved(value);
};

/** ratingWidget.Call("ratingClearSaved") — the value was rejected or the save failed. */
this.ratingClearSaved = function () {
    if (this.widget) this.widget.clearSaved();
};

/** ratingWidget.Call("ratingSetBusy", false) — leave the busy state without confirming a value. */
this.ratingSetBusy = function (busy) {
    if (this.widget) this.widget.setBusy(busy);
};

/** await ratingWidget.CallAsync("getState") — returns the library's own state to the server. */
this.getState = function () {
    if (!this.widget) return { hasInstance: false };
    var s = this.widget.getState();
    s.hasInstance = true;
    return s;
};

/** ratingWidget.Call("reinit") — proves init is safe to run again (nothing is duplicated). */
this.reinit = function () {
    this.init(this._options || {});
};

/**
 * ratingWidget.Call("sendRawPayload", "seven")
 *
 * The teaching hook for "validate what comes from the browser". It fires the contract event
 * with a payload the widget itself would never produce — exactly what a user with developer
 * tools can do by typing fireWidgetEvent by hand. The server must reject it.
 *
 * It is DEFERRED with setTimeout(…, 0): this call arrives from the server, so firing
 * synchronously inside the same client-side response would drop the event.
 */
this.sendRawPayload = function (value) {
    this._fireDeferred("ratingChanged", { value: value });
};

this._fireDeferred = function (name, data) {
    var me = this;
    setTimeout(function () { me.fireWidgetEvent(name, data); }, 0);
};

/** Idempotent teardown shared by init() and dispose(). */
this._teardownRating = function () {
    try {
        if (this.widget) {
            if (this._onRatingChange) this.widget.off("change", this._onRatingChange);
            this.widget.destroy();
        }
    }
    catch (ex) { if (window.console && console.warn) console.warn("ratingWidget: teardown —", ex); }

    this.widget = null;
    this._onRatingChange = null;

    if (this.host && this.host.parentNode) this.host.parentNode.removeChild(this.host);
    this.host = null;

    if (this._registryName && app.widgets[this._registryName] === this) delete app.widgets[this._registryName];
};

// Names this injected script in the DevTools Sources panel so breakpoints can be set in it.
// Must stay the last line of the file.
//# sourceURL=rating-init.js
