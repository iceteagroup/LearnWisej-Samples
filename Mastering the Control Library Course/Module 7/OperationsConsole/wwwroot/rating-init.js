/* ============================================================================================
   rating-init.js — the InitScript of ratingWidget (Wisej.Web.Widget).

   The adapter between rating.js (knows nothing about Wisej.NET) and WidgetsPage.cs (knows nothing
   about the DOM). "this" is the Wisej.NET client widget:
       this.container            the DOM element the framework owns
       this.fireWidgetEvent(n,d) delivers an event to the server → Widget.WidgetEvent

   Delivered as an embedded resource (Widgets/RatingInitScript.cs), never as a <script> in
   Default.html: a script in the startup HTML runs before the widget exists.

   Packages, loaded in this order before init runs:
       1. wwwroot/rating.css   → every colour and size
       2. wwwroot/rating.js    → window.RatingWidget

   Contract (docs/WidgetContract.md):
       state in   (Options → init / update):  value, max, label, saved, theme, profile, name
       events out (WiredEvents):              ratingChanged { value }
       calls in   (Call / CallAsync):         setSaved(value), ratingClearSaved(), ratingSetBusy(busy)
   ============================================================================================ */

// A small registry so the browser console can reach the adapter: app.getWidget("ratingWidget").
window.app = window.app || {};
app.widgets = app.widgets || {};
app.getWidget = app.getWidget || function (name) { return app.widgets[name] || null; };

// init — runs when the packages are loaded and again whenever the framework re-creates the client
// widget (refresh, the tab hidden and shown again), so it tears down anything a previous run left.
this.init = function (options) {

    // Inside the RatingWidget "change" callback "this" is the library object; "me" is the widget.
    var me = this;
    options = options || {};

    this._teardownRating();

    // The library renders into a child element; this.container stays the framework's element.
    var host = document.createElement("div");
    host.className = "opc-rating-host";
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;

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

    // Library event → server event, from the user's gesture.
    this._onRatingChange = function (e) {
        me.widget.setBusy(true);
        me.fireWidgetEvent("ratingChanged", { value: e.value });
    };
    this.widget.on("change", this._onRatingChange);

    this._registryName = options.name || "ratingWidget";
    app.widgets[this._registryName] = this;

    // Wrap the framework's dispose, never replace it. Wrap once, even if init runs again.
    if (!this._disposeWrapped) {
        this._disposeWrapped = true;
        var frameworkDispose = this.dispose;
        this.dispose = function () {
            try { me._teardownRating(); }
            finally { if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments); }
        };
    }
};

// update — called when a first-level field of Options changed on the server and Update() ran.
// The library is updated silently, so a server-driven change never comes back as ratingChanged.
this.update = function (options, old) {
    if (!this.widget) return;
    options = options || {};

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

// ratingChanged is fired directly from the gesture in init, so there is nothing to wire here.
this._addListener = function (name, handler) { };
this._removeListener = function (name, handler) { };

// Functions the server calls. None may be named like a qooxdoo method (setValue, destroy, show …).

/** ratingWidget.CallAsync("setSaved", value) — the server stored the value. */
this.setSaved = function (value) {
    if (this.widget) this.widget.setSaved(value);
};

/** ratingWidget.Call("ratingClearSaved") — the save failed. */
this.ratingClearSaved = function () {
    if (this.widget) this.widget.clearSaved();
};

/** ratingWidget.Call("ratingSetBusy", false) — leave the busy state without confirming a value. */
this.ratingSetBusy = function (busy) {
    if (this.widget) this.widget.setBusy(busy);
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

//# sourceURL=rating-init.js
