// IntegrationLab.LookupWidget — client adapter around VendorGrid, WEBMETHOD path.
//
// Same vendor grid, same dataset, different transport: the grid gets a
// FUNCTION data source and that function calls a [WebMethod] on the server.
// No URL, no HttpResponse, no JSON parsing: typed arguments go in, a marshaled
// object comes back, and the client awaits it.
//
// The WebMethod GetWorkOrders(page, size, sort, desc) exists in two places so the
// two registration styles can be compared with the "Switch WebMethod target" button:
//
//   dataSourceMode = "page"   → App.MainPage.GetWorkOrders…   (instance method on the
//                               top-level Page; Wisej registers it automatically)
//   dataSourceMode = "widget" → this.GetWorkOrders…            (instance method on the
//                               LookupWidget, registered by RegisterWebMethods(config)
//                               in OnWebRender; "this" is the wrapper = the component)
//
// Contract (see docs/WebMethod.md):
//   state in  (Options → init/update): columns, pageSize, dataSourceMode
//   call      (client → server):       GetWorkOrders(page:int, size:int, sort:string, desc:bool)
//   return    (server → client):       {rows,total,page,size,sort,desc}   (marshaled object)
//   events out (WiredEvents):          dataLoaded {count,total,page,pages,elapsed,via}
//                                      error      {status,message,phase}
//                                      rowClick   {id}

this._vendorEvents = {
    dataLoaded: "dataloaded",
    error: "error",
    rowClick: "rowclick"
};

this.init = function (options) {
    var me = this;

    var host = document.createElement("div");
    host.className = "lookup-widget-host";
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;

    this._mode = options.dataSourceMode || "page";
    this._defaultPageSize = options.pageSize || 10;
    this._lastShape = "";

    try {
        this.widget = new VendorGrid(host, {
            columns: options.columns,
            pageSize: this._defaultPageSize,
            dataSource: { load: function (query) { return me._callWebMethod(query); } }
        });
    }
    catch (ex) {
        this.widget = null;
        this._reportError("init", ex.message);
        return;
    }

    this.widget.refresh();

    var frameworkDispose = this.dispose;
    this.dispose = function () {
        try {
            if (me.widget) { me.widget.destroy(); me.widget = null; }
            if (me.host && me.host.parentNode) me.host.parentNode.removeChild(me.host);
            me.host = null;
        }
        finally {
            if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments);
        }
    };
};

this.update = function (options, old) {
    if (!this.widget) return;
    try {
        var changed = false;
        if (options.dataSourceMode && options.dataSourceMode !== this._mode) {
            this._mode = options.dataSourceMode;
            changed = true;
        }
        if (options.pageSize && options.pageSize !== this._defaultPageSize) {
            this._defaultPageSize = options.pageSize;
            this.widget.setOptions({ pageSize: options.pageSize });
            changed = true;
        }
        if (changed) this.widget.refresh();
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
};

// ---------------------------------------------------------------------------
// The function data source: one WebMethod call per page.

this._callWebMethod = function (query) {
    var me = this;
    var args = [Number(query.page), Number(query.size), query.sort || "", !!query.desc];

    // ==== UNVERIFIED (WebMethod client call shapes) =========================
    // Verified from the framework client code: Wisej registers TWO functions per
    // web method on the target object —
    //     target.Name(args…, callback)   → callback(returnValue)      (invokeWebMethod)
    //     target.NameAsync(args…)        → Promise<returnValue>       (invokeWebMethodAsync)
    // The Promise only resolves; when the server throws, Wisej shows its
    // exception popup and the Promise resolves with null. What is unverified
    // is only the runtime wiring for our two targets (App.MainPage / this).
    var target, label;
    if (this._mode === "widget") {
        target = this;
        label = "this.GetWorkOrders (LookupWidget, RegisterWebMethods)";
    }
    else {
        target = (window.App && window.App.MainPage) || null;
        label = "App.MainPage.GetWorkOrders (top-level Page)";
    }

    if (!target)
        return Promise.reject(new Error("WebMethod target not found: " + label));

    var fnAsync = target["GetWorkOrdersAsync"];
    var fnCallback = target["GetWorkOrders"];
    var promise;

    if (typeof fnAsync === "function") {
        // Promise style: the wrapper Wisej generates for every web method.
        this._lastShape = label.replace("GetWorkOrders", "GetWorkOrdersAsync") + " → Promise";
        promise = fnAsync.apply(target, args);
    }
    else if (typeof fnCallback === "function") {
        // Trailing-callback style: the first function argument becomes the callback.
        //     App.MainPage.GetWorkOrders(page, size, sort, desc, function (result) { ... });
        this._lastShape = label + " + trailing callback";
        promise = new Promise(function (resolve) {
            fnCallback.apply(target, args.concat([function (result) { resolve(result); }]));
        });
    }
    else {
        return Promise.reject(new Error("WebMethod GetWorkOrders is not registered on " + label));
    }
    // ======================================================================

    return Promise.resolve(promise).then(function (result) {
        if (result === null || result === undefined)
            throw new Error("WebMethod returned null: the server rejected the call (ArgumentException → Wisej exception popup)");
        // Tolerate either casing of the marshaled object.
        var rows = result.rows !== undefined ? result.rows : result.Rows;
        var total = result.total !== undefined ? result.total : result.Total;
        if (!Array.isArray(rows))
            throw new Error("WebMethod result has no rows array (keys: " + Object.keys(result).join(",") + ")");
        me._lastKeys = Object.keys(result).join(",");
        return { rows: rows, total: Number(total) || 0 };
    });
};

// ---------------------------------------------------------------------------
// Events

this._addListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        if (this.widget) this.widget.on(vendorName, handler);
        if (name === "error") this._errorHandler = handler;
    }
};

this._removeListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        if (this.widget) this.widget.off(vendorName, handler);
        if (name === "error" && this._errorHandler === handler) this._errorHandler = null;
    }
};

this._getEventData = function (type, e) {
    switch (type) {
        case "dataLoaded":
            return {
                count: e.count, total: e.total, page: e.page, pages: e.pages, elapsed: e.elapsed,
                via: this._lastShape, keys: this._lastKeys || ""
            };
        case "error":
            return { status: e.status || 0, message: e.message, phase: e.phase || "load", via: this._lastShape };
        case "rowClick":
            return { id: e.row && (e.row.id || e.row.Id) };
    }
    return null;
};

this._reportError = function (phase, message) {
    var me = this, data = { status: 0, message: message, phase: phase, via: this._lastShape };
    if (this._errorHandler) { this._errorHandler(data); return; }
    setTimeout(function () { me.fireWidgetEvent("error", data); }, 0);
};

// ---------------------------------------------------------------------------
// Functions the server reaches with Control.Call("name", args).

this.reload = function () {
    if (!this.widget) return;
    this.widget.setOptions({ pageSize: this._defaultPageSize });
    this.widget.setPage(1);
};

this.setPage = function (n) {
    if (this.widget) this.widget.setPage(n);
};

this.sort = function (field) {
    if (this.widget) this.widget.sort(field);
};

// Failure path: the WebMethod validates size the same way the postback handler does.
this.loadWithSize = function (size) {
    if (!this.widget) return;
    this.widget.setOptions({ pageSize: size });
    this.widget.refresh();
};

//# sourceURL=integrationlab.widgets.LookupWidget.js
