/*!
 * VendorGauge 1.0 — a tiny stand-in for a third-party gauge library.
 * It knows nothing about Wisej.NET: it takes a host element and an options
 * object, draws into that element, and emits its own lower-case events.
 *
 * API:  new VendorGauge(element, options)
 *       .setValue(number)        .setOptions({...})     .getValue()
 *       .on(name, fn) / .off()   .resize()              .destroy()
 * Events (vendor naming, lower-case): "rangechange", "thresholdexceeded"
 */
(function (global) {
  "use strict";

  var DEFAULTS = { value: 0, min: 40, max: 120, warnAt: 85, threshold: 100, units: "°F", label: "" };
  var SVG = "http://www.w3.org/2000/svg";

  function clamp(v, a, b) { return Math.max(a, Math.min(b, v)); }
  function polar(cx, cy, r, deg) { var a = deg * Math.PI / 180; return [cx + r * Math.cos(a), cy - r * Math.sin(a)]; }
  function arc(cx, cy, r, p0, p1) {
    var a0 = 180 - p0 * 180, a1 = 180 - p1 * 180;
    var s = polar(cx, cy, r, a0), e = polar(cx, cy, r, a1);
    return "M " + s[0].toFixed(1) + " " + s[1].toFixed(1) + " A " + r + " " + r + " 0 0 1 " + e[0].toFixed(1) + " " + e[1].toFixed(1);
  }
  function el(name, attrs, parent) {
    var n = document.createElementNS(SVG, name);
    for (var k in attrs) n.setAttribute(k, attrs[k]);
    if (parent) parent.appendChild(n);
    return n;
  }

  function VendorGauge(element, options) {
    if (!element || !element.appendChild)
      throw new Error("VendorGauge: a host element is required.");

    this.el = element;
    this.opts = {};
    for (var k in DEFAULTS) this.opts[k] = DEFAULTS[k];
    this._handlers = {};
    this._range = null;
    this._destroyed = false;
    this._build();
    this.setOptions(options || {}, true);
  }

  VendorGauge.prototype.on = function (name, fn) {
    (this._handlers[name] = this._handlers[name] || []).push(fn);
    return this;
  };
  VendorGauge.prototype.off = function (name, fn) {
    if (!name) { this._handlers = {}; return this; }
    var list = this._handlers[name] || [];
    this._handlers[name] = fn ? list.filter(function (h) { return h !== fn; }) : [];
    return this;
  };
  VendorGauge.prototype._emit = function (name, data) {
    var list = this._handlers[name] || [];
    for (var i = 0; i < list.length; i++) list[i](data);
  };

  VendorGauge.prototype.getValue = function () { return this.value; };

  VendorGauge.prototype.setOptions = function (partial, silent) {
    this._assertAlive("setOptions");
    var hasValue = false;
    for (var k in partial) {
      if (k === "value") { hasValue = true; continue; }
      if (partial[k] !== undefined) this.opts[k] = partial[k];
    }
    if (!(this.opts.min < this.opts.max))
      throw new Error("VendorGauge.setOptions: min (" + this.opts.min + ") must be less than max (" + this.opts.max + ").");
    this._layout();
    this.setValue(hasValue ? partial.value : (this.value === undefined ? this.opts.min : this.value), silent);
  };

  VendorGauge.prototype.setValue = function (v, silent) {
    this._assertAlive("setValue");
    if (typeof v !== "number" || !isFinite(v))
      throw new Error("VendorGauge.setValue: value must be a finite number, received " + JSON.stringify(v) + ".");

    var prev = this.value;
    this.value = v;
    this._draw();

    var range = this._rangeOf(v);
    if (range !== this._range) {
      var old = this._range;
      this._range = range;
      if (!silent) this._emit("rangechange", { range: range, previous: old, value: v });
    }
    // Rising edge only: one event per crossing, not one per frame.
    if (!silent && v >= this.opts.threshold && (prev === undefined || prev < this.opts.threshold))
      this._emit("thresholdexceeded", { value: v, threshold: this.opts.threshold });
  };

  VendorGauge.prototype.resize = function () {
    this._assertAlive("resize");
    // The SVG scales with its viewBox; a real library would re-measure here.
    this._draw();
  };

  VendorGauge.prototype.destroy = function () {
    if (this._destroyed) return;
    this._destroyed = true;
    this.off();
    if (this.svg && this.svg.parentNode) this.svg.parentNode.removeChild(this.svg);
    this.svg = null;
    this.el = null;
  };

  // --- internals -----------------------------------------------------------
  VendorGauge.prototype._assertAlive = function (op) {
    if (this._destroyed) throw new Error("VendorGauge." + op + ": instance has been destroyed.");
  };
  VendorGauge.prototype._rangeOf = function (v) {
    return v >= this.opts.threshold ? "high" : v >= this.opts.warnAt ? "warm" : "normal";
  };
  VendorGauge.prototype._color = function (range) {
    return range === "high" ? "#e0563b" : range === "warm" ? "#e8a13c" : "#1f9d57";
  };

  VendorGauge.prototype._build = function () {
    var svg = el("svg", { viewBox: "0 0 200 150", preserveAspectRatio: "xMidYMid meet" });
    svg.style.cssText = "display:block;width:100%;height:100%;font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;";
    var cx = 100, cy = 104, r = 82;
    this.cx = cx; this.cy = cy; this.r = r;

    this.bandNormal = el("path", { fill: "none", stroke: "#e3f3ea", "stroke-width": 7, "stroke-linecap": "round" }, svg);
    this.bandWarm = el("path", { fill: "none", stroke: "#fbeccf", "stroke-width": 7 }, svg);
    this.bandHigh = el("path", { fill: "none", stroke: "#f7d8cf", "stroke-width": 7, "stroke-linecap": "round" }, svg);
    el("path", { d: arc(cx, cy, r, 0, 1), fill: "none", stroke: "#e6ebf1", "stroke-width": 13, "stroke-linecap": "round" }, svg);
    this.valueArc = el("path", { fill: "none", stroke: "#1f9d57", "stroke-width": 13, "stroke-linecap": "round" }, svg);
    for (var i = 0; i <= 4; i++) {
      var tp = i / 4, a = polar(cx, cy, r - 9, 180 - tp * 180), b = polar(cx, cy, r - 2, 180 - tp * 180);
      el("line", { x1: a[0], y1: a[1], x2: b[0], y2: b[1], stroke: "#fff", "stroke-width": 1.6 }, svg);
    }
    this.needle = el("line", { x1: cx, y1: cy, stroke: "#16314e", "stroke-width": 3.2, "stroke-linecap": "round" }, svg);
    el("circle", { cx: cx, cy: cy, r: 6.5, fill: "#16314e" }, svg);
    el("circle", { cx: cx, cy: cy, r: 2.6, fill: "#fff" }, svg);
    this.readout = el("text", { x: cx, y: 134, "text-anchor": "middle", "font-size": 27, "font-weight": 800, fill: "#0d1b2a" }, svg);
    this.zoneText = el("text", { x: cx, y: 148, "text-anchor": "middle", "font-size": 9.5, "font-weight": 800, "letter-spacing": 1.2 }, svg);
    this.labelText = el("text", { x: 8, y: 14, "font-size": 9, "font-weight": 700, fill: "#8a98a8" }, svg);

    this.el.appendChild(svg);
    this.svg = svg;
  };

  VendorGauge.prototype._layout = function () {
    var o = this.opts, span = o.max - o.min, cx = this.cx, cy = this.cy, r = this.r;
    var pw = clamp((o.warnAt - o.min) / span, 0, 1), pt = clamp((o.threshold - o.min) / span, 0, 1);
    this.bandNormal.setAttribute("d", arc(cx, cy, r + 10, 0, pw));
    this.bandWarm.setAttribute("d", arc(cx, cy, r + 10, pw, pt));
    this.bandHigh.setAttribute("d", arc(cx, cy, r + 10, pt, 1));
    this.labelText.textContent = o.label || "";
  };

  VendorGauge.prototype._draw = function () {
    var o = this.opts, v = this.value, p = clamp((v - o.min) / (o.max - o.min), 0, 1);
    var range = this._rangeOf(v), color = this._color(range);
    var tip = polar(this.cx, this.cy, this.r - 16, 180 - p * 180);
    this.valueArc.setAttribute("d", p > 0 ? arc(this.cx, this.cy, this.r, 0, p) : "");
    this.valueArc.setAttribute("stroke", color);
    this.needle.setAttribute("x2", tip[0]); this.needle.setAttribute("y2", tip[1]);
    this.readout.textContent = Math.round(v) + o.units;
    this.zoneText.textContent = range.toUpperCase();
    this.zoneText.setAttribute("fill", color);
  };

  global.VendorGauge = VendorGauge;
})(window);
