/*!
 * VendorHeatmap 1.2.0 — a stand-in for an unfamiliar third-party calendar-heatmap library.
 * It knows nothing about Wisej.NET: it takes a host element and an options object, draws a
 * day × hour grid into that element, fetches its own data from a URL, and emits its own
 * lower-case events.
 *
 * API:   new VendorHeatmap(hostElement, { days, hours, thresholds: { warn, high }, palette, dataUrl, title, max })
 *        .load()                 fetches dataUrl, expects JSON { cells: [ { day, hour, value } ] }, returns a Promise
 *        .setData(cells)         replaces the data (validates every cell, throws on malformed input)
 *        .setOptions({...})      re-syncs options in place (days/hours changes rebuild the grid)
 *        .getData()              copy of the current cells
 *        .getCellCount()         number of cells holding data
 *        .highlight(day, hour)   pulses one cell        .clearHighlight()
 *        .resize()               re-measures the host   .destroy()   frees everything
 *        .on(name, fn) / .off(name, fn)
 * Events (vendor naming, lower-case):
 *        "cellselect"  { day, hour, value }       user clicked a cell
 *        "loaded"      { count }                  load() finished
 *        "error"       { status, message }        load() or a callback failed
 *        "cellhover"   { day, hour, value }       every pointer move over a cell (noisy!)
 * Static: VendorHeatmap.version, VendorHeatmap.liveInstances()
 */
