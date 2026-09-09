// IntegrationLab.GridWidget — client adapter around VendorGrid, POSTBACK path.
//
// "this" is the Wisej.NET widget wrapper (wisej.web.Widget); this.container is
// the DOM element the framework positions, sizes, shows, hides, themes and disposes.
//
// The vendor grid wants a URL it can GET with its own paging arguments. The URL
// comes from the framework: this.getPostbackUrl() is bound to THIS component
// instance in THIS session, so the request lands in GridWidget.grid_WebRequest
// on the server. We append our own action parameter to it.
//
// Contract (see docs/PostbackWebRequestHandler.md):
//   state in  (Options → init/update): columns, pageSize
//   request   (vendor → server):       GET postbackUrl&action=load&page=N&size=N&sort=field&desc=bool
//   response  (server → vendor):       200 application/json {rows,total,page,size,sort,desc}
//                                      400 application/json {error} on bad input
//   events out (WiredEvents):          dataLoaded {count,total,page,pages,elapsed,via}
//                                      error      {status,message,phase}
//                                      rowClick   {id}
//
// Rules this adapter follows (same as Module 1):
//   1. The vendor object lives in a child element (this.host), never on this.container.
//   2. The vendor instance is stored in this.widget so Instance.xxx() reaches it.
//   3. Events are wired through _addListener / _getEventData (deferred round trip).

// contract event name → vendor event name
this._vendorEvents = {
    dataLoaded: "dataloaded",
    error: "error",
    rowClick: "rowclick"
};

this.init = function (options) {
    var me = this;

    var host = document.createElement("div");
    host.className = "grid-widget-host";
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;

    this._action = "load";
    this._defaultPageSize = options.pageSize || 10;

    // ==== UNVERIFIED (postback URL, client side) ==========================
    // getPostbackUrl() is a property accessor of wisej.web.Widget ("postbackUrl",
    // rendered by the server for widgets that handle WebRequest). It already
    // contains the session/component identifiers and a query string, so extra
    // arguments are appended with "&" (framework comment: this.getPostbackUrl() + "&param=value").
    var url = (typeof this.getPostbackUrl === "function") ? this.getPostbackUrl() : "";
    if (!url) {
        this.widget = null;
        this._reportError("init", "getPostbackUrl() returned no URL — WebRequest is not wired on the server", 0);
        return;
    }
    this._baseUrl = url;
    // ======================================================================

    try {
        this.widget = new VendorGrid(host, {
            columns: options.columns,
            pageSize: this._defaultPageSize,
            dataSource: { transport: { read: { url: this._readUrl("load") } } }
        });
    }
    catch (ex) {
        this.widget = null;
        this._reportError("init", ex.message, 0);
        return;
    }

    // The vendor owns the fetching: the first page is pulled as soon as the
    // wrapper exists (that GET is the request the video shows in the Network panel).
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

// Called by Wisej.NET when a first-level Options field changes on the server.
this.update = function (options, old) {
    if (!this.widget) return;
    try {
        var changed = false;
        if (options.pageSize && options.pageSize !== this._defaultPageSize) {
            this._defaultPageSize = options.pageSize;
            this.widget.setOptions({ pageSize: options.pageSize });
            changed = true;
        }
        if (options.columns && old && options.columns !== old.columns) {
            this.widget.setOptions({ columns: options.columns });
            changed = true;
        }
        if (changed) this.widget.refresh();
    }
    catch (ex) {
        this._reportError("update", ex.message, 0);
    }
};

// Builds the read URL for one action. Only the server decides which actions
// exist; the client just names one.
this._readUrl = function (action) {
    return this._baseUrl + "&action=" + encodeURIComponent(action);
};

// ---------------------------------------------------------------------------
// Events: Wisej.NET calls _addListener once per name in WiredEvents.

this._addListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        if (this.widget) this.widget.on(vendorName, handler);
        if (name === "error") this._errorHandler = handler;
        return;
    }
};

this._removeListener = function (name, handler) {
    var vendorName = this._vendorEvents[name];
    if (vendorName) {
        if (this.widget) this.widget.off(vendorName, handler);
        if (name === "error" && this._errorHandler === handler) this._errorHandler = null;
    }
};

// Translate the vendor payload into the contract payload: only meaningful data.
this._getEventData = function (type, e) {
    switch (type) {
        case "dataLoaded":
            return {
                count: e.count, total: e.total, page: e.page, pages: e.pages, elapsed: e.elapsed,
                via: "GET postbackUrl&action=" + this._action
            };
        case "error":
            return { status: e.status || 0, message: e.message, phase: e.phase || "load" };
        case "rowClick":
            return { id: e.row && (e.row.id || e.row.Id) };
    }
    return null;
};

this._reportError = function (phase, message, status) {
    var me = this, data = { status: status || 0, message: message, phase: phase };
    if (this._errorHandler) { this._errorHandler(data); return; }
    setTimeout(function () { me.fireWidgetEvent("error", data); }, 0);
};

// ---------------------------------------------------------------------------
// Functions the server reaches with Control.Call("name", args). "this" = wrapper.

// Recovery: back to the known-good request shape, then fetch again.
this.reload = function () {
    if (!this.widget) return;
    this._action = "load";
    this.widget.setOptions({
        pageSize: this._defaultPageSize,
        dataSource: { transport: { read: { url: this._readUrl("load") } } }
    });
    this.widget.setPage(1);
};

this.setPage = function (n) {
    if (this.widget) this.widget.setPage(n);
};

this.sort = function (field) {
    if (this.widget) this.widget.sort(field);
};

// Failure path 1: ask the endpoint for an action it does not offer.
this.loadWithAction = function (action) {
    if (!this.widget) return;
    // one-shot: the bad action is used for exactly this request, then the adapter goes back to "load"
    // so the next paging / sorting request is not poisoned by the failure demo.
    this._action = action;
    this.widget.setOptions({ dataSource: { transport: { read: { url: this._readUrl(action) } } } });
    this.widget.refresh();
    this._action = "load";
    this.widget.setOptions({ dataSource: { transport: { read: { url: this._readUrl("load") } } } });
};

// Failure path 2: ask for more rows than the endpoint is willing to serve.
this.loadWithSize = function (size) {
    if (!this.widget) return;
    this.widget.setOptions({ pageSize: size });
    this.widget.refresh();
};

// For CallAsync("getLastRequestUrl"): the exact URL the vendor fetched last.
this.getLastRequestUrl = function () {
    return this.widget ? this.widget.getState().lastUrl : "";
};

//# sourceURL=integrationlab.widgets.GridWidget.js
