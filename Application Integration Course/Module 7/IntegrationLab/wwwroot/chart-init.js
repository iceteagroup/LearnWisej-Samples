// IntegrationLab.ChartWidget — client adapter around VendorChart (Module 7).
//
// Client event handler #3 — pointClicked
//   subscribed : EVERY vendor event (hover, zoom, render, layout, legendclick, pointclick)
//   forwarded  : pointClicked { index: number, label: string, value: number }  — only this one
//   kept local : hover / zoom / render / layout / legendclick — counted, never sent
//                (getNoiseCount() lets the server show the proof)
//
// The vendor cannot change "theme" in place: update() destroys the instance and
// creates a new one. All vendor callbacks are attached in ONE function, _wire(),
// called from init AND from that recreate path — attaching in init only would
// leave the second instance silent.

this._noise = { hover: 0, zoom: 0, render: 0, layout: 0, legendclick: 0 };
this._forwarded = 0;
this._generation = 0;                                // how many vendor instances this wrapper has created

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
    this._generation++;
    this._wire();
};

this._destroyVendor = function () {
    this._unwire();                                  // every attach has a detach
    if (this.widget) { this.widget.destroy(); this.widget = null; }
};

// subscribe to everything, forward only what is a business decision for the app
this._wire = function () {
    var me = this;                                   // keep the widget context for the vendor callbacks
    this._vendorHandlers = {
        hover:       function () { me._noise.hover++; },          // stays in the browser
        zoom:        function () { me._noise.zoom++; },           // stays in the browser
        render:      function () { me._noise.render++; },         // stays in the browser
        layout:      function () { me._noise.layout++; },         // stays in the browser
        legendclick: function () { me._noise.legendclick++; },    // stays in the browser
        pointclick:  function (e) {                               // user drill-down intent → the server
            me._forwarded++;
            // compact DTO: three primitives, not the vendor event (which carries x, y, domEvent, seriesIndex)
            me.fireWidgetEvent("pointClicked", { index: e.index, label: e.label, value: e.value });
        }
    };
    for (var name in this._vendorHandlers) this.widget.on(name, this._vendorHandlers[name]);
};

this._unwire = function () {
    if (this.widget && this._vendorHandlers)
        for (var name in this._vendorHandlers) this.widget.off(name, this._vendorHandlers[name]);
    this._vendorHandlers = null;
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

// ---- functions the server reaches with Call / CallAsync -----------------------

// CallAsync("getNoiseCount") → proof of filtering: what stayed in the browser vs what was forwarded
this.getNoiseCount = function () {
    var n = this._noise;
    return {
        hover: n.hover, zoom: n.zoom, render: n.render, layout: n.layout, legendclick: n.legendclick,
        total: n.hover + n.zoom + n.render + n.layout + n.legendclick,
        forwarded: this._forwarded,
        generation: this._generation
    };
};

// Call("fireBadPayload") → the failure path: a payload that violates the contract
// (index out of range, label missing). The server must reject it. Deferred because
// a Call runs while the server response is being applied.
this.fireBadPayload = function () {
    var me = this;
    setTimeout(function () { me.fireWidgetEvent("pointClicked", { index: -1 }); }, 0);
};

//# sourceURL=integrationlab.widgets.ChartWidget.js
