/*!
 * VendorGrid 1.0 — a small stand-in for a third-party data grid library.
 *
 * Like a real vendor grid (Kendo, DevExtreme, ag-Grid…) it owns the fetching:
 * you hand it a data source and it pulls pages itself.
 *
 *   var grid = new VendorGrid(hostElement, {
 *       columns:  [{ field: "id", title: "ID" }, ...],
 *       pageSize: 10,
 *       dataSource: { transport: { read: { url: "…" } } }       // URL data source (Ajax GET)
 *                   | { load: function (query) { return Promise<{rows,total}> } }   // function data source
 *   });
 *
 *   grid.refresh()             re-fetch the current page
 *   grid.setPage(n)            go to page n (1-based) and fetch
 *   grid.sort(field)           sort by field; same field again toggles direction
 *   grid.setOptions({...})     replace pageSize / columns / dataSource, no fetch
 *   grid.getState()            { page, size, sort, desc, total, pages, lastUrl }
 *   grid.on(name, fn) / .off(name, fn) / .destroy()
 *
 * Events (vendor-style lower-case names):
 *   "dataloaded" { count, total, page, pages, elapsed, source }
 *   "error"      { status, message, phase, source }      status 0 = not an HTTP failure
 *   "rowclick"   { row }
 *
 * URL data source: GET url + "&page=N&size=N&sort=field&desc=true|false" and
 * expects JSON { rows: [...], total: N }. A non-2xx status is rendered as a red
 * error row: "HTTP <status> — <message>" where message comes from a JSON body
 * { error: "…" } when the response is application/json, else the status text.
 *
 * Function data source: calls load({ page, size, sort, desc }) and awaits the
 * promise; a rejection or a result without rows becomes the same red error row.
 *
 * No CDN, no dependencies. Draws a plain HTML table (see vendor-grid.css).
 */
