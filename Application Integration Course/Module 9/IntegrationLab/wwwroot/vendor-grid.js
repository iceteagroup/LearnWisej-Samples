/*!
 * VendorGrid 1.0 — a tiny stand-in for a third-party editable grid library with
 * Kendo-style DataSource semantics. It knows nothing about Wisej.NET: it takes a host
 * element and an options object, talks HTTP to whatever URLs the transport names,
 * renders an HTML table, and emits its own lower-case events.
 *
 * new VendorGrid(element, {
 *   columns:  [{ field, title, width, editable, type: "text"|"number"|"select", values }],
 *   pageSize: 20,
 *   editable: true,
 *   sort:     "status asc,priority desc",            // optional
 *   filter:   [{ field, op, value }],                // optional
 *   dataSource: {
 *     transport: {
 *       read:    { url, data: { action: "load" } },  // GET  url&action=load&skip=&take=&sort=&filter=
 *       update:  { url, type: "POST" },              // POST { rowKey, changes }  → { row }
 *       create:  { url, type: "POST" },              // POST { values }           → { row }
 *       destroy: { url, type: "POST" }               // POST { rowKey }           → { rowKey, deleted }
 *     },
 *     schema: { model: { id: "id" } }
 *   }
 * })
 *
 * API:  .read()  .readWith({skip,take})  .page(n)  .nextPage()  .prevPage()  .setSort(expr)
 *       .setOptions({pageSize, editable, sort, columns})  .insertRow(values)  .updateRow(key, changes)
 *       .deleteSelected()  .getSelectedKey()  .on(name, fn)  .off(name, fn)  .resize()  .destroy()
 *
 * Events (vendor naming, lower-case):
 *   "cellclick"  { row, field, value }        user clicked a cell
 *   "rowupdate"  { row, changes }             user committed an inline edit (fires BEFORE the transport call)
 *   "rowinsert"  { values }                   user saved the "Add row" form (before the transport call)
 *   "rowdelete"  { row }                      user clicked a row's delete control (before the transport call)
 *   "dataloaded" { rows, total, skip, take }  a read completed
 *   "error"      { status, message, operation } transport or vendor failure (status 0 = not HTTP)
 *   "hover"      { row, field }               noisy: every mouseover
 *   "scroll"     { top }                      noisy: every scroll
 */
