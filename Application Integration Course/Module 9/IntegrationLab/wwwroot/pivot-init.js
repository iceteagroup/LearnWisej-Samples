// IntegrationLab.WorkOrderPivot — client adapter around VendorPivot (DevExtreme-style CustomStore).
//
// "this" is the Wisej.NET widget wrapper (wisej.web.Widget); this.container is the DOM
// element the framework positions, sizes, shows, hides, themes and disposes.
//
// Contract (see docs/PivotIntegrationPlan.md):
//   state in  (Options → init/update): rowField, columnField, measure   (a plain JSON option object)
//   data      (CustomStore.load → [WebMethod] MainPage.LoadPivot(rowField, columnField, measure))
//             → { status, rowField, columnField, measure, rowKeys, columnKeys, cells: [{ row, column, value }] }
//   events out (WiredEvents):           cellClick  { rowKey, columnKey, value }
//                                       dataLoaded { rows, columns, cells }
//                                       error      { phase, status, message }
//
// This is the ONLY file that knows what a DevExtreme CustomStore looks like.

this._vendorEvents = {
    cellClick: "cellclick",
    dataLoaded: "dataloaded"
};

this.init = function (options) {
    var me = this;

    var host = document.createElement("div");
    host.className = "work-order-pivot-host";
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;

    // DevExtreme-style CustomStore: a key and a load(loadOptions) function returning a Promise.
    var store = {
        key: options.rowField,
        load: function (loadOptions) {
            // -------------------------------------------------------------------------
            // WEBMETHOD CALL (implemented per the cookbook + the client core; runtime not
            // yet verified in the browser). Wisej registers every [WebMethod] of the main
            // page on App.MainPage twice: "LoadPivot(args..., callback)" (callback style)
            // and "LoadPivotAsync(args...)" which returns a Promise. Default parameter
            // values are not supported, so all three arguments are always passed.
            //
            // Callback-style alternative:
            //   return new Promise(function (resolve, reject) {
            //       App.MainPage.LoadPivot(loadOptions.rowField, loadOptions.columnField, loadOptions.measure,
            //           function (result) { resolve(result); });
            //   });
            // -------------------------------------------------------------------------
            var page = window.App && window.App.MainPage;
            if (!page || typeof page.LoadPivotAsync !== "function")
                return Promise.reject(new Error("App.MainPage.LoadPivotAsync is not available (is LoadPivot a [WebMethod] on the main page?)."));
            return page.LoadPivotAsync(loadOptions.rowField, loadOptions.columnField, loadOptions.measure);
        }
    };

    try {
        this.widget = new VendorPivot(host, {
            store: store,
            rowField: options.rowField,
            columnField: options.columnField,
            measure: options.measure
        });
    }
    catch (ex) {
        this.widget = null;
        this._reportError("init", 0, ex.message);
        return;
    }

    this.widget.on("error", function (e) { me._reportError(e.operation || "vendor", e.status || 0, e.message); });

    if (typeof ResizeObserver !== "undefined") {
        this._resizeObserver = new ResizeObserver(function () { if (me.widget) me.widget.resize(); });
        this._resizeObserver.observe(host);
    }

    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
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

// The page replaced the whole Options object (JSON options prototype): re-sync and the
// vendor calls store.load() again with the new axes.
this.update = function (options, old) {
    if (!this.widget) return;
    try {
        this.widget.setOptions({ rowField: options.rowField, columnField: options.columnField, measure: options.measure });
    }
    catch (ex) {
        this._reportError("update", 0, ex.message);
    }
};

this._addListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        if (this.widget) this.widget.on(vendorName, handler);
        return;
    }
    if (name === "error") this._errorHandler = handler;
};

this._removeListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        if (this.widget) this.widget.off(vendorName, handler);
        return;
    }
    if (name === "error" && this._errorHandler === handler) this._errorHandler = null;
};

this._getEventData = function (type, e) {
    switch (type) {
        case "cellClick":
            return { rowKey: e.rowKey, columnKey: e.columnKey, value: e.value };
        case "dataLoaded":
            return { rows: e.rows, columns: e.columns, cells: e.cells };
        case "error":
            return { phase: e.phase, status: e.status, message: e.message };
    }
    return null;
};

this._reportError = function (phase, status, message) {
    var me = this, data = { phase: phase, status: status || 0, message: message };
    if (this._errorHandler) { this._errorHandler(data); return; }
    setTimeout(function () { me.fireWidgetEvent("error", data); }, 0);
};

// functions the server reaches with Call("reload")
this.reload = function () {
    if (!this.widget) { this._reportError("load", 0, "the pivot is not initialized."); return; }
    try { this.widget.load(); }
    catch (ex) { this._reportError("load", 0, ex.message); }
};

//# sourceURL=integrationlab.widgets.WorkOrderPivot.js
