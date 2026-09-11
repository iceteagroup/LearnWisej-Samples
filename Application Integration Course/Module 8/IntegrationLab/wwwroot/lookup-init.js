// IntegrationLab.LookupWidget — client adapter around VendorGrid, WEBMETHOD path.
//
// Same vendor grid, same dataset, different transport: the grid gets a
// FUNCTION data source and that function calls the [WebMethod]
// App.MainPage.GetWorkOrders on the server (an instance method on the top-level
// Page, which Wisej registers automatically). No URL, no HttpResponse, no JSON
// parsing: typed arguments go in, a marshaled object comes back, and the client
// awaits it.
//
// Contract (see docs/WebMethod.md):
//   state in  (Options → init/update): columns, pageSize
//   call      (client → server):       GetWorkOrders(page:int, size:int, sort:string, desc:bool)
//   return    (server → client):       {rows,total,page,size,sort,desc}   (marshaled object)
//   events out (WiredEvents):          dataLoaded {count,total,page,pages,elapsed}
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

    this._defaultPageSize = options.pageSize || 10;

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
        if (options.pageSize && options.pageSize !== this._defaultPageSize) {
            this._defaultPageSize = options.pageSize;
            this.widget.setOptions({ pageSize: options.pageSize });
            this.widget.refresh();
        }
    }
    catch (ex) {
        this._reportError("update", ex.message);
    }
};

// ---------------------------------------------------------------------------
// The function data source: one WebMethod call per page.

this._callWebMethod = function (query) {
    var args = [Number(query.page), Number(query.size), query.sort || "", !!query.desc];

    // Wisej registers TWO functions per web method on the target object:
    //     target.Name(args…, callback)   → callback(returnValue)
    //     target.NameAsync(args…)        → Promise<returnValue>
    // The Promise only resolves; when the server throws, Wisej shows its
    // exception popup and the Promise resolves with null.
    var target = (window.App && window.App.MainPage) || null;
    if (!target)
        return Promise.reject(new Error("WebMethod target not found: App.MainPage"));

    var fnAsync = target["GetWorkOrdersAsync"];
    var fnCallback = target["GetWorkOrders"];
    var promise;

    if (typeof fnAsync === "function") {
        promise = fnAsync.apply(target, args);
    }
    else if (typeof fnCallback === "function") {
        promise = new Promise(function (resolve) {
            fnCallback.apply(target, args.concat([function (result) { resolve(result); }]));
        });
    }
    else {
        return Promise.reject(new Error("WebMethod GetWorkOrders is not registered on App.MainPage"));
    }

    return Promise.resolve(promise).then(function (result) {
        if (result === null || result === undefined)
            throw new Error("WebMethod returned null: the server rejected the call");
        // WebMethod return values are not camel-cased: accept either casing.
        var rows = result.rows !== undefined ? result.rows : result.Rows;
        var total = result.total !== undefined ? result.total : result.Total;
        if (!Array.isArray(rows))
            throw new Error("WebMethod result has no rows array (keys: " + Object.keys(result).join(",") + ")");
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
            return { count: e.count, total: e.total, page: e.page, pages: e.pages, elapsed: e.elapsed };
        case "error":
            return { status: e.status || 0, message: e.message, phase: e.phase || "load" };
        case "rowClick":
            return { id: e.row && (e.row.id || e.row.Id) };
    }
    return null;
};

this._reportError = function (phase, message) {
    var me = this, data = { status: 0, message: message, phase: phase };
    if (this._errorHandler) { this._errorHandler(data); return; }
    setTimeout(function () { me.fireWidgetEvent("error", data); }, 0);
};

//# sourceURL=integrationlab.widgets.LookupWidget.js
