/*!
 * VendorChart 1.0 — a tiny stand-in for a third-party line-chart library.
 * It knows nothing about Wisej.NET: it takes a host element and an options
 * object, draws an SVG line chart into that element, and emits MANY of its own
 * lower-case events — exactly the "noisy vendor" the course talks about.
 *
 * API:  new VendorChart(element, { series: [{ label, values: [..] }], labels: [..], theme: "light"|"dark" })
 *       .setData({ series, labels })   .setOptions({...})   .resize()   .destroy()
 *       .on(name, fn) / .off(name, fn) / .off()
 *
 * Events (vendor naming, lower-case):
 *   "hover"       { seriesIndex, index, label, value }            every pointer move over the plot
 *   "zoom"        { scale }                                       mouse wheel over the plot
 *   "render"      { reason, points }                              after every redraw
 *   "layout"      { width, height }                               after resize()
 *   "legendclick" { seriesIndex, label, visible }                 legend entry toggled
 *   "pointclick"  { seriesIndex, index, label, value, x, y, domEvent }   a point circle was clicked
 *
 * Quirk on purpose: "theme" can only be set at construction. setOptions({theme})
 * throws — the integration has to destroy() and create a new instance (and
 * re-attach its handlers!), the "library that recreates its instance" case.
 */
(function (global) {
  "use strict";

  var SVG = "http://www.w3.org/2000/svg";
  var COLORS = ["#1a86ff", "#1f9d6b", "#e8a13c", "#7d5ae0"];
  var DEFAULT_W = 600, DEFAULT_H = 240;             // used until the host has a measurable size
  var PAD = { left: 44, right: 18, top: 34, bottom: 30 };

  function el(name, attrs, parent) {
    var n = document.createElementNS(SVG, name);
    for (var k in attrs) n.setAttribute(k, attrs[k]);
    if (parent) parent.appendChild(n);
    return n;
  }
  function clamp(v, a, b) { return Math.max(a, Math.min(b, v)); }
  function isNum(v) { return typeof v === "number" && isFinite(v); }

  function VendorChart(element, options) {
    if (!element || !element.appendChild)
      throw new Error("VendorChart: a host element is required.");

    options = options || {};
    this.el = element;
    this.theme = options.theme === "dark" ? "dark" : "light";
    this._handlers = {};
    this._zoom = 1;
    this._hidden = {};
    this._hoverIndex = -1;
    this._destroyed = false;
    this.series = [];
    this.labels = [];
    this._build();
    this.setData({ series: options.series || [], labels: options.labels || [] }, "init");
  }

  // ---- events -------------------------------------------------------------
  VendorChart.prototype.on = function (name, fn) {
    (this._handlers[name] = this._handlers[name] || []).push(fn);
    return this;
  };
  VendorChart.prototype.off = function (name, fn) {
    if (!name) { this._handlers = {}; return this; }
    var list = this._handlers[name] || [];
    this._handlers[name] = fn ? list.filter(function (h) { return h !== fn; }) : [];
    return this;
  };
  VendorChart.prototype._emit = function (name, data) {
    var list = this._handlers[name] || [];
    for (var i = 0; i < list.length; i++) list[i](data);
  };

  // ---- public API ---------------------------------------------------------
  VendorChart.prototype.setData = function (data, reason) {
    this._assertAlive("setData");
    data = data || {};
    var series = data.series, labels = data.labels;
    if (!Array.isArray(series)) throw new Error("VendorChart.setData: series must be an array of { label, values }.");
    if (!Array.isArray(labels)) throw new Error("VendorChart.setData: labels must be an array of strings.");
    for (var s = 0; s < series.length; s++) {
      var values = series[s] && series[s].values;
      if (!Array.isArray(values)) throw new Error("VendorChart.setData: series[" + s + "].values must be an array.");
      if (values.length !== labels.length)
        throw new Error("VendorChart.setData: series[" + s + "] has " + values.length + " values but there are " + labels.length + " labels.");
      for (var i = 0; i < values.length; i++)
        if (!isNum(values[i])) throw new Error("VendorChart.setData: series[" + s + "].values[" + i + "] is not a finite number (" + JSON.stringify(values[i]) + ").");
    }
    this.series = series.map(function (x) { return { label: String(x.label || ""), values: x.values.slice() }; });
    this.labels = labels.map(String);
    this._hoverIndex = -1;
    this._render(reason || "data");
  };

  VendorChart.prototype.setOptions = function (partial) {
    this._assertAlive("setOptions");
    partial = partial || {};
    if (partial.theme !== undefined && partial.theme !== this.theme)
      throw new Error("VendorChart.setOptions: theme cannot be changed after construction — destroy() the instance and create a new one.");
    if (partial.series !== undefined || partial.labels !== undefined)
      this.setData({ series: partial.series || this.series, labels: partial.labels || this.labels }, "options");
  };

  VendorChart.prototype.resize = function () {
    this._assertAlive("resize");
    var r = this.root.getBoundingClientRect();
    this._render("resize");
    this._emit("layout", { width: Math.round(r.width), height: Math.round(r.height) });
  };

  VendorChart.prototype.destroy = function () {
    if (this._destroyed) return;
    this._destroyed = true;
    this.svg.removeEventListener("mousemove", this._onMove);
    this.svg.removeEventListener("mouseleave", this._onLeave);
    this.svg.removeEventListener("wheel", this._onWheel);
    this.off();
    if (this.root && this.root.parentNode) this.root.parentNode.removeChild(this.root);
    this.root = null; this.svg = null; this.el = null;
  };

  // ---- internals ----------------------------------------------------------
  VendorChart.prototype._assertAlive = function (op) {
    if (this._destroyed) throw new Error("VendorChart." + op + ": instance has been destroyed.");
  };

  VendorChart.prototype._build = function () {
    var me = this;
    var root = document.createElement("div");
    root.className = "vendor-chart vendor-chart--" + this.theme;
    var svg = el("svg", { "class": "vc-svg" });
    root.appendChild(svg);
    this.el.appendChild(root);
    this.root = root;
    this.svg = svg;

    var clip = el("clipPath", { id: "vc-clip-" + Math.random().toString(36).slice(2, 8) }, el("defs", {}, svg));
    this.clipRect = el("rect", {}, clip);
    this._clipId = clip.getAttribute("id");

    this.gGrid = el("g", { "class": "vc-grid" }, svg);
    this.gLines = el("g", { "class": "vc-lines", "clip-path": "url(#" + this._clipId + ")" }, svg);
    this.gPoints = el("g", { "class": "vc-points", "clip-path": "url(#" + this._clipId + ")" }, svg);
    this.gAxis = el("g", { "class": "vc-axis" }, svg);
    this.gLegend = el("g", { "class": "vc-legend" }, svg);
    this.hoverLine = el("line", { "class": "vc-hoverline", visibility: "hidden" }, svg);
    this._measure();

    // noisy DOM interactions → vendor events
    this._onMove = function (e) {
      var idx = me._indexAt(e);
      if (idx < 0) return;
      me._hoverIndex = idx;
      me._drawHover();
      for (var s = 0; s < me.series.length; s++) {
        if (me._hidden[s]) continue;
        me._emit("hover", { seriesIndex: s, index: idx, label: me.labels[idx], value: me.series[s].values[idx] });
      }
    };
    this._onLeave = function () { me._hoverIndex = -1; me._drawHover(); };
    this._onWheel = function (e) {
      e.preventDefault();
      me._zoom = clamp(me._zoom * (e.deltaY < 0 ? 1.15 : 1 / 1.15), 0.5, 4);
      me._render("zoom");
      me._emit("zoom", { scale: Math.round(me._zoom * 100) / 100 });
    };
    svg.addEventListener("mousemove", this._onMove);
    svg.addEventListener("mouseleave", this._onLeave);
    svg.addEventListener("wheel", this._onWheel, { passive: false });
  };

  // The viewBox follows the host's pixel size (1 unit = 1 px) so text is never stretched.
  VendorChart.prototype._measure = function () {
    var r = this.root.getBoundingClientRect();
    this._w = r.width > 40 ? Math.round(r.width) : DEFAULT_W;
    this._h = r.height > 40 ? Math.round(r.height) : DEFAULT_H;
    this.svg.setAttribute("viewBox", "0 0 " + this._w + " " + this._h);
    this.clipRect.setAttribute("x", PAD.left - 9); this.clipRect.setAttribute("y", PAD.top - 8);   // 9px slack so edge circles are not cut
    this.clipRect.setAttribute("width", Math.max(this._w - PAD.left - PAD.right + 18, 1));
    this.clipRect.setAttribute("height", Math.max(this._h - PAD.top - PAD.bottom + 8, 1));
    this.hoverLine.setAttribute("y1", PAD.top - 8); this.hoverLine.setAttribute("y2", this._h - PAD.bottom);
  };
  VendorChart.prototype._x = function (i) {
    var n = Math.max(this.labels.length - 1, 1);
    return PAD.left + i * ((this._w - PAD.left - PAD.right) / n);
  };
  VendorChart.prototype._y = function (v) {
    var h = this._h - PAD.top - PAD.bottom;
    return this._h - PAD.bottom - (v / this._yMax) * h;
  };
  VendorChart.prototype._indexAt = function (e) {
    if (!this.labels.length) return -1;
    var pt = this.svg.createSVGPoint();
    pt.x = e.clientX; pt.y = e.clientY;
    var ctm = this.svg.getScreenCTM();
    if (!ctm) return -1;
    var p = pt.matrixTransform(ctm.inverse());
    if (p.x < PAD.left - 10 || p.x > this._w - PAD.right + 10) return -1;
    var best = 0, bestD = Infinity;
    for (var i = 0; i < this.labels.length; i++) {
      var d = Math.abs(this._x(i) - p.x);
      if (d < bestD) { bestD = d; best = i; }
    }
    return best;
  };

  VendorChart.prototype._render = function (reason) {
    var me = this;
    this._measure();
    var max = 1;
    for (var s = 0; s < this.series.length; s++) {
      if (this._hidden[s]) continue;
      for (var i = 0; i < this.series[s].values.length; i++) max = Math.max(max, this.series[s].values[i]);
    }
    this._yMax = (max * 1.15) / this._zoom;

    // grid + axis
    this.gGrid.innerHTML = ""; this.gAxis.innerHTML = ""; this.gLines.innerHTML = ""; this.gPoints.innerHTML = ""; this.gLegend.innerHTML = "";
    for (var g = 0; g <= 4; g++) {
      var v = (this._yMax * g) / 4, y = this._y(v);
      el("line", { x1: PAD.left, x2: this._w - PAD.right, y1: y, y2: y, "class": "vc-gridline" }, this.gGrid);
      el("text", { x: PAD.left - 8, y: y + 4, "text-anchor": "end", "class": "vc-tick" }, this.gAxis).textContent = Math.round(v);
    }
    for (var l = 0; l < this.labels.length; l++)
      el("text", { x: this._x(l), y: this._h - 10, "text-anchor": "middle", "class": "vc-tick" }, this.gAxis).textContent = this.labels[l];

    // series
    var points = 0;
    this.series.forEach(function (ser, sIdx) {
      var color = COLORS[sIdx % COLORS.length];
      // legend (clickable)
      var lx = PAD.left + sIdx * 120, ly = 16;
      var legend = el("g", { "class": "vc-legend-item" + (me._hidden[sIdx] ? " vc-legend-item--off" : ""), transform: "translate(" + lx + "," + ly + ")" }, me.gLegend);
      el("rect", { x: 0, y: -6, width: 12, height: 12, rx: 3, fill: color }, legend);
      el("text", { x: 18, y: 4, "class": "vc-legend-text" }, legend).textContent = ser.label;
      legend.addEventListener("click", function () {
        me._hidden[sIdx] = !me._hidden[sIdx];
        me._render("legend");
        me._emit("legendclick", { seriesIndex: sIdx, label: ser.label, visible: !me._hidden[sIdx] });
      });
      if (me._hidden[sIdx]) return;

      var d = ser.values.map(function (v, i) { return (i ? "L " : "M ") + me._x(i).toFixed(1) + " " + me._y(v).toFixed(1); }).join(" ");
      el("path", { d: d, fill: "none", stroke: color, "stroke-width": 2.5, "stroke-linecap": "round", "stroke-linejoin": "round", "class": "vc-line" }, me.gLines);
      ser.values.forEach(function (v, i) {
        var c = el("circle", { cx: me._x(i), cy: me._y(v), r: 5, fill: me.theme === "dark" ? "#16314e" : "#fff", stroke: color, "stroke-width": 2.2, "class": "vc-point" }, me.gPoints);
        points++;
        c.addEventListener("click", function (domEvent) {
          me._emit("pointclick", { seriesIndex: sIdx, index: i, label: me.labels[i], value: v, x: me._x(i), y: me._y(v), domEvent: domEvent });
        });
      });
    });
    this._drawHover();
    this._emit("render", { reason: reason, points: points });
  };

  VendorChart.prototype._drawHover = function () {
    var i = this._hoverIndex;
    if (i < 0) { this.hoverLine.setAttribute("visibility", "hidden"); return; }
    var x = this._x(i);
    this.hoverLine.setAttribute("x1", x); this.hoverLine.setAttribute("x2", x);
    this.hoverLine.setAttribute("visibility", "visible");
  };

  global.VendorChart = VendorChart;
})(window);