(function (global) {
  "use strict";

  function VendorGrid(element, options) {
    if (!element || !element.appendChild)
      throw new Error("VendorGrid: a host element is required.");
    options = options || {};
    if (!Array.isArray(options.columns) || options.columns.length === 0)
      throw new Error("VendorGrid: options.columns must be a non-empty array.");
    var ds = options.dataSource;
    if (!ds || !ds.transport || !ds.transport.read || typeof ds.transport.read.url !== "string" || !ds.transport.read.url)
      throw new Error("VendorGrid: dataSource.transport.read.url is required.");

    this.el = element;
    this.transport = ds.transport;
    this.idField = (ds.schema && ds.schema.model && ds.schema.model.id) || "id";
    this.opts = {
      columns: options.columns.slice(),
      pageSize: clampPage(options.pageSize),
      editable: options.editable !== false,
      sort: typeof options.sort === "string" ? options.sort : "",
      filter: Array.isArray(options.filter) ? options.filter : null
    };
    this._handlers = {};
    this._destroyed = false;
    this._busy = false;
    this._readToken = 0;
    this.skip = 0;
    this.total = 0;
    this.rows = [];
    this.selectedKey = null;
    this._editing = null;
    this._adding = false;

    this._build();
    this.read();
  }

  function clampPage(n) { n = parseInt(n, 10); return isNaN(n) || n < 1 ? 20 : n; }
  function esc(s) { return String(s == null ? "" : s).replace(/[&<>"']/g, function (c) { return { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]; }); }
  function clone(o) { return JSON.parse(JSON.stringify(o)); }
  function h(tag, cls, text) { var n = document.createElement(tag); if (cls) n.className = cls; if (text != null) n.textContent = text; return n; }

  // ---- events --------------------------------------------------------------
  VendorGrid.prototype.on = function (name, fn) { (this._handlers[name] = this._handlers[name] || []).push(fn); return this; };
  VendorGrid.prototype.off = function (name, fn) {
    if (!name) { this._handlers = {}; return this; }
    var list = this._handlers[name] || [];
    this._handlers[name] = fn ? list.filter(function (x) { return x !== fn; }) : [];
    return this;
  };
  VendorGrid.prototype._emit = function (name, data) {
    var list = this._handlers[name] || [];
    for (var i = 0; i < list.length; i++) list[i](data);
  };
  VendorGrid.prototype._fail = function (operation, status, message) {
    this._setBusy(false);
    this.el.classList.add("vgrid-error");
    this._status.textContent = "error " + (status || "") + " — " + message;
    this._emit("error", { status: status || 0, message: message, operation: operation });
  };

  // ---- DOM -------------------------------------------------------------------
  VendorGrid.prototype._build = function () {
    var me = this;
    this.el.innerHTML = "";
    var root = h("div", "vgrid");
    var bar = h("div", "vgrid-toolbar");
    this._addBtn = h("button", "vgrid-btn vgrid-add", "+ Add row");
    this._addBtn.type = "button";
    this._addBtn.addEventListener("click", function () { me._beginAdd(); });
    this._status = h("span", "vgrid-status", "");
    var pager = h("span", "vgrid-pager");
    this._prevBtn = h("button", "vgrid-btn", "‹"); this._prevBtn.type = "button";
    this._nextBtn = h("button", "vgrid-btn", "›"); this._nextBtn.type = "button";
    this._pageLabel = h("span", "vgrid-page", "");
    this._prevBtn.addEventListener("click", function () { me.prevPage(); });
    this._nextBtn.addEventListener("click", function () { me.nextPage(); });
    pager.appendChild(this._prevBtn); pager.appendChild(this._pageLabel); pager.appendChild(this._nextBtn);
    bar.appendChild(this._addBtn); bar.appendChild(this._status); bar.appendChild(pager);

    this._body = h("div", "vgrid-body");
    this._table = h("table", "vgrid-table");
    this._thead = h("thead"); this._tbody = h("tbody");
    this._table.appendChild(this._thead); this._table.appendChild(this._tbody);
    this._body.appendChild(this._table);
    this._body.addEventListener("scroll", function () { me._emit("scroll", { top: me._body.scrollTop }); });

    root.appendChild(bar); root.appendChild(this._body);
    this.el.appendChild(root);
    this._root = root;
    this._renderHeader();
  };

  VendorGrid.prototype._renderHeader = function () {
    var me = this, tr = h("tr");
    var sortField = this._sortField(), sortDesc = this._sortDesc();
    this.opts.columns.forEach(function (c) {
      var th = h("th", "vgrid-th", c.title || c.field);
      th.style.width = (c.width || 100) + "px";
      if (sortField === c.field) th.appendChild(h("span", "vgrid-sort", sortDesc ? " ▼" : " ▲"));
      th.addEventListener("click", function () { me._toggleSort(c.field); });
      tr.appendChild(th);
    });
    tr.appendChild(h("th", "vgrid-th vgrid-th-actions", ""));
    this._thead.innerHTML = "";
    this._thead.appendChild(tr);
  };

  VendorGrid.prototype._renderRows = function () {
    var me = this;
    this._tbody.innerHTML = "";
    if (this._adding) this._tbody.appendChild(this._buildAddRow());
    this.rows.forEach(function (row) {
      var key = row[me.idField];
      var tr = h("tr", "vgrid-row" + (key === me.selectedKey ? " vgrid-selected" : ""));
      tr.setAttribute("data-key", key);
      me.opts.columns.forEach(function (c) {
        var value = row[c.field];
        var td = h("td", "vgrid-td vgrid-type-" + (c.type || "text"), c.type === "number" && typeof value === "number" ? value.toFixed(1) : value);
        td.setAttribute("data-field", c.field);
        if (me.opts.editable && c.editable !== false) td.classList.add("vgrid-editable");
        td.addEventListener("click", function () {
          me.selectedKey = key;
          me._highlight();
          me._emit("cellclick", { row: clone(row), field: c.field, value: value });
        });
        td.addEventListener("dblclick", function () { me._beginEdit(td, row, c); });
        td.addEventListener("mouseover", function () { me._emit("hover", { row: clone(row), field: c.field }); });
        tr.appendChild(td);
      });
      var actions = h("td", "vgrid-td vgrid-actions");
      if (me.opts.editable) {
        var del = h("button", "vgrid-btn vgrid-del", "✕");
        del.type = "button"; del.title = "Delete " + key;
        del.addEventListener("click", function (ev) { ev.stopPropagation(); me._deleteRow(row); });
        actions.appendChild(del);
      }
      tr.appendChild(actions);
      me._tbody.appendChild(tr);
    });
    if (this.rows.length === 0 && !this._adding) {
      var empty = h("tr", "vgrid-empty");
      var td = h("td", "", "no rows"); td.colSpan = this.opts.columns.length + 1;
      empty.appendChild(td); this._tbody.appendChild(empty);
    }
    this._addBtn.style.display = this.opts.editable ? "" : "none";
    var pages = Math.max(1, Math.ceil(this.total / this.opts.pageSize));
    var page = Math.floor(this.skip / this.opts.pageSize) + 1;
    this._pageLabel.textContent = "page " + page + "/" + pages;
    this._prevBtn.disabled = this.skip <= 0;
    this._nextBtn.disabled = this.skip + this.opts.pageSize >= this.total;
  };

  VendorGrid.prototype._highlight = function () {
    var me = this;
    Array.prototype.forEach.call(this._tbody.querySelectorAll("tr.vgrid-row"), function (tr) {
      tr.classList.toggle("vgrid-selected", tr.getAttribute("data-key") === me.selectedKey);
    });
  };

  VendorGrid.prototype._setBusy = function (busy) {
    this._busy = busy;
    this._root.classList.toggle("vgrid-loading", busy);
  };

  // ---- sorting --------------------------------------------------------------
  VendorGrid.prototype._sortField = function () { var s = this.opts.sort.trim(); return s ? s.split(",")[0].trim().split(/\s+/)[0] : null; };
  VendorGrid.prototype._sortDesc = function () { var s = this.opts.sort.trim(); if (!s) return false; var p = s.split(",")[0].trim().split(/\s+/); return p.length > 1 && p[1].toLowerCase() === "desc"; };
  VendorGrid.prototype._toggleSort = function (field) {
    var next;
    if (this._sortField() !== field) next = field + " asc";
    else if (!this._sortDesc()) next = field + " desc";
    else next = "";
    this.setSort(next);
  };
  VendorGrid.prototype.setSort = function (expr) {
    this._assertAlive("setSort");
    this.opts.sort = typeof expr === "string" ? expr : "";
    this.skip = 0;
    this._renderHeader();
    this.read();
  };

  // ---- transport -------------------------------------------------------------
  VendorGrid.prototype._request = function (operation, url, init) {
    var me = this;
    init = init || {};
    init.credentials = "same-origin";
    init.headers = init.headers || {};
    init.headers["Accept"] = "application/json";
    return fetch(url, init).then(function (res) {
      return res.text().then(function (text) {
        var data = null;
        try { data = text ? JSON.parse(text) : null; } catch (ex) { data = null; }
        if (!res.ok) {
          var err = new Error((data && data.message) || ("HTTP " + res.status));
          err.status = res.status;
          throw err;
        }
        if (data == null) throw new Error("VendorGrid: the " + operation + " response is not JSON.");
        return data;
      });
    });
  };

  VendorGrid.prototype._readUrl = function (skip, take) {
    var read = this.transport.read;
    var params = {};
    var extra = read.data || {};
    for (var k in extra) params[k] = extra[k];
    params.skip = skip;
    params.take = take;
    if (this.opts.sort) params.sort = this.opts.sort;
    if (this.opts.filter && this.opts.filter.length) params.filter = JSON.stringify(this.opts.filter);
    var qs = Object.keys(params).map(function (k) { return encodeURIComponent(k) + "=" + encodeURIComponent(params[k]); }).join("&");
    return read.url + (read.url.indexOf("?") >= 0 ? "&" : "?") + qs;
  };

  VendorGrid.prototype._send = function (operation, body) {
    var t = this.transport[operation];
    if (!t || !t.url) throw new Error("VendorGrid: dataSource.transport." + operation + " is not configured.");
    // Kendo-style transports declare type: "POST" for modify operations. Against a Wisej.NET
    // postback URL the document must travel as a GET query parameter instead: the framework
    // answers every POST to postback.wx itself (with [{"type":0}]) and never raises WebRequest.
    // The library honours an explicit type: "POST" only for URLs that are not a Wisej postback URL.
    var json = JSON.stringify(body);
    if (t.type === "POST" && !/postback\.wx/i.test(t.url)) {
      return this._request(operation, t.url, { method: "POST", headers: { "Content-Type": "application/json" }, body: json });
    }
    return this._request(operation, t.url + (t.url.indexOf("?") >= 0 ? "&" : "?") + "payload=" + encodeURIComponent(json), { method: "GET" });
  };

  // ---- read ----------------------------------------------------------------------
  VendorGrid.prototype.read = function () {
    this._assertAlive("read");
    return this._read(this.skip, this.opts.pageSize, true);
  };

  /** Raw read with explicit paging; does not become the current page unless it succeeds. */
  VendorGrid.prototype.readWith = function (paging) {
    this._assertAlive("readWith");
    paging = paging || {};
    return this._read(parseInt(paging.skip, 10) || 0, parseInt(paging.take, 10) || this.opts.pageSize, false);
  };

  VendorGrid.prototype._read = function (skip, take, keepPageSize) {
    var me = this, token = ++this._readToken;
    this._setBusy(true);
    this.el.classList.remove("vgrid-error");
    this._status.textContent = "loading…";
    if (this._editing) { this._editing.done = true; this._editing = null; }   // a reload cancels an open editor
    return this._request("read", this._readUrl(skip, take), { method: "GET" })
      .then(function (data) {
        if (me._destroyed || token !== me._readToken) return;
        if (!data || !Array.isArray(data.rows) || typeof data.total !== "number")
          throw new Error("VendorGrid: the read response must be { rows: [], total: n }.");
        me.rows = data.rows;
        me.total = data.total;
        me.skip = typeof data.skip === "number" ? data.skip : skip;
        if (!keepPageSize && typeof data.take === "number") me.opts.pageSize = data.take;
        me._setBusy(false);
        me._status.textContent = "rows " + (me.total ? me.skip + 1 : 0) + "–" + Math.min(me.skip + me.rows.length, me.total) + " of " + me.total + (me.opts.sort ? " · sort " + me.opts.sort : "");
        me._renderRows();
        me._emit("dataloaded", { rows: me.rows.length, total: me.total, skip: me.skip, take: me.opts.pageSize });
      })
      .catch(function (err) {
        if (me._destroyed || token !== me._readToken) return;
        me._fail("read", err.status, err.message);
      });
  };

  VendorGrid.prototype.page = function (n) {
    this._assertAlive("page");
    var pages = Math.max(1, Math.ceil(this.total / this.opts.pageSize));
    n = Math.max(1, Math.min(pages, parseInt(n, 10) || 1));
    this.skip = (n - 1) * this.opts.pageSize;
    return this.read();
  };
  VendorGrid.prototype.nextPage = function () {
    this._assertAlive("nextPage");
    if (this.skip + this.opts.pageSize >= this.total) { this.skip = 0; } else { this.skip += this.opts.pageSize; }
    return this.read();
  };
  VendorGrid.prototype.prevPage = function () {
    this._assertAlive("prevPage");
    this.skip = Math.max(0, this.skip - this.opts.pageSize);
    return this.read();
  };

  VendorGrid.prototype.setOptions = function (partial) {
    this._assertAlive("setOptions");
    partial = partial || {};
    var reread = false;
    if (partial.columns !== undefined) {
      if (!Array.isArray(partial.columns) || partial.columns.length === 0) throw new Error("VendorGrid.setOptions: columns must be a non-empty array.");
      this.opts.columns = partial.columns.slice(); this._renderHeader(); reread = true;
    }
    if (partial.pageSize !== undefined) { var ps = clampPage(partial.pageSize); if (ps !== this.opts.pageSize) { this.opts.pageSize = ps; this.skip = 0; reread = true; } }
    if (partial.editable !== undefined) { this.opts.editable = !!partial.editable; }
    if (partial.sort !== undefined && partial.sort !== this.opts.sort) { this.opts.sort = partial.sort || ""; this.skip = 0; this._renderHeader(); reread = true; }
    if (partial.filter !== undefined) { this.opts.filter = Array.isArray(partial.filter) ? partial.filter : null; this.skip = 0; reread = true; }
    if (reread) this.read(); else this._renderRows();
  };

  // ---- inline editing -------------------------------------------------------------
  VendorGrid.prototype._beginEdit = function (td, row, column) {
    if (!this.opts.editable || column.editable === false || this._editing || this._busy) return;
    var me = this, original = row[column.field];
    var editor = this._createEditor(column, original);
    td.innerHTML = "";
    td.appendChild(editor);
    td.classList.add("vgrid-editing");
    editor.focus();
    if (editor.select) editor.select();
    var state = { td: td, row: row, column: column, editor: editor, done: false };
    this._editing = state;

    var finish = function (commit) {
      if (state.done) return;              // Enter/Escape/blur/reload: only the first one counts
      state.done = true;
      if (me._editing === state) me._editing = null;
      if (!commit) { me._renderRows(); return; }
      var value = me._readEditor(state.column, state.editor);
      if (value === original || (state.column.type === "number" && isNaN(value) && isNaN(original))) { me._renderRows(); return; }
      var changes = {}; changes[state.column.field] = value;
      // the intent event fires first (the UI decided), then the transport persists it.
      me._emit("rowupdate", { row: clone(row), changes: clone(changes) });
      me._persistUpdate(row[me.idField], changes, "update");
    };
    editor.addEventListener("keydown", function (ev) {
      if (ev.key === "Enter") { ev.preventDefault(); finish(true); }
      else if (ev.key === "Escape") { ev.preventDefault(); finish(false); }
    });
    editor.addEventListener("blur", function () { setTimeout(function () { finish(true); }, 0); });
  };

  VendorGrid.prototype._createEditor = function (column, value) {
    var editor;
    if (column.type === "select" && Array.isArray(column.values)) {
      editor = document.createElement("select");
      column.values.forEach(function (v) { var o = document.createElement("option"); o.value = v; o.textContent = v; if (v === value) o.selected = true; editor.appendChild(o); });
    } else {
      editor = document.createElement("input");
      editor.type = column.type === "number" ? "number" : "text";
      if (column.type === "number") editor.step = "0.5";
      editor.value = value == null ? "" : value;
    }
    editor.className = "vgrid-editor";
    return editor;
  };
  VendorGrid.prototype._readEditor = function (column, editor) {
    var v = editor.value;
    if (column.type === "number") { var n = parseFloat(v); return isNaN(n) ? v : n; }   // a non-number goes to the server as-is: it is the server's job to reject it
    return v;
  };

  VendorGrid.prototype._persistUpdate = function (key, changes, operation) {
    var me = this;
    this._setBusy(true);
    this._status.textContent = "saving " + key + "…";
    var p;
    try { p = this._send("update", { rowKey: key, changes: changes }); }
    catch (ex) { this._fail(operation, 0, ex.message); return Promise.resolve(); }
    return p.then(function (data) {
      if (me._destroyed) return;
      if (!data || !data.row) throw new Error("VendorGrid: the update response must be { row }.");
      var idx = -1;
      for (var i = 0; i < me.rows.length; i++) if (me.rows[i][me.idField] === key) { idx = i; break; }
      if (idx >= 0) me.rows[idx] = data.row;
      me._setBusy(false);
      me._status.textContent = key + " saved";
      me._renderRows();
    }).catch(function (err) {
      if (me._destroyed) return;
      me._fail(operation, err.status, err.message);
      me._renderRows();   // revert to the last server state
    });
  };

  /** Programmatic update (no rowupdate event: the caller already knows). */
  VendorGrid.prototype.updateRow = function (key, changes) {
    this._assertAlive("updateRow");
    if (!key) throw new Error("VendorGrid.updateRow: a row key is required.");
    if (!changes || typeof changes !== "object") throw new Error("VendorGrid.updateRow: changes must be an object.");
    return this._persistUpdate(String(key), changes, "update");
  };

  // ---- insert ------------------------------------------------------------------------
  VendorGrid.prototype._beginAdd = function () {
    if (!this.opts.editable || this._adding || this._busy) return;
    this._adding = true;
    this._renderRows();
    var first = this._tbody.querySelector(".vgrid-new .vgrid-editor");
    if (first) first.focus();
  };
  VendorGrid.prototype._buildAddRow = function () {
    var me = this, tr = h("tr", "vgrid-new"), editors = {};
    this.opts.columns.forEach(function (c) {
      var td = h("td", "vgrid-td");
      if (c.editable === false) { td.textContent = "(server)"; td.classList.add("vgrid-muted"); }
      else {
        var def = c.type === "select" && c.values ? c.values[0] : c.type === "number" ? 0 : "";
        var ed = me._createEditor(c, def);
        ed.addEventListener("keydown", function (ev) { if (ev.key === "Enter") { ev.preventDefault(); save(); } else if (ev.key === "Escape") { ev.preventDefault(); cancel(); } });
        editors[c.field] = { column: c, editor: ed };
        td.appendChild(ed);
      }
      tr.appendChild(td);
    });
    var actions = h("td", "vgrid-td vgrid-actions");
    var ok = h("button", "vgrid-btn vgrid-save", "Save"); ok.type = "button";
    var no = h("button", "vgrid-btn", "Cancel"); no.type = "button";
    actions.appendChild(ok); actions.appendChild(no);
    tr.appendChild(actions);
    function values() { var v = {}; for (var f in editors) v[f] = me._readEditor(editors[f].column, editors[f].editor); return v; }
    function save() { var v = values(); me._adding = false; me._emit("rowinsert", { values: clone(v) }); me._persistInsert(v); }
    function cancel() { me._adding = false; me._renderRows(); }
    ok.addEventListener("click", save);
    no.addEventListener("click", cancel);
    return tr;
  };
  VendorGrid.prototype._persistInsert = function (values) {
    var me = this;
    this._setBusy(true);
    this._status.textContent = "inserting…";
    var p;
    try { p = this._send("create", { values: values }); }
    catch (ex) { this._fail("create", 0, ex.message); return Promise.resolve(); }
    return p.then(function (data) {
      if (me._destroyed) return;
      if (!data || !data.row) throw new Error("VendorGrid: the create response must be { row }.");
      me.selectedKey = data.row[me.idField];
      me._status.textContent = "inserted " + me.selectedKey;
      return me.read();   // the server owns ordering and totals: re-read the page
    }).catch(function (err) {
      if (me._destroyed) return;
      me._fail("create", err.status, err.message);
      me._renderRows();
    });
  };
  /** Programmatic insert (no rowinsert event: the caller already knows). */
  VendorGrid.prototype.insertRow = function (values) {
    this._assertAlive("insertRow");
    if (!values || typeof values !== "object") throw new Error("VendorGrid.insertRow: values must be an object.");
    return this._persistInsert(values);
  };

  // ---- delete ------------------------------------------------------------------------
  VendorGrid.prototype._deleteRow = function (row) {
    if (this._busy) return;
    this._emit("rowdelete", { row: clone(row) });
    return this._persistDelete(row[this.idField]);
  };
  VendorGrid.prototype._persistDelete = function (key) {
    var me = this;
    this._setBusy(true);
    this._status.textContent = "deleting " + key + "…";
    var p;
    try { p = this._send("destroy", { rowKey: key }); }
    catch (ex) { this._fail("destroy", 0, ex.message); return Promise.resolve(); }
    return p.then(function () {
      if (me._destroyed) return;
      if (me.selectedKey === key) me.selectedKey = null;
      if (me.rows.length === 1 && me.skip > 0) me.skip -= me.opts.pageSize;   // last row of the last page
      me._status.textContent = "deleted " + key;
      return me.read();
    }).catch(function (err) {
      if (me._destroyed) return;
      me._fail("destroy", err.status, err.message);
    });
  };
  VendorGrid.prototype.deleteSelected = function () {
    this._assertAlive("deleteSelected");
    if (!this.selectedKey) throw new Error("VendorGrid.deleteSelected: no row is selected (click a cell first).");
    return this._persistDelete(this.selectedKey);
  };
  VendorGrid.prototype.getSelectedKey = function () { return this.selectedKey; };

  // ---- lifecycle ----------------------------------------------------------------------
  VendorGrid.prototype.resize = function () { /* the table is fluid; nothing to do, kept for API parity */ };
  VendorGrid.prototype.destroy = function () {
    if (this._destroyed) return;
    this._destroyed = true;
    this._handlers = {};
    this.el.innerHTML = "";
    this._root = this._body = this._table = this._thead = this._tbody = null;
  };
  VendorGrid.prototype._assertAlive = function (method) {
    if (this._destroyed) throw new Error("VendorGrid." + method + ": the grid has been destroyed.");
  };

  global.VendorGrid = VendorGrid;
})(window);
