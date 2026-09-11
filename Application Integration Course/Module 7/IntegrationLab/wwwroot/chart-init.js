// IntegrationLab.ChartWidget — client adapter around VendorChart (Module 7).
//
// Client event handler #3 — pointClicked
//   subscribed : ONE vendor callback, "pointclick"
//   forwarded  : pointClicked { index: number, label: string, value: number }
//   kept local : hover / zoom / render / layout / legendclick — never subscribed, never sent
//
// The vendor cannot change "theme" in place: update() destroys the instance and
// creates a new one. All vendor callbacks are attached in ONE function, _wire(),
// called from init AND from that recreate path — attaching in init only would
// leave the second instance silent.

this.init = function (options) {
    var me = this;

    var host = document.createElement("div");
    host.className = "chart-host";
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;

    try {
        this._createVendor(options);
    }
    catch (ex) {
        this.widget = null;
        this._reportError("init", ex.message);
        return;
    }

    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () { if (me.widget) me.widget.resize(); });
        this._resizeObserver.observe(host);
    }

    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            if (me._resizeObserver) { me._resizeObserver.disconnect(); me._resizeObserver = null; }
            me._destroyVendor();
            if (me.host && me.host.parentNode) me.host.parentNode.removeChild(me.host);
            me.host = null;
        }
        finally {
            if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments);
        }
    };
};

// create the vendor instance AND wire it — the only place both happen
this._createVendor = function (options) {
    this.widget = new VendorChart(this.host, { series: options.series, labels: options.labels, theme: options.theme });
    this._theme = options.theme;
    this._wire();
};

this._destroyVendor = function () {
    this._unwire();                                  // every attach has a detach
    if (this.widget) { this.widget.destroy(); this.widget = null; }
};

// wire ONE vendor callback: the user's drill-down intent
this._wire = function () {
    var me = this;                                   // keep the widget context for the vendor callback
    this._onPointClick = function (e) {
        // compact DTO: three primitives, not the vendor event (which carries x, y, domEvent, seriesIndex)
        me.fireWidgetEvent("pointClicked", { index: e.index, label: e.label, value: e.value });
    };
    this.widget.on("pointclick", this._onPointClick);
};

this._unwire = function () {
    if (this.widget && this._onPointClick) this.widget.off("pointclick", this._onPointClick);
    this._onPointClick = null;
};

this.update = function (options, old) {
    if (!this.widget) return;
    try {
        if (options.theme !== this._theme) {
            // the vendor throws on setOptions({theme}): destroy, recreate, RE-WIRE
            this._destroyVendor();
            this._createVendor(options);
            return;
        }
        this.widget.setData({ series: options.series, labels: options.labels });
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
};

// pointClicked is fired directly by the vendor callback above (user-driven, never
// inside update()), so there is nothing to attach here; "error" uses the deferred handler.
this._addListener = function (name, handler) {
    if (name === "error") this._errorHandler = handler;
};
this._removeListener = function (name, handler) {
    if (name === "error" && this._errorHandler === handler) this._errorHandler = null;
};
this._getEventData = function (type, e) {
    switch (type) {
        case "pointClicked": return { index: e.index, label: e.label, value: e.value };
        case "error": return { phase: e.phase, message: e.message };
    }
    return null;
};
this._reportError = function (phase, message) {
    var me = this, data = { phase: phase, message: message };
    if (this._errorHandler) { this._errorHandler(data); return; }
    setTimeout(function () { me.fireWidgetEvent("error", data); }, 0);
};

//# sourceURL=integrationlab.widgets.ChartWidget.js