(function (global) {
  "use strict";

  var VERSION = "1.2.0";
  var SVG = "http://www.w3.org/2000/svg";
  var DEFAULTS = {
    days: 7,
    hours: 24,
    max: 100,
    thresholds: { warn: 60, high: 85 },
    palette: ["#dce9f8", "#1a86ff", "#e8a13c", "#e0563b"],   // [cold, busy, warn, high]
    dataUrl: null,
    title: "",
    dayLabels: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]
  };
  var liveInstances = 0;

  function el(name, attrs, parent) {
    var n = document.createElementNS(SVG, name);
    for (var k in attrs) if (attrs[k] !== undefined) n.setAttribute(k, attrs[k]);
    if (parent) parent.appendChild(n);
    return n;
  }
  function isInt(v) { return typeof v === "number" && isFinite(v) && Math.floor(v) === v; }
  function describe(v) {
    if (v === null) return "null";
    if (Array.isArray(v)) return "an array";
    if (typeof v === "object") return "an object with keys [" + Object.keys(v).join(", ") + "]";
    return typeof v;
  }
  function withStatus(message, status) { var e = new Error(message); e.status = status; return e; }
  function mix(a, b, t) {
    var pa = parseInt(a.slice(1), 16), pb = parseInt(b.slice(1), 16);
    var r = Math.round(((pa >> 16) & 255) * (1 - t) + ((pb >> 16) & 255) * t);
    var g = Math.round(((pa >> 8) & 255) * (1 - t) + ((pb >> 8) & 255) * t);
    var bl = Math.round((pa & 255) * (1 - t) + (pb & 255) * t);
    return "rgb(" + r + "," + g + "," + bl + ")";
  }

  function VendorHeatmap(host, options) {
    if (!host || !host.appendChild)
      throw new Error("VendorHeatmap: a host element is required.");

    this.el = host;
    this.opts = {};
    for (var k in DEFAULTS) this.opts[k] = DEFAULTS[k];
    this.opts.thresholds = { warn: DEFAULTS.thresholds.warn, high: DEFAULTS.thresholds.high };
    this._handlers = {};
    this._cells = {};
    this._rects = {};
    this._highlighted = null;
    this._selected = null;
    this._loadSeq = 0;
    this._destroyed = false;

    host.classList.add("vendor-heatmap");
    this._build();
    this.setOptions(options || {});
    liveInstances++;
  }

  VendorHeatmap.version = VERSION;
  VendorHeatmap.liveInstances = function () { return liveInstances; };

  // --- events ----------------------------------------------------------------
  VendorHeatmap.prototype.on = function (name, fn) {
    (this._handlers[name] = this._handlers[name] || []).push(fn);
    return this;
  };
  VendorHeatmap.prototype.off = function (name, fn) {
    if (!name) { this._handlers = {}; return this; }
    var list = this._handlers[name] || [];
    this._handlers[name] = fn ? list.filter(function (h) { return h !== fn; }) : [];
    return this;
  };
  VendorHeatmap.prototype._emit = function (name, data) {
    var list = (this._handlers[name] || []).slice();
    for (var i = 0; i < list.length; i++) list[i](data);
  };

  // --- options ---------------------------------------------------------------
  VendorHeatmap.prototype.setOptions = function (partial) {
    this._assertAlive("setOptions");
    partial = partial || {};
    var rebuild = false;
    for (var k in partial) {
      var v = partial[k];
      if (v === undefined) continue;
      if (k === "thresholds") {
        if (!v || typeof v !== "object") throw new Error("VendorHeatmap.setOptions: thresholds must be an object { warn, high }.");
        if (v.warn !== undefined) this.opts.thresholds.warn = v.warn;
        if (v.high !== undefined) this.opts.thresholds.high = v.high;
        continue;
      }
      if (k === "days" || k === "hours") {
        if (!isInt(v) || v < 1 || v > (k === "days" ? 31 : 24))
          throw new Error("VendorHeatmap.setOptions: " + k + " must be an integer between 1 and " + (k === "days" ? 31 : 24) + ", received " + JSON.stringify(v) + ".");
        if (this.opts[k] !== v) rebuild = true;
      }
      if (k === "palette" && (!Array.isArray(v) || v.length < 4))
        throw new Error("VendorHeatmap.setOptions: palette must be an array of four colors [cold, busy, warn, high].");
      this.opts[k] = v;
    }
    if (!(this.opts.thresholds.warn < this.opts.thresholds.high))
      throw new Error("VendorHeatmap.setOptions: thresholds.warn (" + this.opts.thresholds.warn + ") must be less than thresholds.high (" + this.opts.thresholds.high + ").");

    if (rebuild) this._buildGrid();
    this._layout();
    this._paint();
    return this;
  };

  // --- data ------------------------------------------------------------------
  VendorHeatmap.prototype.load = function () {
    this._assertAlive("load");
    var url = this.opts.dataUrl;
    if (!url || typeof url !== "string")
      throw new Error("VendorHeatmap.load: options.dataUrl is required before calling load().");

    var me = this, seq = ++this._loadSeq;
    return fetch(url, { credentials: "same-origin", cache: "no-store" })
      .then(function (res) {
        return res.text().then(function (text) {
          return { status: res.status, ok: res.ok, text: text, type: res.headers.get("content-type") || "" };
        });
      })
      .then(function (r) {
        if (me._destroyed || seq !== me._loadSeq) return { cancelled: true };
        if (!r.ok)
          throw withStatus("VendorHeatmap.load: " + url + " answered HTTP " + r.status + (r.text ? " (" + r.text.slice(0, 120) + ")" : "") + ".", r.status);
        var json;
        try { json = JSON.parse(r.text); }
        catch (ex) {
          throw withStatus("VendorHeatmap.load: the response is not valid JSON (" + ex.message + "). Content-Type was \"" + r.type + "\", body started with \"" + r.text.slice(0, 40) + "\".", r.status);
        }
        if (!json || typeof json !== "object" || !Array.isArray(json.cells))
          throw withStatus("VendorHeatmap.load: expected {\"cells\":[...]} but received " + describe(json) + ".", r.status);
        me.setData(json.cells);
        me._emit("loaded", { count: json.cells.length });
        return { count: json.cells.length };
      })
      .catch(function (ex) {
        if (me._destroyed) return { cancelled: true };
        me._emit("error", { status: ex.status || 0, message: ex.message });
        throw ex;
      });
  };

  VendorHeatmap.prototype.setData = function (cells) {
    this._assertAlive("setData");
    if (!Array.isArray(cells))
      throw new Error("VendorHeatmap.setData: cells must be an array, received " + describe(cells) + ".");
    var days = this.opts.days, hours = this.opts.hours, next = {};
    for (var i = 0; i < cells.length; i++) {
      var c = cells[i], reason = null;
      if (!c || typeof c !== "object") reason = "not an object";
      else if (!isInt(c.day) || c.day < 0 || c.day >= days) reason = "day must be an integer in 0.." + (days - 1);
      else if (!isInt(c.hour) || c.hour < 0 || c.hour >= hours) reason = "hour must be an integer in 0.." + (hours - 1);
      else if (typeof c.value !== "number" || !isFinite(c.value)) reason = "value must be a finite number";
      if (reason)
        throw new Error("VendorHeatmap.setData: cell #" + i + " is invalid (" + JSON.stringify(c) + "): " + reason + ".");
      next[c.day + ":" + c.hour] = c.value;
    }
    this._cells = next;
    this._paint();
    return this;
  };

  VendorHeatmap.prototype.getData = function () {
    var out = [];
    for (var key in this._cells) {
      var p = key.split(":");
      out.push({ day: +p[0], hour: +p[1], value: this._cells[key] });
    }
    return out;
  };
  VendorHeatmap.prototype.getCellCount = function () { return Object.keys(this._cells).length; };

  // --- highlight -------------------------------------------------------------
  VendorHeatmap.prototype.highlight = function (day, hour) {
    this._assertAlive("highlight");
    if (!isInt(day) || day < 0 || day >= this.opts.days || !isInt(hour) || hour < 0 || hour >= this.opts.hours)
      throw new Error("VendorHeatmap.highlight: (" + day + ", " + hour + ") is outside the " + this.opts.days + " × " + this.opts.hours + " grid.");
    this.clearHighlight();
    var rect = this._rects[day + ":" + hour];
    if (rect) { rect.classList.add("vh-highlight"); this._highlighted = rect; }
    return this;
  };
  VendorHeatmap.prototype.clearHighlight = function () {
    if (this._highlighted) { this._highlighted.classList.remove("vh-highlight"); this._highlighted = null; }
    return this;
  };

  VendorHeatmap.prototype.resize = function () {
    this._assertAlive("resize");
    this._layout();
  };

  VendorHeatmap.prototype.destroy = function () {
    if (this._destroyed) return;
    this._destroyed = true;
    this._loadSeq++;                                   // any fetch still in flight is ignored
    this.off();
    if (this.svg) {
      this.svg.removeEventListener("click", this._onClick);
      this.svg.removeEventListener("mousemove", this._onMove);
      if (this.svg.parentNode) this.svg.parentNode.removeChild(this.svg);
    }
    if (this.el) this.el.classList.remove("vendor-heatmap");
    this.svg = null; this.el = null; this._rects = {}; this._cells = {}; this._highlighted = null; this._selected = null;
    liveInstances--;
  };

  // --- internals ---------------------------------------------------------------
  VendorHeatmap.prototype._assertAlive = function (op) {
    if (this._destroyed) throw new Error("VendorHeatmap." + op + ": instance has been destroyed.");
  };

  VendorHeatmap.prototype._build = function () {
    var me = this;
    var svg = el("svg", { preserveAspectRatio: "none" });
    this.titleText = el("text", { "class": "vh-title", x: 0, y: 11 }, svg);
    this.hourGroup = el("g", { "class": "vh-hours" }, svg);
    this.dayGroup = el("g", { "class": "vh-days" }, svg);
    this.cellGroup = el("g", { "class": "vh-cells" }, svg);

    this._onClick = function (e) {
      var t = e.target;
      if (!t || !t.getAttribute || !t.classList.contains("vh-cell")) return;
      var day = +t.getAttribute("data-day"), hour = +t.getAttribute("data-hour");
      if (me._selected) me._selected.classList.remove("vh-selected");
      me._selected = t; t.classList.add("vh-selected");
      me._emit("cellselect", { day: day, hour: hour, value: me._valueAt(day, hour) });
    };
    this._onMove = function (e) {
      var t = e.target;
      if (!t || !t.getAttribute || !t.classList.contains("vh-cell")) return;
      var day = +t.getAttribute("data-day"), hour = +t.getAttribute("data-hour");
      me._emit("cellhover", { day: day, hour: hour, value: me._valueAt(day, hour) });   // fires on every pointer move: filter it!
    };
    svg.addEventListener("click", this._onClick);
    svg.addEventListener("mousemove", this._onMove);

    this.el.appendChild(svg);
    this.svg = svg;
    this._buildGrid();
  };

  VendorHeatmap.prototype._buildGrid = function () {
    var days = this.opts.days, hours = this.opts.hours;
    this.cellGroup.innerHTML = ""; this.hourGroup.innerHTML = ""; this.dayGroup.innerHTML = "";
    this._rects = {}; this._highlighted = null; this._selected = null;
    for (var d = 0; d < days; d++) {
      el("text", { "class": "vh-label", "data-day": d, "text-anchor": "end" }, this.dayGroup);
      for (var h = 0; h < hours; h++) {
        var r = el("rect", { "class": "vh-cell vh-empty", "data-day": d, "data-hour": h, rx: 2, ry: 2 }, this.cellGroup);
        this._rects[d + ":" + h] = r;
      }
    }
    var step = hours > 12 ? 3 : 1;
    for (var hh = 0; hh < hours; hh += step)
      el("text", { "class": "vh-label", "data-hour": hh, "text-anchor": "middle" }, this.hourGroup);
    // cells that no longer exist are dropped silently; the rest are re-painted
    var kept = {};
    for (var key in this._cells) { var p = key.split(":"); if (+p[0] < days && +p[1] < hours) kept[key] = this._cells[key]; }
    this._cells = kept;
  };

  VendorHeatmap.prototype._layout = function () {
    if (!this.svg || !this.el) return;
    var W = Math.max(this.el.clientWidth || 0, 120), H = Math.max(this.el.clientHeight || 0, 60);
    var top = (this.opts.title ? 18 : 0) + 16, left = 34, gap = 2;
    var days = this.opts.days, hours = this.opts.hours;
    var cw = (W - left) / hours, ch = (H - top) / days;
    this.svg.setAttribute("viewBox", "0 0 " + W + " " + H);
    this.svg.setAttribute("width", W); this.svg.setAttribute("height", H);
    this.titleText.textContent = this.opts.title || "";

    var labels = this.dayGroup.childNodes;
    for (var i = 0; i < labels.length; i++) {
      var d = +labels[i].getAttribute("data-day");
      labels[i].setAttribute("x", left - 6);
      labels[i].setAttribute("y", top + d * ch + ch / 2 + 3.5);
      labels[i].textContent = this.opts.dayLabels[d % this.opts.dayLabels.length] || String(d);
    }
    var hl = this.hourGroup.childNodes;
    for (var j = 0; j < hl.length; j++) {
      var h = +hl[j].getAttribute("data-hour");
      hl[j].setAttribute("x", left + h * cw + cw / 2);
      hl[j].setAttribute("y", top - 5);
      hl[j].textContent = (h < 10 ? "0" : "") + h;
    }
    for (var key in this._rects) {
      var p = key.split(":"), r = this._rects[key];
      r.setAttribute("x", left + (+p[1]) * cw + gap / 2);
      r.setAttribute("y", top + (+p[0]) * ch + gap / 2);
      r.setAttribute("width", Math.max(cw - gap, 1));
      r.setAttribute("height", Math.max(ch - gap, 1));
    }
  };

  VendorHeatmap.prototype._valueAt = function (day, hour) {
    var v = this._cells[day + ":" + hour];
    return v === undefined ? null : v;
  };

  VendorHeatmap.prototype._colorOf = function (v) {
    var o = this.opts, t = o.thresholds, p = o.palette;
    if (v >= t.high) return p[3];
    if (v >= t.warn) return p[2];
    return mix(p[0], p[1], Math.max(0, Math.min(1, v / Math.max(t.warn, 1))));
  };

  VendorHeatmap.prototype._paint = function () {
    for (var key in this._rects) {
      var r = this._rects[key], v = this._cells[key];
      if (v === undefined) { r.classList.add("vh-empty"); r.removeAttribute("fill"); r.removeAttribute("data-value"); continue; }
      r.classList.remove("vh-empty");
      r.setAttribute("fill", this._colorOf(v));
      r.setAttribute("data-value", v);
    }
  };

  global.VendorHeatmap = VendorHeatmap;
})(window);
