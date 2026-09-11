// IntegrationLab.WorkOrderGrid — client adapter around VendorGrid (Kendo-style DataSource).
//
// "this" is the Wisej.NET widget wrapper (wisej.web.Widget); this.container is the DOM
// element the framework positions, sizes, shows, hides, themes and disposes.
//
// Contract (see docs/GridOperationContract.md and docs/EventPayloadContracts.md):
//   state in  (Options → init/update): pageSize, editable, sort, columns[]
//   data      (transport → postback URL): GET  &action=load&skip&take&sort&filter → { rows, total, skip, take }
//                                         GET  &action=update&payload={rowKey,changes} → { row }   (POST bodies never reach WebRequest, see ServerHandlers.md)
//                                         GET  &action=create&payload={values}          → { row }
//                                         GET  &action=destroy&payload={rowKey}         → { rowKey, deleted }
//   events out (WiredEvents):           cellClick  { rowKey, field, value }
//                                       rowUpdated { rowKey, changes }
//                                       error      { phase, status, message }
//
// This is the ONLY file that knows what a Kendo DataSource looks like. Swapping the vendor
// means rewriting the "vendorOptions" block below; the server contract does not move.

// contract event name → vendor event name
this._vendorEvents = {
    cellClick: "cellclick",
    rowUpdated: "rowupdate"
};

this.init = function (options) {
    var me = this;

    var host = document.createElement("div");
    host.className = "work-order-grid-host";
    host.style.cssText = "position:absolute;left:0;top:0;right:0;bottom:0;";
    this.container.innerHTML = "";
    this.container.appendChild(host);
    this.host = host;

    // ---------------------------------------------------------------------------------
    // POSTBACK URL (implemented per the cookbook; runtime not yet verified in the browser).
    // The wrapper's "postbackUrl" property is rendered by the server because WorkOrderGrid
    // subscribes to WebRequest; qooxdoo exposes it as this.getPostbackUrl(). Every request
    // the vendor makes goes to that URL plus "&action=…" and lands in HandleWebRequest.
    // ---------------------------------------------------------------------------------
    var url = typeof this.getPostbackUrl === "function" ? this.getPostbackUrl() : "";
    if (!url) {
        this.widget = null;
        this._reportError("init", 0, "postbackUrl is empty: the server did not render a postback URL for this widget.");
        return;
    }

    // Kendo-style DataSource: transport + schema. The vendor appends the read parameters
    // (skip, take, sort, filter) itself; the action name rides in read.data / the URL.
    // (The postback URL already carries a query string, so the actions are appended with "&".)
    var sep = url.indexOf("?") >= 0 ? "&" : "?";
    var vendorOptions = {
        columns: options.columns,
        pageSize: options.pageSize,
        editable: options.editable,
        sort: options.sort,
        dataSource: {
            transport: {
                read:    { url: url, data: { action: "load" } },
                update:  { url: url + sep + "action=update",  type: "POST" },
                create:  { url: url + sep + "action=create",  type: "POST" },
                destroy: { url: url + sep + "action=destroy", type: "POST" }
            },
            schema: { model: { id: "id" } }
        }
    };

    try {
        this.widget = new VendorGrid(host, vendorOptions);
    }
    catch (ex) {
        this.widget = null;
        this._reportError("init", 0, ex.message);
        return;
    }

    // Transport / vendor failures become ONE contract event each; the vendor's
    // "hover" and "scroll" events are deliberately not forwarded (noise).
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

// Called by Wisej.NET when a first-level Options field changes on the server
// (PageSize, Editable, Sort, Columns). The vendor re-reads when paging/sort/columns change.
this.update = function (options, old) {
    if (!this.widget) return;
    try {
        this.widget.setOptions({
            pageSize: options.pageSize,
            editable: options.editable,
            sort: options.sort,
            columns: options.columns
        });
    }
    catch (ex) {
        this._reportError("update", 0, ex.message);
    }
};

// Wisej.NET calls this once for every name in WiredEvents (after "loaded"); "handler"
// is the framework's deferred dispatcher (it calls _getEventData, then fireWidgetEvent).
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

// Translate the vendor payload into the contract payload: only meaningful data, camelCase.
this._getEventData = function (type, e) {
    switch (type) {
        case "cellClick":
            return { rowKey: e.row && e.row.id, field: e.field, value: e.value };
        case "rowUpdated":
            return { rowKey: e.row && e.row.id, changes: e.changes };
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

//# sourceURL=integrationlab.widgets.WorkOrderGrid.js