(function (global) {
    "use strict";

    function VendorGrid(host, options) {
        if (!(this instanceof VendorGrid))
            return new VendorGrid(host, options);
        if (!host || typeof host.appendChild !== "function")
            throw new Error("VendorGrid: host element is required");

        options = options || {};
        this._host = host;
        this._columns = [];
        this._pageSize = 10;
        this._dataSource = null;
        this._page = 1;
        this._sort = "";
        this._desc = false;
        this._total = 0;
        this._rows = [];
        this._handlers = {};
        this._requestSeq = 0;
        this._lastUrl = "";
        this._destroyed = false;

        this._build();
        this.setOptions(options);
    }

    VendorGrid.prototype = {

        // ---------------------------------------------------------------- options

        setOptions: function (options) {
            options = options || {};
            if (options.columns !== undefined) {
                if (!Array.isArray(options.columns))
                    throw new Error("VendorGrid: columns must be an array of { field, title }");
                this._columns = options.columns.map(function (c) {
                    if (!c || !c.field) throw new Error("VendorGrid: every column needs a field");
                    return { field: c.field, title: c.title || c.field, sortable: c.sortable !== false };
                });
                this._renderHeader();
            }
            if (options.pageSize !== undefined) {
                var n = Number(options.pageSize);
                if (!isFinite(n) || n < 1)
                    throw new Error("VendorGrid: pageSize must be a positive number");
                this._pageSize = Math.floor(n);
            }
            if (options.dataSource !== undefined) {
                var ds = options.dataSource;
                var url = ds && ds.transport && ds.transport.read && ds.transport.read.url;
                var load = ds && ds.load;
                if (typeof load === "function")
                    this._dataSource = { kind: "function", load: load };
                else if (typeof url === "string" && url.length > 0)
                    this._dataSource = { kind: "url", url: url };
                else
                    throw new Error("VendorGrid: dataSource needs transport.read.url or a load(query) function");
            }
            return this;
        },

        getState: function () {
            return {
                page: this._page, size: this._pageSize, sort: this._sort, desc: this._desc,
                total: this._total, pages: this._pages(), lastUrl: this._lastUrl,
                sourceKind: this._dataSource ? this._dataSource.kind : null
            };
        },

        // ---------------------------------------------------------------- data

        refresh: function () {
            this._load();
            return this;
        },

        setPage: function (n) {
            n = Math.floor(Number(n));
            if (!isFinite(n) || n < 1) n = 1;
            this._page = n;
            this._load();
            return this;
        },

        sort: function (field) {
            if (!field) return this;
            if (this._sort === field) this._desc = !this._desc;
            else { this._sort = field; this._desc = false; }
            this._page = 1;
            this._load();
            return this;
        },

        _query: function () {
            return { page: this._page, size: this._pageSize, sort: this._sort, desc: this._desc };
        },

        _load: function () {
            var me = this;
            if (this._destroyed) return;
            if (!this._dataSource) {
                this._fail(0, "no data source configured", "load");
                return;
            }

            var seq = ++this._requestSeq;
            var q = this._query();
            var started = (global.performance && performance.now) ? performance.now() : Date.now();
            this._showLoading(q);

            var promise;
            if (this._dataSource.kind === "url") {
                var url = this._dataSource.url +
                    (this._dataSource.url.indexOf("?") >= 0 ? "&" : "?") +
                    "page=" + encodeURIComponent(q.page) +
                    "&size=" + encodeURIComponent(q.size) +
                    "&sort=" + encodeURIComponent(q.sort || "") +
                    "&desc=" + (q.desc ? "true" : "false");
                this._lastUrl = url;
                promise = this._fetchJson(url);
            }
            else {
                this._lastUrl = "";
                try {
                    promise = Promise.resolve(this._dataSource.load(q));
                }
                catch (ex) {
                    promise = Promise.reject(ex);
                }
                promise = promise.then(function (result) {
                    if (!result || !Array.isArray(result.rows))
                        throw new VendorGridError(0, "data source returned no rows (" + describe(result) + ")");
                    return result;
                });
            }

            promise.then(function (result) {
                if (seq !== me._requestSeq || me._destroyed) return;      // stale response
                var elapsed = Math.round(((global.performance && performance.now) ? performance.now() : Date.now()) - started);
                me._rows = result.rows || [];
                me._total = Number(result.total) || 0;
                me._renderRows();
                me._renderFooter();
                me._emit("dataloaded", {
                    count: me._rows.length, total: me._total, page: me._page, pages: me._pages(),
                    elapsed: elapsed, source: me._dataSource.kind
                });
            }, function (err) {
                if (seq !== me._requestSeq || me._destroyed) return;
                var status = (err && typeof err.status === "number") ? err.status : 0;
                var message = (err && err.message) ? err.message : String(err);
                me._fail(status, message, "load");
            });
        },

        _fetchJson: function (url) {
            return fetch(url, { method: "GET", credentials: "same-origin", headers: { "Accept": "application/json" } })
                .then(function (response) {
                    var contentType = response.headers.get("content-type") || "";
                    var isJson = contentType.toLowerCase().indexOf("application/json") >= 0;
                    if (!response.ok) {
                        return response.text().then(function (text) {
                            var message = response.statusText || ("status " + response.status);
                            if (isJson) {
                                try { var body = JSON.parse(text); if (body && body.error) message = body.error; } catch (ignore) { }
                            }
                            throw new VendorGridError(response.status, message);
                        });
                    }
                    if (!isJson)
                        throw new VendorGridError(response.status, "expected application/json, got \"" + (contentType || "none") + "\"");
                    return response.json();
                })
                .then(function (body) {
                    if (!body || !Array.isArray(body.rows))
                        throw new VendorGridError(0, "JSON body has no rows array");
                    return body;
                });
        },

        _fail: function (status, message, phase) {
            this._rows = [];
            this._renderError(status, message);
            this._renderFooter();
            this._emit("error", { status: status, message: message, phase: phase, source: this._dataSource ? this._dataSource.kind : null });
        },

        _pages: function () {
            return this._total > 0 ? Math.max(1, Math.ceil(this._total / this._pageSize)) : 0;
        },

        // ---------------------------------------------------------------- events

        on: function (name, handler) {
            if (typeof handler !== "function") throw new Error("VendorGrid: handler must be a function");
            (this._handlers[name] = this._handlers[name] || []).push(handler);
            return this;
        },

        off: function (name, handler) {
            if (!name) { this._handlers = {}; return this; }
            var list = this._handlers[name];
            if (!list) return this;
            this._handlers[name] = handler ? list.filter(function (h) { return h !== handler; }) : [];
            return this;
        },

        _emit: function (name, data) {
            var list = this._handlers[name];
            if (!list) return;
            list.slice().forEach(function (h) {
                try { h(data); } catch (ex) { if (global.console) console.error("VendorGrid handler error", ex); }
            });
        },

        // ---------------------------------------------------------------- DOM

        _build: function () {
            var me = this;
            var root = document.createElement("div");
            root.className = "vgrid";
            root.innerHTML =
                '<div class="vgrid-scroll"><table class="vgrid-table"><thead><tr></tr></thead><tbody></tbody></table></div>' +
                '<div class="vgrid-footer">' +
                '  <button type="button" class="vgrid-btn vgrid-prev" title="Previous page">&#8249; Prev</button>' +
                '  <span class="vgrid-pageinfo"></span>' +
                '  <button type="button" class="vgrid-btn vgrid-next" title="Next page">Next &#8250;</button>' +
                '  <span class="vgrid-status"></span>' +
                '</div>';
            this._host.appendChild(root);
            this._root = root;
            this._thead = root.querySelector("thead tr");
            this._tbody = root.querySelector("tbody");
            this._footer = root.querySelector(".vgrid-footer");
            this._pageInfo = root.querySelector(".vgrid-pageinfo");
            this._status = root.querySelector(".vgrid-status");

            this._onPrev = function () { if (me._page > 1) me.setPage(me._page - 1); };
            this._onNext = function () { if (me._page < me._pages()) me.setPage(me._page + 1); };
            root.querySelector(".vgrid-prev").addEventListener("click", this._onPrev);
            root.querySelector(".vgrid-next").addEventListener("click", this._onNext);

            this._onHeaderClick = function (e) {
                var th = e.target.closest ? e.target.closest("th") : null;
                if (th && th.dataset.field && th.dataset.sortable === "true") me.sort(th.dataset.field);
            };
            this._thead.addEventListener("click", this._onHeaderClick);

            this._onRowClick = function (e) {
                var tr = e.target.closest ? e.target.closest("tr") : null;
                if (tr && tr.dataset.index !== undefined && me._rows[tr.dataset.index])
                    me._emit("rowclick", { row: me._rows[tr.dataset.index] });
            };
            this._tbody.addEventListener("click", this._onRowClick);
        },

        _renderHeader: function () {
            var me = this;
            this._thead.innerHTML = "";
            this._columns.forEach(function (c) {
                var th = document.createElement("th");
                th.dataset.field = c.field;
                th.dataset.sortable = String(c.sortable);
                th.className = c.sortable ? "vgrid-sortable" : "";
                th.textContent = c.title;
                if (me._sort === c.field) {
                    th.className += " vgrid-sorted";
                    th.textContent += me._desc ? " ▾" : " ▴";
                }
                me._thead.appendChild(th);
            });
        },

        _renderRows: function () {
            var me = this;
            this._renderHeader();
            this._tbody.innerHTML = "";
            if (this._rows.length === 0) {
                this._tbody.appendChild(this._messageRow("No rows.", "vgrid-empty"));
                return;
            }
            this._rows.forEach(function (row, index) {
                var tr = document.createElement("tr");
                tr.dataset.index = index;
                me._columns.forEach(function (c) {
                    var td = document.createElement("td");
                    var value = row[c.field];
                    if (value === undefined) value = row[c.field.charAt(0).toUpperCase() + c.field.slice(1)];   // tolerate PascalCase payloads
                    td.textContent = value === null || value === undefined ? "" : String(value);
                    if (c.field === "status") td.className = "vgrid-status-" + String(value || "").toLowerCase();
                    tr.appendChild(td);
                });
                me._tbody.appendChild(tr);
            });
        },

        _renderError: function (status, message) {
            this._tbody.innerHTML = "";
            var text = (status ? "HTTP " + status + " — " : "") + message;
            this._tbody.appendChild(this._messageRow("✖ " + text, "vgrid-error"));
        },

        _showLoading: function (q) {
            this._tbody.innerHTML = "";
            this._tbody.appendChild(this._messageRow("Loading page " + q.page + "…", "vgrid-loading"));
            this._status.textContent = "loading…";
        },

        _messageRow: function (text, className) {
            var tr = document.createElement("tr");
            tr.className = className;
            var td = document.createElement("td");
            td.colSpan = Math.max(1, this._columns.length);
            td.textContent = text;
            tr.appendChild(td);
            return tr;
        },

        _renderFooter: function () {
            var pages = this._pages();
            this._pageInfo.textContent = pages ? ("page " + this._page + " / " + pages) : "page –";
            this._status.textContent = this._total ? (this._rows.length + " of " + this._total + " rows") : "";
            this._root.querySelector(".vgrid-prev").disabled = this._page <= 1;
            this._root.querySelector(".vgrid-next").disabled = !pages || this._page >= pages;
        },

        destroy: function () {
            if (this._destroyed) return;
            this._destroyed = true;
            this._requestSeq++;
            this._handlers = {};
            try {
                this._root.querySelector(".vgrid-prev").removeEventListener("click", this._onPrev);
                this._root.querySelector(".vgrid-next").removeEventListener("click", this._onNext);
                this._thead.removeEventListener("click", this._onHeaderClick);
                this._tbody.removeEventListener("click", this._onRowClick);
            } catch (ignore) { }
            if (this._root && this._root.parentNode) this._root.parentNode.removeChild(this._root);
            this._root = this._thead = this._tbody = this._footer = null;
            this._host = null;
        }
    };

    function VendorGridError(status, message) {
        this.name = "VendorGridError";
        this.status = status;
        this.message = message;
    }
    VendorGridError.prototype = Object.create(Error.prototype);

    function describe(value) {
        if (value === null) return "null";
        if (value === undefined) return "undefined";
        if (typeof value !== "object") return typeof value + " " + String(value);
        try { return "object with keys " + JSON.stringify(Object.keys(value)); } catch (ignore) { return "object"; }
    }

    VendorGrid.version = "1.0.0";
    global.VendorGrid = VendorGrid;

})(typeof window !== "undefined" ? window : this);
